<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-tabs  v-model="tabModel"  centered background-color="primary darken-1" dark>
                        <v-tab  @click="forceRerenderTab()" v-for="table in tables" :key="table.code">{{ $t(table.tab_key) }}</v-tab>
                        <v-tab key="STATUS">{{ $t('datasource.hrm.education-history.status-tab') }}</v-tab>
                    </v-tabs>
                    <v-tabs-items v-model="tabModel" class="tab-item-wrapper">
                        <v-tab-item  v-for="table in tables" :key="table.code">
                            <jrs-simple-table
                                :viewName="table.name"
                                v-if="renderTab"
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                                :nbrOfFormColumns="2"
                              
                            >
                            <template  v-slot:simple-table-header="{refreshData}" v-if="table.name == 'HR_VI_EDUCATION_LEVEL'">
                            <jrs-copy-from-to-offices
                                  :v-if="table.name=='HR_VI_EDUCATION_LEVEL'"
                                  :tableName="'HR_EDUCATION_LEVEL'"
                                  :columnNamePK="'HR_EDUCATION_LEVEL_ID'"
                                  :columnNameOffice="'HR_OFFICE_FILTER'"
                                  @refreshData = refreshData();
                            >
                            
                            </jrs-copy-from-to-offices>

                                </template>
                            </jrs-simple-table>
                  
                        </v-tab-item>
                          <v-tab-item key="STATUS">
                            <jrs-simple-table
                                viewName="IMS_VI_STATUS"
                                dataSourceCondition="IMS_STATUS_CLASS_CODE = 'EDUCATION_HISTORY'"
                                :savePayload="{IMS_STATUS_CLASS_CODE:'EDUCATION_HISTORY'}"
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
    import JrsCopyFromToOffice from '../../components/JrsCopyFromToOffices.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable,
            'jrs-copy-from-to-offices': JrsCopyFromToOffice
        },
        data(){
            return {
                tabModel: null,
                tables: [
                    //{name: 'HR_VI_EDUCATION_LEVEL', code:"EDU", tab_key:"datasource.hrm.education-history.level-edu"},
                    {name: 'HR_VI_LANGUAGE_KNOWLEDGE_LEVEL', code:"KNOW_LANG", tab_key:"datasource.hrm.language-knowledge.title"}
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