<template>
  <v-main>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
          <div>
            <v-data-table
              :headers="columns"
              :items="grantList"
              item-key="name"
              multi-sort
              :search="tableFilter"
              class="text-capitalize elevation-2"
              :items-per-page="15"
            >
              <template v-slot:header.IMS_STATUS_NAME="{ header }">
                {{ header.text }}
                <v-icon
                  color="primary lighten-2"
                  class="grey--text text--darken-3"
                  @click="openInfo()"
                  >mdi-information-outline</v-icon
                >
              </template>
              <!-- <template v-slot:header="{ header }">
                <tr class="grey lighten-3">
                  <th v-for="header in columns" :key="header.text">
                    <div v-if="filters.hasOwnProperty(header.value)">
                      <v-autocomplete
                        flat
                        hide-details
                        multiple
                        attach
                        chips
                        dense
                        clearable
                        :items="columnValueList(header.value)"
                        v-model="filters[header.value]"
                      >
                        <template v-slot:selection="{ item, index }">
                          <v-chip v-if="index < 5">
                            <span>
                              {{ item }}
                            </span>
                          </v-chip>
                          <span v-if="index === 5" class="grey--text caption">
                            (+{{ filters[header.value].length - 5 }} others)
                          </span>
                        </template>
                      </v-autocomplete>
                    </div>
                  </th>
                </tr>
              </template> -->

              <template v-slot:top>
                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                  <h3>{{ $tc("datasource.gmt.grant.gts", 2) }}</h3>
                  <v-spacer></v-spacer>
                  <v-dialog
                    v-model="dialog"
                    persistent
                    max-width="1100px"
                    v-if="module == 'GMT' && authorizedRole == true"
                  >
                    <template v-slot:activator="{ on }">
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                        v-on="on"
                        @click="editedItem = {}"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>New
                      </v-btn>
                    </template>
                    <v-spacer></v-spacer>
                    <gmt-gt-form
                      @closeDialoge="closeDialog"
                      @closeGrantDialogeAndSave="closeGrantDialogeAndSave"
                      :formData="(editedItem = {})"
                      :donorPresetted="false"
                      :onlyRead="!authorizedRole"
                      :key="Math.ceil(Math.random() * 1000)"
                    ></gmt-gt-form>
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
                  <v-spacer></v-spacer>
                  <v-tooltip bottom v-if="false">
                    <template v-slot:activator="{ on }">
                      <v-icon
                        color="primary lighten-2"
                        class="grey--text text--darken-3"
                        @click="openInfo()"
                        v-on="on"
                        >mdi-information-outline</v-icon
                      >
                    </template>
                    <span>{{ $t("datasource.gmt.grant.gt-color-legenda") }}</span>
                  </v-tooltip>
                  <v-tooltip bottom v-if="authorizedRole == true">
                    <template v-slot:activator="{ on }">
                      <v-icon
                        color="white"
                        class="grey--text text--darken-3"
                        @click="recoveryItem"
                        v-on="on"
                        >mdi-delete</v-icon
                      >
                    </template>
                    <span>{{
                      $t("datasource.gmt.grant.gt-recently-deleted-grant")
                    }}</span>
                  </v-tooltip>
                </div>
              </template>

              <template v-slot:body="{ items }">
                <tbody style="border: solid">
                  <!-- <tr
                    v-for="item in items"
                    :key="item.ID"
                    style="height: 10px"
                    @click="doClickAction(item)"
                  > -->
                  <tr v-for="item in items" :key="item.ID" style="height: 10px">
                    <td class="tablerow">
                      {{ item.CODE }}
                    </td>
                    <td class="tablerow">
                      {{ item.DONOR_NAME }}
                      <p v-if="item.PASSING_DONOR != undefined">
                        / {{ item.PASSING_DONOR }}
                      </p>
                    </td>
                    <!-- <td class="tablerow">
                      {{ item.PASSING_DONOR }}
                    </td> -->
                    <td class="tablerow">
                      {{ item.DESCR }}
                    </td>
                    <td class="tablerow">
                      <div style="display: inline">
                        <p style="color: gray; margin: 0; padding: 0">From:</p>
                        {{ item.START_DATE | formatDate }}
                      </div>
                      <div v-if="item.END_DATE != undefined">
                        <div style="color: gray; margin: 0; padding: 0">
                          <div v-if="!checkStatus(item.IMS_STATUS_NAME)">To:</div>
                          <div v-else>NCE:</div>
                        </div>

                        {{ item.END_DATE | formatDate }}
                      </div>
                    </td>
                    <td class="tablerow">
                      {{ item.DURATION }}
                    </td>
                    <td class="tablerow">
                        {{ item.AMOUNT |formatNumber }} {{ " " + item.CURRENCY_CODE }}
                    </td>
                    <td class="tablerow">
                      {{ item.LAST_REPORT | formatDate }}
                    </td>
                    <td class="tablerow">
                      {{ item.NEXT_NARRATIVE_REPORT | formatDate }}
                    </td>
                    <td class="tablerow">
                      {{ item.NEXT_FINANCIAL_REPORT | formatDate }}
                    </td>
                    <td class="tablerowLeft">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon :color="checkColor(item)" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{ checkLegend(item) }}</span>
                      </v-tooltip>
                      {{ item.IMS_STATUS_NAME }}
                    </td>
                    <!-- <td v-if="item.STATUS == 0" class="tablerow">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="red" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.design-in-progress")
                        }}</span>
                      </v-tooltip>
                    </td> -->
                    <td style="text-align: center">
                      <v-tooltip bottom v-if="authorizedRole == true">
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                            >mdi-circle-edit-outline</v-icon
                          >
                        </template>
                        <span>{{ $t("datasource.gmt.grant.gt-edit-grant") }}</span>
                      </v-tooltip>
                      <v-tooltip bottom v-else>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                            >mdi-eye</v-icon
                          >
                        </template>
                        <span>{{ $t("datasource.gmt.grant.gt-view-grant") }}</span>
                      </v-tooltip>
                      <v-tooltip bottom v-if="authorizedRole == true">
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
        </v-col>
      </v-row>
    </v-container>
    <v-dialog
      v-model="showInfo"
      persistent
      max-width="50%"
      retain-focus
      :scrollable="true"
      :overlay="true"
      transition="dialog-transition"
    >
      <v-card>
        <v-card-title>
          <span class="h5">Color Legenda</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <span>
              <table>
                <tr>
                  <td>
                    <v-icon color="red" small class="mr-2">mdi-circle</v-icon>:
                    {{ $t("datasource.gmt.grant.gt-legenda-red") }}
                    <tr>
                      <td>
                        <v-icon color="yellow" small class="mr-2">mdi-circle</v-icon>:
                        {{ $t("datasource.gmt.grant.gt-legenda-yellow") }}
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <v-icon color="orange" small class="mr-2">mdi-circle</v-icon>:
                        {{ $t("datasource.gmt.grant.gt-legenda-orange") }}
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <v-icon color="green" small class="mr-2">mdi-circle</v-icon>:
                        {{ $t("datasource.gmt.grant.gt-legenda-green") }}
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <v-icon color="purple" small class="mr-2">mdi-circle</v-icon>:
                        {{ $t("datasource.gmt.grant.gt-legenda-purple") }}
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <v-icon color="black" small class="mr-2">mdi-circle</v-icon>:
                        {{ $t("datasource.gmt.grant.gt-legenda-black") }}
                      </td>
                    </tr>
                  </td>
                </tr>
              </table>
            </span>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-btn text color="secondary darken-1" @click="closeInfo()"
            >X {{ $t("general.close") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog
      v-model="showDialogDetails"
      persistent
      max-width="600"
      v-if="module == 'GMT'"
    >
      <v-card>
        <v-card-title>
          <span class="h5">Grant Details</span>
        </v-card-title>
        <v-card-text>
          <jrs-readonly-detail
            :detailData="formEventsData"
            :detailFields="formEventsFields"
            :nbrOfColumns="1"
            :key="Math.ceil(Math.random() * 1000)"
          ></jrs-readonly-detail>
        </v-card-text>
        <v-card-actions>
          <v-btn text color="secondary darken-1" @click="closeGrantDialogDetails()"
            >X {{ $t("general.close") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-main>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "../../../axiosapi";
import { translate, i18n } from "../../../i18n";
import GrantMainData from "../../../components/GMT/GRANTS/PROPOSAL/GMTGrantMainDataForm.vue";
import { EventBus } from "../../../event-bus";
import { mapActions } from "vuex";
import UtilMix from "../../../mixins/UtilMix";
import JrsModalForm from "../../../components/JrsModalForm.vue";
import JrsReadonyDetail from "../../../components/JrsReadonyDetail.vue";
import moment from "moment";
import FormMixin from "../../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "gmt-gt-form": GrantMainData,
    "jrs-readonly-detail": JrsReadonyDetail,
    // "jrs-modal-form": JrsModalForm,
  },
  mixins: [FormMixin, UtilMix],
  data() {
    return {
      filters: {
        DONOR_NAME: [],
      },
      formEventsFields: [],
      formEventsData: {},
      showInfo: false,
      showDialogDetails: false,
      authorizedRole: false,
      module: "",

      showWait: false,
      dialog: false,
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
          text: "Donor",
          textKey: "datasource.gmt.grant.gt-donor-passing",
          align: "center",
          value: "DONOR_NAME",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: "Passing through organization",
        //   textKey: "datasource.gmt.grant.gt-passing-donor",
        //   align: "center",
        //   value: "PASSING_DONOR",
        //   sortable: true,
        //   filterable: true,
        // },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-title",
          align: "center",
          value: "DESCR",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: "Project",
        //   textKey: "datasource.gmt.grant.gt-prj",
        //   align: "center",
        //   value: "PROJECT_CODE",
        //   sortable: true,
        //   filterable: true,
        // },
        {
          text: "Start Date",
          textKey: "datasource.gmt.grant.gt-period",
          align: "center",
          value: "START_DATE",
          sortable: true,
          filterable: true,
          class: "custom-width-column",
        },

        {
          text: "Duration",
          textKey: "datasource.gmt.grant.gt-duration",
          align: "center",
          value: "DURATION",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Amount",
          textKey: "datasource.gmt.grant.gt-amount-reminder",
          align: "center",
          value: "AMOUNT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Last Report",
          textKey: "datasource.gmt.grant.gt-last-report",
          align: "center",
          value: "LAST_REPORT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Next Narrative Report",
          textKey: "datasource.gmt.grant.gt-next-narrative-report",
          align: "center",
          value: "NEXT_NARRATIVE_REPORT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Next Financial Report",
          textKey: "datasource.gmt.grant.gt-next-financial-report",
          align: "center",
          value: "NEXT_FINANCIAL_REPORT",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: "Start Date",
        //   textKey: "datasource.gmt.grant.gt-end-date",
        //   align: "center",
        //   value: "END_DATE",
        //   sortable: true,
        //   filterable: true,
        // },

        {
          text: "Status",
          textKey: "datasource.gmt.grant.gt-status-name",
          align: "center",
          value: "IMS_STATUS_NAME",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: "Status",
        //   textKey: "datasource.gmt.grant.gt-status",
        //   align: "center",
        //   value: "STATUS",
        //   sortable: true,

        //   filterable: true,
        // },
        // {
        //   text: "Info",
        //   textKey: "",
        //   align: "",
        //   value: "info",
        //   sortable: false,

        //   filterable: true,
        // },

        { text: "Actions", value: "action", sortable: false },
      ],
      rowsObj: {},
      grantList: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
      //,
      // itemModel: {
      //   apcode: null,
      //   descr: null,
      //   locationDescr: null,
      //   adminAreaName: null,
      //   startYear: null,
      //   statusName: null
      // }
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
    columns() {
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }
        // if (col.value === "STATUS") {
        //   col.prepend-icon="mdi-information-outline"
        //   return col;
        // }
        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

      return cols;
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.getGRANTS();

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
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    checkStatus(value: string) {
      if (value.toUpperCase() == "NO-COST EXTENSION (NCE) GRANTED") return true;
      else return false;
    },

    columnValueList(val: any) {
      let localThis = this as any;
      return localThis.grantList.map((d: any) => d[val]);
    },

    openInfo() {
      let localThis = this as any;
      localThis.showInfo = true;
    },
    closeInfo() {
      let localThis = this as any;
      localThis.showInfo = false;
    },
    isGMTModule() {
      let localThis = this as any;
      if (localThis.module == "GMT") return true;
      else return false;
    },
    getFormFields(item: any) {
      let localThis: any = this as any;
      let currentDate = new Date();
      localThis.showWait = true;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_DETAILS",
        colList: null,
        whereCond: `ID = ${item.ID}`,
        orderStmt: null,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          let tmpFormFileds: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis.formEventsFields = localThis.setupSelectFields(
            tmpFormFileds,
            localThis.getCurrentOffice.HR_OFFICE_ID,
            null
          );

          //dati
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              item.STATUS_ID = item.STATUS_ID + "";
              item.DONOR_ID = item.DONOR_ID + "";
              item.DEPARTMENT_ID = item.DEPARTMENT_ID + "";
              item.HR_POSITION_ID = item.HR_POSITION_ID + "";
              item.IMS_USER_USERNAME = localThis.getUserName(item.HR_POSITION_ID);
              //item.PROJECT_ID= item.PROJECT_ID+"";
              item.STATUS = 0;
              if (
                item.START_DATE != undefined &&
                item.START_DATE != "" &&
                item.END_DATE != undefined &&
                item.END_DATE != ""
              ) {
                var startdate = new Date(
                  item.START_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
                );
                var enddate = new Date(
                  item.END_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
                );
                if (
                  (startdate <= currentDate && enddate >= currentDate) ||
                  (startdate >= currentDate &&
                    enddate >= currentDate &&
                    startdate <= enddate)
                ) {
                  item.STATUS = 1; //in the interval of validity
                }
              }

              localThis.formEventsData = item;
            });
          }

          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },
    checkLegend(item: any) {
      if (item.STATUS == 0) {
        //scaduto
        switch (item.IMS_STATUS_NAME) {
          case "Prospecting":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          case "Proposal under development":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          case "Proposal under revision":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          case "Withdraw":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Submitted":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          case "Lost":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Awarded":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          case "Closed":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "No-cost extension (NCE) Requested":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          case "No-cost extension (NCE) granted":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
          default:
            return "";
        }

        // switch (item.IMS_STATUS_NAME) {
        //   case "Prospecting":
        //   case "Proposal under development":
        //   case "Proposal under revision":
        //   case "Submitted":
        //   case "No-cost extension (NCE) Requested":
        //     return "yellow";
        //   case "Awarded":
        //     return "green";
        //   case "Closed":
        //     return "purple";
        // }
      }

      if (item.STATUS == 1) {
        //non scaduto
        switch (item.IMS_STATUS_NAME) {
          case "Prospecting":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Proposal under development":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Proposal under revision":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Withdraw":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Submitted":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Lost":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Awarded":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "Closed":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "No-cost extension (NCE) Requested":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          case "No-cost extension (NCE) granted":
            return this.$i18n.t("datasource.gmt.grant.gt-legenda").toString();
          default:
            return "";
        }
      }
      return "red";
    },
    // checkLegend(item: any) {
    //   if (item.STATUS == 0) {
    //     //scaduto
    //     switch (item.IMS_STATUS_NAME) {
    //       case "Prospecting":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       case "Proposal under development":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       case "Proposal under revision":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       case "Withdraw":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-orange").toString();
    //       case "Submitted":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       case "Lost":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-black").toString();
    //       case "Awarded":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       case "Closed":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-purple").toString();
    //       case "No-cost extension (NCE) Requested":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       case "No-cost extension (NCE) granted":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-red").toString();
    //       default:
    //         return "";
    //     }

    //     // switch (item.IMS_STATUS_NAME) {
    //     //   case "Prospecting":
    //     //   case "Proposal under development":
    //     //   case "Proposal under revision":
    //     //   case "Submitted":
    //     //   case "No-cost extension (NCE) Requested":
    //     //     return "yellow";
    //     //   case "Awarded":
    //     //     return "green";
    //     //   case "Closed":
    //     //     return "purple";
    //     // }
    //   }

    //   if (item.STATUS == 1) {
    //     //non scaduto
    //     switch (item.IMS_STATUS_NAME) {
    //       case "Prospecting":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-yellow").toString();
    //       case "Proposal under development":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-yellow").toString();
    //       case "Proposal under revision":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-orange").toString();
    //       case "Withdraw":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-black").toString();
    //       case "Submitted":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-orange").toString();
    //       case "Lost":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-black").toString();
    //       case "Awarded":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-green").toString();
    //       case "Closed":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-purple").toString();
    //       case "No-cost extension (NCE) Requested":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-green").toString();
    //       case "No-cost extension (NCE) granted":
    //         return this.$i18n.t("datasource.gmt.grant.gt-legenda-green").toString();
    //       default:
    //         return "";
    //     }
    //   }
    //   return "red";
    // },
    checkColor(item: any) {
      if (item.STATUS == 0) {
        //scaduto
        switch (item.IMS_STATUS_NAME) {
          case "Prospecting":
            return "red";
          case "Proposal under development":
            return "green";
          case "Proposal under revision":
            return "red";
          case "Withdraw":
            return "orange";
          case "Submitted":
            return "red";
          case "Lost":
            return "black";
          case "Awarded":
            return "green";
          case "Closed":
            return "purple";
          case "No-cost extension (NCE) Requested":
            return "red";
          case "No-cost extension (NCE) granted":
            return "red";
          default:
            return "white";
        }

        // switch (item.IMS_STATUS_NAME) {
        //   case "Prospecting":
        //   case "Proposal under development":
        //   case "Proposal under revision":
        //   case "Submitted":
        //   case "No-cost extension (NCE) Requested":
        //     return "yellow";
        //   case "Awarded":
        //     return "green";
        //   case "Closed":
        //     return "purple";
        // }
      }



//Here you can change the Grant Status color IMS/IT Unit

      if (item.STATUS == 1) {
        //non scaduto
        switch (item.IMS_STATUS_NAME) {
          case "Prospecting":
            return "yellow";
          case "Proposal under development":
            return "yellow";
          case "Proposal under revision":
            return "orange";
          case "Withdraw":
            return "black";
          case "Submitted":
            return "orange";
          case "Lost":
            return "black";
          case "Awarded":
            return "green";
          case "Approved":
            return "green";
          case "Closed":
            return "purple";
          case "No-cost extension (NCE) Requested":
            return "green";
          case "No-cost extension (NCE) granted":
            return "green";
          default:
            return "white";
        }
      }
      return "red";
    },
    getGRANTS() {
      let localThis = this as any;
      let currentDate = new Date();
      localThis.grantList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT",
        //colList: null,
        whereCond: `OFFICE_ID=${localThis.getCurrentOffice.HR_OFFICE_ID}`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              item.STATUS_ID = item.STATUS_ID + "";
              item.DONOR_ID = item.DONOR_ID + "";
              item.DEPARTMENT_ID = item.DEPARTMENT_ID + "";
              item.HR_POSITION_ID = item.HR_POSITION_ID + "";
              item.IMS_USER_USERNAME = localThis.getUserName(item.HR_POSITION_ID);
              //item.PROJECT_ID= item.PROJECT_ID+"";
              item.STATUS = 0;
              if (
                item.START_DATE != undefined &&
                item.START_DATE != "" &&
                item.END_DATE != undefined &&
                item.END_DATE != ""
              ) {
                var startdate = new Date(
                  item.START_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
                );
                var enddate = new Date(
                  item.END_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
                );
                if (
                  (startdate <= currentDate && enddate >= currentDate) ||
                  (startdate >= currentDate &&
                    enddate >= currentDate &&
                    startdate <= enddate)
                ) {
                  item.STATUS = 1; //in the interval of validity
                }
              }

              localThis.grantList.push(item);
            });
            localThis.showWait = false;
          }
        })
        .finally(() => {
          localThis.showWait = false;
        });
    },
    recoveryItem() {
      this.$router.push({
        name: "Recovery Deleted Grants",
      });
    },
    doClickAction(item: any) {
      let localThis = this as any;
      localThis.showDialogDetails = true;
      localThis.getFormFields(item);
    },
    closeGrantDialogDetails() {
      let localThis = this as any;
      localThis.showDialogDetails = false;
    },
    getUserName(item: any) {
      let localThis = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_USERNAME",
        //colList: null,
        whereCond: `HR_POSITION_ID=${item}`,
        // orderStmt: "VALUE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              let name = item.HR_BIODATA_FULL_NAME;
              return name;
            });
          }
          // localThis.showWait = false;
        });
    },
    getGRANT(grantId: Number) {
      let localThis = this as any;

      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT",
        //colList: null,
        whereCond: `ID=${grantId}`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.showWait = false;
              localThis.editItem(item);
            });
          }
        });
    },

    closeDialog() {
      let localThis = this as any;
      let msgLeave: string = this.$i18n
        .t("datasource.gmt.grant.gt-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.dialog = false;
        }
      });
    },
    closeGrantDialogeAndSave(value: any) {
      let localThis = this as any;
      //localThis.getGRANTS();
      localThis.dialog = false;
      let itm = {} as any;
      let gt = {} as any;
      gt = localThis.$store.getters.getGrant;
      localThis.getGRANT(gt.grant_id);
    },

    editItem(item: any) {
      let localThis: any = this as any;
      let gt = {} as any;
      gt.grant_id = item.ID;
      gt.year = item.START_YEAR;
      gt.end_year = item.END_YEAR;
      gt.currency = item.CURRENCY_CODE;
      gt.donor_id = parseInt("" + item.DONOR_ID);
      gt.donor_name = item.DONOR_NAME;
      localThis.setGrant(gt);
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.$router.push({
        name: "Grant Details",
        params: {
          editItemMainData: localThis.editedItem,
          editItemNarrativeId: localThis.editedItem.ID,
          editItemOBJ: localThis.editedItem,
          donorPresetted: false,
        },
      });
      //this.rout.push(this.$route);
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
              localThis.getGRANTS();
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
  },
  filters: {
    formatDate: function (value: any) {
      // if ((string + "").toUpperCase() == "UNDEFINED") return "";
      // return string.substring(0, 10);
      if (value) {
        return moment(String(value)).format("DD MMM YYYY");
      }
    },
 
    //Format a number by adding thousands separator
    formatNumber: function (value: any) {
       if (value) {
        return Number(value).toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
      }
    },







},
});
</script>
<style scoped>
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
  padding: 0 1em;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
.tablerowLeft {
  text-align: left;
  height: 3.5em;
  padding: 0 1em;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}


</style>
