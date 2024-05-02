<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row>
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-class-attendance.title") }}
                <v-icon large> mdi-format-list-checks</v-icon>
              </h1>
              <p>{{ $t("views.view-class-attendance.view-info") }}</p>
            </v-col>
          </v-row>
          <!-- MAIN FILTERS-->

          <v-card outlined class="lv-0" dense>
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- PROCESS SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="process"
                    :label="$t('views.view-activity-instance.activity-select')"
                    :items="processList"
                    :hint="
                      $t('views.view-class-attendance.activity-select-hint')
                    "
                    :persistent-hint="true"
                    item-value="ACTIVITY_ID"
                    item-text="ACTIVITY_DESCR"
                    return-object
                    @change="getClassesForProcess(process.ACTIVITY_ID)"
                  ></v-autocomplete>
                </v-col>
                <!-- CLASS SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="classSelected"
                    :label="$t('views.view-class.class-select')"
                    :disabled="process ? !process.ACTIVITY_ID : false"
                    :items="classList"
                    :hint="$t('views.view-class-attendance.class-select-hint')"
                    :persistent-hint="true"
                    item-value="PMS_CLASS_ID"
                    item-text="PMS_CLASS_NAME"
                    clearable
                    return-object
                  ></v-autocomplete>
                </v-col>
                <!-- DATE FROM -->
                <v-col :cols="2">
                  <jrs-date-picker
                    :label="$t('datasource.pms.class-attendance.date-from')"
                    v-model="dateSelectedFrom"
                    :hint="$t('views.view-class-attendance.date-from-hint')"
                    :persistent-hint="true"
                  ></jrs-date-picker>
                </v-col>
                <!-- DATE TO -->
                <v-col :cols="2">
                  <jrs-date-picker
                    :label="$t('datasource.pms.class-attendance.date-to')"
                    v-model="dateSelectedTo"
                    :hint="$t('views.view-class-attendance.date-to-hint')"
                    :persistent-hint="true"
                  ></jrs-date-picker>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <v-divider></v-divider>
        </v-col>

        <!-- SIMPLE TABLE PROCESS INSTANCE -->
        <v-col :cols="12">
          <jrs-simple-table
            :viewName="'PMS_VI_ANNUAL_PLAN_ACTIVITY_INSTANCE_POSITION'"
            :savePayload="{
              userUid: getUserUid,
              officeId: getCurrentOffice.HR_OFFICE_ID,
            }"
            :dataSourceOrder="'DESCRIPTION, ACTIVITY_DATE'"
            :dataSourceCondition="conditionProcessInstance"
            :fieldDatasourceConditions="[
              {
                field_name: 'ACTIVITY_ID',
                field_condition: `JRS_EMPLOYEE = '${getUserUid}}' AND OFFICE_FILTER_ID = ${getCurrentOffice.HR_OFFICE_ID}`,
              },
            ]"
          >
            <template
              v-slot:simple-table-item-actions="{
                simpleItemRowItem,
                refreshData,
              }"
            >
              <v-icon
                class="mx-2"
                @click="
                  (selectParticipatingUsers = []),
                    openDialogManagmentAttendance(
                      simpleItemRowItem,
                      refreshData
                    )
                "
              >
                mdi-book</v-icon
              >
            </template>
          </jrs-simple-table>
        </v-col>
      </v-row>
      <!--- ACTIVATE DIALOG OF ATTENDANCE-->
      <v-dialog
        v-model="openDialogActivate"
        persistent
        retain-focus
        :overlay="false"
        max-width="45em"
        transition="dialog-transition"
      >
        <v-card>
          <v-container container-fluid>
            <v-card-title primary-title class="text-capitalize">{{
              $t("general.crud.mark-attendance-users")
            }}</v-card-title>

            <v-card-text
              :key="
                selectParticipatingUsers[0]
                  ? selectParticipatingUsers[0].IMS_USER_UID
                    ? selectParticipatingUsers[0].IMS_USER_UID
                    : 0
                  : 0
              "
            >
              <v-row align="center">
                <v-col cols="6">
                  <v-text-field
                    :label="$t('datasource.pms.activity.descr')"
                    :readonly="true"
                    v-model="selectedClass.DESCRIPTION"
                  ></v-text-field>
                </v-col>
                <v-col cols="6">
                  <jrs-date-picker
                    :label="$t('datasource.pms.activity-instance.date')"
                    :readonly="true"
                    v-model="selectedClass.ACTIVITY_DATE"
                  ></jrs-date-picker>
                </v-col>
              </v-row>
              <v-row align="center">
                <v-toolbar>
                  <v-container>
                    <v-subheader>
                      <v-row>
                        <v-col cols="3">
                          <v-icon> mdi-account-check</v-icon>
                          {{ $t("general.is-present") }}
                        </v-col>
                        <v-col cols="3">
                          <v-icon>mdi-account-remove</v-icon>
                          {{ $t("general.is-absent") }}
                        </v-col>
                        <v-col cols="6">
                          <v-icon>mdi-account-star</v-icon>
                          {{ $t("general.holiday") }} /
                          <v-icon> mdi-account-star-outline</v-icon>
                          {{ $t("general.not-holiday") }}
                        </v-col>
                      </v-row>
                    </v-subheader>
                  </v-container>
                </v-toolbar>
              </v-row>

              <v-row>
                <v-toolbar color="primary">
                  <v-spacer></v-spacer>

                  <v-row align="center" class="ml-4">
                    <v-col cols="5">
                      <v-switch
                        dark
                        @change="
                          switchAllPresent
                            ? selectParticipatingUsers.forEach((u) => {
                                u.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT = false;
                                u.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY = false;
                              })
                            : selectParticipatingUsers.forEach((u) => {
                                u.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT = true;
                                u.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY = false;
                              })
                        "
                        v-model="switchAllPresent"
                        inset
                      >
                        <template v-slot:label>
                          <v-icon v-if="switchAllPresent"
                            >mdi-account-check</v-icon
                          >
                          <v-icon v-else>mdi-account-remove</v-icon>
                        </template>
                      </v-switch>
                    </v-col>
                    <v-col cols="5">
                      <v-switch
                        dark
                        @change="
                          switchAllHoliday
                            ? selectParticipatingUsers.forEach((u) => {
                                u.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY = false;
                                u.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT = false;
                              })
                            : selectParticipatingUsers.forEach((u) => {
                                u.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY = true;
                                u.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT = false;
                              })
                        "
                        v-model="switchAllHoliday"
                        inset
                      >
                        <template v-slot:label>
                          <v-icon v-if="switchAllHoliday"
                            >mdi-account-star</v-icon
                          >
                          <v-icon v-else>mdi-account-star-outline</v-icon>
                        </template>
                      </v-switch>
                    </v-col>
                  </v-row>
                </v-toolbar>

                <v-col cols="12">
                  <template scrollable>
                    <v-expansion-panels>
                      <v-expansion-panel
                        v-for="chat in selectParticipatingUsers"
                        :key="chat.title"
                        hide-actions
                      >
                        <v-expansion-panel-header
                          expand-icon="mdi-tooltip-edit"
                          disable-icon-rotate
                        >
                          <v-container>
                            <v-row align="center" class="spacer" no-gutters>
                              <v-col cols="3" sm="1">
                                <v-avatar size="30" color="primary">
                                  <v-icon dark> mdi-account-circle </v-icon>
                                </v-avatar>
                              </v-col>
                              <v-col>
                                {{ chat.PARTICIPATING_USER }}
                              </v-col>

                              <v-col mr="1">
                                <v-switch
                                  @click.native.stop
                                  v-model="
                                    chat.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT
                                  "
                                  v-if="
                                    !chat.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY
                                  "
                                  inset
                                >
                                  <template v-slot:label>
                                    <v-icon
                                      v-if="
                                        chat.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT
                                      "
                                      >mdi-account-check</v-icon
                                    >
                                    <v-icon v-else>mdi-account-remove</v-icon>
                                  </template>
                                </v-switch>
                              </v-col>
                              <v-col>
                                <v-switch
                                  @click.native.stop
                                  v-model="
                                    chat.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY
                                  "
                                  v-if="
                                    !chat.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT
                                  "
                                  inset
                                >
                                  <template v-slot:label>
                                    <v-icon
                                      v-if="
                                        chat.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY
                                      "
                                      >mdi-account-star</v-icon
                                    >
                                    <v-icon v-else
                                      >mdi-account-star-outline</v-icon
                                    >
                                  </template>
                                </v-switch>
                              </v-col>
                            </v-row>
                          </v-container>
                        </v-expansion-panel-header>

                        <v-expansion-panel-content>
                          <v-divider></v-divider>
                          <v-card-text>
                            <jrs-editor
                              v-model="chat.PMS_CLASS_USER_ATTENDANCE_NOTE"
                            >
                            </jrs-editor>
                          </v-card-text>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </template>
                </v-col>
              </v-row>
            </v-card-text>
            <v-card-actions>
              <v-btn
                text
                color="secondary darken-1"
                @click="openDialogManagmentAttendance([])"
                >X {{ $t("general.close") }}</v-btn
              >
              <v-btn
                color="primary"
                @click="UpdAttendancesForProcessInstance()"
                >{{ $t("general.save") }}</v-btn
              >
            </v-card-actions>
          </v-container>
        </v-card>
      </v-dialog>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsMultiSelectTable from "../../components/JrsMultiSelectedTable.vue";
import JrsDatepicker from "../../components/JrsDatePicker.vue";
import JrsEditor from "../../components/TextEditor.vue";
import JrsModalForm from "../../components/JrsModalForm.vue";
import FormMixin from "../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsDatePicker from "../../components/JrsDatePicker.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-editor": JrsEditor,
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
      newProcessInstanceFields: [],
      newProcessInstanceData: {},
      selectParticipatingUsers: [],
      selectedClass: [],
      openDialogAnonymization: false,
      openDialogActivate: false,
      refreshData: Function,
      dataSourceCondition: "isBeneficiary = 1",
      rowDisableSelectRules: [],
      processList: [],
      process: {},
      classList: [],
      classSelected: {},
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
        whereCond: `JRS_EMPLOYEE = '${localThis.getUserUid}' AND IS_EDUCATION = 1 AND OFFICE_FILTER_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID}`,
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
     * Return a list of class for the provided activity (process) .
     */
    getClassesForProcess(ProcessId: any) {
      let localThis: any = this as any;

      localThis.classList = [];
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_CLASS",
        whereCond: `PMS_CLASS_PROCESS_ID = ${ProcessId}`,
      };
      localThis.classSelected = {};
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.item_count > 0) {
            // Setup projectList
            let projList = res.table_data.map((row: any) => {
              return {
                PMS_CLASS_ID: row.PMS_CLASS_ID,
                PMS_CLASS_NAME: row.PMS_CLASS_NAME,
              };
            });
            localThis.classList = [...projList];
          }
        });
    },

    /**
     * Open the dialog to manage the attendance
     */
    async openDialogManagmentAttendance(userData: any, functionRefresh: any) {
      let localThis: any = this as any;

      localThis.selectedClass = userData;
      localThis.openDialogActivate = !localThis.openDialogActivate;
      localThis.refreshData = functionRefresh;

      localThis.selectParticipatingUsers = await localThis.getAttendanceByClass(
        userData
      );
    },

    /**
     * get the attendance of specific class
     */
    async getAttendanceByClass(process: any) {
      let localThis: any = this as any;

      localThis.users = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_CLASS_USER_ATTENDANCE",
        colList: null,
        whereCond: `PMS_CLASS_USER_ATTENDANCE_PROCESS_INSTANCE_ID = ${process.PROCESS_INSTANCE_ID} AND PMS_CLASS_USER_ATTENDANCE_CLASS_ID = ${process.PMS_CLASS_ID}`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.users.push(item);
            });
          }
          return localThis.users;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    /**
     * Function to call SP :Update/Create attendance
     */
    UpdAttendancesForProcessInstance() {
      let localThis: any = this as any;
      localThis.isSaving = true;

      let saveData = {
        rows: localThis.selectParticipatingUsers,
      };

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_CLASS_USER_ATTENDANCE",
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          });
          localThis.openDialogActivate = !localThis.openDialogActivate;
          localThis.refreshData();
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },
  },
  watch: {},
  mounted() {
    let localThis: any = this as any;
    localThis.getProcess();
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),

    /**
     * condition of value of switch for all set all present
     */

    switchAllPresent: {
      get() {
        let localThis: any = this as any;
        let check = true;
        localThis.selectParticipatingUsers.forEach((u: any) => {
          if (!u.PMS_CLASS_USER_ATTENDANCE_IS_PRESENT) {
            check = false;
          }
        });
        return check;
      },
      set(value) {
        (this as any).value = value;
      },
    },

    /**
     * condition of value of switch for all set all holiday
     */
    switchAllHoliday: {
      get() {
        let localThis: any = this as any;
        let check = true;
        localThis.selectParticipatingUsers.forEach((u: any) => {
          if (!u.PMS_CLASS_USER_ATTENDANCE_IS_HOLIDAY) {
            check = false;
          }
        });

        return check;
      },
      set(value) {
        (this as any).value = value;
      },
    },

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
      let conditionClass = localThis.classSelected
        ? localThis.classSelected.PMS_CLASS_ID
          ? ` AND PMS_CLASS_ID = ${localThis.classSelected.PMS_CLASS_ID}`
          : ""
        : "";
      let conditionDate = localThis.dateSelectedFrom
        ? ` AND ACTIVITY_DATE >=  '${dateFormatFrom} 00:00:00'`
        : "";
      localThis.dateSelectedTo
        ? (conditionDate += ` AND ACTIVITY_DATE < '${dateFormatTo} 23:59:59' `)
        : null;

      let conditionUser = ` HR_BIODATA_USER_UID = '${localThis.getUserUid}' `;
      condition =
        conditionUser + conditionProcess + conditionClass + conditionDate;
      return condition;
    },
  },
});
</script>

<style scoped></style>
