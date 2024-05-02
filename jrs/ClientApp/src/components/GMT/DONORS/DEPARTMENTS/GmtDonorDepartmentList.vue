<template>
  <div>
    <v-row align-center>
      <v-col justify-center :cols="12">
        <div>
          <v-data-table
            :headers="headers"
            :items="donorList"
            item-key="GMT_DONOR_DEPARTMENT_ID"
            multi-sort
            :search="tableFilter"
            class="text-capitalize elevation-2"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $tc("datasource.gmt.donor.donor-contacts", 2) }}</h3>
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog" persistent max-width="1100px" v-if="!onlyRead">
                  <template v-slot:activator="{ on }">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3"
                      v-on="on"
                    >
                      <v-icon>mdi-plus-circle-outline</v-icon>New
                    </v-btn>
                  </template>
                  <pms-donor-form
                    @closeDialoge="closeDialog"
                    @closeDialogeAndSave="closeDialogAndSave"
                    :formData="editedItem"
                    :selectedObjectId="selectedObjectId"
                    :key="keyDialog"
                  ></pms-donor-form>
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
                <tr
                  v-for="item in items"
                  :key="item.GMT_DONOR_DEPARTMENT_ID"
                  style="height: 10px"
                >
                  <td class="tablerow">{{ item.GMT_DONOR_DEPARTMENT_NAME }}</td>
                  <td class="tablerow">
                    {{ item.GMT_DONOR_DEPARTMENT_DESCRIPTION | subStr }}
                  </td>

                  <td style="text-align: center">
                    <v-tooltip bottom v-if="!onlyRead">
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="editItem(item)" v-on="on"
                          >mdi-circle-edit-outline</v-icon
                        >
                      </template>
                      <span>{{ $t("datasource.gmt.donor.edit-donor") }}</span>
                    </v-tooltip>
                    <v-tooltip bottom v-if="!onlyRead">
                      <template v-slot:activator="{ on }">
                        <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                          >mdi-delete</v-icon
                        >
                      </template>
                      <span>{{ $t("datasource.gmt.donor.delete-donor") }}</span>
                    </v-tooltip>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </div>
      </v-col>
    </v-row>
  </div>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../../../axiosapi";
import { translate } from "../../../../i18n";
import { JrsHeader } from "../../../../models/JrsTable";
import { i18n } from "../../../../i18n";
import DonorDepMainData from "./GMTDonorDepartmentMainDataForm.vue";

import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "pms-donor-form": DonorDepMainData,
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      keyDialog: 0,
      dialog: false,
      headers: [],
      donorList: [],
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
    localThis.getDonors();
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
    getDonors() {
      let localThis = this as any;
      localThis.selecteddonor = null;
      localThis.donorList = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_DONOR_DEPARTMENT",
        //colList: null,
        whereCond: `GMT_DONOR_ID=${localThis.selectedObjectId}`,
        orderStmt: "GMT_DONOR_DEPARTMENT_ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              item.GMT_DONOR_ID = localThis.selectedObjectId;
              localThis.donorList.push(item);
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
      localThis.getDonors();
      localThis.editedItem = {};
      return;
    },
    // editItem(item: any) {
    //   let localThis: any = this as any;
    //   localThis.keyDialog = Math.ceil(Math.random() * 1000);
    //   localThis.editedItem = JSON.parse(JSON.stringify(item));
    //   localThis.dialog = true;
    // },

    editItem(item: any) {
      let localThis: any = this as any;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      // localThis.$router.push({
      //   name: "Donor Details",
      //   params: {
      //     editItemMainData: localThis.editedItem,
      //   },
      // });
      localThis.dialog = true;
      //this.rout.push(this.$route);
    },

    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.gmt.donor.donor-contact-delete-confirm")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.GMT_DONOR_DEPARTMENT_ID = item.GMT_DONOR_DEPARTMENT_ID;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_DONOR_DEPARTMENT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.itemsPerPage = 15;
              localThis.getDonors();
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.gmt.donor.name").toString(),
          align: "center",
          value: "GMT_DONOR_NAME",
          sortable: true,
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.gmt.donor.description").toString(),

          align: "center",
          value: "GMT_DONOR_DESCRIPTION",
          sortable: true,
          divider: true,
        },
        // {
        //   text: this.$i18n.t("pms.donor-enabled").toString(),

        //   align: "center",
        //   value: "PMS_COI_ENABLED",
        //   sortable: true,
        //   divider: true
        // },
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
  filters: {
    subStr: function (string: any) {
      if (string != undefined) return string.substring(0, 40) + "...";
      else return "";
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getDonors();
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
