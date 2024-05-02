<template>
  <div>
    <div v-if="!showIndDetails">
      <!-- SRV SELECTION-->

      <v-data-table
        :headers="headers"
        :items="indicatorList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
        v-model="selectedInd"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{$t('datasource.pms.annual-plan.objectives.indicators')}}</h3>

            <v-spacer></v-spacer>
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="switchItems()"
            >
              <v-icon>{{sIcon}}</v-icon>
            </v-btn>
            <v-spacer></v-spacer>
            <v-dialog
              v-model="editMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              :max-width="((50 * nbrOfColumns) + 25) / 3 + 'em'"
              transition="dialog-transition"
            >
              <template v-slot:activator="{on}">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  v-if="!onlyRead"
                  @click="editMode = !editMode"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>
              <pms-addind-form
                @closeDialoge="closeDialog"
                @closeEdit="closeDialog"
                @refreshSrvInd="getServiceIndicators"
                :formDataExt="editedItemDialog"
                :selectedObjectId="selectedObjectId"
              ></pms-addind-form>
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
              <v-icon small v-if="!onlyRead" class="mr-2" @click="editItem(item)" v-on="on">mdi-circle-edit-outline</v-icon>
              <v-icon small v-if="onlyRead" class="mr-2" @click="editItem(item)" v-on="on">mdi-eye</v-icon>
            </template>
            <span>{{ $t('datasource.pms.annual-plan.objectives.edit-indicator') }}</span>
          </v-tooltip>
          <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editNarrative(item);" v-on="on">mdi-text</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-indicator-narrative') }}</span>
              </v-tooltip>
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editIndicators(item);" v-on="on">mdi-chart-line</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-indicator-indicators') }}</span>
          </v-tooltip>-->
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon small class="mr-2" v-if="!onlyRead" @click="deleteItem(item)" v-on="on">mdi-delete</v-icon>
            </template>
            <span>{{ $t('datasource.pms.annual-plan.objectives.delete-indicator') }}</span>
          </v-tooltip>
        </template>
      </v-data-table>
    </div>
    <div>
      <!-- <v-row v-if="selectedIndicator && selectedIndicator > 0">
        <v-col :cols="12">
          <div>
            <div v-if="showItemNumber==1">
              <ap-narrative :selectedObjectId="selectedIndicator" :tableName="tblName"></ap-narrative>
            </div>
            <div v-else-if="showItemNumber==2">
              <ap-indicator :selectedObjectId="selectedIndicator"></ap-indicator>
            </div>
          </div>
        </v-col>
      </v-row>-->
      <div v-if="showIndDetails">
        <ap-ind-details
          :editIndMainDataItem="formData"
          :key="showIndDetails"
          :editIndId="editId"
          :editIndDesc="editIndDesc"
          :selectedObjectId="selectedObjectId"
          :onlyRead="onlyRead"
        ></ap-ind-details>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../../components/JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";

import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import IndicatorDetails from "./AnnualPlanIndicatorDetailsForm.vue";
import IndicatorMainData from "./AnnualPlanIndicatorMainDataForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../axiosapi/api";

// interface IndicatorData {
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

    "ap-ind-details": IndicatorDetails,
    "pms-addind-form": IndicatorMainData
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true
    },
    showIndicatorDetails: {
      type: Boolean,
      required: true
    },
    onlyRead:{
      type: Boolean,
      required:false,
      default:false
    }
  },
  data() {
    return {
      showIndTabs: true,
      editIndDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showIndDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedInd: [],
      tableFilter: "",
      tblName: null,
      selectedIndicator: null,
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
      indicatorList: [],
      indicatorListOrig: [],
      tmpSelectedItem: [],
      occ: []
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
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
    },
    reloadIndicators(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getServiceIndicators().then((res: any) => {
        if (
          localThis.tmpSelectedItem &&
          localThis.tmpSelectedItem.length == 1
        ) {
          localThis.tmpSelectedItem = [];
                   if (item) {
            let i = 0;
            for (i = 0; i < localThis.indicatorList.length; i++) {
              if (localThis.indicatorList[i].ID == item.ID) {
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.indicatorList[i]))
                );
                localThis.indicatorList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
        }
      });
    },
    getServiceIndicators() {
      let localThis: any = this as any;
      localThis.selectedIndicator = null;
      localThis.indicatorList = [];
      localThis.indicatorListOrig = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_PLAN_SERVICE_INDICATOR",
        colList: null,
        whereCond: `SERVICE_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID"
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.indicatorList.push(item);
              localThis.indicatorListOrig.push(item);
            });
            if (
              localThis.tmpSelectedItem &&
              localThis.tmpSelectedItem.length == 1
            ){
              let i = 0;
              for (i = 0; i < localThis.indicatorList.length; i++) {
                if (
                  localThis.indicatorList[i].ID == localThis.tmpSelectedItem.ID
                ) {
                  localThis.tmpSelectedItem = [];
                  localThis.tmpSelectedItem.push(
                    JSON.parse(JSON.stringify(localThis.indicatorList[i]))
                  );
                  localThis.indicatorList = localThis.tmpSelectedItem;
                  break;
                }
              }
            }
          }
        }
      );
    },
    getNow: function() {
      let localThis: any = this as any;
      const today = new Date();
      const date =
        today.getFullYear() +
        "-" +
        (today.getMonth() + 1) +
        "-" +
        today.getDate();

      localThis.formData.DATE_FROM = date;
      localThis.formData.DATE_TO = date;
    },
    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented"
      });
    },
    editIndicators(item: any) {
      let localThis: any = this as any;
      localThis.selectedInd = [];
      localThis.selectedIndicator = item.ID;
      localThis.selectedInd.push(item);
      localThis.showItemNumber = 2;
      localThis.contractTable(item);
    },
    switchItems() {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.indicatorList = localThis.indicatorListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.indicatorList = localThis.tmpSelectedItem;
        //localThis.indicatorList=
      }
    },
    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedInd = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      localThis.editIndDesc =
        item.PMS_COI_CODE + "-" + item.PMS_TOS_DESCRIPTION + " ";
      localThis.showIndDetails = true;
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
      localThis.formData.OCCURRENCE = {};
      localThis.formData.OCCURRENCE.ID = item.OC_ID;
      localThis.formData.OCCURRENCE.TYPE = item.OC_TYPE;
      localThis.selectedInd.push(item);
      localThis.indicatorList = [];
      localThis.contractTable(item);
      EventBus.$emit("hideSrvTabs");
      localStorage.SelectedInd = "desc";
      EventBus.$emit("refreshBreadCumb", 3);
    },
    editNarrative(item: any) {
      let localThis: any = this as any;
      localThis.selectedInd = [];
      localThis.selectedIndicator = item.ID;
      localThis.selectedInd.push(item);
      localThis.showItemNumber = 1;
      localThis.contractTable(item);
    },
    contractTable(item: any) {
      let localThis: any = this as any;
      localThis.tmpSelectedItem = [];
      localThis.tmpSelectedItem.push(item);
      localThis.indicatorList = localThis.tmpSelectedItem;

      localThis.itemsPerPage = 1;
      localThis.sIcon = "mdi-chevron-double-down";
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("general.code").toString(),
          value: "CODE",
          align: "center",
          divider: true
        },
        {
          text: this.$i18n.t("general.type").toString(),
          value: "TYPE",
          align: "center",
          divider: true
        },
        {
          text: this.$i18n.t("general.scope").toString(),
          value: "SCOPE",
          align: "center",
          divider: true
        },
        {
          text: "Actions",
          value: "action",
          align: "center",
          sortable: false
        }
      ];

      localThis.headers = tmp;
    }
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedIndicator = null;
    localThis.tblName = "IND_INDICATOR";
    localThis.setupHeaders();
    localThis.getNow();
    localThis.editIndDesc = localThis.editedItem.PMS_COI_CODE;
    if (localThis.selectedObjectId > 0) {
      localThis.getServiceIndicators();
    }
    if (localThis.selectedSrvectId > 0) {
      localThis.getServiceIndicators();
      //localThis.getOccurrences();
    }
    EventBus.$on("closeIndicatorDetails", (data: any) => {
      localThis.showIndDetails = false;
      localThis.reloadIndicators();
      EventBus.$emit("showSrvTabs");
    });

    EventBus.$on("reloadIndicators", (data: any) => {
      localThis.reloadIndicators(data);
    });
  },
  computed: {
    language() {
      return i18n.locale;
    }
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getServiceIndicators();
      localThis.setupHeaders();
    },
    showIndicatorDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showIndDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getServiceIndicators();
    },

    editMode(to: boolean, from: boolean) {}
  }
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
</style>