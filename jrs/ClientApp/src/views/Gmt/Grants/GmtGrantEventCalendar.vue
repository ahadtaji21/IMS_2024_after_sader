<template>
  <v-container fluid>
    <v-tabs v-model="tab" background-color="primary" dark>
      <v-tab
        @click="
          events = [];
          getEvents();
        "
        v-for="lng in tabsInfo"
        :key="lng.code"
        >{{ lng.descr }}</v-tab
      >
      <v-row
        v-if="tab ? tabsInfo[tab].code == 'CALENDAR' : false"
        justify="end"
        class="mr-10"
        style="margin-top:1px"
      >
        <v-col cols="1" align="right"></v-col>
        <!-- <v-col cols="1" align="right">
          <v-btn x-small outlined @click="setToday(true)">
            {{ $t("general.today") }}
          </v-btn>
        </v-col>
        <v-col cols="1" align="right"></v-col> -->
        <v-col cols="3" align="right">
          <!-- DATE-PICKER -->
          <jrs-date-picker
            :label="'Select date'"
            v-model="selectMonthDate"
            :required="false"
            :dense="true"
            class="mb-5"
            @input="setToday(false)"
          ></jrs-date-picker>
        </v-col>
        <v-col cols="1" align="right"></v-col>
        <!-- <v-col cols="1" align="right">
          <v-btn dark small :color="dark ? 'primary' : 'black'" @click="setDark">
            {{ !dark ? $t("general.dark") : $t("general.light") }}
          </v-btn>
        </v-col> -->
        <v-col cols="2" align="right">
          <v-menu bottom right>
            <template v-slot:activator="{ on }">
              <v-btn x-small outlined v-on="on">
                <span>{{ typeToLabel[type] }}</span>
                <v-icon right style="color:white !important">mdi-menu-down</v-icon>
              </v-btn>
            </template>
            <v-list>
              <v-list-item @click="type = 'day'">
                <v-list-item-title>{{ $t("general.day") }}</v-list-item-title>
              </v-list-item>
              <v-list-item @click="type = 'week'">
                <v-list-item-title>{{ $t("general.week") }}</v-list-item-title>
              </v-list-item>
              <v-list-item @click="type = 'month'">
                <v-list-item-title>{{ $t("general.month") }}</v-list-item-title>
              </v-list-item>
              <v-list-item @click="type = '4day'">
                <v-list-item-title>{{ $t("general.4-days") }}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
        </v-col>
        <v-col cols="1" align="right" >
          <v-btn fab text small class="mt-0" @click="prev">
            <v-icon style="margin-bottom:12px" >mdi-chevron-left</v-icon>
          </v-btn>
          <v-btn fab text small @click="next">
            <v-icon  style="margin-bottom:12px">mdi-chevron-right</v-icon>
          </v-btn>
        </v-col>
        <v-col cols="1" align="right"></v-col>
        <v-col cols="1" align="right">
          <p class="font-weight-medium">{{ title }} / {{ current_year }}</p>
        </v-col>
        <v-col cols="2" align="right"></v-col>
      </v-row>
    </v-tabs>
    <v-tabs-items v-model="tab">
      <v-tab-item v-for="item in tabsInfo" :key="item.code">
        <v-row align-center v-show="item.code == 'TABLE'">
          <v-col justify-center :cols="12">
            <jrs-simple-table
              :viewName="'GMT_VI_GRANT_EVENT'"
              :key="JSON.stringify(events)"
              :selectable="false"
              :dataSourceCondition="computeConditionEvents"
              :savePayload="savePayloadCalendar"
              :nbrOfFormColumns="2"
              :hideNewBtn="!authorizedRole"
              :hideActions="!authorizedRole"
            >
            </jrs-simple-table>
          </v-col>
        </v-row>
        <v-row v-show="item.code == 'CALENDAR'">
          <v-col justify-center :cols="12">
            <v-sheet height="600" :elevation="3">
              <v-calendar
                ref="calendarGrant"
                @change="updateRange"
                :short-months="false"
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
                :event-overlap-mode="'stack'"
                :event-overlap-threshold="30"
              >
                <template v-slot:day-label-header="{ date }">
                  <jrs-modal-form
                    :formFields="formEventsFields"
                    :formData="formEventsData"
                    :nbrOfColumns="2"
                  >
                    <template v-slot:activation>
                      <v-btn
                        @click="populateFormData(date)"
                        small
                        block
                        color="secondary lighten-2"
                        class="grey--text text--darken-3 text-center"
                      >
                        <v-icon>mdi-calendar-plus</v-icon>{{ date }}
                      </v-btn>
                    </template>
                    <!-- <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc, resetFunc }"> -->
                    <template
                      v-slot:modal-form-actions="{ closeModalFunc, validateFunc }"
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
                          saveGrantCalendarEvent(
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
                  </div>
                </template>
              </v-calendar>
              <v-menu
                v-model="selectedOpen"
                :close-on-content-click="false"
                :close-on-click="false"
                :activator="selectedElement"
                offset-x
              >
                <v-card color="white" min-width="250px" flat>
                  <v-toolbar :color="selectedEvent.color" dark>
                    <v-toolbar-title v-html="selectedEvent.name"></v-toolbar-title>
                    <v-spacer></v-spacer>
                    <v-btn
                      icon
                      color="white"
                      @click="updateGrantDetailsCalendar(selectedEvent)"
                    >
                      <v-icon v-if="!updateFormCalendar">mdi-lead-pencil</v-icon>
                      <v-icon v-else>mdi-eye</v-icon>
                    </v-btn>
                    <v-btn icon color="white" @click="closeForm()">
                      <v-icon>mdi-close</v-icon>
                    </v-btn>
                  </v-toolbar>

                  <v-card-text>
                    <jrs-form
                      :formFields="formEventsFields"
                      :formData="formEventsData"
                      :nbrOfColumns="nbrOfFormColumns"
                      v-if="updateFormCalendar"
                    >
                      <template v-slot:form-actions="{ validateFunc, resetForm }">
                        <v-btn
                          v-if="authorizedRole"
                          color="primary"
                          :disabled="isSaving"
                          @click="
                            saveGrantCalendarEvent(
                              formEventsData,
                              0,
                              validateFunc,
                              resetForm
                            );
                            updateFormCalendar = false;
                          "
                          class="ma-2"
                          >{{ $t("general.save") }}</v-btn
                        >

                        <v-btn
                          v-if="authorizedRole"
                          color="secondary"
                          :disabled="isSaving"
                          @click="
                            deleteEvent(formEventsData, 3, validateFunc, resetForm);
                            updateFormCalendar = false;
                          "
                          >{{ $t("general.delete") }}</v-btn
                        >
                      </template>
                    </jrs-form>
                    <!-- READONLY DETAIL -->
                    <div v-else>
                      <jrs-readonly-detail
                        :detailFields="formEventsFields"
                        :detailData="formEventsData"
                        :nbrOfColumns="nbrOfFormColumns"
                      ></jrs-readonly-detail>
                    </div>
                  </v-card-text>
                </v-card>
              </v-menu>
            </v-sheet>
          </v-col>
        </v-row>
        <v-row>
          <template>
            <div class="text-left">
              <h3 class="ml-3">{{ $t("general.legend") }}</h3>
              <v-row>
                <v-switch
                  small
                  class="ml-5"
                  v-model="getDisabled"
                  :label="'Disabled'"
                ></v-switch>
                <v-btn
                  class="ma-3"
                  text
                  icon
                  @click="getDeleted = !getDeleted"
                  color="red lighten-2"
                >
                  <v-icon>{{
                    !getDeleted ? "mdi-delete-off-outline" : "mdi-delete-outline"
                  }}</v-icon>
                </v-btn>
              </v-row>
              <v-chip
                v-for="legend in legends"
                :key="legend.id"
                class="ma-2"
                :color="legend.color"
                text-color="white"
              >
                {{ legend.name }}
                {{ legend.code ? "(" + legend.code + ")" : " " }}
                <span v-if="!legend.active">[Disabled]</span>
              </v-chip>
            </div>
          </template>
        </v-row>
        <v-row v-if="tab ? tabsInfo[tab].code == 'CALENDAR' : false">
          <v-col>
            <jrs-multi-select-table
              :colorNameField="'GMT_EVENT_TYPE_COLOR'"
              v-model="eventsFilters"
              :dataSourceCondition="!getDisabled ? 'GMT_TYPE_DAY_ACTIVE = 1' : '1=1'"
              :label="'Filter events'"
              :dataSource="
                !getDeleted ? 'GMT_VI_EVENT_TYPE' : 'GMT_VI_EVENT_TYPE_DELETED'
              "
              :itemValue="'ID'"
              :itemText="'GMT_EVENT_TYPE_CODE'"
              :key="JSON.stringify(eventsFilters)"
              :allowMultiselect="true"
              :required="true"
              :returnOnlyIds="false"
              :rowDisableSelectRules="rowDisableSelectRules"
            >
            </jrs-multi-select-table>
          </v-col>
        </v-row>
      </v-tab-item>
    </v-tabs-items>
  </v-container>
</template>
<script lang="ts">
import Vue from "vue";
import { EventBus } from "../../../event-bus";
import { mapActions, mapGetters } from "vuex";
import FormMixin from "../../../mixins/FormMixin";
import JrsSimpleTable from "../../../components/JrsSimpleTable.vue";
import JrsModalForm from "../../../components/JrsModalForm.vue";
import JrsReadonlyForm from "../../../components/JrsReadonyDetail.vue";
import JrsForm from "../../../components/JrsForm.vue";
import JrsMultiTableSearch from "../../../components/JrsMultiSelectedTable.vue";

import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { stringify } from "querystring";
const ignoredMessage =
  "The .native modifier for v-on is only valid on components but it was used on <div>.";

Vue.config.warnHandler = (message, vm, componentTrace) => {
  if (message !== ignoredMessage) {
    console.error(message + componentTrace);
  }
};
export default Vue.extend({
  watch: {
    getDisabled(value) {
      let localThis: any = this as any;
      localThis.eventsFilters = [];
      localThis.getLegends();
      localThis.getEvents();
    },
    getDeleted(value) {
      let localThis: any = this as any;
      localThis.eventsFilters = [];
      localThis.getLegends();
      localThis.getEvents();
    },
    eventsFilters() {
      let localThis: any = this as any;
      localThis.events = Object.assign([]);
      localThis.getEvents();
    },
  },
  props: {
    selectedGrantId: {
      type: Number,
      required: true,
    },
  },
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-modal-form": JrsModalForm,
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadonlyForm,
    "jrs-multi-select-table": JrsMultiTableSearch,
    "jrs-date-picker": JrsDatePicker,
  },
  mixins: [FormMixin],
  data() {
    return {
      authorizedRole: false,
      isSaving: false,
      selectedOpen: false,
      formEventsData: {},
      selectMonth: { state: "Florida", abbr: "FL" },
      selectMonthDate: new Date(),
      updateFormCalendar: false,
      getDeleted: false,
      eventsFilters: [],
      getActive: true,
      getDisabled: false,
      formEventsFestvityFields: {},
      rowDisableSelectRules: [],
      dark: false,
      nbrOfFormColumns: 2,
      today: new Date().toISOString().substr(0, 10),
      focus: new Date().toISOString().substr(0, 10),
      type: "month",
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
      monthNames: [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
      ],
      start: null,
      current_year: new Date().getFullYear,
      end: null,
      selectedEvent: {},
      selectedElement: null,
      savePayloadCalendar: {},
      tab: null,
      tabsInfo: [
        {
          code: "TABLE",
          descr: "Table",
        },
        {
          code: "CALENDAR",
          descr: "Calendar",
        },
      ],
      events: [],
      name: null,
      details: null,
      color: "#1976D2",
      currentlyEditing: null,
      title: "",
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
    }),

    computeConditionEvents() {
      let localThis: any = this as any;
      let condition = `GMT_GRANT_ID = ${localThis.selectedGrantId} AND GMT_TYPE_DAY_DELETE = 0`;
      if (!localThis.getDeleted) {
        if (!localThis.getDisabled) {
          condition += ` AND GMT_TYPE_DAY_ACTIVE = 1`;
        }
        return condition;
      } else {
        return `GMT_GRANT_ID = ${localThis.selectedGrantId} AND GMT_TYPE_DAY_DELETE = 1`;
      }
    },
  },
  methods: {
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),

    updateRange({ start, end }: any) {
      let localThis: any = this as any;
      (this as any).start = start;
      (this as any).end = end;

      let yr = (this as any).start.year;
      let mn = (this as any).start.month;
      localThis.title = localThis.monthNames[mn - 1];
      localThis.current_year = yr;
    },

    updateGrantDetailsCalendar(event: any) {
      let localThis: any = this as any;
      localThis.updateFormCalendar = !localThis.updateFormCalendar;
      localThis.getEventById(event.id);
    },

    /**
     * Populate the form of add with standard JRS data for timesheet
     */
    populateFormData(date: any) {
      let localThis: any = this as any;
      localThis.formEventsData = {
        GMT_EVENT_START_DATE: date,
      };
    },

    /**
     * Get Legends
     */
    async getLegends() {
      let localThis: any = this as any;
      let cond = `GMT_TYPE_DAY_DELETE = 0`;
      if (!localThis.getDeleted) {
        if (!localThis.getDisabled) {
          cond += ` AND GMT_TYPE_DAY_ACTIVE = 1`;
        }
      } else {
        cond = `GMT_TYPE_DAY_DELETE = 1`;
      }

      localThis.legend = Object.assign([]);
      let list: any = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: !localThis.getDeleted
          ? "GMT_VI_EVENT_TYPE"
          : "GMT_VI_EVENT_TYPE_DELETED",
        colList: null,
        orderStmt: null,
        whereCond: cond,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            res.table_data.forEach((leg: any) => {
              let eventoData = {
                name: leg.GMT_EVENT_TYPE_NAME,
                code: leg.GMT_EVENT_TYPE_CODE,
                color: leg.GMT_EVENT_TYPE_COLOR ? leg.GMT_EVENT_TYPE_COLOR : "blue",
                id: leg.ID,
                active: leg.GMT_TYPE_DAY_ACTIVE,
              };
              list.push(eventoData);
            });

            localThis.legends = Object.assign([], list);
            localThis.eventsFilters = Object.assign([], res.table_data);
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
     * Close the form
     */
    closeForm() {
      let localThis: any = this as any;
      localThis.updateFormCalendar = false;
      localThis.selectedOpen = false;
      localThis.events = [];
      localThis.getEvents();
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
      localThis.getEventById(event.id);
      nativeEvent.stopPropagation();
    },
    /**
     * Get Events by ID. To update it.
     */
    getEventById(id: any) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_EVENT",
        colList: null,
        whereCond: `ID = ${id}`,
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
     * Get Events of User for Timesheet
     */
    async getEvents() {
      let localThis: any = this as any;

      //let condition = `GMT_GRANT_ID = '${localThis.selectedGrantId}'`
      let condition = localThis.computeConditionEvents;

      if (localThis.eventsFilters.length > 0) {
        let cond: any = [];
        localThis.eventsFilters.forEach((i: any) => {
          cond.push(i.ID);
        });
        let codition_str = "AND GMT_EVENT_ID IN (" + cond.join(",") + ")";
        condition += codition_str;
      } else {
        return [];
      }

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_EVENT",
        colList: null,
        whereCond: condition,
        orderStmt: null,
      };
      let list: any = [];
      localThis.events = Object.assign([]);
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            res.table_data.forEach((time: any) => {
              let eventoData = {
                name: time.GMT_EVENT_TYPE_NAME + " (" + time.GMT_EVENT_TYPE_CODE + ")",
                activity_description: time.GMT_EVENT_TYPE_NAME
                  ? time.GMT_EVENT_TYPE_NAME
                  : "",
                start: localThis
                  .formatDate(new Date(time.GMT_EVENT_START_DATE))
                  .substring(0, 10),
                end: time.GMT_EVENT_END_DATE
                  ? localThis
                      .formatDate(new Date(time.GMT_EVENT_END_DATE))
                      .substring(0, 10) + "T:23:59:00"
                  : localThis
                      .formatDate(new Date(time.GMT_EVENT_START_DATE))
                      .substring(0, 10) + "T:23:59:00",
                color: time.GMT_EVENT_TYPE_COLOR ? time.GMT_EVENT_TYPE_COLOR : "blue",
                id: time.ID,
                timed: time.GMT_EVENT_END_DATE ? 3 : false,
              };

              list.push(eventoData);
            });
          }
          localThis.events = Object.assign([], list);
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

    deleteEvent(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc: Function
    ) {
      //Check form validity

      let localThis: any = this as any;
      localThis.isSaving = true;

      let spName: string = localThis.crudInfo.delete_sp;

      let grantIdPayload = {
        grantId: localThis.selectedGrantId,
      };

      //Add external data to payload
      if (localThis.selectedGrantId) {
        saveData = {
          external_data: grantIdPayload,
          rows: saveData,
        };
      }

      let savePayload = {
        spName: spName,
        actionType: "Destroy",
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

          localThis.updateFormCalendar = false;

          formResetFunc();
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
    setToday(check: any) {
      let localThis: any = this as any;
      if (localThis.selectMonthDate && !check) {
        localThis.focus = localThis.selectMonthDate.toISOString().substring(0, 10);
      } else {
        localThis.focus = localThis.today;
        localThis.selectMonthDate = new Date();
      }
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
      localThis.$refs.calendarGrant[0].prev();
    },

    /**
     * Set next day/month,week..
     */
    next() {
      let localThis: any = this as any;
      localThis.$refs.calendarGrant[0].next();
    },

    /**
     * Seve template data.
     * @param {string} saveData Template data to save
     * @param {SqlActionType} actionType Tipe of SQL action (Create|Update)
     * @param {Function} formValidateFunc Function to validate form data
     * @param {Function} formResetFunc Function to reset the form data
     */
    async saveGrantCalendarEvent(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc: Function
    ) {
      //Check form validity

      let localThis: any = this as any;
      localThis.isSaving = true;

      let spName: string =
        actionType == SqlActionType.NUMBER_0
          ? localThis.crudInfo.create_sp
          : localThis.crudInfo.update_sp;

      let grantIdPayload = {
        grantId: localThis.selectedGrantId,
      };

      //Add external data to payload
      if (localThis.selectedGrantId) {
        saveData = {
          external_data: grantIdPayload,
          rows: saveData,
        };
      }

      let savePayload = {
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

          localThis.closeForm();
        });
    },

    /**
     * configuration of month/day etc..
     */
    nth(d: any) {
      return d > 3 && d < 21
        ? "th"
        : ["th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th"][d % 10];
    },
    // Format using reusable function
    padTo2Digits(num: number) {
      return num.toString().padStart(2, "0");
    },

    // format as "DD/MM/YYYY"
    formatDate(date: Date) {
      let localThis: any = this as any;
      return [
        date.getFullYear(),
        localThis.padTo2Digits(date.getMonth() + 1),
        localThis.padTo2Digits(date.getDate()),
      ].join("-");
    },
  },

  mounted() {
    let localThis: any = this;
    localThis.savePayloadCalendar = {
      userUid: localThis.getUserUid,
      officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      grantId: localThis.selectedGrantId,
    };
    localThis.getLegends(); // get the legend of timesheet
    localThis.getEvents(); // get events
    var role: any = localThis.getCurrentRole.ROLE_CODE;
    if (role == "GM") {
      //Grant Manager
      localThis.authorizedRole = true;
    }
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      if (to.ROLE_CODE == "GM") {
        //Grant Manager
        localThis.authorizedRole = true;
      }
    });
  },
});
</script>
<style scoped>
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.startDate {
  font-size: 7px;
}
</style>
