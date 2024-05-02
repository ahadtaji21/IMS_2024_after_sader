<template>
  <v-snackbar v-model="isVisible" :color="getSnackbar.type.color" :timeout="0" dark>
    {{ getSnackbar.text }}
    <v-btn text @click="isVisible = false">Close</v-btn>
  </v-snackbar>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";

export default Vue.extend({
  data() {
    return {
      isVisible: false,
    };
  },
  computed: {
    ...mapGetters({
      getSnackbar: "getSnackbar",
    }),
  },
  created() {
    (this as any).$store.watch(
      (state: any, getters: any) => {
        return getters["getSnackbar"];
      },
      () => {
        (this as any).isVisible = true;
      }
    );
  },
});
</script>

<style scoped></style>
