<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col :cols="12">
          <!-- TABLE -->

          <jrs-simple-table
            :viewName="viewNameViProjects"
            :selectable="false"
            :hideNewBtn="true"
            :hideActions="true"
            :key="keyRnd"
          >
            <!-- TABLE HEADER -->

            <template v-if="!notAuthorizedRole" v-slot:simple-table-header>
              <v-dialog
                v-model="addMode"
                width="auto "
                :fullscreen="$vuetify.breakpoint.xsOnly"
                persistent
                retain-focus
                :scrollable="true"
                transition="dialog-transition"
              >
                <template v-slot:activator="{ on }">
                  <v-btn
                    v-on="on"
                    color="secondary lighten-2"
                    class="grey--text text--darken-3"
                    @click="addItem({})"
                  >
                    <v-icon>mdi-plus-circle-outline</v-icon>New
                  </v-btn>
                </template>
                <v-card>
                  <jrs-form
                    :formData="selectedItemProjects"
                    :formFields="formFieldsProjects"
                    :nbrOfColumns="2"
                    :key="Math.ceil(Math.random() * 1000)"
                  >
                    <template v-slot:form-actions="{ validateFunc }">
                      <v-btn color="secondary" @click="cancel()" class="ma-2">{{
                        $t("general.close")
                      }}</v-btn>
                      <v-btn
                        color="primary"
                        :disabled="isSaving"
                        @click="
                          saveFormData(
                            selectedItemProjects,
                            selectedItemProjects.PMS_PROJECT_CODE_ID ? 1 : 0,
                            validateFunc
                          )
                        "
                        class="ma-2"
                        >{{ $t("general.save") }}</v-btn
                      >
                    </template>
                  </jrs-form>
                </v-card>
              </v-dialog>
            </template>

            <!-- ROW ACTIONS -->
            <!-- ROW ACTIONS -->

            <template
              v-if="!notAuthorizedRole"
              v-slot:simple-table-item-actions="{ simpleItemRowItem }"
            >
              <v-icon @click="editProjectCode(simpleItemRowItem)"
                >mdi-circle-edit-outline</v-icon
              >
              <v-icon @click="deleteProjectCode(simpleItemRowItem)">mdi-delete</v-icon>
            </template>
          </jrs-simple-table>

          <v-dialog
            v-model="editMode"
            width="auto "
            :fullscreen="$vuetify.breakpoint.xsOnly"
            persistent
            retain-focus
            :scrollable="true"
            transition="dialog-transition"
          >
            <v-card>
              <jrs-form
                :formData="selectedItemProjects"
                :formFields="formFieldsProjects"
                :nbrOfColumns="2"
                :key="Math.ceil(Math.random() * 1000)"
              >
                <template v-slot:form-actions="{ validateFunc }">
                  <v-btn color="secondary" @click="cancel()" class="ma-2">{{
                    $t("general.close")
                  }}</v-btn>
                  <v-btn
                    color="primary"
                    :disabled="isSaving"
                    @click="
                      saveFormData(
                        selectedItemProjects,
                        selectedItemProjects.PMS_PROJECT_CODE_ID ? 1 : 0,
                        validateFunc
                      )
                    "
                    class="ma-2"
                    >{{ $t("general.save") }}</v-btn
                  >
                </template>
              </jrs-form>
            </v-card>
          </v-dialog>
          <!-- <v-dialog
            v-model="addMode"
            persistent
            retain-focus
            :scrollable="true"
            :overlay="false"
            :max-width="'60em'"
            transition="dialog-transition"
          >
            <jrs-form
              :formData="selectedItemProjects"
              :formFields="formFieldsProjects"
              :nbrOfColumns="2"
            >
              <template v-slot:form-actions="{ validateFunc, resetFunc }">
                <v-btn
                  color="primary"
                  :disabled="isSaving"
                  @click="
                    saveFormData(
                      selectedItemProjects,
                      selectedItemProjects.PMS_PROJECT_CODE_ID ? 1 : 0,
                      validateFunc
                    )
                  "
                  class="ma-2"
                  >{{ $t("general.save") }}</v-btn
                >
                <v-btn color="primary" @click="resetFunc()" class="ma-2">{{
                  $t("general.reset")
                }}</v-btn>
              </template>
            </jrs-form>
          </v-dialog> -->
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsForm from "../../components/JrsForm.vue";
import { EventBus } from "../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import NavMix from "../../mixins/NavMixin";
import UtilMix from "../../mixins/UtilMix";
export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-form": JrsForm,
  },
  mixins: [FormMixin, NavMix, UtilMix],
  data() {
    return {
      keyRnd: 0,
      viewNameViProjects: "PMS_VI_PROJECT_CODE",
      selectedItemProjects: null,
      originalSelectedItemProjects: null,
      addMode: false,
      editMode: false,
      notAuthorizedRole: true,
      role: "",
      crud_info: {},
      formFieldsProjects: [],
      formFieldsProjectsRepresentativeHistory: [],
      formFieldsProjectsLanguages: [],
      isSaving: false,
    };
  },
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentProjects: "getCurrentProjects",
      getCurrentRole: "getCurrentRole",
    }),
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),

    /**
     * Select the current item to edit, by doing so, close the list and open the detail section.
     * @param {Any} item - Item to select for update
     */
    selectItem(item: any) {
      let localThis: any = this as any;
      localThis.addMode = true;
      localThis.selectedItemProjects = Object.assign(
        {},
        localThis.selectedItemProjects,
        item
      );
    },

    addItem(item: any) {
      let localThis: any = this as any;
      localThis.addMode = true;
      localThis.selectedItemProjects = {} as any;
    },

    editProjectCode(item: any) {
      let localThis: any = this as any;
      localThis.editMode = true;
      localThis.selectedItemProjects = Object.assign(
        {},
        localThis.selectedItemProjects,
        item
      );

      localThis.originalSelectedItemProjects = Object.assign(
        {},
        localThis.originalSelectedItemProjects,
        item
      );
    },

    deleteProjectCode(saveData: any) {
      let value: any = {};
      let localThis: any = this as any;
      localThis.isSaving = true;
      let spName: string = "PMS_SP_INS_UPD_PROJECT_CODE";

      if (
        saveData.PMS_PROJECT_CODE_ID == undefined ||
        saveData.PMS_PROJECT_CODE_ID == 0
      ) {
        saveData.GUID = localThis.makeUUID();
      } else {
        if (saveData.GUID == undefined || saveData.GUID == "")
          saveData.GUID = localThis.makeUUID();
      }
      saveData.BLOCKED = "TRUE";

      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: SqlActionType.NUMBER_3,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        // officeId: localThis.getCurrentProjects.HR_OFFICE_ID,
      };
      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();
      this.$confirm(msg).then((res) => {
        if (res) {
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showNewSnackbar({
                typeName: "succes",
                text: res.message,
              });

              //If "new" then load the data
            })
            .then((res: any) => {
              let ofc = localThis.getOfficeDetailsFromId(saveData.PMS_OFFICE_ID);
              return ofc;
            })

            .then((res: any) => {
              value.dimensionCode = "PROJECT";
              value.project_code = saveData.PMS_PROJECT_CODE;
              value.descr = saveData.PMS_PROJECT_CODE_DESC;
              value.guid = saveData.GUID;
              value.location_description = res.LOCATION_DESCRIPTION;
              value.office_code = saveData.PMS_OFFICE_CODE;
              value.blocked = "TRUE";
              localThis.updateNavDimension(value);
            })
            .catch((err: any) => {
              console.log("There is an error");
              localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
            })
            .finally(() => {
              localThis.isSaving = false;
              localThis.addMode = false;
              localThis.editMode = false;
              localThis.keyRnd = Math.ceil(Math.random() * 1000);
            });
        }
      });
    },

    /**
     * Close detail section and open list.
     */

    closeDetail() {
      let localThis: any = this as any;
      localThis.addMode = false;
      localThis.selectedItemProjects = null; //Assign is done in this way to guarantee reactivity
    },
    /**
     * Get structure of the Biodata Form
     */
    getFormStructProjects() {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewNameViProjects,
        colList: null,
        whereCond: null,
        orderStmt: null,
      };
      // localThis.formFields = localThis
      localThis
        .getJRSTableStructure({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Setup CRUD info
          Object.assign(localThis.crud_info, res.crud_info);

          // Setup Form Fields
          let tmpFormFileds: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis.formFieldsProjects = localThis.setupSelectFields(tmpFormFileds);
        });
    },

    cancel() {
      let localThis: any = this as any;
      localThis.addMode = false;
      localThis.editMode = false;
    },

    checkIfModifiableProject(saveData: any) {
      let localThis: any = this as any;

      if (
        saveData.NUMBER_OF_AP > 0 &&
        (saveData.PMS_OFFICE_ID != localThis.originalSelectedItemProjects.PMS_OFFICE_ID ||
          saveData.PMS_PROJECT_CODE !=
            localThis.originalSelectedItemProjects.PMS_PROJECT_CODE)
      ) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Project Code not modified: There are already Annual Plan that use it",
        });
        return false;
      } else return true;
    },

    checkIfOldProjectMustBeBlocked(saveData: any) {
      let localThis: any = this as any;

      if (
        saveData.NUMBER_OF_AP == 0 &&
        (saveData.PMS_OFFICE_ID != localThis.originalSelectedItemProjects.PMS_OFFICE_ID ||
          saveData.PMS_PROJECT_CODE !=
            localThis.originalSelectedItemProjects.PMS_PROJECT_CODE)
      ) {
        return true;
      } else return false;
    },

    /**
     * Save form data.
     */
    saveFormData(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      // Check form validity
      if (!formValidateFunc()) {
        return null;
      }

      let value: any = {};
      let localThis: any = this as any;

      if (localThis.editMode == true) {
        if (!localThis.checkIfModifiableProject(saveData)) {
          //
          return;
        }
      }

      localThis.isSaving = true;
      let spName: string =
        actionType == SqlActionType.NUMBER_0
          ? localThis.crud_info.create_sp
          : localThis.crud_info.update_sp;

      if (
        saveData.PMS_PROJECT_CODE_ID == undefined ||
        saveData.PMS_PROJECT_CODE_ID == 0
      ) {
        saveData.GUID = localThis.makeUUID();
      } else {
        if (saveData.GUID == undefined || saveData.GUID == "")
          saveData.GUID = localThis.makeUUID();
      }
      saveData.BLOCKED = "FALSE";

      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: actionType,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        // officeId: localThis.getCurrentProjects.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "succes",
            text: res.message,
          });

          if (formResetFunc) {
            formResetFunc();
          }

          //If "new" then load the data
        })
        .then((res: any) => {
          let ofc = localThis.getOfficeDetailsFromId(saveData.PMS_OFFICE_ID);
          return ofc;
        })

        .then((res: any) => {
          if (localThis.checkIfOldProjectMustBeBlocked(saveData)) {
            saveData.GUID = localThis.makeUUID();
            localThis.getOfficeDetailsFromId(saveData.PMS_OFFICE_ID).then((res1: any) => {
              let valueold: any = {};
              valueold.dimensionCode = "PROJECT";
              valueold.project_code =
                localThis.originalSelectedItemProjects.PMS_PROJECT_CODE;
              valueold.descr =
                localThis.originalSelectedItemProjects.PMS_PROJECT_CODE_DESC;
              valueold.guid = localThis.originalSelectedItemProjects.GUID;
              valueold.location_description = res1.LOCATION_DESCRIPTION;
              valueold.office_code =
                localThis.originalSelectedItemProjects.PMS_OFFICE_CODE;
              valueold.blocked = "TRUE";

              localThis.updateNavDimension(valueold);
            });
          }
          value.dimensionCode = "PROJECT";
          value.project_code = saveData.PMS_PROJECT_CODE;
          value.descr = saveData.PMS_PROJECT_CODE_DESC;
          value.guid = saveData.GUID;
          value.location_description = res.LOCATION_DESCRIPTION;
          value.office_code = saveData.selectHolder_PMS_OFFICE_ID.HR_OFFICE_CODE;
          value.blocked = "FALSE";
          localThis.updateNavDimension(value);
        })
        .catch((err: any) => {
          console.log("There is an error");
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
          localThis.addMode = false;
          localThis.editMode = false;
          localThis.keyRnd = Math.ceil(Math.random() * 1000);
        });
    },
  },
  beforeMount() {
    let localThis: any = this;
    localThis.role = localThis.getCurrentRole.ROLE_CODE;

    if (localThis.role == "PC") {
      localThis.notAuthorizedRole = false;
      //localThis.getStatusList();
    } else {
      localThis.notAuthorizedRole = true;
    }
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getFormStructProjects();
    localThis.role = localThis.getCurrentRole.ROLE_ID;

    EventBus.$on("userRoleUpdated", (to: any) => {
      let localThis: any = this as any;
      localThis.role = localThis.getCurrentRole.ROLE_ID;
      localThis.role = localThis.getCurrentRole.ROLE_CODE;
      if (localThis.role == "PC") {
        localThis.notAuthorizedRole = false;
        localThis.keyRnd = "Math.ceil(Math.random() * 1000)";
      } else {
        localThis.notAuthorizedRole = true;
      }
    });
  },
});
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>
