<template>
  <div>
    <div>
      <ap-cc-indicators :onlyRead="onlyRead" :versioned="versioned"> </ap-cc-indicators>
      <!-- <ap-ccs-indicators :onlyRead="onlyRead" :versioned="versioned"> </ap-ccs-indicators> -->
    </div>

    <!-- <div>
      <ap-ccp-indicators :onlyRead="onlyRead" :versioned="versioned"> </ap-ccp-indicators>
    </div> -->
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import JrsTable from "../../JrsTable.vue";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
// import IndicatorsCCS from "../CROSSCUTTING/AnnualPlanCCSIndicatorForm.vue";
// import IndicatorsCCP from "../CROSSCUTTING/AnnualPlanCCPIndicatorForm.vue";
import IndicatorsCC from "../CROSSCUTTING/AnnualPlanCCIndicatorForm.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

export default Vue.extend({
  components: {
    // "ap-ccs-indicators": IndicatorsCCS,
    // "ap-ccp-indicators": IndicatorsCCP,
    "ap-cc-indicators": IndicatorsCC,
  },
  props: {
    // showActivityDetails: {
    //   type: Boolean,
    //   required: true,
    // },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
    onlyRead: {
      type: Boolean,
      required: false,
      default: false,
    },
  },

  data() {
    return {
      index: 0,
      selected: [],
      showWait: false,
      keyRnd: 0,
      bdg: 0,
      resourceId: {},
      json_data: "",
      selectedActivityId: 0,
      selectedActivityItemId: 0,
      selectedResource: "",
      selectedResourceId: 0,
      editObjId: 0,
      showActTabs: true,
      editObjDesc: "",
      editedItemDialog: {},
      editedItem: {},
      editId: null,
      editObj: null,
      showIndicators: false,
      selectedObj: [],
      tableFilter: "",
      tblName: null,
      userrights: null,
      startDate: null,
      endDate: null,
      editMode: false,
      nbrOfColumns: 2,
      isLoading: false,
      headers: [],

      coi: [],
      tos: [],
      objectiveList: [] as any,
      occ: [],
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

    removeUndefined: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      else return string;
    },
  },
  beforeMount() {
    let localThis: any = this as any;
  },
  mounted() {
    let localThis: any = this as any;
  },
  filters: {
    subStr: function (string: any) {
      if ((string + "").toUpperCase() == "UNDEFINED") return "";
      if (string.length > 25) return string.substring(0, 25) + "...";
      else return string;
    },
  },
  computed: {
    language() {
      return i18n.locale;
    },
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
  },
  watch: {
    language(newLanguage: any, oldLanguage: any) {
      let localThis: any = this as any;
    },
  },
});
</script>
<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}

.overall-node {
  margin: 1em 0.5em;
}
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
.selectedCell {
  background-color: whitesmoke;
  color: black;
  font-size: 12px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
  padding: 0px;
  margin: 0px;
}
.notSelectedCell {
  background-color: white;
  font-size: 1px;
  border-style: solid;
  border-width: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 85px;
}

.descClass {
  font-size: 10px;
  width: 150px !important;
  height: 10px;
}
</style>
