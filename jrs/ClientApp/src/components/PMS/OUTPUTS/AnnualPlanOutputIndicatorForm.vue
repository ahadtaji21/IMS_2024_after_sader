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
        item-key="PMS_OUTP_IND_REL_ID"
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
                @UpdateOutputIndicators="updateOutputIndicators"
                :selectedOutId="formData.selectedOutputId"
                :selectedProcOutRelId="formData.selectedProcessOutputRelId"
                :indicatorsAlreadyAdded="indicatorList"
                :key="Math.ceil(Math.random() * 1000)"
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
              <td class="tablerow">{{ item.PMS_OUTP_IND }}</td>
              <!-- <td class="tablerow">{{ item.STANDARD | subStr }}</td> -->
              <!-- <td class="tablerow">{{ item.PMS_OUTP_IND_PERIODICITY }}</td>
              <td class="tablerow">{{ item.PMS_OUTP_IND_DISAGGREGATED }}</td> -->
              <td class="tablerow">{{ item.PMS_OUTP_IND_TECHNIQUE }}</td>
              <td class="tablerow">{{ item.PMS_PROC_OUTP_END_DATE | formatDate }}</td>
              <td v-if="false" class="tablerow">{{ item.VALUE }}</td>
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
      <pms-outp-ind-det-form
        @closeOutpDetDialoge="closeOutpDialog"
        @closeDialogeAndSaveOutpInd="closeOutpDialogAndSave"
        :formData="localFormData"
        :selectedOutpId="formData.selectedOutputId"
        :versioned="versioned"
        :isUpdatableForm="!onlyRead"
        :key="Math.ceil(Math.random() * 1000)"
      ></pms-outp-ind-det-form>
    </v-dialog>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanOutputIndicatorSearch.vue";
import outpIndDet from "./AnnualPlanOutputIndicatorMainDataForm.vue";
import UtilMix from "../../../mixins/UtilMix";

// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
//import ItemIndicator from "./../OUTPUTS/AnnualPlanIndicatorDetailsForm.vue";
import { i18n } from "../../../i18n";
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
/* localThis.localFormData.selectedProcessId = localThis.selectedProcessId;
      localThis.localFormData.selectedActivityId = localThis.selectedActivityId;
      localThis.localFormData.selectedOutputId = localThis.selectedOutputId;
      localThis.localFormData.selectedOutput = localThis.selectedOutput;*/
// }

export default Vue.extend({
  components: {
    "search-table": SearchTable,
    "pms-outp-ind-det-form": outpIndDet,
  },
  mixins: [UtilMix],
  props: {
    formData: {
      type: Object,
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
      editOutp: null,
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

      localFormData: {},
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
    closeOutpDialog() {
      let localThis: any = this as any;
      localThis.showIndicatorDetails = false;
      this.$emit("reloadOutputList");
    },
    closeOutpDialogAndSave() {
      let localThis: any = this as any;
      localThis.showIndicatorDetails = false;
      localThis.itemsPerPage = 15;
      localThis.getOutputIndicators();
      this.$emit("reloadOutputList");
    },
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    closeDialog(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) {
        localThis.itemsPerPage = 15;
        localThis.getOutputIndicators();
        this.$emit("reloadOutputList");
      }
    },

    updateOutputIndicators(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) {
        localThis.itemsPerPage = 15;
        localThis.UpdateItems(item);
      }
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getOutputIndicators();
      this.$emit("reloadOutputList");
    },
    getOutputIndicators() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.indicatorList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_OUTPUT_INDICATOR";
      let wherecond: string = `PMS_PROC_OUTP_REL_ID = ${localThis.formData.selectedProcessOutputRelId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OUTPUT_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_OUTP_IND",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              if (item.STANDARD == undefined || item.STANDARD == "")
                item.STANDARD = item.PMS_OUTP_IND_STANDARD;
              localThis.indicatorList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },

    UpdateItems(item: any) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      let i: number = 0;
      let j: number = 0;
      for (i = 0; i < item.length; i++) {
        payLoadD.PMS_OUTP_IND_REL_ID = 0;
        payLoadD.PMS_OUTP_IND_ID = item[i].PMS_OUTP_IND_ID;
        payLoadD.PMS_PROC_OUTP_REL_ID = localThis.formData.selectedProcessOutputRelId;

        let savePayload: GenericSqlPayload = {
          spName: "PMS_SP_INS_OUTPUT_INDICATOR",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        const response = localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            j++;
            if (j == item.length) {
              localThis.getOutputIndicators();
              this.$emit("reloadOutputList");
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

    editIndicator(item: any) {
      let localThis = this as any;

      localThis.localFormData = {} as any;
      localThis.localFormData.outpIndicatorRelId = item.PMS_OUTP_IND_REL_ID;
      localThis.localFormData.outpIndicatorId = item.PMS_OUTP_IND_ID;
      localThis.localFormData.QUESTION = item.PMS_OUTP_IND_QUESTION;
      localThis.localFormData.INDICATOR = item.PMS_OUTP_IND;
      localThis.localFormData.VALUE = item.VALUE;

   

      localThis.localFormData.STANDARD = item.STANDARD;
      localThis.localFormData.TECHNIQUE = item.PMS_OUTP_IND_TECHNIQUE;
      localThis.localFormData.END_DATE = item.PMS_PROC_OUTP_END_DATE;
      localThis.localFormData.DISAGGREGATED = item.PMS_OUTP_IND_DISAGGREGATED;
      localThis.localFormData.PERIODICITY = item.PMS_OUTP_IND_PERIODICITY;
      localThis.localFormData.VALUE = item.VALUE;


      localThis.localFormData.TARGET_TYPE = item.TARGET_TYPE;
      localThis.localFormData.TARGET = item.TARGET;
      if(item.NOT_DISAGGREGABLE != undefined && item.NOT_DISAGGREGABLE.toUpperCase()=="TRUE")
      localThis.localFormData.NOT_DISAGGREGABLE=true;
      else
      localThis.localFormData.NOT_DISAGGREGABLE=false;

      localThis.localFormData.MALE = item.MALE;
      localThis.localFormData.FEMALE = item.FEMALE;
      localThis.localFormData.OTHERS = item.OTHERS;

      localThis.localFormData.AGE_RANGE = item.AGE_RANGE;
      localThis.localFormData.DISABILITED = item.DISABLED;


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
          payLoadD.PMS_OUTP_IND_REL_ID = item.PMS_OUTP_IND_REL_ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_OUTPUT_INDICATOR",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getOutputIndicators();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              this.$emit("reloadOutputList");
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
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "PMS_OUTP_IND",
          align: "center",
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.indicator.standard").toString(),
        //   value: "STANDARD",
        //   align: "center",
        //   divider: true,
        // },
        // {
        //   text: this.$i18n.t("pms.indicator.periodicity").toString(),
        //   value: "PMS_OUTP_IND_PERIODICITY",
        //   align: "center",
        //   divider: true,
        // },
        // {
        //   text: this.$i18n.t("pms.indicator.disaggregated").toString(),
        //   value: "PMS_OUTP_IND_DISAGGREGATED",
        //   align: "center",
        //   divider: true,
        // },
        {
          text: this.$i18n.t("pms.indicator.techniques").toString(),
          value: "PMS_OUTP_IND_TECHNIQUE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.indicator.end-date").toString(),
          value: "PMS_PROC_OUTP_END_DATE",
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

    if (localThis.formData.selectedOutputId > 0) {
      localThis.getOutputIndicators();
    }
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
  filters: {
    subStr: function (string: any) {
      if (string != undefined) {
        if (string.length < 25) return string;
        else return string.substring(0, 25) + "...";
      } else return "";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getOutputIndicators();
      localThis.setupHeaders();
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
