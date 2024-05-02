<template>
  <v-container fluid fill-height>
    <div style="width: 100%">
      <v-row v-if="mainData">
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
          <div v-if="showMainData && mainDataItems">
            <h4 style="text-align: left">
              GRANT: {{ mainDataItems ? mainDataItems.CODE : null }}
            </h4>
            <h5 style="text-align: left">DONOR: {{ donor_name }}</h5>
            <h5 style="text-align: left">
              STATUS: {{ mainDataItems ? mainDataItems.IMS_STATUS_NAME : null }}
            </h5>
          </div>
          <div v-else>
            <h4>GRANT</h4>
          </div>
        </v-col>
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 8"></v-col>
        <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2">
          <!-- {{ authorizedRole }}
          <div v-if="isAuthorized">
            <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
          </div> -->
        </v-col>
      </v-row>
    </div>
    <div style="width: 100%">
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
              <v-tab v-for="item in tabsItems" :key="item.code">{{ item.descr }}</v-tab>
            </v-tabs>
            <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
              <v-tab-item key="MAINDATA">
                <v-card>
                  <v-card-title primary-title style="background-color:#42a5f5;color:white;font-weight:bold">Main Data</v-card-title>
                  <gt-main-data
                    @closeDialoge="closeDialogMainData"
                    :formData="editItemMainData"
                    :donorPresetted="donorPresetted"
                    @closeGrantDialogeAndSave="closeDialogAndSaveMainData"
                    :onlyRead="onlyRead || !isAuthorized"
                  ></gt-main-data>
                </v-card>
              </v-tab-item>
              <v-tab-item key="CONTACTS">
                <gt-contact
                  :formData="editItemMainData"
                  :key="Math.ceil(Math.random() * 1000)"
                  :onlyRead="!authorizedRole"
                ></gt-contact>
              </v-tab-item>
              <!-- NARRATIVE -->
              <v-tab-item key="NARRATIVE">
                <gt-narrative
                  :onlyRead="onlyRead || !isAuthorized"
                  :selectedObjectId="editItemMainData ? editItemMainData.ID : null"
                  :tableName="tblName"
                  :isUpdatableForm="!(onlyRead || !isAuthorized)"
                ></gt-narrative>
              </v-tab-item>
              <!-- PROJECTS FUNDED -->
              <!-- <v-tab-item key="PROJECTS"> -->
              <!-- <gt-prjs-funded 
                  :selectedObjectId="editItemMainData.ID"
                  @showPrjDetails="showPrjDetails"
            ></gt-prjs-funded>-->
              <!-- <ap-obj :selectedGrant="editItemOBJ"></ap-obj> -->

              <!-- </v-tab-item> -->
              <!-- Objectives -->
              <!-- GRANT EVENT -->
              <v-tab-item key="EVENTS">
                <gt-event-calendar
                  :selectedGrantId="editItemMainData ? editItemMainData.ID : null"
                  :isUpdatableForm="true"
                  :onlyRead="onlyRead || !isAuthorized"
                ></gt-event-calendar>
              </v-tab-item>
              <v-tab-item key="DOCUMENTS">
                <gt-docs
                  :selectedGrantId="editItemMainData ? editItemMainData.ID : null"
                  :isUpdatableForm="true"
                  :onlyRead="onlyRead || !isAuthorized"
                ></gt-docs>
              </v-tab-item>
              <v-tab-item key="PROJECTS">
                <gt-prjs
                  :selectedGrantId="editItemMainData ? editItemMainData.ID : null"
                  :isUpdatableForm="true"
                  :onlyRead="onlyRead || !isAuthorized"
                ></gt-prjs>
              </v-tab-item>
              <v-tab-item key="FUNDING_STATUS">
                <gt-fs
                  :selectedGrantId="editItemMainData ? editItemMainData.ID : null"
                  :isUpdatableForm="true"
                  :onlyRead="onlyRead || !isAuthorized"
                  :currency="currency"
                ></gt-fs>
              </v-tab-item>
              <!-- <v-tab-item key="AGREEMENT">
                <gt-agreement></gt-agreement>
              </v-tab-item> -->
            </v-tabs-items>
          </v-card>
        </v-col>
      </v-row>
    </div>
  </v-container>
</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import GrantMainData from "./GMTGrantMainDataForm.vue";
// import GrantCOUNTRYPRJFunde from "./GMTGrantProjectsFunded.vue";
import GrantOBJ from "./DOCS/GMTGrantSupportingDoc.vue";
import GrantCOUNTRYPRJ from "./PROJECTS/OFFICE/GMTGrantOfficeProject.vue";
import TextEditor from "../../../TextEditor.vue";
import GrantNarrative from "./NARRATIVE/GMTGrantNarrative.vue";
import GrantContact from "./CONTACTS/GMTGrantContactList.vue";
import GrantEventCalendar from "../../../../views/Gmt/Grants/GmtGrantEventCalendar.vue";
import GrantDoc from "./AGREEMENT/GMTAgreementLetter.vue";
import FundinStatus from "./FUNDING/GmtFundingStatusList.vue";
//import JrsTable from "../../../TJrsTable.vue";
//import JrsSimpleTable from "../../../TJrsSimpleTable.vue";
//import JrsModalForm from "../../../TJrsModalForm.vue";
//import JrsLocationPicker from "../../../TJrsLocationPicker.vue";
//import JrsSearchTable from "../../../TJrsSearchTable.vue";
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
    "gt-main-data": GrantMainData,
    "gt-narrative": GrantNarrative,
    "gt-contact": GrantContact,
    "gt-event-calendar": GrantEventCalendar,
    // "gt-prjs-funded": GrantCOUNTRYPRJFunde,
    "gt-docs": GrantOBJ,
    "gt-prjs": GrantCOUNTRYPRJ,
    "gt-fs": FundinStatus,
    // "gt-agreement": GrantDoc,
  },
  props: {
    editItemMainData: {
      type: Object,
    },

    mainData: {
      type: Boolean,
      default: false,
      required: false,
    },
    isAuthorized: {
      type: Boolean,
      default: false,
      required: false,
    },

    originCall: {
      type: String,
      required: false,
      default: "GRANTLIST",
    },

    donorPresetted: {
      type: Boolean,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      module: "",
      tab: null,
      tabsInfo: [
        {
          code: "TABLE",
          descr: "Table",
        },
        {
          code: "CALENDAR",
          descr: "Calendar",
        },
      ],
      role: "",
      authorizedRole: false,
      donor_name: "",
      currency: "",
      showMainData: true,
      tblName: "GMT_GRANT",
      leavePage: false,
      mainDataItems: null,
      serviceRes: null,
      tabIsVisible: null,
      tabsItems: [
        { code: "MAINDATA", descr: "Main Data" },
        { code: "CONTACTS", descr: "Contacts" },
        { code: "NARRATIVE", descr: "Narrative" },
        { code: "EVENTS", descr: "Deliverables" },
        { code: "DOCUMENTS", descr: "Documents" },
        { code: "PROJECTS", descr: "Projects" },
        { code: "FUNDING_STATUS", descr: "Funding Status" },
        // { code: "AGREEMENT", descr: "Agreement" },
      ],

      genericQueryData: false,
      tmpSelectedRows: [],
      locationId: 22,
      profileId: null,
      sIconBack: "mdi-chevron-double-left",
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
    }),
    showPrjDetails(item: any) {
      let localThis = this as any;
      localThis.showMainData = item;
    },
    save() {},

    getList() {
      let localThis = this as any;
      localThis.serviceRes = null;
      axios({
        url:
          "https://jesuitrefugeeservice720.sharepoint.com/teams/TEST_NM_01/Shared%20Documents/SP_TEST_01.txt",
        method: "GET",
        responseType: "json",
      })
        .then((response) => {
          localThis.serviceRes = response;
          // var fileURL = window.URL.createObjectURL(new Blob([response.data]));
          // var fileLink = document.createElement("a");
          // fileLink.href = fileURL;
          // fileLink.setAttribute("download", "file.pdf");
          // document.body.appendChild(fileLink);
          // fileLink.click();
        })
        .catch((err) => {
          localThis.serviceRes = err;
        });
    },
    testPrint(str: string) {
      //(str || "TEST");
    },

    returnToList() {
      let localThis: any = this as any;

      this.$emit("returnBackToList", localThis.editItemMainData);
    },

    testGetGenericSelect() {
      (this as any).genericQueryData = !(this as any).genericQueryData;
    },
    closeDialogMainData() {
      let localThis: any = this as any;
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      if (localThis.originCall == "DONOR") {
        this.$emit("returnBackToDonorList");
      } else {
        this.$router.push({
          name: "Grant List",
        });
      }
      // this.$confirm(msgLeave).then((res) => {
      //   if (res) {
      //     (this as any).leavePage = true;
      //     this.$router.push({
      //       name: "Grant List",
      //     });
      //   } else {
      //     (this as any).leavePage = false;
      //   }
      // });
    },
    closeDialogAndSaveMainData(item: any) {
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
      //         localThis.setGrant(gt);
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
    let localThis: any = this;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
    localThis.role = localThis.getCurrentRole.ROLE_CODE;
    // if (localThis.role == "GM") {
    //   localThis.authorizedRole = true;
    //   //localThis.getStatusList();
    // } else {
    //   localThis.authorizedRole = false;
    //
    localThis.authorizedRole = localThis.isAuthorized;
  },
  computed: {
    ...mapGetters({
      getCurrentRole: "getCurrentRole",
      getActiveModule: "getActiveModule",
    }),
  },
  mounted() {
    let localThis = this as any;
    let gt = {} as any;

    gt = localThis.$store.getters.getGrant;
    localThis.donor_id = gt.donor_id;
    localThis.donor_name = gt.donor_name;
    localThis.currency = gt.currency;
    if (localThis.editItemMainData) {
      localThis.mainDataItems = localThis.editItemMainData;
      localThis.mainDataItems.status_in_bar = localThis.editItemMainData.status_name;
    }
    localThis.role = localThis.getCurrentRole.ROLE_CODE;

    localThis.authorizedRole = Object.assign(
      false,
      localThis.authorizedRole,
      localThis.isAuthorized
    );
    // localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
  },
});
</script>

<style scoped></style>
