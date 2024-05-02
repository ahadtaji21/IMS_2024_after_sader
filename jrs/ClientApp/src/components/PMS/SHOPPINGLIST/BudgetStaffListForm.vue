<template>
  <div>
    <div>
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="shoppingList"
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
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("pms.staff-list") }}</h3>

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
                :InsertedShoppingElement="shoppingList"
                :selectedProcessId="selectedProcessId"
                :selectedResId="selectedResId"
                :key="Math.ceil(Math.random() * 1000)"
              ></search-table>
            </v-dialog>
            <v-dialog
              v-model="shoppinglistMode"
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
                :selectedActivityId="selectedProcessId"
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
            <tr v-for="(item, index) in items" :key="item.ID" style="height: 10px">
              <td :class="'descClass'">
                <v-checkbox
                  v-model="selected[index]"
                  value
                  hide-details
                  class="shrink ma-0 pa-0"
                  @change="checkboxUpdated($event, index, item.ACTIVITY_ID, item.ID)"
                ></v-checkbox>
              </td>
              <td class="tablerow">{{ item.PMS_SHOPPING_LIST_DESC }}</td>
              <td :class="'descClass'">
                <span>{{ item.ITEM_NUMBER }}</span>
              </td>

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
              <ap-narrative :selectedProcessId="selectedItem" :tableName="tblName"></ap-narrative>
            </div>
            <div v-else-if="showItemNumber==2">
              <ap-item :selectedProcessId="selectedItem"></ap-item>
            </div>
          </div>
        </v-col>
      </v-row>-->
      <div v-if="showSLDetails">
        <ap-item
          :onlyRead="onlyRead"
          :selectedObjectId="editActId"
          :selectedResShopId="editResShopId"
          :showItemDetails="true"
          @ReloadShoppingList="reloadShoppingList"
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
import SearchTable from "./BudgetShoppingListSearchForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemBudget from "../BUDGET/AnnualPlanItemBudgetForm.vue";
import PmsItems from "../ITEMS/AnnualPlanItemForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { IPayLoadDataITEM } from "../PMSSharedInterfaces";
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
    "ap-item": PmsItems,
    "search-table": SearchTable,
    "item-budget": ItemBudget,
  },
  props: {
    selectedProcessId: {
      type: Number,
      required: true,
    },
    selectedResId: {
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
      editResShopId: 0,
      editActId: 0,
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
      shoppinglistMode: false,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      shoppingList: [],
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
    closeDialog(item: []) {
      let localThis: any = this as any;
      localThis.newSLMode = false;
      if (item != null) localThis.UpdateItem(item);
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.shoppinglistMode = false;
    },
    closeBudgetDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.shoppinglistMode = false;
      localThis.reloadItems(null);
    },
    async UpdateItem(item: any) {
      let i: number = 0;
      let localThis = this as any;
      for (i = 0; i < item.length; i++) {
        let ap = {} as any;

        let payLoadD = {} as any;
        ap = localThis.$store.getters.getAnnualPlan;
        payLoadD.PROCESS_ID = localThis.selectedProcessId;
        payLoadD.PMS_SHOPPING_LIST_ID = item[i].PMS_SHOPPING_LIST_ID;
        payLoadD.PMS_BUDGET_RESOURCE_ID = localThis.selectedResId;

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_ANNUAL_PLAN_SHOPPING_LIST",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        let a: any = await localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            if (i == item.length - 1) {
              localThis.itemsPerPage = 15;
              localThis.getProcessShoppingList();
              localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
              localThis.$emit("ReloadResourceList");
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
    reloadShoppingList() {
      let localThis = this as any;
      localThis.getProcessShoppingList();
      localThis.showSLDetails = true;
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getProcessShoppingList();
    },
    getProcessShoppingList() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.shoppingList = [];
      //localThis.selected = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_PROCESS_SHOPPING_LIST";
      let wherecond: string = `ACTIVITY_ID = ${localThis.selectedProcessId} AND PMS_BUDGET_RESOURCE_ID=${localThis.selectedResId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_PROCESS_SHOPPING_LIST_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_SHOPPING_LIST_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.shoppingList.push(item);
              //  localThis.selected.push(false);
            });
          }
        }
      );
    },

    budgetItem(item: any) {
      let localThis = this as any;
      localThis.shoppinglistMode = !localThis.shoppinglistMode;
      localThis.selectedResource = item.PMS_SHOPPING_LIST_DESC;
      localThis.selectedResourceId = item.PMS_SHOPPING_LIST_ID;
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
          // localThis.updateBudgetOnNav("true", item.ID, year, true).then((res1: any) => {
          let payLoadD = {} as any;

          payLoadD.ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_ANNUAL_PLAN_SHOPPING_ITEM",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getProcessShoppingList();
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
          // });
        }
      });
    },
    checkboxUpdated(newValue: any, index: number, rel_id: number, res_shop_id: number) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showSLDetails = newValue;
        if (newValue) {
          localThis.editActId = rel_id; // localThis.objectiveList[index].ACT_ID;
          localThis.editResShopId = res_shop_id;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      }
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
          value: "PMS_SHOPPING_LIST_DESC",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.item-number").toString(),
          value: "ITEM_NUMBER",
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
    localThis.selectedItem = null;
    localThis.setupHeaders();

    if (localThis.selectedProcessId > 0) {
      localThis.getProcessShoppingList();
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
      localThis.getProcessShoppingList();
      localThis.setupHeaders();
    },
    showItemDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showSLDetails = to;
    },
    selectedProcessId(to: number) {
      let localThis: any = this as any;
      localThis.selectedProcessId = to;
      localThis.getProcessShoppingList();
    },

    newSLMode(to: boolean, from: boolean) {},
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
