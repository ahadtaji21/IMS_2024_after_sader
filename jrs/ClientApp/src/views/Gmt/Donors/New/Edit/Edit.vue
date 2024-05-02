<template>
  <div id="app" class="root" v-if="tabs && tabs.length > 0">
    <div class="header">
      <h4>Donor name: {{ infoToShow.donor_name }}</h4>
      <p>Donor code: {{ infoToShow.donor_code }}</p>
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
        :theData="theData"
        idName="guid_donor_id"
        :idValue="id"
      />
    </div>
   
  </div>
</template>

<script>
import { mapActions } from "vuex";
import Tab from "../../../../../components/Inputs/Tabs/Tab.vue";

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
      theData: [],
      inputs: [],
      columns: [],

      confs: [],
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
      this.$router.replace({ name: "donnor" });
    },
    handleDonorCountryStartDateChange(newValue) {
      this.inputs[2].find((ele) => ele.model === "donor_country_end_date").min =
        this.convertDateToString(new Date(newValue.target.value));
      console.log("this.inputs: ", this.inputs[2]);
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
      this.inputs[3].find((ele) => ele.model == "guid_state_id").options =
        theStatesOptions;
      this.inputs[3].find(
        (ele) => ele.model == "guid_state_id"
      ).disabled = false;
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
      this.inputs[3].find((ele) => ele.model == "guid_city_id").options =
        cityList;
      this.inputs[3].find(
        (ele) => ele.model == "guid_city_id"
      ).disabled = false;
    },
  },
  async beforeMount() {
    this.infoToShow = JSON.parse(this.info);
    let localThis = this;
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
      [
        {
          id: 0,
          model: "guid_donor_id",
          type: "text",
          hide: true,
        },
        {
          id: 1,
          model: "donor_name",
          type: "text",
          label: "GMT.DONORS.donor_name",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 2,
          model: "donor_description",
          type: "text",
          label: "GMT.DONORS.donor_description",
          // required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 3,
          model: "donor_code",
          type: "text",
          label: "GMT.DONORS.donor_code",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 4,
          model: "guid_donor_category_id",
          type: "select",
          label: "GMT.DONORS.guid_donor_category_id",
          required: true,
          options: newConfs.find((ele) => ele.routeName == "DonorCategory")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
        {
          id: 5,
          model: "donor_number",
          type: "text",
          label: "GMT.DONORS.donor_number",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      [
        {
          id: 1,
          model: "guid_contact_type_id",
          type: "select",
          label: "GMT.DONORS.contact_type",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: newConfs.find((ele) => ele.routeName == "ContactType").data,
        },
        {
          id: 2,
          model: "donor_contact",
          type: "text",
          label: "GMT.DONORS.donor_contact",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      [
        {
          id: 1,
          model: "guid_JRS_region_country_id",
          type: "select",
          label: "GMT.DONORS.guid_JRS_region_country_id",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          options: newConfs.find((ele) => ele.routeName == "JrsRegionCountry")
            .data,
        },
        {
          id: 2,
          model: "donor_country_start_date",
          type: "date",
          label: "GMT.DONORS.donor_country_start_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
          onChange: this.handleDonorCountryStartDateChange,
        },
        {
          id: 3,
          model: "donor_country_end_date",
          type: "date",
          label: "GMT.DONORS.donor_country_end_date",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
          max: this.convertDateToString(new Date()),
        },
        {
          id: 4,
          model: "donor_country_description",
          type: "text",
          label: "GMT.DONORS.donor_country_description",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },
      ],
      [
        {
          id: 1,
          model: "guid_country_id",
          type: "select",
          label: "pme.individual.country",
          required: true,
          options: localThis.confs.find((ele) => ele.routeName == "Country")
            .data,
          widthPercentage: 40,
          marginRightPercentage: 5,
          onChange: this.handleCountryGuidChange,
        },
        {
          id: 2,
          model: "guid_state_id",
          type: "select",
          label: "pme.individual.state",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          disabled: true,
          onChange: this.handleStateGuidChange,
        },
        {
          id: 3,
          model: "guid_city_id",
          type: "select",
          label: "pme.individual.city",
          required: true,
          options: [],
          widthPercentage: 40,
          marginRightPercentage: 5,
          disabled: true,
        },
        {
          id: 4,
          model: "donor_address",
          type: "text",
          label: "GMT.DONORS.donor_address",
          required: true,
          widthPercentage: 40,
          marginRightPercentage: 10,
        },

      ],
    ];
    const columns = [
      [],
      [
        {
          id: 0,
          label: "Guid_donor_contact",
          field: "guid_donor_contact_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "Guid",
          field: "guid_donor_id",
          hide: true,
        },
        {
          id: 2,
          label: "GMT.DONORS.contact_type",
          field: "contact_type",
        },
        {
          id: 3,
          label: "GMT.DONORS.donor_contact",
          field: "donor_contact",
        },
        {
          id: 4,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],
      [
        {
          id: 0,
          label: "guid_donor_country_id",
          field: "guid_donor_country_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "Guid",
          field: "guid_donor_id",
          hide: true,
        },
        {
          id: 2,
          label: "GMT.DONORS.JRS_region_code",
          field: "jrS_region_code",
        },
        {
          id:3,
          label: "GMT.DONORS.country_name",
          field: "country_name",
        },
        
        {
          id: 4,
          label: "Actions",
          field: "actions",
          actions: true,
          hideEdit: true,
        },
      ],
      [
        {
          id: 0,
          label: "guid_donor_address_id",
          field: "guid_donor_address_id",
          primary: true,
          hide: true,
        },
        {
          id: 1,
          label: "GMT.DONORS.country_name",
          field: "country_name",
        },
        {
          id: 2,
          label: "GMT.DONORS.country_code",
          field: "country_code",
        },
        {
          id: 3,
          label: "GMT.DONORS.state_name",
          field: "state_name",
        },

        {
          id: 4,
          label: "GMT.DONORS.city_name",
          field: "city_name",
        },

        {
          id: 5,
          label: "GMT.DONORS.donor_address",
          field: "donor_address",
        },
        {
          id: 6,
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
        name: "Donor Info",
        // getData: this.GetDonorInfo,
        theData: theData[0],
        inputs: inputs[0],
        grid: false,
        routeGet: "Donor/GetDonnorByGuid",
        bodyGet: { guid_donor_id: this.id },
        submitRoute: "Donor/Donor_Update",

        saveDisabled: true,
      },
      {
        id: 1,
        name: "Donor Contact",
        theData: theData[1],
        inputs: inputs[1],
        grid: true,
        routeGet: "Donor/GetDonnorContactByGuid",
        bodyGet: { guid_donor_id: this.id },
        gridData: [],
        gridColumns: this.columns[1],
        submitRoute: "Donor/DonorContact_Insert",

        saveDisabled: true,
        routeDelete: "Donor/DonorContact_Delete",
      },
      {
        id: 2,
        name: "Donated countries",
        theData: theData[2],
        inputs: inputs[2],
        grid: true,
        routeGet: "Donor/GetDonnorCountryByGuid",
        bodyGet: { guid_donor_id: this.id },
        gridData: [],
        gridColumns: this.columns[2],
        submitRoute: "Donor/DonorCountry_Insert",

        saveDisabled: true,

        routeDelete: "Donor/DonorCountry_Delete",
      },
      {
        id: 3,
        name: "Donor Address",
        theData: theData[3],
        inputs: inputs[3],
        grid: true,
        routeGet: "Donor/GetDonnorAddressByGuid",
        bodyGet: { guid_donor_id: this.id },
        gridData: [],
        gridColumns: this.columns[3],
        submitRoute: "Donor/DonorAddress_Insert",
        saveDisabled: true,
        routeDelete: "Donor/DonorAddress_Delete",
      },
    ];
  },

  watch: {
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
      },
    },
  },


  
};
</script>

<style scoped>
@import "./Edit.scss";
</style>
