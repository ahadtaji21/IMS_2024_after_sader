<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="projectList"
          item-key="ID"
          multi-sort
          :search="tableFilter"
          :items-per-page="10"
          class="text-capitalize"
          v-model="selectedPrj"
          show-select
          :single-select="true"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>{{$t('datasource.pms.annual-plan.ap-projects')}}</h3>

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
      <v-card-actions v-if="CloseBtn">
        <v-btn color="secondary darken-1" text @click="clearField()">X {{ $t('general.close') }}</v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { JrsHeader } from "../../../../models/JrsTable";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
export default Vue.extend({
  props: {

    selectedGrantId: {
      type: Number,
      default: 0,
      required: true,
    },
    CloseBtn: {
      type:Boolean
    }
  },
  data() {
    return {
      headers: [],
      projectList: [],
      tableFilter: "",
      selectedPrj: [],
      itemKey: "",
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
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("pms.code").toString(),
          value: "APCODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-description").toString(),
          value: "DESCR",
          align: "center",
          divider: true,
        },
                {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-location").toString(),
          value: "LOCATION_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.annual-plan.ap-status-name").toString(),
          value: "STATUS",
          align: "center",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    getItems() {
      let localThis: any = this as any;
      if(localThis.selectedGrantId==undefined || isNaN(localThis.selectedGrantId) || localThis.selectedGrantId==0) return;
      localThis.selectedItem = null;
      localThis.projectList = [];
      let itemsAlreadyInserted: any = [];
      localThis.projectListOrig = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_ANNUAL_PLAN",
        colList: null,
        whereCond: null, //`ACTIVITY_ID is NULL OR ACTIVITY_ID<>${localThis.selectedActivityId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "APCODE",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              if (item.GRANT_ID == localThis.selectedGrantId) {
                itemsAlreadyInserted.push(item);
              }
            });
            res.table_data.forEach((item: any) => {
              if (
                localThis.projectList.filter(
                  (itemL: any) => itemL.ID == item.ID
                ).length == 0
              ) {
                if (
                  itemsAlreadyInserted.filter(
                    (itemL: any) => itemL.ID == item.ID
                  ).length == 0
                ) {
                  localThis.projectList.push(item);
                }
              }
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateItem", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
  },
  watch: {
    selectedPrj(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;
      if (to.length > 0) {
        let msg: string = this.$i18n
          .t("datasource.gmt.grant.gt-add-prj-to-fund")
          .toString();

        this.$confirm(msg).then((res: any) => {
          if (res) {
            localThis.updateValue(to[0].ID);
          }
        });
        //localThis.selectedPrj = [];
      }
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.selectedPrj = [];
    localThis.getItems();
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