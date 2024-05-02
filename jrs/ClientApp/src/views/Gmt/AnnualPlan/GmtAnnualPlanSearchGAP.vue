<template>
  <v-content>
    <v-container fluid fill-height>
      <div v-if="showSearch">
        <v-row align-center>
          <v-col justify-center :cols="12">
            <gmt-search-gap-filter
              @clearResultList="clearResultList"
              @searchGaps="searchGaps"
              :key="111"
            ></gmt-search-gap-filter>
          </v-col>
        </v-row>
        <v-row align-center v-if="showWait">
          <v-col justify-center :cols="12">
          <div
            class="text-center"
            v-if="showWait"
            style="margin: 0px; padding: 0px"
          >
            <v-progress-circular
              indeterminate
              color="primary"
            ></v-progress-circular>
          </div>
          </v-col>
        </v-row>
        <v-row align-center>
          <v-col justify-center :cols="12">
            <div>
              <v-data-table
                :headers="headers"
                :items="gapList"
                item-key="ACTIVITY_ID"
                multi-sort
                :search="tableFilter"
                class="text-capitalize elevation-2"
              >
                <template v-slot:top>
                  <div
                    class="d-inline-flex align-center primary lighten-2 jrs-table-top"
                  >
                    <h3>{{ $tc("datasource.gmt.budget.gaps", 2) }}</h3>
                    <v-spacer></v-spacer>

                    <v-text-field
                      v-model="tableFilter"
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
                    <tr
                      v-for="item in items"
                      :key="item.ACTIVITY_ID"
                      style="height: 10px"
                    >
                      <td class="tablerow">{{ item.AP_CODE }}</td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{
                              item.LOCATION_DESCR | subStr
                            }}</span>
                          </template>
                          <span>{{ item.LOCATION_DESCR }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.OBJ_DESC | subStr }}</span>
                          </template>
                          <span>{{ item.OBJ_DESC }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.SRV_DESC | subStr }}</span>
                          </template>
                          <span>{{ item.SRV_DESC }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.ACT_DESC | subStr }}</span>
                          </template>
                          <span>{{ item.ACT_DESC }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.COI_DESC | subStr }}</span>
                          </template>
                          <span>{{ item.COI_DESC }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.TOS_DESC | subStr }}</span>
                          </template>
                          <span>{{ item.TOS_DESC }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        {{ round(item.NEEDS,2) }}
                      </td>
                      <td class="tablerow">
                        {{ round(item.FUNDED,2) }}
                      </td>
                      <td
                        :class="CellColor(item.GAP)"
                      >
                        {{ round(item.GAP,2) }}
                      </td>
                      <td class="tablerow">
                        {{ item.CURRENCY }}
                      </td>
                      <td style="text-align: center">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <v-icon
                              small
                              class="mr-2"
                              @click="editItem(item)"
                              v-on="on"
                              >mdi-circle-edit-outline</v-icon
                            >
                          </template>
                          <span>{{
                            $t("datasource.gmt.budget.gap-details")
                          }}</span>
                        </v-tooltip>
                      </td>
                    </tr>
                  </tbody>
                </template>
              </v-data-table>
            </div>
          </v-col>
        </v-row>
      </div>

      <gmt-gap-details
        v-else
        :selectedObjectId="editActId"
        :year="year"
        :key="rndKey"
        :annual_plan_id="id_annual_plan"
        :currencyCode="currencyCode"
        @showGapList="showGapList"
        :displayData="displayData"
      ></gmt-gap-details>
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "../../../axiosapi";
import { translate } from "../../../i18n";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import SearchGAPFilter from "../../../components/GMT/MAPPING/SEARCHGAP/GMTSearchGapFilterForm.vue";
import GAPDetails from "../../../components/GMT/MAPPING/SEARCHGAP/GMTGapDetailsForm.vue";
import FormMixin from "../../../mixins/FormMixin";
import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  GenericSqlApi,
  ImsApi,
  PmsAnnualPlanApi,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gmt-search-gap-filter": SearchGAPFilter,
    "gmt-gap-details": GAPDetails,
  },
  mixins:[FormMixin],
  data() {
    return {
      displayData:{},
      showWait:false,
      showSearch: true,
      rndKey: 1,
      id_annual_plan: 0,
      currencyCode: "",
      year: 0,
      editActId: 0,
      keyDialog: 0,
      dialog: false,
      searchParameters: {},
      headers: [],
      gapList: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
      //,
      // itemModel: {
      //   apcode: null,
      //   descr: null,
      //   locationDescr: null,
      //   adminAreaName: null,
      //   startYear: null,
      //   statusName: null
      // }
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
    }),
    language() {
      return i18n.locale;
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.setupHeaders();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

     CellColor(value:number)
    {
      if (value<0 )
      {
        return 'OverGapCell';
      }
      else 
      {
        if (value==0 )
          return "NoGapCell";
        else
        return "GapCell";
      }
      
      
    },
    clearResultList() {},
    searchGaps(item: any) {
      let localThis = this as any;
      localThis.searchParameters = JSON.parse(JSON.stringify(item));
      if (localThis.searchParameters.START_YEAR == undefined)
        localThis.searchParameters.START_YEAR = 0;
      if (localThis.searchParameters.LOCATION_ID == undefined)
        localThis.searchParameters.LOCATION_ID = 0;
      if (localThis.searchParameters.COI_ID == undefined)
        localThis.searchParameters.COI_ID = 0;
      if (localThis.searchParameters.TOS_ID == undefined)
        localThis.searchParameters.TOS_ID = 0;
      localThis.getGaps();
    },

    getGaps() {
      let localThis = this as any;
      localThis.showWait=true;
      if (
        localThis.searchParameters == {} ||
        localThis.searchParameters.START_YEAR == 0
      ) {
        localThis.showWait=false;
        localThis.showNewSnackbar({
          typeName: "error",
          text: this.$i18n.t("datasource.gmt.budget.set-filter"),
        });
        return;
      }

      const config: Configuration =
        localThis.$store.getters["auth/getApiConfig"];
      const api: GenericSqlApi = new GenericSqlApi(config);
      localThis.gapList = [];

      let jsonPayload = {
        START_YEAR: localThis.searchParameters.START_YEAR,
        LOCATION_ID: localThis.searchParameters.LOCATION_ID,
        COI_ID: localThis.searchParameters.COI_ID,
        TOS_ID: localThis.searchParameters.TOS_ID,
      };

      let payload: GenericSqlPayload = {
        spName: "GMS_SP_GET_ACTIVITY_BUDGET",
        actionType: SqlActionType.NUMBER_3,
        jsonPayload: JSON.stringify(jsonPayload),
      };
      return api
        .genericSqlSPCall(payload)
        .then((res: any) => {
          //Setup data
          if (res.data.rows) {
            let x: number = 0;
            res.data.rows.forEach((item: any) => {
              localThis.gapList.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.showWait=false;
        })
        .catch((err: any) => {
          localThis.showWait=false;
          console.log(err);
          return err;
        });
    },

    closeDialog() {
      let localThis = this as any;

      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.dialog = false;
          localThis.editedItem = {};
          localThis.keyDialog = Math.ceil(Math.random() * 1000);

          // (this as any).editedItem = (this as any).itemModel;
        }
      });
    },

    // editItem(item: any) {
    //   let localThis: any = this as any;
    //   localThis.keyDialog = Math.ceil(Math.random() * 1000);
    //   localThis.editedItem = JSON.parse(JSON.stringify(item));
    //   localThis.dialog = true;
    // },
    showGapList() {
      let localThis: any = this as any;
      localThis.showSearch = true;
      localThis.getGaps();
    },
    editItem(item: any) {

      let localThis: any = this as any;
      localThis.displayData={} as any;
      localThis.displayData.AP_CODE = item.AP_CODE ;
      localThis.displayData.OBJ_DESC = item.OBJ_DESC ;
      localThis.displayData.SRV_DESC = item.SRV_DESC ;
      localThis.displayData.ACT_DESC = item.ACT_DESC ;
      localThis.editActId = item.ACTIVITY_ID;
      localThis.id_annual_plan = item.ANNUAL_PLAN_ID;
      localThis.currencyCode = item.CURRENCY;
      localThis.year = localThis.searchParameters.START_YEAR;
      localThis.rndKey = Math.ceil(Math.random() * 1000);
      localThis.showSearch = false;
      //   localThis.editedItem = JSON.parse(JSON.stringify(item));
      //   localThis.$router.push({
      //     name: "Donor Details",
      //     params: {
      //       editItemMainData: localThis.editedItem,
      //     },
      //  });
      //this.rout.push(this.$route);
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-code").toString(),
          align: "center",
          value: "AP_CODE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.ims.location.title").toString(),

          align: "center",
          value: "LOCATION_DESCR",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.desc")
            .toString(),

          align: "center",
          value: "OBJ_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.service")
            .toString(),

          align: "center",
          value: "SRV_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.activity")
            .toString(),

          align: "center",
          value: "ACT_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t(
              "datasource.pms.category-of-intervention.category-of-intervention"
            )
            .toString(),

          align: "center",
          value: "COI_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.type-of-service.type-of-service")
            .toString(),

          align: "center",
          value: "TOS_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.budget.needs").toString(),

          align: "center",
          value: "NEEDS",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.budget.funded").toString(),

          align: "center",
          value: "FUNDED",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.budget.gap").toString(),

          align: "center",
          value: "GAP",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.ap-currency")
            .toString(),

          align: "center",
          value: "CURRENCY",
          sortable: true,
          divider: true,
        },
        {
          text: "Actions",
          value: "action",
          align: "center",
          sortable: false,
        },
      ];
      localThis.headers = tmp;
    },
  },
  filters: {
    subStr: function (string: any) {
      if (string != undefined) {
        if (string.length < 25) return string;
        else return string.substring(0, 25) + "...";
      } else return "";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getGaps();
      localThis.setupHeaders();
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
.tablerow {
  text-align: center;
  height: 3.5em;
  font-size: 12px;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}

.NACell {
  background-color: greenyellow;
  font-size: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 100%;
}
</style>