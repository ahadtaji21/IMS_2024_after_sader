<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <!-- TABLE -->
                    <jrs-simple-table
                        v-model="selectedTables"
                        viewName="IMS_TABLES"
                        :nbrOfFormColumns="2"
                        :selectable="true"
                    ></jrs-simple-table>
                </v-col>
            </v-row>
            <v-row>
                <v-col :cols="12">
                    <jrs-simple-table
                        viewName="IMS_VI_TABLE_COLUMN_DETAILS"
                        :dataSourceCondition="`TABLE_NAME = '${selectedTables[0].TABLE_NAME}'`"
                        v-if="selectedTables && selectedTables.length > 0"
                        :key="selectedTables[0].TABLE_NAME"
                        :nbrOfFormColumns="3"
                    ></jrs-simple-table>
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
            return{
                selectedTables:[]
            };
        }
        
    })
</script>

<style scoped>

</style>