<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="12">
              <v-text-field
                :label="$t('pms.code')"
                v-model="formData.PMS_TOS_CODE"
                :counter="20"
                :rules="rules.required"
              ></v-text-field>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="12">
              <v-text-field
                :label="$t('pms.description')"
                v-model="formData.PMS_TOS_DESCRIPTION"
                :counter="50"
                :rules="rules.required"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-checkbox
                :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
                :label="$t('datasource.pms.type-of-service.enabled')"
                v-model="formData.PMS_TOS_ENABLED"
              ></v-checkbox>
            </v-col>
          </v-row>
          <v-row v-if="!newTOS">
            <v-col :cols="12">
              <pms-tos-coi
                :relations="formData.coi_relations_placeholder"
                :key="Math.ceil(Math.random() * 1000)"
                :selectedObjectId="formData.PMS_TOS_ID"
              ></pms-tos-coi>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions>
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import TOSCOIEditor from "./PMSTOSCOIForm.vue";
import FormMixin from "../../../../mixins/FormMixin";
import NavMix from "../../../../mixins/NavMixin";
import UtilMix from "../../../../mixins/UtilMix";
import {
  ImsApi,
  PmsCategoryOfInterventionApi,
  PmsTypeOfServiceApi,
  Configuration,
} from "../../../../axiosapi";
import { i18n } from "../../../../i18n";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "pms-tos-coi": TOSCOIEditor,
  },

  props: {
    formData: {
      type: Object,
    },
    presetVal: {
      type: String,
      required: false,
      default: "",
    },
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
      newTOS: false,
      formDataOriginal: {},
      tableName: "PMS_VI_TYPE_OF_SERVICE",
      relations: [],
      idObject: 0,
      formValid: false,
      datePickerMenu: false,
      someString: "some",
      datePlaceholder: null,
      localData: null,
      typeOfServices: [],
      isLoading: false,
      //model:  localThis.formData.pmsCoiTosRel.map((x: any) => {
      //return x.pmsTosId;
      //}),
      tb: null,
      search: this.presetVal,
      descriptions: [],
      descriptionDetails: {
        Name: String,
        Vlue: Object,
      },
      languages: [],
      languagesdef: [],
    };
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
  watch: {
    presetVal: function (val) {
      this.search = val;
    },
    language(oldLanguage: any, newLanguage: any) {
      let localThis: any = this as any;
    },
  },
  beforeMount() {
    let localThis = this as any;
    localThis.formValid = false;
    localThis.idObject = localThis.formData.PMS_TOS_ID;
    if (localThis.idObject == undefined) localThis.newTOS = true;
  },
  mounted() {
    let localThis = this as any;
    localThis.getTOS();
    (localThis.$refs.form as Vue & { resetValidation: () => void }).resetValidation();
    // localThis.getLanguage();
    // localThis.descriptionDetails.Name = "pms.tos-description";
    //         localThis.descriptionDetails.Vlue = localThis.languages ;

    // const data = {
    //   en: "testen",
    //   fr: "testfr"
    // };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    descriptionChanged(desc: any) {
      let localThis = this as any;
      localThis.descriptions = desc;
    },

    close: function () {
      let localThis = this as any;
      //(localThis.$refs.form as Vue & { reset: () => void }).reset();
      this.$emit("closeDialoge");
      localThis.descriptions = null;
    },

    save: function () {
      let localThis = this as any;
      let isValid = true;
      isValid = (localThis.$refs.form as Vue & {
        validate: () => boolean;
      }).validate();
      if (!isValid) return;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n.t("pms.add-tos").toString();
      let value: any = {};
      let id = 0;
      let msg = msgUpd;

      if (localThis.formData.PMS_TOS_ID == undefined) {
        msg = msgAdd;
        localThis.formData.PMS_TOS_ID = 0;
      }
      let obj: any;
      obj = localThis.formData;
      if (obj.PMS_TOS_ID == undefined || obj.PMS_TOS_ID == 0) {
        obj.GUID = localThis.makeUUID();
      } else {
        if (obj.GUID == undefined || obj.GUID == "undefined" || obj.GUID == "")
          obj.GUID = localThis.makeUUID();
      }
      this.$confirm(msg).then(async (res) => {
        if (res) {
          if (obj.PMS_TOS_ID != undefined && obj.PMS_TOS_ID > 0) {
            let retvalue = await localThis.BlockTos();
            localThis.UpdateTos(obj);
          } else {
            localThis.UpdateTos(obj);
          }
        }
      });
    },

    async BlockTos() {
      let localThis = this as any;
      let value: any = {};
      let obj: any = localThis.formDataOriginal;
      obj.PMS_TOS_ENABLED = false;
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_TYPE_OF_SERVICE",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(obj),
      };
      await localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          }); // Feedback to use
        })
        .then(async (res: any) => {
          value.dimensionCode = "TYPE OF SERVICE";
          value.tos_code = obj.PMS_TOS_CODE;
          value.tos_descr = obj.PMS_TOS_DESCRIPTION;
          if (
            obj.coi_relations_placeholder != undefined &&
            obj.coi_relations_placeholder.length > 0
          ) {
            let i: number = 0;
            value.coi_code = "";
            for (i = 0; i < obj.coi_relations_placeholder.length; i++) {
              value.coi_code += obj.coi_relations_placeholder[i].PMS_COI_CODE + "|";
            }
            var lastChar = value.coi_code.slice(-1);
            if (lastChar == "|") {
              value.coi_code = value.coi_code.slice(0, -1);
            }
          } else value.coi_code = "";
          value.guid = obj.GUID;
          value.blocked = "TRUE";
          // value.location_description=res.LOCATION_DESCRIPTION;
          // value.office_code = saveData.PMS_OFFICE_CODE;

          await localThis.updateNavDimension(value);
          return "";
        })
        .catch((err: any) => {
          localThis.editMode = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    UpdateTos(obj: any) {
      let localThis = this as any;
      let value: any = {};
      obj.PMS_TOS_ID = 0;
      obj.GUID = localThis.makeUUID();
      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_TYPE_OF_SERVICE",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(obj),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          }); // Feedback to use
        })
        .then((res: any) => {
          value.dimensionCode = "TYPE OF SERVICE";
          value.tos_code = obj.PMS_TOS_CODE;
          value.tos_descr = obj.PMS_TOS_DESCRIPTION;
          if (
            obj.coi_relations_placeholder != undefined &&
            obj.coi_relations_placeholder.length > 0
          ) {
            let i: number = 0;
            value.coi_code = "";
            for (i = 0; i < obj.coi_relations_placeholder.length; i++) {
              value.coi_code += obj.coi_relations_placeholder[i].PMS_COI_CODE + "|";
            }
            var lastChar = value.coi_code.slice(-1);
            if (lastChar == "|") {
              value.coi_code = value.coi_code.slice(0, -1);
            }
          } else value.coi_code = "";
          value.guid = obj.GUID;
          value.blocked = "FALSE";
          // value.location_description=res.LOCATION_DESCRIPTION;
          // value.office_code = saveData.PMS_OFFICE_CODE;
          if (localThis.newTOS == false)
            //aggiorna nav solo se c'è una categoria di intervento associata al tos
            localThis.updateNavDimension(value);
        })
        .catch((err: any) => {
          localThis.editMode = false;
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        })
        .finally(() => {
          localThis.descriptions = null;
          (localThis.$refs.form as Vue & { reset: () => void }).reset();
          this.$emit("closeDialogeAndSave");
        });
    },

    getTOS() {
      let localThis = this as any;
      let i: number = 0;
      if (localThis.formData.coi_relations_placeholder == undefined) return;
      for (i = 0; i < localThis.formData.coi_relations_placeholder.length; i++) {
        if (
          localThis.formData.coi_relations_placeholder[i].IMS_LANGUAGE_LOCALE ==
          localThis.language
        ) {
          localThis.relations.push(localThis.formData.coi_relations_placeholder[i]);
        }
      }
      localThis.formDataOriginal = JSON.parse(JSON.stringify(localThis.formData));
      if (
        localThis.typeOfServices.length != undefined &&
        localThis.typeOfServices.length > 0
      )
        return;

      //   this.isLoading = true;
      //   const config: Configuration =
      //     localThis.$store.getters["auth/getApiConfig"];
      //   const api: PmsTypeOfServiceApi = new PmsTypeOfServiceApi(config);
      //   return api
      //     .apiPmsTypeOfServiceGet(config.baseOptions)
      //     .then(res => {
      //       localThis.typeOfServices = res.data;
      //       let i: number = 0;
      //       for (i = 0; i < localThis.typeOfServices.length; i++) {
      //         localThis.typeOfServices[i].PMS_TOS_ID =
      //           localThis.typeOfServices[i].pmsTosId;
      //       }

      //       //alert(res.data[0].pmsTosDescription);
      //       return res;
      //     })
      //     .catch(err => {
      //       // console.error(err);
      //       localThis.typeOfServices = [];
      //     })
      //     .finally(() => {
      //       localThis.isLoading = false;
      //     });
    },
  },
});
</script>

<style scoped></style>
