<template>
  <v-card>
    <v-card-text>
      <v-form>
        <v-row>
          <v-col>
            <jrs-location-picker 
            v-model="locationIdLoc" 
            label="Location" 
            :required="true"
            ></jrs-location-picker>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-text-field
              v-model="locationDetailsLoc"
              label="Location Details"
              hint="City, Building, School, Rooms, …"
              :required="true"
              :rule="requiredTextFieldRule"
              :readonly="onlyRead"
            ></v-text-field>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions v-if="!onlyRead">
      <v-btn color="primary" @click="saveLocation()">{{ $t('general.save') }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";
import JrsLocationPicker from "../../../../JrsLocationPicker.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../../../../../axiosapi/api";
interface payLoadData {
  ID_SERVICE: number | null;
  LOCATION_ID: Number;
  LOCATION_DESCRIPTION: String | null;
}
export default Vue.extend({
  components: {
    // "jrs-table": JrsTable,

    "jrs-location-picker": JrsLocationPicker
  },
  props: {
    selectedObjectId: {
      type: Number,
      required: true
    },
    locationId: {
      type: Number,
      default: 0
    },
    locationDetails: {
      type: String
    },
    onlyRead:{
      type:Boolean,
      required:false,
      default:false
    }
  },
  data() {
    return {
      locationIdLoc: 0,
      locationDetailsLoc: "",
      requiredTextFieldRule: [ (v:any) => !!v || 'Desc is required']
    };
  },
  methods: {
       ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    saveLocation() {
      let localThis = this as any;
      let msgUpd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-update")
        .toString();
      let msgAdd: string = this.$i18n
        .t("datasource.pms.annual-plan.objectives.confirm-add-service-location")
        .toString();
      let id = 0;
      let msg = msgUpd;
      let payLoad = {} as payLoadData;
      payLoad.ID_SERVICE = localThis.selectedObjectId;
      payLoad.LOCATION_ID = localThis.locationIdLoc;
      payLoad.LOCATION_DESCRIPTION = localThis.locationDetailsLoc;
      if (localThis.locationId == 0) {
        msg = msgAdd;
      }

      this.$confirm(msg).then(res => {

        if (res) {
          let savePayload: GenericSqlPayload = {
            spName: "GMT_SP_INS_UPD_GRANT_OBJECTIVE_SERVICE_LOCATION",
            actionType: SqlActionType.NUMBER_0,
            jsonPayload: JSON.stringify(payLoad)
          };

          localThis
            .execSPCall(savePayload)
            .then((res1: any) => {

              EventBus.$emit("locationSaved", localThis.locationIdLoc,localThis.locationDetailsLoc);
            
              localThis.showNewSnackbar({
                typeName: "success",
                text: res1.message
              }); // Feedback to user
            })
            .catch((err: any) => {
     
              localThis.showNewSnackbar({
                typeName: "error",
                text: err.message
              }); // Feedback to user
            });

          //
        } else {
          //
        }
      });
    }
  },
  beforeMount() {
    let localThis = this as any;
    localThis.locationIdLoc = localThis.locationId;
  },
  mounted() {
    let localThis = this as any;
    localThis.locationDetailsLoc = localThis.locationDetails;
  }
});
</script>

<style lang="scss" scoped>
</style>