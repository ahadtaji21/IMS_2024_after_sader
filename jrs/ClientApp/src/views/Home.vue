<template>
    <!-- <v-content>
      <v-container fluid fill-height>
        <v-row align-center>
          <v-col justify-center>
            <HelloWorld msg="Welcome to Your Vue.js App" />
          </v-col>
        </v-row>
      </v-container>
    </v-content>-->
    <v-content>
        <v-container>
            <v-row class="justify-center my-Home-Page">
                <transition-group name="card-transition" tag="div" class="row">
                    <v-col :cols="getColumnSize(mod.moduleId)"
                           v-for="(mod, modIndex) in getUserModules"
                           :key="modIndex"
                           class="d-flex">
                        <v-card 
                                @click="() => navigateToModuleDetail(mod.moduleId,mod.moduleName)"
                                :class="['custom-card', { 'expanded-card': expandedCard === mod.moduleId }]"
                                :style="{ 'borderBottomColor': getColor(mod)
                                          }"
                                >
                            <div class="card-content" v-if="expandedCard !== mod.moduleId">
                                <!-- Collapsed card content with logo -->
                                <img  :src="getModuleLogo(mod.moduleName)" :alt="mod.moduleName + ' Logo'"/>
                                <!--<span class="module-name">{{ mod.moduleName }}</span>-->
                            </div>
                            <div v-if="expandedCard === mod.moduleId" class="card-expanded-content">
                                <v-card-title>{{ mod.moduleName }}</v-card-title>
                                <v-card-text>
                                    <!-- ROOT MENU -->
                                    <v-list v-for="(list, listIndex) in groupedMenu.filter( (group) => { return group.moduleId == mod.moduleId && group.parentMenuId == null })"
                                            :key="'ROOT_' + listIndex"
                                            dense>
                                        <v-list-item v-for="(menu, menuIndex) in list.menuList.filter( (rootMenu) => { return !rootMenu.subMenuList[0]; })"
                                                     :key="menuIndex">
                                            <menu-btn :menu="menu" :module="menu.module"></menu-btn>
                                        </v-list-item>
                                    </v-list>

                                    <!-- GROUPED MENU -->
                                    <v-list v-for="(list, listIndex) in groupedMenu.filter( (group) => { return group.moduleId == mod.moduleId && group.parentMenuId != null })"
                                            :key="'CHILD_' + listIndex"
                                            subheader
                                            dense>
                                        <v-subheader>{{$t(list.parentMenuLabel)}}</v-subheader>
                                        <v-list-item v-for="(menu, menuIndex) in list.menuList" :key="menuIndex">
                                            <menu-btn :menu="menu" :module="menu.module"></menu-btn>
                                        </v-list-item>
                                    </v-list>
                                </v-card-text>
                            </div>
                            <!--<template v-else>-->
                            <!-- Collapsed card content -->
                            <!--<div class="module-logo">
                        <img :src="getModuleLogo(mod.moduleName)" :alt="mod.moduleName + ' Logo'" />
                    </div>
                </template>-->
                        </v-card>
                    </v-col>
                </transition-group>

            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    // @ is an alias to /src
    import Vue from "vue";
    import { mapGetters, mapActions } from "vuex";
    import MenuBtn from "../components/MenuBtn.vue";
    // import HelloWorld from "@/components/HelloWorld.vue";

    export default {
        name: "home",
        components: {
            // HelloWorld
            "menu-btn": MenuBtn
        },
        computed: {
            ...mapGetters({
                getUserModules: "getUserModules",
                getUserMenu: "getUserMenu"
            }),
            groupedMenu() {
                let localThis: any = this as any;
                let grouped: Array<any> = [];

                grouped = localThis.flatMenuList
                    .filter((menuItem: any) => !menuItem.isHidden)
                    .map((menuItem: any) => {
                        if (menuItem.parentMenuId) {
                            let parentMenu = localThis.flatMenuList.find(
                                (parentMenu: any) => parentMenu.id == menuItem.parentMenuId
                            );
                            menuItem.parentMenuLabel = parentMenu
                                ? parentMenu.labelKey
                                : undefined;
                        }
                        // Add module definition to menuItem
                        menuItem.module = localThis.getUserModules.find((mod: any) => mod.moduleName == menuItem.moduleName);
                        return menuItem;
                    })
                    .reduce((accumulator: Array<any>, currentItem: any) => {
                        let currentGroupIndex = accumulator.findIndex(
                            (group: any) =>
                                group.parentMenuId == currentItem.parentMenuId &&
                                group.moduleId == currentItem.moduleId
                        );

                        // Create new grouping
                        if (currentGroupIndex == -1) {
                            accumulator.push({
                                parentMenuId: currentItem.parentMenuId,
                                parentMenuLabel: currentItem.parentMenuLabel,
                                moduleId: currentItem.moduleId,
                                menuList: [currentItem],
                                module: currentItem.module
                            });
                        } else {
                            accumulator[currentGroupIndex].menuList.push(currentItem);
                        }
                        return accumulator;
                    }, []);
                return grouped;
            },

            moduleCols() {
                let localThis: any = this as any;
                let modules = localThis.getUserModules;
                let breakpoint = localThis.$vuetify.breakpoint;
                let cols: Number;

                if (localThis.expandedCard) {
                    // If a card is expanded, it takes the full width.
                    cols = 5;
                } else {
                    // If no card is expanded, adjust the number of columns based on the screen size.
                    switch (breakpoint.name) {
                        case 'lg':
                        case 'xl':
                        case 'md':
                            cols = 4; // 3 columns for larger screens (large, xlarge, and medium)
                            break;
                        default:
                            cols = 5; // 1 column for small screens
                            break;
                    }
                }
                return cols;
            }
        },
        data(): {
            flatMenuList: any[],
            expandedCard: null,
            clickedCard: null,
            hover:null

        } {
            return {
                flatMenuList: [],
                expandedCard: null,
                clickedCard: null,
                hover:null
            };
        },
        methods: {
            ...mapActions({
                caclculateFlatMenu: "caclculateFlatMenu"
            }),
            /**
             * Get flat menu list for the user.
             */
            setupMenu() {
                let localThis: any = this as any;
                localThis.caclculateFlatMenu().then((val: any) => {
                    localThis.flatMenuList = val;
                });
            },
            getColumnSize(moduleId: string | number) {
                if ((this as any).expandedCard === moduleId) {
                    // If this card is expanded
                    return 5; // Full width for the expanded card
                } else {
                    // For non-expanded cards
                    return 4; // Adjust this as per your design for non-expanded cards
                }
            },
            //toggleCard(moduleId: string | number) {
            //    if ((this as any).expandedCard === moduleId) {
            //        (this as any).expandedCard = null;
            //    } else {
            //        (this as any).expandedCard = moduleId;
            //    }
            //},
            toggleCard(moduleId: string | number) {
                (this as any).expandedCard = (this as any).expandedCard === moduleId ? null : moduleId;

            },
            navigateToModuleDetail(moduleId: string | number, moduleName:String) {
                let localThis: any = this as any;
                const filteredMenu = localThis.flatMenuList.filter((menuItem: any) => menuItem.moduleId === moduleId && !menuItem.isHidden && menuItem.path !== null && menuItem.path !== "/en/");
                //const specificModuleGroupedMenu = localThis.groupedMenuSpecefic(moduleId);
                const moduleDetailsObj = {
                    moduleId,
                    moduleName,
                    filteredMenu
                };
                (this as any).$router.push({ name: 'ModuleDetail', params: { moduleDetailsObj } });
            },
            //expandCard(moduleId: string | number) {
            //    (this as any).expandedCard = moduleId;
            //},
            //collapseCard() {
            //    (this as any).collapseTimeout = setTimeout(() => {
            //        (this as any).expandedCard = null;
            //    }, 300); // Delay of 300 milliseconds
            //},
            getModuleLogo(moduleName: string) {
                // Convert the module name to a format suitable for filename, for example: "Dashboard" -> "Dashboard.png"
                return require('@/assets/moduleLogos/' + moduleName + '.png');
            },
            getColor(mod:any) {
                let localThis: any = this as any;
                let color: any = null;
                //color = localThis.getUserModules.find((mod: any) => {
                //    return mod.moduleName == localThis.menuItem.moduleName;
                //});
                color = mod.moduleColor
               // return color ? color.moduleColor : null;
                return color || '#5b5f6b'; //? color.moduleColor : null;
            },
            getHoverStyle(mod:any) {
                const color = mod.moduleColor || '#5b5f6b'; // Fallback color
                return {
                    boxShadow: `0 4px 8px ${color} !important`,
                };
            },
        },
        mounted() {
            let localThis: any = this as any;
            localThis.setupMenu();
        }
    };
</script>