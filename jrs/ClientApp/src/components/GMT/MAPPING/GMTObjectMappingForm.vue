<template>
  <div>
    <div v-if="!showSrvDetails">
      <!-- OBJ SELECTION-->

      <v-data-table
        :headers="headers"
        :items="objectList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="100"
        style="{'font-size':'14px','width':'80px' }"
        class="text-capitalize"
        v-model="selectedObj"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{$t('datasource.pms.annual-plan.objectives.objectives')}}</h3>

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
                <v-card-title>Objective: {{ editObjDesc }}</v-card-title>
                <v-card-text>
                  <srv-budget :selectedObjectId="editObjId" :key="Math.ceil(Math.random() * 1000)"></srv-budget>
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
              <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editActivities(item);" v-on="on">mdi-chart-line</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-service-activities') }}</span>
              </v-tooltip>-->
              <!-- <v-tooltip bottom>
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
import ServiceBudget from "./GMTServiceMappingForm.vue";
import MappingCell from "./GMTMappingCell.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface OBJECT_MONTH {
  SELECTED: boolean;
  REQUIRED_VALUE: any;
  ALLOCATED_VALUE: any;
  ALLOCATED_PERCENTAGE: any;
  CURRENCY: string;
}
interface objectItem {
  ID: number;
  OBJECT_ID: number;
  DESCRIPTION: string;
  YEAR: number;
  JAN: OBJECT_MONTH;
  FEB: OBJECT_MONTH;
  MAR: OBJECT_MONTH;
  APR: OBJECT_MONTH;
  MAY: OBJECT_MONTH;
  JUN: OBJECT_MONTH;
  JUL: OBJECT_MONTH;
  AUG: OBJECT_MONTH;
  SEP: OBJECT_MONTH;
  OCT: OBJECT_MONTH;
  NOV: OBJECT_MONTH;
  DEC: OBJECT_MONTH;
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

    "srv-budget": ServiceBudget,
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
      editObjId: 0,
      budgetMode: false,
      showActTabs: true,
      editObjDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showSrvDetails: false,
      selectedObj: [],
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
      objectList: [] as objectItem[],
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

      localThis.getAnnualPlanObjects();
    },
    getAnnualPlanObjects() {
      let localThis: any = this as any;
      localThis.selectedObjct = null;
      localThis.objectList = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET",
        colList: null,
        whereCond: `ANNP_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "OBJ_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let i: number = 0;
              let k: number = -1;
              for (i = 0; i < localThis.objectList.length; i++) {
                if (localThis.objectList[i].ID == item.OBJ_ID) {
                  k = i;
                  break;
                }
              }
              if (k == -1) {
                let objItem = {} as objectItem;
                objItem.ID = item.OBJ_ID;
                objItem.DESCRIPTION = item.DESCRIPTION;

                objItem.OBJECT_ID = item.OBJ_ID;
                objItem.YEAR = item.YEAR;
                switch (item.MONTH) {
                  case 1:
                    objItem.JAN = {} as OBJECT_MONTH;
                    objItem.JAN = localThis.GetMonth(objItem.JAN, item);
                    break;
                  case 2:
                    objItem.FEB = {} as OBJECT_MONTH;
                    objItem.FEB = localThis.GetMonth(objItem.FEB, item);
                    break;
                  case 3:
                    objItem.MAR = {} as OBJECT_MONTH;
                    objItem.MAR = localThis.GetMonth(objItem.MAR, item);
                    break;
                  case 4:
                    objItem.APR = {} as OBJECT_MONTH;
                    objItem.APR = localThis.GetMonth(objItem.APR, item);
                    break;
                  case 5:
                    objItem.MAY = {} as OBJECT_MONTH;
                    objItem.MAY = localThis.GetMonth(objItem.MAY, item);
                    break;
                  case 6:
                    objItem.JUN = {} as OBJECT_MONTH;
                    objItem.JUN = localThis.GetMonth(objItem.JUN, item);
                    break;
                  case 7:
                    objItem.JUL = {} as OBJECT_MONTH;
                    objItem.JUL = localThis.GetMonth(objItem.JUL, item);
                    break;
                  case 8:
                    objItem.AUG = {} as OBJECT_MONTH;
                    objItem.AUG = localThis.GetMonth(objItem.AUG, item);
                    break;
                  case 9:
                    objItem.SEP = {} as OBJECT_MONTH;
                    objItem.SEP = localThis.GetMonth(objItem.SEP, item);
                    break;
                  case 10:
                    objItem.OCT = {} as OBJECT_MONTH;
                    objItem.OCT = localThis.GetMonth(objItem.OCT, item);
                    break;
                  case 11:
                    objItem.NOV = {} as OBJECT_MONTH;
                    objItem.NOV = localThis.GetMonth(objItem.NOV, item);
                    break;
                  case 12:
                    objItem.DEC = {} as OBJECT_MONTH;
                    objItem.DEC = localThis.GetMonth(objItem.DEC, item);
                    break;
                }

                localThis.objectList.push(objItem);
              } else {
                switch (item.MONTH) {
                  case 1:
                    localThis.objectList[k].JAN = {} as OBJECT_MONTH;
                    localThis.objectList[k].JAN = localThis.GetMonth(
                      localThis.objectList[k].JAN,
                      item
                    );
                    break;

                  case 2:
                    localThis.objectList[k].FEB = {} as OBJECT_MONTH;
                    localThis.objectList[k].FEB = localThis.GetMonth(
                      localThis.objectList[k].FEB,
                      item
                    );
                    break;
                  case 3:
                    localThis.objectList[k].MAR = {} as OBJECT_MONTH;
                    localThis.objectList[k].MAR = localThis.GetMonth(
                      localThis.objectList[k].MAR,
                      item
                    );
                    break;
                  case 4:
                    localThis.objectList[k].APR = {} as OBJECT_MONTH;
                    localThis.objectList[k].APR = localThis.GetMonth(
                      localThis.objectList[k].APR,
                      item
                    );
                    break;
                  case 5:
                    localThis.objectList[k].MAY = {} as OBJECT_MONTH;
                    localThis.objectList[k].MAY = localThis.GetMonth(
                      localThis.objectList[k].MAY,
                      item
                    );
                    break;
                  case 6:
                    localThis.objectList[k].JUN = {} as OBJECT_MONTH;
                    localThis.objectList[k].JUN = localThis.GetMonth(
                      localThis.objectList[k].JUN,
                      item
                    );
                    break;
                  case 7:
                    localThis.objectList[k].JUL = {} as OBJECT_MONTH;
                    localThis.objectList[k].JUL = localThis.GetMonth(
                      localThis.objectList[k].JUL,
                      item
                    );
                    break;
                  case 8:
                    localThis.objectList[k].AUG = {} as OBJECT_MONTH;
                    localThis.objectList[k].AUG = localThis.GetMonth(
                      localThis.objectList[k].AUG,
                      item
                    );
                    break;
                  case 9:
                    localThis.objectList[k].SEP = {} as OBJECT_MONTH;
                    localThis.objectList[k].SEP = localThis.GetMonth(
                      localThis.objectList[k].SEP,
                      item
                    );
                    break;
                  case 10:
                    localThis.objectList[k].OCT = {} as OBJECT_MONTH;
                    localThis.objectList[k].OCT = localThis.GetMonth(
                      localThis.objectList[k].OCT,
                      item
                    );
                    break;
                  case 11:
                    localThis.objectList[k].NOV = {} as OBJECT_MONTH;
                    localThis.objectList[k].NOV = localThis.GetMonth(
                      localThis.objectList[k].NOV,
                      item
                    );
                    break;
                  case 12:
                    localThis.objectList[k].DEC = {} as OBJECT_MONTH;
                    localThis.objectList[k].DEC = localThis.GetMonth(
                      localThis.objectList[k].DEC,
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
    GetMonth(obj: OBJECT_MONTH, item: OBJECT_MONTH) {
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
      localThis.getAnnualPlanObjects();
      localThis.budgetMode = false;
    },

    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.budgetMode = true;
      localThis.editObjId = item.ID;
      localThis.editObjDesc = item.DESCRIPTION;
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
      localThis.getAnnualPlanObjects();
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
      localThis.getAnnualPlanObjects();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showSrvDetails = to;
    // },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getAnnualPlanObjects();
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