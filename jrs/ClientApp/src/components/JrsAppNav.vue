<template>
  <v-navigation-drawer
    app
    v-model="drawerIsExpanded"
    clipped
    tag="nav"
    id="ims-navigation-drawer"
    readonly
    class="indigo lighten-5"
  >
    <!-- General -->
    <v-list dense>
      <template>
        <br />
        <v-row justify="space-around">
          <v-avatar v-if="avatar">
            <img :src="avatar" />
          </v-avatar>
        </v-row>
        <br />
        <jrs-session-manager displayChoice="user-name"></jrs-session-manager>
        <v-row justify="space-around"> <br /> </v-row>
      </template>
      <!-- <v-subheader class="text-capitalize">{{ $t('general.general') }}</v-subheader> -->
      <v-list-item-group color="primary">
        <div
          v-for="(item, i) in menuItems.filter(
            (menu) => menu.moduleId == null && !menu.isHidden
          )"
          :key="i"
        >
          <!-- LEAF MENU -->
          <v-list-item
            v-if="!item.subMenuList || item.subMenuList.length == 0"
            link
            :to="item.path"
            value="true"
            color="primary"
          >
            <v-list-item-icon>
              <v-icon v-text="item.iconCode"></v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title
                v-text="item.label"
                class="text-capitalize"
              ></v-list-item-title>
              <!-- <router-link :to="item.path" tag="a">{{ item.label }}</router-link> -->
            </v-list-item-content>
          </v-list-item>

          <!-- BRANCH MENU -->
          <div v-else>
            <v-list-group :prepend-icon="item.iconCode" :value="false" color="primary">
              <template v-slot:activator>
                <v-list-item-content>
                  <v-list-item-title class="text-capitalize">{{
                    item.label
                  }}</v-list-item-title>
                </v-list-item-content>
              </template>

              <v-list-item
                v-for="(sub, subIndex) in item.subMenuList.filter(
                  (subMenu) => !subMenu.isHidden
                )"
                :key="subIndex"
                link
                :to="sub.path"
                value="true"
                color="primary"
              >
                <v-list-item-icon>
                  <v-icon v-text="sub.iconCode"></v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <v-list-item-title
                    v-text="sub.label"
                    class="text-capitalize"
                  ></v-list-item-title>
                </v-list-item-content>
                <!-- <router-link :to="sub.path" tag="a">{{ sub.label }}</router-link> -->
              </v-list-item>
            </v-list-group>
          </div>
        </div>
      </v-list-item-group>
    </v-list>

    <v-divider></v-divider>

    <!-- ACTIVE MODULE -->
    <v-select
      :label="$t('general.active-module')"
      :items="getUserModules"
      item-key="moduleId"
      item-text="moduleName"
      v-model="activeModule"
      return-object
      dense
      outlined
      class="px-7 mt-3 text-capitalize"
    ></v-select>
    <v-list dense>
      <v-list-item-group color="primary">
        <div
          v-for="(item, i) in menuItems.filter(
            (menu) =>
              menu.moduleId && menu.moduleId == activeModule.moduleId && !menu.isHidden
          )"
          :key="i"
        >
          <!-- LEAF MENU -->
          <v-list-item
            v-if="!item.subMenuList || item.subMenuList.length == 0"
            link
            :to="item.path"
            value="true"
            color="primary"
          >
            <v-list-item-icon>
              <v-icon v-text="item.iconCode"></v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-tooltip top :open-delay="500">
                <template v-slot:activator="{ on }">
                  <v-list-item-title
                    v-text="item.label"
                    class="text-capitalize"
                    v-on="on"
                  ></v-list-item-title>
                </template>
                <span class="text-capitalize">{{ item.label }}</span>
              </v-tooltip>
              <!-- <router-link :to="item.path" tag="a">{{ item.label }}</router-link> -->
            </v-list-item-content>
          </v-list-item>

          <!-- BRANCH MENU -->
          <div v-else>
            <v-list-group :prepend-icon="item.iconCode" :value="false" color="primary">
              <template v-slot:activator>
                <v-list-item-content>
                  <v-tooltip top :open-delay="500">
                    <template v-slot:activator="{ on }">
                      <v-list-item-title class="text-capitalize" v-on="on">{{
                        item.label
                      }}</v-list-item-title>
                    </template>
                    <span class="text-capitalize">{{ item.label }}</span>
                  </v-tooltip>
                </v-list-item-content>
              </template>

              <v-list-item
                v-for="(sub, subIndex) in item.subMenuList.filter(
                  (subMenu) => !subMenu.isHidden
                )"
                :key="subIndex"
                link
                :to="sub.path"
                value="true"
                color="primary"
              >
                <v-list-item-icon>
                  <v-icon v-text="sub.iconCode"></v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <v-tooltip top :open-delay="500">
                    <template v-slot:activator="{ on }">
                      <v-list-item-title
                        v-text="sub.label"
                        class="text-capitalize"
                        v-on="on"
                      ></v-list-item-title>
                    </template>
                    <span class="text-capitalize">{{ sub.label }}</span>
                  </v-tooltip>
                </v-list-item-content>
                <!-- <router-link :to="sub.path" tag="a">{{ sub.label }}</router-link> -->
              </v-list-item>
            </v-list-group>
          </div>
        </div>
      </v-list-item-group>
    </v-list>
  </v-navigation-drawer>
</template>
<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { MenuItem } from "../models/MenuItem";
import FormMixin from "../mixins/FormMixin";
import SessionManager from "../components/SessionManager.vue";

export default Vue.extend({
  name: "JrsAppNav",
  mixins: [FormMixin],
  props: {
    avatar: {
      type: String,
      required: false,
      default: null,
    },
  },
  components: {
    "jrs-session-manager": SessionManager,
  },
  data() {
    return {
      // activeModule: {moduleId:null, moduleName:null}
    };
  },
  computed: {
    ...mapGetters({
      menuItems: "getUserMenu", // map `this.menuItems` to `this.$store.getters.getUserMenu`
      getExpandedAppDrawer: "getExpandedAppDrawer",
      getUserModules: "getUserModules",
      getActiveModule: "getActiveModule",
    }),
    drawerIsExpanded: {
      get() {
        return this.getExpandedAppDrawer;
      },
      set(val) {
        let localThis: any = this as any;
        localThis.setAppDrawer(val);
      },
    },
    activeModule: {
      get() {
        let localThis: any = this as any;
        return localThis.getActiveModule;
      },
      set(newVal: any) {
        let localThis: any = this as any;
        localThis.setActiveModule(newVal);
      },
    },
  },
  methods: {
    ...mapActions({
      setActiveModule: "setActiveModule",
      toggleAppDrawer: "toggleAppDrawer",
      setAppDrawer: "setAppDrawer",
    }),
  },
  mounted() {
    let localThis: any = this as any;
    // Set app drawer state
    let breakpoint: String = localThis.$vuetify.breakpoint.name;
    if (breakpoint == "lg" || breakpoint == "xl") {
      localThis.setAppDrawer(true);
    }
  },
});
</script>
<style scoped></style>
