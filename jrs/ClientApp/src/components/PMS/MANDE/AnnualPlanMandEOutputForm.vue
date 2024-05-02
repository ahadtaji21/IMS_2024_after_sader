<template>
  <div>
    <div>
      <!-- SRV SELECTION-->
      <v-row v-if="showWait">
        <v-col>
          <div class="text-center" v-if="showWait">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headers"
            :items="objectiveList"
            item-key="PMS_ACTIVITY_OUTPUT_REL_ID"
            sort-by="['objective', 'service']"
            multi-sort
            :search="tableFilter"
            :items-per-page="1000"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedObj"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $tc("datasource.pms.annual-plan.objectives.outputs", 2) }}</h4>

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
              <tr v-for="(item, index) in items" :key="item.PMS_ACTIVITY_OUTPUT_REL_ID">
                <td :class="'descClass'">
                  <v-checkbox
                    v-model="selected[index]"
                    value
                    hide-details
                    class="shrink ma-0 pa-0"
                    @change="
                      checkboxUpdated(
                        $event,
                        index,
                        item.PMS_ACTIVITY_OUTPUT_REL_ID,
                        item.PMS_OUTPUT_ID
                      )
                    "
                  ></v-checkbox>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.OBJ_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.OBJ_DESC }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COI_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COI_CODE }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'" v-if="false">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COI_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COI_DESCRIPTION }}</span>
                  </v-tooltip>
                </td>

                <td :class="'descClass'" v-if="false">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TOS_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TOS_CODE }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TOS_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TOS_DESCRIPTION }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_PROCESS | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_PROCESS }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_OUTPUT | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_OUTPUT }}</span>
                  </v-tooltip>
                </td>
                <!-- <td :class="'descClass'">
                  <span>{{ item.PMS_OUTPUT_VALUE }}</span>
                </td> -->
                <td :class="'descClass'">
                  <span>{{ item.IND_NUMBER }}</span>
                </td>

                <!-- <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_IND_NUMBER | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_IND_NUMBER }}</span>
                  </v-tooltip>
                </td> -->
              </tr>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <div v-if="showIndicators">
      <ap-outp-indicators
        :onlyRead="onlyRead"
        :formData="formData"
        :showItemDetails="true"
        :versioned="versioned"
        @reloadOutputList="reloadActivities"
        :key="keyRnd"
      >
      </ap-outp-indicators>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import Indicators from "../OUTPUTS/AnnualPlanOutputIndicatorForm.vue";
const Papa = require("papaparse");
const fs = require("fs");
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "ap-outp-indicators": Indicators,
  },
  props: {
    selectedAnnualPlanId: {
      type: Number,
      required: true,
    },
    // showActivityDetails: {
    //   type: Boolean,
    //   required: true,
    // },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },

  data() {
    return {
      index: 0,
      selected: [],
      showWait: false,
      keyRnd: 0,
      bdg: 0,
      resourceId: {},
      json_data: "",
      selectedActivityId: 0,
      selectedActivityItemId: 0,
      selectedResource: "",
      selectedResourceId: 0,
      editObjId: 0,
      showActTabs: true,
      editObjDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showIndicators: false,
      selectedObj: [],
      tableFilter: "",
      tblName: null,
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
      objectiveList: [] as any,
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
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
    },

    /*   interface objectItemExport {
  OBJECTIVE: string;
  SERVICE: string;
  ACTIVITY: string;
  "CATEGORY OF INT CODE": string;
  "CATEGORY OF INT DESC": string;
  "TYPE OF SERVICE CODE": string;
  "TYPE OF SERVICE DESC": string;
  "CHART OF ACC. CODE": string;
  "CHART OF ACC. DESC": string;
  "CHART OF ACC. CUSTOM DESC": string;
  JAN: string;
  FEB: string;
  MAR: string;
  APR: string;
  MAY: string;
  JUN: string;
  JUL: string;
  AUG: string;
  SEP: string;
  OCT: string;
  NOV: string;
  DEC: string;
}
*/
    checkboxUpdated(newValue: any, index: number, rel_id: number, out_id: number) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showIndicators = newValue;
        if (newValue) {
          localThis.formData.selectedOutputId = out_id;
          //localThis.objectiveList[index].PMS_OUTPUT_ID;
          localThis.formData.selectedProcessOutputRelId = rel_id;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      }
    },
    reloadActivities(item: any) {
      let localThis = this as any;

      localThis.getAnnualPlanObjectives();
    },

    getAnnualPlanObjectives() {
      let localThis: any = this as any;
      localThis.objectiveList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTP_M_AND_E";
      let wherecond: string = `AP_ID = ${localThis.selectedAnnualPlanId} `;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTP_M_AND_E_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "OBJ_ID,SRV_ID,ACT_ID,PMS_COI_ID,PMS_TOS_ID,PROCESS_ID,OUTPUT_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let objItem = {} as any;
              objItem.PMS_ACTIVITY_OUTPUT_REL_ID = item.PMS_ACTIVITY_OUTPUT_REL_ID;
              objItem.AI_ID = item.AI_ID;
              objItem.OBJ_ID = item.OBJ_ID;
              objItem.SRV_ID = item.SRV_ID;
              objItem.ACT_ID = item.ACT_ID;
              objItem.PMS_COI_ID = item.PMS_COI_ID;
              objItem.PMS_TOS_ID = item.PMS_TOS_ID;
              objItem.IND_NUMBER = item.IND_NUMBER;

              objItem.OBJ_DESC = item.OBJ_DESC + "";

              objItem.PMS_COI_CODE = item.PMS_COI_CODE + "";
              objItem.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION + "";
              objItem.PMS_TOS_CODE = item.PMS_TOS_CODE + "";
              objItem.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION + "";
              objItem.PMS_PROCESS = item.PROCESS + "";
              objItem.PMS_OUTPUT = item.OUTPUT + "";
              objItem.PMS_OUTPUT_VALUE = item.OUTPUT_VALUE + "";
              objItem.PMS_PROCESS_ID = item.PROCESS_ID;
              objItem.PMS_OUTPUT_ID = item.OUTPUT_ID;

              localThis.objectiveList.push(objItem);
              localThis.selected.push(false);
            });
          }
          localThis.showWait = false;
        }
      );
    },

    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },
    editActivities(item: any) {
      let localThis: any = this as any;
      localThis.selectedObj = [];
      localThis.selectedObj.push(item);
    },

    removeUndefined: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string;
    },

    setupHeaders() {
      let localThis: any = this as any;
      let descWidth: string;
      descWidth = "140px";

      let tmp: JrsHeader[] = [
        {
          text: "",
          value: "",
          align: "center",
          divider: true,
          width: "20px",
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.objective")
            .toString(),
          value: "OBJ_DESC",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-code").toString(),
          value: "PMS_COI_CODE",
          align: "center",
          divider: true,
          width: descWidth,
        },
        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-desc").toString(),
        //   value: "PMS_COI_DESCRIPTION",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },

        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.tos-code").toString(),
        //   value: "PMS_TOS_CODE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.tos-desc").toString(),
          value: "PMS_TOS_DESCRIPTION",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.activity").toString(),
          value: "PMS_PROCESS",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.output").toString(),
          value: "PMS_OUTPUT",
          align: "center",
          divider: true,
          width: descWidth,
        },
        // {
        //   text: this.$i18n.t("pms.output-value").toString(),
        //   value: "PMS_OUTPUT_VALUE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.ind_number")
            .toString(),
          value: "IND_NUMBER",
          align: "center",
          divider: true,
          width: descWidth,
        },
      ];

      localThis.headers = tmp;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
  },
  mounted() {
    let localThis: any = this as any;

    localThis.tblName = "PMS_ACTIVITY";
    localThis.setupHeaders();
    if (localThis.selectedAnnualPlanId > 0) {
      localThis.getAnnualPlanObjectives();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.reloadActivities();
      EventBus.$emit("showObjTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadActivities(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 25) return string.substring(0, 25) + "...";
      else return string;
    },
  },
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;

      localThis.getAnnualPlanObjectives();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showObjDetails = to;
    // },
    selectedAnnualPlanId(to: number) {
      let localThis: any = this as any;
      localThis.selectedAnnualPlanId = to;

      localThis.getAnnualPlanObjectives();
    },

    editMode(to: boolean, from: boolean) {},
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
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
.selectedCell {
  background-color: whitesmoke;
  color: black;
  font-size: 12px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
  padding: 0px;
  margin: 0px;
}
.notSelectedCell {
  background-color: white;
  font-size: 1px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
}

.descClass {
  font-size: 10px;
  width: 150px !important;
  height: 10px;
}
</style>
