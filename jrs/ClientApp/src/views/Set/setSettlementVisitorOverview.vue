<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row>
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-settlement-overview.title") }}
                <v-icon large> mdi-alpha-o-circle</v-icon>
              </h1>
              <p>{{ $t("views.view-settlement-overview.view-info") }}</p>
            </v-col>
          </v-row>
          <!--FILTERS TO SELECT SETTLEMENTS + VISITORS -->
          <!-- MAIN FILTER -->
          <v-card
            outlined
            class="lv-0"
            dense
          >
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- SETTLEMENTS SELECT -->
                <v-col :cols="6">
                  
                  <v-autocomplete
                    v-model="settlement"
                    clearable
                    @click:clear="settlement={}"
           
                    :label="
                      $t(
                        'views.view-settlement.settlement-select'
                      )
                    "
                    :items="settlementsList"
                    :hint="
                      $t(
                        'views.view-settlement.settlement-select-hint'
                      )
                    "
                    :persistent-hint="true"
                    item-value="SET_ID"
                    item-text="SET_NAME"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- VISITORS SELECT -->
                 <v-col :cols="6">
                  <v-autocomplete
                    v-model="visitor"
                    clearable
                    @click:clear="visitor={}"
                     :label="
                      $t(
                        'views.view-settlement-overview.visitors-select'
                      )
                    "
                    :items="visitorsList"
                 
                    :persistent-hint="true"
                    item-value="IMS_USER_UID"
                    item-text="HR_BIODATA_FULL_NAME"
                    return-object
                  >
                  
                  </v-autocomplete>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
          <v-divider></v-divider>

          <!-- DATA ENTRY FOR THE VISIT (SIMPLE TABLE) -->

          <v-row >
            <v-col :cols="12" v-if="settlementsList.length > 0">
                <jrs-simple-table
                     viewName="SET_VI_SETTLEMENT_VISITOR_OVERVIEW_LAST"
                    :dataSourceCondition="conditionOverview" 
                    :hideNewBtn="false"
                    :hideActions="false"
                    :nbrOfFormColumns="3"
                   :selectOnRowClick="true"
                   :externalDialogOpen="externalDialogOpen"
               >
                <!-- TABLE ITEM ACTIONS -->
            <template
              v-slot:simple-table-item-actions="{
                simpleItemRowItem
              }"
            >
             <v-tooltip top>
               <template v-slot:activator="{ on, attrs }">
                  <v-icon
                     small
                     class="ml-1"
                    v-if="simpleItemRowItem"
                    @click="externalDialogOpen=true;openDialogSpecificationVersions(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                  
                    >mdi-history</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.tooltip-chronology") }}</span>
             </v-tooltip>
                </template>
                </jrs-simple-table>
            </v-col>
          </v-row>

            <!-- HISTORY DIALOG SPECIFICATIONS-->
      <v-dialog
        v-model="dialogSpecifications"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="'60em'"
        transition="dialog-transition"
      >
        <v-card >
          <v-card-title>
            <span class="capital-first-letter">{{ $t("general.detail") }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col
                :cols="3"
                style="border-right: solid 1px rgba(0, 0, 0, 0.12)"
              >
                <v-list>
                  <v-subheader class="text-uppercase">{{
                    $t("datasource.hrm.agreement.select-version")
                  }}</v-subheader>
                  <v-divider></v-divider>
                  <v-list-item-group v-model="currentVersion" color="primary">
                    <v-list-item
                      v-for="version in versions"
                      :key="version.SET_OVERVIEW_DATE"
                    >
                      <!--:disabled="versions[currentVersion].SET_DET_PERSON_SPEC_DATE  == version.SET_DET_PERSON_SPEC_DATE ? true : false"-->

                      <v-list-item-content >
                        Version:
                        {{
                          new Date(
                            version.SET_OVERVIEW_DATE
                          ).toLocaleString()
                        }}
                        {{ version.FULL_NAME_OPERATOR }}
                      </v-list-item-content>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
              </v-col>

              <v-col :cols="9" >
                    <jrs-readonly-detail
            :detailData="versions[currentVersion]"
            :detailFields="detailFields"
            :nbrOfColumns="1"
            :key="versions[currentVersion] ? versions[currentVersion].SET_OVERVIEW_ID : 0"
          >
          </jrs-readonly-detail>
              </v-col>
            </v-row>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="dialogSpecifications = false;externalDialogOpen = false"
              >X {{ $t("general.close") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import jrsReadonlyDetail from "../../components/JrsReadonyDetail.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
  import TableMixin from "../../mixins/TableMixin";
import FormMixin from "../../mixins/FormMixin";
import { JrsFormFieldSearch } from "../../models/JrsForm";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-readonly-detail": jrsReadonlyDetail
  },
  mixins: [FormMixin,TableMixin],
  data() {
    return {
      externalDialogOpen:false,
      settlementsList: [],
      settlement:{},
      visitorsList:[],
        versions: [],
    currentVersion: 0,
      visitor:{},
      dialogSpecifications:false,
      detailFields:[],
      conditionOverview: "",
      isSaving: false,
    };
  },

  watch:{
    //watch to control filter for visitor (partial filter)
    visitor(to:any){
            let localThis: any = this as any;
            localThis.conditionOverview  = 
            (to ? to.IMS_USER_UID :false)  ? (localThis.settlement ? localThis.settlement.SET_ID : false)  ?  `SET_OVERVIEW_SETTLEMENT_ID = ${localThis.settlement.SET_ID}  AND SET_OVERVIEW_VISITOR_UID = '${localThis.visitor.IMS_USER_UID}' AND`
            :`SET_OVERVIEW_VISITOR_UID = '${to.IMS_USER_UID}' AND`
            : (localThis.settlement ? localThis.settlement.SET_ID : false) ?  `SET_OVERVIEW_SETTLEMENT_ID = ${localThis.settlement.SET_ID} AND` 
            : ''
            localThis.conditionOverview +=  ` SET_OVERVIEW_IS_DELETED = 0 AND SET_OVERVIEW_OPERATOR_UID = '${localThis.getUserUid}' `
    },

    //watch to control filter for settlement (partial filter)
    settlement(to:any){
          let localThis: any = this as any;
          localThis.conditionOverview = (to ? to.SET_ID  :false)  ? (localThis.visitor ?  localThis.visitor.IMS_USER_UID:false) ?  `SET_OVERVIEW_SETTLEMENT_ID = ${to.SET_ID} AND SET_OVERVIEW_VISITOR_UID = '${localThis.visitor.IMS_USER_UID}' AND`
          :`SET_OVERVIEW_SETTLEMENT_ID = ${to.SET_ID} AND `
          : (localThis.visitor ?  localThis.visitor.IMS_USER_UID:false) ?  `SET_OVERVIEW_VISITOR_UID = '${localThis.visitor.IMS_USER_UID}' AND` 
          : ''
          localThis.conditionOverview +=  ` SET_OVERVIEW_IS_DELETED = 0 AND SET_OVERVIEW_OPERATOR_UID = '${localThis.getUserUid}'`
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
      getUserUid: "getUserUid",
    }),
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),

      async openDialogSpecificationVersions(rowData: any) {
      let localThis: any = this as any;

      let specifications = await localThis.getOverviewVisits(
        rowData.SET_OVERVIEW_VISITOR_UID,
        rowData.SET_OVERVIEW_SETTLEMENT_ID
      );

    
      localThis.versions = specifications; 

      localThis.currentVersion = localThis.versions.length-1;
      localThis.dialogSpecifications = true;
      
    },

        /**
     * Return a list of settlements (biodata) filtered for office
     */
    getOverviewVisits(id:any,settlement_id:any) {
        let localThis: any = this as any;
        let selectPayload: GenericSqlSelectPayload = {
            viewName: "SET_VI_SETTLEMENT_VISITOR_OVERVIEW",
            officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
            whereCond: `SET_OVERVIEW_VISITOR_UID = '${id}' AND SET_OVERVIEW_SETTLEMENT_ID = ${settlement_id} AND SET_OVERVIEW_IS_DELETED = 0`
        };

        return localThis
            .getGenericSelect({ genericSqlPayload: selectPayload })
            .then((res: any) => {
            if (res.item_count > 0) {
                // Setup projectList
                return res.table_data;
            }
            });
    },


    /**
     * Return a list of settlements (biodata) filtered for office
     */
    getSettlementsBiodata() {
        let localThis: any = this as any;
        let selectPayload: GenericSqlSelectPayload = {
            viewName: "SET_VI_SETTLEMENTS_BIODATA",
            officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        };

        return localThis
            .getGenericSelect({ genericSqlPayload: selectPayload })
            .then((res: any) => {
            if (res.item_count > 0) {
                // Setup projectList
                let settList = res.table_data.map((row: any) => {
                return {
                    SET_ID: row.SET_ID,
                    SET_NAME: row.SET_NAME,
                };
                });
                localThis.settlementsList = [...settList];
            }
             localThis.settlement = localThis.settlementsList[0];

             localThis.conditionOverview  = localThis.settlement ? `SET_OVERVIEW_SETTLEMENT_ID = ${localThis.settlement.SET_ID} AND SET_OVERVIEW_OPERATOR_UID = '${localThis.getUserUid}' AND SET_OVERVIEW_IS_DELETED = 0` : `SET_OVERVIEW_OPERATOR_UID = '${localThis.getUserUid}' AND SET_OVERVIEW_IS_DELETED = 0`
            });
    },
   
    /**
     * Return a list of visitors (JRS STAFF OR NOT)
     */
    getVisitorList() {
        let localThis: any = this as any;
        let selectPayload: GenericSqlSelectPayload = {
            viewName: "HR_VI_USER_SELECTOR",
            officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
            whereCond: `isBeneficiary = 0 AND disableSelection = 0`
        };

        return localThis
            .getGenericSelect({ genericSqlPayload: selectPayload })
            .then((res: any) => {
            if (res.item_count > 0) {
                // Setup projectList
                let settList = res.table_data.map((row: any) => {
                return {
                    HR_BIODATA_FULL_NAME: row.HR_BIODATA_NAME + ' ' +row.HR_BIODATA_SURNAME,
                    IMS_USER_UID: row.IMS_USER_UID,
                };
                });
                localThis.visitorsList = [...settList];
            }
            });
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getSettlementsBiodata();
    localThis.getVisitorList();
    localThis.getTableData('SET_VI_SETTLEMENT_VISITOR_OVERVIEW', localThis.getCurrentOffice.HR_OFFICE_ID)
                .then( (res:any) => {
                    localThis.detailFields = res.formFields
                })
            }
  
});
</script>

<style scoped>
</style>