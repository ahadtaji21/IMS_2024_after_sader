<template>
  <v-card>
    <v-card-text>
      <!-- <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>-->
      <v-form v-if="!isLoading">
        <v-row>
          <v-col> </v-col>
        </v-row>
        <v-text-field
          v-model="formData.TITLE"
          label="Service Title"
          hint=""
          :required="true"
          :rule="requiredTextFieldRule"
          :readonly="onlyRead"
        ></v-text-field>
        <v-row>
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
              :readonly="onlyRead || cascade"
              return-object
              :items="coi"
              item-value="PMS_COI_ID"
              item-text="IMS_LABELS_VALUE"
              @change="changeCOI()"
            ></v-autocomplete>
          </v-col>
          <v-col>
            <!-- TOS -->
            <v-autocomplete
              :label="$t('datasource.pms.type-of-service.type-of-service')"
              id="TOS"
              v-model="formData.TOS"
              :hint="$t('datasource.pms.type-of-service.type-of-service')"
              persistent-hint
              required
              return-object
              :items="tos"
              :readonly="onlyRead || cascade"
              item-value="PMS_TOS_ID"
              item-text="PMS_TOS_DESCRIPTION"
            ></v-autocomplete>
          </v-col>
        </v-row>
        <v-row v-if="module == 'IMP' && false">
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-start')"
              v-model="formData.DATE_FROM"
              :hint="$t('general.date-start')"
              :required="true"
              :readonly="formData.ACTIVATED == 1"
            ></jrs-date-picker>
          </v-col>
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-end')"
              v-model="formData.DATE_TO"
              :hint="$t('general.date-end')"
              :required="true"
              :readonly="formData.ACTIVATED == 1"
            ></jrs-date-picker>
          </v-col>
        </v-row>
        <v-row>
          <!-- OCCURRENCE< -->
          <v-col>
            <v-autocomplete
              :label="$t('datasource.pms.annual-plan.objectives.occurrence')"
              id="OCC"
              :readonly="onlyRead"
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
        <v-row>
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
    </v-card-text>
    <v-card-actions>
      <v-btn color="secondary darken-1" v-if="!onlyRead" text @click="closeEdit()"
        >X {{ $t("general.close") }}</v-btn
      >
      <v-btn color="primary" v-if="!onlyRead" @click="saveServices()">{{
        $t("general.save")
      }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import { ImsApi, PmsAnnualPlanApi, Configuration } from "../../../axiosapi";
import { i18n } from "../../../i18n";
import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
import { EventBus } from "@/event-bus";
interface ServiceData {
  ID: number | null;
  TITLE: String | null;
  COI: {} | null;
  TOS: {} | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  OCCURRENCE: {} | null;
  SERVED_PEOPLE: number | null;
  OBJECTIVE_ID: number | null;
}
interface payLoadData {
  ID: number | null;
  TITLE: String | null;
  COI_TOS_REL_ID: number | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  SERVED_PEOPLE: number | null;
  OCCURRENCE_ID: number | null;
  OBJECTIVE_ID: number | null;
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
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },

    selectedCategoryId: {
      type: Number,
      default: 0,
      required: false,
    },
    cascade: {
      type: Boolean,
      default: false,
      required: false,
    },
  },

  data() {
    return {
      module: "",
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
    ...mapGetters({
      userUid: "getUserUid",
      getActiveModule: "getActiveModule",
      getCurrentRole: "getCurrentRole",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  watch: {
    // formData(to: any) {
    //   let localThis = this as any;
    //   debugger;
    // }
  },
  mounted() {
    let localThis = this as any;
    EventBus.$on("saveServiceFromMainSave", (data: any) => {
      localThis.saveServiceFromMainSave();
    });
    localThis.isLoading = true;
    localThis.setDatasets();
    localThis.getOccurrences();
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    if (localThis.formData && localThis.formData.ID) {
      localThis.setDatasets("TOS");
    }
  },
  beforeDestroy() {
    EventBus.$off("saveServiceFromMainSave");
  },
  beforeMount() {
    let localThis: any = this as any;
    localThis.module = localThis.getActiveModule.moduleCode.toUpperCase();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    changeCOI() {
      let localThis = this as any;
      localThis.formData.TOS = {};
      localThis.setDatasets("TOS");
    },
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
          viwName = "PMS_VI_CATEGORY_OF_INTERVENTION_TARGET";
          localThis.coi.length = 0;
          if (
            localThis.selectedCategoryId == undefined ||
            localThis.selectedCategoryId == 0
          )
            condition = `PMS_COI_ENABLED=1 AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`;
          //Richiesto da Cecilia: vedere comunque tutte le categorie AND PMS_TARGET_ID=${localThis.formDataExt.PMS_TARGET_ID}`;
          else
            condition = `PMS_COI_ENABLED=1 AND IMS_LANGUAGE_LOCALE='${i18n.locale}' AND PMS_COI_ID=${localThis.selectedCategoryId}`;
          orderItem = "IMS_LABELS_VALUE";
          localThis.tos.length = 0;
          localThis.formData.TOS = Object.assign({});
          columnList = "PMS_COI_ID,IMS_LABELS_VALUE,PMS_COI_CODE";
          break;
        case "TOS":
          datasetName = "tos";
          viwName = "PMS_VI_TYPE_OF_SERVICE_TARGET";
          localThis.tos.length = 0;
          if (
            localThis.selectedCategoryId == undefined ||
            localThis.selectedCategoryId == 0
          )
            condition = `PMS_COI_ID=${localThis.formData.COI.PMS_COI_ID} AND PMS_TARGET_ID=${localThis.formDataExt.PMS_TARGET_ID}`;
          else if (
            localThis.formDataExt != undefined &&
            localThis.formDataExt.PMS_TARGET_ID != undefined
          ) {
            condition = `PMS_COI_ID=${localThis.selectedCategoryId}  AND PMS_TARGET_ID=${localThis.formDataExt.PMS_TARGET_ID}`;
          } else {
            condition = `PMS_COI_ID=${localThis.selectedCategoryId}`;
          }

          orderItem = "PMS_TOS_DESCRIPTION";
          columnList = "PMS_TOS_ID,PMS_TOS_DESCRIPTION,PMS_COI_TOS_ID";

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
          if (datasetName == "coi") {
            if (
              localThis.selectedCategoryId == undefined ||
              localThis.selectedCategoryId == 0
            ) {
              localThis.coi = [...(res.table_data ? res.table_data : [])];
            } else {
              localThis.coi = [];
              res.table_data.forEach((item: any) => {
                localThis.coi.push(item);
              });
              localThis.formData.COI = localThis.coi[0];
              localThis.formData.TOS = {};
              localThis.setDatasets("TOS");
            }

            // localThis.coi.unshift({
            //   PMS_COI_ID: null,
            //   IMS_LABELS_VALUE: "N/A",
            //   PMS_COI_CODE: "N/A",
            // });
          } else {
            localThis.tos = [...(res.table_data ? res.table_data : [])];
            // localThis.tos.unshift({
            //   PMS_TOS_ID: null,
            //   PMS_TOS_DESCRIPTION: "N/A",
            //   PMS_COI_TOS_ID: "N/A",
            // });

            // if (
            //   tos_id != undefined &&
            //   tos_desc != undefined &&
            //   tos_coi_tos_id != undefined
            // ) {
            //   localThis.formData.TOS.PMS_TOS_ID = tos_id;
            //   localThis.formData.TOS.PMS_TOS_DESCRIPTION = tos_desc;
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
    saveServiceFromMainSave() {
      let localThis = this as any;
      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let date_from = null;
      let date_to = null;
      let id = 0;

      let payLoadD = {} as payLoadData;
      if (localThis.formData.ID == undefined) {
        payLoadD.ID = 0;
      } else {
        payLoadD.ID = localThis.formData.ID;
      }
      localThis.formData.DATE_FROM = date_from;
      localThis.formData.DATE_TO = date_to;
      payLoadD.OBJECTIVE_ID = localThis.selectedObjectId;
      payLoadD.DATE_FROM = date_from;
      payLoadD.DATE_TO = date_to;
      if (localThis.formData.OCCURRENCE != undefined)
        payLoadD.OCCURRENCE_ID = localThis.formData.OCCURRENCE.ID;
      payLoadD.SERVED_PEOPLE = localThis.formData.SERVED_PEOPLE;
      if (localThis.formData.TOS.PMS_COI_TOS_ID == undefined) return;
      payLoadD.COI_TOS_REL_ID = localThis.formData.TOS.PMS_COI_TOS_ID;
      payLoadD.TITLE = localThis.formData.TITLE;

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.$emit("refreshObjSrv");
          EventBus.$emit("reloadObj");
          localThis.editMode = false;
          // Feedback to user

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
    },

    saveServices() {
      let localThis = this as any;

      let ap = {} as any;
      ap = localThis.$store.getters.getAnnualPlan;
      let date_from = null;
      let date_to = null;
      if (localThis.module == "IMP") {
        // if (
        //   localThis.formData.DATE_FROM == undefined ||
        //   localThis.formData.DATE_FROM == "" ||
        //   localThis.formData.DATE_TO == undefined ||
        //   localThis.formData.DATE_TO == ""
        // ) {
        //   localThis.showNewSnackbar({
        //     typeName: "error",
        //     text: "Error in Service date definition",
        //   });
        //   return;
        // }
        // if (localThis.formData.DATE_FROM.constructor.name == "Date") {
        //   date_from = localThis.formData.DATE_FROM;
        // } else {
        //   date_from = new Date(
        //     localThis.formData.DATE_FROM.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
        //   );
        // }
        // if (localThis.formData.DATE_TO.constructor.name == "Date") {
        //   date_to = localThis.formData.DATE_TO;
        // } else {
        //   date_to = new Date(
        //     localThis.formData.DATE_TO.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")
        //   );
        // }
        // if (date_from.getFullYear() < ap.start_year) {
        //   localThis.showNewSnackbar({
        //     typeName: "error",
        //     text: "Start date before than Annual Plan starting",
        //   }); //
        //   return;
        // }
        // if (date_from >= date_to) {
        //   localThis.showNewSnackbar({
        //     typeName: "error",
        //     text: "Error in Service date definition: Start Date > End Date",
        //   });
        //   return;
        // }
        // if (date_to.getFullYear() > ap.end_year) {
        //   localThis.showNewSnackbar({
        //     typeName: "error",
        //     text: "End date after than Annual Plan ending",
        //   }); //
        //   return;
        // }
      }
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
      localThis.formData.DATE_FROM = date_from;
      localThis.formData.DATE_TO = date_to;
      payLoadD.OBJECTIVE_ID = localThis.selectedObjectId;
      payLoadD.DATE_FROM = date_from;
      payLoadD.DATE_TO = date_to;
      if (localThis.formData.OCCURRENCE != undefined)
        payLoadD.OCCURRENCE_ID = localThis.formData.OCCURRENCE.ID;
      payLoadD.SERVED_PEOPLE = localThis.formData.SERVED_PEOPLE;
      if (localThis.formData.TOS.PMS_COI_TOS_ID == undefined) return;
      payLoadD.COI_TOS_REL_ID = localThis.formData.TOS.PMS_COI_TOS_ID;
      payLoadD.TITLE = localThis.formData.TITLE;
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.$emit("refreshObjSrv");
              EventBus.$emit("reloadObj");
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user

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

<style scoped></style>
