<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-card>
                        <v-tabs
                            v-model="tabIsVisible"
                            background-color="primary darken-1"
                            dark
                        >
                            <v-tab
                                v-for="tab in tabItems"
                                :key="tab.code"
                            >{{ $t(tab.tanslationKey) }}</v-tab>
                        </v-tabs>
                        <v-tabs-items v-model="tabIsVisible" style="padding: 0.5em">
                            
                            <!-- COI -->
                            <v-tab-item key="COI">
                                <jrs-simple-table
                                    :title="$t('datasource.pms.category-of-intervention.category-of-intervention')"
                                    viewName="PMS_VI_CATEGORY_OF_INTERVENTION"
                                    :nbrOfFormColumns="2"
                                ></jrs-simple-table>
                            </v-tab-item>

                        </v-tabs-items>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import JrsSimpleTable from '../components/JrsSimpleTable.vue';

    export default Vue.extend({
        components:{
            "jrs-simple-table":JrsSimpleTable
        },
        data(){
            return {
                tabItems:[
                    {
                        code:'COI',
                        descr:'Category of Intervention',
                        tanslationKey:'datasource.pms.category-of-intervention.category-of-intervention'
                    }
                ],
                tabIsVisible: null,
                genericQueryData:false
            }
        }
    })
</script>

<style scoped>

</style>