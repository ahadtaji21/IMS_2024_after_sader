<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row align-center>
                <v-col justify-center>
                    <h1 class="text-center h1 font-weight-bold primary--text">404</h1>
                    <h3 class="text-center secondary--text text--darken-1">{{ errorMessage }}</h3>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue'

    export default Vue.extend({
        props: {
            messageKey:{
				type: String,
				required: false,
				default: null
			}
        },
        data: () => ({
            errorMessage: null
        }),
        mounted(){
            const localThis:any = this as any;
            let key:string = "404-message";
            if(localThis.messageKey){
                key = localThis.messageKey;
            }
            localThis.errorMessage = localThis.$t(`views.view-404.${key}`).toString();
        }
    })
</script>

<style scoped>

</style>