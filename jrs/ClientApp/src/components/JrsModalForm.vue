<template>
  <div>
    <v-dialog
      v-model="dialog"
      persistent
      retain-focus
      :scrollable="scrollable"
      :overlay="false"
      :max-width="
        overrideMaxWidth ? overrideMaxWidth : (50 * nbrOfColumns + 25) / 3 + 'em'
      "
      transition="dialog-transition"
    >
      <template v-slot:activator="{ on }">
        <div v-on="on" @click="dialog = false" class="jrs-modal-form-activator">
          <slot name="activation"></slot>
        </div>
      </template>

      <!-- DIALOG CONTENT -->

      <v-card>
        <v-card-text>
          <!-- EDIT FORM -->
          <jrs-form
            :formData="formData"
            :formFields="formFields"
            :nbrOfColumns="nbrOfColumns"
            v-if="!readonly"
            :ignoreOfficeFilter="ignoreOfficeFilter"
          >
            <template v-slot:form-actions="{ validateFunc, resetFunc }">
              <slot
                name="modal-form-actions"
                :closeModalFunc="closeModal"
                :validateFunc="validateFunc"
                :resetFunc="resetFunc"
              ></slot>
            </template>
          </jrs-form>

          <!-- READONLY DETAIL -->
          <div>
            <jrs-readonly-detail
              :detailFields="formFields"
              :detailData="formData"
              :nbrOfColumns="nbrOfColumns"
              v-if="readonly"
            ></jrs-readonly-detail>
          </div>
        </v-card-text>
        <v-card-actions v-if="readonly">
          <v-btn text color="secondary darken-1" @click="closeModal()"
            >X {{ $t("general.close") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { translate } from "../i18n";
import {
  JrsFormField,
  JrsFormFieldSelect,
  JrsFormFieldSearch,
  JrsFormTab,
  fieldType,
} from "../models/JrsForm";
// import JrsDatePicker from "../components/JrsDatePicker.vue";
// import { VueEditor } from "vue2-editor";
// import JrsLocationPicker from "../components/JrsLocationPicker.vue";
// import JrsSearchTable from "../components/JrsSearchTable.vue";
import JrsForm from "../components/JrsForm.vue";
import JrsReadonyDetail from "../components/JrsReadonyDetail.vue";

export default Vue.extend({
  components: {
    // "jrs-date-picker": JrsDatePicker,
    // "vue-editor": VueEditor,
    // "jrs-location-picker": JrsLocationPicker,
    // "jrs-search-table": JrsSearchTable,
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadonyDetail,
  },
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
    dialogExt: {
      type: Boolean,
      required: false,
      default: false,
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
     * Ignore office filter in data query.
     */
    ignoreOfficeFilter: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Adds scrollbar to form if longer than screen.
     */
    scrollable: {
      type: Boolean,
      required: false,
      default: false,
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
    idx: {
      type: Number,
      required: false,
      default: 0,
    },
    pkname: {
      type: String,
      required: false,
      default: null,
    },
    /**
     * Force readonly instead of edit form.
     */
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Override form width.
     */
    overrideMaxWidth: {
      type: String,
      required: false,
      default: null,
    },
  },
  data() {
    return {
      dialog: false,
      formIsValid: false,
      requiredTextFieldRule: [(v: any) => !!v || "Name is required"],
      formTabsModel: null,
      defaultTabCode: "MAIN",
      showPasswords: false,
    };
  },
  computed: {
    /**
     * Calculated number rows based on number of fields and columns.
     */
    nbrOfRows() {
      let nbr: number =
        Math.floor((this as any).formFields.length / (this as any).nbrOfColumns) +
        ((this as any).formFields.length % (this as any).nbrOfColumns);
      return nbr;
    },
    /**
     * Fields with translated labels and hints.
     */
    translatedFields() {
      return (this.formFields as JrsFormField[])
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
            (this as any).formData &&
            (this as any).formData[field.name]
          ) {
            (this as any).setupSelectDefaultValue(field);
          }

          return field;
        })
        .sort((a: JrsFormField, b: JrsFormField) => {
          let order_A: number = a.form_order ? a.form_order : 1000;
          let order_B: number = b.form_order ? b.form_order : 1000;
          return order_A < order_B ? -1 : 1;
        });
    },

    // dialog: {
    //   get() {
    //     let localThis: any = this as any;
    //     return localThis.dialogExt;
    //   },
    //   set(newVal: Boolean) {
    //     let localThis: any = this as any;
    //     localThis.dialog = newVal;
    //   },
    // },

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
     * There are fields witha tabcCode defined
     */
    tabsAreDefined() {
      let index: number = (this as any).translatedFields.findIndex(
        (field: JrsFormField) => field.tabCode != undefined
      );
      return index > -1 ? true : false;
    },
  },
  methods: {
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
              : newTab.tabCode;
        }

        // If newTab is not in list
        if (
          distinctList.findIndex((item: JrsFormTab) => item.tabCode == newTab.tabCode) ==
          -1
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
     * Closes the modal.
     */
    closeModal() {
      (this as any).dialog = false;
    },
  },

  mounted() {
    let localThis: any = this as any;
    if (localThis.idx != 0 && localThis.pkname != undefined) {
      if (localThis.formData[localThis.pkname] == localThis.idx) {
        localThis.dialog = localThis.dialogExt;
      }
    }
  },
});
</script>

<style scoped>
.jrs-modal-form-activator {
  margin: 0 0.3em;
}
</style>
