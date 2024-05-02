<template>
  <v-card>
    <v-card-text>
      <!-- <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>-->
      <v-form v-if="!isLoading">
        <v-row>
          <v-col>
            <v-text-field
              v-model="formData.TITLE"
              label="Title"
              hint=""
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>

        <v-row>
          <v-col>
            <v-select
              :items="formData.outcome"
              :label="$t('datasource.pms.annual-plan.objectives.outcome')"
              item-text="PMS_CONSTRUCTION_DEF_DESC"
              item-value="PMS_CONSTRUCTION_DEF_ID"
              v-model="otc"
              :rules="[(v) => !!v || 'Item is required']"
            ></v-select>
          </v-col>
        </v-row>

        <v-row v-if="otc != 0">
          <v-col>
            <!-- COI -->
            <v-autocomplete
              :label="
                $t('datasource.pms.category-of-intervention.category-of-intervention')
              "
              id="COI"
              v-model="formData.COI"
              :hint="
                $t('datasource.pms.category-of-intervention.category-of-intervention')
              "
              persistent-hint
              required
              :readonly="onlyRead"
              return-object
              :items="coi"
              item-value="PMS_COI_ID"
              item-text="IMS_LABELS_VALUE"
            ></v-autocomplete>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-btn color="secondary darken-1" v-if="!onlyRead" text @click="closeEdit()"
        >X {{ $t("general.close") }}</v-btn
      >
      <v-btn color="primary" v-if="!onlyRead" @click="saveCategories()">{{
        $t("general.save")
      }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { ImsApi, PmsAnnualPlanApi, Configuration } from "../../../axiosapi";
import { i18n } from "../../../i18n";
import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface CategoryData {
  ID: number | null;
  TITLE: String | null;
  OUTCOME_ID: number | null;
  COI_ID: number | null;
  OBJECTIVE_ID: number | null;
}
interface payLoadData {
  ID: number | null;
  TITLE: String | null;
  OUTCOME_ID: number | null;
  COI_ID: number | null;
  OBJECTIVE_ID: number | null;
}
export default Vue.extend({
  components: {
    //"jrs-date-picker": JrsDatePicker
  },

  props: {
    formDataExt: {
      type: Object,
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      otc: 0,
      outcome: [],
      formData: {},
      formIsValid: false,
      isLoading: false,
      coi: [],
      requiredTextFieldRule: [(v: any) => !!v || "Title is required"],
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
  },
  watch: {
    // formData(to: any) {
    //   let localThis = this as any;
    //   debugger;
    // }
  },
  mounted() {
    let localThis = this as any;
    localThis.isLoading = true;
    localThis.setDatasets();
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.outcome = localThis.formData.outcome;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    /**
     * Refresh autocomplete.
     * @param changedCode - code of the autocomplete which has changed
     */
    setDatasets(changedCode?: any) {
      let localThis: any = this as any;
      let datasetName: string = "";
      let viwName: string | null = null;
      let orderItem: string | null = null;
      let condition: string | null = null;
      let columnList: string | null = null;
      let defaultItemCode: string; //item code for defautl null value on loaded selectItems
      //let language:string = localThis.userLocale();
      switch (changedCode) {
        case undefined:
          datasetName = "coi";
          viwName = "PMS_VI_CATEGORY_OF_INTERVENTION";
          localThis.coi.length = 0;
          condition = `PMS_COI_ENABLED=1 AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`;
          orderItem = "IMS_LABELS_VALUE";
          columnList = "PMS_COI_ID,IMS_LABELS_VALUE,PMS_COI_CODE";
          break;
      }

      let selectPayload: GenericSqlSelectPayload = {
        viewName: viwName,
        colList: columnList,
        whereCond: condition,
        orderStmt: orderItem,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis[datasetName] = [...(res.table_data ? res.table_data : [])];
          localThis.isLoading = false;
        });
    },

    closeEdit() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.$emit("closeEdit");
    },
    saveCategories() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add-category")
        .toString();

      let id = 0;
      let msg = msgUpd;
      let payLoadD = {} as payLoadData;
      if (localThis.formData.ID == undefined) {
        msg = msgAdd;
        payLoadD.ID = 0;
      } else {
        payLoadD.ID = localThis.formData.ID;
      }

      payLoadD.OBJECTIVE_ID = localThis.selectedObjectId;
      payLoadD.COI_ID = localThis.formData.COI;
      payLoadD.OUTCOME_ID = localThis.formData.OUTCOME_ID;
      payLoadD.TITLE = localThis.formData.TITLE;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_CATEGORY",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.$emit("refreshObjSrv");

              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.dialog = false;
              localThis.showSrvDetails = false;
              localThis.tmpSelectedItem = [];
              localThis.serviceList = localThis.serviceListOrig;
              localThis.itemsPerPage = 15;
              localThis.$emit("closeDialoge");
              localThis.$emit("refreshDesc", localThis.formData);
            })
            .catch((err: any) => {
              localThis.formData = {} as CategoryData;
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

<style scoped></style>
