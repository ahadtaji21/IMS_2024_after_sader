<template>
  <div id="app" class="root" v-if="tabs && tabs.length > 0">
    <div class="header">
      <h4>
        {{ infoToShow.staff_first_name + " " + infoToShow.staff_last_name }}
      </h4>
      <h4>{{ infoToShow.staff_code }}</h4>
    </div>
    <div class="tabs">
      <button
        v-for="(tab, idx) in tabs"
        :key="idx"
        :class="{ active: currentTab == tab.id }"
        @click="(currentTab = tab.id), tab.id >= 0 && getData(currentPage)"
      >
        {{ tab.name }}
      </button>
    </div>
    <div class="content">
      <my-tab
        :tab="tabs[currentTab]"
        :currentTab="currentTab"
        :backAction="backAction"
        :theData="theData"
        idName="guid_staff_id"
        :idValue="id"
      />
      <div v-if="viewData">
        <data-table-custom
          :columns="getCurrentTabHeaders()"
          :tableData="tableData"
          :editRow="editRow"
          :deleteRow="deleteRow"
          :currentPage="currentPage"
          :totalPages="totalPages"
          :pagination="true"
          :count="count"
          headerTitle=""
          :limit="limit"
          @getData="getData"
          :searchPagination="searchPagination"
          :storedProcedureName="`DonorList`"
          ROCEDURE_PARAMETERS="PROCEDURE_PARAMETERS"
          :file_name="file_name"
          :withoutHeader="true"
          :handleIsActiveClick="handleIsActiveClick"
        />
      </div>
      <edit-popup
        v-if="isEditPopupVisible"
        :edit-title="`Edit ${editTabName}`"
        :confirm-txt="'Save Changes'"
        :cancel-txt="'Cancel'"
        :edit-inputs="editTabInputs"
        :edit-data="editData"
        :edit-func="saveEditData"
        :dialog.sync="isEditPopupVisible"
      />
    </div>
  </div>
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import Tab from "@/components/Inputs/Tabs/Tab.vue";
import DataTable from "@/components/Inputs/DataTable/DataTable.vue";
import EditPopup from "@/components/Inputs/EditPopup.vue";
export default {
  props: ["id", "info"],
  created() {
    const currentTabData = this.tabs[this.currentTab];
    //this.currentTab = 0;
  },
  components: {
    "my-tab": Tab,
    "data-table-custom": DataTable,
    "edit-popup": EditPopup,
  },
  data() {
    return {
      isEditPopupVisible: false,
      editTabName: "",
      editTabInputs: [],
      //editData: {}, // Store data to be edited
      editData: [],
      tabs: [],
      currentTab: 0,
      infoToShow: {},
      theData: [],
      inputs: [],
      headers: [],
      confs: [],
      tableData: [],
      totalPages: 0,
      currentPage: 1,
      limit: 10,
      count: 0,
      columns: [],
      PROCEDURE_PARAMETERS: [],
      file_name: "JRS_Individuals_",
      viewData: false,
      //editData: null,
    };
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
    }),
    userId() {
      return this.getUserUid;
    },
  },
  methods: {
    ...mapActions("apiHandler", {
      updateWizard: "updateWizard",
      getConfiguration: "getConfiguration",
    }),
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),

    resetTabData() {
      const currentTabData = this.tabs[this.currentTab];
      if (currentTabData.id > 0) {
        // Reset all input fields of the current tab
        const tabInputs = this.inputs[this.currentTab];
        const tabData = this.theData[this.currentTab];
        for (let i = 0; i < tabInputs.length; i++) {
          const input = tabInputs[i];
          tabData[input.model] = "";
        }
      }
    },
    getCurrentTabHeaders() {
      const currentTabData = this.tabs[this.currentTab];
      switch (currentTabData.name) {
        case "Contact":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_contact_id",
              field: "guid_staff_contact_id",
              hide: true,
            },
            {
              id: 2,
              label: "guid_contact_type_id",
              field: "guid_contact_type_id",
              hide: true,
            },
            {
              id: 3,
              label: "Contact Type",
              field: "contact_type",
            },
            {
              id: 4,
              label: "Contact Values",
              field: "staff_contact_values",
            },
            {
              id: 5,
              label: "Is Active ",
              field: "contact_is_active",
            },
            {
              id: 6,
              label: "Created Date ",
              field: "created_date",
            },
            {
              id: 6,
              label: "Actions",
              field: "actions",
              actions: true,
              hideEdit: false, // Default to showing the Edit action
            },
          ];
        case "Documents":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_document_id",
              field: "guid_staff_document_id",
              hide: true,
            },
            {
              id: 2,
              label: "guid_document_type_id",
              field: "guid_document_type_id",
              hide: true,
            },
            {
              id: 3,
              label: "Document Type",
              field: "document_type",
            },
            {
              id: 4,
              label: "Document Notes",
              field: "document_notes",
            },
            {
              id: 5,
              label: "Document Number",
              field: "document_number",
            },
            {
              id: 6,
              label: "Date of Issue",
              field: "document_start_date",
            },
            {
              id: 7,
              label: "Expiry Date",
              field: "document_end_date",
            },
            {
              id: 8,
              label: "is Active",
              field: "document_is_active",
            },
            {
              id: 9,
              label: "Created Date",
              field: "created_date",
            },
            {
              id: 10,
              label: "Actions",
              field: "actions",
              actions: true,
            },
          ];
        case "Nationality":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_nationality_id",
              field: "guid_staff_nationality_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_country_id",
              field: "guid_country_id",
              hide: true,
            },
            {
              id: 2,
              label: "Nationality",
              field: "nationality",
            },
            {
              id: 3,
              label: "is Native",
              field: "is_native",
            },
            {
              id: 4,
              label: "is Active",
              field: "nationality_is_active",
            },
            {
              id: 5,
              label: "Created Date ",
              field: "created_date",
            },
            {
              id: 6,
              label: "Actions",
              field: "actions",
              actions: true,
            },
          ];
        case "Address":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_address_id",
              field: "guid_staff_address_id",
              hide: true,
            },
            {
              id: 2,
              label: "guid_country_id",
              field: "guid_country_id",
              hide: true,
            },
            {
              id: 3,
              label: "guid_state_id",
              field: "guid_state_id",
              hide: true,
            },
            {
              id: 4,
              label: "guid_city_id",
              field: "guid_city_id",
              hide: true,
            },
            {
              id: 5,
              label: "guid_neighborhood_id",
              field: "guid_neighborhood_id",
              hide: true,
            },
            {
              id: 6,
              label: "Country Name",
              field: "country_name",
            },
            {
              id: 7,
              label: "State Name",
              field: "state_name",
            },
            {
              id: 8,
              label: "City Name",
              field: "city_name",
            },
            {
              id: 9,
              label: "Neighborhood Name",
              field: "neighborhood_name",
            },
            {
              id: 10,
              label: "Address Details",
              field: "staff_address_details",
            },
            {
              id: 11,
              label: "is Active",
              field: "address_is_active",
            },
            {
              id: 12,
              label: "Created Date ",
              field: "created_date",
            },
            {
              id: 13,
              label: "Actions",
              field: "actions",
              actions: true,
            },
          ];
        case "Staff Movement":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_datasection_id",
              field: "guid_staff_datasection_id",
              hide: true,
            },
            {
              id: 2,
              label: "guid_data_section_id",
              field: "guid_data_section_id",
              hide: true,
            },
            {
              id: 3,
              label: "Staff Movement",
              field: "data_section",
            },
            {
              id: 4,
              label: "Objective Code",
              field: "objective_code",
            },
            {
              id: 5,
              label: "Start Date",
              field: "datasection_start_date",
            },
            {
              id: 6,
              label: "End Date",
              field: "datasection_end_date",
            },
            {
              id: 7,
              label: "is Active",
              field: "datasection_is_active",
            },

            {
              id: 8,
              label: "Created Date ",
              field: "created_date",
            },
            {
              id: 9,
              label: "Actions",
              field: "actions",
              actions: true,
            },
          ];
        case "Educational Level":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_educational_level_id",
              field: "guid_staff_educational_level_id",
              hide: true,
            },
            {
              id: 2,
              label: "guid_educational_level_id",
              field: "guid_educational_level_id",
              hide: true,
            },
            {
              id: 3,
              label: "Educational Level",
              field: "educational_level",
            },
            {
              id: 4,
              label: "is Active",
              field: "education_is_active",
            },

            {
              id: 5,
              label: "Created Date ",
              field: "created_date",
            },
            {
              id: 6,
              label: "Actions",
              field: "actions",
              actions: true,
            },
          ];
        case "Marital Status":
          return [
            {
              id: 0,
              label: "Id",
              field: "guid_staff_id",
              hide: true,
            },
            {
              id: 1,
              label: "guid_staff_marital_status_id",
              field: "guid_staff_marital_status_id",
              hide: true,
            },
            {
              id: 2,
              label: "guid_marital_status_id",
              field: "guid_marital_status_id",
              hide: true,
            },
            {
              id: 3,
              label: "Marital Status",
              field: "marital_status",
            },
            {
              id: 4,
              label: "is Active",
              field: "marital_is_active",
            },
            {
              id: 5,
              label: "Created Date",
              field: "created_date",
            },
            {
              id: 6,
              label: "Actions",
              field: "actions",
              actions: true,
            },
          ];
        case "Personal Information":
          this.viewData = false;
          break;
        default:
          return [];
      }
    },
    viewdatafalse() {
      this.viewData = false;
    },
    async reloaddatatabledata() {
      this.viewData = true;
      this.getData(this.currentPage); // Add this line if you need to refresh data
    },
    // method for isactive click handler
    async handleIsActiveClick(row, fieldName) {
      const currentTabData = this.tabs[this.currentTab];
      const updateApiRoute = `${this.tabs[this.currentTab].submitRoute}`;
      const userId = this.userId; // Get the userId from computed property
      const guid_staff_id = row.guid_staff_id;
      const IsActiveTrue = "1";
      switch (currentTabData.name) {
        // Handle click for isActive field based on the tab and field name
        //switch (this.currentTab) {
        case "Contact":
          if (fieldName === "contact_is_active") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_staff_contact_id: row.guid_staff_contact_id,
                  contact_is_active: !row.contact_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        case "Nationality":
          if (fieldName === "nationality_is_active") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_staff_nationality_id: row.guid_staff_nationality_id,
                  nationality_is_active: !row.nationality_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          } else if (fieldName === "is_native") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_contact_type_id: row.guid_contact_type_id,
                  contact_is_active: !row.contact_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        case "Address":
          if (fieldName === "address_is_active") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_staff_address_id: row.guid_staff_address_id,
                  address_is_active: !row.address_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        case "Staff Movement":
          if (fieldName === "datasection_is_active") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_staff_datasection_id: row.guid_staff_datasection_id,
                  datasection_is_active: !row.datasection_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        case "Documents":
          if (fieldName === "document_is_active") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_staff_document_id: row.guid_staff_document_id,
                  document_is_active: !row.document_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        case "Educational Level":
          if (fieldName === "education_is_active") {
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_staff_educational_level_id:
                    row.guid_staff_educational_level_id,
                  education_is_active: !row.education_is_active,
                  userId,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        case "Marital Status":
          if (fieldName === "marital_is_active") {
            // row[fieldName] = !row[fieldName];
            try {
              const response = await this.getConfiguration({
                name: updateApiRoute,
                body: {
                  guid_staff_id,
                  guid_marital_status_id: row.guid_marital_status_id,
                  marital_is_active: !row.marital_is_active,
                  userId,
                  guid_staff_marital_status_id:
                    row.guid_staff_marital_status_id,
                  IsActiveTrue: IsActiveTrue,
                },
                // body: this.editData,
                post: true,
              });
              this.showNewSnackbar({
                typeName: "success",
                text: "Edit operation completed successfully",
              });
            } catch (ex) {
              this.showNewSnackbar({ typeName: "error", text: ex });
            }
            this.getData(this.currentPage);
          }
          break;
        // Add cases for other tabs as needed
      }
    },
    //end method for is active handler
    // Function to open the edit popup
    editRow(rowData) {
      console.log("Current Tab Index:", this.currentTab);
      console.log("Inputs for Current Tab:", this.inputs[this.currentTab]);
      // Set the data to be edited
      this.editData = { ...rowData };
      this.editTabName = this.tabs[this.currentTab].name;
      // Use the correct index to get inputs for the current tab
      this.editTabInputs = this.inputs[this.currentTab];
      for (const input of this.editTabInputs) {
        const field = input.model;

        if (field === "guid_country_id") {
          //input.value = rowData[field];
          input.onchange = this.handleCountryGuidChangeEditPopup;
          this.handleCountryGuidChangeEditPopup(rowData[field]);
        } else if (field === "guid_state_id") {
          // Attach the @change event for the specific field
          input.onchange = this.handleStateGuidChangeEditPopup;
          this.handleStateGuidChangeEditPopup(rowData[field]);
        } else if (field === "guid_city_id") {
          // input.model = guid_city_id_new;
          input.value = rowData[field];
          // Attach the @change event for the specific field
          input.onchange = this.handleNeighborhoodGuidChangeEditPopup;
          this.handleNeighborhoodGuidChangeEditPopup(rowData[field]);
        } else {
          // For other fields, set the id and value as usual
          input.value = rowData[field];
        }
      }
      this.isEditPopupVisible = true;
    },
    async saveEditData() {
      const updateApiRoute = `${this.tabs[this.currentTab].submitRoute}`;
      const userId = this.userId; // Get the userId from computed property
      try {
        const response = await this.getConfiguration({
          name: updateApiRoute,
          body: { ...this.editData, userId },
          // body: this.editData,
          post: true,
        });
        this.showNewSnackbar({
          typeName: "success",
          text: "Edit operation completed successfully",
        });
      } catch (ex) {
        this.showNewSnackbar({ typeName: "error", text: ex });
      }
      this.getData(this.currentPage);
    },
    //new edit method
    async deleteRow(row) {
      let localthis = this;
      const currentTabData = localthis.tabs[this.currentTab];
      const guid_staff_id = row.guid_staff_id;
      // if (!row) return;
      switch (parseInt(currentTabData.id)) {
        // personal information
        case 1:
          try {
            const body = {
              guid_staff_document_id: row.guid_staff_document_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/Document_Delete",
              body: body,
              post: true,
            });
            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);
          break;
        case 2:
          try {
            const body = {
              guid_staff_nationality_id: row.guid_staff_nationality_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/Nationality_Delete",
              body: body,
              post: true,
            });

            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);
          break;
        case 3:
          try {
            const body = {
              guid_staff_address_id: row.guid_staff_address_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/Address_Delete",
              body: body,
              post: true,
            });

            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);

          break;
        case 4:
          try {
            const body = {
              guid_staff_contact_id: row.guid_staff_contact_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/Contact_Delete",
              body: body,
              post: true,
            });

            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);
          break;
        case 5:
          try {
            const body = {
              guid_staff_datasection_id: row.guid_staff_datasection_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/DataSection_Delete",
              body: body,
              post: true,
            });

            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);
          break;
        case 6:
          try {
            const body = {
              guid_staff_educational_level_id:
                row.guid_staff_educational_level_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/Education_Delete",
              body: body,
              post: true,
            });
            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);
          break;
        case 7:
          try {
            const body = {
              guid_staff_marital_status_id: row.guid_staff_marital_status_id,
              guid_staff_id: row.guid_staff_id,
            };
            const response = await this.getConfiguration({
              name: "Staff/Marital_Delete",
              body: body,
              post: true,
            });
            this.showNewSnackbar({
              typeName: "success",
              text: "Delete operation completed successfully",
            });
          } catch (ex) {
            this.showNewSnackbar({ typeName: "error", text: ex });
          }
          this.getData(this.currentPage);
          break;
        default:
          break;
      }
    },
    async searchPagination(search_query) {
      let localthis = this;
      const currentTabData = localthis.tabs[this.currentTab];
      let thedata;
      switch (parseInt(currentTabData.id)) {
        // Document
        case 1:
          thedata = await localthis.getConfFromApi(
            "Staff/GetDocumentFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        case 2: //nationality
          thedata = await localthis.getConfFromApi(
            "Staff/GetNationalityFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        case 3: //Address
          thedata = await localthis.getConfFromApi(
            "Staff/GetAddressFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        case 4: //Contact
          thedata = await localthis.getConfFromApi(
            "Staff/GetContactFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        case 5: //Staff Movement
          thedata = await localthis.getConfFromApi(
            "Staff/GetDataSectionFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        case 6: //Educational
          thedata = await localthis.getConfFromApi(
            "Staff/GetEducationalFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        case 7: //Marital Status
          thedata = await localthis.getConfFromApi(
            "Staff/GetMaritalFilterSearch",
            {
              searchBody: search_query,
              guid_staff_id: this.id,
            },
            true
          );
          break;
        default:
          break;
      }
      if (thedata && thedata.data) {
        const tblData = thedata.data.map((ele) => ({
          ...ele,
        }));
        this.tableData = tblData;
      }
    },
    async getData(page) {
      debugger
      this.viewData = false;
      let localthis = this;
      let tabData;
      console.log("localthis.tabs: " , localthis.tabs);
      try {
        const currentTabData = localthis.tabs[this.currentTab];
        switch (currentTabData.name) {
          case "Personal Information":
            this.viewData = false;
            break;
          case "Nationality":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetNationalityListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          case "Contact":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetContactListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          case "Marital Status":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetMaritalListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          case "Address":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetAddressListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          case "Staff Movement":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetDataSectionListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          case "Educational Level":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetEducationListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          case "Documents":
            this.viewData = true;
            tabData = await localthis.getConfFromApi(
              "Staff/GetDocumentListPagination",
              {
                page,
                limit: localthis.limit,
                guid_staff_id: this.id.toString(),
              },
              true
            );
            break;
          default:
            this.viewData = false;
            tabData = []; // Set default data
            break;
        }
        // Update datatable grid based on the fetched data
        if (tabData && tabData.data) {
          const tblData = tabData.data.map((ele) => ({
            ...ele,
            created_date: this.convertDateToString(ele.created_date),
          }));
          // Update headers based on the current tab
          this.headers = this.getCurrentTabHeaders();
          this.tableData = tabData.data;
        }
        if (tabData && tabData.totalCount) {
          this.count = tabData.totalCount;
          this.totalPages = tabData.totalPages;
          this.currentPage = tabData.currentPage;
        }
        // Set viewData to true after fetching and processing data
        this.viewData = true;
      } catch (error) {
        console.error("Error fetching or processing data: ", error);
        // Handle errors as needed
      }
    },
    //end datatable implementation
    async getConfFromApi(name, body, post) {
      return new Promise(async (resolve, reject) => {
        try {
          const confData = await this.getConfiguration({ name, body, post });
          resolve(confData);
          //this.reloaddatatabledata;
        } catch (error) {
          console.error("Error fetching configuration data: ", error);
          reject([]);
        }
      });
    },
    convertDateToString(theDate) {
      const date = new Date(theDate);
      const yyyy = date.getFullYear();
      const mm = String(date.getMonth() + 1).padStart(2, "0"); // Months are 0-11, so we add 1 to get 1-12
      const dd = String(date.getDate()).padStart(2, "0");
      const formattedDate = `${yyyy}-${mm}-${dd}`;
      return formattedDate;
    },
    calculateMinimumDate() {
      const currentDate = new Date();
      const minimumDate = new Date(currentDate);
      minimumDate.setFullYear(currentDate.getFullYear() - 18);
      return this.convertDateToString(minimumDate);
    },
    updateStartDateMaxDate(endDate) {
      const maxStartDate = new Date(endDate);
      maxStartDate.setDate(maxStartDate.getDate() - 1); // Set max start date to one day before the end date
      this.$set(this.inputs, 0, {
        ...this.inputs[0],
        max: this.convertDateToString(maxStartDate),
      });
    },
    backAction() {
      this.$router.replace({ name: "staff" });
    },
    resetDropDown() {
      const tabInputs = this.inputs[this.currentTab];
      const tabData = this.theData[this.currentTab];
      const input = tabInputs.find(
        (ele) => ele.model === "guid_neighborhood_id"
      );
      if (input) {
        tabData[input.model] = "";
      }
      const stateInput = tabInputs.find((ele) => ele.model === "guid_state_id");
      if (stateInput) {
        tabData[stateInput.model] = "";
      }
      const cityInput = tabInputs.find((ele) => ele.model === "guid_city_id");
      if (cityInput) {
        tabData[cityInput.model] = "";
      }
    },
    async handleCountryGuidChange(newValue) {
      //debugger;
      this.resetDropDown();
      const theStates = await this.getConfFromApi(
        "State/basedOnCountry",
        newValue.target.value,
        true
      );
      const theStatesOptions = theStates.map((ele) => ({
        id: ele.guid_state_id,
        label: ele.state_name,
      }));
      this.inputs[3].find((ele) => ele.model == "guid_state_id").options =
        theStatesOptions;
      this.inputs[3].find(
        (ele) => ele.model == "guid_state_id"
      ).disabled = false;
    },
    async handleStateGuidChange(newValue) {
      const tabInputs = this.inputs[this.currentTab];
      const tabData = this.theData[this.currentTab];
      const input = tabInputs.find(
        (ele) => ele.model === "guid_neighborhood_id"
      );
      if (input) {
        tabData[input.model] = "";
      }
      const cityInput = tabInputs.find((ele) => ele.model === "guid_city_id");
      if (cityInput) {
        tabData[cityInput.model] = "";
      }
      const thecities = await this.getConfFromApi(
        "City/basedOnState",
        newValue.target.value,
        true
      );
      const cityList = thecities.map((ele) => ({
        id: ele.guid_city_id,
        label: ele.city_name,
      }));
      this.inputs[3].find((ele) => ele.model == "guid_city_id").options =
        cityList;
      this.inputs[3].find(
        (ele) => ele.model == "guid_city_id"
      ).disabled = false;
    },
    async handleNeighborhoodGuidChange(newValue) {
      const tabInputs = this.inputs[this.currentTab];
      const tabData = this.theData[this.currentTab];
      const input = tabInputs.find(
        (ele) => ele.model === "guid_neighborhood_id"
      );
      if (input) {
        tabData[input.model] = "";
      }
      const thecities = await this.getConfFromApi(
        "Neighborhood/basedOnCity",
        newValue.target.value,
        true
      );
      const cityList = thecities.map((ele) => ({
        id: ele.guid_neighborhood_id,
        label: ele.neighborhood_name,
      }));
      this.inputs[3].find(
        (ele) => ele.model == "guid_neighborhood_id"
      ).options = cityList;
      this.inputs[3].find(
        (ele) => ele.model == "guid_neighborhood_id"
      ).disabled = false;
    },
    async handleCountryGuidChangeEditPopup(newValue) {
      const theStates = await this.getConfFromApi(
        "State/basedOnCountry",
        newValue,
        true
      );
      const theStatesOptions = theStates.map((ele) => ({
        id: ele.guid_state_id,
        label: ele.state_name,
      }));
      this.inputs[3].find((ele) => ele.model == "guid_state_id").options =
        theStatesOptions;
      this.inputs[3].find(
        (ele) => ele.model == "guid_state_id"
      ).disabled = false;
    },
    async handleStateGuidChangeEditPopup(newValue) {
      const thecities = await this.getConfFromApi(
        "City/basedOnState",
        newValue,
        // newValue.target.value,
        true
      );
      const cityList = thecities.map((ele) => ({
        id: ele.guid_city_id,
        label: ele.city_name,
      }));
      this.inputs[3].find((ele) => ele.model == "guid_city_id").options =
        cityList;
      this.inputs[3].find(
        (ele) => ele.model == "guid_city_id"
      ).disabled = false;
      console.log("this.editData:", this.editData);
      console.log("this.editData.model:", this.editData.model);
    },
    async handleNeighborhoodGuidChangeEditPopup(newValue) {
      const theNeighborhoods = await this.getConfFromApi(
        "Neighborhood/basedOnCity",
        newValue,
        true
      );
      const cityList = theNeighborhoods.map((ele) => ({
        id: ele.guid_neighborhood_id,
        label: ele.neighborhood_name,
      }));
      this.inputs[3].find(
        (ele) => ele.model == "guid_neighborhood_id"
      ).options = cityList;
      this.inputs[3].find(
        (ele) => ele.model == "guid_neighborhood_id"
      ).disabled = false;
      //this.editData.find((ele) => ele.model == "guid_neighborhood_id").options =
      //    cityList;
      //this.editData.find(
      //    (ele) => ele.model == "guid_neighborhood_id"
      //).disabled = false;
      // Ensure that this.editData is an object
      //this.editData = { ...this.editData };

      console.log("this.editData:", this.editData);
    },
  },
  async beforeMount() {
    let localThis = this;
    this.infoToShow = JSON.parse(this.info);
    const confs = [
      {
        id: 1,
        routeName: "IndividualDocumentType",
        idName: "guid_document_type_id",
        labelName: "document_type",
        extraName: "guid_country_id",
      },
      {
        id: 2,
        routeName: "Gender",
        idName: "gender_id",
        labelName: "gender",
      },
      {
        id: 200,
        routeName: "Language",
        idName: "guid_language_id",
        labelName: "language_name",
      },
      {
        id: 3,
        routeName: "MaritalStatus",
        idName: "guid_marital_status_id",
        labelName: "marital_status",
      },
      {
        id: 4,
        routeName: "EducationalLevel",
        idName: "guid_educational_level_id",
        labelName: "educational_level",
      },
      {
        id: 8,
        routeName: "Country",
        idName: "guid_country_id",
        labelName: "country_name",
        extraName: "phone_code",
      },
      {
        id: 9,
        routeName: "Country/getNationalities",
        idName: "guid_country_id",
        labelName: "nationality",
        extraName: "country_code",
      },
      {
        id: 13,
        routeName: "DataSection/Enterable",
        idName: "guid_data_section_id",
        labelName: "data_section",
      },
      {
        id: 14,
        routeName: "BloodType",
        idName: "blood_type_id",
        labelName: "blood_type",
      },
      {
        id: 14,
        routeName: "ContactType",
        idName: "guid_contact_type_id",
        labelName: "contact_type",
      },
    ];
    this.confs = confs;
    if (!this.id) {
      this.$router.replace({ name: "NotFound" });
    }
    const newConfs = JSON.parse(JSON.stringify(localThis.confs));
    for (let i = 0; i < newConfs.length; i++) {
      const conf = newConfs[i];
      let requ = await localThis.getConfFromApi(conf.routeName);
      const ar = [];
      for (let j = 0; j < requ.length; j++) {
        let dd = {};
        dd.id = requ[j][conf.idName];
        dd.label = requ[j][conf.labelName];
        if (conf.extraName) {
          dd[conf.extraName] = requ[j][conf.extraName];
        }
        ar.push(dd);
      }
      conf.data = ar;
    }
    localThis.confs = newConfs;
    const inputs = [
      //persnoal information
      [
        {
          id: 0,
          label: "guid_staff_country_id",
          field: "guid_staff_country_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_id",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 13,
          model: "staff_code",
          type: "text",
          label: "HRM.STAFF.staff_code",
          // required: false,
          disabled: true,
          //readonly: ,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 2,
          model: "staff_first_name",
          type: "text",
          label: "HRM.STAFF.staff_first_name",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 3,
          model: "staff_first_name_native",
          type: "text",
          label: "HRM.STAFF.staff_first_name_native",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 4,
          model: "staff_last_name",
          type: "text",
          label: "HRM.STAFF.staff_last_name",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 5,
          model: "staff_last_name_native",
          type: "text",
          label: "HRM.STAFF.staff_last_name_native",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 6,
          model: "staff_father_name",
          type: "text",
          label: "HRM.STAFF.staff_father_name",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 7,
          model: "staff_mother_name",
          type: "text",
          label: "HRM.STAFF.staff_mother_name",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 8,
          model: "staff_gender_id",
          type: "select",
          label: "HRM.STAFF.staff_gender_id",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Gender")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 9,
          model: "staff_date_of_birth",
          type: "date",
          label: "HRM.STAFF.staff_date_of_birth",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          //min: this.convertDateToString(new Date()),
          max: this.calculateMinimumDate(),
        },
        {
          id: 10,
          model: "staff_place_of_birth",
          type: "text",
          label: "HRM.STAFF.staff_place_of_birth",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 11,
          model: "guid_native_language_id",
          type: "select",
          label: "HRM.STAFF.guid_native_language_id",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Language")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 12,
          model: "jrs_entry_date",
          type: "date",
          label: "HRM.STAFF.jrs_entry_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },

        {
          id: 14,
          model: "blood_type_id",
          label: "HRM.STAFF.blood_type_id",
          type: "select",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "BloodType")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 15,
          model: "is_religious",
          //type: "text",
          label: "is Religious?",
          type: "toggle-switch",
          required: false,
          options: [
            { id: false, label: "No" },
            { id: true, label: "Yes" },
          ],
        },
        {
          id: 16,
          model: "document",
          type: "image",
          label: "Profile Picture",
          required: false,
          widthPercentage: 60,
          marginRightPercentage: 10,
        },
      ],
      //staff document
      [
        {
          id: 0,
          label: "guid_staff_id",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_document_id",
          field: "guid_staff_document_id",
          hide: true,
        },
        {
          id: 2,
          model: "guid_document_type_id",
          type: "select",
          label: "HRM.STAFF.guid_document_type_id",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "IndividualDocumentType"
          ).data,
          values: localThis.confs
            .find((ele) => ele.routeName == "IndividualDocumentType")
            .data.map((ele) => ""),
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 4,
          model: "document_start_date",
          type: "date",
          label: "HRM.STAFF.document_start_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
        {
          id: 5,
          model: "document_end_date",
          type: "date",
          label: "HRM.STAFF.document_end_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
        {
          id: 6,
          model: "document_number",
          type: "text",
          label: "HRM.STAFF.document_number",
          // required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 7,
          model: "document_notes",
          type: "text",
          label: "HRM.STAFF.document_notes",
          // required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 70,
          model: "document",
          type: "file",
          label: "HRM.STAFF.document file",
          // required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      //Nationality
      [
        {
          id: 0,
          label: "guid_staff_id",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_nationality_id",
          field: "guid_staff_nationality_id",
          hide: true,
        },
        {
          id: 2,
          model: "guid_country_id",
          label: "Nationality",
          type: "select",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "Country/getNationalities"
          ).data,
          widthPercentage: 40,
          marginRightPercentage: 5,
        },
        {
          id: 3,
          model: "is_native",
          label: "Is Native",
          /* type: "radio",*/
          type: "toggle-switch",
          required: false,
          options: [
            { id: false, label: "No" },
            { id: true, label: "Yes" },
          ],
          widthPercentage: 20,
          marginRightPercentage: 5,
        },
        {
          id: 4,
          model: "nationality_is_active",
          label: "Active Status",
          type: "toggle-switch",
          required: false,
          options: [
            { id: false, label: "No" },
            { id: true, label: "Yes" },
          ],
          widthPercentage: 20,
          marginRightPercentage: 5,
        },
      ],
      //address information
      [
        {
          id: 0,
          label: "guid_staff_id",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_address_id",
          field: "guid_staff_address_id",
          hide: true,
        },

        {
          id: 20,
          model: "guid_country_id",
          step: 4,
          label: "HRM.STAFF.guid_country_id",
          type: "select",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Country")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 5,
          onChange: this.handleCountryGuidChange,
        },
        {
          id: 21,
          model: "guid_state_id",
          step: 4,
          label: "HRM.STAFF.guid_state_id",
          type: "select",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          disabled: true,
          onChange: this.handleStateGuidChange,
        },
        {
          id: 22,
          model: "guid_city_id",
          step: 4,
          label: "HRM.STAFF.guid_city_id",
          type: "select",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          disabled: true,
          onChange: this.handleNeighborhoodGuidChange,
        },
        {
          id: 23,
          model: "guid_neighborhood_id",
          step: 4,
          label: "HRM.STAFF.guid_neighborhood_id",
          type: "select",
          //required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          disabled: true,
        },

        {
          id: 6,
          model: "staff_address_details",
          label: "HRM.STAFF.staff_address_details",
          //field: "staff_address_details",
          type: "text",
          required: true,
          widthPercentage: 85,
          marginRightPercentage: 15,
        },
      ],
      //contact information
      [
        {
          id: 0,
          label: "guid_staff_id",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 90,
          label: "guid_staff_contact_id",
          field: "guid_staff_contact_id",
          hide: true,
        },
        {
          id: 1,
          model: "guid_contact_type_id",
          label: "HRM.STAFF.guid_contact_type_id",
          //field: "guid_contact_type_id",
          type: "select",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "ContactType")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 3,
          model: "staff_contact_values",
          label: "HRM.STAFF.staff_contact_values",
          //field: "staff_contact_values",
          type: "text",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      //DataSection information
      [
        {
          id: 0,
          label: "guid_staff_id",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_datasection_id",
          field: "guid_staff_datasection_id",
          hide: true,
        },
        {
          id: 2,
          model: "guid_data_section_id",
          type: "select",
          label: "HRM.STAFF.guid_data_section_id",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "DataSection/Enterable"
          ).data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 3,
          model: "datasection_start_date",
          type: "date",
          label: "HRM.STAFF.datasection_start_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
        {
          id: 4,
          model: "datasection_end_date",
          type: "date",
          label: "HRM.STAFF.datasection_end_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
      ],

      //educational level
      [
        {
          id: 0,
          label: "Guid",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_educational_level_id",
          field: "guid_staff_educational_level_id",
          hide: true,
        },
        {
          id: 2,
          model: "guid_educational_level_id",
          type: "select",
          label: "HRM.STAFF.guid_educational_level_id",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "EducationalLevel"
          ).data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      //Marital status
      [
        {
          id: 1,
          label: "Guid",
          field: "guid_staff_id",
          hide: true,
        },
        {
          id: 1,
          label: "guid_staff_marital_status_id",
          field: "guid_staff_marital_status_id",
          hide: true,
        },
        {
          id: 13,
          model: "guid_marital_status_id",
          type: "select",
          label: "HRM.STAFF.guid_marital_status_id",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "MaritalStatus"
          ).data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
    ];
    const columns = [[], [], [], [], []];
    const theData = [];
    for (let t = 0; t < inputs.length; t++) {
      const tabInp = inputs[t];
      const theDatTab = {};
      for (let k = 0; k < tabInp.length; k++) {
        theDatTab[tabInp[k].model] = "";
      }
      theData.push(theDatTab);
    }
    localThis.inputs = inputs;
    localThis.columns = columns;
    localThis.theData = theData;
    debugger

    localThis.tabs = [
      {
        id: 0,
        name: "Personal Information",
        theData: theData[0],
        inputs: inputs[0],
        grid: false,
        routeGet: "Staff/GetStaffListPagination",
        bodyGet: { guid_staff_id: this.id },
        submitRoute: "Staff/Staff_Update",
        saveDisabled: true,
        click: this.viewdatafalse(),
        viewData: false,
      },
      {
        id: 1,
        name: "Documents",
        theData: theData[1],
        inputs: inputs[1],
        grid: false,
        submitRoute: "Staff/StaffDocument_insert",
        saveDisabled: true,
        click: this.reloadData, // Call reloadData method when the tab is clicked
      },
      {
        id: 2,
        name: "Nationality",
        theData: theData[2],
        inputs: inputs[2],
        grid: false,
        submitRoute: "Staff/StaffNationality_insert",
        saveDisabled: true,
        click: this.reloadData, // Call reloadData method when the tab is clicked
      },
      {
        id: 3,
        name: "Address",
        theData: theData[3],
        inputs: inputs[3],
        grid: false,
        submitRoute: "Staff/StaffAddress_insert",
        saveDisabled: true,
        click: this.reloadData, // Call reloadData method when the tab is clicked
      },
      {
        id: 4,
        name: "Contact",
        theData: theData[4],
        inputs: inputs[4],
        grid: false,
        submitRoute: "Staff/StaffContact_insert",
        saveDisabled: true,
        click: this.reloadData, // Call reloadData method when the tab is clicked
      },
      {
        id: 5,
        name: "Staff Movement",
        theData: theData[5],
        inputs: inputs[5],
        grid: false,
        submitRoute: "Staff/StaffDataSection_insert",
        saveDisabled: true,
        click: this.reloaddatatabledata(), // Call reloadData method when the tab is clicked
      },
      {
        id: 6,
        name: "Educational Level",
        theData: theData[6],
        inputs: inputs[6],
        grid: false,
        submitRoute: "Staff/StaffEducationalLevel_insert",
        saveDisabled: true,
        click: this.reloadData, // Call reloadData method when the tab is clicked
      },
      {
        id: 7,
        name: "Marital Status",
        theData: theData[7],
        inputs: inputs[7],
        grid: false,
        submitRoute: "Staff/StaffMaritalStatus_insert",
        saveDisabled: true,
        click: this.reloadData, // Call reloadData method when the tab is clicked
      },
    ];

  },
  watch: {
    "theData.document_end_date": function (newEndDate) {
      this.updateStartDateMaxDate(newEndDate);
    },
    theData: {
      deep: true,
      handler(newValue, oldValue) {
        let localDisabled = false;
        const tabInputs = this.inputs[this.currentTab];
        for (let i = 0; i < tabInputs.length; i++) {
          const inp = tabInputs[i];
          const newVal = newValue[this.currentTab][inp.model];
          if (inp.required && (!newVal || newVal == "")) {
            localDisabled = true;
            break;
          }
        }
        this.tabs[this.currentTab].saveDisabled = localDisabled;
        console.log("newvalue: ", newValue);
      },
    },
  },
};
</script>
<style scoped>
@import "./Edit.scss";

.clickable {
  cursor: pointer;
}
</style>
