<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row>
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-case-management-list.title") }}
                <v-icon large> mdi-account-search</v-icon>
              </h1>
              <p>{{ $t("views.view-case-management-list.view-info") }}</p>
            </v-col>
          </v-row>

          <!-- MAIN FILTER -->
          <v-card outlined class="lv-0" dense>
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- PROCESS SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="process"
                    :label="
                      $t('views.view-case-management-list.activity-select')
                    "
                    :items="processList"
                    :hint="
                      $t('views.view-case-management-list.activity-select-hint')
                    "
                    :persistent-hint="true"
                    item-value="ACTIVITY_ID"
                    item-text="ACTIVITY_DESCR"
                    return-object
                    clearable
                    @change="
                      getBeneficiaryList();
                      getHouseholdList();
                    "
                  ></v-autocomplete>
                </v-col>
                <!-- BENEFICIARY SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="biodataSelected"
                    :label="
                      $t('views.view-case-management-list.beneficiary-select')
                    "
                    :hint="
                      $t(
                        'views.view-case-management-list.beneficiary-select-hint'
                      )
                    "
                    :persistent-hint="true"
                    :disabled="processList.length > 0 ? false : true"
                    :items="beneficiaryList"
                    item-value="HR_BIODATA_USER_UID"
                    item-text="HR_BIODATA_FULL_NAME"
                    clearable
                    return-object
                    @change="householdSelected = {}"
                  ></v-autocomplete>
                </v-col>
                <!-- HOUSEHOLD SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="householdSelected"
                    :label="
                      $t('views.view-case-management-list.household-select')
                    "
                    :disabled="processList.length > 0 ? false : true"
                    :items="householdList"
                    :hint="
                      $t(
                        'views.view-case-management-list.household-select-hint'
                      )
                    "
                    :persistent-hint="true"
                    item-value="HOUSEHOLD_UID"
                    item-text="HOUSEHOLD_NAME"
                    clearable
                    @change="biodataSelected = {}"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- DATE FROM -->
              </v-row>
              <v-row>
                <v-col :cols="6">
                  <jrs-date-picker
                    :label="$t('views.view-case-management-list.date-from')"
                    :hint="$t('views.view-case-management-list.date-from-hint')"
                    :persistent-hint="true"
                    v-model="dateSelectedFrom"
                  ></jrs-date-picker>
                </v-col>

                <!-- DATE TO -->
                <v-col :cols="6">
                  <jrs-date-picker
                    :label="$t('views.view-case-management-list.date-to')"
                    :hint="$t('views.view-case-management-list.date-to-hint')"
                    :persistent-hint="true"
                    v-model="dateSelectedTo"
                  ></jrs-date-picker>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <v-divider></v-divider>
        </v-col>

        <!-- SIMPLE TABLE CASE VISIT LIST -->
        <v-col :cols="12">
          <jrs-simple-table
            :viewName="'PMS_VI_CASE_VISIT'"
            :savePayload="{
              userUid: getUserUid,
              officeId: getCurrentOffice.HR_OFFICE_ID,
            }"
            :dataSourceOrder="'CASE_VISIT_TITLE, ACTIVITY_DATE'"
            :dataSourceCondition="conditionProcessInstance"
          >
          </jrs-simple-table>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsDatepicker from "../../components/JrsDatePicker.vue";

import FormMixin from "../../mixins/FormMixin";
import { GenericSqlSelectPayload } from "../../axiosapi/api";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsDatePicker from "../../components/JrsDatePicker.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,

    "jrs-date-picker": JrsDatepicker,
  },
  mixins: [FormMixin],
  data() {
    return {
      switch1: false,
      ok: [],
      isSaving: false,
      copyObject: [],
      tabModel: null,
      check: false,
      tables: [
        {
          name: "PMS_VI_CLASS",
          code: "CLASS",
          tab_key: "datasource.hrm.biodata.deleted.title",
        },
      ],

      processList: [],
      process: {},
      beneficiaryList: [],
      householdList: [],
      biodataSelected: {},
      householdSelected: {},
      dateSelectedFrom: null,
      dateSelectedTo: null,
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    /**
     * Return a list of active process for the provided user (The user that have to take in account the process).
     */
    getProcess() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_PROCESS_FOR_USER",
        colList: null,
        orderStmt: null,
        whereCond: `JRS_EMPLOYEE = '${localThis.getUserUid}' AND OFFICE_FILTER_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID} AND HAS_CASE_VISIT = 1`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.item_count > 0) {
            // Setup projectList
            let projList = res.table_data.map((row: any) => {
              return {
                ACTIVITY_ID: row.PROCESS_ID,
                ACTIVITY_DESCR: row.PROCESS_FULL_DESCR,
              };
            });
            localThis.processList = [...projList];
          }
        });
    },

    /**
     * Return a list of beneficiary and household for the provided activity (process) .
     * ONLY CASE VISIT (CASE_VISIT_ID IS NOT NULL => Condition)
     * Selected the ebneficiary , i will see al case visits of specific beneficiary + all case visit done for the
     * household that he belongs it
     */
    getBeneficiaryList() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_BENEFICIARY_FOR_PROCESS_INSTANCE",
        colList: null,
        whereCond: localThis.process
          ? localThis.process.ACTIVITY_ID
            ? `PROCESS_ID = ${localThis.process.ACTIVITY_ID} AND CASE_VISIT_ID IS NOT NULL`
            : `CASE_VISIT_ID IS NOT NULL`
          : `CASE_VISIT_ID IS NOT NULL`,
        orderStmt: null,
      };
      localThis.beneficiaryList = [];
      localThis.biodataSelected = {};

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            // Setup beneficiary List
            let beneList = res.table_data.map((row: any) => {
              return {
                HR_BIODATA_USER_UID: row.HR_BIODATA_USER_UID,
                HR_BIODATA_FULL_NAME: row.HR_BIODATA_FULL_NAME,
                HOUSEHOLD_UID: row.HOUSEHOLD_UID,
                HOUSEHOLD_NAME: row.HOUSEHOLD_NAME,
              };
            });

            localThis.beneficiaryList = [...beneList];
          }
        });
    },

    /**
     * Return a list of household for the provided activity (process) .
     * ONLY CASE VISIT (CASE_VISIT_ID IS NOT NULL => Condition)
     */
    getHouseholdList() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_CASE_VISIT",
        colList: null,
        whereCond: localThis.conditionProcessInstance,
        orderStmt: null,
      };
      localThis.householdList = [];
      localThis.householdSelected = {};

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            // Setup household List

            let househouldList_ = res.table_data.map((row: any) => {
              return {
                HOUSEHOLD_UID: row.CASE_VISIT_HOUSEHOLD_UID
                  ? row.CASE_VISIT_HOUSEHOLD_UID
                  : null,
                HOUSEHOLD_NAME: row.CASE_VISIT_HOUSEHOLD_NAME
                  ? row.CASE_VISIT_HOUSEHOLD_NAME
                  : null,
              };
            });
            localThis.householdList = [
              ...househouldList_.filter(
                (item: any) => item.HOUSEHOLD_UID != null
              ),
            ];
          }
        });
    },
  },

  mounted() {
    let localThis: any = this as any;
    localThis.getProcess();
    localThis.getBeneficiaryList();
    localThis.getHouseholdList();
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),

    /**
     * All conditions of filters :Process/Class/Date From/Date To
     */
    conditionProcessInstance() {
      let localThis: any = this as any;
      let condition: string = "1=1";
      var dateFormatFrom: string = "";
      var dateFormatTo: string = "";
      if (localThis.dateSelectedFrom) {
        let date = new Date(localThis.dateSelectedFrom.toISOString());
        var dd = String(date.getDate()).padStart(2, "0");
        var mm = String(date.getMonth() + 1).padStart(2, "0"); //January is 0!
        var yyyy = date.getFullYear();
        dateFormatFrom = mm + "/" + dd + "/" + yyyy;
      }
      if (localThis.dateSelectedTo) {
        let date_ = new Date(localThis.dateSelectedTo.toISOString());
        var dd_ = String(date_.getDate()).padStart(2, "0");
        var mm_ = String(date_.getMonth() + 1).padStart(2, "0"); //January is 0!
        var yyyy_ = date_.getFullYear();
        dateFormatTo = mm_ + "/" + dd_ + "/" + yyyy_;
      }

      let conditionProcess = localThis.process
        ? localThis.process.ACTIVITY_ID
          ? ` AND ACTIVITY_ID = ${localThis.process.ACTIVITY_ID}`
          : ""
        : "";
      let conditionBeneficiary = localThis.biodataSelected
        ? localThis.biodataSelected.HR_BIODATA_USER_UID
          ? ` AND CASE_VISIT_BENEFICIARY_USER_UID = '${localThis.biodataSelected.HR_BIODATA_USER_UID}'`
          : ""
        : "";

      let conditionHouseholdBen = localThis.biodataSelected
        ? localThis.biodataSelected.HOUSEHOLD_UID
          ? ` OR CASE_VISIT_HOUSEHOLD_UID = '${localThis.biodataSelected.HOUSEHOLD_UID}'`
          : ""
        : "";
      let conditionHousehold = localThis.householdSelected
        ? localThis.householdSelected.HOUSEHOLD_UID
          ? ` AND CASE_VISIT_HOUSEHOLD_UID = '${localThis.householdSelected.HOUSEHOLD_UID}'`
          : ""
        : "";
      let conditionDate = localThis.dateSelectedFrom
        ? ` AND ACTIVITY_DATE >=  '${dateFormatFrom} 00:00:00'`
        : "";
      localThis.dateSelectedTo
        ? (conditionDate += ` AND ACTIVITY_DATE < '${dateFormatTo} 23:59:59' `)
        : null;

      let conditionUser = ` CASE_VISIT_OPERATOR_USER_UID = '${localThis.getUserUid}' `;
      condition =
        conditionUser +
        conditionProcess +
        conditionBeneficiary +
        conditionHouseholdBen +
        conditionHousehold +
        conditionDate;
      return condition;
    },
  },
});
</script>

<style scoped></style>
