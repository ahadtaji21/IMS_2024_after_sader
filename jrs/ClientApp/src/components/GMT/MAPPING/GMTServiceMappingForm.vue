<template>
  <div>
    <div v-if="!showSrvDetails">
      <!-- SRV SELECTION-->

      <v-data-table
        :headers="headers"
        :items="serviceList"
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
            <h3>{{$t('datasource.pms.annual-plan.objectives.services')}}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="budgetMode"
              persistent
              retain-focus
              :scrollable="false"
              :overlay="false"
              transition="dialog-transition"
            >
              <v-card>
                <v-card-title>Service:{{ editSrvDesc }}</v-card-title>
                <v-card-text>
                  <ap-budget :selectedObjectId="editSrvId" :key="Math.ceil(Math.random() * 1000)"></ap-budget>
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
                    <span v-on="on">{{ item.CODE | subStr}}</span>
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
import ActivityBudget from "./GMTActivityMappingForm.vue";
import MappingCell from "./GMTMappingCell.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface SERVICE_MONTH {
  SELECTED: boolean;
  REQUIRED_VALUE: any;
  ALLOCATED_VALUE: any;
  ALLOCATED_PERCENTAGE: any;
  CURRENCY: string;
}
interface serviceItem {
  ID: number;
  SERVICE_ID: number;
  CODE:string,
  DESCRIPTION: string;
  YEAR: number;
  JAN: SERVICE_MONTH;
  FEB: SERVICE_MONTH;
  MAR: SERVICE_MONTH;
  APR: SERVICE_MONTH;
  MAY: SERVICE_MONTH;
  JUN: SERVICE_MONTH;
  JUL: SERVICE_MONTH;
  AUG: SERVICE_MONTH;
  SEP: SERVICE_MONTH;
  OCT: SERVICE_MONTH;
  NOV: SERVICE_MONTH;
  DEC: SERVICE_MONTH;
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

    "ap-budget": ActivityBudget,
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
      editSrvId: 0,
      budgetMode: false,
      showActTabs: true,
      editSrvDesc: "",
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
      serviceList: [] as serviceItem[],
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

      localThis.getObjectServices();
    },
    getObjectServices() {
      let localThis: any = this as any;
      localThis.selectedObjct = null;
      localThis.serviceList = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ANNUAL_PLAN_OBJECTIVE_SERVICE_BUDGET",
        colList: null,
        whereCond: `OBJ_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "SRV_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let i: number = 0;
              let k: number = -1;
              for (i = 0; i < localThis.serviceList.length; i++) {
                if (localThis.serviceList[i].ID == item.SRV_ID) {
                  k = i;
                  break;
                }
              }
              if (k == -1) {
               let srvItem = {} as serviceItem;
                srvItem.ID = item.SRV_ID;
                if (item.TITLE == undefined) {
                  srvItem.CODE = item.PMS_COI_CODE;
                  srvItem.DESCRIPTION = "";
                } else {
                  srvItem.CODE = item.PMS_COI_CODE + " - " + item.TITLE;
                  srvItem.DESCRIPTION = item.TITLE;
                }

                srvItem.SERVICE_ID = item.SRV_ID;
                srvItem.YEAR = item.YEAR;
                switch (item.MONTH) {
                  case 1:
                    srvItem.JAN = {} as SERVICE_MONTH;
                    srvItem.JAN = localThis.GetMonth(srvItem.JAN, item);
                    break;
                  case 2:
                    srvItem.FEB = {} as SERVICE_MONTH;
                    srvItem.FEB = localThis.GetMonth(srvItem.FEB, item);
                    break;
                  case 3:
                    srvItem.MAR = {} as SERVICE_MONTH;
                    srvItem.MAR = localThis.GetMonth(srvItem.MAR, item);
                    break;
                  case 4:
                    srvItem.APR = {} as SERVICE_MONTH;
                    srvItem.APR = localThis.GetMonth(srvItem.APR, item);
                    break;
                  case 5:
                    srvItem.MAY = {} as SERVICE_MONTH;
                    srvItem.MAY = localThis.GetMonth(srvItem.MAY, item);
                    break;
                  case 6:
                    srvItem.JUN = {} as SERVICE_MONTH;
                    srvItem.JUN = localThis.GetMonth(srvItem.JUN, item);
                    break;
                  case 7:
                    srvItem.JUL = {} as SERVICE_MONTH;
                    srvItem.JUL = localThis.GetMonth(srvItem.JUL, item);
                    break;
                  case 8:
                    srvItem.AUG = {} as SERVICE_MONTH;
                    srvItem.AUG = localThis.GetMonth(srvItem.AUG, item);
                    break;
                  case 9:
                    srvItem.SEP = {} as SERVICE_MONTH;
                    srvItem.SEP = localThis.GetMonth(srvItem.SEP, item);
                    break;
                  case 10:
                    srvItem.OCT = {} as SERVICE_MONTH;
                    srvItem.OCT = localThis.GetMonth(srvItem.OCT, item);
                    break;
                  case 11:
                    srvItem.NOV = {} as SERVICE_MONTH;
                    srvItem.NOV = localThis.GetMonth(srvItem.NOV, item);
                    break;
                  case 12:
                    srvItem.DEC = {} as SERVICE_MONTH;
                    srvItem.DEC = localThis.GetMonth(srvItem.DEC, item);
                    break;
                }

                localThis.serviceList.push(srvItem);
              } else {
                switch (item.MONTH) {
                  case 1:
                    localThis.serviceList[k].JAN = {} as SERVICE_MONTH;
                    localThis.serviceList[k].JAN = localThis.GetMonth(
                      localThis.serviceList[k].JAN,
                      item
                    );
                    break;

                  case 2:
                    localThis.serviceList[k].FEB = {} as SERVICE_MONTH;
                    localThis.serviceList[k].FEB = localThis.GetMonth(
                      localThis.serviceList[k].FEB,
                      item
                    );
                    break;
                  case 3:
                    localThis.serviceList[k].MAR = {} as SERVICE_MONTH;
                    localThis.serviceList[k].MAR = localThis.GetMonth(
                      localThis.serviceList[k].MAR,
                      item
                    );
                    break;
                  case 4:
                    localThis.serviceList[k].APR = {} as SERVICE_MONTH;
                    localThis.serviceList[k].APR = localThis.GetMonth(
                      localThis.serviceList[k].APR,
                      item
                    );
                    break;
                  case 5:
                    localThis.serviceList[k].MAY = {} as SERVICE_MONTH;
                    localThis.serviceList[k].MAY = localThis.GetMonth(
                      localThis.serviceList[k].MAY,
                      item
                    );
                    break;
                  case 6:
                    localThis.serviceList[k].JUN = {} as SERVICE_MONTH;
                    localThis.serviceList[k].JUN = localThis.GetMonth(
                      localThis.serviceList[k].JUN,
                      item
                    );
                    break;
                  case 7:
                    localThis.serviceList[k].JUL = {} as SERVICE_MONTH;
                    localThis.serviceList[k].JUL = localThis.GetMonth(
                      localThis.serviceList[k].JUL,
                      item
                    );
                    break;
                  case 8:
                    localThis.serviceList[k].AUG = {} as SERVICE_MONTH;
                    localThis.serviceList[k].AUG = localThis.GetMonth(
                      localThis.serviceList[k].AUG,
                      item
                    );
                    break;
                  case 9:
                    localThis.serviceList[k].SEP = {} as SERVICE_MONTH;
                    localThis.serviceList[k].SEP = localThis.GetMonth(
                      localThis.serviceList[k].SEP,
                      item
                    );
                    break;
                  case 10:
                    localThis.serviceList[k].OCT = {} as SERVICE_MONTH;
                    localThis.serviceList[k].OCT = localThis.GetMonth(
                      localThis.serviceList[k].OCT,
                      item
                    );
                    break;
                  case 11:
                    localThis.serviceList[k].NOV = {} as SERVICE_MONTH;
                    localThis.serviceList[k].NOV = localThis.GetMonth(
                      localThis.serviceList[k].NOV,
                      item
                    );
                    break;
                  case 12:
                    localThis.serviceList[k].DEC = {} as SERVICE_MONTH;
                    localThis.serviceList[k].DEC = localThis.GetMonth(
                      localThis.serviceList[k].DEC,
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
    GetMonth(obj: SERVICE_MONTH, item: SERVICE_MONTH) {
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
      localThis.getObjectServices();
      localThis.budgetMode = false;
    },

    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.budgetMode = true;
      localThis.editSrvId = item.ID;
      localThis.editSrvDesc = item.CODE;
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
      localThis.getObjectServices();
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
      localThis.getObjectServices();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showSrvDetails = to;
    // },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getObjectServices();
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