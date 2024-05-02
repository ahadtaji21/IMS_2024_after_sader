<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div>
            <v-data-table
              :headers="headers"
              :items="coiList"
              item-key="PMS_COI_ID"
              multi-sort
              :search="tableFilter"
              class="text-capitalize elevation-2"
            >
              <template v-slot:top>
                <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                  <h3>{{ $tc("pms.coi", 2) }}</h3>
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
                    <pms-coi-form
                      @closeDialoge="closeDialog"
                      @closeDialogeAndSave="closeDialogAndSave"
                      :formData="editedItem"
                      :key="keyDialog"
                    ></pms-coi-form>
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
                  <tr v-for="item in items" :key="item.PMS_COI_ID" style="height: 10px">
                    <td class="tablerow">{{ item.PMS_COI_CODE }}</td>
                    <td class="tablerow">{{ item.IMS_LABELS_VALUE }}</td>
                    <td class="tablerow">
                      <v-icon
                        :color="
                          item.PMS_COI_ENABLED == true
                            ? 'green darken-3'
                            : 'deep-orange darken-3'
                        "
                        >{{
                          item.PMS_COI_ENABLED == true
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
                          $t("datasource.pms.annual-plan.objectives.edit-coi")
                        }}</span>
                      </v-tooltip>
                      <v-tooltip bottom >
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                            >mdi-delete</v-icon
                          >
                        </template>
                        <span>{{
                          $t("datasource.pms.annual-plan.objectives.delete-coi")
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
import COIMainData from "../../../components/PMS/CONFIG/COI/PMSCOIMainDataForm.vue";
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
    "pms-coi-form": COIMainData,
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      keyDialog: 0,
      dialog: false,
      headers: [],
      coiList: [],
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
    localThis.getCOI();
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
      let obj: any = {};
      obj.PMS_COI_ENABLED = true;
      localThis.editedItem = JSON.parse(JSON.stringify(obj));
      localThis.dialog = true;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
    },
    getCOI() {
      let localThis = this as any;
      localThis.selectedcoi = null;
      localThis.coiList = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_CATEGORY_OF_INTERVENTION",
        //colList: null,
        whereCond: `IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_COI_ID",
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
                  let coitos: any = {};
                  coitos.PMS_TOS_ID = item.coi_relations_placeholder[i].PMS_TOS_ID;
                  coitos.START_DATE = this.$i18n.d(
                    new Date(item.coi_relations_placeholder[i].START_DATE),
                    "short"
                  );
                  coitos.END_DATE = this.$i18n.d(
                    new Date(item.coi_relations_placeholder[i].END_DATE),
                    "short"
                  );
                  coitos.PMS_TOS_DESCRIPTION =
                    item.coi_relations_placeholder[i].PMS_TOS_DESCRIPTION;
                  coitos.ID = item.coi_relations_placeholder[i].ID;
                  obj.push(coitos);
                }
                item.coi_relations_placeholder = obj;
              } else {
                item.coi_relations_placeholder = [];
              }
              localThis.coiList.push(item);
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
      localThis.getCOI();
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
    //   let msg: string = this.$i18n.t("pms.coi-delete-confirm").toString();

    //   this.$confirm(msg).then((res) => {
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
      if (obj.PMS_COI_ID == undefined || obj.PMS_COI_ID == 0) {
        obj.GUID = localThis.makeUUID();
      } else {
        if (obj.GUID == undefined || obj.GUID == "undefined" || obj.GUID == "")
          obj.GUID = localThis.makeUUID();
      }
      obj.PMS_COI_ENABLED = false;

      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_CATEGORY_OF_INTERVENTION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(obj),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to use
            })
            .then((res: any) => {
              value.dimensionCode = "CAT. OF INTERVENTION";
              value.coi_code = obj.PMS_COI_CODE;
              value.coi_descr = obj.PMS_COI_DESCRIPTION;
              if (
                obj.coi_relations_placeholder != undefined &&
                obj.coi_relations_placeholder.length > 0
              )
                value.tos_code = obj.coi_relations_placeholder[0].PMS_TOS_CODE;
              else value.tos_code = "";
              value.guid = obj.GUID;
              value.blocked = "TRUE";
              // value.location_description=res.LOCATION_DESCRIPTION;
              // value.office_code = saveData.PMS_OFFICE_CODE;
              localThis.updateNavDimension(value);
            })
            .catch((err: any) => {
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            })
            .finally(() => {
              localThis.getCOI();
            });
        }
      });
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("pms.coi-code").toString(),
          align: "center",
          value: "PMS_COI_CODE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("pms.coi-description").toString(),

          align: "center",
          value: "IMS_LABELS_VALUE",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("pms.coi-enabled").toString(),

          align: "center",
          value: "PMS_COI_ENABLED",
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
      localThis.getCOI();
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
