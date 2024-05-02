<template>
  <div class="d-flex " id="lang-select-wrapper">
    <v-select
      :items="localesList"
      v-model="appLocaleSelected"
      :label="languageSelectLab"
      prepend-icon="mdi-translate"
      class="language-selector"
    ></v-select>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n  }from "../i18n";

interface LanguageSelectorData{
  localesList: Array<string>
}

export default Vue.extend({
  name: "LanguageSelector",
  data() {
    return {
      localesList: new Array<string>(),
    };
  },
  computed: {
    languageSelectLab() {
      return i18n.t("general.language");
    },
    appLocaleSelected:{
      get(){
        return i18n.locale;
      },
      set(val:string){
        i18n.locale = val;
        this.updateMenuForLang(val);
      }
    }
  },
  methods: {
    ...mapActions({
      updateMenuForLang: "updateMenuForLang"
    })
  },
  watch: {},
  mounted() {
      this.localesList = i18n.availableLocales;
      this.appLocaleSelected = i18n.locale;
  },
});
</script>

<style scoped>
/* #lang-select-wrapper{
    margin-top: 2em;
}
.language-selector {
  max-width: 6em;
} */
</style>