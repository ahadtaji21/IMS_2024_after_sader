<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <h1 class="text-capitalize">
                        {{ $t("views.view-template-def.title") }}
                    </h1>
                    <p>{{ $t("views.view-template-def.page-descr") }}</p>
                </v-col>
            </v-row>
            <v-row>
                <v-col :cols="3">
                    <v-row>
                        <v-col>
                            <h2>Template type</h2>
                            <jrs-field
                                :field="templateTypeField"
                                v-model="currentTemplateTypeId"
                                @changeFullObject="SetSelectedObject"
                            ></jrs-field>
                        </v-col>
                    </v-row>
                    <v-row v-if="currentTemplateTypeObject">
                        <v-col>
                            <v-list two-line subheader>
                                <v-subheader>Placeholder codes</v-subheader>
                                <template
                                    v-for="(
                                        param, paramIndex
                                    ) in currentTemplateTypeObject.templateParams"
                                >
                                    <v-list-item :key="param.templateParamId">
                                        <v-list-item-content>
                                            <v-list-item-title
                                                >##{{
                                                    param.templateParamColumnName
                                                }}##</v-list-item-title
                                            >
                                            <v-list-item-subtitle>{{
                                                param.templateParamDescr
                                            }}</v-list-item-subtitle>
                                        </v-list-item-content>
                                    </v-list-item>
                                    <v-divider
                                        :key="`PARAM-DIVIDER_${paramIndex}`"
                                        v-if="
                                            paramIndex <
                                            currentTemplateTypeObject
                                                .templateParams.length -
                                                1
                                        "
                                    ></v-divider>
                                </template>
                            </v-list>
                        </v-col>
                    </v-row>
                </v-col>
                <v-divider vertical></v-divider>
                <v-col>
                    <div v-if="!showForm">
                        <v-row>
                            <v-col>
                                <h2>Template List</h2>
                                <jrs-simple-table
                                    viewName="IMS_VI_TEMPLATE"
                                    v-model="selectedTemplates"
                                    :selectable="true"
                                    :dataSourceCondition="`TEMPLATE_TYPE_ID = ${currentTemplateTypeId}`"
                                    :savePayload="{
                                        TEMPLATE_TYPE_ID: currentTemplateTypeId,
                                    }"
                                    :columnList="[
                                        'TEMPLATE_ID',
                                        'TEMPLATE_CODE',
                                        'TEMPLATE_TYPE_CODE',
                                        'TEMPLATE_TEXT',
                                    ]"
                                    hideNewBtn
                                    hideActions
                                    :nbrOfFormColumns="1"
                                    :overrideFormMaxWidth="'75em'"
                                >
                                    <template v-slot:simple-table-header="">
                                        <v-btn
                                            color="secondary lighten-2"
                                            class="grey--text text--darken-3"
                                            v-if="currentTemplateTypeId"
                                            @click="setupdAndShowForm()"
                                        >
                                            <v-icon
                                                >mdi-plus-circle-outline</v-icon
                                            >New
                                        </v-btn>
                                    </template>
                                    <template
                                        v-slot:simple-table-item-actions="{
                                            simpleItemRowItem,
                                        }"
                                    >
                                        <!-- BTN - OPEN TEMPLATE FORM -->
                                        <v-tooltip top>
                                            <template
                                                v-slot:activator="{ on, attrs }"
                                            >
                                                <v-btn
                                                    icon
                                                    @click="
                                                        setupdAndShowForm(
                                                            simpleItemRowItem
                                                        )
                                                    "
                                                    v-bind="attrs"
                                                    v-on="on"
                                                >
                                                    <v-icon small
                                                        >mdi-circle-edit-outline</v-icon
                                                    >
                                                </v-btn>
                                            </template>
                                            <span>{{
                                                $t("general.crud.tooltip-edit")
                                            }}</span>
                                        </v-tooltip>
                                    </template>
                                </jrs-simple-table>
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-col>
                                <jrs-simple-table
                                    viewName="IMS_VI_TEMPLATE_OFFICE"
                                    :dataSourceCondition="`TEMPLATE_ID = ${selectedTemplates[0].TEMPLATE_ID}`"
                                    :savePayload="{ TEMPLATE_ID: selectedTemplates[0].TEMPLATE_ID }"
                                    :columnList="['ID','TEMPLATE_CODE','OFFICE_ID','HR_OFFICE_CODE','HR_OFFICE_LEGAL_NAME','IS_DELETED']"
                                    :nbrOfFormColumns="1"
                                    v-if="selectedTemplates.length > 0"
                                ></jrs-simple-table>
                            </v-col>
                        </v-row>
                    </div>

                    <jrs-form
                        v-else
                        :formData="formData"
                        :formFields="formFields"
                        :nbrOfColumns="1"
                        :overrideMaxWidth="'25em'"
                    >
                        <template v-slot:form-actions="{ validateFunc }">
                            <v-btn
                                text
                                color="secondary darken-1"
                                @click="closeForm()"
                                >X {{ $t("general.close") }}</v-btn
                            >

                            <v-btn
                                color="primary"
                                :disabled="isSaving"
                                @click="
                                    saveTemplateFunc(
                                        formData,
                                        formData.TEMPLATE_ID ? 1 : 0,
                                        validateFunc
                                    )
                                "
                                class="ma-2"
                                >{{ $t("general.save") }}</v-btn
                            >
                        </template>
                    </jrs-form>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from "vue";
    import { mapActions, mapGetters } from "vuex";
    import JrsField from "../components/JrsField.vue";
    import JrsSimpleTable from "../components/JrsSimpleTable.vue";
    import JrsForm from "../components/JrsForm.vue";
    import { fieldType, JrsFormFieldSelect } from "../models/JrsForm";
    import FormMixin from "../mixins/FormMixin";
    import { translate } from "../i18n";
    import {
        GenericSqlPayload,
        GenericSqlSelectPayload,
        SqlActionType,
    } from "../axiosapi/api";

    export default Vue.extend({
        components: {
            "jrs-field": JrsField,
            "jrs-simple-table": JrsSimpleTable,
            "jrs-form": JrsForm,
        },
        mixins: [FormMixin],
        data: () => ({
            templateTypes: [],
            currentTemplateTypeId: 0,
            currentTemplateTypeObject: null,
            formData: {},
            formFields: [],
            crudInfo: {},
            showForm: false,
            isSaving: false,
            selectedTemplates: [],
        }),
        computed: {
            ...mapGetters(["getUserUid", "getCurrentOffice"]),
            templateTypeField() {
                const localThis: any = this as any;
                const fieldConfig: JrsFormFieldSelect = {
                    name: "templateType",
                    type: fieldType["select"],
                    label: translate("datasource.ims.template.type").toString(),
                    selectItems: localThis.templateTypes,
                    itemText: "templateTypeDescr",
                    itemKey: "templateTypeId",
                    lookupName: "tmpLookup",
                    // sendOnlyKey: false
                };
                return fieldConfig;
            },
        },
        methods: {
            ...mapActions({
                showNewSnackbar: "showNewSnackbar",
            }),
            ...mapActions("apiHandler", {
                getTemplateTypes: "getTemplateTypes",
                getJRSTableStructure: "getJRSTableStructure",
                execSPCall: "execSPCall",
            }),
            /**
             * Load and set template types.
             */
            refreshTemplateTypes() {
                const localThis: any = this as any;
                localThis
                    .getTemplateTypes()
                    .then((res: any) => {
                        localThis.templateTypes = [
                            { templateTypeDescr: "N/A", templateTypeId: null },
                            ...res,
                        ];
                    })
                    .catch((err: any) => {
                        localThis.showNewSnackbar({ typeName: "error", text: err });
                    });
            },
            SetSelectedObject({ objectValue }: any) {
                const localThis: any = this as any;
                localThis.currentTemplateTypeObject = objectValue;
            },
            /**
             * Load Form structure.
             */
            getFormStruct(view: string) {
                let localThis: any = this as any;
                let selectPayload: GenericSqlSelectPayload = {
                    viewName: view,
                    colList: null,
                    whereCond: null,
                    orderStmt: null,
                };
                localThis
                    .getJRSTableStructure({ genericSqlPayload: selectPayload })
                    .then((res: any) => {
                        // Setup CRUD info
                        localThis.crudInfo = res.crud_info;

                        // Setup Form Fields
                        let tmpFormFileds: Array<any> = res.columns
                            .filter(
                                (header: any) =>
                                    !header.hide_in_form &&
                                    header.column_name != "TEMPLATE_TYPE_ID"
                            )
                            .map((field: any) =>
                                localThis.parseJrsFormField(field)
                            );

                        localThis.formFields = localThis.setupSelectFields(
                            tmpFormFileds
                        );
                    });
            },
            /**
             * Data to set in the form for updating data.
             */
            setupdAndShowForm(formData: any) {
                let localThis: any = this as any;
                localThis.formData = formData ? formData : {};
                localThis.showForm = true;
            },
            /**
             * Close the form
             */
            closeForm(){
                let localThis: any = this as any;
                localThis.showForm = false;
                localThis.selectedTemplates = [];
            },
            /**
             * Seve template data.
             * @param {string} saveData Template data to save
             * @param {SqlActionType} actionType Tipe of SQL action (Create|Update)
             * @param {Function} formValidateFunc Function to validate form data
             * @param {Function} formResetFunc Function to reset the form data
             */
            saveTemplateFunc(
                saveData: any,
                actionType: SqlActionType,
                formValidateFunc: Function,
                formResetFunc?: Function
            ) {
                //Check form validity
                if (!formValidateFunc()) {
                    return;
                }
                let localThis: any = this as any;
                localThis.isSaving = true;
                let spName: string =
                    actionType == SqlActionType.NUMBER_0
                        ? localThis.crudInfo.create_sp
                        : localThis.crudInfo.update_sp;

                //Add external data to payload
                saveData = {
                    external_data: {
                        TEMPLATE_TYPE_ID: localThis.currentTemplateTypeId,
                    },
                    rows: saveData,
                };

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
                            typeName: "success",
                            text: res.message,
                        }); // Feedback to user
                        if (formResetFunc) {
                            formResetFunc(); // Reset Form
                        }
                        // Close form
                        localThis.closeForm();
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
        },
        async mounted() {
            const localThis: any = this as any;
            localThis.refreshTemplateTypes();
            localThis.getFormStruct("IMS_VI_TEMPLATE");
        },
    });
</script>

<style scoped>
</style>