<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('general.name')"
                v-model="formData.DESCRIPTION"
                :counter="150"
                required
                :readonly="onlyRead"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-select
                :items="activity_type"
                :label="$t('datasource.pms.annual-plan.objectives.act-type')"
                item-text="text"
                item-value="value"
                v-model="formData.ACTIVITY_TYPE_ID"
                :readonly="onlyRead"
              ></v-select>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" v-if="!onlyRead" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" v-if="!onlyRead" class="--darken-1" @click="save()">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { i18n } from "../../../i18n";

import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi";
export default Vue.extend({
  components: {},

  props: {
    formDataExt: {
      type: Object,
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
    onlyRead:{
      type: Boolean,
      required: false,
      default:false
    }
  },
  data() {
    return {
      formData: {},
      activity_type: [],
      formIsValid: false,
      isLoading:false
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
  },
  watch: {},

  mounted() {
    let localThis = this as any;
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.formData.SERVICE_ID = localThis.selectedObjectId;
    if (
      localThis.activity_type.length == undefined ||
      localThis.activity_type.length == 0
    )
      localThis.getList("ACTIVITY_TYPE");
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    close: function () {
      this.$emit("closeDialoge");
    },

    save() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-upd-activity")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add-activity")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (localThis.formData.ID == undefined) {
        msg = msgAdd;
      }

      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE_ACTIVITY",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(localThis.formData),
          };

          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              this.$emit("refreshAct", localThis.formData);
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.$emit("closeDialogeAndSave", localThis.formData);
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      localThis.isLoading=true;
      const config: Configuration =
        localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          localThis.activity_type = res.data;
          return res;
        })
        .catch((err) => {
          // console.error(err);
          return [];
        })
        .finally(() => (localThis.isLoading = false));
    },
  },
});
</script>

<style scoped>
</style>