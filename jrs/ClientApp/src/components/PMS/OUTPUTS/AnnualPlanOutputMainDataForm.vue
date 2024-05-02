<template>
  <v-card>
    <v-card-title primary-title>{{
      $t("datasource.pms.annual-plan.output")
    }}</v-card-title>
    <v-container fluid>
      <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.description')"
              v-model="formData.selectedOutput"
              :counter="150"
              readonly="readonly"
            ></v-text-field>
          </v-col>
        </v-row>
        <!-- <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.indicator-estimated-value')"
              v-model="formData.value"
              :counter="150"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row> -->
      </v-form>
    </v-container>
    <v-card-actions>
      <v-btn color="primary" v-if="!onlyRead" @click="saveOutput()">{{
        $t("general.save")
      }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
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
import { EventBus } from "../../../event-bus";
//import JrsMultiSelectTable from "../../../components/JrsMultiSelectedTable.vue";
import { mapActions, mapGetters } from "vuex";
export default Vue.extend({
  components: {
    //"jrs-multi-select-table": JrsMultiSelectTable,
  },

  props: {
    formData: {
      type: Object,
      required: false,
    },
    onlyRead: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      formIsValid: false,
      readonly: false,
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
    EventBus.$on("saveOutputFromMainSave", (data: any) => {
      localThis.$emit("saveOutputMainData", localThis.formData);
    });
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    saveOutput() {
      let localThis = this as any;
      localThis.$emit("saveOutputMainData", localThis.formData);
    },
  },
  beforeMount() {
    let localThis: any = this;

    //   if (localThis.targets.length == undefined || localThis.targets.length == 0)
    //     localThis.getList("PMS_OBJ_OUTCOME_TARGET");
  },
});
</script>

<style scoped></style>
