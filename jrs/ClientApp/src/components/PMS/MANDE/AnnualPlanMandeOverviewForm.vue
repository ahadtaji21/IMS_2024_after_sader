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
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersOVG"
            :items="ovgList"
            sort-by=""
            multi-sort
            :search="tableFilterOVG"
            :items-per-page="-1"
            :hide-default-footer="true"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedOvg"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.overall-indicators", 2) }}
                </h4>
                <v-spacer></v-spacer>
                <v-tooltip top v-if="false">
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3 mx-1"
                      small
                      v-on="on"
                      @click="downloadTableDataOvgInd"
                    >
                      <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                  </template>
                  <span>{{ $t("general.download-table-data") }}</span>
                </v-tooltip>
                <!-- <v-text-field
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
              <tbody>
                <template v-for="(subitemInd, iSubInd) in items">
                  <tr :key="subitemInd.OVG | getKey">
                    <td v-if="iSubInd === 0" :rowspan="items.length" :class="'descClass'">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on" style="width: 140px !important">{{
                            subitemInd.OVG | subStr
                          }}</span>
                        </template>
                        <span>{{ subitemInd.OVG }} </span>
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
                          <span v-on="on">{{ subitemInd.PMS_OVG_IND | subStr }}</span>
                        </template>
                        <span>{{ subitemInd.PMS_OVG_IND }}</span>
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
                            subitemInd.PMS_OBJ_IND_TECHNIQUE | subStr
                          }}</span>
                        </template>
                        <span>{{ subitemInd.PMS_OBJ_IND_TECHNIQUE }}</span>
                      </v-tooltip>
                    </td>
                  </tr>
                </template>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>

      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersOUTC"
            :items="resultArrOutc"
            sort-by=""
            multi-sort
            :search="tableFilterOUTC"
            :items-per-page="-1"
            :hide-default-footer="true"
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
                  {{ $tc("datasource.pms.annual-plan.objectives.outcome-indicators", 2) }}
                </h4>
                <v-spacer></v-spacer>
                <v-tooltip top v-if="false"
                  >>
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3 mx-1"
                      small
                      v-on="on"
                      @click="downloadTableDataOutcInd"
                    >
                      <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                  </template>
                  <span>{{ $t("general.download-table-data") }}</span>
                </v-tooltip>
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
                    v-for="(subitemOutcome, iSubOutcome) in item.subCatTarget"
                    :key="subitemOutcome.TARGET | getKey"
                  >
                    <td
                      v-if="iSubOutcome === 0"
                      :rowspan="item.subCatTarget.length"
                      :class="'descClass'"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on" style="width: 140px !important">{{
                            item.TARGET | subStr
                          }}</span>
                        </template>
                        <span>{{ item.TARGET }} </span>
                      </v-tooltip>
                    </td>
                    <td :colspan="3" style="margin: 0px; padding: 0px">
                      <table style="margin: 0px; padding: 0px; width: 100%">
                        <tr
                          v-for="(subitemInd, iSubInd) in subitemOutcome.subCatOutcome"
                          :key="subitemInd.OUTCOME | getKey"
                        >
                          <td
                            v-if="iSubInd === 0"
                            :rowspan="subitemOutcome.subCatOutcome.length"
                            :style="{ width: '33.3% !important' }"
                            :class="'descClass'"
                          >
                            <span style="width: 140px !important">
                              {{ subitemInd.OUTCOME | subStr }}
                            </span>
                          </td>

                          <td
                            :style="{
                              margin: '0px',
                              padding: '0px',
                              width: '33.3% !important',
                            }"
                            :class="'descClass'"
                          >
                            <v-tooltip bottom>
                              <template v-slot:activator="{ on }">
                                <span v-on="on">{{ subitemInd.INDICATOR | subStr }}</span>
                              </template>
                              <span>{{ subitemInd.INDICATOR }}</span>
                            </v-tooltip>
                          </td>

                          <td
                            :style="{
                              margin: '0px',
                              padding: '0px',
                              width: '33.3% !important',
                            }"
                            :class="'descClass'"
                          >
                            <v-tooltip bottom>
                              <template v-slot:activator="{ on }">
                                <span v-on="on">{{ subitemInd.TECHNIQUE | subStr }}</span>
                              </template>
                              <span>{{ subitemInd.TECHNIQUE }}</span>
                            </v-tooltip>
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

      <v-row>
        <v-col :cols="12">
          <v-data-table
            ref="outputTable"
            :headers="headersOUTP"
            :items="resultArrOutp"
            sort-by=""
            multi-sort
            :items-per-page="-1"
            :hide-default-footer="true"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedOutp"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.output-indicators", 2) }}
                </h4>
                <v-spacer></v-spacer>
                <v-tooltip top v-if="false"
                  >>
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3 mx-1"
                      small
                      v-on="on"
                      @click="downloadTableDataOutputInd"
                    >
                      <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                  </template>
                  <span>{{ $t("general.download-table-data") }}</span>
                </v-tooltip>

                <!-- <v-text-field
                  v-model="tableFilterOUTP"
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
                          <span v-on="on" style="width: 15.38% !important">{{
                            item.OUTCOME | subStr
                          }}</span>
                        </template>
                        <span>{{ item.OUTCOME }} </span>
                      </v-tooltip>
                    </td>
                    <td :colspan="6" style="margin: 0px; padding: 0px">
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
                            :style="{ width: '9.1% !important' }"
                            :class="'descClass'"
                          >
                            <span style="width: 9.1% !important">
                              {{ subitemCoiCode.PMS_COI_CODE | subStr }}
                            </span>
                          </td>

                          <td
                            :style="{
                              width: '90.9% !important',
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
                                    width: '16.66% !important',
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
                                    width: '66.64% !important',
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
                                          width: '25% !important',
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
                                              subitemProc.PMS_PROCESS | subStr
                                            }}</span>
                                          </template>
                                          <span>{{ subitemProc.PMS_PROCESS }}</span>
                                        </v-tooltip>
                                      </td>
                                      <td
                                        :style="{
                                          width: '75% !important',
                                          margin: '0px',
                                          padding: '0px',
                                        }"
                                        :class="'descClass'"
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
                                              subitemInd, iSubInd
                                            ) in subitemOut.subCatPmsOutput"
                                            :key="subitemInd.PMS_OUTPUT | getKey"
                                          >
                                            <td
                                              :style="{
                                                margin: '0px',
                                                padding: '0px',
                                              }"
                                              v-if="iSubInd === 0"
                                              :rowspan="subitemOut.subCatPmsOutput.length"
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
                                            <td
                                              :style="{
                                                margin: '0px',
                                                padding: '0px',
                                              }"
                                              :class="'descClass'"
                                            >
                                              <div v-html="subitemInd.INDICATOR"></div>

                                              <!-- <v-tooltip bottom>
                                                <template v-slot:activator="{ on }">
                                                  <span v-on="on">{{
                                                    subitemInd.INDICATOR
                                                  }}</span>
                                                </template>
                                                <span>{{ subitemInd.INDICATOR }}</span>
                                              </v-tooltip> -->
                                            </td>
                                            <td 
                                              :style="{
                                                margin: '0px',
                                                padding: '0px',
                                              }"
                                              :class="'btnDescClass'"
                                              :colspan="1"
                                            >
                                              <table
                                                :style="{
                                                  margin: '0px',
                                                  padding: '0px',
                                                  width: '100%',
                                                  border:'none'
                                                }"
                                              >
                                                <tr
                                                 :style="{
                                            
                                                      margin: '0px',
                                                      padding: '0px',
                                                      border:'none'
                                                    }"
                                                >
                                                  <td
                                                    :style="{
                                                      margin: '0px',
                                                      padding: '0px',
                                                      border:'none'
                                                    }"
                                                    :class="'descClass'"
                                                  >
                                                    <v-tooltip bottom>
                                                      <template v-slot:activator="{ on }">
                                                        <span v-on="on">{{
                                                          subitemInd.TECHNIQUE | subStr
                                                        }}</span>
                                                      </template>
                                                      <span>{{
                                                        subitemInd.TECHNIQUE
                                                      }}</span>
                                                    </v-tooltip>
                                                  </td>
                                                  <td v-if="false"
                                                    :style="{
                                                     width: '20px !important',
                                                      margin: '0px',
                                                      padding: '0px',
                                                    }"
                                                 
                                                  >
                                                    <v-btn
                                                    :style="{
                                                      width: '20px !important',
                                                      margin: '0px',
                                                      padding: '0px',
                                                    }"
                                                      color="primary"
                                                      class=" btnClass --darken-1"
                                                      @click="save()"
                                                      ><span
                                                       
                                                     :class="'btnClass'"
                                                    >S</span></v-btn
                                                    >
                                                  </td>
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
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersCCC"
            :items="resultArrccc"
            sort-by=""
            multi-sort
            :search="tableFilterCCC"
            :items-per-page="-1"
            :hide-default-footer="true"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedCcc"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.indicators-ccc", 2) }}
                </h4>
                <v-spacer></v-spacer>
                <v-tooltip top v-if="false"
                  >>
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3 mx-1"
                      small
                      v-on="on"
                      @click="downloadTableDataCCInd"
                    >
                      <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                  </template>
                  <span>{{ $t("general.download-table-data") }}</span>
                </v-tooltip>
                <!-- 
                <v-text-field
                  v-model="tableFilterCCC"
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
                    v-for="(subitemCCC, iSubCCC) in item.subCatCCC"
                    :key="subitemCCC.CCC | getKey"
                  >
                    <td
                      v-if="iSubCCC === 0"
                      :rowspan="item.subCatCCC.length"
                      :class="'descClass'"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on" style="width: 140px !important">{{
                            item.CCC | subStr
                          }}</span>
                        </template>
                        <span>{{ item.CCC }} </span>
                      </v-tooltip>
                    </td>
                    <td
                      :colspan="1"
                      style="margin: 0px; padding: 0px"
                      :class="'descClass'"
                    >
                      <span style="width: 140px !important">
                        {{ subitemCCC.INDICATOR | subStr }}
                      </span>
                    </td>
                    <td
                      :colspan="1"
                      style="margin: 0px; padding: 0px"
                      :class="'descClass'"
                    >
                      <span style="width: 140px !important">
                        {{ subitemCCC.TECHNIQUE | subStr }}
                      </span>
                    </td>
                  </tr>
                </template>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headersSSS"
            :items="resultArrsss"
            sort-by=""
            multi-sort
            :search="tableFilterSSS"
            :items-per-page="-1"
            :hide-default-footer="true"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedSss"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>
                  {{ $tc("datasource.pms.annual-plan.objectives.indicators-sss", 2) }}
                </h4>
                <v-spacer></v-spacer>
                <v-tooltip top v-if="false"
                  >>
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3 mx-1"
                      small
                      v-on="on"
                      @click="downloadTableDataSSSInd"
                    >
                      <v-icon>mdi-microsoft-excel</v-icon>
                    </v-btn>
                  </template>
                  <span>{{ $t("general.download-table-data") }}</span>
                </v-tooltip>
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
              <tbody>
                <template v-for="item in items">
                  <tr
                    v-for="(subitemSSS, iSubSSS) in item.subCatSSS"
                    :key="subitemSSS.SSS | getKey"
                  >
                    <td
                      v-if="iSubSSS === 0"
                      :rowspan="item.subCatSSS.length"
                      :class="'descClass'"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <span v-on="on" style="width: 140px !important">{{
                            item.SSS | subStr
                          }}</span>
                        </template>
                        <span>{{ item.SSS }} </span>
                      </v-tooltip>
                    </td>
                    <td
                      :colspan="1"
                      style="margin: 0px; padding: 0px"
                      :class="'descClass'"
                    >
                      <span style="width: 140px !important">
                        {{ subitemSSS.INDICATOR | subStr }}
                      </span>
                    </td>
                    <td
                      :colspan="1"
                      style="margin: 0px; padding: 0px"
                      :class="'descClass'"
                    >
                      <span style="width: 140px !important">
                        {{ subitemSSS.TECHNIQUE | subStr }}
                      </span>
                    </td>
                  </tr>
                </template>
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
      resultArrOutc: [],
      resultArrOutp: [],
      resultArrsss: [],
      resultArrccc: [],
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
      tableFilterCCC: "",
      tableFilterCCS: "",
      tableFilterCCP: "",
      selectedCC: [],
      selectedObj: [],
      selectedOutc: [],
      selectedOvg: [],
      selectedSss: [],
      selectedCcc: [],
      selectedOutp: [],
      tableFilterOVG: "",
      tableFilterOUTC: "",
      tableFilterOUTP: "",
      tblName: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headersOUTC: [],
      headersOUTP: [],
      headersOUTP_XLS: [],
      headersSSS: [],
      headersOVG: [],
      headersCCP: [],
      headersCCC: [],
      outcomeList: [] as any,
      outcomeListGrouped: [] as any,

      ovgList: [] as any,
      ovgListGrouped: [] as any,

      cccList: [] as any,
      cccListGrouped: [] as any,
      outputList: [] as any,
      outputListGrouped: [] as any,
      formData: {},
      coi: [],
      tos: [],
      overallList: [] as any,
      sssList: [] as any,
      sssListGrouped: [] as any,

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
      generateExcelForOverviewIndicators: "generateExcelForOverviewIndicators",
      generateExcelForTable: "generateExcelForTable",
      generateExcelForTableOvwInd: "generateExcelForTableOvwInd",
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

    getAnnualPlanCCCIndicators() {
      let localThis: any = this as any;
      localThis.showWait = true;
      localThis.selectedItem = null;
      localThis.indicatorList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_CC_INDICATOR";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CC_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_CC_IND",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.cccList.push(item);
            });
            localThis.groupListCCC();
            localThis.showWait = false;
          }
        }
      );
    },

    getAnnualPlanSSSIndicators() {
      let localThis: any = this as any;
      localThis.showWait = true;
      localThis.selectedItem = null;
      localThis.indicatorList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_STRAT_SUPP_SERV_INDICATOR";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_STRAT_SUPP_SERV_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_SSS_IND",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.sssList.push(item);
            });
            localThis.groupListSSS();
            localThis.showWait = false;
          }
        }
      );
    },

    getAnnualPlanOutcomeIndicators() {
      let localThis: any = this as any;
      localThis.showWait = true;

      localThis.outcomeList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_OBJ_INDICATOR";
      let wherecond: string = `ANNUAL_PLAN_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OBJ_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_OBJ_IND",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.outcomeList.push(item);
            });
          }
          localThis.groupListOutcome();
          localThis.showWait = false;
        }
      );
    },

    groupListSSS() {
      let localThis: any = this as any;
      localThis.resultArrsss = [] as any;
      localThis.sssListGrouped = [];
      localThis.sssList.forEach((item: any) => {
        let objItem = {} as any;
        objItem.SSS = item.SSS;
        objItem.INDICATOR = item.PMS_SSS_IND;
        objItem.TECHNIQUE = item.PMS_SSS_IND_TECHNIQUE;
        localThis.sssListGrouped.push(objItem);
      });

      const groupBySSS = localThis.sssListGrouped.reduce((group: any, item: any) => {
        const { SSS } = item;
        group[SSS] = group[SSS] ?? [];
        group[SSS].push(item);
        return group;
      }, {});
      Object.keys(groupBySSS).forEach((item) => {
        localThis.resultArrsss.push({
          SSS: item,
          subCatSSS: groupBySSS[item],
        });
      });
    },
    groupListCCC() {
      let localThis: any = this as any;
      localThis.resultArrccc = [] as any;
      localThis.cccListGrouped = [];
      localThis.cccList.forEach((item: any) => {
        let objItem = {} as any;
        objItem.CCC = item.CCC;
        objItem.INDICATOR = item.PMS_CC_IND;
        objItem.TECHNIQUE = item.PMS_CC_IND_TECHNIQUE;
        localThis.cccListGrouped.push(objItem);
      });

      const groupByCCC = localThis.cccListGrouped.reduce((group: any, item: any) => {
        const { CCC } = item;
        group[CCC] = group[CCC] ?? [];
        group[CCC].push(item);
        return group;
      }, {});
      Object.keys(groupByCCC).forEach((item) => {
        localThis.resultArrccc.push({
          CCC: item,
          subCatCCC: groupByCCC[item],
        });
      });
    },
    groupListOutcome() {
      let localThis: any = this as any;
      localThis.resultArrOutc = [] as any;
      localThis.outcomeListGrouped = [];
      localThis.outcomeList.forEach((item: any) => {
        let objItem = {} as any;
        objItem.PMS_OBJ_ID = item.PMS_OBJ_ID;
        objItem.OUTCOME = item.Construct_definition;
        objItem.TARGET = item.Target;
        objItem.INDICATOR = item.PMS_OBJ_IND;
        objItem.TECHNIQUE = item.PMS_OBJ_IND_TECHNIQUE;
        localThis.outcomeListGrouped.push(objItem);
      });

      const groupByTARGET = localThis.outcomeListGrouped.reduce(
        (group: any, item: any) => {
          const { TARGET } = item;
          group[TARGET] = group[TARGET] ?? [];
          group[TARGET].push(item);
          return group;
        },
        {}
      );
      Object.keys(groupByTARGET).forEach((item) => {
        localThis.resultArrOutc.push({
          TARGET: item,
          subCatTarget: groupByTARGET[item],
        });
      });

      let i: number = 0;
      for (i = 0; i < localThis.resultArrOutc.length; i++) {
        const groupByOUTCOME = localThis.resultArrOutc[i].subCatTarget.reduce(
          (group: any, item: any) => {
            const { OUTCOME } = item;
            group[OUTCOME] = group[OUTCOME] ?? [];
            group[OUTCOME].push(item);
            return group;
          },
          {}
        );
        let objOutcome: any = [];
        Object.keys(groupByOUTCOME).forEach((item) => {
          objOutcome.push({
            OUTCOME: item,
            subCatOutcome: groupByOUTCOME[item],
          });
        });
        localThis.resultArrOutc[i].subCatTarget = objOutcome;
      }

      //   let actualColor = "background-color: white";
      //   for (i = 0; i < localThis.resultArrOutc.length; i++) {
      //     let target: any = {};
      //     let targetInserted: boolean = false;
      //     target = localThis.resultArrOutc[i];
      //     let j: number = 0;
      //     for (j = 0; j < target.subCatTarget.length; j++) {
      //       let outcome: any = {};
      //       let outcomeInserted: boolean = false;
      //       outcome = target.subCatTarget[j];

      //       let itmobj: any = {};
      //       if (!targetInserted) {
      //         targetInserted = true;
      //         if (actualColor == "background-color: white") {
      //           actualColor = "background-color: whitesmoke";
      //         } else {
      //           actualColor = "background-color: white";
      //         }
      //         itmobj.TARGET = target.OUTCOME;
      //       } else {
      //         itmobj.TARGET = "";
      //       }
      //       if (!outcomeInserted) {
      //         outcomeInserted = true;
      //         if (actualColor == "background-color: white") {
      //           actualColor = "background-color: whitesmoke";
      //         } else {
      //           actualColor = "background-color: white";
      //         }

      //         itmobj.OUTCOME = outcome.OUTCOME;
      //       } else {
      //         itmobj.OUTCOME = "";
      //       }
      //       itmobj.INDICATOR = outcome.INDICATOR;
      //       localThis.objList1.push(itmobj);
      //     }
      //   }
    },

    getAnnualPlanOVGIndicators() {
      let localThis: any = this as any;
      localThis.showWait = true;
      localThis.ovgList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_OVG_INDICATOR_OVERVIEW";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      //   if (localThis.versioned > 0) {
      //     view = "PMS_VI_OBJ_INDICATOR_VER";
      //     wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      //   }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_OVG_IND",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.ovgList.push(item);
            });
          }
          //localThis.groupListOVG();
          localThis.showWait = false;
        }
      );
    },
    downloadTableDataOutputInd() {
      const localThis: any = this as any;

      // let childElement = localThis.$refs.outputTable;
      // const html = new Vue(Object.assign({}, childElement.$el)).$mount().$el;
      const fileName: string = "OVW_OUTP_IND";
      const fileExtension: string = ".xlsx";
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];

      localThis.headersOUTP
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
      let bodyValues = JSON.stringify(localThis.resultArrOutp);
      localThis
        .generateExcelForOverviewIndicators({
          colList: colList.join(","),
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          colLabels,
          bodyValues: bodyValues,
        })
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
    downloadTableDataOvgInd() {
      const localThis: any = this as any;

      // let childElement = localThis.$refs.outputTable;
      // const html = new Vue(Object.assign({}, childElement.$el)).$mount().$el;
      const fileName: string = "OVW_OVG_IND";
      const fileExtension: string = ".xlsx";
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
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
      //let bodyValues =  JSON.stringify(localThis.resultArrOutp);
      localThis
        .generateExcelForTable({
          viewName: "PMS_VI_OVG_INDICATOR_OVERVIEW",
          whereCond: wherecond,
          orderStmt: "PMS_OVG_IND",
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          colList: colList.join(","),
          colLabels: colLabels,
        })
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

    downloadTableDataOutcInd() {
      const localThis: any = this as any;

      // let childElement = localThis.$refs.outputTable;
      // const html = new Vue(Object.assign({}, childElement.$el)).$mount().$el;
      const fileName: string = "OVW_OUTC_IND";
      const fileExtension: string = ".xlsx";
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_OBJ_INDICATOR";
      let wherecond: string = `ANNUAL_PLAN_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OBJ_INDICATOR_VER";
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
      //let bodyValues =  JSON.stringify(localThis.resultArrOutp);
      localThis
        .generateExcelForTable({
          viewName: view,
          whereCond: wherecond,
          orderStmt: "PMS_OBJ_IND",
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          colList: colList.join(","),
          colLabels: colLabels,
        })
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
    downloadTableDataCCInd() {
      const localThis: any = this as any;

      let view: string = "PMS_VI_CC_INDICATOR";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CC_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      // let childElement = localThis.$refs.outputTable;
      // const html = new Vue(Object.assign({}, childElement.$el)).$mount().$el;
      const fileName: string = "OVW_CC_IND";
      const fileExtension: string = ".xlsx";
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];

      localThis.headersCCC
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
      //let bodyValues =  JSON.stringify(localThis.resultArrOutp);
      localThis
        .generateExcelForTable({
          viewName: view,
          whereCond: wherecond,
          orderStmt: "PMS_CC_IND",
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          colList: colList.join(","),
          colLabels: colLabels,
        })
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

    downloadTableDataSSSInd() {
      const localThis: any = this as any;

      let view: string = "PMS_VI_STRAT_SUPP_SERV_INDICATOR";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_STRAT_SUPP_SERV_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      // let childElement = localThis.$refs.outputTable;
      // const html = new Vue(Object.assign({}, childElement.$el)).$mount().$el;
      const fileName: string = "OVW_SSS_IND";
      const fileExtension: string = ".xlsx";
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];

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
      //let bodyValues =  JSON.stringify(localThis.resultArrOutp);
      localThis
        .generateExcelForTable({
          viewName: view,
          whereCond: wherecond,
          orderStmt: "PMS_SSS_IND",
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          colList: colList.join(","),
          colLabels: colLabels,
        })
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
    generateTableDataOvgInd() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
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

      (obj.viewName = "PMS_VI_OVG_INDICATOR_OVERVIEW"), (obj.whereCond = wherecond);
      obj.orderStmt = "PMS_OVG_IND";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Overall Goal Indicators";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataOutcInd() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_OBJ_INDICATOR";
      let wherecond: string = `ANNUAL_PLAN_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OBJ_INDICATOR_VER";
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
      obj.orderStmt = "PMS_OBJ_IND";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Outcome Indicators";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataCCCInd() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_CC_INDICATOR";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_CC_INDICATOR_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      localThis.headersCCC
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
      obj.orderStmt = "PMS_CC_IND";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Principles, Values, Criteria Cross Cutting Indicators";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataSSSInd() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_STRAT_SUPP_SERV_INDICATOR";
      let wherecond: string = `PMS_AP_ID = ${localThis.selectedAnnualPlanId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_STRAT_SUPP_SERV_INDICATOR_VER";
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
      obj.orderStmt = "PMS_SSS_IND";
      obj.officeId = localThis.getCurrentOffice.HR_OFFICE_ID;
      obj.colList = colList.join(",");
      obj.columnLabels = colLabels;
      obj.sheetName = "Strategic And Support Service Indicators";
      localThis.objDownloadStruct.push(obj);
    },
    generateTableDataOUTPInd() {
      const localThis: any = this as any;
      const colList: Array<any> = [];
      const colLabels: Array<any> = [];
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTP_IND_OVERVIEW";
      let wherecond: string = `AP_ID = ${localThis.selectedAnnualPlanId} `;
      if (localThis.versioned > 0) {
        view = "PMS_VI_ANNUAL_PLAN_OUTP_OVERVIEW_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      localThis.headersOUTP_XLS
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
      obj.sheetName = "Output Indicators";
      localThis.objDownloadStruct.push(obj);
    },
    downloadTableData() {
      const localThis: any = this as any;
      localThis.objDownloadStruct = [];
      const fileName: string = "OVW_IND";
      const fileExtension: string = ".xlsx";
      localThis.generateTableDataOvgInd();
      localThis.generateTableDataOutcInd();
      localThis.generateTableDataOUTPInd();
      localThis.generateTableDataCCCInd();
      localThis.generateTableDataSSSInd();
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

    getAnnualPlanOutputIndicators() {
      let localThis: any = this as any;
      localThis.outputList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_ANNUAL_PLAN_OUTP_IND_OVERVIEW";
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
              objItem.SRV_ID = item.SRV_ID;
              objItem.ACT_ID = item.ACT_ID;
              objItem.PMS_COI_ID = item.PMS_COI_ID;
              objItem.PMS_TOS_ID = item.PMS_TOS_ID;
              objItem.IND_NUMBER = item.IND_NUMBER;

              objItem.OUTCOME = item.OUTCOME + "";

              objItem.PMS_COI_CODE = item.PMS_COI_CODE + "";
              objItem.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION + "";
              objItem.PMS_TOS_CODE = item.PMS_TOS_CODE + "";
              objItem.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION + "";
              objItem.PMS_PROCESS = item.PROCESS + "";
              objItem.PMS_OUTPUT = item.OUTPUT + "";
              objItem.PMS_PROCESS_ID = item.PROCESS_ID;
              objItem.PMS_OUTPUT_ID = item.OUTPUT_ID;
              if (item.INDICATOR != undefined) {
                objItem.INDICATOR = "<b>" + item.INDICATOR + "</b>";
                if (item.TARGET != "") objItem.INDICATOR += "<br>Target:" + item.TARGET;
                if (item.DISAGGREGATION != "")
                  objItem.INDICATOR += "<br>" + item.DISAGGREGATION;
                if (item.DISABLED != undefined)
                  objItem.INDICATOR += "<br>Disabilited:" + item.DISABLED;
                if (item.AGE_RANGE != "")
                  objItem.INDICATOR += "<br>Age Range:" + item.AGE_RANGE;
              } else {
                objItem.INDICATOR = "";
              }

              objItem.TECHNIQUE = item.TECHNIQUE;

              localThis.outputList.push(objItem);
              localThis.selected.push(false);
            });
            localThis.groupListOutput();
          }
          localThis.showWait = false;
        }
      );
    },
    groupListOutput() {
      let localThis: any = this as any;
      localThis.resultArrOutp = [] as any;
      localThis.outputListGrouped = [];
      localThis.outputList.forEach((item: any) => {
        let objItem = {} as any;
        objItem.PMS_ACTIVITY_OUTPUT_REL_ID = item.PMS_ACTIVITY_OUTPUT_REL_ID;
        objItem.OUTCOME = item.OUTCOME;
        objItem.PMS_COI_CODE = item.PMS_COI_CODE;
        //objItem.PMS_COI_DESCRIPTION = item.PMS_COI_DESCRIPTION;
        //objItem.PMS_TOS_CODE = item.PMS_TOS_CODE;
        objItem.PMS_TOS_DESCRIPTION = item.PMS_TOS_DESCRIPTION;
        objItem.PMS_PROCESS = item.PMS_PROCESS;
        objItem.PMS_OUTPUT = item.PMS_OUTPUT;
        objItem.INDICATOR = item.INDICATOR;
        objItem.TECHNIQUE = item.TECHNIQUE;
        localThis.outputListGrouped.push(objItem);
      });
      const groupByOUTCOME = localThis.outputListGrouped.reduce(
        (group: any, item: any) => {
          const { OUTCOME } = item;
          group[OUTCOME] = group[OUTCOME] ?? [];
          group[OUTCOME].push(item);
          return group;
        },
        {}
      );
      Object.keys(groupByOUTCOME).forEach((item) => {
        localThis.resultArrOutp.push({
          OUTCOME: item,
          subCatOutcome: groupByOUTCOME[item],
        });
      });

      let i: number = 0;
      for (i = 0; i < localThis.resultArrOutp.length; i++) {
        const groupByPMS_COI_CODE = localThis.resultArrOutp[i].subCatOutcome.reduce(
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
        localThis.resultArrOutp[i].subCatOutcome = objCoiCode;
      }

      localThis.resultArrOutp.forEach((itemArray: any) => {
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

      localThis.resultArrOutp.forEach((itemArray: any) => {
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
                PMS_PROCESS: item,
                subCatPmsProcess: groupByPMS_PROCESS[item],
              });
            });

            itemsPmsTosDescription.subCatPmsTosDescription = objPmsProcess;
          });
        });
      });

      localThis.resultArrOutp.forEach((itemArray: any) => {
        itemArray.subCatOutcome.forEach((itemsPmsCoiCode: any) => {
          itemsPmsCoiCode.subCatPmsCoiCode.forEach((itemsPmsTosDescription: any) => {
            itemsPmsTosDescription.subCatPmsTosDescription.forEach(
              (itemsPmsProcDescription: any) => {
                const groupByPMS_OUTPUT = itemsPmsProcDescription.subCatPmsProcess.reduce(
                  (group: any, item: any) => {
                    const { PMS_OUTPUT } = item;
                    group[PMS_OUTPUT] = group[PMS_OUTPUT] ?? [];
                    group[PMS_OUTPUT].push(item);
                    return group;
                  },
                  {}
                );

                let objPmsOutput: any = [];
                Object.keys(groupByPMS_OUTPUT).forEach((item) => {
                  objPmsOutput.push({
                    PMS_OUTPUT: item,
                    subCatPmsOutput: groupByPMS_OUTPUT[item],
                  });
                });

                itemsPmsProcDescription.subCatPmsProcess = objPmsOutput;
              }
            );
          });
        });
      });

      //   let actualColor = "background-color: white";
      //   for (i = 0; i < localThis.resultArrOutp.length; i++) {
      //     let outcome: any = {};
      //     let outcomeInserted: boolean = false;
      //     outcome = localThis.resultArrOutp[i];
      //     let j: number = 0;
      //     for (j = 0; j < outcome.subCatOutcome.length; j++) {
      //       let coicode: any = {};
      //       let coicodeInserted: boolean = false;
      //       coicode = outcome.subCatOutcome[j];
      //       let k: number = 0;
      //       for (k = 0; k < coicode.subCatPmsCoiCode.length; k++) {
      //         let q: number = 0;
      //         let tos: any = {};
      //         let tosInserted: boolean = false;
      //         tos = coicode.subCatPmsCoiCode[k];
      //         for (q = 0; q < tos.subCatPmsTosDescription.length; q++) {
      //           let z: number = 0;
      //           let proc: any = {};
      //           let procInserted: boolean = false;
      //           proc = tos.subCatPmsTosDescription[q];
      //           for (z = 0; z < proc.subCatPmsProcess.length; z++) {
      //             let t: number = 0;
      //             let outInserted: boolean = false;
      //             let out: any = proc.subCatPmsProcess[z];
      //             for (t = 0; t < out.subCatPmsOutput.length; t++) {
      //               let ind: any = out.subCatPmsOutput[t];

      //               let itmobj: any = {};
      //               if (!outcomeInserted) {
      //                 outcomeInserted = true;
      //                 if (actualColor == "background-color: white") {
      //                   actualColor = "background-color: whitesmoke";
      //                 } else {
      //                   actualColor = "background-color: white";
      //                 }

      //                 itmobj.OUTCOME = outcome.OUTCOME;
      //               } else {
      //                 itmobj.OUTCOME = "";
      //               }
      //               itmobj.OUTCOMEBACKCOLOR = actualColor;
      //               if (!coicodeInserted) {
      //                 coicodeInserted = true;
      //                 itmobj.PMS_COI_CODE = coicode.PMS_COI_CODE;
      //               } else {
      //                 itmobj.PMS_COI_CODE = "";
      //               }
      //               if (!tosInserted) {
      //                 tosInserted = true;
      //                 itmobj.PMS_TOS_DESCRIPTION = tos.PMS_TOS_DESCRIPTION;
      //               } else {
      //                 itmobj.PMS_TOS_DESCRIPTION = "";
      //               }

      //               if (!procInserted) {
      //                 procInserted = true;
      //                 itmobj.PMS_PROCESS = proc.PMS_PROCESS;
      //               } else {
      //                 itmobj.PMS_PROCESS = "";
      //               }

      //               if (!outInserted) {
      //                 outInserted = true;
      //                 itmobj.PMS_OUTPUT = out.PMS_OUTPUT;
      //               } else {
      //                 itmobj.PMS_OUTPUT = "";
      //               }
      //               itmobj.INDICATOR = ind.INDICATOR;
      //               localThis.objList1.push(itmobj);
      //             }
      //           }
      //         }
      //       }
      //     }
      //   }

      // console.log(localThis.resultArrOutp);
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
    //   for (i = 0; i < localThis.outputList.length; i++) {
    //     var exportItemobj = {} as any;
    //     var itmobj = {} as any;
    //     itmobj = localThis.outputList[i];
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
      descWidth = "140px";

      let tmpOUTC: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-obj-target").toString(),
          value: "Target",
          align: "center",
          divider: true,
          width: descWidth,
        },

        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.outcome-obj-title")
            .toString(),
          value: "Construct_definition",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "PMS_OBJ_IND",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("pms.indicator.technique").toString(),
          value: "PMS_OBJ_IND_TECHNIQUE",
          align: "center",
          divider: true,
          width: descWidth,
        },
      ];

      localThis.headersOUTC = tmpOUTC;
      let percWidht: string = "15.38%";
      let percWidht2: string = "15.38%";
      let tmpOUTP: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.outcome-obj-title")
            .toString(),
          value: "OUTCOME",
          align: "center",
          divider: true,
          width: percWidht,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-code").toString(),
          value: "PMS_COI_CODE",
          align: "center",
          divider: true,
          width: "7.69%",
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
          width: percWidht2,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.activity").toString(),
          value: "PROCESS",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.output").toString(),
          value: "OUTPUT",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "INDICATOR",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.technique").toString(),
          value: "TECHNIQUE",
          align: "center",
          divider: true,
          width: percWidht2,
        },
      ];

      localThis.headersOUTP = tmpOUTP;

      let tmpOUTP1: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.outcome-obj-title")
            .toString(),
          value: "OUTCOME",
          align: "center",
          divider: true,
          width: percWidht,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.coi-code").toString(),
          value: "PMS_COI_CODE",
          align: "center",
          divider: true,
          width: "7.69%",
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
          width: percWidht2,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.activity").toString(),
          value: "PROCESS",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.output").toString(),
          value: "OUTPUT",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "INDICATOR",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.target").toString(),
          value: "TARGET",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.disaggregation").toString(),
          value: "DISAGGREGATION",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.disabled").toString(),
          value: "DISABLED",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.age_range").toString(),
          value: "AGE_RANGE",
          align: "center",
          divider: true,
          width: percWidht2,
        },
        {
          text: this.$i18n.t("pms.indicator.technique").toString(),
          value: "TECHNIQUE",
          align: "center",
          divider: true,
          width: percWidht2,
        },
      ];

      localThis.headersOUTP_XLS = tmpOUTP1;

      let tmpSSS: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.strat-supp-serv")
            .toString(),
          value: "SSS",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "PMS_SSS_IND",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("pms.indicator.technique").toString(),
          value: "PMS_SSS_IND_TECHNIQUE",
          align: "center",
          divider: true,
          width: descWidth,
        },
      ];

      localThis.headersSSS = tmpSSS;

      let tmpOVG: JrsHeader[] = [
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.overall-goal")
            .toString(),
          value: "OVG",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "PMS_OVG_IND",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("pms.indicator.technique").toString(),
          value: "PMS_OBJ_IND_TECHNIQUE",
          align: "center",
          divider: true,
          width: descWidth,
        },
      ];

      localThis.headersOVG = tmpOVG;
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
          value: "CCC",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.indicator.indicator").toString(),
          value: "PMS_CC_IND",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("pms.indicator.technique").toString(),
          value: "PMS_CC_IND_TECHNIQUE",
          align: "center",
          divider: true,
          width: descWidth,
        },
      ];

      localThis.headersCCC = tmpCC;
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
      // localThis.getAnnualPlanOverallGoals();
      localThis.getAnnualPlanOVGIndicators();
      localThis.getAnnualPlanSSSIndicators();
      localThis.getAnnualPlanCCCIndicators();
      localThis.getAnnualPlanOutputIndicators();
      localThis.getAnnualPlanOutcomeIndicators();
      // localThis.getAnnualPlanSSSs();
      // localThis.getAnnualPlanCCSs();
      // localThis.getAnnualPlanCCPs();
      //localThis.getAnnualPlanCCs();
    }
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 50) return string.substring(0, 50) + "...";
      else return string;
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
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;

      localThis.getAnnualPlanSSSIndicators();
      localThis.getAnnualPlanCCCIndicators();
      localThis.getAnnualPlanOVGIndicators();
      localThis.getAnnualPlanOutputIndicators();
      localThis.getAnnualPlanOutcomeIndicators();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showObjDetails = to;
    // },
    selectedAnnualPlanId(to: number) {
      let localThis: any = this as any;
      localThis.selectedAnnualPlanId = to;
      localThis.getAnnualPlanSSSIndicators();
      localThis.getAnnualPlanCCCIndicators();
      localThis.getAnnualPlanOVGIndicators();
      localThis.getAnnualPlanOutputIndicators();
      localThis.getAnnualPlanOutcomeIndicators();
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
.btnDescClass {
  font-size: 10px;
  width: 140px !important;
  min-width: 140px !important;
  height: 10px;
  border: thin;
  border-style: solid;
  border-color: gainsboro;
  margin: 0px;
  padding: 0px;
}
.btnClass {
  font-size: 10px;
 width: 20px !important;
 min-width:20px !important;
  height: 10px;
  border: thin;
  border-style: none;
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
