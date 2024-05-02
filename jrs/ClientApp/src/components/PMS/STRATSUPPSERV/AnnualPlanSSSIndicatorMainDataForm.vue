<template>
  <v-card>
    <v-container fluid>
      <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
        <v-row v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator-question')"
              v-model="localFormData.QUESTION"
              :counter="150"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.indicator')"
              v-model="localFormData.INDICATOR"
              :counter="150"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row  v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.standard')"
              v-model="localFormData.STANDARD"
              :counter="150"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row  v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.periodicity')"
              v-model="localFormData.PERIODICITY"
              :counter="150"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row  v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.disaggregated')"
              v-model="localFormData.DISAGGREGATED"
              :counter="150"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator.technique')"
              v-model="localFormData.TECHNIQUE"
              :counter="150"
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
        <v-row  v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator-estimated-value')"
              v-model="localFormData.VALUE"
              :counter="150"
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
      localFormData: {},
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
      )
        return;
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
        localThis.localFormData.sssIndicatorRelId == undefined ||
        localThis.localFormData.sssIndicatorRelId == 0
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
    updateItem(end_date: Date) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;
      payLoadD.PMS_SSS_ID = localThis.localFormData.sssId;
      payLoadD.END_DATE = end_date;
      payLoadD.PMS_IND_ID = localThis.localFormData.sssIndicatorId;

      payLoadD.VALUE = localThis.localFormData.VALUE;
      if (localThis.localFormData.sssIndicatorRelId == undefined)
        payLoadD.PMS_SSS_IND_REL_ID = 0;
      else payLoadD.PMS_SSS_IND_REL_ID = localThis.localFormData.sssIndicatorRelId;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_SSS_INDICATOR",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          EventBus.$emit("closeDialogeAndSaveSSSInd", null);
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
  //   EventBus.$off("closeDialogeAndSaveSSSInd");
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
  },
});
</script>

<style scoped></style>
