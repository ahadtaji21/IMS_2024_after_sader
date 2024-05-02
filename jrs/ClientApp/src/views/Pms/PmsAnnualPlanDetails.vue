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
            <v-btn
              color="primary"
              class="--darken-1"
              @click="approve()"
              v-if="false && module != 'IMP'"
              >Approve</v-btn
            >
          </v-col>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
            <div class="text-center" v-if="showWait">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
            </div>
          </v-col>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"> </v-col>

          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
            <v-row :class="'my-border'">
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 5">
                <h6 style="text-align: left">Actual state:{{ switchLabel }}</h6>
                <v-switch
                  v-model="designInProgess"
                  @change="changeDesignState()"
                  hide-details
                  outlined
                  dense
                  v-if="isAuthorized() && module != 'IMP'"
                >
                  <template v-slot:label>
                    <span class="switchLbl">Switch State</span>
                  </template>
                </v-switch>
              </v-col>
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
              <v-tab v-for="item in activeTabs" :key="item.code">{{ item.descr }}</v-tab>
            </v-tabs>
            <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
              <v-tab-item key="MAINDATA">
                <v-card>
                  <v-card-title primary-title>Static Data</v-card-title>
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
                    <div v-if="module == 'IMP'">
                      <ap-obj
                        :selectedAnnualPlan="editItemOBJ"
                        :isUpdatableForm="isAuthorized()"
                        :onlyRead="!isAuthorized()"
                        :versioned="versioned"
                        :key="rndVarOutp"
                      ></ap-obj>
                    </div>
                    <div v-if="module != 'IMP'">
                      <ap-obj-casc
                        :selectedAnnualPlan="editItemOBJ"
                        :isUpdatableForm="isAuthorized()"
                        :onlyRead="!isAuthorized()"
                        :versioned="versioned"
                        :key="Math.ceil(Math.random() * 1000)"
                      ></ap-obj-casc>
                    </div>
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

                  <!-- Cross Cutting  -->
                  <v-tab-item key="CC">
                    <ap-cc
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-cc>
                  </v-tab-item>

                  <!-- Cross Cutting Process
                  <v-tab-item key="CCP">
                    <ap-ccp
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-ccp>
                  </v-tab-item>

                  <v-tab-item key="CCS">
                    <ap-ccs
                      :isUpdatableForm="isAuthorized()"
                      :onlyRead="!isAuthorized()"
                      :selectedAnnualPlanId="editItemOBJ.id"
                      :versioned="versioned"
                    ></ap-ccs>
                  </v-tab-item> -->
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
                  <v-radio label="Outcome Based Objective" value="outcome"></v-radio>
                  <v-radio label="Output" value="output"></v-radio>

                  <v-radio
                    label="Principles, values, criteria  cross cutting"
                    value="crosscutting"
                  ></v-radio>
                  <!-- <v-radio
                    label="Cross Cutting Process"
                    value="crosscuttingprocess"
                  ></v-radio>
                  <v-radio
                    label="Cross Cutting Service"
                    value="crosscuttingservice"
                  ></v-radio> -->
                  <v-radio
                    label="Strategic and support services"
                    value="stratprocserv"
                  ></v-radio>
                  <!-- <v-radio label="Overview" value="overview"></v-radio> -->
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    @click="viewOverview"
                  >
                    overview
                  </v-btn>
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
                <div v-if="indicatorType == 'crosscutting'">
                  <ap-mande-cc
                    :key="Math.ceil(Math.random() * 1000)"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :onlyRead="!isAuthorized()"
                    :versioned="versioned"
                  >
                  </ap-mande-cc>
                </div>

                <!--  <div v-if="indicatorType == 'crosscuttingprocess'">
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
                </div>-->
                <div v-if="indicatorType == 'stratprocserv'">
                  <ap-mande-stratprocserv
                    :key="Math.ceil(Math.random() * 1000)"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :onlyRead="!isAuthorized()"
                    :versioned="versioned"
                  >
                  </ap-mande-stratprocserv>
                </div>
                <div v-if="indicatorType == 'overview'">
                  <ap-mande-overview
                    :key="Math.ceil(Math.random() * 1000)"
                    :selectedAnnualPlanId="editItemOBJ.id"
                    :onlyRead="!isAuthorized()"
                    :versioned="versioned"
                  >
                  </ap-mande-overview>
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

              <v-tab-item key="PRJACT">
                <ap-acts-proc
                  :key="Math.ceil(Math.random() * 1000)"
                  :onlyRead="!isAuthorized()"
                  :selectedAnnualPlanId="editItemOBJ.id"
                  :versioned="versioned"
                ></ap-acts-proc>
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
import AnnualPlanMainData from "../../components/PMS/ANNUALPLAN/AnnualPlanMainDataForm.vue";
import AnnualPlanActivatedProcesses from "../../components/PMS/ANNUALPLAN/AnnualPlanActivatedProcessForm.vue";
import AnnualOverallGoal from "../../components/PMS/OVERALLGOAL/AnnualPlanOverallGoalForm.vue";
import AnnualSSS from "../../components/PMS/STRATSUPPSERV/AnnualPlanSSSForm.vue";
// import AnnualCCP from "../../components/PMS/CROSSCUTTING/AnnualPlanCCPForm.vue";
// import AnnualCCS from "../../components/PMS/CROSSCUTTING/AnnualPlanCCSForm.vue";
import AnnualCC from "../../components/PMS/CROSSCUTTING/AnnualPlanCCForm.vue";
import AnnualPlanOBJ from "../../components/PMS/OBJECTIVES/AnnualPlanObjectiveForm.vue";
import AnnualPlanOBJCasc from "../../components/PMS/CASCADE/AnnualPlanCascObjForm.vue";
import AnnualPlanBudgetMappingPMS from "../../components/PMS/BUDGET/AnnualPlanObjectiveBudgetDetails.vue";
import AnnualPlanResource from "../../components/PMS/RESOURCES/AnnualPlanResourceProspectForm.vue";
import TextEditor from "../../components/TextEditor.vue";
import AnnualPlanNarrative from "../../components/PMS/ANNUALPLAN/AnnualPlanNarrative.vue";
import AnnualPlanOverview from "../../components/PMS/ANNUALPLAN/AnnualPlanOverView.vue";
import JrsTable from "../../components/JrsTable.vue";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsModalForm from "../../components/JrsModalForm.vue";
import JrsLocationPicker from "../../components/JrsLocationPicker.vue";
import JrsSearchTable from "../../components/JrsSearchTable.vue";
import AnnualPlanMandEOutput from "../../components/PMS/MANDE/AnnualPlanMandEOutputForm.vue";
import AnnualPlanMandEOutcObj from "../../components/PMS/MANDE/AnnualPlanMandEOutcObjForm.vue";
import AnnualPlanMandEOverall from "../../components/PMS/MANDE/AnnualPlanMandEOverallForm.vue";
// import AnnualPlanMandECCP from "../../components/PMS/MANDE/AnnualPlanMandECCPForm.vue";
// import AnnualPlanMandECCS from "../../components/PMS/MANDE/AnnualPlanMandECCSForm.vue";
import AnnualPlanMandECC from "../../components/PMS/MANDE/AnnualPlanMandECCForm.vue";
import AnnualPlanMandEStratProcServ from "../../components/PMS/MANDE/AnnualPlanMandEStrattSuppServForm.vue";
import AnnualPlanMandEOverview from "../../components/PMS/MANDE/AnnualPlanMandeOverviewForm.vue";
import AnnualPlanGMT from "../../components/GMT/ANNUALPLAN/GRANTS/GMTProjectGrantsForm.vue";
import UtilMix from "../../mixins/UtilMix";
import NavMix from "../../mixins/NavMixin";
import {
  PmsAnnualPlanApi,
  ImsApi,
  Configuration,
  NavIntegrationApi,
  NavDimension1,
  NavBudget1,
  ImsLookupsApi,
  AnnualPlanDocApi,
  //AnnualPlanDocGeneratorApi,
} from "../../axiosapi";
import { EventBus } from "../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
//TMP
import { JrsHeader } from "../../models/JrsTable";
import { JrsFormField, JrsFormFieldText, JrsFormFieldSelect } from "../../models/JrsForm";
//TMP

export default Vue.extend({
  name: "home",
  components: {
    "ap-main-data": AnnualPlanMainData,
    "ap-narrative": AnnualPlanNarrative,
    "ap-obj-casc": AnnualPlanOBJCasc,
    "ap-obj": AnnualPlanOBJ,
    "ap-ovg": AnnualOverallGoal,
    "ap-sss": AnnualSSS,
    // "ap-ccp": AnnualCCP,
    // "ap-ccs": AnnualCCS,
    // "ap-csp": AnnualCrossProcess,
    "ap-cc": AnnualCC,
    "ap-bdg-pms": AnnualPlanBudgetMappingPMS,
    "ap-mande-output": AnnualPlanMandEOutput,
    "ap-mande-outcobj": AnnualPlanMandEOutcObj,
    "ap-mande-overall": AnnualPlanMandEOverall,
    "ap-mande-overview": AnnualPlanMandEOverview,
    "ap-bdg-resource": AnnualPlanResource,
    "ap-overview": AnnualPlanOverview,
    // "ap-mande-ccp": AnnualPlanMandECCP,
    // "ap-mande-ccs": AnnualPlanMandECCS,
    "ap-mande-cc": AnnualPlanMandECC,
    "ap-mande-stratprocserv": AnnualPlanMandEStratProcServ,
    "ap-acts-proc": AnnualPlanActivatedProcesses,
    "ap-gnt": AnnualPlanGMT,
  },
  props: {
    editItemMainData: {
      type: Object,
    },
    editItemNarrativeId: {
      type: Number,
    },
    editItemOBJ: {
      type: Object,
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
        { id: 0, code: "MAINDATA", descr: "Static Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
      ],
      tabsItemsFull: [
        { id: 0, code: "MAINDATA", descr: "Static Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
        { id: 2, code: "MANDE", descr: "M&E", active: true },
        { id: 3, code: "BUDGET", descr: "Budget", active: true },
      ],
      tabsItemsFullImplementation: [
        { id: 0, code: "MAINDATA", descr: "Static Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
        { id: 2, code: "MANDE", descr: "M&E", active: true },
        { id: 3, code: "BUDGET", descr: "Budget", active: true },
        { id: 4, code: "PRJACT", descr: "Activated Processes", active: true },
      ],
      tabsItemsFullGrant: [
        { id: 0, code: "MAINDATA", descr: "Static Data", active: true },
        { id: 1, code: "PB", descr: "Plan Builder", active: true },
        { id: 2, code: "MANDE", descr: "M&E", active: true },
        { id: 3, code: "BUDGET", descr: "Budget", active: true },
        { id: 4, code: "GNT", descr: "Grants", active: true },
      ],
      tabsSubItems: [
        { id: 0, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 1, code: "OVG", descr: "Overall Goal", active: true },
        { id: 2, code: "OBJ", descr: "Outcome Based Objectives", active: true },
        { id: 3, code: "SSS", descr: "Strategic and Support Services", active: true },
        // { id: 4, code: "CCP", descr: "Cross Cutting Processes", active: true },
        // { id: 5, code: "CCS", descr: "Cross Cutting Services", active: true },
        {
          id: 4,
          code: "CC",
          descr: "Principles, values, criteria  cross cutting",
          active: true,
        },
        // { id: 3, code: "CSP", descr: "Cross Cutting", active: true },
        { id: 5, code: "OVW", descr: "Overview", active: true },
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
      execSPCall: "execSPCall",
    }),
    viewOverview() {
      let localThis = this as any;
      localThis.indicatorType = "overview";
    },

    isAuthorized() {
      let localThis = this as any;

      if (
        (localThis.module == "PMS" || localThis.module == "IMP") &&
        localThis.versioned == 0
      ) {
        return localThis.authorizedRole;
      } else {
        return false;
      }
    },
    save() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-all-update-confirm")
        .toString();
      this.$confirm(msgUpd).then((res) => {
        if (res) {
          EventBus.$emit("saveMainDataFromMainSave");
          EventBus.$emit("saveObjectiveFromMainSave");
          EventBus.$emit("saveNarrativeFromMainSave");
          EventBus.$emit("saveServiceFromMainSave");
          EventBus.$emit("saveOutputFromMainSave");
        }
      });
    },
    approve() {
      let localThis = this as any;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let annual_plan_id = ap.annal_plan_id;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-approve")
        .toString();
      this.$confirm(msgUpd).then((res) => {
        if (res) {
          let payLoadD = {} as any;

          payLoadD.ID = annual_plan_id;
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_UPD_ANNUAL_PLAN_APPROVE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.leavePage = true;
              this.$router.push({
                name: "Annual Plan List",
              });
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    GenerateWord() {
      let localThis = this as any;
      var id: number = 0;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      id = ap.annal_plan_id;
      if (id == undefined || id == 0) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "error generating the document",
        });
        return;
      }
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: AnnualPlanDocApi = new AnnualPlanDocApi(config);
      localThis.showWait = true;
      let doc: string;
      return api
        .apiAnnualPlanDocIdGet(id, localThis.versioned, config.baseOptions)
        .then((res) => {
          doc = res.data;
          const byteCharacters = atob(doc);
          const byteNumbers = new Array(byteCharacters.length);
          for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
          }
          const byteArray = new Uint8Array(byteNumbers);
          const blob = new Blob([byteArray], { type: "application/msword" });

          var link = document.createElement("a");

          var url = URL.createObjectURL(blob);
          link.setAttribute("href", url);
          link.setAttribute("download", "annualplan.doc");
          link.style.visibility = "hidden";
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        })
        .catch((err) => {
          // console.error(err);
          return "";
        })
        .finally(() => (localThis.showWait = false));
    },
    // GenerateWord() {
    //   let localThis = this as any;
    //   var id: number = 0;
    //   let ap = {} as any;
    //   ap = localThis.$store.getters.getAnnualPlan;
    //   id = ap.annal_plan_id;
    //   if (id == undefined || id == 0) {
    //     localThis.showNewSnackbar({
    //       typeName: "error",
    //       text: "error generating the document",
    //     });
    //     return;
    //   }
    //   const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
    //   const api: AnnualPlanDocGeneratorApi = new AnnualPlanDocGeneratorApi(config);
    //   localThis.showWait = true;
    //   let doc: string;
    //   return api
    //     .apiAnnualPlanDocGeneratorIdGet(id, localThis.versioned, config.baseOptions)
    //     .then((res: any) => {
    //       doc = res.data;
    //       const byteCharacters = atob(doc);
    //       const byteNumbers = new Array(byteCharacters.length);
    //       for (let i = 0; i < byteCharacters.length; i++) {
    //         byteNumbers[i] = byteCharacters.charCodeAt(i);
    //       }
    //       const byteArray = new Uint8Array(byteNumbers);
    //       const blob = new Blob([byteArray], { type: "application/msword" });

    //       var link = document.createElement("a");

    //       var url = URL.createObjectURL(blob);
    //       link.setAttribute("href", url);
    //       link.setAttribute("download", "annualplan.doc");
    //       link.style.visibility = "hidden";
    //       document.body.appendChild(link);
    //       link.click();
    //       document.body.removeChild(link);
    //     })
    //     .catch((err) => {
    //       // console.error(err);
    //       return "";
    //     })
    //     .finally(() => (localThis.showWait = false));
    // },
    changeDesignState() {
      let localThis = this as any;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let annual_plan_id = ap.annal_plan_id;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.upd-design-status")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.DESIGN_STATUS = localThis.designInProgess == false ? "0" : "1";
          payLoadD.ID = annual_plan_id;
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_UPD_ANNUAL_PLAN_DESIGN_STATUS",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        } else {
          localThis.designInProgess = !localThis.designInProgess;
        }
      });
    },

    getInChargeTo() {
      let localThis: any = this as any;

      localThis.inChargeTo = "";
      var id: number = 0;
      if (
        localThis.localEditItemMainData != undefined &&
        localThis.localEditItemMainData.id != undefined
      ) {
        id = localThis.localEditItemMainData.id;
      }
      let view: string = "IMS_VI_ANNUAL_PLAN_STATUS_LIST";
      let wherecond: string = `(${id}=0 OR ID=${id})`;
      if (localThis.versioned > 0) {
        view = "IMS_VI_ANNUAL_PLAN_STATUS_LIST_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      if (localThis.versioned == true) view = "IMS_VI_ANNUAL_PLAN_STATUS_LIST_VER";
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          if (res.table_data) {
            //Setup data
            res.table_data.forEach((item: any) => {
              localThis.inChargeTo = item.IMS_USER_ROLE_DESCR;
            });
          }
        }
      );
    },
    indicators() {
      let localThis: any = this as any;
      if (localThis.indicatorType == "output")
        localThis.rndVarOutp = Math.ceil(Math.random() * 1000);
      else localThis.rndVarOuc = Math.ceil(Math.random() * 1000);
      return true;
    },
    getStatusList() {
      let localThis: any = this as any;

      localThis.status = [];
      //if (localThis.idObject == 0) return;
      var st_id: number = 0;
      if (
        localThis.localEditItemMainData != undefined &&
        localThis.localEditItemMainData.status_id != undefined
      ) {
        st_id = localThis.localEditItemMainData.status_id;
      }

      var id: number = 0;
      if (
        localThis.localEditItemMainData != undefined &&
        localThis.localEditItemMainData.id != undefined
      ) {
        id = localThis.localEditItemMainData.id;
      }
      let view: string = "IMS_VI_ANNUAL_PLAN_STATUS_LIST";
      let wherecond: string = `(${id}=0 OR ID=${id}) AND (${localThis.role}=0 OR ROLE_ID=${localThis.role}) AND (${st_id}=0 OR STATUS_ID=${st_id})`;
      if (localThis.versioned > 0) {
        view = "IMS_VI_ANNUAL_PLAN_STATUS_LIST_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            var localStatus: any = [];
            //Setup data
            res.table_data.forEach((item: any) => {
              if (id == 0) {
                if (item.IMS_STATUS_DESCR.indexOf("draft") != -1) {
                  let st: any = {};
                  st.text = item.IMS_STATUS_DESCR;
                  st.value = item.NEXT_STATUS_ID + "";
                  localStatus.push(st);
                }
              } else {
                let st: any = {};
                st.text = item.IMS_STATUS_DESCR;
                st.value = item.NEXT_STATUS_ID + "";
                localStatus.push(st);
              }
            });
            localThis.status = localStatus;
          }
        })
        .then(async (res) => {
          if (localThis.role != 0 && localThis.status.length > 1) {
            localThis.authorizedRole = true;
          } else {
            localThis.authorizedRole = false;
          }
          await localThis.getInChargeTo();
        });
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
        value.guid = localThis.makeUUID();
      } else {
        id = Number(value["id"]);
        if (value.guid == undefined || value.guid == "")
          value.guid = localThis.makeUUID();
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
          return (
            api
              .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
              .then((response) => {
                //localThis.updateBudgetOnNav(value);
                //debugger
                //(this as any).getPMSAP();
                localThis.localEditItemMainData.status_id = value.status_id;
                localThis.mainDataItems = value;
                localThis.mainDataItems.apcode = value.office_code + "_" + value.code;
                localThis.mainDataItems.status_in_bar =
                  localThis.mainDataItems.status_name;
                let ap = {} as any;
                ap.annal_plan_id = value.id;
                ap.start_year = value.start_year;
                ap.end_year = value.end_year;
                ap.currency = value.currency_code;
                // ap.location_id = value.location_id;
                // ap.location_description = value.location_description;
                localThis.setAnnualPlan(ap);
                EventBus.$emit("userRoleUpdated");
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: "Succesfully updated Annual Plan static data.",
                });
              })
              // .then((res) => {
              //   value.dimensionCode="PROJECT";
              //   localThis.updateNavDimension(value);
              // })
              .catch((err: any) => {
                localThis.showNewSnackbar({ typeName: "error", text: err.response.data });
              })
          );
        }
      });
    },
    closeDialogAndSaveMainDataFromMain(value: any) {
      let localThis = this as any;

      let id = 0;

      id = Number(value["id"]);
      if (value.guid == undefined || value.guid == "") value.guid = localThis.makeUUID();

      localThis.dialog = false;
      //(this as any).editedItem = (this as any).itemModel;
      //for (var key in value) {
      //    alert('keyy: ' + key + '\n' + 'value: ' + value[key]);
      //}
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);

      //ADD
      return (
        api
          .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
          .then((response) => {
            //localThis.updateBudgetOnNav(value);
            //debugger
            //(this as any).getPMSAP();
            localThis.localEditItemMainData.status_id = value.status_id;
            localThis.mainDataItems = value;
            localThis.mainDataItems.apcode = value.office_code + "_" + value.code;
            localThis.mainDataItems.status_in_bar = localThis.mainDataItems.status_name;
            let ap = {} as any;
            ap.annal_plan_id = value.id;
            ap.start_year = value.start_year;
            ap.end_year = value.end_year;
            ap.currency = value.currency_code;
            // ap.location_id = value.location_id;
            // ap.location_description = value.location_description;
            localThis.setAnnualPlan(ap);
            EventBus.$emit("userRoleUpdated");
            localThis.showNewSnackbar({
              typeName: "success",
              text: "Succesfully updated Annual Plan static data.",
            });
          })
          // .then((res) => {
          //   value.dimensionCode="PROJECT";
          //   localThis.updateNavDimension(value);
          // })
          .catch((err: any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err.response.data });
          })
      );
    },

    // updateBudgetOnNavApi(nvbList: NavBudget1[]) {
    //   let localThis = this as any;
    //   const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
    //   const api: NavIntegrationApi = new NavIntegrationApi(config);
    //   let nvbListStr: string;
    //   nvbListStr = JSON.stringify(nvbList);
    //   var options = {
    //     timeout: 1000000,
    //   };
    //   return (
    //     api
    //       //.apiNavGLBudgetPutGet(undefined)
    //       .apiNavGLBulkBudgetPutGet(nvbListStr)
    //       .catch((err: any) => {
    //         localThis.showNewSnackbar({ typeName: "error", text: err.response.data });
    //       })
    //   );
    // },
    // updateBudgetOnNav(value: any) {
    //   if (value.status_id == "12") {
    //     //approved by RO
    //     let localThis = this as any;

    //     let nvbList = [] as NavBudget1[];
    //     let nvb = {} as NavBudget1;

    //     let view: string = "PMS_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET_DETAILS_FOR_NAV_V1";
    //     let wherecond: string = `AP_ID = ${value.id}`;
    //     let selectPayload: GenericSqlSelectPayload = {
    //       viewName: view,
    //       colList: null,
    //       whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
    //       orderStmt: null,
    //     };

    //     return localThis
    //       .getGenericSelect({ genericSqlPayload: selectPayload })
    //       .then((res: any) => {
    //         //Setup data
    //         if (res.table_data) {
    //           res.table_data.forEach((item: any) => {
    //             if (item.VALUE != undefined) {
    //               let nvb = {} as NavBudget1;
    //               nvb.pMBDGNAME = "BUDGET";
    //               nvb.pMACCNUM = item.PMS_JRS_COA_CODE;
    //               nvb.pMDATE = ""; // DateTime.Now.ToString("yyyy-MM-dd");
    //               nvb.pMGBLDIM1CODE = item.HR_OFFICE_CODE + item.CODE;
    //               nvb.pMGBLDIM2CODE = ""; //LEGATO A GMT

    //               nvb.pMAMOUNT = item.VALUE;
    //               nvb.pMDESCRIPTION = item.PMS_JRS_COA_DESCRIPTION;

    //               nvb.pMBDGDIM1CODE = "IT";
    //               nvb.pMBDGDIM2CODE = "GSC";
    //               nvb.pMBDGDIM3CODE = item.PMS_COI_CODE;
    //               nvb.pMBDGDIM4CODE = item.PMS_TOS_CODE;

    //               nvb.pMMEASUREUNITCODE = item.UNIT_TYPE;
    //               nvb.pMENTRYNMB =
    //                 (item.AP_ID + "").padEnd(5, "0") +
    //                 (item.OBJ_ID + "").padEnd(6, "0") +
    //                 (item.SRV_ID + "").padEnd(6, "0") +
    //                 (item.ACT_ID + "").padEnd(6, "0") +
    //                 (item.PMS_COI_ID + "").padEnd(6, "0") +
    //                 (item.PMS_TOS_ID + "").padEnd(6, "0") +
    //                 (item.PMS_JRS_COA_ID + "").padEnd(6, "0") +
    //                 (item.YEAR + "").padEnd(4, "0") +
    //                 (item.MONTH + "").padStart(2, "0");
    //               nvb.pMQUANTITY = item.UNIT;
    //               nvb.pMUNITCOST = item.UNIT_PRICE;
    //               //nvb.pMENTRYNMB = "2";
    //               nvb.pMLASTMODDT = ""; //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    //               nvb.pMDELETED = "false";

    //               nvb.pMCOMPANY = item.HR_OFFICE_CODE;
    //               nvbList.push(nvb);
    //             }
    //           });
    //         }
    //         return nvbList;
    //       })
    //       .then((res1: any) => {
    //         if (nvbList.length > 0) {
    //          // localThis.updateBudgetOnNavApi(nvbList); //.then((res2: any) => {
    //           return;
    //           // });
    //         }
    //       });
    //   } else {
    //     return "";
    //   }
    // },
  },

  beforeMount() {
    let localThis: any = this;
    localThis.module = localThis.getActiveModule.moduleCode;


    localThis.role = localThis.getCurrentRole.ROLE_ID;
    localThis.designInProgess =
      localThis.editItemMainData.design_in_progress == "0" ? false : true;
    localThis.localEditItemMainData = JSON.parse(
      JSON.stringify(localThis.editItemMainData)
    );
    localThis.actualStatus = localThis.localEditItemMainData.status_id;
    if (localThis.status.length == undefined || localThis.status.length == 0)
      localThis.getStatusList().then(function () {
        return;
      });

    if (localThis.isAuthorized()) {
      /*localThis.tabsItems= [
        { id: 0, code: "MAINDATA", descr: "Static Data", active: true },
        { id: 1, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 2, code: "OBJ", descr: "Objectives", active: true },
        { id: 3, code: "BMPMS", descr: "Budget", active: true }
      ];
    } else {
     localThis.tabsItems=[
        { id: 0, code: "MAINDATA", descr: "Static Data", active: true },
        { id: 1, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 2, code: "OBJ", descr: "Objectives", active: true },
        { id: 3, code: "GNT", descr: "Grants", active: true },
        { id: 4, code: "BMGMT", descr: "Budget Mapping", active: true },

      ]; */
    }
    // localThis.formDataOVG = {} as any;
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    let annual_plan_id = ap.annal_plan_id;

    // let view: string = "PMS_OVERALL_GOAL";
    // let wherecond: string = `PMS_AP_ID = ${annual_plan_id}`;
    // if (localThis.versioned > 0) {
    //   view = "PMS_OVERALL_GOAL_VER";
    //   wherecond += ` AND VERSION_ID=${localThis.versioned}`;
    // }
    // let selectPayload: GenericSqlSelectPayload = {
    //   viewName: view,
    //   colList: null,
    //   whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
    //   orderStmt: null,
    // };

    // return localThis
    //   .getGenericSelect({ genericSqlPayload: selectPayload })
    //   .then((res: any) => {
    //     //Setup data
    //     if (res.table_data) {
    //       res.table_data.forEach((item: any) => {
    //         localThis.formDataOVG.PMS_TARGET_ID_STR = item.PMS_TARGET_ID + "";
    //         localThis.formDataOVG.OVERALL_GOAL = [] as any;
    //         localThis.formDataOVG.OVERALL_GOAL[0] = {} as any;
    //         localThis.formDataOVG.OVERALL_GOAL[0].PMS_CONSTRUCTION_DEF_ID =
    //           item.PMS_OVG_ID;
    //       });
    //     }
    //   });
    if (localThis.versioned > 0) localThis.designInProgess = false;
  },
  mounted() {
    let localThis = this as any;

    localThis.mainDataItems = localThis.localEditItemMainData;
    localThis.mainDataItems.status_in_bar = localThis.localEditItemMainData.status_name;
    localThis.localEditItemMainData.location_id = Number(
      localThis.mainDataItems.location_id
    );
    localThis.role = localThis.getCurrentRole.ROLE_ID;

    EventBus.$on("annualPlanStatusUpdated", async (status: any) => {
      let localThis: any = this as any;
      localThis.localEditItemMainData.status_id = status;
      localThis.authorizedRole = false;

      localThis.status = [];
      localThis.role = localThis.getCurrentRole.ROLE_ID;
      if (localThis.role) {
        await localThis.getStatusList();
      } else {
        localThis.authorizedRole = false;
      }
      localThis.rndVarOutp = Math.ceil(Math.random() * 1000);
    });
    EventBus.$on("userRoleUpdated", async (to: any) => {
      let localThis: any = this as any;
      localThis.authorizedRole = false;

      localThis.status = [];
      localThis.role = localThis.getCurrentRole.ROLE_ID;
      if (localThis.role) {
        await localThis.getStatusList();
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
      getCurrentRole: "getCurrentRole",
    }),

    activeTabs: function () {
      let localThis = this as any;
      console.log("localThis.module: " , localThis.module);
      switch (localThis.module) {
        case "IMP":
          localThis.switchLabel = this.$i18n
            .t("datasource.pms.annual-plan.objectives.design-completed")
            .toString();
          return localThis.tabsItemsFullImplementation.filter(function (u: any) {
            return u.active;
          });

        case "Grants":
          localThis.switchLabel = this.$i18n
            .t("datasource.pms.annual-plan.objectives.design-completed")
            .toString();
          return localThis.tabsItemsFullGrant.filter(function (u: any) {
            return u.active;
          });

        default:
          //"PME"
          if (localThis.designInProgess == false) {
            localThis.switchLabel = this.$i18n
              .t("datasource.pms.annual-plan.objectives.design-completed")
              .toString();
            return localThis.tabsItemsFull.filter(function (u: any) {
              return u.active;
            });
          } else {
            localThis.switchLabel = this.$i18n
              .t("datasource.pms.annual-plan.objectives.design-in-progress")
              .toString();
            return localThis.tabsItems.filter(function (u: any) {
              return u.active;
            });
          }

        // if (localThis.designInProgess == false && localThis.module == "PME") {
        //   localThis.switchLabel = this.$i18n
        //     .t("datasource.pms.annual-plan.objectives.design-completed")
        //     .toString();
        //   return localThis.tabsItemsFull.filter(function (u: any) {
        //     return u.active;
        //   });
        // } else {
        //   if (localThis.designInProgess == false && localThis.module == "IMP") {
        //     localThis.switchLabel = this.$i18n
        //       .t("datasource.pms.annual-plan.objectives.design-completed")
        //       .toString();
        //     return localThis.tabsItemsFullImplementation.filter(function (u: any) {
        //       return u.active;
        //     });
        //   } else {
        //     if (localThis.designInProgess == true) {
        //       localThis.switchLabel = this.$i18n
        //         .t("datasource.pms.annual-plan.objectives.design-in-progress")
        //         .toString();
        //       return localThis.tabsItems.filter(function (u: any) {
        //         return u.active;
        //       });
        //     } else {
        //       localThis.switchLabel = this.$i18n
        //         .t("datasource.pms.annual-plan.objectives.design-in-progress")
        //         .toString();
        //       return localThis.tabsItems.filter(function (u: any) {
        //         return u.active;
        //       });
        //     }
        //   }
        // }
      }
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

<style scoped>
.my-border {
  border-left: 1px solid gainsboro;
}
.switchLbl {
  display: block;
  font-size: 0.67em;
  margin-top: 0em;
  margin-bottom: 0em;
  margin-left: 0;
  margin-right: 0;
  font-weight: bold;
}
</style>
