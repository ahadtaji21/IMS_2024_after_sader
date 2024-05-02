<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-multi-select-table
                v-model="categoriesAlreadyAdded"
                :label="$t('pms.categories')"
                :dataSource="'PMS_VI_MTX_COI'"
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
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-multi-select-table
                v-model="codeAlreadyAdded"
                :label="$t('pms.code')"
                :dataSource="'PMS_MTX_CODE'"
                :itemValue="'PMS_MTX_CODE_ID'"
                :itemText="'PMS_MTX_CODE_VALUE'"
                :key="keyTblCode"
                :allowMultiselect="true"
                :required="true"
                :returnOnlyIds="false"
                :dataSourceCondition="dataSourceConditionCode"
                :rowDisableSelectRules="rowDisableSelectRules"
              ></jrs-multi-select-table>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="dimensions"
                :label="$t('indicators.Dimension')"
                item-text="PMS_DIMENSION_DESC"
                item-value="PMS_DIMENSION_ID"
                v-model="formData.Dimension"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="target.filter((c) => c.PMS_DIMENSION_ID == formData.Dimension)"
                :label="$t('indicators.Target')"
                item-text="PMS_TARGET_DESC"
                item-value="PMS_TARGET_ID"
                v-model="formData.TargetId"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('indicators.Construct_Definition')"
                v-model="formData.Construct_definition"
                :counter="150"
                required
                :readonly="onlyRead"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="processes"
                :label="$t('indicators.Process')"
                item-text="PROCESS"
                item-value="PROCESS"
                v-model="formData.Process"
              ></v-select>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="services"
                :label="$t('indicators.Service')"
                item-text="PMS_TOS_DESCRIPTION"
                item-value="PMS_TOS_DESCRIPTION"
                v-model="formData.Service"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('indicators.Indicator')"
                v-model="formData.Indicator"
                :counter="150"
                required
                :readonly="onlyRead"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-multi-select-table
                v-model="techAlreadyAdded"
                :label="$t('pms.indicator.techniques')"
                :dataSource="'PMS_MTX_TECHNIQUE'"
                :itemValue="'PMS_MTX_TECHNIQUE_ID'"
                :itemText="'PMS_MTX_TECHNIQUE_DESC'"
                :key="keyTblTech"
                :allowMultiselect="false"
                :required="true"
                :returnOnlyIds="false"
                :dataSourceCondition="dataSourceConditionTech"
                :rowDisableSelectRules="rowDisableSelectRules"
              ></jrs-multi-select-table>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" v-if="!onlyRead" class="--darken-1" @click="close()"
        >Cancel</v-btn
      >
      <v-btn color="primary" v-if="!onlyRead" class="--darken-1" @click="save()"
        >Save</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsMultiSelectTable from "../../JrsMultiSelectedTable.vue";
import { i18n } from "../../../i18n";

import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi";
export default Vue.extend({
  components: {
    "jrs-multi-select-table": JrsMultiSelectTable,
  },

  props: {
    selectedIndicatorId: {
      type: Number,
      required: false,
      default: 0,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      keyTblCode: 0,
      keyTbl: 0,
      keyTblTech: 0,
      dimensions: [],
      target: [],
      services: [],
      processes: [],
      showWait: false,
      formData: {} as any,
      categoryList: [],
      formIsValid: false,
      isLoading: false,
      dataSourceOrder: "",
      dataSourceCondition: "",
      dataSourceConditionCode: "",
      dataSourceConditionTech: "",
      rowDisableSelectRules: [],
      categoriesAlreadyAdded: [],
      techAlreadyAdded: [],
      codeAlreadyAdded: [],
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
    localThis.getIndicatorCategories();
    if (localThis.categoryList.length == undefined || localThis.categoryList.length == 0)
      localThis.getList("categoryList");
    // if (localThis.selectedIndicatorId == 0) {
    //   localThis.getIndicatorDimension();
    // }
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
      this.$emit("closeDialogeImsInd");
    },

    save() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-upd-indicator")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add-indicator")
        .toString();

      let payload: any = {};
      payload.categories = localThis.categoriesAlreadyAdded;
      payload.Code = localThis.codeAlreadyAdded;
      payload.dimension = localThis.dimensions.filter(
        (c: any) => c.PMS_DIMENSION_ID == localThis.formData.Dimension
      )[0].PMS_DIMENSION_DESC;
      payload.target = localThis.target.filter(
        (c: any) => c.PMS_TARGET_ID == localThis.formData.TargetId
      )[0].PMS_TARGET_DESC;
      payload.constructionDef = localThis.formData.Construct_definition;
      payload.Processes = localThis.formData.Process;
      payload.Service = localThis.formData.Service;
      payload.Indicator = localThis.formData.Indicator;
      payload.Techniques = localThis.techAlreadyAdded;
      payload.ID = localThis.formData.id;
      let id = 0;
      let msg = msgUpd;
      if (localThis.formData.id == undefined || localThis.formData.id == 0) {
        msg = msgAdd;
      }

      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_MTX_INDICATOR",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payload),
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              this.$emit("refreshAct", localThis.formData);
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.$emit("closeDialogeAndSave", localThis.formData);
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
    getIndicatorCategories() {
      let localThis = this as any;
      localThis.categoriesAlreadyAdded = [];
      localThis.showWait = true;
      let view: string = "PMS_VI_MTX_CATEGORY";
      let wherecond: string = `ID = ${localThis.selectedIndicatorId}`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_COI_ID = item.PMS_COI_ID + "";
              itm.PMS_COI_CODE = item.PMS_COI_CODE + "";
              itm.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION;
              localThis.categoriesAlreadyAdded.push(itm);
            });
            localThis.showWait = false;
            localThis.keyTbl = Math.ceil(Math.random() * 1000);
            localThis.getIndicatorCode();
          } else {
            localThis.keyTbl = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getIndicatorCode();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getIndicatorTech() {
      let localThis = this as any;
      localThis.techAlreadyAdded = [];
      localThis.showWait = true;
      let view: string = "PMS_VI_MTX_TECHNIQUE";
      let wherecond: string = `ID = ${localThis.selectedIndicatorId}`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_MTX_TECHNIQUE_ID = item.PMS_MTX_TECHNIQUE_ID + "";
              itm.PMS_MTX_TECHNIQUE_DESC = item.PMS_MTX_TECHNIQUE_DESC + "";
              localThis.techAlreadyAdded.push(itm);
            });
            localThis.showWait = false;
            localThis.keyTblTech = Math.ceil(Math.random() * 1000);
            localThis.getIndicatorDimension();
          } else {
            localThis.keyTblTech = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getIndicatorDimension();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getIndicatorCode() {
      let localThis = this as any;
      localThis.codeAlreadyAdded = [];
      localThis.showWait = true;
      let view: string = "PMS_VI_MTX_CODE";
      let wherecond: string = `ID = ${localThis.selectedIndicatorId}`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_MTX_CODE_ID = item.PMS_MTX_CODE_ID + "";
              itm.PMS_MTX_CODE_VALUE = item.PMS_MTX_CODE_VALUE + "";
              localThis.codeAlreadyAdded.push(itm);
            });
            localThis.showWait = false;
            localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.getIndicatorTech();
          } else {
            localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getIndicatorTech();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getIndicatorDimension() {
      let localThis = this as any;
      localThis.dimensions = [];
      localThis.showWait = true;
      let view: string = "PMS_MTX_DIMENSION";
      let wherecond: null;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: null,
        orderStmt: "PMS_DIMENSION_DESC",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_DIMENSION_ID = item.PMS_DIMENSION_ID + "";
              itm.PMS_DIMENSION_DESC = item.PMS_DIMENSION_DESC + "";
              localThis.dimensions.push(itm);
            });
            localThis.showWait = false;
            localThis.getTarget();

            //localThis.keyTblCode = Math.ceil(Math.random() * 1000);
          } else {
            // localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getTarget();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    getTarget() {
      let localThis = this as any;
      localThis.target = [];
      localThis.showWait = true;
      let view: string = "PMS_MTX_TARGET";
      let wherecond: null;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: null,
        orderStmt: "PMS_TARGET_DESC",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_DIMENSION_ID = item.PMS_DIMENSION_ID;
              itm.PMS_TARGET_DESC = item.PMS_TARGET_DESC;
              itm.PMS_TARGET_ID = item.PMS_TARGET_ID;
              localThis.target.push(itm);
            });
            localThis.showWait = false;
            localThis.getProcesses();

            //localThis.keyTblCode = Math.ceil(Math.random() * 1000);
          } else {
            // localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getProcesses();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    getProcesses() {
      let localThis = this as any;
      localThis.processes = [];
      localThis.showWait = true;
      let view: string = "PMS_MTX_PROCESS";
      let wherecond: null;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: null,
        orderStmt: "PROCESS",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PROCESS = item.PROCESS + "";
              itm.PROCESS_ID = item.PROCESS_ID + "";
              localThis.processes.push(itm);
            });
            localThis.showWait = false;
            localThis.getServices();

            //localThis.keyTblCode = Math.ceil(Math.random() * 1000);
          } else {
            // localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getServices();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getServices() {
      let localThis = this as any;
      localThis.services = [];
      localThis.showWait = true;
      let view: string = "PMS_TYPE_OF_SERVICE";
      let wherecond: null;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: "PMS_TOS_ENABLED=1 AND PMS_TOS_DELETED=0",
        orderStmt: "PMS_TOS_DESCRIPTION",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_TOS_ID = item.PMS_TOS_ID + "";
              itm.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION + "";
              localThis.services.push(itm);
            });
            localThis.showWait = false;
            localThis.getIndicatorData();

            //localThis.keyTblCode = Math.ceil(Math.random() * 1000);
          } else {
            // localThis.keyTblCode = Math.ceil(Math.random() * 1000);

            localThis.showWait = false;
            localThis.getIndicatorData();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getIndicatorData() {
      let localThis = this as any;
      let indData = [] as any;
      localThis.showWait = true;
      let view: string = "PMS_MTX_MATRIX";
      let wherecond: string = `ID = ${localThis.selectedIndicatorId}`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: wherecond,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              indData.push(item);
              let fd = {} as any;
              fd.Category = indData[0].Category;

              fd.Code = indData[0].Code;
              fd.Dimension = localThis.dimensions.filter(
                (c: any) => c.PMS_DIMENSION_DESC == indData[0].Dimension
              )[0].PMS_DIMENSION_ID;
              fd.Target = indData[0].Target;
              fd.TargetId = localThis.target.filter(
                (c: any) =>
                  c.PMS_DIMENSION_ID == fd.Dimension && c.PMS_TARGET_DESC == fd.Target
              )[0].PMS_TARGET_ID;
              localThis.trg = indData[0].Target;
              fd.Construct_definition = indData[0].Construct_definition;
              fd.Service = indData[0].Service;
              fd.Process = indData[0].Process;
              fd.Output = indData[0].Output;
              fd.Question = indData[0].Question;
              fd.id_Indicator = indData[0].id_Indicator;
              fd.Indicator = indData[0].Indicator;
              fd.Standard = indData[0].Standard;
              fd.Additional_information = indData[0].Additional_information;
              fd.Periodicity = indData[0].Periodicity;
              fd.Disaggregated = indData[0].Disaggregated;
              fd.Technique = indData[0].Technique;
              fd.M_E = indData[0].M_E;
              fd.Dashboard = indData[0].Dashboard;
              fd.Technique_question = indData[0].Technique_question;
              fd.Methodological_strategy = indData[0].Methodological_strategy;
              fd.Phase_of_implementation = indData[0].Phase_of_implementation;
              fd.Collection_type = indData[0].Collection_type;
              fd.id = indData[0].id;
              localThis.formData = Object.assign({}, localThis.formData, fd);
              localThis.keyTrg = Math.ceil(Math.random() * 1000);
              localThis.showWait = false;
            });
          } else {
            localThis.showWait = false;
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      localThis.showWait = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          localThis.categoryList = res.data;
          return res;
        })
        .catch((err) => {
          // console.error(err);
          return [];
        })
        .finally(() => (localThis.showWait = false));
    },
  },
  beforeMount() {
    let localThis: any = this;

    let ap = {} as any;
    ap = localThis.$store.getters.getAnnualPlan;
    localThis.annual_plan_id = ap.annal_plan_id;
    let i: number = 0;
    let cond: string = "-1,";
    for (i = 0; i < localThis.categoriesAlreadyAdded.length; i++) {
      cond += localThis.categoriesAlreadyAdded[i].PMS_COI_ID;
      cond += ",";
    }
    cond += "-2";
    localThis.dataSourceCondition = " PMS_COI_ID NOT IN (" + cond + ")";

    cond = "-1,";
    for (i = 0; i < localThis.codeAlreadyAdded.length; i++) {
      cond += localThis.codeAlreadyAdded[i].PMS_MTX_CODE_ID;
      cond += ",";
    }
    cond += "-2";
    localThis.dataSourceConditionCode = " PMS_MTX_CODE_ID NOT IN (" + cond + ")";

    cond = "-1,";
    for (i = 0; i < localThis.techAlreadyAdded.length; i++) {
      cond += localThis.techAlreadyAdded[i].PMS_MTX_TECHNIQUE_ID;
      cond += ",";
    }
    cond += "-2";
    localThis.dataSourceConditionTech = " PMS_MTX_TECHNIQUE_ID NOT IN (" + cond + ")";
  },
});
</script>

<style scoped></style>
