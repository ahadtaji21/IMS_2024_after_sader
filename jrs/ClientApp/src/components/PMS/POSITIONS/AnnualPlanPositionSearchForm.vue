<template>
  <div>
    <v-card>

        <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
                <v-progress-circular
                  indeterminate
                  color="primary"
                ></v-progress-circular>
              </div>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="positionList"
          item-key="HR_POSITION_ID"
          multi-sort
          :search="tableFilter"
          :items-per-page="10"
          class="text-capitalize"
          v-model="selectedPos"
          show-select
          :single-select="true"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>{{ $t("datasource.pms.annual-plan.objectives.positions") }}</h3>

              <v-spacer></v-spacer>
              <v-dialog
                v-if="false"
                v-model="editMode"
                persistent
                max-width="50%"
                retain-focus
                :scrollable="true"
                :overlay="true"
                transition="dialog-transition"
              >
                <template v-slot:activator="{ on }">
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    v-on="on"
                    @click="OpenDialog()"
                  >
                    <v-icon>mdi-plus-circle-outline</v-icon>New
                  </v-btn>
                </template>
                <v-card>
                  <v-card-title>
                    <span class="h5">User Profile</span>
                  </v-card-title>
                  <v-card-text>
                    <v-container>
                      <jrs-form
                        :formData="formData"
                        :formFields="formFields"
                        :nbrOfColumns="2"
                      >
                        <template v-slot:form-actions="{ validateFunc }">
                          <v-btn color="secondary darken-1" text @click="closeEdit()"
                            >X {{ $t("general.close") }}</v-btn
                          >
                          <v-btn
                            color="primary"
                            :disabled="isSaving"
                            @click="
                              savePositionFunc(
                                formData,
                                formData.PRIMARY_KEY ? 1 : 0,
                                validateFunc
                              )
                            "
                            class="ma-2"
                            >{{ $t("general.save") }}</v-btn
                          >
                          <!-- <v-btn
                            color="primary"
                            @click="resetFunc()"
                            class="ma-2"
                          >{{ $t('general.reset') }}</v-btn> -->
                        </template>
                      </jrs-form>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-dialog>

              <v-spacer></v-spacer>

              <v-text-field
                v-model="tableFilter"
                append-icon="mdi-magnify"
                :label="$t('general.search')"
                hide-details
                clearable
                outlined
                dense
                class="white"
                color="secondary darken-2"
              ></v-text-field>
            </div>
          </template>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-btn color="secondary darken-1" text @click="clearField()"
          >X {{ $t("general.close") }}</v-btn
        >
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import JrsForm from "../../../components/JrsForm.vue";
import FormMixin from "../../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    "jrs-form": JrsForm,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },

    insertedPositions: {
      type: Array,
      required: false,
    },
  },
  mixins: [FormMixin],
  data() {
    return {
      showWait:false,
      userUid: null,
      editMode: false,
      formData: {},
      formFields: [],
      headers: [],
      positionList: [],
      tableFilter: "",
      selectedPos: [],
      itemKey: "",
      position_CRUD: {},
      isSaving: false,
    };
  },
  methods: {


    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),
    closeEdit() {
      let localThis: any = this as any;
      localThis.editMode = false;
    },
    OpenDialog() {
      let localThis: any = this as any;
      localThis.editMode = !localThis.editMode;
    },
    /**
     * Save user and position info.
     */
    savePositionFunc(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      let a: boolean = true;
      let localThis: any = this as any;
      localThis.isSaving = true;
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.create-add-position")
        .toString();

      //Check form validity
      if (formValidateFunc()) {
        this.$confirm(msg).then((res1: any) => {
          if (res1) {
            let spName: string =
              actionType == SqlActionType.NUMBER_0
                ? localThis.position_CRUD.create_sp
                : localThis.position_CRUD.update_sp;
            //Add external data to payload
            if (localThis.savePayload) {
              saveData = {
                external_data: localThis.savePayload,
                rows: saveData,
              };
            }
            let savePayload: GenericSqlPayload = {
              spName: spName,
              actionType: actionType,
              jsonPayload: JSON.stringify(saveData),
            };

            localThis
              .execSPCall(savePayload)
              .then((res: any) => {
                localThis.showNewSnackbar({
                  typeName: "success",
                  text: res.message,
                }); // Feedback to user
                if (formResetFunc) {
                  formResetFunc(); // Reset Form
                }
                //If "new" then load the data
                if (actionType == SqlActionType.NUMBER_0 && res.output) {
                  //localThis.formData.PRIMARY_KEY = res.output;
                  localThis.updateValue(res.output);
                }
              })
              .catch((err: any) => {
                localThis.showNewSnackbar({
                  typeName: "error",
                  text: err.message,
                }); // Feedback to user
              })
              .finally(() => {
                localThis.isSaving = false;
              });
          }
        });
      }
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.hrm.position.biodata-full-name").toString(),
          value: "HR_BIODATA_FULL_NAME",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.department").toString(),
          value: "HR_DEPARTMENT_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.title").toString(),
          value: "HR_POS",
          align: "center",
          divider: true,
        },

        {
          text: this.$i18n.t("datasource.hrm.position.descr").toString(),
          value: "HR_POSITION_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.status").toString(),
          value: "IMS_STATUS_DESCR",
          align: "center",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    getActivityPositions() {
      let localThis: any = this as any;
      localThis.showWait=true;
      let ap = {} as any;
      localThis.selectedPosition = null;
      localThis.positionList = [];
      localThis.positionListOrig = [];

      ap = localThis.$store.getters.getAnnualPlan;
      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_POSITION_HR",
        colList: null,
        //whereCond: `(ACTIVITY_ID is NULL OR ACTIVITY_ID<>${localThis.selectedObjectId} ) AND PROJECT_ID=${ap.annal_plan_id}`,
        whereCond: `OFFICE_FILTER_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID}`,
        //whereCond: `PROJECT_ID = ${ap.annal_plan_id}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null, // "ID"
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let found: boolean = false;
              if (localThis.insertedPositions != undefined) {
                for (let i: number = 0; i < localThis.insertedPositions.length; i++) {
                  let pos_id: number = localThis.insertedPositions[i].HR_POSITION_ID;
                  if (pos_id == item.HR_POSITION_ID) {
                    found = true;
                    break;
                  }
                }
              }
              if (found == false) {
                for (let i: number = 0; i < localThis.positionList.length; i++) {
                  let pos_id: number = localThis.positionList[i].HR_POSITION_ID;
                  if (pos_id == item.HR_POSITION_ID) {
                    found = true;
                    break;
                  }
                }

                if (found == false) {
                  item.HR_POS =
                    item.HR_TITLE_SCOPE_DESCR +
                    " " +
                    item.HR_TITLE_FUNCTION_DESCR +
                    " " +
                    item.HR_TITLE_LEVEL_DESCR;
                  localThis.positionList.push(item);
                }
              }
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        }) .finally(() => {
            localThis.showWait = false;
          });
      
    },
    updateValue(newVal: String) {
      (this as any).$emit("input", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
    /**
     * Get structure of the Position Form
     */
    getFormStruct() {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_POSITION_HR",
        colList: null,
        whereCond: null,
        orderStmt: null,
      };
      localThis.formFields = localThis
        .getJRSTableStructure({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Setup CRUD info
          localThis.position_CRUD = res.crud_info;

          // Setup Form Fields
          let tmpFormFileds: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis.formFields = localThis.setupSelectFields(tmpFormFileds);
        })
        .then((res: any) => {
          localThis.formData.HR_POSITION_STATUS_ID = 13; //Annual Plan Draft
          let ap = {} as any;
          ap = localThis.$store.getters.getAnnualPlan;
          localThis.formData.HR_POSITION_PROJECT_ID = ap.annal_plan_id; // annual Plan in editing
        });
    },
  },
 computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
  },  
  watch: {
    selectedPos(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;
      if (to.length > 0) {
        let msg: string = this.$i18n
          .t("datasource.pms.annual-plan.add-user-position")
          .toString();

        this.$confirm(msg).then((res: any) => {
          if (res) {
            localThis.updateValue(to[0].HR_POSITION_ID);
          }
        });
        //localThis.selectedPos = [];
      }
    },
  },
  mounted() {
    let localThis: any = this as any;

    localThis.setupHeaders();
    localThis.selectedPos = [];

    localThis.getActivityPositions();
    localThis.getFormStruct();
  },
});
</script>
<style scoped>
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
</style>
