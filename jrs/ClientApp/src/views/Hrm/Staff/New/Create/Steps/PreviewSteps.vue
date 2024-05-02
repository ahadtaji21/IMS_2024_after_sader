<template>
  <div class="preview">
    <h2 class="title">JRS individual registration form</h2>

    <div class="part" v-for="(step, index) in steps" :key="`step-${index}`">
      <h3 class="partTitle">{{ step.name }}</h3>
      <div class="cont">
        <div
          v-for="input in filteredInputs(step.id)"
          :key="input.model"
          class="rect"
        >
          <div class="label">
            <b> {{ $t(input.label) }}: </b>
          </div>

          <div class="value" v-if="input.type == 'select'">
            <label>
              {{
                input.options.find((ele) => ele.id == theData[input.model])
                  ? input.options.find((ele) => ele.id == theData[input.model])
                      .label
                  : ""
              }}
            </label>
          </div>
          <div class="value" v-if="input.type == 'select-checkbox-withInput'">
            <p v-for="opt, idx2 in JSON.parse(theData[input.model])" :key="idx2">
            <b>
              {{
                opt.name
              }}: 
            </b>
            <label>
              {{ opt.value }}
            </label>
          </p>

          </div>
          <div class="value" v-if="input.type == 'radio'">
            {{ theData[input.model] }}
          </div>
          
          <div class="value" v-if="input.type == 'checkbox'">
            {{
              theData[input.model]
                .split(",")
                .filter((ee) => ee != "")
                .map((ee) => input.options.find((op) => op.id == ee).label)
                .join(",")
            }}
          </div>
          <div
            class="value"
            v-if="
              input.type != 'select' &&
              input.type != 'select-checkbox-withInput' &&
              input.type != 'radio' &&
              input.type != 'checkbox'
            "
          >
            <label> {{ theData[input.model] }} </label>
          </div>
        </div>
      </div>
    </div>
    <div class="save_cont_btn">
      <v-btn
        v-if="submit"
        color="secondary lighten-2"
        @click="localsubmit"
        :loading="addLoading"
      >
        Save</v-btn
      >
    </div>
  </div>
</template>

<script>
export default {
  props: {
    submit: {
      type: Function,
    },
    inputs: {
      type: Array,
      default: () => [],
    },
    steps: {
      type: Array,
      default: () => [],
    },
    theData: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      addLoading: false,
    };
  },

  methods: {
    
    filteredInputs(stepId) {
      const myInputs = this.inputs.filter((input) => input.step === stepId);
      return myInputs;
    },
    async localsubmit() {
      this.addLoading = true;
      try {
        await this.submit();
      } catch (ex) {
        console.log("error submitting: ", ex);
      } finally {
        this.addLoading = false;
      }
    },
  },
  // computed: {},
  // methods: {

  // },
};
</script>
<style scoped>
@import "./Preview.scss";
</style>
