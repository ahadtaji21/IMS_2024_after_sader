<template>
    <wizard-comp :steps="steps"
                 :inputs="inputs"
                 :theData="theData"
                 :submit="submit"
                 previewTitle="Staff Form Preview" />
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    // import Wizard from "../../../../components/Inputs/Wizard/Wizard.vue";
    import Wizard from "@/components/Inputs/Wizard/Wizard.vue";
    export default {
        components: {
            "wizard-comp": Wizard,
        },
        data() {
            return {
                steps: [],
                inputs: [],
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
            async submit() {
                const filteredBody = {};
                const body = this.theData;
                for (const key in body) {
                    const val = body[key];
                    if (key == "guid_special_need_type_id") {
                        const obj = val
                            .split(",")
                            .filter((ele) => ele != "")
                            .map((ele) => ({ guid_special_need_type_id: ele }));

                        filteredBody[key] = JSON.stringify(obj);
                    } else if (
                        key == "familyDocumentType" ||
                        key == "individualDocumentType"
                    ) {
                        const ar = JSON.parse(val);
                        let newVal = ar.map((ele) => ({
                            guid_document_type_id: ele.id,
                            document_number: ele.value,
                        }));
                        filteredBody[key] = JSON.stringify(newVal);
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
                        name: "Staff/Staff_Add",
                        body: filteredBody,
                        post: true,
                    });
                    this.showNewSnackbar({
                        typeName: "success",
                        text: "Completed successfully",
                    });
                    // this.$router.replace({ name: "individual" });
                } catch (ex) {
                    this.showNewSnackbar({ typeName: "error", text: ex });
                }
            },
            nextStep() {
                if (this.currentStep < 7) {
                    this.currentStep = parseInt(this.currentStep) + 1;
                }
            },
            prevStep() {
                if (this.currentStep > 1) {
                    this.currentStep = parseInt(this.currentStep) - 1;
                }
            },
            stepClick(st) {
                if (st > 0 && st < 7 && st < this.currentStep) {
                    this.currentStep = parseInt(st);
                }
            },
            getStepClass(step, currentStep) {
                if (step.id < currentStep) {
                    return ["progress-step-done"];
                } else if (step.id === currentStep) {
                    return ["progress-step-active"];
                } else {
                    return ["progress-step-undone"];
                }
            },
            resetWizard() {
                this.currentStep = 1;
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
                {
                    id: 14,
                    routeName: "BloodType",
                    idName: "blood_type_id",
                    labelName: "blood_type",
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
                    name: "Personal Information",
                    icon: "mdi-account",
                    nextDisabled: true,
                    validateInputs: true,
                },
                {
                    id: 2,
                    name: "Staff Document",
                    icon: "mdi-file",
                    // nextlabel: "Skip",
                    nextDisabled: true,
                    validateInputs: true,
                },
                
                {
                    id: 3,
                    name: "Nationality Information",
                    icon: "mdi-passport",
                    nextDisabled: true,
                    validateInputs: true,
                },
                {
                    id: 4,
                    name: "Address Information",
                    icon: "mdi-map-marker",
                    nextDisabled: true,
                    validateInputs: true,
                },
                {
                    id: 5,
                    name: "Contact Information ",
                    icon: "mdi-card-account-phone",
                    nextDisabled: true,
                    validateInputs: true,
                },
                {
                    id: 6,
                    name: "Data Section",
                    icon: "mdi-database-outline",
                    // nextlabel: "Skip",
                    nextDisabled: true,
                    validateInputs: true,
                },
                //{
                //    id: 6,
                //    name: "Staff Document",
                //    icon: "mdi-file",
                //    // nextlabel: "Skip",
                //    nextDisabled: true,
                //    validateInputs: true,
                //},
                {
                    id: 7,
                    name: "Educational Level",
                    icon: "mdi-school",
                    // nextlabel: "Skip",
                    nextDisabled: true,
                    validateInputs: true,
                },
                {
                    id: 8,
                    name: "Marital Status",
                    icon: " mdi-human-male-female",
                    // nextlabel: "Skip",
                    nextDisabled: true,
                    validateInputs: true,
                },
                {
                    id: 9,
                    name: "Preview data",
                    icon: "mdi-eye",
                },
            ];
            this.inputs = [
                {
                    id: 1,
                    step: 1,
                    model: "staff_first_name",
                    type: "text",
                    label: "HRM.STAFF.staff_first_name",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 2,
                    step: 1,
                    model: "staff_first_name_native",
                    type: "text",
                    label: "HRM.STAFF.staff_first_name_native",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 3,
                    step: 1,
                    model: "staff_last_name",
                    type: "text",
                    label: "HRM.STAFF.staff_last_name",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 4,
                    step: 1,
                    model: "staff_last_name_native",
                    type: "text",
                    label: "HRM.STAFF.staff_last_name_native",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 5,
                    step: 1,
                    model: "staff_father_name",
                    type: "text",
                    label: "HRM.STAFF.staff_father_name",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 6,
                    step: 1,
                    model: "staff_mother_name",
                    type: "text",
                    label: "HRM.STAFF.staff_mother_name",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 7,
                    step: 1,
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
                    id: 8,
                    step: 1,
                    model: "staff_date_of_birth",
                    type: "date",
                    label: "HRM.STAFF.staff_date_of_birth",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                    //max: this.convertDateToString(new Date()),
                      max: this.calculateMinimumDate(),
                },
                {
                    id: 800,
                    step: 1,
                    model: "staff_place_of_birth",
                    type: "text",
                    label: "HRM.STAFF.staff_place_of_birth",
                    max: this.convertDateToString(new Date()),
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 9,
                    step: 1,
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
                    id: 10,
                    step: 1,
                    model: "jrs_entry_date",
                    type: "date",
                    label: "HRM.STAFF.jrs_entry_date",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                    //max: this.convertDateToString(new Date()),
                },
                //{
                //    id: 11,
                //    step: 1,
                //    model: "staff_code",
                //    type: "text",
                //    label: "HRM.STAFF.staff_code",
                //    required: true,
                //   // disabled: true,
                //    widthPercentage: 40,
                //    marginRightPercentage: 10,
                //},
                {
                    id: 12,
                    step: 1,
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
                    id: 13,
                    step: 1,
                    model: "is_religious",
                    //type: "text",
                    label: "HRM.STAFF.is_religious",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    // required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,

                },
                {
                    id: 14,
                    step: 1,
                    model: "is_active",
                    label: "HRM.STAFF.is_active",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 40,
                    marginRightPercentage: 10,

                },
                     {
                     id: 16,
                      step: 1,
                     model: "document",
                     type: "image",
                     label: "Profile Picture",
                     required: false,
                     widthPercentage: 60,
                     marginRightPercentage: 10,
                 },
                //--------------------->step 2 start
                {
                    id: 16,
                    step: 2,
                    model: "guid_country_id",
                    label: "HRM.STAFF.guid_country_id",
                    type: "select",
                    required: true,
                    options: localThis.confs.find((ele) => ele.routeName == "Country")
                        .data,
                    /*onChange: this.handleCountryGuidChange,*/
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 17,
                    step: 2,
                    model: "is_native",
                    label: "HRM.STAFF.is_native",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 60,
                    marginRightPercentage: 5,
                },
                //--------------------->step 3 start
                {
                    id: 14,
                    step: 3,
                    model: "guid_country_id",
                    label: "HRM.STAFF.country",
                    type: "select",
                    required: true,
                    options: localThis.confs.find((ele) => ele.routeName == "Country").data,
                    widthPercentage: 40,
                    marginRightPercentage: 5,
                    
                },
                {
                    id: 15,
                    step: 3,
                    model: "guid_state_id",
                    label: "HRM.STAFF.state",
                    type: "select",
                    required: true,
                    options: [],
                    widthPercentage: 40,
                    marginRightPercentage: 5,
                    disabled: true,
                   
                },
                {
                    id: 18,
                    step: 3,
                    model: "guid_city_id",
                    label: "HRM.STAFF.city",
                    type: "select",
                    required: true,
                    options: [],
                    widthPercentage: 40,
                    marginRightPercentage: 5,
                    disabled: true,
                },
                {
                    id: 19,
                    step: 3,
                    model: "guid_neighborhood_id",
                    label: "HRM.STAFF.neighborhood",
                    type: "select",
                    required: false,
                    options: [],
                    widthPercentage: 40,
                    marginRightPercentage: 5,
                    disabled: true,
                },
               
                {
                    id: 20,
                    step: 3,
                    label: "HRM.STAFF.staff_address_details",
                    field: "staff_address_details",
                    type: "text",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 5,
                },
                {
                    id: 21,
                    step: 3,
                    model: "is_active",
                    label: "HRM.STAFF.is_active",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },

                //--------------------->step 4 start
                {
                    id: 22,
                    step: 4,
                    model: "guid_contact_type_id",
                    label: "HRM.STAFF.guid_contact_type_id",
                    field: "guid_contact_type_id",
                    type: "select",
                    required: true,
                    options: localThis.confs.find(
                        (ele) => ele.routeName == "EducationalLevel"
                    ).data,
                    widthPercentage: 100,
                    marginRightPercentage: 0,
                },
                {
                    id: 23,
                    step: 4,
                    model: "staff_contact_values",
                    label: "HRM.STAFF.staff_contact_values",
                    field: "staff_contact_values",
                    type: "text",
                    required: true,
                    widthPercentage: 100,
                    marginRightPercentage: 0,
                },

                {
                    id: 26,
                    step: 4,
                    model: "is_active",
                    label: "HRM.STAFF.is_active",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                //--------------------->step 5 start
                {
                    id: 27,
                    step: 5,
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
                    id: 28,
                    step: 5,
                    model: "start_date",
                    type: "date",
                    label: "HRM.STAFF.start_date",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                    //max: this.convertDateToString(new Date()),
                },

                {
                    id: 29,
                    step: 5,
                    model: "end_date",
                    type: "date",
                    label: "HRM.STAFF.end_date",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                    //max: this.convertDateToString(new Date()),
                },
                {
                    id: 30,
                    step: 5,
                    model: "objective_code",
                    type: "text",
                    label: "HRM.STAFF.objective_code",
                    required: true,
                    disabled: true,
                    readonly: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                //--------------------->step 6 start
                {
                    id: 31,
                    step: 6,
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
                    widthPercentage: 100,
                    marginRightPercentage: 0,
                },
                {
                    id: 32,
                    step: 6,
                    model: "start_date",
                    type: "date",
                    label: "HRM.STAFF.start_date",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                    //max: this.convertDateToString(new Date()),
                },
                {
                    id: 33,
                    step: 6,
                    model: "end_date",
                    type: "date",
                    label: "HRM.STAFF.end_date",
                    required: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                    //max: this.convertDateToString(new Date()),
                },
                {
                    id: 34,
                    step: 6,
                    model: "document_number",
                    type: "text",
                    label: "HRM.STAFF.document_number",
                    // required: true,
                    // disabled: true,
                    // readonly: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 35,
                    step: 6,
                    model: "document_notes",
                    type: "text",
                    label: "HRM.STAFF.document_notes",
                    // required: true,
                    // disabled: true,
                    // readonly: true,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },

                //{
                //    id: 36,
                //    step: 5,
                //    widthPercentage: 90,
                //    marginRightPercentage: 0,
                //    type: "checkbox",
                //    required: false,
                //    hint: "Not required",
                //    model: "guid_special_need_type_id",
                //    label: "pme.individual.specialNeeds",
                //    options: newConfs.find((ele) => ele.routeName == "SpecialNeed").data,
                //},

                {
                    id: 37,
                    step: 6,
                    model: "is_active",
                    label: "HRM.STAFF.is_active",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                //--------------------->step 7 start
                {
                    id: 38,
                    step: 7,
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
                {
                    id: 39,
                    step: 7,
                    model: "education_level_description",
                    type: "text",
                    label: "HRM.STAFF.educationLvelTxt",
                    required: true,
                    widthPercentage: 100,
                    marginRightPercentage: 0,
                },
                //{
                //    id: 40,
                //    step: 8,
                //    //    model: "start_date",
                //    //    type: "date",
                //    //    label: "HRM.STAFF.start_date",
                //    //    required: true,
                //    //    widthPercentage: 40,
                //    //    marginRightPercentage: 10,
                //    //    //max: this.convertDateToString(new Date()),
                //},
                //{
                //    id: 41,
                //    step: 8,
                //    //    model: "end_date",
                //    //    type: "date",
                //    //    label: "HRM.STAFF.end_date",
                //    //    required: true,
                //    //    widthPercentage: 40,
                //    //    marginRightPercentage: 10,
                //    //    //max: this.convertDateToString(new Date()),
                //    //},
                //},
                {
                    id: 42,
                    step: 7,
                    model: "is_active",
                    label: "HRM.STAFF.is_active",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                //--------------------->step 8 start
                {
                    id: 43,
                    step: 8,
                    model: "guid_marital_status_id",
                    type: "select",
                    label: "HRM.STAFF.maritalStatus",
                    required: true,
                    options: localThis.confs.find(
                        (ele) => ele.routeName == "MaritalStatus"
                    ).data,
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
                {
                    id: 43,
                    step: 8,
                    model: "is_active",
                    label: "HRM.STAFF.is_active",
                    type: "radio",
                    required: false,
                    options: [
                        { id: "false", label: "No" },
                        { id: "true", label: "Yes" },
                    ],
                    widthPercentage: 40,
                    marginRightPercentage: 10,
                },
            ];
        },
        watch: {
            "theData.guid_country_id": async function (newValue) {
                const co = this.confs
                    .find((ele) => ele.routeName == "Country")
                    .data.find((ele) => ele.id == newValue);
                //if (co) {
                //    const phone_code = co.phone_code;
                //    this.inputs.find((ele) => ele.model == "mobileNumber").countryCode =
                //        "+" + phone_code;
                //    this.inputs.find((ele) => ele.model == "mobileNumber2").countryCode =
                //        "+" + phone_code;
                //}

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
