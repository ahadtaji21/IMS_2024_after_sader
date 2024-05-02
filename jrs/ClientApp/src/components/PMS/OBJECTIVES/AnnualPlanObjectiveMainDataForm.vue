<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
          <!-- <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.ap-brief-description')"
                v-model="formData.DESCRIPTION"
                :counter="150"
                required
              ></v-text-field>
            </v-col>
          </v-row> -->
          <v-row align-center v-if="showWait">
            <v-col justify-center :cols="12">
              <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
                <v-progress-circular indeterminate color="primary"></v-progress-circular>
              </div>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-select
                :items="targets"
                :label="$t('datasource.pms.annual-plan.objectives.outcome-obj-target')"
                item-text="text"
                item-value="value"
                v-model="formData.PMS_TARGET_ID_STR"
                :rules="[(v) => !!v || 'Item is required']"
                v-on:change="resetOutcome()"
              ></v-select>
            </v-col>
          </v-row>
          <v-row
            v-if="
              formData.PMS_TARGET_ID_STR != undefined && formData.PMS_TARGET_ID_STR != ''
            "
          >
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <jrs-multi-select-table
                v-model="formData.OUTCOME"
                :label="$t('datasource.pms.annual-plan.objectives.outcome')"
                :dataSource="'PMS_VI_CONSTRUCTION_DEF_DIMENSION'"
                :itemValue="'PMS_CONSTRUCTION_DEF_ID'"
                :itemText="'PMS_CONSTRUCTION_DEF_DESC'"
                :key="keyTbl"
                :allowMultiselect="allowMultiselect"
                :required="true"
                :returnOnlyIds="false"
                :dataSourceCondition="dataSourceCondition"
                :rowDisableSelectRules="rowDisableSelectRules"
              ></jrs-multi-select-table>
            </v-col>
          </v-row>
          <v-row v-if="false">
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-text-field
                :label="$t('datasource.pms.annual-plan.objectives.est-srv-nbr')"
                v-model="formData.SERVED_PEOPLE"
                :counter="150"
                type="number"
                required
              ></v-text-field>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>

    <v-card-actions v-if="isUpdatableForm">
      <v-btn
        color="secondary"
        class="--darken-1"
        @click="close()"
        v-if="formData.ID == undefined || formData.ID == 0"
        >Cancel</v-btn
      >
      <v-btn
        color="primary"
        class="--darken-1"
        @click="save()"
        v-if="formData.ID == undefined || formData.ID == 0"
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
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { i18n } from "../../../i18n";
import JrsMultiSelectTable from "../../../components/JrsMultiSelectedTable.vue";
import { mapActions, mapGetters } from "vuex";
import { EventBus } from "../../../event-bus";
import anymatch from "anymatch";
export default Vue.extend({
  components: {
    "jrs-multi-select-table": JrsMultiSelectTable,
  },

  props: {
    formDataExt: {
      type: Object,
      required: false,
    },
    isUpdatableForm: {
      type: Boolean,
      required: true,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
    allowMultiselect: {
      type: Boolean,
      default: false,
      required: false,
    },
  },

  data() {
    return {
      formData: {} as any,
      showWait: false,
      keyTbl: 0,
      formIsValid: false,
      targets: [],

      dataSourceOrder: "",
      dataSourceCondition: "PMS_TARGET_ID=-1",
      rowDisableSelectRules: [],
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
    EventBus.$on("saveObjectiveFromMainSave", (data: any) => {
      localThis.saveFromMain();
    });
  },
  beforeDestroy() {
    EventBus.$off("saveObjectiveFromMainSave");
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
    save: function () {
      let localThis = this as any;

      localThis.$emit("closeDialogeAndSaveObj", localThis.formData);
    },
    saveFromMain: function () {
      let localThis = this as any;

      localThis.$emit("closeDialogeAndSaveObjFromMain", localThis.formData);
    },
    resetOutcome() {
      let localThis = this as any;
      localThis.fieldValue = {};
      localThis.formData.OUTCOME = [];
      localThis.dataSourceCondition =
        "PMS_DIMENSION='Outcome Based Objective' and PMS_TARGET_ID=" +
        Number.parseInt(localThis.formData.PMS_TARGET_ID_STR);
      localThis.keyTbl = Math.ceil(Math.random() * 1000);
    },

    getList(listName: any) {
      // if (
      //   obj.length != undefined &&
      //   obj.length > 0
      // )
      //  return;
      let localThis = this as any;
      localThis.showWait = true;
      //this.isLoading = true;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: ImsLookupsApi = new ImsLookupsApi(config);
      return api
        .apiImsLookupsListNameGet(listName, config.baseOptions)
        .then((res) => {
          localThis.showWait = false;
          switch (listName) {
            case "PMS_OBJ_OUTCOME_TARGET":
              localThis.targets = res.data;
              break;
          }
          //obj = res.data;
          //alert(res.data[0].pmsTosDescription);
          return res;
        })
        .catch((err) => {
          localThis.showWait = false;
          // console.error(err);
          return [];
        })
        .finally(() => (localThis.isLoading = false));
    },
  },
  filters: {
    toNmbr: function (str: any) {
      if (str != undefined) {
        return Number.parseInt(str);
      } else return 0;
    },
  },
  beforeMount() {
    let localThis: any = this;
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.dataSourceCondition =
      "PMS_DIMENSION='Outcome Based Objective' and PMS_TARGET_ID=" +
      localThis.formData.PMS_TARGET_ID;
    if (localThis.targets.length == undefined || localThis.targets.length == 0)
      localThis.getList("PMS_OBJ_OUTCOME_TARGET");
  },
});
</script>

<style scoped></style>
