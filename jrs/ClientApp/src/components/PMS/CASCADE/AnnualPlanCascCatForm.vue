<template>
  <div>
    <div>
      <!-- ITM SELECTION-->
      <v-row align-center v-if="showWait">
        <v-col justify-center :cols="12">
          <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row align-center :cols="12">
        <v-col justify-center>
          <v-data-table
            :headers="headers"
            :items="categoryList"
            item-key="PMS_CATEGORY_REL_ID"
            multi-sort
            :search="tableFilter"
            :items-per-page="itemsPerPage"
            style="
               {
                'font-size':'14px','width': '85px';
              }
            "
            class="text-capitalize"
            v-model="selectedItm"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h4>{{ $t("pms.categories") }}</h4>

                <v-spacer></v-spacer>
                <span v-if="!onlyRead">
                  <v-dialog
                    v-model="editMode"
                    persistent
                    retain-focus
                    :scrollable="true"
                    :overlay="false"
                    transition="dialog-transition"
                  >
                    <template v-slot:activator="{ on }">
                      <v-btn
                        color="secondary lighten-2"
                        class="grey--text text--darken-3"
                        v-on="on"
                        @click="editMode = !editMode"
                        v-if="!onlyRead"
                      >
                        <v-icon>mdi-plus-circle-outline</v-icon>ADD CATEGORY
                      </v-btn>
                    </template>
                    <search-table
                      :isUpdatableForm="!onlyRead"
                      :formDataExt="formData"
                      @closeDialogCat="closeDialog"
                      @saveDialoge="saveDialoge"
                      :versioned="versioned"
                      @generateError="generateError"
                      :key="Math.ceil(Math.random() * 1000)"
                      :categoriesAlreadyAdded="categoryList"
                    ></search-table>
                  </v-dialog>
                </span>
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
                  v-for="(item, index) in items"
                  :key="item.PMS_CATEGORY_REL_ID"
                  style="height: 10px"
                >
                  <td class="tablerow">
                    <v-checkbox
                      v-model="selected[index]"
                      value
                      hide-details
                      class="shrink ma-0 pa-0"
                      @change="checkboxUpdated($event, index, item.PMS_COI_ID)"
                    ></v-checkbox>
                  </td>
                  <td class="tablerow">{{ item.PMS_COI_DESCRIPTION }}</td>
                  <td style="text-align: center">
                    <span v-if="!onlyRead">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on"
                            >mdi-delete</v-icon
                          >
                        </template>
                        <span>{{ $t("pms.delete-coi") }}</span>
                      </v-tooltip>
                    </span>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-data-table>
        </v-col>
      </v-row>

      <v-row v-if="showServices">
        <v-col justify-center :cols="12">
          <ap-services
            :onlyRead="onlyRead"
            :selectedObjectId="formData.selectedOBJId"
            :formDataInput="editObjMainDataItem"
            :showServiceDetails="showSrvDetails"
            :isUpdatableForm="!onlyRead"
            :selectedCategoryId="selectedCategoryId"
            :versioned="versioned"
            :key="keyRnd"
            :cascade="true"
          ></ap-services>
        </v-col>
      </v-row>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import SearchTable from "./AnnualPlanCascCatSearch.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
//import ItemCategory from "./../OUTPUTS/AnnualPlanCategoryDetailsForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import UtilMix from "../../../mixins/UtilMix";
import Services from "./AnnualPlanCascSrvForm.vue";
import { IPayLoadDataITEM, IPayLoadDataOUTPUT } from "./../PMSSharedInterfaces";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

// interface ItemData {
//   ID: number | null;
//   COI: {} | null;
//   TOS: {} | null;
//   DATE_FROM: Date | null;
//   DATE_TO: Date | null;
//   OCATURRENCE: {} | null;
//   OBJECTIVE_ID: number | null;
/* localThis.localFormData.selectedProcessId = localThis.selectedProcessId;
      localThis.localFormData.selectedActivityId = localThis.selectedActivityId;
      localThis.localFormData.selectedOBJId = localThis.selectedOBJId;
      localThis.localFormData.selectedOutput = localThis.selectedOutput;*/
// }

export default Vue.extend({
  components: {
    "search-table": SearchTable,
    "ap-services": Services,
  },
  mixins: [UtilMix],
  props: {
    formData: {
      type: Object,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      showWait: false,
      keyRnd: 0,
      editObjMainDataItem: {},
      showSrvDetails: false,
      showServices: false,
      selected: [],
      nbrOfColumns: 2,
      showCategoryDetails: false,
      categoryId: {},
      showItmTabs: true,
      selectedCategory: "",
      selectedCategoryId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editOutp: null,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      selectedItm: [],
      tableFilter: "",
      selectedItem: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      isLoading: false,
      headers: [],

      localFormData: {},
      coi: [],
      tos: [],
      categoryList: [],
      occ: [],
    };
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    closeOutpDialog() {
      let localThis: any = this as any;
      localThis.showCategoryDetails = false;
    },
    async closeOutpDialogAndSave() {
      let localThis: any = this as any;
      localThis.showCategoryDetails = false;
      localThis.itemsPerPage = 15;
      const retv = await localThis.getObjCategories();
      //this.$emit("reloadCATList");
    },
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    async closeDialog(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) {
        localThis.itemsPerPage = 15;
        const retv = await localThis.getObjCategories();
        localThis.uncheckList();
        //this.$emit("reloadCATList");
      }
    },

    async saveDialoge() {
      let localThis: any = this as any;
      localThis.editMode = false;
      const retv = await localThis.getObjCategories();
      localThis.uncheckList();
    },

    async reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      const retv = await localThis.getObjCategories();
      //this.$emit("reloadCATList");
    },
    getObjCategories() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.categoryList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "PMS_VI_OBJ_CAT";
      let wherecond: string = `PMS_OBJECTIVE_ID = ${localThis.formData.selectedOBJId}`;
      if (localThis.versioned > 0) {
        view = "PMS_VI_OBJ_CAT_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: "PMS_COI_DESCRIPTION",
      };
      localThis.showWait = true;
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.showWait = false;
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.categoryList.push(item);
            });
          }
          localThis.uncheckList();
          return "";
        })
        .catch((err: any) => {
          localThis.showWait = false;
          localThis.generateError(err);
          return "";
        });
    },

    uncheckList() {
      let localThis = this as any;
      let i: number = 0;
      localThis.showServices = false;
      if (localThis.categoryList == undefined || localThis.categoryList.length == 0)
        return;
      for (i = 0; i < localThis.categoryList.length; i++) {
        if (localThis.selected != undefined && localThis.selected[i] != undefined) {
          localThis.selected[i] = false;
        }
      }
    },
    checkboxUpdated(newValue: any, index: number, coi_id: number) {
      let localThis: any = this as any;

      var count = localThis.selected.length;
      let i: number;

      for (i = 0; i < count; i++) {
        if (i != index) {
          localThis.selected[i] = false;
        }
        localThis.showServices = newValue;
        if (newValue) {
          localThis.selectedCategoryId = coi_id;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
          localThis.showServices = true;
        }
      }
    },
    async deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.showWait = true;
          let payLoadD = {} as any;
          payLoadD.PMS_CATEGORY_REL_ID = item.PMS_CATEGORY_REL_ID;

          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_OBJ_CAT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then(async (res: any) => {
              localThis.showWait = false;
              localThis.itemsPerPage = 15;
              const retv = await localThis.getObjCategories();
              //this.$emit("reloadCATList");
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.uncheckList();
            })
            .catch((err: any) => {
              localThis.showWait = false;
              localThis.generateError(err);
            });
        }
      });
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: "",
          value: "",
          align: "center",
          divider: true,
          width: "20px",
        },
        {
          text: this.$i18n.t("pms.category").toString(),
          value: "PMS_COI_DESCRIPTION",
          align: "center",
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
  beforeDestroy() {
    //do something before destroying vue instance
  },
  async mounted() {
    let localThis: any = this as any;
    localThis.selectedItem = null;
    localThis.editObjMainDataItem.PMS_TARGET_ID = localThis.formData.PMS_TARGET_ID;
    localThis.setupHeaders();
    if (localThis.formData.selectedOBJId > 0) {
      const retv = await localThis.getObjCategories();
    }

    EventBus.$on("closeDialogeAndSaveCATInd", async (data: any) => {
      localThis.showCategoryDetails = false;
      if (localThis.formData.selectedOBJId > 0) {
        localThis.editMode = false;
        const retv = await localThis.getObjCategories();
        //this.$emit("reloadCATList");
      }
    });
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
      localThis.getObjCategories();
      localThis.setupHeaders();
    },
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
.narrative-section {
  margin-bottom: 1em;
}
.overall-node {
  margin: 1em 0.5em;
}
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.selectedRow {
  background-color: whitesmoke;
  font-weight: bold;
  border-width: 1px;
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
  border-left: none;
}
</style>
