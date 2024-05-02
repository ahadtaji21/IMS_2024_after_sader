<template>
  <div class="custom-date-input" :class="{ input__invalid: isInvalid }">
    <div class="input">
      <input
        class="input__field"
        type="date"
        :value="value"
        @input="handleInput"
        @blur="handleBlur"
        v-bind="{ min: minDate || undefined, max: maxDate || undefined }"
        @change="changeHandler"
      />
      <span class="input__label">
        {{ label }} <span v-if="required" class="star_REQUIRED">*</span>
      </span>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    value: {
      default: "",
    },
    label: {
      type: String,
      default: "Label *",
    },
    required: {
      type: Boolean,
      default: false,
    },
    minDate: {
      type: String,
      default: null,
    },
    maxDate: {
      type: String,
      default: null,
    },
    change: {
      type: Function,
    },
  },
  data() {
    return {
      isInvalid: false,
    };
  },
  methods: {
    handleInput(event) {
      this.$emit("input", event.target.value);
      this.isInvalid =
        this.required && (!event.target.value || event.target.value == "");
    },
    handleBlur(event) {
      this.isInvalid =
        this.required && (!event.target.value || event.target.value == "");
    },
    changeHandler(event) {
      if (typeof this.change === "function") {
        this.change(event);
      }
    },
  },
  // mounted() {
  //   console.log("maxDate: " , this.maxDate);
  // }
};
</script>
