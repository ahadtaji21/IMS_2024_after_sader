import { ActionTree, MutationTree, StoreOptions, Module, GetterTree } from 'vuex';
import { i18n } from '../i18n';
import { MenusApi, GenericSqlApi, GenericSqlPayload, Configuration } from '../axiosapi';
import { MenuItem, moduleColors } from "../models/MenuItem";
import { SqlActionType } from "../axiosapi/api";

const userRights = {
    state: {
        userUid: '',
        userLocale: 'en',
        userMenuItems: [],
        userOfficeList: [],
        currentOffice: { HR_OFFICE_ID: null, HR_OFFICE_LEGAL_NAME: null, HR_OFFICE_CODE: null },
        userRoleList: [],
        currentRole: { ROLE_ID: null, DESCR: null,ROLE_CODE:null }
    },
    getters: {
        getUserUid(state: any): String {
            return state.userUid;
        },
        getUserMenu(state: any): Array<MenuItem> | null {
            return state.userMenuItems;
        },
        getUserLocale(state: any): string {
            return state.userLocale;
        },
        getUserModules(state: any): Array<any> | null {
            let flatMenus: Array<MenuItem> = getFlatMenuItems(state.userMenuItems);
            let modules = flatMenus
                .filter((menu: MenuItem) => menu.moduleId && !menu.isHidden)
                .map((menu: MenuItem) => {
                    let color: any = moduleColors.find(mc => mc.moduleName == menu.moduleName);
                    return {
                        moduleId: menu.moduleId,
                        moduleName: menu.moduleName,
                        moduleCode: menu.moduleCode,
                        moduleColor: color ? color.moduleColor : undefined
                    }
                }
                )
                .reduce((prev: Array<any>, module: any) => {
                    let foundIndex = prev.findIndex((item: any) => item.moduleId == module.moduleId);
                    if (foundIndex == -1) {
                        prev.push(module);
                    }
                    return prev;
                }, [])
                // .filter((module: any) => module.moduleId)
                .sort((a: any, b: any) => a.moduleName < b.moduleName ? -1 : 1);
            return modules;
        },
        getUserOfficeList(state: any): Array<any> {
            return state.userOfficeList;
        },
        getCurrentOffice(state: any): any {
            return state.currentOffice;
        },
        getUserRoleList(state: any): Array<any> {
            return state.userRoleList;
        },
        getCurrentRole(state: any): any {
            return state.currentRole;
        },
    },
    mutations: {
        SET_USER_UID(state: any, uid: string) {
            state.userUid = uid;
        },
        SET_USER_LOCALE(state: any, lang: string) {
            state.userLocale = lang;
        },
        SET_MENU_LIST_FOR_RIGHTS(state: any, menuTree: Array<MenuItem>) {
            state.userMenuItems = menuTree;
        },
        SET_MENU_LIST_FOR_LANGUAGE(state: any, payload: { menuItem: MenuItem, newLang: string }) {
            let menuItem: MenuItem, newLang: string;
            ({ menuItem, newLang } = payload);              //Destructure payload

            menuItem.path = menuItem.url != null ? `/${newLang}/${menuItem.url}` : null;
            menuItem.label = `${i18n.t(menuItem.labelKey)}`;
        },
        SET_USER_OFFICE_LIST(state: any, officeList: Array<any>) {
            state.userOfficeList = officeList;
        },
        SET_USER_ROLE_LIST(state: any, roleList: Array<any>) {
            state.userRoleList = roleList;
        },
        SET_CURRENT_OFFICE(state: any, office: any) {
            Object.assign(state.currentOffice, office);
            // Save current office in cookies
            document.cookie = `cur_office=${JSON.stringify(office)}`;
        },
        SET_CURRENT_ROLE(state: any, role: any) {
            Object.assign(state.currentRole, role);
        }
    },
    actions: {
        /**
         * Sets the active user UID
         * @param context object that exposes the same set of methids/properties on the store instance
         * @param userUID Unique identifier of the user
         */
        setUserUid(context: any, userUID: string) {
            context.commit('SET_USER_UID', userUID);
            return;
        },
        /**
         * Updates the menu item's labels and paths
         * @param context 
         * @param lang language code to use to update the menu items
         */
        updateMenuForLang(context: any, lang: string) {
            let flat = getFlatMenuItems(context.getters['getUserMenu']);
            flat.forEach((menuItem) => {
                context.commit('SET_MENU_LIST_FOR_LANGUAGE', { menuItem: menuItem, newLang: lang });
            });
            return { msg: "Update menu labels" };
        },
        /**
         * Sets the users language and updates the menu paths and labels
         * @param context object that exposes the same set of methids/properties on the store instance
         * @param lang language code to set in i18n object.
         */
        setUserLocale(context: any, lang: string) {
            context.commit('SET_USER_LOCALE', lang);
            //Update i18n locale
            i18n.locale = lang;
            //Update the menuList
            // let flat = getFlatMenuItems(context.getters['getUserMenu']);
            // flat.forEach((menuItem) => {
            //     context.commit('SET_MENU_LIST_FOR_LANGUAGE', { menuItem: menuItem, newLang: lang });
            // });
            // return { msg: "Set user locale" };
            return context.dispatch('updateMenuForLang', lang)
                .then((res: any) => {
                    return { msg: `${res.msg}\nSet user locale` };
                });
        },
        /**
         * Sets the menu list for the user depending on their user-rights
         * @param context object that exposes the same set of methids/properties on the store instance
         * @param userUID Unique identifier of the user
         */
        setUserMenu(context: any, userUID: string) {
            const config: Configuration = context.getters["auth/getApiConfig"];
            const api: MenusApi = new MenusApi(config);

            return api
                .apiMenusUserGuidGet(userUID, config.baseOptions)
                .then(res => {
                    // let menuTree = unflattenMenuList(res.data);
                    let menuTree = res.data.map(m => {
                        return {
                            id: m.id,
                            path: null,
                            label: null,
                            // icon: m.iconCode,
                            iconCode: m.iconCode,
                            url: m.url,
                            labelKey: m.labelKey,
                            parentMenuId: m.parentMenuId,
                            subMenuList: m.inverseParentMenu ? m.inverseParentMenu.sort((a: any, b: any) => a.ordinalPosition - b.ordinalPosition) : null,
                            ordinalPosition: m.ordinalPosition,
                            moduleId: m.moduleId,
                            moduleName: m.moduleName,
                            moduleCode: m.moduleCode,
                            isHidden: m.isHidden
                        }
                    }).sort((a: any, b: any) => a.ordinalPosition - b.ordinalPosition);

                    context.commit('SET_MENU_LIST_FOR_RIGHTS', menuTree);
                    // context.dispatch('setUserLocale', i18n.locale);
                    return { msg: "Set user menu" };
                })
                .catch(err => {
                    console.error(err);
                })
        },
        caclculateFlatMenu(context: any) {
            return getFlatMenuItems(context.getters['getUserMenu']);
        },
        setUserLogin(context: any, isLogged: boolean) {
            context.commit('SET_USER_IS_LOGGED', isLogged);
        },

        /**
         * Recover and set the office list for the user.
         * @param context object that exposes the same set of methids/properties on the store instance
         */
        setUserOfficeList(context: any) {
            const config: Configuration = context.getters["auth/getApiConfig"];
            const api: GenericSqlApi = new GenericSqlApi(config);

            let jsonPayload = {
                // user_uid: context.getters['getUserUid']
                user_uid: context.rootGetters['auth/loggedUser'].userUid
            };

            let payload: GenericSqlPayload = {
                spName: "HR_SP_GET_OFFICE_FOR_USER",
                actionType: SqlActionType.NUMBER_3,
                jsonPayload: JSON.stringify(jsonPayload),
                userUid: context.getters['getUserUid'],
                officeId: context.getters['getCurrentOffice'].HR_OFFICE_ID
            };

            return api.genericSqlSPCall(payload)
                .then((res: any) => {
                    context.commit('SET_USER_OFFICE_LIST', res.data.rows);
                })
                .catch((err: any) => {
                    console.log(err);
                    return err;
                });
        },
        /**
           * Recover and set the role list for the user.
           * @param context object that exposes the same set of methids/properties on the store instance
           */
        // setUserRoleList(context:any){
        //     const config: Configuration = context.getters["auth/getApiConfig"];
        //     const api:GenericSqlApi = new GenericSqlApi(config);

        //     let jsonPayload = {
        //         // user_uid: context.getters['getUserUid']
        //         user_uid: context.rootGetters['auth/loggedUser'].userUid
        //     };

        //     let payload:GenericSqlPayload = {
        //         spName: "HR_SP_GET_ROLE_FOR_USER",
        //         actionType: SqlActionType.NUMBER_3,
        //         jsonPayload: JSON.stringify(jsonPayload),
        //         userUid: context.getters['getUserUid'],
        //         officeId: context.getters['getCurrentOffice'].HR_OFFICE_ID
        //     };

        //     return api.genericSqlSPCall(payload)
        //         .then( (res:any) => {
        //             context.commit('SET_USER_ROLE_LIST', res.data.rows);
        //         })
        //         .catch( (err:any) => {
        //             console.log(err);
        //             return err;
        //         });
        // },

        setCurrentOffice(context: any, office: any) {
            let localThis: any = this as any;
            let prom = new Promise((resolve, reject) => {
                if (office.HR_OFFICE_ID && office.HR_OFFICE_LEGAL_NAME) {
                    context.commit('SET_CURRENT_OFFICE', office);


                    resolve("Set new active office.");
                } else {
                    reject(new Error('Provided object is not a valid "Office".'));
                }
            }).then((res: any) => {
                //setUserRoleList
                context.commit('SET_CURRENT_ROLE', undefined);
                const config: Configuration = context.getters["auth/getApiConfig"];
                const api: GenericSqlApi = new GenericSqlApi(config);

                let jsonPayload = {
                    // user_uid: context.getters['getUserUid']
                    user_uid: context.rootGetters['auth/loggedUser'].userUid
                };

                let payload: GenericSqlPayload = {
                    spName: "HR_SP_GET_ROLE_FOR_USER",
                    actionType: SqlActionType.NUMBER_3,
                    jsonPayload: JSON.stringify(jsonPayload),
                    userUid: context.getters['getUserUid'],
                    officeId: context.getters['getCurrentOffice'].HR_OFFICE_ID
                };

                return api.genericSqlSPCall(payload)
                    .then((res: any) => {
                        context.commit('SET_USER_ROLE_LIST', res.data.rows);
                        if (res.data && res.data.rows && res.data.rows.length > 0) {
                            //Object.assign(context.state.currentRole, res.data.rows[0]);
                            //localThis.setCurrentRole(context,res.data.rows[0]);
                            context.commit('SET_CURRENT_ROLE', res.data.rows[0]);
                        }

                        else {
                            //localThis.setCurrentRole(context,undefined);
                            let norole:any={} ;
                            norole.ROLE_ID=0;
                            norole.DESCR="";
                            norole.ROLE_CODE="";
                            context.commit('SET_CURRENT_ROLE', norole);
                        }
                    })
                    .catch((err: any) => {
                        console.log(err);
                        return err;
                    });
            }).finally(() => {
            return prom;
            });
        },
        setCurrentRole(context: any, role: any) {
            let prom = new Promise((resolve, reject) => {
                context.commit('SET_CURRENT_ROLE', role);
            });
            return prom;
        }
    },
}

function getFlatMenuItems(mList: Array<MenuItem>) {
    let newList: Array<MenuItem> = [];
    mList = mList ? mList : [];

    newList = [...mList];
    mList.filter(m => { return m.subMenuList }).forEach(item => {
        let sub: Array<any> = item.subMenuList ? item.subMenuList : []
        newList = [...newList, ...(getFlatMenuItems(sub))]
    });
    return newList;
}

/**
 * Takes a flat list of menuItems and returns a structired menu tree
 * @param list flat list of items to turn into tree
 * @param parent nex item to build children for
 * @param tree tree being built
 */
function unflattenMenuList(list: Array<any>, parent?: any, tree?: Array<MenuItem>) {
    tree = typeof tree !== 'undefined' ? tree : [];
    parent = typeof parent !== 'undefined' ? parent : { id: 0 };

    //Get children of current item
    let children: Array<MenuItem> = list
        .filter(child => child.parentMenuId == parent.id)
        .map(child => {
            return {
                path: null,
                label: null,
                // icon: child.iconCode,
                iconCode: child.iconCode,
                url: child.url,
                labelKey: child.labelKey,
                parentMenuId: child.parentMenuId,
                id: child.id,
                subMenuList: child.subMenuList
            }
        });

    if (children.length > 0) {
        if (parent.id == 0) {
            tree = children;
        } else {
            parent['subMenuList'] = children;
        }
        //Iterate
        children.forEach(child => {
            unflattenMenuList(list, child);
        });
    }

    return tree;
}


export default userRights;