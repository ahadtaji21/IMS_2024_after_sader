<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col>
          <!-- TABLE -->

          <jrs-simple-table
            viewName="SET_VI_SETTLEMENTS_BIODATA"
            :title="$t('nav.settlements')"
            :nbrOfFormColumns="3"
            :selectable="false"
            :hideNewBtn="true"
            :hideActions="true"
            :selectOnRowClick="selectOnRowClick"
            @rowClick="doClickAction"
            @setSubOfficeId="setSubOffice"
            v-if="!selectedSettlement"
          >
            <!-- TABLE HEAD -->
            <template v-slot:simple-table-header="{ whereCondition, order }">
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="setupNewForm($event)"
              >
                <v-icon>mdi-plus-circle-outline</v-icon>New
              </v-btn>
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn
                    color="secondary lighten-2"
                    class="grey--text text--darken-3 ml-2"
                    @click="downloadLatest(whereCondition, order)"
                    v-bind="attrs"
                    v-on="on"
                  >
                    <v-icon>mdi-download</v-icon><v-icon>mdi-file-excel</v-icon> Download
                    Spec
                  </v-btn>
                </template>
                <span
                  >Download Last version of Detention centers with specifications</span
                >
              </v-tooltip>
            </template>
            <!-- TABLE ITEM ACTIONS -->
            <template
              v-slot:simple-table-item-actions="{
                simpleItemRowItem,
                //refreshData,
              }"
            >
              <!-- BTN - UPDATE BIODATA USER -->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn
                    icon
                    @click="setupEditForm(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                  >
                    <v-icon>mdi-circle-edit-outline</v-icon>
                  </v-btn>
                </template>
                <span>{{ $t("general.crud.tooltip-edit") }}</span>
              </v-tooltip>
              <!-- BTN - HISTORY OF BIO SETTLEMENT  AT:cancellato temporaneamente, richiesta di LUCA-->
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-icon
                    v-if="simpleItemRowItem && simpleItemRowItem.DATE_LAST_VERSION_SPEC"
                    @click="openDialog(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                    >mdi-history</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.tooltip-chronology") }}</span>
              </v-tooltip>
            </template>
          </jrs-simple-table>

          <!-- NEW/EDIT FORM -->

          <set-biodata-form
            :settlementId="selectedSettlement.SET_ID"
            :readonly="false"
            @newSettlement="setNewBiodata($event)"
            :selectedOffice="selectedOffice"
            v-if="selectedSettlement"
          >
            <template v-slot:form-toolbar-title>
              <span style="white-space: pre" v-if="formTitle">{{ formTitle }}</span>
            </template>
            <template v-slot:form-toolbar-content>
              <v-spacer></v-spacer>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3 ma-2"
                @click="openBackDialog = true"
              >
                <v-icon>mdi-table</v-icon>
                {{ $t("datasource.hrm.biodata.btn-to-list") }}
              </v-btn>
            </template>
          </set-biodata-form>
        </v-col>
      </v-row>

      <!-- BACK CONFIRM -->
      <v-dialog
        v-model="openBackDialog"
        persistent
        retain-focus
        :overlay="false"
        max-width="45em"
        transition="dialog-transition"
        prepend-icon="mdi-alert"
      >
        <v-card>
          <v-card-title primary-title class="text-capitalize">
            <v-icon color="warning" class="mr-1 mb-1"> mdi-alert </v-icon
            >{{ $t("general.alert") }}</v-card-title
          >
          <v-card-text class="capital-first-letter">{{
            $t("datasource.pms.annual-plan.ap-leave-confirm")
          }}</v-card-text>
          <v-card-actions>
            <v-btn text color="secondary darken-1" @click="openBackDialog = false"
              >X {{ $t("general.close") }}</v-btn
            >
            <v-btn
              color="primary"
              @click="
                selectedSettlement = null;
                openBackDialog = false;
              "
              >{{ $t("general.confirm") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- HISTORY DIALOG SPECIFICATIONS-->
      <v-dialog
        v-model="dialog"
        :key="setSelectedId"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="'99em'"
        transition="dialog-transition"
      >
        <v-card>
          <v-card-title>
            <span class="capital-first-letter">{{ $t("general.detail") }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col :cols="3" style="border-right: solid 1px rgba(0, 0, 0, 0.12)">
                <v-list>
                  <v-subheader class="text-uppercase">{{
                    $t("datasource.hrm.agreement.select-version")
                  }}</v-subheader>
                  <v-divider></v-divider>
                  <v-list-item-group v-model="currentVersion" color="primary">
                    <v-list-item
                      v-for="version in versions"
                      :key="version.SET_SPEC_ID"
                      :disabled="
                        versions[currentVersion]
                          ? versions[currentVersion].SET_SPEC_DATE ==
                            version.SET_SPEC_DATE
                            ? true
                            : false
                          : false
                      "
                    >
                      <!--                 :disabled="
                        versions[currentVersion].SET_SPEC_DATE ==
                        version.SET_SPEC_DATE
                          ? true
                          : false
                      "-->
                      <v-list-item-content>
                        Version:
                        {{ new Date(version.SET_SPEC_DATE).toLocaleString() }}
                        {{ version.HR_BIODATA_FULL_NAME }}
                      </v-list-item-content>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
              </v-col>
              <v-col :cols="9">
                <set-biodata-form
                  :settlementId="setSelectedId"
                  :readonly="true"
                  :selectedOffice="selectedOffice"
                  :specifications="versions ? versions[currentVersion] : null"
                  :pre_specifications="versions[currentVersion + 1]"
                >
                </set-biodata-form>
              </v-col>
            </v-row>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="
                dialog = false;
                selectOnRowClick = true;
              "
              >X {{ $t("general.close") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- HISTORY DIALOG FOR LEGAL -->
      <v-dialog
        v-model="dialogLegal"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="'100em'"
        transition="dialog-transition"
      >
        <v-card>
          <v-card-title>
            <span class="capital-first-letter">{{ $t("general.detail") }}</span>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col :cols="3" style="border-right: solid 1px rgba(0, 0, 0, 0.12)">
                <v-list>
                  <v-subheader class="text-uppercase">{{
                    $t("datasource.hrm.agreement.select-version")
                  }}</v-subheader>
                  <v-divider></v-divider>

                  <v-list-item-group v-model="currentVersionLegal" color="primary">
                    <v-list-item
                      v-for="version in versionsLegal_"
                      :key="version.SET_LEGAL_ID"
                    >
                      <v-list-item-content
                        @click="
                          legalVersionSelected = versionsLegal.filter((ele) => {
                            return ele.SET_LEGAL_DATE == version.SET_LEGAL_DATE;
                          })
                        "
                      >
                        Version:
                        {{ new Date(version.SET_LEGAL_DATE).toLocaleString() }}
                        {{ version.HR_BIODATA_FULL_NAME }}
                      </v-list-item-content>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
              </v-col>
              <v-col :cols="9" v-if="versionsLegal[0]">
                <set-biodata-form
                  :settlementId="setSelectedId"
                  :readonly="true"
                  :selectedOffice="selectedOffice"
                  :legalDetails="legalVersionSelected"
                >
                </set-biodata-form>
              </v-col>
            </v-row>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="dialogLegal = false"
              >X {{ $t("general.close") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- DELETE CONFIRM -->
      <v-dialog
        v-model="deleteDialog"
        persistent
        retain-focus
        :overlay="false"
        max-width="45em"
        transition="dialog-transition"
      >
        <v-card>
          <v-card-title primary-title class="text-capitalize">{{
            $t("general.crud.deletion-user")
          }}</v-card-title>
          <v-card-text class="capital-first-letter">{{
            $t("general.crud.delete-user-confirm")
          }}</v-card-text>
          <v-card-actions>
            <v-btn text color="secondary darken-1" @click="toggleDeleteDialogBiodata()"
              >X {{ $t("general.close") }}</v-btn
            >
            <v-btn color="primary" @click="deleteBiodataUser(deleteItemBiodata)">{{
              $t("general.delete")
            }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import SetSettlementsBiodataForm from "../../components/SET/SetSettlementsBiodataForm.vue";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import JrsReadonlyDetail from "../../components/JrsReadonyDetail.vue";
import FormMixin from "../../mixins/FormMixin";
import UtilMix from "../../mixins/UtilMix";
export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "set-biodata-form": SetSettlementsBiodataForm,
    // "jrs-readonly-detail":JrsReadonlyDetail
  },
  data: () => ({
    selectedSettlement: null,
    selectedOffice: 0,
    formTitle: "",
    isSaving: false,
    deleteDialog: false,
    resfreshBiodataFunction: null,
    versions: [],
    openBackDialog: false,
    versionsLegal: [],
    versionsLegal_: [],
    legalVersionSelected: [],
    dataLegalSelected: [],
    currentVersion: 0,
    firstDateVersion: null,
    currentVersionLegal: 0,
    legalDateSelected: null,
    dialog: false,
    dialogLegal: false,
    setSelectedId: 0,
    selectOnRowClick: true,
  }),
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  mixins: [FormMixin, UtilMix],
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),

    ...mapActions("apiHandler", {
      execSPCall: "execSPCall",
      getGenericSelect: "getGenericSelect",
      getJRSTableStructure: "getJRSTableStructure",
      generateExcelForTable: "generateExcelForTable",
    }),
    setSubOffice(suboffice: any) {
      const localThis: any = this as any;
      if (suboffice == undefined || suboffice == 0) {
        localThis.selectedOffice = localThis.getCurrentOffice.HR_OFFICE_ID;
      } else {
        localThis.selectedOffice = suboffice;
      }
    },
    async downloadLatest(wherecondition: any, order: any) {
      let colLabels: Array<any> = [];
      const localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_VI_SETTLEMENTS_SPECIFICATIONS_LAST",
      };

      // Get Columns from view with translated labels
      colLabels = await localThis
        .getJRSTableStructure({
          genericSqlPayload: selectPayload,
        })
        .then((table_data: any) => {
          const table_columns = table_data.columns.filter((col: any) => {
            return !col.hide_in_form;
          });
          const fields = table_columns.map((t: any) => {
            return localThis.parseJrsFormField(t);
          });
          const tabs_list = localThis.getFormTabs(fields);

          return table_columns
            .sort((a: any, b: any) => {
              let TabA: any;
              let TabB: any;
              tabs_list.forEach((t: any, index: any) => {
                if (t.tabCode == a.tabCode) {
                  TabA = index;
                }
                if (t.tabCode == b.tabCode) {
                  TabB = index;
                }
              });

              if (TabA > TabB) {
                return 1;
              } else if (TabA < TabB) {
                return -1;
              } else {
                return a.form_order >= b.form_order ? 1 : -1;
              }
            })
            .map((ele: any) => {
              return {
                columnName: ele.column_name,
                columnLabel: localThis.$t(ele.translationtKey),
              };
            });
        });

      colLabels = [
        {
          columnName: "SET_NAME",
          columnLabel: localThis.$t("datasource.set.settlement-biodata.name"),
        },
        {
          columnName: "SET_SPEC_DATE",
          columnLabel: localThis.$t("datasource.set.settlement-biodata.date-upd-latest"),
        },
        {
          columnName: "HR_BIODATA_FULL_NAME",
          columnLabel: localThis.$t(
            "datasource.set.settlement-biodata.operator-insert-last-version-spec"
          ),
        },
        ...colLabels,
      ];

      // Condition
      let condition =
        wherecondition != "1=1"
          ? " AND SET_SETTLEMENT_ID IN (SELECT SET_ID FROM SET_VI_SETTLEMENTS_BIODATA WHERE " +
            wherecondition +
            ")"
          : wherecondition;
      const listColumn: any = [];
      colLabels.forEach((e) => {
        listColumn.push(e.columnName);
      });
      localThis
        .generateExcelForTable({
          viewName: "SET_VI_SETTLEMENTS_SPECIFICATIONS_LAST",
          whereCond: condition,
          orderStmt: undefined,
          officeId: localThis.selectedOffice, //localThis.getCurrentOffice.HR_OFFICE_ID,
          colList: listColumn.join(","),
          colLabels,
        })
        .then((byteArray: any[]) => {
          localThis.downloadFileFromBytes(
            byteArray,
            "SPECIFICATIONS_" + new Date().toLocaleDateString("en-US") + ".xlsx"
          );
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    /**
     * Set selectedSettlement for new user creation.
     */
    setupNewForm() {
      const localThis: any = this as any;
      localThis.selectedSettlement = {};
      // Set form title
      localThis.formTitle = localThis.$t("datasource.set.settlement-biodata.title-new");
    },

    async getSpecifications(settlement_id: any) {
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_VI_SETTLEMENTS_SPECIFICATIONS",
        colList: null,
        whereCond: `SET_SPEC_SETTLEMENT_ID = ${settlement_id}`,
        orderStmt: `SET_SPEC_DATE`,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            return res.table_data;
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },
    async getLegalVersions(settlement_id: any) {
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_VI_SETTLEMENTS_LEGAL",
        colList: null,
        whereCond: `SET_SETTLEMENT_ID = ${settlement_id}`,
        orderStmt: `SET_LEGAL_DATE`,
      };
      const localThis: any = this as any;
      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          this.versionsLegal_ = [];
          //Setup data
          if (res.table_data) {
            let date: any = null;
            res.table_data.forEach((ele: any) => {
              if (ele.SET_LEGAL_DATE != date) localThis.versionsLegal_.push(ele);
              date = ele.SET_LEGAL_DATE;
            });

            localThis.firstDateVersion = res.table_data[0].SET_LEGAL_DATE
              ? res.table_data[0].SET_LEGAL_DATE
              : null;

            return res.table_data;
          } else {
            return [];
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },
    doClickAction(item: any) {
      const localThis: any = this as any;
      localThis.setupEditForm(item);
    },

    /**
     * Set selectedSettlement from seleted user.
     * @param {any} selectedItem Data of the selected user and biodata.
     */
    setupEditForm(selectedItem: any) {
      const localThis: any = this as any;
      localThis.selectedSettlement = { ...selectedItem };
      localThis.selectedSettlement.OFFICE_FILTER_ID = localThis.selectedOffice;
      // Set form title
      const tranlsationKey = "datasource.set.settlement-biodata.title-update";
      let title = selectedItem.DATE_LAST_VERSION_SPEC
        ? "Detailed Information: " +
          localThis.formatDate(new Date(selectedItem.DATE_LAST_VERSION_SPEC))
        : "";
      let title_legal = selectedItem.DATE_LAST_VERSION_LEGAL
        ? "Legal: " + localThis.formatDate(new Date(selectedItem.DATE_LAST_VERSION_LEGAL))
        : "";
      localThis.formTitle = `${localThis.$t(
        tranlsationKey
      )} -  Last Versions : ${title}  ${title_legal}`;
    },
    async openDialog(rowData: any) {
      let localThis: any = this as any;
      localThis.selectOnRowClick = false;

      let specifications = await localThis.getSpecifications(rowData.SET_ID);

      localThis.versions = specifications;

      localThis.versions.sort(function (a: any, b: any) {
        const date1: any = new Date(a.SET_SPEC_DATE);
        const date2: any = new Date(b.SET_SPEC_DATE);
        let result = date2 - date1;
        return result;
      });

      localThis.setSelectedId = rowData.SET_ID;

      localThis.currentVersion = 0;
      localThis.dialog = true;
    },
    // Format using reusable function
    padTo2Digits(num: number) {
      return num.toString().padStart(2, "0");
    },

    // format as "DD/MM/YYYY"
    formatDate(date: Date) {
      let localThis: any = this as any;
      return [
        localThis.padTo2Digits(date.getDate()),
        localThis.padTo2Digits(date.getMonth() + 1),
        date.getFullYear(),
      ].join("/");
    },

    async openDialogLegal(rowData: any) {
      let localThis: any = this as any;

      let legalVersions = await localThis.getLegalVersions(rowData.SET_ID);

      localThis.versionsLegal = legalVersions; //JSON.parse(specifications);

      localThis.setSelectedId = rowData.SET_ID;
      localThis.legalVersionSelected = localThis.versionsLegal.filter((ele: any) => {
        return ele.SET_LEGAL_DATE == localThis.firstDateVersion;
      });
      localThis.currentVersionLegal = 0;
      localThis.dialogLegal = true;
    },
    /**
     * Set selectedSettlement from newly created IMS_USER and HR_BIODATA.
     * @param {any} newSettlement Data of newly created user and biodata.
     */
    setNewBiodata(newSettlement: any) {
      const localThis: any = this as any;
      localThis.selectedSettlement = { ...newSettlement };
    },
    /**
     * Open event to eliminate biodata user (in logic way)
     @param saveData The data to delete 
        @param actionType The action to delete CRUD
        */
    deleteBiodataUser(saveData: any) {
      let localThis: any = this as any;
      let spName: string = "HR_SP_INS_UPD_BIODATA";
      localThis.isSaving = true;
      //Add external data to payload
      if (localThis.savePayload) {
        saveData = {
          external_data: localThis.savePayload,
          rows: saveData,
        };
      }
      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: SqlActionType.NUMBER_2,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.selectedOffice, // localThis.getCurrentOffice.HR_OFFICE_ID,
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "success",
            text: res.message,
          });
          if (localThis.resfreshBiodataFunction) {
            localThis.resfreshBiodataFunction.call();
          }
          localThis.toggleDeleteDialogBiodata();
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          });
        })
        .finally(() => {
          localThis.isSaving = false;
        });
    },
    /**
     * Toggle Delete dialog and set item to delete and table refreshFunction.
     * @param {any} deleteItemBiodata Item to send to SP to delete.
     * @param {Function} resfreshData Function to call to refresh table data.
     */
    toggleDeleteDialogBiodata(deleteItemBiodata: any = {}, resfreshData?: Function) {
      const localThis: any = this as any;
      localThis.deleteDialog = !localThis.deleteDialog;
      localThis.deleteItemBiodata = deleteItemBiodata;
      localThis.resfreshBiodataFunction = resfreshData;
    },
  },
});
</script>

<style scoped></style>
