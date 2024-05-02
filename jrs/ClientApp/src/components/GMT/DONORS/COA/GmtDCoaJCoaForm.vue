<template>
  <div>
    <div>
      <!-- JCOA SELECTION-->
      <v-data-table
        :headers="headers"
        :items="dcoajcoaList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="5"
        class="text-capitalize"
        v-model="selectedCTS"
      >
        <template v-slot:top>
          <div
            class="d-inline-flex align-center primary lighten-2 jrs-table-top"
          >
            <h3>{{ $t("datasource.gmt.donor.jrs-coa") }}</h3>

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
              <gmt-addjcoa-form
                @UpdateRelations="UpdateRelations"
                @closeEdit="closeDialog"
                :key="rndVar"
                :selectedDonorId="selectedObjectId"
                :alreadyInserted="dcoajcoaList"
              ></gmt-addjcoa-form>
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
            <tr
              v-for="item in items"
              :key="item.PMS_JRS_COA_ID"
              style="height: 10px"
            >
              <td class="tablerow">{{ item.PMS_JRS_COA_CODE }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_DESCRIPTION }}</td>
              <td class="tablerow">{{ item.PMS_JRS_COA_TYPE }}</td>
              <td class="tablerow">
                <v-icon
                  :color="
                    item.PMS_JRS_COA_ENABLED == true
                      ? 'green darken-3'
                      : 'deep-orange darken-3'
                  "
                  >{{
                    item.PMS_JRS_COA_ENABLED == true
                      ? "mdi-checkbox-marked-circle-outline"
                      : "mdi-checkbox-blank-circle-outline"
                  }}</v-icon
                >
              </td>
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon
                      small
                      class="mr-2"
                      @click="deleteItem(item)"
                      v-on="on"
                      >mdi-delete</v-icon
                    >
                  </template>
                  <span>{{ $t("datasource.gmt.donor.delete-coa") }}</span>
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
import JCOADCOASearch from "./GmtJCoaSearchForm.vue";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";

interface PayloadD {
  GMT_DONOR_COA_ID: number | null;
  GMT_JRS_COA_ID: number | null;
}
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    // "jrs-table": JrsTable,

    "gmt-addjcoa-form": JCOADCOASearch,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: false,
    },
    selectedCOAId: {
      type: Number,
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
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},

      dcoajcoaList: [],
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
      localThis.editedItemDialog.dcoajcoaList = localThis.dcoajcoaList;
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    UpdateRelations(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item == null) return;
      let itm: any = {};
      itm.PMS_JRS_COA_ID = item.ACC_ID;
      itm.PMS_JRS_COA_CODE = item.ACC_CODE;
      itm.PMS_JRS_COA_DESCRIPTION = item.ACC_DESCRIPTION;
      itm.PMS_JRS_COA_TYPE = item.ACC_TYPE;
      itm.PMS_JRS_COA_ENABLED = item.ACC_STATUS;
      let found: boolean = false;
      let i: number = 0;
      for (i = 0; i < localThis.dcoajcoaList.length; i++) {
        if (localThis.dcoajcoaList[i].PMS_JRS_COA_ID == item.ACC_ID) {
          Vue.set(localThis.dcoajcoaList, i, itm);
          found = true;
          break;
        }
      }
      if (!found) {
        localThis.dcoajcoaList.push(itm);
      }
      this.$emit("UpdateRelations",localThis.dcoajcoaList);
      //   let payLoadD = {} as PayloadD;
      //   payLoadD.GMT_DONOR_COA_ID = localThis.formData.selectedCOAId;
      //   payLoadD.GMT_JRS_COA_ID = Number.parseInt(item.ACC_ID);

      //   let savePayload: GenericSqlPayload = {
      //     spName: "GMT_SP_INS_DCOA_JCOA_REL",
      //     actionType: SqlActionType.NUMBER_0,
      //     jsonPayload: JSON.stringify(payLoadD),
      //   };
      //   localThis
      //     .execSPCall(savePayload)
      //     .then((res: any) => {
      //       localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
      //       this.$emit("UpdateItem");
      //     })
      //     .catch((err: any) => {
      //       localThis.showNewSnackbar({
      //         typeName: "error",
      //         text: err.message,
      //       }); // Feedback to user
      //     });
    },

    deleteItem(item: any) {
      let localThis: any = this as any;
      let msg: string = this.$i18n
        .t("datasource.gmt.donor.del-jrs-coa")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.dcoajcoaList.splice(
            localThis.dcoajcoaList.indexOf(item),
            1
          );
        }
      });
    },

    editItem(item: any) {
      let localThis: any = this as any;
      //console.log("Clicked", item.name);
      localThis.editMode = true;
      localThis.editedItemDialog.dcoajcoaList = localThis.dcoajcoaList;
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },

    setupHeaders() {
      let localThis = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("pms.donor-coa-code").toString(),
          value: "PMS_JRS_COA_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.donor-coa-descr").toString(),
          value: "PMS_JRS_COA_DESCRIPTION",
          align: "center",
          divider: true,
        },

        {
          text: this.$i18n.t("pms.donor-coa-type").toString(),
          value: "PMS_JRS_COA_TYPE",
          align: "center",
          divider: true,
        },

        {
          text: this.$i18n.t("pms.donor-coa-status").toString(),
          value: "PMS_JRS_COA_STATUS",
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

    localThis.setupHeaders();
    localThis.dcoajcoaList = localThis.relations;
    localThis.editedItemDialog = {};
    localThis.editedItemDialog.dcoajcoaList = localThis.dcoajcoaList;
    if (localThis.dcoajcoaList == undefined) localThis.dcoajcoaList = [];
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
}
</style>