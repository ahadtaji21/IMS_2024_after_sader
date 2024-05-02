<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
          <div>
            <v-data-table
              :headers="columns"
              :items="grantList"
              item-key="name"
              multi-sort
              :search="tableFilter"
              class="text-capitalize elevation-2"
              :items-per-page="15"
            >
              <template v-slot:top>
                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                  <h3>{{ $tc("datasource.gmt.grant.gtsd", 2) }}</h3>
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
                  <tr v-for="item in items" :key="item.ID" style="height: 10px">
                    <td class="tablerow">
                      {{ item.CODE }}
                    </td>
                    <td class="tablerow">
                      {{ item.DONOR_NAME }}
                    </td>
                    <td class="tablerow">
                      {{ item.DESCR }}
                    </td>
                    <td class="tablerow">
                      {{ item.START_DATE | formatDate }}
                    </td>
                    <td class="tablerow">
                      {{ item.END_DATE | formatDate }}
                    </td>
                    <td class="tablerow">
                      {{ item.DURATION }}
                    </td>
                    <td class="tablerow">
                      {{ item.AMOUNT + " " + item.CURRENCY_CODE }}
                    </td>
                    <td class="tablerow">
                      {{ item.IMS_STATUS_NAME }}
                    </td>
                    <td v-if="item.STATUS == 1" class="tablerow">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="green" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.design-in-progress")
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td v-if="item.STATUS == 0" class="tablerow">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon color="red" small class="mr-2" v-on="on"
                            >mdi-circle</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.design-in-progress")
                        }}</span>
                      </v-tooltip>
                    </td>
                    <td style="text-align: center">
                      <v-tooltip bottom v-if="authorizedRole == true">
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="undeleteItem(item)" v-on="on"
                            >mdi-undo</v-icon
                          >
                        </template>
                        <span>{{ $t("datasource.gmt.grant.gt-undelete-grant") }}</span>
                      </v-tooltip>
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
import { Configuration } from "../../../axiosapi";
import { translate, i18n } from "../../../i18n";
import GrantMainData from "../../../components/GMT/GRANTS/PROPOSAL/GMTGrantMainDataForm.vue";
import { EventBus } from "../../../event-bus";
import { mapActions } from "vuex";
import UtilMix from "../../../mixins/UtilMix";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    //"gmt-gt-form": GrantMainData,
  },
  mixins: [UtilMix],
  data() {
    return {
      authorizedRole: false,
      module: "",

      showWait: false,
      dialog: false,
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.gmt.grant.gt-code",
          align: "center",
          value: "CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Donor",
          textKey: "datasource.gmt.grant.gt-donor",
          align: "center",
          value: "DONOR_NAME",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.gmt.grant.gt-title",
          align: "center",
          value: "DESCR",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: "Project",
        //   textKey: "datasource.gmt.grant.gt-prj",
        //   align: "center",
        //   value: "PROJECT_CODE",
        //   sortable: true,
        //   filterable: true,
        // },
        {
          text: "Start Date",
          textKey: "datasource.gmt.grant.gt-start-date",
          align: "center",
          value: "START_DATE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Start Date",
          textKey: "datasource.gmt.grant.gt-end-date",
          align: "center",
          value: "END_DATE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Duration",
          textKey: "datasource.gmt.grant.gt-duration",
          align: "center",
          value: "DURATION",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Amount",
          textKey: "datasource.gmt.grant.gt-amount",
          align: "center",
          value: "AMOUNT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Status",
          textKey: "datasource.gmt.grant.gt-status-name",
          align: "center",
          value: "IMS_STATUS_NAME",
          sortable: true,
          filterable: true,
        },
        {
          text: "Active",
          textKey: "datasource.gmt.grant.gt-active",
          align: "center",
          value: "STATUS",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      rowsObj: {},
      grantList: [],
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
      let cols = (this as any).columnsTemplate.map((col: any) => {
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
  mounted() {
    let localThis = this as any;
    localThis.getGRANTS();
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    var role: any = localThis.getCurrentRole.ROLE_CODE;
    if (role == "GM") {
      //Grant Manager
      localThis.authorizedRole = true;
    }
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      if (to.ROLE_CODE == "GM") {
        //Grant Manager
        localThis.authorizedRole = true;
      }
    });
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    isGMTModule() {
      let localThis = this as any;
      if (localThis.module == "GMT") return true;
      else return false;
    },
    getGRANTS() {
      let localThis = this as any;
      let currentDate = new Date();
      localThis.grantList = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_DELETED",
        //colList: null,
        whereCond: `OFFICE_ID=${localThis.getCurrentOffice.HR_OFFICE_ID}`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              item.STATUS_ID = item.STATUS_ID + "";
              item.DONOR_ID = item.DONOR_ID + "";
              item.DEPARTMENT_ID = item.DEPARTMENT_ID + "";
              item.HR_POSITION_ID = item.HR_POSITION_ID + "";
              item.IMS_USER_USERNAME = localThis.getUserName(item.HR_POSITION_ID);
              //item.PROJECT_ID= item.PROJECT_ID+"";
              item.STATUS = 0;
              if (
                item.START_DATE != undefined &&
                item.START_DATE != "" &&
                item.END_DATE != undefined &&
                item.END_DATE != ""
              ) {
                var startdate = new Date(
                  item.START_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
                );
                var enddate = new Date(
                  item.END_DATE.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
                );
                if (startdate <= currentDate && enddate >= currentDate) {
                  item.STATUS = 1; //in the interval of validity
                }
              }

              localThis.grantList.push(item);
            });
            localThis.showWait = false;
          }
        })
        .finally(() => {
          localThis.showWait = false;
        });
    },
    recoveryItem() {
      this.$router.push({
        name: "Recovery Deleted Grants",
      });
    },
    getUserName(item: any) {
      let localThis = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_USERNAME",
        //colList: null,
        whereCond: `HR_POSITION_ID=${item}`,
        // orderStmt: "VALUE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              let name = item.HR_BIODATA_FULL_NAME;
              return name;
            });
          }
          // localThis.showWait = false;
        });
    },
    getGRANT(grantId: Number) {
      let localThis = this as any;

      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT",
        //colList: null,
        whereCond: `ID=${grantId}`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.showWait = false;
              localThis.editItem(item);
            });
          }
        });
    },

    closeDialog() {
      let localThis = this as any;
      let msgLeave: string = this.$i18n
        .t("datasource.gmt.grant.gt-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.dialog = false;
        }
      });
    },
    closeGrantDialogeAndSave(value: any) {
      let localThis = this as any;
      //localThis.getGRANTS();
      localThis.dialog = false;
      let itm = {} as any;
      let gt = {} as any;
      gt = localThis.$store.getters.getGrant;
      localThis.getGRANT(gt.grant_id);
    },

    editItem(item: any) {
      let localThis: any = this as any;
      let gt = {} as any;
      gt.grant_id = item.ID;
      gt.year = item.START_YEAR;
      gt.end_year = item.END_YEAR;
      gt.currency = item.CURRENCY_CODE;
      gt.donor_id = parseInt("" + item.DONOR_ID);
      gt.donor_name = item.DONOR_NAME;
      localThis.setGrant(gt);
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.$router.push({
        name: "Grant Details",
        params: {
          editItemMainData: localThis.editedItem,
          editItemNarrativeId: localThis.editedItem.ID,
          editItemOBJ: localThis.editedItem,
          donorPresetted: false,
        },
      });
      //this.rout.push(this.$route);
    },

    undeleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.gmt.grant.gt-undelete-confirm")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.GMT_ID = item.ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_UNDEL_GRANT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getGRANTS();
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
  },
  filters: {
    formatDate: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      return string.substring(0, 10);
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
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
</style>
