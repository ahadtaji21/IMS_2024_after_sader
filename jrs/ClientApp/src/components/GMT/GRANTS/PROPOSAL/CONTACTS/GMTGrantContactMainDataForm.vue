<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" ref="myForm" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.first-name')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_FIRST_NAME"
                :counter="150"
                :rules="rules.required"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.family-name')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_NAME"
                :counter="150"
                :rules="rules.required"
                required
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.gmt.donor.description')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_DESCRIPTION"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.contact-email')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_EMAIL"
                :counter="150"
                :rules="rules.required"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.contact-notes')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_NOTES"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.phone')"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_PHONE"
                :counter="500"
              ></v-text-field>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="genders"
                :label="$t('datasource.gmt.donor.gender')"
                item-text="text"
                item-value="value"
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_GENDER"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-location-picker
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID"
                :requiredCity="false"
                :requiredAddress="false"
                :requiredState="false"
                label="Location"
                :key="Math.ceil(Math.random() * 1000)"
              ></jrs-location-picker>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE"
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
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_CURRENCY_CODE"
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
                v-model="localFormData.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE"
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
      <!-- <v-btn color="tertiary" class="--darken-1" @click="Delete()">Delete</v-btn> -->
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
} from "../../../../../axiosapi";
import { i18n } from "../../../../../i18n";
import JrsLocationPicker from "../../../../JrsLocationPicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "jrs-location-picker": JrsLocationPicker,
  },

  props: {
    formData: {
      type: Object,
      required: false,
    },
    grantId: {
      type: Number,
      required: true,
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
  },
  data() {
    return {
      localFormData: {},
      genders: [],
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
    if (localThis.genders.length == undefined || localThis.genders.length == 0)
      localThis.getList("GENDER");
  },
  mounted() {
    let localThis = this as any;
    localThis.formValid = false;
    localThis.localFormData = JSON.parse(JSON.stringify(localThis.formData));
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
      this.$emit("closeContactDialoge");
    },
    Delete() {
      let localThis = this as any;
      let msg: string = this.$i18n
        .t("datasource.gmt.donor.donor-grant-contact-delete")
        .toString();
      this.$confirm(msg).then((res) => {
        if (res) {
          this.$emit("closeContactDialogeAndSave", null);
        }
      });
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
            case "GENDER":
              localThis.genders = res.data;
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

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then((res) => {
        if (res) {
          localThis.saveContact();
        }
      });
    },
    saveContact: function () {
      let localThis = this as any;

      let id = 0;

      if (localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID == undefined) {
        //msg = msgAdd;
      } else {
        id = Number(localThis.localFormData.GMT_GRANT_DONOR_CONTACT_ID);
      }

      let payLoadD = {} as any;
      payLoadD.ID = localThis.grantId;
      payLoadD.GMT_GRANT_DONOR_CONTACT_FIRST_NAME =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_FIRST_NAME;
      payLoadD.GMT_GRANT_DONOR_CONTACT_NAME =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NAME;
      payLoadD.GMT_GRANT_DONOR_CONTACT_ID = id;
      payLoadD.GMT_GRANT_DONOR_CONTACT_DESCRIPTION =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_DESCRIPTION;
      payLoadD.GMT_GRANT_DONOR_CONTACT_EMAIL =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_EMAIL;
      payLoadD.GMT_GRANT_DONOR_CONTACT_NOTES =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_NOTES;
      payLoadD.GMT_GRANT_DONOR_CONTACT_PHONE =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PHONE;
      payLoadD.GMT_GRANT_DONOR_CONTACT_GENDER =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_GENDER;
      payLoadD.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_LOCATION_COVERAGE_ID;
      payLoadD.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_PROPOSAL_LANGUAGE;
      payLoadD.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE =
        localThis.localFormData.GMT_GRANT_DONOR_CONTACT_SECTOR_COVERAGE;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_UPD_GRANT_CONTACT",
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

          this.$emit("closeContactDialogeAndSave");
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
  },
});
</script>

<style scoped></style>
