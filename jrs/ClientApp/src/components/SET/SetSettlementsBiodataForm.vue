<template>
  <div>
    <div v-if="loadingData" class="text-center">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <div v-if="!loadingData">
      <!-- FORM TOOLBAR -->
      <v-toolbar color="primary darken-1" dark>
        <v-toolbar-title>
          <slot name="form-toolbar-title"></slot>
        </v-toolbar-title>
        <slot name="form-toolbar-content"></slot>
        <template v-slot:extension>
          <v-tabs v-model="formTabsModel" show-arrows>
            <v-tab
              v-for="tabInfo in formTabs"
              :disabled="tabInfo.disable"
              :key="tabInfo.code"
              >{{ $t(tabInfo.labelKey) }}</v-tab
            >
          </v-tabs>
          <slot name="form-toolbar-extension"></slot>
        </template>
      </v-toolbar>

      <!-- FORM PANELS -->

      <v-tabs-items v-model="formTabsModel" class="tab-item-wrapper">
        <v-tab-item key="BASIC" v-if="!specifications && !legalDetails">
          <!-- EDIT FORM -->

          <jrs-form
            :formData="biodataFormData"
            :formFields="biodataFormFields"
            :nbrOfColumns="3"
            :key="biodataFormData.SET_ID"
            v-if="!readonly"
          >
            <template v-slot:form-actions-up="{ validateFunc }">
              <v-btn
                color="primary"
                :disabled="isSaving"
                @click="
                  showConfirmSaveDetentionCenter = true;
                  validateFuncVar = validateFunc;
                "
                class="ma-2"
                >{{ $t("general.save") }}</v-btn
              >
            </template>
          </jrs-form>
          <!-- READONLY DETAILS -->

          <jrs-readonly-detail
            :detailData="biodataFormData"
            :detailFields="biodataFormFields"
            :nbrOfColumns="1"
            v-else
          ></jrs-readonly-detail>
        </v-tab-item>

        <!-- LEGAL -- -->
        <v-tab-item key="LEGAL" v-if="!specifications">
          <v-toolbar>
            <v-tabs v-model="formTabsModelTypeLaw" show-arrows>
              <v-tab v-for="tabInfo in formTabsTypeLaw" :key="tabInfo.code">{{
                tabInfo.labelKey
              }}</v-tab>
            </v-tabs>
          </v-toolbar>

          <v-tabs-items v-model="formTabsModelTypeLaw" class="tab-item-wrapper">
            <v-tab-item v-for="i in formTabsTypeLaw" :key="i.code">
              <!-- READONLY DETAILS -->
              <jrs-readonly-detail
                :detailData="i.data ? i.data : {}"
                :detailFields="legalFormFields"
                :nbrOfColumns="1"
                :key="JSON.stringify(legalDetails)"
              ></jrs-readonly-detail>
            </v-tab-item>
          </v-tabs-items>
        </v-tab-item>

        <!-- SPECIFICATIONS DATA -->
        <v-tab-item key="SPECIFICATIONS">
          <jrs-form
            :disabled="true"
            :formData="allSpecificationsData"
            :formFields="specificationsFormFields"
            :nbrOfColumns="2"
            :key="allSpecificationsData.SET_SPEC_ID"
            v-if="!readonly"
          >
            <template v-slot:form-actions-up="{ validateFunc, resetFunc }">
              <v-btn
                color="primary"
                :disabled="isSaving"
                @click="
                  //openDialogFunc('specifications',validateFunc)
                  showConfirmSaveSpecification = true;
                  validateFuncVar = validateFunc;
                "
                class="ma-2"
                >{{ $t("general.save") }}</v-btn
              >
              <v-btn color="primary" @click="resetFunc()" class="ma-2">{{
                $t("general.reset")
              }}</v-btn>
            </template>
          </jrs-form>
          <!-- READONLY DETAILS -->

          <jrs-readonly-detail
            :detailData="allSpecificationsData"
            :detailFields="specificationsFormFields"
            :detailDataDiff="diffSpecifications"
            :nbrOfColumns="1"
            v-else
          ></jrs-readonly-detail>
        </v-tab-item>

        <!-- USER STORIES -->
        <v-tab-item key="STORIES">
          <jrs-simple-table
            viewName="SET_VI_STORY"
            :dataSourceCondition="`SETTLEMENT_ID = ${
              settlementId ? settlementId : settlementIdVal
            }`"
            :savePayload="{ settlementId: settlementId ? settlementId : settlementIdVal }"
            :hiddenColumns="['SETTLEMENT_ID']"
            :nbrOfFormColumns="3"
            :columnList="storyColumnList"
          ></jrs-simple-table>
        </v-tab-item>
      </v-tabs-items>
    </div>
    <!-- CONFIRM DIALOG BIODATA DETENTION DENTER-->
    <v-dialog
      v-model="showConfirmSaveDetentionCenter"
      persistent
      retain-focus
      :overlay="false"
      max-width="45em"
      transition="dialog-transition"
    >
      <v-card>
        <v-card-title primary-title>
          <span>
            {{ $t("general.confirm-submit-title") }}
          </span>
        </v-card-title>
        <v-card-text> {{ $t("general.confirm-submit-text-general") }}</v-card-text>
        <v-card-actions>
          <v-btn
            text
            color="secondary darken-1"
            @click="showConfirmSaveDetentionCenter = false"
            >X {{ $t("general.cancel") }}</v-btn
          >
          <v-btn
            color="primary"
            @click="
              saveSettlementFunc(
                biodataFormData,
                biodataFormData.SET_ID ? 1 : 0,
                validateFuncVar,
                biodataCrudInfo,
                false
              );
              showConfirmSaveDetentionCenter = false;
            "
            >{{ $t("general.save") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- CONFIRM DIALOG SPECIFICATIONS-->
    <v-dialog
      v-model="showConfirmSaveSpecification"
      persistent
      retain-focus
      :overlay="false"
      max-width="45em"
      transition="dialog-transition"
    >
      <v-card>
        <v-card-title primary-title>
          <span>
            {{ $t("general.confirm-submit-title") }}
          </span>
        </v-card-title>
        <v-card-text> {{ $t("general.confirm-submit-text-general") }}</v-card-text>
        <v-card-actions>
          <v-btn
            text
            color="secondary darken-1"
            @click="showConfirmSaveSpecification = false"
            >X {{ $t("general.cancel") }}</v-btn
          >
          <v-btn
            color="primary"
            @click="
              //openDialogFunc('specifications',validateFunc)
              saveSpecification(allSpecificationsData, 0, validateFuncVar);
              showConfirmSaveSpecification = false;
            "
            >{{ $t("general.save") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import JrsForm from "../JrsForm.vue";
import JrsModalForm from "../JrsModalForm.vue";
import JrsReadonyDetail from "../../components/JrsReadonyDetail.vue";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";

export default Vue.extend({
  props: {
    settlementId: {
      type: Number,
      required: false,
      default: null,
    },
    selectedOffice: {
      type: Number,
      required: false,
      default: null,
    },
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
    userIsBeneficiary: {
      type: Boolean,
      required: false,
      default: false,
    },
    hasServicesUsed: {
      type: Boolean,
      required: false,
      default: false,
    },
    specifications: {
      type: [Object, Array],
      required: false,
    },
    legalDetails: {
      type: [Object, Array],
      required: false,
    },
    pre_specifications: {
      type: [Object, Array],
      required: false,
    },
  },
  components: {
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadonyDetail,
    "jrs-simple-table": JrsSimpleTable,
  },
  mixins: [FormMixin],
  data: () => {
    return {
      formTabsModel: null,
      biodataFormData: {},
      biodataFormFields: [],
      selectedTypeLegal: {},
      legalFormData: [],
      showConfirmSaveDetentionCenter: false,
      showConfirmSaveSpecification: false,
      validateFuncVar: null,
      legalFormDataObject: {},
      openDialog: false,
      titleDialog: "",
      messagewarming:
        "To unlock this section it is necessary to enter and then save the Basic Information (Tab 'BASIC INFORMATION')",
      textDialog: "",
      functionDialog: null,
      legalCrudInfo: {},
      legalFormFields: [],
      biodataCrudInfo: {},
      formTabsModelTypeLaw: null,
      accomodationFormData: {},
      formTabsTypeLaw: [],
      selectedTypeLaw: null,
      allSpecificationsData: {},
      allSpecificationsDataPre: {},
      diffSpecifications: {},
      healtyHygieneFormFields: [],
      specificationsFormFields: [],
      foodFormData: {},
      foodFormFields: [],
      specificationCrudInfo: {},
      loadingData: false,
      isSaving: false,
      storyColumnList: [
        "STORY_ID",
        "FILL_IN_USER",
        "FILL_IN_HOUSEHOLD",
        "FILL_IN_USER_NAME",
        "FILL_IN_HOUSEHOLD_NAME",
        "SETTLEMENT_NAME",
        "TAGS",
        "STORY_TITLE",
        "STORY_TEXT",
        "ATTACHMENT_DOCUMENT_ID",
        "HAS_GIVEN_CONSENT",
        "IS_PRIVATE_DATA",
        "FILL_IN_ASSISTANT_NAME",
        "FILL_IN_DATE",
        "FORMATTED_TAG_LIST",
      ],
      settlementIdVal: null,
    };
  },
  watch: {
    specifications(to: any, from: any) {
      let localThis: any = this as any;
      localThis.allSpecificationsData = { ...to };
    },
    pre_specifications(to: any, from: any) {
      let localThis: any = this as any;
      localThis.diffSpecifications = {};

      if (to) {
        for (const [key, value] of Object.entries(localThis.allSpecificationsData)) {
          localThis.diffSpecifications[key] = to[key];
        }
        localThis.diffSpecifications["DATE_PREV"] = to["SET_SPEC_DATE"];
      }
    },
    legalDetails(to: any, from: any) {
      let localThis: any = this as any;

      localThis.legalFormDataObject = to.filter((ele: any) => {
        return ele.SET_LEGAL_TYPE_ID == localThis.selectedTypeLaw;
      });
    },
    formTabsModelTypeLaw(to: any, from: any) {
      let localThis: any = this as any;
      localThis.selectedTypeLaw = localThis.formTabsTypeLaw[to].primaryKey;
      if (!localThis.readonly) {
        let data = localThis.legalFormData.filter((ele: any) => {
          return ele.SET_LEGAL_TYPE_ID == localThis.selectedTypeLaw;
        })[0];

        localThis.formTabsTypeLaw[to].data = data
          ? data
          : localThis.formTabsTypeLaw[to].data;

        localThis.formTabsTypeLaw[to].data.SET_LEGAL_TYPE_ID = localThis.selectedTypeLaw;
        //let formData = localThis.legalFormData;
        let list: any = [];

        localThis.formTabsTypeLaw.forEach((element: any) => {
          let data = localThis.legalFormData.filter((ele: any) => {
            return ele.SET_LEGAL_TYPE_ID == element.primaryKey;
          })[0];
          data = data ? data : element.data;
          list.push(data);
        });
        localThis.legalFormDataObject = list;
      } else {
        localThis.legalFormDataObject = localThis.legalDetails.filter((ele: any) => {
          return ele.SET_LEGAL_TYPE_ID == localThis.selectedTypeLaw;
        });
      }
    },
  },
  computed: {
    ...mapGetters(["getUserUid", "getCurrentOffice"]),
    formTabs() {
      const localThis: any = this as any;
      let allTabs: any[] = [
        {
          code: "BASIC",
          labelKey: "datasource.set.settlement-biodata.basic-info",
          disable: false,
        },
        {
          code: "LEGAL",
          labelKey: "datasource.set.settlement-legal.title",
          disable: true,
        },
        {
          code: "SPECIFICATIONS",
          labelKey: "datasource.set.settlement-biodata.tab-specifications",
          disable: true,
        },
        {
          code: "STORIES",
          labelKey: "datasource.set.story.title",
          disable: true,
        },
      ];

      // Override if new biodata
      if (!localThis.settlementId) {
        allTabs = [
          {
            code: "BASIC",
            labelKey: "datasource.set.settlement-biodata.basic-info",
            disable: false,
          },
          {
            code: "LEGAL",
            labelKey: "datasource.set.settlement-legal.title",
            disable: true,
          },
          {
            code: "SPECIFICATIONS",
            labelKey: "datasource.set.settlement-biodata.tab-specifications",
            disable: true,
          },
          {
            code: "STORIES",
            labelKey: "datasource.set.story.title",
            disable: true,
          },
        ];
      }
      if (localThis.readonly && localThis.specifications) {
        allTabs = [
          {
            code: "SPECIFICATIONS",
            labelKey: "datasource.set.settlement-biodata.tab-specifications",
            disable: false,
          },
        ];
      }
      if (localThis.readonly && localThis.legalDetails) {
        allTabs = [
          {
            code: "LEGAL",
            labelKey: "datasource.set.settlement-legal.title",
            disable: false,
          },
        ];
      }
      return allTabs;
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getJRSTableStructure: "getJRSTableStructure",
      execSPCall: "execSPCall",
    }),

    /** OPEN DIALOG FOR CONFIRMATION **/
    openDialogFunc(type: any, validateFunc: any) {
      const localThis: any = this as any;
      if (type == "specifications") {
        localThis.titleDialog = localThis.$t(
          "general.crud.add-settlement-specifications"
        );
        localThis.textDialog = localThis.$t(
          "general.crud.add-text-settlement-specifications"
        );
        localThis.functionDialog = localThis.saveSpecification(
          localThis.allSpecificationsData,
          0,
          validateFunc
        );
      }
      if (type == "legal") {
        localThis.titleDialog = localThis.$t("general.crud.add-settlement-legal");
        localThis.textDialog = localThis.$t("general.crud.add-text-settlement-legal");
        localThis.functionDialog = localThis.saveSettlementFunc(
          localThis.legalFormDataObject,
          0,
          validateFunc,
          localThis.legalCrudInfo,
          true
        );
      }
      localThis.openDialog = true;
    },

    /** GET TYPE OF LEGAL **/
    getTypeLegal() {
      const localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_SETTLEMENT_LEGAL_TYPE",
        colList: null,
        orderStmt: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.formTabsTypeLaw = [];

          //Setup data
          if (res.table_data) {
            res.table_data.forEach((element: any) => {
              localThis.formTabsTypeLaw.push({
                code: element.SET_LEGAL_TYPE_CODE,
                labelKey: element.SET_LEGAL_TYPE_NAME,
                primaryKey: element.ID,
                data: {},
              });
            });
            localThis.selectedTypeLaw = localThis.formTabsTypeLaw[0].primaryKey;
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },

    /**
     * Load load form field configuration for a given jrs table
     * @param {string} jrsTableName Name of the JrsTable to load configuration for.
     * @param {string} formDataPropName Name of the property that will hold the form data
     * @param {string} formFieldsPropName Name of the data property that will hold the form field config.
     * @param {string} formCrudPropName Name of the data property that will hold form crud info.
     * @param {string} dataCondition Condition for data retrieval.
     * @param {string} colList List of columns to recover.
     */
    async getFormDataAndStruct(
      jrsTableName: string,
      formDataPropName: string,
      formFieldsPropName: string,
      formCrudPropName: string,
      dataCondition: string,
      colList: string
    ): Promise<any> {
      const localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: jrsTableName,
        colList: colList,
        whereCond: dataCondition,
        orderStmt: null,
        itemNumber: null,
        skipNumber: 0,
        officeId: localThis.selectedOffice, // localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      // Get form structure
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Setup CRUD info
          localThis[formCrudPropName] = { ...res.crud_info };

          // Setup form fields
          let tmpFormFields: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis[formFieldsPropName] = [...localThis.setupSelectFields(tmpFormFields)];
          if (formDataPropName == "legalFormData") {
            res.table_data
              ? res.table_data.forEach((element: any) => {
                  element.SET_LEGAL_ID = 0;
                  localThis[formDataPropName].push(element);
                })
              : [];
            localThis.legalFormDataObject = localThis[formDataPropName];
          } else {
            // Setup form data
            localThis[formDataPropName] =
              res.table_data && res.table_data.length > 0 ? { ...res.table_data[0] } : {};
          }

          return res;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    saveSettlementFunc(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      crud: any,
      legal: boolean,
      formResetFunc?: Function
    ) {
      //Check form validity
      if (formValidateFunc()) {
        let localThis: any = this as any;
        localThis.isSaving = true;
        let spName: string =
          actionType == SqlActionType.NUMBER_0 ? crud.create_sp : crud.update_sp;

        //Add external data to payload
        if (legal) {
          saveData.forEach((element: any) => {
            element.SET_SETTLEMENT_ID = localThis.settlementId;
          });
        }
        saveData = {
          external_data: {
            ...localThis.savePayload,
          },
          rows: saveData,
        };

        let savePayload: GenericSqlPayload = {
          spName: spName,
          actionType: actionType,
          jsonPayload: JSON.stringify(saveData),
          userUid: localThis.getUserUid,
          officeId: localThis.selectedOffice, // localThis.getCurrentOffice.HR_OFFICE_ID,
        };

        localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            // Feedback to user
            if (!localThis.settlementId) {
              localThis.settlementIdVal = res.pk_id;
              localThis.formTabs.forEach((e: any) => {
                e.disable = false;
              });
            }
            localThis.showNewSnackbar({
              typeName: "success",
              text: res.message,
            });

            // Reset Form
            if (formResetFunc) {
              formResetFunc();
            }
          })
          .catch((err: any) => {
            // Feedback to user
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            });
          })
          .finally(() => {
            localThis.isSaving = false;
          });
      }
    },

    /*
     * Save Settlements .
     */
    saveSpecification(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      //Check form validity
      if (formValidateFunc()) {
        let localThis: any = this as any;
        let spName: string =
          actionType == SqlActionType.NUMBER_0
            ? localThis.specificationCrudInfo.create_sp
            : localThis.specificationCrudInfo.update_sp;
        //Add external data to payload

        saveData.SET_SPEC_SETTLEMENT_ID = localThis.settlementId
          ? localThis.settlementId
          : localThis.settlementIdVal;
        saveData.SET_SPEC_ID = 0;

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
          userUid: this.getUserUid,
          officeId: localThis.selectedOffice, // this.getCurrentOffice.HR_OFFICE_ID,
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
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
          });
      }
    },

    /**
     * Setup forms and tables structures.
     */
    async setupFormAndTabels() {
      const localThis: any = this as any;
      localThis.loadingData = true;

      // Setup biodata info and forms structure
      const SettlementId: string = localThis.settlementId || null;

      // BIODATA SETTLEMENT GET DATA
      localThis.loadingData = true;

      await localThis.getFormDataAndStruct(
        "SET_VI_SETTLEMENTS_BIODATA",
        "biodataFormData",
        "biodataFormFields",
        "biodataCrudInfo",
        `SET_ID = ${SettlementId}`,
        null
      );

      // LEGAL GET DATA

      await localThis.getFormDataAndStruct(
        "SET_VI_SETTLEMENTS_LEGAL",
        "legalFormData",
        "legalFormFields",
        "legalCrudInfo",
        // `SET_SETTLEMENT_ID = ${SettlementId} AND SET_LEGAL_DATE =  (select MAX(SET_LEGAL_DATE) from SET_SETTLEMENT_LEGAL WHERE SET_SETTLEMENT_ID =  ${SettlementId} )`,
        `SET_SETTLEMENT_ID = ${SettlementId} AND IS_LATEST_VERSION = 1`,
        null
      );

      // ACCOMODATION GET DATA

      await localThis.getFormDataAndStruct(
        "SET_VI_SETTLEMENTS_SPECIFICATIONS",
        "allSpecificationsData",
        "specificationsFormFields",
        "specificationCrudInfo",
        `SET_SPEC_SETTLEMENT_ID = ${SettlementId} AND SET_SPEC_DATE =  (select MAX(SET_SPEC_DATE) from SET_SETTLEMENT_SPECIFICATIONS WHERE SET_SPEC_SETTLEMENT_ID =  ${SettlementId} )`,
        null
      );

      // ACCOMODATION GET DATA

      localThis.loadingData = false;
    },
  },
  mounted() {
    const localThis: any = this as any;

    localThis.setupFormAndTabels();
    localThis.getTypeLegal();

    if (localThis.settlementId) {
      localThis.settlementIdVal = localThis.settlementId;
      localThis.formTabs.forEach((e: any) => {
        e.disable = false;
      });
    }

    if (localThis.readonly) {
      localThis.allSpecificationsData = { ...localThis.specifications };
      localThis.diffSpecifications = {};
      if (localThis.pre_specifications) {
        for (const [key, value] of Object.entries(localThis.allSpecificationsData)) {
          if (
            localThis.pre_specifications[key] != value ||
            localThis.pre_specifications[key] == value
          ) {
            localThis.diffSpecifications[key] = localThis.pre_specifications[key];
          }
        }
        localThis.diffSpecifications["DATE_PREV"] =
          localThis.pre_specifications["SET_SPEC_DATE"];
      }
    }
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>
