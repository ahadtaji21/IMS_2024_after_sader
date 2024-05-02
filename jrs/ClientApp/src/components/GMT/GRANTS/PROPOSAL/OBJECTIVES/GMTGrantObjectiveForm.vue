<template>
  <div>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <!-- OBJ SELECTION-->
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
            <div
              class="d-inline-flex align-center primary lighten-2 jrs-table-top"
            >
              <h3>
                {{ $tc("datasource.gmt.gmt-obj", 2) }}
              </h3>

              <v-spacer></v-spacer>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="switchItems()"
              >
                <v-icon>{{ sIcon }}</v-icon>
              </v-btn>
              <v-spacer></v-spacer>
              <span v-if="isUpdatableForm">
              <v-dialog v-model="dialog" persistent max-width="1100px">
                <template v-slot:activator="{ on }" v-if="!onlyRead">
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    v-on="on"
                    @click="clearEditItem"
                    
                  >
                    <v-icon>mdi-plus-circle-outline</v-icon>New
                  </v-btn>
                </template>
                <gt-addobj-form
                  @closeDialoge="closeDialog"
                  @closeDialogeAndSave="closeDialogAndSave"
                  :formData="editedItemDialog"
                  :isUpdatableForm="isUpdatableForm"
               ></gt-addobj-form>
              </v-dialog>
              </span>
              <v-spacer></v-spacer>

              <v-text-field
                v-model="tableFilter"
                :readonly ="onlyRead"
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

          <template v-slot:[`item.action`]="{ item }">
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-icon v-if="!onlyRead" small class="mr-2" @click="editItem(item)" v-on="on"
                  >mdi-circle-edit-outline</v-icon
                >
                 <v-icon v-if="onlyRead" small class="mr-2" @click="editItem(item)" v-on="on"
                  >mdi-eye</v-icon
                >
              </template>
              <span>{{
                $t("datasource.pms.annual-plan.objectives.edit")
              }}</span>
            </v-tooltip>
            <!-- <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editNarrative(item);" v-on="on">mdi-text</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.narrative.title') }}</span>
              </v-tooltip>
              <v-tooltip bottom>
                <template v-slot:activator="{ on }">
                  <v-icon small class="mr-2" @click="editServices(item);" v-on="on">mdi-cog</v-icon>
                </template>
                <span>{{ $t('datasource.pms.annual-plan.objectives.services') }}</span>
            </v-tooltip>-->
            <v-tooltip bottom v-if="isUpdatableForm">
              <template v-slot:activator="{ on }">
                 <v-icon v-if="!onlyRead"  small class="mr-2" @click="deleteItem(item)" v-on="on"
                  >mdi-delete</v-icon
                >
              </template>
              <span>{{
                $t("datasource.pms.annual-plan.objectives.delete")
              }}</span>
            </v-tooltip>
          </template>
        </v-data-table>
      </v-col>
    </v-row>
    <v-row>
      <v-col align-center :cols="12">
        <div v-if="showObjDetails">
          <v-breadcrumbs :items="itemsBreadCumb">
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
          </v-breadcrumbs>

          <gt-obj-details
            :editObjMainDataItem="editedItem"
            :key="editId"
            :editObjId="editId"
            :editObjDesc="editObjDesc"
            :showSrvDetails="showSrvDetails"
            @reloadObjectives="reloadObjectives"
            @closeObjectiveDetails="closeObjectiveDetails"
            @hideObjDetails="hideObjDetails"
            :isUpdatableForm="isUpdatableForm"
            :onlyRead="onlyRead"

          ></gt-obj-details>
        </div>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { VueEditor } from "vue2-editor";
import { translate } from "../../../../../i18n";
import AddObjective from "./GMTGrantObjectiveMainDataForm.vue";
// import Narrative from "../PMS/AnnualPlanNarrative.vue";
// import Services from "../PMS/AnnualPlanServiceForm.vue";
import ObjDetails from "./GMTGrantObjectiveDetailsForm.vue";
// Import the EventBus.
import { EventBus } from "../../../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

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
    "gt-obj-details": ObjDetails,
    "gt-addobj-form": AddObjective,
    //  "jrs-simple-table": JrsSimpleTable
  },
  props: {
    selectedGrant: {
      type: Object,
    },
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
    onlyRead :  {
      type: Boolean,
      required: false,
      default:false
    }
  },
  data() {
    return {
      showWait: false,
      itemsBreadCumb: [],
      componentKey: 0,
      showObjDetails: false,
      showSrvDetails: true,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      editObjDesc: "",
      editId: null,
      editObj: null,
      objectiveList: [],
      objectiveListOrig: [],
      tmpSelectedItem: [],
      tableFilter: "",
      editedItem: {},
      editedItemDialog: {},
      itemModel: {
        ID: null,
        DESCRIPTION: null,
        GRANT_ID: null,
      },
      dialog: false,
      columnsTemplate: [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.ap-brief-description")
            .toString(),
          textKey: "datasource.pms.annual-plan.ap-brief-description",
          align: "left",
          value: "DESCRIPTION",
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
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }

        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

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
      localThis.getOBJ().then((res: any) => {
        if (
          localThis.tmpSelectedItem &&
          localThis.tmpSelectedItem.length == 1
        ) {
          localThis.tmpSelectedItem = [];
          if (item) {
            let i = 0;
            for (i = 0; i < localThis.objectiveList.length; i++) {
              if (localThis.objectiveList[i].ID == item.ID) {
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.objectiveList[i]))
                );
                localThis.objectiveList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
        }
      });
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
    switchItems() {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.objectiveList = localThis.objectiveListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.objectiveList = localThis.tmpSelectedItem;
      }
    },
    contractTable(item: any) {
      let localThis: any = this as any;
      localThis.tmpSelectedItem = [];
      localThis.tmpSelectedItem.push(item);
      localThis.objectiveList = localThis.tmpSelectedItem;

      localThis.itemsPerPage = 1;
      localThis.sIcon = "mdi-chevron-double-down";
    },
    clearEditItem() {
      let localThis = this as any;
      localThis.editedItemDialog = {};
      localThis.editedItemDialog.GRANT_ID = localThis.selectedGrant.ID;
    },
    closeDialog() {
      let localThis = this as any;
      localThis.dialog = false;
      // localThis.editedItem = localThis.itemModel;
      // localThis.editedItem.GRANT_ID = localThis.selectedGrant.id;
      localThis.editedItem = {};
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
      value.GRANT_ID = localThis.selectedGrant.ID;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_GRANT_OBJECTIVE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(value),
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.getOBJ();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              // localThis.editedItem = localThis.itemModel;
              // localThis.editedItem.GRANT_ID =
              //   localThis.selectedGrant.id;
              localThis.editedItemDialog = {};
              if (localThis.dialog == false) {
                if (
                  localThis.tmpSelectedItem &&
                  localThis.tmpSelectedItem.length == 1
                ) {
                  localThis.tmpSelectedItem = [];
                  localThis.tmpSelectedItem.push(
                    JSON.parse(JSON.stringify(value))
                  );
                  localThis.objectiveList = localThis.tmpSelectedItem;
                }
              } else {
                localThis.dialog = false;
                localThis.showObjDetails = false;
                localThis.tmpSelectedItem = [];
                localThis.objectiveList = localThis.objectiveListOrig;
                localThis.itemsPerPage = 15;
              }
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
    closeObjectiveDetails() {
      let localThis = this as any;
      localThis.showObjDetails = false;
    },

    editItem(item: any) {
      let localThis = this as any;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.editObjDesc = localThis.editedItem.DESCRIPTION;
      localThis.selectedObjective = [];
      localThis.editId = item.ID;
      localThis.editObj = item.DESCRIPTION;
      localThis.selectedObjective.push(item);
      localThis.contractTable(item);
      localThis.showObjDetails = true;
      localThis.showSrvDetails = true;
      localStorage.SelectedObj = localThis.editObjDesc;
      localThis.refreshBreadCumb(1);
    },
    editNarrative(item: any) {
      let localThis = this as any;
      localThis.selectedObjective = [];
      localThis.editId = item.ID;
      localThis.editObj = item.DESCRIPTION;
      localThis.showItemNumber = 1;
      // localThis.selectedObjective =Object.assign([], item) ;//Object.assign(item);// JSON.parse(JSON.stringify(item));
      localThis.selectedObjective.push(item);
      localThis.contractTable(item);
    },

    editServices(item: any) {
      let localThis = this as any;
      localThis.selectedObjective = [];
      localThis.editId = item.ID;
      localThis.editObj = item.DESCRIPTION;
      localThis.showItemNumber = 2;
      // localThis.selectedObjective =Object.assign([], item) ;//Object.assign(item);// JSON.parse(JSON.stringify(item));
      localThis.selectedObjective.push(item);
      localThis.contractTable(item);
    },

    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },
    getOBJ() {
      let localThis = this as any;
      localThis.showWait = true;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_OBJECTIVE",
        colList: "ID, DESCRIPTION, GRANT_ID",
        whereCond: `GRANT_ID = ${localThis.selectedGrant.ID}`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.objectiveList = [...(res.table_data ? res.table_data : [])];
          localThis.objectiveListOrig = [
            ...(res.table_data ? res.table_data : []),
          ];
          if (
            localThis.tmpSelectedItem &&
            localThis.tmpSelectedItem.length == 1
          ) {
            let i = 0;
            for (i = 0; i < localThis.objectiveList.length; i++) {
              if (
                localThis.objectiveList[i].ID == localThis.tmpSelectedItem.ID
              ) {
                localThis.tmpSelectedItem = [];
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.objectiveList[i]))
                );
                localThis.indicatorList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
          localThis.showWait = false;
        });
    },

    /**
     * Clear narrative sections.
     * @param projectId - ID of the project
     */
    clearNarrativeData(projectId: number) {
      let localThis: any = this as any;
    },
  },
  mounted() {
    let localThis = this as any;

    localThis.editedItem = localThis.itemModel;
    localThis.editObjDesc = localThis.editedItem.DESCRIPTION;
    localThis.editedItem.GRANT_ID = localThis.selectedGrant.ID;
    localThis.getOBJ();
    let vl = [];
    vl.push("test");
    localThis.refreshBreadCumb(1);
    EventBus.$on("refreshBreadCumb", (data: any) => {
      localThis.refreshBreadCumb(data);
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
</style>
