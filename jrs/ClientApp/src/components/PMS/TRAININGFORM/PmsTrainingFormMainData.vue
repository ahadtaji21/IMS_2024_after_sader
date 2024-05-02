<template>
  <v-card>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card-text>
      <v-form>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-text-field
              v-model="formData.TRAINER_NAME"
              :label="$t('views.view-training-form.trainer-name')"
              hint=""
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-text-field
              v-model="formData.TRAIN_TITLE"
              :label="$t('views.view-training-form.train-title')"
              hint=""
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
            <jrs-date-picker
              :label="$t('views.view-training-form.start-date')"
              v-model="formData.START_DATE"
              :hint="$t('views.view-training-form.start-date')"
              :required="false"
            ></jrs-date-picker>
          </v-col>

          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
            <jrs-date-picker
              :label="$t('views.view-training-form.end-date')"
              v-model="formData.END_DATE"
              :hint="$t('views.view-training-form.end-date')"
              :required="false"
            ></jrs-date-picker>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-text-field
              v-model="formData.TOPIC"
              :label="$t('views.view-training-form.topic')"
              hint=""
              :required="false"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-select
              :items="categories"
              :label="$t('views.view-training-form.category')"
              item-text="PMS_COI_DESCRIPTION"
              item-value="PMS_COI_ID"
              v-model="formData.CATEGORY"
            ></v-select>
            <!-- <jrs-multi-select-table
                v-model="formData.CATEGORY"
                :label="$t('views.view-training-form.category')"
                :dataSource="'PMS_VI_CATEGORY_OF_INTERVENTION_TRAINING'"
                :itemValue="'PMS_COI_ID'"
                :dataSourceOrder="'IDX,PMS_COI_DESCRIPTION'"
                :itemText="'PMS_COI_DESCRIPTION'"
                :key="keyTblMlt"
                :allowMultiselect="true"
                :required="true"
                :returnOnlyIds="false"
                :dataSourceCondition="dataSourceCondition"
                :rowDisableSelectRules="rowDisableSelectRules"
              ></jrs-multi-select-table>-->
          </v-col>

          <v-col
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
            v-if="formData.CATEGORY == '0'"
          >
            <v-text-field
              v-model="formData.OTHER_CATEGORY"
              :label="$t('views.view-training-form.other')"
              hint=""
              :required="false"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>

        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-select
              v-model="formData.TARGET"
              :items="targets"
              :label="$t('views.view-training-form.target')"
              item-text="value"
              item-value="value"
            ></v-select>
          </v-col>

          <v-col
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
            v-if="formData.TARGET == 'Other'"
          >
            <v-text-field
              v-model="formData.OTHER_TARGHET"
              :label="$t('views.view-training-form.other-target')"
              hint=""
              :required="false"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>

        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-select
              v-model="formData.TYPE"
              :items="trainingTypes"
              :label="$t('views.view-training-form.types')"
              item-text="value"
              item-value="value"
            ></v-select>
          </v-col>

          <v-col
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
            v-if="formData.TYPE == 'Other'"
          >
            <v-text-field
              v-model="formData.OTHER_TYPE"
              :label="$t('views.view-training-form.other-target')"
              hint=""
              :required="false"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-text-field
              v-model="formData.SESSION_NMB"
              :label="$t('views.view-training-form.num-session')"
              hint=""
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
              type="number"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-text-field
              v-model="formData.ENROLLED_NMB"
              :label="$t('views.view-training-form.num-enrolled')"
              hint=""
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
              type="number"
              @change="calculateDropOput"
            ></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
            <v-text-field
              v-model="formData.ATTENDEE_NMB"
              :label="$t('views.view-training-form.num-attendee')"
              hint=""
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
              type="number"
              @change="calculateDropOput"
            ></v-text-field>
          </v-col>
        </v-row>
      </v-form>
      <v-row>
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
          <v-text-field
            v-model="dropOut"
            :label="$t('views.view-training-form.num-dropout')"
            hint=""
            :required="true"
            :rule="requiredTextFieldRule"
            :readonly="true"
            type="number"
          ></v-text-field>
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-btn color="secondary darken-1" v-if="!onlyRead" text @click="closeEdit()"
        >X {{ $t("general.close") }}</v-btn
      >
      <v-btn color="primary" v-if="!onlyRead" @click="saveFormData()">{{
        $t("general.save")
      }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import {
  ImsApi,
  PmsAnnualPlanApi,
  Configuration,
  ImsLookupsApi,
} from "../../../axiosapi";
import { i18n } from "../../../i18n";
import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { EventBus } from "@/event-bus";

export default Vue.extend({
  components: {
    "jrs-date-picker": JrsDatePicker,
  },

  props: {
    formDataExt: {
      type: Object,
    },
    selectedTrainingFormId: {
      type: Number,
      required: false,
      default: 0,
    },

    processId: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },

    selectedCategoryId: {
      type: Number,
      default: 0,
      required: false,
    },
    cascade: {
      type: Boolean,
      default: false,
      required: false,
    },
  },

  data() {
    return {
      dropOut: 0,
      showWait: false,
      categories: [],
      keyTblMlt: 0,
      module: "",
      formData: {},
      formIsValid: false,
      isLoading: false,
      targets: [],
      trainingTypes: [],
      occ: [],
      requiredTextFieldRule: [(v: any) => !!v || "Title is required"],
      dataSourceCondition: "",
      rowDisableSelectRules: [],
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  watch: {
    // formData(from:any, to: any) {
    //   let localThis = this as any;
    //   debugger;
    // }
  },
  mounted() {
    let localThis = this as any;
    localThis.isLoading = true;
    localThis.getCategories();
  },

  beforeMount() {
    let localThis: any = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    calculateDropOput() {
      let localThis = this as any;
      if (localThis.formData.ENROLLED_NMB == undefined)
        localThis.formData.ENROLLED_NMB = 0;
      if (localThis.formData.ATTENDEE_NMB == undefined)
        localThis.formData.ATTENDEE_NMB = 0;
      localThis.dropOut =
        localThis.formData.ENROLLED_NMB - localThis.formData.ATTENDEE_NMB;
    },

    retrieveData() {
      let localThis = this as any;
      localThis.formData = {};
      localThis.showWait = true;
      let view: string = "PMS_TRAINING_FORM";
      let wherecond: null;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: `ID=${localThis.selectedTrainingFormId}`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.formData.ID = item.ID;
              localThis.formData.PROCESS_ID = item.processId;
              localThis.formData.TRAINER_NAME = item.TRAINER_NAME;
              localThis.formData.TRAIN_TITLE = item.TRAIN_TITLE;
              localThis.formData.START_DATE = item.START_DATE;
              localThis.formData.END_DATE = item.END_DATE;
              localThis.formData.TOPIC = item.TOPIC;
              localThis.formData.CATEGORY = item.CATEGORY;
              localThis.formData.OTHER_CATEGORY = item.OTHER_CATEGORY;
              localThis.formData.TARGET = item.TARGET;
              localThis.formData.OTHER_TARGHET = item.OTHER_TARGHET;
              localThis.formData.TYPE = item.TYPE;
              localThis.formData.OTHER_TYPE = item.OTHER_TYPE;
              localThis.formData.SESSION_NMB = item.SESSION_NMB;
              localThis.formData.ENROLLED_NMB = item.ENROLLED_NMB;
              localThis.formData.ATTENDEE_NMB = item.ATTENDEE_NMB;
              localThis.calculateDropOput();
            });
            localThis.showWait = false;

            //localThis.keyTblCode = Math.ceil(Math.random() * 1000);
          } else {
            // localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    getCategories() {
      let localThis = this as any;
      localThis.categories = [];
      localThis.showWait = true;
      let view: string = "PMS_VI_CATEGORY_OF_INTERVENTION_TRAINING";
      let wherecond: null;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,

        whereCond: `IMS_LANGUAGE_LOCALE='${i18n.locale}' OR IMS_LANGUAGE_LOCALE=''`,
        orderStmt: "IDX,PMS_COI_DESCRIPTION",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as any;
              itm.PMS_COI_ID = item.PMS_COI_ID + "";
              itm.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION + "";
              localThis.categories.push(itm);
            });
            localThis.showWait = false;
            localThis.getListTarget();

            //localThis.keyTblCode = Math.ceil(Math.random() * 1000);
          } else {
            // localThis.keyTblCode = Math.ceil(Math.random() * 1000);
            localThis.showWait = false;
            localThis.getListTarget();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    getListTarget() {
      let localThis = this as any;
      localThis.showWait = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet("TRAINING_TARGET", config.baseOptions)
        .then((res) => {
          localThis.showWait = false;
          localThis.targets = res.data;
          localThis.getListType();
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    getListType() {
      let localThis = this as any;
      localThis.showWait = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet("TRAINING_TYPE", config.baseOptions)
        .then((res) => {
          localThis.showWait = false;
          localThis.trainingTypes = res.data;
          if (
            localThis.selectedTrainingFormId != undefined &&
            localThis.selectedTrainingFormId != 0
          ) {
            localThis.retrieveData();
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    closeEdit() {
      let localThis: any = this as any;
      localThis.editMode = false;
        this.$router.push({
            name: "trainingFormList",
          });
    },

    saveFormData() {
      let localThis = this as any;

      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;

      let msgUpd: string = this.$i18n
        .t("views.view-training-form.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n.t("views.view-training-form.add-form").toString();

      let id = 0;
      let msg = msgUpd;
      let payLoadD = {} as any;
      if (localThis.formData.ID == undefined) {
        msg = msgAdd;
      }
      let payLoad = {} as any;
      payLoad.ID = localThis.formData.ID;
      payLoad.PROCESS_ID = localThis.processId;
      payLoad.TRAINER_NAME = localThis.formData.TRAINER_NAME;
      payLoad.TRAIN_TITLE = localThis.formData.TRAIN_TITLE;
      payLoad.START_DATE = localThis.formData.START_DATE;
      payLoad.END_DATE = localThis.formData.END_DATE;
      payLoad.TOPIC = localThis.formData.TOPIC;
      payLoad.CATEGORY = localThis.formData.CATEGORY;
      payLoad.OTHER_CATEGORY = localThis.formData.OTHER_CATEGORY;
      payLoad.TARGET = localThis.formData.TARGET;
      payLoad.OTHER_TARGHET = localThis.formData.OTHER_TARGHET;
      payLoad.TYPE = localThis.formData.TYPE;
      payLoad.OTHER_TYPE = localThis.formData.OTHER_TYPE;
      payLoad.SESSION_NMB = localThis.formData.SESSION_NMB;
      payLoad.ENROLLED_NMB = localThis.formData.ENROLLED_NMB;
      payLoad.ATTENDEE_NMB = localThis.formData.ATTENDEE_NMB;

      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_TRAINING_FORM_DATA",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoad),
            userUid: localThis.userUid,
            officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
                this.$router.push({
            name: "trainingFormList",
          });
            })
            .catch((err: any) => {
              localThis.formData = {} as any;
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
