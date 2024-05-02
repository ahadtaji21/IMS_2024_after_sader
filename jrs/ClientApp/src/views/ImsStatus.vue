<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-tabs  v-model="tabModel"  centered background-color="primary darken-1" dark>
                        <v-tab  @click="forceRerenderTab()" v-for="table in tables" :key="table.code">{{ $t(table.tab_key) }}</v-tab>
                    </v-tabs>
                    <v-tabs-items v-model="tabModel" class="tab-item-wrapper">
                        <v-tab-item  v-for="table in tables" :key="table.code">
                            <jrs-simple-table
                                :viewName="table.name"
                                v-if="renderTab"
                                :savePayload="{userUid:getUserUid, officeId:getCurrentOffice.HR_OFFICE_ID}"
                            ></jrs-simple-table>
                        </v-tab-item>
                    </v-tabs-items>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapGetters } from 'vuex';
    import JrsSimpleTable from '../components/JrsSimpleTable.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable
        },
        data(){
            return {
                tabModel: null,
                tables: [
                    {name: 'IMS_VI_STATUS', code:"STATUS", tab_key:"datasource.ims.status.title"},
                    {name: 'IMS_VI_STATUS_CLASS', code:"STATUS_CLASS", tab_key:"datasource.ims.status.class.title"}
                ],
                renderTab: true,
            }
        },
        methods:{
                forceRerenderTab() {
                        // Remove my-component from the DOM
                        this.renderTab = false;

                        this.$nextTick(() => {
                        // Add the component back in
                        this.renderTab = true;
                        });
                    }
                
        },
        computed: {
            ...mapGetters({
            getUserUid: 'getUserUid',
            getCurrentOffice: 'getCurrentOffice'
            })
        }
    })
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>