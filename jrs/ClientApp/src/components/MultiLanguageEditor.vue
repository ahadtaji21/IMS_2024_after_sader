<template>
  <v-card>
    <v-tabs v-model="tab" background-color="primary" dark>
      <v-tab v-for="lng in descriptions" :key="lng.IMS_LANGUAGE_NAME">{{ lng.IMS_LANGUAGE_NAME }}</v-tab>
    </v-tabs>
    <v-tabs-items v-model="tab">
      <v-tab-item v-for="item in descriptions" :key="item.IMS_LANGUAGE_NAME">
        <v-container fluid>
          <v-row>
            <v-col :cols="12">
              <v-text-field
                :label="$t(item.IMS_LANGUAGE_NAME)"
                v-model="item.IMS_LABELS_VALUE"
                :counter="50"
                @change="updateObj"
              ></v-text-field>
            </v-col>
          </v-row>
        </v-container>
      </v-tab-item>
    </v-tabs-items>
  </v-card>
</template>


<script lang="ts">
import Vue from "vue";
import { mapGetters } from "vuex";
import { mapActions } from "vuex";
import { ImsApi, Configuration } from "../axiosapi";
import { translate } from "../i18n";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../axiosapi/api";

export default Vue.extend({
  props: {
    // fieldDetails: {
    //   type: Object,
    //   required: true
    // },
    tableName: {
      type: String,
      required: true
    },
    idObject: {
      type: Number,
      required: true,
      default:0
    }
  },

  data: () => ({
    descriptions: [],
    length: 15,
    tab: null,
    items: [],
    languages: []
  }),

  mounted() {
    let localThis = this as any;
    localThis.getLanguages();

    //debugger;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),

    updateObj() {
      let localThis: any = this as any;
      localThis.$emit("descriptionChanged", localThis.descriptions);
    },
    getLanguages() {
      let localThis: any = this as any;
      let i: number = 0;
      //if (localThis.idObject == 0) return;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_VI_LANGUAGES",
        colList: null,
        whereCond: null
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.languages.push(item);
            });
          }
        })
        .then((res: any) => {
          localThis.getLanguageValues().then((res: any) => {
            {
              let i: number = 0;
              for (i = 0; i < localThis.languages.length; i++) {
                let j: number = 0;
                let found: boolean = false;
                for (j = 0; j < localThis.items.length; j++) {
                  if (
                    localThis.languages[i].IMS_LANGUAGE_NAME ==
                    localThis.items[j].IMS_LANGUAGE_NAME
                  ) {
                    found = true;
                    localThis.descriptions.push(localThis.items[j]);
                    break;
                  }
                }
                if (!found) {
                  localThis.descriptions.push({
                    IMS_LANGUAGE_NAME: localThis.languages[i].IMS_LANGUAGE_NAME,
                    IMS_LANGUAGE_ISO_639_1_CODE:localThis.languages[i].IMS_LANGUAGE_ISO_639_1_CODE,
                    IMS_LABELS_ID:0,
                    IMS_LABELS_TABLE_NAME:localThis.tableName,
                    IMS_LABELS_NUMBER:localThis.idObject,
                    IMS_LABELS_VALUE: ""
                  });
                }
              }
              localThis.$emit("descriptionChanged", localThis.descriptions);
            }
          });
        });
    },

    getLanguageValues() {
      let localThis: any = this as any;
      //if (localThis.idObject == 0) return;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_VI_LABELS",
        colList: null,
        whereCond: `IMS_LABELS_TABLE_NAME='${localThis.tableName}' AND IMS_LABELS_NUMBER=${localThis.idObject}`
        // orderStmt: "IMS_LANGUAGE_ISO_639_1_CODE"
      };
      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data

          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.items.push(item);
            });
          }
        }
      );
    }
  },
  watch: {
    idObject(oldid: any, newid: any) {
      let localThis: any = this as any;
      localThis.getLanguages();
    }
    // descriptions(oldd: any, newd: any) {
    //   debugger;
    //   let localThis: any = this as any;
    //   localThis.$emit("descriptionChanged", newd);
    // }
  }
});
</script>
