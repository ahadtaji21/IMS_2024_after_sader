<template>
  <v-row justify="center">
    <v-dialog
      ref="dialog"
      v-model="modalSlideField"
      :return-value.sync="fieldValue"
      persistent
      max-width="350"
    >
      <template v-slot:activator="{ on, attrs }">
                 <v-text-field
                    v-model="fieldValue"
                    :label="label"
                    prepend-icon="mdi-clipboard-list-outline"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                  ></v-text-field>
      </template>
      <v-card>
            <v-card-title  class="primary lighten-2 black--text">
                      {{label}}
        </v-card-title>
        <v-card-text>
            <v-row>
             <v-col
                cols="6"
                sm="6"
              >
            <v-text-field
                v-model="minValue"
                label="Min Value"
                readonly
             ></v-text-field>
           </v-col>

           <v-col
                cols="6"
                sm="6"
           >
             <v-text-field
                    v-model="maxValue"
                    label="Max Value"
                    readonly
              ></v-text-field>
            </v-col>
           </v-row>
             <v-slider  
                    v-model="fieldValue"    
                    thumb-label="always"
                    :min="minValue"
                    :max="maxValue"
                    class="mt-10"
                     :thumb-size="40"
                 ></v-slider> 
                <v-text-field
                    v-model="fieldValue"
                    :label="label"
                    prepend-icon="mdi-clipboard-list-outline"
                    readonly
                  ></v-text-field>
        </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
                  <v-btn
                            color="secondary darken-1"
                            text
                            @click="modalSlideField = false"
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
                    return localThis.value;
                },
                set(newVal:any){
                    //(this as any).fiedlValue = newVal;
                    (this as any).updateValue(newVal);
                }
            },
        },
        methods:{
           updateValue(newVal: number) {
                (this as any).$emit("input", newVal);
            },
        },
        props:{
            value:{
                type: [Number,String]
            },
            minValue:{
                //type: Object as () => JrsFormField,
                type: Number,
                required: false
            },
            maxValue:{
                //type: Object as () => JrsFormField,
                type: Number,
                required: false
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