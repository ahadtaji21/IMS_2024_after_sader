<template>
  <v-container fluid fill-height>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-row align-center>
      <v-col v-if="!visualizeDetailsGrant" justify-center :cols="12">
        <div>
          <v-data-table
            :headers="columns"
            :items="rows"
            item-key="name"
            multi-sort
            :search="tableFilter"
            :items-per-page="itemsPerPage"
            class="text-capitalize elevation-2"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $tc("datasource.gmt.grant.gts", 2) }}</h3>
                <v-spacer></v-spacer>
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  @click="switchItems(item)"
                >
                  <v-icon>{{ sIcon }}</v-icon>
                </v-btn>
                <v-dialog v-model="dialog" persistent max-width="1100px">
                  <template v-slot:activator="{ on }" v-if="!onlyRead">
                    <v-btn
                      color="secondary lighten-2"
                      class="grey--text text--darken-3"
                      v-on="on"
                    >
                      <v-icon>mdi-plus-circle-outline</v-icon>New
                    </v-btn>
                  </template>

                  <gt-add-gnt
                    @closeDialoge="closeDialog"
                    v-model="prjId"
                    @UpdateItem="closeDialogeAndUpdateList"
                    :selectedProjectId="selectedAnnualPlan.id"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></gt-add-gnt>
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
            <template v-slot:[`item.action`]="{ item }">
              <v-icon v-if="!onlyRead" small class="mr-2" @click="editItem(item)"
                >mdi-circle-edit-outline</v-icon
              >
              <v-icon v-if="onlyRead" small class="mr-2" @click="editItem(item)"
                >mdi-eye</v-icon
              >

              <v-icon small v-if="!onlyRead" @click="deleteItem(item)">mdi-delete</v-icon>
            </template>
          </v-data-table>
        </div>
      </v-col>
    </v-row>
    <v-row align-center>
      <v-col v-if="visualizeDetailsGrant" justify-center :cols="12">
        <!--<v-breadcrumbs :items="itemsBreadCumb">
            <v-breadcrumbs-item
              slot="item"
              slot-scope="{ item }"
              exact
              :to="item.to"
              click.native="handleFunctionCall(item.act, $event)"
            >
              <v-chip class="ma-2" :color="item.color">
                <v-avatar left>
                  <v-icon>{{ item.icon }}</v-icon>
                </v-avatar>
                {{ item.text }}
              </v-chip>
            </v-breadcrumbs-item>
            <template v-slot:divider>
              <v-icon>mdi-forward</v-icon>
            </template>
             <div class="vertical-center">
            <v-btn
              color="secondary lighten-2"
              class="grey--text text--darken-3"
              @click="returnToObjList()"
            >
              <v-icon>{{sIconBack}}</v-icon>
            </v-btn>
          </div>
          </v-breadcrumbs>-->

        <gt-details-gnt
          :editItemMainData="editItemMainData"
          :editItemNarrativeId="editItemMainData.ID"
          :editItemOBJ="editItemMainData"
          :donorPresetted="true"
          @returnBackToList="returnBackToList"
          :onlyRead="onlyRead"
          :key="Math.ceil(Math.random() * 1000)"
        >
        </gt-details-gnt>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "../../../../axiosapi";
import { translate, i18n } from "../../../../i18n";
import SearchTable from "./GMTProjectGrantSearch.vue";
import { mapActions } from "vuex";
import { IPayLoadDataPRJFUND } from "./../../../PMS/PMSSharedInterfaces";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
import DetailsGrant from "../../GRANTS/PROPOSAL/GMTGrantDetailsForm.vue";

interface grantDetailsFields {
  AMOUNT: Number | null;
  CODE: String | null;
  CURRENCY_CODE: String | null;
  DESCR: String | null;
  DONOR_ID: Number | 0;
  DONOR_NAME: String | null;
  ID: Number | null;
  IMS_STATUS_NAME: String | null;
  START_YEAR: String | null;
  STATUS_ID: Number | 0;
}
export default Vue.extend({
  props: {
    selectedAnnualPlan: {
      type: Object,
      required: true,
    },
  },

  components: {
    "gt-add-gnt": SearchTable,
    "gt-details-gnt": DetailsGrant,
  },

  data() {
    return {
      showWait: false,
      prjId: {},
      dialog: false,
      keyDialog: 0,
      editItemMainData: {},
      onlyRead: true,
      columnsTemplate: [
        {
          text: "Donor Name",
          textKey: "datasource.gmt.donor.dn-name",
          align: "center",
          value: "GMT_DONOR_NAME",
          sortable: true,
          filterable: true,
        },

        {
          text: "Start Year",
          textKey: "datasource.gmt.grant.gt-start-year",
          align: "center",
          value: "START_YEAR",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Code",
          textKey: "datasource.gmt.grant.gt-code",
          align: "center",
          value: "GMT_GRANT_CODE",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Amount",
          textKey: "datasource.gmt.grant.gt-amount",
          align: "center",
          value: "AMOUNT",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      rowsObj: {},
      rows: [],
      descriptions: [],
      tableFilter: "",
      editedItem: {},
      visualizeDetailsGrant: false,
      itemsPerPage: 15,
      sIcon: "mdi-chevron-double-up",
      grantsListOrig: [],
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
    }),
    columns() {
      let cols = (this as any).columnsTemplate.map((col: any) => {
        if (col.value === "action") {
          return col;
        }

        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

      return cols;
    },
  },
  mounted() {
    (this as any).getProjectGrants();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    getProjectGrants() {
      let localThis = this as any;

      localThis.rows = [];
      localThis.showWait = true;

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_PROJECTS",
        //colList: null,
        whereCond: `ID='${localThis.selectedAnnualPlan.id}'`,
        orderStmt: "GMT_DONOR_NAME",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.rows.push(item);
              localThis.grantsListOrig.push(item);
            });
          }
          localThis.keyDialog = Math.ceil(Math.random() * 1000);
          localThis.showWait = false;
        });
    },
    closeDialog() {
      let localThis = this as any;
      localThis.dialog = false;
    },
    closeDialogeAndUpdateList(item: any) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as IPayLoadDataPRJFUND;
      payLoadD.ID_PROJECT = localThis.selectedAnnualPlan.id;
      payLoadD.ID_GRANT = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_GRANT_PROJECT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getProjectGrants();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    switchItems(item: any) {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.rows = localThis.grantsListOrig;
      } else {
        localThis.itemsPerPage = 1;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.tmpSelectedItem && localThis.tmpSelectedItem.length == 1)
          localThis.rows = localThis.tmpSelectedItem;
      }
    },

    reloadGrant(item: any) {
      let localThis = this as any;
      localThis.grantsListOrig = [];
      localThis.getProjectGrants().then((res: any) => {
        localThis.tmpSelectedItem = [];

        if (item) {
          for (let i = 0; i < localThis.grantsListOrig.length; i++) {
            if (localThis.grantsListOrig[i].ID_GRANT == item.ID) {
              localThis.tmpSelectedItem.push(
                JSON.parse(JSON.stringify(localThis.grantsListOrig[i]))
              );
              localThis.rows = localThis.tmpSelectedItem;
              localThis.itemsPerPage == 1;
              break;
            }
          }
        }
      });
    },

    editItem(item: any) {
      let localThis = this as any;
      localThis.visualizeDetailsGrant = true;
      //console.log("Clicked", item.name);

      //localThis.editItemMainData = JSON.parse(JSON.stringify(item));

      let grantDetails: grantDetailsFields = {
        AMOUNT: item.AMOUNT,
        CODE: item.GMT_GRANT_CODE,
        CURRENCY_CODE: item.CURRENCY_CODE,
        DESCR: item.DESCR,
        DONOR_ID: item.GMT_DONOR_ID,
        DONOR_NAME: item.GMT_DONOR_NAME,
        ID: item.ID_GRANT,
        IMS_STATUS_NAME: item.IMS_STATUS_NAME,
        START_YEAR: item.START_YEAR,
        STATUS_ID: item.STATUS_ID,
      };

      let gt = {} as any;
      gt.grant_id = grantDetails.ID;
      gt.year = grantDetails.START_YEAR;
      gt.currency = grantDetails.CURRENCY_CODE;
      gt.donor_id = grantDetails.DONOR_ID;
      gt.donor_name = grantDetails.DONOR_NAME;
      localThis.setGrant(gt);
      localThis.editItemMainData = grantDetails;

      // let localThis: any = this as any;
      // let gt = {} as any;
      // gt.grant_id = item.ID;
      // gt.year = item.START_YEAR;
      // gt.currency = item.CURRENCY_CODE;
      // localThis.setGrant(gt);
      // localThis.editedItem = JSON.parse(JSON.stringify(item));
      // localThis.$router.push({
      //   name: "Grant Details",
      //   params: {
      //     editItemMainData: localThis.editedItem,
      //     editItemNarrativeId: localThis.editedItem.ID,
      //   },
      // });
      //this.rout.push(this.$route);
    },

    returnBackToList(item: any) {
      let localThis = this as any;

      localThis.visualizeDetailsGrant = !localThis.visualizeDetailsGrant;
      localThis.reloadGrant(item);
      localThis.switchItems(item);
    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n.t("datasource.gmt.grant.gt-delete-prj-gnt").toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as IPayLoadDataPRJFUND;
          ap = localThis.$store.getters.getAnnualPlan;
          payLoadD.ID_GRANT = Number.parseInt(item.GMT_GRANT_ID);
          payLoadD.ID_PROJECT = localThis.selectedAnnualPlan.id;

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_ANNUAL_GRANT_PROJECT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.getProjectGrants();
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
</style>
