<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row v-if="showWait">
        <v-col :cols="12">
          <div class="text-center">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>
        </v-col>
      </v-row>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row>
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-training-form-list.title") }}
                <v-icon large> mdi-account-search</v-icon>
              </h1>
              <p>{{ $t("views.view-training-form-list.view-info") }}</p>
            </v-col>
          </v-row>

          <!-- MAIN FILTER -->
          <v-card outlined class="lv-0" dense>
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="left">
                <!-- PROCESS SELECT -->
                <v-col :cols="4">
                  <v-autocomplete
                    v-model="process"
                    :label="$t('views.view-training-form-list.activity-select')"
                    :items="processList"
                    :hint="$t('views.view-training-form-list.activity-select-hint')"
                    :persistent-hint="true"
                    item-value="ACTIVITY_ID"
                    item-text="ACTIVITY_DESCR"
                    return-object
                    clearable
                    @change="setDatatableCOnditions()"
                  ></v-autocomplete>
                </v-col>
              </v-row>
              <v-row>
                <v-col :cols="6">
                  <jrs-date-picker
                    :label="$t('views.view-training-form-list.date-from')"
                    :hint="$t('views.view-training-form-list.date-from-hint')"
                    :persistent-hint="true"
                    v-model="dateSelectedFrom"
                  ></jrs-date-picker>
                </v-col>

                <!-- DATE TO -->
                <v-col :cols="6">
                  <jrs-date-picker
                    :label="$t('views.view-training-form-list.date-to')"
                    :hint="$t('views.view-training-form-list.date-to-hint')"
                    :persistent-hint="true"
                    v-model="dateSelectedTo"
                  ></jrs-date-picker>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <v-divider></v-divider>
        </v-col>

        <!-- SIMPLE TABLE CASE VISIT LIST -->
        <v-col :cols="12">
          <jrs-simple-table
            :viewName="viewNameViProjects"
            :selectable="false"
            :hideNewBtn="true"
            :hideActions="true"
            :key="keyRnd"
            :dataSourceCondition="conditionProcessInstance"
          >
            <!-- TABLE HEADER -->

            <!-- ROW ACTIONS -->
            <!-- ROW ACTIONS -->

            <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
              <v-icon @click="editFormData(simpleItemRowItem)"
                >mdi-circle-edit-outline</v-icon
              >
              <v-icon @click="deleteFormData(simpleItemRowItem)">mdi-delete</v-icon>
            </template>
          </jrs-simple-table>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsDatepicker from "../../components/JrsDatePicker.vue";

import FormMixin from "../../mixins/FormMixin";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsDatePicker from "../../components/JrsDatePicker.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,

    "jrs-date-picker": JrsDatepicker,
  },
  mixins: [FormMixin],
  data() {
    return {
      viewNameViProjects: "PMS_VI_TRAINING_FORM",
      keyRnd: 0,
      showWait: false,
      tabModel: null,
      check: false,

      processList: [],
      process: {},

      biodataSelected: {},

      dateSelectedFrom: null,
      dateSelectedTo: null,
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
    editFormData(item: any) {
      let localThis = this as any;
      localThis.$router.push({
        name: "trainingForm",
        params: {
          selectedTrainingFormId: item.ID,
          processId:item.PROCESS_ID,
          projectDescr:item.PROJECT,
          processDescr:item.PROCESS_DESCR,
        
        },
      });
    },
    deleteFormData(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as any;
          payLoadD.ID = item.ID;
          localThis.showWait = true;
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_DEL_TRAINING_FORM",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.showWait = false;

              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message,
              }); // Feedback to user
              localThis.keyRnd = Math.ceil(Math.random() * 1000);
            })
            .catch((err: any) => {
              localThis.showWait = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message,
              }); // Feedback to user
            });
        }
      });
    },

    /**
     * Return a list of active process for the provided user (The user that have to take in account the process).
     */

    getProcess() {
      let localThis: any = this as any;

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_PROCESS_FOR_USER",
        colList: null,
        orderStmt: null,
        whereCond: `OFFICE_FILTER_ID = ${localThis.getCurrentOffice.HR_OFFICE_ID}`,
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.item_count > 0) {
            // Setup projectList
            let projList = res.table_data.map((row: any) => {
              return {
                ACTIVITY_ID: row.PROCESS_ID,
                ACTIVITY_DESCR: row.PROCESS_FULL_DESCR,
              };
            });
            localThis.processList = [...projList];
          }
        });
    },
  },

  mounted() {
    let localThis: any = this as any;
    localThis.getProcess();
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),

    /**
     * All conditions of filters :Process/Class/Date From/Date To
     */
    conditionProcessInstance() {
      let localThis: any = this as any;
      let condition: string = "OFFICE_FILTER_ID = "+localThis.getCurrentOffice.HR_OFFICE_ID+" ";
      var dateFormatFrom: string = "";
      var dateFormatTo: string = "";
      if (localThis.dateSelectedFrom) {
        let date = new Date(localThis.dateSelectedFrom.toISOString());
        var dd = String(date.getDate()).padStart(2, "0");
        var mm = String(date.getMonth() + 1).padStart(2, "0"); //January is 0!
        var yyyy = date.getFullYear();
        dateFormatFrom = mm + "/" + dd + "/" + yyyy;
      }
      if (localThis.dateSelectedTo) {
        let date_ = new Date(localThis.dateSelectedTo.toISOString());
        var dd_ = String(date_.getDate()).padStart(2, "0");
        var mm_ = String(date_.getMonth() + 1).padStart(2, "0"); //January is 0!
        var yyyy_ = date_.getFullYear();
        dateFormatTo = mm_ + "/" + dd_ + "/" + yyyy_;
      }

      let conditionProcess = localThis.process
        ? localThis.process.ACTIVITY_ID
          ? ` AND PROCESS_ID = ${localThis.process.ACTIVITY_ID}`
          : ""
        : "";
      let conditionBeneficiary = localThis.biodataSelected;

      let conditionDate = localThis.dateSelectedFrom
        ? ` AND  START_DATE >=  '${dateFormatFrom} 00:00:00'`
        : "";
      localThis.dateSelectedTo
        ? (conditionDate += ` AND END_DATE < '${dateFormatTo} 23:59:59' `)
        : null;

      //let conditionUser = ` CASE_VISIT_OPERATOR_USER_UID = '${localThis.getUserUid}' `;
      condition +=
        //conditionUser +
        conditionProcess + conditionDate;
      return condition;
    },
  },
});
</script>

<style scoped></style>
