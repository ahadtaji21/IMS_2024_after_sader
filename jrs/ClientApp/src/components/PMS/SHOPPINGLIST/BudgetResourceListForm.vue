<template>
  <div>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <div>
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="budgetresourceList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        style="
           {
            'font-size':'14px','width': '85px';
          }
        "
        class="text-capitalize"
        v-model="selectedItm"
      >
        <template v-slot:top>
          <div
            class="d-inline-flex align-center primary lighten-2 jrs-table-top"
          >
            <h3>{{ $t("pms.budgetresource-list") }}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="newSLMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <template v-slot:activator="{ on }">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="newSLMode = !newSLMode"
                  v-if="!onlyRead"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>

              <search-table
                v-model="resourceId"
                @UpdateItem="closeDialog"
                :InsertedBudgetResourceElement="budgetresourceList"
                :selectedActivityId="selectedObjectId"
                :key="Math.ceil(Math.random() * 1000)"
              ></search-table>
            </v-dialog>
            <v-dialog
              v-model="budgetresourcelistMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <item-budget
                v-model="resourceId"
                :onlyRead="onlyRead"
                @UpdateBudgetItem="closeBudgetDialogReload"
                @CloseDialog="closeBudgetDialog"
                :selectedActivityId="selectedObjectId"
                :resource="selectedResource"
                :resource_id="selectedResourceId"
                :selectedActivityItemId="selectedActivityItemId"
                :key="Math.ceil(Math.random() * 1000)"
              ></item-budget>
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
          <tbody style="border: solid">
            <tr
              v-for="(item, index) in items"
              :key="item.ID"
              style="height: 10px"
            >
              <td :class="'descClass'">
                <v-checkbox
                  v-model="selected[index]"
                  value
                  hide-details
                  class="shrink ma-0 pa-0"
                  @change="
                    checkboxUpdated(
                      $event,
                      index,
                      item.ACTIVITY_ID,
                      item.PMS_BUDGET_RESOURCE_LIST_ID
                    )
                  "
                ></v-checkbox>
              </td>
              <td class="tablerow">{{ item.PMS_BUDGET_RESOURCE_LIST_DESC }}</td>
              <!-- <td :class="'descClass'">
                <span>{{ item.ITEM_NUMBER }}</span>
              </td> -->

              <td style="text-align: center">
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
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.delete-item")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
    <div>
      <!-- <v-row v-if="selectedItem && selectedItem > 0">
        <v-col :cols="12">
          <div>
            <div v-if="showItemNumber==1">
              <ap-narrative :selectedObjectId="selectedItem" :tableName="tblName"></ap-narrative>
            </div>
            <div v-else-if="showItemNumber==2">
              <ap-item :selectedObjectId="selectedItem"></ap-item>
            </div>
          </div>
        </v-col>
      </v-row>-->
      <div v-if="showSLDetails && selected[0]">
        <ap-staff-list
          :onlyRead="onlyRead"
          :selectedProcessId="editActId"
          :selectedResId="resId"
          @ReloadResourceList="reloadResourceList"
          :showItemDetails="true"
          :key="Math.ceil(Math.random() * 1000)"
          :versioned="versioned"
        ></ap-staff-list>
      </div>
      <div v-if="showSLDetails && !selected[0]">
        <ap-item
          :onlyRead="onlyRead"
          :selectedObjectId="editActId"
          :selectedResShopId="inserted_id_no_hr"
          :showItemDetails="true"
          @ReloadShoppingList="reloadResourceList"
          :key="Math.ceil(Math.random() * 1000)"
          :versioned="versioned"
        ></ap-item>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./BudgetResourceSearchForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemBudget from "../BUDGET/AnnualPlanItemBudgetForm.vue";
import PmsItems from "../ITEMS/AnnualPlanItemForm.vue";

import StaffList from "./BudgetStaffListForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { IPayLoadDataITEM } from "./../PMSSharedInterfaces";
import NavMixin from "../../../mixins/NavMixin";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

// interface ItemData {
//   ID: number | null;
//   COI: {} | null;
//   TOS: {} | null;
//   DATE_FROM: Date | null;
//   DATE_TO: Date | null;
//   OCCURRENCE: {} | null;
//   OBJECTIVE_IfD: number | null;
// }

export default Vue.extend({
  components: {
    "ap-staff-list": StaffList,
    "search-table": SearchTable,
    "item-budget": ItemBudget,
    "ap-item": PmsItems,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    showItemDetails: {
      type: Boolean,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  mixins: [NavMixin],
  data() {
    return {
      inserted_id_no_hr: [0],
      showWait: false,
      editActId: 0,
      resId: 0,
      resIds: [0],

      selected: [],
      resourceId: {},
      showItmTabs: true,
      selectedResource: "",
      selectedResourceId: 0,
      selectedActivityItemId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
      showSLDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedItm: [],
      tableFilter: "",
      selectedItem: null,
      userrights: null,
      startDate: null,
      endDate: null,
      newSLMode: false,
      budgetresourcelistMode: false,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      budgetresourceList: [],
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
    closeDialog(item: any) {
      let localThis: any = this as any;
      localThis.newSLMode = false;
      if (item != null) localThis.UpdateItem(item);
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetresourcelistMode = false;
    },
    closeBudgetDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.budgetresourcelistMode = false;
      localThis.reloadItems(null);
    },
    async UpdateItem(item: any) {
      let i: number = 0;
      let localThis = this as any;
      for (i = 0; i < item.length; i++) {
        let ap = {} as any;

        let payLoadD = {} as any;

        payLoadD.PROCESS_ID = localThis.selectedObjectId;
        payLoadD.PMS_BUDGET_RESOURCE_LIST_ID = Number.parseInt(
          item[i].PMS_BUDGET_RESOURCE_LIST_ID
        );

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_ANNUAL_PLAN_BUDGET_RESOURCE_LIST",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        let a: any = await localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            if (Number.parseInt(item[i].PMS_BUDGET_RESOURCE_LIST_ID) != 1)
              localThis.UpdateItemNOHR(item[i].PMS_BUDGET_RESOURCE_LIST_ID);

            if (i == item.length - 1) {
              localThis.itemsPerPage = 15;
              localThis.getProcessBudgetResourceList();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              //localThis.$emit("ReloadResourceList");
            }
            return "";
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
          });
      }
    },
    // UpdateItem(item: string) {
    //   let ap = {} as any;
    //   let localThis = this as any;
    //   let payLoadD = {} as any;
    //   ap = localThis.$store.getters.getAnnualPlan;
    //   payLoadD.PROCESS_ID = localThis.selectedObjectId;
    //   payLoadD.PMS_BUDGET_RESOURCE_LIST_ID = Number.parseInt(item);

    //   let savePayload: GenericSqlPayload = {
    //     spName: "PMS_SP_INS_ANNUAL_PLAN_BUDGET_RESOURCE_LIST",
    //     actionType: SqlActionType.NUMBER_0,
    //     jsonPayload: JSON.stringify(payLoadD),
    //   };
    //   localThis
    //     .execSPCall(savePayload)
    //     .then((res: any) => {
    //       localThis.itemsPerPage = 15;
    //       localThis.getProcessBudgetResourceList();
    //       if (Number.parseInt(item) != 1) localThis.UpdateItemNOHR(item);

    //       localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
    //     })

    //     .catch((err: any) => {
    //       localThis.showNewSnackbar({
    //         typeName: "error",
    //         text: err.message,
    //       }); // Feedback to user
    //     });
    // },
    UpdateItemNOHR(item: string) {
      //for non HR
      let ap = {} as any;
      let localThis = this as any;
      localThis.showWait = true;
      let payLoadD = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      payLoadD.PROCESS_ID = localThis.selectedObjectId;
      payLoadD.PMS_SHOPPING_LIST_ID = 1; //generic other stuff
      payLoadD.PMS_BUDGET_RESOURCE_ID = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_ANNUAL_PLAN_SHOPPING_LIST",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.inserted_id_no_hr = [Number.parseInt(res.INSERTED_ID)];
          localThis.showWait = false;
          // localThis.itemsPerPage = 15;
          // localThis.getProcessShoppingList();
          // localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          // localThis.$emit("ReloadResourceList");
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getProcessBudgetResourceList();
    },

    reloadResourceList() {
      let localThis = this as any;
      localThis.getProcessBudgetResourceList();
      localThis.showSLDetails = true;
    },
    getProcessBudgetResourceList() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.budgetresourceList = [];
      //localThis.selected= [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_PROCESS_BUDGET_RESOURCE_LIST";
      let wherecond: string = `ACTIVITY_ID = ${localThis.selectedObjectId}`;
      if (localThis.versioned > 0) {
        view = "MS_VI_ANNUAL_PLAN_PROCESS_BUDGET_RESOURCE_LIST_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_BUDGET_RESOURCE_LIST_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.budgetresourceList.push(item);
              //localThis.selected.push(false);
            });
          }
          // localThis.showSLDetails=false;
        }
      );
    },

    budgetItem(item: any) {
      let localThis = this as any;
      localThis.budgetresourcelistMode = !localThis.budgetresourcelistMode;
      localThis.selectedResource = item.PMS_BUDGET_RESOURCE_LIST_DESC;
      localThis.selectedResourceId = item.PMS_BUDGET_RESOURCE_LIST_ID;
      localThis.selectedActivityItemId = item.ID;
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;
      var year = -1;
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_ANNUAL_PLAN_BUDGET_RESOURCE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getProcessBudgetResourceList();
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
    async checkboxUpdated(
      newValue: any,
      index: number,
      rel_id: number,
      res_id: number
    ) {
      let localThis: any = this as any;

      if (newValue) {
        if (res_id == 1) {
          let i: number;

          for (i = 0; i < localThis.selected.length; i++) {
            if (i == 0) {
              localThis.selected[i] = true;
            } else {
              localThis.selected[i] = false;
            }
          }
          console.log("localThis.selected: ", localThis.selected);
          localThis.editActId = rel_id; // localThis.objectiveList[index].ACT_ID;
          localThis.resId = res_id;
          localThis.resIds = [1];
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        } else {
          localThis.selected[0] = false;

          localThis.resId = res_id;
          localThis.resIds = [...localThis.resIds, res_id];
          await localThis.getResId(rel_id, localThis.resIds);
          localThis.editActId = rel_id;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      } else {
        localThis.resIds = localThis.resIds.filter(
          (ele: number) => ele != res_id
        );
        if (localThis.resIds.length > 0 && localThis.resIds[0] != 1) {
          await localThis.getResId(rel_id, localThis.resIds);
        }
      }
      localThis.showSLDetails = localThis.resIds.length > 0;
    },

    async getResId(rel_id: number, res_ids: number[]) {
      let localThis: any = this as any;
      localThis.showWait = true;
      let view: string = "PMS_PROCESS_SHOPPING_LIST_YEAR_REL";
      let wherecond: string = `PROCESS_ID = ${rel_id} AND PMS_BUDGET_RESOURCE_ID in(${res_ids
        .filter((ele) => ele != 0)
        .join(",")}) AND PMS_SHOPPING_LIST_ID=1`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      const retValue = await this.getGenericSelect({
        genericSqlPayload: selectPayload,
      }).then(async (res: any) => {
        //Setup data
        if (res.table_data) {
          let x: number = 0;
          localThis.inserted_id_no_hr = [];
          res.table_data.forEach((item: any) => {
            localThis.showWait = false;
            localThis.inserted_id_no_hr.push(item.ID);
            return item.ID;
          });
        } else {
          return -1;
        }
      });
      return retValue;
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: "",
          value: "",
          align: "center",
          divider: true,
          width: "20px",
        },

        {
          text: this.$i18n.t("pms.description").toString(),
          value: "PMS_BUDGET_RESOURCE_LIST_DESC",
          align: "center",
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.budgetresource-shopping-number").toString(),
        //   value: "PMS_ITEM_NUMBER",
        //   align: "center",
        //   divider: true,
        // },

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
    localThis.selectedItem = null;
    localThis.setupHeaders();

    if (localThis.selectedObjectId > 0) {
      localThis.getProcessBudgetResourceList();
    }

    EventBus.$on("closeItemDetails", (data: any) => {
      localThis.showSLDetails = false;
      localThis.reloadItems();
      EventBus.$emit("showSrvTabs");
    });

    EventBus.$on("reloadItems", (data: any) => {
      localThis.reloadItems(data);
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
      localThis.getProcessBudgetResourceList();
      localThis.setupHeaders();
    },
    showItemDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showSLDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getProcessBudgetResourceList();
    },

    newSLMode(to: boolean, from: boolean) {},

    inserted_id_no_hr(to: any) {
      // SADER
      console.log("NEW inserted_id_no_hr: ", to);
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
  background-color: whitesmoke;
  font-weight: bold;
  border-width: 1px;
}
.tablerow {
  text-align: center;
  height: 3.5em;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
  border-left: none;
}
</style>
