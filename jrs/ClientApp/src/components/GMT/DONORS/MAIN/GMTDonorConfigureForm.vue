<template>
  <div>
    <v-row align-center v-if="showWait">
      <v-col justify-center :cols="12">
        <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 8">
        <span
          >Clone the following structures  :<br />°
          JRS Chart of Account <br />° JRS Category of Intervention <br />° JRS Type of
          Services
        </span>
      </v-col>
      <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 4">
        <v-btn text color="primary darken-1" @click="ExecClone()"
          ><v-icon>mdi-content-copy</v-icon>Clone</v-btn
        >
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import {
  ImsApi,
  GenericSqlApi,
  ImsLookupsApi,
  Configuration,
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi";

export default Vue.extend({
  props: {
    donor_id: {
      type: Number,
      required: true,
    },
  },

  data() {
    return {
      showWait: false,
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
    ExecClone() {
      let localThis = this as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: GenericSqlApi = new GenericSqlApi(config);

      localThis.showWait = true;
      let payLoad = {} as any;

      payLoad.DONOR_ID = localThis.donor_id;

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_CONFIG_DONOR",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoad),
      };
      return api
        .genericSqlSPCall(savePayload)
        .then((res: any) => {
          if (res.data != undefined && res.data.status == "error") {
            localThis.showWait = false;
            localThis.showNewSnackbar({
              typeName: "error",
              text: res.data.message,
            }); // Feedback to user
          } else {
            localThis.showNewSnackbar({
              typeName: "success",
              text: "Configuration Cloned",
            }); // Feedback to user
            localThis.showWait = false;
          }
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.data.message,
          }); // Feedback to user
        });
    },
  },
});
</script>

<style scoped></style>
