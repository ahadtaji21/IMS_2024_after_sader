<template>
  <v-card>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid fill-height>
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
                  <v-tab v-for="item in tabsItems" :key="item.code">{{
                    item.descr
                  }}</v-tab>
                </v-tabs>
                <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
                  <v-tab-item key="DETAILS">
                    <v-card>
                      <v-card-title primary-title>Details</v-card-title>
                      <gt-prj-dets
                        @closeDialogeGtPrjDetails="closeDialogMainData"
                        :formData="editItemMainData"
                        :onlyRead="onlyRead || !isAuthorized"
                      ></gt-prj-dets>
                    </v-card>
                  </v-tab-item>
                </v-tabs-items>
              </v-card>
            </v-col>
          </v-row>
        </div>
      </v-container>
    </v-card>
  </v-card>
</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import GMTGrantProjectForm from "./GMTGrantOfficeProjectForm.vue";

//import JrsTable from "../../../TJrsTable.vue";
//import JrsSimpleTable from "../../../TJrsSimpleTable.vue";
//import JrsModalForm from "../../../TJrsModalForm.vue";
//import JrsLocationPicker from "../../../TJrsLocationPicker.vue";
//import JrsSearchTable from "../../../TJrsSearchTable.vue";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../../../axiosapi";

//TMP
import { JrsHeader } from "../../../../../../models/JrsTable";
import {
  JrsFormField,
  JrsFormFieldText,
  JrsFormFieldSelect,
} from "../../../../../../models/JrsForm";
//TMP

export default Vue.extend({
  name: "home",
  components: {
    "gt-prj-dets": GMTGrantProjectForm,
  },
  props: {
    editItemMainData: {
      type: Object,
    },

    isAuthorized: {
      type: Boolean,
      default: false,
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
      showWait: false,
      module: "",
      onlyReadStatus: true,
      role: "",
      authorizedRole: false,
      donor_name: "",
      showMainData: true,
      tblName: "GMT_GRANT",
      leavePage: false,
      mainDataItems: null,
      serviceRes: null,
      tabIsVisible: null,
      tabsItems: [{ code: "Details", descr: "Details" }],

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
    closeDialogAndSaveMainData() {},
    save() {},

    returnToList() {
      let localThis: any = this as any;

      this.$emit("returnBackToList", localThis.editItemMainData);
    },

    testGetGenericSelect() {
      (this as any).genericQueryData = !(this as any).genericQueryData;
    },
    closeDialogMainData() {
      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-confirm")
        .toString();
      this.$emit("closeDialogeGtPrjDetails");
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
    localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
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
    localThis.mainDataItems = localThis.editItemMainData;
    localThis.mainDataItems.status_in_bar = localThis.editItemMainData.status_name;
    localThis.role = localThis.getCurrentRole.ROLE_CODE;

    localThis.authorizedRole = Object.assign(
      false,
      localThis.authorizedRole,
      localThis.isAuthorized
    );
    // localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
    localThis.onlyReadStatus = Object.assign(
      false,
      localThis.onlyReadStatus,
      localThis.onlyRead || !localThis.authorizedRole
    );
  },
});
</script>

<style scoped></style>
