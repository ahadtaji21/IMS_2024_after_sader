<template>
  <v-content>
    <v-container fluid fill-height v-if="1 == 1">
      <h1>Welcome to the playground. We've got fun and games!</h1>
      <v-row>
        <v-col :cols="12">
          <div>{{ ok }}</div>
          <v-btn @click="dialog = !dialog"></v-btn>
        </v-col>
      </v-row>
      <v-row>
        <v-dialog
          v-model="dialog"
          persistent
          retain-focus
          :overlay="false"
          max-width="45em"
          transition="dialog-transition"
        >
          <v-card>
            <v-container container-fluid>
                <v-row>
                    <v-col cols="4">
              <v-card-title primary-title class="text-capitalize">Approve Timesheet</v-card-title>
        lo       </v-col >
                     <v-col cols="4">
                   <jrs-date-picker
                        :label="'Date'"
                        v-model="date"
                        :hint="'From'"
                        primary
                        :is_only_year="true"
                     
                    ></jrs-date-picker >
                      </v-col >
                          <v-col cols="4">
                          <jrs-date-picker
                        :label="'Date'"
                        v-model="date"
                        :hint="'From'"
                        primary
                    
                     
                    ></jrs-date-picker >
                   </v-col>
                </v-row>
              <v-card-text
                :key="0"
              >
                
                <v-row>
                <v-toolbar color="primary">
                  <v-row>
                      <v-col cols="6" class="mt-4">
                              <v-btn     @click="dialogLegend = !dialogLegend">Legend</v-btn>
                    </v-col>
                    <v-col cols="6" class="mt-4">
                      <v-switch
                        dark
                        :label="$t('datasource.hrm.timesheet.approve-supervisor')"
                        @change="switchAllPresent
                            ? selectedClass.forEach((u) => {
                                u.APPROVED_SUPERVISOR = true;
                              })
                            : selectedClass.forEach((u) => {
                                u.APPROVED_SUPERVISOR = false;
                              })
                        "
                        v-model="switchAllPresent"
                      >
                      
                      </v-switch>
                    </v-col>
                  </v-row>
                </v-toolbar>
          
          
                    <v-col cols="12">
                  <template scrollable>
                    <v-expansion-panels>
                      <v-expansion-panel
                        v-for="chat in selectedClass"
                        :key="chat.title"
                        hide-actions
                        dense
                      
                      >
                        <v-expansion-panel-header
                          disable-icon-rotate  dense   expand-icon=""
                        >
                          <v-container>
                            <v-row align="center"  >
                              <v-col cols="5">
                                    
                                 {{ chat.HR_TIMESHEET_DATE }} 
                           
                                / 
                                {{ chat.HR_TIMESHEET_DATE_END }}
                             
                              
                              </v-col>
                                 <v-col cols="4">
                                 
                                 {{ chat.HR_TIMESHEET_START_TIME }} 
                              
                                 - 
                                {{ chat.HR_TIMESHEET_END_TIME }}
                          
                            </v-col>
                                        <v-col cols="1" >
                                {{ chat.HR_TYPE_DAY_CODE }}
                              </v-col>
                                <v-col cols="1">
                                    <v-avatar
                                :color="chat.HR_TYPE_COLOR"
                                size="15"
                                >
                                
                                </v-avatar> 
                              </v-col>
                                  
                              <v-col cols="1">
                                <v-switch
                                  @click.native.stop
                                  v-model="chat.APPROVED_SUPERVISOR"
                                >
                                </v-switch>
                              </v-col>
                            </v-row>
                          </v-container>
                        </v-expansion-panel-header>

                        <v-expansion-panel-content>
                          <v-divider></v-divider>
                          <v-card-text>
                            <jrs-editor
                              v-model="chat.HR_TIMESHEET_NOTES_SUP"
                            >
                            </jrs-editor>
                          </v-card-text>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </template>
                </v-col>
                </v-row>

              </v-card-text>
              <v-card-actions>
                <v-btn
                  text
                  color="secondary darken-1"
                  @click="dialog = !dialog"
                  >X {{ $t("general.cancel") }}</v-btn
                >
                <v-btn
                  color="primary"
                  @click="dialog = !dialog"
                  >{{ $t("general.save") }}</v-btn
                >
              </v-card-actions>
            </v-container>
          </v-card>
        </v-dialog>
         <v-dialog
                ref="dialog"
                v-model="dialogLegend"
                persistent
                max-width="600"
            >

      <v-card>
            <v-card-title  class="primary lighten-2 black--text">
                     {{ $t("general.legend") }}
        </v-card-title>
        <v-card-text>
        <v-row>
          <template>
          <div class="text-left">
            <v-chip
              v-for="legend in legends"
              :key="legend.id"
              class="ma-2"
              :color="legend.color"
              text-color="white"
            >
              {{ legend.name }}
              {{ legend.code ? "(" + legend.code + ")" : "" }}
            </v-chip>
          </div>
        </template>
         </v-row>
      
        </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
                  <v-btn
                            color="secondary darken-1"
                            text
                            @click="dialogLegend = !dialogLegend"
                            >X {{ $t('general.cancel') }}</v-btn>   
        </v-card-actions>
      </v-card>
    </v-dialog>
      </v-row>
    </v-container>
  </v-content>
</template>

<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
<script lang="ts">
import { mapActions, mapGetters } from "vuex";
import FormMixin from "../mixins/FormMixin";
import JrsEditor from "../components/TextEditor.vue";
import UtilMix from "../mixins/UtilMix";
import JrsDatePicker from "../components/JrsDatePicker.vue";
import { i18n } from "../i18n";
    import {
        GenericSqlSelectPayload,
        GenericSqlPayload,
        SqlActionType,
    } from "../axiosapi/api";
export default {
  name: "home",
  components: {
   "jrs-editor": JrsEditor,
   "jrs-date-picker":JrsDatePicker
  },
  mixins: [FormMixin, UtilMix],
  data: () => ({
    ok: "ok",
    dialog: false,
    dialogLegend:false,
    selectedClass: [],
    date: null,
    legends:[],
    switchAllPresent:false
  }),
  computed: {
    /**
     * condition of value of switch for all set all present
     */

    /*switchAllPresent: {
      get() {
    
        let localThis: any = this as any;
        let check = true;
        localThis.selectedClass.forEach((u: any) => {
          if (!u.APPROVED_SUPERVISOR) {
            check = false;
          }
        });
        return check;
      },
      set(value:any) {
        (this as any).value = value;
      },
    },*/
  },
  watch:{
   switchAllPresent(){
         let check = true;
        let localThis: any = this as any;
        localThis.selectedClass.forEach((u: any) => {
          if (!u.APPROVED_SUPERVISOR) {
            check = false;
          }
         });
     }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {}),
     getTimesheet(){
                    let localThis: any = this as any;

                    let selectPayload: GenericSqlSelectPayload = {
                        viewName: "HR_VI_TIMESHEET",
                        colList: null,
                        whereCond: `HR_BIODATA_USER_UID = 'e63de8ea-5729-4834-94cf-db43721b32ed'`,
                    };

                    return localThis.getGenericSelect({ genericSqlPayload: selectPayload })
                        .then((res: any) => {
                        //Setup data
                        if (res.table_data) {
                            res.table_data.forEach((item: any) => {
                                 item.HR_TIMESHEET_START_TIME = item.HR_TIMESHEET_START_TIME;
                                 item.HR_TIMESHEET_END_TIME = item.HR_TIMESHEET_END_TIME;
                                 localThis.selectedClass.push(item);
                            });
                        }
                        }).catch((err:any)=>{
                                    console.error(err);
                        });
            },
       /**
     * Get Legends
     */
    async getLegends() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_TIMESHEET_TYPE_DAY",
        colList: null,
        orderStmt: null,
        whereCond: `HR_TYPE_DAY_DELETE = 0`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            res.table_data.forEach((leg: any) => {
              let eventoData = {
                name: leg.HR_TYPE_DAY_DESCRIPTION,
                code: leg.HR_TYPE_DAY_CODE,
                color: leg.HR_TYPE_COLOR ? leg.HR_TYPE_COLOR : "blue",
                id: leg.HR_TYPE_DAY_ID,
              };

              localThis.legends.push(eventoData);
            });
          }

          let festivityDays = {
            name: "Festivity Days",
            color: "green",
            id: 0,
          };

          localThis.legends.push(festivityDays);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },
  },
  mounted() {
    const localThis = this as any;
    localThis.getTimesheet();
    localThis.getLegends(); // get the legend of timesheet
  },
};
</script>

