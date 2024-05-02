<template>
  <div
    class="telephone_container"
    :class="{ input__general_invalid: generalInvalid }"
  >
    <label>
      <span class="label">
        {{ label }} <span v-if="required" class="star_REQUIRED"> *</span>
      </span>
      <div class="telephone_input">
        <input
          :type="type"
          v-model="countryCode"
          class="country_code input__field"
          placeholder="+1"
          @input="validateCountryCode"
          :class="{ input__invalid: isInvalid1 }"
        />
        <input
          :type="type"
          v-model="phoneNumber"
          placeholder="1234567890"
          class="input__field phoneNumber"
          @input="validatePhoneNumber"
          :class="{ input__invalid: isInvalid2 }"
          @change="changeHandler"
        />
      </div>
    </label>
    <p v-if="errorMsg" class="errormsg">{{ errorMsg }}</p>
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
    defaultCountryCode: {
      type: String,
      default: "",
    },

    change: {
      type: Function,
    },
  },
  data() {
    return {
      countryCode: this.defaultCountryCode,
      phoneNumber: "",
      errorMsg: "",
      isInvalid1: false,
      isInvalid2: false,
      generalInvalid: false,
    };
  },
  methods: {
    validateCountryCode() {
      const countryCodePattern = /^\+\d{1,4}$/;
      if (this.countryCode && !countryCodePattern.test(this.countryCode)) {
        this.errorMsg = "Invalid country code.";
        this.isInvalid1 = true;
        this.generalInvalid = true;
      } else {
        this.errorMsg = "";
        this.isInvalid1 = false;
        this.generalInvalid = false;
      }
    },
    validatePhoneNumber() {
      const phoneNumberPattern = /^\d{5,15}$/;
      if (this.phoneNumber && !phoneNumberPattern.test(this.phoneNumber)) {
        this.errorMsg = "Invalid phone number.";
        this.isInvalid2 = true;
        this.generalInvalid = true;
      } else {
        this.errorMsg = "";
        this.isInvalid2 = false;
        this.generalInvalid = false;
      }
    },
    changeHandler(event) {
      if (typeof this.change === "function") {
        this.change(event);
      }
    },
  },
  watch: {
    countryCode: function (newVal) {
      this.$emit("input", newVal + this.phoneNumber);
    },
    phoneNumber: function (newVal) {
      this.$emit("input", this.countryCode + newVal);
    },
    defaultCountryCode: function (newVal) {
      this.countryCode = newVal;
    },
  },
};
</script>

<style scoped>
.telephone-input input {
  border: 1px solid #ccc;
  padding: 5px;
  margin-right: 5px;
}

.error {
  color: red;
}

.star_REQUIRED {
  color: red;
}
</style>
