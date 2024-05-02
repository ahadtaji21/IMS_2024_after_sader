<template>
  <v-card>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-type-of-disbursement')"
                v-model="formData.GMT_FS_TYPE_OF_DISBURSEMENT"
                :readonly="onlyReadStatus"
              ></v-text-field>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <label
                id="lbl"
                :class="
                  toValidate == true &&
                  (formData.GMT_FS_FUND_AMOUNT == undefined ||
                    formData.GMT_FS_FUND_AMOUNT == '')
                    ? 'v-label theme--light redStyle'
                    : 'v-label theme--light'
                "
                >{{ $t("datasource.gmt.grant.gt-fund-disbursement") }}:</label
              >
              <currency-input
                :readonly="onlyReadStatus"
                v-model="formData.GMT_FS_FUND_AMOUNT"
                v-bind="options"
                :currency="formData.GMT_FS_CURRENCY"
                :class="
                  toValidate == true &&
                  (formData.GMT_FS_FUND_AMOUNT == undefined ||
                    formData.GMT_FS_FUND_AMOUNT == '')
                    ? 'redBudget'
                    : 'Budget'
                "
                aria-labelledby="lbl"
                :rules="rules.required"
              ></currency-input>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-date-picker
                :label="$t('datasource.gmt.grant.gt-date-expected')"
                v-model="formData.GMT_FS_DATE_EXPECTED"
                :hint="$t('datasource.gmt.grant.gt-date-expected')"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></jrs-date-picker>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <jrs-date-picker
                :label="$t('datasource.gmt.grant.gt-payment-date')"
                v-model="formData.GMT_FS_PAYMENT_DATE"
                :hint="$t('datasource.gmt.grant.gt-payment-date')"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></jrs-date-picker>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-donor-requirements')"
                v-model="formData.GMT_FS_DONOR_REQUIREMENTS"
                :counter="150"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></v-text-field>
            </v-col>

            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('datasource.gmt.grant.gt-comments')"
                v-model="formData.GMT_FS_COMMENT"
                :counter="150"
                :rules="rules.required"
                :readonly="onlyReadStatus"
              ></v-text-field>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" v-if="!onlyReadStatus" class="--darken-1" @click="save()"
        >Save</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { ImsApi, ImsLookupsApi, Configuration } from "../../../../../axiosapi";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";
import UtilMix from "../../../../../mixins/UtilMix";
import JrsDatePicker from "../../../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "jrs-date-picker": JrsDatePicker,
  },

  props: {
    formData: {
      type: Object,
    },

    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    currency: {
      type: String,
      required: true,
    },
  },
  mixins: [UtilMix],
  data() {
    return {
      formValid: true,
      toValidate: false,
      showWait: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
    ...mapGetters({
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  watch: {},

  async beforeMount() {
    let localThis: any = this;
    localThis.role = localThis.getCurrentRole.ROLE_CODE;
    if (localThis.role == "GM") {
      localThis.authorizedRole = !localThis.onlyRead;
      //localThis.getStatusList();
    } else {
      localThis.authorizedRole = false;
    }
    localThis.onlyReadStatus = localThis.onlyRead || !localThis.authorizedRole;
    //localThis.formValid = false;
  },

  async mounted() {
    let localThis: any = this;

    localThis.role = localThis.getCurrentRole.ROLE_CODE;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    close: function () {
      let localThis = this as any;
      // (localThis.$refs.myForm as Vue & { reset: () => void }).reset();
      this.$emit("closeDialogeGtFSDetails");
    },

    save: function () {
      let localThis = this as any;
      localThis.toValidate = true;
      let isValid = true;
      // isValid = (localThis.$refs.myForm as Vue & {
      //   validate: () => boolean;
      // }).validate();
      // isValid =
      //   isValid &&
      //   localThis.formData.GPR_AMOUNT != undefined &&
      //   localThis.formData.GPR_AMOUNT + "" != "";
      // if (!isValid) return;

      let msgUpd: string = this.$i18n
        .t("datasource.gmt.grant.gt-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n.t("datasource.gmt.grant.gt-add-item").toString();

      let id = 0;
      let msg = msgUpd;
      if (localThis.formData.GMT_FS_ID == undefined) {
        msg = msgAdd;
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          let ap = {} as any;
          let payLoadD = {} as any;
          payLoadD = JSON.parse(JSON.stringify(localThis.formData));

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_FUNDING_STATUS",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user

              this.$emit("closeDialogeGtFSDetails");
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

    async closeDialog(item: any) {
      let localThis = this as any;
      if (item != undefined && item != "") {
        localThis.hr_position_id = item;

        localThis.showWait = true;
        localThis.userName = await localThis.getUserName(item).then((res: any) => {
          return res;
        });
        localThis.showWait = false;
      }
      localThis.searchUser = false;
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
.redBudget {
  background-color: white;
  font-size: 16px;
  border-style: solid;
  border-width: 1px;
  border-color: red;
  width: 60%;
  height: 40px;
}
.redStyle {
  color: red;
}
</style>
