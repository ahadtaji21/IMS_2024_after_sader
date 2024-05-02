<template>
  <div id="app" class="root" v-if="tabs && tabs.length > 0">
    <div class="header">
      <h4>
        {{
          infoToShow.individual_first_name + " " + infoToShow.individual_last_name
        }}
      </h4>
    </div>

    <div class="tabs">
      <button
        v-for="(tab, idx) in tabs"
        :key="idx"
        @click="currentTab = tab.id"
        :class="{ active: currentTab == tab.id }"
      >
        {{ tab.name }}
      </button>
    </div>
    <div class="content">
      <my-tab
        :tab="tabs[currentTab]"
        :currentTab="currentTab"
        :backAction="backAction"
        idName="guid_individual_id"
        :idValue="id"
      />
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import Tab from "../../../components/Inputs/Tabs/Tab.vue";

export default {
  props: ["id", "info"],

  created() {
    this.currentTab = 0;
  },
  components: {
    "my-tab": Tab,
  },
  data() {
    return {
      tabs: [],
      currentTab: 0,
      infoToShow: {},
      idToShow: "",
      theData: [],
      inputs: [],
      columns: [],
      confs: [],
      hidePregnancy: false,

    };
  },

  methods: {
    ...mapActions("apiHandler", {
      updateWizard: "updateWizard",
      getConfiguration: "getConfiguration",
    }),

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

    convertDateToString(theDate) {
      const date = new Date(theDate);
      const yyyy = date.getFullYear();
      const mm = String(date.getMonth() + 1).padStart(2, "0"); // Months are 0-11, so we add 1 to get 1-12
      const dd = String(date.getDate()).padStart(2, "0");
      const formattedDate = `${yyyy}-${mm}-${dd}`;

      return formattedDate;
    },
    backAction() {
      this.$router.replace({ name: "individual" });
    },

    async getStatesByCountry() {
      const newValue = this.theData[3].guid_country_id;

      const theStates = await this.getConfFromApi(
        "State/basedOnCountry",
        newValue,
        true
      );

      this.stateList = theStates.map((ele) => ({
        id: ele.guid_state_id,
        label: ele.state_name,
      }));

      this.inputs[3].find((ele) => ele.model == "guid_state_id").options =
        this.stateList;
    },
    async getCitiesByState() {
      const newValue = this.theData[3].guid_state_id;
      const theCities = await this.getConfFromApi(
        "City/basedOnState",
        newValue,
        true
      );

      this.cityList = theCities.map((ele) => ({
        id: ele.guid_city_id,
        label: ele.city_name,
      }));

      this.inputs[3].find((ele) => ele.model == "guid_city_id").options =
        this.cityList;
    },
    async getNeighborhoodsByCity() {
      const newValue = this.theData[3].guid_city_id;
      const theNeighborhoods = await this.getConfFromApi(
        "Neighborhood/basedOnCity",
        newValue,
        true
      );

      this.neighborhoodList = theNeighborhoods.map((ele) => ({
        id: ele.guid_neighborhood_id,
        label: ele.neighborhood_name,
      }));

      this.inputs[3].find(
        (ele) => ele.model == "guid_neighborhood_id"
      ).options = this.neighborhoodList;
    },

    // async updateSubmit() {
    //   const route = this.tabs[this.currentTab].route;
    //   const body = this.theData[this.tabs[this.currentTab].name];

    //   return new Promise(async (res, rej) => {
    //     try {
    //       await this.updateWizard({ route, body });
    //       res(body);
    //     } catch (ex) {
    //       console.log("error in wizard submit");
    //       rej(ex);
    //     }
    //   });
    // },
  },
  async beforeMount() {
    let localThis = this;

    if (!this.info && !this.id) {
      // localStorage.setItem("C($#)JASC()JCV$%", JSON.stringify(params));
      const params = localStorage.getItem("C($#)JASC()JCV$%");
      console.log("params: " , params);
      if (!params) {
        this.$router.replace({ name: "NotFound" });
      } else {
        const parsedParams = JSON.parse(params);
        this.idToShow = parsedParams.id;
        this.infoToShow = parsedParams.info;
      }
    }else{
      this.infoToShow = JSON.parse(this.info);
      this.idToShow = this.id;
    }


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
        id: 5,
        routeName: "SpecialNeed",
        idName: "guid_special_need_type_id",
        labelName: "special_need_type",
      },
      {
        id: 6,
        routeName: "HouseholdStatusType",
        idName: "guid_household_status_type_id",
        labelName: "household_status_type",
      },
      {
        id: 7,
        routeName: "HouseholdDocumentType",
        idName: "guid_document_type_id",
        labelName: "document_type",
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
        id: 10,
        routeName: "RelationshipsType",
        idName: "relationships_type_id",
        labelName: "relationships_type",
      },
      {
        id: 11,
        routeName: "AddressStatusType",
        idName: "guid_address_status_type_id",
        labelName: "address_status_type",
      },
      {
        id: 12,
        routeName: "AddressPropertyType",
        idName: "guid_address_property_type_id",
        labelName: "address_property_type",
      },
      {
        id: 13,
        routeName: "DataSection/Enterable",
        idName: "guid_data_section_id",
        labelName: "data_section",
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

    const testConfs = await localThis.getConfFromApi(
      "Individual/IndividualUpdate_GetConfigurations"
    );
    console.log("testConfs", testConfs);

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
      [
        {
          id: 1,
          model: "guid_data_section_id",
          type: "select",
          label: "pme.individual.DataSection",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "DataSection/Enterable"
          ).data,
          widthPercentage: 90,
          marginRightPercentage: 10,
        },
        {
          id: 2,
          model: "individual_first_name",
          type: "text",
          label: "pme.individual.firstName",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 3,
          model: "individual_first_name_native_lang",
          type: "text",
          label: "pme.individual.individual_first_name_native_lang",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 4,
          model: "individual_last_name",
          type: "text",
          label: "pme.individual.lastName",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 5,
          model: "individual_last_name_native_lang",
          type: "text",
          label: "pme.individual.individual_last_name_native_lang",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 6,
          model: "individual_father_name",
          type: "text",
          label: "pme.individual.fatherName",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },

        {
          id: 7,
          model: "individual_mother_name",
          type: "text",
          label: "pme.individual.motherName",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },

        {
          id: 8,
          model: "gender_id",
          type: "select",
          label: "pme.individual.gender",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Gender")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },

        {
          id: 9,
          model: "date_of_birth",
          type: "date",
          label: "pme.individual.dateOfBirth",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
        {
          id: 10,
          model: "place_of_birth",
          type: "text",
          label: "pme.individual.place_of_birth",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 11,
          model: "place_of_birth_native_lang",
          type: "text",
          label: "pme.individual.place_of_birth_native_lang",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },

        {
          id: 13,
          model: "guid_native_language_id",
          type: "select",
          label: "pme.individual.native_language",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Language")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      //1 documents
      [
        {
          id: 16,
          model: "individualDocumentType",
          hint: "At least one",
          label: "pme.individual.individualDocumentType",
          type: "select-checkbox-withInput",
          options: localThis.confs.find(
            (ele) => ele.routeName == "IndividualDocumentType"
          ).data,
          values: localThis.confs
            .find((ele) => ele.routeName == "IndividualDocumentType")
            .data.map((ele) => ""),
          widthPercentage: 100,
          marginRightPercentage: 0,
        },

        {
          id: 17,
          model: "householdDocumentType",
          hint: "At least one",
          label: "pme.individual.householdDocumentType",
          type: "select-checkbox-withInput",
          options: localThis.confs.find(
            (ele) => ele.routeName == "HouseholdDocumentType"
          ).data,
          values: localThis.confs
            .find((ele) => ele.routeName == "HouseholdDocumentType")
            .data.map((ele) => ""),
          widthPercentage: 100,
          marginRightPercentage: 0,
        },
      ],
      //2 Education
      [
        {
          id: 18,
          model: "guid_educational_level_id",
          label: "pme.individual.educationLevel",
          type: "select",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "EducationalLevel"
          ).data,
          widthPercentage: 100,
          marginRightPercentage: 0,
        },
        {
          id: 19,
          model: "education_level_description",
          type: "text",
          label: "pme.individual.educationLvelTxt",
          required: true,
          widthPercentage: 100,
          marginRightPercentage: 0,
        },
      ],
      //3 household address
      [
        {
          id: 1,
          model: "guid_country_id",
          label: "pme.individual.country",
          type: "select",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Country")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 5,
          onChange: localThis.getStatesByCountry,
        },
        {
          id: 2,
          model: "guid_state_id",
          label: "pme.individual.state",
          type: "select",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          onChange: localThis.getCitiesByState,
        },
        {
          id: 3,
          model: "guid_city_id",
          label: "pme.individual.city",
          type: "select",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          onChange: localThis.getNeighborhoodsByCity,
        },
        {
          id: 4,
          model: "guid_neighborhood_id",
          label: "pme.individual.neighborhood",
          type: "select",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
        },

        {
          id: 5,
          model: "rent_value",
          type: "number",
          label: "pme.individual.rentValue",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 5,
        },
        {
          id: 6,
          model: "household_address",
          type: "text",
          label: "pme.individual.household_address",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 5,
        },

        {
          id: 7,
          model: "guid_address_property_type_id",
          options: localThis.confs.find(
            (ele) => ele.routeName == "AddressPropertyType"
          ).data,
          label: "pme.individual.propertyType",
          type: "select",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 5,
        },
        {
          id: 8,
          model: "guid_address_status_type_id",

          label: "pme.individual.livingStatus",
          type: "select",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "AddressStatusType"
          ).data,
          widthPercentage: 40,
          marginRightPercentage: 5,
        },

        {
          id: 9,
          model: "is_native_address",
          type: "radio",
          label: "pme.individual.is_native_address",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: [
            { id: "true", label: "Yes" },
            { id: "false", label: "No", default: true },
          ],
        },
      ],
      //4 special need
      [
        {
          id: 36,
          model: "guid_special_need_type_ids",
          hint: "Not required",
          label: "pme.individual.specialNeeds",
          type: "checkbox",
          required: false,

          options: localThis.confs.find((ele) => ele.routeName == "SpecialNeed")
            .data,
          widthPercentage: 100,
          marginRightPercentage: 0,
        },
      ],
      //5 pregnancy
      [
        {
          id: 39,
          model: "pregnancy_start_date",
          label: "pme.individual.pregnancy_start_date",
          type: "date",
          required: true,
          widthPercentage: 100,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
      ],
      //6 contact
      [
        {
          id: 1,
          model: "guid_contact_type_id",
          type: "select",
          label: "pme.individual.contact_type",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "ContactType")
            .data,
          widthPercentage: 70,
          marginRightPercentage: 10,
        },
        {
          id: 2,
          model: "contact_value",
          type: "text",
          label: "pme.individual.contact_value",
          required: true,
          widthPercentage: 70,
          marginRightPercentage: 10,
        },
      ],
      //7 nationality
      [
        {
          id: 1,
          model: "guid_country_id",
          type: "select",
          label: "pme.individual.native_nationality_id",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "Country/getNationalities"
          ).data,
          widthPercentage: 100,
          marginRightPercentage: 10,
        },
      ],
      //8 marital status
      [
        {
          id: 1,
          model: "guid_marital_status_id",
          type: "select",
          label: "pme.individual.maritalStatus",
          required: true,
          options: localThis.confs.find(
            (ele) => ele.routeName == "MaritalStatus"
          ).data,
          widthPercentage: 100,
          marginRightPercentage: 10,
        },
      ],
      //9 household
      [
        {
          id: 1,
          model: "jrs_household_identifier",
          type: "text",
          label: "pme.individual.jrs_household_identifier",
          disabled: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 50,
          type: "staticText",
          label: "pme.individual.is_owner",
          model: "is_owner",
        },
        {
          id: 3,
          model: "owner_same_as_provider",
          type: "radio",
          label: "Is the owner the same as the provider?",
          hint: ``,
          required: true,
          widthPercentage: 90,
          marginRightPercentage: 10,
          options: [
            { id: "true", label: "Yes" },
            { id: "false", label: "No", default: true },
          ],
        },

        {
          id: 5,
          model: "household_provider",
          type: "text",
          label: "pme.individual.household_provider",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 7,
          model: "household_provider_gender_id",
          type: "select",
          label: "pme.individual.household_provider_gender_id",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Gender")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 4,
          model: "household_provider_relation_type_id",
          type: "select",
          label: "pme.individual.provider_relationship_type",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: localThis.confs.find(
            (ele) => ele.routeName == "RelationshipsType"
          ).data,
        },

        {
          id: 6,
          model: "relationships_type_id",
          type: "select",
          label: "pme.individual.relationships_type",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: localThis.confs.find(
            (ele) => ele.routeName == "RelationshipsType"
          ).data,
        },

        {
          id: 8,
          model: "file_open_date",
          type: "date",
          label: "pme.individual.file_open_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },

        {
          id: 9,
          model: "is_orphan",
          type: "radio",
          label: "pme.individual.is_orphan",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: [
            { id: "true", label: "Yes" },
            { id: "false", label: "No", default: true },
          ],
        },
        {
          id: 10,
          model: "is_active",
          type: "radio",
          label: "pme.individual.is_active",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: [
            { id: "true", label: "Yes" },
            { id: "false", label: "No", default: true },
          ],
        },
        {
          id: 100,
          model: "_org_household_provider",
          type: "text",
          hide: true,
        },
        {
          id: 101,
          model: "_org_household_provider_gender_id",
          type: "text",
          hide: true,
        },
        {
          id: 102,
          model: "_org_household_provider_relation_type_id",
          type: "text",
          hide: true,
        },
      ],
    ];

    const columns = [
      [],
      [
        {
          id: 0,
          label: "guid_individual_document_type_id",
          field: "guid_individual_document_type_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        {
          id: 2,
          label: "Type",
          field: "document_parent",
          width: 30,
        },
        { id: 3, label: "Document type", field: "document_type", width: 30 },
        {
          id: 4,
          label: "Document number",
          field: "document_number",
          width: 30,
        },
        { id: 5, label: "Is active", field: "is_active", width: 30 },
      ],
      //2 education
      [
        {
          id: 0,
          label: "guid_educational_level_id",
          field: "guid_educational_level_id",
          primary: true,
          hide: true,
        },
        { id: 1, label: "Guid", field: "guid_individual_id", hide: true },
        { id: 2, label: "Educational level", field: "educational_level" },
        { id: 3, label: "Description", field: "description" },
        { id: 4, label: "Is active", field: "is_active" },
      ],
      //3 address
      [
        {
          id: 0,
          label: "guid_houshold_address_id",
          field: "guid_houshold_address_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        { id: 2, label: "Country", field: "country_name" },
        { id: 3, label: "State", field: "state_name" },
        { id: 4, label: "City", field: "city_name" },
        { id: 5, label: "Neighborhood", field: "neighborhood_name" },
        { id: 6, label: "Rent value", field: "rent_value" },
        { id: 7, label: "Household address", field: "household_address" },
        { id: 8, label: "Property type", field: "address_property_type" },
        { id: 9, label: "Status type", field: "address_status_type" },
      ],
      //special
      [
        {
          id: 0,
          label: "guid_special_need_id",
          field: "guid_special_need_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        { id: 2, label: "Special need", field: "special_need_type" },
        { id: 3, label: "Is active", field: "is_active", toggle: true },
        {
          id: 4,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],

      //5 pregnancy
      [
        {
          id: 0,
          label: "guid_pregnant_id",
          field: "guid_pregnant_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        {
          id: 2,
          label: "Pregnancy start date",
          field: "pregnancy_start_date",
          date: true,
        },
        { id: 3, label: "Is active", field: "is_active", toggle: true },
        {
          id: 4,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],

      //6 Household
      [],
      //7 contact
      [
        {
          id: 0,
          label: "guid_individual_contact_id",
          field: "guid_individual_contact_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        { id: 2, label: "pme.individual.contact_type", field: "contact_type" },
        {
          id: 3,
          label: "pme.individual.contact_value",
          field: "contact_value",
        },
        { id: 4, label: "Is active", field: "is_active", toggle: true },
        {
          id: 5,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],
      //8 nationality
      [
        {
          id: 0,
          label: "guid_individual_nationality_id",
          field: "guid_individual_nationality_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        {
          id: 2,
          field: "native_nationality",
          label: "pme.individual.native_nationality_id",
        },
        { id: 4, label: "Is active", field: "is_active", toggle: true },
        {
          id: 5,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],

      //9 marital status
      [
        {
          id: 0,
          label: "guid_individual_marital_status_id",
          field: "guid_individual_marital_status_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "guid_individual_id",
          field: "guid_individual_id",
          hide: true,
        },
        {
          id: 2,
          field: "marital_status",
          label: "pme.individual.maritalStatus",
        },
        { id: 4, label: "Is active", field: "is_active", toggle: true },
        {
          id: 5,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],
    ];

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

    localThis.tabs = [
      {
        id: 0,
        name: "Personal Information",
        theData: theData[0],
        inputs: inputs[0],
        grid: false,
        routeGet: "Individual/GetIndividualByGuid",
        bodyGet: { guid_individual_id: this.id },
        submitRoute: "Individual/Individual_Update",
        saveDisabled: true,
      },

      {
        id: 1,
        name: "Individual Details",
        grid: false,
        additionalTabs: [
          {
            gridTitle: "Individual Contact",
            submitRoute: "Individual/IndividualContact_Insert",
            routeGet: "Individual/GetIndividualContactByGuid",
            bodyGet: { guid_individual_id: this.id },
            inputs: inputs[6],
            theData: theData[6],
            gridData: [],
            gridColumns: this.columns[7],
            routeDelete: "Individual/IndividualContact_Delete",
            routeChangeStatus: "Individual/IndividualContact_ChangeStatus",

            grid: true,
          },
          {
            gridTitle: "Individual Nationality",
            submitRoute: "Individual/IndividualNationality_Insert",
            routeGet: "Individual/GetIndividualNationalityByGuid",
            bodyGet: { guid_individual_id: this.id },
            inputs: inputs[7],
            theData: theData[7],
            gridData: [],
            gridColumns: this.columns[8],
            routeDelete: "Individual/IndividualNationality_Delete",
            routeChangeStatus: "Individual/IndividualNationality_ChangeStatus",

            grid: true,
          },
          {
            gridTitle: "Individual Marital status",
            submitRoute: "Individual/IndividualMaritalStatus_Insert",
            routeGet: "Individual/GetIndividualMaritalStatusByGuid",
            bodyGet: { guid_individual_id: this.id },
            inputs: inputs[8],
            theData: theData[8],
            gridData: [],
            gridColumns: this.columns[9],
            routeDelete: "Individual/IndividualMaritalStatus_Delete",
            routeChangeStatus:
              "Individual/IndividualMaritalStatus_ChangeStatus",

            grid: true,
          },
          {
            gridTitle: "Special need",
            submitRoute: "Individual/IndividualSpecialNeed_Insert",
            routeGet: "Individual/GetIndividualSpecialNeedByGuid",
            bodyGet: { guid_individual_id: this.id },
            inputs: inputs[4],
            theData: theData[4],
            gridData: [],
            gridColumns: this.columns[4],
            routeDelete: "Individual/IndividualSpecialNeed_Delete",
            routeChangeStatus: "Individual/IndividualSpecialNeed_ChangeStatus",
            grid: true,
          },
          {
            gridTitle: "Pregnancy",
            submitRoute: "Individual/IndividualPregnancy_Insert",
            routeGet: "Individual/GetIndividualPregnancyByGuid",
            bodyGet: { guid_individual_id: this.id },
            inputs: inputs[5],
            theData: theData[5],
            gridData: [],
            gridColumns: this.columns[5],
            routeDelete: "Individual/IndividualPregnancy_Delete",
            routeChangeStatus: "Individual/IndividualPregnancy_ChangeStatus",

            grid: true,

            hide: this.hidePregnancy,
          },
        ],
      },
      {
        id: 2,
        name: "Formal Documents",
        theData: theData[1],
        inputs: inputs[1],
        gridColumns: this.columns[1],

        grid: true,
        routeGet: "Individual/GetIndividualHouseholdDocumentsByGuid",
        bodyGet: { guid_individual_id: this.id },
        submitRoute: "Individual/IndividualDocument_Insert",
        gridData: [],
        saveDisabled: false,
      },
      {
        id: 3,
        name: "Educational Information",
        theData: theData[2],
        inputs: inputs[2],
        gridColumns: this.columns[2],

        grid: true,
        routeGet: "Individual/GetIndividualEducationByGuid",
        bodyGet: { guid_individual_id: this.id },
        submitRoute: "Individual/IndividualEducation_Insert",
        gridData: [],
        saveDisabled: true,
      },
      {
        id: 4,
        name: "Household",
        inputs: inputs[9],
        theData: theData[9],
        grid: false,
        routeGet: "Individual/GetIndividualHouseholdByGuid",
        bodyGet: { guid_individual_id: this.id },
        submitRoute: "Individual/IndividualHousehold_Update",
        saveDisabled: true,
        afterGet: (data) => {
          this.theData[9]._org_household_provider =
            data[0][0].household_provider;
          this.theData[9]._org_household_provider_gender_id =
            data[0][0].household_provider_gender_id;
          this.theData[9]._org_household_provider_relation_type_id =
            data[0][0].household_provider_relation_type_id;
        },
      },
      {
        id: 5,
        name: "Household Address",
        inputs: inputs[3],
        theData: theData[3],
        grid: false,
        routeGet: "Individual/GetIndividualAddressByGuid",
        bodyGet: { guid_individual_id: this.id },
        submitRoute: "Individual/IndividualHouseholdAddress_Update",
        saveDisabled: true,
      },
    ];
  },

  watch: {
    theData: {
      deep: true,
      handler(newValue, oldValue) {
        console.log("newvalue: ", newValue);
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
      },
    },
    "theData.9.owner_same_as_provider": function (newValue) {
      const i1 = this.inputs[9].find(
        (ele) => ele.model == "household_provider"
      );
      const i2 = this.inputs[9].find(
        (ele) => ele.model == "household_provider_gender_id"
      );
      const i3 = this.inputs[9].find(
        (ele) => ele.model == "household_provider_relation_type_id"
      );

      if (newValue == "true") {
        i1.disabled = i2.disabled = i3.disabled = true;

        this.theData[9].household_provider =
          this.theData[9]._org_household_provider;
        this.theData[9].household_provider_gender_id =
          this.theData[9]._org_household_provider_gender_id;
        this.theData[9].household_provider_relation_type_id =
          this.theData[9]._org_household_provider_relation_type_id;

        i1.label = "Provider, owner full name";
        i2.label = "Provider, owner gender";
        i3.label = "Provider, owner relation type";
      } else {
        i1.disabled = i2.disabled = i3.disabled = false;

        i1.label = "Provider full name";
        i2.label = "Provider gender";
        i3.label = "Provider relation type";
      }
    },
    "theData.9.household_provider": function (newValue, oldValue) {
      const i1 = this.inputs[9].find(
        (ele) => ele.model == "owner_same_as_provider"
      );
      i1.hint = `Right now the provider is ${newValue}`;
    },

    "theData.0.gender_id": function (newValue) {
      if (newValue == 2) {
        this.hidePregnancy = false;
      } else {
        this.hidePregnancy = true;
      }
    },
  },
};
</script>

<style scoped>
@import "./IndividualEdit.scss";
</style>
