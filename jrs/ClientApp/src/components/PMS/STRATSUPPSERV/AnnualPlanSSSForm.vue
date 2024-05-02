<template>
  <div>
    <div v-if="!showOutpDetails">
      <!-- ITM SELECTION-->
      <v-data-table
        :headers="headers"
        :items="sssList"
        item-key="PMS_SSS_ID"
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
            <h3>{{ $t("datasource.pms.annual-plan.objectives.strat-supp-serv") }}</h3>
            <v-spacer></v-spacer>
            <span v-if="!onlyRead">
              <v-dialog
                v-model="editMode"
                persistent
                retain-focus
                :scrollable="true"
                :overlay="false"
                transition="dialog-transition"
                max-width="45em"
              >
                <template v-slot:activator="{ on }">
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    v-on="on"
                    @click="newSSS()"
                  >
                    <v-icon>mdi-plus-circle-outline</v-icon>New
                  </v-btn>
                </template>
                <ap-sss
                  :isUpdatableForm="!onlyRead"
                  :formDataExt="formData"
                  @closeDialogSSS="closeDialogSSS"
                  @saveDialoge="saveDialoge"
                  :versioned="versioned"
                  @generateError="generateError"
                  :key="Math.ceil(Math.random() * 1000)"
                ></ap-sss>
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
        <template v-slot:body="{ items }">
          <tbody style="border: solid">
            <tr v-for="item in items" :key="item.PMS_SSS_ID" style="height: 10px">
              <!-- <td class="tablerow">{{ item.PMS_STRAT_SUPP_SERV_TARGET_DESC }}</td> -->
              <td class="tablerow">{{ item.PMS_STRAT_SUPP_SERV_DESC }}</td>
              <td style="text-align: center">
                <!-- <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editSSS(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>

                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.sss-details")
                  }}</span>
                </v-tooltip> -->

                <v-tooltip bottom v-if="!onlyRead">
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                      >mdi-delete</v-icon
                    >
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.delete-sss")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
    <div>
      <div v-if="showOutpDetails">
        <item-sss-details
          :formData="formData"
          :key="Math.ceil(Math.random() * 1000)"
          @reloadSSS="strategicSupportServiceDialogReload"
          :onlyRead="onlyRead"
          :versioned="versioned"
        ></item-sss-details>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import AnnualSSS from "./AnnualPlanSSSMainDataForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import ItemSSSDetails from "./AnnualPlanSSSDetailsForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { IPayLoadDataITEM, IPayLoadDataOUTPUT } from "./../PMSSharedInterfaces";
import UtilMix from "../../../mixins/UtilMix";
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
    "ap-sss": AnnualSSS,
    "item-sss-details": ItemSSSDetails,
  },
  props: {
    selectedAnnualPlanId: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  mixins: [UtilMix],
  data() {
    return {
      onlyReadLocal: false,
      sssId: {},
      showItmTabs: true,
      selectedSSS: "",
      selectedSSSId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
      showOutpDetails: false,
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
      sssList: [],
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

    closeDialogSSS(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
    },
    closeSSSDialog(item: string) {
      let localThis: any = this as any;
      localThis.showOutpDetails = false;
      EventBus.$emit("showActTabs");
    },
    saveDialoge() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.strategicSupportServiceDialogReload();
    },
    newSSS() {
      let localThis: any = this as any;
      localThis.formData = {};
      localThis.formData.selectedAnnualPlanId = localThis.selectedAnnualPlanId;
      localThis.formData.STRAT_SUPP_SERV = [] as any;
      localThis.formData.STRAT_SUPP_SERV[0] = {} as any;
      localThis.editMode = true;
    },

    strategicSupportServiceDialogReload(item: string) {
      let localThis: any = this as any;
      //localThis.showOutpDetails = false;
      EventBus.$emit("showActTabs");
      localThis.reloadItems(null);
    },
    closeSSSDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.showOutpDetails = false;
      EventBus.$emit("showActTabs");
      localThis.reloadItems(null);
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getAnnualPlanSSSs();
    },
    getAnnualPlanSSSs() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.sssList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_STRAT_SUPP_SERV";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_STRAT_SUPP_SERV_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.sssList.push(item);
            });
          }
        }
      );
    },

    editSSS(item: any) {
      let localThis = this as any;
      if (localThis.onlyRead) return;
      localThis.formData = {};
      localThis.formData.selectedOvgId = item.PMS_SSS_KEY_ID;
      localThis.formData.selectedAnnualPlanId = localThis.selectedAnnualPlanId;
      // localThis.formData.PMS_TARGET_ID_STR = item.PMS_TARGET_ID + "";
      localThis.formData.STRAT_SUPP_SERV = [] as any;
      localThis.formData.STRAT_SUPP_SERV[0] = {} as any;
      localThis.formData.STRAT_SUPP_SERV[0].PMS_CONSTRUCTION_DEF_ID = item.PMS_SSS_ID;
      //localThis.showOutpDetails = true;
      localThis.editMode = true;
      //EventBus.$emit("hideActTabs");
      //EventBus.$emit("showOutpTabs");
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
          payLoadD.PMS_SSS_KEY_ID = item.PMS_SSS_KEY_ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_SSS",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getAnnualPlanSSSs();
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
        //   text: this.$i18n
        //     .t("datasource.pms.annual-plan.objectives.outcome-obj-target")
        //     .toString(),
        //   value: "PMS_STRAT_SUPP_SERV_TARGET_DESC",
        //   align: "center",
        //   divider: true,
        // },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.strat-supp-serv")
            .toString(),
          value: "PMS_STRAT_SUPP_SERV_DESC",
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
  beforeDestroy() {
    //do something before destroying vue instance
    EventBus.$off("hideActTabs");
    EventBus.$off("showActTabs");
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedItem = null;
    localThis.onlyReadLocal = localThis.onlyRead;
    localThis.setupHeaders();

    if (localThis.selectedAnnualPlanId > 0) {
      localThis.getAnnualPlanSSSs();
    }

    EventBus.$on("closeItemDetails", (data: any) => {
      localThis.showOutpDetails = false;
      localThis.reloadItems();
      EventBus.$emit("showActTabs");
    });

    EventBus.$on("hideOutpDetails", (data: any) => {
      localThis.showOutpDetails = false;
    });

    EventBus.$on("reloadItems", (data: any) => {
      localThis.reloadItems(data);
    });

    // EventBus.$on("userRoleUpdated", (to: any) => {
    //   if (to != undefined && (to.ROLE_CODE == "PC" || to.ROLE_CODE == "PD")) {
    //     //Program Coordinator
    //     localThis.onlyReadLocal = false;
    //   } else {
    //     localThis.onlyReadLocal = true;
    //   }
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
      localThis.getAnnualPlanSSSs();
      localThis.setupHeaders();
    },
    selectedAnnualPlanId(to: number) {
      let localThis: any = this as any;
      localThis.selectedAnnualPlanId = to;
      localThis.getAnnualPlanSSSs();
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
