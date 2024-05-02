import { GenericSqlSelectPayload } from "../axiosapi/api";
import { JrsHeader } from "../models/JrsTable";
import { mapActions} from "vuex";
import FormMixin from "./FormMixin";

export default {
    mixins: [FormMixin],
    methods:{
        ...mapActions("apiHandler", {
          getGenericSelect: "getGenericSelect"
        }),
        /**
         * Returns information regardin the table strucure and it's data.
         * @param {string} tableName Name of the sql VIEW/TABLE for which data must be recovered.
         * @param {number} officeId reference of the active office. Used to filter table data.
         * @param {Array<any>} fieldDatasourceConditions Array of conditions for the datasources of "select-like" fields in the form.
         * @param {any} options Configuraiton options.
         * @returns {Promise<any>} Promise that resolves to an Object in the form { crud_info:Object, columns:Array<any>, table_data:Array<any>, item_count:Number}.
         */
        getTableData(
            tableName:string,
            officeId:number,
            fieldDatasourceConditions:Array<any>,
            queryOptions:any={ colList:null, whereCond:null, orderStmt:null, itemNumber:null, skipNumber:null }
            ):Promise<any> {
            let localThis: any = this as any;

            // let selectPayload: GenericSqlSelectPayload = {
            // viewName: tableName,
            // colList: colList,
            // whereCond: whereCond,
            // orderStmt: localThis.orderCondition,
            // itemNumber: !localThis.clientPagination ? localThis.tableOptions.itemsPerPage : undefined,
            // skipNumber: !localThis.clientPagination ? ((localThis.tableOptions.page == 0 ? 0 :localThis.tableOptions.page-1) * localThis.tableOptions.itemsPerPage) : undefined,
            // officeId: localThis.getCurrentOffice.HR_OFFICE_ID
            // };
            let selectPayload: GenericSqlSelectPayload = {
                viewName: tableName,
                // colList: null,
                // whereCond: null,
                // orderStmt: null,
                // itemNumber: null,
                // skipNumber: null,
                officeId: officeId
            };
            selectPayload = Object.assign(selectPayload, queryOptions);

    
            return this.getGenericSelect({ genericSqlPayload: selectPayload })
            .then((res: any) => {
                // Setup Headers
                let jrsHeaders = res.columns.filter(
                    (header: JrsHeader) => !header.is_hidden
                ).sort(
                (a:JrsHeader, b:JrsHeader) => {
                    let order_A:number = a.list_order ? a.list_order : 1000;
                    let order_B:number = b.list_order ? b.list_order : 1000;
                    return order_A < order_B ? -1 : 1;
                });
    
                // if(!localThis.hideActions || !!this.$scopedSlot['simple-table-item-actions']){
                //     jrsHeaders.push({ text: "Actions", value: "action" });
                // }
                // localThis.pkName = res.columns.find(
                // (header: JrsHeader) => header.is_pk
                // ).value;

                // Setup Rows
                let rows = res.table_data || [];
    
                // Setup FormFields
                let tmpFormFileds:Array<any> = res.columns
                    .filter((header: any) => !header.hide_in_form)
                    .map((field:any) => localThis.parseJrsFormField(field));
                let formFields = localThis.setupSelectFields(tmpFormFileds, officeId, fieldDatasourceConditions);
                
                // Return data
                return { jrsHeaders, rows, formFields };
            })
            .catch((err: any) => {
                // localThis.progress = false;
                // localThis.showNewSnackbar({ typeName: "error", text: err });
                console.log('error: ', err);
            });
        }
    }
}