<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col>
          <v-sheet height="64">
            <v-toolbar flat color="white">
              <v-btn outlined class="mr-4" @click="setToday">
                {{ $t("general.today") }}
              </v-btn>
              <v-btn
                dark
                :color="dark ? 'primary' : 'black'"
                class="mr-4"
                @click="setDark"
              >
                {{ !dark ? $t("general.dark") : $t("general.light") }}
              </v-btn>
              <v-btn fab text small @click="prev">
                <v-icon small>mdi-chevron-left</v-icon>
              </v-btn>
              <v-btn fab text small @click="next">
                <v-icon small>mdi-chevron-right</v-icon>
              </v-btn>
              <v-toolbar-title>{{ title }}</v-toolbar-title>
              <v-spacer></v-spacer>
              <v-menu bottom right>
                <template v-slot:activator="{ on }">
                  <v-btn outlined v-on="on">
                    <span>{{ typeToLabel[type] }}</span>
                    <v-icon right>mdi-menu-down</v-icon>
                  </v-btn>
                </template>
                <v-list>
                  <v-list-item @click="type = 'day'">
                    <v-list-item-title>{{
                      $t("general.day")
                    }}</v-list-item-title>
                  </v-list-item>
                  <v-list-item @click="type = 'week'">
                    <v-list-item-title>{{
                      $t("general.week")
                    }}</v-list-item-title>
                  </v-list-item>
                  <v-list-item @click="type = 'month'">
                    <v-list-item-title>{{
                      $t("general.month")
                    }}</v-list-item-title>
                  </v-list-item>
                  <v-list-item @click="type = '4day'">
                    <v-list-item-title>{{
                      $t("general.4-days")
                    }}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>
            </v-toolbar>
          </v-sheet>
          <v-flex>
            <v-sheet height="800">
              <v-calendar
                ref="calendar"
                v-model="focus"
                color="primary"
                :events="events"
                :dark="dark"
                :event-color="getEventColor"
                :event-margin-bottom="3"
                :now="today"
                :type="type"
                @click:event="showEvent"
                @click:more="viewDay"
                @click:date="viewDay"
                :weekdays="[1, 2, 3, 4, 5, 6, 0]"
                locale="eng"
                :short-weekdays="false"
              >
                <template v-slot:day-header="{ date }">
                  <jrs-modal-form
                    :formFields="formEventsFields"
                    :formData="formEventsData"
                    :nbrOfColumns="nbrOfFormColumns"
                  >
                    <template v-slot:activation>
                      <v-btn
                        block
                        @click="populateFormData(date)"
                        color="secondary lighten-2"
                        class="grey--text text--darken-3 text-center"
                      >
                        <v-icon>mdi-calendar-plus</v-icon>{{ date }}
                      </v-btn>
                    </template>
                    <!-- <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc, resetFunc }"> -->
                    <template
                      v-slot:modal-form-actions="{
                        closeModalFunc,
                        validateFunc,
                      }"
                    >
                      <v-btn
                        color="secondary darken-1"
                        class="mt-2 mr-1"
                        text
                        @click="closeModalFunc()"
                        >X {{ $t("general.close") }}</v-btn
                      >
                      <v-btn
                        color="primary"
                        class="mt-2 mx-1"
                        :disabled="isSaving"
                        @click="
                          saveTimesheetData(
                            formEventsData,
                            1,
                            closeModalFunc,
                            validateFunc
                          )
                        "
                        >Save</v-btn
                      >
                    </template>
                  </jrs-modal-form>
                </template>

                <template v-slot:event="{ event }">
                  <div>
                    <v-list-item-subtitle class="font-weight-black">
                      {{ event.name }}</v-list-item-subtitle
                    >
                    <v-list-item-subtitle>
                      {{ event.start.slice(-8) }}</v-list-item-subtitle
                    >
                    <v-list-item-subtitle>
                      {{ event.end.slice(-8) }}
                    </v-list-item-subtitle>
                    <v-list-item-subtitle>
                      {{ $t("general.approved") }} :
                    </v-list-item-subtitle>
                    <v-list-item-subtitle>
                      <v-icon
                        :color="
                          event.is_approved == true
                            ? 'white green darken-3'
                            : 'white darken-3'
                        "
                        >{{
                          event.is_approved == true
                            ? "mdi-checkbox-marked-circle-outline"
                            : "mdi-checkbox-blank-circle-outline"
                        }}</v-icon
                      >
                    </v-list-item-subtitle>
                  </div>
                </template>
              </v-calendar>

              <v-menu
                v-model="selectedOpen"
                :close-on-content-click="false"
                :activator="selectedElement"
                offset-x
              >
                <v-card color="white" min-width="250px" max-width="850px" flat>
                  <v-toolbar :color="selectedEvent.color" dark>
                    <v-btn text color="white" @click="selectedOpen = false">
                      X
                    </v-btn>
                    <v-toolbar-title
                      v-html="selectedEvent.name"
                    ></v-toolbar-title>
                    <v-spacer></v-spacer>
                  </v-toolbar>

                  <v-card-text>
                    <jrs-form
                      :formFields="formEventsFields"
                      :formData="formEventsData"
                      :nbrOfColumns="nbrOfFormColumns"
                      v-if="
                        !selectedEvent.is_approved && !selectedEvent.festivity
                      "
                    >
                      <template
                        v-slot:form-actions="{ validateFunc, resetForm }"
                      >
                        <v-btn
                          text
                          color="secondary darken-1"
                          @click="closeForm()"
                          >X {{ $t("general.close") }}</v-btn
                        >

                        <v-btn
                          color="primary"
                          :disabled="isSaving"
                          @click="
                            saveTimesheetData(
                              formEventsData,
                              0,
                              validateFunc,
                              resetForm
                            )
                          "
                          class="ma-2"
                          >{{ $t("general.save") }}</v-btn
                        >
                      </template>
                    </jrs-form>
                    <!-- READONLY DETAIL -->
                    <div v-else-if="!selectedEvent.festivity">
                      <jrs-readonly-detail
                        :detailFields="formEventsFields"
                        :detailData="formEventsData"
                        :nbrOfColumns="nbrOfFormColumns"
                      ></jrs-readonly-detail>
                    </div>
                    <div v-else>
                      <jrs-readonly-detail
                        :detailFields="formEventsFestvityFields"
                        :detailData="formEventsData"
                        :nbrOfColumns="nbrOfFormColumns"
                      ></jrs-readonly-detail>
                    </div>
                  </v-card-text>
                </v-card>
              </v-menu>
            </v-sheet>
          </v-flex>
        </v-col>
      </v-row>
      <v-row>
        <template>
          <div class="text-left">
            <h3 class="ml-10">{{ $t("general.legend") }}</h3>
            <v-chip
              v-for="legend in legends"
              :key="legend.id"
              class="ma-2"
              :color="legend.color"
              text-color="white"
            >
              {{ legend.name }}
              {{ legend.code ? "(" + legend.code + ")" : "" }}
            </v-chip>
          </div>
        </template>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsModalForm from "../components/JrsModalForm.vue";
import JrsReadOnlyForm from "../components/JrsReadonyDetail.vue";
import JrsForm from "../components/JrsForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../axiosapi/api";
import FormMixin from "../mixins/FormMixin";
const ignoredMessage =
  "The .native modifier for v-on is only valid on components but it was used on <div>.";

Vue.config.warnHandler = (message, vm, componentTrace) => {
  if (message !== ignoredMessage) {
    console.error(message + componentTrace);
  }
};

export default Vue.extend({
  components: {
    "jrs-modal-form": JrsModalForm,
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadOnlyForm,
  },
  mixins: [FormMixin],
  data: () => ({
    formEventsData: {},
    formEventsFestvityFields: {},
    isSaving: false,
    dark: false,
    nbrOfFormColumns: 2,
    today: new Date().toISOString().substr(0, 10),
    focus: new Date().toISOString().substr(0, 10),
    type: "week",
    formEventsFields: [],
    typeToLabel: {
      month: "Month",
      week: "Week",
      day: "Day",
      "4day": "4 Days",
    },
    validateUserUid: null,
    legends: [],
    crudInfo: {},
    start: null,
    end: null,
    selectedEvent: {},
    selectedElement: null,
    selectedOpen: false,
    events: [],
    name: null,
    details: null,
    color: "#1976D2",
    currentlyEditing: null,
  }),
  computed: {
    ...mapGetters({
      userLocale: "userLocale",
      activeModule: "getActiveModule",
      getCurrentOffice: "getCurrentOffice",
    }),
    title(): any {
      let localThis: any = this as any;
      const { start, end }: any = this;
      if (!start || !end) {
        return "";
      }

      const startMonth = localThis.monthFormatter(start);
      const endMonth = localThis.monthFormatter(end);
      const suffixMonth = startMonth === endMonth ? "" : endMonth;

      const startYear = start.year;
      const endYear = end.year;
      const suffixYear = startYear === endYear ? "" : endYear;

      const startDay = start.day + localThis.nth(start.day);
      const endDay = end.day + localThis.nth(end.day);

      switch (localThis.type) {
        case "month":
          return `${startMonth} ${startYear}`;
        case "week":
        case "4day":
          return `${startMonth} ${startDay} ${startYear} - ${suffixMonth} ${endDay} ${suffixYear}`;
        case "day":
          return `${startMonth} ${startDay} ${startYear}`;
      }
      return "";
    },

    monthFormatter(): any {
      let localThis: any = this as any;
      return localThis.$refs.calendar.getFormatter({
        timeZone: "UTC",
        month: "long",
      });
    },
  },
  mounted() {
    let localThis: any = this as any;

    localThis.getEvents(); // get events
    localThis.getLegends(); // get the legend of timesheet
    localThis.getValidateUserUid(); // get the Validate User Uid of user
  },
  methods: {
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    /**
     * Close the form
     */
    closeForm() {
      let localThis: any = this as any;
      localThis.selectedOpen = false;
    },
    /**
     * Populate the form of add with standard JRS data for timesheet
     */
    populateFormData(date: any) {
      let localThis: any = this as any;
      localThis.formEventsData = {
        HR_TIMESHEET_DATE: date,
        HR_TIMESHEET_START_TIME: "9:00",
        HR_TIMESHEET_END_TIME: "17:00",
        HR_TIMESHEET_USER_VALIDATE_UID: localThis.validateUserUid
          ? localThis.validateUserUid
          : undefined,
      };
    },

    /**
     * Seve template data.
     * @param {string} saveData Template data to save
     * @param {SqlActionType} actionType Tipe of SQL action (Create|Update)
     * @param {Function} formValidateFunc Function to validate form data
     * @param {Function} formResetFunc Function to reset the form data
     */
    async saveTimesheetData(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      //Check form validity

      let localThis: any = this as any;
      localThis.isSaving = true;

      let spName: string =
        actionType == SqlActionType.NUMBER_0
          ? localThis.crudInfo.create_sp
          : localThis.crudInfo.update_sp;

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
            typeName: "success",
            text: res.message,
          }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
          formValidateFunc();
          if (actionType == 0) {
            localThis.closeForm();
          }
          localThis.getEvents();
        });
    },
    /**
     * Get Legends
     */
    async getLegends() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_TIMESHEET_TYPE_DAY",
        colList: null,
        orderStmt: null,
        whereCond: `HR_TYPE_DAY_DELETE = 0`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            res.table_data.forEach((leg: any) => {
              let eventoData = {
                name: leg.HR_TYPE_DAY_DESCRIPTION,
                code: leg.HR_TYPE_DAY_CODE,
                color: leg.HR_TYPE_COLOR ? leg.HR_TYPE_COLOR : "blue",
                id: leg.HR_TYPE_DAY_ID,
              };

              localThis.legends.push(eventoData);
            });
          }

          let festivityDays = {
            name: "Festivity Days",
            color: "green",
            id: 0,
          };

          localThis.legends.push(festivityDays);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },
    /**
     * Get Validate User for current user
     */
    async getValidateUserUid() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SUPERVISOR_FOR_USER",
        colList: null,
        whereCond: `USER_UID = '${localThis.getUserUid}'`,
        orderStmt: null,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            localThis.validateUserUid = res.table_data[0].SUPERVISOR_UID;
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },
    /**
     * Get Events of User for Timesheet
     */
    async getEvents() {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_TIMESHEET",
        colList: null,
        whereCond: `HR_BIODATA_USER_UID = '${localThis.getUserUid}'`,
        orderStmt: null,
      };
      localThis.events = [];
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            res.table_data.forEach((time: any) => {
              let eventoData = {
                name: time.HR_TYPE_DAY_CODE,
                activity_description: time.ACTIVITY_DESCRIPTION
                  ? time.ACTIVITY_DESCRIPTION
                  : "",
                project_code: time.PROJECT_APCODE ? time.PROJECT_APCODE : "",
                is_approved: time.APPROVED_SUPERVISOR,
                validate_user: time.HR_BIODATA_FULL_NAME_VALIDATE_USER
                  ? time.HR_BIODATA_FULL_NAME_VALIDATE_USER
                  : "",
                start:
                  time.HR_TIMESHEET_DATE + " " + time.HR_TIMESHEET_START_TIME,
                end: time.HR_TIMESHEET_DATE_END
                  ? time.HR_TIMESHEET_DATE_END +
                    " " +
                    (time.HR_TIMESHEET_END_TIME
                      ? time.HR_TIMESHEET_END_TIME
                      : "17:00:00")
                  : time.HR_TIMESHEET_DATE +
                    " " +
                    (time.HR_TIMESHEET_END_TIME
                      ? time.HR_TIMESHEET_END_TIME
                      : "17:00"),
                color: time.HR_TYPE_COLOR ? time.HR_TYPE_COLOR : "blue",
                festivity: false,
                id: time.HR_TIMESHEET_ID,

                timed: time.HR_TIMESHEET_DATE_END ? 3 : false,
              };

              localThis.events.push(eventoData);
            });
            localThis.getFestivityDaysForOffice();
          }
          Object.assign(localThis.crudInfo, res.crud_info);
          let tmpFormFileds: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis.formEventsFields = localThis.setupSelectFields(
            tmpFormFileds,
            localThis.getCurrentOffice.HR_OFFICE_ID,
            null
          );
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },
    /**
     * Get Events by ID. To update it.
     */
    getEventById(id: any) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_TIMESHEET",
        colList: null,
        whereCond: `HR_BIODATA_USER_UID = '${localThis.getUserUid}' AND HR_TIMESHEET_ID = ${id}`,
        orderStmt: null,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            localThis.formEventsData = res.table_data[0];
          }
        })
        .catch((err: any) => {
          console.log("ERRORE: " + err);
        });
    },
    /**
     * Get Festivity Events by ID. To visualiza it.
     */
    getFestivityById(id: any) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_TIMESHEET_FESTIVITY_DAY",
        colList: null,
        whereCond: `HR_FESTIVITY_DAY_ID = ${id}`,
        orderStmt: null,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            localThis.formEventsData = res.table_data[0];
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },
    /**
     * Get Festivity Events by office.
     */
    async getFestivityDaysForOffice() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_TIMESHEET_FESTIVITY_DAY",
        colList: null,
        whereCond: `OFFICE_FILTER_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID} AND HR_FESTIVITY_DAY_DELETE = 0`,
        orderStmt: null,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            res.table_data.forEach((festivityDay: any) => {
              let eventoData = {
                name: festivityDay.HR_FESTIVITY_DAY_DESCRIPTION,
                details: festivityDay.HR_FESTIVITY_DAY_DESCRIPTION,
                start: festivityDay.HR_FESTIVITY_DAY_DATE + " 00:00",
                end: festivityDay.HR_FESTIVITY_DAY_DATE + " 23:59",
                festivity: true,
                id: festivityDay.HR_FESTIVITY_DAY_ID,
                color: "green",
                timed: false,
              };
              localThis.events.push(eventoData);

              let tmpFormFileds: Array<any> = res.columns
                .filter((header: any) => !header.hide_in_form)
                .map((field: any) => localThis.parseJrsFormField(field));
              localThis.formEventsFestvityFields = localThis.setupSelectFields(
                tmpFormFileds,
                localThis.getCurrentOffice.HR_OFFICE_ID,
                null
              );
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err,
          });
        });
    },

    /**
     * See Day of Calendar
     */
    viewDay({ date }: any) {
      let localThis: any = this as any;
      localThis.focus = date;
      localThis.type = "day";
    },

    /**
     * Get color of events configurated by user
     */
    getEventColor(event: any) {
      return event.color;
    },

    /**
     * Set Today as current day to visualize
     */
    setToday() {
      let localThis: any = this as any;
      localThis.focus = localThis.today;
    },

    /**
     * Set Dark mode of timesheet
     */
    setDark() {
      let localThis: any = this as any;
      localThis.dark = !localThis.dark;
    },

    /**
     * Set previous day/month,week..
     */
    prev() {
      let localThis: any = this as any;
      localThis.$refs.calendar.prev();
    },

    /**
     * Set next day/month,week..
     */
    next() {
      let localThis: any = this as any;
      localThis.$refs.calendar.next();
    },

    /**
     * Show Event by click it
     */
    showEvent({ nativeEvent, event }: any) {
      let localThis: any = this as any;

      const open = () => {
        localThis.selectedEvent = event;
        localThis.selectedElement = nativeEvent.target;
        setTimeout((u: any) => (localThis.selectedOpen = true), 10);
      };
      if (localThis.selectedOpen) {
        localThis.selectedOpen = false;
        setTimeout(open, 10);
      } else {
        open();
      }
      if (!event.festivity) {
        localThis.getEventById(event.id);
      } else {
        localThis.getFestivityById(event.id);
      }

      nativeEvent.stopPropagation();
    },

    /**
     * configuration of month/day etc..
     */
    nth(d: any) {
      return d > 3 && d < 21
        ? "th"
        : ["th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th"][d % 10];
    },
  },
});
</script>
