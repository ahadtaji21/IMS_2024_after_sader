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
                    prepend-icon="mdi-account-circle"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                    chips
                    clearable
                  >
                    <template v-slot:prepend-inner>
                        <v-chip
                            pill
                            v-on="on"
                            v-if="fieldValue"
                        >
                            <v-avatar left  size="6">
                            <v-img   :src="fieldValue" ></v-img>
                            </v-avatar>
                           {{label}}
                        </v-chip>
                        <v-chip
                        
                            pill
                            v-on="on"
                            v-else
                        >
                          <v-avatar color="indigo">
                            <v-icon dark>
                              mdi-account-circle
                            </v-icon>
                          </v-avatar>
                              {{label}}
                        </v-chip>
                </template>
               </v-text-field>
      </template>
      <v-card>
            <v-card-title  class="primary lighten-2 black--text">
                      {{label}}
        </v-card-title>
        <v-card-text>
             <!-- <v-image-input
                        v-model="fieldValue"
                        class="mt-1"
                        :image-quality="0.95"
                        image-format="jpeg"
                        clearable
                        >
                        </v-image-input>-->
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
    import { debug } from 'webpack';
    //import VuetifyImageInput  from "vuetify-image-input/a-la-carte";
 
    export default Vue.extend({

        components: {
         // "v-image-input": VuetifyImageInput
        },
        
        data(){
          
            return {
                modalSlideField: false,
                avatar_src: "",
                imageFile: [],
                imageName: ""
            }
        },
        computed: {
            ...mapGetters('auth',{
                loggedUser: 'loggedUser'
            }),
            fieldValue: {
                get(){
                    let localThis:any = this as any;
                     if(localThis.value){
                         return localThis.value;
                     }else{
                       return null
                     }
                
                },
                set(newVal:any){
                         (this as any).updateValue(newVal);   
                }
            },
        },
        methods:{
           updateValue(newVal: any) {
                 let localThis:any = this as any;
                 localThis.$emit("input", newVal);
            },
           cancel(){
                   let localThis:any = this as any;
                   localThis.modalSlideField = false;
           },
  
          
        },
        props:{
            // the image to upload 
            value:{
                type: String
            },
            // the label of field 
            label:{
                type:String,
                required:true,
                default: ""
            }

        }
        
    }
   )
  
</script>

<style scoped>

</style>