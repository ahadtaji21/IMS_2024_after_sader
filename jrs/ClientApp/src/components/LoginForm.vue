<template>
    <v-card id="login-card" max-width="500">
        <v-card-title primary-title class="primary white--text text-center">Login</v-card-title>
        <v-form>
            <v-container grid-list-xs>
                <v-row>
                    <v-col>
                        <v-text-field
                            name="username"
                            :label="$t('general.username')"
                            v-model="accessData.username"
                            :counter="150"
                            required
                        ></v-text-field>
                        <v-text-field
                            name="password"
                            :label="$t('general.password')"
                            v-model="accessData.password"
                            hint="At least 8 characters"
                            :counter="150"
                            min="8"
                            :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                            @click:append="showPassword = !showPassword"
                            :type="!showPassword ? 'password' : 'text'"
                        ></v-text-field>
                    </v-col>
                </v-row>
            </v-container>
        </v-form>
        <v-card-actions align-end>
            <div class="d-flex justify-end" style="width:100%">
                <v-btn
                    color="primary"
                    class="--darken-1"
                    @click="submit(accessData)"
                >Login</v-btn>
            </div>
            
        </v-card-actions>
    </v-card>
</template>
<script lang="ts">
    import Vue from 'vue'
    import { mapActions, mapGetters } from "vuex";

    export default Vue.extend({
        data(){
            return {
                accessData:{
                    username: null,
                    password: null
                },
                showPassword: false
            }
        },
        computed: {
            ...mapGetters('auth',{
                loggedUser: 'loggedUser'
            })
        },
        methods: {
            ...mapActions({
                updateMenuForLang: 'updateMenuForLang'
            }),
            ...mapActions('auth',{
                login: 'login'
            }),
            submit(accessData:any){
                let localThis = this as any;
                this.login(accessData).then( res => {
                    this.$router.push(localThis.$route)
                        .catch( (err:any) => {});
                })
            }
        }
        
    })
</script>

<style scoped>

</style>