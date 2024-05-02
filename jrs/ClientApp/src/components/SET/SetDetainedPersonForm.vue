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
        <v-tab-item key="BASIC">
          <!-- EDIT FORM -->
          <jrs-form
            @getTabsInfo="getTabInfo"
            :formData="biodataFormData"
            :formFields="biodataFormFields"
            :nbrOfColumns="actionTab == 'DETAINED_INFORMATION' ? 2 : 3"
            :key="biodataFormData.PRIMARY_KEY"
            :dynamicFullName="{
              fullValueFields: biodataFormData,
              FULL_NAME_COLUMN_NAME: 'HR_BIODATA_FULL_NAME',
              NAME_COLUMN_NAME: 'HR_BIODATA_NAME',
              SURNAME_COLUMN_NAME: 'HR_BIODATA_SURNAME',
            }"
            v-if="!readonly"
          >
            <template v-slot:form-actions-up="{ validateFunc }">
              <!-- <v-alert
                v-if="actionTab != 'BIODATA'"
                outlined
                type="warning"
                prominent
                border="left"
                dense
              >
                <span font-size="70%">{{
                  $t("views.view-settlement-detained-person.confirm-save")
                }}</span>
              </v-alert> -->
              <v-divider></v-divider>

              <v-btn
                color="primary"
                :disabled="isSaving"
                :loading="isSaving"
                @click="
                  showConfirmSaveDetainedPerson = true;
                  validateFuncVar = validateFunc;
                "
                class="ma-2"
                >{{ $t("general.save") }}</v-btn
              >
            </template>
          </jrs-form>
          <!-- READONLY DETAILS -->

          <jrs-readonly-detail
            :detailData="specifications"
            :detailFields="biodataFormFields"
            :detailDataDiff="diffSpecifications"
            :tabNoDiff="[0]"
            :currentTab="actionTab"
            @getTabsInfo="getTabInfo"
            :nbrOfColumns="1"
            :highlightAnswers="true"
            v-else
          ></jrs-readonly-detail>
        </v-tab-item>

        <!-- NATIONALITIES -->
        <!-- <v-tab-item key="NATION" v-if="!specifications">
          <div>
            <v-list>
              <v-list-item
                v-for="(nat, natIndex) in nationalities"
                :key="nat.IMS_ADMIN_AREA_ID"
                dense
              >
                <v-list-item-content id="testtest">
                  <template v-if="!readonly">
                    <v-select
                      v-model="nat.IMS_ADMIN_AREA_ID"
                      :label="$t('datasource.hrm.biodata.nationality')"
                      :items="nations"
                      item-value="IMS_ADMIN_AREA_ID"
                      item-text="IMS_ADMIN_AREA_NAME"
                      prepend-icon="mdi-flag-checkered"
                      class="text-capitalize"
                    >
                      <template v-slot:append-outer>
                        <v-icon @click="deleteNationality(natIndex)"
                          >mdi-close</v-icon
                        >
                      </template>
                    </v-select>
                  </template>
                  <template v-else>
                    <v-list-item-title>{{
                      nations.find(
                        (nation) =>
                          nation.IMS_ADMIN_AREA_ID == nat.IMS_ADMIN_AREA_ID
                      ).IMS_ADMIN_AREA_NAME
                    }}</v-list-item-title>
                    <v-list-item-subtitle>{{
                      $t("views.view-personal-area.nationality")
                    }}</v-list-item-subtitle>
                  </template>
                </v-list-item-content>
              </v-list-item>
            </v-list>
            <div class="d-flex felx-column" v-if="!readonly">
              <v-btn
                color="primary"
                @click="addNationality()"
                :disabled="isSaving"
                >{{ $t("datasource.hrm.biodata.add-nationality") }}</v-btn
              >
              <v-spacer></v-spacer>
              <v-btn color="primary" @click="saveNationality()">{{
                $t("general.save")
              }}</v-btn>
            </div>
          </div>
        </v-tab-item> -->

        <!-- EXPERIENCES AND SKILLS -->
        <v-tab-item key="EXPERIENCES_SKILLS" v-if="!specifications">
          <jrs-simple-table
            viewName="HR_VI_LANGUAGE_SKILL"
            :dataSourceCondition="languageSkillTableOptions.dataSourceCondition"
            :savePayload="languageSkillTableOptions.savePayload"
            :readonly="readonly"
          ></jrs-simple-table>
        </v-tab-item>
        <!-- SPECIFICATIONS DATA (IF IS A DETAINED PERSON)-->
        <v-tab-item key="SPECIFICATIONS" v-if="isDetainedPerson">
          <jrs-form
            :formData="allSpecificationsData"
            :formFields="specificationsFormFields"
            :nbrOfColumns="2"
            :key="allSpecificationsData.SET_DET_PERSON_SPEC_ID"
            v-if="!readonly"
          >
            <template v-slot:form-actions-up="{ validateFunc, resetFunc }">
              <!-- <v-alert outlined type="warning" prominent border="left" dense>
                <span font-size="70%">{{
                  $t("views.view-settlement-detained-person.confirm-save")
                }}</span>
              </v-alert> -->
              <!-- <v-badge bordered color="warning" icon="mdi-exclamation" overlap> -->
              <v-btn
                color="primary"
                :disabled="isSaving"
                :loading="isSaving"
                @click="
                  //openDialogFunc('specifications',validateFunc)
                  showConfirmSaveSpecification = true;
                  validateFuncVar = validateFunc;
                "
                class="ma-2"
                >{{ $t("general.save") }}</v-btn
              >
              <!-- </v-badge> -->
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
            :key="JSON.stringify(allSpecificationsData)"
          ></jrs-readonly-detail>
        </v-tab-item>

        <!-- USER STORIES -->
        <v-tab-item key="USER_STORIES" v-if="showUserStories">
          <jrs-simple-table
            viewName="SET_VI_STORY"
            :dataSourceCondition="userStoriesTableOptions.dataSourceCondition"
            :savePayload="userStoriesTableOptions.savePayload"
            :columnList="userStoriesTableOptions.columnList"
            :nbrOfFormColumns="3"
          ></jrs-simple-table>
        </v-tab-item>
      </v-tabs-items>
    </div>
    <!-- CONFIRM DIALOG -->
    <v-dialog
      v-model="showConfirmSaveDetainedPerson"
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
        <v-card-text>
          {{ $t("general.confirm-submit-text-general") }}
          {{
            $t("views.view-settlement-detained-person.confirm-save")
          }}</v-card-text
        >
        <v-card-actions>
          <v-btn
            text
            color="secondary darken-1"
            @click="showConfirmSaveDetainedPerson = false"
            >X {{ $t("general.cancel") }}</v-btn
          >
          <v-btn
            color="primary"
            @click="
              saveBiodataFunc(
                mergeObject,
                biodataFormData.PRIMARY_KEY ? 1 : 0,
                validateFuncVar
              );
              showConfirmSaveDetainedPerson = false;
            "
            >{{ $t("general.save") }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
    <!-- CONFIRM DIALOG -->
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
        <v-card-text>
          {{ $t("general.confirm-submit-text-general") }}
          {{
            $t("views.view-settlement-detained-person.confirm-save")
          }}</v-card-text
        >
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
              saveSpecification(mergeObject, 0, validateFuncVar);
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
import JrsSimpleTable from "../JrsSimpleTable.vue";
import JrsReadonyDetail from "../../components/JrsReadonyDetail.vue";

export default Vue.extend({
  props: {
    userUid: {
      type: String,
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
    isDetainedPerson: {
      type: Boolean,
      required: false,
      default: false,
    },
    specifications: {
      type: [Object, Array],
      required: false,
    },
    showUserStories: {
      type: Boolean,
      required: false,
      default: false,
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
      biodataId: null,
      saveFunc: null,
      formTabsModel: null,
      familyTabsModel: null,
      biodataFormData: {},
      showConfirmSaveDetainedPerson: false,
      showConfirmSaveSpecification: false,
      biodataFormFields: [],
      validateFuncVar: null,
      biodataCrudInfo: {},
      beneficiaryFormData: {},
      beneficiaryCrudInfo: {},
      familyCensusFormFields: [],
      newFamilyCensusFormData: {},
      familyList: [],
      actionTab: "BIODATA",
      nationalities: [],
      beneficiary_CRUD: null,
      diffSpecifications: {},
      nations: [],
      docTableOptions: {},
      allSpecificationsData: {},
      specificationsFormFields: [],
      contactTabelOptions: {},
      caseVisitsTableOptions: {},
      distributionTableOptions: {},
      emergencyContactTableOptions: {},
      educationTableOptions: {},
      workExperienceTableOptions: {},
      languageSkillTableOptions: {},
      userStoriesTableOptions: {},
      loadingData: false,
      isSaving: false,
      tabsPannelFamily: [
        {
          code: "FAMILY_STRUCTURE_PANNEL",
          tab_key: "datasource.hrm.biodata.tab-family",
        },
        {
          code: "FAMILY_DOCUMENT_PANNEL",
          tab_key: "datasource.hrm.document.title",
        },
      ],
    };
  },
  computed: {
    ...mapGetters(["getUserUid", "getCurrentOffice"]),
    mergeObject() {
      const localThis: any = this as any;
      return {
        ...localThis.allSpecificationsData,
        ...localThis.biodataFormData,
      };
    },
    formTabs() {
      const localThis: any = this as any;
      let allTabs: any[] = [
        {
          code: "BASIC",
          labelKey: "datasource.hrm.biodata.basic-info",
          disable: false,
        },
        // {
        //   code: "NATION",
        //   labelKey: "datasource.hrm.biodata.nationality",
        //   disable: false,
        // },
        {
          code: "DOC",
          labelKey: "datasource.hrm.document.title",
          disable: false,
        },
        {
          code: "CONTACT",
          labelKey: "datasource.hrm.contact.title-bio",
          disable: false,
        },
        {
          code: "EXPERIENCES_SKILLS",
          labelKey: "datasource.hrm.biodata.languages-spoken",
          disable: false,
        },
        {
          code: "FAMILY_DEF",
          labelKey: "datasource.hrm.biodata.tab-family",
          disable: false,
        },
      ];
      // Add beneficiary data
      if (localThis.userIsBeneficiary) {
        allTabs.splice(1, 0, {
          code: "BENEFICIARY",
          labelKey: "datasource.hrm.biodata.beneficiary",
          disable: false,
        });
      }

      // Add case visit data
      if (localThis.hasServicesUsed) {
        allTabs.splice(allTabs.length, 0, {
          code: "SERVICES_USED",
          labelKey: "datasource.hrm.biodata.tab-services-used",
          disable: false,
        });
      }

      // Add user-stories
      if (localThis.showUserStories) {
        allTabs.splice(allTabs.length, 0, {
          code: "USER_STORIES",
          labelKey: "datasource.hrm.biodata.tab-user-stories",
          disable: false,
        });
      }

      if (localThis.isDetainedPerson) {
        allTabs = [
          {
            code: "BASIC",
            labelKey: "datasource.hrm.biodata.basic-info",
            disable: false,
          },

          // {
          //   code: "NATION",
          //   labelKey: "datasource.hrm.biodata.nationality",
          //   disable: false,
          // },
          {
            code: "EXPERIENCES_SKILLS",
            labelKey: "datasource.hrm.biodata.languages-spoken",
            disable: false,
          },
          {
            code: "SPECIFICATIONS",
            labelKey: "datasource.set.settlement-biodata.tab-specifications",
            disable: false,
          },
          {
            code: "USER_STORIES",
            labelKey: "datasource.hrm.biodata.tab-user-stories",
            disable: false,
          },
        ];
      }

      if (localThis.isDetainedPerson && localThis.readonly) {
        {
          allTabs = [
            {
              code: "BASIC",
              labelKey: "datasource.hrm.biodata.basic-info",
              disable: false,
            },
            {
              code: "SPECIFICATIONS",
              labelKey: "datasource.set.settlement-biodata.tab-specifications",
              disable: false,
            },
          ];
        }
      }

      // Override if new biodata
      if (!localThis.userUid) {
        allTabs = [
          {
            code: "BASIC",
            labelKey: "datasource.hrm.biodata.basic-info",
            disable: false,
          },
          // {
          //   code: "NATION",
          //   labelKey: "datasource.hrm.biodata.nationality",
          //   disable: true,
          // },
          {
            code: "EXPERIENCES_SKILLS",
            labelKey: "datasource.hrm.biodata.languages-spoken",
            disable: true,
          },
          {
            code: "SPECIFICATIONS",
            labelKey: "datasource.set.settlement-biodata.tab-specifications",
            disable: true,
          },
          {
            code: "USER_STORIES",
            labelKey: "datasource.hrm.biodata.tab-user-stories",
            disable: true,
          },
        ];
      }
      return allTabs;
    },
  },
  watch: {
    specifications(to: any, from: any) {
      let localThis: any = this as any;
      localThis.allSpecificationsData = { ...to };
    },
    pre_specifications(to: any, from: any) {
      let localThis: any = this as any;
      localThis.diffSpecifications = {};
      if (to && localThis.actionTab!= 'BIODATA') {
        for (const [key, value] of Object.entries(
          localThis.allSpecificationsData
        )) {
          if (to[key] != value || to[key] == value) {
            localThis.diffSpecifications[key] = to[key];
          }
        }
        localThis.diffSpecifications["DATE_PREV"] =
          to["SET_DET_PERSON_SPEC_DATE"];
      }
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

    /**
     * get Info Tab - see JrsForm
     */
    getTabInfo(value: any) {
      let localThis: any = this as any;
      if (
        value.value.tabCode == "LEGAL_PROC" ||
        value.value.tabCode == "FAMILY" ||
        value.value.tabCode == "POST_DET_I"
      ) {
        localThis.actionTab = "DETAINED_INFORMATION";
      } else {
        localThis.actionTab = "BIODATA";
      }      
    },

    /**
     * Emit the new created userUid
     */
    emitNewUser(userUid: string) {
      let localThis: any = this as any;
      localThis.$emit("newUser", userUid);
    },
    emitNewBiodata(biodata: any) {
      let localThis: any = this as any;
      localThis.$emit("newBiodata", biodata);
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
      // if (
      //   !jrsTableName ||
      //   // !formDataPropName ||
      //   !formFieldsPropName ||
      //   !formCrudPropName
      // ) {
      //   // TODO: handle error
      //   return;
      // }
      const localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: jrsTableName,
        colList: colList,
        whereCond: dataCondition,
        orderStmt: null,
        itemNumber: 1,
        skipNumber: 0
       // officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      // Set loading state
      localThis.loadingData = true;

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
          localThis[formFieldsPropName] = [
            ...localThis.setupSelectFields(tmpFormFields),
          ];

          // Setup form data
          localThis[formDataPropName] =
            res.table_data && res.table_data.length > 0
              ? { ...res.table_data[0] }
              : {};

          // Stop loading
          localThis.loadingData = false;

          return res;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    /**
     * Save Biodata info.
     */
    saveBiodataFunc(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      //Check form validity
      if (formValidateFunc()) {
        let localThis: any = this as any;
        localThis.isSaving = true;
        let spName: string =
          actionType == SqlActionType.NUMBER_0
            ? localThis.biodataCrudInfo.create_sp
            : localThis.biodataCrudInfo.update_sp;
        saveData.SET_DET_PERSON_SPEC_ID = 0;
        //Add external data to payload
        saveData = {
          external_data: {
            ...localThis.savePayload,
            ...{ isBeneficiary: true },
          },
          rows: saveData,
          action: localThis.actionTab,
        };
        let savePayload: GenericSqlPayload = {
          spName: spName,
          actionType: actionType,
          jsonPayload: JSON.stringify(saveData),
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        };

        localThis
          .execSPCall(savePayload)
          .then((res: any) => {
            // Feedback to user
            localThis.showNewSnackbar({
              typeName: "success",
              text: res.message,
            });

            // Reset Form
            if (formResetFunc) {
              formResetFunc();
            }

            //If "new" then load the data
            if (actionType == SqlActionType.NUMBER_0 && res.pk_id) {
              localThis.biodataFormData.PRIMARY_KEY = res.pk_id;
              localThis.biodataFormData.HR_BIODATA_USER_UID =
                res.HR_BIODATA_USER_UID;
            }

            return res;
          })
          .then((res: any) => {
            const localThis: any = this as any;
            // If "new" then emit values
            if (actionType == SqlActionType.NUMBER_0) {
              localThis.formTabs.forEach((e: any) => {
                e.disable = false;
              });
              localThis.setupFormAndTabels();
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

    /**
     * Query DB for list of nations.
     */
    getNations() {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_VI_ADMIN_AREA_BY_TYPE",
        colList: "IMS_ADMIN_AREA_ID, IMS_ADMIN_AREA_NAME",
        whereCond: "IMS_ADMIN_AREA_TYPE_CODE = 'COUNT'",
        orderStmt: "IMS_ADMIN_AREA_NAME",
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.nations = [...res.table_data];
          //localThis.nations.push('Stateless') // add Stateless as option
        });
    },
    /**
     * Save nationalities to biodata.
     */
    saveNationality() {
      let localThis: any = this as any;
      localThis.isSaving = true;
      let saveData: any = {
        HR_BIODATA_USER_UID: localThis.biodataFormData.HR_BIODATA_USER_UID,
        HR_BIODATA_NATIONALITY: localThis.nationalities,
      };

      let savePayload: GenericSqlPayload = {
        spName: "HR_SP_UPD_NATIONALITY",
        actionType: SqlActionType.NUMBER_1,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          }); // Feedback to user
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
    },
    /**
     * Remove nationality from biodata.
     */
    deleteNationality(index: number) {
      let localThis: any = this as any;
      localThis.nationalities = localThis.nationalities.filter(
        (item: any, itemIndex: number) => itemIndex != index
      );
    },
    addNationality() {
      let localThis: any = this as any;
      localThis.nationalities = [
        ...localThis.nationalities,
        { IMS_ADMIN_AREA_ID: null },
      ];
    },
    /**
     * Load family information for the current biodata.
     */
    getFamilyInfo(): any {
      let localThis: any = this as any;
      let biodataId: Number = localThis.biodataFormData.PRIMARY_KEY;
      if (!biodataId) {
        return;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_FAMILY_BY_BIODATA",
        colList: null,
        whereCond: `FILTER_BIODATA_ID = ${biodataId}`,
        orderStmt: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          let tmp: any = [];
          if (res.table_data) {
            let relationList: Array<any> = res.table_data;
            tmp = relationList.reduce((list: Array<any>, curr: any) => {
              let currCopy: any = Object.assign({}, curr);
              let role =
                curr.BIODATA_ID == curr.FILTER_BIODATA_ID
                  ? curr.ROLE_DESCR
                  : null;

              delete currCopy.FILTER_BIODATA_ID;

              let family: any = list.find(
                (relation: any) => relation.FAMILY_ID == currCopy.FAMILY_ID
              );
              if (family) {
                family.relations.push(currCopy);
                family.roleInFamily = family.roleInFamily || role;
              } else {
                list.push({
                  FAMILY_ID: curr.FAMILY_ID,
                  FAMILY_CODE: curr.FAMILY_CODE,
                  roleInFamily: role,
                  // TODO: Add info of users role in vamily "if(currCopy.FILTER_BIODATA_ID == biodataId)"
                  relations: [currCopy],
                });
              }
              return list;
            }, []);
          }
          localThis.familyList = tmp;
        });
    },
    createUpdateFamily(
      saveData: any,
      actionType: SqlActionType,
      formClosingFunc: Function,
      formValidateFunc: Function,
      typeOfSave: String
    ) {
      let localThis: any = this as any;
      localThis.isSaving = true;

      saveData.BIODATA_ID = localThis.biodataFormData.PRIMARY_KEY;
      localThis.actionTab = "BIODATA";

      let savePayload: GenericSqlPayload = {
        spName: "HR_SP_INS_UPD_DEL_FAMILY_BIODATA_RELATION",
        actionType: actionType,
        jsonPayload: JSON.stringify(saveData),
        userUid: this.getUserUid,
        officeId: this.getCurrentOffice.HR_OFFICE_ID,
      };

      this.execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: res.status,
            text: res.message,
          }); // Feedback to user
          formClosingFunc(); // Close ModalForm
          localThis.getFamilyInfo(); // reload family info
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
        localThis.isSaving = true;
        let spName: string =
          actionType == SqlActionType.NUMBER_0
            ? localThis.specificationCrudInfo.create_sp
            : localThis.specificationCrudInfo.update_sp;
        //Add external data to payload
        saveData.SET_DET_PERSON_UID = localThis.userUid
          ? localThis.userUid
          : localThis.biodataFormData.HR_BIODATA_USER_UID;
        saveData.SET_DET_PERSON_SPEC_ID = 0;

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
          officeId: this.getCurrentOffice.HR_OFFICE_ID,
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
            localThis.isSaving = false;
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
            localThis.isSaving = false;
          });
      }
    },
    async setupFormAndTabels() {
      const localThis: any = this as any;
      // Load all nations
      localThis.getNations();

      // Setup biodata info and forms structure
      let userUid: string =
        localThis.biodataFormData.HR_BIODATA_USER_UID ||
        localThis.userUid ||
        null;
      await localThis.getFormDataAndStruct(
        "SET_VI_DETAINED_PERSON_BIODATA",
        "biodataFormData",
        "biodataFormFields",
        "biodataCrudInfo",
        `HR_BIODATA_USER_UID = ${userUid ? "'" + userUid + "'" : null}`
      );
      // Set nationality array
      localThis.nationalities = localThis.biodataFormData.HR_BIODATA_NATIONALITY
        ? JSON.parse(localThis.biodataFormData.HR_BIODATA_NATIONALITY)
        : [];

      // Set user stories table options
      localThis.userStoriesTableOptions = {
        dataSourceCondition: `FILL_IN_USER = '${localThis.biodataFormData.HR_BIODATA_USER_UID}'`,
        savePayload: {
          fillInUser: localThis.biodataFormData.HR_BIODATA_USER_UID,
        },
        columnList: [
          "STORY_ID",
          "FILL_IN_USER",
          "FILL_IN_HOUSEHOLD",
          "FILL_IN_USER_NAME",
          "FILL_IN_HOUSEHOLD_NAME",
          "SETTLEMENT_ID",
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
      };

      // If is a deatined person (load data for specifications)

      if (localThis.isDetainedPerson && userUid) {
        await localThis.getFormDataAndStruct(
          "SET_VI_DETAINED_PERSON_SPECIFICATIONS",
          "allSpecificationsData",
          "specificationsFormFields",
          "specificationCrudInfo",
          `SET_DET_PERSON_UID = ${
            userUid ? "'" + userUid + "'" : 0
          } AND SET_DET_PERSON_SPEC_DATE =  (select MAX(SET_DET_PERSON_SPEC_DATE) from SET_DETAINED_PERSON_SPECIFICATIONS WHERE SET_DET_PERSON_UID =  ${
            userUid ? "'" + userUid + "'" : null
          } )`,
          null
        );

                if (localThis.readonly) {
          let localThis: any = this as any;
          localThis.diffSpecifications = {};
          if (localThis.pre_specifications) {
            for (const [key, value] of Object.entries(
              localThis.allSpecificationsData
            )) {
              if (
                localThis.pre_specifications[key] != value ||
                localThis.pre_specifications[key] == value
              ) {
                localThis.diffSpecifications[key] =
                  localThis.pre_specifications[key];
              }
            }
            localThis.diffSpecifications["DATE_PREV"] =
              localThis.pre_specifications["SET_DET_PERSON_SPEC_DATE"];
          }
        }
      
      }

      // Set work experience table options
      localThis.languageSkillTableOptions = {
        dataSourceCondition: `HR_LANGUAGE_SKILL_BIODATA_ID = ${
          localThis.biodataFormData.PRIMARY_KEY || 0
        }`,
        savePayload: {
          HR_BIODATA_ID: localThis.biodataFormData.PRIMARY_KEY,
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        },
      };
    },
  },
  mounted() {
    const localThis: any = this as any;
    localThis.setupFormAndTabels();
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>