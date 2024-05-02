<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-tabs  v-model="tabModel"  centered background-color="primary darken-1" dark>
                        <v-tab  @click="forceRerenderTab()" v-for="table in tables" :key="table.code">{{ $t(table.tab_key) }}</v-tab>
                        <v-tab key="STATUS">{{ $t('datasource.ims.status.title') }}</v-tab>
                    </v-tabs>
                    <v-tabs-items v-model="tabModel" class="tab-item-wrapper">
                        <v-tab-item  v-for="table in tables" :key="table.code">
                            <jrs-simple-table
                                :viewName="table.name"
                                v-if="renderTab"
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                                :dataSourceCondition="`${table.delete_bit} = 0`"
                            ></jrs-simple-table>
                        </v-tab-item>
                        <v-tab-item key="STATUS">
                            <jrs-simple-table
                                viewName="IMS_VI_STATUS"
                                dataSourceCondition="IMS_STATUS_CLASS_CODE = 'VACANCY'"
                                :savePayload="{IMS_STATUS_CLASS_CODE:'VACANCY'}"
                            ></jrs-simple-table>
                        </v-tab-item>
                    </v-tabs-items>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapGetters } from 'vuex';
    import JrsSimpleTable from '../../components/JrsSimpleTable.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable
        },
        data(){
            return {
                tabModel: null,
                tables: [
                    {name: 'HR_VI_PUBLICATION_PLATFORM', code:"PUBLICATION_PLATFORM",delete_bit:"HR_PUBLICATION_PLATFORM_DELETE", tab_key:"datasource.hrm.vacancy.config.publication-platform"},
                    {name: 'HR_VI_CRITERIA', code:"CRITERIA",delete_bit:"HR_CRITERIA_DELETE",  tab_key:"datasource.hrm.vacancy.config.criteria"}
                ],
                renderTab: true,
            }
        },
        methods:{
                forceRerenderTab() {
                        // Remove my-component from the DOM
                        this.renderTab = false;

                        this.$nextTick(() => {
                        // Add the component back in
                        this.renderTab = true;
                        });
                    }
                
        },
        computed: {
            ...mapGetters({
            getUserUid: 'getUserUid',
            getCurrentOffice: 'getCurrentOffice'
            })
        }
    })
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>