<template>
  <div>
    <div>
      <!-- SRV SELECTION-->
      <v-row v-if="showWait">
        <v-col>
          <div class="text-center" v-if="showWait">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <v-data-table
            :headers="headers"
            :items="countryList"
            item-key="ID"
            sort-by=""
            multi-sort
            :search="tableFilter"
            :items-per-page="1000"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedObj"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("datasource.gmt.grant.gt-prjs") }}</h4>
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog" persistent max-width="1100px" v-if="!onlyRead">
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
                    :onlyRead="onlyRead"
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
            <template v-slot:body="{ items }">
              <tr v-for="(item, index) in items" :key="item.ID">
                <td :class="'descClass'">
                  <v-checkbox
                    v-model="selected[index]"
                    value
                    hide-details
                    class="shrink ma-0 pa-0"
                    @change="checkboxUpdated($event, index, item.ID, item)"
                  ></v-checkbox>
                </td>
                <td :class="'descClass'">
                  <span>{{ item.COUNTRY }}</span>
                </td>
                <td :class="'descClass'">
                  <span>{{ item.AMOUNT }}</span>
                </td>
                <td :class="'descClass'">
                  <span>{{ item.CURRENCY_CODE }}</span>
                </td>

                <td :class="'descClass'" style="text-align: center">
                  <v-tooltip bottom>
                    <template v-slot:activator="{ on }">
                      <v-icon
                        small
                        class="mr-2"
                        @click="generateAgreement(item)"
                        v-on="on"
                        >mdi-file-word</v-icon
                      >
                    </template>
                    <span>{{ $t("datasource.gmt.grant.gt-agreement-letter") }}</span>
                  </v-tooltip>
                </td>
              </tr>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <div v-if="showProjectList">
      <gt-prj-list
        :selectedGrantId="selectedGrantId"
        :country_id="country_id"
        :isUpdatableForm="true"
        :onlyRead="onlyRead"
        @updateCountryProjects="updateCountryProjects"
        :key="keyRnd"
      >
      </gt-prj-list>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../JrsTable.vue";
import { JrsHeader } from "../../../../../models/JrsTable";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";
import Projects from "./GMTGrantProjects.vue";
import PrjSearch from "./GMTGrantProjectSearch.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";
import { Configuration, GMTApi } from "../../../../../axiosapi";
export default Vue.extend({
  components: {
    "gt-prj-list": Projects,
    "gmt-prj-search": PrjSearch,
  },
  props: {
    selectedGrantId: {
      type: Number,
      required: true,
    },

    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },

  data() {
    return {
      dialog: false,
      prjs: [],
      index: 0,
      selected: [],
      showWait: false,
      keyRnd: 0,
      bdg: 0,
      resourceId: {},
      json_data: "",
      selectedActivityId: 0,
      selectedActivityItemId: 0,
      selectedResource: "",
      selectedResourceId: 0,
      editObjId: 0,
      showActTabs: true,
      editObjDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showProjectList: false,
      selectedObj: [],
      tableFilter: "",
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      country_id: 0,
      coi: [],
      tos: [],
      countryList: [] as any,
      occ: [],
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

    closeDialog() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog = {};
    },
    closeEdit() {
      let localThis = this as any;
      localThis.dialog = false;
      localThis.showPrjDetails = false;
      localThis.getGmtPrjCountries();
    },
    checkboxUpdated(newValue: any, index: number, rel_id: number, item: any) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showProjectList = newValue;
        if (newValue) {
          localThis.country_id = item.COUNTRY_ID; // localThis.countryList[index].PMS_CC_KEY_ID;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        }
      }
    },
    reloadCC(item: any) {
      let localThis = this as any;

      localThis.getGmtPrjCountries();
    },
    updateCountryProjects() {
      let localThis = this as any;
      localThis.getGmtPrjCountries();
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
        whereCond: `ID_GRANT=${localThis.selectedGrantId}`,
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
        .finally(() => {
          localThis.showWait = false;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        });
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
    getGmtPrjCountries() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.showWait = true;
      localThis.countryList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "GMT_VI_PROJECTS_COUNTRY";
      let wherecond: string = `ID_GRANT = ${localThis.selectedGrantId}`;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.countryList.push(item);
            });
          }
          localThis.showWait = false;
          localThis.getGrantProjects();
        }
      );
    },
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
              localThis.getGmtPrjCountries();
              localThis.dialog = false;
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
    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },
    editActivities(item: any) {
      let localThis: any = this as any;
      localThis.selectedObj = [];
      localThis.selectedObj.push(item);
    },

    removeUndefined: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string;
    },

    setupHeaders() {
      let localThis: any = this as any;
      let descWidth: string;
      descWidth = "140px";

      let tmp: JrsHeader[] = [
        {
          text: "",
          value: "",
          align: "center",
          divider: true,
          width: "20px",
        },
        {
          text: this.$i18n.t("datasource.gmt.grant.gt-country").toString(),
          value: "COUNTRY",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.gmt.grant.gt-amount").toString(),
          value: "AMOUNT",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: this.$i18n.t("datasource.gmt.grant.gt-currency").toString(),
          value: "CURRENCY_CODE",
          align: "center",
          divider: true,
          width: descWidth,
        },
        {
          text: "Actions",
          value: "action",
          align: "center",
          width: descWidth,
          sortable: false,
        },
      ];

      localThis.headers = tmp;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
  },
  mounted() {
    let localThis: any = this as any;

    localThis.setupHeaders();
    if (localThis.selectedGrantId > 0) {
      localThis.getGmtPrjCountries();
    }

    EventBus.$on("closeActivityDetails", (data: any) => {
      localThis.reloadCC();
      EventBus.$emit("showObjTabs");
      // EventBus.$emit("sectActiveTab", "ACTIVITIES");
    });

    EventBus.$on("reloadActivities", (data: any) => {
      localThis.reloadCC(data);
    });
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 25) return string.substring(0, 25) + "...";
      else return string;
    },
  },
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;

      localThis.getGmtPrjCountries();
      localThis.setupHeaders();
    },
    // showActivityDetails(to: boolean) {
    //   let localThis: any = this as any;
    //   localThis.showObjDetails = to;
    // },
    selectedGrantId(to: number) {
      let localThis: any = this as any;
      localThis.selectedGrantId = to;

      localThis.getGmtPrjCountries();
    },

    editMode(to: boolean, from: boolean) {},
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}

.overall-node {
  margin: 1em 0.5em;
}
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.selectedCell {
  background-color: whitesmoke;
  color: black;
  font-size: 12px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
  padding: 0px;
  margin: 0px;
}
.notSelectedCell {
  background-color: white;
  font-size: 1px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
}

.descClass {
  font-size: 10px;
  width: 150px !important;
  border-bottom-style: solid;
  border-bottom-width: 1px;
  border-bottom-color: gainsboro;
  height: 10px;
}
</style>
