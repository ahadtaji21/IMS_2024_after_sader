<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-tabs v-model="tabModel"  centered background-color="primary darken-1" dark>
                        <v-tab v-for="table in tables" :key="table.code">{{ $t(table.tab_key) }}</v-tab>
                    </v-tabs>
                    <v-tabs-items v-model="tabModel" class="tab-item-wrapper">
                        <v-tab-item v-for="table in tables" :key="table.code">
                            <jrs-simple-table
                                :viewName="table.name"
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
    import JrsSimpleTable from '../../components/JrsSimpleTable.vue';

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable
        },
        data(){
            return {
                tabModel: null,
                tables: [
                    {name: 'HR_DEPARTMENT_AREA', code:"AREA", tab_key:"datasource.hrm.department-area.title"},
                    {name: 'HR_VI_DEPARTMENT', code:"DEP", tab_key:"datasource.hrm.department.title"}
                ]
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