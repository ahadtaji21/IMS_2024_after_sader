<template>
  <div>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <div>
      <!-- SRV SELECTION-->

      <v-data-table
        :headers="columns"
        :items="grantList"
        item-key="ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{ $t("datasource.gmt.donor.grant") }}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="editMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              :max-width="(80 * nbrOfColumns + 25) / 3 + 'em'"
              transition="dialog-transition"
              v-if="module == 'GMT' && authorizedRole == true"
            >
              <template v-slot:activator="{ on }">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="openDlg()"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>
              <v-card>
                <v-card-text><h2>New Grant</h2></v-card-text>

                <gmt-grant-form
                  @closeDialoge="closeDialog"
                  @closeGrantDialogeAndSave="closeDialogeAndUpdateList"
                  :formData="editedItemDialog"
                  :donorPresetted="true"
                  :key="dlgKey"
                ></gmt-grant-form>

                <!-- <v-card-actions>
                  <v-btn color="secondary darken-1" text @click="closeDetails"
                    >X {{ $t("general.close") }}</v-btn
                  >
                </v-card-actions> -->
              </v-card>
            </v-dialog>

            <v-dialog
              v-model="detailsMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              transition="dialog-transition"
            >
              <v-card>
                <v-spacer></v-spacer>
                <v-btn icon @click="closeDetails">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
                <v-card-text>
                  <gmt-grant-details
                    :editItemMainData="editItemMainData"
                    :donorPresetted="true"
                    :originCall="'DONOR'"
                    @returnBackToDonorList="closeDetailsAndReload()"
                    :isAuthorized="authorizedRole"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></gmt-grant-details>
                </v-card-text>
                <!-- <v-card-actions>
                  <v-btn color="secondary darken-1" text @click="closeDetails"
                    >X {{ $t("general.close") }}</v-btn
                  >
                </v-card-actions> -->
              </v-card>
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
              <td class="tablerow">
                {{ item.CODE }}
              </td>
              <!-- <td class="tablerow">
                {{ item.DONOR_NAME }}
              </td> -->
              <td class="tablerow">
                {{ item.OFFICE }}
              </td>
              <td class="tablerow">
                {{ item.DESCR }}
              </td>
              <td class="tablerow">
                {{ item.START_DATE | formatDate }}
              </td>
              <td class="tablerow">
                {{ item.END_DATE | formatDate }}
              </td>
              <!-- <td class="tablerow">
                {{ item.DURATION }}
              </td> -->
              <td class="tablerow">
                {{ item.AMOUNT + " " + item.CURRENCY_CODE }}
              </td>
              <td class="tablerow">
                {{ item.IMS_STATUS_NAME }}
              </td>
              <!-- <td v-if="item.STATUS == 1" class="tablerow">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon color="green" small class="mr-2" v-on="on">mdi-circle</v-icon>
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.design-in-progress")
                  }}</span>
                </v-tooltip>
              </td>
              <td v-if="item.STATUS == 0" class="tablerow">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon color="red" small class="mr-2" v-on="on">mdi-circle</v-icon>
                  </template>
                  <span>{{
                    $t("datasource.pms.annual-plan.objectives.design-in-progress")
                  }}</span>
                </v-tooltip>
              </td> -->
              <td style="text-align: center">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                      >mdi-circle-edit-outline</v-icon
                    >
                  </template>
                  <span>{{ $t("datasource.gmt.grant.gt-edit-grant") }}</span>
                </v-tooltip>
                <v-tooltip bottom v-if="authorizedRole">
                  <template v-slot:activator="{ on }">
                    <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                      >mdi-delete</v-icon
                    >
                  </template>
                  <span>{{ $t("datasource.gmt.grant.gt-delete-grant") }}</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </template>
      </v-data-table>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsTable from "../../../../components/JrsTable.vue";
import { JrsHeader } from "../../../../models/JrsTable";
import { IImsList } from "./../../../PMS/PMSSharedInterfaces";
import GRANTMainData from "./../../GRANTS/PROPOSAL/GMTGrantMainDataForm.vue";
import GRANTDetails from "./../../GRANTS/PROPOSAL/GMTGrantDetailsForm.vue";
import { translate, i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "gmt-grant-form": GRANTMainData,
    "gmt-grant-details": GRANTDetails,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      authorizedRole: false,
      module: "",
      showWait: false,
      showIndTabs: true,
      dlgKey: 0,
      editItemMainData: {},
      detailsMode: false,
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.gmt.grant.gt-code",
          align: "center",
          value: "CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "JRS Country Office",
          textKey: "datasource.gmt.grant.gt-grant-office",
          align: "center",
          value: "OFFICE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-title",
          align: "center",
          value: "DESCR",
          sortable: true,
          filterable: true,
        },
        {
          text: "Start Date",
          textKey: "datasource.gmt.grant.gt-start-date",
          align: "center",
          value: "START_DATE",
          sortable: true,
          filterable: true,
        },
        {
          text: "End Date",
          textKey: "datasource.gmt.grant.gt-end-date",
          align: "center",
          value: "END_DATE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Amount",
          textKey: "datasource.gmt.grant.gt-amount",
          align: "center",
          value: "AMOUNT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Status",
          textKey: "datasource.gmt.grant.gt-status-name",
          align: "center",
          value: "IMS_STATUS_NAME",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      itemsPerPage: 15,
      showItemNumber: 0,
      tableFilter: "",

      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},
      grantList: [],
      tmpSelectedItem: [],
      occ: [],
    };
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),

    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    closeDialog() {
      let localThis = this as any;
      let msgLeave: string = this.$i18n
        .t("datasource.gmt.grant.gt-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.editMode = false;

          localThis.editedItemDialog = {};
          localThis.getDonorGRANT();
        }
      });
    },
    closeDialogeAndUpdateList(value: any) {
      let localThis = this as any;
      localThis.editMode = false;
      localThis.editedItemDialog = {};
      localThis.getDonorGRANT();
    },

    openDlg() {
      let localThis: any = this as any;
      localThis.editMode = !localThis.editMode;
      localThis.editedItemDialog = {};
      localThis.dlgKey = Math.ceil(Math.random() * 1000);
      localThis.editedItemDialog.DONOR_ID = localThis.selectedObjectId + "";
      localThis.editedItemDialog.DEPARTMENT_ID = "0";
    },
    getDonorGRANT() {
      let localThis: any = this as any;
      localThis.grantList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT",
        colList: null,
        whereCond: `DONOR_ID = ${localThis.selectedObjectId} AND (${localThis.getCurrentOffice.HR_OFFICE_ID}=1 OR OFFICE_ID=${localThis.getCurrentOffice.HR_OFFICE_ID})`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "CODE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              if (item.DEPARTMENT_ID == undefined) item.DEPARTMENT_ID = "0";
              localThis.grantList.push(item);
            });
          }
          localThis.showWait = false;
        });
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n.t("datasource.gmt.grant.gt-delete-confirm").toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.GMT_ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_GRANT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getDonorGRANT();
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

    closeDetails() {
      let localThis: any = this as any;
      localThis.detailsMode = false;
      localThis.editMode = false;
    },
    closeDetailsAndReload() {
      let localThis: any = this as any;
      localThis.detailsMode = false;
      localThis.editMode = false;
      localThis.getDonorGRANT();
    },
    editItem(item: any) {
      let localThis: any = this as any;
      localThis.detailsMode = true;
      //console.log("Clicked", item.name);

      localThis.editItemMainData = JSON.parse(JSON.stringify(item));
      let gt = {} as any;
      gt.grant_id = localThis.editItemMainData.ID;
      gt.year = localThis.editItemMainData.START_YEAR;
      gt.currency = localThis.editItemMainData.CURRENCY_CODE;
      gt.donor_id = localThis.editItemMainData.DONOR_ID;
      gt.department_id = localThis.editItemMainData.DEPARTMENT_ID;
      localThis.setGrant(gt);
      localThis.editItemMainData.DEPARTMENT_ID =
        localThis.editItemMainData.DEPARTMENT_ID + "";
    },
    isGMTModule() {
      let localThis = this as any;
      if (localThis.module == "GMT") return true;
      else return false;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
    localThis.getDonorGRANT();
  },
  mounted() {
    let localThis = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    var role: any = localThis.getCurrentRole.ROLE_CODE;
    if (role == "GM") {
      //Grant Manager
      localThis.authorizedRole = true;
    }
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      if (to.ROLE_CODE == "GM") {
        //Grant Manager
        localThis.authorizedRole = true;
      }
    });
  },
  computed: {
    language() {
      return i18n.locale;
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
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  filters: {
    formatDate: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      return string.substring(0, 10);
    },
  },
  watch: {
    presetVal: function (val) {
      let localThis: any = this as any;
      localThis.search = val;
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
  background-color: red;
  font-weight: bold;
}
</style>
