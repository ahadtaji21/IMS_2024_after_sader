<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="outputList"
          item-key="PMS_OUTPUT_ID"
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
              <h3>{{ $t("datasource.pms.annual-plan.objectives.outputs") }}</h3>

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
    selectedOutputList: {
      type: Array,
      required: true,
    },
    selectedPositionId: {
      type: Number,
      default: 0,
      required: false,
    },
    selectedProcessId: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      headers: [],
      outputList: [],
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
          value: "PMS_OUTPUT_DESC",
          align: "start",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    checkIfAlreadyInserted(value: number) {
      let localThis: any = this as any;
      let i: number = 0;
      if (
        localThis.selectedOutputList == undefined ||
        localThis.selectedOutputList.length == 0
      )
        return false;
      for (i = 0; i < localThis.selectedOutputList.length; i++) {
        if (localThis.selectedOutputList[i].PMS_OUTPUT_ID == value) return true;
      }
      return false;
    },
    getItems() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.outputList = [];
      let itemsAlreadyInserted: any = [];
      localThis.outputListOrig = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_OUTPUT",
        colList: "PMS_OUTPUT_ID,PMS_PROCESS_ID,PMS_OUTPUT_DESC",
        whereCond: `PMS_PROCESS_ID = ${localThis.selectedProcessId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_OUTPUT_DESC",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            // res.table_data.forEach((item: any) => {
            //   if (item.PMS_OUTPUT_ID == localThis.selectedProcessId) {
            //     itemsAlreadyInserted.push(item);
            //   }
            // });

            res.table_data.forEach((item: any) => {
              if (!localThis.checkIfAlreadyInserted(item.PMS_OUTPUT_ID)) {
                localThis.outputList.push(item);
              }
            });
            // res.table_data.forEach((item: any) => {
            //   if (
            //     localThis.outputList.filter(
            //       (itemL: any) => itemL.PMS_OUTPUT_ID == item.PMS_OUTPUT_ID
            //     ).length == 0
            //   ) {
            //     if (
            //       itemsAlreadyInserted.filter(
            //         (itemL: any) => itemL.PMS_OUTPUT_ID == item.PMS_OUTPUT_ID
            //       ).length == 0
            //     ) {
            //       localThis.outputList.push(item);
            //     }
            //   }
            // });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateOutput", newVal);
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
        .t("datasource.pms.annual-plan.objectives.add-output")
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
    //       .t("datasource.pms.annual-plan.objectives.add-output")
    //       .toString();
    //     this.$confirm(msg).then((res: any) => {
    //       if (res) {
    //         localThis.updateValue(to[0].PMS_OUTPUT_ID);
    //       }
    //     });
    //localThis.selectedItm = [];
    //   }
    //},
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
