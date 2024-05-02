<template>
  <div>
    <div>
      <!-- SRV SELECTION-->
      <v-data-table
        :headers="headers"
        :items="toscoiList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="5"
        class="text-capitalize"
        v-model="selectedCTS"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("pms.coi") }}</h3>

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
              <template v-slot:activator="{ on }">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="openDialog"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>
              <pms-addcts-form
                @refreshTOSCOIRelList="refreshTOSCOIRelList"
                @closeEdit="closeDialog"
                :key="rndVar"
                :formDataExt="editedItemDialog"
                :selectedObjectId="selectedObjectId"
              ></pms-addcts-form>
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
            <tr v-for="item in items" :key="item.PMS_COI_TOS_ID" style="height: 10px">
              <td class="tablerow">{{ item.PMS_COI_CODE }}</td>
              <td class="tablerow">{{ item.IMS_LABELS_VALUE }}</td>
              <td class="tablerow">{{ item.START_DATE }}</td>
              <td class="tablerow">{{ item.END_DATE }}</td>
              <td class="tablerow">
                <v-icon
                  :color="
                    item.PMS_COI_ENABLED == true
                      ? 'green darken-3'
                      : 'deep-orange darken-3'
                  "
                  >{{
                    item.PMS_COI_ENABLED == true
                      ? "mdi-checkbox-marked-circle-outline"
                      : "mdi-checkbox-blank-circle-outline"
                  }}</v-icon
                >
              </td>
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>
                  <span>{{ $t("datasource.pms.annual-plan.objectives.edit-coi") }}</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                      >mdi-delete</v-icon
                    >
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.delete-activity")
                  }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
    <div></div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { JrsHeader } from "../../../../models/JrsTable";
import COITOSMainData from "./PMSTOSCOIMainDataForm.vue";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    // "jrs-table": JrsTable,

    "pms-addcts-form": COITOSMainData,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: false,
    },
    showCOITOSDetails: {
      type: Boolean,
      required: false,
    },
    relations: {
      type: Array,
    },
  },
  data() {
    return {
      rndVar: 0,
      editCTSDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedCTS: [],
      tableFilter: "",
      tblName: null,
      selectedCOITOS: null,
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
      toscoiList: [],
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

    openDialog() {
      let localThis: any = this as any;
      localThis.editMode = !localThis.editMode;
      localThis.editedItemDialog.PMS_COI_ID = localThis.selectedObjectId;
      localThis.editedItemDialog.toscoiList = localThis.toscoiList;
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    reloadCOITOSS(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      // localThis.getCOITOSS();
    },
    refreshTOSCOIRelList(item: any) {
      let localThis: any = this as any;
      let i: number = 0;
      let found: boolean = false;
      item.START_DATE = this.$i18n.d(new Date(item.START_DATE), "short");
      item.END_DATE = this.$i18n.d(new Date(item.END_DATE), "short");
      for (i = 0; i < localThis.toscoiList.length; i++) {
        if (localThis.toscoiList[i].ID == item.ID) {
          Vue.set(localThis.toscoiList, i, item);
          found = true;
          break;
        }
      }
      if (!found) {
        //localThis.toscoiList.push(item);
        localThis.toscoiList.push(item);
      }
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
      // localThis.showNewSnackbar({
      //   typeName: "error",
      //   text: "To be Implemented"
      // });
      let i: number = 0;
      for (i = 0; i < localThis.toscoiList.length; i++) {
        if (localThis.toscoiList[i].ID == item.ID) {
          Vue.delete(localThis.toscoiList, i);
          break;
        }
      }
    },

    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.editMode = true;
      localThis.editedItemDialog = {};
      localThis.editedItemDialog.START_DATE = item.START_DATE;
      localThis.editedItemDialog.END_DATE = item.END_DATE;
      localThis.editedItemDialog.PMS_COI_ID = item.PMS_COI_ID;
      localThis.editedItemDialog.ID = item.ID;
      localThis.editedItemDialog.IMS_LABEL = item.IMS_LABEL;
      localThis.editedItemDialog.toscoiList = localThis.toscoiList;
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },

    setupHeaders() {
      let localThis = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("pms.coi-code").toString(),
          align: "center",
          value: "PMS_COI_CODE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("pms.coi-description").toString(),

          align: "center",
          value: "IMS_LABELS_VALUE",
          sortable: true,
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
          text: this.$i18n.t("pms.coi-enabled").toString(),

          align: "center",
          value: "PMS_COI_ENABLED",
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
  mounted() {
    let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.toscoiList = localThis.relations;
    localThis.editedItemDialog = {};
    localThis.editedItemDialog.PMS_COI_ID = localThis.selectedObjectId;
    localThis.editedItemDialog.toscoiList = localThis.toscoiList;
    if (localThis.toscoiList == undefined) localThis.toscoiList = [];
  },

  watch: {},
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
  font-size: 12px;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
</style>
