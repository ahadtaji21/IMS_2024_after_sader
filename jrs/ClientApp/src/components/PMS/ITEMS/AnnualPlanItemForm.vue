<template>
  <div>
    <div v-if="!showItmDetails">
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="itemList"
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
            <h3>{{ $t("datasource.pms.annual-plan.objectives.items") }}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="editMode"
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
                  @click="editMode = !editMode"
                  v-if="!onlyRead"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>

              <search-table
                v-model="resourceId"
                @UpdateItem="closeDialog"
                :selectedActivityId="selectedObjectId"
                :selectedResShopId="selectedResShopId"
                :key="Math.ceil(Math.random() * 1000)"
              ></search-table>
            </v-dialog>
            <v-dialog
              v-model="budgetMode"
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
            <tr v-for="item in items" :key="item.ID" style="height: 10px">
              <td class="tablerow">{{ item.PMS_BUDGET_RESOURCE_LIST_DESC }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_DESCRIPTION }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_CODE }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_TYPE }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_CATEGORY }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_NAV_UPDATE_DATE }}</td>
              <td class="tablerow">
                <v-icon
                  :color="
                    item.HAS_BUDGET == 1 ? 'green darken-3' : 'deep-orange darken-3'
                  "
                  >{{
                    item.HAS_BUDGET == 1
                      ? "mdi-checkbox-marked-circle-outline"
                      : "mdi-checkbox-blank-circle-outline"
                  }}</v-icon
                >
              </td>
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="budgetItem(item)" v-on="on"
                      >mdi-wallet</v-icon
                    >
                  </template>

                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.budget-item")
                  }}</span>
                </v-tooltip>

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
      <div v-if="showItmDetails">
        <!-- <ap-ind-details
          :editItmMainDataItem="formData"
          :key="showItmDetails"
          :editItmId="editId"
          :editItmDesc="editItmDesc"
          :selectedObjectId="selectedObjectId"
        ></ap-ind-details>-->
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanItemSearchMultiSelectForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemBudget from "../BUDGET/AnnualPlanItemBudgetForm.vue";
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
    "search-table": SearchTable,
    "item-budget": ItemBudget,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    selectedResShopId: {
      type: Array,
      required: false,
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
      resourceId: {},
      showItmTabs: true,
      selectedResource: "",
      selectedResourceId: 0,
      selectedActivityItemId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
      showItmDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedItm: [],
      tableFilter: "",
      selectedItem: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      budgetMode: false,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      itemList: [],
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
      localThis.editMode = false;
      if (item != null) localThis.UpdateItem(item);
    },
    closeBudgetDialog(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
    },
    closeBudgetDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.budgetMode = false;
      localThis.reloadItems(null);
    },
    async UpdateItem(item: any) {
      let i: number = 0;
      let localThis = this as any;
      for (i = 0; i < item.length; i++) {
        let ap = {} as any;

        let payLoadD = {} as any;
        ap = localThis.$store.getters.getAnnualPlan;
      payLoadD.ID_ACTIVITY = localThis.selectedObjectId;
      payLoadD.ID_ITEM = Number.parseInt(item[i].PMS_JRS_COA_ID);
      payLoadD.START_YEAR = Number.parseInt(ap.start_year);
      payLoadD.END_YEAR = Number.parseInt(ap.end_year);
      payLoadD.RES_SHOP_ID = localThis.selectedResShopId;
      

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_ANNUAL_PLAN_RESOURCE_ITEM_V1",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        let a: any = await localThis
          .execSPCall(savePayload)
          .then((res: any) => {
        
            if (i == item.length - 1) {
              localThis.getActivityItems();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          localThis.$emit("ReloadShoppingList");
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
    //   payLoadD.ID_ACTIVITY = localThis.selectedObjectId;
    //   payLoadD.ID_ITEM = Number.parseInt(item);
    //   payLoadD.START_YEAR = Number.parseInt(ap.start_year);
    //   payLoadD.END_YEAR = Number.parseInt(ap.end_year);
    //   payLoadD.RES_SHOP_ID = localThis.selectedResShopId;
    //   let savePayload: GenericSqlPayload = {
    //     spName: "PMS_SP_INS_ANNUAL_PLAN_RESOURCE_ITEM_V1",
    //     actionType: SqlActionType.NUMBER_0,
    //     jsonPayload: JSON.stringify(payLoadD),
    //   };
    //   localThis
    //     .execSPCall(savePayload)
    //     .then((res: any) => {
    //       localThis.itemsPerPage = 15;
    //       localThis.getActivityItems();
    //       localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
    //       localThis.$emit("ReloadShoppingList");
    //     })
    //     .catch((err: any) => {
    //       localThis.showNewSnackbar({
    //         typeName: "error",
    //         text: err.message,
    //       }); // Feedback to user
    //     });
    // },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getActivityItems();
    },
    getActivityItems() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.itemList = [];
      let i: number = 0;
      let j: number = 0;
      let wherecond: string;
      if (localThis.selectedResShopId[0] == 0)
        wherecond = `ACTIVITY_ID = ${localThis.selectedObjectId}`;
      else
        wherecond = `ACTIVITY_ID = ${localThis.selectedObjectId} AND RES_SHOP_ID in (${localThis.selectedResShopId})`;
      let view: string = "PMS_VI_ANNUAL_PLAN_ACTIVITY_ITEM_V1";

      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_ACTIVITY_ITEM_VER_V1";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_JRS_COA_CODE",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.itemList.push(item);
            });
          }
        }
      );
    },

    budgetItem(item: any) {
      let localThis = this as any;
      localThis.budgetMode = !localThis.budgetMode;
      localThis.selectedResource = item.PMS_JRS_COA_DESCRIPTION;
      localThis.selectedResourceId = item.PMS_JRS_COA_ID;
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
          localThis.updateBudgetOnNav("true", item.ID, year, true).then((res1: any) => {
            let payLoadD = {} as IPayLoadDataITEM;
            ap = localThis.$store.getters.getAnnualPlan;
            payLoadD.ID_ANNUAL_PLAN = ap.annal_plan_id;
            payLoadD.ID_ACTIVITY_ITEM = item.ID;

            let savePayload: GenericSqlPayload = {
              spName: "PMS_SP_DEL_ANNUAL_PLAN_RESOURCE_ITEM_V1",
              actionType: SqlActionType.NUMBER_0,
              jsonPayload: JSON.stringify(payLoadD),
            };
            localThis
              .execSPCall(savePayload)
              .then((res: any) => {
                localThis.itemsPerPage = 15;
                localThis.getActivityItems();
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
          });
        }
      });
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        
        {
          text: this.$i18n.t("pms.budgetResourceList").toString(),
          value: "PMS_BUDGET_RESOURCE_LIST_DESC",
          align: "center",
          divider: true,
        },
        {
          text:  this.$i18n.t("pms.item_resource").toString(),
          value: "PMS_JRS_COA_DESCRIPTION",
          align: "center",
          divider: true,

        },
        {
          text: this.$i18n.t("pms.coa_code").toString(),
          value: "PMS_JRS_COA_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.type").toString(),
          value: "PMS_JRS_COA_TYPE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.category").toString(),
          value: "PMS_JRS_COA_CATEGORY",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.category").toString(),
          value: "PMS_JRS_COA_NAV_UPDATE_DATE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.budget-assigned")
            .toString(),
          value: "HAS_BUDGET",
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

    if (localThis.selectedObjectId > 0) {
      localThis.getActivityItems();
    }

    EventBus.$on("closeItemDetails", (data: any) => {
      localThis.showItmDetails = false;
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
      localThis.getActivityItems();
      localThis.setupHeaders();
    },
    showItemDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showItmDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getActivityItems();
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
