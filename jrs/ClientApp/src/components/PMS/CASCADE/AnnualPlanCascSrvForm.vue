<template>
  <div>
    <div v-if="!showSrvDetails">
      <v-row align-center v-if="showWait">
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
          <!-- SRV SELECTION-->
          <v-data-table
            :headers="headers"
            :items="serviceList"
            item-key="ID"
            multi-sort
            :search="tableFilter"
            :items-per-page="itemsPerPage"
            class="text-capitalize"
            v-model="selectedSrv"
            :key="Math.ceil(Math.random() * 1000)"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $t("datasource.pms.annual-plan.objectives.services") }}</h3>

                <v-spacer></v-spacer>
                <span v-if="!onlyRead">
                  <v-dialog
                    v-model="editMode"
                    persistent
                    retain-focus
                    :scrollable="true"
                    :overlay="false"
                    :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
                    transition="dialog-transition"
                  >
                    <template v-slot:activator="{ on }">
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                        v-on="on"
                        @click="AddNewSrv()"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>ADD SERVICE
                      </v-btn>
                    </template>
                    <pms-addsrv-form
                      @closeDialoge="closeDialog"
                      @closeEdit="closeDialog"
                      :selectedsrvList="serviceList"
                      :key="rndVar"
                      @updateServicesrvList="updateServicesrvList"
                      @refreshObjSrv="getObjectiveServices"
                      :formDataExt="editedItemDialog"
                      :selectedObjectId="selectedObjectId"
                      :selectedCategoryId="selectedCategoryId"
                      :selectedTargetId="editedItemDialog.PMS_TARGET_ID"
                    ></pms-addsrv-form>
                  </v-dialog>
                </span>
                <v-spacer></v-spacer>

                <v-text-field
                  v-model="tableFilter"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field>
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody style="border: solid">
                <tr v-for="(item, index) in items" :key="item.ID" style="height: 10px">
                  <td class="tablerow">
                    <v-checkbox
                      v-model="selected[index]"
                      value
                      hide-details
                      class="shrink ma-0 pa-0"
                      @change="checkboxUpdated($event, index, item.ID, item)"
                    ></v-checkbox>
                  </td>
                  <td class="tablerow">{{ item.PMS_COI_CODE }}</td>
                  <td class="tablerow">{{ item.TITLE }}</td>
                  <td class="tablerow">{{ item.COI_LABEL }}</td>
                  <td class="tablerow">{{ item.PMS_TOS_DESCRIPTION }}</td>
                  <td v-if="module == 'IMP'" class="tablerow">
                    {{ item.ACTIVATION_DATE | formatDate }}
                  </td>
                  <td v-if="module == 'IMP'" class="tablerow">
                    {{ item.END_DATE | formatDate }}
                  </td>
                  <td class="tablerow">{{ item.SERVED_PEOPLE }}</td>

                  <td
                    v-if="module == 'IMP' && item.ACTIVATED == 0"
                    class="tablerow"
                  >
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon color="red" small class="mr-2" v-on="on"
                          >mdi-circle</v-icon
                        >
                      </template>
                      <span>{{
                        $t(
                          "datasource.pms.annual-plan.objectives.srv-implementation-status-no"
                        )
                      }}</span>
                    </v-tooltip>
                  </td>

                  <td
                    v-if="module == 'IMP' && item.ACTIVATED > 0"
                    class="tablerow"
                  >
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon color="green" small class="mr-2" v-on="on"
                          >mdi-circle</v-icon
                        >
                      </template>
                      <span>{{
                        $t(
                          "datasource.pms.annual-plan.objectives.srv-implementation-status-yes"
                        )
                      }}</span>
                    </v-tooltip>
                  </td>
                  <td style="text-align: center">
                    <span
                      v-if="
                        !onlyRead && module == 'IMP' && item.ACTIVATED == 0
                      "
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon
                            color="black"
                            small
                            class="mr-2"
                            @click="activateService(item)"
                            v-on="on"
                            >mdi-play</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.activate-service")
                        }}</span>
                      </v-tooltip>
                    </span>
                    <span
                      v-if="
                        !onlyRead && module == 'IMP' && item.ACTIVATED == 1
                      "
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon
                            color="black"
                            small
                            class="mr-2"
                            @click="activateService(item)"
                            v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.service-activated")
                        }}</span>
                      </v-tooltip>
                    </span>
                    <span v-if="!onlyRead">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                            >mdi-circle-edit-outline</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.edit-service")
                        }}</span>
                      </v-tooltip>
                    </span>
                    <span v-if="onlyRead">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                            >mdi-eye</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.edit-service")
                        }}</span>
                      </v-tooltip>
                    </span>
                    <span v-if="!onlyRead">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                            >mdi-delete</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.delete-service")
                        }}</span>
                      </v-tooltip>
                    </span>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
      <v-row v-if="showProcesses">
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
          <proc-form
            :onlyRead="onlyRead"
            :selectedObjectId="selectedServiceId"
            :showActivityDetails="true"
            :formDataExt="formDataCasc"
            :versioned="versioned"
            :key="keyRnd"
          >
          </proc-form>
        </v-col>
      </v-row>
    </div>
    <div>
      <!-- <v-row v-if="selectedService && selectedService > 0">
        <v-col :cols="12">
          <div>
            <div v-if="showItemNumber==1">
              <ap-narrative :selectedObjectId="selectedService" :tableName="tblName"></ap-narrative>
            </div>
            <div v-else-if="showItemNumber==2">
              <ap-indicator :selectedObjectId="selectedService"></ap-indicator>
            </div>
          </div>
        </v-col>
      </v-row>-->
      <div v-if="showSrvDetails">
        <ap-srv-details
          :editSrvMainDataItem="formData"
          :key="Math.ceil(Math.random() * 1000)"
          :editSrvId="editId"
          :editSrvDesc="editSrvDesc"
          :showSrvDetails="showSrvDetails"
          :selectedObjectId="selectedObjectId"
          :isUpdatableForm="isUpdatableForm"
          :onlyRead="onlyRead"
          :versioned="versioned"
          :cascade="true"
        ></ap-srv-details>
      </div>
      <!-- <v-dialog
        v-model="activateMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
        transition="dialog-transition"
      >
        <v-card>
          <pms-activate-form
            :formDataInput="formDataActivation"
            @closeActivation="closeActivation"
            @closeActDialog="closeActDialog"
            :onlyRead="!onlyRead && module == 'PMS' && selectedActivated == 1"
            :key="keyActivation"
          ></pms-activate-form>
        </v-card>
      </v-dialog> -->
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsTable from "../../../components/JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import Processes from "./AnnualPlanCascProcForm.vue";
import SearchTable from "./AnnualPlanCascSrvSearch.vue";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import Indicator from "../INDICATORS/AnnualPlanIndicatorForm.vue";
import ServiceDetails from "../SERVICES/AnnualPlanServiceDetailsForm.vue";
import ServiceMainData from "../SERVICES/AnnualPlanServiceMainDataForm.vue";
//import ActivateForm from "./AnnualPlanServiceActivateForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import UtilMix from "../../../mixins/UtilMix";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

// interface ServiceData {
//   ID: number | null;
//   COI: {} | null;
//   TOS: {} | null;
//   DATE_FROM: Date | null;
//   DATE_TO: Date | null;
//   OCCURRENCE: {} | null;
//   OBJECTIVE_ID: number | null;
// }

export default Vue.extend({
  components: {
    // "jrs-table": JrsTable,

    "ap-srv-details": ServiceDetails,
    //"pms-addsrv-form": ServiceMainData,
    "pms-addsrv-form": SearchTable,
    "proc-form": Processes,
    //"pms-activate-form": ActivateForm,
  },
  mixins: [UtilMix],
  props: {
    formDataInput: {
      type: Object,
      required: true,
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
    showServiceDetails: {
      type: Boolean,
      required: true,
    },
    isUpdatableForm: {
      type: Boolean,
      required: true,
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

    selectedCategoryId: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      showWait: false,
      formDataCasc: {},
      keyRnd: 0,
      showProcesses: false,
      selectedServiceId: 0,
      selected: [],
      selectedActivated: 0,
      keyActivation: 0,
      formDataActivation: {},
      activateMode: false,
      module: "",
      rndVar: 0,
      showSrvTabs: true,
      editSrvDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showSrvDetails: false,

      itemsPerPage: -1,
      showItemNumber: 0,
      selectedSrv: [],
      tableFilter: "",
      tblName: null,
      selectedService: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      serviceList: [],

      occ: [],
    };
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    uncheckList() {
      let localThis = this as any;
      let i: number = 0;
      localThis.showProcesses = false;
      if (localThis.serviceList == undefined || localThis.serviceList.length == 0) return;
      for (i = 0; i < localThis.serviceList.length; i++) {
        if (localThis.selected != undefined && localThis.selected[i] != undefined) {
          localThis.selected[i] = false;
        }
      }
    },
    checkboxUpdated(newValue: any, index: number, srv_id: number, item: any) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showProcesses = newValue;
        if (newValue) {
          localThis.fillFormData(item);
          localThis.selectedServiceId = srv_id;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
          localThis.showProcesses = true;
        }
      }
    },
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    reloadServices(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = -1;
      localThis.getObjectiveServices();
    },

    updateServicesrvList(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = -1;
      localThis.editMode = false;
      localThis.UpdateItems(item);
      //localThis.getObjectiveServices();
    },

    UpdateItems(item: any) {
      if (item == undefined) return;
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      let i: number = 0;
      let j: number = 0;
      localThis.showWait = true;
      for (i = 0; i < item.length; i++) {
        payLoadD.OBJECTIVE_ID = localThis.selectedObjectId;
        payLoadD.COI_TOS_REL_ID = item[i].PMS_COI_TOS_ID;
        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        const response = localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            j++;
            if (j == item.length) {
              localThis.showWait = false;
              localThis.getObjectiveServices();
            }
            return res;
          })
          .catch((err: any) => {
            localThis.showWait = false;
            localThis.generateError(err);
            throw err;
          });
      }
      localThis.itemsPerPage = -1;

      localThis.showNewSnackbar({ typeName: "success", text: "Service Updated" }); // Feedback to user
    },

    refreshSrvFormData(value: any) {
      let localThis: any = this as any;
      localThis.formData = value;
    },
    AddNewSrv() {
      let localThis: any = this as any;
      localThis.editMode = !localThis.editMode;
      localThis.editedItemDialog = {} as any;
      localThis.editedItemDialog.PMS_TARGET_ID = localThis.formDataInput.PMS_TARGET_ID;
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    getNow: function () {
      let localThis: any = this as any;
      const today = new Date();
      const date =
        today.getFullYear() + "-" + (today.getMonth() + 1) + "-" + today.getDate();

      localThis.formData.DATE_FROM = date;
      localThis.formData.DATE_TO = date;
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;
          localThis.showWait = true;
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_SERVICE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showWait = false;
              if (res != undefined && (res + "").indexOf('"status":"error"') != -1) {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text:
                    "The service can't be deleted; Delete the connected resources first",
                });
              } else {
                localThis.itemsPerPage = -1;
                localThis.reloadServices();
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: res.message,
                }); // Feedback to user
              }
            })
            .catch((err: any) => {
              localThis.showWait = false;
              localThis.generateError(err);
            });
        }
      });
    },
    editIndicators(item: any) {
      let localThis: any = this as any;
      localThis.selectedSrv = [];
      localThis.selectedService = item.ID;
      localThis.selectedSrv.push(item);
      localThis.showItemNumber = 2;
    },

    closeActivation() {
      let localThis: any = this as any;
      localThis.activateMode = false;
      localThis.reloadServices();
    },
    closeActDialog() {
      let localThis: any = this as any;
      localThis.activateMode = false;
    },
    activateService(item: any) {
      let localThis: any = this as any;
      localThis.activateMode = true;
      localThis.selectedActivated = item.ACTIVATED;
      if (item.ACTIVATION_DATE == undefined) {
        localThis.formDataActivation.START_DATE = item.START_DATE;
      } else {
        localThis.formDataActivation.START_DATE = item.ACTIVATION_DATE;
      }
      localThis.formDataActivation.ACTIVATION_NOTE = item.ACTIVATION_NOTE;
      localThis.keyActivation = Math.ceil(Math.random() * 1000);
      localThis.formDataActivation.ID = item.ID;
      localThis.formDataActivation.OCCURRENCE = {};
      localThis.formDataActivation.OCCURRENCE.ID = item.OC_ID;
      localThis.formDataActivation.OCCURRENCE.TYPE = item.OC_TYPE;
    },

    fillFormData(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);

      localThis.formDataCasc = {};
      localThis.formDataCasc.PMS_TARGET_ID = item.PMS_TARGET_ID;
      localThis.formDataCasc.DATE_FROM = item.START_DATE;
      localThis.formDataCasc.DATE_TO = item.END_DATE;
      localThis.formDataCasc.ID = item.ID;
      localThis.formDataCasc.OBJECTIVE_ID = item.OBJECTIVE_ID;
      localThis.formDataCasc.COI = {};
      localThis.formDataCasc.COI.PMS_COI_ID = item.PMS_COI_ID;
      localThis.formDataCasc.COI.IMS_LABELS_VALUE = item.PMS_COI_DESCRIPTION;
      localThis.formDataCasc.COI.PMS_COI_CODE = item.PMS_COI_CODE;
      localThis.formDataCasc.TOS = {};
      localThis.formDataCasc.TOS.PMS_TOS_ID = item.PMS_TOS_ID;
      localThis.formDataCasc.TOS.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION;
      localThis.formDataCasc.TOS.PMS_COI_TOS_ID = item.PMS_COI_TOS_ID;
      localThis.formDataCasc.PMS_TARGET_ID = item.PMS_TARGET_ID;
      localThis.formDataCasc.OCCURRENCE = {};
      localThis.formDataCasc.OCCURRENCE.ID = item.OC_ID;
      localThis.formDataCasc.OCCURRENCE.TYPE = item.OC_TYPE;
      localThis.formDataCasc.LOCATION_ID = item.LOCATION_ID;
      localThis.formDataCasc.LOCATION_DETAILS = item.LOCATION_DETAILS;
      localThis.formDataCasc.TITLE = item.TITLE;
      localThis.formDataCasc.SERVED_PEOPLE = item.SERVED_PEOPLE;
      localThis.formDataCasc.ACTIVATED = item.ACTIVATED;
    },
    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedSrv = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      localThis.editSrvDesc = item.PMS_COI_CODE + "-" + item.PMS_TOS_DESCRIPTION + " ";
      localThis.showSrvDetails = true;
      localThis.formData.PMS_TARGET_ID = item.PMS_TARGET_ID;
      localThis.formData.DATE_FROM = item.START_DATE;
      localThis.formData.DATE_TO = item.END_DATE;
      localThis.formData.ID = item.ID;
      localThis.formData.OBJECTIVE_ID = item.OBJECTIVE_ID;
      localThis.formData.COI = {};
      localThis.formData.COI.PMS_COI_ID = item.PMS_COI_ID;
      localThis.formData.COI.IMS_LABELS_VALUE = item.PMS_COI_DESCRIPTION;
      localThis.formData.COI.PMS_COI_CODE = item.PMS_COI_CODE;
      localThis.formData.TOS = {};
      localThis.formData.TOS.PMS_TOS_ID = item.PMS_TOS_ID;
      localThis.formData.TOS.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION;
      localThis.formData.TOS.PMS_COI_TOS_ID = item.PMS_COI_TOS_ID;
      localThis.formData.PMS_TARGET_ID = item.PMS_TARGET_ID;
      localThis.formData.OCCURRENCE = {};
      localThis.formData.OCCURRENCE.ID = item.OC_ID;
      localThis.formData.OCCURRENCE.TYPE = item.OC_TYPE;
      localThis.formData.LOCATION_ID = item.LOCATION_ID;
      localThis.formData.LOCATION_DETAILS = item.LOCATION_DETAILS;
      localThis.formData.TITLE = item.TITLE;
      localThis.formData.SERVED_PEOPLE = item.SERVED_PEOPLE;
      localThis.formData.ACTIVATED = item.ACTIVATED;
      localThis.selectedSrv.push(item);
      localThis.serviceList = [];

      EventBus.$emit("hideObjecTabs");
      localStorage.SelectedSrv = localThis.editSrvDesc;
      EventBus.$emit("refreshBreadCumb", 2);
    },
    editNarrative(item: any) {
      let localThis: any = this as any;
      localThis.selectedSrv = [];
      localThis.selectedService = item.ID;
      localThis.selectedSrv.push(item);
      localThis.showItemNumber = 1;
    },

    getObjectiveServices() {
      let localThis: any = this as any;
      localThis.selectedService = null;
      localThis.serviceList = [];
      localThis.selected = [];
      localThis.showProcesses = false;
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_SERVICE";
      let wherecond: string = `OBJECTIVE_ID = ${localThis.selectedObjectId} AND PMS_COI_ID= ${localThis.selectedCategoryId} AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_SERVICE_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      localThis.showWait = true;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          localThis.showWait = false;
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.serviceList.push(item);
            });
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.generateError(err);
        });
    },

    setupHeaders() {
      let localThis = this as any;
      let tmp: JrsHeader[];
      if (localThis.module == "IMP") {
        tmp = [
          {
            text: this.$i18n.t("general.code").toString(),
            value: "PMS_COI_CODE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n.t("general.title").toString(),
            value: "TITLE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n
              .t("datasource.pms.category-of-intervention.category-of-intervention")
              .toString(),
            value: "COI_LABEL",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n
              .t("datasource.pms.type-of-service.type-of-service")
              .toString(),
            value: "PMS_TOS_DESCRIPTION",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n.t("general.date-start").toString(),
            value: "START_DATE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n.t("general.date-end").toString(),
            value: "END_DATE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n
              .t("datasource.pms.annual-plan.objectives.est-srv-nbr")
              .toString(),
            value: "SERVED_PEOPLE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n
              .t("datasource.pms.annual-plan.objectives.imp-status")
              .toString(),
            align: "center",
            value: "ACTIVATION_STATE",
            sortable: true,
            filterable: true,
          },
          {
            text: "Actions",
            value: "action",
            align: "center",
            sortable: false,
          },
        ];
      } else {
        tmp = [
          {
            text: "",
            value: "",
            align: "center",
            divider: true,
            width: "20px",
          },
          {
            text: this.$i18n.t("general.code").toString(),
            value: "PMS_COI_CODE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n.t("general.title").toString(),
            value: "TITLE",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n
              .t("datasource.pms.category-of-intervention.category-of-intervention")
              .toString(),
            value: "COI_LABEL",
            align: "center",
            divider: true,
          },
          {
            text: this.$i18n
              .t("datasource.pms.annual-plan.objectives.service")
              .toString(),
            value: "PMS_TOS_DESCRIPTION",
            align: "center",
            divider: true,
          },

          {
            text: this.$i18n
              .t("datasource.pms.annual-plan.objectives.est-srv-nbr")
              .toString(),
            value: "SERVED_PEOPLE",
            align: "center",
            divider: true,
          },
          {
            text: "Actions",
            value: "action",
            align: "center",
            sortable: false,
          },
        ];
      }

      localThis.headers = tmp;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
  },

  mounted() {
    let localThis: any = this as any;
    localThis.selectedService = null;
    localThis.tblName = "PMS_SERVICE";
    localThis.setupHeaders();
    localThis.getNow();
    localThis.editSrvDesc = localThis.editedItem.PMS_COI_CODE;
    if (localThis.selectedObjectId > 0) {
      localThis.getObjectiveServices();
    }

    EventBus.$on("reloadSelectedServices", (data: any) => {
      localThis.formData = data;
    });

    EventBus.$on("closeServiceDetails", (data: any) => {
      localThis.showSrvDetails = false;
      localThis.reloadServices();
      EventBus.$emit("showObjecTabs");
      localThis.uncheckList();
    });

    EventBus.$on("reloadServices", (data: any) => {
      localThis.reloadServices(data);
    });
    EventBus.$on("locationSaved", (id: Number, desc: String) => {
      let localThis: any = this as any;
      localThis.formData.LOCATION_ID = id;
      localThis.formData.LOCATION_DETAILS = desc;
    });
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
    language() {
      return i18n.locale;
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getObjectiveServices();
      localThis.setupHeaders();
    },
    showServiceDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showSrvDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getObjectiveServices();
    },
    editMode(to: boolean, from: boolean) {
      // let localThis: any = this as any;
      // // Initial load if inserting new data
      // if (to == true && !localThis.coi.PMS_COI_ID) {
      //   localThis.isLoading = true;
      //   localThis.setDatasets();
      // }
    },
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
.narrative-section {
  margin-bottom: 1em;
}
.overall-node {
  margin: 1em 0.5em;
}
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.selectedRow {
  background-color: red;
  font-weight: bold;
}
.tablerow {
  text-align: center;
  height: 3.5em;
  font-size: 12px;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
</style>
