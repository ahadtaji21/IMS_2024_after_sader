<template>
  <v-card>
    <v-card-text>
      <!-- <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>-->
      <v-form v-if="!isLoading">
        <v-row>
          <v-col></v-col>
        </v-row>
        <v-text-field
          v-model="formData.TITLE"
          label="Service Title"
          :readonly="onlyRead"
          hint
          :required="true"
          :rule="requiredTextFieldRule"
        ></v-text-field>
        <v-row>
          <v-col>
            <!-- COI -->
            <v-autocomplete
            :readonly="onlyRead"
              :label="
                $t(
                  'datasource.pms.category-of-intervention.category-of-intervention'
                )
              "
              id="COI"
              v-model="formData.COI"
              :hint="
                $t(
                  'datasource.pms.category-of-intervention.category-of-intervention'
                )
              "
              persistent-hint
              required
              return-object
              :items="coi"
              item-value="GMT_COI_ID"
              item-text="IMS_LABELS_VALUE"
              @change="setDatasets('TOS')"
            ></v-autocomplete>
          </v-col>
          <v-col>
            <!-- TOS -->
            <v-autocomplete
              :label="$t('datasource.pms.type-of-service.type-of-service')"
              id="TOS"
              :readonly="onlyRead"
              v-model="formData.TOS"
              :hint="$t('datasource.pms.type-of-service.type-of-service')"
              persistent-hint
              required
              return-object
              :items="tos"
              item-value="GMT_TOS_ID"
              item-text="GMT_TOS_DESCRIPTION"
            ></v-autocomplete>
          </v-col>
        </v-row>
        <v-row >
          <v-col>
            <jrs-date-picker
              
              :label="$t('general.date-start')"
              v-model="formData.DATE_FROM"
              :hint="$t('general.date-start')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
          <v-col>
            
            <jrs-date-picker
              :label="$t('general.date-end')"
              v-model="formData.DATE_TO"
              :hint="$t('general.date-end')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
        </v-row>
        <v-row>
          <!-- OCCURRENCE< -->
          <v-col>
            <v-autocomplete
              :readonly="onlyRead"
              :label="$t('datasource.pms.annual-plan.objectives.occurrence')"
              id="OCC"
              v-model="formData.OCCURRENCE"
              :hint="$t('datasource.pms.annual-plan.objectives.occurrence')"
              persistent-hint
              required
              return-object
              :items="occ"
              item-value="ID"
              item-text="TYPE"
            ></v-autocomplete>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions v-if="!onlyRead">
      <v-btn color="secondary darken-1" text @click="closeEdit()"
        >X {{ $t("general.close") }}</v-btn
      >
      <v-btn color="primary"  @click="saveServices()">{{
        $t("general.save")
      }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import {
  ImsApi,
  PmsAnnualPlanApi,
  Configuration,
} from "../../../../../axiosapi";
import { i18n } from "../../../../../i18n";
import JrsDatePicker from "../../../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";

interface ServiceData {
  ID: number | null;
  TITLE: String | null;
  COI: {} | null;
  TOS: {} | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  OCCURRENCE: {} | null;
  OBJECTIVE_ID: number | null;
  DONOR_ID: number | null;
}
interface payLoadData {
  ID: number | null;
  TITLE: String | null;
  COI_TOS_REL_ID: number | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  OCCURRENCE_ID: Date | null;
  OBJECTIVE_ID: number | null;
  DONOR_ID: number | null;
}
export default Vue.extend({
  components: {
    "jrs-date-picker": JrsDatePicker,
  },

  props: {
    formDataExt: {
      type: Object,
    },
    selectedObjectId: {
      type: Number,
      required: true,
    },
    onlyRead:{
      type:Boolean,
      required:false,
      default:false

    }
  },
  data() {
    return {
      donor_id: null,
      formData: {},
      formIsValid: false,
      isLoading: false,
      coi: [],
      tos: [],
      occ: [],
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
  beforeMount() {
    let localThis = this as any;
    let gt = {} as any;
    gt = localThis.$store.getters.getGrant;
    localThis.donor_id = gt.donor_id;
    localThis.donor_name = gt.donor_name;
    localThis.isLoading = true;
    localThis.setDatasets();
    localThis.getOccurrences();
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    if (localThis.formData && localThis.formData.ID) {
      localThis.setDatasets("TOS");
    }
  },
  mounted() {},
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
          viwName = "GMT_VI_CATEGORY_OF_INTERVENTION";
          localThis.coi.length = 0;
          condition = `GMT_COI_ENABLED=1 AND IMS_LANGUAGE_LOCALE='${i18n.locale}' AND GMT_DONOR_ID=${localThis.donor_id}`;
          orderItem = "IMS_LABELS_VALUE";
          localThis.tos.length = 0;
          localThis.formData.TOS = Object.assign({});
          columnList = "GMT_COI_ID,IMS_LABELS_VALUE,GMT_COI_CODE";
          break;
        case "TOS":
          datasetName = "tos";
          viwName = "GMT_VI_TYPE_OF_SERVICE_1";
          localThis.tos.length = 0;

          condition = `GMT_COI_ID=${localThis.formData.COI.GMT_COI_ID} AND GMT_DONOR_ID=${localThis.donor_id}`;
          orderItem = "GMT_TOS_DESCRIPTION";
          columnList = "GMT_TOS_ID,GMT_TOS_DESCRIPTION,PMS_COI_TOS_ID";

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
          if (datasetName == "coi") {
            localThis[datasetName].unshift({
              GMT_COI_ID: null,
              IMS_LABELS_VALUE: "N/A",
              GMT_COI_CODE: "N/A",
            });
          } else {
            localThis[datasetName].unshift({
              GMT_TOS_ID: null,
              GMT_TOS_DESCRIPTION: "N/A",
              PMS_COI_TOS_ID: "N/A",
            });
            // if (
            //   tos_id != undefined &&
            //   tos_desc != undefined &&
            //   tos_coi_tos_id != undefined
            // ) {
            //   localThis.formData.TOS.GMT_TOS_ID = tos_id;
            //   localThis.formData.TOS.GMT_TOS_DESCRIPTION = tos_desc;
            //   localThis.formData.TOS.PMS_COI_TOS_ID = tos_coi_tos_id;
            // }
          }
          localThis.isLoading = false;
        });
    },

    closeEdit() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.$emit("closeEdit");
    },
    saveServices() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add-service")
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
      payLoadD.DONOR_ID = localThis.donor_id;
      payLoadD.OBJECTIVE_ID = localThis.selectedObjectId;
      payLoadD.DATE_FROM = localThis.formData.DATE_FROM;
      payLoadD.DATE_TO = localThis.formData.DATE_TO;
      payLoadD.OCCURRENCE_ID = localThis.formData.OCCURRENCE.ID;
      payLoadD.COI_TOS_REL_ID = localThis.formData.TOS.PMS_COI_TOS_ID;
      payLoadD.TITLE = localThis.formData.TITLE;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_GRANT_OBJECTIVE_SERVICE",
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
              localThis.formData = {} as ServiceData;
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    getOccurrences() {
      let localThis: any = this as any;
      localThis.serviceList = [];
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_OCCURRENCE_TYPE",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.occ.push(item);
            });
          }
        }
      );
    },
  },
});
</script>

<style scoped>
</style>