<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row v-if="showWait">
        <v-col :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-card>
            <jrs-simple-table
              :title="$t('datasource.pms.jrs-chart-of-accounts.jrs-chart-of-accounts')"
              viewName="PMS_VI_JRS_CHART_OF_ACCOUNT"
              :hideNewBtn="true"
              :nbrOfFormColumns="1"
              :showActionBtn="true"
              :hideActions="true"
              :actionBtnDesc="'Nav Sync'"
              :dataSourceCondition="selectedCompany"
              @doAction="doAction"
            >
              <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
                <!-- BTN - COA READONLY POPUP -->
                <v-tooltip top>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn
                      icon
                      @click="showDetail(simpleItemRowItem)"
                      v-bind="attrs"
                      v-on="on"
                    >
                      <v-icon>mdi-eye-outline</v-icon>
                    </v-btn>
                  </template>
                  <span>{{ $t("general.crud.tooltip-view") }}</span>
                </v-tooltip>
              </template>
            </jrs-simple-table>
          </v-card>
          <!-- DETAILS DIALOG -->
          <v-dialog
            v-model="detailDialog"
            persistent
            retain-focus
            :scrollable="true"
            :overlay="false"
            :max-width="'60em'"
            transition="dialog-transition"
          >
            <v-card>
              <v-card-text>
                <v-row>
                  <v-col :cols="12">
                    <h1 class="capital-first-letter">{{ $t("general.detail") }}</h1>

                    <jrs-simple-readonly-detail
                      viewName="PMS_VI_JRS_CHART_OF_ACCOUNT"
                      :whereCondition="`PMS_JRS_COA_ID = '${selectedCOAId}'`"
                    ></jrs-simple-readonly-detail>
                  </v-col>
                </v-row>
              </v-card-text>
              <v-card-actions>
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="detailDialog = false"
                  >X {{ $t("general.close") }}</v-btn
                >
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";

import { mapActions, mapGetters } from "vuex";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsSimpleReadOnlyDetails from "../../components/JrsSimpleReadonlyDetail.vue";
import { NavImsApi, ImsApi, Configuration } from "../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-simple-readonly-detail": JrsSimpleReadOnlyDetails,
  },
  data() {
    return {
      rows: [],
      detailDialog: false,
      selectedCompany: "",
      selectedCOAId: "",
      showWait: false,
      tabItems: [
        {
          code: "JRSCOA",
          descr: "JRS Chart Of Accounts",
          tanslationKey: "datasource.pms.jrs-chart-of-accounts.jrs-chart-of-accounts",
        },
      ],
      tabIsVisible: null,
      genericQueryData: false,
    };
  },
  methods: {
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    /**
     * Open detail dialog with the selected data.
     * @param data The data to dispaly in the detail dialog
     */
    showDetail(data: any) {
      let localThis: any = this as any;
      console.log("details: " , data);
      let data_copy = Object.assign({}, data);
      localThis.selectedCOAId = data.PMS_JRS_COA_ID;
      localThis.detailDialog = true;
    },
    doAction() {
      let localThis: any = this as any;
      let msg: string = this.$i18n.t("datasource.gmt.donor.add-jrs-coa").toString();
      
      console.log("do action: " , msg);
      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.syncNAV();
        }
      });
    },
    syncNAV() {
      let localThis = this as any;
      localThis.showWait = true;
      localThis.rows = [];
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: NavImsApi = new NavImsApi(config);
      console.log("syncnav api: " , api);
      return api
        .apiNavImsGet(config.baseOptions)
        .then((res: any) => {
          localThis.rows = res.data;
          localThis.UpdateDB();
          localThis.showNewSnackbar({
            typeName: "success",
            text: "Chart of Account Extracted from NAV",
          });
        })
        .catch((err: any) => {
          // console.error(err);
          localThis.showNewSnackbar({ typeName: "error", text: err });
          localThis.rows = [];
          localThis.showWait = false;
        });
    },
    UpdateDB() {
      let localThis = this as any;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_JRS_COA",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(localThis.rows).toUpperCase(),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: "Chart of Account Synchronized with NAV " + res.message,
          }); // Feedback to user
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  beforeMount() {
    let localThis: any = this as any;
    let company: string = localThis.getCurrentOffice.HR_OFFICE_CODE.substring(0, 3);
    localThis.selectedCompany = `PMS_JRS_COMPANY = '${company}'`;
  },
});
</script>

<style scoped></style>
