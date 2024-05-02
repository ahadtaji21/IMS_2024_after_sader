
export interface IBudgetItemGET {
  ID: number;
  ACTIVITY_ID: number;
  RESOURCE_ID: number;
  POSITION_ID: number;
  YEAR: number;
  PERCENTAGE: number;
  SELECTED: boolean[];
  YEARBUDGET: number;
  MONTHBUDGET: number[];
  MONTHFUNDED: number[];
  PMS_JRS_COA_DESCRIPTION: string;
  DESCRIPTION: string;
  PMS_JRS_COA_ID: number;
  PMS_JRS_COA_CODE: string;
  UNIT_TYPE: string;
  TIME_UNIT: string;
  UNIT: number;
  TIMING: number;
  MODIFIED: boolean[];
  UNIT_PRICE: number;
  BUDGET: number;
  CURRENCY: string;
  PMS_JRS_COI_ID:number;
  PMS_JRS_TOS_ID:number;
  AI_ID:number;
  AIR_ID:number;
}
export interface IDonor{
  ID:number;         
  GRANT_CODE:string;
  GRANT_ID:number;     
  GRANT_PRJ_REL_ID:number;  
  CURRENCY_CODE:string;
  APCODE:string;
  OFFICE_ID:number; 
  OFFICE_CODE:string;
  CODE:string;
  DESCR:string;
  LOCATION_DESCR:string;
  IMS_ADMIN_AREA_ID:number; 
  IMS_ADMIN_AREA_NAME:string;
  START_YEAR:number;   
  STATUS_ID:number;    
  PROJECT_ID:number;   
  IMS_STATUS_NAME:string;
  ID_GRANT:number;     
  GMT_DONOR_ID:number;  
  GMT_DONOR_NAME:string;
  GRANT_INITIAL_AMOUNT:number; 
  GRANT_CURRENCY_CODE:string;
  ALREADY_ALLOCATED:number; 
  AVAILABLE:number;
}
export interface IBudgetItemUPD {

  ID: number;
  DESCRIPTION:string;
  ACTIVITY_ID: number;
  RESOURCE_ID: number;
  POSITION_ID: number;
  PERCENTAGE: number;
  YEAR: number;
  SELECTED: string;
  BUDGET: number;
  CURRENCY: string;
  UNIT_TYPE: string;
  TIME_UNIT: string;
  UNIT_PRICE: string;
  UNIT: number;
  TIMING: number;
  PMS_ACTIVITY_ITEM_REL_ID:number;
}
export interface IEstimatedYearBudget {
  ID: number;
  ACTIVITY_ID: number;
  YEAR: number;
  BUDGET: number;
  CURRENCY: string;
}
export interface IPayLoadDataOUTPUT {

  ACTIVITY_ID: number | null;
  OUTPUT_ID: number | null;
}

export interface IPayLoadDataITEM {
  ID_ANNUAL_PLAN: number | null;
  ID_ACTIVITY_ITEM: number | null;
  ID_ACTIVITY: number | null;
  ID_ITEM: number | null;
}

export interface IPayLoadDataGRANT {
  ID: number | null;
  CODE: string | null;
  DESCR: number | null;
  START_YEAR: number | null;
  VERSION: number | null;
  CURRENCY_CODE: string | null;
  STATUS_ID: number | null;
  DONOR_ID: number | null;
  AMOUNT: string | null;
  DEPARTMENT_ID: number | null;
  //PROJECT_ID: number | null;

}

export interface IPayLoadDataPRJFUND {
  ID_GRANT: number | null;
  ID_PROJECT: number | null;

}

export interface IImsList {
  text: string | null;
  value: string | null;

}

export interface IImsList1 {
  text: string | null;
  value: number | null;

}

export interface IPayLoadDataDonorCOA {
  ACC_ID: number | null;
  DONOR_ID: number | null;
  ACC_CODE: string | null;
  ACC_DESCRIPTION: string | null;
  ACC_TYPE: string | null;
  // ACC_JRS_ID: number | null;
  ACC_STATUS: string | null;
  START_DATE: Date | null;
  END_DATE: Date | null;
  CURRENCY_CODE: string | null;
  coa_relations_placeholder: Object | null;
}

export interface IBudgetDetails {
  ID: number;
  DNR_ID: number;
  ACTIVITY_ID: number;
  JRS_COI_ID: number;
  JRS_TOS_ID: number;
  JRS_COA_ID: number;
  INITIAL_GRANT_AMOUNT: number;
  GRANT_ID: number;
  GRANT_PRJ_REL_ID: number;
  DNR_COI_ID: number;
  DNR_TOS_ID: number;
  DNR_COA_ID: number;
  GMT_COI_ID: number;
  GMT_JRS_COI_ID: number;
  GMT_COI_CODE: string;
  GMT_COI_ENABLED: string;
  GMT_COI_DELETED: string;
  GMT_DONOR_ID: number;
  GMT_TOS_ID: number;
  GMT_TOS_CODE: string;
  GMT_TOS_DESCRIPTION: string;
  GMT_TOS_ENABLED: string;
  ACC_ID: number;
  DONOR_ID: number;
  ACC_CODE: string;
  ACC_DESCRIPTION: string;
  ACC_TYPE: string;
  ACC_STATUS: string;
  ASSIGNED_PERCENTAGE: number;
  ASSIGNED_VALUE: number;
  FUNDING_STATE_ID: number;
  BUDGET_ID: number;
}
