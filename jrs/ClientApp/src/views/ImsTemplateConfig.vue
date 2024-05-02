<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <h1 class="text-capitalize">
                        {{ $t("views.view-template-config.title") }}
                    </h1>
                    <p>{{ $t("views.view-template-config.page-descr") }}</p>
                </v-col>
            </v-row>
            <v-row>
                <v-col :cols="12">
                    <jrs-simple-table viewName="IMS_TEMPLATE_TYPE">
                        <template
                            v-slot:simple-table-item-actions="{
                                simpleItemRowItem: rowItem,
                            }"
                        >
                            <v-icon
                                small
                                @click="
                                    showParamTable(rowItem.TEMPLATE_TYPE_ID)
                                "
                                >mdi-view-module</v-icon
                            >
                        </template>
                    </jrs-simple-table>
                </v-col>
            </v-row>

            <!-- TEMPLATE PARAMS -->
            <v-dialog 
                v-model="showParamsDialog"
                persistent
                retain-focus
                :overlay="false"
                max-width="60em"
                transition="dialog-transition"
            >
                <v-card>
                    <v-card-title></v-card-title>
                    <v-card-text>
                        <jrs-simple-table
                            viewName="IMS_TEMPLATE_PARAMS"
                            :title="$t('datasource.ims.template-params.title')"
                            :showSearchbar="true"
                            :dataSourceCondition="`TEMPLATE_TYPE_ID = ${currentTemplateId}`"
                            :savePayload="{ TEMPLATE_TYPE_ID: currentTemplateId }"
                        ></jrs-simple-table>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn
                            color="secondary darken-1"
                            text
                            @click="showParamsDialog = false"
                        >X {{ $t('general.close') }}</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-container>
    </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsSimpleTable from "../components/JrsSimpleTable.vue";

export default Vue.extend({
    components: {
        "jrs-simple-table": JrsSimpleTable,
    },
    data: () => {
        return {
            // templateTypes: [],
            templateTypeColumns: [],
            showParamsDialog: false,
            currentTemplateId: 0,
        };
    },
    methods: {
        ...mapActions({
            showNewSnackbar: "showNewSnackbar",
        }),
        ...mapActions("apiHandler", {
            getTemplateTypes: "getTemplateTypes",
        }),
        // /**
        //  * Load and set template types.
        //  */
        // refreshTemplateTypes() {
        //     const localThis: any = this as any;
        //     localThis
        //         .getTemplateTypes()
        //         .then((res: any) => {
        //             localThis.templateTypes = res;
        //         })
        //         .catch((err: any) => {
        //             localThis.showNewSnackbar({ typeName: "error", text: err });
        //         });
        // },
        showParamTable(templateId: number) {
            const localThis: any = this as any;
            localThis.showParamsDialog = true;
            localThis.currentTemplateId = templateId;
        },
    },
    // async mounted() {
    //     const localThis: any = this as any;
    //     localThis.refreshTemplateTypes();
    // },
});
</script>

<style scoped></style>
