<template>
  <div class="d-flex">
    <v-text-field
      :label="label"
      v-model="parsedValue"
      :hint="hint ? hint : ''"
      :persistent-hint="hint && hint != '' ? true : false"
      :required="is_required"
      :rules="fieldRules"
      :step="step"
      @input="(v) => updateValue(Number(v))"
      type="number"
      class="jrs-number-field"
      prepend-icon="mdi-numeric"
    >
    </v-text-field>
    <!-- <div class="d-flex flex-column">
            <v-btn  outlined x-small fab color="primary" class="mx-2 my-1" @click="increment">
                <v-icon>mdi-plus</v-icon>
            </v-btn>
            <v-btn outlined x-small fab color="primary" class="mx-2 mb-1" @click="decrement">
                <v-icon dark>mdi-minus</v-icon>
            </v-btn>
        </div> -->
    <v-icon @click="clearField()">mdi-close</v-icon>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { translate } from "../i18n";

export default Vue.extend({
  props: {
    value: {
      type: Number,
      required: false,
    },
    label: {
      type: String,
      required: false,
    },
    hint: {
      type: String,
      required: false,
    },
    is_required: {
      type: Boolean,
      required: false,
      default: false,
    },
    /**
     * Number of decimal numbers.
     */
    decimalPrecision: {
      type: Number,
      required: false,
      default: 0,
      validator(value: any) {
        return value >= 0;
      },
    },
  },
  data() {
    return {};
  },
  computed: {
    parsedValue: {
      get() {
        let localThis: any = this as any;
        return localThis.value;
      },
      set(newValue: number) {
        let localThis: any = this as any;
        //localThis.parsedValue = newValue;
      },
    },
    /**
     * Step for number increment/decrement.
     */
    step() {
      let localThis: any = this as any;
      if (localThis.decimalPrecision == 0) {
        return "1";
      }
      let stepValue = "";
      let zeroes = new Array(localThis.decimalPrecision).fill("0");
      zeroes[localThis.decimalPrecision - 1] = "1";

      stepValue = "0." + zeroes.join("");
      return stepValue;
    },
    fieldRules() {
      let localThis: any = this as any;
      let rules = [];

      // Check if the decimal precision is correct
      const precisionRule = (v: number) => {
        let vStr = String(v);

        // If needed add decimal part
        if (vStr.indexOf(".") == -1 && localThis.decimalPrecision > 0) {
          vStr = Number(v).toFixed(localThis.decimalPrecision);
        }

        let precision: number = vStr.split(".")[1] ? vStr.split(".")[1].length : 0;
        return (
          precision <= localThis.decimalPrecision ||
          translate("error.decimal-precision")
            .toString()
            .replace("##PRECISION##", localThis.decimalPrecision)
        );
      };
      rules.push(precisionRule);

      if (localThis.is_required) {
        // const requiredRule = (v:number) => !!v || translate('error.required-field');
        const requiredRule = (v: number) => {
          // !!v || translate('error.required-field')
          const ret: any = !!v || translate("error.required-field");
          return ret;
          // if(v === null || v === undefined){
          //     return translate('error.required-field');
          // }else{
          //     return true;
          // }
        };
        rules.push(requiredRule);
      }

      return rules;
    },
  },
  methods: {
    /**
     * Emit value changes to parent component.
     */
    updateValue(newVal: number) {
      // if(typeof newVal == "string"){
      //     newVal = Number(newVal);
      // }
      (this as any).$emit("input", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;
      localThis.updateValue(null);
    },
    // /**
    //  * Increment value by step and emit update
    //  */
    // increment() {
    //     let localThis:any = this as any;
    //     let precision = Math.pow(10, localThis.decimalPrecision)

    //     let newVal = ((localThis.parsedValue * precision) + 1) / precision;
    //     localThis.updateValue(newVal);

    // },
    // /**
    //  * Decrement value by step and emit update
    //  */
    // decrement () {
    //     let localThis:any = this as any;
    //     let precision = Math.pow(10, localThis.decimalPrecision)

    //     let newVal = ((localThis.parsedValue * precision) - 1) / precision;
    //     localThis.updateValue(newVal);
    // }
  },
});
</script>

<style>
.jrs-number-field input[type="number"] {
  -webkit-appearance: textfield;
  -moz-appearance: textfield;
  appearance: textfield;
}
.jrs-number-field input[type="number"]::-webkit-inner-spin-button,
.jrs-number-field input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
}
</style>
