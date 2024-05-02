<template>
    <v-content>
        <v-container fluid fill-height>
            <!-- PAGE TITLE -->
            <v-row>
                <v-col>
                    <h1 class="text-capitalize">
                        {{ $t("views.view-template-doc-generator.title") }}
                    </h1>
                    <p>
                        {{ $t("views.view-template-doc-generator.page-descr") }}
                    </p>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <jrs-field
                        :field="templateSelectField"
                        v-model="selectedTemplate"
                        v-if="templateSelectField"
                        key="F1"
                    ></jrs-field>
                </v-col>
                <v-col>
                    <jrs-field
                        :field="pkSelectField"
                        v-model="selectedPK"
                        v-if="pkSelectField"
                        key="F2"
                    ></jrs-field>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <jrs-field
                        :field="fileNameField"
                        v-model="downloadFileName"
                        v-if="fileNameField"
                        key="F3"
                    ></jrs-field>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <v-btn
                        color="primary"
                        class="mt-2 mx-1"
                        :disabled="!selectedPK || !selectedTemplate"
                        @click="downloadDoc()"
                    >
                        <v-icon>mdi-file-download-outline</v-icon>DOWNLOAD
                    </v-btn>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from "vue";
    import { mapActions } from "vuex";
    import JrsSimpleTable from "../components/JrsSimpleTable.vue";
    import JrsField from "../components/JrsField.vue";
    import FormMixin from "../mixins/FormMixin";
    import UtilMix from "../mixins/UtilMix";
    import { translate } from "../i18n";

    export default Vue.extend({
        components: {
            // "jrs-simple-table": JrsSimpleTable,
            "jrs-field": JrsField,
        },
        mixins: [FormMixin, UtilMix],
        data: () => ({
            templateSelectField: null,
            selectedTemplate: 0,
            pkSelectField: null,
            selectedPK: null,
            fileNameField: null,
            downloadFileName: null,
            templateType: null,
            defualtPKSelectField:{
                column_name: "HR_BIODATA_USER_UID",
                type: "search_table",
                required: true,
                translationtKey: "datasource.hrm.biodata.title",
                select_items_datasource: "HR_VI_USER_SELECTOR",
                itemKey: "IMS_USER_UID",
                itemText: "HR_BIODATA_FULL_NAME",
                list_order: 2,
                form_order: 2,
                readonly:true
            }
        }),
        watch: {
            selectedTemplate(to: any, from: any) {
                const localThis: any = this as any;
                if(to){
                    localThis.getTemplateTypeOptions(to);
                }else{
                    localThis.pkSelectField = localThis.parseJrsFormField(localThis.defualtPKSelectField);
                    localThis.pkSelectField.label = "Data Item";
                }
            },
        },
        methods: {
            ...mapActions({
                showNewSnackbar: "showNewSnackbar",
            }),

            ...mapActions("apiHandler", [
                "downloadDocxFromTemplate",
                "getTemplateTypeForTemplate",
            ]),
            /**
             * Get the template type for the specified templateId.
             * @param {Number} templateId ID fo the template to get type for.
             */
            async getTemplateTypeOptions(templateId: number) {
                const localThis: any = this as any;
                try {
                    localThis.templateType = await localThis.getTemplateTypeForTemplate(templateId);
                    localThis.setupPKSelectField();
                } catch (err) {
                    localThis.showNewSnackbar({ typeName: "error", text: err });
                }
            },
            setupPKSelectField() {
                const localThis: any = this as any;
                if(!localThis.templateType){
                    console.log("No template type configured...");
                }
                localThis.pkSelectField = localThis.parseJrsFormField({
                    column_name: "PK_SELECTOR",
                    type: "search_table",
                    required: true,
                    translationtKey: localThis.templateType.fieldTranslationKey,
                    select_items_datasource: localThis.templateType.selectItemsDatasource,
                    itemKey: localThis.templateType.selectItemKey,
                    itemText: localThis.templateType.selectItemText,
                    list_order: 2,
                    form_order: 2,
                    readonly:false,
                });
                localThis.pkSelectField.label = localThis.$t(localThis.templateType.fieldTranslationKey).toString() || "Data Item";
                console.log(`KEY: ${localThis.templateType.fieldTranslationKey}`);
            },
            downloadDoc(fileName: string, fileExtension: string) {
                const localThis: any = this as any;
                if (!localThis.selectedPK) {
                    // ShowSnackbar
                }

                fileName =
                    fileName || localThis.downloadFileName || "DOWNLOADED_TEMPLATE";
                fileExtension = fileExtension || ".docx";
                localThis.showNewSnackbar({
                    typeName: "info",
                    text: "Preparing requested document.",
                });
                let condition:string|undefined = undefined;
                if(localThis.templateType.defaultDataQueryCondition){
                    condition = localThis.templateType.defaultDataQueryCondition.split("##PK##").join(localThis.selectedPK);
                }
                localThis
                    .downloadDocxFromTemplate({
                        templateId: localThis.selectedTemplate,
                        queryCondition: condition,
                    })
                    .then((byteArray: any[]) => {
                        const byteBufffer = localThis._base64ToArrayBuffer(
                            byteArray,
                            fileExtension
                        );
                        localThis.saveByteArray(
                            fileName + fileExtension,
                            byteBufffer
                        );
                        const msg:string = localThis.$t("info-message.downloading-document").toString();
                        return { message: msg };
                    })
                    .then((res: any) => {
                        localThis.showNewSnackbar({
                            typeName: "success",
                            text: res.message,
                        });
                    })
                    .catch((err: any) => {
                        localThis.showNewSnackbar({ typeName: "error", text: err });
                    });
            },
        },
        mounted() {
            const localThis: any = this as any;
            // Setup fields
            localThis.templateSelectField = localThis.parseJrsFormField({
                column_name: "TEMPLATE_ID",
                type: "search_table",
                required: true,
                translationtKey: "datasource.ims.template.title",
                select_items_datasource: "IMS_VI_TEMPLATE",
                itemKey: "TEMPLATE_ID",
                itemText: "TEMPLATE_CODE",
                list_order: 1,
                form_order: 1,
            });
            localThis.templateSelectField.label = "Template";

            localThis.pkSelectField = localThis.parseJrsFormField(localThis.defualtPKSelectField);
            localThis.pkSelectField.label = "Data Item";

            localThis.fileNameField = localThis.parseJrsFormField({
                column_name: "FILE_NAME",
                type: "text",
                required: false,
                list_order: 3,
                form_order: 3,
            });
            localThis.fileNameField.label = "File Name";
        },
    });
</script>

<style scoped>
</style>