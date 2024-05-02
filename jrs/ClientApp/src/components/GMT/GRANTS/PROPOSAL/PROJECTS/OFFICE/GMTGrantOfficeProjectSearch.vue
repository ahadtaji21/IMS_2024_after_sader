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
          item-key="PMS_PROJECT_CODE_ID"
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
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../../../axiosapi";
import { translate } from "../../../../../../i18n";

import { mapActions } from "vuex";
import { EventBus } from "../../../../../../event-bus";
import UtilMix from "../../../../../../mixins/UtilMix";
import NavMix from "../../../../../../mixins/NavMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
  NavDimension1,
  NavIntegrationApi,
} from "../../../../../../axiosapi/api";
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
          textKey: "datasource.hrm.office.code",
          align: "center",
          value: "PMS_OFFICE_CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.pms.project.projects-code.code",
          align: "center",
          value: "PMS_PROJECT_CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Location",
          textKey: "datasource.pms.project.projects-code.description",
          align: "center",
          value: "PMS_PROJECT_CODE_DESC",
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
      (this as any).$emit("UpdateOffPrjsGrant", newVal);
    },
    getPMSAP() {
      let localThis: any = this as any;
      localThis.rows = [] as any;
      localThis.isLoading = true;
      let stateCond = null as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_PROJECT_CODE",
        colList: null,
        whereCond:null,// "IS_COUNTRY=0 AND IS_REGIONAL=0 AND IS_INTERNATIONAL=0", //`office_id = ${localThis.getCurrentOffice.PMS_PROJECT_CODE_ID})`,
        officeId: null, // localThis.getCurrentOffice.PMS_PROJECT_CODE_ID,
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
                  localThis.prjsAlreadyAdded.filter(
                    (itemL: any) => itemL.PMS_PROJECT_CODE_ID == item.PMS_PROJECT_CODE_ID
                  ).length == 0
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
