<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row>
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-case-management.title") }}
                <v-icon large> mdi-account-reactivate</v-icon>
              </h1>
              <p>{{ $t("views.view-case-management.view-info") }}</p>
            </v-col>
          </v-row>
          <!-- MAIN FILTER -->
          <v-card outlined class="lv-0" dense>
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- PROJECT SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="project"
                    :label="$t('views.view-case-management.project-select')"
                    :items="projectList"
                    :hint="$t('views.view-case-management.project-select-hint')"
                    :persistent-hint="true"
                    item-value="PROJECT_ID"
                    item-text="PROJECT_DESCR"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- SERVICE SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="service"
                    :label="$t('views.view-case-management.service-select')"
                    :hint="$t('views.view-case-management.service-select-hint')"
                    :persistent-hint="true"
                    :disabled="!project.PROJECT_ID"
                    :items="
                      serviceList.filter(
                        (item) => item.PROJECT_ID == project.PROJECT_ID
                      )
                    "
                    item-value="SERVICE_ID"
                    item-text="SERVICE_DESCR"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- ACTIVITY SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="activity"
                    :label="$t('views.view-case-management.activity-select')"
                    :disabled="!service.SERVICE_ID"
                    :hint="
                      $t('views.view-case-management.activity-select-hint')
                    "
                    :persistent-hint="true"
                    :items="
                      activityList.filter(
                        (item) => item.SERVICE_ID == service.SERVICE_ID
                      )
                    "
                    item-value="PROCESS_ID"
                    item-text="ACTIVITY_DESCR"
                    return-object
                  ></v-autocomplete>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <v-divider></v-divider>

          <!-- DATA ENTRY FOR THE VISIT (FORM) -->
          <v-row v-if="Object.keys(activity).length > 0">
            <v-col :cols="12">
              <jrs-form
                :formData="caseVisit"
                :formFields="caseVisitFields"
                :key="'ACTIVITY_' + activity.PROCESS_ID"
              >
                <template v-slot:form-actions="{ validateFunc, resetFunc }">
                  <v-btn
                    color="primary"
                    :disabled="isSaving"
                    @click="saveFormDataCaseVisit(caseVisit, 0, validateFunc)"
                    class="ma-2"
                    >{{ $t("general.save") }}</v-btn
                  >
                  <v-btn color="primary" @click="resetFunc()" class="ma-2">{{
                    $t("general.reset")
                  }}</v-btn>
                </template>
              </jrs-form>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsForm from "../../components/JrsForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import { JrsFormField, JrsFormFieldSearch } from "../../models/JrsForm";

export default Vue.extend({
  components: {
    "jrs-form": JrsForm,
  },
  mixins: [FormMixin],
  data() {
    return {
      projectList: [],
      project: {},
      activityList: [],
      activity: {},
      serviceList: [],
      service: {},
      casevisit_crud_info: {},
      caseVisitFields: [],
      caseVisit: {},
      distribution: {},
      isSaving: false,
    };
  },
  watch: {
    activity() {
      let localThis: any = this as any;
      // get structure of view for form of case visit
      localThis.getFormStruct(
        "PMS_VI_CASE_VISIT",
        "caseVisitFields",
        "casevisit_crud_info"
      );
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
      getUserUid: "getUserUid",
    }),
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),
    /**
     * Return a list of active projects for the provided user and office.
     */
    getProjectsServiceActivities(officeId: number) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_PROJECT_SERVICE_PROCESS_FOR_USER",
        officeId: officeId,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.item_count > 0) {
            // Setup projectList
            let projList = res.table_data.map((row: any) => {
              return {
                PROJECT_ID: row.PROJECT_ID,
                PROJECT_DESCR: row.PROJECT_FULL_DESCR,
              };
            });
            // Setup serviceList
            let servList = res.table_data.map((row: any) => {
              return {
                PROJECT_ID: row.PROJECT_ID,
                SERVICE_ID: row.SERVICE_ID,
                SERVICE_DESCR: row.SERVICE_FULL_DESCR,
              };
            });
            // Setup activityList
            let actList = res.table_data.map((row: any) => {
              return {
                SERVICE_ID: row.SERVICE_ID,
                PROCESS_ID: row.PROCESS_ID,
                ACTIVITY_TYPE_ID: row.ACTIVITY_TYPE_ID,
                ACTIVITY_DESCR: row.PROCESS_FULL_DESCR,
                IS_EDUCATION: row.IS_EDUCATION,
                IS_DISTRIBUTION: row.IS_DISTRIBUTION,
                IS_WELFARE: row.IS_WELFARE,
              };
            });

            localThis.projectList = [...projList];
            localThis.serviceList = [...servList];
            localThis.activityList = [...actList];
          }
        });
    },
    /**
     * Get structure of form for case visit (view).
     */
    getFormStruct(
      viewName: string,
      formStructPropName: string,
      crudInfoPropName: string
    ) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: viewName,
        colList: null,
        whereCond: null,
        orderStmt: null,
      };

      localThis
        .getJRSTableStructure({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Setup CRUD info
          if (res) {
            Object.assign(localThis[crudInfoPropName], res.crud_info);

            // Setup Form Fields
            let tmpFormFileds: Array<any> = res.columns
              .filter((header: any) => !header.hide_in_form)
              .map((field: any) => localThis.parseJrsFormField(field));

            localThis[formStructPropName] =
              localThis.setupSelectFields(tmpFormFileds);
            if (formStructPropName == "caseVisitFields") {
              let condition = `EXISTS((SELECT 1 from PMS_VI_PROCESS_FOR_USER WHERE PROCESS_ID = ${localThis.activity.PROCESS_ID} AND JRS_EMPLOYEE = IMS_USER_UID))`;
              localThis[formStructPropName].find(
                (field: JrsFormField) =>
                  field.name == "CASE_VISIT_OPERATOR_USER_UID"
              ).dataSourceCondition = condition;
            }
          }

          // populate The operator Uid with the current user. Otherwise null.
          // If the user is not available into table of serach table, it will not appear
          localThis.caseVisit = {
            CASE_VISIT_OPERATOR_USER_UID: localThis.getUserUid,
          };
        });
    },
    /**
     * Save form data for the case visit.
     */
    saveFormDataCaseVisit(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      // Check for validity
      if (!formValidateFunc()) {
        return null;
      }

      let localThis: any = this as any;
      localThis.isSaving = true;
      let spName: string = "PMS_SP_INS_UPD_CASE_VISIT";

      //Add external data to payload : process_id and process_type_id (If the process_type_id is null, then it will computed by SP )
      saveData = {
        process_id: localThis.activity.PROCESS_ID,
        process_type_id: localThis.activity.ACTIVITY_TYPE_ID,
        rows: saveData,
      };
      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: actionType,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: res.status,
            text: res.message,
          });

          if (formResetFunc) {
            formResetFunc();
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getProjectsServiceActivities(
      localThis.getCurrentOffice.HR_OFFICE_ID
    );
  },
});
</script>

<style scoped>
</style>