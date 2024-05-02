<template>
  <v-menu
    ref="open"
    v-model="open"
    :close-on-content-click="false"
    :nudge-right="40"
    transition="scale-transition"
    offset-y
    min-width="290px"
  >
    <template v-slot:activator="{ on }">
      <v-text-field
        v-model="formattedDate"
        prepend-icon="mdi-calendar"
        :dense="dense"
        readonly
        v-on="on"
        :label="label"
        :hint="hint ? hint : ''"
        :persistent-hint="hint && hint != '' ? true : false"
        :required="is_required"
        :rules="is_required ? requiredTextFieldRule : []"
      >
        <template v-slot:append>
          <v-icon @click="clearField">mdi-close</v-icon>
        </template>
      </v-text-field>
    </template>

    <v-date-picker
      v-if="is_only_month"
      type="month"
      :disabled="readonly"
      :value="datePickerDate"
      no-title
      @input="updateDate($event)"
    ></v-date-picker>
    <v-date-picker
      v-else-if="is_only_year"
      reactive
      ref="picker"
      :max="`${new Date().getFullYear() + 15}0101`"
      min="1900-01-01"
      show-current
      :disabled="readonly"
      :value="datePickerDate"
      no-title
      @input="updateDate($event)"
    ></v-date-picker>
    <v-date-picker
      v-else
      :disabled="readonly"
      :value="datePickerDate"
      no-title
      @input="updateDate($event)"
    ></v-date-picker>
  </v-menu>
</template>

<script lang="ts">
import Vue from "vue";
import { formatDateTime, translate } from "../i18n";

export default Vue.extend({
  props: {
    value: {
      type: [Date, String, Number],
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
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
    is_only_year: {
      type: Boolean,
      required: false,
      default: false,
    },
    is_only_month: {
      type: Boolean,
      required: false,
      default: false,
    },
    dense: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      open: false,
    };
  },
  watch: {
    open(val) {
      let localThis: any = this as any;
      if (localThis.is_only_year) {
        val &&
          this.$nextTick(
            () => ((this.$refs.picker as any).activePicker = "YEAR")
          );
      }
    },
  },
  computed: {
    dateValue() {
      let localThis: any = this as any;
      let recievedValue = localThis.value;
      if (!localThis.is_only_year) {
        let newValue: Date =
          typeof recievedValue == "string"
            ? new Date(recievedValue)
            : recievedValue;
        return newValue;
      } else {
        let newValue: Date =
          typeof recievedValue == "number" ? new Date() : recievedValue;
        return newValue;
      }
    },
    formattedDate: {
      get() {
        let localThis: any = this as any;
        if (!localThis.is_only_year) {
          return localThis.dateValue
            ? localThis.formatDate(localThis.dateValue)
            : null;
        } else {
          if (typeof localThis.value == "number") {
            return localThis.value;
          } else {
            return localThis.dateValue
              ? localThis.dateValue.toISOString().substr(0, 4)
              : null;
          }
        }
      },
      set(newVal: Date) {
        let localThis: any = this as any;
        localThis.formatDateTime = newVal;
      },
    },
    datePickerDate() {
      let localThis: any = this as any;
      //FIX to correct the d.toISOString that scale to one day before the passed date
      if (!localThis.is_only_year) {
        let d = localThis.dateValue
          ? localThis.dateValue.getHours() == 0
            ? new Date(localThis.dateValue.setHours(12, 0, 0, 0))
            : localThis.dateValue
          : new Date();

        return d.toISOString().substr(0, 10);
      } else {
        let date = localThis.dateValue ? localThis.dateValue : new Date();
        return date.toISOString().substr(0, 25);
      }
    },
    requiredTextFieldRule() {
      return [(v: any) => !!v || translate("error.required-field")];
    },
  },
  methods: {
    updateDate(newVal: string) {
      let localThis: any = this as any;
      if (!localThis.is_only_year) {
        localThis.$emit("input", newVal ? new Date(newVal) : null);
      } else {
        localThis.$emit(
          "input",
          newVal ? new Date(newVal).getFullYear() : null
        );
      }
      localThis.open = false;
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;
      localThis.updateDate(null);
    },

    // Format using reusable function
    padTo2Digits(num: number) {
      return num.toString().padStart(2, "0");
    },

    // format as "DD/MM/YYYY"
    //formatDate(date: Date) {
    //  let localThis: any = this as any;
    //  return [
    //    localThis.padTo2Digits(date.getDate()),
    //    localThis.padTo2Digits(date.getMonth() + 1),
    //    date.getFullYear(),
    //  ].join("/");
    //  },
      // Format as "DD MMM YYYY"
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


  },
});
</script>

<style scoped>
</style>