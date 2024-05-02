<template>
  <v-container>
    <v-card>
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
                :items="locationList"
                item-value="START_YEAR"
                item-text="START_YEAR"
                return-object
              ></v-autocomplete>
            </v-col>
            <v-col :cols="6">
              <v-autocomplete
                v-model="location"
                :label="$t('datasource.ims.location.title')"
                :items="
                  locationList.filter(
                    (item) =>
                      item.START_YEAR == 0 ||
                      item.START_YEAR == startyear.START_YEAR
                  )
                "
                item-value="LOCATION_ID"
                item-text="LOCATION_DESCR"
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
                  $t(
                    'datasource.pms.category-of-intervention.category-of-intervention'
                  )
                "
                :items="
                  coiList.filter(
                    (item) =>
                      item.LOCATION_ID == 0 ||
                      item.LOCATION_ID == location.LOCATION_ID
                  )
                "
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
                :items="
                  coiList.filter(
                    (item) =>
                      item.LOCATION_ID == 0 ||
                      (item.LOCATION_ID == location.LOCATION_ID &&
                        item.PMS_COI_ID == coi.PMS_COI_ID)
                  )
                "
                item-value="PMS_TOS_ID"
                item-text="PMS_TOS_DESCRIPTION"
                return-object
              ></v-autocomplete>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
      <v-card-actions>
        <v-btn color="primary" class="--darken-1" @click="search()"
          >Search</v-btn
        >
        <v-btn color="secondary" class="--darken-1" @click="clear()"
          >Cancel</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
import { JrsFormField, JrsFormFieldSearch } from "../../../../models/JrsForm";
interface IFormData {
  START_YEAR: number;
  LOCATION_ID: number;
  COI_ID: number;
  TOS_ID: number;
}
export default Vue.extend({
  components: {},
  data() {
    return {
      showWait: false,
      startyear: 0,
      locationList: [],
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
    /**
     * Return a list of active locations for the provided user and office.
     */
    clear() {
      let localThis = this as any;
      localThis.startyear = 0;
      localThis.coi = {};
      localThis.tos = {};
      localThis.location = {};
      localThis.$emit("clearResultList");
    },
    search() {
      let localThis = this as any;
      let formData = {} as IFormData;
      formData.START_YEAR = localThis.startyear.START_YEAR;
      formData.LOCATION_ID = localThis.location.LOCATION_ID;
      formData.COI_ID = localThis.coi.PMS_COI_ID;
      formData.TOS_ID = localThis.tos.PMS_TOS_ID;
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
        }).then((res: any) => {
          localThis.showWait = false;
        }).catch((err: any) => {
          localThis.showWait=false;
          console.log(err);
          return err;
        });
    },

    getLocations() {
      let localThis: any = this as any;
      localThis.locationList = [];
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
              localThis.locationList.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.showWait = false;
        }).catch((err: any) => {
          localThis.showWait=false;
          console.log(err);
          return err;
        });
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getLocations();

    localThis.getCOI();
  },
});
</script>

<style scoped>
</style>