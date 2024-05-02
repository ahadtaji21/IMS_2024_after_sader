<template>
  <v-card>
    <div class="text-center" v-if="showWait" style="margin: 0px; padding: 0px">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>
    <v-card>
      <v-container fluid>
        <v-form
          v-model="formValid"
          ref="myForm"
          lazy-validation
          class="text-capitalize"
        >
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <v-select
                :items="grants"
                :label="$t('datasource.gmt.grant.gt')"
                item-text="text"
                item-value="value"
                :loading="isLoading"
                v-model="formData.GRANT_ID"
                :rules="[(v) => !!v || 'Item is required']"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col :cols="$vuetify.breakpoint.xsOnly ? 12 : 12">
              <gt-search-prj
                v-model="formData.PROJECT_ID"
                @UpdateItem="closeDialog"
                :selectedGrantId="parseInt(formData.GRANT_ID)"
                :key="Math.ceil(Math.random() * 1000)"
                :CloseBtn="false"
              ></gt-search-prj>
            </v-col>
          </v-row>
        </v-form>
      </v-container>
    </v-card>
    <v-card-actions>
      <v-btn color="secondary darken-1" text @click="close()"
        >X {{ $t("general.close") }}</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { ImsApi, ImsLookupsApi, Configuration } from "../../../../axiosapi";
import SearchTable from "../../GRANTS/FUNDED/GMTGrantProjectsSearch.vue";
import { IPayLoadDataPRJFUND } from "./../../../PMS/PMSSharedInterfaces";
import { i18n } from "../../../../i18n";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../../axiosapi/api";
export default Vue.extend({
  components: {
    "gt-search-prj": SearchTable,
  },

  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      showWait: false,
      formValid: false,
      formData: {},
      grants: [],
      isLoading: false,
    };
  },
  computed: {
    userLocale() {
      return i18n.locale;
    },
  },
  watch: {},
  beforeMount() {
    let localThis = this as any;
    if (localThis.grants.length == undefined || localThis.grants.length == 0)
      localThis.getGRANTS();
  },
  mounted() {
    let localThis = this as any;
    localThis.formData.DONOR_ID = localThis.selectedObjectId;
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    close: function () {
      this.$emit("closeDialoge");
    },

    closeDialog(item: string) {
      let localThis = this as any;
      localThis.dialog = false;
      if (item == null) return;

      let payLoadD = {} as IPayLoadDataPRJFUND;
      payLoadD.ID_GRANT = localThis.formData.GRANT_ID;
      payLoadD.ID_PROJECT = Number.parseInt(item);

      let savePayload: GenericSqlPayload = {
        spName: "GMT_SP_INS_GRANT_PROJECT",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(payLoadD),
      };
      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          this.$emit("UpdateItem");
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: err.message,
          }); // Feedback to user
        });
    },
    getGRANTS() {
      let localThis = this as any;
      localThis.showWait = true;
      localThis.grants = [];

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "GMT_VI_DONOR_GRANT",
        //colList: null,
        whereCond: `DONOR_ID=${this.selectedObjectId}`,
        orderStmt: "TEXT",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              let itm: any = {};
              itm.text = item.TEXT;
              itm.value = item.VALUE;
              localThis.grants.push(itm);
            });
          }
          localThis.showWait = false;
        });
    },
  },
});
</script>

<style scoped>
</style>