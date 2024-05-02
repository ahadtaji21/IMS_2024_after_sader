<template>
  <v-container fluid fill-height>
    <v-row align-center>
      <v-col justify-center :cols="12">
        <div v-if="!showPrjDetails">
          <v-data-table
            :headers="columns"
            :items="rows"
            item-key="name"
            multi-sort
            :search="tableFilter"
            class="text-capitalize elevation-2"
          >
            <template v-slot:top>
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
                <h3>{{ $tc('datasource.gmt.grant.gts', 2) }}</h3>
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog" persistent max-width="1100px">
                  <template v-slot:activator="{ on }">
                    <v-btn color="secondary lighten-2" class="grey--text text--darken-3" v-on="on">
                      <v-icon>mdi-plus-circle-outline</v-icon>New
                    </v-btn>
                  </template>
                  <search-table
                    v-model="prjId"
                    @UpdateItem="closeDialog"
                    :selectedGrantId="selectedObjectId"
                    :key="Math.ceil(Math.random() * 1000)"
                    :CloseBtn="true"
                  ></search-table>
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
              <v-icon small class="mr-2" @click="editItem(item)">mdi-circle-edit-outline</v-icon> 
              <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
            </template>
          </v-data-table>
        </div>
      </v-col>
    </v-row>
        <v-row>
      <v-col align-center :cols="12">
        <div v-if="showPrjDetails">

          <ap-prj-details
          editItemOBJ="editedItem"
 
          ></ap-prj-details>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "../../../axiosapi";
import { translate, i18n } from "../../../i18n";
import SearchTable from "./FUNDED/GMTGrantProjectsSearch.vue";
import PrjDetails from "./../GRANTS/PROPOSAL/GMTAnnualPlanDetailsForm.vue";
import { mapActions } from "vuex";
import { IPayLoadDataPRJFUND } from "./../../PMS/PMSSharedInterfaces";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  components: {
    "search-table": SearchTable,
    "ap-prj-details": PrjDetails
  },
  data() {
    return {

      showPrjDetails:false,
      prjId: {},
      dialog: false,
      keyDialog: 0,
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.pms.annual-plan.ap-code",
          align: "center",
          value: "APCODE",
          sortable: true,
          filterable: true,
        },

        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-description",
          align: "center",
          value: "DESCR",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "LOCATION_DESCR",
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
        //  if (col.value === "pmsCoiDescription") {
        //      let newColDesc = col;
        //     newColDesc.text = translate(col.textKey);
        //     newColDesc.value = "pmsCoiCode";
        //     return newColDesc;
        // }

        let newCol = col;
        newCol.text = translate(col.textKey);

        return newCol;
      });

      return cols;
    },
  },
  mounted() {
    let localThis : any =this;
    localThis.getGRANTS();
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
    getGRANTS() {
      let localThis = this as any;

      localThis.rows = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_GRANT_PROJECTS",
        //colList: null,
        whereCond: `ID_GRANT='${localThis.selectedObjectId}'`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.rows.push(item);
            });
          }
        });
    },
    closeDialog(item: string) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as IPayLoadDataPRJFUND;
      payLoadD.ID_GRANT = localThis.selectedObjectId;
      payLoadD.ID_PROJECT = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_GRANT_PROJECT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.getGRANTS();
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    closeDialogeAndUpdateList(value: any) {
      let localThis = this as any;
      localThis.getGRANTS();
      localThis.dialog = false;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
    },
    editItem(item: any) {
      let localThis: any = this as any;
      localThis.showPrjDetails=true;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.$emit("showPrjDetails",false);

    },
    deleteItem(item: any) {
      let ap = {} as any;
      let localThis = this as any;

      let msg: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.del-item")
        .toString();

      this.$confirm(msg).then((res: any) => {
        if (res) {
          let payLoadD = {} as IPayLoadDataPRJFUND;
          ap = localThis.$store.getters.getAnnualPlan;
          payLoadD.ID_GRANT = localThis.selectedObjectId;
          payLoadD.ID_PROJECT = Number.parseInt(item.ID);

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_ANNUAL_GRANT_PROJECT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.getGRANTS();
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