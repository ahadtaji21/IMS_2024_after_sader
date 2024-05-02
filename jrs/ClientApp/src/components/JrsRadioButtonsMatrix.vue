<template>
  <div>
    <!-- <div>INNER_VALUE: {{JSON.stringify(value)}}</div> -->

    <v-row>
      <v-col>
        <div>
          <v-icon>mdi-view-list</v-icon>
          <p>{{ label }}</p>
        </div>
      </v-col>
    </v-row>

    <tbody style="border: solid">
      <tr >
        <td class="tablerow">
          <div></div>
        </td>
        <td
          class="tablerow"
          v-for="(item, itemIndex) in matrixItems[0]"
          :key="`col_${itemIndex}`"
        >
          <div>
            {{ item[itemText] }}
          </div>
        </td>
      </tr>
      <tr v-for="(itemRow, indexRow) in matrixItems" :key="`row_${indexRow}`" >
        <!-- <v-radio-group
        v-model="fieldValue[indexRow]"
      
        :hint="hint ? hint : ''"
       
        :multiple="multiple"
       
        :required="is_required"
        :rules="fieldRules"
        
      > -->
        <!-- <tr > -->
        <td class="tablerow" style="vertical-align: midde !important;">
          <div>
            {{ itemRow[0].matrix_code }}
          </div>
        </td>

        <td
          class="tablerow"
          v-for="(item, itemIndex) in itemRow"
          :key="`col_${indexRow * 100 + itemIndex}`"
         style="vertical-align: middle !important;"
        >
          <div class="rgrp">
          <v-radio-group
            v-model="fieldValue[indexRow]"
            :hint="hint ? hint : ''"
            :multiple="multiple"
            hide-details
   
            :required="is_required"
            :rules="fieldRules"
            class="rgrp"
           
          >
        
            <v-radio
              :key="`radio_${indexRow * 100 + itemIndex}`"
              :value="item[itemKey]"
              @click="changedRadioState(item[itemKey], $event)"
              :name="`RADIO_${indexRow * 100 + itemIndex}`"
              hide-details
              class="justify-center"
            >
           
            </v-radio>
            
          </v-radio-group>
           </div>
        </td>

        <!-- </tr> -->
        <!-- </v-radio-group> -->
      </tr>
    </tbody>

    <!-- <v-row >
      <v-col >
       
      </v-col>
      <v-col v-for="(item, itemIndex) in matrixItems[0]" :key="`col_${itemIndex}`">
        <div >
          {{ item[itemText] }}
        </div>
      </v-col>
    </v-row>

    <v-row v-for="(itemRow, indexRow) in matrixItems" :key="`row_${indexRow}`" >
      <v-col >
      <v-radio-group
        v-model="fieldValue[indexRow]"
      
        :hint="hint ? hint : ''"
       
        :multiple="multiple"
       
        :required="is_required"
        :rules="fieldRules"
        
      >
      <v-row >
        <v-col >
          <div >
            {{ itemRow[0].matrix_code }}
          </div>
        </v-col>
        <v-col
          v-for="(item, itemIndex) in itemRow"
          :key="`col_${indexRow * 100 + itemIndex}`"
          style="border: solid; color: gainsboro;margin:0;padding:0;"
        >
          <v-radio
            :key="`radio_${indexRow * 100 + itemIndex}`"
            :value="item[itemKey]"
            @click="changedRadioState(item[itemKey], $event)"
            :name="`RADIO_${indexRow * 100 + itemIndex}`"
        
          >
          </v-radio>
        </v-col>
      </v-row>
      </v-radio-group>
    </v-col>
    </v-row> -->

    <!-- <template v-slot:append>
        <v-icon @click="clearField">mdi-close</v-icon>
      </template> -->
  </div>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  props: {
    value: {
      type: [String, Number, Array],
      required: false,
      default: () => null,
    },
    items: {
      type: Array,
      required: true,
    },
    itemKey: {
      type: String,
      required: true,
    },
    itemText: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      required: false,
      default: null,
    },
    hint: {
      type: String,
      required: false,
      default: null,
    },
    is_required: {
      type: Boolean,
      required: false,
      default: false,
    },
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
    multiple: {
      type: Boolean,
      required: false,
      default: false,
    },
    matrixId: {
      type: Number,
      required: false,
      default: 0,
    },
  },
  data: () => ({
    receivedValue: [],
    matrixItems: [],
  }),
  computed: {
    fieldValue: {
      get() {
        const localThis: any = this as any;
        // Override return for values when "multiple"
        // if (localThis.multiple) {

        let i: number = 0;
        if (localThis.receivedValue.length == 0) {
          for (i = 0; i < localThis.matrixItems.length; i++) {
            localThis.receivedValue.push([]);
          }
        }

        for (i = 0; i < localThis.matrixItems.length; i++) {
          let itemRow = localThis.matrixItems[i];
          let j: number = 0;
          for (j = 0; j < itemRow.length; j++) {
            if (itemRow[j].id == localThis.value) {
              localThis.receivedValue[i] = localThis.value;
            }
          }
        }
        return localThis.receivedValue;

        // if (receivedValue==null || receivedValue.indexOf(localThis.itemKey) == -1) receivedValue.push(localThis.itemKey);
        // return receivedValue.map((item: any) => item[localThis.itemKey]);
        // }

        // return localThis.value;
      },
      set(newVal: any) {
        const localThis: any = this as any;

        let i: number = 0;
        if (localThis.receivedValue.length == 0) {
          for (i = 0; i < localThis.matrixItems.length; i++) {
            localThis.receivedValue.push([]);
          }
        }

        for (i = 0; i < localThis.matrixItems.length; i++) {
          let itemRow = localThis.matrixItems[i];
          let j: number = 0;
          for (j = 0; j < itemRow.length; j++) {
            if (itemRow[j].id == newVal) {
              localThis.receivedValue[i] = newVal;
            }
          }
        }
        localThis.updateValue(newVal);
      },
    },
    fieldRules() {
      const localThis: any = this as any;
      let rules = [];

      // Check required field
      if (localThis.is_required) {
        const requiredRule = (v: number) => !!v || localThis.$t("error.required-field");
        rules.push(requiredRule);
      }
      return rules;
    },
  },
  beforeMount() {
    const localThis: any = this as any;
    let i: number = 0;
    if (localThis.items == undefined || localThis.items == null) return;
    var rows = [];
    //get rows
    for (i = 0; i < localThis.items.length; i++) {
      var row = localThis.items[i].matrix_row;
      if (rows.indexOf(row) === -1) {
        rows.push(row);
      }
    }
    if (rows.length == 0) return;
    let j: number = 0;
    //sort rows
    rows = rows.sort(localThis.compare);

    for (j = 0; j < rows.length; j++) {
      let matrixRow = [];
      for (i = 0; i < localThis.items.length; i++) {
        if (localThis.items[i].matrix_row == rows[j]) {
          matrixRow.push(localThis.items[i]);
        }
      }
      //sort column in a row
      let k: number = 0;
      // var columns = [];
      // for (k = 0; k < tmpmatrixRow.length; k++) {
      //   var column = localThis.items[i].matrix_column;
      //   if (columns.indexOf(column) === -1) {
      //     columns.push(column);
      //   }
      // }
      //  columns = columns.sort(localThis.compare);
      //  let q: number = 0;
      // let matrxRow = [];
      // for(q=0;q<columns.length;q++)
      // {

      // }
      matrixRow = matrixRow.sort(localThis.compareColumns);

      localThis.matrixItems.push(matrixRow);
    }
  },

  methods: {
    compare(a: any, b: any) {
      if (a < b) return -1;
      if (a > b) return 1;
      return 0;
    },

    compareColumns(a: any, b: any) {
      if (a.matrix_column < b.matrix_column) return -1;
      if (a.matrix_column > b.matrix_column) return 1;
      return 0;
    },
    /**
     * Emit new field data.
     */
    updateValue(newVal: any) {
      const localThis: any = this as any;
      // Emit update of v-model to parent component.
      if (localThis.multiple) {
        const selectedObject: Array<any> = localThis.items.filter((item: any) =>
          newVal.includes(item[localThis.itemKey])
        );
        localThis.$emit("input", selectedObject);
      } else {
        //localThis.$emit("input",null);
        localThis.$emit("input", JSON.stringify(localThis.receivedValue));
      }
    },
    /**
     * Clear field data.
     */
    clearField() {
      const localThis: any = this as any;
      localThis.updateValue(null);
    },
    changedRadioState(radioValue: any, evt: any) {
      const localThis: any = this as any;
      // Clear selection if necessary
      if (localThis.multiple) {
        const itemIndex: number = localThis.fieldValue.indexOf(radioValue);
        // Add or remove value from selected Values
        if (itemIndex == -1) {
          localThis.fieldValue = [...localThis.fieldValue, radioValue];
        } else {
          localThis.fieldValue.splice(itemIndex, 1);
          localThis.fieldValue = [...localThis.fieldValue];
        }
      } else {
        localThis.fieldValue = radioValue;
      }
    },
  },
});
</script>

<style scoped>
.tablerow {
  text-align: center;
  vertical-align: middle;
  padding: 0;
  border: solid;
  border-color: whitesmoke;
  margin: 0;
}
.rgrp {
  color:green !important;
  vertical-align: top !important;
  padding: 0;
}
</style>
