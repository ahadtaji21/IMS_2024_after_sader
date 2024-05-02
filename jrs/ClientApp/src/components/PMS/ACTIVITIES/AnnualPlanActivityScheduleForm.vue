<template>
  <div id="app">
    <v-app id="inspire">
      <v-container class="grey lighten-5">
        <v-card>
          <v-card-title primary-title>{{year}}</v-card-title>
          <v-row no-gutters>
            <template v-for="n in 13">
              <v-col :key="n">
                <v-card class="pa-2" outlined tile :style="headerStyle">{{rowDesk[n]}}</v-card>
              </v-col>
            </template>
          </v-row>

          <v-row no-gutters :key="rowKey">
            <template v-for="n in 13">
              <v-col :key="n">
                <v-card  v-if="n>1"
                  v-model="selected[n]"
                  class="pa-2"
                  @click="changeColor(n)"
                  :style="selected[n] ? { 'background-color': 'red' } : { 'background-color': 'white' }"
                  outlined
                  tile
                ></v-card>
                <v-card v-else
                style="height:16px; font-size: 12px; font-family:Montserrat; text-align:center">
                {{actDesc}}
                </v-card>
              </v-col>
            </template>
          </v-row>
          <v-card-actions>
            <v-btn color="primary" @click="saveTimeLine()">{{ $t('general.save') }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-container>
    </v-app>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../axiosapi/api";
interface PayLoadD {
  ID: number;
  ACTIVITY_ID: number;
  YEAR: number;
  SELECTED: string;
}
export default Vue.extend({
  props: {
    selectedObjectId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      actDesc:'',
      year: 0,
      rowKey: 0,
      rowDesk: [],
      selected: [],
      headerStyle: {
        color: "gray",
        fontFamily: "Roboco",
        fontSize: "14px"
      }
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    changeColor(idx: number) {
      if (idx == 1) return;
      let localThis: any = this as any;
      localThis.selected[idx] = !localThis.selected[idx];
      localThis.rowKey = localThis.getRnd();
    },
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    saveTimeLine() {
      let localThis: any = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-activity-scheduling")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then(res => {
        if (res) {
          let payLoad = {} as PayLoadD;
          let i: number;
          payLoad.ACTIVITY_ID = localThis.selectedObjectId;

          payLoad.YEAR = localThis.year;
          payLoad.SELECTED = "";
          for (i = 0; i < localThis.selected.length; i++) {
            if (localThis.selected[i] == true) payLoad.SELECTED += i-1 + ":";
          }
          let savePayload: GenericSqlPayload = {
            spName:
              "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE_ACTIVITY_SCHEDULE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoad)
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message
              }); // Feedback to user
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message
              }); // Feedback to user
            });
        }
      });
    },
    getActivityScheduling() {
      let localThis: any = this as any;
      localThis.resetSelected();
      localThis.activityList = [];

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_ACTIVITY_SCHEDULING",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedObjectId} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID"
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.activityList.push(item);
            });
            let i: number = 0;
            for (i = 0; i < localThis.activityList.length; i++) {
              localThis.selected[localThis.activityList[i].MONTH+1] = true;
            }
            localThis.rowKey = localThis.getRnd();
          }
        });
    },
    resetSelected() {
      let localThis: any = this as any;
      let i: number = 0;
      for (i = 0; i < 13; i++) {
        localThis.selected[i] = false;
      }
    }
  },
  mounted() {
    let localThis = this as any;
    localThis.getActivityScheduling();
  },
  beforeMount() {
    let localThis = this as any;
    localThis.resetSelected();
    localThis.actDesc=localStorage.SelectedAct;
    localThis.rowDesk[1] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.activity"
    );
    localThis.rowDesk[2] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jan"
    );
    localThis.rowDesk[3] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.feb"
    );
    localThis.rowDesk[4] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.mar"
    );
    localThis.rowDesk[5] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.apr"
    );
    localThis.rowDesk[6] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.may"
    );
    localThis.rowDesk[7] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jun"
    );
    localThis.rowDesk[8] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.jul"
    );
    localThis.rowDesk[9] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.aug"
    );
    localThis.rowDesk[10] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.sep"
    );
    localThis.rowDesk[11] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.oct"
    );
    localThis.rowDesk[12] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.nov"
    );
    localThis.rowDesk[13] = this.$i18n.t(
      "datasource.pms.annual-plan.objectives.dec"
    );
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.year = ap.start_year;
  }
});
</script>

<style scoped>
</style>