<template>
  <div class="additionalTab">
    <h2 v-if="gridTitle" class="title">{{ gridTitle }}</h2>
   
    <div class="additionalForm">

      <form-comp v-if="inputs" :inputs="inputs" :theData="theData" />
      
      <div class="buttons" v-if="submitRoute">
        <v-btn color="secondary lighten-2" @click="addAction">
          {{ grid ? "Save" : "Update" }}
        </v-btn>
      </div>
    </div>

    <div v-if="gridData && gridData.length > 0">
      <data-table-custom
        :tableData="gridData"
        :headerTitle="gridTitle"
        :count="gridData.length"
        :columns="gridColumns"
        :deleteRow="deleteRow"
        :changeStatus="changeStatus"
        :storedProcedureName="storedProcedureName"
      />
    </div>
    <div class="buttons" v-if="showBack">
      <v-btn color="primary lighten-2" @click="backAction"> Back </v-btn>
    </div>
  </div>
</template>

<script>
import Form from "../Form/Form.vue";
import DataTable from "../DataTable/DataTable.vue";
import { mapActions, mapGetters } from "vuex";

export default {
  components: {
    "form-comp": Form,
    "data-table-custom": DataTable,
  },
  props: {
    gridTitle: {
      type: String,
    },
    submitRoute: {
      type: String,
    },
    onChangeTab: {
      type: Function,
    },
    inputs: {
      type: Array,
    },
    theData: {
      type: Object,
    },
    gridData: {
      type: Array,
    },
    gridColumns: {
      type: Array,
    },
    backAction: {
      type: Function,
    },
    routeDelete: {
      type: String,
    },
    routeChangeStatus: {
      type: String,
    },
    grid: {
      type: Boolean,
    },
    storedProcedureName: {
      type: String,
    },
    idName: {
      type: String,
    },
    idValue: {
      type: String,
    },
    showBack: {
      type: Boolean,
    },
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getConfiguration: "getConfiguration",
    }),

    async addAction() {
      const filteredBody = {};
      const body = this.theData;
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
          name: this.submitRoute,
          body: filteredBody,
          post: true,
        });

        this.onChangeTab();
        this.showNewSnackbar({
          typeName: "success",
          text: "Completed successfully",
        });
      } catch (ex) {
        this.showNewSnackbar({ typeName: "error", text: ex });
      }
    },

    async deleteRow(row) {
      if (this.routeDelete && this.routeDelete != "") {
        const primaryCol = this.gridColumns.find((ele) => ele.primary);
        const body = {};
        body[primaryCol.field] = row[primaryCol.field];
        try {
          const response = await this.getConfiguration({
            name: this.routeDelete,
            body: body,
            post: true,
          });

          this.onChangeTab();
          this.showNewSnackbar({
            typeName: "success",
            text: "Delete operation completed successfully",
          });
        } catch (ex) {
          this.showNewSnackbar({ typeName: "error", text: ex });
        }
      }
    },
    async changeStatus(row) {
      if (this.routeChangeStatus && this.routeChangeStatus != "") {
        const primaryCol = this.gridColumns.find((ele) => ele.primary);
        const body = {};
        body[primaryCol.field] = row[primaryCol.field];
        try {
          const response = await this.getConfiguration({
            name: this.routeChangeStatus,
            body: body,
            post: true,
          });

          this.onChangeTab();
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
