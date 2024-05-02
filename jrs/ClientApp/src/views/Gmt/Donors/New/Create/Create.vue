<template>
  <wizard-comp
    :steps="steps"
    :inputs="inputs"
    :theData="theData"
    :submit="submit"
    previewTitle="Donor Form Preview"
  />
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import Wizard from "../../../../../components/Inputs/Wizard/Wizard.vue";
export default {
  components: {
    "wizard-comp": Wizard,
  },
  data() {
    return {
      steps: [],
      inputs: [],
      theData: {},
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getConfiguration: "getConfiguration",
    }),
    async submit() {
      const filteredBody = {};
      const body = this.theData;
      for (const key in body) {
        if (body[key] !== "") {
          filteredBody[key] = body[key];
        }
      }
      
      filteredBody.userId = this.userId;
      try {
        const response = await this.getConfiguration({
          name: "Donor/Donnor_Add",
          body: filteredBody,
          post: true,
        });

        this.showNewSnackbar({
          typeName: "success",
          text: "Completed successfully",
        });
        this.$router.replace({ name: "donnor" });

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
    convertDateToString(theDate) {
      const date = new Date(theDate);
      const yyyy = date.getFullYear();
      const mm = String(date.getMonth() + 1).padStart(2, "0"); // Months are 0-11, so we add 1 to get 1-12
      const dd = String(date.getDate()).padStart(2, "0");
      const formattedDate = `${yyyy}-${mm}-${dd}`;

      return formattedDate;
    },
    handleDonorCountryStartDateChange(newValue) {
      this.inputs.find((ele) => ele.model === "donor_country_end_date").min =
        this.convertDateToString(new Date(newValue.target.value));
    },
    async handleCountryGuidChange(newValue) {
      const theStates = await this.getConfFromApi(
        "State/basedOnCountry",
        newValue.target.value,
        true
      );

      const theStatesOptions = theStates.map((ele) => ({
        id: ele.guid_state_id,
        label: ele.state_name,
      }));
      this.inputs.find((ele) => ele.model == "guid_state_id").options =
        theStatesOptions;
      this.inputs.find((ele) => ele.model == "guid_state_id").disabled = false;
    },
    async handleStateGuidChange(newValue) {
      const thecities = await this.getConfFromApi(
        "City/basedOnState",
        newValue.target.value,
        true
      );
      const cityList = thecities.map((ele) => ({
        id: ele.guid_city_id,
        label: ele.city_name,
      }));
      this.inputs.find((ele) => ele.model == "guid_city_id").options = cityList;
      this.inputs.find((ele) => ele.model == "guid_city_id").disabled = false;
    },
  },
  async beforeMount() {
    const confs = [
      {
        id: 1,
        routeName: "DonorCategory",
        idName: "guid_donor_category_id",
        labelName: "donor_category",
      },
      {
        id: 2,
        routeName: "ContactType",
        idName: "guid_contact_type_id",
        labelName: "contact_type",
      },
      {
        id: 3,
        routeName: "JrsRegionCountry",
        idName: "guid_JRS_region_country_id",
        labelName: "region_country_name",
      },
      {
        id: 4,
        routeName: "Country",
        idName: "guid_country_id",
        labelName: "country_name",
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
        icon: "mdi-account",
        name: "Donor Info",
      },
      {
        id: 2,
        icon: "mdi-account",
        name: "Donor Contact",
      },
      {
        id: 3,
        icon: "mdi-account",
        name: "Donor Countries",
      },
      {
        id: 4,
        icon: "mdi-account",
        name: "Donor Address",
      },
      {
        id: 5,
        name: "Preview data",
        icon: "mdi-file",
      },
    ];
    this.inputs = [
      {
        id: 1,
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,

        model: "donor_name",
        type: "text",
        label: "GMT.DONORS.donor_name",
      },
      {
        id: 2,
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        type: "text",

        model: "donor_description",
        label: "GMT.DONORS.donor_description",
      },
      {
        id: 3,
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "text",

        model: "donor_code",
        label: "GMT.DONORS.donor_code",
      },
      {
        id: 4,
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "select",
        options: newConfs.find((ele) => ele.routeName == "DonorCategory").data,

        model: "guid_donor_category_id",
        label: "GMT.DONORS.guid_donor_category_id",
      },
      {
        id: 5,
        step: 1,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,

        model: "donor_number",
        type: "text",
        label: "GMT.DONORS.donor_number",
      },
      {
        id: 6,
        step: 2,
        widthPercentage: 40,
        marginRightPercentage: 10,

        required: true,
        type: "select",
        options: newConfs.find((ele) => ele.routeName == "ContactType").data,

        model: "guid_contact_type_id",
        label: "GMT.DONORS.contact_type",
      },
      {
        id: 7,
        step: 2,
        widthPercentage: 40,
        marginRightPercentage: 10,

        required: true,
        type: "text",

        model: "donor_contact",
        label: "GMT.DONORS.donor_contact",
      },
      {
        id: 8,
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "select",
        options: newConfs.find((ele) => ele.routeName == "JrsRegionCountry")
          .data,
        model: "guid_JRS_region_country_id",
        label: "GMT.DONORS.guid_JRS_region_country_id",
      },
      {
        id: 9,
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "date",

        model: "donor_country_start_date",
        label: "GMT.DONORS.donor_country_start_date",

        max: this.convertDateToString(new Date()),
        onChange: this.handleDonorCountryStartDateChange,
      },
      {
        id: 10,
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "date",

        model: "donor_country_end_date",
        label: "GMT.DONORS.donor_country_end_date",
        max: this.convertDateToString(new Date()),
      },
      {
        id: 11,
        step: 3,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "text",

        model: "donor_country_description",
        label: "GMT.DONORS.donor_country_description",
      },
      {
        id: 12,
        step: 4,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "select",

        model: "guid_country_id",
        label: "GMT.DONORS.country_name",

        options: this.confs.find((ele) => ele.routeName == "Country").data,
        onChange: this.handleCountryGuidChange,
      },
      {
        id: 13,
        step: 4,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "select",

        model: "guid_state_id",
        label: "GMT.DONORS.state_name",
        options: [],
        disabled: true,
        onChange: this.handleStateGuidChange,
      },
      {
        id: 14,
        step: 4,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "select",

        model: "guid_city_id",
        label: "GMT.DONORS.city_name",
        options: [],
        disabled: true,
      },

      {
        id: 15,
        step: 4,
        widthPercentage: 40,
        marginRightPercentage: 10,
        required: true,
        type: "text",

        model: "donor_address",
        label: "GMT.DONORS.donor_address",
      },
    ];
    const theData = {};
    for (let t = 0; t < this.inputs.length; t++) {
      theData[this.inputs[t].model] = "";
    }
    this.theData = theData;
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
