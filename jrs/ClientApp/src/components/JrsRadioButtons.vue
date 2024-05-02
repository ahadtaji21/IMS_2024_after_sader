<template>
	<div>
		<!-- <div>INNER_VALUE: {{JSON.stringify(value)}}</div> -->
		<v-radio-group
			v-model="fieldValue"
			:label="label"
			:hint ="hint ? hint : ''"
            :persistent-hint ="hint && hint != '' ? true : false"
			:multiple="multiple"
			prepend-icon="mdi-view-list"
			:required="is_required"
			:rules="fieldRules"
		>	
			<template v-for="(item, itemIndex) in items">
				<v-radio
					:key="`radio_${itemIndex}`"
					:value="item[itemKey]"
					@click="changedRadioState(item[itemKey], $event)"
					:name="`RADIO_${itemIndex}`"
				>
					<template v-slot:label>
						{{ item[itemText] }}
					</template>
				</v-radio>
			</template>
			<template v-slot:append>
				<v-icon @click="clearField">mdi-close</v-icon>
			</template>
		</v-radio-group>
	</div>
</template>

<script lang="ts">
	import Vue from "vue";

	export default Vue.extend({
		props: {
			value: {
				type: [String, Number, Array],
				required: false,
				default: () => null,
			},
			items: {
				type: Array,
				required: true,
			},
			itemKey: {
				type: String,
				required: true,
			},
			itemText: {
				type: String,
				required: true,
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
			is_required: {
				type: Boolean,
				required: false,
				default: false,
			},
			readonly: {
				type: Boolean,
				required: false,
				default: false,
			},
			multiple: {
				type: Boolean,
				required: false,
				default: false,
			},
		},
		data: () => ({}),
		computed: {
			fieldValue: {
				get() {
					const localThis: any = this as any;
					// Override return for values when "multiple"
					if (localThis.multiple) {
						const recievedValue:Array<any> = localThis.value || [];
						return recievedValue.map((item:any) => item[localThis.itemKey]);
					}

					return localThis.value;
				},
				set(newVal: any) {
					const localThis: any = this as any;
					localThis.updateValue(newVal);
				},
			},
			fieldRules(){
                const localThis:any = this as any;
                let rules = [];

				// Check required field
                if(localThis.is_required){
                    const requiredRule = (v:number) => !!v || localThis.$t('error.required-field');
                    rules.push(requiredRule);
                }
                return rules;
            }
		},
		methods: {
			/**
			 * Emit new field data.
			 */
			updateValue(newVal: any) {
				const localThis: any = this as any;
				// Emit update of v-model to parent component.
				if (localThis.multiple) {
					const selectedObject: Array<any> = localThis.items.filter(
						(item: any) => newVal.includes(item[localThis.itemKey])
					);
					localThis.$emit("input", selectedObject);
				}else{
					localThis.$emit("input", newVal);
				}
			},
			/**
			 * Clear field data.
			 */
			clearField() {
				const localThis: any = this as any;
				localThis.updateValue(null);
			},
			changedRadioState(radioValue: any, evt: any) {
				const localThis: any = this as any;
				// Clear selection if necessary
				if (localThis.multiple) {
					const itemIndex: number =
						localThis.fieldValue.indexOf(radioValue);
					// Add or remove value from selected Values
					if (itemIndex == -1) {
						localThis.fieldValue = [
							...localThis.fieldValue,
							radioValue,
						];
					} else {
						localThis.fieldValue.splice(itemIndex, 1);
						localThis.fieldValue = [...localThis.fieldValue];
					}
				} else {
					localThis.fieldValue = radioValue;
				}
			},
		},
	});
</script>

<style scoped>
</style>