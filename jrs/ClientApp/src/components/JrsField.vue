<template>
    <div>
        <!-- TEXT FIELD -->
        <v-text-field
            :name="field.name"
            :label="field.label"
            :id="field.name"
            v-model="fieldValue"
            :hint="field.hint ? field.hint : ''"
            :persistent-hint="field.hint ? true : false"
            :required="field.is_required ? field.is_required : false"
            :rules="field.rules"
            :counter="textCounterValue"
            :disabled="field.readonly"
            v-if="field.type == 'text'"
            :clearable='clearebleComputed'
            :messages="field.messages"
        ></v-text-field>

        <!-- SELECT -->
        <v-select
            :label="field.label"
            :id="field.lookupName"
            v-model="fieldValue"
            :hint="field.hint ? field.hint : ''"
            :persistent-hint="field.hint ? true : false"
            :required="field.is_required ? field.is_required : false"
            :rules="field.is_required ? requiredTextFieldRule : []"
            :items="field.selectItems"
            :item-value="field.itemKey"
            :item-text="field.itemText"
            :multiple="field.multiple"
            :chips="field.multiple"
            :return-object="field.sendOnlyKey ? false : true"
            :disabled="field.readonly"
            v-if="field.type == 'select'"
            :messages="field.messages"
        >
            <template v-slot:append-outer>
                <v-icon @click="fieldValue=undefined">mdi-close</v-icon>
            </template>
        </v-select>

        <!-- AUTOCOMPLETE -->
        <v-autocomplete
            :label="field.label"
            :id="field.lookupName"
            v-model="fieldValue"
            :hint="field.hint ? field.hint : ''"
            :persistent-hint="field.hint ? true : false"
            :required="field.is_required ? field.is_required : false"
            :rules="field.is_required ? requiredTextFieldRule : []"
            :items="field.selectItems"
            :item-value="field.itemKey"
            :item-text="field.itemText"
            :multiple="field.multiple"
            :chips="field.multiple"
            :return-object="field.sendOnlyKey ? false : true"
            :disabled="field.readonly"
            v-if="field.type == 'auto'"
            :messages="field.messages"
        ></v-autocomplete>

        <!-- COMBOBOX -->
        <jrs-combobox 
            v-if="field.type == 'combobox_multi'"
            v-model="fieldValue"
            :label="field.label"
            :hint="field.hint ? field.hint : ''"
            :required="field.is_required ? field.is_required : false"
            :items="field.selectItems"
            :item-value="field.itemKey"
            :item-text="field.itemText"
            :multiple="field.type == 'combobox_multi'"
        ></jrs-combobox>
        <v-combobox
            :label="field.label"
            :id="field.lookupName"
            :ref="`field_${field.name}`"
            v-model="tmpAutocompleteValue"
            :hint="field.hint ? field.hint : ''"
            :persistent-hint="field.hint ? true : false"
            :required="field.is_required ? field.is_required : false"
            :rules="field.is_required ? requiredTextFieldRule : []"
            :items="field.selectItems"
            :item-value="field.itemKey"
            :item-text="field.itemText"
            v-if="field.type == 'combobox'"
            :messages="field.messages"
            @change="onAutocompleteSelection"
            @keyup="OnChangeHandlerDebouncedFunction"
            @paste="OnChangeHandlerDebouncedFunction"
        ></v-combobox>

        <!-- CHECKBOX -->
        <v-checkbox
            :label="field.label"
            :id="field.name"
            v-model="fieldValue"
            :readonly="field.readonly"
            v-if="field.type == 'checkbox'"
        ></v-checkbox>

        <!-- DATE-PICKER -->
        <jrs-date-picker
            :label="field.label"
            v-model="fieldValue"
            :hint="field.hint ? field.hint : ''"
            :is_required="field.is_required ? field.is_required : false"
            v-if="field.type == 'date'"
            :readonly="field.readonly"
            :is_only_year="returnIsOnlyYearDataPicker"
        ></jrs-date-picker>

        <!-- TEXT_EDIT -->
        <div v-if="field.type == 'text_edit'">
            <text-editor
                v-model="fieldValue"
                :readonly="field.readonly"
                :label="field.label"
                :hint="field.hint ? field.hint : ''"
                :fieldKey="field.name"
            ></text-editor>
        </div>

        <!-- LOCATION -->
        <jrs-location-picker
            :label="field.label"
            v-model="fieldValue"
            :hint="field.hint"
            :required="field.is_required ? field.is_required : false"
            v-if="field.type == 'location'"
        ></jrs-location-picker>

        <!-- SEARCH TABLE -->
        <jrs-search-table
            v-model="fieldValue"
            :label="field.label"
            :dataSource="field.dataSource"
            :itemValue="field.itemKey"
            :itemText="field.itemText"
            :dataSourceCondition="field.dataSourceCondition"
            :dataSourceOrder="field.dataSourceOrder"
            :required="field.is_required ? field.is_required : false"
            :rowDisableSelectRules="selectDisablePropName"
            v-if="field.type == 'search_table'"
            :messages="field.messages"
            :isDisabled="field.readonly"
            :ignoreOfficeFilter="ignoreOfficeFilter"
        ></jrs-search-table>

        <!-- PASSWORD -->
        <v-text-field
            :name="field.name"
            :label="field.label"
            :id="field.name"
            v-model="fieldValue"
            :hint="field.hint ? field.hint : ''"
            :persistent-hint="field.hint ? true : false"
            :required="field.is_required ? field.is_required : false"
            :rules="field.is_required ? requiredTextFieldRule : []"
            :counter="textCounterValue"
            :readonly="field.readonly"
            :append-icon="showPasswords ? 'mdi-eye' : 'mdi-eye-off'"
            @click:append="showPasswords = !showPasswords"
            :type="!showPasswords ? 'password' : 'text'"
            v-if="field.type == 'password'"
        ></v-text-field>

        <!-- NUMBER -->
        <jrs-input-nbr
            :label="field.label"
            v-model="fieldValue"
            :hint="field.hint"
            :is_required="field.is_required ? field.is_required : false"
            v-if="field.type == 'number'"
            :decimalPrecision="numberDecimalPrecision"
        ></jrs-input-nbr>

        <!--MULTI SELECT-->
        <jrs-multi-select-table
            v-model="fieldValue"
            :label="field.label"
            :dataSource="field.dataSource"
            :itemValue="field.itemKey"
            :itemText="field.itemText"
            :dataSourceCondition="field.dataSourceCondition"
            :dataSourceOrder="field.dataSourceOrder"
            :required="field.is_required ? field.is_required : false"
            :returnOnlyIds="returnOnlyIds"
            :rowDisableSelectRules="selectDisablePropName"
            v-if="field.type == 'search_table_multi'"
            :messages="field.messages"
            :displayChipsColumn="
                field.additional_config
                    ? field.additional_config.displayChipsColumn
                    : false
            "
            :ignoreOfficeFilter="ignoreOfficeFilter"
        ></jrs-multi-select-table>

        <!--TIME--->
        <v-dialog
            ref="dialog"
            v-model="modalTimeField"
            :return-value.sync="fieldValue"
            v-if="field.type == 'time'"
            persistent
            width="350px"
        >
            <template v-slot:activator="{ on, attrs }">
                <v-text-field
                    v-model="fieldValue"
                    :label="field.label"
                    prepend-icon="mdi-clock-time-four-outline"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                ></v-text-field>
            </template>
            <v-time-picker
                v-if="modalTimeField"
                v-model="fieldValue"
                full-width
            >
                <v-spacer></v-spacer>
                <v-btn
                    color="secondary darken-1"
                    text
                    @click="modalTimeField = false"
                    >X {{ $t("general.close") }}</v-btn
                >
                <v-btn
                    text
                    color="primary"
                    @click="$refs.dialog.save(fieldValue)"
                    >{{ $t("general.save") }}</v-btn
                >
            </v-time-picker>
        </v-dialog>

        <!--SLIDE (with Min and Max)-->
        <jrs-slider
            v-model="fieldValue"
            :minValue="retunrMinSlide"
            :maxValue="returnMaxSlide"
            :label="field.label"
            v-if="field.type == 'slide'"
        ></jrs-slider>

        <!--Document Component to upload/download file to specific field-->

        <!-- TODO: GENERAL CONFIGURATION TO SAVE DOCUMENT WHERE WHEREVER YOU WANT: TABLE, COLUMN , ID, VALUE ID-->
        <!--:columnNameToSaveDocument="returncolumnNameToSaveDocument"-->
        <!--:tableNameToSaveDocument="returntableNameToSaveDocument"-->
        <!--:OfficeId="returnOfficeIdDocument"-->

        <jrs-document
            v-model="fieldValue"
            :acceptExtensions="returnacceptExtensions"
            :documentTypeId="returndocumentTypeIdDocument"
            :OfficeCode="returnOfficeCodeDocument"
            :FolderSaveDocument="returnFolderSaveDocument"
            :truncate_length="returnTruncatelengthDocument"
            :label="field.label"
            :is_required="field.is_required"
            @filechange="emitfilechange"
            v-if="field.type == 'document'"
        >
        </jrs-document>

        <!--Color Picker Component-->
        <jrs-color-picker
            v-model="fieldValue"
            :label="field.label"
            v-if="field.type == 'color_picker'"
        >
        </jrs-color-picker>

        <!-- RADIO BUTTONS -->
        <jrs-radio-buttons
            v-model="fieldValue"
            :label="field.label"
            :hint="field.hint"
            :items="field.selectItems"
            :itemKey="field.itemKey"
            :itemText="field.itemText"
            :readonly="field.readonly"
            :is_required="field.is_required"
            :multiple="field.type == 'radio_multi' ? true : false"
            v-if="(['radio', 'radio_multi']).includes(field.type)"
        ></jrs-radio-buttons>

                <!-- RADIO BUTTONS MATRIX-->
        <jrs-radio-buttons-matrix
            v-model="fieldValue"
            :label="field.label"
            :hint="field.hint"
            :items="field.selectItems"
            :itemKey="field.itemKey"
            :itemText="field.itemText"
            :readonly="field.readonly"
            :is_required="field.is_required"
            :multiple="false"
            v-if="(['radio_matrix']).includes(field.type)"
        ></jrs-radio-buttons-matrix>

        <!--Image Picker Component ( To manage photo or other image) -->
        <!--TODO: change compoennt vuetify-image-input with other component (problem qith otehr packages)-->
        <!-- <jrs-img-picker
            v-model="fieldValue"
            :label="field.label"
            v-if="field.type == 'image_picker'"
        >
        </jrs-img-picker>-->
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import { fieldType, JrsFormField } from "../models/JrsForm";
    // import VueEditor from "@tinymce/tinymce-vue";
    import JrsDatePicker from "../components/JrsDatePicker.vue";
    import JrsLocationPicker from "../components/JrsLocationPicker.vue";
    import JrsSearchTable from "../components/JrsSearchTable.vue";
    import JrsInputNbr from "../components/JrsInputNbr.vue";
    import JrsDocument from "../components/JrsDocuments.vue";
    import { translate } from "../i18n";
    import JrsMultiSelectTable from "../components/JrsMultiSelectedTable.vue";
    import JrsSlider from "../components/JrsSlider.vue";
    import TestEditod from "../components/TextEditor.vue";
    import JrsColorPicker from "../components/JrsColorPicker.vue";
    import JrsRadioButtons from "../components/JrsRadioButtons.vue";
    import JrsRadioButtonsMatrix from "../components/JrsRadioButtonsMatrix.vue";
    import JrsCombobox from "../components/JrsCombobox.vue";
    import UtilMix from "../mixins/UtilMix";
    //import JrsImagePicker from "../components/JrsImagePicker.vue";

    export default Vue.extend({
        components: {
            "jrs-date-picker": JrsDatePicker,
            // "vue-editor": VueEditor,
            "jrs-location-picker": JrsLocationPicker,
            "jrs-search-table": JrsSearchTable,
            "jrs-input-nbr": JrsInputNbr,
            "jrs-multi-select-table": JrsMultiSelectTable,
            "jrs-slider": JrsSlider,
            "jrs-document": JrsDocument,
            "text-editor": TestEditod,
            "jrs-color-picker": JrsColorPicker,
            "jrs-radio-buttons": JrsRadioButtons,
            "jrs-radio-buttons-matrix": JrsRadioButtonsMatrix,
            "jrs-combobox": JrsCombobox,
            //"jrs-img-picker": JrsImagePicker
        },
        props: {
            value: {
                type: [
                    String,
                    Number,
                    Boolean,
                    Array,
                    Object,
                    Date,
                    Function,
                    Symbol,
                ],
            },
            field: {
                //type: Object as () => JrsFormField,
                type: Object,
                required: true,
            },
            dynamicFullName:{
                type: Object,
                required: false,
            },
             /**
             * Ignore office filter in data query.
             */
            ignoreOfficeFilter: {
            type: Boolean,
            required: false,
            default: false,
            },
        },
        mixins: [UtilMix],

        data() {
            return {
                // fieldValue: null
                showPasswords: false,
                modalTimeField: false,
                modalSlideField: false,
                tmpAutocompleteValue: null,
                OnChangeHandlerDebouncedFunction: () => null,
                showField: true,
            };
        },
        computed: {
            fieldValue: {
                get() {
                    if((this as any).dynamicFullName){
                    if((this as any).field.name == (this as any).dynamicFullName.FULL_NAME_COLUMN_NAME){ 
                        let name =  (this as any).dynamicFullName.fullValueFields[(this as any).dynamicFullName.NAME_COLUMN_NAME]? (this as any).dynamicFullName.fullValueFields[(this as any).dynamicFullName.NAME_COLUMN_NAME] +' ' :''
                        let surname =  (this as any).dynamicFullName.fullValueFields[(this as any).dynamicFullName.SURNAME_COLUMN_NAME]? (this as any).dynamicFullName.fullValueFields[(this as any).dynamicFullName.SURNAME_COLUMN_NAME].toUpperCase() :''
                        let concatenateName = name+surname
                        if((this as any).value && (this as any).value != ''){
                        if((this as any).value != concatenateName){
                                return (this as any).value
                        }else{
                                return concatenateName
                        }                           
                        }else{
                            return concatenateName
                        }
                    }else{
                        return (this as any).value;
                    }
                    }else{
                          return (this as any).value;
                    }
                },
                set(newVal: any) {
                    //(this as any).fiedlValue = newVal;
                    (this as any).emitValueInput(newVal);
                },
            },
            requiredTextFieldRule() {
                return [(v: any) => !!v || translate("error.required-field")];
            },
            /**
             * Number of decimal values allowed for Number type field.
             */
            numberDecimalPrecision() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};

                return fieldConfigs.decimalPrecision
                    ? fieldConfigs.decimalPrecision
                    : 0;
            },
            /**
             * Return a JSON of only Ids for JrsMultiSearch component.
             */
            returnOnlyIds() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};

                return fieldConfigs.returnOnlyIds
                    ? fieldConfigs.returnOnlyIds
                    : false;
            },

            /**
             * Return the Min for the Slide component.
             */
            retunrMinSlide() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.intervalValue.minValue
                    ? fieldConfigs.intervalValue.minValue
                    : 0;
            },

            /**
             * Return the Max for the Slide component.
             */
            returnMaxSlide() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.intervalValue.maxValue
                    ? fieldConfigs.intervalValue.maxValue
                    : 100;
            },

            /**
             * Return the acceptExtensions for the Document component.
             */
            returnacceptExtensions() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.AcceptExtensions
                    ? fieldConfigs.AcceptExtensions
                    : ".pdf, .xls, .xlsx, .doc, .docx, .jpg, .jpeg, .png, .gif";
            },

            /**
             * Return the table name to save the ID of document uploaded for the Document component.
             */

            returntableNameToSaveDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.TableNameToSaveDocument
                    ? fieldConfigs.TableNameToSaveDocument
                    : "";
            },

            /**
             * Return the folder to upload the document , for the Document component.
             */
            returnFolderSaveDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.FolderSaveDocument
                    ? fieldConfigs.FolderSaveDocument
                    : "General";
            },

            /**
             * Return the table name to save the ID of document uploaded for the Document component.
             */
            returncolumnNameToSaveDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.ColumnNameToSaveDocument
                    ? fieldConfigs.ColumnNameToSaveDocument
                    : "ST_DOCUMENT_ID";
            },

            /**
             * Return the Office Code to save document with specific office code for the Document component.
             */
            returnOfficeCodeDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.OfficeCode ? fieldConfigs.OfficeCode : null;
            },

            /**
             * Return the Office Id to save document with specific office id for the Document component.
             */
            returnOfficeIdDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.OfficeId ? fieldConfigs.OfficeId : null;
            },

            /**
             * Return the truncate lenght of text of document name for the Document component.
             */
            returnTruncatelengthDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.Truncatelength
                    ? fieldConfigs.Truncatelength
                    : "15";
            },

            /**
             * Return the Document Type ID for the Document component.
             */
            returndocumentTypeIdDocument() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.DocumentTypeId
                    ? fieldConfigs.DocumentTypeId
                    : 2;
            },

            /**
             * Return the Office Id to save document with specific office id for the Document component.
             */
            returnIsOnlyYearDataPicker() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                return fieldConfigs.OnlyYear ? fieldConfigs.OnlyYear : false;
            },

            /**
             * Number of character allowed in text and password type fields.
             */
            textCounterValue() {
                let localThis: any = this as any;
                let fieldConfigs = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};

                let fieldCounter: number = localThis.field.counter
                    ? localThis.field.counter
                    : undefined;
                let configCounter: number = fieldConfigs.counterValue
                    ? fieldConfigs.counterValue
                    : undefined;

                return fieldCounter || configCounter || false;
            },
            /**
             * Name of row property to disable selction.
             * Used by search_table and search_table_multi type fields.
             */
            selectDisablePropName(): Array<Function> {
                let localThis: any = this as any;
                if (
                    localThis.field.type != "search_table" ||
                    localThis.field.type != "search_table_multi"
                ) {
                    return [() => false];
                }
                let fieldConfigs: any = localThis.field.additional_config
                    ? localThis.field.additional_config
                    : {};
                let rules: Array<any> = [];
                rules = fieldConfigs.disableSelectionRules
                    ? [...fieldConfigs.disableSelectionRules]
                    : [];
                let ruleFunctions: Array<Function> = rules.map((rule: any) => {
                    let func: Function;
                    switch (rule.operator) {
                        case "=":
                            func = (item: any) =>
                                item[rule.propName] == rule.propValue;
                            break;
                        case ">":
                            func = (item: any) =>
                                item[rule.propName] > rule.propValue;
                            break;
                        case "<":
                            func = (item: any) =>
                                item[rule.propName] < rule.propValue;
                            break;
                        default:
                            func = (item: any) => null;
                            break;
                    }
                    return func;
                });

                // let tmp:any = ruleFunctions.reduce((lastVal, currVal) => lastVal && currVal);
                return ruleFunctions;
            },
            /**
             * Field value for Combobox Fields.
             */
            autocompleteValue: {
                get() {
                    const localThis: any = this as any;
                    if (localThis.field.type != "combobox") {
                        return localThis.value;
                    }
                    const itemValueName: string = localThis.field.itemKey;
                    const retValue: any = localThis.field.selectItems.find(
                        (item: any) => item[itemValueName] == localThis.value
                    );
                    return retValue
                        ? retValue
                        : {
                              [localThis.field.itemKey]: null,
                              [localThis.field.itemText]: localThis.value,
                          };
                },
                set(newVal: any) {
                    const localThis: any = this as any;
                    let emitValue: any = null;
                    if (newVal) {
                        // If the user hase selected an existing vlaue return the key otherwise the new text
                        emitValue = newVal[localThis.field.itemKey]
                            ? newVal[localThis.field.itemKey]
                            : newVal;
                    }
                    localThis.emitValueInput(emitValue);
                },
            },

            /**
             * clearable computed
             */
            clearebleComputed(){
                const localThis: any = this as any;
                return localThis.dynamicFullName ? localThis.dynamicFullName.FULL_NAME_COLUMN_NAME && localThis.field.name == localThis.dynamicFullName.FULL_NAME_COLUMN_NAME ?  true : false : false
            }
        },
        watch: {
            fieldValue(newVal: any, oldVal: any) {
                (this as any).emitValueUpdate(newVal);
            },
        },
        methods: {
            /**
             * Emit value changes to parent component.
             * Initial setup is included.
             * NOTE: For select/AutoSelect fields, full object is returned, key value must be calculated.
             */
            emitValueInput(newVal: any) {
                let localThis: any = this as any;

                // Not multi "select like" fields
                let isSelect =
                    localThis.field.type == "select" && !localThis.field.multiple;
                let isAutoSelect =
                    localThis.field.type == "auto" && !localThis.field.multiple;

                if (isSelect || isAutoSelect) {
                    (this as any).emitFullObject(newVal);
                    let selectValue: any = localThis.calcSelectValue(newVal);
                    localThis.$emit("input", selectValue);
                } else {
                    localThis.$emit("input", newVal);
                }
            },
            /**
             * Emit value update to parent component.
             * Initial setup is excluded.
             * NOTE: For select/AutoSelect fields, key values is already returned, no need to calculate.
             */
            emitValueUpdate(newVal: any) {
                let localThis: any = this as any;

                // // Not multi "select like" fields
                // let isSelect = localThis.field.type == 'select' && !localThis.field.multiple;
                // let isAutoSelect = localThis.field.type == 'auto' && !localThis.field.multiple;

                // if(isSelect || isAutoSelect){
                //     let selectValue:any = localThis.calcSelectValue(newVal);
                //     localThis.$emit("update", selectValue);
                // }else{
                //     localThis.$emit("update", newVal);
                // }
                localThis.$emit("update", newVal);
            },
            /**
             * Emit the full objet to parent component.
             */
            emitFullObject(obj: any) {
                let emitPayload: any = {
                    holderName: (this as any).field.lookupName,
                    objectValue: obj,
                };
                (this as any).$emit("changeFullObject", emitPayload);
            },
            /**
             * Return the select value recovedere from the full object.
             */
            calcSelectValue(a: any) {
                // (this as any).emitFullObject(a);
                return a ? a[(this as any).field.itemKey] : undefined;
            },

            emitfilechange(a: any) {
                let emitPayload: any = {
                    holderName: "fileHolder_" + (this as any).field.name,
                    objectValue: a,
                };
                (this as any).$emit("changeFullObject", emitPayload);
            },
            /**
             * Manage update of field model based on Autcomplete/Comobox selection.
             */
            onAutocompleteSelection() {
                const localThis: any = this as any;
                // Set field value quals selected list value
                localThis.autocompleteValue = localThis.tmpAutocompleteValue;
            },
            /**
             * Manage update of field model based on Autcomplete/Comobox string change.
             * The function must be debounced to only be triggered when user has eneded typing.
             */
            customOnChangeHandler(val: any) {
                const localThis: any = this as any;
                const fieldRef: string = `field_${localThis.field.name}`;
                if (localThis.$refs[fieldRef]) {
                    localThis.autocompleteValue =
                        localThis.$refs[fieldRef].internalSearch;
                }
            },
        },
        mounted() {
            const localThis: any = this as any;

            // Setup configuration for Combobox fields
            if (localThis.field.type == "combobox") {
                // Set debounced function for onChange event of Autocomplete/Combobox
                localThis.OnChangeHandlerDebouncedFunction =
                    localThis.getDebouncedFunc(
                        localThis.customOnChangeHandler,
                        1000
                    );
                // Set initial value for Autocomplete/Combobox field
                localThis.tmpAutocompleteValue = localThis.autocompleteValue;
            }
        },
    });
</script>


<style scoped>
</style>