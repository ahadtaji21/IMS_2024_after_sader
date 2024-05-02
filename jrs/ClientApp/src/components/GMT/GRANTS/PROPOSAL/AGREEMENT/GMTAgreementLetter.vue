<template>
  <v-content>
    <v-container fluid fill-height>
      <div class="text-center" v-if="showWait">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>
      <div style="width: 100%">
        <v-row>
          <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 3">
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="GenerateWord()"
            >
              <v-icon>mdi-file-word</v-icon>Download
            </v-btn>
          </v-col>
        </v-row>
      </div>
    </v-container>
  </v-content>
</template>
<script lang="ts">
// @ is an alias to /src
// import HelloWorld from "@/components/HelloWorld.vue";
import axios from "axios";
import Vue from "vue";
import { mapGetters } from "vuex";
import { mapActions } from "vuex";
import {
  PmsAnnualPlanApi,
  ImsApi,
  Configuration,
  NavIntegrationApi,
  NavDimension1,
  NavBudget1,
  ImsLookupsApi,
  AnnualPlanDocApi,
  GMTApi,
} from "../../../../../axiosapi";
export default Vue.extend({
  data() {
    return {
      showWait: false,
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    GenerateWord() {
      let localThis = this as any;
      var id: number = 0;
      let ap = {} as any;

      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: GMTApi = new GMTApi(config);
      localThis.showWait = true;
      let doc: any;
      return api
        .apiGMTGetGMTTemplateGet(config.baseOptions)
        .then((res) => {
          doc = res.data;
          const byteCharacters = atob(doc);
          const byteNumbers = new Array(byteCharacters.length);
          for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
          }
          const byteArray = new Uint8Array(byteNumbers);
          const blob = new Blob([byteArray], {
            type:
              "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
          });

          var link = document.createElement("a");

          var url = URL.createObjectURL(blob);
          link.setAttribute("href", url);
          link.setAttribute("download", "gm.docx");
          link.style.visibility = "hidden";
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        })
        .catch((err) => {
          // console.error(err);
          return "";
        })
        .finally(() => (localThis.showWait = false));
    },
  },
});
</script>
