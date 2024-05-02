
import { mapGetters, mapActions } from "vuex";
import { IapiBlobStorageUpload, IapiBlobStorageUploadOverwrite,IapiBlobStorageDownload} from "../store/apiHandler"
import { JrsFormField, JrsFormFieldSelect } from "@/models/JrsForm";
import {
    ImsApi,
    PmsAnnualPlanApi,
    ImsLookupsApi,
    Configuration,
    NavIntegrationApi,
    NavBudget1,
    NavDimension1,
    NavPayload
  } from "../axiosapi";
  import {
    GenericSqlSelectPayload,
    GenericSqlPayload,
    SqlActionType,
  } from "../axiosapi/api";
export default {
    methods:{

      updateNavDimension(value: any) {
        let localThis = this as any;
        const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
        const api: NavIntegrationApi = new NavIntegrationApi(config);
        let navdimension: NavDimension1={};
        let _boolean = false;
        if (value.blocked=="TRUE") _boolean=true;
        switch(value.dimensionCode)
        {
          case "PROJECT":
            navdimension = {
          
              dimensionCode: "PROJECT",
              code: value.project_code,
              name: value.descr,
              dimensionValueType: "Standard",
              totaling: "",
              blocked: _boolean,
              consolidationCode: "",
              indentation: 1,
              globalDimensionNo: "1",
              maptoICDimensionCode: "",
              maptoICDimensionValueCode: "",
              dimensionValueID: 1,
              id: "1",
              lastModifiedDateTime: new Date(),
              mappingCatOfIntervention: false,
              catOfInterventionCode: "",
              projectCity: value.location_description,
              company: value.office_code,
              guid: value.guid,
            };
            break;
          case "DEPARTMENT":
            
            navdimension = {
          
              dimensionCode: "DEPARTMENT",
              code: value.department_code,
              name: value.department_descr,
              dimensionValueType: "Standard",
              totaling: "",
              blocked: _boolean,
              consolidationCode: "",
              indentation: 1,
              globalDimensionNo: "3",
              maptoICDimensionCode: "",
              maptoICDimensionValueCode: "",
              dimensionValueID: 1,
              id: "1",
              lastModifiedDateTime: new Date(),
              mappingCatOfIntervention: false,
              catOfInterventionCode: "",
              projectCity: "",
              company: "",
              guid: value.guid,
            };
            break;

            case "TYPE OF SERVICE":
            
              navdimension = {
            
                dimensionCode: "TYPE OF SERVICE",
                code: value.tos_code,
                name: value.tos_descr,
                dimensionValueType: "Standard",
                totaling: "",
                blocked: _boolean,
                consolidationCode: "",
                indentation: 1,
                globalDimensionNo: "4",
                maptoICDimensionCode: "",
                maptoICDimensionValueCode: "",
                dimensionValueID: 1,
                id: "1",
                lastModifiedDateTime: new Date(),
                mappingCatOfIntervention: false,
                catOfInterventionCode: value.coi_code,
                projectCity: "",
                company: "",
                guid: value.guid,
              };
              break;

              case "CAT. OF INTERVENTION":
            
                navdimension = {
              
                  dimensionCode: "CAT. OF INTERVENTION",
                  code: value.coi_code,
                  name: value.coi_descr,
                  dimensionValueType: "Standard",
                  totaling: "",
                  blocked: _boolean,
                  consolidationCode: "",
                  indentation: 1,
                  globalDimensionNo: "5",
                  maptoICDimensionCode: "",
                  maptoICDimensionValueCode: "",
                  dimensionValueID: 1,
                  id: "1",
                  lastModifiedDateTime: new Date(),
                  mappingCatOfIntervention: true,
                  catOfInterventionCode: "",
                  projectCity: "",
                  company: "",
                  guid: value.guid,
                };
                break;
        }
       
  
        return api
          .apiNavDimensionsPost(navdimension, config.baseOptions)
          .then((response) => {})
          .catch((err: any) => {
            throw err;
          });
      },  
    async updateBudgetOnNav(del: string,AI_ID:number,year:number,showWait:boolean) {
        let localThis = this as any;
  
        let nvbList = [] as NavBudget1[];
        let nvb = {} as NavBudget1;
  
        let view: string = "PMS_VI_ANNUAL_PLAN_OBJECTIVE_BUDGET_LINE_DETAILS_FOR_NAV_V2";
        let wherecond: string = `AI_ID = ${AI_ID} AND (${year}=-1 OR YEAR=${year})`;
        let selectPayload: GenericSqlSelectPayload = {
          viewName: view,
          colList: null,
          whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
          orderStmt: null,
        };
  
        return localThis
          .getGenericSelect({ genericSqlPayload: selectPayload })
          .then((res: any) => {
            //Setup data
            if (res.table_data) {
              res.table_data.forEach((item: any) => {
                if (item.VALUE != undefined) {
                  let nvb = {} as NavBudget1;
                  nvb.pMBDGNAME = "BUDGET";
                  nvb.pMACCNUM = item.PMS_JRS_COA_CODE;
                  nvb.pMDATE = item.YEAR + "-" + item.MONTH + "-1"; // DateTime.Now.ToString("yyyy-MM-dd");
                  nvb.pMGBLDIM1CODE = item.CODE;
                  nvb.pMGBLDIM2CODE = ""; //LEGATO A GMT
  
                  nvb.pMAMOUNT = item.VALUE;
                  nvb.pMDESCRIPTION = item.PMS_COA_CUSTOM_DESC;
  
                  nvb.pMBDGDIM1CODE = item.DEPARTMENT;
                  nvb.pMBDGDIM2CODE = item.FUNCTION_CODE;
                  nvb.pMBDGDIM3CODE = item.PMS_COI_CODE;
                  nvb.pMBDGDIM4CODE = item.PMS_TOS_CODE;
                  nvb.pMBDGDIM5CODE = item.SUB_TRANS;
                  nvb.pMMEASUREUNITCODE = item.UNIT_TYPE;
                  nvb.pMENTRYNMB = item.BDG_ID + "";
                  // (item.AP_ID + "").padEnd(5, "0") +
                  // (item.OBJ_ID + "").padEnd(6, "0") +
                  // (item.SRV_ID + "").padEnd(6, "0") +
                  // (item.ACT_ID + "").padEnd(6, "0") +
                  // (item.PMS_COI_ID + "").padEnd(6, "0") +
                  // (item.PMS_TOS_ID + "").padEnd(6, "0") +
                  // (item.PMS_JRS_COA_ID + "").padEnd(6, "0") +
                  // (item.YEAR + "").padEnd(4, "0") +
                  // (item.MONTH + "").padStart(2, "0");
                  nvb.pMQUANTITY = item.UNIT;
                  nvb.pMUNITCOST = item.UNIT_PRICE;
                  //nvb.pMENTRYNMB = "2";
                  nvb.pMLASTMODDT = ""; //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
  
                  nvb.pMDELETED = del;
  
                  nvb.pMCOMPANY = item.HR_OFFICE_CODE;
                  nvbList.push(nvb);
                }
              });
            }
            return nvbList;
          })
          .then((res1: any) => {
            if (nvbList.length > 0) {
              //let aa = localThis.updateBudgetOnNavApi(nvbList); //.then((res2: any) => {
              return localThis.updateBudgetOnNavApi(nvbList,showWait);
              // });
            } else {
              showWait = false;
              return "bbb";
            }
          })
          .catch((err: any) => {
            showWait = false;
            let errore:any=err.message;
            if(err.response != undefined && err.response.data != undefined)
            {
              if(err.response.data.message != undefined)
              errore +=" - "+err.response.data.message ;
            }
            localThis.showNewSnackbar({ typeName: "error", text: errore });
            throw err;
            //
          });
      },
  
      updateBudgetOnNavApi(nvbList: NavBudget1[],showWait:boolean) {
        let localThis = this as any;
        const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
        const api: NavIntegrationApi = new NavIntegrationApi(config);
        let nvbListStr: string;
         
        nvbListStr = JSON.stringify(nvbList);
        let navpayload:NavPayload = {
            data: nvbListStr,
            info:""
        }
    
        var options = {
          //timeout: 1000000,
          maxContentLength: Infinity,
          maxBodyLength: Infinity
        };
        return api
           
          .apiNavGLBulkBudgetPost(navpayload)         
          .then((res1: any) => {
            showWait = false;
          })
          .catch((err: any) => {
            showWait = false;
            throw err;
            //localThis.showNewSnackbar({ typeName: "error", text: err.response.data });
          });
      },
    }

}