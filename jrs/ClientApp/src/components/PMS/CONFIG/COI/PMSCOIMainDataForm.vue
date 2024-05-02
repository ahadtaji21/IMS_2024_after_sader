<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="12">
              <v-text-field
                :label="$t('pms.coi-code')"
                v-model="formData.PMS_COI_CODE"
                :counter="20"
                :rules="rules.required"
              ></v-text-field>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="12">
              <!-- <v-text-field :label="$t('pms.coi-description')"
                                          v-model="formData.pmsCoiDescription"
              :counter="150"></v-text-field>-->
              <ml-editor
                :tableName="tableName"
                :idObject="idObject"
                @descriptionChanged="descriptionChanged"
              ></ml-editor>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-checkbox
                :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
                :label="$t('pms.coi-enabled')"
                v-model="formData.PMS_COI_ENABLED"
                :readonly="true"
              ></v-checkbox>
            </v-col>
          </v-row>

          <v-row v-if="!newCOI">
            <v-col :cols="12">
              <pms-coi-tos
                :relations="formData.coi_relations_placeholder"
                :key="Math.ceil(Math.random() * 1000)"
                :selectedObjectId="formData.PMS_COI_ID"
              ></pms-coi-tos>
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
import MultiLanguageEditor from "../../../../components/MultiLanguageEditor.vue";
import COITOSEditor from "./PMSCOITOSForm.vue";
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
    "ml-editor": MultiLanguageEditor,
    "pms-coi-tos": COITOSEditor,
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
      newCOI: false,
      tableName: "PMS_CATEGORY_OF_INTERVENTION",
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
    userLocale() {
      return i18n.locale;
    },
  },
  watch: {
    presetVal: function (val) {
      this.search = val;
    },
  },
  beforeMount() {
    let localThis = this as any;
    localThis.formValid = false;
    localThis.idObject = localThis.formData.PMS_COI_ID;
    if (localThis.idObject == undefined) localThis.newCOI = true;
  },
  mounted() {
    let localThis = this as any;
    localThis.getTOS();
    (localThis.$refs.form as Vue & {
      resetValidation: () => void;
    }).resetValidation();
    // localThis.getLanguage();
    // localThis.descriptionDetails.Name = "pms.coi-description";
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
      (localThis.$refs.form as Vue & { reset: () => void }).reset();
      this.$emit("closeDialoge");
      localThis.descriptions = null;
    },

    save() {
      let localThis = this as any;
      localThis.formValid = (localThis.$refs.form as Vue & {
        validate: () => boolean;
      }).validate();
      if (!localThis.formValid) return;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-add-confirm")
        .toString();

      let id = 0;
      let msg = msgUpd;
      let value:any={};

      if (localThis.formData.PMS_COI_ID == undefined) {
        msg = msgAdd;
        localThis.formData.PMS_COI_ID = 0;
      }
      let obj: any;
      obj = localThis.formData;
      obj.descriptions = localThis.descriptions;
      if(obj.descriptions==undefined || obj.descriptions.length==0 ||
       obj.descriptions[0]==undefined || localThis.descriptions[0].IMS_LABELS_VALUE==undefined ||
       localThis.descriptions[0].IMS_LABELS_VALUE.trim()=='')
       {
        localThis.showNewSnackbar({
                typeName: "error",
                text: "Insert a description",
              }); // Feedback to user
          return;
       }

     if (localThis.formData.PMS_COI_ID == undefined || localThis.formData.PMS_COI_ID == 0) {
        localThis.formData.GUID = localThis.makeUUID();
      } else {
        if (
          localThis.formData.GUID == undefined ||
          localThis.formData.GUID == "undefined" ||
          localThis.formData.GUID == ""
        )
          localThis.formData.GUID = localThis.makeUUID();
      }
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_CATEGORY_OF_INTERVENTION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(obj),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
             
            }).then((res: any) => {
              value.dimensionCode = "CAT. OF INTERVENTION";
              value.coi_code = obj.PMS_COI_CODE;
              value.coi_descr = localThis.descriptions[0].IMS_LABELS_VALUE;
              if(obj.coi_relations_placeholder != undefined && obj.coi_relations_placeholder.length>0)
                value.tos_code = obj.coi_relations_placeholder[0].PMS_TOS_CODE;
              else
                value.tos_code = "";
              value.guid = obj.GUID;
              value.blocked = "FALSE";
              // value.location_description=res.LOCATION_DESCRIPTION;
              // value.office_code = saveData.PMS_OFFICE_CODE;
              localThis.updateNavDimension(value);
            })
            .catch((err: any) => {
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            }) .finally(() => {
           localThis.descriptions = null;
              (localThis.$refs.form as Vue & { reset: () => void }).reset();
              this.$emit("closeDialogeAndSave");
        });
        }
      });
    },

    getTOS() {
      let localThis = this as any;
      if (
        localThis.typeOfServices.length != undefined &&
        localThis.typeOfServices.length > 0
      )
        return;

      this.isLoading = true;
      const config: Configuration =
        localThis.$store.getters["auth/getApiConfig"];
      const api: PmsTypeOfServiceApi = new PmsTypeOfServiceApi(config);
      return api
        .apiPmsTypeOfServiceGet(config.baseOptions)
        .then((res) => {
          localThis.typeOfServices = res.data;
          let i: number = 0;
          for (i = 0; i < localThis.typeOfServices.length; i++) {
            localThis.typeOfServices[i].PMS_TOS_ID =
              localThis.typeOfServices[i].pmsTosId;
          }

          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch((err) => {
          // console.error(err);
          localThis.typeOfServices = [];
        })
        .finally(() => {
          localThis.isLoading = false;
        });
    },
  },
});
</script>

<style scoped>
</style>