<template>
  <div>
    <v-row no-gutters align="center" justify="center">
      <v-col cols="6">
        <span v-if="checkDetailDifferences"
          >Differences with {{ detailDataDiff["DATE_PREV"] }}</span
        >
      </v-col>
      <v-col cols="3">
        <v-btn
          class="ma-2"
          text
          v-if="checkDetailDifferences"
          icon
          :color="colorDiffCorrect"
        >
          <v-icon>mdi-format-color-highlight</v-icon>
          New
        </v-btn>
      </v-col>
      <v-col cols="3">
        <v-btn class="ma-2" text v-if="checkDetailDifferences" icon :color="colorDiff">
          <v-icon>mdi-format-color-highlight</v-icon>
          Older
        </v-btn>
      </v-col>
    </v-row>
    <div v-if="getFormTabs(detailFields).length <= 1">
      <v-row v-for="(row, i) in fieldsMatrix" :key="i">
        <template v-for="(field, j) in fieldsMatrix[i]">
          <v-col :key="j" :cols="$vuetify.breakpoint.xsOnly ? 12 : 12 / nbrOfColumns">
            <div class="d-inline-flex justify-space-between" style="width: 100%">
              <span class="font-weight-bold text-capitalize">{{ field.label }}:</span>
              <span
                v-if="field.type == 'text_edit'"
                v-html="resolvedData[field.name]"
              ></span>
              <span
                v-else-if="
                  field.type == 'combobox_multi' ||
                  field.type == 'combobox' ||
                  field.type == 'search_table_multi'
                "
              >
                <template v-for="item in detailData[field.name]">
                  <v-chip v-if="item" :key="item[0]" class="ma-2">
                    {{ item[field.itemText] }}
                  </v-chip>
                </template>
              </span>
              <span
                v-else-if="
                  field.type == 'radio_multi' && detailData[field.name]
                    ? detailData[field.name].length > 0
                    : false
                "
              >
                <template v-for="item in JSON.parse(detailData[field.name])">
                  <v-chip v-if="item" :key="item[0]" class="ma-2">
                    {{ item.imsLookupValue }}
                  </v-chip>
                </template>
              </span>
              <span v-else-if="field.type == 'select'">
                <v-chip v-show="resolvedData[field.name]" class="ma-2">
                  {{ resolvedData[field.name] }}
                </v-chip>
              </span>
              <span
                v-else-if="field.type == 'radio_matrix' && resolvedData[field.name]"
                style="white-space: pre-wrap"
              >
                {{ resolvedData[field.name] }}
              </span>
              <span v-else>{{ resolvedData[field.name] }}</span>
            </div>
          </v-col>
          <!-- <v-divider :key="'DIVIDER_'+j" vertical v-if="j < fieldsMatrix[i].length-1"></v-divider> -->
        </template>
      </v-row>
    </div>
    <v-tabs v-if="getFormTabs(detailFields).length > 1" v-model="tabsModel">
      <v-tab
        @click="
          emitInfoTab(
            tabInfo,
            getFormTabs(detailFields).findIndex((object) => {
              return object.tabCode === tabInfo.tabCode;
            }),
            getFormTabs(detailFields).length
          )
        "
        v-for="tabInfo in getFormTabs(translatedFields)"
        :key="'TAB-' + tabInfo.tabCode"
        >{{ tabInfo.tabName }}</v-tab
      >
    </v-tabs>
    <v-tabs-items v-model="tabsModel">
      <v-tab-item
        v-for="tabInfo in getFormTabs(translatedFields)"
        :key="'TAB-' + tabInfo.tabCode"
      >
        <v-row
          v-for="(row, i) in getFieldMatrixForTab(tabInfo.tabCode, translatedFields)"
          :key="'ROW-' + i"
        >
          <template
            v-for="(field, j) in getFieldMatrixForTab(tabInfo.tabCode, translatedFields)[
              i
            ]"
          >
            <v-col :key="'COL-' + j" class="px-3">
              <div class="d-inline-flex justify-space-between" style="width: 100%">
                <span class="font-weight-bold text-capitalize">{{ field.label }}:</span>
                <span
                  v-if="field.type == 'text_edit'"
                  v-html="resolvedData[field.name]"
                ></span>
                <span
                  v-else-if="
                    field.type == 'search_table_multi' || field.type == 'radio_multi'
                  "
                >
                  <template v-for="item in detailData[field.name]">
                    <v-chip
                      v-if="item"
                      :key="item[field.itemKey]"
                      class="ma-2"
                      :color="
                        checkDetailDifferences
                          ? checkDiffMultiSelect(
                              field.name,
                              field.itemKey,
                              item[field.itemKey]
                            ) || !(item && detailDataDiff[field.name])
                            ? colorDiffCorrect
                            : ''
                          : ''
                      "
                    >
                      {{ item[field.itemText] }}
                    </v-chip>
                  </template>
                  <div v-if="checkDetailDifferences">
                    <template v-for="item in detailDataDiff[field.name]">
                      <v-chip
                        v-if="
                          checkDiffMultiSelect(
                            field.name,
                            field.itemKey,
                            item[field.itemKey]
                          )
                        "
                        :key="item[field.itemKey] + 'diff'"
                        class="ma-2"
                        :color="
                          checkDetailDifferences
                            ? checkDiffMultiSelect(
                                field.name,
                                field.itemKey,
                                item[field.itemKey]
                              )
                              ? colorDiff
                              : ''
                            : ''
                        "
                      >
                        {{ item[field.itemText] }}
                      </v-chip>
                    </template>
                  </div>
                </span>
                <span v-else-if="field.type == 'checkbox'">
                  <v-icon
                    :color="
                      resolvedData[field.name] ? 'green darken-3' : 'deep-orange darken-3'
                    "
                    >{{
                      resolvedData[field.name]
                        ? "mdi-checkbox-marked-circle-outline"
                        : "mdi-checkbox-blank-circle-outline"
                    }}</v-icon
                  >
                </span>
                <div v-else>
                  <span
                    :class="
                      checkDetailDifferences
                        ? JSON.stringify(detailDataDiff[field.name]) !=
                          JSON.stringify(detailData[field.name])
                          ? 'font-weight-bold white--text text-no-wrap ' +
                            colorDiffCorrect
                          : ''
                        : ''
                    "
                    >{{ resolvedData[field.name] }}</span
                  >
                  <v-spacer></v-spacer>
                  <span
                    v-if="
                      checkDetailDifferences
                        ? JSON.stringify(detailDataDiff[field.name]) !=
                            JSON.stringify(detailData[field.name]) &&
                          detailDataDiff[field.name]
                        : false
                    "
                    :class="
                      checkDetailDifferences
                        ? detailDataDiff[field.name]
                          ? 'font-weight-bold white--text text-no-wrap ' + colorDiff
                          : ''
                        : ''
                    "
                    >{{ resolvedDataDiff[field.name] }}</span
                  >
                </div>
              </div>
            </v-col>
            <v-divider
              :key="'DIVIDER_' + j"
              vertical
              v-if="
                j < getFieldMatrixForTab(tabInfo.tabCode, translatedFields)[i].length - 1
              "
            ></v-divider>
          </template>
        </v-row>
      </v-tab-item>
    </v-tabs-items>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import { GenericSqlSelectPayload, ImsLookup } from "../axiosapi/api";
import { translate, formatDateTime } from "../i18n";
import {
  JrsFormField,
  JrsFormTab,
  JrsFormFieldSelect,
  fieldType,
} from "../models/JrsForm";

export default Vue.extend({
  props: {
    /**
     * List of fields for the dialog. Items bust be of thipe "JrsFormField" or one of the extensions.
     */
    detailFields: {
      type: Array as () => JrsFormField[],
      required: true,
    },
    /**
     * Object with te data of the dialog.
     */
    detailData: {
      type: Object,
      required: true,
    },
    /**
     * Number of columns fo the form.
     * Excepted values: {1, 2, 3}
     */
    nbrOfColumns: {
      type: Number,
      required: false,
      default: 2,
      validator: function (value: any) {
        // The value must match one of these values
        return [1, 2, 3, 4].indexOf(value) !== -1;
      },
    },
    detailDataDiff: {
      type: Object,
      required: false,
    },
    tabNoDiff: {
      type: Array,
      required: false,
    },
  },
  data() {
    return {
      tabsModel: null,
      defaultTabCode: "MAIN",
      selectKeyValues: [],
      selectKeyValuesDiff: [],
      multiSelectValue: [],
      changeColorText: false,
      colorDiff: "yellow accent-3",
      colorDiffCorrect: "primary",
      labelDiff: "The difference from version ",
      currentTab: 0,
    };
  },
  computed: {
    ...mapGetters({
      getLookupsByTypeCode: "getLookupsByTypeCode",
    }),
    /**
     * Fields with translated labels and hints.
     */
    translatedFields() {
      let localThis = this as any;
      return (localThis.detailFields as JrsFormField[])
        .map((field: JrsFormField | JrsFormFieldSelect) => {
          field.label =
            field.labelTranslationKey != undefined
              ? translate(field.labelTranslationKey).toString()
              : field.label;
          field.hint =
            field.hintTranslationKey != undefined
              ? translate(field.hintTranslationKey).toString()
              : field.hint;

          //Setup select default value
          if (
            (field.type == fieldType["select"] || field.type == fieldType["auto"]) &&
            localThis.detailData &&
            localThis.detailData[field.name]
          ) {
            localThis.setupSelectDefaultValue(field);
          }

          return field;
        })
        .sort((a: JrsFormField, b: JrsFormField) => {
          let order_A: number = a.form_order ? a.form_order : 1000;
          let order_B: number = b.form_order ? b.form_order : 1000;
          return order_A < order_B ? -1 : 1;
        });
    },
    /**
     * Matrix of the fields divided in rows and columns.
     */
    fieldsMatrix() {
      let list = (this as any).translatedFields;
      let width = (this as any).nbrOfColumns;
      if (!list || !width) {
        return undefined;
      }
      return (this as any).listToMatix(list, width);
    },
    /**
     * Fields with resolved values for select and search-table fields.
     */
    resolvedData() {
      let localThis: any = this as any;
      let data = Object.assign({}, localThis.detailData);
      localThis.selectKeyValues.forEach((item: any) => {
        data[item.key] = item.value;
      });
      return data;
    },

    /**
     * Fields with resolved values for select and search-table fields.
     */
    resolvedDataDiff() {
      let localThis: any = this as any;
      if (localThis.checkDetailDifferences) {
        let data = Object.assign({}, localThis.detailDataDiff);
        localThis.selectKeyValuesDiff.forEach((item: any) => {
          data[item.key] = item.value;
        });
        return data;
      } else {
        return [];
      }
    },

    checkDetailDifferences() {
      let localThis: any = this as any;
      return localThis.detailDataDiff &&
        (localThis.tabNoDiff ? !localThis.tabNoDiff.includes(localThis.currentTab) : true)
        ? Object.keys(localThis.detailDataDiff).length !== 0
        : false;
    },
  },
  watch: {
    detailData(to: any, from: any) {
      let localThis: any = this as any;
      if (to != from) {
        localThis.setupResolvedFields(localThis.detailFields);
      }
    },
    detailDataDiff(to: any, from: any) {
      let localThis: any = this as any;
      if (to != from) {
        localThis.setupResolvedFields(localThis.detailFields);
      }
      if (localThis.checkDetailDifferences) {
        localThis.labelDiff =
          "The difference from version " +
          to["SET_SPEC_DATE"] +
          " - " +
          localThis.detailDataDiff["SET_SPEC_DATE"];
      }
    },
    detailFields(to: JrsFormField[]) {
      const localThis: any = this as any;
      localThis.setupResolvedFields(to);
    },
  },
  methods: {
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    /*
    Method to get the Info of Tab : Value {code, name}, Index, Max Index of Tabs.
    Method to perform 'Next' 'Previous' mechanism for JrsForm (See Questionaire)
    */
    emitInfoTab(value: any, index: any, length: any) {
      let object = { value: value, index: index, maxIndex: length - 1 };
      (this as any).currentTab = object.index;
      this.$emit("getTabsInfo", object);
    },
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
              : newTab.tabCode;
        }

        // If newTab is not in list
        if (
          distinctList.findIndex((item: JrsFormTab) => item.tabCode == newTab.tabCode) ==
          -1
        ) {
          // If MAIN tab then it must be first
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
      let filteredFields = fields.filter((item) =>
        item.tabCode ? item.tabCode == tabCode : tabCode == (this as any).defaultTabCode
      );
      let width = (this as any).nbrOfColumns;
      if (!filteredFields || !width) {
        return undefined;
      }
      return (this as any).listToMatix(filteredFields, width);
    },
    /**
     * Setup "holder" property in detailData to hold the default value of a select field.
     *
     * @param field - select filed that requires default value
     */
    setupSelectDefaultValue(field: JrsFormFieldSelect) {
      let lookupItemKey = field.itemKey;
      let fieldValue = (this as any).detailData[field.name];
      // var def:Array<any> = [];
      // def = field.selectItems.find( (item:any) => item[lookupItemKey] == fieldValue );

      //Multi select override. Expects formatted object from server

      const lookupName: string = field.lookupName
        ? field.lookupName
        : "selectHolder_" + field.name;
      if (field.multiple == true && fieldValue) {
        (this as any).detailData[lookupName] = (this as any).detailData[field.name];
      } else if (!field.multiple) {
        (this as any).detailData[lookupName] = field.selectItems.find(
          (item: any) => item[lookupItemKey] == fieldValue
        );
      }
    },
    /**
     * Get description value for a select-like field.
     * @param field Select field to calculate description for
     * @return {Promise} The promise resolves a string with the description for the field
     */
    getDescriptionForValue(field: any): Promise<string | null> {
      let localThis: any = this as any;

      // If null don't calc description value

      if (!localThis.detailData[field.name]) {
        return new Promise((resolve, reject) => {
          resolve(null);
        });
      }
      // If generic lookup
      if (field.select_lookup_code) {
        let lookupValue = localThis
          .getLookupsByTypeCode(field.select_lookup_code)

          .find(
            (lookup: ImsLookup) => lookup.imsLookupId == localThis.detailData[field.name]
          ).imsLookupValue;
        return new Promise((resolve, reject) => {
          resolve(lookupValue);
        });
      }

      let condition: string = `${field.itemKey} = '${localThis.detailData[field.name]}'`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: field.dataSource,
        colList: `${field.itemKey}, ${field.itemText}`,
        whereCond: condition,
        orderStmt: null,
        officeId: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          const localThis: any = this as any;
          const missingDataMessage: string = localThis.$t("error.missing-data");
          return res.table_data && res.table_data.length > 0
            ? res.table_data[0][field.itemText]
            : missingDataMessage;
        });
    },

    getDescriptionForValueDiff(field: any): Promise<string | null> {
      let localThis: any = this as any;

      // If null don't calc description value

      if (!localThis.detailDataDiff[field.name]) {
        return new Promise((resolve, reject) => {
          resolve(null);
        });
      }
      // If generic lookup
      if (field.select_lookup_code) {
        let lookupValue = localThis
          .getLookupsByTypeCode(field.select_lookup_code)

          .find(
            (lookup: ImsLookup) =>
              lookup.imsLookupId == localThis.detailDataDiff[field.name]
          ).imsLookupValue;
        return new Promise((resolve, reject) => {
          resolve(lookupValue);
        });
      }

      let condition: string = `${field.itemKey} = '${
        localThis.detailDataDiff[field.name]
      }'`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: field.dataSource,
        colList: `${field.itemKey}, ${field.itemText}`,
        whereCond: condition,
        orderStmt: null,
        officeId: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          const localThis: any = this as any;
          const missingDataMessage: string = localThis.$t("error.missing-data");
          return res.table_data && res.table_data.length > 0
            ? res.table_data[0][field.itemText]
            : missingDataMessage;
        });
    },

    getDescriptionForValueMatrix(field: any): Promise<string | null> {
      let localThis: any = this as any;

      // If null don't calc description value

      if (!localThis.detailData[field.name]) {
        return new Promise((resolve, reject) => {
          resolve(null);
        });
      }
      // If generic lookup
      if (field.select_lookup_code) {
        let lookupValue = localThis
          .getLookupsByTypeCode(field.select_lookup_code)

          .find(
            (lookup: ImsLookup) => lookup.imsLookupId == localThis.detailData[field.name]
          ).imsLookupValue;
        return new Promise((resolve, reject) => {
          resolve(lookupValue);
        });
      }
      let cond: string = localThis.detailData[field.name]
        .replace("[", "(")
        .replace("]", ")");
      let condition: string = `${field.itemKey} in ${cond}`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: field.dataSource,
        colList: `${field.itemKey}, ${field.itemText}`,
        whereCond: condition,
        orderStmt: null,
        officeId: null,
      };
      let i: number = 0;
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          const localThis: any = this as any;
          const missingDataMessage: string = localThis.$t("error.missing-data");
          if (res.table_data && res.table_data.length > 0) {
            let retStr: string = "\n";
            for (i = 0; i < res.table_data.length; i++) {
              retStr += res.table_data[i][field.itemText] + "\n";
            }
            return retStr;
          } else return missingDataMessage;
        });
    },

    /**
     * Get description value for a location picker field.
     * @param field Location field to calculate description for
     * @return {Promise} The promise resolves a string with the description for the field
     */
    getDesctiptionForLocation(field: any): Promise<string | null> {
      let localThis: any = this as any;

      // If null don't calc description value
      if (!localThis.detailData[field.name]) {
        return new Promise((resolve, reject) => {
          resolve(null);
        });
      }

      let condition: string = `IMS_LOCATION_ID = '${localThis.detailData[field.name]}'`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "VI_IMS_LOCATION_DESCRIPTIVE",
        colList: `IMS_LOCATION_ID, AREA_1_NAME, AREA_2_NAME, AREA_3_NAME, IMS_LOCATION_ADDRESS, IMS_LOCATION_POSTAL_CODE`,
        whereCond: condition,
        orderStmt: null,
        officeId: null,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (!res.table_data || res.table_data.length == 0) {
            return null;
          }
          let row: any = res.table_data[0];

          let add: string = row.IMS_LOCATION_ADDRESS;
          let pCode: string = row.IMS_LOCATION_POSTAL_CODE
            ? " " + row.IMS_LOCATION_POSTAL_CODE
            : "";
          let adm1: string = row.AREA_1_NAME ? ", " + row.AREA_1_NAME : "";
          let adm2: string = row.AREA_2_NAME ? ", " + row.AREA_2_NAME : "";
          let adm3: string = row.AREA_3_NAME ? row.AREA_3_NAME : "";

          return `${add} ${adm3}${adm2}${pCode}${adm1}`;
        });
    },
    /**
     * Setup the description values for all fields of the detail, based on the field type.
     * @param {Array} fields List of all the fields of the detail
     */
    setupResolvedFields(fields: Array<JrsFormField>): void {
      let localThis: any = this as any;

      // Clear old key-value objects
      localThis.selectKeyValues.length = 0;

      localThis.selectKeyValuesDiff.length = 0;

      fields.forEach((field: any) => {
        switch (field.type) {
          case "select":
          case "auto":
          case "search_table":
          case "radio":
            localThis.getDescriptionForValue(field).then((res: string | null) => {
              if (
                res &&
                localThis.selectKeyValues.findIndex((item: any) => item.key == res) == -1
              ) {
                localThis.selectKeyValues.push({ key: field.name, value: res });
              }
            });
            if (localThis.checkDetailDifferences) {
              localThis.getDescriptionForValueDiff(field).then((res: string | null) => {
                if (
                  res &&
                  localThis.selectKeyValuesDiff.findIndex(
                    (item: any) => item.key == res
                  ) == -1
                ) {
                  localThis.selectKeyValuesDiff.push({ key: field.name, value: res });
                }
              });
            }
            break;
          case "radio_matrix":
            localThis.getDescriptionForValueMatrix(field).then((res: string | null) => {
              if (
                res &&
                localThis.selectKeyValues.findIndex((item: any) => item.key == res) == -1
              ) {
                localThis.selectKeyValues.push({ key: field.name, value: res });
              }
            });
            break;
          case "search_table_multi":
          case "radio_multi":
          case "combobox_multi":
            /* if(localThis.detailData[field.name]){
                                 localThis.detailData[field.name].array.forEach(element => {
                                        localThis.getDescriptionForValue(field)
                                        .then((res:string|null) => {
                                           if(res && localThis.selectKeyValues.findIndex( (item:any) => item.key == res) == -1){
                                               localThis.selectKeyValues.push({ key:field.name, value:res });
                                           }
                               });
                                 });


                            }*/
            localThis.selectKeyValues.push({
              key: field.name,
              value: localThis.detailData[field.name],
            });
            if (localThis.checkDetailDifferences) {
              localThis.selectKeyValuesDiff.push({
                key: field.name,
                value: localThis.detailDataDiff[field.name],
              });
            }

            break;

          case "location":
            localThis.getDesctiptionForLocation(field).then((res: string | null) => {
              if (res) {
                localThis.selectKeyValues.push({ key: field.name, value: res });
                if (localThis.checkDetailDifferences) {
                  localThis.selectKeyValuesDiff.push({ key: field.name, value: res });
                }
              }
            });
            break;

          case "date":
            localThis.selectKeyValues.push({
              key: field.name,
              value: localThis.detailData[field.name]
                ? formatDateTime(new Date(localThis.detailData[field.name]), "short")
                : undefined,
            });
            if (localThis.checkDetailDifferences) {
              localThis.selectKeyValuesDiff.push({
                key: field.name,
                value: localThis.detailDataDiff[field.name]
                  ? formatDateTime(
                      new Date(localThis.detailDataDiff[field.name]),
                      "short"
                    )
                  : undefined,
              });
            }
            break;

          case "checkbox":
            localThis.selectKeyValues.push({
              key: field.name,
              value: localThis.detailData[field.name],
            });
            if (localThis.checkDetailDifferences) {
              localThis.selectKeyValuesDiff.push({
                key: field.name,
                value: localThis.detailDataDiff[field.name],
              });
            }
            break;

          default:
            break;
        }
      });
    },

    /*checkDiffMultiSelect(fieldName:any, key:any, value:any){
      let localThis: any = this as any;
      if(localThis.detailData[fieldName] && localThis.detailDataDiff[fieldName]){
        return !localThis.detailDataDiff[fieldName].some((e:any) => {
          return e[key] === value}
          )
      }else{
        return false
      }

    }*/

    checkDiffMultiSelect(fieldName: any, key: any, value: any) {
      let localThis: any = this as any;
      if (localThis.checkDetailDifferences) {
        if (localThis.detailData[fieldName] && localThis.detailDataDiff[fieldName]) {
          return JSON.stringify(localThis.detailData[fieldName]) !=
            JSON.stringify(localThis.detailDataDiff[fieldName])
            ? true
            : false;
        } else {
          return false;
        }
      } else {
        return false;
      }
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.setupResolvedFields(localThis.detailFields);
  },
});
</script>

<style scoped></style>
