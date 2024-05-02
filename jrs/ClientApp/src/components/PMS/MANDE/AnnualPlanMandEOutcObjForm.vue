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
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.outcome-obj-title", 2) }}
                </h4>

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
                    @change="checkboxUpdated($event, index, item.ID)"
                  ></v-checkbox>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.DESCRIPTION | stripHTML | subStr }}</span>
                    </template>
                    <span>{{ item.DESCRIPTION | stripHTML }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TARGET_DESC | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TARGET_DESC }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.OUTCOME | subStr }}</span>
                    </template>
                    <span>{{ item.OUTCOME }}</span>
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
      <ap-objoutc-indicators
        :onlyRead="onlyRead"
        :selectedObjectiveId="editObjId"
        :showItemDetails="true"
        :versioned="versioned"
        @reloadObjList="reloadObjList"
        :key="keyRnd"
      >
      </ap-objoutc-indicators>
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
import Indicators from "../OBJECTIVES/AnnualPlanObjectiveIndicatorForm.vue";
const Papa = require("papaparse");
const fs = require("fs");
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "ap-objoutc-indicators": Indicators,
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

    checkboxUpdated(newValue: any, index: number, rel_id: number) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showIndicators = newValue;
        if (newValue) {
          localThis.editObjId = rel_id; //localThis.objectiveList[index].ID;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      }
    },
    reloadObjList(item: any) {
      let localThis = this as any;

      localThis.getAnnualPlanObjectives();
    },
    getAnnualPlanObjectives() {
      let localThis = this as any;
      localThis.showWait = true;
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_M_AND_E";
      let wherecond: string = `ANNUAL_PLAN_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_M_AND_E_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList:
          "ID, DESCRIPTION, ANNUAL_PLAN_ID,PMS_TARGET_DESC,PMS_TARGET_ID,OUTCOME,IND_NUMBER",
        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.objectiveList = [...(res.table_data ? res.table_data : [])];
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
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
            .t("datasource.pms.annual-plan.ap-brief-description")
            .toString(),
          align: "center",
          value: "DESCRIPTION",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),

          align: "center",
          value: "PMS_TARGET_DESC",
          sortable: true,
          filterable: true,
        },

        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.outcome").toString(),

          align: "center",
          value: "OUTCOME",
          sortable: true,
          filterable: true,
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
      localThis.getAnnualPlanObjectives();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.reloadObjList();
      EventBus.$emit("showObjTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadObjList(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 25) return string.substring(0, 25) + "...";
      else return string;
    },

    stripHTML: function (value: any) {
      if ((value + "").toUpperCase() == "UNDEFINED") return "";
      const div = document.createElement("div");
      div.innerHTML = value;
      const text = div.textContent || div.innerText || "";
      return text;
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
