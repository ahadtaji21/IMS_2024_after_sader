<template>
  <v-app>
    <!-- SNACKBAR -->
    <jrs-snackbar></jrs-snackbar>

    <!-- SHOW PROGRESS -->
    <v-content color="primary" v-if="isLogging || isLoggingOut">
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <div class="d-flex flex-row justify-center">
              <v-progress-circular
                indeterminate
                class="finder-nm"
                color="primary"
              ></v-progress-circular>
            </div>
          </v-flex>
        </v-layout>
      </v-container>
    </v-content>

    <!-- APPLICATION -->
    <div v-if="loggedUser != null">
      <!-- APP NAVIGATION-->
      <jrs-app-nav :avatar="avatar_src"></jrs-app-nav>

      <!-- APP TOP BAR -->
      <!-- <v-app-bar app clipped-left dense :color="activeModule.moduleColor + ' darken-1'" dark> -->
      <v-app-bar app clipped-left dense :color="module_color" dark>
        <v-app-bar-nav-icon @click.stop="toggleAppDrawer"></v-app-bar-nav-icon>
        <v-img
          alt="Jrs Logo"
          class="shrink mr-2"
          contain
          src="./assets/jrs-logo-white.png"
          transition="scale-transition"
          width="100"
        />
        <!-- <span class="ml-3 text-h2 font-weight-bold text-uppercase">{{activeModule.moduleName ? ` - ${activeModule.moduleName}` : ''}}</span> -->

        <v-spacer></v-spacer>
        <!-- <jrs-session-manager  :avatar="avatar_src" ></jrs-session-manager> -->

        <span class="ml-3 h2 font-weight-bold text-uppercase">{{
          activeModule.moduleName ? ` ${activeModule.moduleName}` : ""
        }}</span>

        <v-spacer></v-spacer>
        <jrs-session-manager displayChoice="user-office"></jrs-session-manager>
      </v-app-bar>

      <!-- APP CONTENT -->
      <router-view
        :key="$route.fullPath + '_' + getCurrentOffice.HR_OFFICE_ID"
      ></router-view>

      <!-- APP FOOTER -->
      <v-footer app></v-footer>
    </div>
  </v-app>
</template>

<script lang="ts">
import Vue from "vue";
import router from "./router/index";
import { mapGetters, mapActions } from "vuex";
import axios from "axios";
import { AuthApi, Configuration } from "./axiosapi";
import { i18n } from "./i18n";
// const JsonExcel = require('vue-json-excel');

import JrsAppNav from "./components/JrsAppNav.vue";
import SessionManager from "./components/SessionManager.vue";
import JrsSnackbar from "./components/JrsSnackbar.vue";
import { AuthModule } from "./services/AuthModule";
import { moduleColors } from "./models/MenuItem";

// Vue.component("downloadExcel", JsonExcel);
export default Vue.extend({
  name: "App",
  components: {
    "jrs-app-nav": JrsAppNav,
    "jrs-session-manager": SessionManager,
    "jrs-snackbar": JrsSnackbar,
  },
  data: () => ({
    drawer: true,
    showHW1: true,
    avatar_user: null,
    avatar_src: "",
    module_color: "primary",
  }),
  computed: {
    ...mapGetters({
      userLocale: "userLocale",
      activeModule: "getActiveModule",
      getCurrentOffice: "getCurrentOffice",
    }),
    ...mapGetters("auth", {
      loggedUser: "loggedUser",
      getApiConfig: "getApiConfig",
      isLogging: "isLogging",
      isLoggedIn: "isLoggedIn",
      isLoggingOut: "isLoggingOut",
    }),
    avatar() {
      return this.avatar_user; //localStorage.getItem("jrs-user-avatar")
    },
  },
  watch: {
    isLoggedIn(val) {
      if (val) {
        console.log("Load avatar");
        (this as any).getAvatarFromMicrosoftGraphAccount();
      }
    },
    activeModule(newVal) {
      // console.log("localThis.aaaaaaaaaaaaaaa: " ,  this.activeModule);
      this.module_color = moduleColors.find(
        (ele) => ele.moduleName == this.activeModule.moduleName
      ).moduleColor;

      
    },
  },
  methods: {
    ...mapActions({
      toggleAppDrawer: "toggleAppDrawer",
      recoverAllLookups: "recoverAllLookups",
      setUserMenu: "setUserMenu",
      setUserLocale: "setUserLocale",
      setUserLogin: "setUserLogin",
    }),

    getAvatarFromMicrosoftGraphAccount() {
      const authModule = (this as any).$msal;
      const localThis: any = this as any;
      authModule
        .getUserAvatar("v1.0")
        .then((res: any) => {
          //localThis.avatar_user = res.data;
          // localThis.avatar_user =  'data:image/jpeg;base64,' + Buffer.from(res.data, 'base64');
          let binaryData = [];
          binaryData.push(res.data);
          const blobUrl = window.URL.createObjectURL(
            new Blob(binaryData, { type: "image/jpeg" })
          );
          localThis.avatar_src = blobUrl;
        })
        .catch((err: any) => {
          authModule
            .getUserAvatar("beta")
            .then((res: any) => {
              //localThis.avatar_user = res.data;
              // localThis.avatar_user =  'data:image/jpeg;base64,' + Buffer.from(res.data, 'base64');
              let binaryData = [];
              binaryData.push(res.data);
              const blobUrl = window.URL.createObjectURL(
                new Blob(binaryData, { type: "image/jpeg" })
              );
              localThis.avatar_src = blobUrl;
            })
            .catch((err: any) => {
              console.error(err);
              localThis.avatar_src = null;
            });
          //  console.error(err);
          //localThis.avatar_src = null;
        });
    },
  },
  beforeMount() {},
  mounted() {
    //Setup axios interceptors
    axios.interceptors.response.use(
      (response) => {
        let localThis: any = this as any;
        //localThis.avatar_user = localStorage.getItem('jrs-user-avatar');

        return response;
      },
      (error) => {
        const config: Configuration = this.getApiConfig;
        const api: AuthApi = new AuthApi(config);

        const originalRequest = error.config;

        // if (error.response.status === 401 && originalRequest.url === 'http://13.232.130.60:8081/v1/auth/token') {
        //       router.push('/login');
        //       return Promise.reject(error);
        //   }

        if (error.response.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          let oldToken = localStorage.getItem("jrs-user-token") || undefined;
          let oldRefreshToken =
            localStorage.getItem("jrs-refresh-token") || undefined;

          if (!oldToken || !oldRefreshToken) {
            return new Promise((resolve, reject) => {
              reject(
                'Could not recover "jrs-user-token" or "jrs-refresh-token" from localstorage'
              );
            });
          }

          return api
            .refreshToken(oldToken, oldRefreshToken)
            .then((res: any) => {
              if (res.status === 200 || res.status === 201) {
                // 1) Save new tokens in localStorage
                localStorage.setItem("jrs-user-token", res.data.token);
                localStorage.setItem(
                  "jrs-refresh-token",
                  res.data.refreshToken
                );

                // 2) Set new token for Header of calls
                axios.defaults.headers.common["Authorization"] =
                  "Bearer " + res.data.token;

                // 3) Return originalRequest object.
                originalRequest.headers["Authorization"] =
                  axios.defaults.headers.common["Authorization"];
                return axios(originalRequest);
              }
              throw "Error while refreshing token";
            })
            .catch((err: any) => {
              console.error(err);
            });
        }

        return Promise.reject(error);
      }
    );

    (this as any).recoverAllLookups();
  },
});
</script>
<style>
.v-application {
  font-family: "Montserrat";
}
</style>
