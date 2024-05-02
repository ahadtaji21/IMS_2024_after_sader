<template>
  <v-dialog v-model="localDialog" persistent max-width="290">
    <v-card>
      <v-card-title class="headline">{{ ConfirmTitle }}</v-card-title>
      <v-card-text>{{ ConfirmationMsg }}</v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="green darken-1" text @click="cancel">
          {{ refuseTxt }}
        </v-btn>
        <v-btn color="red darken-1" text @click="confirmDelete">
          {{ acceptTxt }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  props: {
    ConfirmationMsg: {
      type: String,
      default: "Are you sure you want to delete?",
    },
    ConfirmTitle: {
      type: String,
      default: "Confirmation",
    },
    acceptTxt: {
      type: String,
      default: "Yes",
    },
    refuseTxt: {
      type: String,
      default: "Cancel",
    },
    theFunc: {
      type: Function,
      required: true,
    },
    dialog: {
      type: Boolean,
      default: false,
    },
    confirmData: {
      type: Object,
    },
  },
  data() {
    return {
      localDialog: this.dialog,
    };
  },
  methods: {
    confirmDelete() {
      
      if (!this.confirmData) return;
      this.theFunc(this.confirmData);
      this.closeDialog();
      
    },
    cancel() {
        this.closeDialog();

    },
    closeDialog() {
      this.localDialog = false;
      this.$emit('update:dialog', this.localDialog); // Notify parent to update state
    }
  },
  watch: {
    dialog(newValue) {
        console.log("new dialog: ", newValue)
    //   this.localDialog = newValue;
    },
  },
//   mounted() {
//     console.log("this.theFunc: ", this.theFunc);
//   },
};
</script>
