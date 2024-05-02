<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                            <jrs-simple-table
                                :viewName="'HR_VI_FAMILY'"
                                v-if="renderTab"
                                
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                            >
                         <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                            <v-icon 
                                small
                                @click="visualizeFamilyStructure(simpleItemRowItem)"
                            >mdi-account-group</v-icon>
                        </template>
                        </jrs-simple-table>
                </v-col>
            </v-row>
                <!-- DETAILS DIALOG -->
          <v-dialog
            v-model="detailDialog"
            persistent
            retain-focus
            :scrollable="true"
            :overlay="false"
            :max-width="'60em'"
            transition="dialog-transition"
          >
            <v-card>
              <v-card-text>
                <v-row>
                  <v-col :cols="12">
                       <jrs-simple-table
                               viewName="HR_VI_FAMILY_MEMBERS"
                               :dataSourceCondition="familyMembersFilterCondition"
                               :hideNewBtn="true"
                               :hideActions="true"
                          ref="table_members"
                        ></jrs-simple-table>
                  </v-col>
                </v-row>
              </v-card-text>
              <v-card-actions>
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="detailDialog=false"
                >X {{ $t('general.close') }}</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
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
                    {name: 'HR_VI_FAMILY', code:"FAMILY", tab_key:"nav.family-list"},
                  //  {name: 'HR_VI_CONTACT_CLASS', code:"CLASS", tab_key:"datasource.hrm.contact-class.title"}
                ],
                renderTab: true,
                selectItem: [],
                detailDialog:false,
                familyMembersFilterCondition: ""
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
                    },
                visualizeFamilyStructure(item:any){
                     let localThis: any = this as any;
                     localThis.selectItem = item;
                     localThis.detailDialog=true;
                     localThis.familyMembersFilterCondition = `FAMILY_ID = '${ item.ID ? item.ID : 0 }'`;
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