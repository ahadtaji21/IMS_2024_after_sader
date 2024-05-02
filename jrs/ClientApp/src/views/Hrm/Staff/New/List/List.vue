<template>
    <div class="root">
        <div class="charts" v-if="viewData">
            <div class="graph" id="graph1">
                <div class="header">
                    <h4>Staff:</h4>
                </div>
                <div class="body">
                    <div class="circle">
                        <div>
                            <i class="mdi mdi-account-tie"> </i>
                        </div>
                        <span> {{ count }} </span>
                        <span class="don"> Staff</span>
                    </div>
                </div>
            </div>

            <div class="graph" id="graph2">
                <div class="header">
                    <h4>Staff by gender:</h4>
                </div>
                <div class="body">
                    <apexchart :options="chartData1.options"
                               :series="chartData1.series"></apexchart>
                </div>
            </div>

            <div class="graph" id="graph3">
                <div class="header">
                    <h4>Staff countries:</h4>
                </div>
                <div class="body">
                    <apexchart :options="chartData2.options"
                               :series="chartData2.series"></apexchart>
                </div>
            </div>
        </div>
        <div class="datasection">
            <custom-select-input v-model="guid_data_section_id"
                                 :label="$t('HRM.STAFF.guid_data_section_id')"
                                 :required="true"
                                 :options="datasections" />

            <v-btn color="secondary lighten-2"
                   class="add"
                   @click="() => getData(currentPage)"
                   :disabled="disableViewBtn">
                <span class="mdi mdi-search"></span>
                View
            </v-btn>
        </div>
        <div v-if="viewData">
            <data-table-custom :columns="columns"
                               :tableData="tableData"
                               :editRow="editRow"
                               :deleteRow="deleteRow"
                               :currentPage="currentPage"
                               :totalPages="totalPages"
                               :pagination="true"
                               :count="count"
                               headerTitle="Staff"
                               :limit="limit"
                               @getData="getData"
                               :searchPagination="searchPagination"
                               :storedProcedureName="`DonorList`"
                               ROCEDURE_PARAMETERS="PROCEDURE_PARAMETERS"
                               :file_name="file_name"
                               :withoutHeader="true" />
            <div class="buttons">
                <v-btn color="secondary lighten-2" class="add" @click="addAction">
                    <span class="mdi mdi-plus"></span>
                    Add
                </v-btn>
            </div>
        </div>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from "vuex";

    import CustomSelectInput from "@/components/Inputs/CustomSelectInput.vue";

    import DataTable from "@/components/Inputs/DataTable/DataTable.vue";

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
                        label: "Id",
                        field: "guid_staff_id",
                        hide: true,
                    },
                     {
                         id:1,
                         label: "Code",
                         field: "staff_code",
                     },
                    {
                        id: 2,
                        label: "First Name",
                        field: "staff_first_name",
                    },
                    {
                        id: 3,
                        label: "Last Name",
                        field: "staff_last_name",
                    },
                    {
                        id: 4,
                        label: "Gender",
                        field: "gender",
                    },
                    {
                        id: 5,
                        label: "Birthday Date",
                        field: "staff_date_of_birth",
                    },
                    {
                        id: 6,
                        label: "Age",
                        field: "age",
                    },
                    {
                        id: 7,
                        label: "Entry Date",
                        field: "jrs_entry_date",
                    },

                    
                    {
                        id: 8,
                        label: "Project",
                        field: "objective_code",
                    },
                    {
                        id: 9,
                        label: "Created",
                        field: "created_date",
                    },
                    {
                        id: 10,
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

                //PROCEDURE_PARAMETERS: [],
                //file_name: "JRS_Donors_",
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
                this.$router.push({ name: "staffAdd" });
            },
            editRow(row) {
                this.$router.push({
                    name: "staffEdit",
                    params: { id: row.guid_staff_id, info: JSON.stringify(row) },
                });
            },
            async deleteRow(row) {
                if (!row) return;
                const body = {
                    guid_staff_id: row.guid_staff_id,
                };
                try {
                    const response = await this.getConfiguration({
                        name: "Staff/Donor_Delete",
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
                    "Staff/GetStatistics",
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
                            labels: thedata.genderStatistics.map(
                                (ele) => ele.gender_category
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
                        series: thedata.genderStatistics.map(
                            (ele) => ele.genderCount
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
                                categories: thedata.staffStatistics_Country.map(
                                    (ele) => ele.staffcountry_name
                                ),
                            },
                        },
                        series: [
                            {
                                data: thedata.staffStatistics_Country.map(
                                    (ele) => ele.staffCount
                                ),
                            },
                        ],
                    };

                    console.log("this.chart1 lab: ", this.chartData1.options.labels);
                    console.log("this.chart1 ser: ", this.chartData1.series);

                    console.log(
                        "this.chartData2 categories: ",
                        this.chartData2.options.xaxis.categories
                    );
                    console.log("this.chartData2 ser: ", this.chartData2.series);
                }
            },
            async getData(page) {
                this.viewData = true;
                let localthis = this;
                let thedata = await localthis.getConfFromApi(
                    "Staff/GetListPagination",
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
                    "Staff/GetListSearch",
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
            /*
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
        },*/
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
    @import "./List.scss";
</style>
