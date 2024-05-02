<template>
  <div>
    <!-- OBJ SELECTION-->

    <v-row align-center v-if="showWait">
      <v-col justify-center :cols="12">
        <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-row align-center :cols="12" v-if="!showObjDetails">
      <v-col justify-center>
        <v-data-table
          :headers="columns"
          v-model="selectedObjective"
          :items="objectiveList"
          item-key="ID"
          :items-per-page="itemsPerPage"
          multi-sort
          :search="tableFilter"
          class="text-capitalize"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>
                {{ $tc("datasource.pms.annual-plan.objectives.outcome-obj-title", 2) }}
              </h3>

              <v-spacer></v-spacer>
              <span v-if="isUpdatableForm">
                <v-dialog
                  v-model="dialog"
                  persistent
                  :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
                >
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3"
                      v-on="on"
                      @click="clearEditItem"
                    >
                      <v-icon>mdi-plus-circle-outline</v-icon
                      >{{ $tc("pms.add-target", 2) }}
                    </v-btn>
                  </template>
                  <pms-addobj-form
                    @closeDialoge="closeDialog"
                    @closeDialogeAndSaveObj="SaveObjMainData"
                    :formDataExt="editedItemDialog"
                    :isUpdatableForm="isUpdatableForm"
                    :key="Math.ceil(Math.random() * 1000)"
                    :versioned="versioned"
                    :allowMultiselect="true"
                  ></pms-addobj-form>
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
              <tr v-for="(item, index) in items" :key="item.ID" style="height: 10px">
                <td class="tablerow">
                  <v-checkbox
                    v-model="selected[index]"
                    value
                    hide-details
                    class="shrink ma-0 pa-0"
                    @change="checkboxUpdated($event, index, item.ID, item)"
                  ></v-checkbox>
                </td>

                <td class="tablerow">{{ item.PMS_OBJ_OUTCOME }}</td>
                <td class="tablerow">{{ item.PMS_TARGET_DESC }}</td>
                <!-- <td class="tablerow">{{ item.PMS_OBJ_OUTCOME }}</td> -->
                <!-- <td class="tablerow">{{ item.SERVED_PEOPLE }}</td> -->
                <td
                  v-if="
                    module == 'IMP' &&
                    item.ACTIVATION_STATE == 0 &&
                    item.SERVICE_NUMBER > 0
                  "
                  class="tablerow"
                >
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-icon color="red" small class="mr-2" v-on="on">mdi-circle</v-icon>
                    </template>
                    <span>{{
                      $t("datasource.pms.annual-plan.objectives.implementation-status-no")
                    }}</span>
                  </v-tooltip>
                </td>
                <td
                  v-if="
                    module == 'IMP' &&
                    item.ACTIVATION_STATE > 0 &&
                    item.SERVICE_NUMBER > 0
                  "
                  class="tablerow"
                >
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-icon color="yellow" small class="mr-2" v-on="on"
                        >mdi-circle</v-icon
                      >
                    </template>
                    <span>{{
                      $t(
                        "datasource.pms.annual-plan.objectives.implementation-status-partial"
                      )
                    }}</span>
                  </v-tooltip>
                </td>
                <td
                  v-if="module == 'IMP' && item.ACTIVATION_STATE == item.SERVICE_NUMBER"
                  class="tablerow"
                >
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-icon color="green" small class="mr-2" v-on="on"
                        >mdi-circle</v-icon
                      >
                    </template>
                    <span>{{
                      $t(
                        "datasource.pms.annual-plan.objectives.implementation-status-yes"
                      )
                    }}</span>
                  </v-tooltip>
                </td>
                <td style="text-align: center">
                  <span v-if="!onlyRead">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                          >mdi-circle-edit-outline</v-icon
                        >
                      </template>
                      <span>{{ $t("datasource.pms.annual-plan.objectives.edit") }}</span>
                    </v-tooltip>
                  </span>
                  <span v-if="onlyRead">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                          >mdi-eye</v-icon
                        >
                      </template>
                      <span>{{ $t("datasource.pms.annual-plan.objectives.edit") }}</span>
                    </v-tooltip>
                  </span>
                  <span v-if="isUpdatableForm">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                          >mdi-delete</v-icon
                        >
                      </template>
                      <span>{{
                        $t("datasource.pms.annual-plan.objectives.delete")
                      }}</span>
                    </v-tooltip>
                  </span>
                </td>
              </tr>
            </tbody>
          </template>
        </v-data-table>
      </v-col>
    </v-row>
    <v-row>
      <v-col align-center :cols="12">
        <div v-if="showObjDetails">
          <!-- <v-breadcrumbs :items="itemsBreadCumb">
            <v-breadcrumbs-item
              slot="item"
              slot-scope="{ item }"
              exact
              :to="item.to"
              click.native="handleFunctionCall(item.act, $event)"
            >
              <v-chip class="ma-2" :color="item.color">
                <v-avatar left>
                  <v-icon>{{ item.icon }}</v-icon>
                </v-avatar>
                {{ item.text }}
              </v-chip>
            </v-breadcrumbs-item>
            <template v-slot:divider>
              <v-icon>mdi-forward</v-icon>
            </template>
          </v-breadcrumbs> -->

          <ap-obj-details
            :editObjMainDataItem="editedItem"
            :key="keyRnd"
            :editObjId="editId"
            :editObjDesc="editObjDesc"
            :showSrvDetails="showSrvDetails"
            @reloadObjectives="reloadObjectives"
            @closeObjectiveDetails="closeObjectiveDetails"
            @hideObjDetails="hideObjDetails"
            :isUpdatableForm="isUpdatableForm"
            :onlyRead="onlyRead"
            :versioned="versioned"
            :cascade="true"
          ></ap-obj-details>
        </div>
      </v-col>
    </v-row>
    <v-row v-if="showCategories">
      <v-col justify-center :cols="12">
        <cat-form
          :onlyRead="onlyRead"
          :formData="formData"
          :showItemDetails="true"
          :versioned="versioned"
          @reloadObjList="reloadObjectives"
          :key="keyRnd"
        ></cat-form>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import { VueEditor } from "vue2-editor";
import { translate } from "../../../i18n";
import AddObjective from "../OBJECTIVES/AnnualPlanObjectiveMainDataForm.vue";
// import Narrative from "../PMS/AnnualPlanNarrative.vue";
// import Services from "../PMS/AnnualPlanServiceForm.vue";
import ObjDetails from "../OBJECTIVES/AnnualPlanObjectiveDetailsForm.vue";
import Categories from "./AnnualPlanCascCatForm.vue";
// Import the EventBus.
import { EventBus } from "../../../event-bus";
import UtilMix from "../../../mixins/UtilMix";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface NarrativeSection {
  ID: number | null;
  CODE: string;
  REFERENCE_ID: number;
  SECTION_TITLE?: string;
  SECTION_TEXT: string;
  REMOVED?: boolean;
}

export default Vue.extend({
  components: {
    // "ap-narrative": Narrative,
    // "ap-services": Services,
    "ap-obj-details": ObjDetails,
    "pms-addobj-form": AddObjective,
    "cat-form": Categories,
    //  "jrs-simple-table": JrsSimpleTable
  },
  props: {
    selectedAnnualPlan: {
      type: Object,
    },
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: true,
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
      showCategories: false,
      selected: [],
      showWait: false,
      module: "",
      keyRnd: 0,
      nbrOfColumns: 2,
      itemsBreadCumb: [],
      componentKey: 0,
      showObjDetails: false,
      showSrvDetails: true,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: -1,
      showItemNumber: 0,
      editObjDesc: "",
      tblName: "PMS_OBJECTIVE",
      editId: null,
      editObj: null,
      objectiveList: [],
      formData: {},
      tableFilter: "",
      editedItem: {},
      editedItemDialog: {},
      itemModel: {
        ID: null,
        DESCRIPTION: null,
        ANNUAL_PLAN_ID: null,
      },
      dialog: false,
      columnsTemplate: [
        {
          text: "",
          value: "",
          align: "center",
          divider: true,
          width: "20px",
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.outcome").toString(),
          textKey: "datasource.pms.annual-plan.objectives.outcome",
          align: "left",
          value: "PMS_OBJ_OUTCOME",
          sortable: true,
          filterable: true,
        },

        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          textKey: "datasource.pms.annual-plan.ap-obj-target",
          align: "left",
          value: "PMS_TARGET_DESC",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: this.$i18n
        //     .t("datasource.pms.annual-plan.ap-brief-description")
        //     .toString(),
        //   textKey: "datasource.pms.annual-plan.ap-brief-description",
        //   align: "left",
        //   value: "DESCRIPTION",
        //   sortable: true,
        //   filterable: true,
        // },
        // {
        //   text: this.$i18n
        //     .t("datasource.pms.annual-plan.objectives.est-srv-nbr")
        //     .toString(),
        //   textKey: "datasource.pms.annual-plan.objectives.est-srv-nbr",
        //   align: " left",
        //   value: "SERVED_PEOPLE",
        //   sortable: true,
        //   filterable: true,
        // },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          textKey: "datasource.pms.annual-plan.ap-obj-target",
          align: " d-none",
          value: "OUTCOME",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      columnsTemplateImp: [
        {
          text: "",
          value: "",
          align: "center",
          divider: true,
          width: "20px",
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.outcome").toString(),
          textKey: "datasource.pms.annual-plan.objectives.outcome",
          align: "left",
          value: "PMS_OBJ_OUTCOME",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          textKey: "datasource.pms.annual-plan.ap-obj-target",
          align: "left",
          value: "PMS_TARGET_DESC",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: this.$i18n
        //     .t("datasource.pms.annual-plan.ap-brief-description")
        //     .toString(),
        //   textKey: "datasource.pms.annual-plan.ap-brief-description",
        //   align: "left",
        //   value: "DESCRIPTION",
        //   sortable: true,
        //   filterable: true,
        // },

        // {
        //   text: this.$i18n
        //     .t("datasource.pms.annual-plan.objectives.est-srv-nbr")
        //     .toString(),
        //   textKey: "datasource.pms.annual-plan.objectives.est-srv-nbr",
        //   align: " left",
        //   value: "SERVED_PEOPLE",
        //   sortable: true,
        //   filterable: true,
        // },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.imp-status")
            .toString(),
          textKey: "datasource.pms.annual-plan.objectives.imp-status",
          align: " left",
          value: "ACTIVATION_STATE",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          textKey: "datasource.pms.annual-plan.ap-obj-target",
          align: " d-none",
          value: "OUTCOME",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      activeTab: null,
      selectedObjective: [],
    };
  },
  computed: {
    ...mapGetters({
      getActiveModule: "getActiveModule",
    }),
    narrativeData() {
      let localThis: any = this as any;
      return [
        ...localThis.overallCustomSections,
        localThis.overallNarrative,
        localThis.outcomeNarrative,
        localThis.otherNarrative,
      ];
    },
    columns() {
      let cols = {};
      let localThis = this as any;
      if (localThis.module == "IMP") {
        cols = (this as any).columnsTemplateImp.map((col: any) => {
          if (col.value === "action") {
            return col;
          }

          let newCol = col;
          newCol.text = translate(col.textKey);

          return newCol;
        });
      } else {
        cols = (this as any).columnsTemplate.map((col: any) => {
          if (col.value === "action") {
            return col;
          }

          let newCol = col;
          newCol.text = translate(col.textKey);

          return newCol;
        });
      }

      return cols;
    },
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    handleFunctionCall(functionName: any, event: any) {
      let localThis = this as any;
      localThis[functionName](event);
    },
    checkboxUpdated(newValue: any, index: number, rel_id: number, item: any) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showCategories = newValue;
        if (newValue) {
          localThis.formData.selectedOBJId = item.ID; // localThis.objectiveList[index].ID;
          localThis.formData.PMS_TARGET_ID = item.PMS_TARGET_ID; // localThis.objectiveList[index].PMS_TARGET_ID;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      }
    },
    uncheckList() {
      let localThis = this as any;
      let i: number = 0;
      if (localThis.objectiveList == undefined || localThis.objectiveList.length == 0)
        return;
      for (i = 0; i < localThis.objectiveList.length; i++) {
        if (localThis.selected != undefined && localThis.selected[i] != undefined) {
          localThis.selected[i] = false;
        }
      }
    },
    refreshBreadCumb(item: Number) {
      let localThis = this as any;
      localThis.itemsBreadCumb = [];
      switch (item) {
        case 1:
          localThis.itemsBreadCumb.push({
            text: "Objective:" + localStorage.SelectedObj,
            disabled: true,
            act: "hideObjDetails",
            color: "default",
            icon: "mdi-target",
          });
          break;
        case 2:
          localThis.itemsBreadCumb.push({
            text: "Objective:" + localStorage.SelectedObj,
            disabled: false,
            act: "hideSrvDetails",
            color: "default",
            icon: "mdi-target",
          });
          localThis.itemsBreadCumb.push({
            text: "Service:" + localStorage.SelectedSrv,
            disabled: false,
            act: "hideObjDetails",
            color: "primary",
            icon: "mdi-cog",
          });
          break;
        case 3:
          localThis.itemsBreadCumb.push({
            text: "Objective:" + localStorage.SelectedObj,
            disabled: false,
            act: "hideObjDetails",
            color: "default",
            icon: "mdi-target",
          });
          localThis.itemsBreadCumb.push({
            text: "Service:" + localStorage.SelectedSrv,
            disabled: false,
            act: "hideObjDetails",
            color: "primary",
            icon: "mdi-cog",
          });
          localThis.itemsBreadCumb.push({
            text: "Indicator:" + localStorage.SelectedInd,
            disabled: false,
            act: "hideObjDetails",
            color: "secondary",
            icon: "mdi-chart-line",
          });
          break;
        case 4:
          localThis.itemsBreadCumb.push({
            text: "Objective:" + localStorage.SelectedObj,
            disabled: false,
            act: "hideObjDetails",
            color: "default",
            icon: "mdi-target",
          });
          localThis.itemsBreadCumb.push({
            text: "Service:" + localStorage.SelectedSrv,
            disabled: false,
            act: "hideObjDetails",
            color: "primary",
            icon: "mdi-cog",
          });
          localThis.itemsBreadCumb.push({
            text: "Process:" + localStorage.SelectedAct,
            disabled: false,
            act: "hideObjDetails",
            color: "yellow",
            icon: "mdi-pen",
          });
          break;
      }
    },

    reloadObjectives(item: any) {
      let localThis = this as any;
      localThis.getOBJ();
    },
    hideObjDetails() {
      let localThis = this as any;
      localThis.showObjDetails = false;

      localThis.refreshBreadCumb(1);
    },
    hideSrvDetails() {
      let localThis = this as any;
      localThis.showSrvDetails = false;
    },

    clearEditItem() {
      let localThis = this as any;
      localThis.editedItemDialog = {};
      localThis.editedItemDialog.ANNUAL_PLAN_ID = localThis.selectedAnnualPlan.id;
    },
    closeDialog() {
      let localThis = this as any;
      localThis.dialog = false;
      // localThis.editedItem = localThis.itemModel;
      // localThis.editedItem.ANNUAL_PLAN_ID = localThis.selectedAnnualPlan.id;
      localThis.editedItem = {};
    },

    SaveObjMainData(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["ID"] == undefined) {
        msg = msgAdd;
      } else {
        id = Number(value["ID"]);
      }
      this.$confirm(msg).then(async (res) => {
        if (res) {
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
          let i: number = 0;
          let value1 = {} as any;
          if (value.OUTCOME == undefined) return;
          for (i = 0; i < value.OUTCOME.length; i++) {
            value1 = JSON.parse(JSON.stringify(value));
            value1.OUTCOME = [];
            value1.OUTCOME[0] = value.OUTCOME[i];
            localThis.showWait = true;
            let savePayload: GenericSqlPayload = {
              spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OUTCOME_OBJECTIVE",
              actionType: SqlActionType.NUMBER_0,
              jsonPayload: JSON.stringify(value1),
            };
            try {
              let retvalue = await localThis.synchInsert(savePayload);
              localThis.showWait = false;
              if (retvalue.message.indexOf('"status":"error"') != -1) {
                throw retvalue;
              }
            } catch (error) {
              localThis.showWait = false;
              localThis.generateError(error);
              throw error;
            }
          }

          localThis.getOBJ();
          localThis.showNewSnackbar({
            typeName: "success",
            text: "Outcome objective inserted",
          }); // Feedback to user

          localThis.editedItemDialog = {};
          localThis.dialog = false;
          localThis.showCategories = false;
          localThis.uncheckList();
        }
      });
    },
    closeDialogAndSave(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["ID"] == undefined) {
        msg = msgAdd;
      } else {
        id = Number(value["ID"]);
      }
      value.ANNUAL_PLAN_ID = localThis.selectedAnnualPlan.id;
      this.$confirm(msg).then(async (res) => {
        if (res) {
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OUTCOME_OBJECTIVE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(value),
          };
          try {
            let retvalue = await localThis.synchInsert(savePayload);
          } catch (error) {
            localThis.generateError(error);
            throw error;
          }
        }

        localThis.getOBJ();
        localThis.showNewSnackbar({
          typeName: "success",
          text: "Outcome objective inserted",
        });
        localThis.showCategories = false;
        localThis.uncheckList();
        // Feedback to user
        // localThis.editedItem = localThis.itemModel;
        // localThis.editedItem.ANNUAL_PLAN_ID =
        //   localThis.selectedAnnualPlan.id;
        localThis.editedItemDialog = {};
        if (localThis.dialog == false) {
          if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
            localThis.tmpSelectedItem = [];
            localThis.tmpSelectedItem.push(JSON.parse(JSON.stringify(value)));
            localThis.objectiveList = localThis.tmpSelectedItem;
          }
        } else {
          localThis.dialog = false;
          localThis.showObjDetails = false;
          localThis.tmpSelectedItem = [];
          localThis.objectiveList = localThis.objectiveListOrig;
          localThis.itemsPerPage = -1;
        }
      });
    },
    closeObjectiveDetails() {
      let localThis = this as any;
      localThis.showObjDetails = false;
    },

    editItem(item: any) {
      let localThis = this as any;
      localThis.showCategories = false;
      localThis.uncheckList();
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
      item.PMS_TARGET_ID_STR = item.PMS_TARGET_ID + "";
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.editObjDesc = localThis.editedItem.PMS_OBJ_OUTCOME;
      localThis.selectedObjective = [];
      localThis.editId = item.ID;
      localThis.editObj = item.PMS_OBJ_OUTCOME;
      localThis.selectedObjective.push(item);

      localThis.showObjDetails = true;
      localThis.showSrvDetails = true;
      localStorage.SelectedObj = localThis.editObjDesc;
      localThis.refreshBreadCumb(1);
    },
    editNarrative(item: any) {
      let localThis = this as any;
      localThis.selectedObjective = [];
      localThis.editId = item.ID;
      localThis.editObj = item.PMS_OBJ_OUTCOME;
      localThis.showItemNumber = 1;
      // localThis.selectedObjective =Object.assign([], item) ;//Object.assign(item);// JSON.parse(JSON.stringify(item));
      localThis.selectedObjective.push(item);
    },

    editServices(item: any) {
      let localThis = this as any;
      localThis.selectedObjective = [];
      localThis.editId = item.ID;
      localThis.editObj = item.PMS_OBJ_OUTCOME;
      localThis.showItemNumber = 2;
      // localThis.selectedObjective =Object.assign([], item) ;//Object.assign(item);// JSON.parse(JSON.stringify(item));
      localThis.selectedObjective.push(item);
    },

    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
          localThis.showCategories = false;
          localThis.uncheckList();
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;
          localThis.showWait = true;
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_OBJECTIVE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showWait = false;
              if (res != undefined && (res + "").indexOf('"status":"error"') != -1) {
                localThis.generateError(
                  "The objective can't be deleted; Delete the connected resources first"
                );
              } else {
                localThis.itemsPerPage = -1;
                localThis.reloadObjectives();
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: res.message,
                }); // Feedback to user
              }
            })
            .catch((err: any) => {
              localThis.showWait = false;
              localThis.generateError(err);
            });
        }
      });
    },
    getOBJ() {
      let localThis = this as any;
      localThis.showWait = true;
      localThis.selected = [];
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE";
      let wherecond: string = `ANNUAL_PLAN_ID = ${localThis.selectedAnnualPlan.id}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList:
          "ID, DESCRIPTION, ANNUAL_PLAN_ID,PMS_TARGET_DESC,PMS_TARGET_ID,OUTCOME,SERVED_PEOPLE,ACTIVATION_STATE,SERVICE_NUMBER,PMS_OBJ_OUTCOME",
        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.objectiveList = [...(res.table_data ? res.table_data : [])];

          let i = 0;
          for (i = 0; i < localThis.objectiveList.length; i++) {
            localThis.selected.push(false);
          }
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.generateError(err);
        });
    },

    getOBJLeaveSelected() {
      let localThis = this as any;
      localThis.showWait = true;

      let view: string = "PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE";
      let wherecond: string = `ANNUAL_PLAN_ID = ${localThis.selectedAnnualPlan.id}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList:
          "ID, DESCRIPTION, ANNUAL_PLAN_ID,PMS_TARGET_DESC,PMS_TARGET_ID,OUTCOME,SERVED_PEOPLE,ACTIVATION_STATE,SERVICE_NUMBER,PMS_OBJ_OUTCOME",
        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.objectiveList = [...(res.table_data ? res.table_data : [])];

          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.generateError(err); // Feedback to user
        });
    },

    /**
     * Clear narrative sections.
     * @param projectId - ID of the project
     */
    clearNarrativeData(projectId: number) {
      let localThis: any = this as any;
    },
    /**
     * Load Project narrative from server and set in tabs.
     * @param projectId - ID of the project
     */

    /**
     * Save the project narrative sections.
     */
    saveNarrativeData() {
      let localThis: any = this as any;

      // Check if all overall sections have a title and a body
      let invalidSectionIndex = localThis.overallCustomSections.findIndex(
        (section: NarrativeSection) => {
          return (
            !section.REMOVED &&
            (section.SECTION_TITLE == null ||
              section.SECTION_TEXT == null ||
              section.SECTION_TITLE == "" ||
              section.SECTION_TEXT == "")
          );
        }
      );
      if (invalidSectionIndex > -1) {
        localThis.showNewSnackbar({
          typeName: "warning",
          text: "Warning: All overall sections defined must have a title and a body.",
        });
        return;
      }
      localThis.showWait = true;
      let savePayload: GenericSqlPayload = {
        spName: "IMS_SP_INS_UPD_NARRATIVE_SECTION",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(localThis.narrativeData),
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.generateError(err); // Feedback to user
        });
    },
  },
  beforeMount() {
    let localThis = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
  },
  mounted() {
    let localThis = this as any;
    localThis.editedItem = localThis.itemModel;
    localThis.editObjDesc = localThis.editedItem.PMS_OBJ_OUTCOME;
    localThis.editedItem.ANNUAL_PLAN_ID = localThis.selectedAnnualPlan.id;
    localThis.getOBJ();
    let vl = [];
    vl.push("test");
    localThis.refreshBreadCumb(1);
    EventBus.$on("refreshBreadCumb", (data: any) => {
      localThis.refreshBreadCumb(data);
    });
    EventBus.$on("reloadObj", (data: any) => {
      localThis.getOBJLeaveSelected(data);
    });
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
.tablerow {
  text-align: center;
  height: 3.5em;
  font-size: 12px;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
</style>
