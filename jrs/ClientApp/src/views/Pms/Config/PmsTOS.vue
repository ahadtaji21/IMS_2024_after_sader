<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div>
            <v-data-table
              :headers="headers"
              :items="tosList"
              item-key="PMS_TOS_ID"
              multi-sort
              :search="tableFilter"
              class="text-capitalize elevation-2"
            >
              <template v-slot:top>
                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                  <h3>{{ $tc("pms.tos", 2) }}</h3>
                  <v-spacer></v-spacer>
                  <v-dialog v-model="dialog" persistent max-width="1100px">
                    <template v-slot:activator="{ on }">
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                        v-on="on"
                        @click="openNew()"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>New
                      </v-btn>
                    </template>
                    <pms-tos-form
                      @closeDialoge="closeDialog"
                      @closeDialogeAndSave="closeDialogAndSave"
                      :formData="editedItem"
                      :key="keyDialog"
                    ></pms-tos-form>
                  </v-dialog>
                  <v-spacer></v-spacer>

                  <v-text-field
                    v-model="tableFilter"
                    append-icon="mdi-magnify"
                    :label="$t('general.search')"
                    hide-details
                    clearable
                    outlined
                    dense
                    class="white"
                    color="secondary darken-2"
                  ></v-text-field>
                </div>
              </template>
              <template v-slot:body="{ items }">
                <tbody style="border: solid">
                  <tr v-for="item in items" :key="item.PMS_TOS_ID" style="height: 10px">
                    <td class="tablerow">{{ item.PMS_TOS_CODE }}</td>
                    <td class="tablerow">{{ item.PMS_TOS_DESCRIPTION }}</td>
                    <td class="tablerow">
                      <v-icon
                        :color="
                          item.PMS_TOS_ENABLED == true
                            ? 'green darken-3'
                            : 'deep-orange darken-3'
                        "
                        >{{
                          item.PMS_TOS_ENABLED == true
                            ? "mdi-checkbox-marked-circle-outline"
                            : "mdi-checkbox-blank-circle-outline"
                        }}</v-icon
                      >
                    </td>
                    <td style="text-align: center">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                            >mdi-circle-edit-outline</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.edit-service")
                        }}</span>
                      </v-tooltip>
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                            >mdi-delete</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.delete-service")
                        }}</span>
                      </v-tooltip>
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-data-table>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../axiosapi";
import { translate } from "../../../i18n";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import TOSMainData from "../../../components/PMS/CONFIG/TOS/PMSTOSMainDataForm.vue";
import FormMixin from "../../../mixins/FormMixin";
import NavMix from "../../../mixins/NavMixin";
import UtilMix from "../../../mixins/UtilMix";
import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  components: {
    "pms-tos-form": TOSMainData,
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      keyDialog: 0,
      dialog: false,
      headers: [],
      tosList: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
      //,
      // itemModel: {
      //   apcode: null,
      //   descr: null,
      //   locationDescr: null,
      //   adminAreaName: null,
      //   startYear: null,
      //   statusName: null
      // }
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
    }),
    language() {
      return i18n.locale;
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.getTOS();
    localThis.setupHeaders();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    openNew() {
      let localThis = this as any;

      localThis.keyDialog = Math.ceil(Math.random() * 1000);
      let obj: any = {};
      obj.PMS_TOS_ENABLED = true;
      localThis.editedItem = JSON.parse(JSON.stringify(obj));
      localThis.dialog = true;
    },
    getTOS() {
      let localThis = this as any;
      localThis.selectedtos = null;
      localThis.tosList = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_TYPE_OF_SERVICE",
        //colList: null,
        //whereCond: `IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_TOS_ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              let i: number = 0;
              let obj: any = [];
              if (item.coi_relations_placeholder != undefined) {
                for (i = 0; i < item.coi_relations_placeholder.length; i++) {
                  if (
                    item.coi_relations_placeholder[i].IMS_LANGUAGE_LOCALE ==
                    localThis.language
                  ) {
                    let coitos: any = {};
                    coitos.PMS_COI_ID = item.coi_relations_placeholder[i].PMS_COI_ID;
                    coitos.PMS_COI_CODE = item.coi_relations_placeholder[i].PMS_COI_CODE;
                    coitos.START_DATE = this.$i18n.d(
                      new Date(item.coi_relations_placeholder[i].START_DATE),
                      "short"
                    );
                    coitos.END_DATE = this.$i18n.d(
                      new Date(item.coi_relations_placeholder[i].END_DATE),
                      "short"
                    );
                    coitos.IMS_LABELS_VALUE =
                      item.coi_relations_placeholder[i].IMS_LABELS_VALUE;
                    coitos.IMS_LANGUAGE_LOCALE =
                      item.coi_relations_placeholder[i].IMS_LANGUAGE_LOCALE;
                    coitos.PMS_COI_ENABLED =
                      item.coi_relations_placeholder[i].PMS_COI_ENABLED;
                    coitos.ID = item.coi_relations_placeholder[i].ID;
                    obj.push(coitos);
                  }
                }
                item.coi_relations_placeholder = obj;
              } else {
                item.coi_relations_placeholder = [];
              }

              localThis.tosList.push(item);
            });
          }
        });
    },
    closeDialog() {
      let localThis = this as any;

      let msgLeave: string = this.$i18n
        .t("datasource.pms.annual-plan.ap-leave-dialog-confirm")
        .toString();
      this.$confirm(msgLeave).then((res) => {
        if (res) {
          localThis.dialog = false;
          localThis.editedItem = {};
          localThis.keyDialog = Math.ceil(Math.random() * 1000);

          // (this as any).editedItem = (this as any).itemModel;
        }
      });
    },
    closeDialogAndSave(value: any) {
      let localThis = this as any;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
      localThis.dialog = false;
      localThis.getTOS();
      localThis.editedItem = {};
      return;
    },
    editItem(item: any) {
      let localThis: any = this as any;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);

      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.dialog = true;
    },
    // deleteItem(item: any) {
    //   let localThis: any = this as any;
    //   let msg: string = this.$i18n.t("pms.tos-delete-confirm").toString();

    //   this.$confirm(msg).then(res => {
    //     if (res) {
    //       item["pmsCoiDeleted"] = true;
    //       const config: Configuration =
    //         localThis.$store.getters["auth/getApiConfig"];

    //       // const api: PmsCategoryOfInterventionApi = new PmsCategoryOfInterventionApi(config);
    //       // return api
    //       //     .apiPmsCategoryOfInterventionIdPut(Number(item['pmsCoiId']),item,  config.baseOptions)
    //       //     .then(response => {
    //       //         (this as any).getOBJ();
    //       //     })
    //       //     .catch(error => {
    //       //         alert(error);
    //       //     });
    //     }
    //   });
    // },
    deleteItem(item: any) {
      let localThis = this as any;
      let value: any = {};
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      let obj: any;
      obj = item;
      if (obj.PMS_TOS_ID == undefined || obj.PMS_TOS_ID == 0) {
        obj.GUID = localThis.makeUUID();
      } else {
        if (obj.GUID == undefined || obj.GUID == "undefined" || obj.GUID == "")
          obj.GUID = localThis.makeUUID();
      }
      obj.PMS_TOS_ENABLED = false;

      this.$confirm(msg).then(async (res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_TYPE_OF_SERVICE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(obj),
          };
          let retValue = await localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to use
              return "";
            })
            .then(async (res: any) => {
              value.dimensionCode = "TYPE OF SERVICE";
              value.tos_code = obj.PMS_TOS_CODE;
              value.tos_descr = obj.PMS_TOS_DESCRIPTION;
              if (
                obj.coi_relations_placeholder != undefined &&
                obj.coi_relations_placeholder.length > 0
              )
                value.coi_code = obj.coi_relations_placeholder[0].PMS_COI_CODE;
              else value.coi_code = "";
              value.guid = obj.GUID;
              value.blocked = "TRUE";
              // value.location_description=res.LOCATION_DESCRIPTION;
              // value.office_code = saveData.PMS_OFFICE_CODE;
              await localThis.updateNavDimension(value);
              return "";
            })
            .catch((err: any) => {
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            })
            .finally(() => {
              localThis.getTOS();
            });
        }
      });
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("pms.code").toString(),
          align: "center",
          value: "PMS_TOS_CODE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("pms.description").toString(),

          align: "center",
          value: "PMS_TOS_DESCRIPTION",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.pms.type-of-service.enabled").toString(),

          align: "center",
          value: "PMS_TOS_ENABLED",
          sortable: true,
          divider: true,
        },
        {
          text: "Actions",
          value: "action",
          align: "center",
          sortable: false,
        },
      ];
      localThis.headers = tmp;
    },
  },

  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getTOS();
      localThis.setupHeaders();
    },
  },
});
</script>
<style scoped>
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.tablerow {
  text-align: center;
  height: 3.5em;
  padding: 0 1em;
  text-align: center;
  border: solid;
  border-width: 1px;
  border-color: rgba(0, 0, 0, 0.12);
  box-sizing: border-box;
}
</style>
