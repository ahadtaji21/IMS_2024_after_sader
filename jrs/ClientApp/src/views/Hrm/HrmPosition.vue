<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                  <!-- <v-tabs  v-model="tabModel"  centered background-color="primary darken-1" dark>
                        <v-tab  @click="forceRerenderTab()" v-for="table in tables" :key="table.code">{{ $t(table.tab_key) }}</v-tab>
                    </v-tabs>
                        <v-tab-item  v-for="table in tables" :key="table.code">-->
                            <jrs-simple-table
                                :viewName="tables[0].name"
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                                :dataSourceCondition="`${tables[0].delete_bit} = 0`"
                                :selectOnRowClick="true"
                            >

                            </jrs-simple-table>
                       
                       
               <!--  </v-tab-item>
               </v-tabs-items>-->
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
                    {name: 'HR_VI_SALARY_LEVEL', code:"SALARY_LEVEL",delete_bit:"HR_SALARY_IS_DELETED", tab_key:"datasource.hrm.position.salary-level"}
                ],
                renderTab: true        
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