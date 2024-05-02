import { IAccount, AccountStatus } from '../models/authtypes';
import { ActionTree, MutationTree, StoreOptions, Module, GetterTree } from 'vuex';
import { User, AuthApi, Configuration } from '../axiosapi';
import Axios from 'axios';
import { translate } from '../i18n';
import * as FormMixin from "../mixins/FormMixin";

import * as msal from "@azure/msal-browser";
import { debug } from 'webpack';
import axios from 'axios';

export const state: IAccount = {
    status: AccountStatus.NotLogged,
    user: null
}

const getters: GetterTree<IAccount, any> = {
    isLoggedIn(state) {
        if (state.user && state.user.token) {
            return true;
        }
        return false;
    },
    getToken(state): string | null {
        if (state.user && state.user.token) {
            return state.user.token;
        }
        return null;
    },
    loggedUser(state): User | null {
        if (state.user) {
            return state.user;
        }
        return null;
    },
    getApiConfig(state, getters): Configuration {
        const config: Configuration = {
            // basePath: 'http://localhost:5000'
            // basePath: 'https://w2012.blogic.it/jrs'
            basePath: process.env.VUE_APP_BASE_PATH ? process.env.VUE_APP_BASE_PATH : ''
        };
        const token: string = getters.getToken;
        if (token) {
            config.baseOptions = { headers: { Authorization: 'Bearer ' + token } };
        }
        return config;
    },
    isLogging(state): boolean {
        return state.status == AccountStatus.LoggingIn;
    },
    isLoggingOut(stete): boolean {
        return state.status == AccountStatus.LoggingOut;
    }
}

const mutations: MutationTree<IAccount> = {
    logging(s: IAccount, p: any) {
        s.status = AccountStatus.LoggingIn;
    },
    loggedin(s: IAccount, user: User) {
        s.status = AccountStatus.LoggedIn
        s.user = user;
    },
    logoutM(s: IAccount) {
        s.status = AccountStatus.NotLogged;
        s.user = null;
    },
    loggingOut(s: IAccount){
        s.status = AccountStatus.LoggingOut;
    },
    clearUser(s: IAccount){
        s.user = null;
    }
}

const actions: ActionTree<IAccount, any> = {

    /**
     * Login using Azure AD.
     * If "accountIdentifier" is supplied, then the method will use it
     * otherwise it will try to authenticate using the "useName".
     * @param context - object that exposes the same set of methids/properties on the store instance
     * @param userName - email address used as username qhen authenticating on Azure AD
     * @param accountIdentifier - accountIdentifier recieved by Azure AD autentication
     */
    loginAzure(context, { userName, accountIdentifier ,MsalObject}){
        const config: Configuration = context.getters["getApiConfig"];
        const api: AuthApi = new AuthApi(config);
        
        context.commit('logging');
        

        return new Promise( (resolve, reject) => {
            if(accountIdentifier){
                resolve(api.loginFromAzure(accountIdentifier))       
            }else if(userName){
                resolve(api.loginFromAzure(userName, true));
            }
            else{
                reject({ response:{ data: {message: 'Error: Bad LogIn credentials.'} } } );
            }
        })
            // Set tokens
            .then( (res:any) => {
                let user = res.data;
                if(user && user.token){
                    context.commit('loggedin', user);
                    context.dispatch('setUserUid', user.userUid, {root:true});
                    localStorage.setItem('jrs-user-token', user.token);
                    localStorage.setItem('jrs-refresh-token', user.refreshToken || '');
                }

                Axios.defaults.headers.common['Authorization'] = user.token;

                return res.data;
            })
            // Set user menu tree
            .then( (user:User) =>   {
                return context.dispatch('setUserMenu', user.userUid, {root:true})
                .then( () => {
                    return user;
                });
            })
            // Set user locale 
            .then( user => {
                return context.dispatch('setUserLocale', user.localCode || 'en', {root:true})
                    .then( () => {
                        return user;
                    });
            })
            // Set user office list
            .then( user => {
                return context.dispatch('setUserOfficeList', null, {root:true})
                    .then( () => {
                        return user;
                    }).catch((err)=>{
                        console.log(err)
                    });
                    
            })
            // Set user role list
            // .then( user => {
            //     return context.dispatch('setUserRoleList', null, {root:true})
            //         .then( () => {
            //             return user;
            //         });
            // })
            .catch( err => {
                let erroMsg = 'Error'
                if( err.response && err.response.data && ( err.response.data.message || err.response.data.translationKey ) ){
                    erroMsg = err.response.data.translationKey ? translate(err.response.data.translationKey) : err.response.data.message;
                }
                context.commit('logoutM');
                context.dispatch('showNewSnackbar', { typeName: 'error', text: erroMsg }, {root: true});
                MsalObject.logout();
                return err;
            });
    },

    //login({ dispatch, commit, rootGetters }, { username, password }) {
    login(context, { username, password }) {
        const config: Configuration = context.getters["getApiConfig"];
        const api: AuthApi = new AuthApi(config);

        context.commit('logging');
        let headers:any = {
            "Content-Type": "",
            "accept":"text/plain",
            "Sec-Fetch-Mode": "cors"
        };

        // commit('clearErrorMsg', null, { root: true });
        
        return api.login(username, password)
            .then( res => {
                let user = res.data;
                if(user && user.token){
                    context.commit('loggedin', user);
                    localStorage.setItem('jrs-user-token', user.token);
                    localStorage.setItem('jrs-refresh-token', user.refreshToken || '');
                    
                }

                Axios.defaults.headers.common['Authorization'] = user.token;

                return res.data;
            })
            .then( (user:User) =>   {
                return context.dispatch('setUserMenu', user.userUid, {root:true})
                .then( () => {
                    return user;
                });
            })
            .then( user => {
                return context.dispatch('setUserLocale', user.localCode || 'en', {root:true})
                    .then( () => {
                        return user;
                    });
            })
            .catch( err => {
                let erroMsg = 'Error'
                if( err.response.data && ( err.response.data.message || err.response.data.translationKey ) ){
                    erroMsg = err.response.data.translationKey ? translate(err.response.data.translationKey) : err.response.data.message;
                }
                context.dispatch('showNewSnackbar', { typeName: 'error', text: erroMsg }, {root: true});
                return err;
            });
    },
    /**
     * Logout the user.
     * The user will be logged out of the JRS IMS application as well as using MSAL.
     * @param context - object that exposes the same set of methids/properties on the store instance
     * @param payload - object expected in form { vm: Object }. vm is the reference to the Vue instance 
     */
    logout(context, payload) {
        // Log user out of IMS
        context.commit('loggingOut');
        context.commit('clearUser');
        localStorage.removeItem('jrs-user-token');

        // Log user out of MSAL
        const myMSALObj = payload.vm.$msal;
        myMSALObj.logout();

        context.commit('logoutM');
    }
}


const namespaced: boolean = true;
const auth: Module<IAccount, any> = {
    namespaced,
    state,
    actions,
    getters,
    mutations
};

/*
**************** LOGIN MICROSOFT GRAPH *************************
**Login-process to get access to Microsoft Graph through MSAL **
**************** LOGIN MICROSOFT GRAPH *************************
*/

/**
     * Function that give all Fileds that we need to get access to Microsoft Graph 
     * the fields are:
     * - tenant = The tenant of APP registred to Azure Portal
     * - authority_url = URL del provider di identità (l' istanza) e destinatari dell'accesso per l'applicazione. L'istanza e i destinatari di accesso, se concatenati, costituiscono l' autorità. The url in which there is the tenant of APP , example: "https://login.microsoftonline.com/"+tenant;
     * - grant_type= The type of grant for example : 'client_credentials';
     * - client_id = The id of client 
     * - client_secret = The secret id of app
     * - resource= The resource, in this case is not used, for example :  '00000003-0000-0ff1-ce00-000000000000/jesuitrefugeeservice720.sharepoint.com@'+tenant;
     * - scope_url = The scope of APP, the token that you received will respect all authorizations that are inclused in the scope url. default : "https://graph.microsoft.com/.default"
     * - username = the usernam eof user regostred to azure portal.
*/
/*
function getCredentialForMicrosfotGraph(){
    let clientId:String|undefined = process.env.VUE_APP_CLIENT_ID;
    let tenantId:String|undefined = process.env.VUE_APP_TENANT_ID;
    //IMSDEV
    //clientId: 'bf1c0434-10e7-480b-94d5-3bb7eab10189',
    //tenantId: '5759d742-e295-4457-b9da-e23691927426',
    //LOCAL JRSAPP
    // clientId: '866dab3e-3ebd-4cd4-9876-b0f487c7848f',
    // tenantId: '9f4e3457-d5dc-4907-af7a-0ddd9936b102',
    //W2012
    //  clientId: '7bc85786-a580-46b8-a3d4-583a4e15739e',
    // tenantId: '9f4e3457-d5dc-4907-af7a-0ddd9936b102',

    var body = []
    // var tenant = tenantId ? tenantId : '5759d742-e295-4457-b9da-e23691927426'
    var tenant = '5759d742-e295-4457-b9da-e23691927426'
    // var client_id = clientId ? clientId :'bf1c0434-10e7-480b-94d5-3bb7eab10189'
    var client_id = 'bf1c0434-10e7-480b-94d5-3bb7eab10189'
    var authority_url  = "https://login.microsoftonline.com/"+tenant;
    var grant_type='client_credentials';
    // var client_secret = "2be44d15-76d6-4cb0-878b-0fbc21c3e1cf"
    // var resource='00000003-0000-0ff1-ce00-000000000000/jesuitrefugeeservice720.sharepoint.com@'+tenant;
    var scope_url ="https://graph.microsoft.com/.default"
    var username ="vat100.imsblogic@jrsexternal.info";
    body=[{
        authority:authority_url,
        username:username,
        grant_type:grant_type,
        client_id:client_id,
        // client_secret:client_secret,
        // resource:resource,
        scopes:scope_url
    }];
    // body = [{
    //     authority:"https://login.microsoftonline.com/925bd55b-0521-4003-b7c9-c3e68367d724",
    //     username:'n.migliore@blogic.it',
    //     grant_type:grant_type,
    //     client_id:'70fcabad-89a4-47a7-8b9d-c8e892944e96',
    //     // client_secret:client_secret,
    //     // resource:resource,
    //     scopes:scope_url
    // }]

    return body
  }

  /**
     * Function that give the access oken using Msal library 
     * @param authority url to access to azure portal that has sharepoint as application
     * @param username username of account related to azure portal
     * @param grant_type the grant type of rest 
     * @param client_id the client id of application (in our case share point application)
     * @param scopes_url the scope url to do rest to api
     * The function , one it is called, give the possibility to open an popup and insert credentials in order to get
     * the access token
    */
   /*
 function getAccessTokenMicrosoftGraph(authority_url:any,username:any,grant_type:any,client_id:any,scopes_url:any){      
    var config = {
        auth: {
            clientId: client_id,
            authority: authority_url,
            requireAuthOnInitialize: true,
            validateAuthority: false,
        }
    };

    // var myMSALObj = new msal.UserAgentApplication(config);
    const myMSALObj = new msal.PublicClientApplication(config);
    
    let accessTokenRequest = {
        scopes: [
                scopes_url
                ],
        loginHint: username,
    
    
    extraQueryParameters: { 
            grant_type: grant_type,
            domain_hint: 'organizations',
            },

    }

    return myMSALObj.acquireTokenSilent(accessTokenRequest)
    .then(accessTokenResponse => {
        // Do something with the tokenResponse
        let accessToken = accessTokenResponse.accessToken;
        localStorage.setItem('jrs-user-microsoft-graph-token', accessToken);
        return accessToken;
    }).catch(async (error) => {
        if (error.name === "InteractionRequiredAuthError") {
            // fallback to interaction when silent call fails
            return myMSALObj.acquireTokenPopup(accessTokenRequest).then(function(accessTokenResponse) {
            // Acquire token silent success
            // call API with token 
            let accessToken = accessTokenResponse.accessToken;
            localStorage.setItem('jrs-user-microsoft-graph-token', accessToken);
            return accessToken;
            });
            
        }
    }).catch(error => {
        console.log(error);
    });         
        
  }
  /**
     * Function that give the Picture of profile with MG
     * @param token The token to access to MG 
     * The function , one it is called, give the possibility to obtain the picture of profile of user on MG
    */
   /*
  function getAvatarUserMG(token:any){
    let url = "https://graph.microsoft.com/v1.0/me/photo/$value"
    Axios.get(url,{
      headers:{ 
        'Authorization': `Bearer ${token}`
      },responseType: 'arraybuffer'                               
      }).then((response:any) => {
            let avatar_b64 =  new Buffer(response.data, 'binary').toString('base64');
            let avatar = `data:image/jpeg;base64,${avatar_b64}`;
            localStorage.setItem('jrs-user-avatar',avatar);
        }).catch(function (error:any) {
          console.log(error);
        })
}*/

export default auth;