<template>
  <div>
    <div v-if="formTitle">
      <h3 class="h4">{{ formTitle }}</h3>
      <v-divider class="mt-3"></v-divider>
    </div>
    <v-form v-model="formIsValid" ref="form" lazy-validation>
      <v-row>
          <v-col align="right">
             <slot
          name="form-actions-up"
          :validateFunc="validateForm"
          :resetFunc="resetForm"
          :formIsValid="formIsValid"
        ></slot>
        </v-col>
        </v-row>
      <div v-if="getFormTabs(formFields).length <= 1 && !calculatingDependecies">
        <v-row>
          <v-col
            v-for="(field, j) in translatedFields"
            :key="'COL-' + j"
            :md="calcFieldColumnSpan(field, translatedFields.length, j)"
            :cols="
              calcFieldColumnSpan(field, translatedFields.length, j) ? 12 : null
            "
          >
            <jrs-field
              :field="field"
              v-model="formData[field.name]"
              :dynamicFullName="dynamicFullName"
              @changeFullObject="updateSelectHolder"
              @update="manageDependecies($event, field)"
               :ignoreOfficeFilter="ignoreOfficeFilter"
            ></jrs-field>
          </v-col>
        </v-row>
        
      </div>
      <v-tabs v-if="getFormTabs(formFields).length > 1 && !calculatingDependecies" v-model="formTabsModel">
        <v-tab
          @click="
            emitInfoTab(
              tabInfo,
              getFormTabs(formFields).findIndex((object) => {
                return object.tabCode === tabInfo.tabCode;
              }),
              getFormTabs(formFields).length
            )
          "
          v-for="tabInfo in getFormTabs(translatedFields)"
          :key="'TAB-' + tabInfo.tabCode"
          :class="{
            'error--text': invalidTabCodes.includes(tabInfo.tabCode),
          }"
          >{{ tabInfo.tabName }}</v-tab
        >
      </v-tabs>

      <v-tabs-items
        v-if="getFormTabs(formFields).length > 1"
        v-model="formTabsModel"
      >
        <v-tab-item
          v-for="tabInfo in getFormTabs(translatedFields)"
          :key="'TAB-' + tabInfo.tabCode"
          eager
        >
          <v-row :key="'ROW-TEST'">
            <v-col
              v-for="(field, j) in tmpFieldForTab(
                tabInfo.tabCode,
                translatedFields
              )"
              :key="'COL-' + j"
              :md="
                calcFieldColumnSpan(
                  field,
                  tmpFieldForTab(tabInfo.tabCode, translatedFields).length,
                  j
                )
              "
              :cols="
                calcFieldColumnSpan(
                  field,
                  tmpFieldForTab(tabInfo.tabCode, translatedFields).length,
                  j
                )
                  ? 12
                  : null
              "
            >
              <jrs-field
                :field="field"
                v-model="formData[field.name]"
                :dynamicFullName="dynamicFullName"
                @changeFullObject="updateSelectHolder"
                @update="manageDependecies($event, field)"
                :ignoreOfficeFilter="ignoreOfficeFilter"
              ></jrs-field>
            </v-col>
          </v-row>
        </v-tab-item>
      </v-tabs-items>

      <!-- ACTIONS -->

      <v-divider class="mt-3"></v-divider>
      <div>
        <slot
          name="form-actions"
          :validateFunc="validateForm"
          :resetFunc="resetForm"
          :formIsValid="formIsValid"
        ></slot>
      </div>
    </v-form>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { GenericSqlSelectPayload } from "../axiosapi/api";
import { translate } from "../i18n";
import {
  JrsFormField,
  JrsFormTab,
  JrsFormFieldSelect,
  JrsFormFieldSearch,
  fieldType,
} from "../models/JrsForm";
import JrsField from "./JrsField.vue";
import FormMixin from "../mixins/FormMixin";
import JrsSnackbarVue from "./JrsSnackbar.vue";

export default Vue.extend({
  props: {
    /**
     * List of fields for the form. Items bust be of thipe "JrsFormField" or one of the extensions.
     */
    formFields: {
      type: Array as () => JrsFormField[],
      required: true,
    },
    /**
     * Object with te data of the form. If the form is for new item, "formData" must have the porperties but "empty".
     */
    formData: {
      type: Object,
      required: true,
    },
    /**
     * Title of the form.
     */
    formTitle: {
      type: String,
      required: false,
      default: undefined,
    },
    /**
     * Subtitle of the form.
     */
    formSubtitle: {
      type: String,
      required: false,
      default: undefined,
    },
    /**
     * Number of columns fo the form.
     * Excepted values: {1, 2, 3}
     */
    nbrOfColumns: {
      type: Number,
      required: false,
      default: 2,
      validator: function (value:any) {
        // The value must match one of these values
        return [1, 2, 3, 4].indexOf(value) !== -1;
      },
    },
    currentTabIndex: {
      type: Number,
      required: false,
      default: null,
    },
    dynamicFullName:{
       type: Object,
      required: false,
      default: null
    },
     /**
     * Ignore office filter in data query.
     */
    ignoreOfficeFilter: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  components: {
    "jrs-field": JrsField,
  },
  mixins: [FormMixin],
  data() {
    return {
      formIsValid: false,
      formTabsModel: null,
      defaultTabCode: "MAIN",
      updatedSearcTableConditions: {},
      updatedSelectFieldItems: {},
      fieldMessages: {},
      fieldReadonlyRules: {},
      invalidTabCodes: [],
      calculatingDependecies: true,
    };
  },
  watch: {
    formFields() {
      const localThis: any = this as any;

      const formFieldNames: string[] = localThis.formFields.map(
        (field: JrsFormField) => field.name
      );

      // Set field condition for all hidden fields
      const hiddenFieldNames: string[] = Object.keys(localThis.formData).filter(
        (fieldName: string) => !formFieldNames.includes(fieldName)
      );
      hiddenFieldNames.forEach((fieldName: string) => {
        const hiddenFieldValue: any = localThis.formData[fieldName];
        localThis.manageDependecies(hiddenFieldValue, null, fieldName);
      });
    },
    currentTabIndex(nextTabIndex) {
      const localThis: any = this as any;
      localThis.formTabsModel = nextTabIndex;
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),

    /**
     * Fields with translated labels and hints.
     */
    translatedFields() {
      const localThis: any = this as any;
      let transFields: Array<any> = (this.formFields as JrsFormField[])
        .map((oldField: any) => {
          let field: JrsFormField = Object.assign({}, oldField);
          // Translated labels and hints
          field.label =
            field.labelTranslationKey != undefined
              ? translate(field.labelTranslationKey).toString()
              : field.label;
          // Mark required fields
          if (field.is_required) {
            field.label = `* ${field.label}`;
          }
          field.hint =
            field.hintTranslationKey != undefined
              ? translate(field.hintTranslationKey).toString()
              : field.hint;
          field.messages = !field.messageTranslationKeys
            ? []
            : field.messageTranslationKeys.map((messageKey: string) =>
                translate(messageKey).toString()
              );

          // Setup select default value
          if (
            (field.type == fieldType["select"] ||
              field.type == fieldType["auto"]) &&
            (this as any).formData &&
            (this as any).formData[field.name]
          ) {
            (this as any).setupSelectDefaultValue(field);
          }

          // Override readonly setting
          if (Object.keys(localThis.fieldReadonlyRules).includes(field.name)) {
            field.readonly = localThis.fieldReadonlyRules[field.name];
          }

          // Setup field update messages
          field.messages = [
            ...field.messages,
            ...(localThis.fieldMessages[field.name]
              ? localThis.fieldMessages[field.name]
              : []),
          ];

          return field;
        })
        .sort((a: JrsFormField, b: JrsFormField) => {
          let order_A: number = a.form_order != null ? a.form_order : 1000;
          let order_B: number = b.form_order != null ? b.form_order : 1000;
          return order_A < order_B ? -1 : 1;
        });

      const selectFieldTypes: fieldType[] = [
        fieldType.select,
        fieldType.auto,
        fieldType.combobox,
        fieldType.combobox_multi,
      ];
      const searchTableFieldTypes: fieldType[] = [
        fieldType.search_table,
        fieldType.search_table_multi,
      ];

      // Update fields based on runtime conditions
      transFields.forEach((field: JrsFormField) => {
        // Update select items
        if (
          selectFieldTypes.includes(field.type) &&
          localThis.updatedSelectFieldItems[field.name]
        ) {
          (field as JrsFormFieldSelect).selectItems =
            localThis.updatedSelectFieldItems[field.name];
        }

        // Update searchtable conditions
        if (
          searchTableFieldTypes.includes(field.type) &&
          localThis.updatedSearcTableConditions[field.name]
        ) {
          const conditions: Array<any> = [
            localThis.updatedSearcTableConditions[field.name],
          ];
          if ((field as JrsFormFieldSearch).dataSourceCondition) {
            conditions.unshift(
              (field as JrsFormFieldSearch).dataSourceCondition
            );
          }
          (field as JrsFormFieldSearch).dataSourceCondition =
            conditions.join(" AND ");
        }
      });
      return transFields;
    },
    /**
     * Matrix of the fields divided in rows and columns.
     */
    fieldsMatrix() {
      let localThis: any = this as any;
      let breakpoint = localThis.$vuetify.breakpoint.name;

      let list = (this as any).translatedFields;
      let width = (this as any).nbrOfColumns;

      // Depending on screensize, limit the number of field in a row
      if (breakpoint == "xs" || breakpoint == "sm") {
        width = 1;
      }

      if (!list || !width) {
        return undefined;
      }

      /*
    Method to get the Info of Tab : Value {code, name}, Index, Max Index of Tabs.
    Method to perform 'Next' 'Previous' mechanism for JrsForm (See Questionaire)
    - Start Value
    */
      localThis.emitInfoTab(
        localThis.formFields[0],
        0,
        localThis.getFormTabs(localThis.formFields).length
      );

      return (this as any).listToMatix(list, width);
    },
    /**
     * Array of dependency rules for all fields in the form
     */
    dependencyRules() {
      // let localThis:any = this as any;
      const ret: any = (this.formFields as JrsFormField[])
        .filter(
          (field: JrsFormField) =>
            field.additional_config && field.additional_config.dependencyRules
        )
        .map(
          (field: JrsFormField | JrsFormFieldSelect) =>
            field.additional_config.dependencyRules
        )
        .reduce((rules, currentRuleList) => [...rules, ...currentRuleList], []);
      return ret;
    },
  },
  methods: {
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    /**
     * Convert a list to a matrix of a given size.
     *
     * @param list the list that must be converted
     * @param elementPerSubarray size of the sub arrays in the matrix
     * @returns A matrix of with == "elementPerSubarray" containing the elements of "list"
     */
    listToMatix(list: Array<any>, elementsPerSubarray: number) {
      let matrix: Array<any> = [];
      let i: number, k: number;

      for (i = 0, k = -1; i < list.length; i++) {
        if (i % elementsPerSubarray === 0) {
          k++;
          matrix[k] = [];
        }
        matrix[k].push(list[i]);
      }
      return matrix;
    },
    /**
     * Setup "holder" property in formData to hold the default value of a select field.
     *
     * @param field - select filed that requires default value
     */
    setupSelectDefaultValue(field: JrsFormFieldSelect) {
      let lookupItemKey = field.itemKey;
      let fieldValue = (this as any).formData[field.name];
      // var def:Array<any> = [];
      // def = field.selectItems.find( (item:any) => item[lookupItemKey] == fieldValue );

      //Multi select override. Expects formatted object from server
      const lookupName: string = field.lookupName
        ? field.lookupName
        : "selectHolder_" + field.name;
      if (field.multiple == true && fieldValue) {
        (this as any).formData[lookupName] = (this as any).formData[field.name];
      } else if (!field.multiple) {
        (this as any).formData[lookupName] = field.selectItems.find(
          (item: any) => item[lookupItemKey] == fieldValue
        );
      }
    },
    /**
     * Get distinct form tab codes.
     *
     * @param fields - array of JrsFormFields from which to extract the distinct tabs of the form
     */
    getFormTabs(fields: Array<JrsFormField>) {
      let distinctTabs: Array<JrsFormTab> = [];
      let defaultTab: JrsFormTab = {
        tabCode: (this as any).defaultTabCode,
        tabName: (this as any).defaultTabCode,
      };

      distinctTabs = fields.reduce((distinctList: any, curr: any) => {
        let newTab: JrsFormTab = Object.assign({}, defaultTab);

        // If field has tab defined
        if (curr.tabCode) {
          newTab.tabCode = curr.tabCode;
          newTab.tabName =
            curr.tabTranslationKey != undefined
              ? translate(curr.tabTranslationKey).toString()
              : curr.tabLabel || newTab.tabCode;
        }

        // If newTab is not in list
        if (
          distinctList.findIndex(
            (item: JrsFormTab) => item.tabCode == newTab.tabCode
          ) == -1
        ) {
          // If MAIN tab the it must be first
          if (newTab.tabCode == (this as any).defaultTabCode) {
            distinctList.unshift({
              tabCode: newTab.tabCode,
              tabName: newTab.tabName,
            });
          } else {
            distinctList.push({
              tabCode: newTab.tabCode,
              tabName: newTab.tabName,
            });
          }
        }

        return distinctList;
      }, distinctTabs);

      return distinctTabs;
    },
    /**
     * Get a field matrix for a given TabCode.
     *
     * @param tabCode - TabCode for which to recover the fields
     * @param fields - Array of the JrsFormFields to filter by the tabCode
     */
    getFieldMatrixForTab(tabCode: String, fields: Array<JrsFormField>) {
      let localThis: any = this as any;
      let breakpoint = localThis.$vuetify.breakpoint.name;

      let filteredFields = fields.filter((item) =>
        item.tabCode
          ? item.tabCode == tabCode
          : tabCode == (this as any).defaultTabCode
      );
      let width = (this as any).nbrOfColumns;

      // Depending on screensize, limit the number of field in a row
      if (breakpoint == "xs" || breakpoint == "sm") {
        width = 1;
      }

      if (!filteredFields || !width) {
        return undefined;
      }

      return (this as any).listToMatix(filteredFields, width);
    },
    /**
     * Calculate the number of columns a field should fill. values mut be between 1 and 12.
     */
    calcFieldColumnSpan(
      field: JrsFormField,
      nbrFields: number,
      fieldIndex: number
    ) {
      const localThis: any = this as any;
      let colSpan: number | null = 12 / localThis.nbrOfColumns;

      // Check if it is the last field
      if (nbrFields && fieldIndex && nbrFields - 1 == fieldIndex) {
        colSpan = null;
      }

      // Check if has defined col_span
      if (field.additional_config && field.additional_config.col_span) {
        colSpan = field.additional_config.col_span;
      }

      return colSpan;
    },
    tmpFieldForTab(tabCode: String, fields: Array<JrsFormField>) {
      return fields.filter((item) =>
        item.tabCode
          ? item.tabCode == tabCode
          : tabCode == (this as any).defaultTabCode
      );
    },
    /**
     * Validate form fields.
     */
    validateForm() {
      const localThis: any = this as any;
      const isValid: boolean = localThis.$refs.form.validate();

      const allTabs: JrsFormTab[] = localThis.getFormTabs(
        localThis.translatedFields
      );
      if (!isValid && allTabs.length > 0) {
        const invalidTabCodeList: String[] = allTabs.reduce(
          (invalidList: String[], curTab: JrsFormTab) => {
            const tabFields: JrsFormField[] = localThis.tmpFieldForTab(
              curTab.tabCode,
              localThis.translatedFields
            );
            // TODO: Review use of rules rather than only check required status
            // TODO: For required fields, empty strings pass the check...
            const hasInvalidFields: boolean = tabFields.some(
              (field: JrsFormField) => {
                // const isRequired: boolean = field.is_required || false;
                // const hasValue: boolean = localThis.formData[field.name] != null;
                // return isRequired && !hasValue;
                const rules: Array<Function> = field.rules ? field.rules : [];
                const hasErrors: boolean | undefined = rules.reduce(
                  (lastResult: boolean, rule: Function) => {
                    let hasError: boolean = false;
                    const functionEvaluation: boolean | string = rule(
                      localThis.formData[field.name]
                    );
                    if (typeof functionEvaluation == "string") {
                      hasError = true;
                    } else {
                      hasError = functionEvaluation as boolean;
                    }
                    return lastResult || hasError;
                  },
                  false
                );
                return hasErrors || false;
              }
            );
            if (hasInvalidFields) {
              invalidList.push(curTab.tabCode);
            }
            return invalidList;
          },
          []
        );
        localThis.invalidTabCodes = [...invalidTabCodeList];
      } else {
        localThis.invalidTabCodes = [];
      }

      return isValid;
    },
    /**
     * Reset form fields.
     */
    resetForm() {
      let localThis: any = this as any;

      try {
        //Clear formData object
        for (var propName in localThis.formData) {
          if (localThis.formData.hasOwnProperty(propName)) {
            delete localThis.formData[propName];
          }
        }
      } catch (err) {
        console.log("Encountered error: ", err);
      }

      return (this as any).$refs.form.reset();
    },
    /**
     * Update select holder field.
     */
    updateSelectHolder(payload: any) {
      let localThis: any = this as any;

      let { holderName, objectValue } = payload;
      localThis.formData[holderName] = objectValue;
      // Object.assign(localThis.formData[holderName], objectValue);
    },
    /**
     * Check for field dependecies based on a field update
     * @param updatedValue Value of the master field
     * @param updatedField JrsFormField of master field
     * @param updatedFieldName Field name of master field (used for hidden fields)
     */
    async manageDependecies(
      updatedValue: any,
      updatedField?: any,
      updatedFieldName?: string
    ) {
      const localThis: any = this as any;
      const dependencyRules: any = localThis.dependencyRules;
      const masterFieldName: string = updatedField
        ? updatedField.name
        : updatedFieldName;

      // Exit function if no master field was given
      if (!masterFieldName) {
        return;
      }

      // Get rules for current field
      const currentRules: Array<any> = dependencyRules.filter(
        (rule: any) => rule.masterFieldName == masterFieldName
      );

      // Exit function if no rules exists
      if (currentRules.length == 0) {
        return;
      }

      // Group rules for target fields
      let targetRules: any = {};

      const filterTargetFieldNames: Array<string> = [
        ...new Set(
          currentRules
            .filter((rule: any) => rule.dependencyType == "filter")
            .map((rule: any) => rule.targetFieldName)
        ),
      ];
      const disableTargetNames: Array<string> = [
        ...new Set(
          currentRules
            .filter((rule: any) => rule.dependencyType == "disable")
            .map((rule: any) => rule.targetFieldName)
        ),
      ];

      // Manage filter conditions
      filterTargetFieldNames.forEach((targetField: string) => {
        // Merge conditions for all rules with a common Target Field
        const condition: string = currentRules
          .filter(
            (rule: any) =>
              rule.targetFieldName == targetField &&
              rule.dependencyType == "filter"
          )
          // .map(rule => rule.targetCondition)
          .map((rule) =>
            rule.targetCondition
              .split(`##${rule.masterFieldName}##`)
              .join(updatedValue)
          )
          .join(" AND ");
        // Set target rule
        targetRules[targetField] = {
          field_name: targetField,
          field_condition: condition,
        };
      });

      const filterTargetFields: Array<JrsFormField> =
        localThis.translatedFields.filter((field: JrsFormField) =>
          filterTargetFieldNames.includes(field.name)
        );
      const selectFieldTypes: fieldType[] = [
        fieldType.select,
        fieldType.auto,
        fieldType.combobox,
        fieldType.combobox_multi,
      ];
      const searchTableFieldTypes: fieldType[] = [
        fieldType.search_table,
        fieldType.search_table_multi,
      ];

      // Refresh target Select fields
      const getRefreshedSelectFields: Function = async (
        fields: JrsFormField[]
      ) => {
        const promList: Promise<JrsFormFieldSelect>[] = fields.map(
          (field: JrsFormField) => {
            const newField: Promise<JrsFormFieldSelect> =
              localThis.setupSelectFieldAsync(
                field,
                localThis.getCurrentOffice.HR_OFFICE_ID,
                Object.values(targetRules)
              );
            return newField;
          }
        );
        return Promise.all(promList);
      };

      // Set new select field items
      getRefreshedSelectFields(
        filterTargetFields.filter((field: JrsFormField) =>
          selectFieldTypes.includes(field.type)
        )
      ).then((NewSelectFields: JrsFormFieldSelect[]) => {
        NewSelectFields.forEach((field: JrsFormFieldSelect) => {
          // Refresh Select target fields
          if (selectFieldTypes.includes(field.type)) {
            localThis.updatedSelectFieldItems = {
              ...localThis.updatedSelectFieldItems,
            };
            localThis.updatedSelectFieldItems[field.name] =
              NewSelectFields.find(
                (newField: JrsFormFieldSelect) => newField.name == field.name
              )?.selectItems || [];
          }
        });
      });

      filterTargetFields.forEach((field: JrsFormField) => {
        // Refresh SerachTable target fields
        if (searchTableFieldTypes.includes(field.type)) {
          localThis.updatedSearcTableConditions = {
            ...localThis.updatedSearcTableConditions,
          };
          localThis.updatedSearcTableConditions[field.name] =
            targetRules[field.name].field_condition;
        }

        // Refresh field messages
        if (updatedField) {
          localThis.fieldMessages = { ...localThis.fieldMessages };
          localThis.fieldMessages[field.name] = [
            localThis
              .$t("general.crud.filtered-dataset-by")
              .split("##FIELD_NAME##")
              .join(updatedField.label),
          ];
        }

        // Reset target field value
        localThis.formData[field.name] = null;
      });

      // Manage disable conditions
      const disableTargetFields: Array<JrsFormField> =
        localThis.translatedFields.filter((field: JrsFormField) =>
          disableTargetNames.includes(field.name)
        );
      let evaluationFuncs: Array<Function> = disableTargetNames.map(
        (targetName: string) => {
          console.log("Creating function");
          console.log("Called Function");
          const rule: any = currentRules.find(
            (rule: any) =>
              rule.targetFieldName == targetName &&
              rule.masterFieldName == masterFieldName
          );
          const targetField: JrsFormField = localThis.translatedFields.find(
            (field: JrsFormField) => field.name == targetName
          );
          const payload: GenericSqlSelectPayload = {
            viewName: rule.datasource,
            colList: `${rule.evaluation} AS EVALUATION_RESULT`,
            whereCond: rule.datasourceCondition
              .split(`##${rule.masterFieldName}##`)
              .join(updatedValue),
            itemNumber: 1,
          };
          return localThis
            .getGenericSelect({ genericSqlPayload: payload })
            .then((res: any) => {
              const evaluationResult: boolean = res.table_data
                ? res.table_data[0].EVALUATION_RESULT
                : !updatedValue
                ? false
                : undefined;
              if (evaluationResult == undefined) {
                throw new Error("Field condition evaluation error.");
              }

              // Disable target field
              const readOnlyCheck: Function = (a: boolean, b: boolean) =>
                (a && b) || !(a || b);
              const readonlyValue: boolean = readOnlyCheck(
                rule.disableField,
                evaluationResult
              );
              localThis.fieldReadonlyRules = {
                ...localThis.fieldReadonlyRules,
              };
              localThis.fieldReadonlyRules[targetName] = readonlyValue;

              // Reset target field value
              if (readonlyValue) {
                localThis.formData[targetName] = null;
              }
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err,
              });
            });
        }
      );

      // Resolve all field disabling promisses
      Promise.all(evaluationFuncs).then((res: any) => {
        console.log(JSON.stringify(res));
      });
    },
    inputTest() {
      console.log("INPUT");
    },
    updateTest() {
      console.log("UPDATE");
    },

    /*
    Method to get the Info of Tab : Value {code, name}, Index, Max Index of Tabs.
    Method to perform 'Next' 'Previous' mechanism for JrsForm (See Questionaire)
    */
    emitInfoTab(value: any, index: any, length: any) {
      let object = { value: value, index: index, maxIndex: length - 1 };
      this.$emit("getTabsInfo", object);
    },
  },
  async mounted() {
    const localThis: any = this as any;

    try {
      localThis.calculatingDependecies = true;
  
      const formFieldNames: string[] = localThis.formFields.map(
        (field: JrsFormField) => field.name
      );
  
      // Set field condition for all hidden fields
      const hiddenFieldNames: string[] = Object.keys(localThis.formData).filter(
        (fieldName: string) => !formFieldNames.includes(fieldName)
      );
      
      hiddenFieldNames.forEach(async (fieldName: string) => {
        const hiddenFieldValue: any = localThis.formData[fieldName];
        await localThis.manageDependecies(hiddenFieldValue, null, fieldName);
      });
      
      localThis.calculatingDependecies = false;
    } catch (error) {
      localThis.showNewSnackbar({
        typeName: "error",
        text: error,
      });
    }
  },
});
</script>

<style scoped>
</style>