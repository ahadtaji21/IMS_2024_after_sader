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
                <h3>{{ $tc("datasource.gmt.grant.gt-project", 2) }}</h3>
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
                    @UpdateOffPrjsGrant="UpdateOffPrjsGrant"
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
            <template v-slot:body="{ items }">
              <tbody style="border: solid">
                <tr v-for="item in items" :key="item.ID" style="height: 10px">
                  <td class="tablerow">
                    {{ item.HR_OFFICE_CODE }}
                  </td>
                  <td class="tablerow">
                    {{ item.PMS_PROJECT_CODE }}
                  </td>
                  <td class="tablerow">
                    {{ item.PMS_PROJECT_CODE_DESC }}
                  </td>
                  <td class="tablerow">
                    {{ item.GPR_DESCRIPTION }}
                  </td>
                  <!-- <td class="tablerow">
                    {{ item.LOCATION_DESCR }}
                  </td> -->
                  <td class="tablerow">
                    {{ item.GPR_AMOUNT }}
                  </td>
                  <td class="tablerow">
                    {{ item.CURRENCY_CODE }}
                  </td>
                  <td class="tablerow">
                    <div v-html="item.GPR_AREA_DESCR"></div>
                  </td>
                  <td style="text-align: center">
                    <v-tooltip bottom v-if="module == 'GMT' && authorizedRole == true">
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                          >mdi-circle-edit-outline</v-icon
                        >
                      </template>
                      <span>{{ $t("datasource.gmt.donor.edit-donor") }}</span>
                    </v-tooltip>
                    <v-tooltip bottom v-if="module == 'GMT' && authorizedRole == true">
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                          >mdi-delete</v-icon
                        >
                      </template>
                      <span>{{ $t("datasource.gmt.donor.delete-donor") }}</span>
                    </v-tooltip>
                  </td>
                </tr>
              </tbody>
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
import { Configuration, GMTApi } from "./../../../../../../axiosapi";
import { translate, i18n } from "./../../../../../../i18n";
//import SearchTable from "./FUNDED/GMTGrantProjectsSearch.vue";
import PrjSearch from "./GMTGrantOfficeProjectSearch.vue";
import PrjDetails from "./GMTGrantOfficeProjectDetails.vue";
import { EventBus } from "./../../../../../../event-bus";
import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "./../../../../../../axiosapi/api";

export default Vue.extend({
  props: {
    selectedGrantId: {
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
          textKey: "datasource.gmt.grant.gt-project-office-code",
          align: "center",
          value: "HR_OFFICE_CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Code",
          textKey: "datasource.gmt.grant.gt-project-code",
          align: "center",
          value: "PMS_PROJECT_CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Code",
          textKey: "datasource.gmt.grant.gt-project-code-descr",
          align: "center",
          value: "PMS_PROJECT_CODE_DESC",
          sortable: true,
          filterable: true,
        },

        {
          text: "Title",
          textKey: "datasource.gmt.grant.gt-title",
          align: "center",
          value: "GPR_DESCRIPTION",
          sortable: true,
          filterable: true,
        },
        // {
        //   text: "Location Description",
        //   textKey: "datasource.pms.annual-plan.ap-location",
        //   align: "center",
        //   value: "LOCATION_DESCR",
        //   sortable: true,
        //   filterable: true,
        // },
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
        {
          text: "Area",
          textKey: "datasource.gmt.grant.gt-project-area",
          align: "center",
          value: "GPR_AREA_DESCR",
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
    UpdateOffPrjsGrant(item: any) {
      let localThis: any = this as any;
      if (item != null) {
        localThis.itemsPerPage = 15;
        localThis.UpdateItems(item);
      }
    },
    generateAgreement(item: any) {
      let localThis = this as any;
      var id: number = 0;
      let ap = {} as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: GMTApi = new GMTApi(config);
      localThis.showWait = true;
      let doc: any;
      return api
        .apiGMTGetGMTTemplateGet(config.baseOptions)
        .then((res) => {
          doc = res.data;
          const byteCharacters = atob(doc);
          const byteNumbers = new Array(byteCharacters.length);
          for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
          }
          const byteArray = new Uint8Array(byteNumbers);
          const blob = new Blob([byteArray], {
            type:
              "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
          });

          var link = document.createElement("a");

          var url = URL.createObjectURL(blob);
          link.setAttribute("href", url);
          link.setAttribute("download", "gm.docx");
          link.style.visibility = "hidden";
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        })
        .catch((err) => {
          // console.error(err);
          return "";
        })
        .finally(() => (localThis.showWait = false));
    },
    UpdateItems(item: any) {
      let localThis = this as any;

      let i: number = 0;
      let j: number = 0;
      for (i = 0; i < item.length; i++) {
        let payLoadD = {} as any;
        payLoadD.ID_GRANT = localThis.selectedGrantId;
        payLoadD.PMS_PROJECT_CODE_ID = item[i].PMS_PROJECT_CODE_ID;

        let savePayload: GenericSqlPayload = {
          spName: "GMT_SP_INS_GRANT_OFFICE_PROJECT_CODE",
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
            }
            return res;
          })
          .catch((err: any) => {
            let exc: string = "";
            if (err == undefined || err.message == undefined) exc = "Error";
            else {
              exc = err.message;
            }
            localThis.showNewSnackbar({
              typeName: "error",
              text: exc,
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
        viewName: "GMT_VI_GRANT_PROJECTS_OFFICE_CODE",
        //colList: null,
        whereCond: `ID_GRANT=${localThis.selectedGrantId}`,
        orderStmt: "HR_OFFICE_CODE",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              if (item.GPR_AREA != undefined) {
                item.GPR_AREA = JSON.parse(item.GPR_AREA);
                let i: number = 0;
                item.GPR_AREA_DESCR = "";
                for (i = 0; i < item.GPR_AREA.length; i++) {
                  item.GPR_AREA_DESCR += item.GPR_AREA[i].PMS_TOS_DESCRIPTION + "<br>";
                }
              } else {
                item.GPR_AREA = {};
                item.GPR_AREA_DESCR = "";
              }
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
    },
    closeDialog(item: string) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as any;
      payLoadD.ID_GRANT = localThis.selectedGrantId;
      payLoadD.ID_OFFICE = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_GRANT_PROJECT_OFFICE",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getGrantProjects();

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

      let msg: string = this.$i18n.t("datasource.gmt.grant.gt-del-project").toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.showWait = true;
          let payLoadD = {} as any;
          payLoadD.ID = item.GP_ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_GRANT_PROJECT_OFFICE",
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
