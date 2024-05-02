<template>
  <div class="d-flex">
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>

    <v-dialog
      v-model="editMode"
      persistent
      retain-focus
      :overlay="false"
      max-width="60em"
      transition="dialog-transition"
    >
      <template v-slot:activator="{ on }">
        <div style="width: 100%" v-on="on">
          <v-autocomplete
            v-model="selectedItems"
            :items="selectedItems"
            outlined
            :hint="hint"
            chips
            readonly
            dense
            small-chips
            :label="label"
            multiple
            prepend-icon="mdi-magnify"
            :item-text="itemText"
            :item-value="itemValue"
            :messages="messages"
            :class="{ 'multiselect-list-flex-column': displayChipsColumn }"
          >
            <template v-slot:selection="data">
              <v-chip
                :key="data.item[itemValue]"
                class="ma-2"
                @click:close="remove(data.item[itemValue])"
                close
                text-color="white"
                close-icon="mdi-delete"
                :color="colorNameField ? data.item[colorNameField] :'primary'"
              >
                <v-icon v-if="iconAppend" left>
                  {{ iconAppend }}
                </v-icon>
                {{
                  data.item[itemText].length > 50
                    ? data.item[itemText].substring(0, 49) + "&hellip;"
                    : data.item[itemText]
                }}
              </v-chip>
            </template>
          </v-autocomplete>
        </div>
      </template>

      <v-card>
        <v-card-title></v-card-title>
        <v-card-text>
          <jrs-table
            :title="title || $t(tableTitleKey) || dataSource"
            :headers="headers"
            :rows="rows"
            :pkName="pkName"
            v-model="selectedItems"
            :showSearchbar="true"
            :selectable="true"
            :multiSelect="allowMultiselect"
            :forMultiSelect="true"
            :rowDisableSelectRules="rowDisableSelectRules"
          ></jrs-table>
        </v-card-text>
        <v-card-actions>
          <v-btn color="primary" text @click="editMode = false">{{
            $t("general.close")
          }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-icon class="grey--text text--darken-3 mx-3 mb-6" @click="clearField()"
      >mdi-close</v-icon
    >
  </div>
</template>

<script lang="ts">
/**
         * This component aims to select multiple element from table and return a object of type Chips for 
         * each element of table with value "itemText". The object returned is a list of Ids passing from
         * name of column "itemValue" if the prop "returnOnlyIds" is true otherwise return the entire object. Example of call of component:
         *   <jrs-multi-selected-table
             hint="select the placement interval for placement test"
             label="Placement Interval"
             data-source="PMS_QUESTION"
             itemValue="QUESTION_ID"
             itemText="QUESTION_TEXT"
             v-model="itemsSelected"
          >   
          </jrs-multi-selected-table>
    */
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import { GenericSqlSelectPayload } from "../axiosapi/api";
import { JrsHeader } from "../models/JrsTable";
import JrsTable from "../components/JrsTable.vue";
import { formatDateTime, translate } from "../i18n";

export default Vue.extend({
  components: {
    "jrs-table": JrsTable,
  },
  props: {
    value: {
      type: [Number, String, Array, Object],
      required: false,
    },
    allowMultiselect: {
      type: Boolean,
      required: false,
      default: true,
    },
    label: {
      type: String,
      required: false,
      default: null,
    },
    hint: {
      type: String,
      required: false,
      default: null,
    },
    required: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Title to display on table header.
     */
    title: {
      type: String,
      required: false,
    },
    /**
     * Name of the sql data-source to query.
     */
    dataSource: {
      type: String,
      required: true,
    },
    /**
     * Name of the value column of the data-source.
     */
    itemValue: {
      type: [String, Object],
      required: true,
    },
    /**
     * Name of the text column of the data-source.
     */
    itemText: {
      type: [String, Object],
      required: true,
    },
    /**
     * Allows the parent component to pass a list of desired columns.
     */
    columnList: {
      type: [String, Array],
      required: false,
      default: null,
    },
    /**
     * Condition for data-source query.
     */
    dataSourceCondition: {
      type: String,
      required: false,
    },
    /**
     * Order for data-source query.
     */
    dataSourceOrder: {
      type: String,
      required: false,
    },
    /**
     * Return ARRAY of Ids
     */
    returnOnlyIds: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Defines the property name to use to disable selection of a row.
     */
    rowDisableSelectRules: {
      type: Array,
      required: false,
      default: null,
    },
    /**
     * Field message array.
     * NOTE: Messages will be rendered as hints.
     */
    messages: {
      type: Array,
      required: false,
      default: () => [],
    },
    iconAppend: {
      type: String,
      required: false,
      default: null,
    },
    /**
     * Display selection chips in column rather than in row.
     */
    displayChipsColumn: {
      type: Boolean,
      required: false,
      default: false,
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
     * Allows the parent component to pass a list of desired columns.
     */
     colorNameField: {
      type: String,
      required: false,
      default: null,
    },
  },
  data() {
    return {
      showWait: false,
      editMode: false,
      selectedItems: [],
      selectedItemsList: [],
      headers: [],
      rows: [],
      pkName: "",
      tableTitleKey: "",
    };
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
    tableColumns() {
      let localThis: any = this as any;
      let cols: string = localThis.columnList;

      // Calculate columnList
      if (localThis.columnList && typeof localThis.columnList == "object") {
        cols = localThis.columnList.reduce((prev: string, curr: string) => {
          return prev.length == 0 ? curr : prev + "," + curr;
        }, "");
      }
      return cols;
    },

    requiredTextFieldRule() {
      return [(v: any) => !!v || translate("error.required-field")];
    },
  },
  watch: {
    dataSource(to: string, form: string) {
      //Reload data if viewName is updated
      (this as any).getData();
    },
    dataSourceCondition(to: any, from: any) {
      //Reload data if datasource condition is updated
      (this as any).getData();
    },
    selectedItems(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;
      if (to.length >= 0) {
        localThis.selectedItemsList = localThis.selectedItems.map((element: any) => {
          if (element != undefined) {
            return element[localThis.itemText];
          }
        });

        if (localThis.returnOnlyIds) {
          let ArrayIds = localThis.selectedItems.map((element: any) => {
            let ArrayId: any = {};
            ArrayId[localThis.itemValue] = element[localThis.itemValue];
            return ArrayId;
          });

          localThis.updateValue(ArrayIds);
        } else {
          localThis.updateValue(localThis.selectedItems);
        }
      }
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "selectedItems",
    }),
    /**
     * Emit value changes to parent component.
     */
    updateValue(newVal: any) {
      (this as any).$emit("input", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;
      localThis.selectedItems.splice(0, localThis.selectedItems.length);
      localThis.updateValue(null);
    },
    /**
     * Get table data.
     */
    getData() {
      let localThis: any = this as any;
      localThis.showWait = true;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.dataSource,
        colList: localThis.tableColumns,
        whereCond: localThis.dataSourceCondition,
        orderStmt: localThis.dataSourceOrder,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        ignoreOfficeFilter:localThis.ignoreOfficeFilter
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.showWait = false;
          // Set table title
          localThis.tableTitleKey = res.crud_info.tableTitleKey;

          // Setup Headers
          localThis.headers = res.columns
            .filter((header: JrsHeader) => !header.is_hidden)
            .sort((a: JrsHeader, b: JrsHeader) => {
              let order_A: number = a.list_order ? a.list_order : 1000;
              let order_B: number = b.list_order ? b.list_order : 1000;
              return order_A < order_B ? -1 : 1;
            });

          // Setup primary key
          localThis.pkName = res.columns.find((header: JrsHeader) => header.is_pk).value;
          localThis.rows = res.table_data || [];
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    remove(id: any) {
      let localThis: any = this as any;
      const index = localThis.selectedItems.findIndex((item: any) => {
        return item[localThis.itemValue] == id;
      });
      localThis.selectedItems.splice(index, 1);
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getData().then((res: any) => {
      if (localThis.value && localThis.value.length > 0) {
        // localThis.value.forEach((ele: any) => {
        //   localThis.selectedItems.push(
        //     localThis.rows.find((item: any) => {
        //                                   return (
        //                         item[localThis.itemValue] ==
        //                         ele[localThis.itemValue]
        //                     );
        //     })
        //   );
        // });
        localThis.selectedItems = [];
        // Recover a filtered list of Rows that match with Value
        localThis.selectedItems = localThis.rows.filter((item: any) =>
          localThis.value.some(
            (ele: any) => item[localThis.itemValue] == ele[localThis.itemValue]
          )
        );
      }
    });
  },
});
</script>
<style>
.multiselect-list-flex-column .v-select__selections {
  flex-direction: column;
}
</style>
