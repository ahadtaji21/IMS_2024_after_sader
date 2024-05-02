import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import colors from 'vuetify/es5/util/colors';

Vue.use(Vuetify);

export const lightTheme = {
    primary: "#214885,#1BA7CB",//colors.teal.base,
    secondary: "#DC7D11,#00B6B2,#017E9C,#5B616B", //colors.orange.base,
    tertiary:"#77AA2B",
    accent: colors.amber.base,
    error: colors.deepOrange.base,
    warning: colors.amber.base,
    info: colors.cyan.base,
    success: colors.lightGreen.base
};

export default new Vuetify({
    theme: {
        themes: {
            light: lightTheme
        },
    },
});
