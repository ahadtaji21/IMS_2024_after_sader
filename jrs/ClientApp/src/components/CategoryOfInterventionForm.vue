<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :label="$t('pms.coi-code')"
                v-model="formData.pmsCoiCode"
                :counter="20"
                required
              ></v-text-field>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <!-- <v-text-field :label="$t('pms.coi-description')"
                                          v-model="formData.pmsCoiDescription"
              :counter="150"></v-text-field>-->
              <ml-editor :fieldDetails="descriptionDetails"></ml-editor>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-checkbox
                :cols="$vuetify.breakpoint.xsOnly ? 12 : 6"
                :label="$t('pms.coi-enabled')"
                v-model="formData.pmsCoiEnabled"
              ></v-checkbox>
            </v-col>
          </v-row>

          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-autocomplete
                v-model="formData.pmsCoiTosRel"
                :items="typeOfServices"
                outlined
                :loading="isLoading"
                :search-input.sync="search"
                dense
                chips
                small-chips
                :label="$t('pms.toss')"
                item-text="pmsTosDescription"
                item-value="pmsTosId"
                multiple
              ></v-autocomplete>
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
import MultiLanguageEditor from "../components/MultiLanguageEditor.vue";
import {
  ImsApi,
  PmsCategoryOfInterventionApi,
  PmsTypeOfServiceApi,
  Configuration
} from "../axiosapi";
import { i18n } from "../i18n";

export default Vue.extend({
  components: {
    "ml-editor": MultiLanguageEditor
  },

  props: {
    formData: {
      type: Object
    },
    presetVal: {
      type: String,
      required: false,
      default: ""
    }
  },
  data() {
    return {
      formIsValid: false,
      datePickerMenu: false,
      someString: "some",
      datePlaceholder: null,
      localData: null,
      typeOfServices: [],
      isLoading: false,
      //model:  (this as any).formData.pmsCoiTosRel.map((x: any) => {
      //return x.pmsTosId;
      //}),
      tb: null,
      search: this.presetVal,
      descriptionDetails: {
        Name: String,
        Vlue: Object
      },
      languages: [],
      languagesdef: []
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    }
  },
  watch: {
    presetVal: function(val) {
      this.search = val;
    }
  },

  mounted() {
    (this as any).getTOS();
    (this as any).getLanguage();
    // (this as any).descriptionDetails.Name = "pms.coi-description";
    //         (this as any).descriptionDetails.Vlue = (this as any).languages ;
 
    // const data = {
    //   en: "testen",
    //   fr: "testfr"
    // };
      
  },
  methods: {
    getLanguage() {
      const config: Configuration = (this as any).$store.getters[
        "auth/getApiConfig"
      ];
      const api: ImsApi = new ImsApi(config);
      return api
        .apiImsGetImsLanguageGet(config.baseOptions)
        .then(res => {
          (this as any).languagesdef = res.data.map((x: any) => {
            return {
              id: x.imsLanguageId,
              code: x.imsLanguageIso6391Code
            };
          });
        })
        .then(
            (this as any).languages = (this as any).formData.descriptions.map(
            (x: any) => {
              return {
                languagecode: (this as any).languagesdef[x.id],
                value: x.imsLabelsValue
              };
            }
          )
        )
        .then(
            (this as any).descriptionDetails = (this as any).languages.map(
            (x: any) => {
              return {
                Name: "pms.coi-description",
                value: x
              };
            }
          )

        )
        .catch(err => {
          // console.error(err);
          (this as any).rows = [];
        });
    },

    close: function() {
      this.$emit("closeDialoge");
    },
    save: function() {

      if (
        (this as any).formData.pmsCoiTosRel != null &&
        (this as any).formData.pmsCoiTosRel.length > 0
      ) {
        let tmpobj = (this as any).formData.pmsCoiTosRel.map((x: any) => {
          if (x.pmsCoiTosId == undefined) {
            return {
              pmsCoiTosId: 0,
              pmsCoiId: (this as any).formData.pmsCoiId,
              pmsTosId: x,
              pmsTos: null,
              pmsCoi: null
            };
          } else {
            //x.pmsCoi=null;
            return {
              pmsCoiTosId: x.pmsCoiTosId,
              pmsCoiId: (this as any).formData.pmsCoiId,
              pmsTosId: x.pmsTosId,
              pmsTos: null,
              pmsCoi: null
              //x
            };
          }
        });
        (this as any).formData.pmsCoiTosRel = tmpobj;
      }
      this.$emit("closeDialogeAndSave", this.formData);
    },
    getTOS() {
      if (
        (this as any).typeOfServices.length != undefined &&
        (this as any).typeOfServices.length > 0
      )
        return;

      this.isLoading = true;
      const config: Configuration = (this as any).$store.getters[
        "auth/getApiConfig"
      ];
      const api: PmsTypeOfServiceApi = new PmsTypeOfServiceApi(config);
      return api
        .apiPmsTypeOfServiceGet(config.baseOptions)
        .then(res => {
          (this as any).typeOfServices = res.data;
          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch(err => {
          // console.error(err);
          (this as any).typeOfServices = [];
        })
        .finally(() => (this.isLoading = false));
    }
  }
});
</script>

<style scoped>
</style>