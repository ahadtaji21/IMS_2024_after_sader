<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="JRSCOIList"
          item-key="GMT_COI_ID"
          multi-sort
          :search="tableFilter"
          :items-per-page="10"
          class="text-capitalize"
          v-model="selectedJRSCoi"
          show-select
          :single-select="true"
        >
          <template v-slot:top>
            <div
              class="d-inline-flex align-center primary lighten-2 jrs-table-top"
            >
              <h3>{{ $t("datasource.gmt.donor.dnr-coi") }}</h3>
              <v-spacer></v-spacer>
              <div v-if="showWait">
                <v-progress-circular
                  indeterminate
                  color="primary"
                ></v-progress-circular>
              </div>
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
              <tr
                v-for="item in items"
                :key="item.GMT_COI_ID"
                style="height: 10px"
                @click="updateList(item)"
              >
                <td></td>
                <td class="tablerow">{{ item.GMT_COI_CODE }}</td>
                <td class="tablerow">{{ item.IMS_LABELS_VALUE }}</td>
         
                <td class="tablerow">
                  <v-icon
                    :color="
                      item.GMT_COI_ENABLED== true
                        ? 'green darken-3'
                        : 'deep-orange darken-3'
                    "
                    >{{
                      item.GMT_COI_ENABLED ==  true
                        ? "mdi-checkbox-marked-circle-outline"
                        : "mdi-checkbox-blank-circle-outline"
                    }}</v-icon
                  >
                </td>
              </tr>
            </tbody>
          </template>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-btn color="secondary darken-1" text @click="closeWindow()"
          >X {{ $t("general.close") }}</v-btn
        >
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
    selectedDonorId: {
      type: Number,
      required: true,
    },
    selectedResourceId: {
      type: Number,
      required: true,
    },
    alreadyInserted: {
      type: Array,
      required: false,
    },
  },
  data() {
    return {
      showWait: false,
      headers: [],
      JRSCOIList: [],
      tableFilter: "",
      selectedJRSCoi: [],
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
          text: this.$i18n.t("pms.coi-code").toString(),
          value: "GMT_COI_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.coi-description").toString(),
          value: "IMS_LABELS_VALUE",
          align: "center",
          divider: true,
        },

        // {
        //   text: this.$i18n.t("pms.donor-coa-type").toString(),
        //   value: "ACC_TYPE",
        //   align: "center",
        //   divider: true,
        // },

        {
          text: this.$i18n.t("pms.coi-enabled").toString(),
          value: "GMT_COI_ENABLED",
          align: "center",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    updateList(item: any) {
      let localThis: any = this as any;
      let msg: string = this.$i18n
        .t("datasource.gmt.donor.add-jrs-coi")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.updateValue(item);
        }
      });
    },
    getDCOA() {
      let localThis: any = this as any;
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_CATEGORY_OF_INTERVENTION",
        colList: null,
        whereCond: `GMT_DONOR_ID=${localThis.selectedDonorId} AND GMT_JRS_COI_ID=${localThis.selectedResourceId}  AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "GMT_COI_CODE", // "ID"
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              if (
                localThis.JRSCOIList.filter(
                  (itemL: any) => itemL.GMT_COI_ID == item.GMT_COI_ID
                ).length == 0
              ) {
                if (
                  localThis.alreadyInserted.filter(
                    (itemL: any) => itemL.PMS_JRS_COI_ID == item.GMT_JRS_COI_ID
                  ).length == 0
                ) {
                  localThis.JRSCOIList.push(item);
                }
              }
            });
          }
        })
        .then((res: any) => {
          localThis.showWait = false;
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateRelations", newVal);
    },
    /**
     * Clear field data.
     */
    closeWindow() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
  },
  watch: {
    // selectedJRSCoi(to: Array<any>, from: Array<any>) {
    //   let localThis: any = this as any;
    //   if (to.length > 0) {
    //     let msg: string = this.$i18n
    //       .t("datasource.gmt.donor.add-jrs-coa")
    //       .toString();
    //     this.$confirm(msg).then((res: any) => {
    //       if (res) {
    //         localThis.updateValue(to[0]);
    //       }
    //     });
    //     //localThis.selectedJRSCoi = [];
    //   }
    // },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.selectedJRSCoi = [];
    localThis.getDCOA();
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