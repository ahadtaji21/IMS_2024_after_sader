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
          :items="itemList"
          item-key="PMS_JRS_COA_ID"
          multi-sort
          :search="tableFilter"
          :items-per-page="10"
          class="text-capitalize"
          v-model="selectedItm"
          show-select
          :single-select="false"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>{{ $t("datasource.pms.annual-plan.objectives.items") }}</h3>

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
        <v-btn text color="primary" @click="save()">{{ $t("general.save") }}</v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  props: {
    selectedPositionId: {
      type: Number,
      default: 0,
      required: false,
    },
    selectedActivityId: {
      type: Number,
      default: 0,
      required: true,
    },
    selectedResShopId: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      showWait: false,
      headers: [],
      itemList: [],
      tableFilter: "",
      selectedItm: [],
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
          value: "PMS_JRS_COA_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.description").toString(),
          value: "PMS_JRS_COA_DESCRIPTION",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.type").toString(),
          value: "PMS_JRS_COA_TYPE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.category").toString(),
          value: "PMS_JRS_COA_CATEGORY",
          align: "center",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },

    getItems() {
      let localThis: any = this as any;
      localThis.showWait = true;
      localThis.selectedItem = null;
      localThis.itemList = [];
      let itemsAlreadyInserted: any = [];
      localThis.itemListOrig = [];
      let company: string = localThis.getCurrentOffice.HR_OFFICE_CODE.substring(0, 3);
      let i: number = 0;
      let j: number = 0;
      let wherecond: string = "";
      let vname: string = "PMS_VI_JRS_CHART_OF_ACCOUNT";
      if (localThis.selectedResShopId[0] == 0) {
        wherecond =
          "PMS_JRS_COMPANY='" + company + "' AND upper(PMS_JRS_COA_TYPE)='POSTING'";
      } else {
        wherecond = `PMS_JRS_COMPANY='${company}' AND upper(PMS_JRS_COA_TYPE)='POSTING' AND resId in (${localThis.selectedResShopId})`;
        vname = "PMS_VI_JRS_CHART_OF_ACCOUNT_RESOURCE";
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: vname,
        colList: null,
        whereCond: wherecond, //`ACTIVITY_ID is NULL OR ACTIVITY_ID<>${localThis.selectedActivityId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            // res.table_data.forEach((item: any) => {
            //   if (item.ACTIVITY_ID == localThis.selectedActivityId) {
            //     itemsAlreadyInserted.push(item);
            //   }
            // });
            res.table_data.forEach((item: any) => {
              // if (
              //   localThis.itemList.filter(
              //     (itemL: any) => itemL.PMS_JRS_COA_ID == item.PMS_JRS_COA_ID
              //   ).length == 0
              // ) {
              //   if (
              //     itemsAlreadyInserted.filter(
              //       (itemL: any) => itemL.PMS_JRS_COA_ID == item.PMS_JRS_COA_ID
              //     ).length == 0
              //   ) {
              //     localThis.itemList.push(item);
              //   }
              // }
              localThis.itemList.push(item);
            });
          }
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },

    save() {
      let localThis: any = this as any;
      if (localThis.selectedItm == undefined || localThis.selectedItm.length == 0) return;
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.add-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          // let i:number=0;
          // for(i=0;i<localThis.selectedItm.length;i++)
          // {
          localThis.updateValue(localThis.selectedItm);
          // }
        }
      });
    },

    updateValue(newVal: []) {
      let localThis: any = this as any;
      localThis.$emit("UpdateItem", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  watch: {
    /* selectedItm(to: Array<any>, from: Array<any>) {
        let localThis: any = this as any;
        if (to.length > 0) {
          let msg: string = this.$i18n
            .t("datasource.pms.annual-plan.objectives.add-item")
            .toString();

          this.$confirm(msg).then((res: any) => {
            if (res) {
              localThis.updateValue(to[0].PMS_JRS_COA_ID);
            }
          });
          //localThis.selectedItm = [];
        }
      }, */
  },
  mounted() {
    let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.selectedItm = [];
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
