<template>
  <div class="theStepContainer">
    <h2>Step {{ step.id ? step.id.toString() : "0" }}</h2>
    <h4>{{ step.name ? step.name : "" }}</h4>
    <div class="inputs-container">
      <div
        v-for="(input, index) in inputs.filter((ele) => !ele.hide)"
        class="oneInputContainer"
        :style="{
          width: input.widthPercentage + '%',
          marginRight: input.marginRightPercentage + '%',
        }"
        :key="index"
      >
        <custom-text-input
          v-model="theData[input.model]"
          :type="input.type"
          :label="$t(input.label)"
          :required="input.required"
          :disabled="input.disabled"
          v-if="
            input.type === 'text' ||
            input.type === 'number' ||
            input.type === 'email'
          "
        />

        <custom-mobile-input
          v-model="theData[input.model]"
          :type="input.type"
          :label="$t(input.label)"
          :required="input.required"
          v-if="input.type === 'tel'"
          :defaultCountryCode="input.countryCode"
          :disabled="input.disabled"
        />
        <custom-select-input
          v-model="theData[input.model]"
          :label="$t(input.label)"
          v-if="input.type === 'select'"
          :required="input.required"
          :options="input.options"
          :disabled="input.disabled"
          :change="input.onChange"
        />
        <custom-date-picker
          v-model="theData[input.model]"
          :label="$t(input.label)"
          v-if="input.type === 'date'"
          :required="input.required"
          :minDate="input.min"
          :maxDate="input.max"
          :disabled="input.disabled"
          :change="input.onChange"
        />

        <select-checkbox-withInput
          v-if="input.type === 'select-checkbox-withInput'"
          v-model="theData[input.model]"
          :label="$t(input.label)"
          :required="input.required"
          :options="input.options"
          :theLabel="input.label"
          :hint="input.hint"
          :id="input.id"
        />
        <custom-radioboxes
          v-if="input.type === 'radio'"
          v-model="theData[input.model]"
          :label="$t(input.label)"
          :required="input.required"
          :options="input.options"
          :hint="input.hint"
          :id="input.id"
        />
        <custom-radioboxes
          v-if="input.type === 'checkbox'"
          v-model="theData[input.model]"
          :label="$t(input.label)"
          :required="input.required"
          :options="input.options"
          :hint="input.hint"
          :id="input.id"
          :checkbox="true"
        />
      </div>
    </div>
    <div class="erro_message">
      <v-alert
        v-if="step.nextDisabled"
        type="error"
        border="left"
        elevation="2"
        colored-border
      >
        {{
          step.errorMessage
            ? step.errorMessage
            : "Please fill all required fields"
        }}
      </v-alert>
    </div>
    <div class="buttons">
      <v-btn
        v-if="step.id > 1"
        color="secondary lighten-2"
        @click="PreviousStep"
        class="stepBtn"
      >
        Previous</v-btn
      >
      <v-btn
        :disabled="step.nextDisabled"
        color="secondary lighten-2"
        @click="nextStep"
        class="stepBtn"
      >
        {{ step.nextlabel ? step.nextlabel : "Next" }}
      </v-btn>
    </div>
  </div>
</template>

<script>
import TextInput from "../../TextInput.vue";
import CustomSelectInput from "../../CustomSelectInput.vue";
import DatePicker from "../../CustomDatePicker.vue";
import SelectCheckBoxWithInput from "../../SelectCheckBoxWithInput.vue";
import RadioCheckBoxes from "../../RadioCheckBoxes.vue";
import MobileInput from "../../MobileInput";

export default {
  components: {
    "custom-text-input": TextInput,
    "custom-select-input": CustomSelectInput,
    "custom-date-picker": DatePicker,
    "select-checkbox-withInput": SelectCheckBoxWithInput,
    "custom-mobile-input": MobileInput,
    "custom-radioboxes": RadioCheckBoxes,
  },
  props: {
    step: {
      type: Object,
    },

    inputs: {
      type: Array,
      default: () => [],
    },
    theData: {
      type: Object,
      required: true,
    },
    checkStep: {
      type: Function,
    },
  },
  methods: {
    nextStep() {
      this.$emit("next");
    },
    PreviousStep() {
      this.$emit("prev");
    },
  },
  mounted() {
    this.$emit("checkStep");
  },
};
</script>
<style scoped>
@import "./TheStep.scss";
</style>
