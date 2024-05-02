<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row v-if="settlementModule && !selectedBiodata">
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-settlement-detained-person.title") }}
                <v-icon large> mdi-account-child</v-icon>
              </h1>
              <p>{{ $t("views.view-settlement-detained-person.view-info") }}</p>
            </v-col>
          </v-row>

          <!--FILTERS (IN CASE OF DETAINED PERSON -> SETTLEMENT)-->
          <!-- MAIN FILTER -->
          <v-card v-if="settlementModule && !selectedBiodata" outlined class="lv-0" dense>
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- SETTLEMENTS SELECT -->
                <v-col :cols="12">
                  <v-autocomplete
                    v-model="settlement"
                    :label="$t('views.view-settlement.settlement-select')"
                    :items="settlementsList"
                    :hint="$t('views.view-settlement.settlement-select-hint')"
                    clearable
                    :persistent-hint="true"
                    item-value="SET_ID"
                    item-text="SET_NAME"
                    return-object
                  ></v-autocomplete>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
          <!-- TABLE -->

          <v-divider></v-divider>
        </v-col>

        <v-col :cols="12">
          <jrs-simple-table
            :viewName="viewNameTable"
            :dataSourceCondition="conditionBeneficiary"
            :title="
              !settlementModule
                ? $t('nav.beneficiary-list')
                : $t('nav.detained-person-list')
            "
            
            :nbrOfFormColumns="3"
            :selectable="false"
            :hideNewBtn="true"
            :hideActions="true"
            v-if="!selectedBiodata"
            :selectOnRowClick="selectOnRowClick"
            @rowClick="doClickAction"
          >
            <!-- TABLE HEAD -->
            <template v-slot:simple-table-header>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="setupNewForm($event)"
              >
                <v-icon>mdi-plus-circle-outline</v-icon>New
              </v-btn>
            </template>
            <!-- TABLE ITEM ACTIONS -->
            <template
              v-slot:simple-table-item-actions="{ simpleItemRowItem, refreshData }"
            >
              <!-- BTN - UPDATE BIODATA USER -->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-icon
                    icon
                    @click="setupEditForm(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                    >mdi-circle-edit-outline</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.tooltip-edit") }}</span>
              </v-tooltip>
              <!-- BTN - DELETE BIODATA USER -->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-icon
                    @click="toggleDeleteDialogBiodata(simpleItemRowItem, refreshData)"
                    v-bind="attrs"
                    v-on="on"
                    :disabled="isSaving"
                    >mdi-delete</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.deletion-user") }}</span>
              </v-tooltip>

              <!-- BTN - HISTORY OF SPECIFICATIONS OF DETAINED PERSONS  AT:cancellato temporaneamente, richiesta di LUCA-->
              <v-tooltip top v-if="settlementModule">
                <template v-slot:activator="{ on, attrs }">
                  <v-icon
                    v-if="simpleItemRowItem"
                    @click="openDialogSpecificationVersions(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                    >mdi-history</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.tooltip-chronology") }}</span>
              </v-tooltip> 
              <!-- BTN - STORY OF DETAINED PERSONS -->
              <v-tooltip top v-if="settlementModule">
                <template v-slot:activator="{ on, attrs }">
                  <v-icon
                    v-if="simpleItemRowItem"
                    @click="openDialogStory(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                    icon
                    >mdi-tag-multiple</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.tooltip-story") }}</span>
              </v-tooltip>
            </template>
          </jrs-simple-table>

          <!-- NEW/EDIT FORM -->
          <set-detained-person-form
            :userUid="selectedBiodata.HR_BIODATA_USER_UID"
            :readonly="false"
            :userIsBeneficiary="true"
            :hasServicesUsed="selectedBiodata.HAS_USED_SERVICE"
            @newBiodata="setNewBiodata($event)"
            v-if="selectedBiodata && settlementModule"
            :isDetainedPerson="settlementModule ? true : false"
            :showUserStories="true"
          >
            <template v-slot:form-toolbar-title>
              <span v-if="formTitle">{{ formTitle }}</span>
            </template>
            <template v-slot:form-toolbar-content>
              <v-spacer></v-spacer>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3 ma-2"
                @click="openBackDialog = true;"
              >
                <v-icon>mdi-table</v-icon>
                {{ $t("datasource.hrm.biodata.btn-to-list") }}
              </v-btn>
            </template>
          </set-detained-person-form>
          <hrm-biodata-form
            :userUid="selectedBiodata.HR_BIODATA_USER_UID"
            :readonly="false"
            :userIsBeneficiary="true"
            :hasServicesUsed="selectedBiodata.HAS_USED_SERVICE"
            @newBiodata="setNewBiodata($event)"
            v-if="selectedBiodata && !settlementModule"
            :isDetainedPerson="settlementModule ? true : false"
            :showUserStories="true"
          >
            <template v-slot:form-toolbar-title>
              <span v-if="formTitle">{{ formTitle }}</span>
            </template>
            <template v-slot:form-toolbar-content>
              <v-spacer></v-spacer>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3 ma-2"
                @click="selectedBiodata = null"
              >
                <v-icon>mdi-table</v-icon>
                {{ $t("datasource.hrm.biodata.btn-to-list") }}
              </v-btn>
            </template>
          </hrm-biodata-form>
        </v-col>
      </v-row>
      <!-- DELETE CONFIRM -->
      <v-dialog
        v-model="deleteDialog"
        persistent
        retain-focus
        :overlay="false"
        max-width="45em"
        transition="dialog-transition"
      >
        <v-card>
          <v-card-title primary-title class="text-capitalize">
            <v-icon>mdi-alert</v-icon>{{
            $t("general.crud.deletion-user")
          }}</v-card-title>
          <v-card-text class="capital-first-letter">{{
            $t("general.crud.delete-user-confirm")
          }}</v-card-text>
          <v-card-actions>
            <v-btn text color="secondary darken-1" @click="toggleDeleteDialogBiodata()"
              >X {{ $t("general.close") }}</v-btn
            >
            <v-btn color="primary" @click="deleteBiodataUser(deleteItemBiodata)">{{
              $t("general.delete")
            }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

       <!-- BACK CONFIRM -->
      <v-dialog
        v-model="openBackDialog"
        persistent
        retain-focus
        :overlay="false"
        max-width="45em"
        transition="dialog-transition"
      >
        <v-card >
          <v-card-title  primary-title class="text-capitalize">
            <v-icon color="warning" class="mr-1 mb-1">
    mdi-alert
  </v-icon>
  {{
            $t("general.alert")
          }}</v-card-title>
          <v-card-text class="capital-first-letter">{{
            $t("datasource.pms.annual-plan.ap-leave-confirm")
          }}</v-card-text>
          <v-card-actions>
            <v-btn text color="secondary darken-1" @click="openBackDialog=false"
              >X {{ $t("general.close") }}</v-btn
            >
            <v-btn color="primary" @click="selectedBiodata = null;openBackDialog=false">{{
              $t("general.confirm")
            }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!---VERSIONS OF DETAINED PERSON SPECIFICATION-->

      <!-- HISTORY DIALOG SPECIFICATIONS-->
      <v-dialog
        v-model="dialogSpecifications"
        :key="JSON.stringify(selectedDetainedPerson)"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="'100em'"
        transition="dialog-transition"
      >
        <v-card>
          <v-card-title>
            <span class="capital-first-letter">{{ $t("general.detail") }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col :cols="3" style="border-right: solid 1px rgba(0, 0, 0, 0.12)">
                <v-list>
                  <v-subheader class="text-uppercase">{{
                    $t("datasource.hrm.agreement.select-version")
                  }}</v-subheader>
                  <v-divider></v-divider>
                  <v-list-item-group v-model="currentVersion" color="primary">
                    <v-list-item
                      v-for="version in versions"
                      :key="version.SET_DET_PERSON_SPEC_DATE"
                    >
                      <!--:disabled="versions[currentVersion].SET_DET_PERSON_SPEC_DATE  == version.SET_DET_PERSON_SPEC_DATE ? true : false"-->

                      <v-list-item-content>
                        Version:
                        {{ new Date(version.SET_DET_PERSON_SPEC_DATE).toLocaleString() }}
                        {{ version.HR_BIODATA_FULL_NAME_OPERATOR }}
                      </v-list-item-content>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
              </v-col>
              <v-col :cols="9">
                <set-detained-person-form
                  :userIsBeneficiary="true"
                  :hasServicesUsed="selectedDetainedPerson.HAS_USED_SERVICE"
                  :userUid="selectedDetainedPerson.HR_BIODATA_USER_UID"
                  :readonly="true"
                  v-if="settlementModule && selectedDetainedPerson && versions"
                  :specifications="versions[currentVersion]"
                  :pre_specifications="versions[currentVersion+1]"
                  :isDetainedPerson="settlementModule ? true : false"
                >
                </set-detained-person-form>
              </v-col>
            </v-row>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="dialogSpecifications = false; selectOnRowClick = true"
              >X {{ $t("general.close") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- STORY OF DETAINED PERSON-->
      <v-dialog
        v-model="dialogStory"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="'70em'"
        transition="dialog-transition"
      >
        <v-card dense>
          <v-card-title> </v-card-title>
          <v-card-text>
            <jrs-simple-table
              :viewName="'SET_VI_STORY'"
              :dataSourceCondition="conditionStory"
              :key="dialogStory"
              :nbrOfFormColumns="1"
              :selectable="false"
              :hideNewBtn="true"
              :hideActions="false"
              :readonly="true"
              v-if="conditionStory"
            >
            </jrs-simple-table>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="otherDialogOpen=false;dialogStory = false"
              >X {{ $t("general.close") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import HrmBiodataForm from "../../components/HRM/HrmBiodataForm.vue";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import SetDetainedPersonForm from "../../components/SET/SetDetainedPersonForm.vue";
import {
  GenericSqlPayload,
  GenericSqlSelectPayload,
  SqlActionType,
} from "../../axiosapi/api";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "hrm-biodata-form": HrmBiodataForm,
    "set-detained-person-form": SetDetainedPersonForm,
  },
  data: () => ({
    otherDialogOpen:false,
    selectedBiodata: null,
    formTitle: "",
    openBackDialog:false,
    isSaving: false,
    deleteDialog: false,
    resfreshBiodataFunction: null,
    dialogStory: false,
    settlementsList: [],
    conditionStory: null,
    selectOnRowClick:true,
    conditionBeneficiary: "IMS_USER_IS_BENEFICIARY=1",
    settlement: null,
    dialogSpecifications: false,
    versions: [],
    currentVersion: 0,
    selectedDetainedPerson: null,
  }),
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
      getActiveModule: "getActiveModule",
    }),


    settlementModule() {
      const localThis: any = this as any;
      let module = localThis.getActiveModule.moduleCode;

      if (module == "SET") {
        return true;
      } else {
        return false;
      }
    },

    viewNameTable() {
      const localThis: any = this as any;
      return !localThis.settlementModule
        ? "HR_VI_BIODATA"
        : "SET_VI_DETAINED_PERSON_BIODATA";
    },
  },
  watch: {
    settlement(to: any, from: any) {
      const localThis: any = this as any;
       if(localThis.settlementModule){
      let cond = " AND OFFICE_FILTER_ID = " + localThis.getCurrentOffice.HR_OFFICE_ID;
      if (to ? to.SET_ID != 0 : false) {
        cond = " AND OFFICE_FILTER_ID = " + localThis.getCurrentOffice.HR_OFFICE_ID +" AND "+ "PMS_SETTLEMENT_ID = " + to.SET_ID;
      }
      localThis.conditionBeneficiary = "IMS_USER_IS_BENEFICIARY=1" + cond;
      }
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      execSPCall: "execSPCall",
      getGenericSelect: "getGenericSelect",
    }),


   doClickAction(item:any)
    {
      const localThis: any = this as any;
      if(!localThis.otherDialogOpen)
        localThis.setupEditForm(item);
 
    },
    /**
     * Set selectedBiodata for new user creation.
     */
    setupNewForm() {
      const localThis: any = this as any;
      localThis.selectedBiodata = {};
      // Set form title
      localThis.formTitle = "New person";
    },
    /**
     * Set selectedBiodata from seleted user.
     * @param {any} selectedItem Data of the selected user and biodata.
     */
    setupEditForm(selectedItem: any) {
      const localThis: any = this as any;
      localThis.selectedBiodata = { ...selectedItem };
      // Set form title
      const tranlsationKey = "datasource.hrm.biodata.title-update";
      localThis.formTitle = `Update person - ${localThis.selectedBiodata.HR_BIODATA_FULL_NAME}`;
    },
    /**
     * Set selectedBiodata from newly created IMS_USER and HR_BIODATA.
     * @param {any} newBiodata Data of newly created user and biodata.
     */
    setNewBiodata(newBiodata: any) {
      const localThis: any = this as any;
      localThis.selectedBiodata = { ...newBiodata };
    },
    /**
             * Open event to eliminate biodata user (in logic way)
             @param saveData The data to delete 
             @param actionType The action to delete CRUD
             */
    deleteBiodataUser(saveData: any) {
      let localThis: any = this as any;
      let spName: string = "HR_SP_INS_UPD_BIODATA";
      localThis.isSaving = true;
      //Add external data to payload
      if (localThis.savePayload) {
        saveData = {
          external_data: localThis.savePayload,
          rows: saveData,
        };
      }
      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: SqlActionType.NUMBER_2,
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
          });
          if (localThis.resfreshBiodataFunction) {
            localThis.resfreshBiodataFunction.call();
          }
          localThis.toggleDeleteDialogBiodata();
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },
    /**
     * Toggle Delete dialog and set item to delete and table refreshFunction.
     * @param {any} deleteItemBiodata Item to send to SP to delete.
     * @param {Function} resfreshData Function to call to refresh table data.
     */
    toggleDeleteDialogBiodata(deleteItemBiodata: any = {}, resfreshData?: Function) {
      const localThis: any = this as any;
      localThis.deleteDialog = !localThis.deleteDialog;
      localThis.otherDialogOpen=localThis.deleteDialog;
      localThis.deleteItemBiodata = deleteItemBiodata;
      localThis.resfreshBiodataFunction = resfreshData;
    },
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
          //localThis.settlement = localThis.settlementsList[0];
        });
    },

    async openDialogSpecificationVersions(rowData: any) {
      let localThis: any = this as any;
      localThis.selectOnRowClick = false;

      
      let biodata = await localThis.getBiodata(
        rowData.HR_BIODATA_USER_UID
      );

      let getBiodataDetained = await localThis.getBiodataDetained(
        rowData.HR_BIODATA_USER_UID
      );
      
      let specifications = await localThis.getSpecifications(
        rowData.HR_BIODATA_USER_UID,
        biodata,
        getBiodataDetained
      );

      localThis.versions = specifications;
      if(localThis.versions){
          localThis.versions.sort(function(a:any,b:any){
          const date1:any = new Date(a.SET_DET_PERSON_SPEC_DATE);
          const date2:any = new Date(b.SET_DET_PERSON_SPEC_DATE);
          let result = date2 - date1
          return result
        });

        localThis.selectedDetainedPerson = rowData;
        localThis.currentVersion = 0
        localThis.dialogSpecifications = true;
      }
    },

    async openDialogStory(rowData: any) {
      let localThis: any = this as any;
      if (localThis.settlementModule && rowData.HR_BIODATA_USER_UID) {
        localThis.conditionStory = `FILL_IN_USER = '${rowData.HR_BIODATA_USER_UID}'`;
      } else {
        localThis.conditionStory = null;
      }
      localThis.dialogStory = true;
      localThis.otherDialogOpen=true;
    },
    async getSpecifications(biodata_uid: any,biodata:any, getBiodataDetained: any) {
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_VI_DETAINED_PERSON_SPECIFICATIONS",
        colList: null,
        whereCond: `SET_DET_PERSON_UID = '${biodata_uid}'`,
        orderStmt: `SET_DET_PERSON_SPEC_DATE`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
        
          if (res.table_data) {
            let array: any = [];
            res.table_data.forEach((ele: any) => {
              array.push({ ...ele, ...biodata[0], ...getBiodataDetained });
            });

            return array;
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },

    async getBiodataDetained(user_uid: any) {
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_VI_DETAINED_PERSON_BIODATA",
        colList: null,
        whereCond: `SET_DET_PERSON_UID = '${user_uid}'`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            return res.table_data;
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },

    async getBiodata(user_uid: any) {
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_BIODATA",
        colList: null,
        whereCond: `HR_BIODATA_USER_UID = '${user_uid}'`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            return res.table_data;
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },
  },

  mounted() {
    let localThis: any = this;

    if (localThis.settlementModule) {
        localThis.getSettlementsBiodata();
        localThis.conditionBeneficiary =
        "IMS_USER_IS_BENEFICIARY=1";
    }else{
         localThis.conditionBeneficiary = "IMS_USER_IS_BENEFICIARY=1 AND PMS_IS_DETAINED_PERSON <> 1";
    }
  },
});
</script>

<style scoped></style>
