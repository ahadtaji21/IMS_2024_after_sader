<template>
    <v-dialog v-model="localDialog" persistent max-width="700">
        <v-card>
            <v-card-title class="headline text-center">{{ editTitle }}</v-card-title>
            <div>
                <form @submit.prevent="editFunc">
                    <!-- Include Form component and pass necessary props -->
                    <form-component :inputs="editInputs" :the-data="editData" />
                    <!--<button type="submit">{{ confirmTxt }}</button>
                    <button @click="dialog = false">{{ cancelTxt }}</button>-->
                </form>
            </div>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" text @click="cancelEdit">{{ cancelTxt }}</v-btn>
                <v-btn color="green darken-1" text @click="confirmEdit" :disabled="saveDisabled">{{ confirmTxt }}</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    // Import Form component
    import FormComponent from "@/components/Inputs/Form/Form.vue";
    export default {
        components: {
            // Register FormComponent under a different name if there's a conflict
            "form-component": FormComponent,
        },
        props: {
            editTitle: {
                type: String,
                default: "Edit",
            },
            confirmTxt: {
                type: String,
                default: "Save Changes",
            },
            cancelTxt: {
                type: String,
                default: "Cancel",
            },
            editInputs: {
                type: Array,
                required: true,
            },
            editData: {
                type: Object,
                required: true,
            },
            editFunc: {
                type: Function,
                required: true,
            },
            dialog: {
                type: Boolean,
                default: false,
            },
        },
        data() {
            return {
                localDialog: this.dialog,
            };
        },
        computed: {
            saveDisabled() {
                // Add your logic to determine whether the Save button should be disabled
                return this.editInputs.some((input) => input.required && !this.editData[input.model]);
            },
        },
        methods: {
            confirmEdit() {
                this.editFunc(this.editData);
                this.$nextTick(() => {
                    this.closeDialog();
                });
            },
            cancelEdit() {
                this.closeDialog();
            },
            closeDialog() {
                this.localDialog = false;
                this.$emit("update:dialog", this.localDialog);
            },
        },
        watch: {
            editInputs(newValue) {
                console.log("editInputs:", newValue);
            },
            dialog(newValue) {
                console.log("new dialog: ", newValue);
                // You can perform additional actions based on the dialog state
            },
        },
    };
</script>
