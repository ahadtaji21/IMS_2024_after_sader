<template>
  <div >
    <div class="text-center" v-if="showWait" >
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <div v-if="!showActDetails">
      <!-- SRV SELECTION-->

      <v-data-table
        :headers="headers"
        :items="activityList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="100"
        style="
           {
            'font-size':'14px','width': '85px';
          }
        "
        class="text-capitalize"
        v-model="selectedAct"
      >
        <template v-slot:top>
          <div
            class="d-inline-flex align-center primary lighten-2 jrs-table-top"
          >
            <h3>
              {{ $t("datasource.pms.annual-plan.objectives.activities") }}
            </h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="editMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
              transition="dialog-transition"
            >
              <template v-slot:activator="{ on }" v-if="!onlyRead">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="addActivities"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>
              <v-card>
                <gt-addact-form
                  @closeDialoge="closeAddDialog"
                  :formDataExt="editedItemDialog"
                  @refreshAct="getServiceActivities"
                  :key="keyAddAct"
                ></gt-addact-form>
              </v-card>
            </v-dialog>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="mapMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <template v-slot:activator="{ on }" v-if="!onlyRead">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="mapActivities"
                >
                  <v-icon>mdi-link</v-icon>Map
                </v-btn>
              </template>
              <v-card>
                <gt-mapact-form
                  @closeDialoge="closeDialog"
                  @closeDialogeAndSave="closeDialogAndRefresh"
                  @refreshAct="getServiceActivities"
                  :formDataExt="editedItemDialog"
                  :selectedObjectId="selectedObjectId"
                  :createGrantActivity="true"
                  :key="Math.ceil(Math.random() * 1000)"
                ></gt-mapact-form>
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
        <template v-slot:[`item.action`]="{ item }">
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon v-if="!onlyRead" small class="mr-2" @click="editItem(item)" v-on="on"
                >mdi-circle-edit-outline</v-icon
              >
              <v-icon v-if="onlyRead" small class="mr-2" @click="editItem(item)" v-on="on"
                >mdi-eye</v-icon
              >
            </template>
            <span>{{ $t("datasource.pms.annual-plan.objectives.edit") }}</span>
          </v-tooltip>
          <v-tooltip>
            <template v-if="!onlyRead" v-slot:activator="{ on }">
              <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                >mdi-delete</v-icon
              >
            </template>
            <span>{{
              $t("datasource.pms.annual-plan.objectives.delete")
            }}</span>
          </v-tooltip>
        </template>
      </v-data-table>
    </div>

      <div v-if="showActDetails" >
        <gt-act-details
          :editActMainDataItem="formData"
          :key="showActDetails"
          :editActId="editId"
          :editActDesc="editActDesc"
          :selectedObjectId="selectedObjectId"
          :onlyRead="onlyRead"
        ></gt-act-details>
      </div>
   
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { JrsHeader } from "../../../../../models/JrsTable";
// import ActivityBudget from "./AnnualPlanActivityPlanForm.vue";
import Narrative from "../NARRATIVE/GMTGrantNarrative.vue";
import ActivityDetails from "./GMTGrantActivityDetailsForm.vue";
import MapActivity from "./GMTGrantMapActivityMainDataForm.vue";
import AddActivity from "./GMTGrantActivityMainDataForm.vue";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

interface ACTIVITY_MONTH {
  SELECTED: boolean;
  VALUE: any;
  CURRENCY: string;
}
interface activityItem {
  ID: number;
  ACTIVITY_TYPE_ID: string;
  ACTIVITY_TYPE_NAME: number;
  SERVICE_ID: number;
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

    "gt-act-details": ActivityDetails,
    "gt-mapact-form": MapActivity,
    "gt-addact-form": AddActivity,
    // "gt-budget": ActivityBudget,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    showActivityDetails: {
      type: Boolean,
      required: false,
    },
    onlyRead:{
      type:Boolean,
      required:false,
      default:false
    }
  },

  data() {
    return {
      showWait: false,
      keyAddAct: 0,
      editActId: 0,
      budgetMode: false,
      showActTabs: true,
      editActDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showActDetails: false,
      selectedAct: [],
      tableFilter: "",
      tblName: null,
      selectedActivity: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      mapMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      activityList: [] as activityItem[],
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

    closeAddDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;

      localThis.editedItemDialog = {};
    },

    closeDialogAndRefresh() {
      let localThis: any = this as any;
      localThis.mapMode = false;
      localThis.editedItemDialog = {};
      localThis.getServiceActivities();
    },

    closeDialog() {
      let localThis: any = this as any;
      localThis.mapMode = false;
      localThis.editedItemDialog = {};
    },

    reloadActivities(item: any) {
      let localThis = this as any;

      localThis.getServiceActivities();
    },
    mapActivities(item: any) {
      let localThis: any = this as any;
      localThis.mapMode = !localThis.mapMode;
      localThis.editedItemDialog = {};
      localThis.editedItemDialog.GMT_SERVICE_ID = localThis.selectedObjectId;
      localThis.editedItemDialog.GMT_ACTIVITY_ID = 0;
      localThis.editedItemDialog.PMS_ACTIVITY_ID = 0;
      localThis.editedItemDialog.GMT_ACTIVITY_MAPPING_ID = 0;
      //localThis.editedItemDialog.PMS_ACTIVITY_ID = item.ACTIVITY_ID;
    },

    addActivities() {
      let localThis: any = this as any;

      localThis.editMode = !localThis.editMode;

      localThis.editedItemDialog.SERVICE_ID = localThis.selectedObjectId;
      localThis.keyAddAct = Math.ceil(Math.random() * 1000);
    },
    getServiceActivities() {
      let localThis: any = this as any;
      localThis.selectedActivity = null;
      localThis.activityList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_SERVICE_ACTIVITY",
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
              let actItem = {} as activityItem;
              actItem.ID = item.ID;
              actItem.DESCRIPTION = item.DESCRIPTION;
              actItem.SERVICE_ID = item.SERVICE_ID;
              actItem.ACTIVITY_TYPE_ID = item.ACTIVITY_TYPE_ID + "";
              actItem.ACTIVITY_TYPE_NAME = item.ACTIVITY_TYPE_NAME;
              //actItem.YEAR = item.YEAR;
              localThis.activityList.push(actItem);
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
      localThis.selectedAct = [];
      localThis.selectedActivity = item.ID;
      localThis.selectedAct.push(item);
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

    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedAct = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      localThis.editActDesc = item.DESCRIPTION;
      localThis.showActDetails = true;

      localThis.selectedAct.push(item);
      localThis.activityList = [];

      EventBus.$emit("hideSrvTabs");
      localStorage.SelectedAct = "desc";
      EventBus.$emit("refreshBreadCumb", 4);
      localThis.formData.ID = item.ID;
      localThis.formData.DESCRIPTION = item.DESCRIPTION;
      localThis.formData.SERVICE_ID = localThis.selectedObjectId;
      localThis.formData.ACTIVITY_TYPE_ID = item.ACTIVITY_TYPE_ID + "";
      localThis.formData.ACTIVITY_TYPE_NAME = item.ACTIVITY_TYPE_NAME;
    },
    editNarrative(item: any) {
      let localThis: any = this as any;
      localThis.selectedAct = [];
      localThis.selectedActivity = item.ID;
      localThis.selectedAct.push(item);
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("general.name").toString(),
          value: "DESCRIPTION",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.activity-type.type").toString(),
          value: "ACTIVITY_TYPE_NAME",
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
    localThis.selectedActivity = null;
    localThis.tblName = "GMT_ACTIVITY";
    localThis.setupHeaders();
    if (localThis.selectedObjectId > 0) {
      localThis.getServiceActivities();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.showActDetails = false;
      localThis.reloadActivities();
      EventBus.$emit("showSrvTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadActivities(data);
    });
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
    showActivityDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showActDetails = to;
    },
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
.selectedCell {
  background-color: whitesmoke;
  color: black;
  font-size: 12px;
  border-style: solid;
  border-color: gainsboro;
  border-width: 1px;
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
.valuedBudget {
  font-size: 12px;
  width: 85px;
}
</style>