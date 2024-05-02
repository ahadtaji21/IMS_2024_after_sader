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
                  <h3>{{ $tc("nav.mande-matrix", 2) }}</h3>
                  <v-spacer></v-spacer>
                  <v-dialog
                    v-model="dialog"
                    persistent
                    max-width="1100px"
                    v-if="false && module == 'PMS' && authorizedRole == true"
                  >
                    <template v-slot:activator="{ on }">
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                        v-on="on"
                        @click="InitializeAnnualPlan()"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>New
                      </v-btn>
                    </template>
                    <pms-ind-form
                      @closeDialogeImsInd="closeDialogImsInd"
                      @closeDialogeAndSave="closeDialogAndSave"
                      :selectedIndicatorId="0"
                      :isUpdatableForm="true"
                      :key="keyMD"
                      :versioned="versioned"
                    ></pms-ind-form>
                  </v-dialog>
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
                    <td class="tablerow">{{ item.Category }}</td>
                    <td class="tablerow">{{ item.Code }}</td>
                    <td class="tablerow">{{ item.Dimension }}</td>
                    <td class="tablerow">{{ item.Target }}</td>
                    <td class="tablerow">{{ item.Construct_definition }}</td>
                    <td class="tablerow">{{ item.Service }}</td>
                    <td class="tablerow">{{ item.Process }}</td>
                    <td class="tablerow">{{ item.Indicator }}</td>

                    <td style="text-align: center">
                      <v-icon small class="mr-2" @click="editItem(item)"
                        >mdi-circle-edit-outline</v-icon
                      >
                      <v-icon small @click="deleteItem(item)" style="display: none"
                        >mdi-delete</v-icon
                      >
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-data-table>
          </div>
        </v-col>
      </v-row>
      <div>
        <v-dialog v-model="showDetails" persistent max-width="1100px">
          <pms-ind-form
            @closeDialogeImsInd="closeDialogImsInd"
            @closeDialogeAndSave="closeDialogAndSave"
            :selectedIndicatorId="selectedIndicatorId"
            :isUpdatableForm="true"
            :key="keyMD"
            :versioned="versioned"
          ></pms-ind-form>
        </v-dialog>
      </div>
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../axiosapi";
import { translate } from "../../i18n";
import ImsIndicator from "../../components/PMS/MATRIXIND/ImsIndicatorMainDataForm.vue";
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
    "pms-ind-form": ImsIndicator,
  },
  mixins: [UtilMix, NavMix],
  data() {
    return {
      showDetails: false,
      selectedIndicatorId: 0,
      keyMD: 0,
      versioned: 0,
      isLoading: false,
      authorizedRole: false,
      module: "",
      dialog: false,
      columnsTemplate: [
        {
          text: "Category",
          textKey: "indicators.Category",
          align: "center",
          value: "Category",
          sortable: true,
          filterable: true,
        },
        {
          text: "Code",
          textKey: "indicators.Code",
          align: "center",
          value: "Code",
          sortable: true,
          filterable: true,
        },
        {
          text: "Dimension",
          textKey: "indicators.Dimension",
          align: "center",
          value: "Dimension",
          sortable: true,
          filterable: true,
        },
        {
          text: "Target",
          textKey: "indicators.Target",
          align: "center",
          value: "Target",
          sortable: true,
          filterable: true,
        },
        {
          text: "Construction Definition",
          textKey: "indicators.Construct_Definition",
          align: "center",
          value: "Construction_definition",
          sortable: true,
          filterable: true,
        },
        {
          text: "Service",
          textKey: "indicators.Service",
          align: "center",
          value: "Service",
          sortable: true,
          filterable: true,
        },
        {
          text: "Process",
          textKey: "indicators.Process",
          align: "center",
          value: "Process",
          sortable: true,
          filterable: true,
        },
        {
          text: "Indicator",
          textKey: "indicators.Indicator",
          align: "center",
          value: "Indicator",
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
      {
        cols = (this as any).columnsTemplate.map((col: any) => {
          if (col.value === "action") {
            return col;
          }
          let newCol = col;
          newCol.text = translate(col.textKey);

          return newCol;
        });
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
    localThis.getIMSIND();
    localThis.editedItem = {} as any;
    localThis.editedItem.office_id = "0";
    localThis.editedItem.code_id = "0";
    var role: any = localThis.getCurrentRole.ROLE_CODE;
    if (role == "PC" || role == "PD") {
      //Program coordinator || Project Director
      localThis.authorizedRole = true;
    }
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      if (to != undefined && (to.ROLE_CODE == "PC" || to.ROLE_CODE == "PD")) {
        //Program Coordinator
        localThis.authorizedRole = true;
      }
    });
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setAnnualPlan: "setAnnualPlan",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    getIMSIND() {
      let localThis: any = this as any;
      localThis.rows = [] as any;
      localThis.isLoading = true;
      let stateCond = null as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_MTX_MATRIX",
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
      localThis.keyMD = Math.ceil(Math.random() * 1000);
    },

    // getIMSIND() {
    //   const config: Configuration = (this as any).$store.getters["auth/getApiConfig"];
    //   const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);
    //   return api

    //     .apiPmsAnnualPlanGet(config.baseOptions)
    //     .then((res) => {
    //       (this as any).rows = res.data;
    //       var i = 0;
    //       // for (i=0;i<(this as any).rows.length;i++)
    //       // {
    //       //     (this as any).getLBLDesc(i,(this as any).rows[i].pmsCoiDescriptionLabelId);

    //       // }
    //       return res;
    //     })
    //     .catch((err) => {
    //       // console.error(err);
    //       (this as any).rows = [];
    //     });
    // },
    closeDialogImsInd() {
      let localThis = this as any;
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.showDetails = false;
          localThis.dialog = false;
          // (this as any).editedItem = (this as any).itemModel;
        }
      });
    },
    closeDialogAndSave(value: any) {
      let localThis = this as any;
      localThis.showDetails = false;
      localThis.dialog = false;
      localThis.getIMSIND();
    },

    editItem(item: any) {
      let localThis: any = this as any;
      localThis.selectedIndicatorId = item.id;
      localThis.keyMD = Math.ceil(Math.random() * 1000);
      localThis.showDetails = true;
    },
    deleteItem(item: any) {
      let msg: string = this.$i18n.t("pms.delete-confirm").toString();

      this.$confirm(msg).then((res) => {
        if (res) {
          item["pmsCoiDeleted"] = true;
          const config: Configuration = (this as any).$store.getters["auth/getApiConfig"];

          // const api: PmsCategoryOfInterventionApi = new PmsCategoryOfInterventionApi(config);
          // return api
          //     .apiPmsCategoryOfInterventionIdPut(Number(item['pmsCoiId']),item,  config.baseOptions)
          //     .then(response => {
          //         (this as any).getOBJ();
          //     })
          //     .catch(error => {
          //         alert(error);
          //     });
        }
      });
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
