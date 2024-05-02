import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import vuetify from './plugins/vuetify';
import { i18n } from './i18n';
import VuetifyConfirm from 'vuetify-confirm';
import VueCurrencyInput from 'vue-currency-input';
import './assets/css/style.css';
import axios from 'axios';
import Msal from './plugins/jrsMsal';
import EnvConfig from './plugins/envConfig';
//import  {UUID}  from "vue-uuid";

(async()=>{


  const pluginOptions = {
    /* see config reference */
    globalOptions: { currency: 'USD' }
  }
  Vue.use(VueCurrencyInput, pluginOptions)
  
  //Vue.use(UUID);
  // Download and instance EnvironmentConfig
  const config = await axios
  .get(`./configs/config.json`)
  .then((res:any) => {
    Vue.use(EnvConfig, res.data);
    return res.data;
  });
  
  Vue.use(Msal, config);
  Vue.use(VuetifyConfirm, { vuetify }  )
  Vue.config.productionTip = false;
  
  new Vue({
    router,
    store,
    vuetify,
    i18n,
  
    render: h => h(App)
  }).$mount('#app')
})();
