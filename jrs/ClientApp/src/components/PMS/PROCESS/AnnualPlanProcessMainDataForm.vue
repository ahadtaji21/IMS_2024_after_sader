<template>
  <v-card>
    <v-card-title primary-title>{{
      $t("datasource.pms.annual-plan.process")
    }}</v-card-title>
    <v-container fluid>
      <v-form ref="form" lazy-validation class="text-capitalize">
        <v-row v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
            <v-text-field
              :label="$t('pms.code')"
              v-model="formData.PMS_LOGIC_MODEL_CODE"
              :counter="150"
              readonly="readonly"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row v-if="false">
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
            <v-text-field
              :label="$t('datasource.pms.annual-plan.objectives.act-type')"
              v-model="formData.ACTIVITY_TYPE_NAME"
              :counter="150"
              readonly="readonly"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-text-field
              :label="$t('pms.description')"
              v-model="formData.DESCRIPTION"
              :counter="150"
              readonly="readonly"
            ></v-text-field>
          </v-col>
        </v-row>
      </v-form>
    </v-container>
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
    // isUpdatableForm: {
    //   type: Boolean,
    //   required: true,
    // },
  },
  data() {
    return {
      readonly: true,
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
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
  },
  beforeMount() {
    let localThis: any = this;

    //   if (localThis.targets.length == undefined || localThis.targets.length == 0)
    //     localThis.getList("PMS_OBJ_OUTCOME_TARGET");
  },
});
</script>

<style scoped></style>
