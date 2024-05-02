<template>
  <div>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <div v-if="!showSrvDetails">
      <!-- SRV SELECTION-->
      <v-data-table
        :headers="headers"
        :items="serviceList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
        v-model="selectedSrv"
      >
        <template v-slot:top>
          <div
            class="d-inline-flex align-center primary lighten-2 jrs-table-top"
          >
            <h3>{{ $t("datasource.pms.annual-plan.objectives.services") }}</h3>

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
              :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
              transition="dialog-transition"
            >
              <template v-slot:activator="{ on }" v-if="!onlyRead">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="
                    editMode = true;
                    rndVar = Math.ceil(Math.random() * 1000);
                  "
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>
              <pms-addsrv-form
                @closeDialoge="closeDialog"
                @closeEdit="closeDialog"
                :key="rndVar"
                @refreshObjSrv="getObjectiveServices"
                :formDataExt="editedItemDialog"
                :selectedObjectId="selectedObjectId"
              ></pms-addsrv-form>
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
              <v-icon v-if="!onlyRead" small class="mr-2" @click="editItem(item)" v-on="on"
                >mdi-circle-edit-outline</v-icon
              >
              <v-icon v-if="onlyRead"  small class="mr-2" @click="editItem(item)" v-on="on"
                >mdi-eye</v-icon
              >
            </template>
            <span>{{
              $t("datasource.pms.annual-plan.objectives.edit-service")
            }}</span>
          </v-tooltip>
          <v-tooltip bottom>
            <template v-slot:activator="{ on }" v-if="!onlyRead">
              <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                >mdi-delete</v-icon
              >
            </template>
            <span>{{
              $t("datasource.pms.annual-plan.objectives.delete-service")
            }}</span>
          </v-tooltip>
        </template>
      </v-data-table>
    </div>
    <div>
      <div v-if="showSrvDetails">
        <gt-srv-details
          :editSrvMainDataItem="formData"
          :key="Math.ceil(Math.random() * 1000)"
          :editSrvId="editId"
          :editSrvDesc="editSrvDesc"
          :showSrvDetails="showSrvDetails"
          :selectedObjectId="selectedObjectId"
          :isUpdatableForm="isUpdatableForm"
          :onlyRead="onlyRead"
        ></gt-srv-details>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { JrsHeader } from "../../../../../models/JrsTable";

import ServiceDetails from "./GMTGrantServiceDetailsForm.vue";
import ServiceMainData from "./GMTGrantServiceMainDataForm.vue";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

export default Vue.extend({
  components: {
    // "jrs-table": JrsTable,

    "gt-srv-details": ServiceDetails,
    "pms-addsrv-form": ServiceMainData,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    showServiceDetails: {
      type: Boolean,
      required: true,
    },
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
     onlyRead:{
      type:Boolean,
      required:false,
      default:false

    }
  },
  data() {
    return {
      showWait: false,
      donor_id: 0,
      rndVar: 0,
      showSrvTabs: true,
      editSrvDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showSrvDetails: false,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedSrv: [],
      tableFilter: "",
      selectedService: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},
      coi: [],
      tos: [],
      serviceList: [],
      serviceListOrig: [],
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

    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      //localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    reloadServices(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getObjectiveServices().then((res: any) => {
        if (
          localThis.tmpSelectedItem &&
          localThis.tmpSelectedItem.length == 1
        ) {
          localThis.tmpSelectedItem = [];

          if (item) {
            let i = 0;
            for (i = 0; i < localThis.serviceList.length; i++) {
              if (localThis.serviceList[i].ID == item.ID) {
                localThis.tmpSelectedItem.push(
                  JSON.parse(JSON.stringify(localThis.serviceList[i]))
                );
                localThis.serviceList = localThis.tmpSelectedItem;
                break;
              }
            }
          }
        }
      });
    },

    getNow: function () {
      let localThis: any = this as any;
      const today = new Date();
      const date =
        today.getFullYear() +
        "-" +
        (today.getMonth() + 1) +
        "-" +
        today.getDate();

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

    switchItems() {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.serviceList = localThis.serviceListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.serviceList = localThis.tmpSelectedItem;
        //localThis.serviceList=
      }
    },
    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.selectedSrv = [];
      //localThis.editMode = true;
      localThis.editId = item.ID;
      localThis.editSrvDesc =
        item.GMT_COI_CODE + "-" + item.GMT_TOS_DESCRIPTION + " ";
      localThis.showSrvDetails = true;
      localThis.formData.DATE_FROM = item.START_DATE;
      localThis.formData.DATE_TO = item.END_DATE;
      localThis.formData.ID = item.ID;
      localThis.formData.OBJECTIVE_ID = item.OBJECTIVE_ID;
      localThis.formData.COI = {};
      localThis.formData.COI.GMT_COI_ID = item.GMT_COI_ID;
      localThis.formData.COI.IMS_LABELS_VALUE = item.GMT_COI_DESCRIPTION;
      localThis.formData.COI.GMT_COI_CODE = item.GMT_COI_CODE;
      localThis.formData.TOS = {};
      localThis.formData.TOS.GMT_TOS_ID = item.GMT_TOS_ID;
      localThis.formData.TOS.GMT_TOS_DESCRIPTION = item.GMT_TOS_DESCRIPTION;
      localThis.formData.TOS.GMT_COI_TOS_ID = item.GMT_COI_TOS_ID;
      localThis.formData.OCCURRENCE = {};
      localThis.formData.OCCURRENCE.ID = item.OC_ID;
      localThis.formData.OCCURRENCE.TYPE = item.OC_TYPE;
      localThis.formData.LOCATION_ID = item.LOCATION_ID;
      localThis.formData.LOCATION_DETAILS = item.LOCATION_DETAILS;
      localThis.formData.TITLE = item.TITLE;
      localThis.formData.TOS.PMS_COI_TOS_ID = item.PMS_COI_TOS_ID;
      localThis.selectedSrv.push(item);
      localThis.serviceList = [];
      localThis.contractTable(item);
      EventBus.$emit("hideObjecTabs");
      localStorage.SelectedSrv = localThis.editSrvDesc;
      EventBus.$emit("refreshBreadCumb", 2);
    },

    contractTable(item: any) {
      let localThis: any = this as any;
      localThis.tmpSelectedItem = [];
      localThis.tmpSelectedItem.push(item);
      localThis.serviceList = localThis.tmpSelectedItem;

      localThis.itemsPerPage = 1;
      localThis.sIcon = "mdi-chevron-double-down";
    },

    getObjectiveServices() {
      let localThis: any = this as any;
      localThis.selectedService = null;
      localThis.serviceList = [];
      localThis.serviceListOrig = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_SERVICE",
        colList: null,
        whereCond: `OBJECTIVE_ID = ${localThis.selectedObjectId} AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.serviceList.push(item);
              localThis.serviceListOrig.push(item);
            });
            if (
              localThis.tmpSelectedItem &&
              localThis.tmpSelectedItem.length == 1
            ) {
              let i = 0;
              for (i = 0; i < localThis.serviceList.length; i++) {
                if (
                  localThis.serviceList[i].ID == localThis.tmpSelectedItem.ID
                ) {
                  localThis.tmpSelectedItem = [];
                  localThis.tmpSelectedItem.push(
                    JSON.parse(JSON.stringify(localThis.serviceList[i]))
                  );
                  localThis.serviceList = localThis.tmpSelectedItem;
                  break;
                }
              }
            }
          }
          localThis.showWait = false;
        }
      );
    },

    setupHeaders() {
      let localThis = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("general.code").toString(),
          value: "GMT_COI_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.title").toString(),
          value: "TITLE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t(
              "datasource.pms.category-of-intervention.category-of-intervention"
            )
            .toString(),
          value: "COI_LABEL",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.type-of-service.type-of-service")
            .toString(),
          value: "GMT_TOS_DESCRIPTION",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.date-start").toString(),
          value: "START_DATE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.date-end").toString(),
          value: "END_DATE",
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
    let gt = {} as any;
    gt = localThis.$store.getters.getGrant;
    localThis.donor_id = gt.DONOR_ID;
    localThis.selectedService = null;
    localThis.setupHeaders();
    localThis.getNow();
    localThis.editSrvDesc = localThis.editedItem.GMT_COI_CODE;
    if (localThis.selectedObjectId > 0) {
      localThis.getObjectiveServices();
    }
    EventBus.$on("closeServiceDetails", (data: any) => {
      localThis.showSrvDetails = false;
      localThis.reloadServices();
      EventBus.$emit("showObjecTabs");
    });

    EventBus.$on("reloadServices", (data: any) => {
      localThis.reloadServices(data);
    });
    EventBus.$on("locationSaved", (id: Number, desc: String) => {
      let localThis: any = this as any;
      localThis.formData.LOCATION_ID = id;
      localThis.formData.LOCATION_DETAILS = desc;
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
      localThis.getObjectiveServices();
      localThis.setupHeaders();
    },
    showServiceDetails(to: boolean) {
      let localThis: any = this as any;
      localThis.showSrvDetails = to;
    },
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.selectedObjectId = to;
      localThis.getObjectiveServices();
    },
    editMode(to: boolean, from: boolean) {},
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
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