<template>
  <div>
    <div v-if="!showResDetails">
      <!-- RES SELECTION-->
      <v-data-table
        :headers="headers"
        :items="resourceList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
        v-model="selectedRes"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("datasource.pms.annual-plan.objectives.resources") }}</h3>

            <v-spacer></v-spacer>
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="switchItems()"
            >
              <v-icon>{{ sIcon }}</v-icon>
            </v-btn>
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
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>

              <search-table
                v-model="profileId"
                @UpdateResource="closeDialog"
                :selectedServiceId="selectedObjectId"
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
        <template v-slot:[`item.action`]="{ item }">
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                >mdi-delete</v-icon
              >
            </template>
            <span>{{ $t("datasource.pms.annual-plan.objectives.delete-resource") }}</span>
          </v-tooltip>
        </template>
      </v-data-table>
    </div>
    <div>
      <!-- <v-row v-if="selectedResource && selectedResource > 0">
        <v-col :cols="12">
          <div>
            <div v-if="showItemNumber==1">
              <ap-narrative :selectedObjectId="selectedResource" :tableName="tblName"></ap-narrative>
            </div>
            <div v-else-if="showItemNumber==2">
              <ap-resource :selectedObjectId="selectedResource"></ap-resource>
            </div>
          </div>
        </v-col>
      </v-row>-->
      <div v-if="showResDetails">
        <!-- <ap-ind-details
          :editResMainDataItem="formData"
          :key="showResDetails"
          :editResId="editId"
          :editResDesc="editResDesc"
          :selectedObjectId="selectedObjectId"
        ></ap-ind-details> -->
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanResourceSearchForm.vue";
// import ResourceDetails from "./AnnualPlanResourceDetailsForm.vue";
// import ResourceMainData from "./AnnualPlanResourceMainDataForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

// interface ResourceData {
//   ID: number | null;
//   COI: {} | null;
//   TOS: {} | null;
//   DATE_FROM: Date | null;
//   DATE_TO: Date | null;
//   OCCURRENCE: {} | null;
//   OBJECTIVE_ID: number | null;
// }
interface payLoadData {
  ID_ANNUAL_PLAN: number | null;
  ID_SERVICE: number | null;
  ID_USER: number | null;
  ID_POSITION: number | null;
}

export default Vue.extend({
  components: {
    "search-table": SearchTable,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    showResourceDetails: {
      type: Boolean,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      profileId: {},
      showResTabs: true,
      editResDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showResDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedRes: [],
      tableFilter: "",
      tblName: null,
      selectedResource: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,

      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      resourceList: [],
      resourceListOrig: [],
      tmpSelectedItem: [],
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
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      if (item != null) localThis.UpdateResource(item);
    },

    UpdateResource(item: String) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as payLoadData;
      ap = localThis.$store.getters.getAnnualPlan;
      payLoadD.ID_SERVICE = localThis.selectedObjectId;
      payLoadD.ID_USER = Number.parseInt(item.split("-")[0]);
      payLoadD.ID_POSITION = Number.parseInt(item.split("-")[1]);
      payLoadD.ID_ANNUAL_PLAN = ap.annal_plan_id;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_ANNUAL_PLAN_RESOURCE",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.itemsPerPage = 15;
          localThis.tmpSelectedItem = [];
          localThis.getServiceResources();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    reloadResources(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getServiceResources().then((res: any) => {
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
          localThis.tmpSelectedItem = [];
          if (item) {
            let i = 0;
            for (i = 0; i < localThis.resourceList.length; i++) {
              if (localThis.resourceList[i].ID == item.ID) {
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.resourceList[i]))
                );
                localThis.resourceList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
        }
      });
    },
    getServiceResources() {
      let localThis: any = this as any;
      localThis.selectedResource = null;
      localThis.resourceList = [];
      localThis.resourceListOrig = [];

      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_SERVICE_RESOURCE";
      let wherecond: string = `SERVICE_ID = ${localThis.selectedObjectId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_SERVICE_RESOURCE_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.resourceList.push(item);
              localThis.resourceListOrig.push(item);
            });
            if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1) {
              let i = 0;
              for (i = 0; i < localThis.resourceList.length; i++) {
                if (localThis.resourceList[i].ID == localThis.tmpSelectedItem.ID) {
                  localThis.tmpSelectedItem = [];
                  localThis.tmpSelectedItem.push(
                    JSON.parse(JSON.stringify(localThis.resourceList[i]))
                  );
                  localThis.resourceList = localThis.tmpSelectedItem;
                  break;
                }
              }
            }
          }
        }
      );
    },
    getNow: function () {
      let localThis: any = this as any;
      const today = new Date();
      const date =
        today.getFullYear() + "-" + (today.getMonth() + 1) + "-" + today.getDate();

      localThis.formData.DATE_FROM = date;
      localThis.formData.DATE_TO = date;
    },
    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },
    editResources(item: any) {
      let localThis: any = this as any;
      localThis.selectedRes = [];
      localThis.selectedResource = item.ID;
      localThis.selectedRes.push(item);
      localThis.showItemNumber = 2;
      localThis.contractTable(item);
    },
    switchItems() {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.resourceList = localThis.resourceListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.resourceList = localThis.tmpSelectedItem;
        //localThis.resourceList=
      }
    },
    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedRes = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;

      localThis.selectedRes.push(item);
      localThis.resourceList = [];
      localThis.contractTable(item);
      EventBus.$emit("hideSrvTabs");
      localStorage.SelectedRes = "desc";
      EventBus.$emit("refreshBreadCumb", 3);
    },

    contractTable(item: any) {
      let localThis: any = this as any;
      localThis.tmpSelectedItem = [];
      localThis.tmpSelectedItem.push(item);
      localThis.resourceList = localThis.tmpSelectedItem;

      localThis.itemsPerPage = 1;
      localThis.sIcon = "mdi-chevron-double-down";
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.hrm.position.department").toString(),
          value: "HR_DEPARTMENT_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.title-full").toString(),
          value: "HR_POSITION_FULL_TITLE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.descr").toString(),
          value: "HR_POSITION_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.last-name").toString(),
          value: "HR_BIODATA_SURNAME",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.name").toString(),
          value: "HR_BIODATA_NAME",
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
    localThis.selectedResource = null;
    localThis.tblName = "IND_INDICATOR";
    localThis.setupHeaders();
    localThis.getNow();
    localThis.editResDesc = localThis.editedItem.PMS_COI_CODE;
    if (localThis.selectedObjectId > 0) {
      localThis.getServiceResources();
    }

    EventBus.$on("closeResourceDetails", (data: any) => {
      localThis.showResDetails = false;
      localThis.reloadResources();
      EventBus.$emit("showSrvTabs");
    });

    EventBus.$on("reloadResources", (data: any) => {
      localThis.reloadResources(data);
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
      localThis.getServiceResources();
      localThis.setupHeaders();
    },
    showResourceDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showResDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getServiceResources();
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
  background-color: red;
  font-weight: bold;
}
</style>
