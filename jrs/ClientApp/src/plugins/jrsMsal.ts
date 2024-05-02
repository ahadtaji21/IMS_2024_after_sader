import { AuthModule } from '../services/AuthModule';

const Msal:any = {
  async install(Vue:any, envConfig:any){
    Vue.prototype.$msal = new AuthModule(envConfig);
  }
};

export default Msal;