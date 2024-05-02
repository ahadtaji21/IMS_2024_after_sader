<template>
  <v-card>
    <v-card>
      <v-container fluid>
        <v-form v-model="formIsValid" ref="form" lazy-validation class="text-capitalize">
  
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 6">
              <v-text-field
                :readonly="onlyRead"
                :label="$t('datasource.pms.annual-plan.ap-description')"
                v-model="formData.DESCRIPTION"
                :counter="150"
                required
              ></v-text-field>
            </v-col>
          </v-row>


        </v-form>
      </v-container>
    </v-card>

    <v-card-actions v-if="isUpdatableForm && !onlyRead">
      <v-btn color="secondary" class="--darken-1" @click="close()">Cancel</v-btn>
      <v-btn color="primary" class="--darken-1" @click="save()">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import {
  ImsApi,
  PmsAnnualPlanApi,
  Configuration
} from "../../../../../axiosapi";
import { i18n } from "../../../../../i18n";

export default Vue.extend({
  components: {},

  props: {
    formData: {
      type: Object
    },
    isUpdatableForm:{
      type:Boolean,
      required:true

    },
    onlyRead:{
      type:Boolean,
      required:false,
      default:false

    }
  },
  data() {
    return {
      formIsValid: false,
  
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    }
  },
  watch: {
   
  },

  mounted() {
 let localThis = this as any;
  },
  methods: {
    close: function() {
      this.$emit("closeDialoge");
    },
    save: function() {
      let localThis = this as any;

      localThis.$emit("closeDialogeAndSave", localThis.formData);
    },

 

   
  }
});
</script>

<style scoped>
</style>