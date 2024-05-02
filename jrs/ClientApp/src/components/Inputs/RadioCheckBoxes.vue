<template>
  <div class="custom-select-checkbox">
    <div class="label-wrapper">
      <p class="label">
        {{ label }}
      </p>
      <span class="hint">{{ hint }}</span>
    </div>
    <div class="options">
      <div
        v-for="(option, idx) in options"
        class="option"
        :key="option.id"
        @click="toggleOption(option.id, idx)"
      >
        <input
          :type="checkbox ? 'checkbox' : 'radio'"
          :name="id"
          :value="option.id"
          :checked="isSelected(option.id)"
          @change="changeHandler"
        />
        <span class="label2">
          {{ option.label }}
        </span>
      </div>
    </div>
  </div>
</template>

<script>
// import TextInput from "./TextInput.vue";

export default {
  props: {
    id: {
      type: Number,
      required: true,
    },
    type: {
      type: String,
      default: "text",
    },
    value: {
      type: String,
      default: "false",
    },
    label: {
      type: String,
      default: "Label *",
    },
    required: {
      type: Boolean,
      default: false,
    },
    checkbox: {
      type: Boolean,
      default: false,
    },
    hint: {
      type: String,
    },
    options: {
      type: Array,
      default: () => [],
    },

    change: {
      type: Function,
    },
  },
  data() {
    let santizedVal;
    if (this.checkbox) {
      santizedVal = this.value.startsWith(",")
        ? this.value.substring(1)
        : this.value;
    }

    return {
      selectedOption: this.checkbox
        ? santizedVal.split(",")
        : this.value || null,
    };
  },
  methods: {
    isSelected(optionId) {
      if (this.checkbox) {
        return this.selectedOption.includes(optionId);
      }

      return this.selectedOption === optionId;
    },
    toggleOption(optionId) {
      if (this.checkbox) {
        if (this.isSelected(optionId)) {
          this.selectedOption = this.selectedOption.filter(
            (id) => id !== optionId
          );
        } else {
          this.selectedOption.push(optionId);
        }
        if (this.selectedOption.includes("false")) {
          this.selectedOption = this.selectedOption.filter(
            (ele) => ele != "false"
          );
        }
        console.log("this.selectedOption", this.selectedOption);
        const updatedValue = this.selectedOption.join(",");

        const santizedVal = updatedValue.startsWith(",")
          ? updatedValue.substring(1)
          : updatedValue;

        this.$emit("input", santizedVal);
      } else {
        this.selectedOption = optionId;
        this.$emit("input", this.selectedOption);
      }
    },
    changeHandler(event) {
      if (typeof this.change === "function") {
        this.change(event);
      }
    },
  },
  mounted() {
    if (!this.checkbox) {
      // const inp  =  this.options.find(opt => opt.default);
      // if(inp){
      //   this.selectedOption = inp.id;
      //   this.$emit("input", this.selectedOption);
      // }
    }
  },
};
</script>

<style scoped>
@import "./InputsStyle.scss";
</style>
