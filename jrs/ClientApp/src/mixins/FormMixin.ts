import { GenericSqlSelectPayload } from "../axiosapi/api";
import { mapGetters, mapActions } from "vuex";
import { IapiBlobStorageUpload, IapiBlobStorageUploadOverwrite, IapiBlobStorageDownload } from "../store/apiHandler"
import { fieldType, JrsFormField, JrsFormFieldSearch, JrsFormFieldSelect, JrsFormTab } from "../models/JrsForm";
import { translate } from "../i18n"
export default {
  computed: {
    ...mapGetters({
      getLookupsByTypeCode: 'getLookupsByTypeCode',
      getCurrentOffice: 'getCurrentOffice',
      activeModule: "getActiveModule",
      getUserUid: "getUserUid"
    })
  },
  methods: {
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      uploadBlobStorageFile: "uploadBlobStorageFile",
      uploadOverwriteBlobStorageFile: "uploadOverwriteBlobStorageFile",
      downloadBlobStorageFile: "downloadBlobStorageFile"
    }),
    /**
     * Retrieve lookup values based on lookup-type code.
     * @param code code of lookup-type
     */
    retrieveLookupByTyepCode(code: string) {
      // return (this as any).$store.getters.getLookupsByTypeCode(code);
      const localThis = (this as any);
      return localThis.getLookupsByTypeCode(code);
    },
    /**
     * Converts an object recieved by API into a JrsFormField.
     * @param obj - object to try and cast to JrsFormField
     */
    parseJrsFormField(obj: any) {
      let field: any = {
        type: obj.type,
        name: obj.column_name,
        label: obj.text,
        labelTranslationKey: obj.translationtKey || obj.labelTranslationKey,
        is_required: obj.is_required,
        hint: obj.hint,
        hintTranslationKey: obj.hintTranslationKey,
        readonly: obj.readonly,
        tabCode: obj.tabCode,
        tabTranslationKey: obj.tabTranslationKey,
        form_order: obj.form_order,
        additional_config: obj.additional_config ? JSON.parse(obj.additional_config) : undefined,
        messageTranslationKeys: obj.messageTranslationKeys,
        messages: obj.messages,
        rules: null,
      }

      // Set rules
      field.rules = [...this.getFieldRules(field)];

      switch (obj.type) {

        case 'select':
        case 'auto':
          field.sendOnlyKey = obj.sendOnlyKey;
          field.lookupName = "selectHolder_" + obj.column_name;
          field.itemText = obj.itemText;
          field.itemKey = obj.itemKey;
          field.selectItems = [];
          field.multiple = obj.multiple;
          field.select_lookup_code = obj.select_lookup_code;
          field.dataSource = obj.select_items_datasource;
          field.dataSourceCondition = obj.dataSourceCondition;
          field.dataSourceOrder = obj.dataSourceOrder;
          break;
        case 'combobox':
        case 'combobox_multi':
        case 'radio':
        case 'radio_multi':
          field.lookupName = "selectHolder_" + obj.column_name;
          field.itemText = obj.itemText;
          field.itemKey = obj.itemKey;
          field.selectItems = [];
          field.multiple = obj.multiple;
          field.select_lookup_code = obj.select_lookup_code;
          field.dataSource = obj.select_items_datasource;
          field.dataSourceCondition = obj.dataSourceCondition;
          field.dataSourceOrder = obj.dataSourceOrder;
          break;
        case 'search_table':
        case 'search_table_multi':
          field.dataSource = obj.select_items_datasource;
          field.itemText = obj.itemText;
          field.itemKey = obj.itemKey;
          field.dataSourceCondition = obj.dataSourceCondition;
          field.dataSourceOrder = obj.dataSourceOrder;
          break;
        default:
          break;
      }

      return field;
    },
    /**
     * Setup field with items for select type fields and return promise with resulting field.
     * @param field Fields to setup as a select
     * @param currentOfficeId ID of the currently selected office to apply as filter to select datasource.
     * @param fieldDatasourceConditions Array of conditions to apply to field. conditions must have the form {field_name:string, field_condition:string}
     */
    async setupSelectFieldAsync(field: any, currentOfficeId?: number, fieldDatasourceConditions?: Array<any>): Promise<JrsFormFieldSelect | JrsFormFieldSearch> {
      const localThis: any = this as any;
      const searchTableFieldTypes: fieldType[] = [
        fieldType.search_table,
        fieldType.search_table_multi,
      ];
      // Get select item list
      if (field.select_lookup_code) {
        field.selectItems = this.retrieveLookupByTyepCode(field.select_lookup_code);
        field.itemText = "imsLookupValue";
        field.itemKey = "imsLookupId";

        // Null value to selectItems except radioButtons and combobox
        if (!['radio', 'radio_multi', 'combobox', 'combobox_multi'].includes(field.type)) {
          let nullValue: any = {};
          nullValue[field.itemText] = 'N/A';
          nullValue[field.itemKey] = null;

          //Add null value to selectItems
          field.selectItems.unshift(nullValue);
        }
      } else if (field.dataSource) {
        // Null value to selectItems
        let nullValue: any = {};
        nullValue[field.itemText] = 'N/A';
        nullValue[field.itemKey] = null;

        // Add prop field condition
        let fieldConditions: any = !fieldDatasourceConditions ? [] : fieldDatasourceConditions.filter((item: any) => item.field_name == field.name)
          .map((item: any) => item.field_condition)

        const condition: string = [field.dataSourceCondition, ...fieldConditions]
          .filter((condition: string) => condition)
          .join(' AND ');

        const columns: string = (field.itemKey != field.itemText) ? `${field.itemKey},${field.itemText}` : field.itemKey;

        // For search-like fields
        if (searchTableFieldTypes.includes(field.type)) {
          field.dataSourceCondition = condition || null;
        } else {
          let selectPayload: GenericSqlSelectPayload = {
            viewName: field.dataSource,
            // colList: `${field.itemKey},${field.itemText}`,
            colList: columns,
            whereCond: condition || null,
            orderStmt: field.dataSourceOrder || null,
            officeId: currentOfficeId || null
          };
          return localThis
            .getGenericSelect({ genericSqlPayload: selectPayload })
            .then((retData: any) => {
              field.selectItems = retData.table_data ? [...retData.table_data] : [];
              //Add null value to selectItems except radioButtons and combobox
              if (!['radio', 'radio_multi', 'combobox', 'combobox_multi'].includes(field.type)) {
                field.selectItems.unshift(nullValue);
              }
              return field;
            });
        }
      }
      return Promise.resolve(field);
    },

    /**
     * Setup field with items for select type fields.
     * @param field Fields to setup as a select
     * @param currentOfficeId ID of the currently selected office to apply as filter to select datasource.
     * @param fieldDatasourceConditions Array of conditions to apply to field. conditions must have the form {field_name:string, field_condition:string}
     */
    setupSelectField(field: any, currentOfficeId?: number, fieldDatasourceConditions?: Array<any>) {
      // Get select item list
      if (field.select_lookup_code) {
        field.selectItems = this.retrieveLookupByTyepCode(field.select_lookup_code);
        field.itemText = "imsLookupValue";
        field.itemKey = "imsLookupId";

        // Null value to selectItems except radioButtons and combobox
        if (!['radio', 'radio_multi', 'combobox', 'combobox_multi'].includes(field.type)) {
          let nullValue: any = {};
          nullValue[field.itemText] = 'N/A';
          nullValue[field.itemKey] = null;

          //Add null value to selectItems
          field.selectItems.unshift(nullValue);
        }
      } else if (field.dataSource) {
        
        // Null value to selectItems
        let nullValue: any = {};
        if (!field.is_required){
          nullValue[field.itemText] = 'N/A';
          nullValue[field.itemKey] = null;
        }

        // Add prop field condition
        let fieldConditions: any = !fieldDatasourceConditions ? [] : fieldDatasourceConditions.filter((item: any) => item.field_name == field.name)
          .map((item: any) => item.field_condition)

        const condition: string = [field.dataSourceCondition, ...fieldConditions]
          .filter((condition: string) => condition)
          .join(' AND ');

        const columns: string = (field.itemKey != field.itemText) ? `${field.itemKey},${field.itemText}` : field.itemKey;

        //let columnList = (field.type != 'search_table' || field.type != 'search_table_multi') ? `${field.itemKey},${field.itemText}` : null;
        if (field.type != 'search_table' && field.type != 'search_table_multi') {

          let selectPayload: GenericSqlSelectPayload = {
            viewName: field.dataSource,
            // colList: `${field.itemKey},${field.itemText}`,
            colList: columns,
            whereCond: condition || null,
            orderStmt: field.dataSourceOrder || null,
            officeId: currentOfficeId || null
          };
          (this as any)
            .getGenericSelect({ genericSqlPayload: selectPayload })
            .then((retData: any) => {
              field.selectItems = retData.table_data ? [...retData.table_data] : [];
              //Add null value to selectItems except radioButtons and combobox
              if (!['radio', 'radio_multi', 'combobox', 'combobox_multi'].includes(field.type)) {
                if (!field.is_required){
                   field.selectItems.unshift(nullValue);
                }
              }
            });
        } else {
          field.dataSourceCondition = condition || null;
        }
      }
      return field;
    },
    /**
     * Setup fields with items for select fields.
     * @param items Array of fields to setups a select
     * @param currentOfficeId ID of the currently selected office to apply as filter to select datasource.
     * @param fieldDatasourceConditions Array of conditions to apply to field. conditions must have the form {field_name:string, field_condition:string}
     */
    setupSelectFields(items: Array<any>, currentOfficeId?: number, fieldDatasourceConditions?: Array<any>) {
      return items.map((field: any) => {
        this.setupSelectField(field, currentOfficeId, fieldDatasourceConditions);
        return field;
      });
    },


    /**
     * Get the OFFICE Id given the OFFICE CODE 
     * @param officeCode The office code
     */
    getOfficeIdFromCode(officeCode: string) {
      let localThis: any = this as any;
     
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_OFFICE",
        colList: null,
        whereCond: `HR_OFFICE_CODE = '${officeCode}'`,
        orderStmt: null
      };
      if (officeCode != null) {
        return localThis.getGenericSelect({ genericSqlPayload: selectPayload })
          .then((res: any) => {

            return res.table_data[0].HR_OFFICE_ID ? res.table_data[0].HR_OFFICE_ID : null;

          })
          .catch((err: any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err });
          });
      } else {
        return new Promise((resolve, reject) => {
          resolve(null);
        });
      }

    },

    getOfficeDetailsFromId(officeId: number) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "HR_VI_OFFICE_DIMENSIONS",
        colList: null,
        whereCond: `HR_OFFICE_ID = ${officeId}`,
        orderStmt: null
      };
      if (officeId != null) {
        return localThis.getGenericSelect({ genericSqlPayload: selectPayload })
          .then((res: any) => {

            let retOffice: any = res.table_data[0];
            return retOffice;

          })
          .catch((err: any) => {
            localThis.showNewSnackbar({ typeName: "error", text: err });
          });
      } else {
        return new Promise((resolve, reject) => {
          resolve(null);
        });
      }

    },


    /**
     * Perform the Upload/Overwrite of documents from JRS DOCUMENTS COMPONENT
     * ---------------------------------------------------------
     * VARIABLE USED FOR FIRST UPLOAD OF THE FILE
     * ---------------------------------------------------------
     * @param value The value from DOCUMENT component
     * @param OfficeCode The office code( if is null the will be taken the office code ).
     * @param folder Array of conditions to apply to field. conditions must have the form {field_name:string, field_condition:string}
     * @param documentTypeId ID of the currently selected office to apply as filter to select datasource.
     * @param file Array of conditions to apply to field. conditions must have the form {field_name:string, field_condition:string}
     * ---------------------------------------------------------
     */
    async UploadFileBlobAzureFromForm(value: any, OfficeCode: string, folder: string, documentTypeId: number, file: any) {
      let localThis: any = this as any;
      let document_id: any;

      // return 1;

      if (!value) {

        const office_id: number = await localThis.getOfficeIdFromCode(OfficeCode);

        let payload: IapiBlobStorageUpload = {
          nameContainer: OfficeCode == null ? localThis.getCurrentOffice.HR_OFFICE_CODE.toLowerCase().replace("_", "") : OfficeCode.toLowerCase().replace("_", ""),
          folder: folder,
          userUid: localThis.getUserUid,
          officeId: OfficeCode == null ? localThis.getCurrentOffice.HR_OFFICE_ID : office_id,
          documentTypeId: documentTypeId,
          file: file
        };

        try {
          document_id = await localThis.uploadBlobStorageFile(payload);

        } catch (err) {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        }

      } else {

        let selectPayload: GenericSqlSelectPayload = {
          viewName: "ST_DOCUMENT",
          colList: null,
          whereCond: `ID = ${value}`,
          orderStmt: "ID",
          officeId: localThis.getCurrentOffice.HR_OFFICE_ID
        };

        try {


          const document_query_result = await localThis.getGenericSelect({ genericSqlPayload: selectPayload });

          let document: any = document_query_result.table_data ? document_query_result.table_data[0] : null;

          if (document) {
            let payload: IapiBlobStorageUploadOverwrite = {
              nameContainer: document.ST_CONTAINER_NAME,
              uniqueFileName: document.ST_UNIQUE_FILE_NAME,
              folder: folder,
              guidFile: document.ST_GUID,
              userUid: localThis.getUserUid,
              officeId: document.ST_OFFICE_ID,
              file: file,
            };


            document_id = await localThis.uploadOverwriteBlobStorageFile(payload)

          } else {
            localThis.showNewSnackbar({ typeName: "error", text: "Record missing in document list" });
          }

        } catch (err) {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        }
      }

      return document_id.ID_DOCUMENT_UPLOADED;

    },

    isNumber: function (evt: any) {
      evt = evt ? evt : window.event;
      var charCode = evt.which ? evt.which : evt.keyCode;
      if (
        charCode > 31 &&
        (charCode < 48 || charCode > 57) &&
        charCode !== 46
      ) {
        evt.preventDefault();
      } else {
        return true;
      }
    },
    round: function (value: any, exp: any) {
      if (typeof exp === "undefined" || +exp === 0) return Math.round(value);

      value = +value;
      exp = +exp;

      if (isNaN(value) || !(typeof exp === "number" && exp % 1 === 0))
        return NaN;

      // Shift
      value = value.toString().split("e");
      value = Math.round(
        +(value[0] + "e" + (value[1] ? +value[1] + exp : exp))
      );

      // Shift back
      value = value.toString().split("e");
      return +(value[0] + "e" + (value[1] ? +value[1] - exp : -exp));
    },

    /**
     * Get distinct form tab codes.
     *
     * @param fields - array of JrsFormFields from which to extract the distinct tabs of the form
     */
    getFormTabs(fields: Array<JrsFormField>) {
      let distinctTabs: Array<JrsFormTab> = [];
      let defaultTab: JrsFormTab = {
        tabCode: (this as any).defaultTabCode,
        tabName: (this as any).defaultTabCode,
      };

      distinctTabs = fields.reduce((distinctList: any, curr: any) => {
        let newTab: JrsFormTab = Object.assign({}, defaultTab);

        // If field has tab defined
        if (curr.tabCode) {
          newTab.tabCode = curr.tabCode;
          newTab.tabName =
            curr.tabTranslationKey != undefined
              ? translate(curr.tabTranslationKey).toString()
              : curr.tabLabel || newTab.tabCode;
        }

        // If newTab is not in list
        if (
          distinctList.findIndex(
            (item: JrsFormTab) => item.tabCode == newTab.tabCode
          ) == -1
        ) {
          // If MAIN tab the it must be first
          if (newTab.tabCode == (this as any).defaultTabCode) {
            distinctList.unshift({
              tabCode: newTab.tabCode,
              tabName: newTab.tabName,
            });
          } else {
            distinctList.push({
              tabCode: newTab.tabCode,
              tabName: newTab.tabName,
            });
          }
        }

        return distinctList;
      }, distinctTabs);

      return distinctTabs;
    },

    /**
     * Get a list of field rules based on the field configuration.
     * @param {JrsFormField} field The field to generate rules for
     * @returns List of rule functions to be assigned to the field. If no rules exist, returns empty array.
     */
    getFieldRules(field: JrsFormField) {
      let rules: Array<Function> = [];
      const fieldConfig:any = field.additional_config || {};

      // Add "required" rule
      if (field.is_required) {
        rules.push((v: any) => !!v || translate("error.required-field"));
      }

      // Add type specific rules
      switch (field.type) {
        case fieldType["text"]:
          // Text fields count rule
          if (fieldConfig.counterValue) {
            rules.push((v: string | null) => {
              // Evaluate null strings as empty
              v = v || "";
              const withinLenght: boolean = v.length <= fieldConfig.counterValue;
              const errMsg: string = translate("error.char-limit-exeeded").toString();
              return withinLenght || errMsg;
            })
          }
          break;

        default:
          break;
      }
      // Return field rules
      return rules;
    }
  }
}
