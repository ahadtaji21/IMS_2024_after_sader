<template>
    <div class="d-flex">
        <v-dialog
            v-model="editMode"
            persistent
            retain-focus
            :overlay="false"
            max-width="60em"
            transition="dialog-transition"
        >
            <template v-slot:activator="{on}">
                <v-text-field
                    v-on="on"
                    @click="editMode = !editMode"
                    :label="label"
                    v-model="itemDescription"
                    :hint ="hint ? hint : ''"
                    :persistent-hint ="hint ? true : false"
                    readonly
                    :required="required"
                    :rules="required ? requiredTextFieldRule : []"
                    prepend-icon="mdi-magnify"
                    :messages="messages"
                    :disabled="isDisabled"
                >
                    <template v-slot:append-outer>
                        <v-icon @click="clearField()" :disabled="isDisabled">mdi-close</v-icon>
                    </template>
                </v-text-field>
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
                            :rowDisableSelectRules="rowDisableSelectRules"
                        ></jrs-table>
                </v-card-text>
                <v-card-actions>
                    <v-btn
                        color="secondary darken-1"
                        text
                        @click="editMode = false"
                    >X {{ $t('general.close') }}</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import { mapActions, mapGetters } from "vuex";
    import { GenericSqlSelectPayload } from "../axiosapi/api";
    import { JrsHeader } from "../models/JrsTable";
    import JrsTable from "../components/JrsTable.vue";
    import { formatDateTime, translate } from '../i18n';

    export default Vue.extend({
        components:{
            'jrs-table': JrsTable
        },
        props:{
            value: {
                type: [Number,String],
                required: false
            },
            label: {
                type: String,
                required: false,
                default: null
            },
            hint: {
                type: String,
                required: false,
                default: null
            },
            required: {
                type: Boolean,
                required: false,
                default: false
            },
            /**
             * Title to display on table header.
             */
            title: {
                type: String,
                required: false
            },
            /**
             * Name of the sql data-source to query.
             */
            dataSource:{
                type: String,
                required: true
            },
            /**
             * Name of the value column of the data-source.
             */
            itemValue:{
                type: String,
                required: true
            },
            /**
             * Name of the text column of the data-source.
             */
            itemText:{
                type: String,
                required: true
            },        
            /**
             * Allows the parent component to pass a list of desired columns.
             */
            columnList: {
                type: [String , Array],
                required: false,
                default: null
            },
            /**
             * Condition for data-source query.
             */
            dataSourceCondition:{
                type: String,
                required: false
            },
            /**
             * Order for data-source query.
             */
            dataSourceOrder:{
                type: String,
                required: false
            },
            /**
             * Defines the property name to use to disable selection of a row.
             */
            rowDisableSelectRules:{
                type: Array,
                required: false,
                default: undefined,
            },
            /**
             * Field message array.
             * NOTE: Messages will be rendered as hints.
             */
            messages:{
                type: Array,
                required: false,
                default: () => []
            },
            isDisabled:{
                type: Boolean,
                required: false,
                default: false
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
        data(){
            return {
                editMode: false,
                itemDescription: '',
                selectedItems: [],
                headers:[],
                rows:[],
                pkName: '',
                tableTitleKey: ''
            }
        },
        computed:{
            ...mapGetters({
            getCurrentOffice: 'getCurrentOffice'
            }),
            tableColumns(){
                let localThis:any = this as any;
                let cols:string = localThis.columnList;

                // Calculate columnList
                if(localThis.columnList && typeof localThis.columnList == 'object'){
                    cols = localThis.columnList.reduce( (prev:string, curr:string) => {
                    return prev.length == 0 ? curr : prev + ',' + curr;
                    }, '');
                }
                return cols;
            },
            requiredTextFieldRule(){
                return [ (v:any) => !!v || translate('error.required-field')]
            }
        },
        watch:{
            async dataSource(to:string, form:string){
                //Reload data if viewName is updated
                await (this as any).getData();
            },
            async dataSourceCondition(to:any, from:any){
                //Reload data if datasource condition is updated
                if(to != from){
                    try {
                        await (this as any).getData();
                        console.log('FINISGED WATCH CALL')
                    } catch (error) {
                        this.showNewSnackbar({ typeName: "error", text: error });
                    }
                }
            },
            selectedItems( to:Array<any>, from:Array<any> ){
                let localThis:any = this as any;
                if(to.length > 0){
                    localThis.updateValue(to[0][localThis.itemValue]);
                    localThis.itemDescription = to[0][localThis.itemText];
                    localThis.editMode = false;
                }
            },
            value(to:any){
                let localThis:any = this as any;
                if(!to){
                    localThis.itemDescription = '';
                    localThis.selectedItems.splice(0, localThis.selectedItems.length);
                }
            }
        },
        methods: {
            ...mapActions({
                showNewSnackbar: "showNewSnackbar"
            }),
            ...mapActions("apiHandler", {
                getGenericSelect: "getGenericSelect",
                execSPCall: "execSPCall"
            }),
            /**
             * Emit value changes to parent component.
             */
            updateValue(newVal: number) {
                (this as any).$emit("input", newVal);
            },
            /**
             * Clear field data.
             */
            clearField(){
                let localThis:any = this as any;
                localThis.itemDescription = '';
                localThis.selectedItems.splice(0, localThis.selectedItems.length);
                localThis.updateValue(null);
            },
            /**
             * Get table data.
             */
            async getData() {
                let localThis: any = this as any;
                let selectPayload: GenericSqlSelectPayload = {
                    viewName: localThis.dataSource,
                    colList: localThis.tableColumns,
                    whereCond: localThis.dataSourceCondition,
                    orderStmt: localThis.dataSourceOrder,
                    officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
                    ignoreOfficeFilter: localThis.ignoreOfficeFilter,
                };

                try {
                    const res = await localThis.getGenericSelect({ genericSqlPayload: selectPayload });

                    // Set table title
                    localThis.tableTitleKey = res.crud_info.tableTitleKey;

                    // Setup Headers
                    localThis.headers = res.columns.filter(
                        (header: JrsHeader) => !header.is_hidden
                    ).sort(
                        (a:JrsHeader, b:JrsHeader) => {
                            let order_A:number = a.list_order ? a.list_order : 1000;
                            let order_B:number = b.list_order ? b.list_order : 1000;
                            return order_A < order_B ? -1 : 1;
                    });

                    // Setup primary key
                    localThis.pkName = res.columns.find(
                        (header: JrsHeader) => header.is_pk
                    ).value;
                    localThis.rows = res.table_data || [];
                    return res;

                } catch (err) {
                    localThis.showNewSnackbar({ typeName: "error", text: err });
                }
            },
        },
        async mounted() {
            let localThis: any = this as any;

            try {
                const res = await localThis.getData();
                if(localThis.value){
                    let selected:any = localThis.rows.find( (item:any) => {
                        // case insensitive check for GUID
                        if(
                            typeof item[localThis.itemValue] == "string" &&
                            typeof localThis.value == "string"
                        ){
                            return item[localThis.itemValue].toUpperCase() == localThis.value.toUpperCase();
                        }
                        return item[localThis.itemValue] == localThis.value;
                    });
                    if(selected != undefined){
                    localThis.selectedItems.push(selected);
                    }
                }
            } catch (error) {
                localThis.showNewSnackbar({ typeName: "error", text: error });
            }
        }
    })
</script>
