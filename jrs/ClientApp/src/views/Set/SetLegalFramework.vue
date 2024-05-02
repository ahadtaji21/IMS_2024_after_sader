<template>
  <v-content>
    <v-container fluid fill-height>
      <v-row>
        <v-col :cols="12">
          <jrs-simple-table
            viewName="SET_VI_SETTLEMENT_LEGAL_BY_COUNTRY"
            dataSourceCondition="IS_LATEST_VERSION = 1"
            :nbrOfFormColumns="4"
            :savePayload="{ territortialScopeId }"
            :fieldDatasourceConditions="[
              {
                field_name: 'SET_LEGAL_TERRITORIAL_SCOPE_ID',
                field_condition: 'HR_DOCUMENT_CLASS_ID = 1',
              },
            ]"
            :selectOnRowClick="true"
			:externalDialogOpen="externalDialogOpen"
          >
            <template v-slot:simple-table-item-actions="{ simpleItemRowItem }">
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-icon
                    @click="externalDialogOpen=true;openDialogLegal(simpleItemRowItem)"
                    v-bind="attrs"
                    v-on="on"
                    >mdi-library</v-icon
                  >
                </template>
                <span>{{ $t("general.crud.tooltip-history") }}</span>
              </v-tooltip>
            </template>
          </jrs-simple-table>
        </v-col>
      </v-row>

      <!-- HISTORY DIALOG FOR LEGAL -->
      <v-dialog
        v-model="dialogLegal"
        persistent
        retain-focus
        :scrollable="true"
        :overlay="false"
        :max-width="'90em'"
        transition="dialog-transition"
      >
        <v-card>
          <v-card-title>
            <span class="capital-first-letter">{{ $t("general.detail") }}</span>
          </v-card-title>
          <v-card-text>
            <set-legal-framework-form
              :countryId="selectedCountryId"
              :legalTypeId="selectedLegalTypeId"
              :showVersions="true"
              :key="`LEGAL_FRAMEWORK_${selectedCountryId}_${selectedLegalTypeId}`"
            ></set-legal-framework-form>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="secondary darken-1"
              class="mt-2 mr-1"
              text
              @click="externalDialogOpen=false;dialogLegal = false"
              >X {{ $t("general.close") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters, mapActions } from "vuex";
import { ImsLookup } from "../../axiosapi";
import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import SetLegalFrameworkForm from "../../components/SET/SetLegalFrameworkForm.vue";

export default Vue.extend({
  components: {
    "jrs-simple-table": JrsSimpleTable,
    "set-legal-framework-form": SetLegalFrameworkForm,
  },
  data: () => ({
	externalDialogOpen:false,
    territortialScopeId: 0,
    dialogLegal: false,
    legalVersionsHistory: [],
    currentVersionLegal: 0,
    selectedLegalVersion: null,
    selectedCountryId: 0,
    selectedLegalTypeId: 0,
  }),
  computed: {
    ...mapGetters(["getLookupsByTypeCode"]),
  },
  methods: {

    /**
     * Get the lookup value for TerrirotialScope of a country.
     */
    getTerritorialScope() {
      const localThis: any = this as any;
      const lookups: ImsLookup[] = localThis.getLookupsByTypeCode(
        "SET_LEGAL_TERRIT_SCOPE"
      );
      const terrScopeLookup: any = lookups.find(
        (lookup: ImsLookup) =>
          lookup.imsLookupCode == "SET_LEGAL_TERRIT_SCOPE_NATIONAL_LEVEL"
      );
      return terrScopeLookup ? terrScopeLookup.imsLookupId : null;
    },
    async openDialogLegal(rowData: any) {
      const localThis: any = this as any;

      // Set params required by form
      localThis.selectedCountryId = rowData.SET_LEGAL_COUNTRY_ID;
      localThis.selectedLegalTypeId = rowData.SET_LEGAL_TYPE_ID;
      localThis.dialogLegal = true;
    },
  },
  mounted() {
    const localThis: any = this as any;
    localThis.territortialScopeId = localThis.getTerritorialScope();
  },
});
</script>

<style scoped></style>
