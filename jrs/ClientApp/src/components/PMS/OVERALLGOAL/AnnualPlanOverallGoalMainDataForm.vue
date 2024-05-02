<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <!-- <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="targets"
                :label="$t('datasource.pms.annual-plan.objectives.outcome-obj-target')"
                item-text="text"
                item-value="value"
                v-model="formData.PMS_TARGET_ID_STR"
                :rules="[(v) => !!v || 'Item is required']"
                v-on:change="resetOverall()"
              ></v-select>
            </v-col>
          </v-row> -->
          <v-row
            v-if=" 0==0 ||
              formData.PMS_TARGET_ID_STR != undefined && formData.PMS_TARGET_ID_STR != ''
            "
          >
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-multi-select-table
                v-model="formData.OVERALL_GOAL"
                :label="$t('datasource.pms.annual-plan.objectives.overall-goal')"
                :dataSource="'PMS_VI_CONSTRUCTION_DEF_DIMENSION_OVG'"
                :itemValue="'PMS_CONSTRUCTION_DEF_ID'"
                :itemText="'PMS_CONSTRUCTION_DEF_DESC'"
                :key="keyTbl"
                :allowMultiselect="false"
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
  },
  data() {
    return {
      annual_plan_id: 0,

      keyTbl: 0,
      formIsValid: false,
      targets: [],

      dataSourceOrder: "",
      dataSourceCondition: "PMS_TARGET_ID=-1",
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
      this.$emit("closeDialogOVG");
    },
    save: function () {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let id = 0;
      let msg = msgUpd;
      let payLoadD: any = {};
      payLoadD.ANNUAL_PLAN_ID = localThis.annual_plan_id;
      payLoadD.PMS_TARGET_ID = localThis.formData.OVERALL_GOAL[0].PMS_TARGET_ID; //Number.parseInt(localThis.formData.PMS_TARGET_ID_STR);
      payLoadD.OVERALL_GOAL_ID =
        localThis.formData.OVERALL_GOAL[0].PMS_CONSTRUCTION_DEF_ID;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OVERALL_GOAL",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              });
              this.$emit("saveDialoge");
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    resetOverall() {
      let localThis = this as any;
      localThis.fieldValue = {};
      localThis.formData.OVERALL_GOAL = [];
      localThis.dataSourceCondition =
        "PMS_DIMENSION='Overall Goal' and PMS_TARGET_ID=" +
        Number.parseInt(localThis.formData.PMS_TARGET_ID_STR);

      localThis.keyTbl = Math.ceil(Math.random() * 1000);
    },

    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      //this.isLoading = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "PMS_AP_OVG_TARGET":
              localThis.targets = res.data;
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
    if (localThis.targets.length == undefined || localThis.targets.length == 0)
      localThis.getList("PMS_AP_OVG_TARGET");
    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.annual_plan_id = ap.annal_plan_id;

    localThis.dataSourceCondition =
      "PMS_DIMENSION='Overall Goal'"; // and PMS_TARGET_ID=" + localThis.formData.PMS_TARGET_ID_STR;
  },
});
</script>

<style scoped></style>
