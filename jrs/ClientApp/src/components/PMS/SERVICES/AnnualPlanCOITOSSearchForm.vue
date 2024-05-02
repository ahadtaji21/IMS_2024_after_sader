<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="itemList"
          item-key="PMS_COI_TOS_ID"
          multi-sort
          :search="tableFilter"
          :items-per-page="10"
          class="text-capitalize"
          v-model="selectedItm"
          show-select
          :single-select="true"
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
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
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
  },
  data() {
    return {
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
          value: "PMS_COI_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.description").toString(),
          value: "IMS_LABELS_VALUE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.type").toString(),
          value: "PMS_TOS_DESCRIPTION",
          align: "center",
          divider: true,
        }
       
      ];

      localThis.headers = tmp;
    },
    getItems() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.itemList = [];
      let itemsAlreadyInserted: any = [];
      localThis.itemListOrig = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_COI_TOS",
        colList: null,
        whereCond: `IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_COI_CODE",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.itemList.push(item);
            });
            //   if (item.ACTIVITY_ID == localThis.selectedActivityId) {
            //     itemsAlreadyInserted.push(item);
            //   }
            //});
            // res.table_data.forEach((item: any) => {
            //   if (
            //     localThis.itemList.filter(
            //       (itemL: any) => itemL.PMS_COI_TOS_ID == item.PMS_COI_TOS_ID
            //     ).length == 0
            //   ) {
            //     if (
            //       itemsAlreadyInserted.filter(
            //         (itemL: any) => itemL.PMS_COI_TOS_ID == item.PMS_COI_TOS_ID
            //       ).length == 0
            //     ) {
            //       localThis.itemList.push(item);
            //     }
            //   }
            //});
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
    selectedItm(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;
      if (to.length > 0) {
        let msg: string = this.$i18n
          .t("datasource.pms.annual-plan.objectives.add-item")
          .toString();

        this.$confirm(msg).then((res: any) => {
          if (res) {
            localThis.updateValue(to[0].PMS_COI_TOS_ID);
          }
        });
        //localThis.selectedItm = [];
      }
    },
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
