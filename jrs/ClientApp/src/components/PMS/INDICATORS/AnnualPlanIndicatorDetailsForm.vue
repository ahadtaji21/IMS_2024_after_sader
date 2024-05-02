<template>

<div>
        <v-tabs
          v-model="tabIsVisible"
          background-color="primary darken-1"
          dark
          v-if="showObjectTabs"
        >
          <div class="vertical-center">
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="returnToIndicatorList()"
            >
              <v-icon>{{sIconBack}}</v-icon>
            </v-btn>
          </div>

          <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab>
          <v-spacer></v-spacer>
 <v-chip class="ma-2" color="default">
                <v-avatar left>
                  <v-icon>mdi-chart-line</v-icon>
                </v-avatar>
                {{localEditIndDesc}}
              </v-chip>
        </v-tabs>
        <v-tabs-items v-model="tabIsVisible" style="padding-left: 1em">
          <v-tab-item key="MAINDATA">
            <v-card>
              <!-- <ap-ind-main-data
                @closeDialoge="closeIndMainData"
                @closeDialogeAndSave="SaveIndMainData"
                :formData="editIndMainDataItem"
                :selectedObjectId="selectedObjectId"
              ></ap-ind-main-data> -->
            </v-card>
          </v-tab-item>

          <!-- NARRATIVE -->
          <v-tab-item key="NARRATIVE">
            <ap-narrative :selectedObjectId="editIndId" :tableName="tblName"></ap-narrative>
          </v-tab-item>
        
        </v-tabs-items>

</div>
</template>

<script lang="ts">
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";
import { translate } from "../../../i18n";
import AnnualPlanIndicatorMainData from "./AnnualPlanIndicatorMainDataForm.vue";
import Narrative from "../ANNUALPLAN/AnnualPlanNarrative.vue";
import { EventBus } from '../../../event-bus';
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
     "ap-narrative": Narrative
     //,
    // "ap-ind-main-data": AnnualPlanIndicatorMainData
  },
  props: {
    // selectedAnnualPlan: {
    //   type: Object
    // },
    editIndMainDataItem: {
      type: Object
    },
    editIndId: {
      type: Number
    },
    editIndDesc: {
      type: String,
      required: true
    },
   
  selectedObjectId: {
      type: Number,
      required: true
    },
    onlyRead:{
      type: Boolean,
      required:false,
      default:false
    }

  },
  data() {
    return {
      showObjectTabs: true,
      sIconBack: "mdi-chevron-double-left",
      tblName: "IND_INDICATOR",
      tabIsVisible: null,
      localEditIndDesc: "",
      editedIndMainDataItem: {},
      tabsItems: [
        { code: "MAINDATA", descr: "Main Data" },
        { code: "NARRATICE", descr: "Narrative" }
      ],
      oldInd: {}
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    hideIndecTabs() {
      let localThis: any = this as any;
      localThis.showObjectTabs = false;
    },

  returnToIndicatorList() {
      let localThis: any = this as any;
      EventBus.$emit("closeIndicatorDetails");
    },


    closeIndMainData() {
      let localThis = this as any;
      localThis.$emit("closeIndicatorDetails");
    },

    SaveIndMainData(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["ID"] == undefined) {
        msg = msgAdd;
      } else {
        id = Number(value["ID"]);
      }
      this.$confirm(msg).then(res => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OUTCOME_OBJECTIVE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(value)
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message
              }); // Feedback to user
              //   localThis.editedIndMainDataItem = localThis.itemModel;
              //   localThis.editedIndMainDataItem.ANNUAL_PLAN_ID =
              //     localThis.selectedAnnualPlan.id;

              localThis.$emit("reloadIndicators", value);
              localThis.localEditIndDesc = value["DESCRIPTION"];
              localStorage.SelectedInd=localThis.localEditIndDesc;
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message
              }); // Feedback to user
            });
        }
      });
    }
  },
  mounted() {
    let localThis = this as any;
    localThis.localEditIndDesc = localThis.editIndDesc;
    localStorage.SelectedInd=localThis.localEditIndDesc;
     EventBus.$on("hideSrvTabs", (data: any) => {
      //localThis.showSrvTabs = false;
    });

    // EventBus.$on("showSrvTabs", (data: any) => {
    //   localThis.showSrvTabs = true;
      
    //   EventBus.$emit("refreshBreadCumb", 2);
    // });
  },
  watch: {
    editIndDesc(desc: String) {
      let localThis = this as any;
      localThis.localEditIndDesc = desc;
      localStorage.SelectedInd=localThis.localEditIndDesc;
      EventBus.$emit("refreshBreadCumb", 2);
    }
  }
});
</script>
<style scoped>
.vertical-center {
  margin-right: 0.2rem;
  margin-top: 0.3rem;
}
</style>