<template>
  <div>
    <div>
      <v-card>
        <v-card-title></v-card-title>
        <v-card-text>
          <v-data-table
            :headers="headers"
            :items="indicatorList"
            item-key="PMS_SSS_IND_ID"
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
                <h3>{{ $t("datasource.pms.annual-plan.objectives.indicators") }}</h3>

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
    <v-dialog
      v-model="showIndicatorDetails"
      persistent
      :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
    >
      <pms-outp-ind-det-form
        @closeOutpDetDialoge="closeDialog"
        :formData="formData"
        @closeDialogeAndSaveSSSInd="closeDialog"
        :key="Math.ceil(Math.random() * 1000)"
        :versioned="versioned"
      ></pms-outp-ind-det-form>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import outpIndDet from "./AnnualPlanSSSIndicatorMainDataForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "pms-outp-ind-det-form": outpIndDet,
    //"item-indicator-details": ItemIndicator,
  },
  props: {
    indicatorsAlreadyAdded: {
      type: Array,
      required: false,
    },
    selectedSSSId: {
      type: Number,
      default: 0,
      required: true,
    },

    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      formData: {},
      nbrOfColumns: 2,
      sssIndicatorId: 0,
      showIndicatorDetails: false,
      headers: [],
      indicatorList: [],
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
        // {
        //   text: this.$i18n.t("pms.indicator-question").toString(),
        //   value: "PMS_SSS_IND_QUESTION",
        //   align: "start",
        //   divider: true,
        // },
        {
          text: this.$i18n.t("pms.indicator-details").toString(),
          value: "PMS_SSS_IND",
          align: "start",
          divider: true,
        },
                {
          text: this.$i18n.t("pms.indicator.techniques").toString(),
          value: "PMS_OBJ_IND_TECHNIQUE",
          align: "start",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    closeDialog() {
      let localThis: any = this as any;
      localThis.showIndicatorDetails = false;
      localThis.$emit("UpdateItem", "1");
    },
    getIndicators() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.indicatorList = [];
      let itemsAlreadyInserted: any = [];
      localThis.indicatorListOrig = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_SSS_INDICATOR",
        colList: null,
        whereCond: `PMS_SSS_KEY_ID = ${localThis.selectedSSSId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_SSS_IND_QUESTION",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              if (localThis.indicatorsAlreadyAdded != undefined) {
                if (
                  localThis.indicatorsAlreadyAdded.filter(
                    (itemL: any) => itemL.PMS_SSS_IND_ID == item.PMS_SSS_IND_ID
                  ).length == 0
                ) {
                  localThis.indicatorList.push(item);
                }
              } else {
                localThis.indicatorList.push(item);
              }
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    closeDialogAndSave() {
      let localThis: any = this as any;
      localThis.showIndicatorDetails = false;
      localThis.$emit("UpdateItem", "1");
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
    updateValue(newVal: String) {
      (this as any).$emit("UpdateSSSIndicators", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.$emit("UpdateItem", null);
    },
  },
  watch: {
    // selectedItm(to: Array<any>, from: Array<any>) {
    //   let localThis: any = this as any;
    //   if (to.length > 0) {
    //     let localThis: any = this as any;
    //     localThis.formData = {} as any;
    //     localThis.formData.sssId = localThis.selectedSSSId;
    //     localThis.formData.sssIndicatorId = to[0].PMS_SSS_IND_ID;
    //     localThis.formData.QUESTION = to[0].PMS_SSS_IND_QUESTION;
    //     localThis.formData.INDICATOR = to[0].PMS_SSS_IND;
    //     localThis.formData.STANDARD = to[0].PMS_OBJ_IND_STANDARD;
    //     localThis.formData.TECHNIQUE = to[0].PMS_OBJ_IND_TECHNIQUE;
    //     localThis.formData.DISAGGREGATED = to[0].PMS_OBJ_IND_DISAGGREGATED;
    //     localThis.formData.PERIODICITY = to[0].PMS_OBJ_IND_PERIODICITY;
    //     localThis.showIndicatorDetails = true;
    //   }
    // },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.selectedItm = [];
    localThis.getIndicators();
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
