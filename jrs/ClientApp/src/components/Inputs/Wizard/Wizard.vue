<template>
  <v-content>
    <v-container fluid fill-height>
      <div class="progress-bar">
        <div
          v-for="step in steps"
          class="progress-step"
          :key="'progress-step-' + step.id"
          :class="getStepClass(step, currentStep)"
          @click="() => stepClick(step.id)"
          :style="{ cursor: step.id < currentStep ? 'pointer' : 'not-allowed' }"
        >
          <div class="icon-container">
            <span
              :class="[
                'mdi',
                step.icon,
                { 'icon-active': step.id <= currentStep },
              ]"
            ></span>
          </div>
          <p class="step_name">{{ step.name }}</p>
          <div
            v-if="step.id !== steps.length"
            :class="['progress-line', { 'line-active': step.id < currentStep }]"
          ></div>
        </div>
      </div>

      <div class="step_loop" v-for="st in steps" :key="'thestep-' + st.id">
        <wizard-step
          v-if="st.id == currentStep && st.id != steps.length"
          :step="st"
          :inputs="inputs.filter((ele) => ele.step == st.id)"
          :theData="theData"
          :checkStep="checkStep"
          @next="nextStep"
          @prev="prevStep"
        />
      </div>
      <wizard-preview
        v-if="currentStep == steps.length"
        :submit="submit"
        :inputs="inputs"
        :steps="steps.filter((ele) => ele.id != steps.length)"
        :theData="theData"
        :title="previewTitle"
      />
    </v-container>
  </v-content>
</template>
<script>

import TheStep from "./Steps/TheStep.vue";
import PreviewSteps from "./Steps/PreviewSteps.vue";

export default {
  components: {
    "wizard-step": TheStep,
    "wizard-preview": PreviewSteps,
  },
  props: {
    steps: {
      type: Array,
      // shape: [{id, icon, name,nextlabel,nextDisabled}...]
    },
    inputs: {
      type: Array,
      // shape: [{id, step, widthPercentage,  marginRightPercentage, model, type, label,  required,disabled, {type == "select" ? options}, {type == "date" ? min & max} }...]
    },
    theData: {
      type: Object,
      //shape: object with the keys as the model in inputs
    },
    submit: {
      type: Function,
    },
    previewTitle: {
      type: String,
      default: "JRS individual registration form",
    },
  },
  data() {
    return {
      currentStep: 1,
    };
  },
  methods: {
    nextStep() {
      const current = this.steps[this.currentStep - 1];
      if (this.currentStep < this.steps.length) {
        if (current.nextAdditional) {
          current.nextAdditional(() => {
            this.currentStep = parseInt(this.currentStep) + 1;
          });
        } else {
          this.currentStep = parseInt(this.currentStep) + 1;
        }
      }
    },
    prevStep() {
      if (this.currentStep > 1) {
        this.currentStep = parseInt(this.currentStep) - 1;
      }
    },
    stepClick(st) {
      if (st > 0 && st < 7 && st < this.currentStep) {
        this.currentStep = parseInt(st);
      }
    },
    getStepClass(step, currentStep) {
      if (step.id < currentStep) {
        return ["progress-step-done"];
      } else if (step.id === currentStep) {
        return ["progress-step-active"];
      } else {
        return ["progress-step-undone"];
      }
    },

    checkStep(data) {
      let localDisabled = false;
      const stepInputs = this.inputs.filter(
        (ele) => ele.step == this.currentStep
      );
      for (let i = 0; i < stepInputs.length; i++) {
        const inp = stepInputs[i];
        const newVal = data[inp.model];
        if (!inp.hide && inp.required && (!newVal || newVal == "")) {
          localDisabled = true;
          break;
        }
      }

      this.steps[this.currentStep - 1].nextDisabled = localDisabled;
    },
  },

  watch: {
    theData: {
      deep: true,
      handler(newValue, oldValue) {
        this.checkStep(newValue);
      },
    },
  },
};
</script>
<style scoped>
@import "./Wizard.scss";
</style>
