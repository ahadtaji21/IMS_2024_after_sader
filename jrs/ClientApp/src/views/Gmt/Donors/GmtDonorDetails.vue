<template>
  <v-content>
    <v-container fluid fill-height>
      <div v-if="mainDataItems">
        <h4 style="text-align: left">DONOR: {{ mainDataItems.GMT_DONOR_NAME }}</h4>
      </div>
      <div v-else>
        <h4>DONOR</h4>
      </div>
      <v-row>
        <v-col :cols="12">
          <v-card>
            <v-tabs v-model="tabIsVisible" background-color="primary darken-1" dark>
              <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab>
            </v-tabs>
            <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
              <v-tab-item key="MAINDATA">
                <v-card>
                  <v-card-title primary-title>Main Data</v-card-title>
                  <dn-main-data
                    :isUpdating="true"
                    @closeDialoge="closeDialogMainData"
                    :formData="mainDataItems"
                    :onlyRead="!authorizedRole"
                  ></dn-main-data>
                </v-card>
              </v-tab-item>

              <!-- DEPARTMENTS -->
              <v-tab-item key="DEP">
                <dn-dep
                  :selectedObjectId="mainDataItems.GMT_DONOR_ID"
                  :key="Math.ceil(Math.random() * 1000)"
                  :onlyRead="!authorizedRole"
                ></dn-dep>
              </v-tab-item>
              <!-- CHART OF ACCOUNT -->
              <v-tab-item key="COA" v-if="false">
                <dn-coa
                  :selectedObjectId="mainDataItems.GMT_DONOR_ID"
                  :key="Math.ceil(Math.random() * 1000)"
                ></dn-coa>
              </v-tab-item>

              <!-- CATEGORY OF INTERVENTION -->
              <v-tab-item key="COI" v-if="false">
                <dn-coi
                  :selectedObjectId="mainDataItems.GMT_DONOR_ID"
                  :key="Math.ceil(Math.random() * 1000)"
                ></dn-coi>
              </v-tab-item>

              <!-- TYPE OF SERVICE -->
              <v-tab-item key="TOS" v-if="false">
                <dn-tos
                  :selectedObjectId="mainDataItems.GMT_DONOR_ID"
                  :key="Math.ceil(Math.random() * 1000)"
                ></dn-tos>
              </v-tab-item>

              <!-- GRANTS -->
              <v-tab-item key="GRANTS">
                <dn-grant
                  :selectedObjectId="mainDataItems.GMT_DONOR_ID"
                  :key="Math.ceil(Math.random() * 1000)"
                ></dn-grant>
              </v-tab-item>

              <!-- PROJECTS FUNDED -->
              <v-tab-item key="PROJECTS" v-if="false">
                <dn-prjs-funded
                  :selectedObjectId="mainDataItems.GMT_DONOR_ID"
                ></dn-prjs-funded>
              </v-tab-item>
            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapActions } from "vuex";
import DonorMainData from "../../../components/GMT/DONORS/MAIN/GMTDonorMainDataForm.vue";
import DonorDepartments from "../../../components/GMT/DONORS/DEPARTMENTS/GmtDonorDepartmentList.vue";
import DonorPRJFunded from "../../../components/GMT/DONORS/PROJECTS/GMTDonorProjectsForm.vue";
import DonorCOA from "../../../components/GMT/DONORS/COA/GMTDonorChartOfAccountForm.vue";
import DonorCOI from "../../../components/GMT/DONORS/COI/GMTDonorCategoryOfInterventionForm.vue";
import DonorTOS from "../../../components/GMT/DONORS/TOS/GMTDonorTypeOfServiceForm.vue";
import DonorGRANT from "../../../components/GMT/DONORS/GRANTS/GMTDonorGrantForm.vue";
import TextEditor from "../../../components/TextEditor.vue";
import JrsTable from "../../../components/JrsTable.vue";
import JrsSimpleTable from "../../../components/JrsSimpleTable.vue";
import JrsModalForm from "../../../components/JrsModalForm.vue";
import JrsLocationPicker from "../../../components/JrsLocationPicker.vue";
import JrsSearchTable from "../../../components/JrsSearchTable.vue";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
//TMP
import { JrsHeader } from "../../../models/JrsTable";
import {
  JrsFormField,
  JrsFormFieldText,
  JrsFormFieldSelect,
} from "../../../models/JrsForm";
//TMP

import { mapGetters } from "vuex";
import { EventBus } from "../../../event-bus";
import { i18n } from "../../../i18n";
export default Vue.extend({
  name: "home",
  components: {
    "dn-main-data": DonorMainData,
    "dn-dep": DonorDepartments,
    "dn-prjs-funded": DonorPRJFunded,
    "dn-coa": DonorCOA,
    "dn-coi": DonorCOI,
    "dn-tos": DonorTOS,
    "dn-grant": DonorGRANT,
  },
  props: {
    editItemMainData: {
      type: Object,
      required: false,
    },
    editItemNarrativeId: {
      type: Number,
      required: false,
    },
    editItemOBJ: {
      type: Object,
      required: false,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      currentOffice: 0,
      tblName: "GMT_DONOR",
      authorizedRole: false,
      leavePage: false,
      mainDataItems: null,
      serviceRes: null,
      tabIsVisible: null,
      tabsItems: [
        { code: "MAINDATA", descr: "Main Data" },
        { code: "DEP", descr: "Contacts" },
        // { code: "COA", descr: "Chart Of Accounts" },
        // { code: "COI", descr: "Category of Intervention" },
        // { code: "TOS", descr: "Service" },
        { code: "GRANTS", descr: "Grants" },
        // { code: "PROJECTS", descr: "Projects" },
      ],

      genericQueryData: false,
      tmpSelectedRows: [],
      locationId: 22,
      profileId: null,
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
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
            name: "Donors List",
          });
        } else {
          (this as any).leavePage = false;
        }
      });
    },
    closeDialogAndSaveMainData(value: any) {
      // let localThis = this as any;
      // let msgUpd: string = this.$i18n
      //   .t("datasource.pms.annual-plan.ap-update-confirm")
      //   .toString();
      // let msgAdd: string = this.$i18n
      //   .t("datasource.pms.annual-plan.ap-add-confirm")
      //   .toString();
      // let id = 0;
      // let msg = msgUpd;
      // if (value["id"] == undefined) {
      //   msg = msgAdd;
      // } else {
      //   id = Number(value["id"]);
      // }
      // this.$confirm(msg).then(res => {
      //   if (res) {
      //     localThis.dialog = false;
      //     //(this as any).editedItem = (this as any).itemModel;
      //     //for (var key in value) {
      //     //    alert('keyy: ' + key + '\n' + 'value: ' + value[key]);
      //     //}
      //     const config: Configuration = localThis.$store.getters[
      //       "auth/getApiConfig"
      //     ];
      //     const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);
      //     //ADD
      //     return api
      //       .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
      //       .then(response => {
      //         //debugger
      //         //(this as any).getPMSAP();
      //         localThis.mainDataItems = value;
      //         localThis.mainDataItems.apcode =
      //           value.office_code + "_" + value.code;
      //         localThis.mainDataItems.status_in_bar=localThis.mainDataItems.status_name
      //         let gt = {} as any;
      //         gt.grant_id=value.id;
      //         gt.year = value.start_year;
      //         gt.currency = value.currency_code;
      //         localThis.setDonor(gt);
      //         localThis.showNewSnackbar({
      //           typeName: "success",
      //           text: "Succesfully updated Annual Plan main data."
      //         });
      //       })
      //       .catch(error => {
      //         alert(error);
      //       });
      //   }
      // });
    },
  },
  beforeMount() {
    let localThis = this as any;
    localThis.mainDataItems = localThis.editItemMainData;
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
    language() {
      return i18n.locale;
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.authorizedRole = false;
    localThis.currentOffice = localThis.getCurrentOffice.HR_OFFICE_ID;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    var role: any = localThis.getCurrentRole.ROLE_CODE;
    if (role == "GM" && localThis.currentOffice == 1) {
      //Grant Manager
      localThis.authorizedRole = true;
    }
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      if (to.ROLE_CODE == "GM" && localThis.currentOffice == 1) {
        //Grant Manager
        localThis.authorizedRole = true;
      }
    });
  },
  beforeRouteLeave(to, from, next) {
    let localThis = this as any;
    let gt = {} as any;
    gt.grant_id = 0;
    gt.year = 0;
    gt.currency_code = "";

    if (localThis.leavePage == false) {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          //   localThis.setGrant(gt);
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
