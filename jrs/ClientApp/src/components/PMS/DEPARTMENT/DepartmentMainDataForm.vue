<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.objectives.code')"
                v-model="formData.CODE"
                :counter="20"
                 :rules="rules.required"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.objectives.description')"
                v-model="formData.DESC"
                :counter="50"
                 :rules="rules.required"
              ></v-text-field>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn v-if="isUpdatableForm" color="primary" class="--darken-1" @click="save()"
        >Save</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import {
  ImsApi,
  PmsAnnualPlanApi,
  ImsLookupsApi,
  Configuration,
} from "../../../axiosapi";
import FormMixin from "../../../mixins/FormMixin";
import NavMix from "../../../mixins/NavMixin";
import UtilMix from "../../../mixins/UtilMix";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { i18n } from "../../../i18n";
import { mapActions, mapGetters } from "vuex";
export default Vue.extend({
  components: {},

  props: {
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
    formDataExt: {
      type: Object,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
      formData: {},
      formDataOriginal: {},
      annual_plan_id: 0,

      keyTbl: 0,
      formIsValid: false,
      targets: [],
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
      this.$emit("closeDialogDPT");
    },
    // BlockDepartment: async function () {
    //   let localThis = this as any;
    //   if (localThis.formDataOriginal.GUID == undefined) return;
    //   let value: any = {};
    //   value.dimensionCode = "DEPARTMENT";
    //   value.department_code = localThis.formDataOriginal.CODE;
    //   value.department_descr = localThis.formDataOriginal.DESC;
    //   value.guid = localThis.formDataOriginal.GUID;
    //   value.blocked = "TRUE";

    //   await localThis.updateNavDimension(value);
    //   return;
    // },
    async BlockDepartment() {
      let localThis = this as any;

      let value: any = {};

      let id = 0;

      let payLoadD: any = {};

      if (
        localThis.formDataOriginal.ID == undefined ||
        localThis.formDataOriginal.ID == 0
      ) {
        localThis.formDataOriginal.GUID = localThis.makeUUID();
      } else {
        if (
          localThis.formDataOriginal.GUID == undefined ||
          localThis.formDataOriginal.GUID == "undefined" ||
          localThis.formDataOriginal.GUID == ""
        )
          localThis.formDataOriginal.GUID = localThis.makeUUID();
      }

      payLoadD.ID = Number.parseInt(localThis.formDataOriginal.ID);
      payLoadD.CODE = localThis.formDataOriginal.CODE;
      payLoadD.GUID = localThis.formDataOriginal.GUID;
      payLoadD.DESC = localThis.formDataOriginal.DESC;
      payLoadD.BLOCKED = "TRUE";

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_DEPARTMENT",
        actionType: SqlActionType.NUMBER_3,
        jsonPayload: JSON.stringify(payLoadD),
      };
      await localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          });
        })
        .then(async (res: any) => {
          value.dimensionCode = "DEPARTMENT";
          value.department_code = payLoadD.CODE;
          value.department_descr = localThis.formDataOriginal.DESC;
          value.guid = localThis.formDataOriginal.GUID;
          value.blocked = "TRUE";
          // value.location_description=res.LOCATION_DESCRIPTION;
          // value.office_code = saveData.PMS_OFFICE_CODE;
          await localThis.updateNavDimension(value);
          return "";
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
          return "";
        });
    },

    save: function () {
      let localThis = this as any;
        let isValid = true;
      isValid = (localThis.$refs.form as Vue & {
        validate: () => boolean;
      }).validate();
      if (!isValid) return;
      let value: any = {};
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let id = Number.parseInt(localThis.formData.ID);
      let msg = msgUpd;
      let payLoadD: any = {};
      this.$confirm(msg).then(async (res) => {
        if (res) {
          if (id > 0) {
            let retvalue = await localThis.BlockDepartment();
            localThis.UpdateDepartment();
          } else {
            localThis.UpdateDepartment();
          }
        }
      });
    },

    UpdateDepartment: function () {
      let localThis = this as any;
      let value: any = {};

      let payLoadD: any = {};

      // if (localThis.formData.ID == undefined || localThis.formData.ID == 0) {
      //   localThis.formData.GUID = localThis.makeUUID();
      // } else {
      //   if (
      //     localThis.formData.GUID == undefined ||
      //     localThis.formData.GUID == "undefined" ||
      //     localThis.formData.GUID == ""
      //   )
      //     localThis.formData.GUID = localThis.makeUUID();
      // }
      localThis.formData.GUID = localThis.makeUUID();
      payLoadD.ID = Number.parseInt(localThis.formData.ID);
      payLoadD.CODE = localThis.formData.CODE;
      payLoadD.GUID = localThis.formData.GUID;
      payLoadD.DESC = localThis.formData.DESC;
      payLoadD.BLOCKED = "FALSE";

      payLoadD.ID = 0;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_DEPARTMENT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          });
        })
        .then((res: any) => {
          value.dimensionCode = "DEPARTMENT";
          value.department_code = payLoadD.CODE;
          value.department_descr = localThis.formData.DESC;
          value.guid = localThis.formData.GUID;
          value.blocked = "FALSE";
          // value.location_description=res.LOCATION_DESCRIPTION;
          // value.office_code = saveData.PMS_OFFICE_CODE;
          localThis.updateNavDimension(value);
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        })
        .finally(() => this.$emit("saveDialoge"));
    },
  },

  beforeMount() {
    let localThis: any = this;
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.formDataOriginal = JSON.parse(JSON.stringify(localThis.formDataExt));
  },
});
</script>

<style scoped></style>
