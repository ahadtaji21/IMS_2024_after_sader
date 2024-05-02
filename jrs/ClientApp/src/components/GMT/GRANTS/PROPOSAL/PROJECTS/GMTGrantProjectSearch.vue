<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>

      <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>

      <v-card-text>
        <v-data-table
          :headers="columns"
          :items="rows"
          item-key="id"
          multi-sort
          :search="tableFilter"
          v-model="selectedItm"
          class="text-capitalize elevation-2"
          show-select
          :single-select="false"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>{{ $t("pms.ap") }}</h3>

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
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-btn color="secondary darken-1" text @click="close()"
          >X {{ $t("general.close") }}</v-btn
        >
        <v-btn text color="primary" @click="save()">{{ $t("general.save") }}</v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../../axiosapi";
import { translate } from "../../../../../i18n";

import { mapActions } from "vuex";
import { EventBus } from "../../../../../event-bus";
import UtilMix from "../../../../../mixins/UtilMix";
import NavMix from "../../../../../mixins/NavMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  NavDimension1,
  NavIntegrationApi,
} from "../../../../../axiosapi/api";
export default Vue.extend({
  props: {
    prjsAlreadyAdded: {
      type: Array,
      required: false,
      default: Array,
    },
  },
  components: {},
  mixins: [UtilMix, NavMix],
  data() {
    return {
      selectedItm: [],
      keyMD: 0,
      versioned: 0,
      isLoading: false,
      authorizedRole: false,
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

      cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }
        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

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
    close() {
      let localThis: any = this as any;
      localThis.$emit("closeEdit");
    },
    save() {
      let localThis: any = this as any;
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.add-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let i: number = 0;
          try {
            localThis.updateValue(localThis.selectedItm);
          } catch (error) {
            // Feedback to user
          }
        }
      });
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdatePrjsGrant", newVal);
    },
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
        officeId: null, // localThis.getCurrentOffice.HR_OFFICE_ID,
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            //Setup data
            res.table_data.forEach((item: any) => {
              if (
                localThis.prjsAlreadyAdded != undefined &&
                localThis.prjsAlreadyAdded.length > 0
              ) {
                if (
                  localThis.prjsAlreadyAdded.filter((itemL: any) => itemL.ID == item.id)
                    .length == 0
                ) {
                  localThis.rows.push(item);
                }
              } else {
                localThis.rows.push(item);
              }
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

    // getPMSAP() {
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
    closeDialog() {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          (this as any).dialog = false;
          // (this as any).editedItem = (this as any).itemModel;
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
