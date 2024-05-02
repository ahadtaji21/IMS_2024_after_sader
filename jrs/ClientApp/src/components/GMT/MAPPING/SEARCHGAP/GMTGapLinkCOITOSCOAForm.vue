<template>
  <div>
    <!-- scelta del COI TOS e COA da associare alla budget line -->
    <template>
      <v-card>
        <v-card>
          <v-container fluid>
            <v-form
              v-model="formValid"
              ref="form"
              lazy-validation
              class="text-capitalize"
            >
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
                <v-col :cols="6">
  
                    <v-autocomplete
                    v-model="objective"
                    :label="$t('datasource.gmt.donor.dnr-obj')"
                    :items="
                      objectiveList
                    "
                    item-value="ID"
                    item-text="DESCRIPTION"
                    :rules="rules.required"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <v-col :cols="6">
                                      <v-autocomplete
                    v-model="service"
                    :label="$t('datasource.gmt.donor.dnr-srv')"
                    :items="
                       serviceList.filter(
                        (item) =>
                         
                          item.OBJECTIVE_ID_STR == objective.ID
                      )
                    "
                    item-value="SERVICE_ID"
                    item-text="SRV_DESC"
                    :rules="rules.required"
                    return-object
                  ></v-autocomplete>
                </v-col>
              </v-row>
              <v-row>
                <!-- ACTIVITY TYPE -->
                <v-col :cols="6">
                  <v-autocomplete
                    v-model="activityType"
                    :label="$t('datasource.gmt.donor.dnr-act-type')"
                    :items="activityTypeList"
                    item-value="value"
                    item-text="text"
                    return-object
                  ></v-autocomplete>
                </v-col>
                <v-col :cols="6">
                  <v-autocomplete
                    v-model="activity"
                    :label="$t('datasource.gmt.donor.dnr-act')"
                    :items="
                      activityList.filter(
                        (item) =>
                         ( activityType.value == '0' ||
                          item.ACTIVITY_TYPE_ID_STR == activityType.value) &&
                          (item.SERVICE_ID_STR == service.SERVICE_ID)
                      )
                    "
                    item-value="ACTIVITY_ID"
                    item-text="ACT_DESC"
                    return-object
                  ></v-autocomplete>
                </v-col>
              </v-row>

              <!-- <v-row>
                <v-col :cols="6">
                  <v-text-field
                    :label="$t('datasource.gmt.donor.dnr-coi')"
                    v-model="donorFormData.COI_DESCR"
                    @click="openCOIDialog"
                    :counter="150"
                    :rules="rules.required"
                    :readonly="true"
                  ></v-text-field>
                </v-col>

                <v-col :cols="6">
                  <v-text-field
                    :label="$t('datasource.gmt.donor.dnr-tos')"
                    v-model="donorFormData.TOS_DESCR"
                    @click="openTOSDialog"
                    :counter="150"
                    :rules="rules.required"
                    :readonly="true"
                  ></v-text-field>
                </v-col>
           
              </v-row> -->
              <v-row>
                <v-col :cols="12">
                  <v-text-field
                    :label="$t('datasource.gmt.donor.dnr-coa')"
                    v-model="donorFormData.COA_DESCR"
                    :counter="150"
                    @click="openCOADialog"
                    :rules="rules.required"
                    :readonly="true"
                  ></v-text-field>
                </v-col>
              </v-row>
            </v-form>
          </v-container>
        </v-card>

        <v-card-actions>
          <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
          <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
        </v-card-actions>
      </v-card>
      <v-dialog
        v-model="editCOAMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
        transition="dialog-transition"
      >
        <gmt-dcoa-form
          @UpdateRelations="UpdateCOA"
          :key="rndVar"
          :selectedDonorId="donorId"
          :selectedResourceId="formData.PMS_JRS_COA_ID"
          :alreadyInserted="dcoajcoaList"
        ></gmt-dcoa-form>
      </v-dialog>
      <v-dialog
        v-model="editCOIMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
        transition="dialog-transition"
      >
        <gmt-dcoi-form
          @UpdateRelations="UpdateCOI"
          :key="rndVar"
          :selectedDonorId="donorId"
          :selectedResourceId="formData.PMS_JRS_COI_ID"
          :alreadyInserted="dcoijcoiList"
        ></gmt-dcoi-form>
      </v-dialog>
      <v-dialog
        v-model="editTOSMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
        transition="dialog-transition"
      >
        <gmt-dtos-form
          @UpdateRelations="UpdateTOS"
          :key="rndVar"
          :selectedDonorId="donorId"
          :selectedResourceId="formData.PMS_JRS_TOS_ID"
          :alreadyInserted="dtosjtosList"
        ></gmt-dtos-form>
      </v-dialog>
    </template>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";
import JDCOASearch from "./../../DONORS/COA/GmtDCoaSearchForm.vue";
import JDCOISearch from "./../../DONORS/COI/GmtDCoiSearchForm.vue";
import JDTOSSearch from "./../../DONORS/TOS/GmtDTosSearchForm.vue";
import { ImsApi, ImsLookupsApi, Configuration } from "../../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
interface IDATA {
  COI_ID: number;
  COI_DESCR: string;
  TOS_ID: number;
  TOS_DESCR: string;
  COA_ID: number;
  COA_DESCR: string;
  DONOR_ID: number;
  GRANT_ID: number;
  GMT_ACTIVITY_ID: number;
  PMS_ACT_ID: number;
}
export default Vue.extend({
  props: {
    formData: {
      type: Object,
      required: true,
    },
    donorId: {
      type: Number,
      required: true,
    },
    grantId: {
      type: Number,
      required: true,
    },
    presetVal: {
      type: String,
      required: false,
      default: "",
    },
  },

  components: {
    "gmt-dcoa-form": JDCOASearch,
    "gmt-dcoi-form": JDCOISearch,
    "gmt-dtos-form": JDTOSSearch,
  },
  data() {
    return {
      showWait: false,
      objectiveList: [],
      serviceList: [],
      activityList: [],
      activityTypeList: [],
      activityType: {},
      activity: {},
       objective: {},
      service: {},
      dcoajcoaList: [],
      dcoijcoiList: [],
      dtosjtosList: [],
      rndVar: 0,
      nbrOfColumns: 2,
      editCOAMode: false,
      editCOIMode: false,
      editTOSMode: false,
      donorFormData: {} as IDATA,
      formValid: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    deleteItem(item: any) {},
    close: function () {
      let localThis = this as any;
      //   (localThis.$refs.myFormCOI as Vue & { reset: () => void }).reset();
      this.$emit("closeDialoge");
    },

    getList(listName: any) {
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


    getObjectives() {
      let localThis: any = this as any;
      localThis.objectiveList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_OBJECTIVE",
        colList: null,
        whereCond: `GRANT_ID = ${localThis.grantId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "DESCRIPTION",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;

            res.table_data.forEach((item: any) => {
              localThis.objectiveList.push(item);
            });
             localThis.getServices();
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
  getServices() {
      let localThis: any = this as any;
      localThis.serviceList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_SERVICE",
        colList: null,
        whereCond: `GRANT_ID = ${localThis.grantId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "SRV_DESC",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;

            res.table_data.forEach((item: any) => {
              item.OBJECTIVE_ID_STR = item.OBJECTIVE_ID + "";
              localThis.serviceList.push(item);
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
        viewName: "GMT_VI_ACTIVITY",
        colList: null,
        whereCond: `GRANT_ID = ${localThis.grantId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "ACT_DESC",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;

            res.table_data.forEach((item: any) => {
              item.ACTIVITY_TYPE_ID_STR = item.ACTIVITY_TYPE_ID + "";
               item.SERVICE_ID_STR = item.SERVICE_ID + "";
              localThis.activityList.push(item);
            });
            localThis.getObjectives();
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
    UpdateCOA(item: any) {
      let localThis: any = this as any;
      localThis.closeCOAEdit();
      if (item == null) return;
      localThis.donorFormData.COA_ID = item.ACC_ID;
      localThis.donorFormData.COA_DESCR = item.ACC_CODE + "-" + item.ACC_DESCRIPTION;
    },
    UpdateCOI(item: any) {
      let localThis: any = this as any;
      localThis.closeCOIEdit();
      if (item == null) return;
      localThis.donorFormData.COI_ID = item.GMT_COI_ID;
      localThis.donorFormData.COI_DESCR = item.GMT_COI_CODE + "-" + item.IMS_LABELS_VALUE;
    },
    UpdateTOS(item: any) {
      let localThis: any = this as any;
      localThis.closeTOSEdit();
      if (item == null) return;
      localThis.donorFormData.TOS_ID = item.GMT_TOS_ID;
      localThis.donorFormData.TOS_DESCR =
        item.GMT_TOS_CODE + "-" + item.GMT_TOS_DESCRIPTION;
    },
    openCOADialog() {
      let localThis: any = this as any;
      localThis.editCOAMode = !localThis.editCOAMode;
      localThis.dcoajcoaList = [];
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    openCOIDialog() {
      let localThis: any = this as any;
      localThis.editCOIMode = !localThis.editCOIMode;
      localThis.dcoijcoiList = [];
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    openTOSDialog() {
      let localThis: any = this as any;
      localThis.editTOSMode = !localThis.editTOSMode;
      localThis.dtosjtosList = [];
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },
    closeCOAEdit() {
      let localThis = this as any;
      localThis.editCOAMode = false;
    },
    closeCOIEdit() {
      let localThis = this as any;
      localThis.editCOIMode = false;
    },
    closeTOSEdit() {
      let localThis = this as any;
      localThis.editTOSMode = false;
    },
    save() {
      let localThis: any = this as any;
      if (
        // localThis.donorFormData.TOS_ID == null ||
        // localThis.donorFormData.COI_ID == null ||
        // localThis.donorFormData.COA_ID == null
        localThis.activity.ACTIVITY_ID == undefined || localThis.activity.ACTIVITY_ID==0 || localThis.donorFormData.COA_ID == null
      ) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Fill all the fields",
        });
        return;
      }
      let msg: string = this.$i18n
        .t("datasource.gmt.donor.add-jrs-dnr-budget-line")
        .toString();

      localThis.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.donorFormData.GMT_ACTIVITY_ID=localThis.activity.ACTIVITY_ID;
          localThis.donorFormData.GRANT_ID= localThis.grantId;

          localThis.$emit("addRelation", localThis.donorFormData);
        }
      });
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.donorFormData = {} as IDATA;
    localThis.donorFormData.DONOR_ID = localThis.donorId;
    localThis.getList("ACTIVITY_TYPE");
  },
});
</script>

<style scoped></style>
