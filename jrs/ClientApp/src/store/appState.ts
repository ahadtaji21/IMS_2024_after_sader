import { ContextReplacementPlugin } from 'webpack';
import { ActionTree, MutationTree, StoreOptions, Module, GetterTree } from 'vuex';
import { ImsLookup, Configuration, ImsLookupsApi } from '../axiosapi';
import { ISnackbar, ISnackbarType, snackbarCategories, snackbarTypes } from '../models/Snackbar';
import { ImsModule } from '../models/MenuItem';

export interface AnnualPlan {
    annal_plan_id: number,
    start_year: number,
    end_year: number,
    currency: string,
    annal_plan_designed: boolean,
    max_categories_number:number
}

export interface Grant {
    grant_id: number,
    year: number,
    end_year: number,
    currency: string,
    donor_id: number,
    donor_name: string,
    department_id: number,
}
export interface IDonor {
    ID: number;
    GRANT_CODE: string;
    GRANT_ID: number;
    GRANT_PRJ_REL_ID: number;
    CURRENCY_CODE: string;
    APCODE: string;
    OFFICE_ID: number;
    OFFICE_CODE: string;
    CODE: string;
    DESCR: string;
    LOCATION_DESCR: string;
    IMS_ADMIN_AREA_ID: number;
    IMS_ADMIN_AREA_NAME: string;
    START_YEAR: number;
    STATUS_ID: number;
    PROJECT_ID: number;
    IMS_STATUS_NAME: string;
    ID_GRANT: number;
    GMT_DONOR_ID: number;
    GMT_DONOR_NAME: string;
    GRANT_INITIAL_AMOUNT: number;
    GRANT_CURRENCY_CODE: string;
    ALREADY_ALLOCATED: number;
    AVAILABLE: number;
}

export interface IAppState {
    expandedAppDrawer: Boolean,
    lookups: Array<ImsLookup>,
    snackBar: ISnackbar,
    annual_plan: AnnualPlan,
    grant: Grant,
    activeModule: ImsModule
    donor_list: Array<IDonor>
}

const state: IAppState = {
    expandedAppDrawer: false,
    lookups: [],
    snackBar: {
        type: snackbarTypes[0],
        text: ''
    },
    annual_plan: {
        start_year: 0,
        end_year: 0,
        annal_plan_id: 0,
        currency: "",
        annal_plan_designed:false,
        max_categories_number:3
    },
    grant: {
        year: 0,
        end_year:0,
        grant_id: 0,
        currency: "",
        donor_id: 0,
        donor_name: "",
        department_id: 0
    },
    donor_list: [] as IDonor[],
    activeModule: { moduleId: undefined, moduleName: undefined,moduleCode: undefined, moduleColor: 'primary' }
}

const getters: GetterTree<IAppState, any> = {

    getAnnualPlan(state: IAppState) {
        return state.annual_plan;
    },
    getDonorList(state: IAppState) {
        return state.donor_list;
    },
    getGrant(state: IAppState) {
        return state.grant;
    },
    getExpandedAppDrawer(state: IAppState): Boolean {
        return state.expandedAppDrawer;
    },
    getLookups(state: IAppState) {
        // return state.lookups;
        const retLookups: any = (state.lookups as ImsLookup[]).map((lookup: ImsLookup) => ({
            imsLookupId: lookup.imsLookupId,
            imsLookupLookupTypeId: lookup.imsLookupLookupTypeId,
            imsLookupCode: lookup.imsLookupCode,
            imsLookupValue: lookup.imsLookupValue,
        }));
        return retLookups;
    },
    getLookupsByTypeCode: (state: IAppState) => (typeCode: string) => {
        // return state.lookups.filter(l => l.imsLookupLookupType && l.imsLookupLookupType.imsLookupTypeCode == typeCode);
        return state.lookups.filter(l => l.imsLookupLookupType && l.imsLookupLookupType.imsLookupTypeCode == typeCode)
            .map((lookup: ImsLookup) => ({
                imsLookupId: lookup.imsLookupId,
                imsLookupLookupTypeId: lookup.imsLookupLookupTypeId,
                imsLookupCode: lookup.imsLookupCode,
                imsLookupValue: lookup.imsLookupValue,
            }));
    },
    getSnackbar(state: IAppState) {
        return state.snackBar;
    },
    getSnackbarText(state: IAppState) {
        return state.snackBar.text;
    },
    getSnackbarColor(state: IAppState) {
        return state.snackBar.type.color;
    },
    getActiveModule(state: IAppState) {
        return state.activeModule;
    }
}

const mutations: MutationTree<IAppState> = {
    SET_EXPANDED_APP_DRAWER(state: IAppState, exp: boolean) {
        state.expandedAppDrawer = exp;
    },
    SET_LOOKUPS(state: IAppState, l: ImsLookup[]) {
        state.lookups = l;
    },
    SET_SNACKBAR(state: IAppState, snackbar: ISnackbar) {
        state.snackBar = snackbar;
    },
    SET_SNACKBAR_TEXT(state: IAppState, msg: string) {
        state.snackBar.text = msg;
    },
    SET_ANNUAL_PLAN(state: IAppState, ap: AnnualPlan) {
        state.annual_plan = ap;
    },
    SET_GRANT(state: IAppState, gt: Grant) {
        state.grant = gt;
    },
    SET_ACTIVE_MODULE(state: IAppState, mod: ImsModule) {
        state.activeModule = mod;
    },
    SET_DONOR_LIST(state: IAppState, l: IDonor[]) {
        state.donor_list = l;
    },
    
}

const actions: ActionTree<IAppState, any> = {
    /**
     * Toggle the app navigation drawer.
     * @param context object that exposes the same set of methods/properties on the store instance
     */
    toggleAppDrawer(context: any) {
        let cur = context.getters['getExpandedAppDrawer'];
        context.commit('SET_EXPANDED_APP_DRAWER', !cur);
    },
    /**
     * Toggle the app navigation drawer.
     * @param context object that exposes the same set of methods/properties on the store instance
     * @param appDrawerState new state for the app drawer
     */
    setAppDrawer(context: any, appDrawerState: boolean) {
        context.commit('SET_EXPANDED_APP_DRAWER', appDrawerState);
    },
    /**
     * Get all lookups from DB.
     * @param context object that exposes the same set of methods/properties on the store instance
     */
    recoverAllLookups(context: any) {
        context.dispatch('apiHandler/getLookups')
            .then((res: ImsLookupsApi[]) => {
                context.commit('SET_LOOKUPS', res);
            }).catch((err: any) => {
                console.error(err);
            });
    },
    /**
     * Displays a new snackbar.
     * @param context object that exposes the same set of methods/properties on the store instance
     * @param typeName name of type of snackbar (info|success|alert|error)
     * @param text text to display in the snackbar
     */
    showNewSnackbar(context: any, { typeName, text }) {
        let defaultType: ISnackbarType = snackbarTypes[0];
        let newType: ISnackbarType = snackbarTypes.find((type: ISnackbarType) => type.category == typeName) || defaultType;

        let newSnakcbar: ISnackbar = {
            type: newType,
            text: text
        };

        context.commit('SET_SNACKBAR', newSnakcbar);
    },
    setAnnualPlan(context: any, ap: AnnualPlan) {
        context.commit('SET_ANNUAL_PLAN', ap);
    },
    setGrant(context: any, gt: Grant) {
        context.commit('SET_GRANT', gt);
    },
    setDonorList(context: any, dl: IDonor[]) {
        context.commit('SET_DONOR_LIST', dl);
    },

    /**
     * Set a new active module.
     * @param context object that exposes the same set of methods/properties on the store instance
     * @param mod new active module
     */
    setActiveModule(context: any, mod: ImsModule) {
        context.commit('SET_ACTIVE_MODULE', mod);
    }

}

const appState = {
    state,
    getters,
    mutations,
    actions
}
export default appState;