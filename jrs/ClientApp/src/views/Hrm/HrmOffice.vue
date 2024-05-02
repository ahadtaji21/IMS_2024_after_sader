<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col :cols="12">
          <!-- TABLE -->
          <jrs-simple-table
            :viewName="viewNameViOffice"
            :selectable="false"
            :hideNewBtn="true"
            :hideActions="true"
            v-if="!selectedItemOffice"
          >
            <!-- TABLE HEADER -->
            <template v-slot:simple-table-header>
              <v-btn
                color="secondary lighten-2"
                class="grey--text text--darken-3"
                @click="selectItem({})"
              >
                <v-icon>mdi-plus-circle-outline</v-icon>
                {{$t('general.new')}}
              </v-btn>
            </template>

            <!-- ROW ACTIONS -->
            <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
              <v-icon @click="selectItem(simpleItemRowItem)">mdi-circle-edit-outline</v-icon>
            </template>
            
          </jrs-simple-table>

          <!-- FORM -->
          <div v-if="selectedItemOffice" class="elevation-1">
            <v-toolbar color="primary darken-1" dark>
              <v-spacer></v-spacer>
              <v-btn color="secondary" class="ma-2" @click="closeDetail()">
                <v-icon>mdi-table</v-icon>
                {{ $t('datasource.hrm.household.btn-to-list') }}
              </v-btn>

              <template v-slot:extension>
                <v-tabs v-model="activeTab" centered background-color="primary darken-1" dark>
                  <v-tab key="BASIC">{{ $t('datasource.hrm.office.basic-info') }}</v-tab>
                  <v-tab key="OFFICE_REPRESENTATIVE_HISTORY" v-if="selectedItemOffice.HR_OFFICE_ID">{{ $t('datasource.hrm.office-representative-history.title') }}</v-tab>
                  <v-tab key="OFFICE_RELATIONS" v-if="selectedItemOffice.HR_OFFICE_ID">{{ $t('datasource.hrm.office-relations.title') }}</v-tab>
                  <v-tab key="OFFICE_COVERAGE" v-if="selectedItemOffice.HR_OFFICE_ID">{{ $t('datasource.hrm.office-coverage.title') }}</v-tab>
                  <v-tab key="OFFICE_LANGUAGE" v-if="selectedItemOffice.HR_OFFICE_ID">{{ $t('datasource.hrm.office-language.country-language') }}</v-tab>
                  <v-tab key="SETTLEMENTS" v-if="selectedItemOffice.HR_OFFICE_ID">{{ $t('datasource.hrm.office-settlements.title') }}</v-tab>
                </v-tabs>
              </template>
            </v-toolbar>
            <v-tabs-items v-model="activeTab" class="tab-item-wrapper">
              <!-- BASIC INFO -->
              <v-tab-item key="BASIC">
                <jrs-form :formData="selectedItemOffice" :formFields="formFieldsOffice" :nbrOfColumns="2">
                  <template v-slot:form-actions="{ validateFunc, resetFunc }">
                    <v-btn
                      color="primary"
                      :disabled="isSaving"
                      @click="saveFormData(selectedItemOffice, selectedItemOffice.HR_OFFICE_ID ? 1 : 0, validateFunc)"
                      class="ma-2"
                    >{{ $t('general.save') }}</v-btn>
                    <v-btn
                      color="primary"
                      @click="resetFunc()"
                      class="ma-2"
                    >{{ $t('general.reset') }}</v-btn>
                  </template>
                </jrs-form>
              </v-tab-item
              >
              <!-- OFFICE REPRESENTATIVE HISTORY -->
              <v-tab-item key="OFFICE_REPRESENTATIVE_HISTORY">
                <div>
                  <jrs-simple-table
                    viewName="HR_VI_OFFICE_REPRESENTATIVE_HISTORY"
                    :dataSourceCondition="`HR_OFFICE_ID = '${selectedItemOffice.HR_OFFICE_ID}'`"
                    :savePayload="{ HR_OFFICE_ID: selectedItemOffice.HR_OFFICE_ID }"
                    :hideNewBtn="false"
                    :hideActions="false"
                  ></jrs-simple-table>
                </div>
              </v-tab-item>

               <!-- OFFICE RELATIONS -->
              <v-tab-item key="OFFICE_RELATIONS">
                <div>
                  <jrs-simple-table
                    viewName="HR_VI_OFFICE_RELATIONS"
                    :dataSourceCondition="`PARENT_OFFICE_ID = '${selectedItemOffice.HR_OFFICE_ID}'`"
                    :savePayload="{ PARENT_OFFICE_ID : selectedItemOffice.HR_OFFICE_ID }"
                    :hideNewBtn="false"
                    :hideActions="false"
                  ></jrs-simple-table>
                </div>
              </v-tab-item>
              
              <!-- OFFICE COVERAGE-->
              <v-tab-item key="OFFICE_COVERAGE">
                   <div>
                  <jrs-simple-table
                    viewName="HR_VI_OFFICE_COVERAGE"
                    :dataSourceCondition="`HR_OFFICE_ID = '${selectedItemOffice.HR_OFFICE_ID}'`"
                    :hideNewBtn="false"
                    :hideActions="true"
                  ></jrs-simple-table>
                </div>
              </v-tab-item>


                <!-- OFFICE LANGUAGES -->
              <v-tab-item key="OFFICE_LANGAUGE">
                  <div>
                  <jrs-simple-table
                    viewName="HR_VI_OFFICE_LANGUAGE"
                    :dataSourceCondition="`HR_OFFICE_ID = '${ selectedItemOffice.HR_OFFICE_ID}'`"
                    :savePayload="{ HR_OFFICE_ID: selectedItemOffice.HR_OFFICE_ID , userUid:getUserUid }"
                    :hideNewBtn="false"
                    :hideActions="false"
                  ></jrs-simple-table>
                </div>
              </v-tab-item>

                 <!-- SETTLEMENTS OF OFFICE -->
              <v-tab-item key="SETTLEMENTS">
                  <div>
                  <jrs-simple-table
                    viewName="SET_VI_SETTLEMENTS_BIODATA"
                    :dataSourceCondition="`OFFICE_FILTER_ID = ${ selectedItemOffice.HR_OFFICE_ID}`"
                    :hideNewBtn="true"
                    :hideActions="true"
                    :ignoreOfficeFilter="true"
                  ></jrs-simple-table>
                </div>
              </v-tab-item>
            </v-tabs-items>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions, mapGetters } from "vuex";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import JrsForm from "../../components/JrsForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "jrs-form": JrsForm,
  },
  mixins: [FormMixin],
  data() {
    return {
      viewNameViOffice: "HR_VI_OFFICE",
      selectedItemOffice: null,
      activeTab: "",
      crud_info: {},
      formFieldsOffice: [],
      formFieldsOfficeRepresentativeHistory: [],
      formFieldsOfficeLanguages: [],
      isSaving:false
    };
  },
  computed:{
    ...mapGetters({
      getUserUid: 'getUserUid',
      getCurrentOffice: 'getCurrentOffice'
    })
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
    /**
     * Select the current item to edit, by doing so, close the list and open the detail section.
     * @param {Any} item - Item to select for update
     */
    selectItem(item: any) {
      let localThis: any = this as any;
      // localThis.selectedItemOffice = Object.assign({}, localThis.selectedItemOffice, item);
      localThis.selectedItemOffice = Object.assign({}, item);
      
    },
    /**
     * Close detail section and open list.
     */
    closeDetail() {
      let localThis: any = this as any;
      localThis.selectedItemOffice = null;                                                        //Assign is done in this way to guarantee reactivity
    },
    /**
     * Get structure of the Biodata Form
     */
    getFormStructOffice() {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: localThis.viewNameViOffice,
        colList: null,
        whereCond: null,
        orderStmt: null,
      };
      // localThis.formFields = localThis
      localThis
        .getJRSTableStructure({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Setup CRUD info
          Object.assign(localThis.crud_info, res.crud_info);

          // Setup Form Fields
          let tmpFormFileds: Array<any> = res.columns
            .filter((header: any) => !header.hide_in_form)
            .map((field: any) => localThis.parseJrsFormField(field));
          localThis.formFieldsOffice = localThis.setupSelectFields(tmpFormFileds);
        });
    },
   
    /**
     * Save form data.
     */
    saveFormData(
      saveData: any,
      actionType: SqlActionType,
      formValidateFunc: Function,
      formResetFunc?: Function
    ) {
      // Check form validity
      if (!formValidateFunc()) {
        return null;
      }

      let localThis: any = this as any;
      localThis.isSaving=true;
      let spName: string =
        actionType == SqlActionType.NUMBER_0
          ? localThis.crud_info.create_sp
          : localThis.crud_info.update_sp;

      let savePayload: GenericSqlPayload = {
        spName: spName,
        actionType: actionType,
        jsonPayload: JSON.stringify(saveData),
        userUid: localThis.getUserUid,
        officeId: localThis.getCurrentOffice.HR_OFFICE_ID
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({
            typeName: "succes",
            text: res.message,
          });

          if (formResetFunc) {
            formResetFunc();
          }

          //If "new" then load the data
          if (actionType == SqlActionType.NUMBER_0 && res.pk_id) {
            localThis.selectedItemOffice = { ...localThis.selectedItemOffice, ...{ HR_OFFICE_ID: res.pk_id} };
          }
        })
        .catch((err: any) => {
          console.log("There is an error");
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        }).finally(()=>{
                localThis.isSaving=false;
        });
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getFormStructOffice();
  },
});
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>