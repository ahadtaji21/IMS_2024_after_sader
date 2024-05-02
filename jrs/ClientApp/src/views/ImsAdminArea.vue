<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <h1 class="text-capitalize">{{ $t('datasource.ims.admin-area.title') }}</h1>
                    <p>{{ $t('datasource.ims.admin-area.page-descr') }}</p>
                    <v-tabs v-model="tabModel"
                        background-color="primary darken-1"
                        dark
                        centered
                    >
                        <v-tab v-for="areaType in  adminAreaTypes" :key="areaType.IMS_ADMIN_AREA_TYPE_CODE">{{ areaType.IMS_ADMIN_AREA_TYPE_NAME }}</v-tab>
                    </v-tabs>
                    <v-tabs-items v-model="tabModel" class="tab-item-wrapper">
                        <v-tab-item v-for="areaType in  adminAreaTypes" :key="areaType.IMS_ADMIN_AREA_TYPE_CODE">
                            <p>
                                <span class="text-uppercase font-weight-medium">{{$t('general.definition')}}:</span> {{ areaType.IMS_ADMIN_AREA_TYPE_DESCR }}
                                <jrs-simple-table
                                    viewName="IMS_VI_ADMIN_AREA_BY_TYPE"
                                    :title="areaType.IMS_ADMIN_AREA_TYPE_NAME"
                                    :dataSourceCondition="`IMS_ADMIN_AREA_TYPE_CODE = '${areaType.IMS_ADMIN_AREA_TYPE_CODE}'`"
                                    :savePayload="{ IMS_ADMIN_AREA_TYPE_CODE: areaType.IMS_ADMIN_AREA_TYPE_CODE }"
                                    :fieldDatasourceConditions="[{
                                        field_name:'ADMIN_AREA_PARENT_ID', 
                                        field_condition:`IMS_ADMIN_AREA_TYPE_ID = ISNULL(${areaType.IMS_ADMIN_AREA_TYPE_PARENT_ID || 'null'}, IMS_ADMIN_AREA_TYPE_ID)`
                                    }]"
                                    :clientPagination="true"
                                    :showSearchField="true"
                                    :selectOnRowClick="true"
                                ></jrs-simple-table>
                            </p>
                        </v-tab-item>
                    </v-tabs-items>
                </v-col>  
            </v-row>
        </v-container>
    </v-content>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { mapActions } from 'vuex';
    import JrsSimpleTable from '../components/JrsSimpleTable.vue';
    import {
        GenericSqlSelectPayload,
        GenericSqlPayload,
        SqlActionType
    } from "../axiosapi/api";

    export default Vue.extend({
        components:{
            'jrs-simple-table': JrsSimpleTable
        },
        data(){
            return {
                tabModel:null,
                adminAreaTypes: [],
            }
        },
        methods: {
            ...mapActions("apiHandler", {
              getGenericSelect: "getGenericSelect"
            }),
            /**
             * Recover list of administrative area types.
             */
            getAdminAreaType(){
                let localThis:any = this as any;

                let selectPayload: GenericSqlSelectPayload = {
                    viewName: 'IMS_ADMIN_AREA_TYPE',
                    colList: 'IMS_ADMIN_AREA_TYPE_CODE, IMS_ADMIN_AREA_TYPE_NAME, IMS_ADMIN_AREA_TYPE_DESCR, IMS_ADMIN_AREA_TYPE_PARENT_ID',
                    whereCond: 'IMS_ADMIN_AREA_TYPE_CODE != \'GLOB\'',
                    orderStmt: 'IMS_ADMIN_AREA_TYPE_LVL'
                };

                return localThis
                    .getGenericSelect({ genericSqlPayload: selectPayload })
                    .then( (res:any) => {
                        localThis.adminAreaTypes = res.table_data;
                    });
            }
        },
        mounted(){
            let localThis:any = this as any;
            localThis.getAdminAreaType();
        }       
    })
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
</style>