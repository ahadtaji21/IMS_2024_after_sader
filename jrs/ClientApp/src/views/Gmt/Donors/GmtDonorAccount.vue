<template>
    <v-content>
        <v-container fluid fill-height>
            <v-row>
                <v-col :cols="12">
                    <v-card>
                        <jrs-simple-table 
                            :title="$t('datasource.pms.donor-account.title')"
                            viewName="GMT_VI_DONORS_CHART_OF_ACCOUNT" />
                        </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-content>
</template>
<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import JrsSimpleTableVue from '../../../components/JrsSimpleTable.vue';
import { GenericSqlSelectPayload } from '../../../axiosapi';

@Component({
    components: {'jrs-simple-table': JrsSimpleTableVue}
})
export default class GmtDonorAccount extends Vue {
}
</script>