<template>
  <v-card>
    <v-container fluid>
      <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
        <v-row v-if="showWait">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
            </div>
          </v-col>
        </v-row>
        <v-row v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator-question')"
              v-model="localFormData.QUESTION"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.indicator')"
              v-model="localFormData.INDICATOR"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <!-- <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.standard')"
              v-model="localFormData.STANDARD"
              
              :readonly="false"
            ></v-text-field>
          </v-col>
        </v-row> -->

        <v-row>
          <v-col :cols="4">
            <v-select
              v-model="localFormData.TARGET_TYPE"
              :items="targetTypes"
              :label="$t('pms.indicator.target_type')"
              item-text="text"
              item-value="value"
            ></v-select>
          </v-col>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 8">
            <v-text-field
              v-model="localFormData.TARGET"
              :label="$t('pms.indicator.target')"
              :readonly="false"
            ></v-text-field>
          </v-col>
        </v-row>
        <!-- <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.periodicity')"
              v-model="localFormData.PERIODICITY"
              
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row> -->

        <!-- <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.disaggregated')"
              v-model="localFormData.DISAGGREGATED"
              
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row> -->
        <v-row>
          <v-col>
            <v-checkbox
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
              :label="$t('pms.indicator.not_disaggregable')"
              v-model="localFormData.NOT_DISAGGREGABLE"
            ></v-checkbox>
          </v-col>
        </v-row>
        <v-row
          v-if="!localFormData.NOT_DISAGGREGABLE"
          style="border: solid; border-color: gainsboro"
        >
          <v-col>
            <v-row>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
                <h4>{{ $t("pms.indicator.disaggregation") }}</h4>
              </v-col>
            </v-row>
            <v-row>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
                <v-text-field
                  :label="$t('pms.indicator.female')"
                  v-model="localFormData.FEMALE"
                  type="number"
                ></v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
                <v-text-field
                  :label="$t('pms.indicator.male')"
                  v-model="localFormData.MALE"
                  type="number"
                ></v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
                <v-text-field
                  :label="$t('pms.indicator.others')"
                  v-model="localFormData.OTHERS"
                  type="number"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="12">
            <v-select
              v-model="localFormData.AGE_RANGE"
              :items="ageRange"
              :label="$t('pms.indicator.age_range')"
              item-text="text"
              item-value="value"
            ></v-select>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.disabled')"
              v-model="localFormData.DISABILITED"
              type="number"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.techniques')"
              v-model="localFormData.TECHNIQUE"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <jrs-date-picker
              :label="$t('general.date-end')"
              v-model="localFormData.END_DATE"
              :hint="$t('general.date-end')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
        </v-row>
        <v-row v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator-estimated-value')"
              v-model="localFormData.VALUE"
              required
            ></v-text-field>
          </v-col>
        </v-row>
      </v-form>
    </v-container>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn v-if="isUpdatableForm" color="primary" class="--darken-1" @click="save()"
        >Save</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { EventBus } from "../../../event-bus";
import Vue from "vue";
import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
} from "../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { i18n } from "../../../i18n";
import { mapActions, mapGetters } from "vuex";
export default Vue.extend({
  components: {
    "jrs-date-picker": JrsDatePicker,
  },

  props: {
    isUpdatableForm: {
      type: Boolean,
      required: false,
      default: true,
    },
    formData: {
      type: Object,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      showWait: true,
      targetTypes: [],
      ageRange: [],
      selectedTarget: "",
      localFormData: {} as any,
      keyTbl: 0,
      formIsValid: false,
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
  },
  watch: {},

  mounted() {
    let localThis = this as any;
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
      this.$emit("closeOutpDetDialoge");
    },
    save: function () {
      let localThis = this as any;
      if (
        // localThis.localFormData.VALUE == undefined ||
        // localThis.localFormData.VALUE == "" ||
        localThis.localFormData.END_DATE == undefined ||
        localThis.localFormData.END_DATE == ""
      ) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Missing END DATE",
        }); //
        return;
      }
      let end_date = null as any;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      if (localThis.localFormData.END_DATE.constructor.name == "Date") {
        end_date = localThis.localFormData.END_DATE;
      } else {
        end_date = new Date(
          localThis.localFormData.END_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
        );
      }
      if (end_date.getFullYear() > ap.end_year) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "End date after than Annual Plan end",
        }); //
        return;
      }

      if (end_date.getFullYear() < ap.start_year) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "End date before than Annual Plan starting",
        }); //
        return;
      }

      // if (
      //   localThis.localFormData.VALUE == undefined ||
      //   localThis.localFormData.VALUE == ""
      // )
      //   return;
      let msg: string = "";

      if (
        localThis.localFormData.outpIndicatorRelId == undefined ||
        localThis.localFormData.outpIndicatorRelId == 0
      ) {
        msg = this.$i18n
          .t("datasource.pms.annual-plan.objectives.add-obj-indicator")
          .toString();
      } else {
        msg = this.$i18n
          .t("datasource.pms.annual-plan.objectives.upd-obj-indicator")
          .toString();
      }

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.updateItem(end_date);
        }
      });
    },

    getList(listName: any) {
      let localThis = this as any;
      this.showWait = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "AGE_RANGE":
              localThis.ageRange = res.data;
              break;
            case "IND_OUTP_TARGET_TYPE":
              localThis.targetTypes = res.data;
              break;
          }

          return res;
        })
        .catch((err) => {
          return [];
        })
        .finally(() => (localThis.showWait = false));
    },

    updateItem(end_date: any) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      payLoadD.STANDARD = localThis.localFormData.STANDARD;
      payLoadD.PMS_PROC_OUTP_REL_ID = localThis.localFormData.procOutpRelId;
      payLoadD.PMS_OUTP_IND_ID = localThis.localFormData.outpIndicatorId;
      payLoadD.VALUE = localThis.localFormData.VALUE;
      payLoadD.END_DATE = end_date;
      payLoadD.TARGET_TYPE = localThis.localFormData.TARGET_TYPE;
      payLoadD.TARGET = localThis.localFormData.TARGET;
      if (localThis.localFormData.NOT_DISAGGREGABLE == false)
        payLoadD.NOT_DISAGGREGABLE = "False";
      else payLoadD.NOT_DISAGGREGABLE = "True";
      payLoadD.MALE = localThis.localFormData.MALE;
      payLoadD.FEMALE = localThis.localFormData.FEMALE;
      payLoadD.OTHERS = localThis.localFormData.OTHERS;
      payLoadD.AGE_RANGE = localThis.localFormData.AGE_RANGE;
      payLoadD.DISABLED = localThis.localFormData.DISABILITED;
      if (localThis.localFormData.outpIndicatorRelId == undefined)
        payLoadD.PMS_OUTP_IND_REL_ID = 0;
      else payLoadD.PMS_OUTP_IND_REL_ID = localThis.localFormData.outpIndicatorRelId;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_OUTPUT_INDICATOR",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          localThis.$emit("closeDialogeAndSaveOutpInd", null);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
  },
  // beforeDestroy() {
  //   EventBus.$off("closeDialogeAndSaveCCPInd");
  // },
  filters: {
    toNmbr: function (str: any) {
      if (str != undefined) {
        return Number.parseInt(str);
      } else return 0;
    },
  },
  beforeMount() {
    let localThis: any = this;
    if (localThis.targetTypes.length == undefined || localThis.targetTypes.length == 0)
      localThis.getList("IND_OUTP_TARGET_TYPE");
    if (localThis.ageRange.length == undefined || localThis.ageRange.length == 0)
      localThis.getList("AGE_RANGE");
  },
});
</script>

<style scoped></style>
