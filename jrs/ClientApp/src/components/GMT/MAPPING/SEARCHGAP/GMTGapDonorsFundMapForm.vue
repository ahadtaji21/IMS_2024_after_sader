<template>
  <!-- componente che carica e mostra la lista dei donors che hanno grants sul progetto  -->

  <v-row :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
    <v-col
      v-for="(donor, n) in donorList"
      :key="n"
      :class="'rowStyleNoPadUD'"
      :cols="$vuetify.breakpoint.xsOnly ? 12 : 12"
    >
      <gmt-donor-item
        :donorOriginal="donor"
        :formData="formData"
        :month="month"
        :year="year"
        :RefreshAfterModifies="RefreshAfterModifies"
   
      ></gmt-donor-item>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { i18n } from "../../../../i18n";
import { EventBus } from "../../../../event-bus";
import DonorItem from "./GMTGapDonorFundMapForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";

export default Vue.extend({
  components: {
    "gmt-donor-item": DonorItem,
  },
  props: {
    selectedProjectId: {
      type: Number,
      required: true,
    },
    formData: {
      type: Object,
      required: true,
    },

    year: {
      type: Number,
      required: true,
    },
    month: {
      type: Number,
      required: true,
    },

  },
  data() {
    return {
      donorList:[]
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

    RefreshAfterModifies()
    {
      let localThis:any=this;
      localThis.$emit("RefreshAfterModifies");
    }
  },

  mounted() {
    let localThis = this as any;
    localThis.donorList = localThis.$store.getters.getDonorList;
  },
});
</script>

<style scoped>
</style>