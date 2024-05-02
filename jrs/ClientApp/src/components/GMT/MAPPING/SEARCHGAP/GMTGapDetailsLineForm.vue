<template>
  <div id="bl">
    <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        idx
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.PMS_JRS_COA_CODE
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">{{
        localBudgetLine.PMS_JRS_COA_DESCRIPTION
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.UNIT_TYPE
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.UNIT
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.TIME_UNIT
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.TIMING
      }}</v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 1">{{
        localBudgetLine.PERCENTAGE
      }}</v-col>
      <v-col
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        :class="
          !localBudgetLine.MODIFIED.includes(true) ? 'dataStyle' : 'rowStyleModified'
        "
      >
        <currency-input
          :style="{ 'font-size': '12px', 'font-weight': 'bold' }"
          v-model="localBudgetLine.YEARBUDGET"
          v-bind="options"
          :readonly="true"
        ></currency-input>
      </v-col>
      <v-col :class="'dataStyle'" :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="ShowDonors()" v-on="on"
              >mdi-google-circles-extended</v-icon
            >
          </template>
          <span>{{ $t("datasource.gmt.budget.gap-mapping-details") }}</span>
        </v-tooltip>
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="mapItem()" v-on="on">mdi-cash-usd</v-icon>
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.modify-budget-details")
          }}</span>
        </v-tooltip>
        <!-- <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="saveBudget()" v-on="on"
              >mdi-content-save</v-icon
            >
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.save-inline-modifies")
          }}</span>
        </v-tooltip>
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon small class="mr-4" @click="deleteBudget()" v-on="on"
              >mdi-delete</v-icon
            >
          </template>
          <span>{{
            $t("datasource.pms.annual-plan.objectives.delete-budget-line")
          }}</span>
        </v-tooltip> -->
      </v-col>
    </v-row>
    <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <template v-for="n in 12">
        <v-col
          :key="n"
          :class="'headerStyleGapGrid'"
          :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
        >
          <label for="n" class="vlabel">{{ rowDesk[n] }}</label>
        </v-col>
      </template>
    </v-row>
    <v-row :key="rowKey" :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
      <v-col
        v-for="n in 12"
        :class="!localBudgetLine.MODIFIED[n - 1] ? 'rowStyle' : 'rowStyleModified'"
        :key="n"
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
      >
        <div
          v-if="localBudgetLine.SELECTED[n - 1]"
          :class="CellColor(localBudgetLine, n)"
        >
          <v-row align="center" justify="center">
            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
              :class="CellColor(localBudgetLine, n)"
              class="d-flex justify-center"
            >
              <span
                >{{
                  round(
                    (100 * localBudgetLine.MONTHFUNDED[n - 1]) /
                      localBudgetLine.MONTHBUDGET[n - 1],
                    2
                  )
                }}%</span
              >
            </v-col>
          </v-row>
          <v-row align="center" justify="center">
            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
              :class="CellColor(localBudgetLine, n)"
              class="d-flex justify-center"
            >
              <span
                >{{ localBudgetLine.MONTHFUNDED[n - 1] }}/{{
                  round(localBudgetLine.MONTHBUDGET[n - 1], 2)
                }}</span
              >
            </v-col>
          </v-row>
        </div>
        <div v-else :class="'NoGapCell'">
          <v-row align="center" justify="center">
            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
              :class="'NoGapCell'"
              class="d-flex justify-center"
            >
              <span>N/A</span>
            </v-col>
          </v-row>
          <v-row align="center" justify="center">
            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
              :class="'NoGapCell'"
              class="d-flex justify-center"
            >
              <span>0</span>
            </v-col>
          </v-row>
        </div>
      </v-col>
    </v-row>
    <div class="text-center" v-if="showWaitGeneral" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-row :class="'mappingDonorGrid'">
      <v-col
        :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
        style="padding-top: 0px !important; padding-bottom: 0px !important"
      >
        <div v-if="donorListMode">
          <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
              >Donor</v-col
            >
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
              >Grant</v-col
            >
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
              >Initial</v-col
            >
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
              >Actual</v-col
            >
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            ></v-col>
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            ></v-col>
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
            ></v-col>
            <v-col
              :class="'headerStyleGapGrid'"
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
            >
            </v-col>
          </v-row>

          <v-row
            v-for="(donor, n) in donorList"
            :key="n"
            :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
            :class="'divideWithBorders'"
          >
            <v-col
              :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
              style="padding-top: 0px !important; padding-bottom: 0px !important"
            >
              <v-row
                :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
                style="padding: 0px !important"
              >
                <v-col
                  :class="'dataStyleClear'"
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                  >{{ donor.GMT_DONOR_NAME }}</v-col
                >
                <v-col
                  :class="'dataStyleClear'"
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                  >{{ donor.GRANT_CODE }}
                </v-col>
                <v-col
                  :class="'dataStyleClear'"
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                >
                  <currency-input
                    v-model="donor.GRANT_INITIAL_AMOUNT"
                    v-bind="options"
                    :currency="donor.GRANT_CURRENCY_CODE"
                    :readonly="true"
                  ></currency-input>
                </v-col>
                <v-col
                  :class="'dataStyleClear'"
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                >
                  <currency-input
                    v-model="donor.AVAILABLE"
                    v-bind="options"
                    :currency="donor.GRANT_CURRENCY_CODE"
                    :readonly="true"
                  ></currency-input>
                </v-col>
                <v-col
                  :class="'dataStyleClear'"
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 3"
                >
                </v-col>
                <v-col
                  :class="'dataStyleClear'"
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                >
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-icon
                        small
                        class="mr-4"
                        @click="ShowDonorBudgetLines(n)"
                        v-on="on"
                        >mdi-google-circles-extended</v-icon
                      >
                    </template>
                    <span>{{
                      $t("datasource.pms.annual-plan.objectives.modify-budget-details")
                    }}</span>
                  </v-tooltip>
                </v-col>
              </v-row>
              <v-row :class="'mappingDetailsGrid'">
                <v-col
                  :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
                  style="padding-top: 0px !important; padding-bottom: 0px !important"
                >
                  <div
                    v-if="
                      mpDonorLineListMode[n] != undefined &&
                      mpDonorLineListMode[n].showData
                    "
                  >
                    <v-row
                      :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
                      style="padding: 0px !important"
                    >
                      <v-col
                        :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
                        style="
                          padding-top: 0px !important;
                          padding-bottom: 0px !important;
                        "
                      >
                        <div style="margin: 0px; padding: 0px">
                          <v-row
                            :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
                            style="padding: 0px !important"
                          >
                            <v-col
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              :class="'mappingDetailsGrid'"
                            ></v-col>

                            <v-col
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                              :class="'headerStyleGapGrid'"
                            >
                              <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> OBJ </span>
                                </template>
                                <span>{{
                                  $t("datasource.pms.annual-plan.objectives.desc")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                              :class="'headerStyleGapGrid'"
                              >   <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> SRV </span>
                                </template>
                                <span>{{
                                  $t("datasource.pms.annual-plan.objectives.service")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                              :class="'headerStyleGapGrid'"
                              > <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> ACT </span>
                                </template>
                                <span>{{
                                  $t("datasource.pms.annual-plan.objectives.activity")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :class="'headerStyleGapGrid'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              ><v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> COI </span>
                                </template>
                                <span>{{
                                  $t("datasource.pms.category-of-intervention.category-of-intervention")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :class="'headerStyleGapGrid'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              ><v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> TOS </span>
                                </template>
                                <span>{{
                                  $t("datasource.pms.type-of-service.type-of-service")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :class="'headerStyleGapGrid'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              ><v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> COA Code </span>
                                </template>
                                <span>{{
                                  $t("pms.donor-coa-code")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :class="'headerStyleGapGrid'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              ><v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <span v-on="on"> COA Desc </span>
                                </template>
                                <span>{{
                                  $t("pms.donor-coa-descr")
                                }}</span>
                              </v-tooltip></v-col
                            >
                            <v-col
                              :class="'dataStyle'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                            >
                              <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <v-icon
                                    small
                                    class="mr-4"
                                    @click="AddDonorBudgetLines(n)"
                                    v-on="on"
                                    >mdi-playlist-plus</v-icon
                                  >
                                </template>
                                <span>{{
                                  $t(
                                    "datasource.pms.annual-plan.objectives.modify-budget-details"
                                  )
                                }}</span>
                              </v-tooltip>
                            </v-col>
                          </v-row>
                          <v-row
                            v-for="(map, k) in mappedDonorLineList[n]"
                            :key="k"
                            :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
                          >
                            <v-col
                              :class="'dataStyleLevel1'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                            ></v-col>
                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                            >{{ map.OBJECTIVE_DESC }}</v-col
                            >

                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                            >{{ map.SERVICE_DESC }}</v-col
                            >
                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 2"
                            >{{ map.ACTIVITY_DESC }}</v-col
                            >
                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              >{{ map.GMT_COI_CODE }}</v-col
                            >
                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              >{{ map.GMT_TOS_DESCRIPTION }}</v-col
                            >
                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              >{{ map.ACC_CODE }}</v-col
                            >
                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                              >{{ map.ACC_DESCRIPTION }}</v-col
                            >

                            <v-col
                              :class="'dataStyleClear'"
                              :cols="$vuetify.breakpoint.xsOnly ? 12 : 1"
                            >
                              <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                  <v-icon
                                    small
                                    class="mr-4"
                                    @click="DeleteBudgetLine(n)"
                                    v-on="on"
                                    >mdi-delete</v-icon
                                  >
                                </template>
                                <span>{{
                                  $t(
                                    "datasource.pms.annual-plan.objectives.modify-budget-details"
                                  )
                                }}</span>
                              </v-tooltip>
                            </v-col>
                          </v-row>
                        </div>
                      </v-col>
                    </v-row>
                    <v-dialog
                      v-model="matchMode"
                      persistent
                      retain-focus
                      :scrollable="true"
                      :overlay="false"
                      transition="dialog-transition"
                    >
                      <gmt-coi-tos-coa
                        v-model="linkedData"
                        @addRelation="closeAndUpdateCOITOSCOADialog"
                        @closeDialoge="closeCOITOSCOADialog"
                        :formData="formData"
                        :donorId="donor.GMT_DONOR_ID"
                        :grantId="donor.GRANT_ID"
                        :key="Math.ceil(Math.random() * 1000)"
                      ></gmt-coi-tos-coa>
                    </v-dialog>
                  </div>
                  <div
                    class="text-center"
                    v-if="
                      mpDonorLineListMode[n] != undefined &&
                      mpDonorLineListMode[n].showWait
                    "
                    style="margin: 0px; padding: 0px"
                  >
                    <v-progress-circular
                      indeterminate
                      color="primary"
                      :key="n"
                    ></v-progress-circular>
                  </div>
                </v-col>
              </v-row>
            </v-col>
          </v-row>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col></v-col>
    </v-row>
    <!-- <v-dialog
      v-model="donorListMode"
      persistent
      retain-focus
      :scrollable="true"
      :overlay="false"
      transition="dialog-transition"
    >
      <gmt-coi-tos-coa
        v-model="linkedData"
        @UpdateBudgetItem="closeBudgetDialog"
        @closeDialoge="closeLinkedDataDialog"
        :formData="formData"
        :donorId="donor.GMT_DONOR_ID"
        :key="Math.ceil(Math.random() * 1000)"
      ></gmt-coi-tos-coa>
    </v-dialog> -->
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";
import ItemBudget from "./../../../PMS/BUDGET/AnnualPlanItemBudgetForm.vue";
import matchCOITOSCOA from "./GMTGapLinkCOITOSCOAForm.vue";
import FormMixin from "../../../../mixins/FormMixin";
import {
  IBudgetItemGET,
  IBudgetItemUPD,
  IPayLoadDataITEM,
} from "./../../../PMS/PMSSharedInterfaces";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
interface IShow {
  showData: boolean;
  showWait: boolean;
}
export default Vue.extend({
  components: {
    "gmt-coi-tos-coa": matchCOITOSCOA,
  },
  props: {
    selectedActivityId: {
      type: Number,
      required: false,
    },
    resource: {
      type: String,
      required: false,
    },
    resource_id: {
      type: Number,
      required: false,
    },
    budgetLine: {
      type: Object,
      required: true,
    },
    idx: {
      type: Number,
      required: true,
    },
    annual_plan_id: {
      type: Number,
      required: true,
    },
    year: {
      type: Number,
      required: true,
    },
    currencyCode: {
      type: String,
      required: true,
    },
    donorList: {
      type: Array,
      required: true,
    },
    formData: {
      type: Object,
      required: true,
    },
  },

  data() {
    return {
   
      toBeReloaded: 0,
      linkedData: {},
      matchMode: false,
      showWaitGeneral: false,
      mpDonorLineListMode: [],
      donorListMode: false,
      localBudgetLine: {},
      mappedDonorLineList: [],

      bdg: 0,
      percentage: 0,
      locale: "en",
      actDesc: "",
      rowKey: 0,
      rowDesk: [],
      selected: [],
      budget: [],
    };
  },
  mixins: [FormMixin],
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
        currency: localThis.currencyCode,
        // ,
        // valueRange: this.valueRangeEnabled
        //   ? { min: this.valueRange[0], max: this.valueRange[1] }
        //   : undefined,
        // precision: this.precisionEnabled
        //   ? (this.precisionRangeEnabled ? { min: this.precisionRange[0], max: this.precisionRange[1] } : this.precisionFixed)
        //   : undefined,
        // distractionFree: this.distractionFree
        //   ? {
        //     hideNegligibleDecimalDigits: this.hideNegligibleDecimalDigits,
        //     hideCurrencySymbol: this.hideCurrencySymbol,
        //     hideGroupingSymbol: this.hideGroupingSymbol
        //   } : false,
        // autoDecimalMode: this.autoDecimalMode,
        // valueAsInteger: this.valueAsInteger,
        // allowNegative: this.allowNegative
      };
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.locale = newLanguage;
      localThis.options.locale = newLanguage;
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    DeleteBudgetLine() {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },

    CellColor(item: any, n: number) {
      let value = item.MONTHFUNDED[n - 1] / item.MONTHBUDGET[n - 1];
      if (value > 1) {
        return "OverGapCell";
      } else {
        if (value == 1) return "NoGapCell";
        else return "GapCell";
      }
    },
    ShowDonorBudgetLines(n: number) {
      let localThis = this as any;
      if (localThis.mpDonorLineListMode[n].showData == false) {
        localThis.mpDonorLineListMode[n].showData = true;
        if (localThis.mappedDonorLineList[n] == null) localThis.getBudgetLine(n);
      } else localThis.mpDonorLineListMode[n].showData = false;
      localThis.$emit("UpdateFormData");
    },

    AddDonorBudgetLines(n: number) {
      let localThis = this as any;
      localThis.$emit("UpdateFormData");
      localThis.toBeReloaded = n;
      localThis.matchMode = true;
    },

    ShowDonors() {
      let localThis = this as any;
      localThis.donorListMode = !localThis.donorListMode;
    },
    mapItem() {
      let localThis = this as any;
      let obj: any = {};
      obj.RESOURCE_ID = localThis.localBudgetLine.PMS_JRS_COA_ID;
      obj.ACTIVITY_ID = localThis.localBudgetLine.ACTIVITY_ID;
      localThis.$emit("UpdateMap");
    },

    closeAndUpdateCOITOSCOADialog(item: any) {
      let localThis: any = this as any;
      localThis.matchMode = false;
      localThis.addRelation(item);
      (this as any).$emit("UpdateBudgetList");
    },
    budgetItem(item: any) {
      let localThis = this as any;
      localThis.matchMode = !localThis.matchMode;
    },
    closeCOITOSCOADialog() {
      let localThis = this as any;
      localThis.matchMode = false;
    },
    refreshTotal(idx: number) {
      let localThis: any = this as any;
      localThis.bdg = 0;
      localThis.localBudgetLine.MODIFIED[idx] = true;
      localThis.bdg = localThis.localBudgetLine.MONTHBUDGET.reduce(
        (acc: any, item: any) => acc + item,
        0
      );
      localThis.localBudgetLine.YEARBUDGET = localThis.bdg;
    },

    UndoModifies() {
      let localThis: any = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-undo-modifies")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then((res) => {
        localThis.updateValue();
      });
    },
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    splitBudgetAllMonths() {
      let localThis: any = this as any;
      localThis.budget = [];
      let i: number;
      for (i = 0; i < 12; i++) {
        localThis.budget.push(localThis.bdg / 12);
        localThis.selected[i] = true;
      }
    },

    addRelation(item: any) {
      let localThis = this as any;
      localThis.showWaitGeneral = true;
      let payLoadD = {} as any;
      
      // payLoadD.DONOR_COI_ID = item.COI_ID;
      payLoadD.DONOR_COA_ID = item.COA_ID;
      // payLoadD.DONOR_TOS_ID = item.TOS_ID;
      payLoadD.DNR_ID = item.DONOR_ID;
      payLoadD.ACTIVITY_ID = localThis.localBudgetLine.ACTIVITY_ID;
      payLoadD.GMT_ACTIVITY_ID = item.GMT_ACTIVITY_ID;
      payLoadD.JRS_COI_ID = localThis.formData.PMS_JRS_COI_ID;
      payLoadD.JRS_COA_ID = localThis.formData.PMS_JRS_COA_ID;
      payLoadD.JRS_TOS_ID = localThis.formData.PMS_JRS_TOS_ID;
      payLoadD.GRANT_PRJ_REL_ID =
        localThis.donorList[localThis.toBeReloaded].GRANT_PRJ_REL_ID;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_MAPPING_SG",
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
          localThis.mappedDonorLineList[localThis.toBeReloaded] = null;
          localThis.mpDonorLineListMode[localThis.toBeReloaded].showData = false;
          localThis.ShowDonorBudgetLines(localThis.toBeReloaded);
        })
        .then((res: any) => {
          localThis.showWaitGeneral = false;
        })
        .catch((err: any) => {
          localThis.showWaitGeneral = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    deleteBudget() {
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-budget-line")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as IPayLoadDataITEM;
          payLoadD.ID_ANNUAL_PLAN = localThis.annual_plan_id;
          payLoadD.ID_ACTIVITY_ITEM = localThis.localBudgetLine.PMS_JRS_COA_ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_ANNUAL_PLAN_RESOURCE_BUDGET",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.updateValue();
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },

    editItem() {},
    splitBudgetSelectedMonths() {
      let localThis: any = this as any;
      localThis.budget = [];
      var count = localThis.selected.filter(function (s: any) {
        return s;
      }).length;
      let i: number;
      for (i = 0; i < 12; i++) {
        if (localThis.selected[i] == true) localThis.budget.push(localThis.bdg / count);
        else localThis.budget.push(0);
      }
    },
    saveBudget() {
      let localThis: any = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-activity-budget")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then((res) => {
        if (res) {
          let payLoad = {} as IBudgetItemUPD;
          let i: number;
          payLoad.ACTIVITY_ID = localThis.localBudgetLine.ACTIVITY_ID;
          payLoad.RESOURCE_ID = localThis.localBudgetLine.PMS_JRS_COA_ID;
          payLoad.POSITION_ID = 0;
          payLoad.PERCENTAGE = localThis.localBudgetLine.PERCENTAGE;
          payLoad.YEAR = localThis.localBudgetLine.YEAR;
          payLoad.SELECTED = "";
          payLoad.UNIT_PRICE = localThis.localBudgetLine.UNIT_PRICE;
          payLoad.TIME_UNIT = localThis.localBudgetLine.TIME_UNIT;
          payLoad.TIMING = localThis.localBudgetLine.TIMING;
          payLoad.UNIT = localThis.localBudgetLine.UNIT;
          payLoad.UNIT_TYPE = localThis.localBudgetLine.UNIT_TYPE;
          payLoad.CURRENCY = localThis.localBudgetLine.CURRENCY;
          for (i = 0; i < localThis.localBudgetLine.SELECTED.length; i++) {
            if (localThis.localBudgetLine.SELECTED[i] == true) {
              if (localThis.localBudgetLine.MONTHBUDGET[i] != null) {
                payLoad.SELECTED +=
                  i +
                  1 +
                  ";" +
                  localThis.localBudgetLine.MONTHBUDGET[i] +
                  "^" +
                  localThis.localBudgetLine.CURRENCY +
                  ":";
              }
            }
          }
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_ACTIVITY_BUDGET",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoad),
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
            })
            .then((res: any) => {
              localThis.updateValue();
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    updateValue() {
      (this as any).$emit("UpdateBudgetList");
    },
    getBudgetLine(n: number) {
      let localThis: any = this as any;
      localThis.mpDonorLineListMode[n].showWait = true;
      let donor = localThis.donorList[n];
      localThis.mappedDonorLineList[n] = [];

      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ACTIVITY_BUDGET_LINES",
        colList: null,
        whereCond: `DONOR_ID = ${localThis.donorList[n].GMT_DONOR_ID} AND GRANT_PRJ_REL_ID = ${localThis.donorList[n].GRANT_PRJ_REL_ID}  AND ACTIVITY_ID = ${localThis.localBudgetLine.ACTIVITY_ID} AND JRS_COA_ID = ${localThis.localBudgetLine.PMS_JRS_COA_ID} AND JRS_COI_ID = ${localThis.localBudgetLine.PMS_JRS_COI_ID} AND JRS_TOS_ID = ${localThis.localBudgetLine.PMS_JRS_TOS_ID}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.mappedDonorLineList[n].push(item);
            });
          }
          localThis.rowKey = localThis.getRnd();
        })
        .then((res: any) => {
          localThis.mpDonorLineListMode[n].showWait = false;
          // let obj = {} as any;
          // obj.COA_ID = n;
          // obj.TOS_ID = n;
          // obj.COI_ID = n;
          // localThis.mappedDonorLineList[n].push(obj);
          // let obj1 = {} as any;
          // obj1.COA_ID = n + 1;
          // obj1.TOS_ID = n + 1;
          // obj1.COI_ID = n + 1;
          //localThis.mappedDonorLineList[n].push(obj1);
        })
        .catch((err: any) => {
          localThis.mpDonorLineListMode[n].showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        });
    },
    // getBudget() {
    //   let localThis: any = this as any;
    //   localThis.resetSelected();
    //   var budgetList: any = [];
    //   localThis.budget = [];
    //   let i: number = 0;
    //   for (i = 0; i < 12; i++) {
    //     localThis.budget[i] = null;
    //     localThis.selected[i] = false;
    //   }

    //   let j: number = 0;
    //   let selectPayload: GenericSqlSelectPayload = {
    //     viewName: "PMS_VI_ACTIVITY_BUDGET",
    //     colList: null,
    //     whereCond: `ACTIVITY_ID = ${localThis.selectedActivityId} AND RESOURCE_ID = ${localThis.resource_id} AND YEAR=${localThis.year}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
    //     orderStmt: "ID",
    //   };

    //   return localThis
    //     .getGenericSelect({ genericSqlPayload: selectPayload })
    //     .then((res: any) => {
    //       //Setup data
    //       if (res.table_data) {
    //         let x: number = 0;
    //         res.table_data.forEach((item: any) => {
    //           budgetList.push(item);
    //         });
    //         let k: number = 0;
    //         for (k = 0; k < budgetList.length; k++) {
    //           localThis.percentage = budgetList[k].PERCENTAGE;
    //           localThis.selected[budgetList[k].MONTH - 1] = true;
    //           localThis.budget[budgetList[k].MONTH - 1] = budgetList[k].VALUE;
    //           localThis.bdg += budgetList[k].VALUE;
    //         }
    //       }
    //       localThis.rowKey = localThis.getRnd();
    //     });
    // },

    resetSelected() {
      let localThis: any = this as any;
      let i: number = 0;
      for (i = 0; i < 12; i++) {
        localThis.selected[i] = false;
      }
    },
  },
  mounted() {
    let localThis = this as any;
    let i: number = 0;
    if (localThis.donorList.length > 0) {
      localThis.mpDonorLineListMode = [];

      for (i = 0; i < localThis.donorList.length; i++) {
        let obj = {} as IShow;
        obj.showData = false;
        obj.showWait = false;
        localThis.mpDonorLineListMode.push(obj);
      }
    }
  },
  beforeMount() {
    let localThis = this as any;
    localThis.resetSelected();

    localThis.actDesc = localStorage.SelectedAct;
    localThis.localBudgetLine = localThis.budgetLine;
    localThis.localBudgetLine.MODIFIED = [];
    let i: number = 0;
    for (i = 0; i < 100; i++) {
      let obj = {} as IShow;
      obj.showData = false;
      obj.showWait = false;
      localThis.mpDonorLineListMode.push(obj);
    }
    i = 0;
    for (i = 0; i < 12; i++) {
      localThis.localBudgetLine.MODIFIED.push(false);
    }
    localThis.rowDesk[1] = this.$i18n.t("datasource.pms.annual-plan.objectives.jan");
    localThis.rowDesk[2] = this.$i18n.t("datasource.pms.annual-plan.objectives.feb");
    localThis.rowDesk[3] = this.$i18n.t("datasource.pms.annual-plan.objectives.mar");
    localThis.rowDesk[4] = this.$i18n.t("datasource.pms.annual-plan.objectives.apr");
    localThis.rowDesk[5] = this.$i18n.t("datasource.pms.annual-plan.objectives.may");
    localThis.rowDesk[6] = this.$i18n.t("datasource.pms.annual-plan.objectives.jun");
    localThis.rowDesk[7] = this.$i18n.t("datasource.pms.annual-plan.objectives.jul");
    localThis.rowDesk[8] = this.$i18n.t("datasource.pms.annual-plan.objectives.aug");
    localThis.rowDesk[9] = this.$i18n.t("datasource.pms.annual-plan.objectives.sep");
    localThis.rowDesk[10] = this.$i18n.t("datasource.pms.annual-plan.objectives.oct");
    localThis.rowDesk[11] = this.$i18n.t("datasource.pms.annual-plan.objectives.nov");
    localThis.rowDesk[12] = this.$i18n.t("datasource.pms.annual-plan.objectives.dec");
  },
});
</script>

<style scoped></style>
