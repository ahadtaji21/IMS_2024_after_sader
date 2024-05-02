<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <v-card-text>
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
            <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
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
                show-select
                :single-select="true"
                style="cursor: pointer"
              >
                <template v-slot:top>
                  <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
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
                  <tbody style="border: solid; cursor: pointer">
                    <tr
                      v-for="item in items"
                      :key="item.ACTIVITY_ID"
                      style="height: 10px"
                      @click="selectItm(item)"
                    >
                      <td class="tablerow" colspan="2">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.AP_CODE | subStr }}</span>
                          </template>
                          <span>{{ item.AP_CODE }}</span>
                        </v-tooltip>
                      </td>
                      <td class="tablerow">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <span v-on="on">{{ item.LOCATION_DESCR | subStr }}</span>
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
                            <span v-on="on">{{ item.ACT_TYPE | subStr }}</span>
                          </template>
                          <span>{{ item.ACT_TYPE }}</span>
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
                        {{ round(item.NEEDS, 2) }}
                      </td>
                      <td class="tablerow">
                        {{ round(item.FUNDED, 2) }}
                      </td>
                      <td :class="CellColor(item.GAP)">
                        {{ round(item.GAP, 2) }}
                      </td>
                      <td class="tablerow">
                        {{ item.CURRENCY }}
                      </td>
                    </tr>
                  </tbody>
                </template>
              </v-data-table>
            </div>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-btn color="secondary darken-1" text @click="Close()"
          >X {{ $t("general.close") }}</v-btn
        >
      </v-card-actions>
    </v-card>
  </div>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "../../../../../axiosapi";
import { translate, i18n } from "../../../../../i18n";
import { JrsHeader } from "../../../../../models/JrsTable";
import FormMixin from "../../../../../mixins/FormMixin";
import FiltersForm from "./GMTGrantMapActivityFilterForm.vue";
import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  GenericSqlApi,
  ImsApi,
  PmsAnnualPlanApi,
} from "../../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gmt-search-gap-filter": FiltersForm,
  },
  mixins: [FormMixin],
  props: {
    formDataExt: {
      type: Object,
      required: true,
    },
    createGrantActivity: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      formData: {},
      showWait: false,
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
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    //localThis.getGaps();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    CellColor(value: number) {
      if (value < 0) {
        return "OverGapCell";
      } else {
        if (value == 0) return "NoGapCell";
        else return "GapCell";
      }
    },
    clearResultList() {},
    searchGaps(item: any) {
      let localThis = this as any;
      localThis.searchParameters = JSON.parse(JSON.stringify(item));
      if (localThis.searchParameters.START_YEAR == undefined)
        localThis.searchParameters.START_YEAR = 0;
      if (localThis.searchParameters.ANNUAL_PLAN_ID == undefined)
        localThis.searchParameters.ANNUAL_PLAN_ID = 0;
      if (localThis.searchParameters.LOCATION_ID == undefined)
        localThis.searchParameters.LOCATION_ID = 0;
      if (localThis.searchParameters.COI_ID == undefined)
        localThis.searchParameters.COI_ID = 0;
      if (localThis.searchParameters.TOS_ID == undefined)
        localThis.searchParameters.TOS_ID = 0;
      if (localThis.searchParameters.ACT_TYPE_ID == undefined)
        localThis.searchParameters.ACT_TYPE_ID = 0;
      if (localThis.searchParameters.ACT_ID == undefined)
        localThis.searchParameters.ACT_ID = 0;
      localThis.getGaps();
    },

    getGaps() {
      let localThis = this as any;
      localThis.showWait = true;
      if (
        localThis.searchParameters == {} ||
        localThis.searchParameters.START_YEAR == 0
      ) {
        localThis.showWait = false;
        localThis.showNewSnackbar({
          typeName: "error",
          text: this.$i18n.t("datasource.gmt.budget.set-filter"),
        });
        return;
      }

      localThis.gapList = [];
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ACTIVITY_BUDGET_GAP",
        colList: null,
        whereCond: `START_YEAR = ${localThis.searchParameters.START_YEAR}
        AND (${localThis.searchParameters.ANNUAL_PLAN_ID}=0 OR ANNUAL_PLAN_ID=${localThis.searchParameters.ANNUAL_PLAN_ID})  
        AND (${localThis.searchParameters.LOCATION_ID}=0 OR LOCATION_ID=${localThis.searchParameters.LOCATION_ID}) 
        AND (${localThis.searchParameters.ACT_TYPE_ID}=0 OR ACTIVITY_TYPE_ID=${localThis.searchParameters.ACT_TYPE_ID})  
        AND (${localThis.searchParameters.ACT_ID}=0 OR ACTIVITY_ID=${localThis.searchParameters.ACT_ID})`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ANNUAL_PLAN_ID",
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.gapList.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          console.log(err);
          return err;
        });
    },
    Close() {
      let localThis = this as any;
      localThis.$emit("closeDialoge");
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
      //localThis.getGaps();
    },
    editItem(item: any) {
      let localThis: any = this as any;
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
    updateValue(newVal: String) {
      (this as any).$emit("closeDialogeAndSave", newVal);
    },
    selectItm(itm: any) {
      let localThis: any = this as any;
      if (itm != undefined) {
        let msg: string = this.$i18n.t("datasource.gmt.map-item").toString();

        this.$confirm(msg).then((res: any) => {
          if (res) {
            localThis.formData.PMS_ACTIVITY_ID = itm.ACTIVITY_ID;
            if (localThis.createGrantActivity == true) {
              let savePayload: GenericSqlPayload = {
                spName: "GMT_SP_INS_UPD_GRANT_ACTIVITY_MAP",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(localThis.formData),
              };

              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({
                    typeName: "success",
                    text: res.message,
                  }); // Feedback to user
                  localThis.$emit("closeDialogeAndSave");
                })
                .catch((err: any) => {
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message,
                  }); // Feedback to user
                });
            } else {
              // MAPPA l'attività senza crearne una nuova sul grant
              let savePayload: GenericSqlPayload = {
                spName: "GMT_SP_INS_UPD_GRANT_ACTIVITY_MAP",
                actionType: SqlActionType.NUMBER_0,
                jsonPayload: JSON.stringify(localThis.formData),
              };

              localThis
                .execSPCall(savePayload)
                .then((res: any) => {
                  localThis.showNewSnackbar({
                    typeName: "success",
                    text: res.message,
                  }); // Feedback to user
                  localThis.$emit("closeDialogeAndSave");
                })
                .catch((err: any) => {
                  localThis.showNewSnackbar({
                    typeName: "error",
                    text: err.message,
                  }); // Feedback to user
                });
            }

            localThis.updateValue(itm.ACTIVITY_ID);
          }
        });
      }
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
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.desc").toString(),

          align: "center",
          value: "OBJ_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.service").toString(),

          align: "center",
          value: "SRV_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.activity").toString(),

          align: "center",
          value: "ACT_DESC",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.act-type").toString(),

          align: "center",
          value: "ACT_TYPE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.category-of-intervention.category-of-intervention")
            .toString(),

          align: "center",
          value: "COI_DESC",
          sortable: true,
          divider: true,
        },

        {
          text: this.$i18n.t("datasource.pms.type-of-service.type-of-service").toString(),
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
          text: this.$i18n.t("datasource.pms.annual-plan.ap-currency").toString(),

          align: "center",
          value: "CURRENCY",
          sortable: true,
          divider: true,
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
    console.log("MapActMainDataForm");
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
