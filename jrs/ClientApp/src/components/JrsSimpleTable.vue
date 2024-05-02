<template>
  <div>
    <div class="text-center" v-if="progress" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <jrs-table
      :title="title || $t(tableTitleKey) || viewName"
      :headers="headers"
      v-if="progress == false"
      :rows="rows"
      :iconActions="iconActions"
      :pkName="pkName"
      v-model="selectedRows"
      :showSearchbar="showSearchField"
      :multiSelect="multiSelect"
      :selectable="selectable"
      @optionsChange="updateOptions"
      @filterChange="updateWhereCondition"
      :serverItemsLength="rowNbr"
      :isLoading="tableIsLoading"
      :hiddenColumns="hiddenColumns"
      :selectOnRowClick="selectOnRowClick"
      @rowClick="rowClick"
      :selectFirstRowDeafult="selectFirstRowDeafult"
    >
      <!-- TABLE HEADER -->
        <template v-slot:tableHeader>
            <v-spacer></v-spacer>
            <!-- DOWNLOAD DATA -->
            <v-tooltip top v-if="showDataDownloadBtn">
                <template v-slot:activator="{ on }">
                    <v-btn color="secondary lighten-2"
                           class="grey--text text--darken-3 mx-1"
                           small
                           v-on="on"
                           @click="downloadTableData">
                        <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                </template>
                <span>{{ $t("general.download-table-data") }}</span>
            </v-tooltip>

            <!-- Excel number 2 customized-->
            <v-tooltip top v-if="showSPExcelBtn">
                <template v-slot:activator="{ on }">
                    <v-btn color="secondary lighten-4"
                           class="grey--text text--darken-3 mx-1"
                           small
                           v-on="on"
                           @click="downloadExcelFromSP">
                        <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                </template>
                <span>{{ $t("general.download-table-data_det") }}</span>
            </v-tooltip>



            <!-- NEW FORM -->
            <jrs-modal-form v-if="crudInfo.create_sp && formFields && formFields.length > 0"
                            formTitle="New"
                            formSubtitle="Define item."
                            :formFields="formFields"
                            :formData="newFormData"
                            :nbrOfColumns="nbrOfFormColumns"
                            :scrollable="true"
                            :overrideMaxWidth="overrideFormMaxWidth"
                            :ignoreOfficeFilter="ignoreOfficeFilterTotally">
                <template v-slot:activation>
                    <v-btn color="secondary lighten-2"
                           class="grey--text text--darken-3"
                           v-if="!hideNewBtn && !readonly">
                        <v-icon>mdi-plus-circle-outline</v-icon>New
                    </v-btn>
                </template>
                <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc }">
                    <v-btn color="secondary darken-1"
                           class="mt-2 mr-1"
                           text
                           @click="
                closeModalFunc();
                resetNewFormData();
                refreshData();
              ">X {{ $t("general.close") }}</v-btn>
                    <v-btn color="primary"
                           class="mt-2 mx-1"
                           @click="saveData(newFormData, 0, closeModalFunc, validateFunc)">Save</v-btn>
                    <!-- <v-btn color="primary" class="mt-2 mx-1" @click="resetFunc()">Reset</v-btn> -->
                </template>
            </jrs-modal-form>
            <v-btn color="secondary lighten-2"
                   class="grey--text text--darken-3"
                   v-if="showActionBtn"
                   @click="doAction()">
                <v-icon>mdi-cog</v-icon>{{ actionBtnDesc }}
            </v-btn>

            <!-- HEADER SLOT -->
            <slot name="simple-table-header"
                  v-bind="{
            refreshData,
            whereCondition: computedDatasourceCondition,
            order: computerOrderDataSource,
          }"></slot>

            <!-- SUB-OFFICE FILTER -->
            <v-select v-model="selectedSubOfficeId"
                      v-if="showSubOfficeFilter"
                      :items="subOfficeList"
                      item-text="hrOfficeCode"
                      item-value="hrOfficeId"
                      outlined
                      dense
                      hide-details
                      @change="refreshData()"
                      class="white"
                      style="max-width: 150px"></v-select>
        </template>
      <!--ROW ACTIONS-->
      <template v-slot:itemActions="{ rowItem }">
        <!-- READONLY ICON -->
        <jrs-modal-form
          :formFields="formFields"
          :formData="rowItem"
          :nbrOfColumns="nbrOfFormColumns"
          :readonly="readonly"
          v-if="readonly && !hideActions"
          :overrideMaxWidth="overrideFormMaxWidth"
          :dialogExt="rowClicked"
          :pkname="pkName"
          :idx="idx"
          :key="rndDialog"
          :ignoreOfficeFilter="ignoreOfficeFilterTotally"
        >
          <template v-slot:activation>
            <v-icon small @click="showReadOnly">mdi-eye-outline</v-icon>
          </template>
        </jrs-modal-form>

        <!-- UPDATE ICON -->
        <jrs-modal-form
          :formFields="formFields"
          :formData="rowItem"
          :nbrOfColumns="nbrOfFormColumns"
          v-if="crudInfo.update_sp && !readonly && !hideActions"
          :overrideMaxWidth="overrideFormMaxWidth"
          :dialogExt="rowClicked"
          :pkname="pkName"
          :idx="idx"
          :key="rndDialog"
          :ignoreOfficeFilter="ignoreOfficeFilterTotally"
        >
          <template v-slot:activation>
            <v-icon small>mdi-circle-edit-outline</v-icon>
          </template>
          <!-- <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc, resetFunc }"> -->
          <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc }">
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="
                closeModalFunc();
                refreshData();
              "
              >X {{ $t("general.close") }}</v-btn
            >
            <v-btn
              color="primary"
              class="mt-2 mx-1"
              :disabled="isSaving"
              @click="saveData(rowItem, 1, closeModalFunc, validateFunc)"
              >Save</v-btn
            >

            <!-- <v-btn color="primary" class="mt-2 mx-1" @click="resetFunc()">Reset</v-btn> -->
          </template>
        </jrs-modal-form>

        <!-- DELETE ICON -->
        <v-icon
          small
          @click="toggleDeleteDialog(rowItem)"
          v-if="crudInfo.delete_sp && !readonly && !hideActions"
          >mdi-delete</v-icon
        >

        <!-- SLOT ACTIONS -->
        <slot
          name="simple-table-item-actions"
          v-bind="{ simpleItemRowItem: rowItem, refreshData }"
        ></slot>
      </template>
      <!-- <template v-slot:itemActions="{rowItem}">
            <v-btn color="warning" @click="test(rowItem)">test</v-btn>
      </template>-->
    </jrs-table>

    <!--- DELETE DIALOG -->
    <v-dialog
      v-model="deleteDialog"
      persistent
      retain-focus
      :overlay="false"
      max-width="45em"
      transition="dialog-transition"
    >
      <v-card>
        <v-card-title primary-title class="text-capitalize">{{
          $t("general.crud.item-deletion")
        }}</v-card-title>
        <v-card-text class="capital-first-letter">{{
          $t("general.crud.item-delete-confirm")
        }}</v-card-text>
        <v-card-actions>
          <v-btn text color="secondary darken-1" @click="toggleDeleteDialog()"
            >X {{ $t("general.close") }}</v-btn
          >
          <v-btn color="primary" @click="deleteData()">{{ $t("general.delete") }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsTable from "../components/JrsTable.vue";
import JrsModalForm from "../components/JrsModalForm.vue";
import FormMixin from "../mixins/FormMixin";
import UtilMix from "../mixins/UtilMix";
import { JrsHeader } from "../models/JrsTable";
import { IapiBlobStorageDeleteFile } from "../store/apiHandler";
import { JrsFormField, JrsFormFieldSelect, JrsFormFieldSearch } from "../models/JrsForm";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  HrOffice,
} from "../axiosapi/api";
import { translate } from "../i18n";

export default Vue.extend({
  components: {
    "jrs-table": JrsTable,
    "jrs-modal-form": JrsModalForm,
  },
  props: {
    /**
     * Model to be passed as v-model of v-data-table.
     */
    value: {
      type: [Array],
      required: false,
    },
    /**
     * Name of the view for which to recover table and form definition.
     */
    viewName: {
      type: String,
      required: true,
    },
    /**
     * Title to display on table header.
     */
    title: {
      type: String,
      required: false,
    },
    /**
     * Number of columns fo the form.
     */
    nbrOfFormColumns: {
      type: Number,
      required: false,
      default: 2,
    },
    /**
     * Is the table selectable.
     */
    selectable: {
      type: Boolean,
      required: false,
      default: false,
    },

    externalDialogOpen: {
      type: Boolean,
      required: false,
      default: false,
    },

    /**
     * Is the table a multiselect.
     * WARNING: requires "selectable":true.
     */
    multiSelect: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Allows the parent component to pass a condition for data-source.
     */
    dataSourceCondition: {
      type: String,
      required: false,
      default: null,
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
     * Allows to disable "New" button.
     */
    hideNewBtn: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Allows to show an "Action" button.
     */
    showActionBtn: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Show the "Action" description.
     */
    actionBtnDesc: {
      type: String,
      required: false,
      default: "Action",
    },
    /**
     * Allows to hide "Search" Text field.
     */
    showSearchField: {
      type: Boolean,
      required: false,
      default: true,
    },
    /**
     * Allows to disable "Action" column.
     */
    hideActions: {
      type: Boolean,
      required: false,
      default: false,
    },

    /**
     * allow to edit the item clickin on a cell of the row
     */
    selectOnRowClick: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Data to be added to the saving payload.
     */
    savePayload: {
      type: Object,
      required: false,
      default: null,
    },
    /**
     * Array of conditions for the datasources of "select-like" fields.
     */
    fieldDatasourceConditions: {
      type: Array,
      required: false,
      default: null,
      validator: (val: Array<any>) => {
        let valid: boolean = true;
        val.forEach((field: any) => {
          if (!field.field_name || !field.field_condition) {
            valid = false;
          }
        });
        return valid;
      },
    },
    /**
     * Determines if the table should use client-side pagination
     */
    clientPagination: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Defines a list of hidden table columns.
     * The data will not be removed from the datasets, but not shown in the table.
     */
    hiddenColumns: {
      type: Array,
      required: false,
      default: () => [],
    },
    /**
     * Defines a list of hidden form fields.
     * The data will not be removed from the datasets, but not shown in the form.
     */
    hiddenFields: {
      type: Array,
      required: false,
      default: () => [],
    },
    /**
     * Define if table is in readonly mode.
     */
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Override form width.
     */
    overrideFormMaxWidth: {
      type: String,
      required: false,
      default: null,
    },
    /**
     * Enable action to download taba data as excel.
     */
    EnableDataDownload: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Enable action to download taba data as excel.
     */
    dataSourceOrder: {
      type: String,
      required: false,
      default: null,
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
     * Ignore office filter in data query.
     */
    ignoreOfficeFilterTotally: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Select default first item (if is selectable or multi)
     */
    selectFirstRowDeafult: {
      type: Boolean,
      required: false,
      default: false,
      },

      storedProcedureName: {
          type: String,
          required: false,
          default: null,
      },

  },
  mixins: [FormMixin, UtilMix],
  data() {
    return {
      readOnlyShown: false,
      idx: 0,
      rndDialog: 0,
      rowClicked: false,
      headers: [],
      rows: [],
      pkName: "",
      formFields: [],
      crudInfo: {},
      newFormData: {},
      document_info: null,
      iconActions: [],
      deleteDialog: false,
      deleteItem: {},
      tableTitleKey: "",
      selectedRows: [],
      tableOptions: {
        page: 0,
        itemsPerPage: 15,
        sortBy: [],
        sortDesc: [],
        groupBy: [],
        groupDesc: [],
      },
      filterCondition: "",
      rowNbr: undefined,
      firstOptionUpdateDone: false,
      orderCondition: "1",
      tableIsLoading: true,
      progress: false,
      isSaving: false,
      showSubOfficeFilter: true,
      subOfficeList: [] as HrOffice[],
      selectedSubOfficeId: 0,
    };
  },
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
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
    computerOrderDataSource() {
      let localThis: any = this as any;
      return !localThis.dataSourceOrder
        ? localThis.orderCondition
        : localThis.dataSourceOrder;
    },
    computeItemNumber() {
      let localThis: any = this as any;
      return !localThis.clientPagination
        ? localThis.tableOptions.itemsPerPage
        : undefined;
    },
    computedDatasourceCondition() {
      let localThis: any = this as any;
      let condition: string = "1=1";
      condition += localThis.dataSourceCondition
        ? " AND " + localThis.dataSourceCondition
        : "";
      condition += localThis.filterCondition ? " AND " + localThis.filterCondition : "";

      return condition;
    },
    // computedOrderCondition(){
    //   let localThis:any = this as any;
    //   let ordering:string = '1';
    //   //TODO: compose order by clause using the 2 order arrays
    //   console.log(localThis.tableOptions);
    //   console.log(localThis.tableOptions.sortBy);
    //   console.log(`Ordering array: ${localThis.tableOptions.sortBy.length} - ${localThis.tableOptions.sortBy}`);
    //   if(localThis.tableOptions.sortBy.length > 0){
    //     let colsOrder:Array<string> =  localThis.tableOptions.sortBy
    //       .map( (colName:string, index:number) => `${colName} ${localThis.tableOptions.sortDesc[index] ? 'DESC' : ''}` );
    //     ordering = colsOrder.join(' AND ');
    //   }
    //   console.log(`Clause: ${ordering}`);
    //   return ordering;
    // }
    showDataDownloadBtn() {
      const localThis: any = this as any;
      return localThis.EnableDataDownload || localThis.crudInfo.allowDataDownload;
      },

      showSPExcelBtn() {
          const localThis: any = this as any;
          return localThis.storedProcedureName;
      },
      //formattedRows() {
      //    console.log("d5lnaaaaaaaaaaaaaaa");
      //    return this.rows.map((row: Record<string, any>) => {
      //        for (const key in row) {
      //            if (Object.prototype.hasOwnProperty.call(row, key)) {
      //                const cellValue: any = row[key]; // Use 'any' type for cellValue

      //                // Check if the cellValue is a valid date (in any format)
      //                if (typeof cellValue === 'string' && this.isDate(cellValue)) {
      //                    // Format date as 'dd mmm yyyy'
      //                    const formattedDate = new Date(cellValue).toLocaleDateString('en-US', {
      //                        day: '2-digit',
      //                        month: 'short',
      //                        year: 'numeric',
      //                    });
      //                    row[key] = formattedDate;
      //                    console.log("n7ne jwa el if");
      //                   // console.log(row[key]);
      //                }
      //            }
      //        }
      //        console.log(row);
      //        return row;
      //    });
      //},

  },
  watch: {
    viewName(to: string, form: string) {
      //Reload data if viewName is updated
      (this as any).getData();
    },
    dataSourceCondition(to: string, from: string) {
      //Reload data if datasource condition is updated
      (this as any).getData();
    },
    selectedRows(to: any, from: any) {
      (this as any).updateValue(to);
    },
    rows(to: any, from: any) {
      this.$emit("getTableDataRecords", to, from, this.refreshData);
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
        generateExcelForTable: "generateExcelForTable",
        generateExcelFromSP: "generateExcelFromSP",

      deleteBlobStorageFile: "deleteBlobStorageFile",
      getOffice: "getOffice",
    }),
    rowClick(item: any) {
      let localThis: any = this as any;
      if (localThis.deleteDialog) return;
      if (localThis.externalDialogOpen) return;
      localThis.idx = item[localThis.pkName];
      localThis.rowClicked = true;
      localThis.rndDialog = Math.ceil(Math.random() * 1000);

      localThis.$emit("rowClick", item);
    },
    showReadOnly() {
      let localThis: any = this as any;
      localThis.readOnlyShown = true;
    },
    retrieveLookupByTyepCode(code: string) {
      return (this as any).$store.getters.getLookupsByTypeCode(code);
    },
    doAction() {
      let localThis: any = this as any;
      localThis.$emit("doAction");
    },
    async getData() {
      let localThis: any = this as any;
      const officeFilterId: number = localThis.selectedSubOfficeId;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewName,
        colList: localThis.tableColumns,
        whereCond: localThis.computedDatasourceCondition,
        orderStmt: !localThis.dataSourceOrder
          ? localThis.orderCondition
          : localThis.dataSourceOrder,
        itemNumber: !localThis.clientPagination
          ? localThis.tableOptions.itemsPerPage
          : undefined,
        skipNumber: !localThis.clientPagination
          ? (localThis.tableOptions.page == 0 ? 0 : localThis.tableOptions.page - 1) *
            localThis.tableOptions.itemsPerPage
          : undefined,
        officeId: officeFilterId,
        ignoreOfficeFilter: localThis.ignoreOfficeFilterTotally
          ? localThis.ignoreOfficeFilterTotally
          : localThis.ignoreOfficeFilter,
      };

      //active progress circle component
      localThis.progress = true;

      // Display loading bar
      localThis.tableIsLoading = true;
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //disable progress circle component
          localThis.progress = false;
          // Remove loading bar
          localThis.tableIsLoading = false;

          //Setup CRUD info
          localThis.crudInfo = res.crud_info;
          localThis.tableTitleKey = res.crud_info.tableTitleKey;

          //Manage pagination
          if (!localThis.clientPagination) {
            localThis.rowNbr = res.item_count;
          }

          //Setup Header
          localThis.headers = localThis.setupColumnHeaders(res.columns);

          // Identify PK column
          localThis.pkName = res.columns.find((header: JrsHeader) => header.is_pk).value;

          // Set form fields
          localThis.formFields = localThis.setupFormFields(res.columns);

          // Set table data
          localThis.rows = res.table_data || [];

          // Clear selected rows
          localThis.selectedRows = [];
        })
        .catch((err: any) => {
          localThis.progress = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    /**
     * Query DB for dataset qnd refresh rows without calculating again the headers and form definition
     */
    refreshData(forceFieldReload: boolean) {
      let localThis: any = this as any;
      const officeFilterId: number = localThis.selectedSubOfficeId;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewName,
        colList: localThis.tableColumns,
        whereCond: localThis.computedDatasourceCondition,
        orderStmt: localThis.computerOrderDataSource,
        itemNumber: localThis.computeItemNumber,
        skipNumber: !localThis.clientPagination
          ? (localThis.tableOptions.page == 0 ? 0 : localThis.tableOptions.page - 1) *
            localThis.tableOptions.itemsPerPage
          : undefined,
        officeId: officeFilterId,
        ignoreOfficeFilter: localThis.ignoreOfficeFilterTotally
          ? localThis.ignoreOfficeFilterTotally
          : localThis.ignoreOfficeFilter,
      };

      // Display loading bar
      localThis.tableIsLoading = true;

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Remove loading bar
          localThis.tableIsLoading = false;

          //Manage pagination
          if (!localThis.clientPagination) {
            localThis.rowNbr = res.item_count;
          }

          localThis.rows = res.table_data || [];

          // Refresh item list for combobox type fields it there are any
          // 1) Divide fields into comboFields and not.
          const isComboField: Function = (field: any) =>
            ["combobox", "combobox_multi"].includes(field.type);
          let [comboFields, notComboFields] = localThis.formFields.reduce(
            ([comboList, notComboList]: any, currentField: any) => {
              return isComboField(currentField)
                ? [[...comboList, currentField], notComboList]
                : [comboList, [...notComboList, currentField]];
            },
            [[], []]
          );
          // 2) If there are combofields, update the item lists
          if (comboFields.length > 0) {
            comboFields = localThis.setupSelectFields(
              comboFields,
              officeFilterId,
              localThis.fieldDatasourceConditions
            );

            // 3) Update FormFields
            localThis.formFields = [...comboFields, ...notComboFields];
          }
          localThis.$emit("setSubOfficeId", officeFilterId);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    async saveData(
      saveData: any,
      actionType: SqlActionType,
      formClosingFunc: Function,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      let localThis: any = this as any;

      //Check form validity
      if (!formValidateFunc()) {
        return;
      }

      // check if the form contains a component of type "DOCUMENT" and perform the save data + save docuement to blob storage
      let field = localThis.formFields.filter((field: any) => field.type == "document");

      if (field.length != 0) {
        let return_document_component = saveData["fileHolder_" + field[0].name];
        let id_document = saveData[field[0].name];

        if (return_document_component && return_document_component.File) {
          let office_code_config =
            field[0].additional_config.OfficeCode == undefined ||
            field[0].additional_config.OfficeCode == null
              ? null
              : field[0].additional_config.OfficeCode;

          const document_id = await localThis.UploadFileBlobAzureFromForm(
            return_document_component.ID,
            office_code_config,
            field[0].additional_config.FolderSaveDocument,
            field[0].additional_config.DocumentTypeId,
            return_document_component.File
          );

          saveData[field[0].name] = document_id;
        } else {
          saveData[field[0].name] = return_document_component
            ? return_document_component.ID
            : id_document;
        }
      }

      // The classic saving whitout field "DOCUEMNT"

      localThis.isSaving = true;
      let spName: string =
        actionType == SqlActionType.NUMBER_0
          ? localThis.crudInfo.create_sp
          : localThis.crudInfo.update_sp;

      //Add external data to payload
      if (localThis.savePayload) {
        saveData = {
          external_data: localThis.savePayload,
          rows: saveData,
        };
      }

      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: actionType,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: res.status,
            text: res.message,
          }); // Feedback to user
          formClosingFunc(); // Close ModalForm
          localThis.refreshData(); // Refresh table data

          // TODO: Fix application freeze on form reset. (error:150)
          // if (formResetFunc) {
          //   formResetFunc(); // Reset Form
          // }

          // Reset new Object if actionType CREATE
          if (actionType == SqlActionType.NUMBER_0) {
            // localThis.ResetObject(localThis.newFormData);
            localThis.resetNewFormData();
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },
    deleteData() {
      let localThis: any = this as any;
      //Check if there is data to delete
      if (localThis.deleteItem) {
        let spName: string = localThis.crudInfo.delete_sp;
        let deleteData: any = localThis.deleteItem;

        //Add external data to payload
        if (localThis.savePayload) {
          deleteData = {
            external_data: localThis.savePayload,
            rows: deleteData,
          };
        }

        let deletePayload: GenericSqlPayload = {
          spName: spName,
          actionType: SqlActionType.NUMBER_2,
          jsonPayload: JSON.stringify(deleteData),
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        };

        localThis
          .execSPCall(deletePayload)
          .then((res: any) => {
            localThis.showNewSnackbar({
              typeName: "success",
              text: res.message,
            }); // Feedback to user

            if (localThis.document_info) {
              let payload: IapiBlobStorageDeleteFile = {
                nameContainer: localThis.document_info.ST_CONTAINER_NAME,
                stPathRoot: localThis.document_info.ST_PATH_ROOT,
              };

              let id = localThis.document_info.ID;
              localThis
                .deleteBlobStorageFile(payload)
                .then((res: any) => {
                  let savePayload: GenericSqlPayload = {
                    spName: "ST_SP_DEL_STORAGE_DOCUMENT",
                    actionType: SqlActionType.NUMBER_1,
                    jsonPayload: JSON.stringify({
                      ID: id,
                    }),
                  };
                  localThis
                    .execSPCall(savePayload)
                    .then((res: any) => {
                      localThis.showNewSnackbar({ typeName: "info", text: res.message }); //Feedback to user
                    })
                    .catch((err: any) => {
                      localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
                    });
                })
                .catch((err: any) => {
                  localThis.showNewSnackbar({ typeName: "error", text: err });
                });
            }
            localThis.toggleDeleteDialog(); // Close Dialog
            localThis.refreshData(); // Refresh table data
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
          });
      }
    },
    async toggleDeleteDialog(deleteItem?: any) {
      (this as any).deleteDialog = !(this as any).deleteDialog;
      (this as any).document_info = null;
      if (deleteItem) {
        let field = (this as any).formFields.filter(
          (field: any) => field.type == "document"
        );
        if (field.length > 0) {
          if (deleteItem[field[0].name]) {
            (this as any).document_info = await (this as any).getInfoDocumentById(
              deleteItem[field[0].name]
            );
          }
        }
      }

      (this as any).deleteItem = deleteItem ? deleteItem : {};
    },
    updateValue(newVal: any) {
      // Emit update of v-model to parent component.
      (this as any).$emit("input", newVal);
    },
    /**
     * Update the tabel options.
     * @param opt - new options
     */
    updateOptions(opt: any) {
      let localThis: any = this as any;

      // Skip the first update
      if (localThis.firstOptionUpdateDone) {
        Object.assign(localThis.tableOptions, opt);

        // Ordering
        let ordering: string = "1";
        if (opt.sortBy.length > 0) {
          let colsOrder: Array<string> = opt.sortBy.map(
            (colName: string, index: number) =>
              `${colName} ${opt.sortDesc[index] ? "DESC" : ""}`
          );
          ordering = colsOrder.join(",");
        }
        localThis.orderCondition = ordering;

        //Refresh data-source
        if (!localThis.clientPagination) {
          localThis.refreshData();
        }
      } else {
        localThis.firstOptionUpdateDone = true;
      }
    },
    /**
     * Handle filter condition changes.
     * @param filter - new query condition
     */
    updateWhereCondition(filter: string) {
      let localThis: any = this as any;
      localThis.filterCondition = filter;

      //Refresh data-source
      if (!localThis.clientPagination) {
        localThis.refreshData();
      }
    },
    /**
     * Reset all properties of the given object.
     * @param targetObject - Object to reset
     */
    ResetObject(targetObject: any) {
      // Reset value of the form properties
      for (const property in targetObject) {
        targetObject[property] = null;
      }
    },
    resetNewFormData() {
      const localThis: any = this as any;
      localThis.newFormData = { ...{} };
    },
    downloadTableData() {
      const localThis: any = this as any;
      const fileName: string = "TMP_NAME";
      const fileExtension: string = ".xlsx";
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];

      const officeFilterId: number = localThis.selectedSubOfficeId;

      localThis.headers
        .filter(
          (h: JrsHeader) =>
            !h.is_hidden &&
            h.value != "action" &&
            !localThis.hiddenColumns.some((colName: string) => colName == h.column_name)
        )
        .forEach((h: JrsHeader) => {
          colList.push(h.column_name);
          if (h.translationtKey) {
            colLabels.push({
              columnName: h.column_name,
              columnLabel: translate(h.translationtKey).toString(),
            });
          }
        });

      localThis
        .generateExcelForTable({
          viewName: localThis.viewName,
          colList: colList.join(","),
          whereCond: localThis.computedDatasourceCondition,
          orderStmt: localThis.orderCondition,
          officeId: officeFilterId,
          colLabels,
        })
        .then((byteArray: any[]) => {
          const byteBufffer = localThis._base64ToArrayBuffer(byteArray, fileExtension);
          localThis.saveByteArray(
            fileName + fileExtension,
            byteBufffer,
            "vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          );
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
      },

      downloadExcelFromSP() {
          const localThis: any = this as any;
          const fileName: string = "TMP_NAME";
          const fileExtension: string = ".xlsx";
          //Set the parameters
          const officeId = localThis.getCurrentOffice.HR_OFFICE_ID;

          const PROCEDURE_PARAMETERS: Array<any> = [{ name: "HR_OFFICE_ID", value: officeId.toString() }];
          const PROCEDURE_NAME = localThis.storedProcedureName;

          localThis
              .generateExcelFromSP({
                  PROCEDURE_PARAMETERS,
                  PROCEDURE_NAME
              })
              .then((byteArray: any[]) => {
                  const byteBufffer = localThis._base64ToArrayBuffer(byteArray, fileExtension);
                  localThis.saveByteArray(
                      fileName + fileExtension,
                      byteBufffer,
                      "vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                  );
              })
              .catch((err: any) => {
                  localThis.showNewSnackbar({ typeName: "error", text: err });
              });
      },



    /**
     * Seutp table column headers.
     * @param {Array} jrsTableColumns List of columns.
     */
    setupColumnHeaders(jrsTableColumns: any[]) {
      const localThis: any = this as any;
      let columnHeaders: JrsHeader[] = [];
      if (jrsTableColumns.length == 0) {
        return;
      }

      // Map, filter and order columns
      columnHeaders = jrsTableColumns
        .map((col: any) => {
          const additional_config = col.additional_config
            ? JSON.parse(col.additional_config)
            : { prevent_truncation: false };
          let header: JrsHeader = { ...col };
          header.prevent_truncation = additional_config.prevent_truncation;
          return header;
        })
        .filter((header: JrsHeader) => !header.is_hidden)
        .sort((a: JrsHeader, b: JrsHeader) => {
          let order_A: number = a.list_order ? a.list_order : 1000;
          let order_B: number = b.list_order ? b.list_order : 1000;
          return order_A < order_B ? -1 : 1;
        });

      // Add Action column if needed
      if (!localThis.hideActions || !!this.$scopedSlots["simple-table-item-actions"]) {
        columnHeaders.push({ text: "Actions", value: "action" });
      }
      return columnHeaders;
    },
    /**
     * Seutp table column headers.
     * @param {Array} jrsTableColumns List of columns.
     */
    setupFormFields(jrsTableColumns: any[]) {
      const localThis: any = this as any;
      const officeFilterId: number = localThis.selectedSubOfficeId;
      let formFields: JrsFormField[] = [];
      if (jrsTableColumns.length == 0) {
        return;
      }

      // Parse form fields
      let tmpFormFileds: Array<any> = jrsTableColumns
        .filter(
          (header: any) =>
            !header.hide_in_form && !localThis.hiddenFields.includes(header.column_name)
        )
        .map((field: any) => localThis.parseJrsFormField(field));

      // Setup selectFields
      formFields = localThis.setupSelectFields(
        tmpFormFileds,
        officeFilterId,
        localThis.fieldDatasourceConditions
      );

      return formFields;
    },

    async getInfoDocumentById(id: any) {
      let localThis: any = this as any;
      const officeFilterId: number = localThis.selectedSubOfficeId;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "ST_DOCUMENT",
        colList: "ID,ST_PATH_ROOT,ST_CONTAINER_NAME",
        whereCond: "ID = " + id,
        officeId: officeFilterId,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          return res.table_data[0];
        })
        .catch((err: any) => {
          localThis.progress = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    async getSubOfficeList(officeId: number) {
      try {
        const office: HrOffice = await this.getOffice({
          officeId,
          includeDescendants: true,
        });
        const descendants: HrOffice[] = office.flatDescendantOfficeList || [];
        this.subOfficeList = [office, ...descendants];
      } catch (error) {
        this.showNewSnackbar({ typeName: "error", text: error });
      }
      },
      // Check if a value is a valid date
      //isDate(value: string) {
      //    if (isNaN(new Date(value).getDate())) {
      //        // If the value is not a valid date, return false
      //        console.log(value);
      //        console.log(isNaN(new Date(value).getDate()));
      //        return false;
      //    }
      //    console.log(value);
      //    console.log(isNaN(new Date(value).getDate()));

      //    return true;
      //},
  },
  async mounted() {
    let localThis: any = this as any;
    const currentOfficeId = localThis.getCurrentOffice.HR_OFFICE_ID;
    localThis.selectedSubOfficeId = currentOfficeId;
    await localThis.getSubOfficeList(currentOfficeId);
    await localThis.getData();
  },
});
</script>
<style scoped></style>

