import Vue from 'vue';
import Vuex, { ModuleTree } from 'vuex';
import { StoreOptions, MutationTree, GetterTree, ActionTree } from 'vuex';
import appState from './appState';
import auth from './auth';
import userRights from './userRights';
import apiHandler from './apiHandler';


Vue.use(Vuex);

interface IState {
    version: string;
}
export const state: IState = {
    version: '1.0.0'
};
const getters: GetterTree<IState, any> = {

}
const mutations: MutationTree<IState> = {

}
const actions: ActionTree<IState, any> = {

}
const modules: ModuleTree<IState> = {
    'auth': auth,
    'userRihts': userRights,
    'appState': appState,
    'apiHandler':apiHandler
}
const store: StoreOptions<IState> = {
    state,
    mutations,
    actions,
    getters,
    modules
};
export default new Vuex.Store<IState>(store);
