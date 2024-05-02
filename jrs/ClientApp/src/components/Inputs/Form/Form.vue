<template>
  <div class="inputs-container" v-if="inputs && inputs.length > 0">
    <div
      v-for="(input, index) in inputs.filter((ele) => !ele.hide)"
      class="oneInputContainer"
      :class="input.type"
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
        :readonly="input.readonly"
        v-if="
          input.type === 'text' ||
          input.type === 'number' ||
          input.type === 'email'
        "
        :change="input.onChange"
      />
      <p class="staticTextInput" v-if="input.type === 'staticText'">
        <b> {{ $t(input.label) }}: </b> {{ theData[input.model] }}
      </p>
      <custom-mobile-input
        v-model="theData[input.model]"
        :type="input.type"
        :label="$t(input.label)"
        :required="input.required"
        v-if="input.type === 'tel'"
        :defaultCountryCode="input.countryCode"
        :disabled="input.disabled"
        :change="input.onChange"
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
        :values="input.values"
        :hint="input.hint"
        :id="input.id"
        :change="input.onChange"
      />
      <custom-radioboxes
        v-if="input.type === 'radio'"
        v-model="theData[input.model]"
        :label="$t(input.label)"
        :required="input.required"
        :options="input.options"
        :hint="input.hint"
        :id="input.id"
        :change="input.onChange"
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
        :change="input.onChange"
      />
      <v-file-input
        v-if="input.type === 'file'"
        :truncate-length="truncate_length"
        counter
        :accept="acceptExtensions"
        :label="label"
        chips
        :rules="extensionRules"
        v-model="fileUpload"
        :persistent-hint="true"
        :hint="'Accept Ext. :' + acceptExtensions"
        @change="handleFileUpload"
      >
      </v-file-input>
      <!-- Use the ImageUpload component for image upload -->
      <image-upload
        v-if="input.type === 'image'"
        v-model="theData[input.model]"
        :label="$t(input.label)"
      />

      <custom-toggle-switch
        v-if="input.type === 'toggle-switch'"
        v-model="theData[input.model]"
        :label="input.label"
      />
    </div>
  </div>
</template>

<script>
import TextInput from "../TextInput.vue";
import CustomSelectInput from "../CustomSelectInput.vue";
import DatePicker from "../CustomDatePicker.vue";
import SelectCheckBoxWithInput from "../SelectCheckBoxWithInput.vue";
import RadioCheckBoxes from "../RadioCheckBoxes.vue";
import MobileInput from "../MobileInput.vue";
import CustomToggleSwitch from "../CustomToggleSwitch.vue";
import ImageUpload from "../ImageUpload.vue";
export default {
  components: {
    "custom-text-input": TextInput,
    "custom-select-input": CustomSelectInput,
    "custom-date-picker": DatePicker,
    "select-checkbox-withInput": SelectCheckBoxWithInput,
    "custom-mobile-input": MobileInput,
    "custom-radioboxes": RadioCheckBoxes,
    "custom-toggle-switch": CustomToggleSwitch,
    "image-upload": ImageUpload,
  },
  props: {
    inputs: {
      type: Array,
    },
    theData: {
      type: Object,
    },
  },
};
</script>
<style scoped>
@import "./Form.scss";
</style>
