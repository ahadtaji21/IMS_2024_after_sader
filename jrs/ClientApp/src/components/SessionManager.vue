<template>
    <div class="text-center">
        <v-menu offset-y :close-on-content-click="false">
            <template v-slot:activator="{ on }">
            <!-- <v-btn color="primary" dark slot="activator" v-on="on">Session</v-btn> -->
                <div>
                    <!--<v-icon>mdi-account-circle</v-icon>-->
                    <v-avatar size="30" v-if="avatar">
                        <!-- <img :src="avatar" /> -->
                    </v-avatar>

                    <!-- <span id="user-name">{{ loggedUser.username }} - {{ getCurrentOffice.HR_OFFICE_LEGAL_NAME }} - {{ getCurrentRole.DESCR }}</span> -->
                <div v-on="displayChoice === 'user-office' ? on : {}" :class="{'mouse-pointer': displayChoice === 'user-office'}">
                    <v-avatar size="30" v-if="avatar">
                        <!-- <img :src="avatar" /> -->
                    </v-avatar>
                    <span id="user-name" v-if="displayChoice === 'user-name'" class="mouse-default">{{ loggedUser.username.split('.').join(' ') }}</span>
                    <div v-if="displayChoice === 'user-office'" class="user-office-container">
                        <span id="user-office">{{ getCurrentOffice.HR_OFFICE_LEGAL_NAME }}</span>
                        <v-icon>mdi-dots-vertical</v-icon>
                    </div>
                </div>
                </div>
            </template>
            <v-list dense>
                <v-list-item 
                    v-for="(item, index) in menuItems"
                    :key="index"
                    @click="menuClick(item.code)"
                >
                    <v-list-item-icon>
                        <v-icon v-text="item.icon"></v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>{{ item.descr }}</v-list-item-content>
                </v-list-item>

                <!-- PERSONAL AREA -->
                <v-list-item link :to="{name:'personalArea'}">
                    <v-list-item-icon>
                        <v-icon>mdi-account-circle</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>{{$t('general.personal-area')}}</v-list-item-content>
                </v-list-item>

                <v-divider></v-divider>
                <v-list-item>
                    <v-select
                        :items="getUserOfficeList"
                        item-value="HR_OFFICE_ID"
                        item-text="HR_OFFICE_LEGAL_NAME"
                        v-model="activeOffice"
                        label="Office"
                        prepend-icon="mdi-office-building"
                        return-object
                    ></v-select>
                </v-list-item>
               <v-list-item>
                    <v-select
                        :items="getUserRoleList"
                        item-value="ROLE_ID"
                        item-text="DESCR"
                        v-model="activeRole"
                        label="Role"
                        prepend-icon="mdi-chevron-double-up"
                        return-object
                    ></v-select>
                </v-list-item>
                <v-list-item>
                    <jrs-lang-select></jrs-lang-select>
                </v-list-item>
            </v-list>
        </v-menu>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import LanguageSelector from './LanguageSelector.vue';
    import { mapGetters, mapActions } from 'vuex';
    import { EventBus } from "./../event-bus";
    import UtilMix from "../mixins/UtilMix";

    export default Vue.extend({
        components: {
            "jrs-lang-select": LanguageSelector
        },
        props:{
            avatar:{
            type: String,
            required: false,
            default: null
            },
            displayChoice: {
                type: String,
                required: false,
                default: 'user-name' // default to showing user-name
            }
        },
        mixins:[UtilMix],
        data(){
            return {
                menuItems: [
                    { code: "logout", descr: "Logout", icon:"mdi-exit-to-app", comp: null }
                ],
                activeOffice:{
                    HR_OFFICE_ID: null,
                    HR_OFFICE_LEGAL_NAME: null
                },
                activeRole:{
                    ROLE_ID: null,
                    DESCR: null,
                    ROLE_CODE:null
                }
            }
        },
        computed: {
            ...mapGetters('auth', {
                loggedUser: 'loggedUser'
            }),
            ...mapGetters({
                getUserOfficeList: 'getUserOfficeList',
                getCurrentOffice: 'getCurrentOffice',
                getUserRoleList: 'getUserRoleList',
                getCurrentRole: 'getCurrentRole',
            })
        },
        watch:{
            activeOffice(to:any, from:any){
                let localThis:any = this as any;
                if(to.HR_OFFICE_ID != from.HR_OFFICE_ID )
                    localThis.setCurrentOffice(to);
                //localThis.activeRole=undefined;
            },
            getUserOfficeList(to:any, from:any){
                let localThis:any = this as any;
                const cookieString:any = localThis.getCookie("cur_office");

                if(localThis.getCurrentOffice && localThis.getCurrentOffice.HR_OFFICE_ID){
                    localThis.activeOffice = localThis.getCurrentOffice;
                }else if(cookieString){
                    localThis.activeOffice = JSON.parse(cookieString);
                }else if(to && to.length > 0){
                    localThis.activeOffice = to[0];
                }
            },
            activeRole(to:any, from:any){
                let localThis:any = this as any;
                localThis.setCurrentRole(to);
               
                EventBus.$emit("userRoleUpdated",to);
               
            },
            getUserRoleList(to:any, from:any){
                let localThis:any = this as any;

                if(localThis.getCurrentRole && localThis.getCurrentRole.ROLE_ID && localThis.getCurrentRole.ROLE_ID>0){
                    localThis.activeRole = localThis.getCurrentRole;
                }else if(to && to.length > 0){
                    localThis.activeRole = to[0];
                }
                else
                {
                    let norole:any={} ;
                    norole.ROLE_ID=0;
                    norole.DESCR="";
                    norole.ROLE_CODE="";
                    localThis.activeRole=norole;
                }
            },
        },
            beforeDestroy() {
      //do something before destroying vue instance
        EventBus.$off('userRoleUpdated');
    },
        methods:{
            ...mapActions('auth', {
                logout: 'logout'
            }),
            ...mapActions({
                setCurrentOffice: 'setCurrentOffice',
                setCurrentRole: 'setCurrentRole'
            }),
            menuClick(itemCode:string){
                if(itemCode == 'logout'){
                    this.logout( { vm: this });
                }
            }
        }
    })
</script>

<style scoped>
    #user-name{
        margin-left: 0.5em;
    }
    .mouse-pointer{
        cursor: pointer;
    }
    .mouse-default{
        cursor:default;
    }

    .user-office-container {
        display: flex; /* Enables flexbox */
        align-items: center; /* Centers items vertically */
        justify-content: center; /* Centers items horizontally (if needed) */
    }
</style>