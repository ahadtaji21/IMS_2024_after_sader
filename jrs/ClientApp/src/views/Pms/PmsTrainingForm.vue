<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row v-if="showFilter">
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-training-form.title") }}
                <v-icon large> mdi-account-reactivate</v-icon>
              </h1>
              <p>{{ $t("views.view-training-form.view-info") }}</p>
            </v-col>
          </v-row>
          <!-- MAIN FILTER -->
          <v-card outlined class="lv-0" dense v-if="showFilter">
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- PROJECT SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="project"
                    :label="$t('views.view-training-form.project-select')"
                    :items="projectList"
                    :hint="$t('views.view-training-form.project-select-hint')"
                    :persistent-hint="true"
                    item-value="PROJECT_ID"
                    item-text="PROJECT_DESCR"
                    @onchange="resetForm"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- SERVICE SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="service"
                    :label="$t('views.view-training-form.service-select')"
                    :hint="$t('views.view-training-form.service-select-hint')"
                    :persistent-hint="true"
                    :disabled="!project.PROJECT_ID"
                    :items="
                      serviceList.filter((item) => item.PROJECT_ID == project.PROJECT_ID)
                    "
                    item-value="SERVICE_ID"
                    item-text="SERVICE_DESCR"
                    @onchange="resetForm"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- ACTIVITY SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="activity"
                    :label="$t('views.view-training-form.activity-select')"
                    :disabled="!service.SERVICE_ID"
                    :hint="$t('views.view-training-form.activity-select-hint')"
                    :persistent-hint="true"
                    :items="
                      activityList.filter((item) => item.SERVICE_ID == service.SERVICE_ID)
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
          <v-row v-if="showProcessDesc">
            <v-col :cols="12">
              <h3>{{ $t("datasource.pms.annual-plan.process") }} :{{ projectDescr }} - {{ processDescr }}</h3>
            </v-col>
          </v-row>
          <!-- DATA ENTRY FOR THE FORM -->
          <v-row v-if="showForm">
            <v-col :cols="12">
              <pms-training-form
                :key="keyForm"
                :processId="activity.PROCESS_ID"
                :selectedTrainingFormId="selectedTrainingFormId"
              ></pms-training-form>
              <!-- :formData="caseVisit"
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
              </jrs-form> -->
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
import TrainingForm from "../../components/PMS/TRAININGFORM/PmsTrainingFormMainData.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import { JrsFormField, JrsFormFieldSearch } from "../../models/JrsForm";

export default Vue.extend({
  components: {
    // "jrs-form": JrsForm,
    "pms-training-form": TrainingForm,
  },
  mixins: [FormMixin],
  props: {
    selectedTrainingFormId: {
      type: Number,
      required: false,
      default: 0,
    },
    projectDescr: {
      type: String,
      required: false,
    },
    processDescr: {
      type: String,
      required: false,
    },
    processId: {
      type: Number,
      required: false,
      default: 0,
    },
  },
  data() {
    return {
      showProcessDesc: false,
      showFilter: true,
      keyForm: 0,
      showForm: false,
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
      localThis.keyForm = Math.ceil(Math.random() * 1000);
      localThis.showForm = true;
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

    resetForm() {
      let localThis: any = this as any;
      localThis.keyForm = Math.ceil(Math.random() * 1000);
      localThis.showForm = false;
    },
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
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getProjectsServiceActivities(localThis.getCurrentOffice.HR_OFFICE_ID);
    if (localThis.processId != undefined && localThis.processId != 0) {
      localThis.activity.PROCESS_ID = localThis.processId;
      localThis.showForm = true;
      localThis.showFilter = false;
      localThis.showProcessDesc = true;
    }
  },
});
</script>

<style scoped></style>
