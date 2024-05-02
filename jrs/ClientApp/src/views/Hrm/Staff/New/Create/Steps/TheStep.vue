<template>
  <div class="theStepContainer">
    <h2>Step {{ stepId }}</h2>
    <h4>{{ stepName }}</h4>
    <div class="inputs-container">
      <div
        v-for="(input, index) in inputs"
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
        />

        <custom-date-picker
          v-model="theData[input.model]"
          :label="$t(input.label)"
          v-if="input.type === 'date'"
          :required="input.required"
          :minDate="input.min"
          :maxDate="input.max"
          :disabled="input.disabled"
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

    <div class="buttons">
      <v-btn
        v-if="stepId > 1"
        color="secondary lighten-2"
        @click="PreviousStep"
        class="stepBtn"
      >
        Previous</v-btn
      >
      <v-btn
        :disabled="nextDisabled"
        v-if="!submit"
        color="secondary lighten-2"
        @click="nextStep"
        class="stepBtn"
      >
        {{ nextLabel }}
      </v-btn>
      
    </div>
  </div>
</template>

<script>
import TextInput from "../../../../components/Inputs/TextInput.vue";
import CustomSelectInput from "../../../../components/Inputs/CustomSelectInput.vue";
import DatePicker from "../../../../components/Inputs/CustomDatePicker.vue";
import SelectCheckBoxWithInput from "../../../../components/Inputs/SelectCheckBoxWithInput.vue";
import RadioCheckBoxes from "../../../../components/Inputs/RadioCheckBoxes.vue";
import MobileInput from "../../../../components/Inputs/MobileInput";

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
    stepName: {
      type: String,
      required: true,
    },
    stepId: {
      type: String,
      required: true,
    },
    inputs: {
      type: Array,
      default: () => [],
    },
    theData: {
      type: Object,
      required: true,
    },
    submit: {
      type: Function,
    },
    nextDisabled: {
      type: Boolean,
      default: true,
    },
    nextLabel: {
      type: String,
      default: "Next",
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
  
};
</script>
<style scoped>
@import "./TheStep.scss";
</style>
