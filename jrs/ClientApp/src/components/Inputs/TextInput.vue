<template>
  <div class="custom-text-input" :class="{ input__invalid: isInvalid }">
    <label class="input">
      <input
        class="input__field"
        :type="type"
        :value="value"
        :disabled="disabled"
        @input="handleInput"
        @blur="handleBlur"
        @change="changeHandler"
      />
      <span class="input__label"
        >{{ label }} <span v-if="required" class="star_REQUIRED"> *</span>
      </span>
    </label>
  </div>
</template>

<script>
export default {
  props: {
    type: {
      type: String,
      default: "text",
    },
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
    change: {
      type: Function,
    },
    readonly: {
      type: Boolean,
      default: false,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      isFocused: false,
      isInvalid: false,
    };
  },
  methods: {
    validateEmail(email) {
      var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return re.test(String(email).toLowerCase());
    },

    handleInput(event) {
      this.$emit("input", event.target.value);
      if (this.type === "email") {
        this.isInvalid = !this.validateEmail(event.target.value);
      } else {
        this.isInvalid =
          this.required && (!event.target.value || event.target.value === "");
      }
    },
    handleBlur(event) {
      if (this.type === "email") {
        this.isInvalid = !this.validateEmail(event.target.value);
      } else {
        this.isInvalid =
          this.required && (!event.target.value || event.target.value === "");
      }
    },
    changeHandler(event) {
      if (typeof this.change === "function") {
        this.change(event);
      }
    },
  },
};
</script>

<style scoped>
@import "./InputsStyle.scss";
</style>
