<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form
          v-model="formValid"
          ref="myForm"
          lazy-validation
          class="text-capitalize"
        >
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('pms.donor-coa-code')"
                v-model="formData.ACC_CODE"
                :counter="150"
                :rules="rules.required"
                :key="1"
              ></v-text-field>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('pms.donor-coa-type')"
                v-model="formData.ACC_TYPE"
                :counter="150"
                :rules="rules.required"
                :key="2"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('pms.donor-coa-descr')"
                v-model="formData.ACC_DESCRIPTION"
                :counter="150"
                :rules="rules.required"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-select
                :items="status"
                :label="$t('pms.donor-coa-status')"
                item-text="text"
                item-value="value"
                v-model="formData.ACC_STATUS"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col>
          </v-row>
          <v-row v-if="!newCOA">
            <v-col :cols="12">
              <gmt-jcoa-dcoa
                @updateRelations="updateRelations"
                :relations="coa_relations_placeholder"
                :key="rndKey"
                :selectedObjectId="selectedObjectId"
                :selectedCOAId="formData.ACC_ID"
              ></gmt-jcoa-dcoa>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()"
        >Cancel</v-btn
      >
      <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { ImsApi, ImsLookupsApi, Configuration } from "../../../../axiosapi";
import { i18n } from "../../../../i18n";
import JCOADCOA from "./GmtDCoaJCoaForm.vue";
import {
  IImsList,
  IPayLoadDataDonorCOA,
} from "./../../../PMS/PMSSharedInterfaces";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "gmt-jcoa-dcoa": JCOADCOA,
  },

  props: {
    formData: {
      type: Object,
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
    presetVal: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      coa_relations_placeholder: [],
      rndKey: 0,
      newCOA: false,
      // jrscode: [],
      search: this.presetVal,
      formValid: false,
      isLoading: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
      status: [],
      donors: [],
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
  },
  beforeMount() {
    let localThis: any = this;
    localThis.formValid = false;
    if (localThis.status.length == undefined || localThis.status.length == 0)
      localThis.getList("COA_STATUS_LIST");
    if (localThis.selectedObjectId == undefined) localThis.newCOA = true;
  },

  mounted() {
    let localThis: any = this;
    localThis.rndKey = Math.ceil(Math.random() * 1000);
    localThis.coa_relations_placeholder =
      localThis.formData.coa_relations_placeholder;
    if (localThis.coa_relations_placeholder == null)
      localThis.coa_relations_placeholder = [];
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
    updateRelations(item: any) {
      let localThis = this as any;
      localThis.coa_relations_placeholder = item;
    },
    save: function () {
      let localThis = this as any;
      let isValid = true;
      isValid = (localThis.$refs.myForm as Vue & {
        validate: () => boolean;
      }).validate();
      if (!isValid) return;

      let msgUpd: string = this.$i18n
        .t("datasource.gmt.grant.gt-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.gmt.grant.gt-add-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;
      if (localThis.formData.ACC_ID == undefined) {
        msg = msgAdd;
      } else {
        id = Number(localThis.formData.ACC_ID);
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          let i: number;
          let status_name = "";
          for (i = 0; i < localThis.status.length; i++) {
            if (localThis.status[i].value == localThis.formData.status_id) {
              status_name = localThis.status[i].text;
              break;
            }
          }
          localThis.formData.status_name = status_name;

          let donor_name = "";
          for (i = 0; i < localThis.donors.length; i++) {
            if (localThis.donors[i].value == localThis.selectedObjectId) {
              donor_name = localThis.donors[i].text;
              break;
            }
          }
          localThis.formData.donor_name = donor_name;
          let ap = {} as any;
          let payLoadD = {} as IPayLoadDataDonorCOA;

          payLoadD.ACC_ID = localThis.formData.ACC_ID;
          payLoadD.DONOR_ID = localThis.selectedObjectId;
          payLoadD.ACC_CODE = localThis.formData.ACC_CODE;
          payLoadD.ACC_DESCRIPTION = localThis.formData.ACC_DESCRIPTION;
          payLoadD.ACC_TYPE = localThis.formData.ACC_TYPE;
          // payLoadD.ACC_JRS_ID = localThis.formData.ACC_JRS_ID;
          // payLoadD.START_DATE = localThis.formData.START_DATE;
          // payLoadD.END_DATE = localThis.formData.END_DATE;
          payLoadD.ACC_STATUS = localThis.formData.ACC_STATUS;
          payLoadD.coa_relations_placeholder =
            localThis.coa_relations_placeholder;
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_DONOR_COA",
            actionType:
              payLoadD.ACC_ID == 0 || payLoadD.ACC_ID == undefined
                ? SqlActionType.NUMBER_0
                : SqlActionType.NUMBER_1,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              this.$emit("closeDialoge");
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
      this.isLoading = true;
      const config: Configuration =
        localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          switch (listName) {
            case "COA_STATUS_LIST":
              localThis.status = res.data;
              break;
          }
          //obj = res.data;
          //alert(res.data[0].pmsTosDescription);
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
.Budget {
  background-color: white;
  font-size: 16px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  width: 60%;
  height: 40px;
}
</style>