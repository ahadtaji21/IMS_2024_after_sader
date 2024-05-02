<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div class="d-flex flex-row justify-center" v-if="isLoading">
            <v-progress-circular indeterminate></v-progress-circular>
          </div>
          <div>
            <v-data-table
              :headers="columns"
              :items="rows"
              item-key="name"
              multi-sort
              :search="tableFilter"
              class="text-capitalize elevation-2"
            >
              <template v-slot:top>
                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                  <h3>{{ $tc("nav.ap-config", 2) }}</h3>
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
                  <tr v-for="item in items" :key="item.id" style="height: 10px">
                    <td class="tablerow">{{ item.apcode }}</td>
                    <td class="tablerow">{{ item.descr }}</td>
                    <td class="tablerow">{{ item.location_description }}</td>
                    <td class="tablerow">{{ item.start_year }}</td>
                    <td class="tablerow">{{ item.end_year }}</td>
                    <td class="tablerow">{{ item.status_name }}</td>
                    <td style="text-align: center">
                      <v-icon small class="mr-2" @click="editItem(item)"
                        >mdi-circle-edit-outline</v-icon
                      >
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-data-table>
          </div>
        </v-col>
      </v-row>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <v-dialog v-model="dialogCFG" persistent max-width="1100px">
            <pms-cfg-form
              @closeDialoge="closeDialog"
              @closeDialogeAndSaveAnnualPlanCFG="closeDialogAndSave"
              :formData="editedItem"
              :isUpdatableForm="true"
              :key="keyMD"
              :versioned="versioned"
            ></pms-cfg-form>
          </v-dialog>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../axiosapi";
import { translate } from "../../i18n";
import AnnualPlanConfig from "../../components/PMS/ANNUALPLAN/AnnualPlanConfigForm.vue";
import { mapActions } from "vuex";
import { EventBus } from "../../event-bus";
import UtilMix from "../../mixins/UtilMix";
import NavMix from "../../mixins/NavMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  NavDimension1,
  NavIntegrationApi,
} from "../../axiosapi/api";
export default Vue.extend({
  components: {
    "pms-cfg-form": AnnualPlanConfig,
  },
  mixins: [UtilMix, NavMix],
  data() {
    return {
      keyMD: 0,
      dialogCFG: false,
      versioned: 0,
      isLoading: false,
      authorizedRole: true,
      module: "",
      dialog: false,
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.pms.annual-plan.ap-code",
          align: "center",
          value: "apcode",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-description",
          align: "center",
          value: "descr",
          sortable: true,
          filterable: true,
        },
        {
          text: "Location",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "location_description",
          sortable: true,
          filterable: true,
        },
        {
          text: "Start Year",
          textKey: "datasource.pms.annual-plan.ap-start-year",
          align: "center",
          value: "start_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "End Year",
          textKey: "datasource.pms.annual-plan.ap-end-year",
          align: "center",
          value: "end_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "Status",
          textKey: "datasource.pms.annual-plan.ap-status-name",
          align: "center",
          value: "status_name",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      columnsTemplateImpl: [
        {
          text: "Code",
          textKey: "datasource.pms.annual-plan.ap-code",
          align: "center",
          value: "apcode",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-description",
          align: "center",
          value: "descr",
          sortable: true,
          filterable: true,
        },
        {
          text: "Location",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "location_description",
          sortable: true,
          filterable: true,
        },
        {
          text: "Start Year",
          textKey: "datasource.pms.annual-plan.ap-start-year",
          align: "center",
          value: "start_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "End Year",
          textKey: "datasource.pms.annual-plan.ap-end-year",
          align: "center",
          value: "end_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "Status",
          textKey: "datasource.pms.annual-plan.ap-status-name",
          align: "center",
          value: "status_name",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],

      columnsTemplateGnt: [
        {
          text: "Code",
          textKey: "datasource.pms.annual-plan.ap-code",
          align: "center",
          value: "apcode",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-description",
          align: "center",
          value: "descr",
          sortable: true,
          filterable: true,
        },
        {
          text: "Location",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "location_description",
          sortable: true,
          filterable: true,
        },
        {
          text: "Start Year",
          textKey: "datasource.pms.annual-plan.ap-start-year",
          align: "center",
          value: "start_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "End Year",
          textKey: "datasource.pms.annual-plan.ap-end-year",
          align: "center",
          value: "end_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "Status",
          textKey: "datasource.pms.annual-plan.ap-status-name",
          align: "center",
          value: "status_name",
          sortable: true,
          filterable: true,
        },
        { text: "Actions", value: "action", sortable: false },
      ],
      rowsObj: {},
      rows: [],
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
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
    columns() {
      let localThis = this as any;
      let cols = {} as any;
      if (localThis.module == "IMP") {
        cols = (this as any).columnsTemplateImpl.map((col: any) => {
          if (col.value === "action") {
            return col;
          }

          let newCol = col;
          newCol.text = translate(col.textKey);

          return newCol;
        });
      } else {
        if (localThis.module == "GMT") {
          cols = (this as any).columnsTemplateGnt.map((col: any) => {
            if (col.value === "action") {
              return col;
            }

            let newCol = col;
            newCol.text = translate(col.textKey);

            return newCol;
          });
        } else {
          cols = (this as any).columnsTemplate.map((col: any) => {
            if (col.value === "action") {
              return col;
            }
            let newCol = col;
            newCol.text = translate(col.textKey);

            return newCol;
          });
        }
      }

      return cols;
    },
  },
  beforeMount() {
    let localThis = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
  },
  mounted() {
    let localThis = this as any;
    localThis.getPMSAP();
    localThis.editedItem = {} as any;
    localThis.editedItem.office_id = "0";
    localThis.editedItem.code_id = "0";
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setAnnualPlan: "setAnnualPlan",
      getAnnualPlan: "getAnnualPlan",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    getPMSAP() {
      let localThis: any = this as any;
      localThis.rows = [] as any;
      localThis.isLoading = true;
      let stateCond = null as any;

      if (localThis.module.toUpperCase() == "IMP") {
        stateCond = `(status_id ='12' or status_id='1044')`; //approved from RO or activated
      } else {
        stateCond = `status_id<>'1044'`; //not activated
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_PLAN_LIST",
        colList: null,
        whereCond: stateCond, //`office_id = ${localThis.getCurrentOffice.HR_OFFICE_ID})`,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            //Setup data
            res.table_data.forEach((item: any) => {
              localThis.rows.push(item);
            });
          }
          localThis.isLoading = false;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
          localThis.isLoading = false;
        });
    },
    InitializeAnnualPlan() {
      let localThis = this as any;
      localThis.editedItem = {} as any;
      localThis.editedItem = {} as any;
      localThis.editedItem.office_id = "0";
      localThis.editedItem.code_id = "0";
      localThis.editedItem.max_categories_number = 3;
      localThis.keyMD = Math.ceil(Math.random() * 1000);
    },

    closeDialog() {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          (this as any).dialogCFG = false;
          // (this as any).editedItem = (this as any).itemModel;
        }
      });
    },
    closeDialogAndSave(item: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;

      this.$confirm(msg).then((res) => {
        if (res) {
          let payLoad = {} as any;
          let i: number;
          payLoad.ID = item.id;

          payLoad.MAX_CAT_NUMBER = item.maxCatNumber;
          let ap = localThis.$store.getters.getAnnualPlan;
          ap.max_categories_number = item.maxCatNumber;
          localThis.setAnnualPlan(ap);

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_CONFIGURATION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoad),
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.dialogCFG = false;
              localThis.getPMSAP();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
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

    editItem(item: any) {
      let localThis: any = this as any;
      let ap = {} as any;
      ap.annal_plan_id = item.id;
      ap.start_year = item.start_year;
      ap.end_year = item.end_year;
      ap.currency = item.currency_code;
      ap.max_categories_number = item.max_categories_number;
      localThis.setAnnualPlan(ap);
      //localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.keyMD = Math.ceil(Math.random() * 1000);
      localThis.dialogCFG = true;
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
</style>
