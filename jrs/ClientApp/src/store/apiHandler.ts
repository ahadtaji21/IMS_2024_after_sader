import {
  BlobStorageAzureApi,
  ConfigGenericApi,
  GenericConditionRule,
  HrOfficeApi,
  ImsQuestionnaireTab,
  RuleOperator,
  SfipActivitySchedule,
  SfipPlanApi,
} from "./../axiosapi/api";
import {
  Module,
  GetterTree,
  MutationTree,
  ActionTree,
  ActionContext,
} from "vuex";
import { translate } from "../i18n";
import {
  Configuration,
  ImsLookup,
  ImsLookupsApi,
  HrBiodataApi,
  GenericSqlApi,
  GenericSqlPayload,
  GenericSqlSelectPayload,
  ImsTemplateApi,
  GenericExelSelectPayload,
  ExcelStoredProcedurePayload,
  ImsQuestionnaireApi,
  ImsQuestionnaire,
  ImsQuestionnaireQuestion,
  ImsQuestion,
  ImsQuestionnaireInstance,
} from "../axiosapi";
import { codeToRuleOperator, Operators } from "@/models/JrsTable";
import { AxiosResponse, AxiosInstance, AxiosRequestConfig } from "axios";
import axios from "axios";

export enum ApiCallStatus {
  NoCall,
  Pending,
  Success,
  Error,
}

export interface IapiHandlerState {
  status: ApiCallStatus;
  error: any;
}

export interface IapiBlobStorageUpload {
  nameContainer?: string;
  folder?: string;
  userUid?: string;
  officeId?: number;
  documentTypeId?: number;
  file?: any;
}

export interface IapiBlobStorageUploadOverwrite {
  nameContainer?: string;
  folder?: string;
  uniqueFileName?: string;
  guidFile?: string;
  userUid?: string;
  officeId?: number;
  file?: any;
}

export interface IapiBlobStorageDeleteFile {
  stPathRoot?: string;
  nameContainer?: string;
}

export interface IapiBlobStorageDownload {
  nameContainer?: string;
  pathRoot?: string;
  extension?: string;
  fileName?: string;
}

const state: IapiHandlerState = {
  status: ApiCallStatus.NoCall,
  error: null,
};

const getters: GetterTree<IapiHandlerState, any> = {};

const mutations: MutationTree<IapiHandlerState> = {
  SET_API_STATUS(state: IapiHandlerState, newStatus: ApiCallStatus) {
    state.status = newStatus;
  },
};

const actions: ActionTree<IapiHandlerState, any> = {
  /**
   * Loads all lookups from DB
   * @param context store context
   */
  getLookups(context: any) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsLookupsApi = new ImsLookupsApi(config);

    return new Promise((resolve, reject) => {
      api
        .apiImsLookupsGet()
        .then((res: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          resolve(res.data);
        })
        .catch((err: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Error);
          reject(err);
        });
    });
  },
  getGenericSelect(context: any, payload: any) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: GenericSqlApi = new GenericSqlApi(config);
    return new Promise((resolve, reject) => {
      api
        .genericSqlSelect(payload.genericSqlPayload)
        .then((res: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          let ret = res.data;
          //Clean the Headers
          ret.columns = !ret.columns
            ? []
            : ret.columns.map((header: any) => {
                // 1) TABLE - Setup headers for table
                //header.text = header.column_name;               //TODO: add trnaslation of label
                header.text = header.translationtKey
                  ? translate(header.translationtKey)
                  : header.column_name;
                header.value = header.column_name;
                header.sortable = true;
                header.filterable = true;
                header.divider = true;

                // Setup date columns
                if (header.js_type == "Date") {
                  header.isDateTime = true;
                  header.dateTimeFormat = "short";
                }

                // Setup checkbox columns
                if (header.js_type == "Boolean") {
                  header.isCheckbox = true;
                  header.align = "center";
                }

                // // 2) MODAL FORM - Setup selects
                // //  selcts form Lookup
                // if(header.select_lookup_code){
                //     debugger;
                //     header.type = 'select';
                //     header.selectItems = context.rootGetters['getLookupsByTypeCode'](header.select_lookup_code);
                //     header.lookupName = 'Holder' + 'HR_BIODATA_GENDER_ID';      //TODO: Change
                //     header.itemText = 'imsLookupValue';
                //     header.itemKey = 'imsLookupId';
                // }

                return header;
              });

          resolve(ret);
        })
        .catch((err: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Error);
          reject(err);
        });
    });
  },

  /**
   * Generic call to a DB SP.
   * @param context object that exposes the same set of methids/properties on the store instance
   * @param payload
   */
  execSPCall(context: any, GenericSqlPayload: GenericSqlPayload) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: GenericSqlApi = new GenericSqlApi(config);

    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    return new Promise((resolve, reject) => {
      api
        .genericSqlSPCall(GenericSqlPayload)
        .then((res: any) => {
          if (res.data.status == "error") {
            throw res;
          }
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          resolve(res.data);
        })
        .catch((err: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Error);
          reject(err.data);
        });
    });
  },
  /**
   * Manages dynamic db save calls.
   * @param context store context
   * @param payload information required to save to db. Payload must be "{ actionName: <name of acion in store for saving>, callPayload: <payload required by acion in store for saving> }"
   */
  DbSave(context: any, payload: any) {
    //Destructure payload
    let { userUid, actionName, callPayload } = payload;
    return context.dispatch(actionName, callPayload);
  },
  /**
   * Get table structure information.
   * @param context store context
   * @param payload information of the table structure to query. example: { "genericSqlPayload": {} }
   */
  getJRSTableStructure(context: any, payload: any) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: GenericSqlApi = new GenericSqlApi(config);
    
    return api
      .genericSqlSelectStructure(payload.genericSqlPayload)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
      });
  },

  /**
   * Get table structure information.
   * @param context store context
   * @param container the name of container
   * @param folder the name of folder
   * @param file the file to upload
   */
  uploadBlobStorageFile(context: any, payload: IapiBlobStorageUpload) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: BlobStorageAzureApi = new BlobStorageAzureApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    return new Promise((resolve, reject) => {
      api
        .apiBlobStorageAzureUploadFileToContainerPost(
          payload.nameContainer,
          payload.folder,
          payload.userUid,
          payload.officeId,
          payload.documentTypeId,
          payload.file
        )
        .then((res: any) => {
          if (res.data.status == "error") {
            throw res;
          }
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          resolve(res.data);
        })
        .catch((err: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Error);
          reject(err.data);
        });
    });
  },
  /**
   * Get table structure information.
   * @param context store context
   * @param container the name of container
   * @param folder the name of folder
   * @param file the file to upload
   */
  uploadOverwriteBlobStorageFile(
    context: any,
    payload: IapiBlobStorageUploadOverwrite
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: BlobStorageAzureApi = new BlobStorageAzureApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    return new Promise((resolve, reject) => {
      api
        .apiBlobStorageAzureUploadOverwriteFileToContainerPost(
          payload.nameContainer,
          payload.folder,
          payload.uniqueFileName,
          payload.guidFile,
          payload.userUid,
          payload.file
        )
        .then((res: any) => {
          if (res.data.status == "error") {
            throw res;
          }
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          resolve(res.data);
        })
        .catch((err: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Error);
          reject(err.data);
        });
    });
  },

  /**
   * Get table structure information.
   * @param context store context
   * @param container the name of container
   * @param folder the name of folder
   * @param file the file to upload
   */
  deleteBlobStorageFile(context: any, payload: IapiBlobStorageDeleteFile) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: BlobStorageAzureApi = new BlobStorageAzureApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    return new Promise((resolve, reject) => {
      api
        .apiBlobStorageAzureDeleteFileFromContainerGet(
          payload.stPathRoot,
          payload.nameContainer
        )
        .then((res: any) => {
          if (res.data.status == "error") {
            throw res;
          }
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          resolve(res.data);
        })
        .catch((err: any) => {
          context.commit("SET_API_STATUS", ApiCallStatus.Error);
          reject(err.data);
        });
    });
  },

  /**
   * Get table structure information.
   * @param context store context
   * @param payload
   */
  downloadBlobStorageFile(context: any, payload: IapiBlobStorageDownload) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: BlobStorageAzureApi = new BlobStorageAzureApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    return api
      .apiBlobStorageAzureDownloadFileFromContainerGet(
        payload.nameContainer,
        payload.pathRoot,
        payload.extension,
        payload.fileName
      )
      .then((res: any) => {
        if (res) {
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          return res.data;
        }
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        if (err.response.status == 404) {
          throw translate("error.file-not-found");
        } else {
          throw err.message;
        }
      });
  },

  /**
   * Get table structure information.
   * @param context store context
   * @param payload
   */
  downloadDocxFromTemplate(context: any, { templateId, queryCondition }) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);

    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    return api
      .apiMergedTextGet(templateId, queryCondition)
      .then((res: any) => {
        if (res) {
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          return res.data;
        }
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        if (err.response.status == 404) {
          throw translate("error.file-not-found");
        } else {
          throw err.message;
        }
      });
  },

  /**
   * Get data of Template Type. If no ID is provided, all templates will be wueried
   * @param context store context.
   */
  getTemplateTypes(context: any) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    return api
      .apiTemplateTypesGet()
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw err.message;
      });
  },
  /**
   * Get the Template Type for a specific template.
   * @param context store context.
   * @param templateId ID of the template to recover template type for
   */
  getTemplateTypeForTemplate(context: any, templateId: number) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);

    if (!templateId) {
      return Promise.reject("templateId not provided.");
    }
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    return api
      .apiTemplateTypeForTemplateTemplateIdGet(templateId)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw err.message;
      });
  },

  getTemplate(
    context: any,
    { officeId, templateId, templateTypeId, templateTypeCode }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    if (!officeId) {
      return Promise.reject("officeId not provided.");
    }
    return api
      .apiTemplateGet(officeId, templateId, templateTypeId, templateTypeCode)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw new Error(err.message);
      });
  },

  generateExcelForTable(
    context: any,
    { viewName, colList, whereCond, orderStmt, officeId, colLabels }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);
    const payload: GenericExelSelectPayload = {
      viewName,
      colList,
      whereCond,
      orderStmt,
      officeId,
      columnLabels: colLabels,
    };

    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    return api
      .apiCreateExcelForTablePost(payload)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);

        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw new Error(err.message);
      });
  },

  generateExcelFromSP(context: any, { PROCEDURE_PARAMETERS, PROCEDURE_NAME }) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);
    const payload: ExcelStoredProcedurePayload = {
      PROCEDURE_PARAMETERS,
      PROCEDURE_NAME,
    };
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    return api
      .apiCreateExcelForSPPost(payload)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw new Error(err.message);
      });
  },

  generateExcelForTableOvwInd(
    context: any,
    payLoads: GenericExelSelectPayload[]
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);

    let i: number = 0;

    // for( i=0;i<payLoads.length;i++)
    // {
    //     _payLoads.push(
    //         {
    //             payLoads[i].viewName,
    //             payLoads[i].colList,
    //             payLoads[i].whereCond,
    //             payLoads[i].orderStmt,
    //             payLoads[i].officeId,
    //             payLoads[i].colLabels
    //         }

    //     )
    // }

    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    let dt: string = JSON.stringify(payLoads);
    return api
      .apiCreateExcelForOverviewAllIndicatorsPost(dt)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw new Error(err.message);
      });
  },

  generateExcelForOverviewIndicators(
    context: any,
    { viewName, colList, whereCond, orderStmt, officeId, colLabels, bodyValues }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);
    const payload: GenericExelSelectPayload = {
      viewName,
      colList,
      whereCond,
      orderStmt,
      officeId,
      columnLabels: colLabels,
      bodyValues,
    };

    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    return api
      .apiCreateExcelForOverviewIndicatorsPost(payload)
      .then((res: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Success);
        return res.data ? res.data : res;
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw new Error(err.message);
      });
  },

  copyConfigFromToOffices(
    context: any,
    { tableName, columnNameOffice, idOfficeFrom, idOfficeTo, columnNamePK }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ConfigGenericApi = new ConfigGenericApi(config);

    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    return api
      .copyConfigurationFromToOffices(
        tableName,
        columnNameOffice,
        idOfficeFrom,
        idOfficeTo,
        columnNamePK
      )
      .then((res: any) => {
        if (res) {
          context.commit("SET_API_STATUS", ApiCallStatus.Success);
          return res.data;
        }
      })
      .catch((err: any) => {
        context.commit("SET_API_STATUS", ApiCallStatus.Error);
        throw new Error(err.message);
      });
  },
  /**
   * Get list of questionnaire available for the given office.
   * @param context store context.
   * @param payload Payload in form { officeId:Number }.
   * @returns List of questionnaire objects.
   */
  async getOfficeQuestionnaireList(
    context: any,
    { officeId }
  ): Promise<ImsQuestionnaire[]> {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    if (!officeId) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      return Promise.reject("Error: Incorrect parameters for call.");
    }
    try {
      const apiResult: any = await api.apiQuestionnaireOfficeIdGet(officeId);

      // No content returned
      if (apiResult.status == 204) {
        throw new Error("No valid questionnaire was found.");
      }

      const questionnaireList: ImsQuestionnaire[] = apiResult.data || [];
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return questionnaireList;
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },
  /**
   * Get questionnaire structure information.
   * @param context store context.
   * @param payload Payload in form { officeId:Number, questionnaireId:Number }.
   * @returns A questionnaire object with it's related data.
   */
  async getQuestionnaire(
    context: any,
    { officeId, questionnaireId, questionnaireCode }
  ): Promise<ImsQuestionnaire> {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    if (!officeId || !(questionnaireId || questionnaireCode)) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      return Promise.reject("Error: Incorrect parameters for call.");
    }

    try {
      let apiResult: any;
      if (questionnaireId) {
        apiResult = await api.apiQuestionnaireByIdOfficeIdQuestionnaireIdGet(
          officeId,
          questionnaireId
        );
      } else {
        apiResult =
          await api.apiQuestionnaireByCodeOfficeIdQuestionnaireCodeGet(
            officeId,
            questionnaireCode
          );
      }
      // No content returned
      if (apiResult.status == 204) {
        throw new Error("No valid questionnaire was found.");
      }
      const questionnaire: ImsQuestionnaire = apiResult.data || {};
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return questionnaire;
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },

  /**
   * Update Questionnaire-Questions.
   * @param context store context.
   * @param payload Payload in form { questionnaireId:Number, qqList:ImsQuestionnaireQuestion[] }.
   */
  async updateQuestionnaireQuestions(
    context: any,
    { questionnaireId, qqList }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    // Check data
    if (!questionnaireId || !qqList || qqList.length == 0) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      return Promise.reject("Error: Incorrect parameters for call.");
    }

    try {
      // Call api
      const apiResult: any =
        await api.apiQuestionnaireQuestionQuestionnaireIdPost(
          questionnaireId,
          qqList
        ); //api.apiQuestionnaireQuestionQuestionnaireIdPut(questionnaireId, qqList);
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return Promise.resolve();
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },

  /**
   * Get questionnaire structure information.
   * @param context store context.
   * @param payload Payload in form { officeId:Number, questionnaireId:Number }.
   * @returns A questionnaire object with it's related data.
   */
  async getQuestionnaireStructure(context: any, { officeId, questionnaireId }) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    if (!officeId || !questionnaireId) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      return Promise.reject("Error: Incorrect parameters for call.");
    }

    try {
      const fieldTypePayload: GenericSqlSelectPayload = {
        viewName: "IMS_FORM_FIELD_TYPE",
      };
      const fieldTypeList: any[] = (
        await context.dispatch("getGenericSelect", {
          genericSqlPayload: fieldTypePayload,
        })
      ).table_data;
      const getFieldTypeCode: Function = (fieldTypeId: number) => {
        const fieldType: any = fieldTypeList.find(
          (fieldType: any) => fieldType.ID == fieldTypeId
        );
        return fieldType.CODE.toLocaleLowerCase();
      };
      const apiResult: any =
        await api.apiQuestionnaireByIdOfficeIdQuestionnaireIdGet(
          officeId,
          questionnaireId
        );
      // No content returned
      if (apiResult.status == 204) {
        throw new Error("No valid questionnaire was found.");
      }
      const questionnaire: ImsQuestionnaire = apiResult.data || {};
      const ret: any = {
        code: questionnaire.code,
        title: questionnaire.title,
        descr: questionnaire.descr,
        includeBeneficiarySelection: questionnaire.includeBeneficiarySelection,
        includeHouseholdSelection: questionnaire.includeHouseholdSelection,
        includeProjectSelection: questionnaire.includeProjectSelection,
        includeSettlementSelection: questionnaire.includeSettlementSelection,
        questionInfos: questionnaire.questionnaireQuestionList?.map(
          (qq: ImsQuestionnaireQuestion) => ({
            question: qq.question,
            ordinalPosition: qq.ordinalPosition,
          })
        ),
      };
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return ret;
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },

  /**
   * Get questionnaire instance information.
   * @param context store context.
   * @param payload Payload in form { officeId:Number, questionnaireId:Number }.
   * @returns A questionnaire instance data.
   */
  async getQuestionnaireInstances(context: any, { officeId, questionnaireId }) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    // Verify params
    if (!officeId || !questionnaireId) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      return Promise.reject("Error: Incorrect parameters for call.");
    }

    try {
      const apiResult: any =
        await api.apiQuestionnaireInstanceOfficeIdQuestionnaireIdGet(
          officeId,
          questionnaireId
        );
      const instances: ImsQuestionnaireInstance[] = apiResult.data || [];

      const instanceRows: any = instances.map(
        (inst: ImsQuestionnaireInstance) => ({
          id: inst.id,
          questionnaireId: inst.questionnaireId,
          questionnaireCode: inst.questionnaire?.code,
          questionnaireTitle: inst.questionnaire?.title,
          fillInDate: inst.fillInDate,
          reviewDate: inst.reviewDate,
          statusName: inst.status?.imsStatusName,
        })
      );

      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return { instances, instanceRows };
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },
  /**
   * Get questionnaire instance information.
   * @param context store context.
   * @param payload Payload in form { officeId:Number, questionnaireId:Number, conditionRules:GenericConditionRule[] }.
   * @returns A questionnaire instance data.
   */
  async getQuestionnaireInstancesWithConditions(
    context: any,
    { officeId, questionnaireId, conditionRules }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    // Verify params
    if (!officeId || !questionnaireId) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      return Promise.reject("Error: Incorrect parameters for call.");
    }

    try {
      const apiResult: any =
        await api.apiQuestionnaireInstanceWithConditionsOfficeIdQuestionnaireIdPost(
          officeId,
          questionnaireId,
          conditionRules
        );
      const instances: ImsQuestionnaireInstance[] = apiResult.data || [];

      const instanceRows: any = instances.map(
        (inst: ImsQuestionnaireInstance) => ({
          id: inst.id,
          questionnaireId: inst.questionnaireId,
          questionnaireCode: inst.questionnaire?.code,
          questionnaireTitle: inst.questionnaire?.title,
          fillInDate: inst.fillInDate,
          reviewDate: inst.reviewDate,
          statusName: inst.status?.imsStatusName,
        })
      );

      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return { instances, instanceRows };
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },
  /**
   * Get a list of questionnaire-tabs.
   * @param context store context.
   * @param payload Payload in form { officeId:Number, questionnaireId:Number }.
   * @returns A list of questionnaire-tabs.
   */
  async getQuestionnaireTabs(context: any, { officeId, questionnaireId }) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsQuestionnaireApi = new ImsQuestionnaireApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);
    try {
      const apiResult: any =
        await api.apiQuestionnaireTabListOfficeIdQuestionnaireIdGet(
          officeId,
          questionnaireId
        );
      // No content returned
      if (apiResult.status == 204) {
        throw new Error("No valid questionnaire was found.");
      }
      const tabs: ImsQuestionnaireTab[] = apiResult.data || [];

      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return tabs;
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },

  /**
   * Download excel file with questionnaire results.
   * @param context store context.
   * @param payload Payload in form { questionnaireId:Number }.
   * @returns Byte array reprensentin the requested excel document.
   */
  async generateExcelForQuestionnaire(
    context: any,
    { questionnaireId, conditionRules, columnLabels }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: ImsTemplateApi = new ImsTemplateApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    const excelSelectPayload: GenericExelSelectPayload = {
      columnLabels: columnLabels,
      conditionRules: conditionRules,
    };

    try {
      conditionRules = conditionRules || [];
      const apiResult: any = await api.downloadQuestionnaireResults(
        questionnaireId,
        excelSelectPayload
      );
      // Throw errors
      if (apiResult.status != 200) {
        throw new Error(
          "Error in retrieving the requested questionnaire results."
        );
      }
      const bytes: Array<any> = apiResult.data || [];

      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return bytes;
    } catch (err) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((err as any).message);
    }
  },
  /**
   * Get SFIP plan tree structure.
   * @param context Store context.
   * @param param1 Options in the form {planId:number, officeId:number}
   * @returns Plan tree starting with a list of Priority Sets.
   */
  async getPlanTree(
    context: any,
    {
      planId,
      officeId,
      includeActivities,
    }: { planId: number; officeId: number; includeActivities: boolean }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: SfipPlanApi = new SfipPlanApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    try {
      const apiResult: any = await api.getSfipPlanTree(
        planId,
        officeId,
        includeActivities
      );
      // Throw errors
      if (apiResult.status != 200) {
        throw new Error(
          "Error in retrieving Strategic Framework Implementation Plan structure."
        );
      }
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return apiResult.data || [];
    } catch (error) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((error as any).message);
    }
  },

  /**
   * Get Activity Schedule for the provided Activity.
   * @param {ActionContext} context Store context.
   * @param {Number} activityId Id of Activity to get schedule for.
   * @returns {Promise<SfipActivitySchedule[]>} List of schedule items.
   */
  async getActivitySchedule(
    context: ActionContext<IapiHandlerState, any>,
    activityId: number
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: SfipPlanApi = new SfipPlanApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    try {
      const apiResult: AxiosResponse<SfipActivitySchedule[]> =
        await await api.getSfipActivitySchedule(activityId);
      // Throw errors
      if (apiResult.status != 200) {
        throw new Error("Error in retrieving Activity Schedule.");
      }
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return apiResult.data || [];
    } catch (error) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((error as any).message);
    }
  },
  /**
   *
   * @param {ActionContext} context Store context.
   * @param Options in the form {activityId:number, scheduleList:SfipActivitySchedule[]}
   */
  async updateActivityScheduleList(
    context: ActionContext<IapiHandlerState, any>,
    {
      activityId,
      scheduleList,
    }: { activityId: number; scheduleList: SfipActivitySchedule[] }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: SfipPlanApi = new SfipPlanApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    try {
      const apiResult: AxiosResponse =
        await await api.putSfipActivityScheduleList(activityId, scheduleList);
      // Throw errors
      if (apiResult.status != 204) {
        throw new Error("Error in updating Activity Schedule.");
      }
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return apiResult.data || [];
    } catch (error) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((error as any).message);
    }
  },
  /**
   *
   * @param context
   * @param Options in the form { officeId:number, includeDescendants:true }
   * @returns {HrOffice} Office data.
   */
  async getOffice(
    context: ActionContext<IapiHandlerState, any>,
    {
      officeId,
      includeDescendants,
    }: { officeId: number; includeDescendants: true }
  ) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const api: HrOfficeApi = new HrOfficeApi(config);
    context.commit("SET_API_STATUS", ApiCallStatus.Pending);

    try {
      // const apiResult: AxiosResponse = await (await api.getHrOffice(officeId, true, includeDescendants));
      const apiResult: any = await api.getHrOffice(
        officeId,
        true,
        includeDescendants
      );
      // Throw errors
      if (apiResult.status != 200) {
        throw new Error("Error getting Office data.");
      }
      context.commit("SET_API_STATUS", ApiCallStatus.Success);
      return apiResult.data || [];
    } catch (error) {
      context.commit("SET_API_STATUS", ApiCallStatus.Error);
      throw new Error((error as any).message);
    }
  },
  async getConfiguration(
    context: any,
    payload: {
      name: any;
      body: any;
      post: any;
      page?: number;
      limit?: number;
    }
  ) {
    const { name, body, post, page, limit } = payload;

    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const route = `${config.basePath}/api/${name}/`;
    const theheader = {
      ...config.baseOptions.headers,
      "Content-Type": "application/json",
    };

    let getParamsQuery: { page?: number; limit?: number } = {};
    if (page !== undefined && limit !== undefined) {
      getParamsQuery.page = page;
      getParamsQuery.limit = limit;
    }
    const axiosRequestArgs: AxiosRequestConfig = post
      ? {
          url: route,
          method: "POST",
          headers: theheader,
          params: getParamsQuery,
          data: JSON.stringify(body),
        }
      : {
          url: route,
          method: "GET",
          headers: theheader,
          params: getParamsQuery,
        };
    return new Promise(async (resolve, reject) => {
      try {
        const response = await axios.request(axiosRequestArgs);
        resolve(response.data);
      } catch (error) {
        if(error.response){
          console.log("error: ", error.response.data);
          reject(error.response.data);
        }else{
          console.log("error: ", error.message);
          reject(error.message);
        }

      }
    });
  },

  async saveWizard(context: any, body) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];
    const route = `${config.basePath}/api/Individual/AddWizard`;

    const theheader = {
      ...config.baseOptions.headers,
      "Content-Type": "application/json",
    };

    const axiosRequestArgs: AxiosRequestConfig = {
      url: route,
      method: "POST",
      data: JSON.stringify(body),
      headers: theheader,
    };
    return new Promise(async (res, rej) => {
      try {
        const response = await axios.request(axiosRequestArgs);
        res(response);
      } catch (error) {
        console.log("error: ", error);
        rej(error);
      }
    });
  },

  async updateWizard(context: any, payload: { body: any; route: any }) {
    const config: Configuration = context.rootGetters["auth/getApiConfig"];

    const theheader = {
      ...config.baseOptions.headers,
      "Content-Type": "application/json",
    };

    const axiosRequestArgs: AxiosRequestConfig = {
      url: payload.route,
      method: "POST",
      data: JSON.stringify(payload.body),
      headers: theheader,
    };

    return new Promise(async (res, rej) => {
      try {
        const response = await axios.request(axiosRequestArgs);
        res(response);
      } catch (error) {
        console.log("error: ", error);
        rej(error);
      }
    });
  },
};

const namespaced: boolean = true;
const apiHandler: Module<IapiHandlerState, any> = {
  namespaced,
  state,
  actions,
  getters,
  mutations,
};

export default apiHandler;
