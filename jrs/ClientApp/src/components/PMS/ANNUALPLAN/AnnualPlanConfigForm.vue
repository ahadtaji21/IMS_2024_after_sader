<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.ap-max-cat-nmb')"
                v-model="maxCatNumber"
                required
                type="number"
              ></v-text-field>
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
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
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
  components: {},

  props: {},
  data() {
    return {
      maxCatNumber: 0,
      apId: 0,
      currencyDiscrepance: false,
      navCurrency: "",
      readOnlyOffice: false,
      readOnlyCurrency: false,
      showWait: false,
      activateMode: false,
      nbrOfColumns: 2,
      module: "",

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

  beforeMount() {
    let localThis: any = this;
  },
  mounted() {
    let localThis: any = this;
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.maxCatNumber = ap.max_categories_number;
    localThis.apId = ap.annal_plan_id;
    EventBus.$on("saveMainDataFromMainSave", (data: any) => {
      localThis.saveFromMain();
    });
  },
  beforeDestroy() {
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

    close: function () {
      this.$emit("closeDialoge");
    },

    closeActDialog() {
      let localThis = this as any;
      localThis.activateMode = false;
    },

    save: function () {
      let localThis = this as any;
      let locData = {} as any;
      locData.id = localThis.apId;
      locData.maxCatNumber = localThis.maxCatNumber;
      localThis.$emit("closeDialogeAndSaveAnnualPlanCFG", locData);
    },
    saveFromMain: function () {
      let localThis = this as any;

      let locData = {} as any;
      locData.id = localThis.apId;
      locData.maxCatNumber = localThis.maxCatNumber;
      localThis.$emit("closeDialogeAndSaveAnnualPlanCFG", locData);
    },
  },
});
</script>

<style scoped></style>
