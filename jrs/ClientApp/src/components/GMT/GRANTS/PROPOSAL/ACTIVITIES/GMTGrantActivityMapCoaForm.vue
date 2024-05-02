<template>
  <div>
    <!-- scelta del COI TOS e COA da associare alla budget line -->
    <template>
      <v-card>
        <v-card>
          <v-container fluid>
            <v-form
              v-model="formValid"
              ref="form"
              lazy-validation
              class="text-capitalize"
            >
              <v-row align-center v-if="showWait">
                <v-col justify-center :cols="12">
                  <div
                    class="text-center"
                    v-if="showWait"
                    style="margin: 0px; padding: 0px"
                  >
                    <v-progress-circular
                      indeterminate
                      color="primary"
                    ></v-progress-circular>
                  </div>
                </v-col>
              </v-row>


 
              <v-row>
                <v-col :cols="12">
                  <v-text-field
                    :label="$t('datasource.gmt.donor.dnr-coa')"
                    v-model="donorFormData.COA_DESCR"
                    :counter="150"
                    @click="openCOADialog"
                    :rules="rules.required"
                    :readonly="true"
                  ></v-text-field>
                </v-col>
              </v-row>
            </v-form>
          </v-container>
        </v-card>

        <v-card-actions>
          <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
          <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
        </v-card-actions>
      </v-card>
      <v-dialog
        v-model="editCOAMode"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="(50 * nbrOfColumns + 25) / 3 + 'em'"
        transition="dialog-transition"
      >
        <gmt-dcoa-form
          @UpdateRelations="UpdateCOA"
          :key="rndVar"
          :selectedDonorId="donorId"
          :selectedResourceId=-1 
          :alreadyInserted="dcoajcoaList"
        ></gmt-dcoa-form>
      </v-dialog>


    </template>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../../i18n";
import { EventBus } from "../../../../../event-bus";
import JDCOASearch from "./../../../DONORS/COA/GmtDCoaSearchForm.vue";

import { ImsApi, ImsLookupsApi, Configuration } from "../../../../../axiosapi";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../../axiosapi/api";
interface IDATA {
  COI_ID: number;
  COI_DESCR: string;
  TOS_ID: number;
  TOS_DESCR: string;
  COA_ID: number;
  COA_DESCR: string;
  DONOR_ID: number;
  GRANT_ID: number;
  GMT_ACTIVITY_ID: number;
  PMS_ACT_ID: number;
}
export default Vue.extend({
  props: {
    formData: {
      type: Object,
      required: true,
    },
    donorId: {
      type: Number,
      required: true,
    },
    grantId: {
      type: Number,
      required: true,
    },
    presetVal: {
      type: String,
      required: false,
      default: "",
    },
  },

  components: {
    "gmt-dcoa-form": JDCOASearch,
   
  },
  data() {
    return {
      showWait: false,
   
      dcoajcoaList: [],
  
      rndVar: 0,
      nbrOfColumns: 2,
      editCOAMode: false,
    
      donorFormData: {} as IDATA,
      formValid: false,
      rules: {
        required: [(value: any) => !!value || "Required."],
      },
    };
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    deleteItem(item: any) {},
    close: function () {
      let localThis = this as any;
      //   (localThis.$refs.myFormCOI as Vue & { reset: () => void }).reset();
      this.$emit("closeDialoge");
    },

  
    UpdateCOA(item: any) {
      let localThis: any = this as any;
      localThis.closeCOAEdit();
      if (item == null) return;
      localThis.donorFormData.COA_ID = item.ACC_ID;
      localThis.donorFormData.COA_DESCR = item.ACC_CODE + "-" + item.ACC_DESCRIPTION;
    },
   
    openCOADialog() {
      let localThis: any = this as any;
      localThis.editCOAMode = !localThis.editCOAMode;
      localThis.dcoajcoaList = [];
      localThis.rndVar = Math.ceil(Math.random() * 1000);
    },

    closeCOAEdit() {
      let localThis = this as any;
      localThis.editCOAMode = false;
    },

    save() {
      let localThis: any = this as any;
      if (
        // localThis.donorFormData.TOS_ID == null ||
        // localThis.donorFormData.COI_ID == null ||
        // localThis.donorFormData.COA_ID == null
      localThis.donorFormData.COA_ID == null
      ) {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Fill all the fields",
        });
        return;
      }
      // let msg: string = this.$i18n
      //   .t("Confirm the resource mapping ?")
      //   .toString();
      let msg: string ="Confirm the resource mapping ?";
      localThis.$confirm(msg).then((res: any) => {
        if (res) {
          localThis.donorFormData.GMT_ACTIVITY_ID=localThis.formData.GMT_ACTIVITY_ID;
          localThis.donorFormData.GRANT_ID= localThis.grantId;

          localThis.$emit("addRelation", localThis.donorFormData);
        }
      });
    },
  },
  mounted() {
    let localThis = this as any;
    localThis.donorFormData = {} as IDATA;
    localThis.donorFormData.DONOR_ID = localThis.donorId;
   
  },
});
</script>

<style scoped></style>
