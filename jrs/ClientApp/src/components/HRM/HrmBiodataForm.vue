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
        <v-tab-item key="BASIC" v-if="!specifications">
          <!-- EDIT FORM -->
          <jrs-form
            :formData="biodataFormData"
            :formFields="biodataFormFields"
            :nbrOfColumns="3"
            :key="biodataFormData.PRIMARY_KEY"
            v-if="!readonly"
          >
            <template v-slot:form-actions="{ validateFunc }">
              <v-btn
                color="primary"
                :disabled="isSaving"
                @click="
                  saveBiodataFunc(
                    biodataFormData,
                    biodataFormData.PRIMARY_KEY ? 1 : 0,
                    validateFunc
                  )
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

        <!-- BENEFICIARY DATA -->
        <v-tab-item key="BENEFICIARY" v-if="userIsBeneficiary && !specifications">
          <jrs-form
            :formData="beneficiaryFormData"
            :formFields="beneficiaryFormFields"
            :nbrOfColumns="3"
            :key="beneficiaryFormData.PMS_BENEFICIARY_DATA_ID"
            v-if="!readonly && beneficiaryFormFields"
          >
            <template v-slot:form-actions="{ validateFunc, resetFunc }">
              <v-btn
                color="primary"
                :disabled="isSaving"
                @click="
                  saveBeneficiaryFunc(
                    beneficiaryFormData,
                    beneficiaryFormData.PMS_BENEFICIARY_DATA_ID ? 1 : 0,
                    validateFunc
                  )
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
            :detailData="beneficiaryFormData"
            :detailFields="beneficiaryFormFields"
            :nbrOfColumns="1"
            v-else
          ></jrs-readonly-detail>
        </v-tab-item>

        <!-- NATIONALITIES -->
        <v-tab-item key="NATION" v-if="!specifications">
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
                        <v-icon @click="deleteNationality(natIndex)">mdi-close</v-icon>
                      </template>
                    </v-select>
                  </template>
                  <template v-else>
                    <v-list-item-title>{{
                      nations.find(
                        (nation) => nation.IMS_ADMIN_AREA_ID == nat.IMS_ADMIN_AREA_ID
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
              <v-btn color="primary" @click="addNationality()" :disabled="isSaving">{{
                $t("datasource.hrm.biodata.add-nationality")
              }}</v-btn>
              <v-spacer></v-spacer>
              <v-btn color="primary" @click="saveNationality()">{{
                $t("general.save")
              }}</v-btn>
            </div>
          </div>
        </v-tab-item>

        <!-- IDENTIFICATION DOCUMENTS -->
        <v-tab-item key="DOC" v-if="!isDetainedPerson">
          <jrs-simple-table
            ref="table_docs"
            viewName="HR_VI_DOCUMENT_BY_TYPE"
            :dataSourceCondition="docTableOptions.dataSourceCondition"
            :savePayload="docTableOptions.savePayload"
            :fieldDatasourceConditions="[
              {
                field_name: 'HR_DOCUMENT_TYPE_ID',
                field_condition: 'HR_DOCUMENT_CLASS_ID = 1',
              },
            ]"
            :hideNewBtn="readonly"
          ></jrs-simple-table>
        </v-tab-item>

        <!-- CONTACT -->
        <v-tab-item key="CONTACT" v-if="!isDetainedPerson">
          <div>
            <jrs-simple-table
              ref="table_contacts"
              viewName="HR_VI_BIO_CONTACTS"
              :dataSourceCondition="contactTabelOptions.dataSourceCondition"
              :savePayload="contactTabelOptions.savePayload"
              :readonly="readonly"
              class="mb-3"
            ></jrs-simple-table>

            <jrs-simple-table
              viewName="HR_VI_BIO_CONTACTS_EMERGENCY"
              :dataSourceCondition="emergencyContactTableOptions.dataSourceCondition"
              :savePayload="emergencyContactTableOptions.savePayload"
              :readonly="readonly"
            ></jrs-simple-table>
          </div>
        </v-tab-item>

        <!-- EXPERIENCES AND SKILLS -->
        <v-tab-item key="EXPERIENCES_SKILLS" v-if="!isDetainedPerson">
          <div>
            <jrs-simple-table
              viewName="HR_VI_EDUCATION_HISTORY"
              :dataSourceCondition="educationTableOptions.dataSourceCondition"
              :savePayload="educationTableOptions.savePayload"
              :readonly="readonly"
              class="mb-3"
            ></jrs-simple-table>
            <jrs-simple-table
              viewName="HR_VI_WORK_EXPERIENCE"
              :dataSourceCondition="workExperienceTableOptions.dataSourceCondition"
              :savePayload="workExperienceTableOptions.savePayload"
              :readonly="readonly"
              class="mb-3"
            ></jrs-simple-table>
            <jrs-simple-table
              viewName="HR_VI_LANGUAGE_SKILL"
              :dataSourceCondition="languageSkillTableOptions.dataSourceCondition"
              :savePayload="languageSkillTableOptions.savePayload"
              :readonly="readonly"
            ></jrs-simple-table>
          </div>
        </v-tab-item>

        <!-- FAMILY DEFINITION-->
        <v-tab-item key="FAMILY_DEF" v-if="!isDetainedPerson">
          <div class="d-inline-flex ma-3" style="width: 100%">
            <p class="body-1">
              {{ $t("views.view-biodata.family-tab-help") }}
            </p>
            <v-spacer></v-spacer>
            <jrs-modal-form
              formTitle="New"
              formSubtitle="Define item."
              :formFields="
                familyCensusFormFields.filter((field) => field.name != 'BIODATA_ID')
              "
              :formData="newFamilyCensusFormData"
              :nbrOfColumns="3"
              :scrollable="true"
              v-if="!readonly"
            >
              <template v-slot:activation>
                <v-btn color="secondary lighten-2" class="grey--text text--darken-3">
                  <v-icon>mdi-plus-circle-outline</v-icon>New family
                </v-btn>
              </template>
              <template v-slot:modal-form-actions="{ closeModalFunc, validateFunc }">
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="
                    closeModalFunc();
                    newFamilyCensusFormData = {};
                  "
                  >X {{ $t("general.close") }}</v-btn
                >
                <v-btn
                  color="primary"
                  :disabled="isSaving"
                  class="mt-2 mx-1"
                  @click="
                    createUpdateFamily(
                      newFamilyCensusFormData,
                      0,
                      closeModalFunc,
                      validateFunc,
                      'new_fam'
                    )
                  "
                  >Save</v-btn
                >
              </template>
            </jrs-modal-form>
          </div>
          <div>
            <v-expansion-panels dense popout>
              <v-expansion-panel v-for="fam in familyList" :key="fam.FAMILY_ID">
                <v-expansion-panel-header>
                  <v-row>
                    <v-col :cols="2"
                      ><span class="body-1 font-weight-bold"
                        >{{ $t("views.view-biodata.family-code") }}:</span
                      ></v-col
                    >
                    <v-col :cols="4"
                      ><span class="body-1">{{ fam.FAMILY_CODE }}</span></v-col
                    >
                    <v-col :cols="2"
                      ><span class="body-1 font-weight-bold"
                        >{{ $t("views.view-biodata.family-role") }}:</span
                      ></v-col
                    >
                    <v-col :cols="4"
                      ><span class="body-1">{{ fam.roleInFamily }}</span></v-col
                    >
                  </v-row>
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                  <v-tabs
                    v-model="familyTabsModel"
                    centered
                    background-color="primary darken-1"
                    dark
                    class="mb-1"
                  >
                    <v-tab v-for="table in tabsPannelFamily" :key="table.code">{{
                      $t(table.tab_key)
                    }}</v-tab>
                  </v-tabs>
                  <v-tabs-items v-model="familyTabsModel" class="tab-item-wrapper">
                    <v-tab-item key="FAMILY_STRUCTURE_PANNEL">
                      <jrs-simple-table
                        viewName="HR_VI_FAMILY_BY_BIODATA"
                        :title="fam.FAMILY_CODE"
                        :clientPagination="true"
                        :dataSourceCondition="`FAMILY_ID=${fam.FAMILY_ID} AND FILTER_BIODATA_ID=${biodataFormData.PRIMARY_KEY}`"
                        :savePayload="{
                          FAMILY_ID: fam.FAMILY_ID,
                        }"
                        :readonly="readonly"
                      ></jrs-simple-table>
                    </v-tab-item>
                    <v-tab-item key="FAMILY_DOCUMENT_PANNEL">
                      <jrs-simple-table
                        ref="table_docs"
                        viewName="HR_VI_DOCUMENT_BY_TYPE"
                        :dataSourceCondition="`HR_FAMILY_ID = ${
                          fam.FAMILY_ID ? fam.FAMILY_ID : 0
                        }`"
                        :savePayload="{
                          HR_FAMILY_ID: fam.FAMILY_ID,
                          userUid: getUserUid,
                          officeId: getCurrentOffice.HR_OFFICE_ID,
                        }"
                        :fieldDatasourceConditions="[
                          {
                            field_name: 'HR_DOCUMENT_TYPE_ID',
                            field_condition: 'HR_DOCUMENT_CLASS_ID = 3',
                          },
                        ]"
                        :readonly="readonly"
                      ></jrs-simple-table>
                    </v-tab-item>
                  </v-tabs-items>
                </v-expansion-panel-content>
              </v-expansion-panel>
            </v-expansion-panels>
          </div>
        </v-tab-item>
        <!-- SERVICES THAT USER HAS USED 
             WHICH INCLUDE:
             -CASE VISIT 
              OR
             -DISTRIBUTION
         -->
        <v-tab-item key="SERVICES_USED" v-if="hasServicesUsed && !isDetainedPerson">
          <jrs-simple-table
            ref="table_contacts"
            viewName="PMS_VI_CASE_VISIT"
            :dataSourceCondition="caseVisitsTableOptions.dataSourceCondition"
            :readonly="true"
            class="mb-3"
          ></jrs-simple-table>
          <jrs-simple-table
            ref="table_contacts"
            viewName="PMS_VI_DISTRIBUTION"
            :dataSourceCondition="distributionTableOptions.dataSourceCondition"
            :readonly="true"
            class="mb-3"
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
            <template v-slot:form-actions="{ validateFunc, resetFunc }">
              <v-btn
                color="primary"
                :disabled="isSaving"
                @click="
                  //openDialogFunc('specifications',validateFunc)
                  saveSpecification(allSpecificationsData, 0, validateFunc)
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
import JrsSimpleTable from "../JrsSimpleTable.vue";
import JrsReadonyDetail from "../../components/JrsReadonyDetail.vue";

export default Vue.extend({
  props: {
    userUid: {
      type: String,
      required: false,
      default: null,
    },
    // biodataId: {
    //   type: Number,
    //   required: false,
    //   default: null,
    // },
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
  },
  components: {
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadonyDetail,
    "jrs-simple-table": JrsSimpleTable,
    "jrs-modal-form": JrsModalForm,
  },
  mixins: [FormMixin],
  data: () => {
    return {
      biodataId: null,
      formTabsModel: null,
      familyTabsModel: null,
      biodataFormData: {},
      biodataFormFields: [],
      biodataCrudInfo: {},
      beneficiaryFormData: {},
      beneficiaryFormFields: [],
      beneficiaryCrudInfo: {},
      familyCensusFormFields: [],
      newFamilyCensusFormData: {},
      familyList: [],
      nationalities: [],
      beneficiary_CRUD: null,
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
    formTabs() {
      const localThis: any = this as any;
      let allTabs: any[] = [
        {
          code: "BASIC",
          labelKey: "datasource.hrm.biodata.basic-info",
          disable: false,
        },
        {
          code: "NATION",
          labelKey: "datasource.hrm.biodata.nationality",
          disable: false,
        },
        { code: "DOC", labelKey: "datasource.hrm.document.title", disable: false },
        {
          code: "CONTACT",
          labelKey: "datasource.hrm.contact.title-bio",
          disable: false,
        },
        // { code: "CONTACT_EMERGENCY", labelKey: "datasource.hrm.contact-emergency.title-bio"},
        {
          code: "EXPERIENCES_SKILLS",
          labelKey: "datasource.hrm.biodata.experiences-skills",
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

          {
            code: "BENEFICIARY",
            labelKey: "datasource.hrm.biodata.beneficiary",
            disable: false,
          },
          {
            code: "NATION",
            labelKey: "datasource.hrm.biodata.nationality",
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
              code: "SPECIFICATIONS",
              labelKey: "datasource.set.settlement-biodata.tab-specifications",
              disable: false,
            },
          ];
        }
      }

      // Override if new biodata
      if (!localThis.userUid) {
        if (!localThis.isDetainedPerson) {
          allTabs = [
            {
              code: "BASIC",
              labelKey: "datasource.hrm.biodata.basic-info",
              disable: false,
            },
            {
              code: "NATION",
              labelKey: "datasource.hrm.biodata.nationality",
              disable: true,
            },
            { code: "DOC", labelKey: "datasource.hrm.document.title", disable: true },
            {
              code: "CONTACT",
              labelKey: "datasource.hrm.contact.title-bio",
              disable: true,
            },
            // { code: "CONTACT_EMERGENCY", labelKey: "datasource.hrm.contact-emergency.title-bio"},
            {
              code: "EXPERIENCES_SKILLS",
              labelKey: "datasource.hrm.biodata.experiences-skills",
              disable: true,
            },
            {
              code: "FAMILY_DEF",
              labelKey: "datasource.hrm.biodata.tab-family",
              disable: true,
            },
          ];
        } else {
          allTabs = [
            {
              code: "BASIC",
              labelKey: "datasource.hrm.biodata.basic-info",
              disable: false,
            },
            {
              code: "BENEFICIARY",
              labelKey: "datasource.hrm.biodata.beneficiary",
              disable: true,
            },
            {
              code: "NATION",
              labelKey: "datasource.hrm.biodata.nationality",
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
      }
      return allTabs;
    },
  },
  watch: {
    //   biodataFormData(newVal:any){
    //     console.log(`Biodata: ${JSON.stringify(newVal)}`);
    //   },
    specifications(to: any, from: any) {
      let localThis: any = this as any;
      localThis.allSpecificationsData = { ...to };
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
    // /**
    //  * Emit new created biodata
    //  */
    // emitNewBiodata(biodataId:number){
    //   let localThis: any = this as any;
    //   localThis.$emit("newBiodata", biodataId);
    // },
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
        skipNumber: 0,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
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
          localThis[formFieldsPropName] = [...localThis.setupSelectFields(tmpFormFields)];

          // change label of JRS account to email for beneficiary/detained person
          if (
            jrsTableName == "HR_VI_BIODATA" &&
            (localThis.userIsBeneficiary || localThis.isDetainedPerson)
          ) {
            tmpFormFields.map((ele: any) => {
              if (ele.name == "IMS_USER_EMAIL") {
                ele.labelTranslationKey = "datasource.hrm.user.email";
                ele.hintTranslationKey = "";
              }
            });
          }

          // change required field for PMS_SETTLEMENT_I for Deatined Person
          if (jrsTableName == "PMS_VI_BENEFICIARY_DATA" && localThis.isDetainedPerson) {
            for (let i of tmpFormFields) {
              if (i.name == "PMS_SETTLEMENT_ID") {
                i.is_required = true;
                break;
              }
            }
            /*  tmpFormFields.forEach((ele:any)=>{
                     
                      if(ele.name == 'PMS_SETTLEMENT_ID'){
                        ele.is_required = true;
                      }
              })*/
          }

          // Setup form data
          localThis[formDataPropName] =
            res.table_data && res.table_data.length > 0 ? { ...res.table_data[0] } : {};

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
        const beneficiarySavePayload: any = localThis.userIsBeneficiary
          ? { isBeneficiary: true }
          : {};

        //Add external data to payload
        saveData = {
          external_data: {
            ...localThis.savePayload,
            ...beneficiarySavePayload,
          },
          rows: saveData,
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
              localThis.biodataFormData.HR_BIODATA_USER_UID = res.HR_BIODATA_USER_UID;
              localThis.emitNewBiodata(localThis.biodataFormData);
              localThis.formTabs.forEach((e: any) => {
                e.disable = false;
              });
            }

            return res;
          })
          .then((res: any) => {
            const localThis: any = this as any;
            // If "new" then emit values
            if (actionType == SqlActionType.NUMBER_0) {
              localThis.emitNewUser(res.HR_BIODATA_USER_UID);
              localThis.formTabs.forEach((e: any) => {
                e.disable = false;
              });
              // localThis.emitNewBiodata(res.pk_id);
            }
            localThis.setupFormAndTabels();
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
     * Save user and beneficiary data.
     */
    saveBeneficiaryFunc(
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
            ? localThis.beneficiaryCrudInfo.create_sp
            : localThis.beneficiaryCrudInfo.update_sp;
        //Add external data to payload

        saveData = {
          external_data: { ...localThis.savePayload, BeneficiaryUid: localThis.userUid },
          rows: saveData,
        };

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
            //If "new" then load the data
            if (actionType == SqlActionType.NUMBER_0 && res.pk_id) {
              localThis.beneficiaryFormData.PMS_BENEFICIARY_DATA_ID = res.pk_id;
              localThis.beneficiaryFormData.PMS_IMS_USER_UID = res.PMS_IMS_USER_UID;
              /*localThis.emitNewBiodata(res.PMS_IMS_USER_UID);
              //localThis.formTabs.forEach((e:any)=>{
                      //e.disable = false;
              })*/
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
      localThis.nationalities = [...localThis.nationalities, { IMS_ADMIN_AREA_ID: null }];
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
                curr.BIODATA_ID == curr.FILTER_BIODATA_ID ? curr.ROLE_DESCR : null;

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
        let spName: string =
          actionType == SqlActionType.NUMBER_0
            ? localThis.specificationCrudInfo.create_sp
            : localThis.specificationCrudInfo.update_sp;
        //Add external data to payload
        saveData.SET_DET_PERSON_UID = localThis.userUid;
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
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: err.message,
            }); // Feedback to user
          });
      }
    },
    async setupFormAndTabels() {
      const localThis: any = this as any;
      // Load all nations
      localThis.getNations();

      // Setup biodata info and forms structure
      const biodataId: number = localThis.biodataFormData.PRIMARY_KEY || null;
      const userUid: string = localThis.userUid || null;
      await localThis.getFormDataAndStruct(
        "HR_VI_BIODATA",
        "biodataFormData",
        "biodataFormFields",
        "biodataCrudInfo",
        `PRIMARY_KEY = ${biodataId} OR HR_BIODATA_USER_UID = ${
          userUid ? "'" + userUid + "'" : "NULL"
        }`,
        null
      );

      // Set nationality array
      localThis.nationalities = localThis.biodataFormData.HR_BIODATA_NATIONALITY
        ? JSON.parse(localThis.biodataFormData.HR_BIODATA_NATIONALITY)
        : [];

      // Set Beneficiary info and form structure
      await localThis.getFormDataAndStruct(
        "PMS_VI_BENEFICIARY_DATA",
        "beneficiaryFormData",
        "beneficiaryFormFields",
        "beneficiaryCrudInfo",
        `PMS_IMS_USER_UID = ${userUid ? "'" + userUid + "'" : "NULL"}`,
        null
      );

      // Setup family form fields
      await localThis.getFormDataAndStruct(
        "HR_VI_FAMILY_BY_BIODATA",
        null,
        "familyCensusFormFields",
        null,
        null,
        "RELATION_ID,BIODATA_ID,ROLE_ID,START_DATE,END_DATE"
      );

      // Set family info
      localThis.getFamilyInfo();

      // Set identification document table options
      localThis.docTableOptions = {
        dataSourceCondition: `HR_BIODATA_ID = ${
          localThis.biodataFormData.PRIMARY_KEY || 0
        }`,
        savePayload: {
          HR_BIODATA_ID: localThis.biodataFormData.PRIMARY_KEY,
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        },
        fieldDatasourceConditions:
          "[{field_name:'HR_DOCUMENT_TYPE_ID',field_condition:'HR_DOCUMENT_CLASS_ID = 1'}]",
      };

      // Set contact table options
      localThis.contactTabelOptions = {
        dataSourceCondition: `BIODATA_ID = ${localThis.biodataFormData.PRIMARY_KEY || 0}`,
        savePayload: {
          HR_BIODATA_ID: localThis.biodataFormData.PRIMARY_KEY,
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        },
      };

      // Set case visits table options
      localThis.caseVisitsTableOptions = {
        dataSourceCondition: `ID IN (SELECT CASE_VISIT_ID FROM PMS_VI_BENEFICIARY_FOR_PROCESS_INSTANCE WHERE HR_BIODATA_USER_UID  = '${
          localThis.userUid || 0
        }')`,
      };

      localThis.distributionTableOptions = {
        dataSourceCondition: `ID IN (SELECT DISTRIBUTION_ID FROM PMS_VI_BENEFICIARY_FOR_PROCESS_INSTANCE WHERE HR_BIODATA_USER_UID = '${
          localThis.userUid || 0
        }')`,
      };

      // Set emergency contact options
      localThis.emergencyContactTableOptions = {
        dataSourceCondition: `BIODATA_ID = ${localThis.biodataFormData.PRIMARY_KEY || 0}`,
        savePayload: {
          HR_BIODATA_ID: localThis.biodataFormData.PRIMARY_KEY,
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
          isEmergency: true,
        },
      };

      // Set education history table options
      localThis.educationTableOptions = {
        dataSourceCondition: `HR_EDUCATION_HISTORY_BIODATA_ID = ${
          localThis.biodataFormData.PRIMARY_KEY || 0
        }`,
        savePayload: {
          HR_BIODATA_ID: localThis.biodataFormData.PRIMARY_KEY,
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        },
      };

      // Set work experience table options
      localThis.workExperienceTableOptions = {
        dataSourceCondition: `HR_WORK_EXPERIENCE_BIODATA_ID = ${
          localThis.biodataFormData.PRIMARY_KEY || 0
        }`,
        savePayload: {
          HR_BIODATA_ID: localThis.biodataFormData.PRIMARY_KEY,
          userUid: localThis.getUserUid,
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        },
      };

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

      // Set user stories table options
      localThis.userStoriesTableOptions = {
        dataSourceCondition: `FILL_IN_USER = '${localThis.biodataFormData.HR_BIODATA_USER_UID}'`,
        savePayload: {
          fillInUser: localThis.biodataFormData.HR_BIODATA_USER_UID,
        },
        columnList: [
          "STORY_ID",
          // 'FILL_IN_USER',
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

      if (localThis.isDetainedPerson && localThis.userUid) {
        await localThis.getFormDataAndStruct(
          "SET_VI_DETAINED_PERSON_SPECIFICATIONS",
          "allSpecificationsData",
          "specificationsFormFields",
          "specificationCrudInfo",
          `SET_DET_PERSON_UID = '${
            localThis.userUid || 0
          }' AND SET_DET_PERSON_SPEC_DATE =  (select MAX(SET_DET_PERSON_SPEC_DATE) from SET_DETAINED_PERSON_SPECIFICATIONS WHERE SET_DET_PERSON_UID =  '${
            localThis.userUid
          }' )`,
          null
        );
      }
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
