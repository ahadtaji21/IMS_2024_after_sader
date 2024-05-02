import axios from 'axios';

const EnvConfig: any = {
    async install(Vue: any, options: any) {
        console.log(`Recieved: ${JSON.stringify(options)}`);
        const config: any = await axios
            .get(`./configs/config.json`)
            .then((res: any) => res.data );

        Vue.prototype.$envConfig = config
    }
};
export default EnvConfig;
