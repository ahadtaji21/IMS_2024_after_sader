<template>
  <div>
    <div v-if="!showBudgetMap">
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
            <h3>{{ $t("datasource.gmt.jrs-items") }}</h3>

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
            <tr v-for="item in items" :key="item.PMS_JRS_COA_CODE" style="height: 10px">
              <td class="tablerow">{{ item.PMS_JRS_COA_CODE }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_DESCRIPTION }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_TYPE }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_CATEGORY }}</td>
              <td class="tablerow">
                {{ round(item.NEEDS, 2) }}
              </td>
              <td class="tablerow">
                {{ round(item.FUNDED, 2) }}
              </td>
              <td :class="CellColor(2, item.GAP)">
                {{ round(item.GAP, 2) }}
              </td>
              <td class="tablerow">
                {{ item.CURRENCY }}
              </td>
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="listItem(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>
                  <span> Mapped Donor Resource List</span>
                </v-tooltip>

                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editBudget(item)" v-on="on"
                      >mdi-wallet</v-icon
                    >
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.budget-item")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
    <div v-else>
      <item-budget-map
        @closeMap="closeMap"
        :selectedProjectId="idAnnualPlan"
        :activityId="idActivity"
        :formData="formDataBudget"
        @reload="reloadBudget"
        :key="keyRnd"
        :year="year"
      ></item-budget-map>
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
      <v-dialog
        v-model="mappedListMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        transition="dialog-transition"
      >
        <v-card>
          <donor-mapped-items
            :formDataExt="formData"
            :displayData="displayData"
            :showItemDetails="true"
            :key="Math.ceil(Math.random() * 1000)"
            @closeMapDialog="CloseMapDialog"
          >
          </donor-mapped-items>

          <v-card-actions>
            <v-btn color="secondary" class="--darken-1" @click="CloseMapDialog()"
              >Cancel</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../../../models/JrsTable";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemBudget from "./../../../../PMS/BUDGET/AnnualPlanItemBudgetForm.vue";
import DonorResources from "./GMTGrantDonorActivityResourceForm.vue";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";
import { IDonor, IPayLoadDataITEM } from "./../../../../PMS/PMSSharedInterfaces";
import BudgetMap from "./../../../MAPPING/SEARCHGAP/GMTGapMapForm.vue";
import FormMixin from "../../../../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "donor-mapped-items": DonorResources,
    "item-budget-map": BudgetMap,
  },
  mixins: [FormMixin],
  props: {
    formDataExt: {
      type: Object,
      required: true,
    },
    idAnnualPlan: {
      type: Number,
      required: true,
    },
    idActivity: {
      type: Number,
      required: true,
    },
    showItemDetails: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      donorList: [],
      year: 0,
      formDataBudget: {},
      keyRnd: 0,
      showBudgetMap: false,
      displayData: {},
      resourceId: {},
      showItmTabs: true,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
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
      mappedListMode: false,
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
      setDonorList: "setDonorList",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    editBudget(item: any) {
      let localThis: any = this as any;
      localThis.formData.JRS_COA_ID = item.PMS_JRS_COA_ID;
      localThis.year = item.START_YEAR;

      localThis.getDonor(item.GRANT_ID);
    },
    closeMap() {
      let localThis = this as any;
      localThis.showBudgetMap = false;
      localThis.reloadItems(null);
    },

    FillFormData(item: any) {
      let localThis: any = this as any;
      localThis.formDataBudget = {} as any;
      if (item != null) {
        localThis.formDataBudget.ACTIVITY_ID = item.PMS_ACTIVITY_ID;
        localThis.formDataBudget.PMS_JRS_COI_ID = item.JRS_COI_ID;
        localThis.formDataBudget.PMS_JRS_TOS_ID = item.JRS_TOS_ID;
        localThis.formDataBudget.PMS_JRS_COA_ID = item.JRS_COA_ID;
      } else {
        localThis.formDataBudget.ACTIVITY_ID = 0;
        localThis.formDataBudget.PMS_JRS_COI_ID = 0;
        localThis.formDataBudget.PMS_JRS_TOS_ID = 0;
        localThis.formDataBudget.PMS_JRS_COA_ID = 0;
      }
    },

    getDonor(grantid: Number) {
      let localThis = this as any;
      localThis.showWait = true;
      localThis.donorList = [] as IDonor[];
      localThis.setDonorList(localThis.donorList);

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_PROJECTS_GRANT",
        //colList: null,
        whereCond: `PROJECT_ID=${localThis.idAnnualPlan} AND GRANT_ID=${grantid}`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.donorList.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.setDonorList(localThis.donorList);
          localThis.showWait = false;
          localThis.FillFormData(localThis.formData);
          localThis.showBudgetMap = true;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          console.log(err);
          return err;
        });
    },
    CellColor(type: number, value: number) {
      if (type == 1) {
        if (value == 0) {
          return "AlertCell";
        }
        return "";
      } else {
        if (value < 0) {
          return "OverGapCell";
        } else {
          if (value == 0) return "NoGapCell";
          else return "GapCell";
        }
      }
    },

    CloseMapDialog(item: string) {
      let localThis: any = this as any;
      localThis.mappedListMode = false;
    },
    closeBudgetDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.mappedListMode = false;
      localThis.reloadItems(null);
    },

    reloadBudget() {
      let localThis = this as any;
      localThis.reloadItems(null);
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
    },
    reloadItems(_item: any) {
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
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ANNUAL_PLAN_ACTIVITY_ITEM",
        colList: null,
        whereCond: `ACTIVITY_ID = ${localThis.formData.PMS_ACTIVITY_ID} AND GMT_ACTIVITY_ID=${localThis.formData.GMT_ACTIVITY_ID}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
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

    listItem(item: any) {
      let localThis = this as any;
      localThis.mappedListMode = true;

      //localThis.formData.PMS_ACTIVITY_ID = localThis.selectedObjectId;
      // localThis.formData.GMT_ACTIVITY_ID = 0;
      // localThis.formData.JRS_COI_ID = 0;
      // localThis.formData.JRS_TOS_ID = 0;
      localThis.formData.JRS_COA_ID = item.PMS_JRS_COA_ID;
      localThis.displayData = {} as any;
      localThis.displayData.COA_CODE = item.PMS_JRS_COA_CODE;
      localThis.displayData.COA_DESC = item.PMS_JRS_COA_DESCRIPTION;
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("pms.code").toString(),
          value: "PMS_JRS_COA_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.description").toString(),
          value: "PMS_JRS_COA_DESCRIPTION",
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
          text: this.$i18n.t("datasource.gmt.budget.needs").toString(),

          align: "center",
          value: "NEEDS",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.budget.funded").toString(),

          align: "center",
          value: "FUNDED",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.budget.gap").toString(),

          align: "center",
          value: "GAP",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-currency").toString(),

          align: "center",
          value: "CURRENCY",
          sortable: true,
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
  beforeMount() {
    let localThis = this as any;
    localThis.FillFormData(null);
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedItem = null;
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.setupHeaders();

    localThis.getActivityItems();
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
