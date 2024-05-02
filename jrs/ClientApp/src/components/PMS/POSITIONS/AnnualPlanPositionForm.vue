<template>
  <div>
    <div v-if="!showPosDetails">
      <!-- POS SELECTION-->

      <v-data-table
        :headers="headers"
        :items="positionList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
        v-model="selectedPos"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("datasource.pms.annual-plan.objectives.positions") }}</h3>

            <v-spacer></v-spacer>
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="switchItems()"
            >
              <v-icon>{{ sIcon }}</v-icon>
            </v-btn>
            <v-spacer></v-spacer>
            <v-dialog
              v-if="!onlyRead"
              v-model="editMode"
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
                  @click="editMode = !editMode"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>

              <search-table-position
                v-model="profileId"
                @input="closeDialog"
                :key="getRnd()"
                :selectedObjectId="selectedObjectId"
                :insertedPositions="positionList"
              ></search-table-position>
            </v-dialog>
            <v-dialog
              v-model="budgetMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <position-budget
                v-model="profileId"
                @UpdateBudgetPosition="closeBudgetDialog"
                :selectedActivityId="selectedObjectId"
                :position="selectedPosition"
                :position_id="selectedPositionId"
                :key="Math.ceil(Math.random() * 1000)"
              ></position-budget>
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
              :readonly="onlyRead"
              class="white"
              color="secondary darken-2"
            ></v-text-field>
          </div>
        </template>
        <template v-slot:[`item.action`]="{ item }">
          <!-- <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon small class="mr-2" @click="budgetPosition(item)" v-on="on">mdiet</v-icon>
            </template>
            <span>{{ $t('datasource.pms.annual-plan.objectives.budget-item') }}</span>
          </v-tooltip> -->
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon
                v-if="!onlyRead"
                small
                class="mr-2"
                @click="deleteItem(item)"
                v-on="on"
                >mdi-delete</v-icon
              >
            </template>
            <span>{{ $t("datasource.pms.annual-plan.objectives.delete-position") }}</span>
          </v-tooltip>
        </template>
      </v-data-table>
    </div>
    <div>
      <!-- <v-row v-if="selectedPosition && selectedPosition > 0">
        <v-col :cols="12">
          <div>
            <div v-if="showItemNumber==1">
              <ap-narrative :selectedObjectId="selectedPosition" :tableName="tblName"></ap-narrative>
            </div>
            <div v-else-if="showItemNumber==2">
              <ap-position :selectedObjectId="selectedPosition"></ap-position>
            </div>
          </div>
        </v-col>
      </v-row>-->
    </div>
    <!-- <div v-if="showAddResource">
        {{showAddResource}}
        
      <v-dialog
        v-model="editResourceMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        transition="dialog-transition"
      >
        <search-table-resource v-model="resourceId" @UpdateResource="closeDialogResource" :key="getRnd()" 
        :selectedPositionId="selectedPositionId"
        :selectedServiceId="selectedObjectId"
        ></search-table-resource>
      </v-dialog>
    </div>-->
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanPositionSearchForm.vue";
import PositionBudget from "./../BUDGET/AnnualPlanPositionBudgetForm.vue";
// import SearchTableResource from "./AnnualPlanResourceSearchForm.vue";
// import PositionDetails from "./AnnualPlanPositionDetailsForm.vue";
// import PositionMainData from "./AnnualPlanPositionMainDataForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

// interface PositionData {
//   ID: number | null;
//   COI: {} | null;
//   TOS: {} | null;
//   DATE_FROM: Date | null;
//   DATE_TO: Date | null;
//   OCCURRENCE: {} | null;
//   OBJECTIVE_ID: number | null;
// }
interface payLoadData {
  ID_ANNUAL_PLAN: number | null;
  ID_ACTIVITY: number | null;
  ID_POSITION: number | null;
}
interface payLoadDelete {
  ID_ANNUAL_PLAN: number | null;
  ID_ACTIVITY: number | null;
  ID_ACTIVITY_POSITION: number | null;
}
interface payLoadResource {
  ID_ANNUAL_PLAN: number | null;
  ID_ACTIVITY: number | null;
  ID_USER: number | null;
  ID_POSITION: number | null;
}
export default Vue.extend({
  components: {
    "search-table-position": SearchTable,
    "position-budget": PositionBudget,
    // "search-table-resource": SearchTableResource
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    showPositionDetails: {
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
      profileId: {},
      budgetMode: false,
      selectedPositionId: 0,
      resourceId: {},
      showPosTabs: true,
      editPosDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showPosDetails: false,
      //showAddResource: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedPos: [],
      tableFilter: "",
      selectedPosition: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      editResourceMode: false,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      positionList: [],
      positionListOrig: [],
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
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
    },
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      if (item != null) localThis.UpdatePosition(item);
    },
    //   closeDialogResource(item: string) {
    //     let localThis: any = this as any;
    //     localThis.editResourceMode = false;
    //     //localThis.showAddResource=false;
    //     if (item != null)
    //       localThis.UpdateResource(item);
    //   },
    //  UpdateResource(item:String) {

    //     let localThis = this as any;
    //     let payLoadD = {} as payLoadResource;
    //     let ap = {} as any;
    //     payLoadD.ID_ACTIVITY=localThis.selectedObjectId;
    //     payLoadD.ID_USER = Number.parseInt(item.split('-')[0]);
    //     payLoadD.ID_POSITION = Number.parseInt(item.split('-')[1]);
    //     ap =localThis.$store.getters.getAnnualPlan;
    //     payLoadD.ID_ANNUAL_PLAN = ap.annal_plan_id;
    //      let savePayload: GenericSqlPayload = {
    //           spName: "PMS_SP_INS_ANNUAL_PLAN_RESOURCE",
    //           actionType: SqlActionType.NUMBER_0,
    //           jsonPayload: JSON.stringify(payLoadD)
    //         };
    //         localThis
    //           .execSPCall(savePayload)
    //           .then((res: any) => {
    //             localThis.itemsPerPage=5;
    //             localThis.tmpSelectedItem = [];
    //             EventBus.$emit("reloadResources");
    //             localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user

    //           }).catch((err: any) => {

    //             localThis.showNewSnackbar({
    //               typeName: "error",
    //               text: err.message
    //             }); // Feedback to user
    //           });
    //   },

    UpdatePosition(item: string) {
      let localThis = this as any;
      let payLoadD = {} as payLoadData;
      let ap = {} as any;
      payLoadD.ID_ACTIVITY = localThis.selectedObjectId;
      payLoadD.ID_POSITION = Number.parseInt(item);
      ap = localThis.$store.getters.getAnnualPlan;
      payLoadD.ID_ANNUAL_PLAN = ap.annal_plan_id;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_ANNUAL_PLAN_ACTIVITY_POSITION",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.itemsPerPage = 15;
          localThis.tmpSelectedItem = [];
          localThis.getActivityPositions();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    reloadPositions(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getActivityPositions().then((res: any) => {
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
          localThis.tmpSelectedItem = [];
          if (item) {
            let i = 0;
            for (i = 0; i < localThis.positionList.length; i++) {
              if (localThis.positionList[i].ID == item.ID) {
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.positionList[i]))
                );
                localThis.positionList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
        }
      });
    },
    getActivityPositions() {
      let localThis: any = this as any;
      localThis.selectedPosition = null;
      localThis.positionList = [];
      localThis.positionListOrig = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_PLAN_ACTIVITY_POSITION",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              x++;
              localThis.positionList.push(item);
              localThis.positionListOrig.push(item);
            });
            //EventBus.$emit("positionOnService", x);
            if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
              let i = 0;
              for (i = 0; i < localThis.positionList.length; i++) {
                if (
                  localThis.positionList[i].HR_POSITION_ID ==
                  localThis.tmpSelectedItem.HR_POSITION_ID
                ) {
                  localThis.tmpSelectedItem = [];
                  localThis.tmpSelectedItem.push(
                    JSON.parse(JSON.stringify(localThis.positionList[i]))
                  );
                  localThis.positionList = localThis.tmpSelectedItem;
                  break;
                }
              }
            }
          }
        }
      );
    },

    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as payLoadDelete;
          ap = localThis.$store.getters.getAnnualPlan;
          payLoadD.ID_ANNUAL_PLAN = ap.annal_plan_id;
          payLoadD.ID_ACTIVITY_POSITION = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_ANNUAL_PLAN_ACTIVITY_POSITION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 10;
              localThis.getActivityPositions();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },

    // addResource(item: any) {
    //   let localThis = this as any;
    //   localThis.editResourceMode=true;
    //   //localThis.showAddResource = true;
    //   localThis.selectedPositionId=Number.parseInt(item.HR_POSITION_ID);
    // },
    editPositions(item: any) {
      let localThis: any = this as any;
      localThis.selectedPos = [];
      localThis.selectedPosition = item.HR_POSITION_ID;
      localThis.selectedPos.push(item);
      localThis.showItemNumber = 2;
      localThis.contractTable(item);
    },
    switchItems() {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.positionList = localThis.positionListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.positionList = localThis.tmpSelectedItem;
        //localThis.positionList=
      }
    },
    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedPos = [];
      //localThis.editMode = true;
      localThis.editId = item.HR_POSITION_ID;

      localThis.selectedPos.push(item);
      localThis.positionList = [];
      localThis.contractTable(item);
      EventBus.$emit("hideSrvTabs");
      localStorage.SelectedPos = "desc";
      EventBus.$emit("refreshBreadCumb", 3);
    },
    budgetPosition(item: any) {
      let localThis = this as any;
      localThis.budgetMode = !localThis.budgetMode;
      localThis.selectedPosition = item.HR_POSITION_FULL_TITLE;
      localThis.selectedPositionId = item.HR_POSITION_ID;
    },
    contractTable(item: any) {
      let localThis: any = this as any;
      localThis.tmpSelectedItem = [];
      localThis.tmpSelectedItem.push(item);
      localThis.positionList = localThis.tmpSelectedItem;

      localThis.itemsPerPage = 1;
      localThis.sIcon = "mdi-chevron-double-down";
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.hrm.position.biodata-full-name").toString(),
          value: "HR_BIODATA_FULL_NAME",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.department").toString(),
          value: "HR_DEPARTMENT_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.title-full").toString(),
          value: "HR_POSITION_FULL_TITLE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.descr").toString(),
          value: "HR_POSITION_DESCR",
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
    localThis.selectedPosition = null;
    localThis.setupHeaders();
    localThis.editPosDesc = localThis.editedItem.PMS_COI_CODE;
    if (localThis.selectedObjectId > 0) {
      localThis.getActivityPositions();
    }

    EventBus.$on("closePositionDetails", (data: any) => {
      localThis.showPosDetails = false;
      localThis.reloadPositions();
      EventBus.$emit("showSrvTabs");
    });

    EventBus.$on("reloadPositions", (data: any) => {
      localThis.reloadPositions(data);
    });
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getActivityPositions();
      localThis.setupHeaders();
    },
    showPositionDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showPosDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getActivityPositions();
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
.selectedRow {
  background-color: whitesmoke;
  font-weight: bold;
  border-width: 1px;
}
</style>
