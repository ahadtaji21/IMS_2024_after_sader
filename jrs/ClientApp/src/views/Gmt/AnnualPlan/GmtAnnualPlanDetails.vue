<template>
  <v-content>
    <v-container fluid fill-height>
      <div v-if="mainDataItems" style="width: 100%">
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
            <h4 style="text-align: left">
              ANNUAL PLAN: {{ mainDataItems.apcode }}
              <span v-if="versioned" style="text-align: left">
                V: {{ mainDataItems.version_number }}
              </span>
            </h4>
            <h6 style="text-align: left">STATUS: {{ mainDataItems.status_in_bar }}</h6>
            <h6 style="text-align: left">IN CHARGE TO: {{ inChargeTo }}</h6>
          </v-col>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
            <div class="text-center" v-if="showWait">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
            </div>
          </v-col>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"> </v-col>

          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
            <v-row :class="'my-border'">
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 5"> </v-col>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
                <v-btn
                  color="primary"
                  class="--darken-1"
                  @click="save()"
                  v-if="isAuthorized()"
                  >Save</v-btn
                >
              </v-col>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"> </v-col>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  @click="GenerateWord()"
                >
                  <v-icon>mdi-file-word</v-icon>Download
                </v-btn>
              </v-col>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"> </v-col>
            </v-row>
          </v-col>
        </v-row>
      </div>
      <div v-else>
        <h4>ANNUAL PLAN</h4>
      </div>
      <v-row>
        <v-col :cols="12">
          <v-card>
            <v-tabs v-model="tabIsVisible" background-color="primary darken-1" dark>
              <!-- <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab> -->
              <v-tab v-for="item in activeTabs" :key="item.code">{{ item.descr }}</v-tab>
            </v-tabs>
            <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
              <v-tab-item key="MAINDATA">
                <v-card>
                  <v-card-title primary-title>Main Data</v-card-title>
                  <ap-main-data
                    @closeDialoge="closeDialogMainData"
                    @closeDialogeAndSaveAnnualPlanMD="closeDialogAndSaveMainData"
                    @closeDialogeAndSaveAnnualPlanMDFromMain="
                      closeDialogAndSaveMainDataFromMain
                    "
                    :formData="localEditItemMainData"
                    :isUpdatableForm="isAuthorized()"
                    :versioned="versioned"
                  ></ap-main-data>
                </v-card>
              </v-tab-item>
              <v-tab-item key="PB">
                <v-tabs
                  v-model="subTabIsVisible"
                  background-color="orange"
                  dark
                  slider-color="yellow"
                >
                  <v-tab v-for="item1 in activeSubTabs" :key="item1.code">{{
                    item1.descr
                  }}</v-tab>
                </v-tabs>
                <v-tabs-items v-model="subTabIsVisible" style="padding: 0.5em">
                  <!-- NARRATIVE -->
                  <v-tab-item key="NARRATIVE">
                    <ap-narrative
                      :selectedObjectId="editItemNarrativeId"
                      :tableName="tblName"
                      :isUpdatableForm="isAuthorized()"
                      :versioned="versioned"
                    ></ap-narrative>
                  </v-tab-item>
                  <!-- Overall Goal -->
                  <v-tab-item key="OVG">
                    <ap-ovg
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-ovg>
                  </v-tab-item>
                  <!-- Objectives -->
                  <v-tab-item key="OBJ">
                    <ap-obj
                      :selectedAnnualPlan="editItemOBJ"
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :versioned="versioned"
                    ></ap-obj>
                  </v-tab-item>
                  <!-- Strategic And Support Services -->
                  <v-tab-item key="SSS">
                    <ap-sss
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-sss>
                  </v-tab-item>

                  <!-- Cross Cutting Process -->
                  <v-tab-item key="CCP">
                    <ap-ccp
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-ccp>
                  </v-tab-item>

                  <!-- Cross Cutting Services -->
                  <v-tab-item key="CCS">
                    <ap-ccs
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-ccs>
                  </v-tab-item>
                  <!-- Cross Process -->
                  <!-- <v-tab-item key="CSP">
                    <ap-csp
                      :isUpdatableForm="isAuthorized()"
                      :formData="formDataOVG"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-csp>
                  </v-tab-item> -->
                  <!-- OverView -->
                  <v-tab-item key="OVW">
                    <ap-overview
                      :key="Math.ceil(Math.random() * 1000)"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-overview>
                  </v-tab-item>
                </v-tabs-items>
              </v-tab-item>
              <v-tab-item key="MANDE">
                <v-radio-group
                  v-model="indicatorType"
                  row
                  class="justify-center"
                  @change="indicators()"
                >
                  <v-radio label="Overall Goal" value="overall"></v-radio>
                  <v-radio label="Outcome Objective" value="outcome"></v-radio>
                  <v-radio label="Output" value="output"></v-radio>
                  <v-radio
                    label="Cross Cutting Process"
                    value="crosscuttingprocess"
                  ></v-radio>
                  <v-radio
                    label="Cross Cutting Service"
                    value="crosscuttingservice"
                  ></v-radio>
                  <v-radio
                    label="Strategic and support services"
                    value="stratprocserv"
                  ></v-radio>
                </v-radio-group>
                <div v-if="indicatorType == 'overall'">
                  <ap-mande-overall
                    :key="Math.ceil(Math.random() * 1000)"
                    :onlyRead="!isAuthorized()"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :versioned="versioned"
                  >
                  </ap-mande-overall>
                </div>
                <div v-if="indicatorType == 'outcome'">
                  <ap-mande-outcobj
                    :key="Math.ceil(Math.random() * 1000)"
                    :onlyRead="!isAuthorized()"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :versioned="versioned"
                  >
                  </ap-mande-outcobj>
                </div>
                <div v-if="indicatorType == 'output'">
                  <ap-mande-output
                    :key="Math.ceil(Math.random() * 1000)"
                    :onlyRead="!isAuthorized()"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :versioned="versioned"
                  >
                  </ap-mande-output>
                </div>
                <div v-if="indicatorType == 'crosscuttingprocess'">
                  <ap-mande-ccp
                    :key="Math.ceil(Math.random() * 1000)"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :onlyRead="!isAuthorized()"
                    :versioned="versioned"
                  >
                  </ap-mande-ccp>
                </div>
                <div v-if="indicatorType == 'crosscuttingservice'">
                  <ap-mande-ccs
                    :key="Math.ceil(Math.random() * 1000)"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :onlyRead="!isAuthorized()"
                    :versioned="versioned"
                  >
                  </ap-mande-ccs>
                </div>
                <div v-if="indicatorType == 'stratprocserv'">
                  <ap-mande-stratprocserv
                    :key="Math.ceil(Math.random() * 1000)"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :onlyRead="!isAuthorized()"
                    :versioned="versioned"
                  >
                  </ap-mande-stratprocserv>
                </div>
              </v-tab-item>
              <!-- Budget Mapping from PMS-->

              <v-tab-item key="BMPMS">
                <v-tabs
                  v-model="subTabBdgIsVisible"
                  background-color="orange"
                  dark
                  slider-color="yellow"
                >
                  <v-tab v-for="item1 in activeSubBdgTabs" :key="item1.code">{{
                    item1.descr
                  }}</v-tab>
                </v-tabs>
                <v-tabs-items v-model="subTabBdgIsVisible" style="padding: 0.5em">
                  <!-- Resoruces -->
                  <v-tab-item key="RESOURCES">
                    <ap-bdg-resource
                      :key="rndVarOutp"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-bdg-resource>
                  </v-tab-item>
                  <!-- Budget -->
                  <v-tab-item key="BDG">
                    <ap-bdg-pms
                      :key="Math.ceil(Math.random() * 1000)"
                      :selectedAnnualPlan="editItemOBJ"
                      :selectedObjectId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-bdg-pms>
                  </v-tab-item>
                </v-tabs-items>
              </v-tab-item>
              <!-- Grants -->
              <v-tab-item key="GNT" :disabled="isAuthorized()">
                <ap-gnt :selectedAnnualPlan="editItemOBJ"></ap-gnt>
              </v-tab-item>
            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapGetters } from "vuex";
import { mapActions } from "vuex";
import AnnualPlanMainData from "../../../components/PMS/ANNUALPLAN/AnnualPlanMainDataForm.vue";
import AnnualOverallGoal from "../../../components/PMS/OVERALLGOAL/AnnualPlanOverallGoalForm.vue";
import AnnualSSS from "../../../components/PMS/STRATSUPPSERV/AnnualPlanSSSForm.vue";
import AnnualCCP from "../../../components/PMS/CROSSCUTTING/AnnualPlanCCPForm.vue";
import AnnualCCS from "../../../components/PMS/CROSSCUTTING/AnnualPlanCCSForm.vue";
import AnnualPlanOBJ from "../../../components/PMS/OBJECTIVES/AnnualPlanObjectiveForm.vue";
import AnnualPlanGMT from "../../../components/GMT/ANNUALPLAN/GRANTS/GMTProjectGrantsForm.vue";
import AnnualPlanBudgetMappingGMT from "../../../components/GMT/MAPPING/GMTObjectMappingForm.vue";
import TextEditor from "../../../components/TextEditor.vue";
import AnnualPlanNarrative from "../../../components/PMS/ANNUALPLAN/AnnualPlanNarrative.vue";
import AnnualPlanOverview from "../../../components/PMS/ANNUALPLAN/AnnualPlanOverView.vue";
import JrsTable from "../../../components/JrsTable.vue";
import JrsSimpleTable from "../../../components/JrsSimpleTable.vue";
import JrsModalForm from "../../../components/JrsModalForm.vue";
import JrsLocationPicker from "../../../components/JrsLocationPicker.vue";
import JrsSearchTable from "../../../components/JrsSearchTable.vue";
import AnnualPlanMandEOutput from "../../../components/PMS/MANDE/AnnualPlanMandEOutputForm.vue";
import AnnualPlanMandEOutcObj from "../../../components/PMS/MANDE/AnnualPlanMandEOutcObjForm.vue";
import AnnualPlanMandEOverall from "../../../components/PMS/MANDE/AnnualPlanMandEOverallForm.vue";
import AnnualPlanMandECCP from "../../../components/PMS/MANDE/AnnualPlanMandECCPForm.vue";
import AnnualPlanMandECCS from "../../../components/PMS/MANDE/AnnualPlanMandECCSForm.vue";
import AnnualPlanMandEStratProcServ from "../../../components/PMS/MANDE/AnnualPlanMandEStrattSuppServForm.vue";
import AnnualPlanResource from "../../../components/PMS/RESOURCES/AnnualPlanResourceProspectForm.vue";
import UtilMix from "../../../mixins/UtilMix";
import NavMix from "../../../mixins/NavMixin";
import {
  PmsAnnualPlanApi,
  ImsApi,
  Configuration,
  NavIntegrationApi,
  NavDimension1,
  NavBudget1,
  ImsLookupsApi,
  AnnualPlanDocApi,
} from "../../../axiosapi";

import { EventBus } from "../../../event-bus";
import { JrsHeader } from "../../../models/JrsTable";
import {
  JrsFormField,
  JrsFormFieldText,
  JrsFormFieldSelect,
} from "../../../models/JrsForm";
//TMP

export default Vue.extend({
  name: "home",
  components: {
    "ap-main-data": AnnualPlanMainData,
    "ap-narrative": AnnualPlanNarrative,
    "ap-obj": AnnualPlanOBJ,
    "ap-gnt": AnnualPlanGMT,
    "ap-ovg": AnnualOverallGoal,
    "ap-sss": AnnualSSS,
    "ap-ccp": AnnualCCP,
    "ap-ccs": AnnualCCS,
    // "ap-bdg-gmt": AnnualPlanBudgetMappingGMT,
    "ap-mande-output": AnnualPlanMandEOutput,
    "ap-mande-outcobj": AnnualPlanMandEOutcObj,
    "ap-mande-overall": AnnualPlanMandEOverall,
    "ap-bdg-resource": AnnualPlanResource,
    "ap-overview": AnnualPlanOverview,
    "ap-mande-ccp": AnnualPlanMandECCP,
    "ap-mande-ccs": AnnualPlanMandECCS,
    "ap-mande-stratprocserv": AnnualPlanMandEStratProcServ,
  },
  props: {
    editItemMainData: {
      type: Object,
      default: undefined,
      required: false,
    },
    editItemNarrativeId: {
      type: Number,
      default: 0,
      required: false,
    },
    editItemOBJ: {
      type: Object,
      default: undefined,
      required: false,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },

  mixins: [UtilMix, NavMix],
  data() {
    return {
      switchLabel: "",
      designInProgess: false,
      rndVarOutp: 0,
      rndVarOutc: 0,
      indicatorType: null,
      showWait: false,
      //formDataOVG: {},
      localEditItemMainData: {},
      module: "",
      inChargeTo: "",
      actualStatus: 0,
      status: [],
      authorizedRole: false,
      role: 0,
      tblName: "PMS_PROJECT",
      leavePage: false,
      mainDataItems: null,
      serviceRes: null,
      tabIsVisible: null,
      subTabIsVisible: null,
      subTabBdgIsVisible: null,
      subTabPrjActIsVisible: null,
      tabsItems: [
        { id: 0, code: "MAINDATA", descr: "Main Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
      ],
      tabsItemsFull: [
        { id: 0, code: "MAINDATA", descr: "Main Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
        { id: 2, code: "MANDE", descr: "M&E", active: true },
        { id: 3, code: "BUDGET", descr: "Budget", active: true },
        { id: 4, code: "GNT", descr: "Grants", active: true },
      ],
      tabsItemsFullGnt: [
        { id: 0, code: "MAINDATA", descr: "Main Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
        { id: 2, code: "MANDE", descr: "M&E", active: true },
        { id: 3, code: "BUDGET", descr: "Budget", active: true },
        { id: 4, code: "GNT", descr: "Grants", active: true },
      ],
      tabsSubItems: [
        { id: 0, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 1, code: "OVG", descr: "Overall Goal", active: true },
        { id: 2, code: "OBJ", descr: "Outcome Objectives", active: true },
        { id: 3, code: "SSS", descr: "Strategic and Support Services", active: true },
        { id: 4, code: "CCP", descr: "Cross Cutting Processes", active: true },
        { id: 5, code: "CCS", descr: "Cross Cutting Services", active: true },
        // { id: 3, code: "CSP", descr: "Cross Cutting", active: true },
        { id: 6, code: "OVW", descr: "Overview", active: true },
      ],
      tabsSubBdgItems: [
        { id: 0, code: "RESOURCES", descr: "Resources", active: true },
        { id: 1, code: "BDG", descr: "Budget", active: true },
      ],

      genericQueryData: false,
      tmpSelectedRows: [],
      location_id: 0,
      profileId: null,
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setAnnualPlan: "setAnnualPlan",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    isAuthorized() {
      let localThis = this as any;
      if (localThis.module == "PMS") return true;
      else return false; 
    },

    getList() {
      (this as any).serviceRes = null;
      axios({
        url:
          "https://jesuitrefugeeservice720.sharepoint.com/teams/TEST_NM_01/Shared%20Documents/SP_TEST_01.txt",
        method: "GET",
        responseType: "json",
      })
        .then((response) => {
          (this as any).serviceRes = response;
          // var fileURL = window.URL.createObjectURL(new Blob([response.data]));
          // var fileLink = document.createElement("a");
          // fileLink.href = fileURL;
          // fileLink.setAttribute("download", "file.pdf");
          // document.body.appendChild(fileLink);
          // fileLink.click();
        })
        .catch((err) => {
          (this as any).serviceRes = err;
        });
    },
    testPrint(str: string) {
      //(str || "TEST");
    },

    testGetGenericSelect() {
      (this as any).genericQueryData = !(this as any).genericQueryData;
    },
    closeDialogMainData() {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          (this as any).leavePage = true;
          this.$router.push({
            name: "Annual Plan List",
          });
        } else {
          (this as any).leavePage = false;
        }
      });
    },
    closeDialogAndSaveMainData(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-add-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["id"] == undefined) {
        msg = msgAdd;
      } else {
        id = Number(value["id"]);
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          localThis.dialog = false;
          //(this as any).editedItem = (this as any).itemModel;
          //for (var key in value) {
          //    alert('keyy: ' + key + '\n' + 'value: ' + value[key]);
          //}
          const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
          const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);

          //ADD
          return api
            .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
            .then((response) => {
              //debugger
              //(this as any).getPMSAP();
              localThis.mainDataItems = value;
              localThis.mainDataItems.apcode = value.office_code + "_" + value.code;
              localThis.mainDataItems.status_in_bar = localThis.mainDataItems.status_name;
              let ap = {} as any;
              ap.annal_plan_id = value.id;
              ap.start_year = value.start_year;
              ap.currency = value.currency_code;
              ap.location_id = value.location_id;
              ap.location_description = value.location_description;
              localThis.setAnnualPlan(ap);
              localThis.showNewSnackbar({
                typeName: "success",
                text: "Succesfully updated Annual Plan main data.",
              });
            })
            .catch((error) => {
              alert(error);
            });
        }
      });
    },
  },
  beforeMount() {
    let localThis: any = this;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    if (localThis.isAuthorized()) {
      /*localThis.tabsItems= [
        { id: 0, code: "MAINDATA", descr: "Main Data", active: true },
        { id: 1, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 2, code: "OBJ", descr: "Objectives", active: true },
        { id: 3, code: "BMPMS", descr: "Budget", active: true }
      ];
    } else {
     localThis.tabsItems=[
        { id: 0, code: "MAINDATA", descr: "Main Data", active: true },
        { id: 1, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 2, code: "OBJ", descr: "Objectives", active: true },
        { id: 3, code: "GNT", descr: "Grants", active: true },
        { id: 4, code: "BMGMT", descr: "Budget Mapping", active: true },
  
      ]; */
    }
  },
  mounted() {
    let localThis = this as any;

    localThis.mainDataItems = localThis.localEditItemMainData;
    localThis.mainDataItems.status_in_bar = localThis.localEditItemMainData.status_name;
    localThis.localEditItemMainData.location_id = Number(
      localThis.mainDataItems.location_id
    );
    localThis.role = localThis.getCurrentRole.ROLE_ID;
    EventBus.$on("userRoleUpdated", (to: any) => {
      let localThis: any = this as any;
      localThis.authorizedRole = false;

      localThis.status = [];
      localThis.role = localThis.getCurrentRole.ROLE_ID;
      if (localThis.role) {
        localThis.getStatusList();
      } else {
        localThis.authorizedRole = false;
      }
      localThis.rndVarOutp = Math.ceil(Math.random() * 1000);
    });
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
    }),

    activeTabs: function () {
      let localThis = this as any;
      return localThis.tabsItemsFullGnt.filter(function (u: any) {
        return u.active;
      });
    },
    activeSubTabs: function () {
      let localThis = this as any;
      return localThis.tabsSubItems.filter(function (u: any) {
        return u.active;
      });
    },
    activeSubBdgTabs: function () {
      let localThis = this as any;
      return localThis.tabsSubBdgItems.filter(function (u: any) {
        return u.active;
      });
    },
  },
  beforeRouteLeave(to, from, next) {
    let localThis = this as any;
    let ap = {} as any;
    ap.annal_plan_id = 0;
    ap.start_year = 0;
    ap.end_year = 0;
    ap.currency_code = "";

    if (localThis.leavePage == false) {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.setAnnualPlan(ap);
          next();
        } else {
          next(false);
        }
      });
    } else {
      next();
    }
  },
});
</script>

<style scoped></style>
