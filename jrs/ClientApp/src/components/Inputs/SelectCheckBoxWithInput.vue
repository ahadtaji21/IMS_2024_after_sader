<template>
  <div class="custom-select-checkbox">
    <div class="label-wrapper">
      <p class="label">
        {{ label }}   <span v-if="required" class="star_REQUIRED"> *</span>
      </p> 
      <span class="hint">{{ hint }}</span>
    </div>
    <div class="options">
      <div
        v-for="(option, idx) in options"
        class="option"
        :key="option.id"
        @click="toggleOption(option.id, option.label, idx)"
      >
        <input
          type="checkbox"
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
    <div class="text" v-for="(obj, idx) in selectedOptions" :key="idx">
      <div class="custom-text-input">
        <label class="input">
          <input
            class="input__field"
            type="text"
            v-model="obj.value"
            @input="(event) => handleInput(event, obj)"
          />
          <span class="input__label">{{ obj.name }} </span>
        </label>
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
    return {
      selectedOptions: [],
    };
  },
  methods: {
    isSelected(optionId) {
      return this.selectedOptions.find((ele) => ele.id == optionId);
    },
    toggleOption(optionId, optionLabel, idx) {
      if (this.isSelected(optionId)) {
        this.selectedOptions = this.selectedOptions.filter(
          (ele) => ele.id !== optionId
        );

        console.log("this.selectedOptions: ", this.selectedOptions);
      } else {
        this.selectedOptions.push({
          id: optionId,
          value: "",
          name: optionLabel,
        });
        console.log("else this.selectedOptions: ", this.selectedOptions);

      }
      const updatedValue = JSON.stringify(this.selectedOptions);
      this.$emit("input", updatedValue);
    },
    handleInput(event, obj, idx) {
      obj.value = event.target.value;
      const updatedValue = JSON.stringify(this.selectedOptions);
      this.$emit("input", updatedValue);
    },
    changeHandler(event) {
      if (typeof this.change === "function") {
        this.change(event);
      }
    },
  },
  mounted() {
    if (this.value && this.value != "") this.selectedOptions = JSON.parse(this.value);
  },
};
</script>

<style scoped>
@import "./InputsStyle.scss";
</style>
