<template>
  <div>
    <div class="text-center" v-if="showWait">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <div>
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="indicatorList"
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
            <h4>{{ $t("datasource.pms.annual-plan.objectives.indicators") }}</h4>

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
                v-model="indicatorId"
                @UpdateItem="closeDialog"
                :selectedObjId="selectedObjectiveId"
                :indicatorsAlreadyAdded="indicatorList"
                :key="Math.ceil(Math.random() * 1000)"
                @UpdateOUTCIndicators="updateOUTCIndicators"
              ></search-table>
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
              <td v-if="false" class="tablerow">{{ item.PMS_OBJ_IND_QUESTION }}</td>
              <td class="tablerow">{{ item.PMS_OBJ_IND }}</td>
              <!-- <td class="tablerow">{{ item.STANDARD }}</td>
              <td class="tablerow">{{ item.PMS_OBJ_IND_PERIODICITY }}</td>
              <td class="tablerow">{{ item.PMS_OBJ_IND_DISAGGREGATED }}</td> -->
              <td class="tablerow">{{ item.PMS_OBJ_IND_TECHNIQUE }}</td>
              <td class="tablerow">{{ item.END_DATE | formatDate }}</td>
              <!-- <td class="tablerow">{{ item.VALUE }}</td> -->
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editIndicator(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>

                  <span>{{ $t("pms.indicator-details") }}</span>
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
                    $t("datasource.pms.annual-plan.objectives.delete-indicator")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>

    <v-dialog
      v-model="showIndicatorDetails"
      persistent
      :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
    >
      <pms-obj-ind-det-form
        @closeObjDetDialoge="closeObjDialog"
        @closeDialogeAndSaveObjInd="closeObjDialogAndSave"
        :formData="formData"
        :selectedObjId="selectedObjectiveId"
        :versioned="versioned"
        :isUpdatableForm="!onlyRead"
        :key="Math.ceil(Math.random() * 1000)"
      ></pms-obj-ind-det-form>
    </v-dialog>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanObjectiveIndicatorSearch.vue";
import objIndDet from "./AnnualPlanObjectiveIndicatorMainDataForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
//import ItemIndicator from "./../OUTPUTS/AnnualPlanIndicatorDetailsForm.vue";
import { i18n } from "../../../i18n";
import UtilMix from "../../../mixins/UtilMix";
import { EventBus } from "../../../event-bus";
import { IPayLoadDataITEM, IPayLoadDataOUTPUT } from "./../PMSSharedInterfaces";

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
//   OBJECTIVE_ID: number | null;
// }

export default Vue.extend({
  components: {
    "search-table": SearchTable,
    "pms-obj-ind-det-form": objIndDet,
  },
  mixins: [UtilMix],
  props: {
    selectedObjectiveId: {
      type: Number,
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
    isUpdatableForm: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      showWait: false,
      nbrOfColumns: 2,
      showIndicatorDetails: false,
      indicatorId: {},
      showItmTabs: true,
      selectedIndicator: "",
      selectedIndicatorId: 0,
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
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      indicatorList: [],
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
    closeObjDialog() {
      let localThis: any = this as any;
      localThis.showIndicatorDetails = false;
    },
    closeObjDialogAndSave() {
      let localThis: any = this as any;
      localThis.showIndicatorDetails = false;
      localThis.itemsPerPage = 15;
      localThis.getObjectiveIndicators();
      this.$emit("reloadObjList");
    },
    updateOUTCIndicators(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) {
        localThis.itemsPerPage = 15;
        localThis.UpdateItems(item);
      }
    },
    UpdateItems(item: any) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      let i: number = 0;
      let j: number = 0;
      for (i = 0; i < item.length; i++) {
        payLoadD.PMS_OBJ_IND_REL_ID = 0;
        payLoadD.PMS_OBJ_ID = localThis.selectedObjectiveId;
        payLoadD.PMS_OBJ_IND_ID = item[i].PMS_OBJ_IND_ID;

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_OBJECTIVE_INDICATOR",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        const response = localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            j++;
            if (j == item.length) {
              localThis.getObjectiveIndicators();
              this.$emit("reloadObjList");
            }
            return res;
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
            throw err;
          });
      }
      localThis.itemsPerPage = -1;
      localThis.showNewSnackbar({ typeName: "success", text: "Indicators Updated" }); // Feedback to user
    },

    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    closeDialog(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) {
        localThis.itemsPerPage = 15;
        localThis.getObjectiveIndicators();
        this.$emit("reloadObjList");
      }
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getObjectiveIndicators();
      this.$emit("reloadObjList");
    },
    getObjectiveIndicators() {
      let localThis: any = this as any;
      localThis.showWait = true;
      localThis.selectedItem = null;
      localThis.indicatorList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_OBJ_INDICATOR";
      let wherecond: string = `PMS_OBJ_ID = ${localThis.selectedObjectiveId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OBJ_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_OBJ_IND_QUESTION",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              if (item.STANDARD == undefined || item.STANDARD == "")
                item.STANDARD = item.PMS_OBJ_IND_STANDARD;
              localThis.indicatorList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },

    editIndicator(item: any) {
      let localThis = this as any;

      localThis.formData = {} as any;
      localThis.formData.objIndicatorRelId = item.PMS_OBJ_IND_REL_ID;
      localThis.formData.objIndicatorId = item.PMS_OBJ_IND_ID;
      localThis.formData.QUESTION = item.PMS_OBJ_IND_QUESTION;
      localThis.formData.INDICATOR = item.PMS_OBJ_IND;
      localThis.formData.STANDARD = item.STANDARD;
      localThis.formData.TECHNIQUE = item.PMS_OBJ_IND_TECHNIQUE;
      localThis.formData.END_DATE = item.END_DATE;
      localThis.formData.DISAGGREGATED = item.PMS_OBJ_IND_DISAGGREGATED;
      localThis.formData.PERIODICITY = item.PMS_OBJ_IND_PERIODICITY;
      localThis.formData.VALUE = item.VALUE;
      localThis.showIndicatorDetails = true;
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
          payLoadD.PMS_OBJ_IND_REL_ID = item.PMS_OBJ_IND_REL_ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_OBJECTIVE_INDICATOR",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getObjectiveIndicators();
              this.$emit("reloadObjList");
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
        // {
        //   text: this.$i18n.t("pms.indicator-question").toString(),
        //   value: "PMS_OBJ_IND_QUESTION",
        //   align: "center",
        //   divider: true,
        // },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "PMS_OBJ_IND",
          align: "center",
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.indicator.standard").toString(),
        //   value: "PMS_OBJ_IND_STANDARD",
        //   align: "center",
        //   divider: true,
        // },
        // {
        //   text: this.$i18n.t("pms.indicator.periodicity").toString(),
        //   value: "PMS_OBJ_IND_PERIODICITY",
        //   align: "center",
        //   divider: true,
        // },
        // {
        //   text: this.$i18n.t("pms.indicator.disaggregated").toString(),
        //   value: "PMS_OBJ_IND_DISAGGREGATED",
        //   align: "center",
        //   divider: true,
        // },
        {
          text: this.$i18n.t("pms.indicator.techniques").toString(),
          value: "PMS_OBJ_IND_TECHNIQUE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.indicator.end-date").toString(),
          value: "END_DATE",
          align: "center",
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.indicator-estimated-value").toString(),
        //   value: "VALUE",
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
  beforeDestroy() {
    //do something before destroying vue instance
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedItem = null;
    localThis.setupHeaders();

    if (localThis.selectedObjectiveId > 0) {
      localThis.getObjectiveIndicators();
    }
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getObjectiveIndicators();
      this.$emit("reloadObjList");
      localThis.setupHeaders();
    },

    selectedObjectiveId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectiveId = to;
      localThis.getObjectiveIndicators();
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
