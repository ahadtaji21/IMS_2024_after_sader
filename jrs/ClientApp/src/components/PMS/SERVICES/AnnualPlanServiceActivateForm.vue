<template>
  <v-card>
    <v-card-text>
      <!-- <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>-->
      <v-form>
        <v-row>
          <!-- OCCURRENCE< -->
          <v-col>
            <v-autocomplete
              :label="$t('datasource.pms.annual-plan.objectives.occurrence')"
              id="OCC"
              :readonly="onlyRead"
              v-model="formData.OCCURRENCE"
              :hint="$t('datasource.pms.annual-plan.objectives.occurrence')"
              persistent-hint
              required
              return-object
              :items="occ"
              item-value="ID"
              item-text="TYPE"
            ></v-autocomplete>
          </v-col>
        </v-row>
        <!-- <v-row>
          <v-col>
            <v-text-field
              v-model="formData.OCCURENCE"
              :label="$t('datasource.pms.annual-plan.objectives.occurrence')"
              :hint="$t('datasource.pms.annual-plan.objectives.occurrence')"
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="true"
            ></v-text-field>
          </v-col>
        </v-row> -->
        <v-row>
          <v-col>
            <v-text-field
              v-model="formData.ACTIVATION_NOTE"
              :label="$t('general.activation-note')"
              :hint="$t('general.activation-note')"
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-activation')"
              v-model="date_from"
              :hint="$t('general.date-activation')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-end')"
              v-model="date_to"
              :hint="$t('general.date-end')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-btn color="secondary darken-1" text @click="closeActivation()"
        >X {{ $t("general.close") }}</v-btn
      >
      <v-btn color="primary" v-if="!onlyRead" @click="activateService()">{{
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

  props: {
    formDataInput: {
      type: Object,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      date_from: null,
      date_to: null,
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
    // },
  },
  beforeMount() {
    let localThis = this as any;
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.formData.ID = localThis.formDataInput.ID;
    let ap_start_date = new Date(ap.start_year, 0, 1);
    let ap_end_date = new Date(ap.end_year, 11, 31);

    //localThis.formData.ACTIVATION_DATE = localThis.formDataInput.START_DATE;
    // if (localThis.formDataInput.START_DATE.constructor.name == "Date") {
    //   localThis.date_from = localThis.formData.DATE_FROM;
    // } else {
    //   localThis.date_from = new Date(
    //     localThis.formDataInput.START_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
    //   );
    // }
    localThis.date_from = ap_start_date;
    localThis.date_to = ap_end_date;
    localThis.formData.ACTIVATION_DATE = localThis.date_from;

    localThis.formData.ACTIVATION_NOTE = localThis.formDataInput.ACTIVATION_NOTE;
    if (localThis.formDataInput.OCCURRENCE != undefined) {
      localThis.formData.OCCURRENCE = localThis.formDataInput.OCCURRENCE;
    }
  },
  mounted() {
    let localThis = this as any;
    localThis.getOccurrences();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    getOccurrences() {
      let localThis: any = this as any;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_OCCURRENCE_TYPE",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.occ.push(item);
            });
          }
        }
      );
    },
    closeActivation() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.$emit("closeActDialog");
    },
    activateService() {
      let localThis = this as any;
      if (localThis.formDataInput.ACTIVATED == 1) {
        //SERVICE ALREADY ACTIVATE
        let msg1: string = this.$i18n
          .t("general.confirm-service-already-activated")
          .toString();
        this.$confirm(msg1).then((res) => {
          if (res) {
            localThis.activateService1();
          }
        });
      } else {
        localThis.activateService1();
      }
    },

    activateService1() {
      let localThis = this as any;

      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let activation_date = new Date();
      let stop_date = new Date();

      if (localThis.date_from.constructor.name == "Date") {
        activation_date = localThis.date_from;
      } else {
        activation_date = new Date(
          localThis.date_from.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
        );
      }

      if (activation_date.getFullYear() < ap.start_year) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Activation date before than Annual Plan starting",
        }); //
        return;
      }
      if (localThis.date_to.constructor.name == "Date") {
        stop_date = localThis.date_to;
      } else {
        stop_date = new Date(
          localThis.date_to.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
        );
      }

      if (stop_date.getFullYear() > ap.end_year) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "End Date after than Annual Plan ending",
        }); //
        return;
      }

      if (
        localThis.formData.OCCURRENCE == undefined ||
        localThis.formData.OCCURRENCE.ID == undefined ||
        localThis.formData.OCCURRENCE.ID == 0
      ) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Missing Service Occurrence",
        }); //
        return;
      }

      let msg: string = this.$i18n.t("general.confirm-service-activation").toString();

      let id = 0;

      let payLoadD = {} as any;

      payLoadD.ID = localThis.formData.ID;
      payLoadD.ACTIVATION_DATE = activation_date;
      payLoadD.END_DATE = stop_date;
      payLoadD.ACTIVATION_NOTE = localThis.formData.ACTIVATION_NOTE;
      payLoadD.OCCURRENCE_ID = localThis.formData.OCCURRENCE.ID;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_UPD_SERVICE_ACTIVATION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.$emit("closeActivation", localThis.formData);
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
