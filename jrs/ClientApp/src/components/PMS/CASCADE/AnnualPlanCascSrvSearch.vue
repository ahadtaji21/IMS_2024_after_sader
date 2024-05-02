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
          :items="srvList"
          item-key="PMS_COI_TOS_ID"
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
              <h3>{{ $t("datasource.pms.annual-plan.objectives.services") }}</h3>

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
    selectedsrvList: {
      type: Array,
      required: true,
    },
    selectedCategoryId: {
      type: Number,
      required: true,
    },
    selectedTargetId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      showWait: false,
      headers: [],
      srvList: [],
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
          text: this.$i18n.t("datasource.pms.type-of-service.type-of-service").toString(),
          value: "PMS_TOS_DESCRIPTION",
          align: "start",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    getItems() {
      let localThis: any = this as any;
      let viwName: string = "";
      let condition: string = "";
      localThis.selectedItem = null;
      localThis.srvList = [];
      let itemsAlreadyInserted: any = [];
      localThis.srvListOrig = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      viwName = "PMS_VI_TYPE_OF_SERVICE_TARGET";

      let selectPayload: GenericSqlSelectPayload = {
        viewName: viwName,
        colList: "PMS_TOS_ID,PMS_TOS_DESCRIPTION,PMS_COI_TOS_ID",
        whereCond: `PMS_COI_ID=${localThis.selectedCategoryId}  AND PMS_TARGET_ID=${localThis.selectedTargetId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        //whereCond: `PMS_COI_TOS_REL_ID= ${1048}`,
        orderStmt: "PMS_TOS_DESCRIPTION",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data && res.table_data.length > 0) {
            let x: number = 0;

            res.table_data.forEach((item: any) => {
              if (!localThis.checkIfAlreadyInserted(item.PMS_COI_TOS_ID)) {
                localThis.srvList.push(item);
              }
            });
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
      if (localThis.selectedsrvList == undefined || localThis.selectedsrvList.length == 0)
        return false;
      for (i = 0; i < localThis.selectedsrvList.length; i++) {
        if (localThis.selectedsrvList[i].PMS_COI_TOS_ID == value) return true;
      }
      return false;
    },
    updateValue(newVal: any) {
      (this as any).$emit("updateServicesrvList", newVal);
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
