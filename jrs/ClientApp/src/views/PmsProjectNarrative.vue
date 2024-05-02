<template>
  <v-content>
    <v-container>
      <!-- PROJECT SELECTION-->
      <v-row>
        <v-col :cols="12">
          <jrs-simple-table
            v-model="selectedProject"
            viewName="PMS_VI_PROJECT"
            :columnList="projectLIstColumns"
            :selectable="true"
          ></jrs-simple-table>
        </v-col>
      </v-row>
      
      <!-- NARRATIVE DEFINITION -->
      <v-row v-if="selectedProject && selectedProject.length > 0">
        <v-col :cols="12">
          <v-toolbar color="primary darken-1" dark>
            <v-toolbar-title class="capital-first-letter">{{ $t('datasource.pms.project-narrative.title') }}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn color="secondary" @click="saveNarrativeData()">
              <v-icon>mdi-content-save-all</v-icon>{{ $t('general.save-all') }}
            </v-btn>

            <template v-slot:extension>
              <v-tabs v-model="activeTab" centered background-color="primary darken-1" dark>
                <v-tab v-for="tab in narrativeDefinitionTabs" :key="tab.key">{{ $t(tab.translationKey) }}</v-tab>
              </v-tabs>
            </template>
          </v-toolbar>

          <div class="elevation-1">
            <v-tabs-items v-model="activeTab" class="tab-item-wrapper">
              <!-- CONTEXT -->
              <v-tab-item key="CONTEXT">
                <vue-editor v-model="contextNarrative.SECTION_TEXT" class="narrative-section"></vue-editor>

                <v-divider></v-divider>
                <h4>{{ $t('datasource.pms.project-narrative.other-context-sections') }}</h4>

                <v-card
                  v-for="(section, sectionIndex) in contextCustomSections.filter( section => !section.REMOVED )"
                  :key="sectionIndex"
                  class="context-node"
                >
                  <v-card-title>
                    <v-text-field
                      :name="'section-' + sectionIndex"
                      label="section title"
                      v-model="section.SECTION_TITLE"
                      single-line
                      counter="250"
                    ></v-text-field>
                    <v-spacer></v-spacer>
                    <v-btn color="primary" @click="removeSection(section.ID)">{{ $t('datasource.pms.project-narrative.remove-section') }}</v-btn>
                  </v-card-title>
                  <v-card-text>
                    <vue-editor v-model="section.SECTION_TEXT"></vue-editor>
                  </v-card-text>
                </v-card>
                <v-btn color="primary" @click="addNewContextSection()">{{ $t('datasource.pms.project-narrative.add-section') }}</v-btn>
              </v-tab-item>

              <!-- RATIONALE -->
              <v-tab-item key="RATIONALE">
                <vue-editor v-model="rationalNarrative.SECTION_TEXT" class="narrative-section"></vue-editor>
              </v-tab-item>

              <!-- GOAL -->
              <v-tab-item key="GOAL">
                <vue-editor v-model="goalNarrative.SECTION_TEXT" class="narrative-section"></vue-editor>
              </v-tab-item>
            </v-tabs-items>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import { VueEditor } from "vue2-editor";
import { translate } from "../i18n";
import JrsSimpleTable from "../components/JrsSimpleTable.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType
} from "../axiosapi/api";

interface NarrativeSection {
  ID: number | null;
  CODE: string;
  REFERENCE_ID: number;
  SECTION_TITLE?: string;
  SECTION_TEXT: string;
  REMOVED?: boolean;
}

export default Vue.extend({
  components: {
    "vue-editor": VueEditor,
    "jrs-simple-table": JrsSimpleTable
  },
  data() {
    return {
      projectLIstColumns: ["ID", "CODE", "DESCR", "IMS_ADMIN_AREA_NAME"],
      activeTab: null,
      selectedProject: null,
      narrativeDefinitionTabs: [
        { key: "CONTEXT", name: "Contex of project", translationKey: "datasource.pms.project-narrative.context" },
        { key: "RATIONALE", name: "Rationale", translationKey: "datasource.pms.project-narrative.rational" },
        { key: "GOAL", name: "Overall Goal", translationKey: "datasource.pms.project-narrative.goal" }
      ],
      contextNarrative: {
        ID: null,
        CODE: "CONTEXT",
        SECTION_TITLE: "",
        SECTION_TEXT: "",
        REFERENCE_ID: 0
      } as NarrativeSection,
      contextCustomSections: [] as Array<NarrativeSection>,
      rationalNarrative: {
        ID: null,
        CODE: "RATIONALE",
        SECTION_TITLE: "",
        SECTION_TEXT: "",
        REFERENCE_ID: 0
      } as NarrativeSection,
      goalNarrative: {
        ID: null,
        CODE: "RATIONALE",
        SECTION_TITLE: "",
        SECTION_TEXT: "",
        REFERENCE_ID: 0
      } as NarrativeSection
    };
  },
  computed: {
    narrativeData() {
      let localThis: any = this as any;
      return [
        ...localThis.contextCustomSections,
        localThis.contextNarrative,
        localThis.rationalNarrative,
        localThis.goalNarrative
      ];
    }
  },
  watch: {
    selectedProject(to: any, from: any) {
      if (to[0]) {
        // Clear narrative sections
        this.clearNarrativeData(to[0].ID);

        // Set narrative sections for selected project
        this.setNarrativeData(to[0].ID);
      }
    }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar"
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall"
    }),
    /**
     * Clear narrative sections.
     * @param projectId - ID of the project
     */
    clearNarrativeData(projectId: number) {
      let localThis: any = this as any;
      localThis.contextNarrative = {
        ID: null,
        CODE: "CONTEXT",
        SECTION_TEXT: "",
        REFERENCE_ID: projectId
      };
      localThis.contextCustomSections.lenght = 0;
      localThis.rationalNarrative = {
        ID: null,
        CODE: "RATIONALE",
        SECTION_TEXT: "",
        REFERENCE_ID: projectId
      };
      localThis.goalNarrative = {
        ID: null,
        CODE: "GOAL",
        SECTION_TEXT: "",
        REFERENCE_ID: projectId
      };
    },
    /**
     * Load Project narrative from server and set in tabs.
     * @param projectId - ID of the project
     */
    setNarrativeData(projectId: number) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_VI_NARRATIVE_BY_TYPE",
        colList: null,
        whereCond: `REFERENCE_ID = ${localThis.selectedProject[0].ID}`,
        orderStmt: null
      };

      // Reset Narrative sections
      localThis.contextCustomSections = [];

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            res.table_data.forEach((item: any) => {
              switch (item.CODE) {
                case "CONTEXT":
                  localThis.contextNarrative = item;
                  break;

                case "CONTEXT_NODE":
                  localThis.contextCustomSections.push(item);
                  break;

                case "RATIONALE":
                  localThis.rationalNarrative = item;
                  break;

                case "GOAL":
                  localThis.goalNarrative = item;
                  break;

                default:
                  break;
              }
            });
          }
        }
      );
    },
    /**
     * Save the project narrative sections.
     */
    saveNarrativeData() {
      let localThis: any = this as any;

      // Check if all context sections have a title and a body
      let invalidSectionIndex = localThis.contextCustomSections.findIndex(
        (section: NarrativeSection) => {
          return (
            !section.REMOVED &&
            (section.SECTION_TITLE == null ||
              section.SECTION_TEXT == null ||
              section.SECTION_TITLE == "" ||
              section.SECTION_TEXT == "")
          );
        }
      );
      if (invalidSectionIndex > -1) {
        localThis.showNewSnackbar({
          typeName: "warning",
          text:
            "Warning: All context sections defined must have a title and a body."
        });
        return;
      }

      let savePayload: GenericSqlPayload = {
        spName: "IMS_SP_INS_UPD_NARRATIVE_SECTION",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(localThis.narrativeData)
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    /**
     * Setup new context node
     */
    addNewContextSection() {
      let localThis = this as any;
      localThis.contextCustomSections.push({
        REFERENCE_ID: localThis.selectedProject[0].ID,
        CODE: "CONTEXT_NODE",
        SECTION_TITLE: null,
        SECTION_TEXT: null
      });
    },
    /**
     * Remove the context node at the given index.
     * @param index - index of the elemet to remove
     */
    removeSection(sectionId: number) {
      let localThis = this as any;
      localThis.contextCustomSections = localThis.contextCustomSections.map(
        (section: NarrativeSection) => {
          if (section.ID == sectionId) {
            section.REMOVED = true;
          }
          return section;
        }
      );
    }
  }
});
</script>

<style scoped>
.tab-item-wrapper {
  padding: 0.2em 1em 1em 1em;
}
.narrative-section {
  margin-bottom: 1em;
}
.context-node {
  margin: 1em 0.5em;
}
</style>