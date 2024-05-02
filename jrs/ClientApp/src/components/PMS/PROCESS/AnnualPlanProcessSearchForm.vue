<template>
  <div>
    <v-card>
      <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="processList"
          item-key="PMS_PROCESS_ID"
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
              <h3>{{ $t("datasource.pms.annual-plan.processes") }}</h3>

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
    selectedProcessList: {
      type: Array,
      required: true,
    },
    formDataExt: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      showWait: false,
      headers: [],
      processList: [],
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
          text: this.$i18n.t("datasource.pms.annual-plan.process").toString(),
          value: "PMS_PROCESS_DESC",
          align: "start",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    getItems() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.processList = [];
      let itemsAlreadyInserted: any = [];
      localThis.processListOrig = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_TARGET_PROCESS",
        colList: null,
        whereCond: `PMS_TARGET_ID=${localThis.formDataExt.PMS_TARGET_ID} AND PMS_TOS_ID= ${localThis.formDataExt.TOS.PMS_TOS_ID}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        //whereCond: `PMS_COI_TOS_REL_ID= ${1048}`,
        orderStmt: "PMS_PROCESS_DESC",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data && res.table_data.length > 0) {
            let x: number = 0;

            res.table_data.forEach((item: any) => {
              if (!localThis.checkIfAlreadyInserted(item.PMS_PROCESS_ID)) {
                localThis.processList.push(item);
              }
            });
          } else {
            //Generic Process
            let item: any = {};

            item.PMS_PROCESS_ID = 0;
            item.PMS_TOS_ID = localThis.formDataExt.TOS.PMS_TOS_ID;
            item.PMS_TARGET_ID = localThis.formDataExt.PMS_TARGET_ID;
            item.PMS_PROCESS_DESC = "Generic Process";
            item.LOGIC_MODEL_CODE = "";
            item.TARGET_POPULATION = "";
            item.ACTIVITY_TYPE_ID = 0;
            item.PMS_PROCESS_DESC_SHORT = "Generic Process";
            localThis.processList.push(item);
          }
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    checkIfAlreadyInserted(value: number) {
      let localThis: any = this as any;
      let i: number = 0;
      if (
        localThis.selectedProcessList == undefined ||
        localThis.selectedProcessList.length == 0
      )
        return false;
      for (i = 0; i < localThis.selectedProcessList.length; i++) {
        if (localThis.selectedProcessList[i].PMS_PROCESS_ID == value) return true;
      }
      return false;
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateServiceProcessList", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
    save() {
      let localThis: any = this as any;
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.add-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let i: number = 0;
          try {
            localThis.updateValue(localThis.selectedItm);
          } catch (error) {
            // Feedback to user
          }
        }
      });
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
    //         localThis.updateValue(to[0].PMS_PROCESS_ID);
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
