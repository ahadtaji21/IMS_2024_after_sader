<template>
  <v-card>
    <div class="text-center" v-if="isLoading" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" ref="myForm" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-code')"
                v-model="localFormData.CODE"
                :counter="150"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 5">
              <v-select
                :items="donors"
                :label="$t('datasource.gmt.grant.gt-donor')"
                item-text="text"
                item-value="value"
                v-model="localFormData.DONOR_ID"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col>

            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 5"
              v-if="localFormData.PASSING"
            >
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-passing-donor')"
                v-model="localFormData.PASSING_DONOR"
                :readonly="true"
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">
              <v-checkbox
                v-model="localFormData.PASSING"
                :readonly="onlyReadStatus"
                @change="SrchDnr"
              ></v-checkbox>
            </v-col>
            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 5"
              v-if="!localFormData.PASSING"
              sm="4"
            >
              <div style="align: bottom">
                <br />
                {{ $t("datasource.gmt.grant.gt-passing-another") }}
              </div>
            </v-col>
          </v-row>
          <!-- <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.gmt.donor.donor-contact')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_NAME"
                :readOnly="true"
                @click="showContact()"
              ></v-text-field>
            </v-col>
          </v-row> -->
          <v-row
            v-if="
              false &&
              departments.filter((item) => item.DONOR_ID == localFormData.DONOR_ID)
                .length > 0
            "
          >
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-select
                :items="
                  departments.filter((item) => item.DONOR_ID == localFormData.DONOR_ID)
                "
                :label="$t('datasource.gmt.donor.donor-contact')"
                item-text="text"
                item-value="value"
                v-model="localFormData.DEPARTMENT_ID"
              ></v-select>
            </v-col>
          </v-row>
          <v-row v-if="true">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-title')"
                v-model="localFormData.DESCR"
                :counter="150"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></v-text-field>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="localFormData.LANGUAGE"
                :items="languages"
                outlined
                :loading="isLoading"
                :search-input.sync="search1"
                dense
                :label="$t('datasource.gmt.donor.proposal-language')"
                item-text="text"
                item-value="value"
           
              ></v-autocomplete>
            </v-col>
            <!-- <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="projects"
                :label="$t('datasource.gmt.grant.gt-prj')"
                item-text="text"
                item-value="value"
                v-model="localFormData.PROJECT_ID"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col> -->
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-user-assigned')"
                v-model="userName"
                :counter="150"
                :rules="rules.required"
                :readonly="true"
                @click="SrchUser"
              ></v-text-field>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-date-picker
                :label="$t('general.date-start')"
                v-model="localFormData.START_DATE"
                :hint="$t('general.date-start')"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></jrs-date-picker>
              <!-- <v-text-field
                :label="$t('datasource.gmt.grant.gt-start-year')"
                v-model="localFormData.START_YEAR"
                :counter="150"
                type="number"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></v-text-field> -->
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-date-picker
                :label="$t('general.date-end')"
                v-model="localFormData.END_DATE"
                :hint="$t('general.date-end')"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></jrs-date-picker>
              <!-- <v-text-field
                :label="$t('datasource.gmt.grant.gt-end-year')"
                v-model="localFormData.END_YEAR"
                :counter="150"
                type="number"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></v-text-field> -->
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="status"
                :label="$t('datasource.gmt.grant.gt-status-name')"
                item-text="text"
                item-value="value"
                v-model="localFormData.STATUS_ID"
                :readonly="onlyReadStatus"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="localFormData.CURRENCY_CODE"
                :items="currency"
                outlined
                :loading="isLoading"
                :search-input.sync="search"
                dense
                :label="$t('datasource.gmt.grant.gt-currency')"
                item-text="text"
                item-value="value"
                :rules="[(v) => !!v || 'Item is required']"
                
              ></v-autocomplete>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <label
                id="lbl"
                :class="
                  toValidate == true &&
                  (localFormData.AMOUNT == undefined || localFormData.AMOUNT == '')
                    ? 'v-label theme--light redStyle'
                    : 'v-label theme--light'
                "
                >{{ $t("datasource.gmt.grant.gt-total-grant") }}:</label
              >
              <currency-input
                :readonly="onlyReadStatus"
                v-model="localFormData.AMOUNT"
                v-bind="options"
                :currency="localFormData.CURRENCY_CODE"
                :class="
                  toValidate == true &&
                  (localFormData.AMOUNT == undefined || localFormData.AMOUNT == '')
                    ? 'redBudget'
                    : 'Budget'
                "
                aria-labelledby="lbl"
                :rules="rules.required"
              ></currency-input>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                v-model="localFormData.INDIRECT_COSTS"
                :label="$t('datasource.gmt.grant.gt-indirect-costs')"
                type="number"
                :rules="rules1"
                :readonly="onlyReadStatus"
                :hint="$t('datasource.gmt.grant.gt-indirect-costs-hint')"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                v-model="localFormData.INDIRECT_COSTS_NOTE"
                :label="$t('datasource.gmt.grant.gt-indirect-costs-note')"
                :readonly="onlyReadStatus"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <a
                target="_blank"
                :href="'https://jesuitrefugeeservice720.sharepoint.com/:x:/r/teams/FinanceResources/_layouts/15/doc2.aspx?sourcedoc=%7BB0DD3624-2BCE-4029-919D-F0430C05E503%7D&file=ISC_Calculation.xlsx&action=default'"
              >
                <label id="lbl1">{{ $t("datasource.gmt.grant.gt-isc-file") }}</label>
              </a>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" v-if="!onlyReadStatus" class="--darken-1" @click="save()"
        >Save</v-btn
      >
    </v-card-actions>
    <v-dialog
      v-if="!onlyRead"
      v-model="searchUser"
      persistent
      retain-focus
      :scrollable="true"
      :overlay="false"
      transition="dialog-transition"
    >
      <search-table-user
        v-model="profileId"
        @input="closeDialog"
        :key="keyRnd"
        :selectedObjectId="selectedObjectId"
        :insertedPositions="positionList"
      ></search-table-user>
    </v-dialog>

    <v-dialog
      v-if="!onlyRead"
      v-model="searchDonor"
      persistent
      retain-focus
      :scrollable="true"
      :overlay="false"
      transition="dialog-transition"
    >
      <search-table-donor
        v-model="localFormData.PASSING_DONOR_ID"
        @UpdateItem="closeDialogDonor"
        :key="keyRndDonor"
      ></search-table-donor>
    </v-dialog>

    <!-- <v-dialog
      v-if="!onlyRead"
      v-model="showGrantContactDetails"
      persistent
      retain-focus
      :scrollable="true"
      :overlay="false"
      transition="dialog-transition"
    >
      <grant-contact
        v-model="localFormData.PASSING_DONOR_ID"
        @closeContactDialoge="closeContact()"
        @closeContactDialogeAndSave="closeContactAndSave"
        :grantDonorId="localFormData.DONOR_ID"
        :formData="localFormData"
        :key="keyRndContact"
      ></grant-contact>
    </v-dialog> -->
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { ImsApi, ImsLookupsApi, Configuration } from "../../../../axiosapi";
import { i18n } from "../../../../i18n";
import { IPayLoadDataGRANT } from "./../../../PMS/PMSSharedInterfaces";
import SearchTableUser from "../../../PMS/POSITIONS/AnnualPlanPositionSearchForm.vue";
import JrsDatePicker from "../../../../components/JrsDatePicker.vue";
import { EventBus } from "../../../../event-bus";
import UtilMix from "../../../../mixins/UtilMix";
import SearchTableDonor from "../../DONORS/GMTDonorSearch.vue";
import GrantContact from "./GMTGrantContactMainDataForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "search-table-user": SearchTableUser,
    "search-table-donor": SearchTableDonor,
    "jrs-date-picker": JrsDatePicker,
    //"grant-contact": GrantContact,
  },

  props: {
    formData: {
      type: Object,
    },
    presetVal: {
      type: String,
      default: "",
    },
    presetVal1: {
      type: String,
      required: false,
      default: "",
    },
    donorPresetted: {
      type: Boolean,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  mixins: [UtilMix],
  data() {
    return {
      search1: this.presetVal1,
      rules1: [
        (v: any) => !v || v <= 100 || "% must be <= 100",
        (v: any) => !v || v >= 0 || "% must be >= 0",
      ],
      localFormData: {},
      showGrantContactDetails: false,
      profileId: 0,
      selectedObjectId: 0,
      hr_position_id: 0,
      keyRnd: 0,
      keyRndContact: 0,
      keyRndDonor: 0,
      userName: "",
      positionList: [],
      searchUser: false,
      searchDonor: false,
      role: "",
      onlyReadStatus: true,
      authorizedRole: false,
      formValid: false,
      toValidate: false,
      isLoading: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
      status: [],
      donors: [],
      languages: [],
      departments: [],
      currency: [],
      projects: [],
      search: this.presetVal,
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
    ...mapGetters({
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  watch: {
    presetVal: function (val) {
      this.search = val;
    },
    presetVal1: function (val) {
      this.search1 = val;
    },
  },
  beforeDestroy() {
    // EventBus.$off("userRoleUpdated");
    EventBus.$off("saveGTMainDataFromMainSave");
  },
  async beforeMount() {
    let localThis: any = this;
    localThis.role = localThis.getCurrentRole.ROLE_CODE;
    if (localThis.role == "GM") {
      localThis.authorizedRole = !localThis.onlyRead;
      //localThis.getStatusList();
    } else {
      localThis.authorizedRole = false;
    }

    localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;

    localThis.formValid = false;
    if (localThis.languages.length == undefined || localThis.languages.length == 0)
      localThis.getList("LANGUAGE_LIST");
    if (localThis.status.length == undefined || localThis.status.length == 0)
      localThis.getList("GT_STATUS_LIST");
    if (localThis.donors.length == undefined || localThis.donors.length == 0)
      localThis.getList("GT_DONOR_LIST");
    if (localThis.projects.length == undefined || localThis.projects.length == 0)
      localThis.getList("GT_PROJECT_LIST");
    if (localThis.currency.length == undefined || localThis.currency.length == 0)
      localThis.getList("CURRENCY_LIST");
    if (localThis.departments.length == undefined || localThis.departments.length == 0) {
      const a = await localThis.getDepartments().then((res: any) => {
        return res;
      });
    }
  },

  async mounted() {
    let localThis: any = this;
    localThis.localFormData = JSON.parse(JSON.stringify(localThis.formData));
    // localThis.onlyReadStatus = Object.assign(
    //   false,
    //   localThis.onlyReadStatus,
    //   localThis.onlyRead || !localThis.authorizedRole
    // );
    localThis.role = localThis.getCurrentRole.ROLE_CODE;
    EventBus.$on("saveGTMainDataFromMainSave", (data: any) => {
      localThis.saveFromMain();
    });
    // EventBus.$on("userRoleUpdated", (to: any) => {
    //   let localThis: any = this as any;
    //   localThis.authorizedRole = localThis.donorPresetted;
    //   localThis.role = localThis.getCurrentRole.ROLE_CODE;
    //   if (localThis.role == "GM") {
    //     localThis.authorizedRole = localThis.donorPresetted;
    //     //localThis.getStatusList();
    //   } else {
    //     localThis.authorizedRole = false;
    //   }
    //   localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
    // });
    localThis.positionList = [];
    if (localThis.localFormData.HR_POSITION_ID == "undefined") {
      localThis.localFormData.HR_POSITION_ID = undefined;
    }
    if (localThis.localFormData.DONOR_ID == "undefined") {
      localThis.localFormData.DONOR_ID = undefined;
    }
    if (localThis.localFormData.DEPARTMENT_ID == "undefined") {
      localThis.localFormData.DEPARTMENT_ID = undefined;
    }
    if (localThis.localFormData.STATUS_ID == "undefined") {
      localThis.localFormData.STATUS_ID = undefined;
    }
    if (
      localThis.localFormData.HR_POSITION_ID != undefined &&
      localThis.localFormData.HR_POSITION_ID + "" != ""
    ) {
      localThis.hr_position_id = localThis.localFormData.HR_POSITION_ID;
      localThis.positionList.push(localThis.localFormData.HR_POSITION_ID);
      localThis.isLoading = true;
      localThis.userName = await localThis
        .getUserName(localThis.localFormData.HR_POSITION_ID)
        .then((res: any) => {
          return res;
        });
      localThis.isLoading = false;
    }

    if (
      localThis.localFormData.DONOR_ID != undefined &&
      localThis.localFormData.DONOR_ID + "" != ""
    ) {
      localThis.localFormData.DONOR_ID = localThis.localFormData.DONOR_ID + "";
    }
    if (
      localThis.localFormData.DEPARTMENT_ID != undefined &&
      localThis.localFormData.DEPARTMENT_ID + "" != ""
    ) {
      localThis.localFormData.DEPARTMENT_ID = localThis.localFormData.DEPARTMENT_ID + "";
    }
    localThis.localFormData.PASSING = false;
    if (
      localThis.localFormData.PASSING_DONOR_ID != undefined &&
      localThis.localFormData.PASSING_DONOR_ID + "" != "" &&
      localThis.localFormData.PASSING_DONOR_ID + "" != "0"
    ) {
      localThis.localFormData.PASSING = true;
    }
    if (
      localThis.localFormData.STATUS_ID != undefined &&
      localThis.localFormData.STATUS_ID + "" != ""
    ) {
      localThis.localFormData.STATUS_ID = localThis.localFormData.STATUS_ID + "";
    }
    (localThis.$refs.myForm as Vue & {
      resetValidation: () => void;
    }).resetValidation();
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
    close: function () {
      let localThis = this as any;
      (localThis.$refs.myForm as Vue & { reset: () => void }).reset();
      this.$emit("closeDialoge");
    },
    showContact() {
      let localThis = this as any;
      localThis.keyRndContact = Math.ceil(Math.random() * 1000);
      localThis.showGrantContactDetails = true;
    },
    closeContact() {
      let localThis = this as any;
      localThis.showGrantContactDetails = false;
    },
    // closeContactAndSave(item: any) {
    //   let localThis = this as any;
    //   localThis.showGrantContactDetails = false;

    //   if (item == undefined || item == null) {
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID = -1;
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NAME = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NAME = "";

    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_DESCRIPTION = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_EMAIL = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NOTES = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PHONE = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_GENDER = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID = 0;
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE = "";
    //     localThis.localFormData.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE = "";
    //     localThis.saveFromMain();
    //     return;
    //   }
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NAME =
    //     item.GMT_GRANT_DONOR_CONTACT_NAME;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID =
    //     item.GMT_GRANT_DONOR_CONTACT_ID;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_DESCRIPTION =
    //     item.GMT_GRANT_DONOR_CONTACT_DESCRIPTION;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_EMAIL =
    //     item.GMT_GRANT_DONOR_CONTACT_EMAIL;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NOTES =
    //     item.GMT_GRANT_DONOR_CONTACT_NOTES;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PHONE =
    //     item.GMT_GRANT_DONOR_CONTACT_PHONE;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_GENDER =
    //     item.GMT_GRANT_DONOR_CONTACT_GENDER;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID =
    //     item.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE =
    //     item.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE;
    //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE =
    //     item.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE;
    //   if (localThis.localFormData.ID != undefined && localThis.localFormData.ID != 0) {
    //     localThis.saveFromMain();
    //   }
    // },
    saveFromMain: function () {
      let localThis = this as any;
      localThis.toValidate = true;
      let isValid = true;
      isValid = (localThis.$refs.myForm as Vue & {
        validate: () => boolean;
      }).validate();
      isValid =
        isValid &&
        localThis.localFormData.AMOUNT != undefined &&
        localThis.localFormData.AMOUNT + "" != "";
      if (!isValid) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Please insert all the needed data",
        }); // Feedback to user
        return;
      }

      let id = 0;

      if (localThis.localFormData.ID == undefined) {
        //
      } else {
        id = Number(localThis.localFormData.ID);
      }

      let i: number;
      let status_name = "";
      for (i = 0; i < localThis.status.length; i++) {
        if (localThis.status[i].value == localThis.localFormData.status_id) {
          status_name = localThis.status[i].text;
          break;
        }
      }
      localThis.localFormData.status_name = status_name;

      let donor_name = "";
      for (i = 0; i < localThis.donors.length; i++) {
        if (localThis.donors[i].value == localThis.localFormData.donor_id) {
          donor_name = localThis.donors[i].text;
          break;
        }
      }
      localThis.localFormData.donor_name = donor_name;
      let ap = {} as any;
      let payLoadD = {} as any;

      payLoadD.ID = id;
      payLoadD.CODE = localThis.localFormData.CODE;
      payLoadD.DESCR = localThis.localFormData.DESCR;
      payLoadD.LANGUAGE = localThis.localFormData.LANGUAGE;
      payLoadD.START_DATE = localThis.localFormData.START_DATE;
      payLoadD.END_DATE = localThis.localFormData.END_DATE;
      payLoadD.AMOUNT_FUNDED = localThis.localFormData.AMOUNT_FUNDED;
      payLoadD.INCOME_TO_DATE = localThis.localFormData.INCOME_TO_DATE;
      payLoadD.AMOUNT = localThis.localFormData.AMOUNT;
      payLoadD.ACTUAL = localThis.localFormData.ACTUAL;
      payLoadD.INDIRECT_COSTS = localThis.localFormData.INDIRECT_COSTS || 0;
      payLoadD.INDIRECT_COSTS_NOTE = localThis.localFormData.INDIRECT_COSTS_NOTE;
      payLoadD.STATUS_ID = localThis.localFormData.STATUS_ID;
      //payLoadD.PROJECT_ID = localThis.localFormData.PROJECT_ID;
      payLoadD.DONOR_ID = localThis.localFormData.DONOR_ID;
      payLoadD.CURRENCY_CODE = localThis.localFormData.CURRENCY_CODE;
      payLoadD.VERSION = localThis.localFormData.VERSION;
      payLoadD.HR_POSITION_ID = localThis.hr_position_id;
      payLoadD.DEPARTMENT_ID = localThis.localFormData.DEPARTMENT_ID;
      payLoadD.OFFICE_ID = localThis.getCurrentOffice.HR_OFFICE_ID;
      payLoadD.PASSING_DONOR_ID = localThis.localFormData.PASSING_DONOR_ID;
      if (payLoadD.VERSION == undefined || localThis.localFormData == null)
        payLoadD.VERSION = 0;

      //contact
      // payLoadD.GMT_GRANT_DONOR_CONTACT_NAME =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NAME;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_ID =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_DESCRIPTION =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_DESCRIPTION;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_EMAIL =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_EMAIL;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_NOTES =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NOTES;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_PHONE =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PHONE;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_GENDER =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_GENDER;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE;
      // payLoadD.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE =
      //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_UPD_GRANT",
        actionType: payLoadD.ID == 0 ? SqlActionType.NUMBER_0 : SqlActionType.NUMBER_1,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          }); // Feedback to user

          let gt = {} as any;
          gt.grant_id = res.ID_GRANT;
          // localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID = res.ID_CONTACT;
          gt.year = payLoadD.START_YEAR;
          gt.currency = payLoadD.CURRENCY_CODE;
          gt.donor_id = payLoadD.DONOR_ID;
          localThis.setGrant(gt);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    save: function () {
      let localThis = this as any;
      localThis.toValidate = true;
      let isValid = true;
      isValid = (localThis.$refs.myForm as Vue & {
        validate: () => boolean;
      }).validate();
      isValid =
        isValid &&
        localThis.localFormData.AMOUNT != undefined &&
        localThis.localFormData.AMOUNT + "" != "";
      if (!isValid) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Please insert all the needed data",
        }); //
        return;
      }

      let msgUpd: string = this.$i18n
        .t("datasource.gmt.grant.gt-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n.t("datasource.gmt.grant.gt-add-confirm").toString();

      let id = 0;
      let msg = msgUpd;
      if (localThis.localFormData.ID == undefined) {
        msg = msgAdd;
      } else {
        id = Number(localThis.localFormData.ID);
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          let i: number;
          let status_name = "";
          for (i = 0; i < localThis.status.length; i++) {
            if (localThis.status[i].value == localThis.localFormData.status_id) {
              status_name = localThis.status[i].text;
              break;
            }
          }
          localThis.localFormData.status_name = status_name;

          let donor_name = "";
          for (i = 0; i < localThis.donors.length; i++) {
            if (localThis.donors[i].value == localThis.localFormData.donor_id) {
              donor_name = localThis.donors[i].text;
              break;
            }
          }
          localThis.localFormData.donor_name = donor_name;
          let ap = {} as any;
          let payLoadD = {} as any;

          payLoadD.ID = id;
          payLoadD.CODE = localThis.localFormData.CODE;
          payLoadD.DESCR = localThis.localFormData.DESCR;
          payLoadD.LANGUAGE = localThis.localFormData.LANGUAGE;
          payLoadD.START_DATE = localThis.localFormData.START_DATE;
          payLoadD.END_DATE = localThis.localFormData.END_DATE;
          payLoadD.AMOUNT_FUNDED = localThis.localFormData.AMOUNT_FUNDED;
          payLoadD.INCOME_TO_DATE = localThis.localFormData.INCOME_TO_DATE;
          payLoadD.ACTUAL = localThis.localFormData.ACTUAL;
          payLoadD.INDIRECT_COSTS = localThis.localFormData.INDIRECT_COSTS || 0;
          payLoadD.INDIRECT_COSTS_NOTE = localThis.localFormData.INDIRECT_COSTS_NOTE;
          payLoadD.AMOUNT = localThis.localFormData.AMOUNT;
          payLoadD.STATUS_ID = localThis.localFormData.STATUS_ID;
          //payLoadD.PROJECT_ID = localThis.localFormData.PROJECT_ID;
          payLoadD.DONOR_ID = localThis.localFormData.DONOR_ID;
          payLoadD.CURRENCY_CODE = localThis.localFormData.CURRENCY_CODE;
          payLoadD.VERSION = localThis.localFormData.VERSION;
          payLoadD.HR_POSITION_ID = localThis.hr_position_id;
          payLoadD.DEPARTMENT_ID = localThis.localFormData.DEPARTMENT_ID;
          payLoadD.OFFICE_ID = localThis.getCurrentOffice.HR_OFFICE_ID;
          payLoadD.PASSING_DONOR_ID = localThis.localFormData.PASSING_DONOR_ID;
          if (payLoadD.VERSION == undefined || localThis.localFormData == null)
            payLoadD.VERSION = 0;

          //contact
          // payLoadD.GMT_GRANT_DONOR_CONTACT_NAME =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NAME;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_ID =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_DESCRIPTION =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_DESCRIPTION;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_EMAIL =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_EMAIL;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_NOTES =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NOTES;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_PHONE =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PHONE;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_GENDER =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_GENDER;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE;
          // payLoadD.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE =
          //   localThis.localFormData.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_GRANT",
            actionType:
              payLoadD.ID == 0 ? SqlActionType.NUMBER_0 : SqlActionType.NUMBER_1,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user

              // if (localThis.authorizedRole) {
              //   (localThis.$refs.myForm as Vue & { reset: () => void }).reset();
              // }

              let gt = {} as any;
              gt.grant_id = res.ID_GRANT;
              // localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID = res.ID_CONTACT;
              gt.year = payLoadD.START_YEAR;
              gt.currency = payLoadD.CURRENCY_CODE;
              gt.donor_id = payLoadD.DONOR_ID;
              localThis.setGrant(gt);

              this.$emit("closeGrantDialogeAndSave");
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
    SrchUser(item: any) {
      let localThis = this as any;
      localThis.searchUser = true;
      localThis.keyRnd = Math.ceil(Math.random() * 1000);
    },
    SrchDnr(item: any) {
      let localThis = this as any;

      if (localThis.localFormData.PASSING == true) {
        localThis.searchDonor = true;
        localThis.keyRndDonor = Math.ceil(Math.random() * 1000);
      } else {
        localThis.localFormData.PASSING_DONOR = "";
        localThis.localFormData.PASSING_DONOR_ID = 0;
      }
    },

    async closeDialog(item: any) {
      let localThis = this as any;
      if (item != undefined && item != "") {
        localThis.hr_position_id = item;

        localThis.isLoading = true;
        localThis.userName = await localThis.getUserName(item).then((res: any) => {
          return res;
        });
        localThis.isLoading = false;
      }
      localThis.searchUser = false;
    },
    closeDialogDonor(item: any) {
      let localThis = this as any;
      if (item != undefined && item != "") {
        localThis.localFormData.PASSING_DONOR_ID = item;
        localThis.localFormData.PASSING = true;
        localThis.localFormData.PASSING_DONOR = localThis.donors
          .filter(
            (item: any) => item.value == localThis.localFormData.PASSING_DONOR_ID + ""
          )
          .map((item: any) => item.text)[0];
      } else {
        localThis.localFormData.PASSING = false;
      }
      localThis.searchDonor = false;
    },

    async getDepartments() {
      let localThis = this as any;
      localThis.isLoading = true;
      localThis.grants = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_DEPARTMENTS",
        //colList: null,
        //whereCond: `DONOR_ID=${this.selectedObjectId}`,
        orderStmt: "VALUE",
      };

      return new Promise((resolve, reject) => {
        localThis
          .getGenericSelect({ genericSqlPayload: selectPayload })
          .then((res: any) => {
            //Setup data
            if (res.table_data) {
              res.table_data.forEach((item: any) => {
                let itm: any = {};
                itm.text = item.TEXT;
                itm.value = item.VALUE;
                itm.DONOR_ID = item.GMT_DONOR_ID;
                localThis.departments.push(itm);
              });
              resolve(localThis.departments);
              return "";
            }
          })
          .catch(() => reject);
        // localThis.showWait = false;
      }).finally(() => (localThis.isLoading = false));
    },

    getUserName(item: any) {
      let localThis = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_USERNAME",
        //colList: null,
        whereCond: `HR_POSITION_ID=${item}`,
        // orderStmt: "VALUE",
      };
      //       return new Promise((resolve, reject) => {
      //   Vue.http.get(url)
      //       .then(response => resolve(response))
      //       .catch(() => reject)
      // })
      return new Promise((resolve, reject) => {
        localThis
          .getGenericSelect({ genericSqlPayload: selectPayload })
          .then((res: any) => {
            //Setup data
            if (res.table_data) {
              res.table_data.forEach((item: any) => {
                let name = item.HR_BIODATA_FULL_NAME;
                resolve(name);
              });
            }
            // localThis.showWait = false;
          })
          .catch(() => reject);
      });
    },

    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      this.isLoading = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "LANGUAGE_LIST":
              localThis.languages = res.data;
              break;
            case "GT_STATUS_LIST":
              localThis.status = res.data;
              break;
            case "GT_DONOR_LIST":
              localThis.donors = res.data;
              break;
            case "GT_PROJECT_LIST":
              localThis.projects = res.data;
              break;
            case "CURRENCY_LIST":
              localThis.currency = res.data;
              break;
          }
          //obj = res.data;
          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch((err) => {
          // console.error(err);
          return [];
        })
        .finally(() => (localThis.isLoading = false));
    },
  },
});

    console.log("hounikkk");
</script>

<style scoped>
.Budget {
  background-color: white;
  font-size: 16px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  width: 60%;
  height: 40px;
}
.redBudget {
  background-color: white;
  font-size: 16px;
  border-style: solid;
  border-width: 1px;
  border-color: red;
  width: 60%;
  height: 40px;
}
.redStyle {
  color: red;
}
</style>
