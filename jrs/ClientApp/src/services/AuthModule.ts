import { redirectPolicy } from "@azure/core-http";
import { PublicClientApplication, SilentRequest, AuthenticationResult, Configuration, LogLevel, AccountInfo, InteractionRequiredAuthError, RedirectRequest, PopupRequest, EndSessionRequest } from "@azure/msal-browser";
// import { UIManager } from "./UIManager";
import { SsoSilentRequest } from "@azure/msal-browser/dist/request/SsoSilentRequest";

import axios from 'axios';


/**
 * Configuration class for @azure/msal-browser: 
 * https://azuread.github.io/microsoft-authentication-library-for-js/ref/msal-browser/modules/_src_config_configuration_.html
 */
const MSAL_CONFIG: Configuration = {
    
    auth: {
        //clientId: process.env.VUE_APP_CLIENT_ID+'', // 'bf1c0434-10e7-480b-94d5-3bb7eab10189',
        clientId: '',
        redirectUri: '', //defaults to application start page
        postLogoutRedirectUri: '',
        authority: "https://login.microsoftonline.com/common"
      },
      cache: {
        cacheLocation: "sessionStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    },
    system: {
        loggerOptions: {
            loggerCallback: (level, message, containsPii) => {
                if (containsPii) {	
                    return;	
                }	
                switch (level) {	
                    case LogLevel.Error:	
                        console.error(message);	
                        return;	
                    case LogLevel.Info:	
                        // console.info(message);	
                        return;	
                    case LogLevel.Verbose:	
                        // console.debug(message);	
                        return;	
                    case LogLevel.Warning:	
                        // console.warn(message);	
                        return;	
                }
            }
        }
    }
};

/**
 * AuthModule for application - handles authentication in app.
 */
export class AuthModule {

    private myMSALObj: PublicClientApplication; // https://azuread.github.io/microsoft-authentication-library-for-js/ref/msal-browser/classes/_src_app_publicclientapplication_.publicclientapplication.html
    private account: AccountInfo | null; // https://azuread.github.io/microsoft-authentication-library-for-js/ref/msal-common/modules/_src_account_accountinfo_.html
    private loginRedirectRequest: RedirectRequest; // https://azuread.github.io/microsoft-authentication-library-for-js/ref/msal-browser/modules/_src_request_redirectrequest_.html
    private loginRequest: PopupRequest; // https://azuread.github.io/microsoft-authentication-library-for-js/ref/msal-browser/modules/_src_request_popuprequest_.html
    private profileRedirectRequest: RedirectRequest;
    private profileRequest: PopupRequest;
    private mailRedirectRequest: RedirectRequest;
    private mailRequest: PopupRequest;
    private sharePointRequest: PopupRequest;
    private silentProfileRequest: SilentRequest; // https://azuread.github.io/microsoft-authentication-library-for-js/ref/msal-browser/modules/_src_request_silentrequest_.html
    private silentMailRequest: SilentRequest;
    private silentLoginRequest: SsoSilentRequest;
    private silentSharepointRequest: SilentRequest;
    private testRequest:SilentRequest;

    constructor(envConfig:any) {
        const msalConfig:Configuration = { ...MSAL_CONFIG };
        // Setup environment auth options
        msalConfig.auth.clientId = envConfig.VUE_APP_CLIENT_ID;
        msalConfig.auth.redirectUri = envConfig.VUE_APP_REDIRECT_URI;
        msalConfig.auth.postLogoutRedirectUri = envConfig.VUE_APP_REDIRECT_URI;

        this.myMSALObj = new PublicClientApplication(msalConfig);
        // this.myMSALObj = new PublicClientApplication(MSAL_CONFIG);
        this.account = null;

        this.loginRequest = {
            scopes: ["openid", "profile", "User.Read"]
        };

        this.loginRedirectRequest = {
            ...this.loginRequest,
            redirectStartPage: window.location.href
        };

        this.profileRequest = {
            scopes: ["User.Read"]
        };

        this.profileRedirectRequest = {
            ...this.profileRequest,
            redirectStartPage: window.location.href
        };

        // Add here scopes for access token to be used at MS Graph API endpoints.
        this.mailRequest = {
            scopes: ["Mail.Read"]
        };

        this.mailRedirectRequest = {
            ...this.mailRequest,
            redirectStartPage: window.location.href
        };

        this.sharePointRequest = {
            scopes: ["User.Read","Files.Read.All","Files.Read","Files.ReadWrite","Files.ReadWrite.All","Sites.ReadWrite.All"]
        };

        this.silentProfileRequest = {
            scopes: ["openid", "profile", "User.Read"],
            forceRefresh: false
        };

        this.silentMailRequest = {
            scopes: ["openid", "profile", "Mail.Read"],
            forceRefresh: false
        };

        this.silentSharepointRequest = {
            scopes: ["openid","profile", "User.Read","Files.Read.All","Files.Read","Files.ReadWrite","Files.ReadWrite.All","Sites.ReadWrite.All"]
        };

        this.silentLoginRequest = {
            loginHint: "IDLAB@msidlab0.ccsctp.net"
        },

        this.testRequest = {
          scopes: ["user.read", "group.read.all"]
      };
    }

    /**
     * Calls getAllAccounts and determines the correct account to sign into, currently defaults to first account found in cache.
     * TODO: Add account chooser code
     * 
     * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-common/docs/Accounts.md
     */
    private getAccount(): AccountInfo | null {
        // need to call getAccount here?
        const currentAccounts = this.myMSALObj.getAllAccounts();
        if (currentAccounts === null) {
            console.log("No accounts detected");
            return null;
        }

        if (currentAccounts.length > 1) {
            // Add choose account code here
            console.log("Multiple accounts detected, need to add choose account code.");
            return currentAccounts[0];
        } else if (currentAccounts.length === 1) {
            return currentAccounts[0];
        }

        return null;
    }

    /**
     * Checks whether we are in the middle of a redirect and handles state accordingly. Only required for redirect flows.
     * 
     * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/initialization.md#redirect-apis
     */
    loadAuthModule(): Promise<any> {
        return this.myMSALObj.handleRedirectPromise().then((resp: AuthenticationResult | null) => {
            this.handleResponse(resp);
        }).catch(console.error);
    }

    /**
     * Handles the response from a popup or redirect. If response is null, will check if we have any accounts and attempt to sign in.
     * @param response 
     */
    handleResponse(response: AuthenticationResult | null) {
        if (response !== null) {
            this.account = response.account;
        } else {
            this.account = this.getAccount();
        }
        this.myMSALObj.setActiveAccount(this.account); 
        // if (this.account) {
        //     UIManager.showWelcomeMessage(this.account);
        // }
    }

    /**
     * Get user profile info.
     */
    async getUserProfile(){
      // Get access token for specific rights
      const token = await this.getTokenPopup(this.silentProfileRequest, this.profileRequest);

      return axios.get('https://graph.microsoft.com/v1.0/me/', {
        headers:{
          'Accept': 'application/json',
          'Authorization': 'Bearer ' + token
        }
      })
      .then( (res:any) => {
        console.log(JSON.stringify(res));
        return res;
      })
      .catch((err:any) => {
        console.log(err);
      })
    }

    /**
     * Recover user avatar
     * @param vers = the version of Rest call API Microsfot Graph:
     * v1.0 = the user has a mailbox 
     * beta = the user might not has a mailbox
     */
    async getUserAvatar(vers:string){
        // Get access token for specific rights
        const token = await this.getTokenPopup(this.silentProfileRequest, this.profileRequest);
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        return axios.get('https://graph.microsoft.com/'+vers+'/me/photo/$value', {
            headers:{
              'Accept': 'application/json',
              'Authorization': 'Bearer ' + token,
              'content-type': 'image/jpeg'
            },
            responseType: 'arraybuffer'
        });
    }


    /*
     * Create container blob storage
     * @param:
     * v1.0 = the user has a mailbox 
     * beta = the user might not has a mailbox
    */
    async createContainerBlobStorage(){
        // Get access token for specific rights
        const token = await this.getSharePointTokenPopup();
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        return axios.put('https://.blob.core.windows.net/mycontainer?restype=container', {
            headers:{
              'Accept': 'application/json',
              'Authorization': 'Bearer ' + token,
              'content-type': 'image/jpeg'
            },
            responseType: 'arraybuffer'
        });
    }


       /*
     * Read File sharepoint
     * @param:
     * FileName = the file name
     * idDrive = the Id of drive folder
    */
       async ReadFileSP(path:string){
        // Get access token for specific rights
        const token = await this.getSharePointTokenPopup();
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        //Complete request :
        //https://graph.microsoft.com/v1.0/sites/ce07eaec-110f-407d-b19a-50f5882b7c30,e05181d1-d65a-4d4d-807f-97f1edfa8586/drives/b!7OoHzg8RfUCxmlD1iCt8MNGBUeBa1k1NgH-X8e36hYYEH283gg3HQ51Ht7Ax4cKB/root:/Documento.docx
        return axios.get('https://graph.microsoft.com/v1.0/sites/root/'+path, {
            headers:{
              'Accept': 'application/json',
              'Authorization': 'Bearer ' + token
            }
        });
    }

      /*
      * Get Id drives
      */
       async GetIdDrivesSP(){
        // Get access token for specific rights
        const token = await this.getSharePointTokenPopup();
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        //Complete request :
        //https://graph.microsoft.com/v1.0/sites/ce07eaec-110f-407d-b19a-50f5882b7c30,e05181d1-d65a-4d4d-807f-97f1edfa8586/drives/b!7OoHzg8RfUCxmlD1iCt8MNGBUeBa1k1NgH-X8e36hYYEH283gg3HQ51Ht7Ax4cKB/root:/Documento.docx
        return axios.get('https://graph.microsoft.com/v1.0/sites/root/drives/', {
            headers:{
              'Accept': 'application/json',
              'Authorization': 'Bearer ' + token
            }
        });
    }

    
      /*
      * Get Documents from IDs
      * @param:
      * idDrive = the Id of drive folder
      */
       async GetDocumentsIdDriveSP(IdDrive:string, path : string){
        // Get access token for specific rights
        const token = await this.getSharePointTokenPopup();
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        return axios.get('https://graph.microsoft.com/v1.0/sites/root/drives/'+IdDrive +'/root/children', {
            headers:{
              'Accept': 'application/json',
              'Authorization': 'Bearer ' + token
            }
        });
    }

    /*
      * Get Documents from IDs
      * @param:
      * idDrive = the Id of drive folder
      */
    async GetDocumentsFolderSP(path : string,root:boolean){
        // Get access token for specific rights
        const token = await this.getSharePointTokenPopup();
        let url = "";
        if(root){
            url = 'https://graph.microsoft.com/v1.0/sites/root'+path+'/children'
        }else{
            url = 'https://graph.microsoft.com/v1.0/sites/root'+path+':/children';
        }
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        return axios.get(url, {
            headers:{
              'Accept': 'application/json',
              'Authorization': 'Bearer ' + token
            }
        });
    }

    
      /*
      * Upload document SharePoint
      * @param:
      * idDrive = the Id of drive folder,
      * fileName = name of file to upload,
      * file = the blob (data) of file (body)
      */
      async UploadFileSP(path:string, fileName:string, file:any){
        // Get access token for specific rights
    
        const token = await this.getSharePointTokenPopup();
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        return axios.put('https://graph.microsoft.com/v1.0/'+path+'/'+fileName+':/content',file, {
            headers:{
              'Authorization': 'Bearer ' + token,
              'Content-Type': 'multipart/form-data'
            }
        });
    }



     /*
      * Create Folder Sharepoint
      * @param:
      * idDrive = the Id of drive folder,
      * folderName = name of folder to create,
      * parentId = the ID of Parent new Folder 
      */
     async CreateFolderSP(IdDrive:string, folderName:string, parentId:any){
        // Get access token for specific rights

        var JsonFolder = {
            "name": folderName,
            "folder": { },
            "@microsoft.graph.conflictBehavior": "rename"
          }
    
        const token = await this.getSharePointTokenPopup();
        //MANAGE CASE API REST BETA OR V1.0 MICROSFOT GRAPH
        return axios.post('https://graph.microsoft.com/v1.0/drives/'+IdDrive+'/items/'+parentId+'/children',JsonFolder, {
            headers:{
              'Authorization': 'Bearer ' + token,
              'Content-Type': 'application/json'
            }
        });
    }
    



// --- EXAMPLES --- //

    /**
     * Calls ssoSilent to attempt silent flow. If it fails due to interaction required error, it will prompt the user to login using popup.
     * @param request 
     */
    attemptSsoSilent() {
        this.myMSALObj.ssoSilent(this.silentLoginRequest).then(() => {
            this.account = this.getAccount();
            if (this.account) {
                // UIManager.showWelcomeMessage(this.account);
            } else {
                console.log("No account!");
            }
        }).catch(error => {
            console.error("Silent Error: " + error);
            if (error instanceof InteractionRequiredAuthError) {
                this.login("loginPopup");
            }
        })
    }

    /**
     * Calls loginPopup or loginRedirect based on given signInType.
     * @param signInType 
     */
    login(signInType: string): void {
        if (signInType === "loginPopup") {
            this.myMSALObj.loginPopup(this.loginRequest).then((resp: AuthenticationResult) => {
                this.handleResponse(resp);
            }).catch(console.error);
        } else if (signInType === "loginRedirect") {
            this.myMSALObj.loginRedirect(this.loginRedirectRequest);
        }
    }

    /**
     * Logs out of current account.
     */
    logout(): void {
        let account: AccountInfo | undefined;
        if (this.account) {
            account = this.account
        }
        const logOutRequest: EndSessionRequest = {
            account
        };
        
        this.myMSALObj.logout(logOutRequest);
    }

    /**
     * Gets the token to read user profile data from MS Graph silently, or falls back to interactive redirect.
     */
    async getProfileTokenRedirect(): Promise<string|null> {
        if (this.account) {
            this.silentProfileRequest.account = this.account;
        }
        return this.getTokenRedirect(this.silentProfileRequest, this.profileRedirectRequest);
    }

    /**
     * Gets the token to read user profile data from MS Graph silently, or falls back to interactive popup.
     */
    async getProfileTokenPopup(): Promise<string|null> {
        if (this.account) {
            this.silentProfileRequest.account = this.account;
        }
        return this.getTokenPopup(this.silentProfileRequest, this.profileRequest);
    }

    /**
     * Gets the token to read mail data from MS Graph silently, or falls back to interactive redirect.
     */
    async getMailTokenRedirect(): Promise<string|null> {
        if (this.account) {
            this.silentMailRequest.account = this.account;
        }
        return this.getTokenRedirect(this.silentMailRequest, this.mailRedirectRequest);
    }

    /**
     * Gets the token to read mail data from MS Graph silently, or falls back to interactive popup.
     */
    async getMailTokenPopup(): Promise<string|null> {
        if (this.account) {
            this.silentMailRequest.account = this.account;
        }
        return this.getTokenPopup(this.silentMailRequest, this.mailRequest);
    }


      /**
     * Gets the token to read files,document from Sharepoint , using Rrest from MS Graph silently, or falls back to interactive popup.
     */
       async getSharePointTokenPopup(): Promise<string|null> {
        if (this.account) {
            this.silentSharepointRequest.account = this.account;
        }
        return this.getTokenPopup(this.silentSharepointRequest, this.sharePointRequest);
    }

    /**
     * Gets a token silently, or falls back to interactive popup.
     */
    private async getTokenPopup(silentRequest: SilentRequest, interactiveRequest: PopupRequest): Promise<string|null> {
        try {
            const response: AuthenticationResult = await this.myMSALObj.acquireTokenSilent(silentRequest);
            return response.accessToken;
        } catch (e) {
            console.log("silent token acquisition fails.");
            if (e instanceof InteractionRequiredAuthError) {
                console.log("acquiring token using redirect");
                return this.myMSALObj.acquireTokenPopup(interactiveRequest).then((resp) => {
                    return resp.accessToken;
                }).catch((err) => {
                    console.error(err);
                    return null;
                });
            } else {
                console.error(e);
            }
        }

        return null;
    }

    /**
     * Gets a token silently, or falls back to interactive redirect.
     */
    private async getTokenRedirect(silentRequest: SilentRequest, interactiveRequest: RedirectRequest): Promise<string|null> {
        try {
            const response = await this.myMSALObj.acquireTokenSilent(silentRequest);
            return response.accessToken;
        } catch (e) {
            console.log("silent token acquisition fails.");
            if (e instanceof InteractionRequiredAuthError) {
                console.log("acquiring token using redirect");
                this.myMSALObj.acquireTokenRedirect(interactiveRequest).catch(console.error);
            } else {
                console.error(e);
            }
        }

        return null;
    }
}
