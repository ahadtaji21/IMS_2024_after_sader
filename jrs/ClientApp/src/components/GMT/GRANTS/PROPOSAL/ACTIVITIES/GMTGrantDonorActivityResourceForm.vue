<template>
  <v-card>
    <v-card-text>
      <v-row>
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >JRS COA CODE:</v-col
        >
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
          displayData.COA_CODE
        }}</v-col>
      </v-row>
      <v-row>
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
          >JRS COA DESCRIPTION:</v-col
        >
        <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
          displayData.COA_DESC
        }}</v-col>
      </v-row>
    </v-card-text>
    <div v-if="!showItmDetails">
      <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>
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
            <h3>{{ $t("datasource.gmt.donor-items") }}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="matchMode"
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
                  @click="MapCOA"
                >
                  <v-icon>mdi-link</v-icon>Map
                </v-btn>
              </template>
              <v-card>
                <gt-donor-coa
                  v-model="linkedData"
                  @addRelation="closeAndUpdateCOITOSCOADialog"
                  @closeDialoge="closeCOITOSCOADialog"
                  :formData="formData"
                  :donorId="donor.DONOR_ID"
                  :grantId="donor.GRANT_ID"
                  :key="Math.ceil(Math.random() * 1000)"
                ></gt-donor-coa>
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
        <template v-slot:body="{ items }">
          <tbody style="border: solid">
            <tr v-for="item in items" :key="item.ID" style="height: 10px">
              <td class="tablerow">{{ item.ACC_CODE }}</td>
              <td class="tablerow">{{ item.ACC_DESCRIPTION }}</td>
              <td class="tablerow">{{ item.ACC_TYPE }}</td>
              <td class="tablerow">{{ item.ACC_CATEGORY }}</td>
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
                    <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
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
    </div>
  </v-card>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../../../models/JrsTable";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemBudget from "./../../../../PMS/BUDGET/AnnualPlanItemBudgetForm.vue";
import MapDonoCOA from "./GMTGrantActivityMapCoaForm.vue";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";
import { IPayLoadDataITEM } from "./../../../../PMS/PMSSharedInterfaces";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "gt-donor-coa": MapDonoCOA,
  },
  props: {
    formDataExt: {
      type: Object,
      required: true,
    },
    displayData: {
      type: Object,
      required: true,
    },
    showItemDetails: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      linkedData:{},
      showWait: false,
      matchMode: false,
      donor: {},
      resourceId: {},
      showItmTabs: true,
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
      tblName: null,
      selectedItem: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      //mappedListMode: false,
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
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) localThis.UpdateItem(item);
    },

    MapCOA(item: string) {
      let localThis: any = this as any;
      localThis.matchMode = true;
    },

    addRelation(item: any) {
      let localThis = this as any;
      localThis.showWait = true;
      let payLoadD = {} as any;

      // payLoadD.DONOR_COI_ID = item.COI_ID;
      payLoadD.DONOR_COA_ID = item.COA_ID;
      // payLoadD.DONOR_TOS_ID = item.TOS_ID;
      payLoadD.DNR_ID = item.DONOR_ID;
      payLoadD.ACTIVITY_ID = localThis.formData.PMS_ACTIVITY_ID;
      payLoadD.GMT_ACTIVITY_ID = item.GMT_ACTIVITY_ID;
      payLoadD.JRS_COI_ID = localThis.formData.JRS_COI_ID;
      payLoadD.JRS_COA_ID = localThis.formData.JRS_COA_ID;
      payLoadD.JRS_TOS_ID = localThis.formData.JRS_TOS_ID;
      payLoadD.GRANT_PRJ_REL_ID = localThis.formData.GRANT_PRJ_REL_ID;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_MAPPING_ITEMS",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getActivityItems();
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          }); // Feedback to user
        })
        .then((res: any) => {
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    closeCOITOSCOADialog() {
      let localThis = this as any;
      localThis.matchMode = false;
    },
    closeAndUpdateCOITOSCOADialog(item: any) {
      let localThis: any = this as any;
      localThis.matchMode = false;
      localThis.addRelation(item);
      (this as any).$emit("UpdateBudgetList");
    },
    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.itemsPerPage();
    },
    getActivityItems() {
      let localThis = this as any;
      localThis.showWait = true;

      localThis.itemList = [];
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_MAPPED_ITEMS",
        colList: null,
        whereCond: `PMS_ACTIVITY_ID=${localThis.formDataExt.PMS_ACTIVITY_ID}  AND GMT_ACTIVITY_ID=${localThis.formDataExt.GMT_ACTIVITY_ID} AND JRS_COI_ID=${localThis.formDataExt.JRS_COI_ID} AND  JRS_TOS_ID=${localThis.formDataExt.JRS_TOS_ID} AND JRS_COA_ID=${localThis.formDataExt.JRS_COA_ID}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.itemList.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          console.log(err);
          return err;
        });
    },

    listItem(item: any) {
      let localThis = this as any;
      //localThis.mappedListMode = true;

      //localThis.formData.PMS_ACTIVITY_ID = localThis.selectedObjectId;
      // localThis.formData.GMT_ACTIVITY_ID = 0;
      // localThis.formData.JRS_COI_ID = 0;
      // localThis.formData.JRS_TOS_ID = 0;
      localThis.formData.JRS_COA_ID = item.PMS_JRS_COA_ID;
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_MAPPING_ITEM",
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
        }
      });
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
    localThis.tblName = "PMS_VI_JRS_CHART_OF_ACCOUNT";
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.setupHeaders();

    localThis.getActivityItems();

    let gt = {} as any;
    gt = localThis.$store.getters.getGrant;
    localThis.donor = {} as any;
    localThis.donor.DONOR_ID = gt.donor_id;
    localThis.donor.GRANT_ID = gt.grant_id;

    // EventBus.$on("closeItemDetails", (data: any) => {
    //   localThis.showItmDetails = false;
    //   localThis.reloadItems();
    //   EventBus.$emit("showSrvTabs");
    // });

    // EventBus.$on("reloadItems", (data: any) => {
    //   localThis.reloadItems(data);
    // });
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
