<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row align-center>
                <v-col justify-center>
                    <h1>This is Continent</h1>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>
<script lang="ts">
import Vue from "vue";
export default Vue.extend({
  mounted() {
    console.log("Continent loaded");
  }
});
</script>