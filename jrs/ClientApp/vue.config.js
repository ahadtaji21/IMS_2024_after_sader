module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  publicPath: process.env.NODE_ENV !== 'development' ? '/jrsapp/' : '/',

  devServer: {
    watchContentBase: false,
    host: '0.0.0.0'
    ,https: false
  },
  pluginOptions: {
    i18n: {
      locale: 'en',
      fallbackLocale: 'en',
      localeDir: 'locales',
      enableInSFC: false
    }
  },
  configureWebpack: {
    devtool: 'source-map'
  }
}
