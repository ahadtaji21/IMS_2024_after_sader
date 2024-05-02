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
          :items="budgetresourcelistList"
          item-key="PMS_BUDGET_RESOURCE_LIST_ID"
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
              <h3>{{ $t("pms.budgetresource-list") }}</h3>

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
    InsertedBudgetResourceElement: {
      type: Array,

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
      showWait: false,
      headers: [],
      budgetresourcelistList: [],
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
          text: this.$i18n.t("pms.description").toString(),
          value: "PMS_BUDGET_RESOURCE_LIST_DESC",
          align: "center",
          divider: true,
        },
      ];

      localThis.headers = tmp;
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
      (this as any).$emit("UpdateItem", newVal);
    },
    getItems() {
      let localThis: any = this as any;
      localThis.showWait = true;
      localThis.selectedItem = null;
      localThis.budgetresourcelistList = [];
      let itemsAlreadyInserted: any = [];
      localThis.budgetresourcelistListOrig = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_BUDGET_RESOURCE_LIST",
        colList: null,
        whereCond: null, //`ACTIVITY_ID is NULL OR ACTIVITY_ID<>${localThis.selectedActivityId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_BUDGET_RESOURCE_LIST_DESC",
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
              if (
                localThis.InsertedBudgetResourceElement != undefined &&
                localThis.InsertedBudgetResourceElement.filter(
                  (itemL: any) =>
                    itemL.PMS_BUDGET_RESOURCE_LIST_ID == item.PMS_BUDGET_RESOURCE_LIST_ID
                ).length == 0
              ) {
                //   if (
                //     itemsAlreadyInserted.filter(
                //       (itemL: any) => itemL.PMS_BUDGET_RESOURCE_LIST_ID == item.PMS_BUDGET_RESOURCE_LIST_ID
                //     ).length == 0
                //   ) {
                //     localThis.budgetresourcelistList.push(item);
                //   }
                // }
                localThis.budgetresourcelistList.push(item);
              }
            });
            localThis.showWait = false;
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
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
    // selectedItm(to: Array<any>, from: Array<any>) {
    //   let localThis: any = this as any;
    //   if (to.length > 0) {
    //     let msg: string = this.$i18n
    //       .t("datasource.pms.annual-plan.objectives.add-item")
    //       .toString();
    //     this.$confirm(msg).then((res: any) => {
    //       if (res) {
    //         localThis.updateValue(to[0].PMS_BUDGET_RESOURCE_LIST_ID);
    //       }
    //     });
    //     //localThis.selectedItm = [];
    //   }
    // },
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
