import Vue from 'vue'
import VueI18n, { LocaleMessages } from 'vue-i18n'

Vue.use(VueI18n)

//TODO: move this configuration to a distinct file
const dateTimeFormats = {
  'en': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'short', hour: 'numeric', minute: 'numeric', hour12: true
    }
  },
  'fr': {
    short: {
      year: 'numeric', month: 'numeric', day: 'numeric'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'short', hour: 'numeric', minute: 'numeric', hour12: false
    }
  }
}

function loadLocaleMessages (): LocaleMessages {
  const locales = require.context('./locales', true, /[A-Za-z0-9-_,\s]+\.json$/i)
  const messages: LocaleMessages = {}
  locales.keys().forEach(key => {
    const matched = key.match(/([A-Za-z0-9-_]+)\./i)
    if (matched && matched.length > 1) {
      const locale = matched[1]
      messages[locale] = locales(key)
    }
  })
  return messages
}

const i18n = new VueI18n({
  locale: process.env.VUE_APP_I18N_LOCALE || 'en',
  fallbackLocale: process.env.VUE_APP_I18N_FALLBACK_LOCALE || 'en',
  messages: loadLocaleMessages(),
  // silentTranslationWarn: true
});


const translate = (key:string) => {
  if(!key){
    return '';
  }
  return i18n.t(key);
}
const formatDateTime = (date:Date, format:string, locale?:string) => {
  if(!locale){
    return i18n.d(date, format)
  }
  return i18n.d(date, format, locale);
}


export  {i18n, translate, formatDateTime }
