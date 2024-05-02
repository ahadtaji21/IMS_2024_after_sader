<template>
  <v-tooltip top :open-delay="500">
    <template v-slot:activator="{ on }">
      <div
        class="menu-item d-flex flex-grow-1"
        v-on="on"
        :id="`MENU_ITEM_${menu.id}`"
      >
        <v-btn
          outlined
          class="menu-btn flex-grow-1 justify-start"
          :color="getColor()"
          small
          @click="updateActiveModule"
        >
          <v-icon left class="menu-icon" id="ModuleDetialIcon">{{
            menu.iconCode
          }}</v-icon>
          <span
            style="color: black; white-space: normal"
            class="menu-label"
            :id="`MENU_LABEL_${menu.id}`"
            :ref="`MENU_LABEL_${menu.id}`"
            >{{ $t(menu.labelKey) }}</span
          >
        </v-btn>
      </div>
    </template>
    <span>{{ $t(menu.labelKey) }}</span>
  </v-tooltip>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import {
  MenuItem,
  ImsModule,
  moduleColors as modColors,
} from "../models/MenuItem";

export default Vue.extend({
  props: {
    menu: {
      type: Object as () => MenuItem,
      required: true,
    },
    module: {
      type: Object as () => ImsModule,
      required: false,
      default: null,
    },
  },
  data: () => ({
    moduleColors: [],
  }),
  computed: {
    // isEllipsisActive() {
    //   const id:string = `MENU_LABEL_${this.menu.id}`;
    //   let e:any = document.getElementById(id);
    //   if(!e){
    //     return false;
    //   }
    //   var c = e.cloneNode(true);
    //   c.style.display = 'inline';
    //   c.style.width = 'auto';
    //   c.style.visibility = 'hidden';
    //   document.body.appendChild(c);
    //   const truncated = c.offsetWidth >= e.clientWidth;
    //   if(["MENU_LABEL_6101"].includes(id)){
    //     console.log('test');
    //   }
    //   c.remove();
    //   return truncated;
    // },
  },
  methods: {
    ...mapActions({
      setActiveModule: "setActiveModule",
    }),
    getColor() {
      let localThis: any = this as any;
      let color: any = null;
      color = localThis.moduleColors.find((mod: any) => {
        return mod.moduleName == localThis.menu.moduleName;
      });
      return color ? color.moduleColor : null;
    },
    /**
     * Set the active module based on the selected menu,
     * if one has been provided by the parnt component.
     */
    updateActiveModule() {
      let localThis: any = this as any;
      localThis.setActiveModule(localThis.module);

      const path = localThis.menu.path;
      localThis.$router.push(path );
    },
  },
  mounted() {
    let localThis: any = this as any;
    localThis.moduleColors = modColors;
  },
});
</script>

<style>
.menu-item,
.menu-btn {
  width: 100%;
}
/* .menu-btn:first-child{
    background-color: cadetblue;
  } */
.menu-btn > .v-btn__content {
  width: 100%;
}

.menu-icon {
  /* margin-right: auto; */
}
.menu-label {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 85%;
  /* margin-right:auto; */
}
</style>
