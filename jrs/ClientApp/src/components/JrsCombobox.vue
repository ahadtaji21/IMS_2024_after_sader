<template>
	<v-combobox
		ref="comboField"
		v-model="computedValue"
		:label="label"
		:hint="hint"
		:required="required"
		:items="items"
		:item-value="itemValue"
		:item-text="itemText"
		:multiple="multiple"
		:small-chips="multiple"
		color="primary"
	>
		<template v-slot:selection="selectionItem">
			<v-chip
				close
				color="primary"
				text-color="white"
				small
				@click:close="removeComboboxChip(selectionItem.index)"
				>{{ selectionItem.item[itemText] }}</v-chip
			>
		</template>
	</v-combobox>
</template>

<script lang="ts">
import Vue from "vue";
import UtilMix from "../mixins/UtilMix";

export default Vue.extend({
	props: {
		value: {
			type: [Number, String, Array],
			required: false,
		},
		label: {
			type: String,
			required: false,
			default: null,
		},
		hint: {
			type: String,
			required: false,
			default: null,
		},
		required: {
			type: Boolean,
			required: false,
			default: false,
		},
		/**
		 * List of available options
		 */
		items: {
			type: Array,
			required: true,
		},
		/**
		 * Name of the value column of the data-source.
		 */
		itemValue: {
			type: String,
			required: true,
		},
		/**
		 * Name of the text column of the data-source.
		 */
		itemText: {
			type: String,
			required: true,
		},
		multiple: {
			type: Boolean,
			required: false,
			default: false,
		},
	},
	mixins: [UtilMix],
	computed: {
		/**
		 * If Single then the component recieves a number and returns a number for existing vlues or a string for missing items.
		 * If Multi then the component recieves and return an array of objects.
		 */
		computedValue: {
			get() {
				const localThis: any = this as any;
				if (!localThis.multiple) {
					// Single selection
					// TODO: Review and implement
					return localThis.value;
				} else {
					// Multi selection
					return localThis.value;
				}
			},
			set(newVal: any) {
				const localThis: any = this as any;
				// Single selection
				if (!localThis.multiple) {
					// TODO: Review and implement
					localThis.updateValue(newVal);
				} else {
					// Multi selection
					// Parse values before emitting
					const valueToEmit = newVal.map((item: any) => {
						if (typeof item === "string") {
							item = {
								[localThis.itemValue]: null,
								[localThis.itemText]: item.split(" ").join(""),
							};
						}
						return item;
					});
					localThis.updateValue(valueToEmit);
				}
			},
		},
	},
	methods: {
		/**
		 * Emit new field data.
		 */
		updateValue(newVal: any) {
			const localThis: any = this as any;
			localThis.$emit("input", newVal);
		},
		removeComboboxChip(removeIndex: number) {
			const localThis: any = this as any;
			localThis.computedValue.splice(removeIndex, 1);
		},
	}
});
</script>

<style scoped></style>
