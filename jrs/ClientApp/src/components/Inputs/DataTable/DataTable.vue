<template>
  <div>
    <div class="header">
      <h3>
        {{ headerTitle }}
      </h3>
      <div v-if="storedProcedureName">
        <excel-generate-report
          :withoutHeader="withoutHeader"
          :storedProcedureName="storedProcedureName"
          :PROCEDURE_PARAMETERS="PROCEDURE_PARAMETERS"
          :file_name="file_name"
        />
      </div>
    </div>

    <div class="table-container">
      <table class="table">
        <thead>
          <tr>
            <th
              v-for="col in columns.filter((ele) => !ele.hide)"
              :key="col.id"
              :style="{ width: col.width ? `${col.width}px` : '220px' }"
            >
              <div class="thdiv">
                <p>{{ $t(col.label) }}</p>
                <input
                  v-if="!col.actions"
                  v-model="searchText[col.field]"
                  type="text"
                  placeholder="Filter"
                  @input="(e) => inputChange(e.target.value, col.field)"
                />
              </div>
            </th>
          </tr>
        </thead>
        <tbody v-if="tableData.length > 0">
          <tr v-for="(d, idx) in filteredData" :key="idx">
            <td
              v-for="(col, idx) in columns.filter((ele) => !ele.hide)"
              :key="idx"
              :style="{ width: col.width ? `${col.width}px` : '220px' }"
            >
              <span
                v-if="
                  !col.actions &&
                  !col.toggle &&
                  !col.field.includes('_is_active') &&
                  !col.field.toLowerCase().endsWith('_is_active')
                "
                class="table-cell-p"
              >
                {{ d[col.field] }}
              </span>
              <span
                v-if="col.field && col.field.includes('_is_active')"
                @click="() => handleIsActiveClick(d, col.field)"
                :class="{
                  'active-button': d[col.field],
                  'inactive-button': !d[col.field],
                }"
              >
                {{ d[col.field] ? "Active" : "Inactive" }}
              </span>

              <span class="actions" v-if="col.actions">
                <span
                  @click="() => editRow(d)"
                  class="mdi mdi-circle-edit-outline"
                  v-if="!col.hideEdit"
                >
                </span>
                <span
                  v-if="!col.hideDelete"
                  class="mdi mdi-delete"
                  @click="() => confirmDelete(d)"
                >
                </span>
              </span>
              <span class="toggle" v-if="col.toggle">
                <v-switch
                  v-model="d[col.field]"
                  @change="() => confirmChangeStatus(d)"
                ></v-switch>
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="pagination" v-if="tableData.length > 0 && pagination">
      <div class="left">
        <p>Total number of rows: {{ count }}</p>
      </div>
      <div class="right">
        <div class="select">
          Rows per page:

          {{ limit }}
        </div>
        <div class="text">
          {{ currentPage }} - {{ limit }} of {{ totalPages }}
        </div>
        <v-btn
          color="secondary lighten-2"
          class="paginationBtn"
          :disabled="currentPage <= 1"
          @click="() => prevPage(currentPage - 1)"
        >
          <span class="mdi mdi-chevron-left"></span>
        </v-btn>
        <v-btn
          color="secondary lighten-2"
          class="paginationBtn"
          :disabled="currentPage >= totalPages"
          @click="nextPage"
        >
          <span class="mdi mdi-chevron-right"></span>
        </v-btn>
      </div>
    </div>
  </div>
</template>

<script>
import ExcelGenerateReport from "../../ExcelGenerateReport.vue";

export default {
  props: {
    handleIsActiveClick: {
      type: Function,
    },
    columns: {
      type: Array,
      default: () => [],
    },
    tableData: {
      type: Array,
      default: () => [],
    },
    editRow: {
      type: Function,
    },
    deleteRow: {
      type: Function,
    },
    changeStatus: {
      type: Function,
    },
    getData: {
      type: Function,
    },
    searchPagination: {
      type: Function,
    },
    currentPage: {
      type: Number,
      default: 1,
    },
    totalPages: {
      type: Number,
      default: 2,
    },
    limit: {
      type: Number,
      default: 10,
    },
    pagination: Boolean,
    count: {
      type: Number,
      default: 0,
    },
    headerTitle: {
      type: String,
      required: true,
    },

    storedProcedureName: { type: String },
    PROCEDURE_PARAMETERS: { type: [Array] },
    file_name: { type: String },
    withoutHeader: { type: Boolean, default: false },
  },

  components: {
    "excel-generate-report": ExcelGenerateReport,
  },
  data() {
    return {
      searchText: {},
      filteredData: [],
      searchQuery: [],
      loadingPagging: false,
    };
  },
  methods: {
    nextPage() {
      this.loadingPagging = true;
      this.$emit("getData", this.currentPage + 1);
    },
    prevPage() {
      this.loadingPagging = true;
      this.$emit("getData", this.currentPage - 1);
    },
    inputChange(search_query, search_key) {
      if (this.searchPagination) {
        if (search_query && search_query.length > 1) {
          console.log("1");
          const searchBefore = this.searchQuery.find(
            (ele) => ele.search_key == search_key
          );
          if (searchBefore) searchBefore.search_query = search_query;
          else this.searchQuery.push({ search_key, search_query });
          console.log("searchbefore: ", searchBefore);
        } else {
          this.searchQuery = this.searchQuery.filter(
            (ele) => ele.search_key != search_key
          );
        }

        if (this.searchQuery.length > 0) {
          this.searchPagination(this.searchQuery);
        } else {
          this.$emit("getData", this.currentPage);
        }
      } else {
        this.filteredData = this.tableData.filter((row) => {
          return Object.keys(this.searchText).every((key) => {
            if (!this.searchText[key]) return true;
            return String(row[key])
              .toLowerCase()
              .includes(this.searchText[key].toLowerCase());
          });
        });
      }
    },

    confirmDelete(rowData) {
      if (confirm("Are you sure you want to delete this row?")) {
        this.deleteRow(rowData);
      }
    },
    confirmChangeStatus(rowData) {
      if (confirm("Are you sure you want to change the status of this row?")) {
        this.changeStatus(rowData);
      }
    },
  },
  async mounted() {
    this.filteredData = this.tableData;
  },
  watch: {
    tableData: {
      deep: true,
      handler(newValue) {
        this.filteredData = newValue;
      },
    },
  },
};
</script>

<style scoped>
@import "./DataTable.scss";
.active-button {
  background-color: green;
  color: white;
  padding: 1px 2px;
  border-radius: 12px;
  cursor: pointer;
}

.inactive-button {
  background-color: red;
  color: white;
  padding: 1px 2px;
  border-radius: 10px;
  cursor: pointer;
}
</style>
