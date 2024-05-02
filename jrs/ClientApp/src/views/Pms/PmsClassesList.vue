<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <!--- SIMPLE TABLE FO CLASSES-->
        <v-col :cols="12">
          <jrs-simple-table
            :viewName="'PMS_VI_CLASS'"
            :savePayload="{
              userUid: getUserUid,
              officeId: getCurrentOffice.HR_OFFICE_ID,
            }"
            :dataSourceCondition="`JRS_EMPLOYEE = '${getUserUid}'`"
            :fieldDatasourceConditions="[
              {
                field_name: 'PMS_CLASS_PROCESS_ID',
                field_condition: `JRS_EMPLOYEE = '${getUserUid}'`,
              },
            ]"
          >
            <template
              v-slot:simple-table-item-actions="{
                simpleItemRowItem,
                refreshData,
              }"
            >
              <v-icon
                class="mx-2"
                v-if="!simpleItemRowItem.IMS_USER_IS_ANONYMIZED"
                @click="
                  (selectParticipatingUsers = []),
                    openOpenDialogParticipatingUser(
                      simpleItemRowItem,
                      refreshData
                    )
                "
                >mdi-account-multiple-plus</v-icon
              >
            </template>
          </jrs-simple-table>
        </v-col>
      </v-row>

      <!--- ACTIVATE DIALOG OF CLASS (PARTECIPATING USER)-->
      <v-dialog
        v-model="openDialogActivate"
        persistent
        retain-focus
        :overlay="false"
        max-width="45em"
        transition="dialog-transition"
      >
        <v-card>
          <v-container fluid>
            <v-card-title primary-title class="text-capitalize">{{
              $t("general.crud.select-partecipating-user")
            }}</v-card-title>
            <v-card-text
              :key="
                selectParticipatingUsers[0]
                  ? selectParticipatingUsers[0].IMS_USER_UID
                    ? selectParticipatingUsers[0].IMS_USER_UID
                    : 0
                  : 0
              "
            >
              <v-row align="center">
                <v-col cols="12">
                  <jrs-multi-select
                    v-model="selectParticipatingUsers"
                    :label="$t('datasource.pms.class.participating-user.title')"
                    :dataSource="'HR_VI_USER_SELECTOR'"
                    :itemValue="'IMS_USER_UID'"
                    :itemText="'HR_BIODATA_FULL_NAME'"
                    :required="true"
                    :returnOnlyIds="false"
                    :dataSourceCondition="dataSourceCondition"
                    :iconAppend="'mdi-account-circle'"
                    :rowDisableSelectRules="rowDisableSelectRules"
                  >
                  </jrs-multi-select>
                </v-col>
              </v-row>
            </v-card-text>
            <v-card-actions>
              <v-btn
                text
                color="secondary darken-1"
                @click="openOpenDialogParticipatingUser([])"
                >X {{ $t("general.close") }}</v-btn
              >
              <v-btn color="primary" @click="UpdParticipatingUser()">{{
                $t("general.save")
              }}</v-btn>
            </v-card-actions>
          </v-container>
        </v-card>
      </v-dialog>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsMultiSelectTable from "../../components/JrsMultiSelectedTable.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-multi-select": JrsMultiSelectTable,
  },
  data() {
    return {
      tabModel: null,
      check: false,
      tables: [
        {
          name: "PMS_VI_CLASS",
          code: "CLASS",
          tab_key: "datasource.hrm.biodata.deleted.title",
        },
      ],
      selectParticipatingUsers: [],
      selectedClass: [],
      openDialogAnonymization: false,
      openDialogActivate: false,
      refreshData: Function,
      dataSourceCondition: "isBeneficiary = 1",
      rowDisableSelectRules: [],
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

    /**
     * Open dialog to manage partecipating users of specific class
     */
    async openOpenDialogParticipatingUser(userData: any, functionRefresh: any) {
      let localThis: any = this as any;

      localThis.selectedClass = userData;
      localThis.openDialogActivate = !localThis.openDialogActivate;
      localThis.refreshData = functionRefresh;

      localThis.selectParticipatingUsers =
        await localThis.getParticipantUserByClass();
    },

    /**
     * Get all partecipating users of specific class
     */
    async getParticipantUserByClass() {
      let localThis: any = this as any;

      localThis.users = [];
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_PARTICIPANTING_USER_CLASS",
        colList: null,
        whereCond: `PMS_CLASS_ID = ${localThis.selectedClass.PMS_CLASS_ID}`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.users.push(item);
            });
          }
          return localThis.users;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },

    /**
     *Funciton to call SP: Update Partecipating users of specific class
     */
    UpdParticipatingUser() {
      let localThis: any = this as any;
      localThis.isSaving = true;

      let saveData = {
        rows: localThis.selectParticipatingUsers,
        class_id: localThis.selectedClass.PMS_CLASS_ID,
      };

      let savePayload: GenericSqlPayload = {
        spName: "PMS_SP_INS_UPD_PARTICIPATING_USER_CLASS",
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          });
          localThis.openDialogActivate = !localThis.openDialogActivate;
          localThis.refreshData();
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
});
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>
