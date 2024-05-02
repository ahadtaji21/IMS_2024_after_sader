<template>
  <div>
    <v-data-table
      :headers="translatedHeaders"
      :items="cleanRows"
      :item-key="pkName"
      :search="tableFilter"
      :options.sync="options"
      multi-sort
      class="text-capitalize elevation-2"
      v-model="tableSelectedRows"
      :single-select="!multiSelect"
      :show-select="selectable"
      :server-items-length="serverItemsLength"
      :loading="isLoading"
      @update:options="updateOptions"
      @click:row="handleClick"
    >
      <template v-slot:top>
        <div
          class="d-flex flex-wrap align-center primary lighten-2 jrs-table-top"
          style="border: solid 1px green"
        >
          <h3 v-if="title">{{ title }}</h3>
          <slot name="tableHeader"></slot>

          <!-- SEARCH -->
          <v-spacer v-if="showSearchbar"></v-spacer>
          <v-text-field
            v-if="showSearchbar"
            v-model="tableFilter"
            append-icon="mdi-magnify"
            :label="$t('general.search')"
            hide-details
            clearable
            outlined
            dense
            class="white"
            color="secondary darken-2"
            @input="updateWhereCondition"
          ></v-text-field>
        </div>
      </template>

      <!-- SELECTION SLOT -->
      <template
        v-slot:[`item.data-table-select`]="{ item, isSelected, select }"
        v-if="selectable"
      >
        <v-simple-checkbox
          :value="rowSelectionTrue(isSelected, item)"
          :readonly="false"
          :ripple="false"
          :disabled="rowSelectionDisabled(item)"
          @input="select($event)"
        ></v-simple-checkbox>
      </template>

      <template
        v-for="(col, colIndex) in headers"
        v-slot:[`item.${col.value}`]="{ item }"
      >
        <div :key="'COL_' + col.value">
          <!-- CUSTOM ROW CELLS : DateTimes -->
          <!-- to change date respect local time : $d(new Date(new Date(item[col.value])))  -->
          <template v-if="col.isDateTime">
            <span>{{
              item[col.value] ? formatDate(new Date(item[col.value])) : null
            }}</span>
          </template>

          <!-- CUSTOM ROW CELLS : Checkbox -->
          <template v-else-if="col.isCheckbox">
            <v-icon
              :color="item[col.value] == true ? 'green darken-3' : 'deep-orange darken-3'"
              >{{
                item[col.value] == true
                  ? "mdi-checkbox-marked-circle-outline"
                  : "mdi-checkbox-blank-circle-outline"
              }}</v-icon
            >
          </template>

          <!-- CUSTOM ROW CELLS : text_edit -->
          <template v-else-if="col.type == 'text_edit'">
            <v-tooltip :key="'TMP_TOOLTIP' + colIndex" top color="black">
              <template v-slot:activator="{ on }">
                <span
                  v-on="on"
                  class="d-inline-block"
                  :class="{ 'text-truncate': !col.prevent_truncation }"
                  style="max-width: 25em"
                  >{{ stripFormatting(item[col.value]) }}</span
                >
              </template>
              <div
                :key="`TEXT_EDIT_${colIndex}`"
                v-html="item[col.value]"
                class="pa-10"
                light
                style="max-width: 50em"
              ></div>
            </v-tooltip>
          </template>

          <!-- CUSTOM ROW CELLS : image_picker -->
          <template class="text-center" v-else-if="col.type == 'image_picker'">
            <div class="text-center">
              <v-tooltip :key="'TMP_TOOLTIP' + colIndex" top color="black">
                <template v-slot:activator="{ on }">
                  <span
                    v-on="on"
                    class="d-inline-block text-truncate"
                    style="max-width: 25em"
                  >
                    <v-avatar left size="36">
                      <v-img :src="item[col.value]"></v-img> </v-avatar
                  ></span>
                </template>

                <v-img width="100" :src="item[col.value]"></v-img>
              </v-tooltip>
            </div>
          </template>

          <!-- CUSTOM ROW CELLS : ColorPicker -->

          <template class="text-center" v-else-if="col.type == 'color_picker'">
            <div class="text-center">
              <v-avatar :color="item[col.value]" size="25"> </v-avatar>
            </div>
          </template>

          <!-- CUSTOM ROW CELLS : search_table_multi -->

          <template class="text-center" v-else-if="col.type == 'search_table_multi'">
            <div class="text-center">
              <v-chip-group active-class="primary--text" column>
                <v-chip v-for="tag in item[col.value]" :key="tag[col.itemKey]">
                  {{ tag[col.itemText] }}
                </v-chip>
              </v-chip-group>
            </div>
          </template>
          <!-- CUSTOM ROW CELLS : Default -->
          <template v-else>
            <span>{{ item[col.value] }}</span>
          </template>
        </div>
      </template>

      <!-- ROW ACTIONS -->
      <template v-slot:[`item.action`]="{ item }">
        <div class="d-flex flex-row justify-center">
          <!-- SLOT ACTIONS -->
          <slot name="itemActions" v-bind:rowItem="item"></slot>

          <!-- ICON ACTIONS -->
          <v-icon
            v-for="(action, actionIndex) in iconActions"
            :key="actionIndex"
            :class="action.class"
            @click="action.func(item)"
            @click.stop="handleClick"
            small
            >{{ action.iconCode }}</v-icon
          >
        </div>
      </template>
    </v-data-table>
  </div>
</template>

<script lang="ts">
import Vue, { PropOptions } from "vue";
import { translate, formatDateTime } from "../i18n";
import { JrsHeader, IIconAction } from "../models/JrsTable";
import UtilMix from "../mixins/UtilMix";

export default Vue.extend({
  props: {
    /**
     * Model to be passed as v-model of v-data-table.
     */
    value: {
      type: Array,
      required: false,
    },
    /**
     * Title of the table.
     */
    title: {
      type: String,
      required: false,
    },
    /**
     * Definition of table headers.
     */
    headers: {
      type: Array as () => JrsHeader[],
      required: true,
    },
    /**
     * Rows of data to display in table.
     */
    rows: {
      type: Array,
      required: true,
      // default: () => []
    },
    /**
     * Name of the PK present in the rows. They must be unique.
     */
    pkName: {
      type: String,
      required: true,
    },
    /**
     * List of actions to display using icons.
     */
    iconActions: {
      type: Array as () => IIconAction[],
      required: false,
    },
    /**
     * Defines if searchbar must be rendered.
     */
    showSearchbar: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Is the table selectable.
     */
    selectable: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Is the table a multiselect.
     * WARNING: requires "selectable":true.
     */
    multiSelect: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Number of items in server-side query.
     * Will only be used for server-side pagination.
     */
    serverItemsLength: {
      type: Number,
      required: false,
      default: undefined,
    },
    /**
     * Determines if data is being loaded from server.
     */
    isLoading: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * **WARMING : USED ONLY FOR COMPONENT: JrsMultiSelectedTable
     * If the JrsTable is used fro MultiSelect Search component
     */
    forMultiSelect: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Defines the property name to use to disable selection of a row.
     * WARNING: requires "selectable":true.
     */
    rowDisableSelectRules: {
      type: Array,
      required: false,
      default: () => [],
    },
    /**
     * Defines a list of hidden table columns.
     * The data will not be removed from the datasets, but not shown in the table.
     */
    hiddenColumns: {
      type: Array,
      required: false,
      default: () => [],
    },
    /**
     * allow to edit the item clickin on a cell of the row
     */
    selectOnRowClick: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Select default first item (if is selectable or multi)
     */
    selectFirstRowDeafult: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  mixins: [UtilMix],
  data() {
    return {
      tableFilter: "",
      tableSelectedRows: [],
      options: {
        page: 1,
        itemsPerPage: 15,
        sortBy: [],
        sortDesc: [],
        groupBy: [],
        groupDesc: [],
        mustSort: false,
      },
    };
  },
  computed: {
    /**
     * Transalated header.
     */
    translatedHeaders() {
      return this.headers
        .filter((header: any) => !this.hiddenColumns.includes(header.column_name))
        .map((header: any) => {
          header.text =
            header.translationtKey != undefined
              ? translate(header.translationtKey).toString()
              : header.text;
          return header;
        });
    },

    /**
     * Input rows with necessary updating for localization.
     */
    cleanRows() {
      return this.rows.map((row: any) => {
        return row;
      });
    },
    // // /**
    // //  * List of date-time columns for localization.
    // //  */
    // // DateTimeColumns():JrsHeader[]{
    // //     let h:JrsHeader[] = this.headers as JrsHeader[];
    // //     return h.filter( (header:JrsHeader) => header.isDateTime );
    // // },
    // // /**
    // //  * List of checkbox columns
    // //  */
    // // CheckboxColumns():JrsHeader[]{
    // //     let h:JrsHeader[] = this.headers as JrsHeader[];
    // //     return h.filter( (header:JrsHeader) => header.isCheckbox );
    // // },
    // // TextEditColumns():JrsHeader[]{
    // //     return this.headers.filter( (header:any) => header.type == 'text_edit' );
    // // }
  },
  watch: {
    tableSelectedRows(to: any, from: any) {
      (this as any).updateValue(to);
    },
    cleanRows(to: any, from: any) {
      if ((this as any).selectable) {
        if ((this as any).tableSelectedRows.length == 0) {
          to["isSelected"] = true;
          (this as any).updateValue(to);
        }
      }
    },
  },
  methods: {
    updateValue(newVal: any) {
      // Emit update of v-model to parent component.
      (this as any).$emit("input", newVal);
      },

      formatDate(date: Date | undefined | null): string {
          if (date instanceof Date) {
              const day = date.getDate();
              const month = date.toLocaleString('default', { month: 'short' });
              const year = date.getFullYear();
              return `${day} ${month} ${year}`;
          } else {
              return '';
          }
      },
    /**
     * Emit option changes for use in server-side pagination.
     * @param newOptions - new options for data refresh
     */
    updateOptions(newOptions: any) {
      (this as any).$emit("optionsChange", (this as any).options);
    },

    handleClick(item: any) {
      let localThis: any = this as any;
      if (localThis.selectOnRowClick) {
        localThis.$emit("rowClick", item);
      }
    },

    /**
     * Emit where-condition changes after debouncing the user input.
     */
    updateWhereCondition() {
      let localThis: any = this as any;
      let cond: string | null = null;
      if (localThis.tableFilter) {
        // Update where condition
        let colsCond: Array<string> = localThis.headers
          .filter((col: any) => col.column_name)
          .map(
            (col: any) =>
              `CAST(${col.column_name} AS NVARCHAR(MAX)) LIKE '%${localThis.tableFilter}%'`
          );
        cond = "(" + colsCond.join(" OR ") + ")";
      }
      // Reset page
      localThis.options.page = 1;

      // Emit changes
      (this as any).$emit("filterChange", cond);
    },
    /**
     * Check if the row selection should be disabled.
     * @param {any} row - The row data
     * @returns {Boolean} - Disable seleciton on the row
     */
    rowSelectionDisabled(row: any): boolean {
      if (this.rowDisableSelectRules.length == 0) {
        return false;
      }
      // Check if all rules for disabling selecion are verified
      let ret: boolean = this.rowDisableSelectRules
        .map((func: any) => func(row))
        .reduce((last, curr) => {
          return last && curr;
        });
      return ret;
    },
    rowSelectionTrue(isSelected: any, item: any) {
      if (
        (this as any).tableSelectedRows.length == 0 &&
        (this as any).selectFirstRowDeafult
      ) {
        if (
          item[(this as any).pkName] == (this as any).cleanRows[0][(this as any).pkName]
        ) {
          return true;
        } else {
          return false;
        }
      } else {
        return isSelected;
      }
    },
    stripFormatting(htmlContent: string) {
      let doc = new DOMParser().parseFromString(htmlContent, "text/html");
      return doc.body.textContent || "";
    },
  },
  mounted() {
    let localThis: any = this as any;

    //Setup debounce functionality for table filter
    localThis.updateWhereCondition = localThis.getDebouncedFunc(
      localThis.updateWhereCondition,
      1000
    );
    if (localThis.forMultiSelect) {
      if (localThis.value && localThis.value.length > 0) {
        localThis.value.forEach((ele: any) => {
          localThis.tableSelectedRows.push(
            localThis.rows.find((item: any) => {
              return item[localThis.pkName] == ele[localThis.pkName];
            })
          );
        });
      }
    }
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
