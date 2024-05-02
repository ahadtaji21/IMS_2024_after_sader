<template>
  <v-card>
    <v-card-title>{{ $t('datasource.pms.annual-plan.objectives.add-indicator') }}</v-card-title>

    <v-card-text>
      <div class="d-flex flex-row justify-center" v-if="isLoading">
        <v-progress-circular indeterminate></v-progress-circular>
      </div>
      <v-form v-if="!isLoading">
        <v-row v-for="(row, i) in getFieldMatrixForTab(attr_list)" :key="'ROW-'+i">
          <v-col v-for="(field, j) in getFieldMatrixForTab(attr_list)[i]" :key="'COL-'+j">
            <!-- SELECT -->
            <v-select
              :label="field.attr_name"
              :id="field.attr_name"
              :items="field.selectItems"
              :item-value="field.itemKey"
              :readonly="onlyRead"
              :item-text="field.itemText"
              v-if="field.attr_type=='1'"
            ></v-select>
            <!-- TEXT FIELD -->
            <v-text-field
              :name="field.attr_name"
              :label="field.attr_name"
              :id="field.name"
              :readonly="onlyRead"
              v-if="field.attr_type=='0'"
            ></v-text-field>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions v-if="!onlyRead">
      <v-btn  color="secondary darken-1" text @click="closeEdit()">X {{ $t('general.close') }}</v-btn>
      <v-btn color="primary" @click="saveIndicators()">{{ $t('general.save') }}</v-btn>
    </v-card-actions>
  </v-card>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { ImsApi, PmsAnnualPlanApi, Configuration } from "../../../axiosapi";
import { i18n } from "../../../i18n";
import JrsDatePicker from "../../../components/JrsDatePicker.vue";
import {
  JrsFormField,
  JrsFormTab,
  JrsFormFieldSelect
} from "../../../models/JrsForm";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../axiosapi/api";
import { EventBus } from "@/event-bus";

interface Attribute {
  attr_id: String;
  attr_name: String;
  attr_type: String;
  attr_value: String;
  attr_order: Number | null;
}

interface Attribute_List_Item {
  attr_id: Number;
  attr_name: String;
  attr_type: String;
  selectItems: Array<Attribute> | null;
  itemKey:String;
  itemText:String;
}
interface payLoadData {
  ID: number | null;
  COI_TOS_REL_ID: number | null;
  DATE_FROM: Date | null;
  DATE_TO: Date | null;
  OCCURRENCE_ID: Date | null;
  OBJECTIVE_ID: number | null;
}
export default Vue.extend({
  components: {
    // "jrs-date-picker": JrsDatePicker
  },

  props: {
    formDataExt: {
      type: Object
    },
    selectedObjectId: {
      type: Number,
      required: true
    },
    onlyRead:{
      type: Boolean,
      required:false,
      default:false
    }
  },
  data() {
    return {
      formData: [],
      formIsValid: false,
      isLoading: false,
      attr_list: []
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
    localThis.setDatasets();

    localThis.formData = JSON.parse(JSON.stringify(localThis.formDataExt));
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),

    setDatasets() {

      let localThis: any = this as any;
      let datasetName: string = "";
      let viwName: string | null = null;
      let orderItem: string | null = null;
      let condition: string | null = null;
      let columnList: string | null = null;
      localThis.attr_list = [];
      viwName = "PMS_VI_INDICATOR_ATTRIBUTES";

      let tmp_list = [] as Attribute[];
      let selectPayload: GenericSqlSelectPayload = {
        viewName: viwName,
        colList: columnList,
        whereCond: condition,
        orderStmt: orderItem
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {

          res.table_data.forEach((item: any) => {
            let attr_item = {} as Attribute;
            attr_item.attr_id = item.INDATT_ID;
            attr_item.attr_name = item.ATTRIBUTE;
            attr_item.attr_type = item.ISDROPDOWN;
            attr_item.attr_value = item.Value;
            attr_item.attr_order = item.VOrder;
            tmp_list.push(attr_item);
          });
          return tmp_list;
        })
        .then((res: any) => {
          let tmp_Map = new Map();

          let i: number;
          for (i = 0; i < tmp_list.length; i++) {
            if (!tmp_Map.has(tmp_list[i].attr_name)) {
              let obj = res.filter(
                (item: any) => item.attr_name == res[i].attr_name
              );
              let attr_items = {} as Attribute_List_Item;
              attr_items.attr_id = i;
              attr_items.attr_name = tmp_list[i].attr_name;
              attr_items.attr_type = tmp_list[i].attr_type;
              attr_items.itemKey="attr_value";
              attr_items.itemText="attr_value";
              attr_items.selectItems = obj;
              tmp_Map.set(tmp_list[i].attr_name, obj);
              localThis.attr_list.push(attr_items);
            }
          }
 
          localThis.isLoading = false;
        });
    },

    /**
     * Get a field matrix 
     *
 
     * @param fields - Array of the JrsFormFields to filter by the tabCode
     */
    getFieldMatrixForTab(fields: Array<Attribute>) {
      let width = 2; //(this as any).nbrOfColumns;
      if (!fields || !width) {
        return undefined;
      }
      return (this as any).listToMatix(fields, width);
    },

    /**
     * Convert a list to a matrix of a given size.
     *
     * @param list the list that must be converted
     * @param elementPerSubarray size of the sub arrays in the matrix
     * @returns A matrix of with == "elementPerSubarray" containing the elements of "list"
     */
    listToMatix(list: Array<any>, elementsPerSubarray: number) {
      let matrix: Array<any> = [];
      let i: number, k: number;

      for (i = 0, k = -1; i < list.length; i++) {
        if (i % elementsPerSubarray === 0) {
          k++;
          matrix[k] = [];
        }
        matrix[k].push(list[i]);
      }
      return matrix;
    },

    closeEdit() {
      let localThis: any = this as any;
      localThis.editMode = false;
      localThis.$emit("closeEdit");
    },
    saveServices() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add-service")
        .toString();

      let id = 0;
      let msg = msgUpd;
      let payLoadD = {} as payLoadData;
      if (localThis.formData.ID == undefined) {
        msg = msgAdd;
        payLoadD.ID = 0;
      } else {
        payLoadD.ID = localThis.formData.ID;
      }

      payLoadD.OBJECTIVE_ID = localThis.selectedObjectId;
      payLoadD.DATE_FROM = localThis.formData.DATE_FROM;
      payLoadD.DATE_TO = localThis.formData.DATE_TO;
      payLoadD.OCCURRENCE_ID = localThis.formData.OCCURRENCE.ID;
      payLoadD.COI_TOS_REL_ID = localThis.formData.TOS.PMS_COI_TOS_ID;
      this.$confirm(msg).then(res => {
        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "PMS_SP_INS_UPD_ANNUAL_PLAN_OBJECTIVE_SERVICE",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoadD)
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              localThis.$emit("refreshObjSrv");
              EventBus.$emit("reloadObj");
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "success",
                text: res.message
              }); // Feedback to user
              localThis.dialog = false;
              localThis.showSrvDetails = false;
              localThis.tmpSelectedItem = [];
              localThis.serviceList = localThis.serviceListOrig;
              localThis.itemsPerPage = 15;
              localThis.$emit("closeDialoge");
              localThis.$emit("refreshDesc", localThis.formData);
            })
            .catch((err: any) => {
              localThis.formData = {}; //;as ServiceData;
              localThis.editMode = false;
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message
              }); // Feedback to user
            });
        }
      });
    }
  }
});
</script>

<style scoped>
</style>