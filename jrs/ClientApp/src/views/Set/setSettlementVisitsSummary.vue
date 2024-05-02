<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row outlined elevation="4">
        <v-col :cols="12">
          <!-- PAGE INFO -->
          <v-row>
            <v-col :cols="12">
             <h1>
                {{ $t("views.view-settlement-visit-summary.title") }}
                <v-icon large> mdi-clipboard-edit</v-icon>
              </h1>
            <!--  <p>{{ $t("views.view-settlement-visit-summary.view-info") }}</p>-->
            </v-col>
          </v-row>
          <!--FILTERS TO SELECT SETTLEMENTS + VISITORS -->
          <!-- MAIN FILTER -->
          <v-card
            outlined
            class="lv-0"
            dense
          >
            <v-card-title>{{ $t("general.filters") }}</v-card-title>
            <v-card-text>
             
              <v-row justify="center">
                <!-- SETTLEMENTS SELECT -->
                <v-col :cols="6">
                  
                  <v-autocomplete
                    v-model="settlement"
                    clearable
                    @click:clear="settlement={};dateSelectedFrom=dateSelectedFrom,dateSelectedTo=dateSelectedTo"
           
                    :label="
                      $t(
                        'views.view-settlement-visit-summary.settlement-select'
                      )
                    "
                    :items="settlementsList"
                    
                    :persistent-hint="true"
                    item-value="SET_ID"
                    item-text="SET_NAME"
                    return-object
                  ></v-autocomplete>
                </v-col>
                  <!-- SETTLEMENTS SELECT -->
                <v-col :cols="3">
                  
                  <jrs-date-picker
                    :label="$t('datasource.pms.class-attendance.date-from')"
                    v-model="dateSelectedFrom"
                    :hint="$t('views.view-class-attendance.date-from-hint')"
                    :persistent-hint="true"
                  ></jrs-date-picker>
                </v-col>
                 <v-col :cols="3">
                  <jrs-date-picker
                    :label="$t('datasource.pms.class-attendance.date-to')"
                    v-model="dateSelectedTo"
                    :hint="$t('views.view-class-attendance.date-to-hint')"
                    :persistent-hint="true"
                  ></jrs-date-picker>
                </v-col>
              </v-row>   
            </v-card-text> 
          </v-card>
          <v-divider></v-divider>
   
          <!-- DATA ENTRY FOR THE VISIT (SIMPLE TABLE) -->
          <v-row >
            <v-col :cols="12" v-if="settlementsList.length > 0">
                <jrs-simple-table
                     viewName="SET_VI_BIO_VISIT_SUMMERY"
                    :dataSourceCondition="conditionVisitors" 
                    :hideNewBtn="false"
                    :hideActions="false"
                    :nbrOfFormColumns="3"
                    :selectOnRowClick="true"
                    :dataSourceOrder="'SET_BIO_VISIT_ADDED DESC'"
               ></jrs-simple-table>
            </v-col>
          </v-row>
<v-divider></v-divider>
           <v-row >
            <v-col :cols="12" v-if="settlementsList.length > 0">
                <jrs-simple-table
                    viewName="SET_VI_BIO_VISIT_SUMMERY"
                    :dataSourceCondition="conditionVisitorsMine" 
                    :hideNewBtn="false"
                    :hideActions="false"
                     :hiddenFields="['SET_BIO_VISIT_OPERATOR_UID']"
                    :nbrOfFormColumns="3"
                    :selectOnRowClick="true"
                    :clientPagination="false"
                    :dataSourceOrder="'SET_BIO_VISIT_ADDED DESC'"
                    :title="$t('views.view-settlement-visit-summary.my-visits')"
               >
               <template v-slot:simple-table-header>
              <div class="ml-10" >
               <v-checkbox x-small v-model="applyFiltertoMyVisits" :label="`Apply Filters`"></v-checkbox>
          </div>
              </template>
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
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../axiosapi/api";
import FormMixin from "../../mixins/FormMixin";
import JrsDatepicker from "../../components/JrsDatePicker.vue";
import { JrsFormFieldSearch } from "../../models/JrsForm";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
       "jrs-date-picker": JrsDatepicker,
  },
  mixins: [FormMixin],
  data() {
    return {
      settlementsList: [],
      settlement:{},
      isSaving: false,
      dateSelectedFrom: null,
      dateSelectedTo:null,
      applyFiltertoMyVisits:false
    };
  },
  computed: {
    ...mapGetters({
      getCurrentOffice: "getCurrentOffice",
      getUserUid: "getUserUid",
    }),

    conditionVisitors(){
        let localThis: any = this as any;
        let arrayConditions = []//[`SET_BIO_VISIT_OPERATOR_UID = '${localThis.getUserUid}'`];
        if(localThis.settlement && localThis.settlement.SET_ID){
               arrayConditions.push(`SET_BIO_VISIT_SETTLEMENT_ID = ${localThis.settlement.SET_ID}`)
        }
        if(localThis.dateSelectedFrom){
              arrayConditions.push(`CAST(SET_BIO_VISIT_DATE AS DATE) >= CAST('${localThis.dateSelectedFrom.toISOString()}' AS DATE)`)
        }
        if(localThis.dateSelectedTo){
              arrayConditions.push(`CAST(SET_BIO_VISIT_DATE AS DATE) <= CAST('${localThis.dateSelectedTo.toISOString()}' AS DATE)`)
        }

        return arrayConditions.join(' AND ');
    },

    conditionVisitorsMine(){
         let localThis: any = this as any;
        let arrayConditions = [`SET_BIO_VISIT_OPERATOR_UID = '${localThis.getUserUid}'`];
        if(localThis.applyFiltertoMyVisits){

        if(localThis.settlement && localThis.settlement.SET_ID){
               arrayConditions.push(`SET_BIO_VISIT_SETTLEMENT_ID = ${localThis.settlement.SET_ID}`)
        }
        if(localThis.dateSelectedFrom){
              arrayConditions.push(`CAST(SET_BIO_VISIT_DATE AS DATE) >= CAST('${localThis.dateSelectedFrom.toISOString()}' AS DATE)`)
        }
        if(localThis.dateSelectedTo){
              arrayConditions.push(`CAST(SET_BIO_VISIT_DATE AS DATE) <= CAST('${localThis.dateSelectedTo.toISOString()}' AS DATE)`)
        }
        }
        return arrayConditions.join(' AND ');
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
    /**
     * Return a list of settlements (biodata) filtered for office
     */
    getSettlementsBiodata() {
        let localThis: any = this as any;
        let selectPayload: GenericSqlSelectPayload = {
            viewName: "SET_VI_SETTLEMENTS_BIODATA",
            officeId: localThis.getCurrentOffice.HR_OFFICE_ID,
        };

        return localThis
            .getGenericSelect({ genericSqlPayload: selectPayload })
            .then((res: any) => {
            if (res.item_count > 0) {
                // Setup projectList
                let settList = res.table_data.map((row: any) => {
                return {
                    SET_ID: row.SET_ID,
                    SET_NAME: row.SET_NAME,
                };
                });
                localThis.settlementsList = [...settList];
            }
            localThis.settlement = localThis.settlementsList[0];
            });
    },
   
  },
  mounted() {
    let localThis: any = this as any;
    localThis.getSettlementsBiodata();
  },
});
</script>

<style scoped>
</style>