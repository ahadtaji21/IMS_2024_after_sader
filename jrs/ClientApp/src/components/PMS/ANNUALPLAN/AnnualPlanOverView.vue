<template>
  <div>
    <div>
      <!-- SRV SELECTION-->
      <v-row v-if="showWait">
        <v-col>
          <div class="text-center" v-if="showWait">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-tooltip top>
            <template v-slot:activator="{ on }">
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3 mx-1"
                small
                v-on="on"
                @click="downloadTableData"
              >
                <v-icon>mdi-microsoft-excel</v-icon>
              </v-btn>
            </template>
            <span>{{ $t("general.download-table-data") }}</span>
          </v-tooltip>
        </v-col>
      </v-row>
      <!-- <v-row>
        <v-col>
          <v-spacer></v-spacer>
          <a>Download Data </a>
          <v-img
            alt="Download Data"
            class="shrink mr-2"
            contain
            :src="require('../../../assets/download-csv-file-icon.png')"
            transition="scale-transition"
            width="40"
            @click="downloadCSV"
            style="cursor: pointer"
          />
        </v-col>
      </v-row> -->
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersOVG"
            :items="overallList"
            item-key="ID"
            sort-by=""
            multi-sort
            :search="tableFilterOVG"
            :items-per-page="-1"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedObj"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("datasource.pms.annual-plan.objectives.overall-goal") }}</h4>

                <v-spacer></v-spacer>
                <!-- 
                <v-text-field
                  v-model="tableFilterOVG"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field> -->
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tr v-for="item in items" :key="item.ID">
                <td :class="'descClass'">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_OVERALL_GOAL_DESC }}</span>
                    </template>
                    <span>{{ item.PMS_OVERALL_GOAL_DESC }}</span>
                  </v-tooltip>
                </td>
              </tr>
            </template>
          </v-data-table>
        </v-col>
      </v-row>

      <!-- <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersOUTC"
            :items="objList1"
            item-key="PMS_ACTIVITY_OUTPUT_REL_ID | getKey"
            sort-by=""
            multi-sort
            :search="tableFilterOUTC"
            :items-per-page="-1"
            style="
               {
                'font-size':'14px','width': '85px','border': '1px solid black';
              }
            "
            class="text-capitalize"
            v-model="selectedOutc"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.outcome-obj-title", 2) }}
                </h4>
                <v-spacer></v-spacer>

                <v-text-field
                  v-model="tableFilterOUTC"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field>
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tr v-for="item in items" :key="item.PMS_ACTIVITY_OUTPUT_REL_ID | getKey">
                <td :class="'descClass'" :style="item.OUTCOMEBACKCOLOR">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.OUTCOME | subStr }}</span>
                    </template>
                    <span>{{ item.OUTCOME }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'" :style="item.OUTCOMEBACKCOLOR">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COI_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COI_CODE }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'" v-if="false" :style="item.OUTCOMEBACKCOLOR">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_COI_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_COI_DESCRIPTION }}</span>
                  </v-tooltip>
                </td>

                <td :class="'descClass'" v-if="false">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TOS_CODE | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TOS_CODE }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'" :style="item.OUTCOMEBACKCOLOR">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_TOS_DESCRIPTION | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_TOS_DESCRIPTION }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'" :style="item.OUTCOMEBACKCOLOR">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_PROCESS | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_PROCESS }}</span>
                  </v-tooltip>
                </td>
                <td :class="'descClass'" :style="item.OUTCOMEBACKCOLOR">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <span v-on="on">{{ item.PMS_OUTPUT | subStr }}</span>
                    </template>
                    <span>{{ item.PMS_OUTPUT }}</span>
                  </v-tooltip>
                </td>
              </tr>
            </template>
          </v-data-table>
        </v-col>
      </v-row> -->

      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersOUTC"
            :items="resultArr"
            sort-by=""
            multi-sort
            :search="tableFilterOUTC"
            :items-per-page="-1"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedOutc"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.outcome-obj-title", 2) }}
                </h4>
                <v-spacer></v-spacer>

                <!-- <v-text-field
                  v-model="tableFilterOUTC"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field> -->
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody>
                <template v-for="item in items">
                  <tr
                    v-for="(subitemCoiCode, iSubCoiCode) in item.subCatOutcome"
                    :key="subitemCoiCode.OUTCOME | getKey"
                  >
                    <td
                      v-if="iSubCoiCode === 0"
                      :rowspan="item.subCatOutcome.length"
                      :class="'descClass'"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on" style="width: 140px !important">{{
                            item.OUTCOME | firstSubstr | subStr
                          }}</span>
                        </template>
                        <span>{{ item.OUTCOME | firstSubstr }} </span>
                      </v-tooltip>
                    </td>
                    <td
                      v-if="iSubCoiCode === 0"
                      :rowspan="item.subCatOutcome.length"
                      :class="'descClass'"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on">{{
                            item.OUTCOME | secondSubstr | stripHTML | subStr
                          }}</span>
                        </template>
                        <span>{{ item.OUTCOME | secondSubstr | stripHTML }}</span>
                      </v-tooltip>
                    </td>
                    <td :colspan="5" style="margin: 0px; padding: 0px">
                      <table style="margin: 0px; padding: 0px; width: 100%">
                        <tr
                          v-for="(
                            subitemTosDesc, iSubTosDesc
                          ) in subitemCoiCode.subCatPmsCoiCode"
                          :key="subitemTosDesc.PMS_COI_CODE | getKey"
                        >
                          <td
                            v-if="iSubTosDesc === 0"
                            :rowspan="subitemCoiCode.subCatPmsCoiCode.length"
                            :style="{ width: '14.3% !important' }"
                            :class="'descClass'"
                          >
                            <span style="width: 140px !important">
                              {{ subitemCoiCode.PMS_COI_CODE | subStr }}
                            </span>
                          </td>

                          <td
                            :style="{
                              width: '85.7% !important',
                              margin: '0px',
                              padding: '0px',
                            }"
                          >
                            <table style="margin: 0px; padding: 0px; width: 100%">
                              <tr
                                v-for="(
                                  subitemProc, iSubProc
                                ) in subitemTosDesc.subCatPmsTosDescription"
                                :key="subitemProc.PMS_TOS_DESCRIPTION | getKey"
                              >
                                <td
                                  :style="{
                                    width: '33.3 !important',
                                    margin: '0px',
                                    padding: '0px',
                                  }"
                                  v-if="iSubProc === 0"
                                  :rowspan="subitemTosDesc.subCatPmsTosDescription.length"
                                  :class="'descClass'"
                                >
                                  <v-tooltip bottom>
                                    <template v-slot:activator="{ on }">
                                      <span v-on="on">{{
                                        subitemTosDesc.PMS_TOS_DESCRIPTION | subStr
                                      }}</span>
                                    </template>
                                    <span>{{ subitemTosDesc.PMS_TOS_DESCRIPTION }}</span>
                                  </v-tooltip>
                                </td>

                                <td
                                  :style="{
                                    width: '66.6% !important',
                                    margin: '0px',
                                    padding: '0px',
                                  }"
                                >
                                  <table
                                    :style="{
                                      margin: '0px',
                                      padding: '0px',
                                      width: '100%',
                                    }"
                                  >
                                    <tr
                                      v-for="(
                                        subitemOut, iSubOut
                                      ) in subitemProc.subCatPmsProcess"
                                      :key="subitemOut.PMS_PROCESS | getKey"
                                    >
                                      <td
                                        :style="{
                                          margin: '0px',
                                          padding: '0px',
                                        }"
                                        v-if="iSubOut === 0"
                                        :rowspan="subitemProc.subCatPmsProcess.length"
                                        :class="'descClass'"
                                      >
                                        <v-tooltip bottom>
                                          <template v-slot:activator="{ on }">
                                            <span v-on="on">{{
                                              subitemProc.PMS_Process | subStr
                                            }}</span>
                                          </template>
                                          <span>{{ subitemProc.PMS_Process }}</span>
                                        </v-tooltip>
                                      </td>
                                      <td
                                        :style="{
                                          margin: '0px',
                                          padding: '0px',
                                        }"
                                        :class="'descClass'"
                                      >
                                        <v-tooltip bottom>
                                          <template v-slot:activator="{ on }">
                                            <span v-on="on">{{
                                              subitemOut.PMS_OUTPUT | subStr
                                            }}</span>
                                          </template>
                                          <span>{{ subitemOut.PMS_OUTPUT }}</span>
                                        </v-tooltip>
                                      </td>
                                      <!-- <td
                                        :style="{
                                          margin: '0px',
                                          padding: '0px',
                                        }"
                                        :class="'descClass'"
                                      >
                                        <v-tooltip bottom>
                                          <template v-slot:activator="{ on }">
                                            <span v-on="on">{{
                                              subitemOut.PMS_OUTPUT_VALUE | subStr
                                            }}</span>
                                          </template>
                                          <span>{{ subitemOut.PMS_OUTPUT_VALUE }}</span>
                                        </v-tooltip>
                                      </td> -->
                                    </tr>
                                  </table>
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </template>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>

      <!-- <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersOUTC"
            :items="resultArr"
            sort-by=""
            multi-sort
            :search="tableFilterOUTC"
            :items-per-page="-1"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedOutc"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.outcome-obj-title", 2) }}
                </h4>
                <v-spacer></v-spacer>

                <v-text-field
                  v-model="tableFilterOUTC"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field>
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody>
                <template v-for="item in items">
                  <tr
                    v-for="(subitemCoiCode, iSubCoiCode) in item.subCatOutcome"
                    :key="subitemCoiCode.OUTCOME | getKey"
                  >
                    <td
                      v-if="iSubCoiCode === 0"
                      :rowspan="item.subCatOutcome.length"
                      :colspan="1"
                      class="s"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on">{{ item.OUTCOME | subStr }}</span>
                        </template>
                        <span>{{ item.OUTCOME }}</span>
                      </v-tooltip>
                    </td>
                    <td :colspan="4">
                      <table>
                        <tr
                          v-for="(
                            subitemTosDesc, iSubTosDesc
                          ) in subitemCoiCode.subCatPmsCoiCode"
                          :key="subitemTosDesc.PMS_COI_CODE | getKey"
                        >
                          <td
                            :colspan="2"
                            v-if="iSubTosDesc === 0"
                            :rowspan="subitemCoiCode.subCatPmsCoiCode.length"
                            class="s"
                          >
                            {{ subitemCoiCode.PMS_COI_CODE | subStr }}
                          </td>

                          <td :colspan="3">
                            <tr
                              v-for="(
                                subitemProc, iSubProc
                              ) in subitemTosDesc.subCatPmsTosDescription"
                              :key="subitemProc.PMS_TOS_DESCRIPTION | getKey"
                            >
                              <td
                                :colspan="1"
                                v-if="iSubProc === 0"
                                :rowspan="subitemTosDesc.subCatPmsTosDescription.length"
                                class="s"
                              >
                                <v-tooltip bottom>
                                  <template v-slot:activator="{ on }">
                                    <span v-on="on">{{
                                      subitemTosDesc.PMS_TOS_DESCRIPTION | subStr
                                    }}</span>
                                  </template>
                                  <span>{{ subitemTosDesc.PMS_TOS_DESCRIPTION }}</span>
                                </v-tooltip>
                              </td>

                              <td :colspan="2">
                                <tr
                                  v-for="(
                                    subitemOut, iSubOut
                                  ) in subitemProc.subCatPmsProcess"
                                  :key="subitemOut.PMS_PROCESS | getKey"
                                >
                                  <td
                                    :colspan="1"
                                    v-if="iSubOut === 0"
                                    :rowspan="subitemProc.subCatPmsProcess.length"
                                    class="s"
                                  >
                                    <v-tooltip bottom>
                                      <template v-slot:activator="{ on }">
                                        <span v-on="on">{{
                                          subitemProc.PMS_Process | subStr
                                        }}</span>
                                      </template>
                                      <span>{{ subitemProc.PMS_Process }}</span>
                                    </v-tooltip>
                                  </td>
                                  <td :colspan="1">
                                    <v-tooltip bottom>
                                      <template v-slot:activator="{ on }">
                                        <span v-on="on">{{
                                          subitemOut.PMS_OUTPUT | subStr
                                        }}</span>
                                      </template>
                                      <span>{{ subitemOut.PMS_OUTPUT }}</span>
                                    </v-tooltip>
                                  </td>
                                </tr>
                              </td>
                            </tr>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </template>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row> -->

      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersSSS"
            :items="sssList"
            item-key="PMS_SSS_ID"
            multi-sort
            :search="tableFilterSSS"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedSSS"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("datasource.pms.annual-plan.objectives.strat-supp-serv") }}</h4>

                <v-spacer></v-spacer>

                <!-- <v-text-field
                  v-model="tableFilterSSS"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field> -->
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody style="border: solid">
                <tr v-for="item in items" :key="item.PMS_SSS_ID" style="height: 10px">
                  <!-- <td class="tablerow">{{ item.PMS_STRAT_SUPP_SERV_TARGET_DESC }}</td> -->
                  <td class="tablerow">{{ item.PMS_STRAT_SUPP_SERV_DESC }}</td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
      <!-- 
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersCCS"
            :items="ccsList"
            item-key="PMS_CCS_ID"
            multi-sort
            :search="tableFilterCCS"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedCCS"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("datasource.pms.annual-plan.objectives.c-c-s") }}</h4>

                <v-spacer></v-spacer>

                <v-text-field
                  v-model="tableFilterCCS"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field>
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody style="border: solid">
                <tr v-for="item in items" :key="item.PMS_CCS_ID" style="height: 10px">
              
                  <td class="tablerow">{{ item.PMS_CCS_DESC }}</td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
      -->
      <!-- <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersCCP"
            :items="ccpList"
            item-key="PMS_CCP_ID"
            multi-sort
            :search="tableFilterCCP"
            style="
               {stop_date
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedCCP"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("datasource.pms.annual-plan.objectives.c-c-p") }}</h4>

                <v-spacer></v-spacer>

                <v-text-field
                  v-model="tableFilterCCP"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field>
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody style="border: solid">
                <tr v-for="item in items" :key="item.PMS_CCP_ID" style="height: 10px">
               
                  <td class="tablerow">{{ item.PMS_CCP_DESC }}</td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row> -->
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersCC"
            :items="ccList"
            item-key="PMS_CC_ID"
            multi-sort
            :search="tableFilterCC"
            style="
               {stop_date
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedCC"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("datasource.pms.annual-plan.objectives.c-c") }}</h4>

                <v-spacer></v-spacer>

                <!-- <v-text-field
                  v-model="tableFilterCC"
                  append-icon="mdi-magnify"
                  :label="$t('general.search')"
                  hide-details
                  clearable
                  outlined
                  dense
                  class="white"
                  color="secondary darken-2"
                ></v-text-field> -->
              </div>
            </template>
            <template v-slot:body="{ items }">
              <tbody style="border: solid">
                <tr v-for="item in items" :key="item.PMS_CC_ID" style="height: 10px">
                  <td class="tablerow">{{ item.PMS_CC_DESC }}</td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsTable from "../../JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n, translate } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import UtilMix from "../../../mixins/UtilMix";
const Papa = require("papaparse");
const fs = require("fs");
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {},
  props: {
    selectedAnnualPlanId: {
      type: Number,
      required: true,
    },
    // showActivityDetails: {
    //   type: Boolean,
    //   required: true,
    // },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },

  data() {
    return {
      objDownloadStruct: [],
      objList1: [],
      index: 0,
      selected: [],
      resultArr: [],
      showWait: false,
      keyRnd: 0,
      bdg: 0,
      resourceId: {},
      json_data: "",
      selectedActivityId: 0,
      selectedActivityItemId: 0,
      selectedResource: "",
      selectedResourceId: 0,
      editObjId: 0,
      showActTabs: true,
      editObjDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showIndicators: false,
      selectedSSS: [],
      selectedCCS: [],
      selectedCCP: [],
      tableFilterSSS: "",
      tableFilterCCS: "",
      tableFilterCCP: "",
      selectedCC: [],
      tableFilterCC: "",
      selectedObj: [],
      selectedOutc: [],
      tableFilterOVG: "",
      tableFilterOUTC: "",
      tblName: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headersOVG: [],
      headersOUTC: [],
      headersSSS: [],
      headersCCS: [],
      headersCCP: [],
      headersCC: [],
      objectiveList: [] as any,
      objectiveListGrouped: [] as any,
      formData: {},
      coi: [],
      tos: [],
      overallList: [] as any,
      sssList: [] as any,
      ccsList: [] as any,
      ccpList: [] as any,
      ccList: [] as any,
      occ: [],
    };
  },
  mixins: [UtilMix],
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      generateExcelForTableOvwInd: "generateExcelForTableOvwInd",
    }),

    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),
    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
    },

    /*   interface objectItemExport {
  OBJECTIVE: string;
  SERVICE: string;
  ACTIVITY: string;
  "CATEGORY OF INT CODE": string;
  "CATEGORY OF INT DESC": string;
  "TYPE OF SERVICE CODE": string;
  "TYPE OF SERVICE DESC": string;
  "CHART OF ACC. CODE": string;
  "CHART OF ACC. DESC": string;
  "CHART OF ACC. CUSTOM DESC": string;
  JAN: string;
  FEB: string;
  MAR: string;
  APR: string;
  MAY: string;
  JUN: string;
  JUL: string;
  AUG: string;
  SEP: string;
  OCT: string;
  NOV: string;
  DEC: string;
}
*/

    generateTableDataOvgAP() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];

      let view: string = "PMS_VI_OVERALL_GOAL_M_AND_E";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OVERALL_GOAL_M_AND_E_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      localThis.headersOVG
        .filter((h: JrsHeader) => h.value != "action")
        .forEach((h: JrsHeader) => {
          colList.push(h.value);
          if (h.text) {
            colLabels.push({
              columnName: h.value,
              columnLabel: translate(h.text).toString(),
            });
          }
        });

      let obj = {} as any;

      obj.viewName = view;
      obj.whereCond = wherecond;
      obj.orderStmt = "PMS_OVG_ID";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Overall Goal";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataOutcAP() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTP_OVERVIEW";
      let wherecond: string = `AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTP_OVERVIEW_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      localThis.headersOUTC
        .filter((h: JrsHeader) => h.value != "action")
        .forEach((h: JrsHeader) => {
          colList.push(h.value);
          if (h.text) {
            colLabels.push({
              columnName: h.value,
              columnLabel: translate(h.text).toString(),
            });
          }
        });

      let obj = {} as any;

      (obj.viewName = view), (obj.whereCond = wherecond);
      obj.orderStmt = "OBJ_ID,SRV_ID,ACT_ID,PMS_COI_ID,PMS_TOS_ID,PROCESS_ID,OUTPUT_ID";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Outcome Based Objective";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataCCCAP() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_CC";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CC_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      localThis.headersCC
        .filter((h: JrsHeader) => h.value != "action")
        .forEach((h: JrsHeader) => {
          colList.push(h.value);
          if (h.text) {
            colLabels.push({
              columnName: h.value,
              columnLabel: translate(h.text).toString(),
            });
          }
        });

      let obj = {} as any;

      (obj.viewName = view), (obj.whereCond = wherecond);
      obj.orderStmt = "PMS_CC_ID";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Principles, Values, Criteria Cross Cutting";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataSSSAP() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_STRAT_SUPP_SERV";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_STRAT_SUPP_SERV_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      localThis.headersSSS
        .filter((h: JrsHeader) => h.value != "action")
        .forEach((h: JrsHeader) => {
          colList.push(h.value);
          if (h.text) {
            colLabels.push({
              columnName: h.value,
              columnLabel: translate(h.text).toString(),
            });
          }
        });

      let obj = {} as any;

      (obj.viewName = view), (obj.whereCond = wherecond);
      obj.orderStmt = "PMS_SSS_ID";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Strategic And Support Service";
      localThis.objDownloadStruct.push(obj);
    },
    // generateTableDataOUTPAP() {
    //       const localThis: any = this as any;
    //       const colList: Array<any> = [];
    //       const colLabels: Array<any> = [];
    //      let view: string = "PMS_VI_ANNUAL_PLAN_OUTP_IND_OVERVIEW";
    //       let wherecond: string = `AP_ID = ${localThis.selectedAnnualPlanId} `;
    //       if (localThis.versioned > 0) {
    //         view = "PMS_VI_ANNUAL_PLAN_OUTP_OVERVIEW_VER";
    //         wherecond += ` AND VERSION_ID=${localThis.versioned}`;
    //       }
    //       localThis.headersOUTP
    //         .filter((h: JrsHeader) => h.value != "action")
    //         .forEach((h: JrsHeader) => {
    //           colList.push(h.value);
    //           if (h.text) {
    //             colLabels.push({
    //               columnName: h.value,
    //               columnLabel: translate(h.text).toString(),
    //             });
    //           }
    //         });

    //       let obj = {} as any;

    //       (obj.viewName = view), (obj.whereCond = wherecond);
    //       obj.orderStmt = "OBJ_ID,SRV_ID,ACT_ID,PMS_COI_ID,PMS_TOS_ID,PROCESS_ID,OUTPUT_ID";
    //       obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
    //       obj.colList = colList.join(",");
    //       obj.columnLabels = colLabels;
    //       obj.sheetName="Output Indicators";
    //       localThis.objDownloadStruct.push(obj);
    //     },

    downloadTableData() {
      const localThis: any = this as any;
      localThis.objDownloadStruct = [];
      const fileName: string = "OVW_AP";
      const fileExtension: string = ".xlsx";
      localThis.generateTableDataOvgAP();
      localThis.generateTableDataOutcAP();
      //localThis.generateTableDataOUTPAP()
      localThis.generateTableDataCCCAP();
      localThis.generateTableDataSSSAP();
      //let bodyValues =  JSON.stringify(localThis.resultArrOutp);
      localThis
        .generateExcelForTableOvwInd(localThis.objDownloadStruct)
        .then((byteArray: any[]) => {
          const byteBufffer = localThis._base64ToArrayBuffer(byteArray, fileExtension);
          localThis.saveByteArray(
            fileName + fileExtension,
            byteBufffer,
            "vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          );
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },

    reloadOvg(item: any) {
      let localThis = this as any;

      localThis.getAnnualPlanOverallGoals();
      localThis.getAnnualPlanObjectives();
    },

    getAnnualPlanObjectives() {
      let localThis: any = this as any;
      localThis.objectiveList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTP_OVERVIEW";
      let wherecond: string = `AP_ID = ${localThis.selectedAnnualPlanId} `;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTP_OVERVIEW_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "OBJ_ID,SRV_ID,ACT_ID,PMS_COI_ID,PMS_TOS_ID,PROCESS_ID,OUTPUT_ID",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let objItem = {} as any;
              objItem.PMS_ACTIVITY_OUTPUT_REL_ID = item.PMS_ACTIVITY_OUTPUT_REL_ID;
              objItem.AI_ID = item.AI_ID;
              objItem.OBJ_ID = item.OBJ_ID;
              objItem.OBJ_DESC = item.OBJ_DESC;
              objItem.SRV_ID = item.SRV_ID;
              objItem.ACT_ID = item.ACT_ID;
              objItem.PMS_COI_ID = item.PMS_COI_ID;
              objItem.PMS_TOS_ID = item.PMS_TOS_ID;
              objItem.IND_NUMBER = item.IND_NUMBER;

              objItem.OUTCOME = item.OUTCOME + "^#^" + item.OBJ_DESC;

              objItem.PMS_COI_CODE = item.PMS_COI_CODE + "";
              objItem.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION + "";
              objItem.PMS_TOS_CODE = item.PMS_TOS_CODE + "";
              objItem.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION + "";
              objItem.PMS_PROCESS = item.PROCESS + "";
              objItem.PMS_OUTPUT = item.OUTPUT + "";
              objItem.PMS_OUTPUT_VALUE = item.PMS_OUTPUT_VALUE + "";
              objItem.PMS_PROCESS_ID = item.PROCESS_ID;
              objItem.PMS_OUTPUT_ID = item.OUTPUT_ID;

              localThis.objectiveList.push(objItem);
              localThis.selected.push(false);
            });
            localThis.groupList();
          }
          localThis.showWait = false;
        }
      );
    },
    groupList() {
      let localThis: any = this as any;
      localThis.resultArr = [] as any;
      localThis.objectiveListGrouped = [];
      localThis.objectiveList.forEach((item: any) => {
        let objItem = {} as any;
        objItem.PMS_ACTIVITY_OUTPUT_REL_ID = item.PMS_ACTIVITY_OUTPUT_REL_ID;
        objItem.OUTCOME = item.OUTCOME;
        objItem.OBJ_DESC = item.OBJ_DESC;
        objItem.PMS_COI_CODE = item.PMS_COI_CODE;
        //objItem.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION;
        //objItem.PMS_TOS_CODE = item.PMS_TOS_CODE;
        objItem.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION;
        objItem.PMS_PROCESS = item.PMS_PROCESS;
        objItem.PMS_OUTPUT = item.PMS_OUTPUT;
        objItem.PMS_OUTPUT_VALUE = item.PMS_OUTPUT_VALUE;
        localThis.objectiveListGrouped.push(objItem);
      });
      const groupByOUTCOME = localThis.objectiveListGrouped.reduce(
        (group: any, item: any) => {
          const { OUTCOME } = item;
          group[OUTCOME] = group[OUTCOME] ?? [];
          group[OUTCOME].push(item);
          return group;
        },
        {}
      );
      Object.keys(groupByOUTCOME).forEach((item) => {
        localThis.resultArr.push({
          OUTCOME: item,

          subCatOutcome: groupByOUTCOME[item],
        });
      });

      let i: number = 0;
      for (i = 0; i < localThis.resultArr.length; i++) {
        const groupByPMS_COI_CODE = localThis.resultArr[i].subCatOutcome.reduce(
          (group: any, item: any) => {
            const { PMS_COI_CODE } = item;
            group[PMS_COI_CODE] = group[PMS_COI_CODE] ?? [];
            group[PMS_COI_CODE].push(item);
            return group;
          },
          {}
        );
        let objCoiCode: any = [];
        Object.keys(groupByPMS_COI_CODE).forEach((item) => {
          objCoiCode.push({
            PMS_COI_CODE: item,
            subCatPmsCoiCode: groupByPMS_COI_CODE[item],
          });
        });
        localThis.resultArr[i].subCatOutcome = objCoiCode;
      }

      localThis.resultArr.forEach((itemArray: any) => {
        itemArray.subCatOutcome.forEach((itemsPmsCoiCode: any) => {
          const groupByPMS_TOS_DESCRIPTION = itemsPmsCoiCode.subCatPmsCoiCode.reduce(
            (group: any, item: any) => {
              const { PMS_TOS_DESCRIPTION } = item;
              group[PMS_TOS_DESCRIPTION] = group[PMS_TOS_DESCRIPTION] ?? [];
              group[PMS_TOS_DESCRIPTION].push(item);
              return group;
            },
            {}
          );

          let objPmsTosDescription: any = [];
          Object.keys(groupByPMS_TOS_DESCRIPTION).forEach((item) => {
            objPmsTosDescription.push({
              PMS_TOS_DESCRIPTION: item,
              subCatPmsTosDescription: groupByPMS_TOS_DESCRIPTION[item],
            });
          });

          itemsPmsCoiCode.subCatPmsCoiCode = objPmsTosDescription;
        });
      });

      localThis.resultArr.forEach((itemArray: any) => {
        itemArray.subCatOutcome.forEach((itemsPmsCoiCode: any) => {
          itemsPmsCoiCode.subCatPmsCoiCode.forEach((itemsPmsTosDescription: any) => {
            const groupByPMS_PROCESS = itemsPmsTosDescription.subCatPmsTosDescription.reduce(
              (group: any, item: any) => {
                const { PMS_PROCESS } = item;
                group[PMS_PROCESS] = group[PMS_PROCESS] ?? [];
                group[PMS_PROCESS].push(item);
                return group;
              },
              {}
            );

            let objPmsProcess: any = [];
            Object.keys(groupByPMS_PROCESS).forEach((item) => {
              objPmsProcess.push({
                PMS_Process: item,
                subCatPmsProcess: groupByPMS_PROCESS[item],
              });
            });

            itemsPmsTosDescription.subCatPmsTosDescription = objPmsProcess;
          });
        });
      });
      let actualColor = "background-color: white";
      for (i = 0; i < localThis.resultArr.length; i++) {
        let outcome: any = {};
        let outcomeInserted: boolean = false;
        outcome = localThis.resultArr[i];
        let j: number = 0;
        for (j = 0; j < outcome.subCatOutcome.length; j++) {
          let coicode: any = {};
          let coicodeInserted: boolean = false;
          coicode = outcome.subCatOutcome[j];
          let k: number = 0;
          for (k = 0; k < coicode.subCatPmsCoiCode.length; k++) {
            let q: number = 0;
            let tos: any = {};
            let tosInserted: boolean = false;
            tos = coicode.subCatPmsCoiCode[k];
            for (q = 0; q < tos.subCatPmsTosDescription.length; q++) {
              let z: number = 0;
              let proc: any = {};
              let procInserted: boolean = false;
              proc = tos.subCatPmsTosDescription[q];
              for (z = 0; z < proc.subCatPmsProcess.length; z++) {
                let out: any = proc.subCatPmsProcess[z];

                let itmobj: any = {};
                if (!outcomeInserted) {
                  outcomeInserted = true;
                  if (actualColor == "background-color: white") {
                    actualColor = "background-color: whitesmoke";
                  } else {
                    actualColor = "background-color: white";
                  }

                  itmobj.OUTCOME = outcome.OUTCOME;
                } else {
                  itmobj.OUTCOME = "";
                }
                itmobj.OUTCOMEBACKCOLOR = actualColor;
                if (!coicodeInserted) {
                  coicodeInserted = true;
                  itmobj.PMS_COI_CODE = coicode.PMS_COI_CODE;
                } else {
                  itmobj.PMS_COI_CODE = "";
                }
                if (!tosInserted) {
                  tosInserted = true;
                  itmobj.PMS_TOS_DESCRIPTION = tos.PMS_TOS_DESCRIPTION;
                } else {
                  itmobj.PMS_TOS_DESCRIPTION = "";
                }

                if (!procInserted) {
                  procInserted = true;
                  itmobj.PMS_PROCESS = proc.PMS_Process;
                } else {
                  itmobj.PMS_PROCESS = "";
                }
                itmobj.PMS_OUTPUT = out.PMS_OUTPUT;
                itmobj.PMS_OUTPUT_VALUE = out.PMS_OUTPUT_VALUE;
                localThis.objList1.push(itmobj);
              }
            }
          }
        }
      }

      // console.log(localThis.resultArr);
    },

    getAnnualPlanOverallGoals() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.showWait = true;
      localThis.overallList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_OVERALL_GOAL_M_AND_E";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OVERALL_GOAL_M_AND_E_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.overallList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },

    getAnnualPlanSSSs() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.sssList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_STRAT_SUPP_SERV";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_STRAT_SUPP_SERV_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.sssList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },
    getAnnualPlanCCSs() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.ccsList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_CCS";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CCS_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.ccsList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },
    getAnnualPlanCCPs() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.ccpList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_CCP";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CCP_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.ccpList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },

    getAnnualPlanCCs() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.ccList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_CC";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CC_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.ccList.push(item);
            });
          }
          localThis.showWait = false;
        }
      );
    },
    // downloadCSV() {
    //   let localThis: any = this as any;

    //   var exportList = [] as any;
    //   var i: number = 0;
    //   for (i = 0; i < localThis.overallList.length; i++) {
    //     var exportItem = {} as any;
    //     var itm = {} as any;
    //     itm = localThis.overallList[i];
    //     exportItem.PMS_OVERALL_GOAL_DESC = localThis.removeUndefined(
    //       itm.PMS_OVERALL_GOAL_DESC
    //     );
    //     exportList.push(exportItem);
    //   }
    //   for (i = 0; i < localThis.objectiveList.length; i++) {
    //     var exportItemobj = {} as any;
    //     var itmobj = {} as any;
    //     itmobj = localThis.objectiveList[i];
    //     exportItemobj.OBJ_DESC = localThis.removeUndefined(itmobj.OBJ_DESC);
    //     exportItemobj.PMS_COI_CODE = localThis.removeUndefined(itmobj.PMS_COI_CODE);
    //     exportList.push(exportItemobj);
    //   }
    //   var js = JSON.stringify(exportList);

    //   var blob = new Blob([Papa.unparse(js)], {
    //     type: "text/csv;charset=utf-8;",
    //   });

    //   var link = document.createElement("a");

    //   var url = URL.createObjectURL(blob);
    //   link.setAttribute("href", url);
    //   link.setAttribute("download", "overview.csv");
    //   link.style.visibility = "hidden";
    //   document.body.appendChild(link);
    //   link.click();
    //   document.body.removeChild(link);
    // },
    removeUndefined: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string;
    },

    setupHeaders() {
      let localThis: any = this as any;
      let descWidth: string;
      let descWidth1_2: string;
      let descWidth2_1: string;
      descWidth = "140px";
      descWidth1_2 = "70px";
      descWidth2_1 = "163px";

      let tmpOVG: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.overall-goal")
            .toString(),
          value: "PMS_OVERALL_GOAL_DESC",
          align: "center",
          divider: true,
        },
      ];

      localThis.headersOVG = tmpOVG;

      let tmpOUTC: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.outcome-obj-title")
            .toString(),
          value: "OUTCOME",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.ap-brief-description")
            .toString(),
          value: "OBJ_DESC",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-code").toString(),
          value: "PMS_COI_CODE",
          align: "center",
          divider: true,
          width: descWidth1_2,
        },
        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-desc").toString(),
        //   value: "PMS_COI_DESCRIPTION",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },

        // {
        //   text: this.$i18n.t("datasource.pms.annual-plan.objectives.tos-code").toString(),
        //   value: "PMS_TOS_CODE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.tos-desc").toString(),
          value: "PMS_TOS_DESCRIPTION",
          align: "center",
          divider: true,
          width: descWidth2_1,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.activity").toString(),
          value: "PROCESS",
          align: "center",
          divider: true,
          width: descWidth2_1,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.output").toString(),
          value: "OUTPUT",
          align: "center",
          divider: true,
          width: descWidth2_1,
        },
        // {
        //   text: this.$i18n.t("pms.output-value").toString(),
        //   value: "PMS_OUTPUT_VALUE",
        //   align: "center",
        //   divider: true,
        //   width: descWidth,
        // },
      ];

      localThis.headersOUTC = tmpOUTC;

      let tmpSSS: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.strat-supp-serv")
            .toString(),
          value: "PMS_STRAT_SUPP_SERV_DESC",
          align: "center",
          divider: true,
        },
      ];

      localThis.headersSSS = tmpSSS;

      let tmpCCS: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.c-c-s").toString(),
          value: "PMS_CCS_DESC",
          align: "center",
          divider: true,
        },
      ];

      localThis.headersCCS = tmpCCS;
      let tmpCCP: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.c-c-p").toString(),
          value: "PMS_CCP_DESC",
          align: "center",
          divider: true,
        },
      ];

      localThis.headersCCP = tmpCCP;

      let tmpCC: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.c-c").toString(),
          value: "PMS_CC_DESC",
          align: "center",
          divider: true,
        },
      ];

      localThis.headersCC = tmpCC;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
  },
  mounted() {
    let localThis: any = this as any;

    localThis.tblName = "PMS_ACTIVITY";
    localThis.setupHeaders();
    if (localThis.selectedAnnualPlanId > 0) {
      localThis.getAnnualPlanOverallGoals();
      localThis.getAnnualPlanObjectives();
      localThis.getAnnualPlanSSSs();
      // localThis.getAnnualPlanCCSs();
      // localThis.getAnnualPlanCCPs();
      localThis.getAnnualPlanCCs();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.reloadOvg();
      EventBus.$emit("showObjTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadOvg(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 50) return string.substring(0, 50) + "...";
      else return string;
    },
    firstSubstr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string.split("^#^")[0];
    },
    secondSubstr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string.split("^#^")[1];
    },

    stripHTML: function (value: any) {
      if ((value + "").toUpperCase() == "UNDEFINED") return "";
      const div = document.createElement("div");
      div.innerHTML = value;
      const text = div.textContent || div.innerText || "";
      return text;
    },

    getKey: function (string: any) {
      return Math.ceil(Math.random() * 1000) + "";
    },
  },
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;

      localThis.getAnnualPlanOverallGoals();
      localThis.getAnnualPlanObjectives();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showObjDetails = to;
    // },
    selectedAnnualPlanId(to: number) {
      let localThis: any = this as any;
      localThis.selectedAnnualPlanId = to;

      localThis.getAnnualPlanOverallGoals();
      localThis.getAnnualPlanObjectives();
    },

    editMode(to: boolean, from: boolean) {},
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}

.overall-node {
  margin: 1em 0.5em;
}
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.selectedCell {
  background-color: whitesmoke;
  color: black;
  font-size: 12px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
  padding: 0px;
  margin: 0px;
}
.notSelectedCell {
  background-color: white;
  font-size: 1px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
}

.descClass {
  font-size: 10px;
  width: 140px !important;
  height: 10px;
  border: thin;
  border-style: solid;
  border-color: gainsboro;
  margin: 0px;
  padding: 0px;
}
.descClassT {
  margin: 0px;
  padding: 0px;
}
</style>

<style lang="scss">
tbody {
  tr:hover {
    background-color: transparent !important;
  }
}
</style>
