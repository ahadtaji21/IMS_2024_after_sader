<template>
  <v-content>
    <v-content style="padding: unset !important">
      <v-container fluid fill-height>
        <v-row align-center>
          <v-col cols="12" class="text-center">
            <!-- Module name with dynamic background color based on the module -->
            <div class="module-header" :style="moduleStyle">
              <span style="margin-left: 4%">
                {{ moduleDetailsObj.moduleName }}</span
              >
              <img
                class="module-header-icon"
                :src="getModuleLogo(moduleDetailsObj.moduleName)"
              />
            </div>
          </v-col>
          <v-col cols="12">
            <!-- Use Vuetify's grid system to layout menu items -->
            <v-row style="margin: 6%; margin-top: 1%">
              <v-col
                cols="12"
                md="4"
                v-for="(menuItem, index) in moduleDetailsObj.filteredMenu"
                :key="index"
              >
                <!-- Use your MenuBtn component for each menu item -->
                <menu-btn
                  :menu="menuItem"
                  :module="{
                    moduleName: menuItem.moduleName,
                    moduleColor: menuItem.moduleColor,
                    moduleId: menuItem.moduleId,
                    moduleCode: menuItem.moduleCode,
                  }"
                ></menu-btn>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </v-container>
    </v-content>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import MenuBtn from "../components/MenuBtn.vue";

export default Vue.extend({
  components: {
    MenuBtn, // Registering MenuBtn component locally
  },
  props: {
    moduleDetailsObj: {
      type: Object,
      required: true,
    },
  },
  methods: {
    getModuleLogo(moduleName: string) {
      // Convert the module name to a format suitable for filename, for example: "Dashboard" -> "Dashboard.png"
      return require("@/assets/moduleLogos/Module_Details_Logo/" +
        moduleName +
        ".png");
    },
  },
  data() {
    return {
      // Define your module colors and icons here 0083B0
      modules: {
        "1": { gradient: ["#5B5F6B", "#ffffff"] },
        "2": { gradient: ["#214885", "#ffffff"] },
        "3": { gradient: ["#77AA2B", "#ffffff"] },
        "4": { gradient: ["#DD7C11", "#ffffff"] },
        "5": { gradient: ["#007E9F", "#ffffff"] },
        "6": { gradient: ["#00B4DB", "#ffffff"] },
        "7": { gradient: ["#07B6B1", "#ffffff"] },
        "8": { gradient: ["#5B5F6B", "#ffffff"] },
        // Add more modules as needed
      },
    };
  },
  computed: {
    moduleStyle() {
      let localThis: any = this as any;
      const module = localThis.modules[localThis.moduleDetailsObj.moduleId];
      if (!module) return {};
      return {
        background: `linear-gradient(to right, ${module.gradient[0]}, ${module.gradient[1]})`,
        // You can include other static styles here if they don't change per module
      };
    },
    moduleIcon() {
      let localThis: any = this as any;
      const module = localThis.modules[localThis.moduleDetailsObj.moduleId];
      return module.icon ? module.icon : "";
    },
  },
  mounted() {
    let localThis: any = this as any;
    console.log("moduleDetailsObj: ", localThis.moduleDetailsObj);
  },
});
</script>

<style></style>
