<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="donorList"
          item-key="GMT_DONOR_ID"
          multi-sort
          :search="tableFilter"
          class="text-capitalize elevation-2"
          v-model="selectedItm"
          show-select
          :single-select="true"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>{{ $tc("datasource.gmt.donor.donors", 2) }}</h3>

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
        <v-btn color="secondary darken-1" text @click="clearField()"
          >X {{ $t("general.close") }}</v-btn
        >
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import { translate } from "../../../i18n";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {},
  data() {
    return {
      selectedItm: [],
      selectedCountry: "",
      countries: [],
      authorizedRole: false,
      module: "",
      showWait: false,
      keyDialog: 0,
      dialog: false,
      headers: [],
      donorList: [],
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
    language() {
      return i18n.locale;
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.selectedItm = [];
    localThis.getDonors();
    localThis.setupHeaders();
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
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    getDonors() {
      let localThis = this as any;
      localThis.selecteddonor = null;
      localThis.donorList = [];
      localThis.showWait = true;
      let wherecond = null;
      if (localThis.selectedCountry != undefined && localThis.selectedCountry != "") {
        wherecond = `COUNTRIES LIKE '%${localThis.selectedCountry}%'`;
      }

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR",
        //colList: null,
        whereCond: wherecond,
        orderStmt: "GMT_DONOR_ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.donorList.push(item);
            });
          }
          localThis.showWait = false;
        });
    },

    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateItem", newVal);
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.gmt.donor.name").toString(),
          align: "center",
          value: "GMT_DONOR_NAME",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.grant.gt-type").toString(),

          align: "center",
          value: "IMS_LOOKUP_VALUE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.donor.description").toString(),

          align: "center",
          value: "GMT_DONOR_DESCRIPTION",
          sortable: true,
          divider: true,
        },
      ];
      localThis.headers = tmp;
    },
  },
  beforeMount() {
    let localThis = this as any;
  },
  filters: {
    subStr: function (string: any) {
      if (string != undefined) return string.substring(0, 40) + "...";
      else return "";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getDonors();
      localThis.setupHeaders();
    },

    selectedItm(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;
      if (to.length > 0) {
        let msg: string = this.$i18n
          .t("datasource.pms.annual-plan.objectives.add-item")
          .toString();

        this.$confirm(msg).then((res: any) => {
          if (res) {
            localThis.updateValue(to[0].GMT_DONOR_ID);
          }
        });
        //localThis.selectedItm = [];
      }
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
