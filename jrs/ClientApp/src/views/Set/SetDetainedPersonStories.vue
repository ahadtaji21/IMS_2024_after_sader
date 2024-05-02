<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->     
          <v-row>
            <v-col :cols="12">
              <h1>
                {{ $t("views.view-settlement-detained-person-story.title") }}
                <v-icon large>mdi-tag-multiple</v-icon>
              </h1>
              <p>
                {{
                  $t("views.view-settlement-detained-person-story.view-info")
                }}
              </p>
            </v-col>
          </v-row>
          <!--FILTERS TO SELECT SETTLEMENTS + VISITORS -->
          <!-- MAIN FILTER -->
          <v-card outlined class="lv-0" dense>
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
              <v-row justify="center">
                <!-- SETTLEMENTS SELECT -->
                <v-col :cols="6">
                  <jrs-field
                    :field="settlementTypeField"
                    v-model="settlement"
                  ></jrs-field>
                </v-col>
                <!-- DETAINED PERSON SELECT -->
                <v-col :cols="3" >
                  <jrs-field
                    :key="JSON.stringify(settlement)"
                    :field="detainedPersonTypeField"
                    v-model="detainedPerson"
                  ></jrs-field>
                </v-col>
                <!-- TAGS -->
                <v-col :cols="3">
                    <jrs-field
                    :field="tagsTypeField"
                    v-model="tag"
                  ></jrs-field>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
          <v-divider></v-divider>

          <!-- DATA ENTRY FOR THE VISIT (SIMPLE TABLE) -->
          <v-row>
            <v-col :cols="12">
              <jrs-simple-table
                :key="JSON.stringify(listDetainedConditionNew)"
                :fieldDatasourceConditions="listDetainedConditionNew"
                viewName="SET_VI_STORY"
                :dataSourceCondition="conditionVisitors"
                :hideNewBtn="false"
                :hideActions="false"
                :nbrOfFormColumns="3"
                :selectOnRowClick="true"
                @rowClick="doClickAction"
              >
              </jrs-simple-table>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import JrsForm from "../../components/JrsForm.vue";
import {
  GenericSqlSelectPayload
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsField from "../../components/JrsField.vue";
import { fieldType, JrsFormFieldSelect } from "../../models/JrsForm";
import { translate } from "../../i18n";
export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
     "jrs-field": JrsField,
  },
  mixins: [FormMixin],
  data() {
    return {
      settlementsList: [],
      detainedPerson: null,
      detainedPersonList: [],
      settlement: null,
      tags: [],
      tag: null,
      isSaving: false,
      dateSelectedFrom: null,
      conditionDetainedPerson:null,
      dateSelectedTo: null,
    };
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
      getUserUid: "getUserUid",
    }),
    settlementTypeField() {

                const localThis: any = this as any;
                const fieldConfig = localThis.parseJrsFormField( {
                    name: "settlementsList",
                    type: fieldType["auto"],
                    text: translate("views.view-settlement-detained-person-story.settlement-select").toString(),
                    select_items_datasource:"SET_VI_SETTLEMENTS_BIODATA",
                    itemText: "SET_NAME",
                    itemKey: "SET_ID"
                });
                 const tmpSelectFields: any[] = localThis.setupSelectFields(
                    [fieldConfig],
                    localThis.getCurrentOffice.HR_OFFICE_ID
                );
                return tmpSelectFields[0];
            },
    detainedPersonTypeField(){
           const localThis: any = this as any;
                const fieldConfig= localThis.parseJrsFormField( {
                    name: "detainedPersonList",
                    type: fieldType["auto"],
                    text: translate("views.view-settlement-detained-person-story.detained-person-select").toString(),
                    select_items_datasource:"SET_VI_DETAINED_PERSON_BIODATA",
                    itemText: "HR_BIODATA_FULL_NAME", 
                    dataSourceCondition: localThis.conditionDetainedPerson ? localThis.conditionDetainedPerson : "IMS_USER_IS_BENEFICIARY = 1",
                    itemKey: "HR_BIODATA_USER_UID",
                    lookupName: "HR_BIODATA_USER_UID"
                });
                 const tmpSelectFields: any[] = localThis.setupSelectFields(
                    [fieldConfig],
                    localThis.getCurrentOffice.HR_OFFICE_ID
                );
                return tmpSelectFields[0];
    },
    tagsTypeField(){
           const localThis: any = this as any;
                  const fieldConfig= localThis.parseJrsFormField( {
                    name: "tag",
                    type: fieldType["auto"],
                    text: translate("views.view-settlement-detained-person-story.tags-select").toString(),
                    hint:translate("views.view-settlement-detained-person-story.tags-select-hint").toString(),
                    select_items_datasource:"SET_VI_EXISTING_TAGS",
                    itemText: "TAG", 
                    itemKey: "TAG",
                    sendOnlyKey:true,
                    multiple:true
                });
                 const tmpSelectFields: any[] = localThis.setupSelectFields(
                    [fieldConfig],
                    localThis.getCurrentOffice.HR_OFFICE_ID
                );
                return tmpSelectFields[0];
    },
    listDetainedConditionNew() {
      let list: any = [];
      let string: any = null;
      let object: any = [];
       let localThis: any = this as any;
      if (localThis.detainedPerson == null) {
        localThis.detainedPersonList.forEach((element: any) => {
          list.push(element.HR_BIODATA_USER_UID);
        });
        if (list.length > 0) {
          string = "('" + list.join("','") + "')";
        }
      } else {
        const UserUid:string = localThis.detainedPerson

        string = UserUid ? "('" + UserUid  + "')" : null;
      }
      if(localThis.settlement){
        object.push(
                  {
                    field_name: 'SETTLEMENT_ID',
                    field_condition: 'SET_ID = ' + localThis.settlement,
                  }
                )
      }
      object.push(
                  {
                    field_name: 'FILL_IN_USER',
                    field_condition: 'IMS_USER_UID IN ' + string,
                  }
                )
      return string ? object  : null
    },
    
    conditionVisitors() {
      let localThis: any = this as any;
      let arrayConditions = [];
      let tags = localThis.tag ? localThis.tag.length > 0 : null;
      if (localThis.settlement) {
        arrayConditions.push(`SETTLEMENT_ID = ${localThis.settlement}`);
      }

       if (localThis.detainedPerson) {
        arrayConditions.push(
          `FILL_IN_USER = '${localThis.detainedPerson}'`
        );
      }
      if (tags) {
        localThis.tag.forEach((element: any) => {   
           arrayConditions.push(`FORMATTED_TAG_LIST LIKE '%#${element.TAG}%'`)
        });       
      }
      return arrayConditions.join(" AND ");
    },

  },
  watch:{
    settlement(value){
      let localThis: any = this as any;
      localThis.detainedPerson = null;
      let cond = value ? `PMS_SETTLEMENT_ID = ${value}` : "1=1";
      localThis.conditionDetainedPerson = "IMS_USER_IS_BENEFICIARY = 1 AND " + cond;
      localThis.getDetainedPersonBiodata();
    }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
      getJRSTableStructure: "getJRSTableStructure",
    }),

      doClickAction(item:any)
    {
      const localThis: any = this as any;
     // localThis.setupEditForm(item);
 
    },
  /**
     * Return a list of settlements (biodata) filtered for office
     */
    async getDetainedPersonBiodata() {
      let localThis: any = this as any;
      localThis.detainedPersonList = [];
      let cond =  + localThis.settlement
          ? `PMS_SETTLEMENT_ID = ${localThis.settlement}` : "1=1"
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "SET_VI_DETAINED_PERSON_BIODATA",
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        whereCond: "IMS_USER_IS_BENEFICIARY = 1 AND " + cond,
      };
      return await localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          if (res.item_count > 0) {
            // Setup projectList
            let settList = res.table_data.map((row: any) => {
              return {
                HR_BIODATA_USER_UID: row.HR_BIODATA_USER_UID,
                HR_BIODATA_FULL_NAME: row.HR_BIODATA_FULL_NAME,
              };
            });
  
            localThis.detainedPersonList = [...settList];
          }

        });
    },


  },


});
</script>

<style scoped>
</style>