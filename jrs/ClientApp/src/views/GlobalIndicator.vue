<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-card>
                        <jrs-simple-table 
                            :title="$t('datasource.pms.global-indicators.title')"
                            viewName="PMS_VI_GLOBAL_INDICATOR" />
                        </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>
<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import JrsSimpleTableVue from '../components/JrsSimpleTable.vue';
import { GenericSqlSelectPayload } from '../axiosapi';

@Component({
    components: {'jrs-simple-table': JrsSimpleTableVue}
})
export default class PmsGlobalIndicator extends Vue {
}
</script>