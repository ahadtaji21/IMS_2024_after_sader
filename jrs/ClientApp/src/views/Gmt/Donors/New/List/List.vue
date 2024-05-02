<template>
  <div class="root">
    <div class="charts">
      <div class="graph" id="graph1">
        <div class="header">
          <h4>Donors:</h4>
        </div>
        <div class="body">
          <div class="circle">
            <div>
              <i class="mdi mdi-account-tie"> </i>
            </div>
            <span> {{ count }} </span>
            <span class="don"> DONORS</span>
          </div>
        </div>
      </div>

      <div class="graph" id="graph2">
        <div class="header">
          <h4>Donors by category:</h4>
        </div>
        <div class="body">
          <apexchart
            :options="chartData1.options"
            :series="chartData1.series"
          ></apexchart>
        </div>
      </div>

      <div class="graph" id="graph3">
        <div class="header">
          <h4>Donated countries:</h4>
        </div>
        <div class="body">
          <apexchart
            :options="chartData2.options"
            :series="chartData2.series"
          ></apexchart>
        </div>
      </div>
    </div>
    
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



      headerTitle="Donors"
      @getData="getData"
      :searchPagination="searchPagination"


      :storedProcedureName="`DonorList`"
      :PROCEDURE_PARAMETERS="PROCEDURE_PARAMETERS"
      :file_name="file_name"
      :withoutHeader="true"

    />
    <div class="buttons">
      <v-btn color="secondary lighten-2" class="add" @click="addAction">
        <span class="mdi mdi-plus"></span>
        Add
      </v-btn>
    </div>
  </div>
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import VueApexCharts from "vue-apexcharts";

import DataTable from "../../../../../components/Inputs/DataTable/DataTable.vue";

export default {
  components: {
    "data-table-custom": DataTable,
    apexchart: VueApexCharts,
  },
  data() {
    return {
      columns: [
        {
          id: 1,
          label: "GMT.DONORS.guid_donor_id",
          field: "guid_donor_id",
          hide: true,
        },
        {
          id: 2,
          label: "GMT.DONORS.donor_name",
          field: "donor_name",
        },
        {
          id: 3,
          label: "GMT.DONORS.donor_code",
          field: "donor_code",
        },
        {
          id: 4,
          label: "GMT.DONORS.donor_category",
          field: "donor_category",
        },
        {
          id: 5,
          label: "Actions",
          field: "actions",
          actions: true,
        },
      ],
      tableData: [],

      guid_data_section_id: "",
      datasections: [],
      disableViewBtn: true,

      totalPages: 0,
      currentPage: 1,
      limit: 10,
      count: 0,

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
      chartData2: {
        options: {
          chart: {
            type: "bar",
            height: 200,
          },
          plotOptions: {
            bar: {
              borderRadius: 1,
              horizontal: true,
            },
          },
          dataLabels: {
            enabled: false,
          },
          xaxis: {
            categories: [],
          },
        },
        series: [],
      },

      PROCEDURE_PARAMETERS: [],
      file_name: "JRS_Donors_",
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
      this.$router.push({ name: "donnorAdd" });
    },
    editRow(row) {
      this.$router.push({
        name: "donnorEdit",
        params: { id: row.guid_donor_id, info: JSON.stringify(row) },
      });
    },
    async deleteRow(row) {
      if (!row) return;
      const body = {
        guid_donor_id: row.guid_donor_id,
      };
      try {
        const response = await this.getConfiguration({
          name: "Donor/Donor_Delete",
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
        "Donor/GetStatistics",
        {
          office_id: localthis.officeId,
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
            labels: thedata.donorStatistics_Category.map(
              (ele) => ele.donor_category
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
          series: thedata.donorStatistics_Category.map(
            (ele) => ele.donnorsCount
          ),
        };

        this.chartData2 = {
          options: {
            chart: {
              type: "bar",
              height: 200,
            },
            plotOptions: {
              bar: {
                borderRadius: 1,
                horizontal: true,
              },
            },
            dataLabels: {
              enabled: false,
            },
            xaxis: {
              categories: thedata.donorStatistics_Country.map(
                (ele) => ele.country_name
              ),
            },
          },
          series: [
            {
              data: thedata.donorStatistics_Country.map(
                (ele) => ele.donnorsCount
              ),
            },
          ],
        };
     }
    },
    async getData(page) {
      let localthis = this;
      let thedata = await localthis.getConfFromApi(
        "Donor/GetListPagination",
        {
          page,
          limit: localthis.limit,
          office_id: localthis.officeId,
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
      if (thedata && thedata.totalCount) {
        this.count = thedata.totalCount;
        this.totalPages = thedata.totalPages;
        this.currentPage = thedata.currentPage;
      }
    },
    async searchPagination(search_query) {
      let localthis = this;

      let thedata = await localthis.getConfFromApi(
        "Donor/GetListSearch",
        {
          searchBody: search_query,
          office_id: localthis.officeId,
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
    this.getData(this.currentPage);

    this.PROCEDURE_PARAMETERS = [
      {
        id: 1,
        type: "number",
        key: "HR_OFFICE_ID",
        value: this.officeId,
        hide: true,
      },
    ];
  },
  async beforeMount() {
    this.getStatistics();
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
@import "./List.scss";
</style>
