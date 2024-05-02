<template>
  <v-container fluid fill-height>
    <v-row v-if="showWait">
      <v-col justify-center :cols="12">
        <div class="text-center" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-tabs v-model="tab" background-color="primary" dark>
        <v-tab v-for="lng in tabsInfo" :key="lng.code">{{ lng.descr }}</v-tab>
        <v-row v-if="tab ? tabsInfo[tab].code == 'CALENDAR' : false"
               justify="end"
               class="mr-10"
               style="margin-top:1px">
            <v-col cols="1" align="right"></v-col>
            <!-- <v-col cols="1" align="right">
          <v-btn x-small outlined @click="setToday(true)">
            {{ $t("general.today") }}
          </v-btn>
        </v-col>
        <v-col cols="1" align="right"></v-col> -->
            <v-col cols="3" align="right">
                <!-- DATE-PICKER -->
                <jrs-date-picker :label="'Select date'"
                                 v-model="selectMonthDate"
                                 :required="false"
                                 :dense="true"
                                 class="mb-5"
                                 @input="setToday(false)"></jrs-date-picker>
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
            <!--<v-col cols="1" align="right">
                <v-btn fab text small class="mt-0" @click="prev">
                    <v-icon>mdi-chevron-left</v-icon>
                </v-btn>
                <v-btn fab text small @click="next">
                    <v-icon>mdi-chevron-right</v-icon>
                </v-btn>
            </v-col>-->
            <v-col cols="1" align="right"></v-col>
            <v-col cols="1" align="right">
                <p class="font-weight-medium">{{ title }} / {{ current_year }}</p>
            </v-col>
            <v-col cols="2" align="right"></v-col>
        </v-row>
        <v-tab-item :key="'TABLE'">
            <v-row align-center>
                <v-col justify-center :cols="12">
                    <div v-if="!showFSDetails">
                        <v-data-table :headers="columns"
                                      :items="fundingstatuslist"
                                      item-key="GMT_FS_ID"
                                      multi-sort
                                      :search="tableFilter"
                                      class="text-capitalize elevation-2">
                            <template v-slot:top>
                                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                                    <h3>{{ $tc("datasource.gmt.grant.gt-funding-status", 2) }}</h3>
                                    <v-spacer></v-spacer>
                                    <v-dialog v-model="dialog"
                                              persistent
                                              max-width="1100px"
                                              v-if="module == 'GMT' && authorizedRole == true">
                                        <template v-slot:activator="{ on }">
                                            <v-btn color="secondary lighten-2"
                                                   class="grey--text text--darken-3"
                                                   v-on="on"
                                                   @click="newFS">
                                                <v-icon>mdi-plus-circle-outline</v-icon>New
                                            </v-btn>
                                        </template>
                                        <gmt-fs-details :selectedGrantId="selectedGrantId"
                                                        :formData="editedItem"
                                                        :onlyRead="!(module == 'GMT' && authorizedRole == true)"
                                                        @closeDialogeGtFSDetails="closeEdit"
                                                        :currency="currency"></gmt-fs-details>
                                    </v-dialog>

                                    <v-spacer></v-spacer>

                                    <v-text-field v-model="tableFilter"
                                                  append-icon="mdi-magnify"
                                                  :label="$t('general.search')"
                                                  hide-details
                                                  clearable
                                                  outlined
                                                  dense
                                                  class="white"
                                                  color="secondary darken-2"></v-text-field>
                                </div>
                            </template>

                            <template v-slot:body="{ items }">
                                <tbody style="border: solid">
                                    <tr v-for="item in items" :key="item.id" style="height: 10px">
                                        <td class="tablerow">{{ item.GMT_FS_TYPE_OF_DISBURSEMENT }}</td>
                                        <td class="tablerow">
                                            {{ item.GMT_FS_FUND_AMOUNT }} {{ item.GMT_FS_CURRENCY }}
                                        </td>
                                        <td class="tablerow">
                                            {{ formatDateForList(item.GMT_FS_DATE_EXPECTED) }}
                                        </td>
                                        <td class="tablerow">
                                            {{ formatDateForList(item.GMT_FS_PAYMENT_DATE) }}
                                        </td>
                                        <td class="tablerow">{{ item.GMT_FS_DONOR_REQUIREMENTS }}</td>
                                        <td class="tablerow">{{ item.GMT_FS_COMMENT }}</td>

                                        <td style="text-align: center">
                                            <v-icon small class="mr-2" @click="editItem(item)">mdi-circle-edit-outline</v-icon>
                                            <span v-if="module == 'GMT' && authorizedRole == true">
                                                <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
                                            </span>
                                        </td>
                                    </tr>
                                </tbody>
                            </template>
                        </v-data-table>
                    </div>
                </v-col>
            </v-row>
            <v-row>
                <v-col align-center :cols="12">
                    <div v-if="showFSDetails">
                        <v-dialog v-model="showFSDetails" persistent max-width="1100px">
                            <gmt-fs-details :selectedGrantId="selectedGrantId"
                                            :onlyRead="!(module == 'GMT' && authorizedRole == true)"
                                            @closeDialogeGtFSDetails="closeEdit"
                                            :formData="editedItem"
                                            :currency="currency"></gmt-fs-details>
                        </v-dialog>
                    </div>
                </v-col>
            </v-row>
        </v-tab-item>
        <v-tab-item :key="'CALENDAR'">
            <v-row justify="center"  align="center">
                <v-col cols="1" class="calendar-config">
                    <v-icon color="green">mdi-calendar-check</v-icon> : {{ cont_green }}
                </v-col>
                <v-col cols="1" class="calendar-config">
                    <v-icon color="red">mdi-calendar-remove</v-icon> : {{ cont_red }}
                </v-col>
                <!--<v-col cols="3">-->
                <!-- DATE-PICKER -->
                <!--<jrs-date-picker :label="'Select date'"
                         v-model="selectMonthDate"
                         :hint="'Date'"
                         :required="false"
                         :dense="true"
                         class="mb-5"
                         @input="setToday(false)"></jrs-date-picker>
    </v-col>-->
                <v-col cols="1" class="calendar-config">
                    <v-btn small outlined @click="setToday(true)">
                        {{ $t("general.today") }}
                    </v-btn>
                </v-col>
                <v-col cols="1" class="calendar-config">
                    <v-btn dark small :color="dark ? 'primary' : 'black'" @click="setDark">
                        {{ !dark ? $t("general.dark") : $t("general.light") }}
                    </v-btn>
                </v-col>
                <!--<v-col cols="1" align="left">
        <v-menu bottom right>
            <template v-slot:activator="{ on }">
                <v-btn small outlined v-on="on">
                    <span>{{ typeToLabel[type] }}</span>
                    <v-icon right>mdi-menu-down</v-icon>
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
    </v-col>-->
                <v-col cols="2" align="left" class="calendar-config">
                    <v-menu bottom right>
                        <template v-slot:activator="{ on }">
                            <v-btn small outlined v-on="on">
                                <span>{{ typeSizeCalendar[sizeCalendar] }}</span>
                                <v-icon right>mdi-menu-down</v-icon>
                            </v-btn>
                        </template>
                        <v-list>
                            <v-list-item @click="sizeCalendar = item.value"
                                         v-for="item in bigCalendar"
                                         :key="item.name">
                                <v-list-item-title>{{ item.name }}</v-list-item-title>
                            </v-list-item>
                        </v-list>
                    </v-menu>
                </v-col>
                <v-col cols="2" align="right" class="calendar-config">
                    <v-btn fab text small @click="prev">
                        <v-icon style="color: #214885 !important">mdi-chevron-left</v-icon>
                    </v-btn>
                    <v-btn fab text small @click="next">
                        <v-icon style="color: #214885 !important">mdi-chevron-right</v-icon>
                    </v-btn>
                </v-col>
            </v-row>
            <v-row style="margin-top: 0px">
                <v-col justify-center :cols="12">
                    <v-sheet :height="sizeCalendar" :elevation="3">
                        <v-calendar ref="calendarGrantFund"
                                    v-model="focus"
                                    color="primary"
                                    :short-months="false"
                                    :events="events"
                                    @click:event="showEvent"
                                    :dark="dark"
                                    :event-color="getEventColor"
                                    :event-margin-bottom="3"
                                    :now="today"
                                    :type="type"
                                    @click:more="viewDay"
                                    @click:date="viewDay"
                                    @change="updateRange"
                                    :weekdays="[1, 2, 3, 4, 5, 6, 0]"
                                    locale="eng"
                                    :short-weekdays="false"
                                    :event-overlap-mode="'column'"
                                    :event-overlap-threshold="100">
                        </v-calendar>

                        <v-menu v-model="selectedOpen"
                                :close-on-content-click="false"
                                :close-on-click="false"
                                :activator="selectedElement"
                                offset-x>
                            <v-card color="white" min-width="250px" flat>
                                <v-toolbar :color="selectedEvent.color" dark>
                                    <v-toolbar-title v-html="selectedEvent.name"></v-toolbar-title>
                                    <v-spacer></v-spacer>
                                    <v-btn icon
                                           color="white"
                                           @click="updateGrantDetailsCalendar(selectedEvent)">
                                        <!--<v-icon v-if="!updateFormCalendar">mdi-lead-pencil</v-icon>
                                    <v-icon v-else>mdi-eye</v-icon>-->
                                    </v-btn>
                                    <v-btn icon
                                           color="white"
                                           @click="
                        selectedOpen = false;
                        updateFormCalendar = false;
                      ">
                                        <v-icon>mdi-close</v-icon>
                                    </v-btn>
                                </v-toolbar>

                                <v-card-text>
                                    <jrs-form :formFields="formEventsFields"
                                              :formData="formEventsData"
                                              :nbrOfColumns="nbrOfFormColumns"
                                              v-if="updateFormCalendar">
                                        <template v-slot:form-actions="{ validateFunc, resetForm }">
                                            <v-btn color="primary"
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
                                                   class="ma-2">{{ $t("general.save") }}</v-btn>

                                            <!-- <v-btn color="secondary"  :disabled="isSaving" @click="
                                          deleteEvent(
                                            formEventsData,
                                            3,
                                            validateFunc,
                                            resetForm
                                          )
                                          updateFormCalendar=false
                                        ">{{ $t("general.delete") }}</v-btn>-->
                                        </template>
                                    </jrs-form>
                                    <!-- READONLY DETAIL -->
                                    <div v-else>
                                        <jrs-readonly-detail :detailFields="formEventsFields"
                                                             :detailData="formEventsData"
                                                             :nbrOfColumns="nbrOfFormColumns"></jrs-readonly-detail>
                                    </div>
                                </v-card-text>
                            </v-card>
                        </v-menu>
                    </v-sheet>
                </v-col>
            </v-row>
        </v-tab-item>
    </v-tabs>
  </v-container>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration, GMTApi } from "./../../../../../axiosapi";
import { formatDateTime, translate, i18n } from "./../../../../../i18n";
//import SearchTable from "./FUNDED/GMTGrantProjectsSearch.vue";
import DocDetails from "./GmtFundigStatusMainDataForm.vue";
import { EventBus } from "./../../../../../event-bus";
import { mapActions } from "vuex";
import JrsForm from "../../../../../components/JrsForm.vue";
import FormMixin from "../../../../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "./../../../../../axiosapi/api";
import JrsDatePickerVue from "../../../../JrsDatePicker.vue";
import JrsReadonlyForm from "../../../../../components/JrsReadonyDetail.vue";
export default Vue.extend({
  props: {
    selectedGrantId: {
      type: Number,
      required: true,
    },
    currency: {
      type: String,
      required: true,
    },
  },
  components: {
    // "search-table": SearchTable,
    "gmt-fs-details": DocDetails,
    "jrs-date-picker": JrsDatePickerVue,
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadonlyForm,
  },
  mixins: [FormMixin],
  data() {
    return {
      module: "",
      docDescription: "",
      selectedOpen: false,
      docFileName: "",
      formEventsData: {},
      selectedEvent: {},
      bigCalendar: [
        { name: "X-Small", value: 200 },
        { name: "Small", value: 400 },
        { name: "Medium", value: 600 },
        { name: "Large", value: 900 },
      ],
      typeSizeCalendar: {
        200: "X-Small",
        400: "Small",
        600: "Medium",
        900: "Large",
      },
      today: new Date().toISOString().substr(0, 10),
      focus: new Date().toISOString().substr(0, 10),
      authorizedRole: false,
      showWait: false,
      showFSDetails: false,
      selectedElement: null,
      docId: 0,
      nbrOfFormColumns: 2,
      sizeCalendar: 600,
      imsID: 0,
      dialog: false,
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
      typeToLabel: {
        month: "Month",
        week: "Week",
        day: "Day",
        "4day": "4 Days",
      },
      keyDialog: 0,
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
      columnsTemplate: [
        {
          textKey: "datasource.gmt.grant.gt-type-of-disbursement",
          align: "center",
          value: "GMT_FS_TYPE_OF_DISBURSEMENT",
          sortable: true,
          filterable: true,
        },

        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-fund-disbursement",
          align: "center",
          value: "GMT_FS_FUND_AMOUNT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-date-expected",
          align: "center",
          value: "GMT_FS_DATE_EXPECTED",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-payment-date",
          align: "center",
          value: "GMT_FS_PAYMENT_DATE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-donor-requirements",
          align: "center",
          value: "GMT_FS_DONOR_REQUIREMENTS",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-comments",
          align: "center",
          value: "GMT_FS_COMMENT",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      cont_green: 0,
      cont_red: 0,
      fundingstatuslistObj: {},
      fundingstatuslist: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
      isSaving: false,
      selectMonth: { state: "Florida", abbr: "FL" },
      selectMonthDate: new Date(),
      eventsFilters: [],
      getActive: true,
      dark: false,
      updateFormCalendar: false,
      type: "month",
      formEventsFields: [],
      crudInfo: {},
      start: null,
      end: null,
      events: [],
      name: null,
      details: null,
      color: "#1976D2",
      currentlyEditing: null,
      title: "",
      current_year: new Date().getFullYear,
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
    }),

    columns() {
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }
        let newCol = col;
        newCol.text = translate(col.textKey);
        return newCol;
      });

      return cols;
    },
  },

  mounted() {
    let localThis: any = this;
    localThis.editedItem = {} as any;
    localThis.editedItem.GMT_FS_CURRENCY = localThis.currency;
    localThis.editedItem.GMT_GRANT_ID = localThis.selectedGrantId;
    localThis.getGrantFundingList();
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
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

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

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
     * Get Events of User for Timesheet
     */
    async getEvents(events: any) {
      let localThis: any = this as any;
      let list: any = [];
      localThis.cont_green = 0;
      localThis.cont_red = 0;

      events.forEach((time: any) => {
        let cond_date = time.GMT_FS_PAYMENT_DATE ? true : false;
        let payment_date = time.GMT_FS_PAYMENT_DATE
          ? time.GMT_FS_PAYMENT_DATE
          : localThis.today;
        let eventoData = {
          name: time.GMT_FS_COMMENT
            ? time.GMT_FS_FUND_AMOUNT +
              " (" +
              time.GMT_FS_CURRENCY +
              ") " +
              time.GMT_FS_COMMENT +
              ", type disbursement: " +
              time.GMT_FS_TYPE_OF_DISBURSEMENT
            : time.GMT_FS_FUND_AMOUNT +
              " (" +
              time.GMT_FS_CURRENCY +
              ")" +
              ", type disbursement: " +
              time.GMT_FS_TYPE_OF_DISBURSEMENT,
          start: localThis
            .formatDate(new Date(time.GMT_FS_DATE_EXPECTED))
            .substring(0, 10),
          end:
            localThis.today <= time.GMT_FS_DATE_EXPECTED
              ? localThis
                  .formatDate(new Date(time.GMT_FS_DATE_EXPECTED))
                  .substring(0, 10) + "T:23:59:00"
              : localThis.formatDate(new Date(payment_date)).substring(0, 10) +
                "T:23:59:00",
          color: cond_date ? "green" : "red",
          id: time.GMT_FS_ID,
          timed: payment_date ? 3 : false,
        };
        if (cond_date) {
          localThis.cont_green += 1;
        } else {
          localThis.cont_red += 1;
        }
        list.push(eventoData);
      });
      localThis.events = list;
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
        viewName: "GMT_VI_FUNDING_STATUS",
        //colList: null,
        whereCond: `GMT_FS_ID= ${id}`,
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
     * Close the form
     */
    closeForm() {
      let localThis: any = this as any;
      localThis.selectedOpen = false;
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
      localThis.$refs.calendarGrantFund.prev();
    },
    /**
     * Set next day/month,week..
     */
    next() {
      let localThis: any = this as any;
      localThis.$refs.calendarGrantFund.next();
    },

    newFS() {
      let localThis = this as any;
      localThis.editedItem = {} as any;
      localThis.editedItem.GMT_FS_CURRENCY = localThis.currency;
      localThis.editedItem.GMT_GRANT_ID = localThis.selectedGrantId;
    },
      formatDateForList(item: any) {
          if (item == undefined) return "";

          const date = new Date(item);

          const months = [
              'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
              'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
          ];

          const day = date.getDate();
          const month = months[date.getMonth()];
          const year = date.getFullYear();

          return `${day} ${month} ${year}`;
      },
    generateAgreement(item: any) {
      let localThis = this as any;
      var id: number = 0;
      let ap = {} as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: GMTApi = new GMTApi(config);
      localThis.showWait = true;
      let doc: any;
      return api
        .apiGMTGetGMTTemplateGet(config.baseOptions)
        .then((res) => {
          doc = res.data;
          const byteCharacters = atob(doc);
          const byteNumbers = new Array(byteCharacters.length);
          for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
          }
          const byteArray = new Uint8Array(byteNumbers);
          const blob = new Blob([byteArray], {
            type:
              "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
          });

          var link = document.createElement("a");

          var url = URL.createObjectURL(blob);
          link.setAttribute("href", url);
          link.setAttribute("download", "gm.docx");
          link.style.visibility = "hidden";
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        })
        .catch((err) => {
          // console.error(err);
          return "";
        })
        .finally(() => (localThis.showWait = false));
    },
    getGrantFundingList() {
      let localThis = this as any;

      localThis.fundingstatuslist = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_FUNDING_STATUS",
        //colList: null,
        whereCond: `GMT_GRANT_ID='${localThis.selectedGrantId}'`,
        orderStmt: "GMT_FS_DATE_EXPECTED",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.fundingstatuslist.push(item);
            });
            localThis.getEvents(localThis.fundingstatuslist);
            Object.assign(localThis.crudInfo, res.crud_info);
            let tmpFormFileds: Array<any> = res.columns
              .filter((header: any) => !header.hide_in_form)
              .map((field: any) => localThis.parseJrsFormField(field));
            localThis.formEventsFields = localThis.setupSelectFields(
              tmpFormFileds,
              localThis.getCurrentOffice.HR_OFFICE_ID,
              null
            );
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        })
        .finally(() => (localThis.showWait = false));
    },
    closeEdit() {
      let localThis = this as any;
      localThis.dialog = false;
      localThis.showFSDetails = false;
      localThis.getGrantFundingList();
    },
    closeDialog(item: string) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as any;
      payLoadD.GMT_GRANT_ID = localThis.selectedGrantId;
      payLoadD.GMT_FS_CURRENCY = localThis.currency;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_UPD_FUNDING_STATUS",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getGrantFundingList();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    closeDialogeAndUpdateList(value: any) {
      let localThis = this as any;
      localThis.getGrantFundingList();
      localThis.dialog = false;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
    },
    editItem(item: any) {
      let localThis: any = this as any;
      localThis.showFSDetails = true;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      if (
        localThis.editedItem.GMT_FS_CURRENCY == undefined ||
        localThis.editedItem.GMT_FS_CURRENCY == ""
      ) {
        localThis.editedItem.GMT_FS_CURRENCY = localThis.currency;
      }
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.showWait = true;
          let payLoadD = {} as any;
          payLoadD.GMT_FS_ID = item.GMT_FS_ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_FUNDING_STATUS",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.showWait = false;
              localThis.getGrantFundingList();
            })

            .catch((err: any) => {
              localThis
                .showNewSnackbar({
                  typeName: "error",
                  text: err.message,
                })
                .finally(() => (localThis.showWait = false));
            });
        }
      });
    },
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
</style>
