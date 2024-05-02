<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row align-center v-if="showWait">
            <v-col justify-center :cols="12">
              <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
                <v-progress-circular indeterminate color="primary"></v-progress-circular>
              </div>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-multi-select-table
                v-model="formData.CAT"
                :label="$t('pms.categories')"
                :dataSource="'PMS_VI_COI'"
                :itemValue="'PMS_COI_ID'"
                :itemText="'PMS_COI_DESCRIPTION'"
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
import UtilMix from "../../../mixins/UtilMix";
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
    categoriesAlreadyAdded: {
      type: Array,
      required: true,
    },
  },
  mixins: [UtilMix],
  data() {
    return {
      showWait: false,
      annual_plan_id: 0,
      maxCatNumber:3,
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
      this.$emit("closeDialogCat");
    },

    save: function () {
      let localThis = this as any;
      if (localThis.categoriesAlreadyAdded.length + localThis.formData.CAT.length > localThis.maxCatNumber) {


        let msgDelete: string = this.$i18n
          .t("datasource.pms.annual-plan.objectives.remove-cat")
          .toString().replace("#",localThis.maxCatNumber);
        localThis.showNewSnackbar({
          typeName: "error",
          text: msgDelete,
        }); // Feedback to user

        return;
      } else {
        let msgUpd: string = this.$i18n
          .t("datasource.pms.annual-plan.objectives.confirm-update")
          .toString();
        let id = 0;
        let msg = msgUpd;
        this.$confirm(msg).then(async (res) => {
          if (res) {
            localThis.showWait = true;
            let i: number = 0;
            for (i = 0; i < localThis.formData.CAT.length; i++) {
              let payLoadD: any = {};
              payLoadD.PMS_CATEGORY_REL_ID = 0;
              payLoadD.PMS_COI_ID = localThis.formData.CAT[i].PMS_COI_ID;
              payLoadD.PMS_OBJECTIVE_ID = localThis.formData.selectedOBJId;

              let savePayload: GenericSqlPayload = {
                spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_CAT_OBJ_REL",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(payLoadD),
              };

              try {
                let retvalue = await localThis.synchInsert(savePayload);
                localThis.showWait = false;

                if (
                  typeof retvalue == "string" &&
                  retvalue.indexOf('"status":"error"') != -1
                ) {
                  throw retvalue;
                }
              } catch (error) {
                localThis.showWait = false;
                localThis.$emit("generateError", error);
              }
              // const response = localThis
              //   .execSPCall(savePayload)
              //   .then((res: any) => {
              //     if (res.indexOf('"status":"error"') != -1) {
              //       throw res;
              //     }
              //     return res;
              //   })
              //   .catch((err: any) => {
              //     localThis.showNewSnackbar({
              //       typeName: "error",
              //       text: err.message,
              //     }); // Feedback to user
              //     throw err;
              //   });
            }
            this.$emit("saveDialoge");
          }
        });
      }
    },
    resetOverall() {
      let localThis = this as any;
      localThis.fieldValue = {};
      localThis.formData.CAT = [];
      localThis.dataSourceCondition = "";

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
    localThis.showAdd = true;

    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.annual_plan_id = ap.annal_plan_id;
    localThis.maxCatNumber = ap.max_categories_number;
  
    let i: number = 0;
    let cond: string = "-1,";
    for (i = 0; i < localThis.categoriesAlreadyAdded.length; i++) {
      cond += localThis.categoriesAlreadyAdded[i].PMS_COI_ID;
      cond += ",";
    }
    cond += "-2";
    localThis.dataSourceCondition = " PMS_COI_ID NOT IN (" + cond + ")";
  },
});
</script>

<style scoped></style>
