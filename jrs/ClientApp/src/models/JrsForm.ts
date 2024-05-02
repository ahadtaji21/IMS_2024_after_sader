/**
 * Enumeration of all possible field types. 
 */
export enum fieldType {
    text = 'text',
    select = 'select',
    checkbox = 'checkbox',
    date = 'date',
    text_edit = 'text_edit',
    auto = 'auto',
    location = 'location',
    search_table = 'search_table',
    password = 'password',
    number = 'number',
    search_table_multi = 'search_table_multi',
    time = 'time',
    combobox = 'combobox',
    combobox_multi = 'combobox_multi',
    radio = 'radio',
    radio_multi = 'radio_multi',
    radio_matrix = 'radio_matrix',
}

/**
 * @param type - defines the type of field, currently: 'text' | 'select' | 'checkbox'
 * @param name - name of the field to update
 * @param label - default lable value for the field
 * @param labelTranslationKey - translation key for translation of the label
 * @param counter - displays a counter of available characters
 * @param hint - displays a hint under the field
 * @param hintTranslationKey - translation key for translation of the label hint
 * @param readonly - set field to readonly
 * @param tabCode - Defines form tab for field
 * @param tabTranslationKey - translation key for form tab
 * @param form_order - ordinal position of field in form
 * @param additional_config - JSON array of addition configurations
 */
export interface JrsFormField {
    type: fieldType,
    name: string,
    label: string,
    labelTranslationKey?: string,
    is_required?: boolean,
    hint?: string,
    hintTranslationKey?: string,
    readonly?: boolean,
    tabCode?: string,
    tabLabel?: string,
    tabTranslationKey?: string,
    form_order?: number,
    additional_config?: any
    messages?: string[],
    messageTranslationKeys?: string[],
    rules?: Array<Function>,
    matrix_code?:string,
    matrix_row?:number,
    matrix_column?:number
}

/**
 * Text and number input types
 * @param counter - displays a counter of available characters
 */
export interface JrsFormFieldText extends JrsFormField {
    counter?: number
}

/**
 * Select and multiselct input types
 * @param selectItems - list of items from which to select
 * @param lookupName - name of property holding the selected object
 * @param itemText - name of descriptive property in selectItems
 * @param itemKey - name of value property in selectItems
 * @param multiple - enable multiple selection 
 */
export interface JrsFormFieldSelect extends JrsFormField {
    selectItems: Array<any>,
    lookupName?: string,
    itemText: string,
    itemKey: string,
    dataSource?: string,
    dataSourceCondition?: string,
    dataSourceOrder?: string,
    multiple?: Boolean,
    sendOnlyKey?: Boolean
}

/**
 * Search table field type.
 * @param dataSource - Name of the data-source to query
 * @param itemText - Name of text field of the data-source
 * @param itemKey - Name of key field of the data-source
 * @param dataSourceCondition - Condition for query
 * @param dataSourceOrder - Ordering for query
 */
export interface JrsFormFieldSearch extends JrsFormField {
    dataSource: String
    itemText: String,
    itemKey: String,
    dataSourceCondition?: String,
    dataSourceOrder?: String,
}

/**
 * Search multi table field type.
 * @param dataSource - Name of the data-source to query
 * @param itemText - Name of text field of the data-source
 * @param itemKey - Name of key field of the data-source
 * @param dataSourceCondition - Condition for query
 * @param dataSourceOrder - Ordering for query
 * @param returnOnlyIds - Return Array with only key from table
 */
export interface JrsFormFieldSearchMulti extends JrsFormFieldSearch {
    returnOnlyIds?: Boolean
}


/**
 * Definition of a form tab.
 * @param tabCode - Code of the tab
 * @param tabName - Name of the tab
 */
export interface JrsFormTab {
    tabCode: String,
    tabName: String
    index?: Number
}

/**
 * Definition of a form tab.
 * @param question - question
 * @param answer - answer
 * @param idQuestion - idQuestion
 * @param questionType - questionType
 */
export interface JrsFormFieldQuestion extends JrsFormField {
    question: String,
    answer: String,
    idQuestion: String,
    questionType: String
}

