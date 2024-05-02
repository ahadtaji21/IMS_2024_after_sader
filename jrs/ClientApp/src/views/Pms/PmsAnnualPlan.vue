<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row align-center>
        <v-col justify-center>
          <v-data-table
            :headers="columns"
            :items="rows"
            item-key="name"
            multi-sort
            :search="tableFilter"
            class="text-capitalize"
          >
            <template v-slot:top>
              <div
                class="d-inline-flex align-center"
                style="width: 100%; height: 3.5em; padding: 0 1em"
              >
                {{ $tc("pms.ap", 2) }}
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog" persistent max-width="1100px">
                  <template v-slot:activator="{ on }">
                    <v-btn color="primary" dark v-on="on">
                      <v-icon>mdi-plus-circle-outline</v-icon>
                    </v-btn>
                  </template>
                  <pms-apd-form
                    @closeDialoge="closeDialog"
                    @closeDialogeAndSaveAnnualPlanMD="closeDialogAndSave"
                    :formData="editedItem"
                  ></pms-apd-form>
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
                ></v-text-field>
              </div>
            </template>
            <template v-slot:[`item.action`]="{ item }">
              <v-icon small class="mr-2" @click="editItem(item)"
                >mdi-circle-edit-outline</v-icon
              >
              <v-icon small @click="deleteItem(item)">mdi-delete</v-icon>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters } from "vuex";
import { PmsAnnualPlanApi, ImsApi, Configuration } from "../../axiosapi";
import { mapActions } from "vuex";
import { translate } from "../../i18n";
import AnnualPlanMainDataForm from "../../components/PMS/ANNUALPLAN/AnnualPlanMainDataForm.vue";
import { EventBus } from "../../event-bus";
import UtilMix from "../../mixins/UtilMix";
export default Vue.extend({
  components: {
    "pms-apd-form": AnnualPlanMainDataForm,
  },
  mixins: [UtilMix],
  data() {
    return {
      columnsTemplate: [
        {
          text: "Code",
          textKey: "datasource.pms.annual-plan.ap-code",
          align: "center",
          value: "apcode",
          sortable: true,
          filterable: true,
        },
        {
          text: "Description",
          textKey: "datasource.pms.annual-plan.ap-description",
          align: "center",
          value: "descr",
          sortable: true,
          filterable: true,
        },
        {
          text: "Location",
          textKey: "datasource.pms.annual-plan.ap-location",
          align: "center",
          value: "location_descr",
          sortable: true,
          filterable: true,
        },
        {
          text: "Country",
          textKey: "datasource.pms.annual-plan.ap-country",
          align: "center",
          value: "admin_area_name",
          sortable: true,
          filterable: true,
        },
        {
          text: "Start Year",
          textKey: "datasource.pms.annual-plan.ap-start-year",
          align: "center",
          value: "start_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "End Year",
          textKey: "datasource.pms.annual-plan.ap-end-year",
          align: "center",
          value: "end_year",
          sortable: true,
          filterable: true,
        },
        {
          text: "Status",
          textKey: "datasource.pms.annual-plan.ap-status-name",
          align: "center",
          value: "status_name",
          sortable: true,
          filterable: true,
        },

        { text: "Actions", value: "action", sortable: false },
      ],
      rowsObj: {},
      rows: [],
      descriptions: [],
      tableFilter: "",
      dialog: false,
      editedItem: {},
      itemModel: {
        apcode: null,
        descr: null,
        locationDescr: null,
        adminAreaName: null,
        startYear: null,
        statusName: null,
      },
    };
  },
  computed: {
    ...mapGetters({
      userUid: "getUserUid",
    }),
    columns() {
      let localThis = this as any;
      let cols = localThis.columnsTemplate.map((col: any) => {
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
    let localThis = this as any;
    localThis.getPMSAP();
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
      setAnnualPlan: "setAnnualPlan",
    }),
    getPMSAP() {
      let localThis = this as any;
      const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
      const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);
      return api
        .apiPmsAnnualPlanGet(config.baseOptions)
        .then((res) => {
          localThis.rows = res.data;
          var i = 0;
          // for (i=0;i<localThis.rows.length;i++)
          // {
          //     localThis.getLBLDesc(i,localThis.rows[i].pmsCoiDescriptionLabelId);

          // }
          return res;
        })
        .catch((err) => {
          // console.error(err);
          localThis.rows = [];
        });
    },
    closeDialog() {
      let localThis = this as any;
      localThis.dialog = false;
      localThis.editedItem = localThis.itemModel;
    },
    closeDialogAndSave(value: any) {
      let localThis = this as any;
      let msgUpd: string = this.$i18n.t("pms.coi-update-confirm").toString();
      let msgAdd: string = this.$i18n.t("pms.coi-add-confirm").toString();

      let id = 0;
      let msg = msgUpd;
      if (value["id"] == undefined) {
        msg = msgAdd;
        value.guid = localThis.makeUUID();
      } else {
        id = Number(value["id"]);
        if (value.guid == undefined || value.guid == "")
          value.guid = localThis.makeUUID();
      }


      this.$confirm(msg).then((res) => {
        if (res) {
          localThis.dialog = false;
          //localThis.editedItem = localThis.itemModel;
          //for (var key in value) {
          //    alert('keyy: ' + key + '\n' + 'value: ' + value[key]);
          //}
          const config: Configuration = localThis.$store.getters["auth/getApiConfig"];
          const api: PmsAnnualPlanApi = new PmsAnnualPlanApi(config);

          //ADD
          return api
            .apiPmsAnnualPlanIdPost(id, value, config.baseOptions)
            .then((response) => {
              let ap = {} as any;
              ap.annal_plan_id = value.id;
              ap.start_year = value.start_year;
              ap.end_year = value.end_year;
              ap.currency = value.currency_code;
              localThis.getPMSAP();
              localThis.editedItem = localThis.itemModel;
              EventBus.$emit("userRoleUpdated");
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({ typeName: "error", text: err.response.data });
            });
        }
      });
    },
    editItem(item: any) {
      //debugger;
      let localThis = this as any;
      localThis.editedItem = JSON.parse(JSON.stringify(item));
      localThis.dialog = true;
    },
    deleteItem(item: any) {
      let localThis = this as any;
      let msg: string = this.$i18n.t("pms.coi-delete-confirm").toString();

      this.$confirm(msg).then((res) => {
        if (res) {
          item["pmsCoiDeleted"] = true;
          const config: Configuration = localThis.$store.getters["auth/getApiConfig"];

          // const api: PmsCategoryOfInterventionApi = new PmsCategoryOfInterventionApi(config);
          // return api
          //     .apiPmsCategoryOfInterventionIdPut(Number(item['pmsCoiId']),item,  config.baseOptions)
          //     .then(response => {
          //         localThis.getCOI();
          //     })
          //     .catch(error => {
          //         alert(error);
          //     });
        }
      });
    },
  },
});
</script>
