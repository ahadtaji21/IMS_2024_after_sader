<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-multi-select-table
                v-model="formData.CCS"
                :label="$t('datasource.pms.annual-plan.objectives.c-c-s')"
                :dataSource="'PMS_VI_CONSTRUCTION_DEF_DIMENSION_CCS'"
                :itemValue="'PMS_CONSTRUCTION_DEF_ID'"
                :itemText="'PMS_CONSTRUCTION_DEF_DESC'"
                :key="keyTbl"
                :allowMultiselect="true"
                :required="true"
                :returnOnlyIds="false"
                :dataSourceCondition="dataSourceCondition"
                :rowDisableSelectRules="rowDisableSelectRules"
              ></jrs-multi-select-table>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions v-if="isUpdatableForm">
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
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
import JrsMultiSelectTable from "../../JrsMultiSelectedTable.vue";
import { mapActions, mapGetters } from "vuex";
export default Vue.extend({
  components: {
    "jrs-multi-select-table": JrsMultiSelectTable,
  },

  props: {
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
    formDataExt: {
      type: Object,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
    alreadyInserted: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      annual_plan_id: 0,

      keyTbl: 0,
      formIsValid: false,

      dataSourceOrder: "",
      dataSourceCondition: "",
      rowDisableSelectRules: [],
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
    close: function () {
      this.$emit("closeDialogCCS");
    },
    save: function () {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let id = 0;
      let msg = msgUpd;
      this.$confirm(msg).then((res) => {
        if (res) {
          let i: number = 0;
          for (i = 0; i < localThis.formData.CCS.length; i++) {
            let payLoadD: any = {};
            payLoadD.ANNUAL_PLAN_ID = localThis.annual_plan_id;
            payLoadD.CCS_ID = localThis.formData.CCS[i].PMS_CONSTRUCTION_DEF_ID;
            payLoadD.PMS_CCS_KEY_ID = localThis.formData.selectedOvgId;

            let savePayload: GenericSqlPayload = {
              spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_CCS",
              actionType: SqlActionType.NUMBER_0,
              jsonPayload: JSON.stringify(payLoadD),
            };
            const response = localThis
              .execSPCall(savePayload)
              .then((res: any) => {
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: res.message,
                });
                return res;
              })
              .catch((err: any) => {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text: err.message,
                }); // Feedback to user
                throw err;
              });
          }
          this.$emit("saveDialoge");
        }
      });
    },
    resetOverall() {
      let localThis = this as any;
      localThis.fieldValue = {};
      localThis.formData.CCS = [];
      localThis.dataSourceCondition = "PMS_DIMENSION='Cross-cutting service'";

      localThis.keyTbl = Math.ceil(Math.random() * 1000);
    },
  },
  filters: {
    toNmbr: function (str: any) {
      if (str != undefined) {
        return Number.parseInt(str);
      } else return 0;
    },
  },
  beforeMount() {
    let localThis: any = this;
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));

    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.annual_plan_id = ap.annal_plan_id;
    let i: number = 0;
    let cond: string = "-1,";
    for (i = 0; i < localThis.alreadyInserted.length; i++) {
      cond += localThis.alreadyInserted[i].PMS_CCS_ID;
      cond += ",";
    }
    cond += "-2";
    localThis.dataSourceCondition =
      "PMS_DIMENSION='Cross-cutting service' AND PMS_CONSTRUCTION_DEF_ID NOT IN (" +
      cond +
      ")";
  },
});
</script>

<style scoped></style>
