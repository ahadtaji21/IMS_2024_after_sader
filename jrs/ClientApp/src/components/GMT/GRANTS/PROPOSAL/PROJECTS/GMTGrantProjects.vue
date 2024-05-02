<template>
  <v-container fluid fill-height>
    <v-row v-if="showWait">
      <v-col justify-center :cols="12">
        <div class="text-center" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-row align-center>
      <v-col justify-center :cols="12">
        <div v-if="!showPrjDetails">
          <v-data-table
            :headers="columns"
            :items="prjs"
            item-key="name"
            multi-sort
            :search="tableFilter"
            class="text-capitalize elevation-2"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $tc("datasource.gmt.grant.gt-prjs-country", 2) }}</h3>
                <v-spacer></v-spacer>
                <v-dialog
                  v-model="dialog"
                  persistent
                  max-width="1100px"
                  v-if="module == 'GMT' && authorizedRole == true"
                >
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3"
                      v-on="on"
                    >
                      <v-icon>mdi-plus-circle-outline</v-icon>New
                    </v-btn>
                  </template>
                  <gmt-prj-search
                    :selectedGrantId="selectedGrantId"
                    :onlyRead="!(module == 'GMT' && authorizedRole == true)"
                    @closeEdit="closeEdit"
                    :prjsAlreadyAdded="prjs"
                    @UpdatePrjsGrant="UpdatePrjsGrant"
                    :key="Math.floor(Math.random() * 256)"
                  ></gmt-prj-search>
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
            <template v-slot:[`item.action`]="{ item }">
              <div>
                <v-icon
                  v-if="module == 'GMT' && authorizedRole == true"
                  small
                  class="mr-2"
                  @click="editItem(item)"
                  >mdi-circle-edit-outline</v-icon
                >
                <span v-if="module == 'GMT' && authorizedRole == true">
                  <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
                </span>
              </div>
            </template>
          </v-data-table>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col align-center :cols="12">
        <div v-if="showPrjDetails">
          <v-dialog v-model="showPrjDetails" persistent max-width="1100px">
            <gmt-prj-details
              :selectedGrantId="selectedGrantId"
              :onlyRead="!(module == 'GMT' && authorizedRole == true)"
              @closeDialogeGtPrjDetails="closeEdit"
              :gmtPrjId="prjId"
              :imsID="imsID"
              :prjDescription="prjDescription"
              :prjFileName="prjFileName"
              :editItemMainData="editedItem"
              :isAuthorized="authorizedRole"
            ></gmt-prj-details>
          </v-dialog>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "./../../../../../axiosapi";
import { translate, i18n } from "./../../../../../i18n";
//import SearchTable from "./FUNDED/GMTGrantProjectsSearch.vue";
import PrjSearch from "./GMTGrantProjectSearch.vue";
import PrjDetails from "./GMTGrantProjectDetails.vue";
import { EventBus } from "./../../../../../event-bus";
import { mapActions } from "vuex";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "./../../../../../axiosapi/api";

export default Vue.extend({
  props: {
    selectedGrantId: {
      type: Number,
      required: true,
    },
    country_id: {
      type: Number,
      required: true,
    },
  },
  components: {
    // "search-table": SearchTable,
    "gmt-prj-search": PrjSearch,
    "gmt-prj-details": PrjDetails,
  },
  data() {
    return {
      module: "",
      prjDescription: "",
      prjFileName: "",
      authorizedRole: false,
      showWait: false,
      showPrjDetails: false,
      prjId: 0,
      imsID: 0,
      dialog: false,
      keyDialog: 0,
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.pms.annual-plan.ap-code",
          align: "center",
          value: "APCODE",
          sortable: true,
          filterable: true,
        },

        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-description",
          align: "center",
          value: "GPR_DESCRIPTION",
          sortable: true,
          filterable: true,
        },
        {
          text: "Location Description",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "LOCATION_DESCR",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Amount",
          textKey: "datasource.gmt.grant.gt-amount",
          align: "center",
          value: "GPR_AMOUNT",
          sortable: true,
          filterable: true,
        },
        {
          text: "Currency",
          textKey: "datasource.gmt.grant.gt-currency",
          align: "center",
          value: "CURRENCY_CODE",
          sortable: true,
          filterable: true,
        },
        { text: "Actions", value: "action", sortable: false },
      ],

      prjs: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
    }),

    columns() {
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }
        //  if (col.value === "pmsCoiDescription") {
        //      let newColDesc = col;
        //     newColDesc.text = translate(col.textKey);
        //     newColDesc.value = "pmsCoiCode";
        //     return newColDesc;
        // }

        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

      return cols;
    },
  },

  mounted() {
    let localThis: any = this;
    localThis.getGrantProjects();
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
    UpdatePrjsGrant(item: any) {
      let localThis: any = this as any;
      if (item != null) {
        localThis.itemsPerPage = 15;
        localThis.UpdateItems(item);
      }
    },
    UpdateItems(item: any) {
      let localThis = this as any;

      let i: number = 0;
      let j: number = 0;
      for (i = 0; i < item.length; i++) {
        let payLoadD = {} as any;
        payLoadD.ID_GRANT = localThis.selectedGrantId;
        payLoadD.ID_PROJECT = item[i].id;

        let savePayload: GenericSqlPayload = {
          spName: "GMT_SP_INS_GRANT_PROJECT",
          actionType: SqlActionType.NUMBER_0,
          jsonPayload: JSON.stringify(payLoadD),
        };
        const response = localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            j++;
            if (j == item.length) {
              localThis.getGrantProjects();
              localThis.dialog = false;
              localThis.$emit("updateCountryProjects");
            }
            return res;
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
            throw err;
          });
      }
      localThis.itemsPerPage = -1;
      localThis.showNewSnackbar({ typeName: "success", text: "Projects List Updated" }); // Feedback to user
    },
    getGrantProjects() {
      let localThis = this as any;

      localThis.prjs = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_PROJECTS",
        //colList: null,
        whereCond: `ID_GRANT=${localThis.selectedGrantId} AND COUNTRY_ID=${localThis.country_id}`,
        orderStmt: "APCODE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.prjs.push(item);
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        })
        .finally(() => (localThis.showWait = false));
    },
    closeEdit() {
      let localThis = this as any;
      localThis.dialog = false;
      localThis.showPrjDetails = false;
      localThis.getGrantProjects();
      localThis.$emit("updateCountryProjects");
    },
    closeDialog(item: string) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as any;
      payLoadD.ID_GRANT = localThis.selectedGrantId;
      payLoadD.ID_PROJECT = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_GRANT_PROJECT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getGrantProjects();
          localThis.$emit("updateCountryProjects");
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    closeDialogeAndUpdateList(value: any) {
      let localThis = this as any;
      localThis.getGrantProjects();
      localThis.dialog = false;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
    },
    editItem(item: any) {
      let localThis: any = this as any;
      localThis.showPrjDetails = true;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.prjId = localThis.editedItem.GMT_PRJ_ID;
      localThis.imsID = localThis.editedItem.ID;
      localThis.prjDescription = localThis.editedItem.GMT_PRJ_DESCRIPTION;
      localThis.prjFileName = localThis.editedItem.GMT_PRJ_NAME;
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.showWait = true;
          let payLoadD = {} as any;
          payLoadD.ID = item.GP_ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_GRANT_PROJECT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.getGrantProjects();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.showWait = false;
              localThis.$emit("updateCountryProjects");
            })

            .catch((err: any) => {
              localThis
                .showNewSnackbar({
                  typeName: "error",
                  text: err.message,
                })
                .finally(() => (localThis.showWait = false));
            });
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
</style>
