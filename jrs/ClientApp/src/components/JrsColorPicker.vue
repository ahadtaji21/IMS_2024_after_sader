<template>
  <v-row justify="center">
    <v-dialog
      ref="dialog"
      v-model="modalSlideField"
      :return-value.sync="fieldValue"
      persistent
      max-width="500"
    >

      <template v-slot:activator="{ on, attrs }">
                 <v-text-field
                    v-model="fieldValue"
                    :label="label"
                    prepend-icon="mdi-format-color-fill"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                    chips
                    clearable
                  >
                    <template v-slot:append>
                 <v-avatar
                        :color="value"
                        size="25"
                        >
                        </v-avatar>
            </template>
               </v-text-field>
      </template>
      <v-card>
            <v-card-title  class="primary lighten-2 black--text">
                      {{label}}
        </v-card-title>
        <v-card-text>
           
        <v-color-picker
            class="ma-2"
            show-swatches
            swatches-max-height="100px"
            width="500px"
            flat
            elevation="15"
            v-model="fieldValue"
        ></v-color-picker>
        </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
                  <v-btn
                            color="secondary darken-1"
                            text
                            @click="cancel()"
                            >X {{ $t('general.close') }}</v-btn>
                   <v-btn
                    text
                    color="primary"
                    @click="$refs.dialog.save(fieldValue)"
                >{{ $t('general.save') }}</v-btn>
             
        </v-card-actions>
      </v-card>
    </v-dialog>

  </v-row>
</template>
<script lang="ts">
    import Vue from 'vue'
    import { mapActions, mapGetters } from "vuex";

    export default Vue.extend({
        data(){
            return {
                modalSlideField: false
            }
        },
        computed: {
            ...mapGetters('auth',{
                loggedUser: 'loggedUser'
            }),
            fieldValue: {
                get(){
                    let localThis:any = this as any;
                    return localThis.value ? localThis.value  : null ;
                },
                set(newVal:any){
                    if((this as any).value){
                         (this as any).updateValue(newVal);   
                    }else{
                           (this as any).updateValue(newVal.hex);
                   }
                }
            },
        },
        methods:{
           updateValue(newVal: any) {
                (this as any).$emit("input", newVal);
            },
           cancel(){
                   let localThis:any = this as any;
                   localThis.modalSlideField = false;
           },
          
        },
        props:{
            value:{
                type: String
            },
            label:{
                type:String,
                required:true
            }

        }
        
    })
</script>

<style scoped>

</style>