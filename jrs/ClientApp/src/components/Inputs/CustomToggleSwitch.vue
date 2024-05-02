<!-- CustomToggleSwitch.vue -->
<template>
    <div class="toggle-container">
        <v-switch v-model="isChecked" :label="label" @change="handleChange" />
        <div v-if="showLabels" class="toggle-labels">
            <!--<span class="false-label" :class="{ 'active-label': !isChecked }">No</span>-->
            <!--<span class="true-label" :class="{ 'active-label': isChecked }">Yes</span>-->
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            value: {
                type: Boolean,
                default: false,
            },
            label: {
                type: String,
                default: "Toggle Switch",
            },
            showLabels: {
                type: Boolean,
                default: true,
            },
        },
        data() {
            return {
                isChecked: this.value,
            };
        },

        methods: {
  handleChange() {
    // Convert the string value to a Boolean
    const newValue = this.isChecked === "true" || this.isChecked === true;
    // Emit the Boolean value
    this.$emit("input", newValue);
  },
},
    };
</script>

<style scoped>
    .toggle-container {
        display: inline-block;
    }

    .toggle-labels {
        display: flex;
        justify-content: space-between;
        margin-top: 5px;
    }

    .true-label, .false-label {
        font-weight: bold;
    }

    .active-label {
        color: #1976d2; /* Customize the color for the active label */
    }

    /* Add your additional styling for the toggle switch and labels here if needed */
</style>
