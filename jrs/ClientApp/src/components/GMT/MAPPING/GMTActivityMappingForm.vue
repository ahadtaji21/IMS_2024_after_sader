<template>
  <div>
    <div v-if="!showSrvDetails">
      <!-- SRV SELECTION-->

      <v-data-table
        :headers="headers"
        :items="activityList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="100"
        style="{'font-size':'14px','width':'80px' }"
        class="text-capitalize"
        v-model="selectedSrv"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{$t('datasource.pms.annual-plan.objectives.activities')}}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="budgetMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <v-card>
                <v-card-title>{{ editActDesc }}</v-card-title>
                <v-card-text>
                  <ap-budget :selectedObjectId="editActId" :key="Math.ceil(Math.random() * 1000)"></ap-budget>
                </v-card-text>
                <v-card-actions>
                  <v-btn
                    color="secondary darken-1"
                    text
                    @click="closeBudget"
                  >X {{ $t('general.close') }}</v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>

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
          <tbody>
            <tr v-for="item in items" :key="item.ID" style="height:10px">
              <td style="height:10px">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <span v-on="on">{{ item.DESCRIPTION | subStr}}</span>
                  </template>
                  <span>{{ item.DESCRIPTION }}</span>
                </v-tooltip>
              </td>

              <td
                :class="item.JAN !=undefined && item.JAN.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.JAN"></mp-cell>
              </td>
              <td
                :class="item.FEB !=undefined && item.FEB.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.FEB"></mp-cell>
              </td>
              <td
                :class="item.MAR !=undefined && item.MAR.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.MAR"></mp-cell>
              </td>
              <td
                :class="item.APR !=undefined && item.APR.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.APR"></mp-cell>
              </td>
              <td
                :class="item.MAY !=undefined && item.MAY.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.MAY"></mp-cell>
              </td>
              <td
                :class="item.JUN !=undefined && item.JUN.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.JUN"></mp-cell>
              </td>
              <td
                :class="item.JUL !=undefined && item.JUL.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.JUL"></mp-cell>
              </td>
              <td
                :class="item.AUG !=undefined && item.AUG.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.AUG"></mp-cell>
              </td>
              <td
                :class="item.SEP !=undefined && item.SEP.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.SEP"></mp-cell>
              </td>
              <td
                :class="item.OCT !=undefined && item.OCT.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.OCT"></mp-cell>
              </td>
              <td
                :class="item.NOV !=undefined && item.NOV.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.NOV"></mp-cell>
              </td>
              <td
                :class="item.DEC !=undefined && item.DEC.SELECTED ? 'selectedCell' : 'notSelectedCell'"
              >
                <mp-cell :cellObject="item.DEC"></mp-cell>
              </td>
              <td>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editBudget(item);" v-on="on">mdi-wallet</v-icon>
                  </template>
                  <span>{{ $t('datasource.pms.annual-plan.objectives.budget-item') }}</span>
                </v-tooltip>
<!-- 
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on">mdi-delete</v-icon>
                  </template>
                  <span>{{ $t('datasource.pms.annual-plan.objectives.delete-service') }}</span>
                </v-tooltip> -->
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../../components/JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import ServiceBudget from "./GMTMappingCell.vue";
import MappingCell from "./GMTMappingCell.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface ACTIVITY_MONTH {
  SELECTED: boolean;
  REQUIRED_VALUE: any;
  ALLOCATED_VALUE: any;
  ALLOCATED_PERCENTAGE: any;
  CURRENCY: string;
}
interface activityIyem {
  ID: number;
  ACTIVITY_ID: number;
  DESCRIPTION: string;
  YEAR: number;
  JAN: ACTIVITY_MONTH;
  FEB: ACTIVITY_MONTH;
  MAR: ACTIVITY_MONTH;
  APR: ACTIVITY_MONTH;
  MAY: ACTIVITY_MONTH;
  JUN: ACTIVITY_MONTH;
  JUL: ACTIVITY_MONTH;
  AUG: ACTIVITY_MONTH;
  SEP: ACTIVITY_MONTH;
  OCT: ACTIVITY_MONTH;
  NOV: ACTIVITY_MONTH;
  DEC: ACTIVITY_MONTH;
}

// interface ActivityData {
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

    "ap-budget": ServiceBudget,
    "mp-cell": MappingCell,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    // showActivityDetails: {
    //   type: Boolean,
    //   required: true,
    // },
  },

  data() {
    return {
      editActId: 0,
      budgetMode: false,
      showActTabs: true,
      editActDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showSrvDetails: false,
      selectedSrv: [],
      tableFilter: "",
      selectedObjct: null,
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
      activityList: [] as activityIyem[],
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

    reloadActivities(item: any) {
      let localThis = this as any;

      localThis.getServiceActivities();
    },
    getServiceActivities() {
      let localThis: any = this as any;
      localThis.selectedObjct = null;
      localThis.activityList = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ANNUAL_PLAN_SERVICE_ACTIVITY_BUDGET",
        colList: null,
        whereCond: `SERVICE_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let i: number = 0;
              let k: number = -1;
              for (i = 0; i < localThis.activityList.length; i++) {
                if (localThis.activityList[i].ID == item.ID) {
                  k = i;
                  break;
                }
              }
              if (k == -1) {
                let actItem = {} as activityIyem;
                actItem.ID = item.ID;

                actItem.DESCRIPTION = item.DESCRIPTION;

                actItem.ACTIVITY_ID = item.ID;
                actItem.YEAR = item.YEAR;
                switch (item.MONTH) {
                  case 1:
                    actItem.JAN = {} as ACTIVITY_MONTH;
                    actItem.JAN = localThis.GetMonth(actItem.JAN, item);
                    break;
                  case 2:
                    actItem.FEB = {} as ACTIVITY_MONTH;
                    actItem.FEB = localThis.GetMonth(actItem.FEB, item);
                    break;
                  case 3:
                    actItem.MAR = {} as ACTIVITY_MONTH;
                    actItem.MAR = localThis.GetMonth(actItem.MAR, item);
                    break;
                  case 4:
                    actItem.APR = {} as ACTIVITY_MONTH;
                    actItem.APR = localThis.GetMonth(actItem.APR, item);
                    break;
                  case 5:
                    actItem.MAY = {} as ACTIVITY_MONTH;
                    actItem.MAY = localThis.GetMonth(actItem.MAY, item);
                    break;
                  case 6:
                    actItem.JUN = {} as ACTIVITY_MONTH;
                    actItem.JUN = localThis.GetMonth(actItem.JUN, item);
                    break;
                  case 7:
                    actItem.JUL = {} as ACTIVITY_MONTH;
                    actItem.JUL = localThis.GetMonth(actItem.JUL, item);
                    break;
                  case 8:
                    actItem.AUG = {} as ACTIVITY_MONTH;
                    actItem.AUG = localThis.GetMonth(actItem.AUG, item);
                    break;
                  case 9:
                    actItem.SEP = {} as ACTIVITY_MONTH;
                    actItem.SEP = localThis.GetMonth(actItem.SEP, item);
                    break;
                  case 10:
                    actItem.OCT = {} as ACTIVITY_MONTH;
                    actItem.OCT = localThis.GetMonth(actItem.OCT, item);
                    break;
                  case 11:
                    actItem.NOV = {} as ACTIVITY_MONTH;
                    actItem.NOV = localThis.GetMonth(actItem.NOV, item);
                    break;
                  case 12:
                    actItem.DEC = {} as ACTIVITY_MONTH;
                    actItem.DEC = localThis.GetMonth(actItem.DEC, item);
                    break;
                }

                localThis.activityList.push(actItem);
              } else {
                switch (item.MONTH) {
                  case 1:
                    localThis.activityList[k].JAN = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].JAN = localThis.GetMonth(
                      localThis.activityList[k].JAN,
                      item
                    );
                    break;

                  case 2:
                    localThis.activityList[k].FEB = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].FEB = localThis.GetMonth(
                      localThis.activityList[k].FEB,
                      item
                    );
                    break;
                  case 3:
                    localThis.activityList[k].MAR = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].MAR = localThis.GetMonth(
                      localThis.activityList[k].MAR,
                      item
                    );
                    break;
                  case 4:
                    localThis.activityList[k].APR = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].APR = localThis.GetMonth(
                      localThis.activityList[k].APR,
                      item
                    );
                    break;
                  case 5:
                    localThis.activityList[k].MAY = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].MAY = localThis.GetMonth(
                      localThis.activityList[k].MAY,
                      item
                    );
                    break;
                  case 6:
                    localThis.activityList[k].JUN = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].JUN = localThis.GetMonth(
                      localThis.activityList[k].JUN,
                      item
                    );
                    break;
                  case 7:
                    localThis.activityList[k].JUL = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].JUL = localThis.GetMonth(
                      localThis.activityList[k].JUL,
                      item
                    );
                    break;
                  case 8:
                    localThis.activityList[k].AUG = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].AUG = localThis.GetMonth(
                      localThis.activityList[k].AUG,
                      item
                    );
                    break;
                  case 9:
                    localThis.activityList[k].SEP = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].SEP = localThis.GetMonth(
                      localThis.activityList[k].SEP,
                      item
                    );
                    break;
                  case 10:
                    localThis.activityList[k].OCT = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].OCT = localThis.GetMonth(
                      localThis.activityList[k].OCT,
                      item
                    );
                    break;
                  case 11:
                    localThis.activityList[k].NOV = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].NOV = localThis.GetMonth(
                      localThis.activityList[k].NOV,
                      item
                    );
                    break;
                  case 12:
                    localThis.activityList[k].DEC = {} as ACTIVITY_MONTH;
                    localThis.activityList[k].DEC = localThis.GetMonth(
                      localThis.activityList[k].DEC,
                      item
                    );
                    break;
                }
              }
            });
          }
        }
      );
    },
    GetMonth(obj: ACTIVITY_MONTH, item: ACTIVITY_MONTH) {
      obj.SELECTED = true;
      obj.ALLOCATED_VALUE = item.ALLOCATED_VALUE;
      obj.REQUIRED_VALUE = item.REQUIRED_VALUE;
      obj.ALLOCATED_PERCENTAGE = item.ALLOCATED_PERCENTAGE;
      obj.CURRENCY = item.CURRENCY;
      return obj;
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
      localThis.selectedObjct = item.ID;
      localThis.selectedObj.push(item);
    },

    closeBudget() {
      let localThis: any = this as any;
      localThis.getServiceActivities();
      localThis.budgetMode = false;
    },

    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.budgetMode = true;
      localThis.editActId = item.ID;
      localThis.editActDesc = item.DESCRIPTION;
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("general.name").toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.jan")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.feb")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.mar")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.apr")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.may")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.jun")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.jul")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.aug")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.sep")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.oct")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.nov")
            .toString(),
          value: "",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.dec")
            .toString(),
          value: "",
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

      localThis.headers = tmp;
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedObjct = null;
    localThis.setupHeaders();
    if (localThis.selectedObjectId > 0) {
      localThis.getServiceActivities();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.showSrvDetails = false;
      localThis.reloadActivities();
      EventBus.$emit("showSrvTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadActivities(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      return string.substring(0, 15) + "...";
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
      localThis.getServiceActivities();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showSrvDetails = to;
    // },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getServiceActivities();
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
  width: 80px;
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
  width: 80px;
}
.valuedBudget {
  font-size: 12px;
  width: 80px;
}
</style>