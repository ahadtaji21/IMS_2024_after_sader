<template>
  <v-container>
    <v-layout
      text-center
      wrap
    >
      <v-flex xs12>
        <v-img
          :src="require('../assets/jrs-logo.png')"
          class="my-3"
          contain
          height="100"
        ></v-img>
      </v-flex>

      <v-flex mb-4>
        <h1 class="h2 font-weight-bold mb-3">{{ $t('views.view-home.welcome-title') }}</h1>
        <p class="subheading font-weight-regular">{{ $t('views.view-home.welcome-message') }}</p>
      </v-flex>

      
    </v-layout>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
// import { WeatherForecastApi, Configuration } from '../axiosapi';

// export default Vue.extend({
//   name: 'HelloWorld',
//     data: () => ({
//     })
// });
</script>
