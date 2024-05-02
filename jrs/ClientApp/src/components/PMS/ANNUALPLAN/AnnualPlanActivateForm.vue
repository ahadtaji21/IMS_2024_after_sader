<template>
  <v-card>
    <v-card-text>
      <!-- <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>-->
      <v-form>
        <v-row :class="'my-border'" v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 5">
            <h6 style="text-align: left">Actual state:{{ switchLabel }}</h6>
            <v-switch v-model="activateAllAnnualPlan" hide-details outlined dense>
              <template v-slot:label>
                <span class="switchLbl">Activate All Services</span>
              </template>
            </v-switch>
          </v-col>
        </v-row>
        <v-row>
          <v-col> </v-col>
        </v-row>
        <v-text-field
          v-model="formData.ACTIVATION_NOTE"
          :label="$t('general.activation-note')"
          hint=""
          :required="true"
          :rule="requiredTextFieldRule"
          :readonly="onlyRead"
        ></v-text-field>

        <v-row>
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-activation')"
              v-model="formData.ACTIVATION_DATE"
              :hint="$t('general.date-activation')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-btn color="secondary darken-1" v-if="!onlyRead" text @click="closeActivation()"
        >X {{ $t("general.close") }}</v-btn
      >
      <v-btn color="primary" v-if="!onlyRead" @click="activateProject()">{{
        $t("general.activate")
      }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { ImsApi, PmsAnnualPlanApi, Configuration } from "../../../axiosapi";
import { i18n } from "../../../i18n";
import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { EventBus } from "@/event-bus";
interface ServiceData {
  ID: number | null;
  TITLE: String | null;
  COI: {} | null;
  TOS: {} | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  OCCURRENCE: {} | null;
  SERVED_PEOPLE: number | null;
  OBJECTIVE_ID: number | null;
}
interface payLoadData {
  ID: number | null;
  TITLE: String | null;
  COI_TOS_REL_ID: number | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  SERVED_PEOPLE: number | null;
  OCCURRENCE_ID: Date | null;
  OBJECTIVE_ID: number | null;
}
export default Vue.extend({
  components: {
    "jrs-date-picker": JrsDatePicker,
  },

  props: {},
  data() {
    return {
      activateAllAnnualPlan: false,
      onlyRead: false,
      formData: {},
      formIsValid: false,
      coi: [],
      tos: [],
      occ: [],
      requiredTextFieldRule: [(v: any) => !!v || "Title is required"],
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
  },
  watch: {
    // formData(to: any) {
    //   let localThis = this as any;
    //   debugger;
    // }
  },
  mounted() {
    let localThis = this as any;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    closeActivation() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.$emit("closeActDialog");
    },

    activateAllServices() {
      let localThis: any = this as any;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let serviceList = [] as any;
      localThis.selectedService = null;
      localThis.serviceList = [];

      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_SERVICE";
      let wherecond: string = `ANNUAL_PLAN_ID = ${ap.annal_plan_id} AND ACTIVATED=0 AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        orderStmt: "ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              if (item.OC_ID != undefined) localThis.activateService(item.ID, item.OC_ID);
            });
          }
        }
      );
    },

    activateService(ID: number, OC_ID: number) {
      let localThis = this as any;

      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      ap = localThis.$store.getters.getAnnualPlan;
      let ap_start_date = new Date(ap.start_year, 0, 1);
      let ap_end_date = new Date(ap.end_year, 11, 31);
      let activation_date = new Date();
      let stop_date = new Date();
      let id = 0;

      let payLoadD = {} as any;

      payLoadD.ID = ID;
      payLoadD.ACTIVATION_DATE = ap_start_date;
      payLoadD.END_DATE = stop_date;
      payLoadD.ACTIVATION_NOTE = localThis.formData.ACTIVATION_NOTE;
      payLoadD.OCCURRENCE_ID = OC_ID;

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_UPD_SERVICE_ACTIVATION",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          //localThis.$emit("closeActivation", localThis.formData);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    activateProject() {
      let localThis = this as any;
      //TODO:FREEZE ANNUAL PLAN MAKING A COPY
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let activation_date = new Date();

      if (localThis.formData.ACTIVATION_DATE.constructor.name == "Date") {
        activation_date = localThis.formData.ACTIVATION_DATE;
      } else {
        activation_date = new Date(
          localThis.formData.ACTIVATION_DATE.replace(
            /(\d{2})-(\d{2})-(\d{4})/,
            "$2/$1/$3"
          )
        );
      }

      if (activation_date.getFullYear() < ap.start_year) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Activation date before than Annual Plan starting",
        }); //
        return;
      }
      let msg: string = "";
      if (localThis.activateAllAnnualPlan == false)
        msg = this.$i18n.t("general.confirm-project-activation").toString();
      else
        msg = this.$i18n.t("general.confirm-project-activation-all-services").toString();

      let id = 0;

      let payLoadD = {} as any;

      payLoadD.ID = ap.annal_plan_id;
      payLoadD.ACTIVATION_DATE = activation_date;
      payLoadD.ACTIVATION_NOTE = localThis.formData.ACTIVATION_NOTE;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_UPD_ANNUAL_PLAN_ACTIVATION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.$emit("closeActivation", localThis.formData);
              if (localThis.activateAllAnnualPlan == true) {
                localThis.activateAllServices();
              }
            })
            .catch((err: any) => {
              localThis.formData = {} as ServiceData;
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
