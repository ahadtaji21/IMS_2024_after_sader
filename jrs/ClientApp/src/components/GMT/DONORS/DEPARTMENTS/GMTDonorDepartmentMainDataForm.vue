<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" ref="myForm" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.name')"
                v-model="formData.GMT_DONOR_DEPARTMENT_NAME"
                :counter="150"
                :rules="rules.required"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.description')"
                v-model="formData.GMT_DONOR_DEPARTMENT_DESCRIPTION"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.contact-email')"
                v-model="formData.GMT_DONOR_DEPARTMENT_EMAIL"
                :counter="150"
                :rules="rules.required"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.contact-notes')"
                v-model="formData.GMT_DONOR_DEPARTMENT_NOTES"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row v-if="false">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.nav-id')"
                v-model="formData.GMT_DONOR_DEPARTMENT_NAV_ID"
                :counter="500"
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.salesforce-id')"
                v-model="formData.GMT_DONOR_DEPARTMENT_SALESFORCE_ID"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-location-picker
                v-model="formData.GMT_DONOR_DEPARTMENT_LOCATION_COVERAGE_ID"
                :requiredCity="false"
                :requiredAddress="false"
                :requiredState="false"
                label="Location"
              ></jrs-location-picker>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formData.GMT_DONOR_DEPARTMENT_PROPOSAL_LANGUAGE"
                :items="languages"
                outlined
                :loading="isLoading"
                :search-input.sync="search1"
                dense
                :label="$t('datasource.gmt.donor.proposal-language')"
                item-text="text"
                item-value="value"
                chips
              ></v-autocomplete>
            </v-col>
          </v-row>
          <v-row v-if="false">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formData.GMT_DONOR_DEPARTMENT_CURRENCY_CODE"
                :items="currency"
                outlined
                :loading="isLoading"
                :search-input.sync="search"
                dense
                :label="$t('datasource.gmt.grant.gt-currency')"
                item-text="text"
                item-value="value"
                :rules="[(v) => !!v || 'Item is required']"
                chips
              ></v-autocomplete>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6" v-if="false">
              <v-text-field
                :label="$t('datasource.gmt.donor.sector-coverage')"
                v-model="formData.GMT_DONOR_DEPARTMENT_SECTOR_COVERAGE"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import {
  ImsApi,
  ImsLookupsApi,
  PmsAnnualPlanApi,
  Configuration,
} from "../../../../axiosapi";
import { i18n } from "../../../../i18n";
import JrsLocationPicker from "../../../JrsLocationPicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "jrs-location-picker": JrsLocationPicker,
  },

  props: {
    formData: {
      type: Object,
    },
    presetVal: {
      type: String,
      required: false,
      default: "",
    },
    presetVal1: {
      type: String,
      required: false,
      default: "",
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      isLoading: false,
      currency: [],
      languages: [],
      formValid: false,
      search: this.presetVal,
      search1: this.presetVal1,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
    ...mapGetters({
      getUserUid: "getUserUid",
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
  beforeMount() {
    let localThis: any = this;
    localThis.formValid = false;
    if (localThis.languages.length == undefined || localThis.languages.length == 0)
      localThis.getList("LANGUAGE_LIST");
    if (localThis.currency.length == undefined || localThis.currency.length == 0)
      localThis.getList("CURRENCY_LIST");
  },
  mounted() {
    let localThis = this as any;
    localThis.formValid = false;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    close: function () {
      this.$emit("closeDialoge");
    },
    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      localThis.isLoading = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "LANGUAGE_LIST":
              localThis.languages = res.data;
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

    save() {
      let localThis = this as any;

      let isValid = true;
      let actType = SqlActionType.NUMBER_0;
      isValid = (localThis.$refs.myForm as Vue & {
        validate: () => boolean;
      }).validate();
      if (!isValid) return;
      let msgUpd: string = this.$i18n
        .t("datasource.gmt.donor.donor-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.gmt.donor.donor-add-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;

      if (localThis.formData.GMT_DONOR_DEPARTMENT_ID == undefined) {
        msg = msgAdd;
        localThis.formData.GMT_DONOR_DEPARTMENT_ID = 0;
      } else {
        actType = SqlActionType.NUMBER_1;
      }
      let obj: any;
      obj = localThis.formData;
      obj.GMT_DONOR_ID = localThis.selectedObjectId;
      let off = localThis.getCurrentOffice;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_DONOR_DEPARTMENT",
            actionType: actType,
            jsonPayload: JSON.stringify(obj),
            userUid: localThis.getUserUid,
            officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.descriptions = null;

              localThis.$emit("closeDialogeAndSave");
            })
            .catch((err: any) => {
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
  },
});
</script>

<style scoped></style>
