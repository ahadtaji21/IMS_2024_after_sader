<!--
    To know how to use the component, please read the README file in WIKI called Excel-generator
    https://dev.azure.com/JRS-IO/IMS/_wiki/wikis/IMS.wiki/9/Excel-generator
-->

<template>
    <div :class="withoutHeader ? '' : 'header'">
        <div v-for="param in PROCEDURE_PARAMETERS" :key="param.id">
            <div v-if="!param.hide">
                <template v-if="param.type === 'text'">
                    <div class="input-wrapper">
                        <label :for="param.id">{{ param.label }}</label>
                        <input :placeholder="param.label"
                               type="text"
                               v-model="param.value" />
                    </div>
                </template>
                <template v-else-if="param.type === 'number'">
                    <div class="input-wrapper">
                        <label :for="param.id">{{ param.label }}</label>
                        <input :placeholder="param.label"
                               type="number"
                               v-model="param.value"
                               :max="param.max"
                               :min="param.min" />
                    </div>
                </template>

                <template v-else-if="param.type === 'date'">
                    <div class="date-picker-container">
                        <label class="date-label">{{ param.label }}</label>
                        <vue-datepicker v-model="param.value"
                                        wrapper-class="vue-datepicker-wrapper"
                                        :disabled-dates="param.disabledDates"
                                        clear-button></vue-datepicker>
                    </div>
                </template>

                <template v-else-if="param.type === 'select'">
                    <label :for="param.id">{{ param.label }}</label>
                    <select v-model="param.value">
                        <option v-for="option in param.options"
                                :key="option"
                                :value="option">
                            {{ option }}
                        </option>
                    </select>
                </template>

            </div>
        </div>

        <v-btn color="secondary lighten-4"
               class="grey--text text--darken-3 mx-1"
               small
               @click="handleExcelExport">
            Generate <v-icon>mdi-microsoft-excel</v-icon>
        </v-btn>
    </div>
</template>

<script>
    import { ref } from "vue";
    import UtilMix from "../mixins/UtilMix";

    import DatePicker from "vuejs-datepicker";
    import { mapActions, mapGetters } from "vuex";

    // const date = ref();

    export default {
        name: "Header",
        components: {
            "vue-datepicker": DatePicker,
        },
        props: {
            storedProcedureName: {
                type: String,
                required: true,
            },
            file_name: {
                type: String,
            },
            PROCEDURE_PARAMETERS: {
                type: [Array],
            },
            withoutHeader:{
                type: Boolean,
                default: false,
            }
        },
        mixins: [UtilMix],

        data() {

            return {
                disableButton: false,
            };
        },

        computed: {
            ...mapGetters({
                getUserUid: "getUserUid",
                getCurrentOffice: "getCurrentOffice",
            }),
        },
        methods: {
            ...mapActions({
                showNewSnackbar: "showNewSnackbar",
            }),
            ...mapActions("apiHandler", {
                generateExcelFromSP: "generateExcelFromSP",
            }),
            async handleExcelExport() {
                const localThis = this;
                let fileName = localThis.file_name ? this.file_name : "TMP_NAME";
               // const offc = await localThis.getOffice();
                //console.log("off: ", offc);
                
                const fileExtension = ".xlsx";

                const PROCEDURE_PARAMETERS = this.PROCEDURE_PARAMETERS.map(param => ({ name: param.key, value: param.value, type: param.type }))
                
                const PROCEDURE_NAME = localThis.storedProcedureName;

                localThis
                    .generateExcelFromSP({
                        PROCEDURE_PARAMETERS,
                        PROCEDURE_NAME,
                    })
                    .then((byteArray) => {
                        const byteBufffer = localThis._base64ToArrayBuffer(
                            byteArray,
                            fileExtension
                        );

                        localThis.saveByteArray(
                            fileName + fileExtension,
                            byteBufffer,
                            "vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        );
                    })
                    .catch((err) => {
                        console.log("error: ", err);
                        localThis.showNewSnackbar({ typeName: "error", text: err });
                    });

            },
        },
    };
</script>

<style>
    .header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 10px;
        border: 1px solid #ccc;
        border-radius: 4px;
        font-family: Arial, sans-serif;
    }

    .date-picker-container,
    .select-container,
    .input-wrapper {
        display: flex;
        /* align-items: center; */
        flex-direction: column;
        margin-bottom: 20px;
    }

        .date-label,
        .select-label,
        .input-wrapper label {
            margin-right: 4px;
            font-size: 14px;
            margin-bottom: 5px;
        }

        .vue-datepicker-wrapper,
        .select-input,
        .input-wrapper input[type="text"] {
            height: 40px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            outline: none;
            font-size: 16px;
            transition: border-color 0.3s ease-in-out;
        }

            .input-wrapper input[type="text"]:focus,
            .select-input:focus,
            .vue-datepicker-wrapper input:focus {
                border-color: #007bff;
            }

            .input-wrapper input[type="text"]::placeholder {
                color: #999;
            }
</style>
