<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col>
          <!-- TABLE -->
          <jrs-simple-table
            viewName="SFIP_VI_IMPLEMENTATION_PLAN"
            :nbrOfFormColumns="3"
            :selectable="true"
            :dataSourceCondition="conditionSourcePlan"
            v-model="selectPlanRow"
            :hideNewBtn="true"
            :hideActions="true"
          >
          <template v-slot:simple-table-header >
              <div class="mt-3 ml-10" >
               <jrs-date-picker
               :dense="true"
               :label="'Year'"
               v-model="dateFrom"
              :is_only_year="true"
          ></jrs-date-picker>
          </div>
              </template> 
          </jrs-simple-table>

          <!-- NEW/EDIT FORM -->
          <v-card elevation="3" class="mt-2" v-if="selectPlanRow.length > 0">
            <v-card-title>
              <div>
                <h3><v-icon class="mb-1 mr-1">mdi-card-text-outline</v-icon>Global Indicator Dashboard</h3>
                <span
                  >Plan Code :
                  {{ selectPlanRow[0].IMPLEMENTATION_PLAN_CODE }}</span
                >
                <span class="ml-2"
                  ><v-icon>mdi-calendar</v-icon> Start Year :
                  {{ selectPlanRow[0].IMPLEMENTATION_PLAN_START_YEAR }}</span
                >
                <span class="ml-2"
                  ><v-icon>mdi-calendar</v-icon> End Year :
                  {{ selectPlanRow[0].IMPLEMENTATION_PLAN_END_YEAR }}</span
                >
              </div>
            </v-card-title>

            <div>
              <jrs-simple-table
                viewName="SFIP_VI_INDICATOR"
                :title="$t('nav.global-dashboard-indicator')"
                :nbrOfFormColumns="3"
                :dataSourceCondition="conditionSource"
                :hideNewBtn="true"
                :hideActions="true"
              >
              </jrs-simple-table>
            </div>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsDatePicker from "../components/JrsDatePicker.vue";
import JrsSimpleTable from "../components/JrsSimpleTable.vue";

import FormMixin from "../mixins/FormMixin";
import UtilMix from "../mixins/UtilMix";
export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-date-picker": JrsDatePicker,
  },
  data: () => ({
    dateFrom: new Date().getFullYear(),
    dataLegalSelected: [],
    selectPlanRow: [],
  }),
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),

    planIdSelected(){
        let list:any = this.selectPlanRow[0];
        return list.IMPLEMENTATION_PLAN_ID;
    },
    conditionSource(){
        return `SFIP_ID = ${this.planIdSelected || 0}`;
    },
    conditionSourcePlan(){
        return `IMPLEMENTATION_PLAN_START_YEAR <= ${this.dateFrom || new Date().getFullYear()} AND IMPLEMENTATION_PLAN_END_YEAR >= ${this.dateFrom || new Date().getFullYear()}`;
    },
  },
  mixins: [FormMixin, UtilMix],
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),

    ...mapActions("apiHandler", {
      execSPCall: "execSPCall",
      getGenericSelect: "getGenericSelect",
      getJRSTableStructure: "getJRSTableStructure",
      generateExcelForTable: "generateExcelForTable",
    }),
  },
});
</script>

<style scoped>
</style>