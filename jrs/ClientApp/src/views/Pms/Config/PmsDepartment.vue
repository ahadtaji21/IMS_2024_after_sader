<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center :cols="12">
          <div>
            <div>
              <!-- ITM SELECTION-->
              <v-data-table
                :headers="headers"
                :items="departmentList"
                item-key="ID"
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
                    <h3>
                      {{ $t("datasource.pms.annual-plan.objectives.departments") }}
                    </h3>

                    <v-spacer></v-spacer>
                    <span v-if="!onlyReadLocal">
                      <v-dialog
                        v-model="editMode"
                        persistent
                        retain-focus
                        :scrollable="true"
                        :overlay="false"
                        transition="dialog-transition"
                        max-width="45em"
                      >
                        <template v-slot:activator="{ on }">
                          <v-btn
                            color="secondary lighten-2"
                            class="grey--text text--darken-3"
                            v-on="on"
                            @click="newDepartment"
                          >
                            <v-icon>mdi-plus-circle-outline</v-icon>New
                          </v-btn>
                        </template>
                        <ap-dep
                          :isUpdatableForm="!onlyReadLocal"
                          :formDataExt="formData"
                          @closeDialogDPT="closeDialogDPT"
                          @saveDialoge="saveDialoge"
                          :key="Math.ceil(Math.random() * 1000)"
                        ></ap-dep>
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
                    <tr v-for="item in items" :key="item.ID" style="height: 10px">
                      <td class="tablerow">{{ item.CODE }}</td>
                      <td class="tablerow">{{ item.DESC }}</td>
                      <td style="text-align: center">
                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <v-icon
                              small
                              class="mr-2"
                              @click="editDepartment(item)"
                              v-on="on"
                              >mdi-circle-edit-outline</v-icon
                            >
                          </template>

                          <span>{{
                            $t("datasource.pms.annual-plan.objectives.department-details")
                          }}</span>
                        </v-tooltip>

                        <v-tooltip bottom>
                          <template v-slot:activator="{ on }">
                            <v-icon
                              v-if="!onlyReadLocal"
                              small
                              class="mr-2"
                              @click="deleteItem(item)"
                              v-on="on"
                              >mdi-delete</v-icon
                            >
                          </template>
                          <span>{{
                            $t("datasource.pms.annual-plan.objectives.delete-department")
                          }}</span>
                        </v-tooltip>
                      </td>
                    </tr>
                  </tbody>
                </template>
              </v-data-table>
            </div>
            <div>
              <div v-if="showOutpDetails">
                <v-dialog
                  v-model="showOutpDetails"
                  persistent
                  retain-focus
                  :scrollable="true"
                  :overlay="false"
                  transition="dialog-transition"
                  max-width="45em"
                >
                  <ap-dep
                    :isUpdatableForm="!onlyReadLocal"
                    :formDataExt="formData"
                    @closeDialogDPT="closeDialogDPT"
                    @saveDialoge="saveDialoge"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></ap-dep>
                </v-dialog>
              </div>
            </div>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import AnnualDepartment from "../../../components/PMS/DEPARTMENT/DepartmentMainDataForm.vue";
// import ItemDetails from "./AnnualPlanItemDetailsForm.vue";
// import ItemMainData from "./AnnualPlanItemMainDataForm.vue";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import FormMixin from "../../../mixins/FormMixin";
import NavMix from "../../../mixins/NavMixin";
import UtilMix from "../../../mixins/UtilMix";
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
//   OCCURRENCE: {} | null;
//   OBJECTIVE_ID: number | null;
// }

export default Vue.extend({
  components: {
    "ap-dep": AnnualDepartment,
  },
  props: {
    selectedAnnualPlanId: {
      type: Number,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: true,
    },
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      onlyReadLocal: false,
      overallgoalId: {},
      showItmTabs: true,
      selectedDepartment: "",
      selectedDepartmentId: 0,
      editItmDesc: "",
      editedItem: {},
      editId: null,
      editObj: null,
      showOutpDetails: false,
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

      formData: {},
      coi: [],
      tos: [],
      departmentList: [],
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
    getRnd() {
      return Math.ceil(Math.random() * 1000);
    },
    newDepartment() {
      let localThis: any = this as any;
      localThis.editMode = true;
      localThis.formData = {};
      localThis.formData.ID = 0;
      localThis.formData.CODE = "";
      localThis.formData.DESC = "";
      localThis.formData.GUID = "";
    },
    closeDialog(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) localThis.UpdateItem(item);
    },
    closeDialogDPT(item: String) {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.showOutpDetails = false;
    },
    closeDepartmentDialog(item: string) {
      let localThis: any = this as any;
      localThis.showOutpDetails = false;
    },
    saveDialoge() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.showOutpDetails = false;
      localThis.departmentDialogReload();
    },

    departmentDialogReload(item: string) {
      let localThis: any = this as any;
      //localThis.showOutpDetails = false;

      localThis.reloadItems(null);
    },
    closeDepartmentDialogReload(item: string) {
      let localThis: any = this as any;
      localThis.showOutpDetails = false;

      localThis.reloadItems(null);
    },
    UpdateItem(item: string) {
      let ap = {} as any;
      let localThis = this as any;
      let payLoadD = {} as any;

      payLoadD.ACTIVITY_ID = localThis.selectedAnnualPlanId;
      payLoadD.OUTPUT_ID = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_PROCESS_OUTPUT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.itemsPerPage = 15;
          localThis.getDepartments();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },

    reloadItems(item: any) {
      let localThis = this as any;
      localThis.itemsPerPage = 15;
      localThis.getDepartments();
    },
    getDepartments() {
      let localThis: any = this as any;
      localThis.selectedItem = null;
      localThis.departmentList = [];
      let i: number = 0;
      let j: number = 0;
      let view: string = "IMS_DEPARTMENT";

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: `BLOCKED=(0)`,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.departmentList.push(item);
            });
          }
        }
      );
    },

    editDepartment(item: any) {
      let localThis = this as any;

      localThis.formData.ID = item.ID;
      localThis.formData.CODE = item.CODE + "";
      localThis.formData.DESC = item.DESC + "";
      localThis.formData.GUID = item.GUID + "";
      localThis.showOutpDetails = true;
      EventBus.$emit("hideActTabs");
      //EventBus.$emit("showOutpTabs");
    },
    deleteItem(item: any) {
      let localThis = this as any;
      localThis.formData.ID = item.ID;
      localThis.formData.CODE = item.CODE + "";
      localThis.formData.DESC = item.DESC + "";
      localThis.formData.GUID = item.GUID + "";
      let value: any = {};
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();
      let id = 0;
      let msg = msgUpd;
      let payLoadD: any = {};

      if (localThis.formData.ID == undefined || localThis.formData.ID == 0) {
        localThis.formData.GUID = localThis.makeUUID();
      } else {
        if (
          localThis.formData.GUID == undefined ||
          localThis.formData.GUID == "undefined" ||
          localThis.formData.GUID == ""
        )
          localThis.formData.GUID = localThis.makeUUID();
      }

      payLoadD.ID = Number.parseInt(localThis.formData.ID);
      payLoadD.CODE = localThis.formData.CODE;
      payLoadD.GUID = localThis.formData.GUID;
      payLoadD.DESC = localThis.formData.DESC;
      payLoadD.BLOCKED = "TRUE";
      this.$confirm(msg).then((res) => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_DEPARTMENT",
            actionType: SqlActionType.NUMBER_3,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              });
            })
            .then((res: any) => {
              value.dimensionCode = "DEPARTMENT";
              value.department_code = payLoadD.CODE;
              value.department_descr = localThis.formData.DESC;
              value.guid = localThis.formData.GUID;
              value.blocked = "TRUE";
              // value.location_description=res.LOCATION_DESCRIPTION;
              // value.office_code = saveData.PMS_OFFICE_CODE;
              localThis.updateNavDimension(value);
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            })
            .finally(() => localThis.reloadItems());
        }
      });
    },

    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.pms.annual-plan.objectives.code").toString(),
          value: "CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n
            .t("datasource.pms.annual-plan.objectives.description")
            .toString(),
          value: "DESC",
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
    EventBus.$off("hideActTabs");
    EventBus.$off("showActTabs");
  },
  mounted() {
    let localThis: any = this as any;
    localThis.selectedItem = null;
    localThis.onlyReadLocal = localThis.onlyRead;
    localThis.setupHeaders();

    localThis.getDepartments();

    EventBus.$on("closeItemDetails", (data: any) => {
      localThis.showOutpDetails = false;
      localThis.reloadItems();
    });

    EventBus.$on("hideOutpDetails", (data: any) => {
      localThis.showOutpDetails = false;
    });

    EventBus.$on("reloadItems", (data: any) => {
      localThis.reloadItems(data);
    });

    EventBus.$on("userRoleUpdated", (to: any) => {
      if (to != undefined && (to.ROLE_CODE == "PC" || to.ROLE_CODE == "PD")) {
        //Program Coordinator
        localThis.onlyReadLocal = false;
      } else {
        localThis.onlyReadLocal = true;
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
      localThis.getDepartments();
      localThis.setupHeaders();
    },
    selectedAnnualPlanId(to: number) {
      let localThis: any = this as any;
      localThis.selectedAnnualPlanId = to;
      localThis.getDepartments();
    },

    editMode(to: boolean, from: boolean) {},
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
