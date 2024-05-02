<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row v-if="formDataLoc.id != undefined && formDataLoc.id != '0'">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-btn
                v-if="false"
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="GenerateWord()"
              >
                <v-icon>mdi-file-word</v-icon>Download
              </v-btn>
              <div class="text-center" v-if="showWait">
                <v-progress-circular indeterminate color="primary"></v-progress-circular>
              </div>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <span v-if="formDataLoc.code_id != undefined && formDataLoc.code_id != '0'"
                >{{ $t("datasource.pms.annual-plan.ap-code") }} :
                {{
                  getSelecteOffice(formDataLoc.office_id) +
                  "_" +
                  getSelectedCode(formDataLoc.code_id)
                }}</span
              >
              <span v-else
                >{{ $t("datasource.pms.annual-plan.ap-code") }} :
                {{ getSelecteOffice(formDataLoc.office_id) }}</span
              >
            </v-col>
          </v-row>
          <v-row v-if="module == 'IMP' && actualStatus == '12'">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              {{ "Project not activated" }}
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
              <v-dialog
                v-model="activateMode"
                persistent
                retain-focus
                :scrollable="true"
                :overlay="false"
                :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
                transition="dialog-transition"
              >
                <template v-slot:activator="{ on }" v-if="!onlyRead">
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    v-on="on"
                    @click="activateProject"
                  >
                    <v-icon>mdi-plus-circle-outline</v-icon>{{ $t("general.activate") }}
                  </v-btn>
                </template>
                <v-card>
                  <pms-activate-form
                    @closeActivation="closeActivation"
                    @closeActDialog="closeActDialog"
                  ></pms-activate-form>
                </v-card>
              </v-dialog>
            </v-col>
          </v-row>
          <v-row v-if="module == 'IMP' && actualStatus == '1044'">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              Project activated in date: {{ formDataLoc.activation_date }}
              <br />
              Activation note: {{ formDataLoc.activation_note }}
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="offices"
                :label="$t('datasource.ims.project.office')"
                item-text="text"
                item-value="value"
                v-model="formDataLoc.office_id"
                :readonly="onlyRead || readOnlyOffice"
                v-on:change="resetName()"
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="
                  project_code.filter(
                    (item) => item.PMS_OFFICE_ID == formDataLoc.office_id
                  )
                "
                :label="$t('datasource.pms.annual-plan.ap-project')"
                item-text="PMS_PROJECT_CODE_SHOW"
                item-value="PMS_PROJECT_CODE_ID"
                v-model="formDataLoc.code_id"
                :readonly="onlyRead"
              ></v-select>
              <!-- <v-text-field
                :label="$t('datasource.pms.annual-plan.code')"
                v-model="formDataLoc.code"
                :counter="150"
                :readonly="onlyRead"
                required
              ></v-text-field> -->
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.ap-description')"
                v-model="formDataLoc.descr"
                :counter="150"
                :readonly="onlyRead"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-location-picker
                v-model="formDataLoc.location_id"
                label="Location"
              ></jrs-location-picker>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.start-year')"
                v-model="formDataLoc.start_year"
                :counter="150"
                :readonly="
                  onlyRead || (module == 'IMP' && actualStatus == '1044')
                "
                type="number"
                required
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.end-year')"
                v-model="formDataLoc.end_year"
                :counter="150"
                :readonly="
                  onlyRead || (module == 'IMP' && actualStatus == '1044')
                "
                type="number"
                required
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="status"
                :label="$t('datasource.pms.annual-plan.ap-status-name')"
                item-text="text"
                :readonly="onlyReadStatus"
                item-value="value"
                v-model="actualStatus"
              ></v-select>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formDataLoc.currency_code"
                :items="currency"
                outlined
                :loading="isLoading"
                :search-input.sync="search"
                dense
                :label="$t('datasource.pms.annual-plan.ap-currency')"
                item-text="text"
                item-value="value"
                :readonly="readOnlyCurrency"
                chips
              ></v-autocomplete>
              <div v-if="currencyDiscrepance" :style="'color:red'">
                {{ navCurrency }}
              </div>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <!--  <v-btn
        color="secondary"
        class="--darken-1"
        v-if="!onlyRead"
        @click="close()"
        >Cancel</v-btn
      > -->
      <v-btn
        color="secondary"
        class="--darken-1"
        v-if="formDataLoc.id == undefined || formDataLoc.id == 0"
        @click="close()"
        >Cancel</v-btn
      >
      <v-btn
        color="primary"
        class="--darken-1"
        @click="save()"
        v-if="formDataLoc.id == undefined || formDataLoc.id == 0"
        >Save</v-btn
      >
      <!-- <v-btn color="primary" class="--darken-1" @click="save()" v-if="isUpdatableForm"
        >Save</v-btn
      > -->
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
//import { uuid } from "vue-uuid";
import { mapGetters, mapActions } from "vuex";
import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
  AnnualPlanDocApi,
} from "../../../axiosapi";
import { i18n, translate } from "../../../i18n";
import JrsLocationPicker from "../../JrsLocationPicker.vue";
import ActivateForm from "./AnnualPlanActivateForm.vue";
import { EventBus } from "../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  NavImsApi,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "jrs-location-picker": JrsLocationPicker,
    "pms-activate-form": ActivateForm,
  },

  props: {
    formData: {
      type: Object,
    },
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
    presetVal: {
      type: String,
      required: false,
      default: "",
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      currencyDiscrepance: false,
      navCurrency: "",
      readOnlyOffice: false,
      readOnlyCurrency: false,
      showWait: false,
      activateMode: false,
      nbrOfColumns: 2,
      module: "",
      formDataLoc: {},
      actualStatus: 0,
      onlyReadStatus: true,
      role: 0,
      authorizedRole: false,
      formIsValid: false,
      isLoading: false,
      offices: [],
      project_code: [],
      country: [],
      status: [],
      currency: [],
      search: this.presetVal,
      location_id: 0,
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
      getCurrentRole: "getCurrentRole",
      getActiveModule: "getActiveModule",
    }),
  },

  watch: {
    presetVal: function (val) {
      this.search = val;
    },
  },
  beforeMount() {
    let localThis: any = this;
    localThis.module = localThis.getActiveModule.moduleCode;
    console.log("AnnualPlanMainDataForm.vue: " , module)
    localThis.role = localThis.getCurrentRole.ROLE_ID;
    localThis.formDataLoc = JSON.parse(JSON.stringify(localThis.formData));

    localThis.formDataLoc.code_id = localThis.formDataLoc.code_id + "";
    localThis.actualStatus = localThis.formDataLoc.status_id;

    if (localThis.status.length == undefined || localThis.status.length == 0)
      localThis.getStatusList().then(function () {
        return;
      });
  },
  mounted() {
    let localThis: any = this;

    if (localThis.country.length == undefined || localThis.country.length == 0)
      localThis.getList("COUNTRY_LIST");
    if (localThis.offices.length == undefined || localThis.offices.length == 0)
      localThis.getList("OFFICE_LIST");
    if (localThis.project_code.length == undefined || localThis.project_code.length == 0)
      localThis.getProjectCodeList();
    if (localThis.currency.length == undefined || localThis.currency.length == 0)
      localThis.getList("CURRENCY_LIST");
    if (
      localThis.formDataLoc.office_id == undefined ||
      localThis.formDataLoc.office_id + "" == "" ||
      localThis.formDataLoc.office_id == "0"
    ) {
      localThis.formDataLoc.office_id = localThis.getCurrentOffice.HR_OFFICE_ID + "";
      localThis.formDataLoc.office_code = localThis.getCurrentOffice.HR_OFFICE_CODE;
      localThis.readOnlyOffice = true;
    } else {
      localThis.formDataLoc.office_id = localThis.formDataLoc.office_id + "";
    }

    localThis.getCurrencyCodeFromNav(localThis.formDataLoc.office_code);

    localThis.role = localThis.getCurrentRole.ROLE_ID;
    EventBus.$on("userRoleUpdated", (to: any) => {
      let localThis: any = this as any;
      localThis.authorizedRole = false;
      localThis.status = [];
      localThis.role = localThis.getCurrentRole.ROLE_ID;
      if (localThis.role) {
        localThis.getStatusList();
      } else {
        localThis.authorizedRole = false;
      }
      localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
    });
    EventBus.$on("saveMainDataFromMainSave", (data: any) => {
      localThis.saveFromMain();
    });
  },
  beforeDestroy() {
    EventBus.$off("userRoleUpdated");
    EventBus.$off("saveMainDataFromMainSave");
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    getCurrencyCodeFromNav(office: string) {
      let localThis = this as any;
      localThis.currencyDiscrepance = false;
      localThis.navCurrency = "";
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: NavImsApi = new NavImsApi(config);
      localThis.showWait = true;
      let doc: string;
      return api
        .apiNavImsApiNavDefaultFromCompanyGet(office, config.baseOptions)
        .then((res) => {
          if (
            res.data != undefined &&
            res.data.value != undefined &&
            res.data.value.length > 0
          ) {
            let vle = res.data.value[0] as any;

            if (
              localThis.formDataLoc.currency_code == undefined ||
              localThis.formDataLoc.currency_code + "" == ""
            ) {
              //se la currency non era definita la setto e metto a readonly
              localThis.formDataLoc.currency_code = vle.lcY_Code;
              localThis.showWait = false;
              localThis.readOnlyCurrency = true;
            } else {
              if (
                localThis.formDataLoc.currency_code != undefined &&
                localThis.formDataLoc.currency_code + "" != ""
              ) {
                if (localThis.formDataLoc.currency_code == vle.lcY_Code) {
                  //se la currency era definita ed uguale a quella di nav allora readonly
                  localThis.readOnlyCurrency = true;
                } else {
                  localThis.currencyDiscrepance = true;
                  localThis.navCurrency = "NAV Currency:" + vle.lcY_Code;
                }
              }
            }
          } else {
            localThis.currencyDiscrepance = true;
            localThis.navCurrency = "Currency not present on NAV";
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        })
        .finally(() => {
          localThis.showWait = false;
          localThis.isLoading = false;
        });
    },
    close: function () {
      this.$emit("closeDialoge");
    },
    activateProject() {
      let localThis = this as any;
      localThis.activateMode = true;
    },
    closeActDialog() {
      let localThis = this as any;
      localThis.activateMode = false;
    },
    closeActivation(item: any) {
      let localThis = this as any;
      EventBus.$emit("annualPlanStatusUpdated", "1044");
      localThis.activateMode = false;
      localThis.actualStatus = "1044";
      localThis.status = [];
      localThis.status[0] = {} as any;
      localThis.status[0].text = "Approved";
      localThis.status[0].value = "1044";
      localThis.formDataLoc.activation_date = item.ACTIVATION_DATE;
      localThis.formDataLoc.activatione_note = item.ACTIVATION_NOTE;
      localThis.formDataLoc.status_id = "1044";
    },
    save: function () {
      let localThis = this as any;
      localThis.getLocation().then((res: any) => {
        let i: number;
        let status_name = "";

        if (localThis.checkDate()) {
          for (i = 0; i < localThis.status.length; i++) {
            if (localThis.status[i].value == localThis.actualStatus) {
              status_name = localThis.status[i].text;
              break;
            }
          }

          if (
            localThis.formDataLoc.descr == undefined ||
            localThis.formDataLoc.descr == "" ||
            localThis.formDataLoc.code_id == undefined ||
            localThis.formDataLoc.code_id == "0" ||
            localThis.formDataLoc.office_id == undefined ||
            localThis.formDataLoc.office_id == "0" ||
            status_name == "" ||
            localThis.actualStatus == undefined
          ) {
            localThis.showNewSnackbar({
              typeName: "error",
              text: translate("error.ap-complete-data-error"),
            });
            return;
          } else {
            if (
              localThis.formDataLoc.location_id == undefined ||
              localThis.formDataLoc.location_id == 0 ||
              localThis.formDataLoc.location_id == "0"
            ) {
              localThis.showNewSnackbar({
                typeName: "error",
                text: translate("error.ap-insert-location-error"),
              });
              return;
            }
          }

          localThis.formDataLoc.status_name = status_name;
          localThis.formDataLoc.status_id = localThis.actualStatus;
          localThis.formDataLoc.code = localThis.getSelectedCode(
            localThis.formDataLoc.code_id
          );
          localThis.formDataLoc.project_code =
            localThis.getSelecteOffice(localThis.formDataLoc.office_id) +
            localThis.getSelectedCode(localThis.formDataLoc.code_id);
          localThis.formDataLoc.office_code = localThis.getSelecteOffice(
            localThis.formDataLoc.office_id
          );
          if (localThis.formData != undefined)
            localThis.formDataLoc.guid = localThis.formData.guid;
          localThis.formDataLoc.location = localThis.getSelecteCountry();
          localThis.$emit("closeDialogeAndSaveAnnualPlanMD", localThis.formDataLoc);
        }
      });
    },
    saveFromMain: function () {
      let localThis = this as any;
      localThis.getLocation().then((res: any) => {
        let i: number;
        let status_name = "";

        if (localThis.checkDate()) {
          for (i = 0; i < localThis.status.length; i++) {
            if (localThis.status[i].value == localThis.actualStatus) {
              status_name = localThis.status[i].text;
              break;
            }
          }

          if (
            localThis.formDataLoc.descr == undefined ||
            localThis.formDataLoc.descr == "" ||
            localThis.formDataLoc.code_id == undefined ||
            localThis.formDataLoc.code_id == "0" ||
            localThis.formDataLoc.office_id == undefined ||
            localThis.formDataLoc.office_id == "0" ||
            status_name == "" ||
            localThis.actualStatus == undefined
          ) {
            localThis.showNewSnackbar({
              typeName: "error",
              text: translate("error.ap-complete-data-error"),
            });
            return;
          } else {
            if (
              localThis.formDataLoc.location_id == undefined ||
              localThis.formDataLoc.location_id == 0 ||
              localThis.formDataLoc.location_id == "0"
            ) {
              localThis.showNewSnackbar({
                typeName: "error",
                text: translate("error.ap-insert-location-error"),
              });
              return;
            }
          }
          localThis.formDataLoc.code = localThis.getSelectedCode(
            localThis.formDataLoc.code_id
          );
          localThis.formDataLoc.status_name = status_name;
          localThis.formDataLoc.status_id = localThis.actualStatus;
          localThis.formDataLoc.project_code =
            localThis.getSelecteOffice(localThis.formDataLoc.office_id) +
            localThis.getSelectedCode(localThis.formDataLoc.code_id);
          localThis.formDataLoc.office_code = localThis.getSelecteOffice(
            localThis.formDataLoc.office_id
          );
          if (localThis.formData != undefined)
            localThis.formDataLoc.guid = localThis.formData.guid;
          localThis.formDataLoc.location = localThis.getSelecteCountry();
          localThis.$emit(
            "closeDialogeAndSaveAnnualPlanMDFromMain",
            localThis.formDataLoc
          );
        }
      });
    },
    getLocation() {
      let localThis = this as any;
      localThis.formDataLoc.location_description = "";
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_LOCATION_DESCR",
        colList: "IMS_LOCATION_ID,LOCATION",
        whereCond: `IMS_LOCATION_ID = ${localThis.formDataLoc.location_id}`,

        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            //Setup data
            res.table_data.forEach((item: any) => {
              localThis.formDataLoc.location_description = item.LOCATION;
            });
          }
          localThis.isLoading = false;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
          localThis.showWait = false;
          localThis.isLoading = false;
        });
    },
    checkDate() {
      let localThis = this as any;
      if (
        localThis.formDataLoc.start_year == undefined ||
        localThis.formDataLoc.end_year == undefined ||
        localThis.formDataLoc.start_year < 2000 ||
        localThis.formDataLoc.start_year > 2050 ||
        localThis.formDataLoc.end_year < 2000 ||
        localThis.formDataLoc.end_year > 2050 ||
        Number.parseInt(localThis.formDataLoc.start_year) >
          Number.parseInt(localThis.formDataLoc.end_year)
      ) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: translate("error.ap-date-error"),
        });
        return false;
      }
      return true;
    },
    // GenerateWord() {
    //   let localThis = this as any;
    //   var id: number = 0;
    //   if (localThis.formDataLoc != undefined && localThis.formDataLoc.id != undefined) {
    //     id = localThis.formDataLoc.id;
    //   }
    //   if (id == undefined || id == 0) {
    //     localThis.showNewSnackbar({
    //       typeName: "error",
    //       text: "error generating the document",
    //     });
    //     return;
    //   }
    //   const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
    //   const api: AnnualPlanDocApi = new AnnualPlanDocApi(config);
    //   localThis.showWait = true;
    //   let doc: string;
    //   return api
    //     .apiAnnualPlanDocIdGet(id, localThis.versioned, config.baseOptions)
    //     .then((res) => {
    //       doc = res.data;
    //       const byteCharacters = atob(doc);
    //       const byteNumbers = new Array(byteCharacters.length);
    //       for (let i = 0; i < byteCharacters.length; i++) {
    //         byteNumbers[i] = byteCharacters.charCodeAt(i);
    //       }
    //       const byteArray = new Uint8Array(byteNumbers);
    //       const blob = new Blob([byteArray], { type: "application/msword" });

    //       var link = document.createElement("a");

    //       var url = URL.createObjectURL(blob);
    //       link.setAttribute("href", url);
    //       link.setAttribute("download", "annualplan.doc");
    //       link.style.visibility = "hidden";
    //       document.body.appendChild(link);
    //       link.click();
    //       document.body.removeChild(link);
    //     })
    //     .catch((err) => {
    //       // console.error(err);
    //       return "";
    //     })
    //     .finally(() => (localThis.showWait = false));
    // },
    resetName() {
      let localThis = this as any;
      localThis.formDataLoc.code_id = "0";
    },
    getSelectedLocation(value: string) {
      return "location";
    },
    getSelecteOffice(value: string) {
      let i: number;
      let localThis = this as any;
      for (i = 0; i < localThis.offices.length; i++) {
        if (localThis.offices[i].value == value) return localThis.offices[i].text;
      }
      return "";
    },
    getSelectedCode(value: string) {
      let i: number;
      let localThis = this as any;
      for (i = 0; i < localThis.project_code.length; i++) {
        if (localThis.project_code[i].PMS_PROJECT_CODE_ID == value)
          return localThis.project_code[i].PMS_PROJECT_CODE;
      }
      return "";
    },
    getSelecteCountry(value: string) {
      let localThis = this as any;
      let i: number;
      for (i = 0; i < localThis.offices.length; i++) {
        if (localThis.offices[i].value == value)
          if (localThis.country[i] != null) return localThis.country[i].text;
          else return "";
      }
      return "";
    },
    getStatusList() {
      let localThis: any = this as any;
      localThis.status = [];
      //if (localThis.idObject == 0) return;
      var st_id: number = 0;
      if (
        localThis.formDataLoc != undefined &&
        localThis.formDataLoc.status_id != undefined
      ) {
        st_id = localThis.formDataLoc.status_id;
      }

      var id: number = 0;
      if (localThis.formDataLoc != undefined && localThis.formDataLoc.id != undefined) {
        id = localThis.formDataLoc.id;
      }

      let view: string = "IMS_VI_ANNUAL_PLAN_STATUS_LIST";
      let wherecond: string = `(${id}=0 OR ID=${id}) AND (${localThis.role}=0 OR ROLE_ID=${localThis.role}) AND (${st_id}=0 OR STATUS_ID=${st_id})`;
      if (localThis.versioned > 0) {
        view = "IMS_VI_ANNUAL_PLAN_STATUS_LIST_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            var localStatus: any = [];
            //Setup data
            res.table_data.forEach((item: any) => {
              if (id == 0) {
                if (item.IMS_STATUS_DESCR.indexOf("draft") != -1) {
                  let st: any = {};
                  st.text = item.IMS_STATUS_DESCR;
                  st.value = item.NEXT_STATUS_ID + "";
                  localStatus.push(st);
                }
              } else {
                let st: any = {};
                st.text = item.IMS_STATUS_DESCR;
                st.value = item.NEXT_STATUS_ID + "";
                localStatus.push(st);
              }
            });
            localThis.status = localStatus;
          }
        })
        .then(function () {
          if (localThis.role != 0 && localThis.status.length > 1) {
            localThis.authorizedRole = true;
          } else {
            localThis.authorizedRole = false;
          }
          localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
        });
    },

    getProjectCodeList() {
      let localThis: any = this as any;
      localThis.project_code = [];
      //if (localThis.idObject == 0) return;
      var st_id: number = 0;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_PROJECT_CODE",
        colList: null,
        whereCond: `BLOCKED=(0)`,
        orderStmt: "PMS_PROJECT_CODE",
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          if (res.table_data) {
            var localStatus: any = [];
            //Setup data
            res.table_data.forEach((item: any) => {
              let pc: any = {};
              pc.PMS_PROJECT_CODE_ID = item.PMS_PROJECT_CODE_ID + "";
              pc.PMS_OFFICE_ID = item.PMS_OFFICE_ID + "";
              pc.PMS_PROJECT_CODE = item.PMS_PROJECT_CODE;
              pc.PMS_PROJECT_CODE_SHOW =
                item.PMS_PROJECT_CODE + " - " + item.PMS_PROJECT_CODE_DESC;
              localThis.project_code.push(pc);
            });
          }
        }
      );
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
            case "OFFICE_LIST":
              localThis.offices = res.data;
              break;
            case "AP_STATUS_LIST":
              localThis.status = res.data;
              break;
            case "COUNTRY_LIST":
              localThis.country = res.data;
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
</script>

<style scoped></style>
