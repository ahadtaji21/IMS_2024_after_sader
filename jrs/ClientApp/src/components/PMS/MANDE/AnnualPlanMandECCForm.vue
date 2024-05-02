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
            :items="ccList"
            item-key="ID"
            sort-by=""
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
                <h4>{{ $t("datasource.pms.annual-plan.objectives.c-c") }}</h4>

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
              <tr v-for="(item, index) in items" :key="item.ID">
                <td :class="'descClass'">
                  <v-checkbox
                    v-model="selected[index]"
                    value
                    hide-details
                    class="shrink ma-0 pa-0"
                    @change="checkboxUpdated($event, index, item.ID, item)"
                  ></v-checkbox>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_CC_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_CC_DESC }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <span>{{ item.IND_NUMBER }}</span>
                </td>
              </tr>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <div v-if="showIndicators">
      <ap-cc-indicators
        :onlyRead="onlyRead"
        :formData="formData"
        :showItemDetails="true"
        :versioned="versioned"
        @reloadCCList="reloadCC"
        :key="keyRnd"
      >
      </ap-cc-indicators>
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
import Indicators from "../CROSSCUTTING/AnnualPlanCCIndicatorForm.vue";
const Papa = require("papaparse");
const fs = require("fs");
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "ap-cc-indicators": Indicators,
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
      ccList: [] as any,
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
    checkboxUpdated(newValue: any, index: number, rel_id: number, item: any) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showIndicators = newValue;
        if (newValue) {
          localThis.formData.selectedCCId = item.PMS_CC_ID;
          localThis.formData.selectedCCKeyId = item.PMS_CC_KEY_ID; // localThis.ccList[index].PMS_CC_KEY_ID;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      }
    },
    reloadCC(item: any) {
      let localThis = this as any;

      localThis.getAnnualPlanCC();
    },
    getAnnualPlanCC() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.showWait = true;
      localThis.ccList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_CC_M_AND_E";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CC_M_AND_E_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.ccList.push(item);
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
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.c-c").toString(),
          value: "PMS_CC_DESC",
          align: "center",
          divider: true,
        },
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
      localThis.getAnnualPlanCC();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.reloadCC();
      EventBus.$emit("showObjTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadCC(data);
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

      localThis.getAnnualPlanCC();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showObjDetails = to;
    // },
    selectedAnnualPlanId(to: number) {
      let localThis: any = this as any;
      localThis.selectedAnnualPlanId = to;

      localThis.getAnnualPlanCC();
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
