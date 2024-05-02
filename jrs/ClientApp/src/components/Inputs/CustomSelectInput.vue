<template>
  <div
    class="custom-select-input"
    :class="{
      input__invalid: isInvalid,
      input__required: required,
      input__disabled: disabled,
    }"
  >
    <label class="input">
      <select
        class="input__field"
        :value="value"
        :disabled="disabled"
        @input="handleInput"
        @blur="handleBlur"
        @change="changeHandler"
      >
        <option value="" disabled selected>Select {{ label }}</option>
        <option v-for="option in options" :value="option.id" :key="option.id">
          {{ option.label }}
        </option>
      </select>
      <span class="input__label"
        >{{ label }} <span v-if="required" class="star_REQUIRED"> *</span>
      </span>
    </label>
  </div>
</template>

<script>
export default {
  props: {
    value: {
      type: String,
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
    options: {
      type: Array,
      default: () => [],
    },
    disabled: {
      type: Boolean,
      default: false,
    },

    change: {
      type: Function,
    },
  },
  data() {
    return {
      isFocused: false,
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
  // mounted(){
  //   console.log("this.value: " , this.value);
  //   this.$emit('input', this.value);

  // }
};
</script>

<style scoped>
@import "./InputsStyle.scss";
</style>
