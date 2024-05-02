<template>
  <v-container>
    <v-card>
      <v-row align-center v-if="showWait">
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <!-- <v-row>
              <v-col :cols="12">
                <h1>{{ $t("views.view-activity-instance.title") }}</h1>
                <p>{{ $t("views.view-activity-instance.view-info") }}</p>
              </v-col>
            </v-row> -->
          <!-- MAIN FILTER -->
          <v-row>
            <!-- PROJECT SELECT -->
            <v-col :cols="6">
              <v-autocomplete
                v-model="startyear"
                :label="$t('datasource.ims.location.year')"
                :items="projectList"
                item-value="START_YEAR"
                item-text="START_YEAR"
                @change="resetAP"
                return-object
              ></v-autocomplete>
            </v-col>
            <v-col :cols="6">
              <v-autocomplete
                v-model="annualPlan"
                :label="$t('pms.ap')"
                :items="
                  projectList.filter((item) => item.START_YEAR == startyear.START_YEAR)
                "
                item-value="APID"
                item-text="APCODE"
                return-object
              ></v-autocomplete>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="12">
              <v-autocomplete
                v-model="location"
                :label="$t('datasource.ims.location.title')"
                :items="projectList"
                item-value="LOCATION_ID"
                item-text="LOCATION_DESCR"
                return-object
              ></v-autocomplete>
            </v-col>
          </v-row>
          <v-row>
            <!-- ACTIVITY TYPE -->
            <v-col :cols="6">
              <v-autocomplete
                v-model="activityType"
                :label="$t('datasource.pms.activity-type.type')"
                :items="activityTypeList"
                item-value="value"
                item-text="text"
                return-object
              ></v-autocomplete>
            </v-col>
            <!-- ACTIVITY TYPE -->

            <v-col :cols="6">
              <v-autocomplete
                v-model="activity"
                :label="$t('datasource.pms.activity-type.name')"
                :items="
                  activityList.filter(
                    (item) =>
                      activityType.value == '0' ||
                      item.ACTIVITY_TYPE_ID_STR == activityType.value
                  )
                "
                item-value="ACTIVITY_ID"
                item-text="ACT_DESC"
                return-object
              ></v-autocomplete>
            </v-col>
          </v-row>
          <v-row>
            <!-- COI SELECT -->
            <v-col :cols="6">
              <v-autocomplete
                v-model="coi"
                :label="
                  $t('datasource.pms.category-of-intervention.category-of-intervention')
                "
                :items="coiList"
                item-value="PMS_COI_ID"
                item-text="COI_DESCR"
                return-object
              ></v-autocomplete>
            </v-col>
            <!-- TOS SELECT -->
            <v-col :cols="6">
              <v-autocomplete
                v-model="tos"
                :label="$t('datasource.pms.type-of-service.type-of-service')"
                :items="coiList.filter((item) => item.PMS_COI_ID == coi.PMS_COI_ID)"
                item-value="PMS_TOS_ID"
                item-text="PMS_TOS_DESCRIPTION"
                return-object
              ></v-autocomplete>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
      <v-card-actions>
        <v-btn color="primary" class="--darken-1" @click="search()">Search</v-btn>
        <v-btn color="secondary" class="--darken-1" @click="clear()">Clear</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../../i18n";
import { ImsApi, ImsLookupsApi, Configuration } from "../../../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";
import { JrsFormField, JrsFormFieldSearch } from "../../../../../models/JrsForm";
interface IFormData {
  START_YEAR: number;
  ANNUAL_PLAN_ID: number;
  LOCATION_ID: number;
  COI_ID: number;
  TOS_ID: number;
  ACT_TYPE_ID: number;
  ACT_ID: number;
}
export default Vue.extend({
  components: {},
  data() {
    return {
      annualPlan: {},
      showWait: false,
      startyear: {},
      projectList: [],
      activityList: [],
      activityTypeList: [],
      activityType: {},
      activity: {},
      location: {},
      coiList: [],
      coi: {},
      tos: {},
    };
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),
    resetAP() {
      let localThis = this as any;
      localThis.annualPlan = {};
    },
    clear() {
      let localThis = this as any;
      localThis.startyear = {};
      localThis.annualPlan = {};
      localThis.coi = {};
      localThis.tos = {};
      localThis.location = {};
      localThis.activity = {};
      localThis.activityType = {};
      localThis.$emit("clearResultList");
    },
    search() {
      let localThis = this as any;
      let formData = {} as IFormData;
      formData.ANNUAL_PLAN_ID = localThis.annualPlan.APID;
      formData.START_YEAR = localThis.startyear.START_YEAR;
      formData.LOCATION_ID = localThis.location.LOCATION_ID;
      formData.COI_ID = localThis.coi.PMS_COI_ID;
      formData.TOS_ID = localThis.tos.PMS_TOS_ID;
      formData.ACT_TYPE_ID = localThis.activityType.value;
      formData.ACT_ID = localThis.activity.ACTIVITY_ID;
      localThis.$emit("searchGaps", formData);
    },
    getCOI() {
      let localThis = this as any;
      localThis.showWait = true;
      localThis.selectedcoi = null;
      localThis.coiList = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_COI_TOS_LOCALIZED",
        //colList: null,
        whereCond: `IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_COI_ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data

          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.coiList.push(item);
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

    getLocations() {
      let localThis: any = this as any;
      localThis.projectList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_PROJECT_LOCATION",
        colList: null,
        whereCond: null, // `DONOR_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "LOCATION_DESCR",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.projectList.push(item);
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

    getActivities() {
      let localThis: any = this as any;
      localThis.activityList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ACTIVITY_BUDGET_GAP",
        colList: null,
        whereCond: null, // `DONOR_ID = ${localThis.selectedObjectId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ACT_DESC",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;

            res.table_data.forEach((item: any) => {
              item.ACTIVITY_TYPE_ID_STR = item.ACTIVITY_TYPE_ID + "";
              localThis.activityList.push(item);
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
    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      this.showWait = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      let itm = {} as any;
      itm.text = "All";
      itm.value = "0";
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "ACTIVITY_TYPE":
              localThis.activityTypeList.push(itm);
              res.data.forEach((item: any) => {
                localThis.activityTypeList.push(item);
              });
              localThis.getActivities();
              break;
          }
          //obj = res.data;
          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch((err) => {
          // console.error(err);
          this.showWait = false;
          return [];
        });
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getLocations();
    localThis.getList("ACTIVITY_TYPE");
    localThis.getCOI();
  },
});
</script>

<style scoped></style>
