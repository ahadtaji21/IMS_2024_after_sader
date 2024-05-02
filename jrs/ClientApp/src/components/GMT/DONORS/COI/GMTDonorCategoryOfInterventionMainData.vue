<template>
  <v-card>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid>
        <v-form
          v-model="formValid"
          ref="myFormCOI"
          lazy-validation
          class="text-capitalize"
        >
          <v-row>
            <v-col :cols="12">
              <v-text-field
                :label="$t('pms.coi-code')"
                v-model="formData.GMT_COI_CODE"
                :rules="rules.required"
                :counter="150"
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
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formData.GMT_JRS_COI_ID"
                :items="jrsCOI"
                outlined
                :loading="isLoading"
                :search-input.sync="search"
                dense
                :label="$t('pms.jrs-coi-code')"
                item-text="text"
                item-value="value"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-autocomplete>
            </v-col>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-checkbox
                :label="$t('pms.coi-enabled')"
                v-model="formData.GMT_COI_ENABLED"
              ></v-checkbox>
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
import { mapActions } from "vuex";
import MultiLanguageEditor from "../../../../components/MultiLanguageEditor.vue";

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
import { IImsList } from "./../../../PMS/PMSSharedInterfaces";
export default Vue.extend({
  components: {
    "ml-editor": MultiLanguageEditor,
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
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      jrsCOI: [],
      showWait: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
      tableName: "GMT_DONOR_CATEGORY_OF_INTERVENTION",
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

    localThis.idObject = localThis.formData.GMT_COI_ID;
    if (localThis.jrsCOI.length == undefined || localThis.jrsCOI.length == 0)
      localThis.getListCond();
  },
  mounted() {
    let localThis = this as any;
    localThis.formValid = false;
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
      (localThis.$refs.myFormCOI as Vue & { reset: () => void }).reset();
      this.$emit("closeDialoge");
      localThis.descriptions = null;
    },
    getListCond() {
      let localThis: any = this as any;
      localThis.jrsCOI = [];
      localThis.showWait = true;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONORS_JRS_CATEGORY_OF_INTERVENTION",
        colList: null,
        whereCond:
          localThis.formData.GMT_JRS_COI_ID == undefined
            ? `IMS_LANGUAGE_LOCALE='${i18n.locale}' AND (GMT_DONOR_ID is null or txt not in (select txt from  GMT_VI_DONORS_JRS_CATEGORY_OF_INTERVENTION where GMT_DONOR_ID = ${localThis.selectedObjectId}))`
            : `IMS_LANGUAGE_LOCALE='${i18n.locale}' AND (vlue=${localThis.formData.GMT_JRS_COI_ID} OR (GMT_DONOR_ID is null or txt not in (select txt from  GMT_VI_DONORS_JRS_CATEGORY_OF_INTERVENTION where GMT_DONOR_ID = ${localThis.selectedObjectId})))`,
        orderStmt: "txt",
      };
      console.log(selectPayload);
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let itm = {} as IImsList;
              itm.text = item.txt;
              itm.value = item.vlue + "";
              if (
                localThis.jrsCOI.filter(
                  (itemL: any) => itemL.value == itm.value
                ).length == 0
              ) {
                localThis.jrsCOI.push(itm);
              }
            });
          }
        })
        .catch((err) => {
          // console.error(err);
          return [];
        })
        .finally(() => {
          localThis.isLoading = false;
          localThis.showWait = false;
        });
    },

    save() {
      let localThis = this as any;
      (localThis.$refs.myFormCOI as Vue & {
        validate: () => boolean;
      }).validate();
      if (!localThis.formValid) return;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-update-confirm")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.gmt.donor.coi.add-coi")
        .toString();

      let id = 0;
      let msg = msgUpd;

      if (localThis.formData.GMT_COI_ID == undefined) {
        msg = msgAdd;
        localThis.formData.GMT_COI_ID = 0;
      }
      let obj: any;
      localThis.formData.GMT_DONOR_ID = localThis.selectedObjectId;
      obj = localThis.formData;
      obj.descriptions = localThis.descriptions;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_CATEGORY_OF_INTERVENTION",
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
              localThis.descriptions = null;
              (localThis.$refs.myFormCOI as Vue & {
                reset: () => void;
              }).reset();
              this.$emit("closeDialogeAndSave");
            })
            .catch((err: any) => {
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
  },
});
</script>

<style scoped>
</style>