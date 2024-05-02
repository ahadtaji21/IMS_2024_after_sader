<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <!-- TABLE -->
                    <jrs-simple-table
                        :viewName="tableName"
                        :nbrOfFormColumns="2"
                        ref="simple_table"
                        v-if="tableName != ''"
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
            return {
                tableName: ''
            }
        },
        watch: {
            $route(to:any, from:any){
                (this as any).tableName = to.params.table_name;
            }
        },
        mounted(){
            this.tableName = this.$route.params.table_name
        }
    })
</script>

<style scoped>

</style>