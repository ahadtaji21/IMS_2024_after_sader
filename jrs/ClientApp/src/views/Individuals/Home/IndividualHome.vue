<template>
  <div class="root">
    <div class="charts" v-if="viewData">
      <div class="graph" id="graph1">
        <div class="header">
          <h4> Individuals: </h4>  
        </div>  
        <div class="body"> 
          <div class="circle">
            <div class="icon">
              <i class="mdi mdi-account-tie"> </i>
            </div>
            <span class="coun"> {{ count }} </span>
            <span class="tit"> individuals</span>
          </div>
        </div>
      </div>
      <div class="graph" id="graph2">
        <div class="header">
          <h4> Individuals by gender: </h4>
        </div>
        <div class="body">
          <apexchart :options="chartData1.options"  :series="chartData1.series"></apexchart>
        </div>
      </div>
    </div>
    
    <div class="datasection">
      <custom-select-input
        v-model="guid_data_section_id"
        :label="$t('pme.individual.DataSection')"
        :required="true"
        :options="datasections"
      />

      <v-btn
        color="secondary lighten-2"
        class="add"
        @click="() => getData(currentPage)"
        :disabled="disableViewBtn"
      >
        <span class="mdi mdi-search"></span>
        View
      </v-btn>
    </div>
    <div class="buttons">
      <v-btn color="secondary lighten-2" class="add" @click="addAction">
        <span class="mdi mdi-plus"></span>
        Add
      </v-btn>
    </div>

    <div v-if="viewData">
      <data-table-custom
        :columns="columns"
        :tableData="tableData"
        :editRow="editRow"
        :deleteRow="deleteRow"
        :limit="limit"
        :currentPage="currentPage"
        :totalPages="totalPages"
        :count="count"
        :pagination="true"
        headerTitle="Individuals"
        @getData="getData"
        :searchPagination="searchPagination"
        :storedProcedureName="`IndividualList`"
        :PROCEDURE_PARAMETERS="PROCEDURE_PARAMETERS"
        :file_name="file_name"
        :withoutHeader="true"
      />
    </div>
  </div>
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import CustomSelectInput from "../../../components/Inputs/CustomSelectInput.vue";
import DataTable from "../../../components/Inputs/DataTable/DataTable.vue";
import VueApexCharts from "vue-apexcharts";

export default {
  components: {
    "custom-select-input": CustomSelectInput,
    "data-table-custom": DataTable,
    apexchart: VueApexCharts,

  },
  data() {
    return {
      columns: [
        {
          id: 0,
          label: "Guid",
          field: "guid_individual_id",
          hide: true,
          width: 250,
        },
        {
          id: 1,
          label: "pme.individual.JRS_individual_identifier",
          field: "jrS_individual_identifier",
        },
        {
          id: 2,
          label: "pme.individual.firstName",
          field: "individual_first_name",
        },
        {
          id: 3,
          label: "pme.individual.lastName",
          field: "individual_last_name",
        },

        { id: 4, label: "pme.individual.gender", field: "gender" },
        {
          id: 5,
          label: "pme.individual.nationality",
          field: "nationality",
        },
        {
          id: 6,
          label: "pme.individual.individualDocumentNumber",
          field: "document_number",
        },
        { id: 7, label: "pme.individual.dateOfBirth", field: "date_of_birth" },
        {
          id: 8,
          label: "Actions",
          field: "actions",
          actions: true,
        },
        //{
        //  id: 9,
        //  label: "pme.individual.place_of_birth_native_lang",
        //  field: "place_of_birth_native_lang",
        //},
        //{
        //  id: 10,
        //  label: "pme.individual.native_nationality_id",
        //  field: "guid_native_nationality_id",
        //},
        //{ id: 11, label: "pme.individual.is_active", field: "is_active" },
        //{ id: 12, label: "pme.individual.created_date", field: "created_date" },
        //{
        //  id: 13,
        //  label: "pme.individual.actions",
        //  field: "actions",
        //  actions: true,
        //},
      ],
      tableData: [],

      guid_data_section_id: "",
      datasections: [],
      disableViewBtn: true,

      totalPages: 0,
      currentPage: 1,
      limit: 10,
      count: 0,

      PROCEDURE_PARAMETERS: [],
      file_name: "JRS_Individuals_",
      viewData: false,

      chartData1: {
        options: {
          chart: {
            type: "pie",
            width: 380,
          },
          labels: [],
          responsive: [
            {
              breakpoint: 480,
              options: {
                chart: {
                  width: 200,
                },
                legend: {
                  position: "bottom",
                },
              },
            },
          ],
        },
        series: [],
      },
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getConfiguration: "getConfiguration",
      execSPCall: "execSPCall",
    }),
    addAction() {
      this.$router.push({ name: "individualAdd" });
    },
    editRow(row) {
      const params = { id: row.guid_individual_id, info: JSON.stringify(row) }
      this.$router.push({
        name: "individualEdit",
        params,
      });

      localStorage.setItem("C($#)JASC()JCV$%", JSON.stringify(params));
    },
    async deleteRow(row) {
      if (!row) return;
      const body = {
        guid_individual_id: row.guid_individual_id,
      };
      try {
        const response = await this.getConfiguration({
          name: "Individual/Individual_Delete",
          body: body,
          post: true,
        });

        this.showNewSnackbar({
          typeName: "success",
          text: "Delete operation completed successfully",
        });
        this.getData(this.currentPage);
      } catch (ex) {
        this.showNewSnackbar({ typeName: "error", text: ex });
      }
    },
    async getConfFromApi(name, body, post) {
      return new Promise(async (resolve, reject) => {
        try {
          const confData = await this.getConfiguration({ name, body, post });
          resolve(confData);
        } catch (error) {
          console.error("Error fetching configuration data: ", error);
          reject([]);
        }
      });
    },

    convertDateToString(thedate) {
      const date = new Date(thedate);
      const yyyy = date.getFullYear();
      const mm = String(date.getMonth() + 1).padStart(2, "0"); // Months are 0-11, so we add 1 to get 1-12
      const dd = String(date.getDate()).padStart(2, "0");
      const formattedDate = `${yyyy}-${mm}-${dd}`;

      return formattedDate;
    },

    async getStatistics() {
      let localthis = this;
      let thedata = await localthis.getConfFromApi(
        "Individual/GetStatistics",
        {
          office_id: localthis.officeId,
          guid_data_section_id: localthis.guid_data_section_id
        },
        true
      );
      if (thedata) {
        this.chartData1 = {
          options: {
            chart: {
              type: "pie",
              width: 380,
            },
            labels: thedata.individualStatistics_Gender.map(
              (ele) => ele.gender
            ),
            responsive: [
              {
                breakpoint: 480,
                options: {
                  chart: {
                    width: 200,
                  },
                  legend: {
                    position: "bottom",
                  },
                },
              },
            ],
          },
          series: thedata.individualStatistics_Gender.map(
            (ele) => ele.individualsCount
          ),
        };

        

      }
    },

    async getData(page) {
      this.viewData = true;
      let localthis = this;
      let thedata = await localthis.getConfFromApi(
        "Individual/GetListPagination",
        {
          page,
          limit: localthis.limit,
          office_id: localthis.officeId,
          guid_data_section_id: this.guid_data_section_id.toString(),
        },
        true
      );
      this.getStatistics();
      if (thedata && thedata.data) {
        const tblData = thedata.data.map((ele) => ({
          ...ele,
          date_of_birth: this.convertDateToString(ele.date_of_birth),
          created_date: this.convertDateToString(ele.created_date),
        }));
        this.tableData = tblData;
      }
      if (thedata && thedata.totalCount) {
        this.count = thedata.totalCount;
        this.totalPages = thedata.totalPages;
        this.currentPage = thedata.currentPage;
      }
    },

    async searchPagination(search_query) {
      let localthis = this;

      let thedata = await localthis.getConfFromApi(
        "Individual/GetListSearch",
        {
          searchBody: search_query,
          office_id: localthis.officeId,
          guid_data_section_id: this.guid_data_section_id.toString(),
        },
        true
      );
      if (thedata && thedata.data) {
        const tblData = thedata.data.map((ele) => ({
          ...ele,
          created_date: this.convertDateToString(ele.created_date),
        }));
        this.tableData = tblData;
      }
    },
  },

  async mounted() {
    try {
      let datasections = await this.getConfiguration({
        name: "DataSection/Enterable",
        body: {},
        post: false,
      });
      this.datasections = datasections.map((ele) => ({
        id: ele.guid_data_section_id,
        label: ele.data_section,
      }));

      this.PROCEDURE_PARAMETERS = [
        {
          id: 1,
          type: "number",
          key: "HR_OFFICE_ID",
          value: this.officeId,
          hide: true,
        },
        {
          id: 2,
          type: "text",
          key: "guid_data_section_id",
          value: this.guid_data_section_id,
          hide: true,
        },
      ];

      // this.guid_data_section_id = this.datasections[0].guid_data_section_id;
    } catch (error) {
      console.error("Error fetching the main data: ", error);
    }
  },
  watch: {
    guid_data_section_id(newValue) {
      if (newValue != "") {
        this.disableViewBtn = false;
        this.PROCEDURE_PARAMETERS.find(
          (ele) => ele.key == "guid_data_section_id"
        ).value = newValue;
      } else this.disableViewBtn = true;
    },
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
    }),
    officeId() {
      return this.getCurrentOffice.HR_OFFICE_ID;
    },
    HR_OFFICE_LEGAL_NAME() {
      return this.getCurrentOffice.HR_OFFICE_LEGAL_NAME;
    },
  },
};
</script>

<style scoped>
@import "./IndividualHome.scss";
</style>
