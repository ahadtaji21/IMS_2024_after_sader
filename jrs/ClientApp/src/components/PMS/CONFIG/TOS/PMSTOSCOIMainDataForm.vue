<template>
  <v-card>
    <v-card-text>
      <!-- <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>-->
      <v-form v-if="!isLoading">
        <v-row>
          <v-col>
            <!-- COI -->
            <v-autocomplete
              :label="$t('datasource.pms.category-of-intervention.category-of-intervention')"
              id="COI"
              v-model="formData.COI"
              :hint="$t('datasource.pms.category-of-intervention.category-of-intervention')"
              persistent-hint
              required
              return-object
              :items="coi"
              item-value="PMS_COI_ID"
              item-text="IMS_LABELS_VALUE"
            ></v-autocomplete>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-start')"
              v-model="formData.START_DATE"
              :hint="$t('general.date-start')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
          <v-col>
            <jrs-date-picker
              :label="$t('general.date-end')"
              v-model="formData.END_DATE"
              :hint="$t('general.date-end')"
              :required="true"
            ></jrs-date-picker>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-btn color="secondary darken-1" text @click="closeEdit()">X {{ $t('general.close') }}</v-btn>
      <v-btn color="primary" @click="saveTOSCOIREL()">{{ $t('general.save') }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { ImsApi, PmsAnnualPlanApi, Configuration } from "../../../../axiosapi";
import { i18n } from "../../../../i18n";
import JrsDatePicker from "../../../../components/JrsDatePicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../../axiosapi/api";

interface TypeOfServiceData {
  ID: number | null;
  COI: {} | null;
  START_DATE: Date | null;
  END_DATE: Date | null;
  OCCURRENCE: {} | null;
  COI_ID: number | null;
}
interface payLoadData {
  ID: number | null;
  START_DATE: Date | null;
  END_DATE: Date | null;
  PMS_TOS_ID: number | null;
  PMS_COI_ID: number | null;
  IMS_LABELS_VALUE: string | null;
  PMS_COI_CODE: string | null;
}
export default Vue.extend({
  components: {
    "jrs-date-picker": JrsDatePicker
  },

  props: {
    formDataExt: {
      type: Object
    },
    selectedObjectId: {
      type: Number,
      required: false
    }
  },
  data() {
    return {
      formData: {},
      formIsValid: false,
      isLoading: false,
      coi: [],
      requiredTextFieldRule: [(v: any) => !!v || "Title is required"]
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    }
  },
  watch: {
    // formData(to: any) {
    //   let localThis = this as any;
    //   debugger;
    // }
  },
  mounted() {

    let localThis = this as any;
    localThis.isLoading = true;
    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
    localThis.formData.COI={};
    localThis.formData.COI.PMS_COI_ID = localThis.formData.PMS_COI_ID;
    localThis.formData.COI.IMS_LABELS_VALUE = localThis.formData.IMS_LABELS_VALUE;
    localThis.formData.COI.PMS_COI_TOS_ID = localThis.formData.ID;
    localThis.formData.COI.PMS_COI_CODE = localThis.formData.PMS_COI_CODE;
    if (localThis.formData && localThis.formData.PMS_COI_ID) {
      localThis.setDatasets("COI");
    }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    /**
     * Refresh autocomplete.
     * @param changedCode - code of the autocomplete which has changed
     */
    setDatasets(changedCode?: any) {
      let localThis: any = this as any;
      let datasetName: string = "";
      let viwName: string | null = null;
      let orderItem: string | null = null;
      let condition: string | null = null;
      let columnList: string | null = null;
      let defaultItemCode: string; //item code for defautl null value on loaded selectItems
      //let language:string = localThis.userLocale();
      switch (changedCode) {
        case "COI":
          datasetName = "coi";
          viwName = "PMS_VI_CATEGORY_OF_INTERVENTION_1";
          localThis.tos = [];
          if(localThis.formData.ID != undefined && localThis.formData.ID !=0)
          {
          condition = `((${localThis.formData.ID}<>0 AND PMS_COI_TOS_ID=${localThis.formData.ID})  OR 
           PMS_COI_TOS_ID is null OR PMS_COI_ID NOT IN (SELECT PMS_COI_ID FROM PMS_COI_TOS_REL WHERE PMS_COI_ID=${localThis.formData.PMS_COI_ID}))`;
          }
          else
          {
              condition=`(PMS_COI_TOS_ID is null OR  PMS_COI_ID NOT IN  (SELECT PMS_COI_ID FROM PMS_COI_TOS_REL WHERE PMS_COI_ID=${localThis.formData.PMS_COI_ID}))`;
          }

          orderItem = "PMS_COI_CODE";
          columnList = "PMS_COI_ID,IMS_LABELS_VALUE,PMS_COI_TOS_ID,PMS_COI_CODE";

          break;
      }

      let selectPayload: GenericSqlSelectPayload = {
        viewName: viwName,
        colList: columnList,
        whereCond: condition,
        orderStmt: orderItem
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis[datasetName] = [...(res.table_data ? res.table_data : [])];
          if (datasetName == "coi") {
            localThis[datasetName].unshift({
              PMS_COI_ID: null,
              IMS_LABELS_VALUE: "N/A",
              PMS_COI_CODE: "N/A"
            });
          
  let i: number = 0;
            let tmpObj:any=[];
            if (
              localThis.formDataExt.toscoiList != undefined &&
              localThis.formDataExt.toscoiList.length > 0
            ) {
              if (localThis[datasetName].length > 0) {
                for (i = 0; i < localThis[datasetName].length; i++) {
                  let j: number = 0;
                  let found:boolean=false;
                  for (
                    j = 0;
                    j < localThis.formDataExt.toscoiList.length;
                    j++
                  ) {
                    if (localThis[datasetName][i].PMS_COI_ID==localThis.formDataExt.toscoiList[j].PMS_COI_ID
                    && localThis[datasetName][i].PMS_COI_ID != localThis.formData.PMS_COI_ID )
                    {
                      found=true;
                      break;
                    }
                  }
                  if(!found)
                  {
                      tmpObj.push(localThis[datasetName][i]);
                  }
                }
                localThis[datasetName]=tmpObj;
              }
            }
          }
          localThis.isLoading = false;
        });
    },

    closeEdit() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.$emit("closeEdit");
    },
    saveTOSCOIREL() {
      let localThis = this as any;
      let payLoadD = {} as payLoadData;
      if (localThis.formData.ID == undefined) {
        payLoadD.ID = 0;
      } else {
        payLoadD.ID = localThis.formData.ID;
      }

      payLoadD.PMS_TOS_ID = localThis.selectedObjectId;
      payLoadD.START_DATE = localThis.formData.START_DATE;
      payLoadD.END_DATE = localThis.formData.END_DATE;
      payLoadD.PMS_COI_ID = localThis.formData.COI.PMS_COI_ID;
      payLoadD.IMS_LABELS_VALUE = localThis.formData.COI.IMS_LABELS_VALUE;
      payLoadD.PMS_COI_CODE = localThis.formData.COI.PMS_COI_CODE;
      localThis.$emit("refreshTOSCOIRelList", payLoadD);
      localThis.closeEdit();
    }
  }
});
</script>

<style scoped>
</style>