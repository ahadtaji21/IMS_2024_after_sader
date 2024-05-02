<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col :cols="6">
          <div class="graph" >
            <div class="header">
              <h4>Staff List by gender and start date:</h4>
            </div>
            <div class="body">
              <div id="chart">
                <apexchart
                  type="bar"
                  :options="chart1.chartOptions"
                  :series="chart1.series"
                  height="500"
                ></apexchart>
              </div>
            </div>
          </div>
        </v-col>
        <v-col :cols="6">
          <div class="graph" id="graph1">
            <div class="header">
              <h4>Staff List Count:</h4>
            </div>
            <div class="body">
              <div class="circle">
                <div>
                  <i class="mdi mdi-account-tie"> </i>
                </div>
                <span> {{ count }} </span>
                <span class="label"> people</span>
              </div>
            </div>
          </div>

          <div class="graph">
            <div class="header">
              <h4>Staff List by departments:</h4>
            </div>
            <div class="body">
              <div id="chart">
                <apexchart
                  type="pie"
                  :options="chart2.chartOptions"
                  :series="chart2.series"
                  height="500"
                ></apexchart>
              </div>
            </div>
          </div>
        </v-col>
      </v-row>

      <v-row v-if="!detailDialog && !detailsTabOpen">
        <v-col :cols="12">
          <excel-generate-report
            :storedProcedureName="`StaffList`"
            :PROCEDURE_PARAMETERS="PROCEDURE_PARAMETERS"
            :file_name="file_name"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col :cols="12">
          <!-- BIODATA SIMPLE-TABLE -->
          <jrs-simple-table
            v-model="selectedBiodata"
            viewName="HR_VI_BIODATA"
            :dataSourceCondition="`IMS_USER_IS_BENEFICIARY = 0`"
            :nbrOfFormColumns="3"
            :selectable="false"
            :hideNewBtn="true"
            :title="$t('nav.biodata-list')"
            :hideActions="true"
            v-if="selectedBiodata.length == 0"
          >
            <template v-slot:simple-table-header>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="
                  selectedBiodata.push({});
                  disableTab = true;
                "
              >
                <v-icon>mdi-plus-circle-outline</v-icon>New
              </v-btn>
            </template>
            <template
              v-slot:simple-table-item-actions="{
                simpleItemRowItem,
                refreshData,
              }"
            >
              <!-- BTN - OPEN BIODATA FORM -->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn
                    icon
                    @click="
                      detailsTabOpen = true;
                      familyList = [];
                      selectBiodataRow(simpleItemRowItem);
                      refreshData();
                      disableTab = false;
                    "
                    v-bind="attrs"
                    v-on="on"
                  >
                    <v-icon>mdi-circle-edit-outline</v-icon>
                  </v-btn>
                </template>
                <span>{{ $t("general.crud.tooltip-edit-label") }}</span>
              </v-tooltip>

              <!-- BTN - OPEN BIODATA READONLY POPUP -->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn
                    icon
                    @click="showDetail(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                  >
                    <v-icon>mdi-eye-outline</v-icon>
                  </v-btn>
                </template>
                <span>{{ $t("general.crud.tooltip-view-label") }}</span>
              </v-tooltip>

              <!-- BTN - OPEN BIODATA EVENT HISTORY -->
              <!-- <v-tooltip top>
                                    <template v-slot:activator="{ on, attrs }">
                                      <v-btn
                                        icon
                                       @click="showEventHistory(simpleItemRowItem)"
                                        v-bind="attrs"
                                        v-on="on"
                                      >
                                        <v-icon>mdi-playlist-edit</v-icon>
                                      </v-btn>
                                    </template>
                                    <span>{{$t('general.crud.tooltip-history')}}</span>
                                  </v-tooltip> -->
              <!-- BTN - DELETE BIODATA USER -->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn
                    icon
                    @click="
                      toggleDeleteDialogBiodata(simpleItemRowItem, refreshData)
                    "
                    v-bind="attrs"
                    v-on="on"
                    :disabled="isSaving"
                  >
                    <v-icon>mdi-delete</v-icon>
                  </v-btn>
                </template>
                <span>{{ $t("general.crud.tooltip-delete-label") }}</span>
              </v-tooltip>
            </template>
          </jrs-simple-table>

          <div v-if="selectedBiodata.length > 0" class="elevation-1">
            <!-- FORM TOOLBAR -->
            <v-toolbar color="primary darken-1" dark>
              <v-toolbar-title class="text-capitalize">
                {{ $t("nav.staff-biodata-detail") }}
              </v-toolbar-title>
              <v-spacer></v-spacer>
              <v-btn color="secondary" class="ma-2" @click="closeDetail()">
                <v-icon>mdi-table</v-icon>
                {{ $t("datasource.hrm.biodata.btn-to-list") }}
              </v-btn>
              <!-- <v-btn color="secondary" class="ma-2" @click="saveNarrativeData()">
                                    <v-icon>mdi-content-save-all</v-icon>
                                    {{ $t('general.save-all') }}
                                  </v-btn>-->

              <template v-slot:extension>
                <!-- FORM TABS -->
                <v-tabs
                  v-model="formTabsModel"
                  centered
                  background-color="primary darken-1"
                  dark
                >
                  <v-tab key="BASIC">
                    {{ $t("datasource.hrm.biodata.basic-info") }}
                  </v-tab>
                  <v-tab key="NATION" :disabled="disableTab">
                    {{ $t("datasource.hrm.biodata.nationality") }}
                  </v-tab>
                  <v-tab key="DOC" :disabled="disableTab">
                    {{ $t("datasource.hrm.document.title") }}
                  </v-tab>
                  <v-tab key="CONTACT" :disabled="disableTab">
                    {{ $t("datasource.hrm.contact.title-bio") }}
                  </v-tab>

                  <v-tab
                    key="LANGUAGE_SKILLS_EDUCATION_HISTORY_WORK_EXPERIENCE"
                    :disabled="disableTab"
                  >
                    {{ $t("datasource.hrm.biodata.experiences-skills") }}
                  </v-tab>
                  <v-tab v-if="false" key="FAMILY_DEF" :disabled="disableTab">
                    {{ $t("datasource.hrm.biodata.tab-family") }}
                  </v-tab>
                  <v-tab key="AGREEMENT" :disabled="disableTab">
                    {{ $t("datasource.hrm.biodata.tab-agreement-position") }}
                  </v-tab>

                  <v-tab key="POSITION" :disabled="disableTab">
                    {{ $t("datasource.hrm.biodata.tab-position-history") }}
                  </v-tab>

                  <!-- <v-tab
                                                key="BENEFICIARY"
                                                v-if="selectedBeneficiary.length > 0"
                                              >{{ $t('datasource.hrm.biodata.beneficiary') }}
                                              </v-tab> -->
                </v-tabs>
              </template>
            </v-toolbar>

            <!-- FORM PANNELS -->
            <v-tabs-items v-model="formTabsModel" class="tab-item-wrapper">
              <!-- BASIC INFORMATION -->
              <v-tab-item key="BASIC">
                <jrs-form
                  :formData="formDataBiodata"
                  :formFields="formFieldsBiodata"
                  :nbrOfColumns="3"
                  :dynamicFullName="{
                    fullValueFields: formDataBiodata,
                    FULL_NAME_COLUMN_NAME: 'HR_BIODATA_FULL_NAME',
                    NAME_COLUMN_NAME: 'HR_BIODATA_NAME',
                    SURNAME_COLUMN_NAME: 'HR_BIODATA_SURNAME',
                  }"
                >
                  <template v-slot:form-actions="{ validateFunc, resetFunc }">
                    <v-btn
                      color="primary"
                      :disabled="isSaving"
                      @click="
                        saveBiodataFunc(
                          formDataBiodata,
                          formDataBiodata.PRIMARY_KEY ? 1 : 0,
                          validateFunc
                        )
                      "
                      class="ma-2"
                      >{{ $t("general.save") }}</v-btn
                    >
                    <v-btn color="primary" @click="resetFunc()" class="ma-2">
                      {{ $t("general.reset") }}
                    </v-btn>
                  </template>
                </jrs-form>
              </v-tab-item>

              <!-- NATIONALITIES -->
              <v-tab-item key="NATION">
                <v-list>
                  <v-list-item
                    v-for="(nat, index) in nationalities"
                    :key="'NATIONALITY_' + index"
                    dense
                  >
                    <v-select
                      :label="$t('datasource.hrm.biodata.nationality')"
                      v-model="nat.IMS_ADMIN_AREA_ID"
                      :items="nations"
                      item-value="IMS_ADMIN_AREA_ID"
                      item-text="IMS_ADMIN_AREA_NAME"
                      prepend-icon="mdi-flag-checkered"
                      class="text-capitalize"
                    ></v-select>
                    <v-icon @click="deleteNationality(index)">mdi-close</v-icon>
                  </v-list-item>
                </v-list>
                <div class="d-flex felx-column">
                  <v-btn
                    color="primary"
                    @click="nationalities.push({ IMS_ADMIN_AREA_ID: null })"
                    :disabled="isSaving"
                    >{{ $t("datasource.hrm.biodata.add-nationality") }}</v-btn
                  >
                  <v-spacer></v-spacer>
                  <v-btn color="primary" @click="saveNationality()">
                    {{ $t("general.save") }}
                  </v-btn>
                </div>
              </v-tab-item>

              <!-- ID DOCUMENT -->
              <!-- fieldDatasourceConditions = condition that aims to filter the select of Document Type based on the class of Biodata -->
              <v-tab-item key="DOC">
                <jrs-simple-table
                  viewName="HR_VI_DOCUMENT_BY_TYPE"
                  :dataSourceCondition="docConditionBiodata"
                  :savePayload="{
                    HR_BIODATA_ID: selectedBiodata[0].PRIMARY_KEY,
                    userUid: getUserUid,
                    officeId: getCurrentOffice.HR_OFFICE_ID,
                  }"
                  v-if="selectedBiodata.length > 0"
                  :fieldDatasourceConditions="[
                    {
                      field_name: 'HR_DOCUMENT_TYPE_ID',
                      field_condition: `HR_DOCUMENT_CLASS_DESCR = '${docBiodataTag}'`,
                    },
                  ]"
                  ref="table_docs"
                ></jrs-simple-table>
              </v-tab-item>

              <!-- CONTACT -->
              <v-tab-item key="CONTACT">
                <jrs-simple-table
                  viewName="HR_VI_BIO_CONTACTS"
                  :dataSourceCondition="contactCondition"
                  :savePayload="{
                    HR_BIODATA_ID: selectedBiodata[0].PRIMARY_KEY,
                    userUid: getUserUid,
                    officeId: getCurrentOffice.HR_OFFICE_ID,
                  }"
                  v-if="selectedBiodata.length > 0"
                  ref="table_contacts"
                  class="mb-3"
                ></jrs-simple-table>
                <jrs-simple-table
                  viewName="HR_VI_BIO_CONTACTS_EMERGENCY"
                  :dataSourceCondition="contactCondition"
                  :savePayload="{
                    HR_BIODATA_ID: selectedBiodata[0].PRIMARY_KEY,
                    userUid: getUserUid,
                    officeId: getCurrentOffice.HR_OFFICE_ID,
                    isEmergency: true,
                  }"
                  v-if="selectedBiodata.length > 0"
                  ref="table_contacts_emergency"
                ></jrs-simple-table>
              </v-tab-item>

              <!-- LANGUAGE SKILLS -->
              <v-tab-item
                key="LANGUAGE_SKILLS_EDUCATION_HISTORY_WORK_EXPERIENCE"
              >
                <jrs-simple-table
                  viewName="HR_VI_EDUCATION_HISTORY"
                  :dataSourceCondition="educationHistoryCondition"
                  :savePayload="{
                    HR_BIODATA_ID: selectedBiodata[0].PRIMARY_KEY,
                    userUid: getUserUid,
                    officeId: getCurrentOffice.HR_OFFICE_ID,
                  }"
                  v-if="selectedBiodata.length > 0"
                  :nbrOfFormColumns="2"
                  ref="table_education_history"
                  class="mb-3"
                ></jrs-simple-table>
                <jrs-simple-table
                  viewName="HR_VI_WORK_EXPERIENCE"
                  :dataSourceCondition="workExperienceCondition"
                  :savePayload="{
                    HR_BIODATA_ID: selectedBiodata[0].PRIMARY_KEY,
                    userUid: getUserUid,
                    officeId: getCurrentOffice.HR_OFFICE_ID,
                  }"
                  v-if="selectedBiodata.length > 0"
                  ref="table_work_experience"
                  :nbrOfFormColumns="2"
                  class="mb-3"
                ></jrs-simple-table>
                <jrs-simple-table
                  viewName="HR_VI_LANGUAGE_SKILL"
                  :dataSourceCondition="languageSkillCondition"
                  :savePayload="{
                    HR_BIODATA_ID: selectedBiodata[0].PRIMARY_KEY,
                    userUid: getUserUid,
                    officeId: getCurrentOffice.HR_OFFICE_ID,
                  }"
                  v-if="selectedBiodata.length > 0"
                  ref="table_language_skill"
                  :nbrOfFormColumns="2"
                ></jrs-simple-table>
              </v-tab-item>

              <!-- FAMILY DEFINITION-->
              <v-tab-item key="FAMILY_DEF" v-if="false">
                <div class="d-inline-flex ma-3" style="width: 100%">
                  <p class="body-1">
                    {{ $t("views.view-biodata.family-tab-help") }}
                  </p>
                  <v-spacer></v-spacer>
                  <jrs-modal-form
                    formTitle="New"
                    :v-if="familyCensusFormFields"
                    formSubtitle="Define item."
                    :formFields="
                      familyCensusFormFields.filter(
                        (field) => field.name != 'BIODATA_ID'
                      )
                    "
                    :formData="newFamilyCensusFormData"
                    :nbrOfColumns="3"
                    :scrollable="true"
                  >
                    <template v-slot:activation>
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>New family
                      </v-btn>
                    </template>
                    <template
                      v-slot:modal-form-actions="{
                        closeModalFunc,
                        validateFunc,
                      }"
                    >
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
                    <v-expansion-panel
                      v-for="fam in familyList"
                      :key="fam.FAMILY_ID"
                    >
                      <v-expansion-panel-header>
                        <!-- <div class="d-inline-flex">
                                                                  <span class="font-weight-bold">Family code: </span>
                                                                  <span>{{fam.FAMILY_CODE}}</span>
                                                                  <span class="font-weight-bold">Role in family: </span>
                                                                  <span>{{fam.roleInFamily}}</span>
                                                                </div> -->
                        <v-row>
                          <v-col :cols="2">
                            <span class="body-1 font-weight-bold"
                              >{{ $t("views.view-biodata.family-code") }}:</span
                            >
                          </v-col>
                          <v-col :cols="4">
                            <span class="body-1">
                              {{ fam.FAMILY_CODE }}
                            </span>
                          </v-col>
                          <v-col :cols="2">
                            <span class="body-1 font-weight-bold"
                              >{{ $t("views.view-biodata.family-role") }}:</span
                            >
                          </v-col>
                          <v-col :cols="4">
                            <span class="body-1">
                              {{ fam.roleInFamily }}
                            </span>
                          </v-col>
                        </v-row>
                      </v-expansion-panel-header>
                      <v-expansion-panel-content>
                        <v-tabs
                          v-model="tabModelPannel"
                          centered
                          background-color="primary darken-1"
                          dark
                        >
                          <v-tab
                            v-for="table in tabsPannelFamily"
                            :key="table.code"
                          >
                            {{ $t(table.tab_key) }}
                          </v-tab>
                        </v-tabs>
                        <v-tabs-items
                          v-model="tabModelPannel"
                          class="tab-item-wrapper"
                        >
                          <v-tab-item key="FAMILY_STRUCTURE_PANNEL">
                            <jrs-simple-table
                              viewName="HR_VI_FAMILY_BY_BIODATA"
                              :title="fam.FAMILY_CODE"
                              :clientPagination="true"
                              :dataSourceCondition="`FAMILY_ID=${fam.FAMILY_ID} AND FILTER_BIODATA_ID=${selectedBiodata[0].PRIMARY_KEY}`"
                              :savePayload="{ FAMILY_ID: fam.FAMILY_ID }"
                            ></jrs-simple-table>
                          </v-tab-item>
                          <v-tab-item key="FAMILY_DOCUMENT_PANNEL">
                            <jrs-simple-table
                              viewName="HR_VI_DOCUMENT_BY_TYPE"
                              :dataSourceCondition="`HR_FAMILY_ID = ${
                                fam.FAMILY_ID ? fam.FAMILY_ID : 0
                              }`"
                              :savePayload="{
                                HR_FAMILY_ID: fam.FAMILY_ID,
                                userUid: getUserUid,
                                officeId: getCurrentOffice.HR_OFFICE_ID,
                              }"
                              v-if="selectedBiodata.length > 0"
                              :fieldDatasourceConditions="[
                                {
                                  field_name: 'HR_DOCUMENT_TYPE_ID',
                                  field_condition: `HR_DOCUMENT_CLASS_DESCR = '${docFamilyTag}'`,
                                },
                              ]"
                              ref="table_docs"
                            ></jrs-simple-table>
                          </v-tab-item>
                        </v-tabs-items>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                  </v-expansion-panels>
                </div>
              </v-tab-item>

              <v-tab-item key="AGREEMENT">
                <v-row>
                  <v-col cols="12">
                    <jrs-simple-table
                      viewName="HR_VI_AGREEMENT"
                      :selectFirstRowDeafult="true"
                      @getTableDataRecords="getTableDataRecords"
                      v-model="agreement"
                      :selectable="true"
                      :dataSourceCondition="docConditionAgreement"
                      :fieldDatasourceConditions="[
                        {
                          field_name: 'IMS_USER_UID',
                          field_condition: `IMS_USER_UID = '${selectedBiodata[0].HR_BIODATA_USER_UID}'`,
                        },
                      ]"
                      v-if="selectedBiodata.length > 0"
                      ref="table_docs"
                      :ignoreOfficeFilterTotally="true"
                    >
                      <template
                        v-slot:simple-table-item-actions="{ simpleItemRowItem }"
                      >
                        <v-icon
                          v-if="
                            detailFieldsAgreement &&
                            simpleItemRowItem.AMMENDMENT_HISTORY
                          "
                          small
                          @click="openDialogAgreementHistory(simpleItemRowItem)"
                          >mdi-history</v-icon
                        >
                      </template>
                    </jrs-simple-table>
                    <v-divider></v-divider>
                  </v-col>
                </v-row>
                <v-row v-if="agreement.length > 0">
                  <v-col cols="12">
                    <v-row dense class="ml-1">
                      <v-col cols="12">
                        <h4>
                          {{ $t("views.view-position-agreement-hr.title") }}
                        </h4>
                      </v-col>
                    </v-row>
                    <v-row dense class="ml-1">
                      <v-col cols="12">
                        <span>{{ textAgreementPosition }}</span>
                      </v-col>
                    </v-row>
                    <jrs-simple-table
                      v-if="agreement.length > 0"
                      viewName="HR_VI_POSITION_HR"
                      :dataSourceCondition="docConditionPosition"
                      :fieldDatasourceConditions="[
                        {
                          field_name: 'HR_POSITION_AGREEMENT_ID',
                          field_condition: `HR_AGREEMENT_ID=${
                            agreement.length > 0
                              ? agreement[0].HR_AGREEMENT_ID
                              : null
                          }`,
                        },
                        {
                          field_name: 'HR_POSITION_SUPERIOR_POSITION_ID',
                          field_condition: `OFFICE_FILTER_ID = ${
                            getCurrentOffice
                              ? getCurrentOffice.HR_OFFICE_ID
                              : null
                          }`,
                        },
                      ]"
                      ref="table_docs"
                      :ignoreOfficeFilterTotally="true"
                    >
                    </jrs-simple-table>
                  </v-col>
                </v-row>
              </v-tab-item>

              <v-tab-item key="POSITION">
                <jrs-simple-table
                  viewName="HR_VI_POSITION_HR"
                  :dataSourceCondition="docConditionPositionHistory"
                  :hideNewBtn="true"
                  :readonly="true"
                  :ignoreOfficeFilterTotally="true"
                  v-if="selectedBiodata.length > 0"
                  ref="table_docs"
                ></jrs-simple-table>
              </v-tab-item>

              <!-- BENEFICIARY DATA -->
              <!-- <v-tab-item key="BENEFICIARY">
                                     <jrs-form :formData="formDataBeneficiaryData" :formFields="formFieldsBeneficiaryData" :nbrOfColumns="2" :key="formDataBeneficiaryData.PMS_BENEFICIARY_DATA_ID">
                                       <template v-slot:form-actions="{ validateFunc, resetFunc }">
                                         <v-btn
                                          color="primary"
                                          :disabled="isSaving"
                                          @click="saveBeneficiaryFunc(formDataBeneficiaryData, formDataBeneficiaryData.PMS_BENEFICIARY_DATA_ID ? 1 : 0, validateFunc)"
                                          class="ma-2"
                                        >{{ $t('general.save') }}</v-btn>
                                        <v-btn
                                          color="primary"
                                          @click="resetFunc()"
                                          class="ma-2"
                                        >{{ $t('general.reset') }}</v-btn>
                                      </template>
                                    </jrs-form>
                                  </v-tab-item> -->
            </v-tabs-items>
          </div>

          <!-- DETAILS DIALOG -->
          <v-dialog
            v-model="detailDialog"
            persistent
            retain-focus
            :scrollable="true"
            :overlay="false"
            :max-width="'60em'"
            transition="dialog-transition"
          >
            <v-card>
              <v-card-text>
                <v-row>
                  <v-col :cols="12">
                    <h1 class="capital-first-letter">
                      {{ $t("general.detail") }}
                    </h1>
                    <jrs-readonly-detail
                      v-if="formFieldsBiodata"
                      :detailFields="formFieldsBiodata"
                      :detailData="detailData"
                    ></jrs-readonly-detail>
                  </v-col>
                </v-row>
              </v-card-text>
              <v-card-actions>
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="detailDialog = false"
                  >X {{ $t("general.close") }}</v-btn
                >
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!--- DELETE DIALOG FOR BIODATAA-->
          <v-dialog
            v-model="deleteDialogBiodata"
            persistent
            retain-focus
            :overlay="false"
            max-width="45em"
            transition="dialog-transition"
          >
            <v-card>
              <v-card-title primary-title class="text-capitalize">
                {{ $t("general.crud.deletion-user") }}
              </v-card-title>
              <v-card-text class="capital-first-letter">
                {{ $t("general.crud.delete-user-confirm") }}
              </v-card-text>
              <v-card-actions>
                <v-btn
                  text
                  color="secondary darken-1"
                  @click="toggleDeleteDialogBiodata({}, null)"
                  >X {{ $t("general.close") }}</v-btn
                >
                <v-btn
                  color="primary"
                  @click="deleteBiodataUser(deleteItemBiodata)"
                >
                  {{ $t("general.delete") }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!--HISTORY AGREEMENT-->
          <!-- DETAIL DIALOG -->
          <v-dialog
            v-model="dialogAgreementHistory"
            persistent
            retain-focus
            :scrollable="true"
            :overlay="false"
            :max-width="'60em'"
            transition="dialog-transition"
          >
            <v-card>
              <v-card-title>
                <span class="capital-first-letter">
                  {{ $t("general.detail") }}
                </span>
              </v-card-title>
              <v-card-text>
                <v-row>
                  <v-col
                    :cols="3"
                    style="border-right: solid 1px rgba(0, 0, 0, 0.12)"
                  >
                    <v-list>
                      <v-subheader class="text-uppercase">
                        {{ $t("datasource.hrm.agreement.select-version") }}
                      </v-subheader>
                      <v-divider></v-divider>
                      <v-list-item-group
                        v-model="currentVersionAgreement"
                        color="primary"
                      >
                        <v-list-item
                          v-for="(version, vIndex) in versionsAgreement"
                          :key="vIndex"
                        >
                          <v-list-item-content
                            @click="detailDataAgreemen = version.agreement"
                          >
                            Version nbr. {{ version.version_nbr }}
                          </v-list-item-content>
                        </v-list-item>
                      </v-list-item-group>
                    </v-list>
                  </v-col>
                  <v-col :cols="9">
                    <!-- <h1 class="capital-first-letter">{{$t('general.detail')}}</h1> -->
                    <jrs-readonly-detail
                      v-if="
                        detailFieldsAgreement &&
                        currentVersionAgreement != null &&
                        versionsAgreement[currentVersionAgreement]
                      "
                      :detailFields="detailFieldsAgreement"
                      :detailData="
                        versionsAgreement[currentVersionAgreement].agreement
                      "
                    ></jrs-readonly-detail>
                  </v-col>
                </v-row>
              </v-card-text>
              <v-card-actions>
                <v-btn
                  color="secondary darken-1"
                  class="mt-2 mr-1"
                  text
                  @click="dialogAgreementHistory = false"
                  >X {{ $t("general.close") }}</v-btn
                >
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue, { ref } from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import ExcelGenerateReport from "../../components/ExcelGenerateReport.vue";
import VueApexCharts from "vue-apexcharts";

import JrsForm from "../../components/JrsForm.vue";
import JrsModalForm from "../../components/JrsModalForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import JrsReadonyDetail from "../../components/JrsReadonyDetail.vue";
import UtilMix from "../../mixins/UtilMix";
// import JrsField from "../../components/JrsField.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-form": JrsForm,
    "jrs-modal-form": JrsModalForm,
    "jrs-readonly-detail": JrsReadonyDetail,
    "excel-generate-report": ExcelGenerateReport,
    apexchart: VueApexCharts,

    // "jrs-field": JrsField
  },
  mixins: [FormMixin, UtilMix],
  data() {
    return {
      selectedBiodata: [],
      detailFieldsAgreement: [],
      selectedBeneficiary: [],
      listAgreementId: [],
      dialogAgreementHistory: false,
      agreement: [],
      nations: [],
      currentVersionAgreement: 0,
      versionsAgreement: [],
      detailDataAgreement: [],
      nationalities: [],
      formTabsModel: null,
      disableTab: true,
      docConditionBiodata: null,
      educationHistoryCondition: null,
      languageSkillCondition: null,
      docConditionFamily: null,
      contactCondition: null,
      HR_BIODATA_ID: 0,
      deleteDialogBiodata: false,
      deleteItemBiodata: {},
      resfreshBiodata: null,
      formDataBiodata: {},
      formDataBeneficiaryData: {},
      biodata_CRUD: {},
      beneficiary_CRUD: {},
      formFieldsBiodata: [],
      formFieldsBeneficiaryData: [],
      detailDialog: false,
      detailData: {},
      eventHistoryDialog: false,
      eventHistoryInfo: {
        objName: "HR_BIODATA",
        keyName: "HR_BIODATA_ID",
        keyValue: "12",
      },
      familyList: [],
      familyRoles: [],
      tmpRole: null,
      newFamilyCensusFormData: {},
      userDeleted: 0,
      familyCensusFormFields: [],
      isSaving: false,
      tabModelPannel: null,
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
      docBiodataTag: "BIODATA",
      docFamilyTag: "FAMILY",
      PROCEDURE_PARAMETERS: [],
      file_name: "JRS_StaffList_",
      detailsTabOpen: false,

      chart1: {
        chartOptions: {
          chart: {
            type: "bar",
            height: 500,
          },
          plotOptions: {
            bar: {
              horizontal: false,
              columnWidth: "55%",
              endingShape: "rounded",
            },
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            show: true,
            width: 2,
            colors: ["transparent"],
          },
          xaxis: {
            categories: [
              "Feb",
              "Mar",
              "Apr",
              "May",
              "Jun",
              "Jul",
              "Aug",
              "Sep",
              "Oct",
            ],
          },
          yaxis: {
            title: {
              text: "$ (thousands)",
            },
          },
          fill: {
            opacity: 1,
          },
          tooltip: {
            y: {
              formatter: function (val: any) {
                return "$ " + val + " thousands";
              },
            },
          },
        },
        series: [
          {
            name: "Net Profit",
            data: [44, 55, 57, 56, 61, 58, 63, 60, 66],
          },
          {
            name: "Revenue",
            data: [76, 85, 101, 98, 87, 105, 91, 114, 94],
          },
          {
            name: "Free Cash Flow",
            data: [35, 41, 36, 26, 45, 48, 52, 53, 41],
          },
        ],
      },
      chart2: {
        chartOptions: {
          chart: {
            width: 500,
            type: "pie",
          },
          labels: ["Team A", "Team B", "Team C", "Team D", "Team E"],
        },
        series: [44, 55, 13, 43, 22],
      },
      count: 0,
    };
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),
    textAgreementPosition() {
      let localThis: any = this as any;
      if (localThis.agreement.length > 0) {
        let agr = localThis.agreement[0];
        let list: Array<String> = [
          agr.HR_AGREEMENT_TYPE_NAME ? agr.HR_AGREEMENT_TYPE_NAME : null,
          "\n / Date from : ",
          agr.HR_EMPLOYEMENT_DATE_FROM
            ? localThis.formatDate(new Date(agr.HR_EMPLOYEMENT_DATE_FROM))
            : "--/--/----",
          "\n / Date to :",
          agr.HR_EMPLOYEMENT_DATE_TO
            ? localThis.formatDate(new Date(agr.HR_EMPLOYEMENT_DATE_TO))
            : "--/--/----",
          "\n / Date first employment :",
          agr.FIRST_EMPLOYMENT_DATE
            ? localThis.formatDate(new Date(agr.FIRST_EMPLOYMENT_DATE))
            : "--/--/----",
        ];
        return list.join(" ");
      } else {
        return null;
      }
    },
    //Load position
    docConditionPosition() {
      let localThis: any = this as any;
      if (localThis.agreement.length > 0) {
        return `HR_POSITION_AGREEMENT_ID = ${
          localThis.agreement[0].HR_AGREEMENT_ID
            ? localThis.agreement[0].HR_AGREEMENT_ID
            : 0
        }`;
      } else {
        return "";
      }
    },
    docConditionPositionHistory() {
      let localThis: any = this as any;
      //debugger
      if (localThis.agreement.length > 0) {
        return `HR_POSITION_AGREEMENT_ID IN ( ${localThis.listAgreementId.join(
          ","
        )})`;
      } else {
        return `HR_POSITION_AGREEMENT_ID = Null`;
      }
    },
    officeId() {
      let localThis: any = this as any;
      return localThis.getCurrentOffice.HR_OFFICE_ID;
    },
    HR_OFFICE_LEGAL_NAME() {
      let localThis: any = this as any;
      localThis.file_name =
        "JRS_StaffList_" + localThis.getCurrentOffice.HR_OFFICE_LEGAL_NAME;
      return localThis.getCurrentOffice.HR_OFFICE_LEGAL_NAME;
    },
  },
  watch: {
    selectedBiodata(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;

      if (to.length > 0) {
        //Set form data
        localThis.formDataBiodata = to[0];

        // Set nationalities
        if (to[0].HR_BIODATA_NATIONALITY) {
          localThis.nationalities = JSON.parse(to[0].HR_BIODATA_NATIONALITY);
        } else {
          localThis.nationalities = [];
        }

        //Load id documents
        localThis.docConditionBiodata = `HR_BIODATA_ID = ${
          to[0].PRIMARY_KEY ? to[0].PRIMARY_KEY : 0
        }`;

        //Set contacts table condition
        localThis.contactCondition = `BIODATA_ID = ${
          to[0].PRIMARY_KEY ? to[0].PRIMARY_KEY : 0
        }`;
        //Set education history table condition
        localThis.educationHistoryCondition = `HR_EDUCATION_HISTORY_BIODATA_ID = ${
          to[0].PRIMARY_KEY ? to[0].PRIMARY_KEY : 0
        }`;
        //Set work experience table condition
        localThis.workExperienceCondition = `HR_WORK_EXPERIENCE_BIODATA_ID = ${
          to[0].PRIMARY_KEY ? to[0].PRIMARY_KEY : 0
        }`;

        //Set language skills table condition
        localThis.languageSkillCondition = `HR_LANGUAGE_SKILL_BIODATA_ID = ${
          to[0].PRIMARY_KEY ? to[0].PRIMARY_KEY : 0
        }`;

        //Load agreement
        localThis.docConditionAgreement = `IMS_USER_UID = ${
          to[0].HR_BIODATA_USER_UID ? "'" + to[0].HR_BIODATA_USER_UID + "'" : 0
        }`;

        localThis.getBeneficiaryData();

        //Load family information
        localThis.getFamilyInfo();
      }
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
      getConfiguration: "getConfiguration",

      // getFormStruct: "getFormStruct"
    }),
    openDialogAgreementHistory(rowData: any) {
      let localThis: any = this as any;
      localThis.versionsAgreement = JSON.parse(rowData.AMMENDMENT_HISTORY);
      localThis.dialogAgreementHistory = true;
    },

    /**
     * Select biodata row
     */
    selectBiodataRow(row: any) {
      (this as any).selectedBiodata.push(row);
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
        });
    },
    /**
     * Remove nationality from biodata.
     */
    deleteNationality(index: Number) {
      let localThis: any = this as any;
      localThis.nationalities.splice(index, 1);
    },
    /**
     * Save nationalities to biodata.
     */
    saveNationality() {
      let localThis: any = this as any;
      localThis.isSaving = true;
      let saveData: any = {
        HR_BIODATA_USER_UID: localThis.selectedBiodata[0].HR_BIODATA_USER_UID,
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
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        // .then(()=>{
        //     localThis.refreshTable('HR_VI_BIODATA');
        // })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },

    /**
     * Get Data from PMS_VI_BENEFICIARY_DATA given USER_UID
     * **/
    getBeneficiaryData() {
      let localThis: any = this as any;
      let user_uid =
        localThis.selectedBiodata.length > 0 &&
        localThis.selectedBiodata[0].HR_BIODATA_USER_UID
          ? `'${localThis.selectedBiodata[0].HR_BIODATA_USER_UID}'`
          : "null";
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_BENEFICIARY_DATA",
        colList: null,
        whereCond: `PMS_IMS_USER_UID = ${user_uid}`,
        orderStmt: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.item_count != 0) {
            localThis.selectedBeneficiary = [...res.table_data];
            localThis.formDataBeneficiaryData = res.table_data[0];
          } else {
            localThis.selectedBeneficiary = [];
            localThis.formDataBeneficiaryData = {};
          }
        });
    },
    /**
     * Load family information for the current biodata.
     */
    getFamilyInfo() {
      let localThis: any = this as any;
      let biodataId: Number = localThis.selectedBiodata[0].PRIMARY_KEY;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_FAMILY_BY_BIODATA",
        colList: null,
        whereCond: `FILTER_BIODATA_ID = ${biodataId}`,
        orderStmt: null,
      };
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.table_data) {
            let relationList: Array<any> = res.table_data;
            // localThis.familyList =
            let tmp: any = relationList.reduce(
              (list: Array<any>, curr: any) => {
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
              },
              []
            );

            // localThis.familyList = tmp.map( (family:any) => {
            //   family.roleInFamily = family.relations.find( (relation:any) =>{
            //     relation.FILTER_BIODATA_ID == biodataId;
            //   }).ROLE_DESCR
            // });
            localThis.familyList = tmp;
          }
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

      // if(typeOfSave == 'new_fam'){
      // If creating new Family the the biodata is known
      saveData.BIODATA_ID = localThis.selectedBiodata[0].PRIMARY_KEY;
      // }

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
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
      // this.getFamilyInfo();
      // }
    },
    // /**
    //  * Get a list of all family roles for dropdown.
    //  */
    // getFamilyRoles(){
    //   let localThis: any = this as any;
    //   let selectPayload: GenericSqlSelectPayload = {
    //     viewName: "HR_FAMILY_ROLE",
    //     colList: null,
    //     whereCond: null,
    //     orderStmt: null,
    //   };
    //   return localThis.getGenericSelect({ genericSqlPayload: selectPayload })
    //     .then( (res:any) => {
    //       console.log(JSON.stringify(res));
    //     });
    // },

    /**
     * Close Detail section and open list
     */
    closeDetail() {
      let localThis: any = this as any;

      localThis.detailsTabOpen = false;
      // localThis.selectedBiodata.length = 0;
      while (localThis.selectedBiodata.length > 0) {
        localThis.selectedBiodata.pop();
      }
      while (localThis.selectedBeneficiary.length > 0) {
        localThis.selectedBeneficiary.pop();
      }
      localThis.formDataBeneficiaryData = {};
      localThis.newFamilyCensusFormData = {};
      localThis.familyList = [];
      localThis.formTabsModel = [];

      // localThis.refreshTable('HR_VI_BIODATA');
    },
    /**
     * Get structure of the Biodata Form.
     */
    getFormStruct(view: string) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: null,
        orderStmt: null,
      };
      if (view == "HR_VI_BIODATA") {
        localThis
          .getJRSTableStructure({ genericSqlPayload: selectPayload })
          .then((res: any) => {
            // Setup CRUD info
            localThis.biodata_CRUD = res.crud_info;

            // Setup Form Fields
            let tmpFormFileds: Array<any> = res.columns
              .filter((header: any) => !header.hide_in_form)
              .map((field: any) => localThis.parseJrsFormField(field));
            localThis.formFieldsBiodata =
              localThis.setupSelectFields(tmpFormFileds);
          });
      } else if (view == "PMS_VI_BENEFICIARY_DATA") {
        localThis
          .getJRSTableStructure({ genericSqlPayload: selectPayload })
          .then((res: any) => {
            // Setup CRUD info
            localThis.beneficiary_CRUD = res.crud_info;

            // Setup Form Fields
            let tmpFormFileds: Array<any> = res.columns
              .filter(
                (header: any) =>
                  !header.hide_in_form &&
                  header.column_name != "PMS_IMS_USER_UID"
              )
              .map((field: any) => localThis.parseJrsFormField(field));
            localThis.formFieldsBeneficiaryData =
              localThis.setupSelectFields(tmpFormFileds);
          });
      } else if (view == "HR_VI_AGREEMENT") {
        localThis
          .getJRSTableStructure({ genericSqlPayload: selectPayload })
          .then((res: any) => {
            let tmpFormFileds: Array<any> = res.columns
              .filter((header: any) => !header.hide_in_form)
              .map((field: any) => localThis.parseJrsFormField(field));
            localThis.detailFieldsAgreement = localThis.setupSelectFields(
              tmpFormFileds,
              localThis.getCurrentOffice.HR_OFFICE_ID,
              localThis.fieldDatasourceConditions
            );
            return tmpFormFileds;
          });
      }
    },
    /**
     * Save user and biodata info.
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
            ? localThis.biodata_CRUD.create_sp
            : localThis.biodata_CRUD.update_sp;
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
            if (formResetFunc) {
              formResetFunc(); // Reset Form
            }
            //If "new" then load the data
            if (actionType == SqlActionType.NUMBER_0 && res.pk_id) {
              localThis.formDataBiodata.PRIMARY_KEY = res.pk_id;
              localThis.formDataBiodata.HR_BIODATA_USER_UID =
                res.HR_BIODATA_USER_UID;
              localThis.formDataBiodata.HR_BIODATA_REGNUMBER =
                res.HR_BIODATA_REGNUMBER;
              localThis.selectedBiodata.push(localThis.formDataBiodata);
              localThis.disableTab = false;

              // Setup family form fields
              localThis.newFamilyCensusFormData = {};
            }
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
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
            ? localThis.beneficiary_CRUD.create_sp
            : localThis.beneficiary_CRUD.update_sp;
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
              localThis.formDataBeneficiaryData.PMS_BENEFICIARY_DATA_ID =
                res.pk_id;
              localThis.formDataBeneficiaryData.PMS_IMS_USER_UID =
                res.PMS_IMS_USER_UID;
              localThis.selectedBeneficiary.push(
                localThis.formDataBeneficiaryData
              );
            }
          })
          .catch((err: any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
          });
      }
    },
    /**
     * Open detail dialog with the selected data.
     * @param data The data to dispaly in the detail dialog
     */
    showDetail(data: any) {
      let localThis: any = this as any;
      let data_copy = Object.assign({}, data);
      localThis.detailData = data_copy;
      localThis.detailDialog = true;
    },
    /**
     * Open event history dialog with the selected data.
     * @param data The data to dispaly in the detail dialog
     */
    showEventHistory(data: any) {
      let localThis: any = this as any;
      let data_copy = Object.assign({}, data);
      localThis.detailData = data_copy;
      localThis.detailDialog = true;
    },
    /**
             * Open event to eliminate biodata user (in logic way)
             @param saveData The data to delete
             @param actionType The action to delete CRUD
             */

    deleteBiodataUser(saveData: any) {
      let localThis: any = this as any;
      let spName: string = localThis.biodata_CRUD.delete_sp;
      localThis.isSaving = true;
      //Add external data to payload
      if (localThis.savePayload) {
        saveData = {
          external_data: localThis.savePayload,
          rows: saveData,
        };
      }
      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: SqlActionType.NUMBER_2,
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
          });
          localThis.resfreshBiodata();
          localThis.toggleDeleteDialogBiodata();
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },

    toggleDeleteDialogBiodata(deleteItemBiodata?: any, resfreshData?: any) {
      (this as any).deleteDialogBiodata = !(this as any).deleteDialogBiodata;
      (this as any).deleteItemBiodata = deleteItemBiodata
        ? deleteItemBiodata
        : {};
      (this as any).resfreshBiodata = resfreshData;
    },

    getTableDataRecords(records_to: any, records_from: any, refresh: any) {
      let localThis: any = this as any;
      localThis.listAgreementId = [];
      records_to.forEach((element: any) => {
        localThis.listAgreementId.push(element.HR_AGREEMENT_ID);
      });
      if (records_to.length != records_from.length) {
        refresh();
      }
    },

    async getConfFromApi(name: any, body: any, post: any) {
      return new Promise(async (resolve, reject) => {
        try {
          const confData = await this.getConfiguration({ name, body, post });
          resolve(confData);
        } catch (error) {
          console.error("Error fetching configuration data: ", error);
          reject([]);
        }
      });
    },
    async getStatistics() {
      let localthis = this;
      let thedata: any = await localthis.getConfFromApi(
        "HrBiodata/GetStatistics",
        {
          office_id: localthis.officeId,
        },
        true
      );
      if (thedata) {
        let gender_series_years = thedata.hrBiodataStatistics_Gender.map(
          (ele: { year: number }) => ele.year
        );
        gender_series_years = [...new Set(gender_series_years)];
        const gender_series_1 = [];
        const gender_series_2 = [];
        // const gender_series_years = [];
        const GENDER_MALE = "Male";
        const GENDER_FEMALE = "Female";

        for (let i = 0; i < gender_series_years.length; i++) {
          const year = gender_series_years[i];
          const mm = thedata.hrBiodataStatistics_Gender.find(
            (ele: { year: number; gender: string; count: number }) =>
              ele.year == year && ele.gender == GENDER_MALE
          );
          const ff = thedata.hrBiodataStatistics_Gender.find(
            (ele: { year: number; gender: string; count: number }) =>
              ele.year == year && ele.gender == GENDER_FEMALE
          );
          if (mm) {
            gender_series_1.push(mm.count);
          } else {
            gender_series_1.push(0);
          }
          if (ff) {
            gender_series_2.push(ff.count);
          } else {
            gender_series_2.push(0);
          }
        }

        this.chart1 = {
          chartOptions: {
            chart: {
              type: "bar",
              height: 500,
            },
            plotOptions: {
              bar: {
                horizontal: false,
                columnWidth: "55%",
                endingShape: "rounded",
              },
            },
            dataLabels: {
              enabled: false,
            },
            stroke: {
              show: true,
              width: 2,
              colors: ["transparent"],
            },
            xaxis: {
              categories: gender_series_years,
            },
            yaxis: {
              title: {
                text: "people",
              },
            },
            fill: {
              opacity: 1,
            },
            tooltip: {
              y: {
                formatter: function (val: any) {
                  return val + " people";
                },
              },
            },
          },
          series: [
            {
              name: "Male",
              data: gender_series_1,
            },
            {
              name: "Female",
              data: gender_series_2,
            },
          ],
        };

        this.chart2 = {
          chartOptions: {
            chart: {
              width: 500,
              type: "pie",
            },
            labels: thedata.hrBiodataStatistics_Departments.map(
              (ele: { department_name: string }) => ele.department_name
            ),
          },
          series: thedata.hrBiodataStatistics_Departments.map(
            (ele: { count: number }) => ele.count
          ),
        };
        this.count = thedata.hrBiodataStatistics_Count[0].count;
        // this.chartData2 = {
        //   options: {
        //     chart: {
        //       type: "bar",
        //       height: 200,
        //     },
        //     plotOptions: {
        //       bar: {
        //         borderRadius: 1,
        //         horizontal: true,
        //       },
        //     },
        //     dataLabels: {
        //       enabled: false,
        //     },
        //     xaxis: {
        //       categories: thedata.donorStatistics_Country.map(
        //         (ele) => ele.country_name
        //       ),
        //     },
        //   },
        //   series: [
        //     {
        //       data: thedata.donorStatistics_Country.map(
        //         (ele) => ele.donnorsCount
        //       ),
        //     },
        //   ],
        // };

        // console.log("this.chart1 lab: ", this.chartData1.options.labels);
        // console.log("this.chart1 ser: ", this.chartData1.series);

        // console.log(
        //   "this.chartData2 categories: ",
        //   this.chartData2.options.xaxis.categories
        // );
        // console.log("this.chartData2 ser: ", this.chartData2.series);
      }
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getNations();
    /*localThis.hasBeneficiaryData();*/
    localThis.getFormStruct("HR_VI_BIODATA");
    localThis.getFormStruct("PMS_VI_BENEFICIARY_DATA");
    localThis.getFormStruct("HR_VI_AGREEMENT");

    // Setup family form fields
    let familyFormStructPayload: GenericSqlSelectPayload = {
      viewName: "HR_VI_FAMILY_BY_BIODATA",
      colList: "RELATION_ID,BIODATA_ID,ROLE_ID,START_DATE,END_DATE",
      whereCond: null,
      orderStmt: null,
    };
    localThis
      .getJRSTableStructure({ genericSqlPayload: familyFormStructPayload })
      .then((res: any) => {
        // Setup Form Fields
        let tmpFormFileds: Array<any> = res.columns
          .filter((header: any) => !header.hide_in_form)
          .map((field: any) => localThis.parseJrsFormField(field));
        localThis.familyCensusFormFields =
          localThis.setupSelectFields(tmpFormFileds);
      });

    localThis.file_name = "JRS_StaffList__" + localThis.HR_OFFICE_LEGAL_NAME;
    localThis.PROCEDURE_PARAMETERS = [
      {
        id: 1,
        type: "number",
        key: "HR_OFFICE_ID",
        value: localThis.officeId,
        hide: true,
      },

      {
        id: 2,
        type: "date",
        label: "Start date",
        key: "start_date",
        value: new Date(new Date().setMonth(new Date().getMonth() - 1)),
        disabledDates: { from: new Date() },
      },
      {
        id: 3,
        type: "date",
        label: "End date",
        key: "end_date",
        value: new Date(),
        disabledDates: {
          from: new Date(),
        },
      },
    ];
  },
  beforeMount() {
    this.getStatistics();
  },
});
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}

.charts {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}
.graph {
  border: 1px solid #00000033;
  border-radius: 5px;
  min-height: 300px;
  width: 100%;
  margin-right: 25px;
}
.graph .header {
  background-color: aliceblue;
  color: black;
  padding: 8px;
  border: none;
}
.graph .body {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  height: 85%;
}
#graph1 .circle {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  height: 200px;
  width: 200px;
  background-color: #214885;
  border-radius: 50%;
  font-size: 50px;
  color: white;
  font-weight: bold;
  text-align: center;
}

#graph1 .circle .label {
  font-size: 13px !important;
}
</style>
