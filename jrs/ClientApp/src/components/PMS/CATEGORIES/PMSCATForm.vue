<template>
  <div>
    <div v-if="!showCtgDetails">
      <!-- CTG SELECTION-->
      <v-data-table
        :headers="columns"
        :items="categoryList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
        v-model="selectedCtg"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("pms.categories") }}</h3>

            <v-spacer></v-spacer>
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="switchItems()"
            >
              <v-icon>{{ sIcon }}</v-icon>
            </v-btn>
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
                    @click="editMode = !editMode"
                  >
                    <v-icon>mdi-plus-circle-outline</v-icon>New
                  </v-btn>
                </template>
                <pms-add-ctg-form
                  @closeDialoge="closeDialog"
                  @closeEdit="closeDialog"
                  :key="rndVar"
                  @refreshObjSrv="getObjectiveCategories"
                  :formDataExt="editedItemDialog"
                  :selectedObjectId="selectedObjectId"
                ></pms-add-ctg-form>
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
        <template v-slot:[`item.action`]="{ item }">
          <span v-if="!onlyRead">
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                  >mdi-circle-edit-outline</v-icon
                >
              </template>
              <span>{{ $t("datasource.pms.annual-plan.objectives.edit-service") }}</span>
            </v-tooltip>
          </span>
          <span v-if="onlyRead">
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                  >mdi-eye</v-icon
                >
              </template>
              <span>{{ $t("datasource.pms.annual-plan.objectives.edit-service") }}</span>
            </v-tooltip>
          </span>
          <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editNarrative(item);" v-on="on">mdi-text</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-service-narrative') }}</span>
              </v-tooltip>
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editIndicators(item);" v-on="on">mdi-chart-line</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.edit-service-indicators') }}</span>
          </v-tooltip>-->
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
        </template>
      </v-data-table>
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
      <div v-if="showCtgDetails">
        <ap-cat-details
          :editSrvMainDataItem="formData"
          :key="Math.ceil(Math.random() * 1000)"
          :editSrvId="editId"
          :editSrvDesc="editSrvDesc"
          :showCtgDetails="showCtgDetails"
          :selectedObjectId="selectedObjectId"
          :isUpdatableForm="isUpdatableForm"
          :onlyRead="onlyRead"
        ></ap-cat-details>
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
import Indicator from "../INDICATORS/AnnualPlanIndicatorForm.vue";
import ServiceDetails from "./PMSCATDetailsForm.vue";
import ServiceMainData from "./PMSCATMainDataForm.vue";
import { i18n, translate } from "../../../i18n";
import { EventBus } from "../../../event-bus";

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

    "ap-cat-details": ServiceDetails,
    "pms-add-ctg-form": ServiceMainData,
  },
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
  },
  data() {
    return {
      rndVar: 0,
      showSrvTabs: true,
      editSrvDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showCtgDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedCtg: [],
      tableFilter: "",
      tblName: null,
      selectedService: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      columnsTemplate: [
        {
          textKey: "Title",
          text: this.$i18n.t("general.title").toString(),
          value: "TITLE",
          align: "center",
          divider: true,
        },
        {
          textKey: "Outcome",
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.outcome").toString(),
          value: "OUTCOME_DESC",
          align: "center",
          divider: true,
        },
        {
          textKey: "COI CODE",
          text: this.$i18n.t("general.code").toString(),
          value: "PMS_COI_CODE",
          align: "center",
          divider: true,
        },

        {
          textKey: "Category of Intervention",
          text: this.$i18n
            .t("datasource.pms.category-of-intervention.category-of-intervention")
            .toString(),
          value: "COI_LABEL",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          textKey: "datasource.pms.annual-plan.ap-obj-target",
          align: " d-none",
          value: "COI_ID",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          textKey: "datasource.pms.annual-plan.ap-obj-target",
          align: " d-none",
          value: "OUTCOME_ID",
          sortable: true,
          filterable: true,
        },
        {
          text: "Actions",
          value: "action",
          align: "center",
          sortable: false,
        },
      ],

      coi: [],
      tos: [],
      categoryList: [],
      categoryListOrig: [],
      tmpSelectedItem: [],
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
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    reloadServices(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getObjectiveCategories().then((res: any) => {
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
          localThis.tmpSelectedItem = [];

          if (item) {
            let i = 0;
            for (i = 0; i < localThis.categoryList.length; i++) {
              if (localThis.categoryList[i].ID == item.ID) {
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.categoryList[i]))
                );
                localThis.categoryList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
        }
      });
    },

    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },
    editIndicators(item: any) {
      let localThis: any = this as any;
      localThis.selectedCtg = [];
      localThis.selectedService = item.ID;
      localThis.selectedCtg.push(item);
      localThis.showItemNumber = 2;
      localThis.contractTable(item);
    },
    switchItems() {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.categoryList = localThis.categoryListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.categoryList = localThis.tmpSelectedItem;
        //localThis.categoryList=
      }
    },
    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedCtg = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      // localThis.editSrvDesc = item.PMS_COI_CODE + "-" + item.PMS_TOS_DESCRIPTION + " ";
      // localThis.showCtgDetails = true;
      // localThis.formData.DATE_FROM = item.START_DATE;
      // localThis.formData.DATE_TO = item.END_DATE;
      // localThis.formData.ID = item.ID;
      // localThis.formData.OBJECTIVE_ID = item.OBJECTIVE_ID;
      // localThis.formData.COI = {};
      // localThis.formData.COI.PMS_COI_ID = item.PMS_COI_ID;
      // localThis.formData.COI.IMS_LABELS_VALUE = item.PMS_COI_DESCRIPTION;
      // localThis.formData.COI.PMS_COI_CODE = item.PMS_COI_CODE;
      // localThis.formData.TOS = {};
      // localThis.formData.TOS.PMS_TOS_ID = item.PMS_TOS_ID;
      // localThis.formData.TOS.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION;
      // localThis.formData.TOS.PMS_COI_TOS_ID = item.PMS_COI_TOS_ID;
      // localThis.formData.OCCURRENCE = {};
      // localThis.formData.OCCURRENCE.ID = item.OC_ID;
      // localThis.formData.OCCURRENCE.TYPE = item.OC_TYPE;
      // localThis.formData.LOCATION_ID = item.LOCATION_ID;
      // localThis.formData.LOCATION_DETAILS = item.LOCATION_DETAILS;
      localThis.formData.TITLE = item.TITLE;
      localThis.selectedCtg.push(item);
      localThis.categoryList = [];
      localThis.contractTable(item);
      EventBus.$emit("hideObjecTabs");
      localStorage.selectedCtg = localThis.editSrvDesc;
      EventBus.$emit("refreshBreadCumb", 2);
    },
    editNarrative(item: any) {
      let localThis: any = this as any;
      localThis.selectedCtg = [];
      localThis.selectedService = item.ID;
      localThis.selectedCtg.push(item);
      localThis.showItemNumber = 1;
      localThis.contractTable(item);
    },
    contractTable(item: any) {
      let localThis: any = this as any;
      localThis.tmpSelectedItem = [];
      localThis.tmpSelectedItem.push(item);
      localThis.categoryList = localThis.tmpSelectedItem;

      localThis.itemsPerPage = 1;
      localThis.sIcon = "mdi-chevron-double-down";
    },

    getObjectiveCategories() {
      let localThis: any = this as any;
      localThis.selectedService = null;
      localThis.categoryList = [];
      localThis.categoryListOrig = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_PLAN_OBJECTIVE_CATEGORY",
        colList: null,
        whereCond: `OBJECTIVE_ID = ${localThis.selectedObjectId} AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.categoryList.push(item);
              localThis.categoryListOrig.push(item);
            });
            if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
              let i = 0;
              for (i = 0; i < localThis.categoryList.length; i++) {
                if (localThis.categoryList[i].ID == localThis.tmpSelectedItem.ID) {
                  localThis.tmpSelectedItem = [];
                  localThis.tmpSelectedItem.push(
                    JSON.parse(JSON.stringify(localThis.categoryList[i]))
                  );
                  localThis.categoryList = localThis.tmpSelectedItem;
                  break;
                }
              }
            }
          }
        }
      );
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedService = null;
    localThis.tblName = "PMS_SERVICE";

    localThis.editSrvDesc = localThis.editedItem.PMS_COI_CODE;
    if (localThis.selectedObjectId > 0) {
      localThis.getObjectiveCategories();
    }
    EventBus.$on("closeServiceDetails", (data: any) => {
      localThis.showCtgDetails = false;
      localThis.reloadServices();
      EventBus.$emit("showObjecTabs");
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
    language() {
      return i18n.locale;
    },
    columns() {
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }

        let newCol = col;
        newCol.text = translate(col.text);

        return newCol;
      });

      return cols;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
    localThis.editedItemDialog.outcome = localThis.formDataInput.OUTCOME;
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      //localThis.getObjectiveCategories();
    },
    showServiceDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showCtgDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getObjectiveCategories();
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
</style>
