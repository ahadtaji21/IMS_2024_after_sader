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
                  <h3>{{ $tc("pms.ap", 2) }}</h3>
                  <v-spacer></v-spacer>
                  <v-dialog
                    v-model="dialog"
                    persistent
                    max-width="1100px"
                    v-if="module == 'PMS' && authorizedRole == true"
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
                    <pms-apd-form
                      @closeDialoge="closeDialog"
                      @closeDialogeAndSaveAnnualPlanMD="closeDialogAndSave"
                      :formData="editedItem"
                      :isUpdatableForm="true"
                      :key="keyMD"
                      :versioned="versioned"
                    ></pms-apd-form>
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
                    <td class="tablerow">{{ item.apcode }}</td>
                    <td class="tablerow">{{ item.descr }}</td>
                    <td class="tablerow">{{ item.location_description }}</td>
                    <td class="tablerow">{{ item.start_year }}</td>
                    <td class="tablerow">{{ item.end_year }}</td>
                    <td class="tablerow">{{ item.status_name }}</td>
                    <td
                      v-if="module == 'PMS' && item.design_in_progress == '0'"
                      class="tablerow"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="green" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.design-completed")
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td
                      v-if="module == 'PMS' && item.design_in_progress != '0'"
                      class="tablerow"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="yellow" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.design-in-progress")
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td
                      v-if="
                        module == 'IMP' &&
                        item.status_name == 'Activated' &&
                        item.service_number > 0 &&
                        item.service_activated == 0
                      "
                      class="tablerow"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="red" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t(
                            "datasource.pms.annual-plan.objectives.ap-implementation-status-no"
                          )
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td
                      v-if="
                        module == 'IMP' &&
                        item.status_name == 'Activated' &&
                        item.service_number > 0 &&
                        item.service_activated > 0 &&
                        item.service_activated < item.service_number
                      "
                      class="tablerow"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="yellow" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t(
                            "datasource.pms.annual-plan.objectives.ap-implementation-status-partial"
                          )
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td
                      v-if="
                        module == 'IMP' &&
                        item.status_name == 'Activated' &&
                        item.service_number > 0 &&
                        item.service_activated == item.service_number
                      "
                      class="tablerow"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="green" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t(
                            "datasource.pms.annual-plan.objectives.implementation-status-no"
                          )
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td
                      v-if="
                        module == 'IMP' &&
                        item.status_name == 'Activated' &&
                        item.service_number == 0
                      "
                      class="tablerow"
                    >
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="black" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t(
                            "datasource.pms.annual-plan.objectives.ap-implementation-status-nosrv"
                          )
                        }}</span>
                      </v-tooltip>
                    </td>

                    <td
                      v-if="module == 'IMP' && item.status_name != 'Activated'"
                      class="tablerow"
                    ></td>
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
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../axiosapi";
import { translate } from "../../i18n";
import AnnualPlanMainData from "../../components/PMS/ANNUALPLAN/AnnualPlanMainDataForm.vue";
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
    "pms-apd-form": AnnualPlanMainData,
  },
  mixins: [UtilMix, NavMix],
  data() {
    return {
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
        {
          text: "Design Status",
          textKey: "datasource.pms.annual-plan.ap-design-status-name",
          align: "center",
          value: "design_in_progress",
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
        {
          text: "Activation State",
          textKey: "datasource.pms.annual-plan.ap-activation-state",
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
    localThis.module = localThis.getActiveModule.moduleCode;
    console.log("PmsannualPlanList.vue: " , localThis.getActiveModule)

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
    getPMSAP() {
      let localThis: any = this as any;
      localThis.rows = [] as any;
      localThis.isLoading = true;
      let stateCond = null as any;

      if (localThis.module == "IMP") {
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
    closeDialogAndSave(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-add-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["id"] == undefined) {
        msg = msgAdd;
        value.guid = localThis.makeUUID();
      } else {
        id = Number(value["id"]);
        if (value.guid == undefined || value.guid == "")
          value.guid = localThis.makeUUID();
      }

      this.$confirm(msg).then((res) => {
        if (res) {
          //(this as any).editedItem = (this as any).itemModel;
          //for (var key in value) {
          //    alert('keyy: ' + key + '\n' + 'value: ' + value[key]);
          //}
          const config: Configuration = (this as any).$store.getters["auth/getApiConfig"];
          const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);

          //ADD
          return (
            api
              .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
              .then((response) => {
                // let ap = {} as any;
                // ap.annal_plan_id = value.id;
                // ap.start_year = value.start_year;
                // ap.end_year = value.end_year;
                // ap.currency = value.currency_code;
                // localThis.setAnnualPlan(ap);
                // localThis.getPMSAP();
                value.id = response.data;
                (this as any).dialog = false;
                // (this as any).editedItem = (this as any).itemModel;
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: "Succesfully Added new Annual Plan main data.",
                });
                localThis.editItem(value);
              })
              // .then((res) => {
              //   localThis.updateNavDimension(value);
              // })
              .catch((err: any) => {
                localThis.showNewSnackbar({ typeName: "error", text: err.response.data });
              })
          );
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
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      switch (localThis.module) {
        case "PMS": 
          localThis.$router.push({
            name: "Annual Plan Details",
            params: {
              editItemMainData: localThis.editedItem,
              editItemNarrativeId: localThis.editedItem.id,
              editItemOBJ: localThis.editedItem,
              versioned: 0,
            },
          });
          break;
        case "IMP":
          localThis.$router.push({
            name: "IMP Annual Plan Details",
            params: {
              editItemMainData: localThis.editedItem,
              editItemNarrativeId: localThis.editedItem.id,
              editItemOBJ: localThis.editedItem,
              versioned: 0,
            },
          });
          break;
        default:
          localThis.$router.push({
            name: "GMT Annual Plan Details",
            params: {
              editItemMainData: localThis.editedItem,
              editItemNarrativeId: localThis.editedItem.id,
              editItemOBJ: localThis.editedItem,
              versioned: 0,
            },
          });
          break;
      }
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
