<template>
  <v-container fluid fill-height>
    <v-row v-if="!visualizeProjDetails" align-center>
      <v-col justify-center :cols="12">
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
            <template v-slot:top >
              <div class="d-inline-flex align-center primary lighten-2 jrs-table-top" >
                <h3>{{ $tc('datasource.gmt.grant.gt-prj', 2) }}</h3>
                <v-spacer></v-spacer>
                  <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="switchItems(item)"
              >
                <v-icon>{{ sIcon }}</v-icon>
              </v-btn>
                <v-dialog v-model="dialog" persistent max-width="1100px">
                  <gt-add-prj
                    v-if="!visualizeProjDetails"
                    @closeDialoge="closeDialog"
                    v-model="prjId"
                    @UpdateItem="closeDialogeAndUpdateList"
                    :selectedObjectId="selectedObjectId"
                    :key="Math.ceil(Math.random() * 1000)"
                  ></gt-add-prj>

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
              <v-icon small class="mr-2" @click="visualizeDetailsProject(item)">mdi-eye</v-icon>
            </template>
            
          </v-data-table>
             
        </div>
         </v-col>
      </v-row>
      <v-row v-if="visualizeProjDetails">
        <v-col>
          <div>

            <pj-details
            :editItemMainData="editedItem"
            :editItemNarrativeId="editedItem.id"
            :editItemOBJ="editedItem"
            :onlyRead="onlyRead"
            @returnBackToList ="returnBackToList"
            >
            </pj-details>
            
        </div>
      </v-col>
      </v-row>
    
  </v-container>
  
</template>
<script lang="ts"> 
import Vue from "vue";

import { mapGetters } from "vuex";
import { Configuration } from "../../../../axiosapi";
import { translate, i18n } from "../../../../i18n";
import GrantsProjectsAddForm from "./GMTDonorProjectsAddForm.vue";
import { mapActions } from "vuex";
import { IPayLoadDataPRJFUND } from "./../../../PMS/PMSSharedInterfaces";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
import GMTAnnualPlanDetails from "./GMTProjectsFormDetails.vue"

interface GMTDonorsProjectsObject{
    location_id: Number| null,
    location_description: String | null,
    office_id: Number| null,
    office_code: String | null,
    status_name: String | null,
    code: String | null,
    code_id: Number| null,
    descr: String | null,
    start_year: Date | null,
    status_id: Number | null,
    currency_code: String | null,
    id: Number| null,
    apcode: String | null,
    location_descr: String | null
}
export default Vue.extend({
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  components: {
    "gt-add-prj": GrantsProjectsAddForm,
    "pj-details":GMTAnnualPlanDetails
  },
  data() {
    return {
      prjId: {},
      onlyRead: true,
      sIcon:"mdi-chevron-double-up",
      dialog: false,
      visualizeProjDetails:false,
      keyDialog: 0,
      itemsPerPage: 15,
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
          text: "Location Description",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "LOCATION_DESCR",
          sortable: true,
          filterable: true,
        },
        {
          text: "Grant Code",
          textKey: "datasource.gmt.grant.gts",
          align: "center",
          value: "GRANT_CODE",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      rowsObj: {},
      rows: [],
      descriptions: [],
      originalListProjects: [],
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
    (this as any).getDonorProjects();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setGrant: "setGrant",
      setAnnualPlan: "setAnnualPlan",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    reloadProject(item: any) {
      let localThis = this as any;
      localThis.originalListProjects = []
      

      localThis.getDonorProjects().then((res: any) => {
          localThis.editedItem = [];
          
          if (item) {
            
            for (let i = 0; i < localThis.originalListProjects.length; i++) {
              if (localThis.originalListProjects[i].ID == item.id) {
                 localThis.editedItem.push(JSON.parse(JSON.stringify(localThis.originalListProjects[i]))
                );
                
                localThis.rows = localThis.editedItem;
                localThis.itemsPerPage == 1;
                break;
              }
            }
          }
        
      });
    },
    switchItems(item:any) {
      let localThis = this as any;
      if (localThis.itemsPerPage == 1) {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-up";
        localThis.rows = localThis.originalListProjects;
      } else {
        localThis.itemsPerPage = 15;
        localThis.sIcon = "mdi-chevron-double-down";
        if (localThis.editedItem && localThis.editedItem.length == 1)
          localThis.rows = localThis.editedItem;
      }
    },
    closeDialogProj(){
        let localThis = this as any;
        localThis.visualizeProjDetails = false;
    },
    getDonorProjects() {
      let localThis = this as any;

      localThis.rows = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_PROJECTS",
        whereCond: `GMT_DONOR_ID='${localThis.selectedObjectId}'`,
        orderStmt: "ID",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              localThis.rows.push(item);
              localThis.originalListProjects.push(item);
            });
          }
        });
    },
    closeDialog() {
      let localThis = this as any;
      localThis.dialog = false;
      
    },
    closeDialogeAndUpdateList(value: any) {
      let localThis = this as any;
      localThis.getDonorProjects();
      localThis.dialog = false;
      localThis.keyDialog = Math.ceil(Math.random() * 1000);
    },
    visualizeDetailsProject(item: any) {
       let localThis: any = this as any;
       let gt = {} as any;
       localThis.visualizeProjDetails = true;
       let projects: GMTDonorsProjectsObject = {
              location_id: item.LOCATION_ID,
              location_description: item.LOCATION_DESCR,
              office_id: item.OFFICE_ID,
              office_code: item.OFFICE_CODE,
              status_name: item.IMS_STATUS_NAME,
              code: item.CODE,
              code_id: item.CODE_ID,
              descr: item.DESCR,
              start_year: item.START_YEAR,
              status_id: item.STATUS_ID,
              currency_code: item.CURRENCY_CODE,
              id: item.ID,
              location_descr: item.LOCATION_DESCR,
              apcode: item.APCODE
      }

      gt.grant_id = item.ID;
      gt.year = item.START_YEAR;
      gt.currency = item.CURRENCY_CODE;
      localThis.setGrant(gt);

      localThis.editedItem = projects;
       
    },
    returnBackToList(item:any){
      let localThis: any = this as any;
      localThis.visualizeProjDetails = false;
      localThis.reloadProject(item)
      localThis.switchItems(item)
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
          payLoadD.ID_GRANT = item.ID_GRANT;
          payLoadD.ID_PROJECT = Number.parseInt(item.ID);

          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_DEL_ANNUAL_GRANT_PROJECT",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.getDonorProjects();
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