<template>
  <div>
    <div>
      <!-- SRV SELECTION-->
      <v-data-table
        :headers="headers"
        :items="coitosList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="5"
        class="text-capitalize"
        v-model="selectedCTS"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{$t('pms.toss')}}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-if="false"
              v-model="editMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              :max-width="((50 * nbrOfColumns) + 25) / 3 + 'em'"
              transition="dialog-transition"
            >
              <template v-slot:activator="{on}">
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
                @refreshCOITOSRelList="refreshCOITOSRelList"
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
        <template v-slot:[`item.action`]="{ item }"  v-if="false">
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-icon small class="mr-2" @click="editItem(item)" v-on="on">mdi-circle-edit-outline</v-icon>
            </template>
            <span>{{ $t('datasource.pms.annual-plan.objectives.edit-coitos') }}</span>
          </v-tooltip>
          <v-tooltip bottom  v-if="false">
            <template v-slot:activator="{ on }">
              <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on">mdi-delete</v-icon>
            </template>
            <span>{{ $t('datasource.pms.annual-plan.objectives.delete-coitos') }}</span>
          </v-tooltip>
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
import COITOSMainData from "./PMSCOITOSMainDataForm.vue";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    // "jrs-table": JrsTable,

    "pms-addcts-form": COITOSMainData
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: false
    },
    showCOITOSDetails: {
      type: Boolean,
      required: false
    },
    relations: {
      type: Array
    }
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
      coitosList: [],
      occ: []
    };
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),

    openDialog() {
      let localThis: any = this as any;
      localThis.editMode = !localThis.editMode;
      localThis.editedItemDialog.PMS_COI_ID = localThis.selectedObjectId;
      localThis.editedItemDialog.coitosList = localThis.coitosList;
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
    refreshCOITOSRelList(item: any) {
      //debugger;
      let localThis: any = this as any;
      let i: number = 0;
      let found: boolean = false;
      item.START_DATE = this.$i18n.d(new Date(item.START_DATE), "short");
      item.END_DATE = this.$i18n.d(new Date(item.END_DATE), "short");
      for (i = 0; i < localThis.coitosList.length; i++) {
        if (localThis.coitosList[i].ID == item.ID) {
          Vue.set(localThis.coitosList, i, item);
          found = true;
          break;
        }
      }
      if (!found) {
        localThis.coitosList.push(item);
      }
    },
    getNow: function() {
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
        text: "To be Implemented"
      });
    },

    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.editMode = true;
      localThis.editedItemDialog = {};
      localThis.editedItemDialog.PMS_COI_ID = localThis.selectedObjectId;
      localThis.editedItemDialog.START_DATE = item.START_DATE;
      localThis.editedItemDialog.END_DATE = item.END_DATE;
      localThis.editedItemDialog.PMS_TOS_ID = item.PMS_TOS_ID;
      localThis.editedItemDialog.ID = item.ID;
      localThis.editedItemDialog.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION;
      localThis.editedItemDialog.coitosList = localThis.coitosList;
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },

    setupHeaders() {
      let localThis = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.type-of-service.type-of-service")
            .toString(),
          value: "PMS_TOS_DESCRIPTION",
          align: "center",
          divider: true
        },
        {
          text: this.$i18n.t("general.date-start").toString(),
          value: "START_DATE",
          align: "center",
          divider: true
        },
        {
          text: this.$i18n.t("general.date-end").toString(),
          value: "END_DATE",
          align: "center",
          divider: true
        },
        // {
        //   text: "Actions",
        //   value: "action",
        //   align: "center",
        //   sortable: false
        // }
      ];

      localThis.headers = tmp;
    }
  },
  mounted() {
    let localThis: any = this as any;

    localThis.setupHeaders();
    localThis.coitosList = localThis.relations;
    localThis.editedItemDialog = {};
    localThis.editedItemDialog.PMS_COI_ID = localThis.selectedObjectId;
    localThis.editedItemDialog.coitosList = localThis.coitosList;
    if (localThis.coitosList == undefined) localThis.coitosList = [];
  },

  watch: {}
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
}
</style>