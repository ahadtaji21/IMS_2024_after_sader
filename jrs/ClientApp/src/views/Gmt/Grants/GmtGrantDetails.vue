<template>
  <v-content>
    <v-row style="margin-bottom:-3%">
      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4" style="margin-left:1%;margin-top:1%">
        <div>
          <h4 style="text-align: left;font-size:larger;color:#00326b">
            GRANT: {{ editItemMainData ? editItemMainData.CODE : "No code" }}
          </h4>
          <h5 style="text-align: left">
            TITLE: {{ editItemMainData ? subStr(editItemMainData.DESCR) : "No Title" }}
          </h5>
          <h5 style="text-align: left">DONOR: {{ donor_name }}</h5>
          <h5 style="text-align: left">
            STATUS: {{ editItemMainData ? editItemMainData.IMS_STATUS_NAME : null }}
          </h5>
        </div>
        <!-- <div v-else>
      <h4>GRANT</h4>
      </div> -->
      </v-col>
      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4"></v-col>
      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 2" style="margin-top:3%">
        <div v-if="authorizedRole">
          <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
        <gmt-grant-details
          :mainData="false"
          :editItemMainData="editItemMainData" 
          :donorPresetted="true"
          :isAuthorized="authorizedRole"
          :key="rndKey"
        ></gmt-grant-details>
      </v-col>
    </v-row>
  </v-content>
</template>

<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import GrantDetails from "../../../components/GMT/GRANTS/PROPOSAL/GMTGrantDetailsForm.vue";
import { EventBus } from "../../../event-bus";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
//TMP
import { JrsHeader } from "../../../models/JrsTable";
import {
  JrsFormField,
  JrsFormFieldText,
  JrsFormFieldSelect,
} from "../../../models/JrsForm";
//TMP

export default Vue.extend({
  name: "home",
  components: {
    "gmt-grant-details": GrantDetails,
  },
  props: {
    editItemMainData: {
      type: Object,
    },
    editItemOBJ: {
      type: Object,
    },
  },
  data() {
    return {
      rndKey: 0,
      donor_name: "",
      authorizedRole: false,
      role: "",
    };
  },
  methods: {
    subStr: function (string: any) {
      if (string != undefined) {
        if (string.length < 50) return string;
        else return string.substring(0, 50) + "...";
      } else return "";
    },
    save() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n.t("Confirm all the Grant Updates?").toString();
      this.$confirm(msgUpd).then((res) => {
        if (res) {
          EventBus.$emit("saveGTMainDataFromMainSave");
          EventBus.$emit("saveGTNarrativeFromMainSave");

          // EventBus.$emit("saveObjectiveFromMainSave");
          // EventBus.$emit("saveNarrativeFromMainSave");
          // EventBus.$emit("saveServiceFromMainSave");
          // EventBus.$emit("saveOutputFromMainSave");
        }
      });
    },
  },
  beforeDestroy() {
    EventBus.$off("userRoleUpdated");
  },
  beforeMount() {
    let localThis = this as any;
    if (localThis.getCurrentRole.ROLE_CODE == "GM") {
      localThis.authorizedRole = true;
    } else {
      localThis.authorizedRole = false;
    }
  },

  mounted() {
    let localThis = this as any;
    let gt = {} as any;
    gt = localThis.$store.getters.getGrant;
    localThis.donor_id = gt.donor_id;
    localThis.donor_name = gt.donor_name;
    EventBus.$on("userRoleUpdated", (to: any) => {
      localThis.authorizedRole = false;
      localThis.role = localThis.getCurrentRole.ROLE_CODE;
      if (localThis.getCurrentRole.ROLE_CODE == "GM") {
        localThis.authorizedRole = true;
      } else {
        localThis.authorizedRole = false;
      }
      localThis.rndKey = Math.ceil(Math.random() * 1000);
      //localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
    });
  },
  computed: {
    ...mapGetters({
      getCurrentRole: "getCurrentRole",
      getActiveModule: "getActiveModule",
    }),
  },

  beforeRouteLeave(to, from, next) {
    let localThis = this as any;

    let msgLeave: string = this.$i18n
      .t("datasource.pms.annual-plan.ap-leave-confirm")
      .toString();
    this.$confirm(msgLeave).then((res) => {
      if (res) {
        next();
      } else {
        next(false);
      }
    });
  },
});
</script>

<style scoped></style>
