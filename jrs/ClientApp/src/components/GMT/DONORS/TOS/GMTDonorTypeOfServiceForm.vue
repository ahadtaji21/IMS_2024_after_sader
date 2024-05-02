<template>
  <div>
    <div>
      <!-- SRV SELECTION-->

      <v-data-table
        :headers="headers"
        :items="tosList"
        item-key="GMT_TOS_ID"
        multi-sort
        :search="tableFilter"
        :items-per-page="itemsPerPage"
        class="text-capitalize"
   
      >
        <template v-slot:top>
          <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
            <h3>{{$t('nav.gmt-donor-tos')}}</h3>

            <v-spacer></v-spacer>
            <v-dialog
              v-model="editMode"
              persistent
              retain-focus
              :scrollable="true"
              :overlay="false"
              :max-width="((50 * nbrOfColumns) + 25) / 3 + 'em'"
              transition="dialog-transition"
            >
              <template v-slot:activator="{on}">
                <v-btn
                  color="secondary lighten-2"
                  class="grey--text text--darken-3"
                  v-on="on"
                  @click="openDlg()"
                >
                  <v-icon>mdi-plus-circle-outline</v-icon>New
                </v-btn>
              </template>
              <gmt-tos-form
                      @closeDialoge="closeDialog"
                      @closeDialogeAndSave="closeDialog"
                :formData="editedItemDialog"
                :selectedObjectId="selectedObjectId"
                :key="dlgKey"

              ></gmt-tos-form>
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
                <tbody style="border:solid">
                  <tr v-for="item in items" :key="item.GMT_TOS_ID" style="height:10px">
                    <td class="tablerow">{{ item.GMT_TOS_CODE }}</td>
                    <td class="tablerow">{{ item.GMT_TOS_DESCRIPTION }}</td>
                     <td class="tablerow">{{ item.PMS_TOS_CODE }}</td>
                    <td class="tablerow">
                      <v-icon
                        :color="item.GMT_TOS_ENABLED == true ? 'green darken-3' : 'deep-orange darken-3'"
                      >{{ item.GMT_TOS_ENABLED == true ? 'mdi-checkbox-marked-circle-outline' : 'mdi-checkbox-blank-circle-outline' }}</v-icon>
                    </td>
                    <td style="text-align:center">
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon
                            small
                            class="mr-2"
                            @click="editItem(item)"
                            v-on="on"
                          >mdi-circle-edit-outline</v-icon>
                        </template>
                        <span>{{ $t('datasource.pms.annual-plan.objectives.edit-activity') }}</span>
                      </v-tooltip>
                      <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                          <v-icon small class="mr-2" @click="deleteItem(item)" v-on="on">mdi-delete</v-icon>
                        </template>
                        <span>{{ $t('datasource.pms.annual-plan.objectives.delete-activity') }}</span>
                      </v-tooltip>
                    </td>
                  </tr>
                </tbody>
              </template>
      </v-data-table>
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../../../components/JrsTable.vue";
import { JrsHeader } from "../../../../models/JrsTable";
import {
  IImsList
} from "./../../../PMS/PMSSharedInterfaces";
import TOSMainData from "./GMTDonorTypeOfServiceMainData.vue";
import { i18n } from "../../../../i18n";

import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";


export default Vue.extend({
  components: {
    "gmt-tos-form": TOSMainData
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      showIndTabs: true,

      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      dlgKey:0,
      sIcon: "mdi-chevron-double-up",
      itemsPerPage: 15,
      showItemNumber: 0,
      tableFilter: "",

      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      formData: {},
      tosList: [],

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
    closeDialog() {
      let localThis: any = this as any;
     
      localThis.editMode = false;
      localThis.dialog = false;
      localThis.editedItemDialog ={} ;
      localThis.getDonorTOS();
    },
    openDlg()
    {
        let localThis: any = this as any;
  
        localThis.editMode = !localThis.editMode;
        localThis.dlgKey =  Math.ceil(Math.random() * 1000);
         
    },
    getDonorTOS() {
      let localThis: any = this as any;
      localThis.tosList = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONORS_TYPE_OF_SERVICE",
        colList: null,
        whereCond: `GMT_DONOR_ID = ${localThis.selectedObjectId}`,
        orderStmt: "GMT_TOS_CODE",
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.tosList.push(item);
            });
          }
        }
      );
    },
    deleteItem(item: any) {
      let localThis = this as any;
      localThis.showNewSnackbar({
        typeName: "error",
        text: "To be Implemented",
      });
    },

    editItem(item: any) {
      let localThis: any = this as any;
      localThis.editMode = true;
      localThis.dlgKey =  Math.ceil(Math.random() * 1000);
      //console.log("Clicked", item.name);
      localThis.editedItemDialog = JSON.parse(JSON.stringify(item));
    },
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
           text: this.$i18n.t("pms.coi-code").toString(),
          value: "GMT_TOS_CODE",
          align: "center",
          divider: true,
        },
        {
         text: this.$i18n.t("pms.coi-description").toString(),
          value: "GMT_TOS_DESCRIPTION",
          align: "center",
          divider: true,
        },
        {
           text: this.$i18n.t("pms.coi-code").toString(),
          value: "PMS_TOS_CODE",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("pms.coi-enabled").toString(),
          value: "GMT_TOS_ENABLED",
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
  beforeMount()
  {
       let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.getDonorTOS();
  }
  ,
  mounted() {
   
  },
  computed: {
    language() {
      return i18n.locale;
    },
  },
   watch: {
    presetVal: function (val) {
      let localThis: any = this as any;
      localThis.search = val;
    }
   }
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
  background-color: red;
  font-weight: bold;
}
</style>