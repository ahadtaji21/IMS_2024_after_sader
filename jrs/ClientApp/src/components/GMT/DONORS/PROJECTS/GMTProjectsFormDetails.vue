<template>
  <v-container fluid fill-height>
    <div v-if="mainDataItems">
      <h4 style="text-align: left">ANNUAL PLAN: {{ mainDataItems.apcode }}</h4>
      <h5 style="text-align: left">STATUS: {{ mainDataItems.status_in_bar }}</h5>
    </div>
    <div v-else>
      <h4>ANNUAL PLAN</h4>
    </div>
    <v-row>
      <v-col :cols="12">
        <v-card>
          <v-tabs v-model="tabIsVisible" background-color="primary darken-1" dark>
            <div v-if="onlyRead" class="vertical-center">
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="returnToList()"
              >
                <v-icon>{{ sIconBack }}</v-icon>
              </v-btn>
            </div>
            <!-- <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab> -->
            <v-tab v-for="item in activeTabs" :key="item.code">{{ item.descr }}</v-tab>
          </v-tabs>
          <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
            <v-tab-item key="MAINDATA">
              <v-card>
                <v-card-title primary-title>Main Data</v-card-title>
                <ap-main-data
                  @closeDialoge="closeDialogMainData"
                  :onlyRead="onlyRead"
                  :formData="editItemMainData"
                  :isUpdatableForm="isPMSModule()"
                ></ap-main-data>
              </v-card>
            </v-tab-item>

            <!-- NARRATIVE -->
            <v-tab-item key="NARRATIVE">
              <ap-narrative
                :selectedObjectId="editItemNarrativeId"
                :tableName="tblName"
                :isUpdatableForm="isPMSModule()"
              ></ap-narrative>
            </v-tab-item>
            <!-- Objectives -->
            <v-tab-item key="OBJ">
              <ap-obj
                :selectedAnnualPlan="editItemOBJ"
                :onlyRead="onlyRead"
                :isUpdatableForm="isPMSModule()"
              ></ap-obj>
            </v-tab-item>
            <!-- Grants -->
            <v-tab-item key="GNT" :disabled="isPMSModule()">
              <ap-gnt :selectedAnnualPlan="editItemOBJ" :onlyRead="onlyRead"></ap-gnt>
            </v-tab-item>
            <!-- Budget Mapping from GMT-->
            <v-tab-item key="BMGMT" :disabled="isPMSModule()">
              <ap-bdg-gmt
                :selectedAnnualPlan="editItemOBJ"
                :onlyRead="onlyRead"
                :selectedObjectId="editItemOBJ.id"
              ></ap-bdg-gmt>
            </v-tab-item>
          </v-tabs-items>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapGetters } from "vuex";
import { mapActions } from "vuex";
import AnnualPlanMainData from "../../../PMS/ANNUALPLAN/AnnualPlanMainDataForm.vue";
import AnnualPlanOBJ from "../../../../components/PMS/OBJECTIVES/AnnualPlanObjectiveForm.vue";
import AnnualPlanGMT from "../../../../components/GMT/ANNUALPLAN/GRANTS/GMTProjectGrantsForm.vue";
import AnnualPlanBudgetMappingGMT from "../../../../components/GMT/MAPPING/GMTObjectMappingForm.vue";
import TextEditor from "../../../../components/TextEditor.vue";
import AnnualPlanNarrative from "../../../PMS/ANNUALPLAN/AnnualPlanNarrative.vue";
import JrsTable from "../../../../components/JrsTable.vue";
import JrsSimpleTable from "../../../../components/JrsSimpleTable.vue";
import JrsModalForm from "../../../../components/JrsModalForm.vue";
import JrsLocationPicker from "../../../../components/JrsLocationPicker.vue";
import JrsSearchTable from "../../../../components/JrsSearchTable.vue";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../axiosapi";
//TMP
import { JrsHeader } from "../../../../models/JrsTable";
import {
  JrsFormField,
  JrsFormFieldText,
  JrsFormFieldSelect,
} from "../../../../models/JrsForm";
//TMP

export default Vue.extend({
  name: "home",
  components: {
    "ap-main-data": AnnualPlanMainData,
    "ap-narrative": AnnualPlanNarrative,
    "ap-obj": AnnualPlanOBJ,
    "ap-gnt": AnnualPlanGMT,
    "ap-bdg-gmt": AnnualPlanBudgetMappingGMT,
  },
  props: {
    editItemMainData: {
      type: Object,
      default: undefined,
      required: false,
    },
    editItemNarrativeId: {
      type: Number,
      default: 0,
      required: false,
    },
    editItemOBJ: {
      type: Object,
      default: undefined,
      required: false,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    module: {
      type: String,
      required: false,
      default: "Grants",
    },
  },
  data() {
    return {
      tblName: "PMS_PROJECT",
      leavePage: false,
      sIconBack: "mdi-chevron-double-left",
      mainDataItems: {},
      serviceRes: null,
      tabIsVisible: null,
      tabsItems: [
        { id: 0, code: "MAINDATA", descr: "Main Data", active: true },
        { id: 1, code: "NARRATIVE", descr: "Narrative", active: true },
        { id: 2, code: "OBJ", descr: "Objectives", active: true },
        { id: 3, code: "GNT", descr: "Grants", active: true },
        { id: 4, code: "BMGMT", descr: "Budget Mapping", active: true },
      ],

      genericQueryData: false,
      tmpSelectedRows: [],
      location_id: 0,
      profileId: null,
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setAnnualPlan: "setAnnualPlan",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    returnToList() {
      let localThis: any = this as any;

      this.$emit("returnBackToList", localThis.editItemMainData);
    },
    isPMSModule() {
      let localThis = this as any;
      if (localThis.module == "PMS") return true;
      else return false;  
    },

    getList() {
      (this as any).serviceRes = null;
      axios({
        url:
          "https://jesuitrefugeeservice720.sharepoint.com/teams/TEST_NM_01/Shared%20Documents/SP_TEST_01.txt",
        method: "GET",
        responseType: "json",
      })
        .then((response) => {
          (this as any).serviceRes = response;
          // var fileURL = window.URL.createObjectURL(new Blob([response.data]));
          // var fileLink = document.createElement("a");
          // fileLink.href = fileURL;
          // fileLink.setAttribute("download", "file.pdf");
          // document.body.appendChild(fileLink);
          // fileLink.click();
        })
        .catch((err) => {
          (this as any).serviceRes = err;
        });
    },
    testPrint(str: string) {
      //(str || "TEST");
    },

    testGetGenericSelect() {
      (this as any).genericQueryData = !(this as any).genericQueryData;
    },
    closeDialogMainData() {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          (this as any).leavePage = true;
          this.$router.push({
            name: "Annual Plan List",
          });
        } else {
          (this as any).leavePage = false;
        }
      });
    },
    /*   closeDialogAndSaveMainData(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-add-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (value["id"] == undefined) {
        msg = msgAdd;
      } else {
        id = Number(value["id"]);
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          localThis.dialog = false;
          //(this as any).editedItem = (this as any).itemModel;
          //for (var key in value) {
          //    alert('keyy: ' + key + '\n' + 'value: ' + value[key]);
          //}
          const config: Configuration =
            localThis.$store.getters["auth/getApiConfig"];
          const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);

          //ADD
          return api
            .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
            .then((response) => {
              //debugger
              //(this as any).getPMSAP();
              localThis.mainDataItems = value;
              localThis.mainDataItems.apcode = value.office_code + "_" + value.code;
              localThis.mainDataItems.status_in_bar = localThis.mainDataItems.status_name;
              let ap = {} as any;
              ap.annal_plan_id = value.id;
              ap.start_year = value.start_year;
              ap.currency = value.currency_code;
              ap.location_id = value.location_id;
              ap.location_description = value.location_description;
              localThis.setAnnualPlan(ap);
              localThis.showNewSnackbar({
                typeName: "success",
                text: "Succesfully updated Annual Plan main data.",
              });
            })
            .catch((error) => {
              alert(error);
            });
        }
      });
    },*/
  },
  beforeMount() {
    let localThis: any = this;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
  },
  mounted() {
    let localThis = this as any;
    localThis.mainDataItems = localThis.editItemMainData;
    localThis.mainDataItems.status_in_bar = localThis.editItemMainData.status_name;
    localThis.mainDataItems.apcode = localThis.editItemMainData.apcode;
    localThis.mainDataItems.location_id = Number(localThis.editItemMainData.location_id);
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
    }),

    activeTabs: function () {
      let localThis = this as any;
      return localThis.tabsItems.filter(function (u: any) {
        return u.active;
      });
    },
  },
  beforeRouteLeave(to, from, next) {
    let localThis = this as any;
    let ap = {} as any;
    ap.annal_plan_id = 0;
    ap.start_year = 0;
    ap.end_year = 0;
    ap.currency_code = "";

    if (localThis.leavePage == false) {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.setAnnualPlan(ap);
          next();
        } else {
          next(false);
        }
      });
    } else {
      next();
    }
  },
});
</script>

<style scoped></style>
