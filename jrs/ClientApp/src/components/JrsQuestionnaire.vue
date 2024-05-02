<template>
  <div>
    <jrs-form
      :formFields="fields"
      :formData="formData"
      @getTabsInfo="getTabInfo"
      :nbrOfColumns="1"
      :currentTabIndex="nextTabIndex"
      v-if="!readonly"
      ref="questionnaireForm"
    >
      <template v-slot:form-header>
        <h3>
          {{ questionnaireStruct.title }} |
          {{ questionnaireStruct.code }}
        </h3>
        <v-divider class="mt-3"></v-divider>
      </template>
      <template v-slot:form-actions="">
        <v-row class="mt-2 mr-1">
          <v-btn
            color="primary"
            @click="previousTabQuestionaire()"
            v-if="currentTab ? (currentTab.index != 0 ? true : false) : false"
            >{{ $t("general.back") }}</v-btn
          >
          <v-spacer></v-spacer>
          <v-btn
            color="primary"
            class="mt-2 mr-1"
            @click="activateConfirmDialog()"
            :disabled="disableSubmit"
            v-if="hideSubmitBtn"
            >{{ $t("general.submit") }}</v-btn
          >
          <v-btn v-else color="primary" @click="nextTabQuestionaire()">{{
            $t("general.next")
          }}</v-btn>
        </v-row>
      </template>
    </jrs-form>
    <jrs-readonly-detail
      :detailFields="fields"
      :detailData="formData"
      :nbrOfColumns="1"
      v-if="readonly"
    ></jrs-readonly-detail>

    <!-- CONFIRM DIALOG -->
    <v-dialog
      v-model="showConfirmDialog"
      persistent
      retain-focus
      :overlay="false"
      max-width="45em"
      transition="dialog-transition"
    >
      <v-card>
        <v-card-title primary-title>
          {{ $t("general.confirm-submit-text") }}
        </v-card-title>
        <v-card-text><b></b></v-card-text>
        <v-card-actions>
          <v-btn text color="secondary darken-1" @click="showConfirmDialog = false"
            >X {{ $t("general.cancel") }}</v-btn
          >
          <v-btn color="primary" @click="submitQuestionnaire()">{{
            $t("general.submit")
          }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import FormMixin from "../mixins/FormMixin";
import {
  GenericSqlPayload,
  ImsAnswerInstance,
  ImsQuestion,
  ImsQuestionnaire,
  ImsQuestionnaireInstance,
  ImsQuestionnaireQuestion,
  SqlActionType,
} from "../axiosapi";
import {
  fieldType,
  JrsFormField,
  JrsFormFieldSearch,
  JrsFormFieldSearchMulti,
  JrsFormFieldSelect,
} from "../models/JrsForm";
import JrsForm from "../components/JrsForm.vue";
import JrsReadonyDetail from "../components/JrsReadonyDetail.vue";

export default Vue.extend({
  props: {
    questionnaireId: {
      type: Number,
      required: true,
    },
    questionnaireData: {
      type: Object as () => ImsQuestionnaireInstance,
      required: false,
      default: () => ({}),
    },
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  components: {
    "jrs-form": JrsForm,
    "jrs-readonly-detail": JrsReadonyDetail,
  },
  mixins: [FormMixin],
  data: () => ({
    dataReady: false,
    formData:{},
    fields: [],
    questionnaireStruct: {},
    wasSubmitted: false,
    showConfirmDialog: false,
    hideSubmitBtn: false,
    currentTab: null,
    nextTabIndex: 0,
  }),
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),

    formData1() {
      const localThis: any = this as any;
      const data: ImsQuestionnaireInstance = localThis.questionnaireData as ImsQuestionnaireInstance;
      let retObject: any = {};
      // TODO: Restructure questionnaireData to be a simple object for JrsForm;
      if (data && data.answerInstanceList) {
        retObject = data.answerInstanceList.reduce(
          (newObj: any, ans: ImsAnswerInstance) => {
            newObj[`FIELD_${ans.questionId}`] =
              ans.answerText ||
              ans.answerDate ||
              ans.answerCheckbox ||
              ans.answerNumber ||
              ans.answerOptionId ||
              ans.answerMatrixOptionId;
            return newObj;
          },
          {}
        );
        // Add Beneficiary, Household and JrsStaff information
        retObject.BENEFICIARY_SELECTOR = data.fillInUser;
        retObject.HOUSEHOLD_SELECTOR = data.fillInHousehold;
        retObject.FILL_IN_ASSISTANT = data.fillInAssistant;
        retObject.ANNUAL_PLAN_SELECTOR = data.selectedProject;
        retObject.SETTLEMENT_SELECTOR = data.selectedSettlement;
      }

      return retObject;
    },
    disableSubmit() {
      const localThis: any = this as any;
      return localThis.readonly || localThis.wasSubmitted;
    },
  },
  watch: {
    currentTab(value) {
      const localThis: any = this as any;
      localThis.nextTabIndex = value.index;
    },
  },
  methods: {
    ...mapActions(["showNewSnackbar"]),
    ...mapActions("apiHandler", [
      "getQuestionnaireStructure",
      "getQuestionnaire",
      "execSPCall",
    ]),
    nextTabQuestionaire() {
      const localThis: any = this as any;
      if (localThis.currentTab) {
        localThis.currentTab.index = localThis.currentTab.index + 1;
        localThis.getTabInfo(localThis.currentTab);
        localThis.nextTabIndex = localThis.currentTab ? localThis.currentTab.index : 1;
      } else {
        localThis.currentTab = 0;
        localThis.nextTabIndex = 1;
      }
    },
    previousTabQuestionaire() {
      const localThis: any = this as any;
      if (localThis.currentTab) {
        localThis.currentTab.index = localThis.currentTab.index - 1;
        localThis.getTabInfo(localThis.currentTab);
        localThis.nextTabIndex = localThis.currentTab ? localThis.currentTab.index : 1;
      } else {
        localThis.currentTab = 0;
        localThis.nextTabIndex = 1;
      }
    },
    getTabInfo(currentTab: any) {
      const localThis: any = this as any;
      if (currentTab.index == currentTab.maxIndex) {
        localThis.hideSubmitBtn = true;
      } else {
        localThis.hideSubmitBtn = false;
      }
      //TabIndex = currentTab.index;
      localThis.currentTab = currentTab;
    },
    getFieldType(typeCode: string | null | undefined): fieldType {
      typeCode = typeCode?.toLowerCase() || "";
      if (typeCode && typeCode in fieldType) {
        return fieldType[typeCode as fieldType];
      }
      return fieldType.text;
    },
    async getQuestionnaireStruct(officeId: number, questionnaireId: number) {
      const localThis: any = this as any;
      try {
        const questionnaire: ImsQuestionnaire =
          // await localThis.getQuestionnaireStructure({
          //     officeId,
          //     questionnaireId,
          // });
          await localThis.getQuestionnaire({
            officeId,
            questionnaireId,
          });
        //-------------------------------------------
        // Alberto:Patch numerazione dei questionari
        //-------------------------------------------
        let i: number = 0;
        let orderList: Array<any> = [];

        if (questionnaire.questionnaireQuestionList != undefined) {
          for (i = 0; i < questionnaire.questionnaireQuestionList.length; i++) {
            if (
              questionnaire.questionnaireQuestionList[i] != undefined &&
              questionnaire.questionnaireQuestionList[i].question != undefined
            ) {
              let tabOrder: number = 0;
              if (questionnaire.questionnaireTabList != undefined) {
                let y: number = 0;
                for (y = 0; y < questionnaire.questionnaireTabList.length; y++) {
                  if (
                    questionnaire.questionnaireTabList[y].id ==
                    questionnaire.questionnaireQuestionList[i].tabId
                  ) {
                    tabOrder = questionnaire.questionnaireTabList[y].ordinalPosition || 0;
                    break;
                  }
                }
              }

              let q: any = questionnaire.questionnaireQuestionList[i];
              // orderList.push(q.ordinalPosition || 0) + (q.tabId || 1) * 1000000;

              orderList.push(q.ordinalPosition + tabOrder * 1000000);
            }
          }
        }
        var orderedIdx: Array<any> = [];
        for (i = 0; i < orderList.length; i++) {
          orderedIdx.push({ name: orderList[i], idx: i });
        }

        orderedIdx.sort(function (a, b) {
          // a & b are the array items (info objects) created above, so make 'em
          // point at the `name` property we want to sort by
          a = a.name;
          b = b.name;
          // ... then return -1/0/1 to reflect relative ordering
          return a < b ? -1 : a > b ? 1 : 0;
        });
        if (questionnaire.questionnaireQuestionList != undefined) {
          let x: any = questionnaire.questionnaireQuestionList.length;
          let numList = Array(x);

          for (i = 0; i < questionnaire.questionnaireQuestionList.length; i++) {
            if (
              questionnaire.questionnaireQuestionList[i] != undefined &&
              questionnaire.questionnaireQuestionList[i].question != undefined
            ) {
              let actualIdx: number = 0;

              let tabOrder: number = 0;
              if (questionnaire.questionnaireTabList != undefined) {
                let y: number = 0;
                for (y = 0; y < questionnaire.questionnaireTabList.length; y++) {
                  if (
                    questionnaire.questionnaireTabList[y].id ==
                    questionnaire.questionnaireQuestionList[i].tabId
                  ) {
                    tabOrder = questionnaire.questionnaireTabList[y].ordinalPosition || 0;
                    break;
                  }
                }
              }

              let qq: any = questionnaire.questionnaireQuestionList[i];

              actualIdx = qq.ordinalPosition + tabOrder * 1000000;

              //   actualIdx =
              //     (qq.ordinalPosition || 0) +
              //     (qq.questionnaireTab?.ordinalPosition || 1) * 1000000;
              let k: number = 0;

              for (k = 0; k < orderedIdx.length; k++) {
                if (orderedIdx[k].name == actualIdx) {
                  let q: any = questionnaire.questionnaireQuestionList[i].question;
                  q.questionText = (k + 1).toString() + ") " + q.questionText;
                  questionnaire.questionnaireQuestionList[i].question = q;
                }
              }
            }
          }
        }
        //-------------------------------------------
        //Alberto: Fine Patch numerazione questionari
        //-------------------------------------------
        localThis.questionnaireStruct = questionnaire;

        // Map JrsFields
        // localThis.fields = questionnaire.questionInfos.map(
        const qqList: ImsQuestionnaireQuestion[] =
          questionnaire.questionnaireQuestionList || [];
        localThis.fields = qqList.map((qq: ImsQuestionnaireQuestion) => {
          // if(qq.question){
          const question: ImsQuestion = qq.question as ImsQuestion;

          // Get Label, Hint and Field Type
          const label: string = question.questionText || "Error_Missing_Label";
          const hint: string | undefined = question.questionHint || undefined;
          const type: fieldType = localThis.getFieldType(question.formFieldType?.code);
          const tabCode: string | undefined = qq.questionnaireTab?.code || undefined;
          const tabLabel: string | undefined = qq.questionnaireTab?.descr || undefined;
          const tabOrder: number | undefined = qq.questionnaireTab?.ordinalPosition || 1;
          const formOrder: number = (qq.ordinalPosition || 0) + 1000000 * tabOrder;

          // Create field
          const newField: JrsFormField = {
            name: `FIELD_${question.id}`,
            label: label,
            hint: hint,
            type: type,
            form_order: formOrder,
            is_required: qq.isRequired,
            tabCode: tabCode,
            tabLabel: tabLabel,
          };
          // Update select-like fields
          const selectFieldTypes: fieldType[] = [fieldType.select, fieldType.radio];
          const selectFieldTypesMatrix: fieldType[] = [fieldType.radio_matrix];
          if (selectFieldTypes.includes(type)) {
            const newSelectField: JrsFormFieldSelect = {
              ...newField,
              selectItems: question.questionAnswerOptionsList || [],
              lookupName: undefined,
              itemKey: "id",
              itemText: "answerText",
              dataSource: "UTIL_VI_QUESTION_ANSWER_OPTIONS_RESOLVER",
            };
            return newSelectField;
          } else {
            if (selectFieldTypesMatrix.includes(type)) {
              const newSelectField: JrsFormFieldSelect = {
                ...newField,
                selectItems: question.questionAnswerOptionsList || [],
                lookupName: undefined,
                itemKey: "id",
                itemText: "answerText",
                dataSource: "UTIL_VI_QUESTION_ANSWER_MATRIX_OPTIONS_RESOLVER",
              };
              return newSelectField;
            }
          }
          return newField;
          // }
        });

        // Add beneficiary selection field
        if (questionnaire.includeBeneficiarySelection) {
          const beneficField: JrsFormFieldSearch = {
            name: "BENEFICIARY_SELECTOR",
            label: localThis.$t("views.view-questionnaire.select-beneficiary").toString(),
            hint: undefined,
            type: fieldType["search_table"],
            form_order: -10,
            dataSource: "HR_VI_USER_SELECTOR",
            dataSourceCondition: "isBeneficiary = 1",
            itemKey: "IMS_USER_UID",
            itemText: "HR_BIODATA_FULL_NAME",
            is_required: false,
          };
          localThis.fields.push(beneficField);
        }

        // Add household selection field
        if (questionnaire.includeHouseholdSelection) {
          const householdField: JrsFormFieldSearch = {
            name: "HOUSEHOLD_SELECTOR",
            label: localThis.$t("views.view-questionnaire.select-household").toString(),
            hint: undefined,
            type: fieldType["search_table"],
            form_order: -9,
            dataSource: "HR_VI_HOUSEHOLD_SELECTOR",
            itemKey: "HR_HOUSEHOLD_UID",
            itemText: "HOUSEHOLD_NAME",
            is_required: false,
          };
          localThis.fields.push(householdField);
        }

        // Add annual plan selection field
        if (questionnaire.includeProjectSelection) {
          const projectField: JrsFormFieldSearch = {
            name: "ANNUAL_PLAN_SELECTOR",
            label: localThis.$t("views.view-questionnaire.select-project").toString(),
            hint: undefined,
            type: fieldType["search_table"],
            form_order: -8,
            dataSource: "PMS_VI_PROJECT",
            itemKey: "ID",
            itemText: "PROJECT_APCODE",
          };
          localThis.fields.push(projectField);
        }

        // Add settlement selection field
        if (questionnaire.includeProjectSelection) {
          const settlementField: JrsFormFieldSearch = {
            name: "SETTLEMENT_SELECTOR",
            label: localThis.$t("datasource.set.settlement-biodata.title").toString(),
            hint: undefined,
            type: fieldType["search_table"],
            form_order: -7,
            dataSource: "SET_VI_SETTLEMENT_SELECTOR",
            itemKey: "SETTLEMENT_ID",
            itemText: "SETTLEMENT_CODE",
          };
          localThis.fields.push(settlementField);
        }
        localThis.fields = localThis.fields.map((field: any) => {
          field.rules = [...localThis.getFieldRules(field)];
          return field;
        });
      } catch (err) {
        localThis.showNewSnackbar({ typeName: "error", text: err });
        console.log(err);
      }
      //return "";
    },
    activateConfirmDialog() {
      const localThis: any = this as any;
      //Check form validity
      if (!localThis.$refs.questionnaireForm.validateForm()) {
        return;
      }
      localThis.showConfirmDialog = true;
    },
    submitQuestionnaire() {
      const localThis: any = this as any;

      // Disable further submission
      localThis.wasSubmitted = true;

      // Create Answer objects
      // const questionAnserList: any =
      // localThis.questionnaireStruct.questionInfos.map(
      //     (questionInfo: any) => {
      const qqList: ImsQuestionnaireQuestion[] =
        localThis.questionnaireStruct.questionnaireQuestionList || [];
      const questionAnserList: any = qqList.map((qq: ImsQuestionnaireQuestion) => {
        const question: ImsQuestion = qq.question as ImsQuestion;
        // Create answer item
        let answer: ImsAnswerInstance = {
          questionId: question.id,
        };
        const fieldName: string = `FIELD_${question.id}`;
        const type: fieldType = localThis.getFieldType(question.formFieldType?.code);
        // Add answer value
        switch (type) {
          case fieldType.text:
            answer.answerText = localThis.formData[fieldName];
            break;
          case fieldType.checkbox:
            answer.answerCheckbox = localThis.formData[fieldName];
            break;
          case fieldType.date:
            answer.answerDate = localThis.formData[fieldName];
            break;
          case fieldType.number:
            answer.answerNumber = localThis.formData[fieldName];
            break;
          case fieldType.select:
          case fieldType.radio:
            // case 'radio_multi':
            answer.answerOptionId = localThis.formData[fieldName];
            break;
          case fieldType.radio_matrix:
            answer.answerMatrixOptionId = localThis.formData[fieldName];
            answer.answerMatrixOptionId = answer.answerMatrixOptionId?.replaceAll(
              "[]",
              "0"
            );
            break;
          default:
            break;
        }
        return answer;
      });

      // const questionnaireInfo:any = {
      // 	questionnaireId: localThis.questionnaireStruct.questionnaireId
      // };

      const dataPayload: any = {
        QUESTIONNAIRE_ID: localThis.questionnaireId,
        BENEFICIARY_SELECTOR: localThis.formData.BENEFICIARY_SELECTOR,
        HOUSEHOLD_SELECTOR: localThis.formData.HOUSEHOLD_SELECTOR,
        ANNUAL_PLAN_SELECTOR: localThis.formData.ANNUAL_PLAN_SELECTOR,
        SETTLEMENT_SELECTOR: localThis.formData.SETTLEMENT_SELECTOR,
        QUESTIONS: questionAnserList,
      };

      let savePayload: GenericSqlPayload = {
        spName: "IMS_SP_SUBMIT_QUESTIONNAIRE_INSTANCE",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(dataPayload),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: res.status,
            text: res.message,
          });
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });

          // Enable new submission
          localThis.wasSubmitted = false;
        })
        .finally(() => {
          localThis.showConfirmDialog = false;
        });
    },
  },
  //async mounted() {
  mounted() {
    const localThis: any = this as any;
    const officeId: number = localThis.getCurrentOffice.HR_OFFICE_ID;
    localThis.getQuestionnaireStruct(officeId, localThis.questionnaireId);
    localThis.formData= localThis.formData1;

    // let a =  await localThis.getQuestionnaireStruct(officeId, localThis.questionnaireId);
    // localThis.dataReady=true;
  },
});
</script>

<style scoped></style>
