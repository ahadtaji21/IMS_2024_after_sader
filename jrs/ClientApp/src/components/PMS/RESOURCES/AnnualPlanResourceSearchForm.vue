<template>
  <div>
    <v-card>
      <v-card-title></v-card-title>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="resourceList"
          item-key="PRIMARY_KEY"
          multi-sort
          :search="tableFilter"
          :items-per-page="20"
          class="text-capitalize"
          v-model="selectedRes"
          show-select
          :single-select="true"
        >
          <template v-slot:top>
            <div class="d-inline-flex align-center primary lighten-2 jrs-table-top">
              <h3>{{ $t("datasource.pms.annual-plan.objectives.resources") }}</h3>

              <v-spacer></v-spacer>

              <v-text-field
                v-model="tableFilter"
                append-icon="mdi-magnify"
                :label="$t('general.search')"
                hide-details
                clearable
                outlined
                dense
                class="white"
                color="secondary darken-2"
              ></v-text-field>
            </div>
          </template>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-btn color="secondary darken-1" text @click="clearField()"
          >X {{ $t("general.close") }}</v-btn
        >
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { JrsHeader } from "../../../models/JrsTable";
import { i18n } from "../../../i18n";
import { EventBus } from "../../../event-bus";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";
export default Vue.extend({
  props: {
    selectedPositionId: {
      type: Number,
      default: 0,
      required: false,
    },
    selectedServiceId: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  data() {
    return {
      headers: [],
      resourceList: [],
      tableFilter: "",
      selectedRes: [],
      itemKey: "",
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
    setupHeaders() {
      let localThis: any = this as any;
      let tmp: JrsHeader[] = [
        {
          text: this.$i18n.t("datasource.hrm.position.department").toString(),
          value: "HR_DEPARTMENT_DESCR",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("datasource.hrm.position.title").toString(),
          value: "HR_POS",
          align: "center",
          divider: true,
        },

        {
          text: this.$i18n.t("datasource.hrm.position.descr").toString(),
          value: "HR_POSITION_DESCR",
          align: "center",
          divider: true,
        },

        {
          text: this.$i18n.t("general.last-name").toString(),
          value: "HR_BIODATA_SURNAME",
          align: "center",
          divider: true,
        },
        {
          text: this.$i18n.t("general.name").toString(),
          value: "HR_BIODATA_NAME",
          align: "center",
          divider: true,
        },
      ];

      localThis.headers = tmp;
    },
    getServiceResources() {
      let localThis: any = this as any;
      localThis.selectedResource = null;
      localThis.resourceList = [];
      localThis.resourceListOrig = [];

      let i: number = 0;
      let j: number = 0;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "PMS_VI_ANNUAL_PLAN_USER_POSITIONS",
        colList: null,
        whereCond: `(${localThis.selectedPositionId}=0 OR HR_POSITION_ID = ${localThis.selectedPositionId}) AND SERVICE_ID=${localThis.selectedServiceId}`, // AND IMS_LANGUAGE_LOCALE='${i18n.locale}'`,
        orderStmt: null, // "ID"
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              localThis.resourceList.push(item);
            });
          }
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err });
        });
    },
    updateValue(newVal: String) {
      (this as any).$emit("UpdateResource", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;

      localThis.updateValue(null);
    },
  },
  watch: {
    selectedRes(to: Array<any>, from: Array<any>) {
      let localThis: any = this as any;
      if (to.length > 0) {
        let msg: string = this.$i18n
          .t("datasource.pms.annual-plan.add-resource")
          .toString();

        this.$confirm(msg).then((res: any) => {
          if (res) {
            localThis.updateValue(to[0].PRIMARY_KEY);
          }
        });
        //localThis.selectedRes = [];
      }
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.setupHeaders();
    localThis.selectedRes = [];
    localThis.getServiceResources();
  },
});
</script>
<style scoped>
.jrs-table-top {
  width: 100%;
  height: 3.5em;
  padding: 0 1em;
  border-top-left-radius: 5px;
  border-top-right-radius: 5px;
}
</style>
