<template>
  <div>
    <div class="theStepContainer" v-if="tab && loaded">
      <h2>{{ tab.name }}</h2>
      <form-comp
        v-if="tab.theData"
        :inputs="tab.inputs"
        :theData="tab.theData"
      />

      <div class="buttons" v-if="tab.submitRoute">
        <v-btn
          v-if="!tab.additionalTabs"
          color="secondary lighten-2"
          @click="backAction"
        >
          Back
        </v-btn>

        <v-btn
          color="secondary lighten-2"
          @click="addAction"
          v-if="tab.submitRoute"
          :disabled="tab.saveDisabled"
        >
          {{ tab.grid ? "Save" : "Update" }}
        </v-btn>
      </div>

      <div v-if="tab.gridData && tab.gridData.length > 0">
        <data-table-custom
          :tableData="tab.gridData"
          :headerTitle="tab.name"
          :count="tab.gridData.length"
          :columns="tab.gridColumns"
          :deleteRow="deleteRow"
          :changeStatus="changeStatusRow"
          :storedProcedureName="tab.storedProcedureName"
        />
      </div>

      <tab-additional
        v-for="(atab, index) in tab.additionalTabs"
        :key="index"
        :gridTitle="atab.gridTitle"
        :submitRoute="atab.submitRoute"
        :inputs="atab.inputs"
        :theData="atab.theData"
        :gridData="atab.gridData"
        :gridColumns="atab.gridColumns"
        :routeDelete="atab.routeDelete"
        :routeChangeStatus="atab.routeChangeStatus"
        :grid="atab.grid"
        :storedProcedureName="atab.storedProcedureName"
        :backAction="backAction"
        :idName="idName"
        :idValue="idValue"
        :onChangeTab="onChangeTab"
        :showBack="index == tab.additionalTabs.length - 1"
      />
    </div>
    <div v-if="!loaded">
      <div class="spinner"></div>
    </div>
  </div>
</template>
<script>
import Form from "../Form/Form.vue";
import DataTable from "../DataTable/DataTable.vue";
import TabAdditional from "./TabAdditional.vue";
import { mapActions, mapGetters } from "vuex";
export default {
  components: {
    "form-comp": Form,
    "data-table-custom": DataTable,
    "tab-additional": TabAdditional,
  },
  data() {
    return {
      loaded: true,
    };
  },
  props: {
    tab: {
      type: Object,
    },
    currentTab: {
      type: Number,
    },
    backAction: {
      type: Function,
    },
    idName: {
      type: String,
    },
    idValue: {
      type: String,
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getConfiguration: "getConfiguration",
    }),

    async onChangeTab(tab) {
      //The variable resData is used to pass into the afterGet function included in the tab properties
      let resData = [];
      this.loaded = false;
      if (!tab) tab = this.tab;
      if (tab.routeGet && tab.routeGet != "") {
        let objData = await this.getConfiguration({
          name: tab.routeGet,
          body: tab.bodyGet,
          post: true,
        });
        resData.push(objData);
        if (tab.grid) {
          tab.gridData = objData;
          for (const key in tab.theData) {
            tab.theData[key] = "";
          }
        } else {
          const ta = tab.theData;
          if (objData.length == 1) objData = objData[0];
          for (let [key, value] of Object.entries(ta)) {
            const innnp = tab.inputs.find((ele) => ele.model == key);
            const type = innnp.type;
            if (type == "date") {
              tab.theData[key] = objData[key]
                ? new Date(objData[key]).toISOString().substr(0, 10)
                : "";
            } else {
              tab.theData[key] = objData[key]
                ? objData[key].toString()
                : type == "radio"
                ? "false"
                : "";
            }

            if (type == "select" && innnp.onChange) {
              console.log("innnp.onChange: ", innnp.onChange);
              innnp.onChange(objData[key]);
            }
          }
        }
      }

      if (tab.additionalTabs && tab.additionalTabs.length > 0) {
        for (let i = 0; i < tab.additionalTabs.length; i++) {
          const draft_tab = tab.additionalTabs[i];
          let objData2 = await this.getConfiguration({
            name: draft_tab.routeGet,
            body: draft_tab.bodyGet,
            post: true,
          });
          resData.push(objData2);
          if (draft_tab.grid) {
            draft_tab.gridData = objData2;
            for (let k = 0; k < draft_tab.gridData.length; k++) {
              const dda = draft_tab.gridData[k];
              for (const key in dda) {
                if (
                  draft_tab.gridColumns.find((ele) => ele.field == key) &&
                  draft_tab.gridColumns.find((ele) => ele.field == key).date
                ) {
                  dda[key] = new Date(dda[key]).toISOString().substr(0, 10);
                }
              }
            }

            for (const key in draft_tab.theData) {
              draft_tab.theData[key] = "";
            }
          } else {
            const ta2 = draft_tab.theData;
            for (let [key, value] of Object.entries(ta2)) {
              if (
                draft_tab.inputs.find((ele) => ele.model == key).type == "date"
              ) {
                draft_tab.theData[key] = objData2[key]
                  ? new Date(objData2[key]).toISOString().substr(0, 10)
                  : "";
              } else {
                draft_tab.theData[key] = objData2[key]
                  ? objData2[key].toString()
                  : "";
              }
            }
          }
        }
      }

      if (tab.afterGet) {
        tab.afterGet(resData);
      }
      this.loaded = true;
    },

    async addAction() {
      const filteredBody = {};
      const body = this.tab.theData;
      body[this.idName] = this.idValue;

      for (const key in body) {
        if (body[key] !== "") {
          filteredBody[key] = body[key];
        }
      }

      filteredBody[this.idName] = this.idValue;
      filteredBody.userId = this.userId;
      try {
        const response = await this.getConfiguration({
          name: this.tab.submitRoute,
          body: filteredBody,
          post: true,
        });
        this.$parent.getData(this.currentPage);
        // Reset the data for the current tab
        this.$parent.resetTabData();
        // this.onChangeTab(this.tab);
        this.showNewSnackbar({
          typeName: "success",
          text: "Completed successfully",
        });
      } catch (ex) {
        this.showNewSnackbar({ typeName: "error", text: ex });
      }
    },
    async deleteRow(row) {
      if (this.tab.routeDelete && this.tab.routeDelete != "") {
        const primaryCol = this.tab.gridColumns.find((ele) => ele.primary);
        const body = {};
        body[primaryCol.field] = row[primaryCol.field];
        try {
          const response = await this.getConfiguration({
            name: this.tab.routeDelete,
            body: body,
            post: true,
          });

          this.onChangeTab(this.tab);
          this.showNewSnackbar({
            typeName: "success",
            text: "Delete operation completed successfully",
          });
        } catch (ex) {
          this.showNewSnackbar({ typeName: "error", text: ex });
        }
      }
    },
    async changeStatusRow(row) {
      if (this.tab.routeChangeStatus && this.tab.routeChangeStatus != "") {
        const primaryCol = this.tab.gridColumns.find((ele) => ele.primary);
        const body = {};
        body[primaryCol.field] = row[primaryCol.field];
        try {
          const response = await this.getConfiguration({
            name: this.tab.routeChangeStatus,
            body: body,
            post: true,
          });

          this.onChangeTab(this.tab);
          this.showNewSnackbar({
            typeName: "success",
            text: "Row has changed its status successfully",
          });
        } catch (ex) {
          this.onChangeTab(this.tab);
          this.showNewSnackbar({ typeName: "error", text: ex });
        }
      }
    },
  },
  async mounted() {
    this.onChangeTab(this.tab);
    this.getData(this.currentPage);
    // Reset the data for the current tab
    this.resetTabData();
  },
  watch: {
    currentTab() {
      this.onChangeTab(this.tab);
    },
  },

  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
    }),

    userId() {
      return this.getUserUid;
    },
  },
};
</script>
<style scoped>
@import "./Tab.scss";
</style>
