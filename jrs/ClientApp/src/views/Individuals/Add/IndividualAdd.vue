<template>
  <wizard-comp
    :steps="steps"
    :inputs="inputs"
    :theData="theData"
    :submit="submit"
    previewTitle="Individual Form Preview"
  />
</template>

<script>
import { mapActions, mapGetters } from "vuex";
// import Wizard from "../../../../components/Inputs/Wizard/Wizard.vue";
import Wizard from "../../../components/Inputs/Wizard/Wizard.vue";
export default {
  components: {
    "wizard-comp": Wizard,
  },
  data() {
    return {
      steps: [],
      inputs: [],
      hidePregnant: false,
      theData: {
      },
    };
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getConfiguration: "getConfiguration",
    }),
    isJsonString(str) {
      try {
        JSON.parse(str);
      } catch (e) {
        return false;
      }
      return true;
    },
    async submit() {
      const filteredBody = {};
      const body = JSON.parse(JSON.stringify(this.theData));
      for (const key in body) {
        const val = body[key];
        if (key == "guid_special_need_type_id" && !this.isJsonString(val)) {
          const obj = val
            .split(",")
            .filter((ele) => ele != "")
            .map((ele) => ({ guid_special_need_type_id: ele }));
          filteredBody[key] = JSON.stringify(obj);
          body[key] = JSON.stringify(obj);
        } else if (
          ((key == "householdDocumentType" ||
            key == "individualDocumentType") &&
            !this.isJsonString(val)) ||
          (this.isJsonString(val) &&
            val &&
            JSON.parse(val)[0] &&
            !JSON.parse(val)[0].guid_document_type_id)
        ) {
          const ar = JSON.parse(val);
          let newVal = ar.map((ele) => ({
            guid_document_type_id: ele.id,
            document_number: ele.value,
          }));
          filteredBody[key] = JSON.stringify(newVal);
          body[key] = JSON.stringify(newVal);
        } else {
          filteredBody[key] = val;
        }

        if (body[key] !== "") {
          filteredBody[key] = body[key];
        }
      }
      filteredBody.userId = this.userId;
      try {
        const response = await this.getConfiguration({
          name: "Individual/Individual_Add",
          body: filteredBody,
          post: true,
        });
        const JRS_individual_identifier = response.message;

        const inpN = {
          id: 39,
          step: 1,
          widthPercentage: 90,
          marginRightPercentage: 0,
          type: "text",
          required: false,
          model: "JRS_individual_identifier",
          label: "JRS individual identifier",
        };
        this.theData.JRS_individual_identifier = JRS_individual_identifier;

        this.inputs = [...this.inputs, inpN];
        if(response.message){
          this.showNewSnackbar({
          typeName: "success",
          text:
            "Individual has been inserted successfully, with JRS individual identifier: " +
            response.message,
        });
        }else{
          this.showNewSnackbar({
          typeName: "error",
          text:
            "Something went wrong, please try again later",
        });
        }
        
        // this.$router.replace({ name: "individual" });
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
    convertDateToString(date) {
      const yyyy = date.getFullYear();
      const mm = String(date.getMonth() + 1).padStart(2, "0"); // Months are 0-11, so we add 1 to get 1-12
      const dd = String(date.getDate()).padStart(2, "0");
      const formattedDate = `${yyyy}-${mm}-${dd}`;

      return formattedDate;
    },
  },

  async beforeMount() {
    const localThis = this;
    const confs = [
      {
        id: 1,
        routeName: "IndividualDocumentType",
        idName: "guid_document_type_id",
        labelName: "document_type",
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
        extraName: "has_original_address",
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
    ];

    const newConfs = JSON.parse(JSON.stringify(confs));

    for (let i = 0; i < newConfs.length; i++) {
      const conf = newConfs[i];
      let requ = await this.getConfFromApi(conf.routeName);
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
    this.confs = newConfs;

    this.steps = [
      {
        id: 1,
        name: " Personal Information",
        icon: "mdi-account",
        nextDisabled: true,

        nextAdditional: async (next) => {
          const theData = this.theData;

          if (
            theData.individual_first_name &&
            theData.individual_first_name != "" &&
            theData.individual_last_name &&
            theData.individual_last_name != "" &&
            theData.gender_id &&
            theData.gender_id != "" &&
            theData.date_of_birth &&
            theData.date_of_birth != "" &&
            theData.guid_native_nationality_id &&
            theData.guid_native_nationality_id != ""
          ) {
            try {
              const body = {
                individual_first_name: theData.individual_first_name,
                individual_last_name: theData.individual_last_name,
                gender_id: theData.gender_id,
                date_of_birth: theData.date_of_birth,
                guid_native_nationality_id: theData.guid_native_nationality_id,
              };
              const response = await this.getConfiguration({
                name: "Individual/CheckPersonExist",
                body,
                post: true,
              });

              if (response == "not exists") next();
              else {
                this.showNewSnackbar({
                  typeName: "error",
                  text: response,
                });
                this.steps[0].nextDisabled = true;
                this.steps[0].errorMessage = response;
              }
              // this.$router.replace({ name: "individual" });
            } catch (ex) {
              this.showNewSnackbar({
                typeName: "error",
                text: "There was an error with checking individual, please try again later",
              });
            }
          }
          return false;
        },
      },
      {
        id: 2,
        name: "Formal Documents",
        icon: "mdi-passport",
        nextDisabled: true,
        nextAdditional: async (next) => {
          const theData = this.theData;

          if (
            theData.householdDocumentType &&
            theData.householdDocumentType != "" &&
            theData.individualDocumentType &&
            theData.individualDocumentType != ""
          ) {
            try {
              const body = {
                householdDocumentType: JSON.stringify(
                  JSON.parse(theData.householdDocumentType).map((ele) => ({
                    guid_document_type_id: ele.id,
                    document_number: ele.value,
                  }))
                ),
                individualDocumentType: JSON.stringify(
                  JSON.parse(theData.individualDocumentType).map((ele) => ({
                    guid_document_type_id: ele.id,
                    document_number: ele.value,
                  }))
                ),
              };
              const response = await this.getConfiguration({
                name: "Individual/Individual_Household_checkDocuments",
                body,
                post: true,
              });

              const res1 = response.result1;
              const res2 = response.result2;
              console.log("res1: ", res1);
              console.log("res2: ", res2);
              if (res1 == "not exists" && res2 == "not exists") next();
              else {
                let msg = "";
                if (res1 != "not exists")
                  msg += "Individual documents: " + res1;
                if (res2 != "not exists") msg += "Household documents: " + res2;
                this.showNewSnackbar({
                  typeName: "error",
                  text: msg,
                });

                // this.steps[0].nextDisabled = true;
                // this.steps[0].errorMessage = response;
              }
              // this.$router.replace({ name: "individual" });
            } catch (ex) {
              console.log("error in checking documents: ", ex);
              this.showNewSnackbar({
                typeName: "error",
                text: "There was an error with checking individual, please try again later",
              });
            }
          }
          return false;
        },
      },
      {
        id: 3,
        name: "Household",
        icon: "mdi-home",
        nextDisabled: true,
      },
      {
        id: 4,
        name: "Educational Information ",
        icon: "mdi-school",
        nextDisabled: true,
      },
      {
        id: 5,
        name: "Address information",
        icon: "mdi-map-marker",
        nextDisabled: true,
      },
      {
        id: 6,
        name: "Protection",
        icon: "mdi-file",
        nextlabel: "Skip",
        nextDisabled: false,
      },
      {
        id: 7,
        name: "Preview data",
        icon: "mdi-file",
      },
    ];
    this.inputs = [
      {
        step: 1,
        model: "guid_data_section_id",
        label: "pme.individual.DataSection",
        type: "select",
        required: true,
        options: newConfs.find(
          (ele) => ele.routeName == "DataSection/Enterable"
        ).data,

        widthPercentage: 90,
        marginRightPercentage: 10,
      },
      {
        step: 1,
        model: "individual_first_name",
        label: "pme.individual.firstName",
        type: "text",
        required: true,

        widthPercentage: 40,
        marginRightPercentage: 10,
      },
      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        model: "individual_last_name",
        label: "pme.individual.lastName",
      },
      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: true,

        label: "pme.individual.gender",
        model: "gender_id",
        options: newConfs.find((ele) => ele.routeName == "Gender").data,
      },
      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "date",
        required: true,

        model: "date_of_birth",
        label: "pme.individual.dateOfBirth",
        max: this.convertDateToString(new Date()),
      },

      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: true,

        model: "guid_native_nationality_id",
        label: "pme.individual.native_nationality_id",
        options: newConfs.find(
          (ele) => ele.routeName == "Country/getNationalities"
        ).data,
      },
      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: true,

        model: "guid_marital_status_id",
        label: "pme.individual.maritalStatus",
        options: newConfs.find((ele) => ele.routeName == "MaritalStatus").data,
      },

      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        label: "pme.individual.fatherName",
        model: "individual_father_name",
      },

      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        model: "individual_mother_name",
        label: "pme.individual.motherName",
      },

      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: true,

        model: "guid_native_language_id",
        label: "pme.individual.native_language",
        options: newConfs.find((ele) => ele.routeName == "Language").data,
      },

      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        model: "place_of_birth",
        label: "pme.individual.place_of_birth",
      },

      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        model: "individual_first_name_native_lang",
        label: "pme.individual.individual_first_name_native_lang",
      },
      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        model: "individual_last_name_native_lang",
        label: "pme.individual.individual_last_name_native_lang",
      },
      {
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: true,

        model: "place_of_birth_native_lang",
        label: "pme.individual.place_of_birth_native_lang",
      },

      {
        step: 2,
        widthPercentage: 100,
        marginRightPercentage: 0,
        type: "select-checkbox-withInput",

        model: "individualDocumentType",
        hint: "At least one",
        label: "pme.individual.individualDocumentType",
        required: true,
        options: newConfs.find(
          (ele) => ele.routeName == "IndividualDocumentType"
        ).data,
        values: newConfs
          .find((ele) => ele.routeName == "IndividualDocumentType")
          .data.map((ele) => ""),
      },

      {
        model: "householdDocumentType",
        step: 2,
        hint: "At least one",
        label: "pme.individual.householdDocumentType",
        type: "select-checkbox-withInput",
        required: true,
        options: newConfs.find(
          (ele) => ele.routeName == "HouseholdDocumentType"
        ).data,
        values: newConfs
          .find((ele) => ele.routeName == "HouseholdDocumentType")
          .data.map((ele) => ""),
        widthPercentage: 100,
        marginRightPercentage: 0,
      },
      {
        step: 3,
        widthPercentage: 90,
        marginRightPercentage: 10,
        type: "radio",
        required: true,
        label: "Is this owner the same person as the provider?",
        options: [
          { id: "true", label: "Yes" },
          { id: "false", label: "No", default: true },
        ],
        model: "owner_same_as_provider",
      },
      {
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",
        required: false,
        label: "Provider and owner full name",
        model: "household_provider",
      },
      {
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: false,

        label: "Provider gender",
        model: "household_provider_gender_id",
        options: newConfs.find((ele) => ele.routeName == "Gender").data,
      },
      {
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: false,
        model: "household_provider_relation_type_id",
        label: "Provider relation type",
        options: newConfs.find((ele) => ele.routeName == "RelationshipsType")
          .data,
      },

      {
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: true,

        model: "guid_household_status_type_id",
        label: "pme.individual.householdStatus",
        options: newConfs.find((ele) => ele.routeName == "HouseholdStatusType")
          .data,
      },
      {
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "select",
        required: true,

        model: "relationships_type_id",
        label: "pme.individual.relationType",
        options: newConfs.find((ele) => ele.routeName == "RelationshipsType")
          .data,
      },

      {
        step: 4,
        widthPercentage: 100,
        marginRightPercentage: 0,
        type: "select",
        required: true,

        model: "guid_educational_level_id",
        label: "pme.individual.educationLevel",
        options: newConfs.find((ele) => ele.routeName == "EducationalLevel")
          .data,
      },
      {
        step: 4,
        widthPercentage: 100,
        marginRightPercentage: 0,
        type: "text",
        required: true,

        label: "pme.individual.educationLvelTxt",
        model: "education_description",
      },

      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "select",
        required: true,

        model: "guid_country_id",
        label: "pme.individual.country",
        options: newConfs.find((ele) => ele.routeName == "Country").data,
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "select",
        disabled: true,
        required: true,

        model: "guid_state_id",
        label: "pme.individual.state",
        options: [],
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "select",
        disabled: true,
        required: true,

        model: "guid_city_id",
        label: "pme.individual.city",
        options: [],
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "select",
        required: true,
        disabled: true,

        label: "pme.individual.neighborhood",
        model: "guid_neighborhood_id",
        options: [],
      },

      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "number",
        required: true,

        model: "totalRooms",
        label: "pme.individual.totalRooms",
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "select",
        required: true,

        model: "guid_address_property_type_id",
        label: "pme.individual.propertyType",
        options: newConfs.find((ele) => ele.routeName == "AddressPropertyType")
          .data,
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "number",
        required: true,

        model: "rent_value",
        label: "pme.individual.rentValue",
        // hide: this.hideRent,
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "text",
        required: true,

        label: "pme.individual.household_address",
        model: "household_address",
      },

      {
        step: 5,
        marginRightPercentage: 5,
        widthPercentage: 40,
        type: "text",
        required: true,

        model: "originalAddress",
        label: "pme.individual.originalAddress",
      },

      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "select",
        required: true,

        label: "pme.individual.livingStatus",
        model: "guid_address_status_type_id",
        options: newConfs.find((ele) => ele.routeName == "AddressStatusType")
          .data,
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "tel",
        required: true,
        countryCode: "+31",

        model: "mobileNumber",
        label: "pme.individual.mobileNumber",
      },
      {
        step: 5,
        marginRightPercentage: 5,
        widthPercentage: 40,
        type: "tel",
        required: false,
        countryCode: "+31",

        model: "mobileNumber2",
        label: "pme.individual.mobileNumber2",
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "email",

        model: "email",
        label: "pme.individual.email",
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "text",
        model: "address_notes",
        label: "Address notes",
      },

      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "radio",

        model: "isThereHousemates",
        label: "Is there housemates?",
        options: [
          { id: "true", label: "Yes" },
          { id: "false", label: "No" },
        ],
      },
      {
        step: 5,
        widthPercentage: 40,
        marginRightPercentage: 5,
        type: "text",
        model: "address_housemates",
        label: "Address housemates",
        hide: true,
      },

      {
        step: 6,
        widthPercentage: 90,
        marginRightPercentage: 0,
        type: "checkbox",
        required: false,
        hint: "Not required",

        model: "guid_special_need_type_id",
        label: "pme.individual.specialNeeds",
        options: newConfs.find((ele) => ele.routeName == "SpecialNeed").data,
      },

      {
        step: 6,
        widthPercentage: 90,
        marginRightPercentage: 0,
        type: "radio",
        required: false,

        model: "is_chronic",
        label: "pme.individual.isThereChronic",
        options: [
          { id: "false", label: "No" },
          { id: "true", label: "Yes" },
        ],
      },
      {
        step: 6,
        widthPercentage: 90,
        marginRightPercentage: 0,
        type: "radio",
        required: false,

        model: "is_pregnant",
        label: "pme.individual.isPregnant",
        hide: this.hidePregnant,
        options: [
          { id: "false", label: "No" },
          { id: "true", label: "Yes" },
        ],
      },
    ].map((ele, idx) => ({ ...ele, id: idx + 1 }));
  },
  watch: {
    "theData.guid_country_id": async function (newValue) {
      const co = this.confs
        .find((ele) => ele.routeName == "Country")
        .data.find((ele) => ele.id == newValue);
      if (co) {
        const phone_code = co.phone_code;
        this.inputs.find((ele) => ele.model == "mobileNumber").countryCode =
          "+" + phone_code;
        this.inputs.find((ele) => ele.model == "mobileNumber2").countryCode =
          "+" + phone_code;
      }

      const theStates = await this.getConfFromApi(
        "State/basedOnCountry",
        newValue,
        true
      );

      this.stateList = theStates.map((ele) => ({
        id: ele.guid_state_id,
        label: ele.state_name,
      }));

      this.inputs.find((ele) => ele.model == "guid_state_id").options =
        this.stateList;
      this.inputs.find((ele) => ele.model == "guid_state_id").disabled = false;
    },
    "theData.guid_state_id": async function (newValue) {
      const thecities = await this.getConfFromApi(
        "City/basedOnState",
        newValue,
        true
      );
      this.cityList = thecities.map((ele) => ({
        id: ele.guid_city_id,
        label: ele.city_name,
      }));
      this.inputs.find((ele) => ele.model == "guid_city_id").options =
        this.cityList;
      this.inputs.find((ele) => ele.model == "guid_city_id").disabled = false;
    },
    "theData.guid_city_id": async function (newValue) {
      const theNeis = await this.getConfFromApi(
        "Neighborhood/basedOnCity",
        newValue,
        true
      );
      this.neisList = theNeis.map((ele) => ({
        id: ele.guid_neighborhood_id,
        label: ele.neighborhood_name,
      }));
      this.inputs.find((ele) => ele.model == "guid_neighborhood_id").options =
        this.neisList;
      this.inputs.find(
        (ele) => ele.model == "guid_neighborhood_id"
      ).disabled = false;
    },

    "theData.individual_first_name": async function (newValue) {
      if (newValue.length > 3) {
        const val = await this.getConfFromApi(
          "Individual/GetGenderByFirstName",
          newValue,
          true
        );
        if ((val && val == 1) || val == 2)
          this.theData.gender_id = val.toString();
      }
    },
    "theData.guid_special_need_type_id": async function (newValue) {
      const sts = this.steps;
      const st = sts.find((ele) => ele.id == 5);

      if (newValue != "") {
        st.nextlabel = "Next";
      } else {
        st.nextLabel = "Skip";
      }

      this.steps = sts;
    },
    "theData.individualDocumentType": async function (newValue) {
      const ar = JSON.parse(newValue);
      let newVal = ar.map((ele) => ({
        guid_document_type_id: ele.id,
        document_number: ele.value,
      }));
    },
    "theData.gender_id": async function (newValue) {
      if (newValue == 2) {
        this.hidePregnant = false;
      } else {
        this.hidePregnant = true;
      }
      this.inputs.find((ele) => ele.model == "is_pregnant").hide =
        this.hidePregnant;
      this.inputs = this.inputs;
    },
    "theData.guid_household_status_type_id": function (newValue) {
      const nn = this.confs
        .find((ele) => ele.routeName == "HouseholdStatusType")
        .data.find((ele) => ele.id == newValue);

      this.inputs.find((ele) => ele.model == "originalAddress").hide =
        !nn.has_original_address;
      this.inputs = this.inputs;
    },
    "theData.guid_address_property_type_id": function (newValue) {
      const hideRent = newValue != "c87e380c-b8f7-432b-9c83-612b0100da48";
      this.inputs.find((ele) => ele.model == "rent_value").hide = hideRent;
      this.inputs = this.inputs;
    },
    "theData.isThereHousemates": function (newValue) {
      const hideHousemates = newValue == "false";
      this.inputs.find((ele) => ele.model == "address_housemates").hide =
        hideHousemates;
      this.inputs = this.inputs;
    },
    "theData.owner_same_as_provider": function (newValue) {
      //Show hide the provider full name disabled

      const i1 = this.inputs.find((ele) => ele.model == "household_provider");
      const i2 = this.inputs.find(
        (ele) => ele.model == "household_provider_gender_id"
      );
      const i3 = this.inputs.find(
        (ele) => ele.model == "household_provider_relation_type_id"
      );

      if (newValue == "true") {
        i1.disabled = i2.disabled = i3.disabled = true;
        this.theData[i1.model] =
          this.theData.individual_first_name +
          " " +
          this.theData.individual_last_name;
        this.theData[i2.model] = this.theData.gender_id;
        this.theData[i3.model] = this.theData.relationships_type_id;

        i1.label = "Provider, owner full name";
        i2.label = "Provider, owner gender";
        i3.label = "Provider, owner relation type";
      } else {
        i1.disabled = i2.disabled = i3.disabled = false;
        this.theData[i1.model] =
          this.theData[i2.model] =
          this.theData[i3.model] =
            "";

        i1.label = "Provider full name";
        i2.label = "Provider gender";
        i3.label = "Provider relation type";
      }

      i1.hide = false;

      this.inputs = this.inputs;
    },
    "theData.relationships_type_id": function (newValue) {
      if (this.theData.owner_same_as_provider == "true") {
        const i3 = this.inputs.find(
          (ele) => ele.model == "household_provider_relation_type_id"
        );
        this.theData[i3.model] = newValue;
      }
    },
  },
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
    }),

    userId() {
      return this.getUserUid;
    },
  },
};
</script>
<style></style>
