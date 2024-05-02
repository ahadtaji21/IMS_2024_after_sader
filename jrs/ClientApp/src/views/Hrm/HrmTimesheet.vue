<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                   <v-tabs  v-model="tabModel"  centered background-color="primary darken-1" dark>
                        <v-tab  @click="forceRerenderTab()" v-for="table in tables" :key="table.code">{{ $t(table.tab_key) }}</v-tab>
                    </v-tabs>
                   <v-tabs-items v-model="tabModel" class="tab-item-wrapper">
                        <v-tab-item  v-for="table in tables" :key="table.code">
                                 <!-- DATE-PICKER -->
                                <jrs-date-picker
                                    :label ="'Filter For Year'"
                                    v-model="YearFilter"
                                    :hint ="'Select Year to filter for year'"
                                    v-if="table.name=='HR_VI_TIMESHEET_FESTIVITY_DAY'"
                                    :is_only_year="true"
                                ></jrs-date-picker>
                            <jrs-simple-table
                                :viewName="table.name"
                                v-if="table.name!='HR_VI_TIMESHEET_FESTIVITY_DAY' ? true :  YearFilter != null && table.name=='HR_VI_TIMESHEET_FESTIVITY_DAY'"
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                                :dataSourceCondition="table.year ? `${table.delete_bit} = 0 AND year(${table.year}) = ${YearFilter}` : `${table.delete_bit} = 0`"
                            >
                            <template v-slot:simple-table-header="{refreshData}" >
                                <jrs-copy-from-to-offices
                                  v-if="table.name=='HR_VI_TIMESHEET_FESTIVITY_DAY'"
                                  :tableName="'HR_TIMESHEET_FESTIVITY_DAY'"
                                  :columnNamePK="'HR_FESTIVITY_DAY_ID'"
                                  :columnNameOffice="'OFFICE_FILTER_ID'"
                                  @refreshData = refreshData();
                            >
                            
                            </jrs-copy-from-to-offices>
                        
                            </template>
                            </jrs-simple-table>
                       
                       
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
    import JrsDatePicker from '../../components/JrsDatePicker.vue';
    import JrsCopyFromToOffice from '../../components/JrsCopyFromToOffices.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable,
            'jrs-date-picker':JrsDatePicker,
            'jrs-copy-from-to-offices': JrsCopyFromToOffice
        },
        data(){
            return {
                tabModel: null,
                tables: [
                    {name: 'HR_VI_TIMESHEET_TYPE_DAY', code:"TIMESHEET_TYPE_DAY",delete_bit:"HR_TYPE_DAY_DELETE", tab_key:"datasource.hrm.timesheet-type-day.title"},
                    {name: 'HR_VI_TIMESHEET_FESTIVITY_DAY', code:"TIMESHEET_FESTIVITY_DAY",delete_bit:"HR_FESTIVITY_DAY_DELETE", tab_key:"datasource.hrm.timesheet-festivity-day.title", year:'HR_FESTIVITY_DAY_DATE'},
                ],
                renderTab: true,
                YearFilter: new Date().getFullYear()
                
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