<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
          <div>
            <v-data-table
              :headers="headers"
              :items="donorList"
              item-key="GMT_DONOR_ID"
              :page="pageNum"
              multi-sort
              :search="tableFilter"
              @update:page="pageChange"
              class="text-capitalize elevation-2"
            >
              <template v-slot:top>
                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                  <h3>{{ $tc("datasource.gmt.donor.donors", 2) }}</h3>
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
                        v-if="module == 'GMT' && authorizedRole == true"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>New
                      </v-btn>
                    </template>

                    <gmt-donor-form
                      v-if="currentOffice == 1"
                      :isUpdating="false"
                      @closeDialoge="closeDialog"
                      @closeDialogeAndSave="closeDialogAndSave"
                      :formData="editedItem"
                      :key="keyDialog"
                      :onlyRead="!authorizedRole"
                    ></gmt-donor-form>
                    <v-card v-if="currentOffice != 1">
                      <v-card-title>
                        <span class="h5">Warning</span>
                      </v-card-title>
                      <v-card-text>
                        <v-container>
                          <span>
                            <table>
                              <tr>
                                <td>
                                  <h3>{{ $tc("datasource.gmt.donor.informio", 2) }}</h3>
                                </td>
                              </tr>
                            </table>
                          </span>
                        </v-container>
                      </v-card-text>
                      <v-card-actions>
                        <v-btn text color="secondary darken-1" @click="cloaseAlert()"
                          >X {{ $t("general.close") }}</v-btn
                        >
                      </v-card-actions>
                    </v-card>
                  </v-dialog>
                  <v-spacer></v-spacer>
                  <v-select
                    v-model="selectedCountry"
                    :items="countries"
                    :label="'Countries'"
                    item-text="text"
                    item-value="value"
                    @change="getDonors"
                    :readonly="false"
                    class="white"
                    hide-details
                    clearable
                    outlined
                    dense
                  ></v-select>
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
                  <!-- <tr
                    v-for="item in items"
                    :key="item.GMT_DONOR_ID"
                    style="height: 10px"
                    @click="doClickAction(item)"
                  > -->
                  <tr
                    v-for="item in items"
                    :key="item.GMT_DONOR_ID"
                    style="height: 10px"
                    @click="doClickAction(item)"
                  >
                    <td class="tablerow">{{item.GMT_DONOR_NAME }}</td>
                    <td class="tablerow">
                      {{ item.IMS_LOOKUP_VALUE }}
                    </td>
                    <td class="tablerow">
                      {{ item.GMT_DONOR_DESCRIPTION | subStr }}
                    </td>

                    <td style="text-align: center">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                            >mdi-circle-edit-outline</v-icon
                          >
                        </template>
                        <span>{{ $t("datasource.gmt.donor.edit-donor") }}</span>
                      </v-tooltip>
                      <v-tooltip
                        bottom
                        v-if="authorizedRole == true && currentOffice == 1"
                      >
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                            >mdi-delete</v-icon
                          >
                        </template>
                        <span>{{ $t("datasource.gmt.donor.delete-donor") }}</span>
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
      v-model="showDialogDetails"
      persistent
      max-width="600"
      v-if="module == 'GMT'"
    >
      <v-card>
        <v-card-title>
          <span class="h5">Donor Details</span>
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
          <v-btn text color="secondary darken-1" @click="closeDonorDialogDetails()"
            >X {{ $t("general.close") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import { translate } from "../../../i18n";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import DonorMainData from "../../../components/GMT/DONORS/MAIN/GMTDonorMainDataForm.vue";
import JrsReadonyDetail from "../../../components/JrsReadonyDetail.vue";
import { EventBus } from "../../../event-bus";
import { mapActions } from "vuex";
import UtilMix from "../../../mixins/UtilMix";
import FormMixin from "../../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gmt-donor-form": DonorMainData,
    "jrs-readonly-detail": JrsReadonyDetail,
  },
  mixins: [FormMixin, UtilMix],
  data() {
    return {
      formEventsFields: [],
      formEventsData: {},
      showDialogDetails: false,
      pageNum: 1,
      selectedCountry: "",
      currentOffice: 0,
      countries: [],
      authorizedRole: false,
      module: "",
      showWait: false,
      keyDialog: 0,
      dialog: false,
      headers: [],
      donorList: [],
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
    language() {
      return i18n.locale;
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.getDonors();

    localThis.setupHeaders();
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    localThis.currentOffice = localThis.getCurrentOffice.HR_OFFICE_ID;
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
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    pageChange(newPage: any) {
      let localThis = this as any;
      localStorage.setItem("pageDonor", newPage);
      localThis.pageNum = newPage;
    },
    getFormFields(item: any) {
      let localThis: any = this as any;
      let currentDate = new Date();
      localThis.showWait = true;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_DETAILS",
        colList: null,
        whereCond: `GMT_DONOR_ID = ${item.GMT_DONOR_ID}`,
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
    getDonors() {
      let localThis = this as any;
      localThis.selecteddonor = null;
      localThis.donorList = [];
      localThis.showWait = true;
      let wherecond = null;
      if (localThis.selectedCountry != undefined && localThis.selectedCountry != "") {
        wherecond = `COUNTRIES LIKE '%${localThis.selectedCountry}%'`;
      }

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR",
        //colList: null,
        whereCond: wherecond,
        orderStmt: "GMT_DONOR_NAME",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.donorList.push(item);
            });
          }
          localThis.showWait = false;
          let pagN = localStorage.getItem("pageDonor");
          if (pagN != undefined && pagN != "0") {
            localThis.pageNum = localStorage.getItem("pageDonor");
            localStorage.setItem("pageDonor", "0");
          }
        });
    },
    doClickAction(item: any) {
      let localThis = this as any;
      localThis.showDialogDetails = true;
      localThis.getFormFields(item);
    },
    closeDonorDialogDetails() {
      let localThis = this as any;
      localThis.showDialogDetails = false;
    },
    loadCountries() {
      let localThis = this as any;
      localThis.selectedCountry = null;
      localThis.countries = [];
      let allCountries = {} as any;
      allCountries.text = "";
      allCountries.value = "";
      localThis.countries.push(allCountries);
      localThis.showWait = true;

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_COUNTRIES",
        //colList: null,
        //whereCond: `IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "text",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.countries.push(item);
            });
          }
          localThis.showWait = false;
        });
    },
    cloaseAlert() {
      let localThis = this as any;
      localThis.dialog = false;
    },
    closeDialog() {
      let localThis = this as any;

      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.dialog = false;
          localThis.editedItem = {};
          localThis.keyDialog = Math.ceil(Math.random() * 1000);

          // (this as any).editedItem = (this as any).itemModel;
        }
      });
    },
    closeDialogAndSave(value: any) {
      let localThis = this as any;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
      localThis.dialog = false;
      localThis.getDonors();
      localThis.editedItem = {} as any;
      localThis.editedItem.GMT_DONOR_ID = value.GMT_DONOR_ID;
      localThis.editedItem.GMT_DONOR_NAME = value.GMT_DONOR_NAME;
      localThis.editedItem.GMT_DONOR_DESCRIPTION = value.GMT_DONOR_DESCRIPTION;
      localThis.editedItem.GMT_DONOR_NAV_ID = value.GMT_DONOR_NAV_ID;
      localThis.editedItem.GMT_DONOR_SALESFORCE_ID = value.GMT_DONOR_SALESFORCE_ID;
      localThis.editedItem.GMT_DONOR_LOCATION_COVERAGE_ID =
        value.GMT_DONOR_LOCATION_COVERAGE_ID;
      localThis.editedItem.GMT_DONOR_PROPOSAL_LANGUAGE =
        value.GMT_DONOR_PROPOSAL_LANGUAGE;
      localThis.editedItem.GMT_DONOR_EMAIL = value.GMT_DONOR_EMAIL;
      localThis.editedItem.GMT_DONOR_TYPE_ID = value.GMT_DONOR_TYPE_ID;

      localThis.editedItem.GMT_DONOR_WEBSITE = value.GMT_DONOR_WEBSITE;
      localThis.editedItem.GMT_DONOR_UID = value.GMT_DONOR_UID;
      localThis.editedItem.GMT_DONOR_CURRENCY_CODE = value.GMT_DONOR_CURRENCY_CODE;
      localThis.editItem(localThis.editedItem);
      return;
    },
    // editItem(item: any) {
    //   let localThis: any = this as any;
    //   localThis.keyDialog = Math.ceil(Math.random() * 1000);
    //   localThis.editedItem = JSON.parse(JSON.stringify(item));
    //   localThis.dialog = true;
    // },

    editItem(item: any) {
      let localThis: any = this as any;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localStorage.setItem("pageDonor", localThis.pageNum);
      localThis.$router.push({
        name: "Donor Details",
        params: {
          editItemMainData: localThis.editedItem,
          onlyRead: !localThis.authorizedRole || localThis.currentOffice != 1,
        },
      });
      //this.rout.push(this.$route);
    },

    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.gmt.donor.donor-delete-confirm")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.GMT_DONOR_ID = item.GMT_DONOR_ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_DONOR",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getDonors();
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
        {
          text: this.$i18n.t("datasource.gmt.donor.name").toString(),
          align: "center",
          value: "GMT_DONOR_NAME",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.grant.gt-type").toString(),

          align: "center",
          value: "IMS_LOOKUP_VALUE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.donor.description").toString(),

          align: "center",
          value: "GMT_DONOR_DESCRIPTION",
          sortable: true,
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.donor-enabled").toString(),

        //   align: "center",
        //   value: "PMS_COI_ENABLED",
        //   sortable: true,
        //   divider: true
        // },
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
  beforeMount() {
    let localThis = this as any;
    localThis.loadCountries();
  },
  filters: {
    subStr: function (string: any) {
      if (string != undefined) return string.substring(0, 40) + "...";
      else return "";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getDonors();
      localThis.setupHeaders();
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
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
</style>
