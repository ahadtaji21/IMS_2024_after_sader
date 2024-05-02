<template>
  <v-card>
    <v-card>
      <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>
      <v-container fluid>
        <v-form v-model="formValid" ref="myForm" lazy-validation class="text-capitalize">
          <v-row v-if="isUpdating">
            <v-col>
              <v-spacer></v-spacer>
              <v-dialog
                v-if="false"
                v-model="cloneMode"
                persistent
                max-width="50%"
                retain-focus
                :scrollable="true"
                :overlay="true"
                transition="dialog-transition"
              >
                <template v-slot:activator="{ on }">
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    v-on="on"
                    @click="OpenClone()"
                  >
                    <v-icon>mdi-content-copy</v-icon>Clone JRS Configuration
                  </v-btn>
                </template>
                <v-card>
                  <v-card-title>
                    <span class="h5">Clone JRS Configuration</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container>
                      <gmt-clone-donor-config
                        :donor_id="formData.GMT_DONOR_ID"
                      ></gmt-clone-donor-config>

                      <template v-slot:form-actions="{ validateFunc }">
                        <v-btn color="secondary darken-1" text @click="closeEdit()"
                          >X {{ $t("general.close") }}</v-btn
                        >
                        <v-btn
                          color="primary"
                          @click="
                            savePositionFunc(
                              formData,
                              formData.PRIMARY_KEY ? 1 : 0,
                              validateFunc
                            )
                          "
                          class="ma-2"
                          >{{ $t("general.save") }}</v-btn
                        >
                      </template>
                    </v-container>
                  </v-card-text>
                  <v-card-actions>
                    <v-btn text color="secondary darken-1" @click="CloseClone()"
                      >X {{ $t("general.close") }}</v-btn
                    >
                  </v-card-actions>
                </v-card>
              </v-dialog>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.name')"
                v-model="formData.GMT_DONOR_NAME"
                :counter="150"
                :rules="rules.required"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.notes')"
                v-model="formData.GMT_DONOR_DESCRIPTION"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row v-if="false">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.nav-id')"
                v-model="formData.GMT_DONOR_NAV_ID"
                :counter="150"
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.salesforce-id')"
                v-model="formData.GMT_DONOR_SALESFORCE_ID"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-location-picker
                v-model="formData.GMT_DONOR_LOCATION_COVERAGE_ID"
                label="Location"
                :requiredCity="false"
                :requiredAddress="false"
                :requiredState="false"
              ></jrs-location-picker>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formData.GMT_DONOR_PROPOSAL_LANGUAGE"
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
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.donor-email')"
                v-model="formData.GMT_DONOR_EMAIL"
                :counter="150"
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.donor.donor-website')"
                v-model="formData.GMT_DONOR_WEBSITE"
                :counter="500"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <div style="displaly: inline">
                <v-select
                  :items="type"
                  :label="$t('datasource.gmt.grant.gt-type')"
                  item-text="text"
                  item-value="value"
                  v-model="formData.GMT_DONOR_TYPE_ID"
                  prepend-icon="mdi-information-outline"
                  @click:prepend="openInfo()"
                ></v-select>
              </div>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formData.GMT_DONOR_CURRENCY_CODE"
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
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" class="--darken-1" @click="save()" v-if="!onlyRead"
        >Save</v-btn
      >
    </v-card-actions>
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
          <span class="h5">Donor Type Info</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <span>
              <table>
                <tr>
                  <td>
                    <b>- Institutional Donors</b> are government and inter-governmental
                    organizations, public trusts, bilateral aid agencies, etc. that
                    systemically give grants.
                    <tr>
                      <td>
                        <b>- Foundation</b> is a non-profit corporation or a charitable
                        trust that makes grants to organizations, institutions, or
                        individuals for charitable purposes."
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <b>- International Organizations:</b> may include NGOs, or other
                        non-governmental organizations. Some very large NGOs have their
                        own grant programs or may contract or sub-grant to local
                        organizations.
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <b>- Multilateral:</b> UN Agencies, World Bank and Asian
                        Development Bank. A multilateral organisation is an international
                        organisation whose membership is made up of member governments,
                        who collectively govern the organisation and are its primary
                        source of funds.
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <b>- Major/Individual Donor:</b> Major donors are supporters who
                        hold the capacity to provide large donations and often hold a
                        personal connection with the organization. Individual donor, a
                        person giving a charity a gift of money, a donation (generally
                        small).
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <b>- Religious Organization:</b> Churches, Faith based
                        Organizations, Jesuit Network, Religious Congregations.
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
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import CloneForm from "./GMTDonorConfigureForm.vue";
import JrsLocationPicker from "../../../JrsLocationPicker.vue";
import FormMixin from "../../../../mixins/FormMixin";
import NavMix from "../../../../mixins/NavMixin";
import UtilMix from "../../../../mixins/UtilMix";
import { mapGetters } from "vuex";
import {
  ImsApi,
  ImsLookupsApi,
  PmsAnnualPlanApi,
  Configuration,
} from "../../../../axiosapi";
import { i18n } from "../../../../i18n";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gmt-clone-donor-config": CloneForm,
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

    isUpdating: {
      type: Boolean,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      showInfo: false,
      type: [],
      authorizedRole: false,
      onlyReadStatus: true,
      showWait: false,
      cloneMode: false,
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
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
    userLocale() {
      return i18n.locale;
    },
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

    localThis.role = localThis.getCurrentRole.ROLE_CODE;
    if (localThis.role == "GM") {
      localThis.authorizedRole = !localThis.onlyRead;
      //localThis.getStatusList();
    } else {
      localThis.authorizedRole = false;
    }
    localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;

    localThis.formValid = false;
    localThis.showWait = true;
    if (localThis.languages.length == undefined || localThis.languages.length == 0)
      localThis.getList("LANGUAGE_LIST");
    if (localThis.currency.length == undefined || localThis.currency.length == 0)
      localThis.getList("CURRENCY_LIST");
    if (localThis.type.length == undefined || localThis.type.length == 0)
      localThis.getList("GMT_DONOR_TYPE");
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
    OpenClone() {
      let localThis = this as any;
      localThis.cloneMode = true;
    },
    CloseClone() {
      let localThis = this as any;
      localThis.cloneMode = false;
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
              localThis.isLoading = false;
              break;
            case "CURRENCY_LIST":
              localThis.currency = res.data;
              localThis.isLoading = false;
              break;
            case "GMT_DONOR_TYPE":
              localThis.type = res.data;
              localThis.isLoading = false;
              break;
          }

          return res;
        })
        .catch((err) => {
          // console.error(err);
          return [];
        })
        .finally(() => (localThis.showWait = false));
    },
    openInfo() {
      let localThis = this as any;
      localThis.showInfo = true;
    },
    closeInfo() {
      let localThis = this as any;
      localThis.showInfo = false;
    },
    save() {
      let localThis = this as any;

      let isValid = true;
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

      if (localThis.formData.GMT_DONOR_ID == undefined) {
        msg = msgAdd;
        localThis.formData.GMT_DONOR_ID = 0;
        localThis.formData.GMT_DONOR_UID = localThis.makeUUID();
      }
      let obj: any;
      obj = localThis.formData;

      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_DONOR",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(obj),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              if (
                res == undefined ||
                res == "" ||
                res.message.split("-").length == 0 ||
                res.message.split("-")[1] == ""
              ) {
                localThis.editMode = false;
                localThis.showNewSnackbar({
                  typeName: "error",
                  text: "error:Donor not inserted",
                }); // Feedback to user
                return;
              }
              let id: string = res.message.split("-")[1];
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message.split("-")[0],
              }); // Feedback to user

              localThis.descriptions = null;
              let revVal = {} as any;
              revVal.GMT_DONOR_ID = id;
              revVal.GMT_DONOR_NAME = localThis.formData.GMT_DONOR_NAME;
              revVal.GMT_DONOR_DESCRIPTION = localThis.formData.GMT_DONOR_DESCRIPTION;

              revVal.GMT_DONOR_NAV_ID = localThis.formData.GMT_DONOR_NAV_ID;
              revVal.GMT_DONOR_SALESFORCE_ID = localThis.formData.GMT_DONOR_SALESFORCE_ID;
              revVal.GMT_DONOR_LOCATION_COVERAGE_ID =
                localThis.formData.GMT_DONOR_LOCATION_COVERAGE_ID;
              revVal.GMT_DONOR_PROPOSAL_LANGUAGE =
                localThis.formData.GMT_DONOR_PROPOSAL_LANGUAGE;
              revVal.GMT_DONOR_EMAIL = localThis.formData.GMT_DONOR_EMAIL;
              revVal.GMT_DONOR_WEBSITE = localThis.formData.GMT_DONOR_WEBSITE;
              revVal.GMT_DONOR_UID = localThis.formData.GMT_DONOR_UID;
              revVal.GMT_DONOR_TYPE_ID = localThis.formData.GMT_DONOR_TYPE_ID;

              localThis.$emit("closeDialogeAndSave", revVal);
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
