<template>
  <v-row v-if="selectedObjectId != undefined && selectedObjectId != null">
    <v-col :cols="12">
      <v-toolbar color="primary darken-1" dark>
        <v-spacer></v-spacer>
        <v-btn
          color="secondary"
          @click="saveNarrativeData()"
          v-if="isUpdatableForm && false"
        >
          <v-icon>mdi-content-save-all</v-icon>
          {{ $t("general.save-all") }}
        </v-btn>

        <template v-slot:extension>
          <v-tabs v-model="activeTab" centered background-color="primary darken-1" dark>
            <v-tab v-for="tab in narrativeDefinitionTabs" :key="tab.key">{{
              $t(tab.translationKey)
            }}</v-tab>
          </v-tabs>
        </template>
      </v-toolbar>

      <div class="elevation-1">
        <v-tabs-items v-model="activeTab" class="tab-item-wrapper">
          <!-- DYNAMIC -->
          <v-tab-item v-for="n in narrativeDefinitionTabs" :key="n.key">
            <vue-editor
              v-if="typeOfNarrative[n.idx]"
              v-model="typeOfNarrative[n.idx].SECTION_TEXT"
              class="narrative-section"
              @onchange="editorChange()"
              api-key="u1zaygen61a491aghepiyyboodl01fl3flltxllqyy8937nq"
              :init="{
                height: 500,
                menubar: false,
                plugins: [
                  'advlist autolink lists link image charmap print preview anchor',
                  'searchreplace visualblocks code fullscreen',
                  'insertdatetime media table paste code help wordcount',
                ],
                toolbar:
                  'undo redo | formatselect | bold italic backcolor | \
                  alignleft aligncenter alignright alignjustify | \
                  bullist numlist outdent indent | removeformat | help',
              }"
            ></vue-editor>
            <vue-editor
              v-else
              class="narrative-section"
              @onchange="editorChange()"
              api-key="u1zaygen61a491aghepiyyboodl01fl3flltxllqyy8937nq"
              :init="{
                height: 500,
                menubar: false,
                plugins: [
                  'advlist autolink lists link image charmap print preview anchor',
                  'searchreplace visualblocks code fullscreen',
                  'insertdatetime media table paste code help wordcount',
                ],
                toolbar:
                  'undo redo | formatselect | bold italic backcolor | \
                  alignleft aligncenter alignright alignjustify | \
                  bullist numlist outdent indent | removeformat | help',
              }"
            ></vue-editor>

            <v-divider></v-divider>
            <div v-if="n.customSections && n.customSections.length > 0">
              <h4>{{ $t("datasource.pms.project-narrative.other-context-sections") }}</h4>
              <div
                v-if="
                  typeOfNarrative &&
                  typeOfNarrative.length > 0 &&
                  typeOfNarrative[n.idx].CustomSections &&
                  typeOfNarrative[n.idx].CustomSections.length > 0
                "
              >
                <v-card
                  v-for="(section, sectionIndex) in typeOfNarrative[
                    n.idx
                  ].CustomSections.filter((section) => !section.REMOVED)"
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
                    <v-btn
                      v-if="isUpdatableForm"
                      color="primary"
                      @click="removeSection(n.idx, section.tmpidx)"
                      >{{ $t("datasource.pms.project-narrative.remove-section") }}</v-btn
                    >
                  </v-card-title>
                  <v-card-text>
                    <vue-editor
                      v-model="section.SECTION_TEXT"
                      api-key="u1zaygen61a491aghepiyyboodl01fl3flltxllqyy8937nq"
                      @onchange="editorChange()"
                      :init="{
                        height: 500,
                        menubar: false,
                        plugins: [
                          'advlist autolink lists link image charmap print preview anchor',
                          'searchreplace visualblocks code fullscreen',
                          'insertdatetime media table paste code help wordcount',
                        ],
                        toolbar:
                          'undo redo | formatselect | bold italic backcolor | \
                  alignleft aligncenter alignright alignjustify | \
                  bullist numlist outdent indent | removeformat | help',
                      }"
                    ></vue-editor>
                  </v-card-text>
                </v-card>
              </div>
              <v-btn
                v-if="isUpdatableForm"
                color="primary"
                @click="addNewSection(n.idx)"
                >{{ $t("datasource.pms.project-narrative.add-section") }}</v-btn
              >
            </div>
          </v-tab-item>
        </v-tabs-items>
      </div>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
//import { VueEditor } from "vue2-editor";
import VueEditor from "@tinymce/tinymce-vue";
import { EventBus } from "../../../event-bus";
import { translate } from "../../../i18n";
//import JrsSimpleTable from "../../components/JrsSimpleTable.vue";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../../../axiosapi/api";

interface NarrativeSection {
  ID: number | null;
  CODE: string;
  REFERENCE_ID: number;
  SECTION_TITLE?: string;
  SECTION_TEXT: string;
  REMOVED?: boolean;
  CustomSections: [];
  tmpidx: number;
}

interface TabDefSection {
  idx: number;
  key: string;
  name: string;
  translationKey?: string;
  customSections: [];
}

export default Vue.extend({
  props: {
    selectedObjectId: {
      type: Number,
      required: true,
    },
    narrativeTipology: {
      type: String,
      required: false,
      default: "",
    },
    tableName: {
      type: String,
      required: true,
    },
    isUpdatableForm: {
      type: Boolean,
      required: false,
    },
    versioned: {
      type: Number,
      default: 0,
      required: true,
    },
  },
  components: {
    "vue-editor": VueEditor,
    //,
    // "jrs-simple-table": JrsSimpleTable
  },
  data() {
    return {
      //projectLIstColumns: ["ID", "CODE", "DESCR", "IMS_ADMIN_AREA_NAME"],
      activeTab: null,
      narrativeDefinitionTabs: [],
      typeOfNarrative: [],
      retList: [],
      narrativeChanged: false,
    };
  },
  beforeDestroy() {
    let localThis: any = this as any;
    EventBus.$off("saveNarrativeFromMainSave");
    // EventBus.$off("saveNarrative");
    //if (localThis.saveNarrativeAction == true) {
    if (localThis.isUpdatableForm == false) return;
    if (localThis.narrativeTipology == "") return;
    if (localThis.narrativeChanged == true) {
      localThis
        .$confirm("Save the modified " + localThis.narrativeTipology + " narrative?")
        .then((res: any) => {
          if (!res) {
            return;
          } else {
            localThis.saveNarrativeData();
          }
        });
      localThis.narrativeChanged = false;
      //   }
    }
  },
  mounted() {
    let localThis: any = this as any;

    localThis.getNarrativeSectionType().then((res: any) => {
      if (localThis.selectedObjectId > 0) {
        localThis.clearNarrativeData(localThis.selectedObjectId);
        localThis.setNarrativeData(localThis.selectedObjectId);
      } else {
        localThis.typeOfNarrative = [];
        let i: number;
        for (i = 0; i < localThis.narrativeDefinitionTabs.length; i++) {
          let obj = {
            ID: null,
            CODE: localThis.narrativeDefinitionTabs[i].key,
            SECTION_TITLE: "",
            SECTION_TEXT: "",
            REFERENCE_ID: 0,
            REMOVED: false,
            CustomSections: [],
            tmpidx: 0,
          } as NarrativeSection;
          localThis.typeOfNarrative.push(obj);
        }
      }
    });

    EventBus.$on("saveNarrativeFromMainSave", (data: any) => {
      if (localThis.narrativeChanged == true) {
        localThis.narrativeChanged = false;
        localThis.saveNarrativeData();
      }
    });

    // EventBus.$on("saveNarrative", (data: any) => {
    //   if (localThis.narrativeChanged == true) {
    //     localThis.$confirm("Save the modified narrative?").then((res: any) => {
    //       if (!res) {
    //         return;
    //       } else {
    //         localThis.saveNarrativeData();
    //       }
    //     });
    //   }
    //});
    //   contextNarrative: {
    //     ID: null,
    //     CODE: "CONTEXT_AP",
    //     SECTION_TITLE: "",
    //     SECTION_TEXT: "",
    //     REFERENCE_ID: 0
    //   } as NarrativeSection,
    //   contextCustomSections: [] as Array<NarrativeSection>,
  },

  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),

    editorChange() {
      let localThis: any = this as any;
      localThis.narrativeChanged = true;
    },

    /**
     * Clear narrative sections.
     * @param projectId - ID of the project
     */
    clearNarrativeData(projectId: number) {
      let localThis: any = this as any;
      let i: number;
      localThis.typeOfNarrative = [];
      for (i = 0; i < localThis.narrativeDefinitionTabs.length; i++) {
        let obj = {
          ID: null,
          CODE: localThis.narrativeDefinitionTabs[i].key,
          SECTION_TITLE: "",
          SECTION_TEXT: "",
          REFERENCE_ID: projectId,
          REMOVED: false,
          CustomSections: [],
          tmpidx: 0,
        } as NarrativeSection;
        localThis.typeOfNarrative.push(obj);
      }
    },

    getNarrativeSectionType() {
      let localThis: any = this as any;
      let view: string = "IMS_NARRATIVE_SECTION_TYPE";
      let wherecond: string = `TABLE_NAME='${localThis.tableName}'`;
      if (localThis.versioned > 0) {
        view = "IMS_NARRATIVE_SECTION_TYPE_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }
      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          let i: number = 0;
          if (res.table_data) {
            if (res.table_data.length > 0) {
              localThis.narrativeDefinitionTabs = [];
            }
            res.table_data.forEach((item: any) => {
              if (item.CODE.indexOf("_NODE") == -1) {
                let obj = {
                  idx: i++,
                  key: item.CODE,
                  name: item.DESCR,
                  translationKey: item.TRANSLATION_KEY,
                  minwords: item.MINWORDS,
                  maxwords: item.MAXWORDS,
                  customSections: [],
                } as TabDefSection;
                localThis.narrativeDefinitionTabs.push(obj);
              } else {
                //CARICO GLI HEADER DELLE SUBSECTIONS
                let sction: string;
                sction = item.CODE.substring(0, item.CODE.indexOf("_NODE"));
                let k: number = 0;
                for (k = 0; k < localThis.narrativeDefinitionTabs.length; k++) {
                  if (localThis.narrativeDefinitionTabs[k].key == sction) {
                    let obj = {
                      idx: 0,
                      key: item.CODE,
                      name: item.DESCR,
                      translationKey: item.TRANSLATION_KEY,
                      minwords: item.MINWORDS,
                      maxwords: item.MAXWORDS,
                      customSections: [],
                    } as TabDefSection;
                    localThis.narrativeDefinitionTabs[k].customSections.push(obj);
                    break;
                  }
                }
              }
            });
          }
        }
      );
    },

    /**
     * Load Project narrative from server and set in tabs.
     * @param projectId - ID of the project
     */
    setNarrativeData(projectId: number) {
      let localThis: any = this as any;
      let i: number = 0;
      let j: number = 0;

      let view: string = "IMS_VI_NARRATIVE_BY_TYPE";
      let wherecond: string = `REFERENCE_ID = ${localThis.selectedObjectId} AND TABLE_NAME='${localThis.tableName}'`;
      if (localThis.versioned > 0) {
        view = "IMS_VI_NARRATIVE_BY_TYPE_VER";
        wherecond += ` AND VERSION_ID=${localThis.versioned}`;
      }

      let selectPayload: GenericSqlSelectPayload = {
        viewName: view,
        colList: null,
        whereCond: wherecond,
        orderStmt: null,
      };

      return this.getGenericSelect({ genericSqlPayload: selectPayload }).then(
        (res: any) => {
          //Setup data
          if (res.table_data) {
            if (res.table_data.length > 0) {
              localThis.typeOfNarrative = [];
            }
            let x: number = 0;
            res.table_data.forEach((item: any) => {
              if (item.CODE.indexOf("_NODE") == -1) {
                item.CustomSections = [];
                localThis.typeOfNarrative.push(item);
              } else {
                let sction: string;
                sction = item.CODE.substring(0, item.CODE.indexOf("_NODE"));
                let k: number = 0;
                for (k = 0; k < localThis.typeOfNarrative.length; k++) {
                  if (localThis.typeOfNarrative[k].CODE == sction) {
                    item.REMOVED = false;
                    item.tmpidx = x++;
                    localThis.typeOfNarrative[k].CustomSections.push(item);

                    break;
                  }
                }
              }
            });
            for (i = 0; i < localThis.narrativeDefinitionTabs.length; i++) {
              let found: boolean = false;
              for (j = 0; j < localThis.typeOfNarrative.length; j++) {
                //
                if (
                  localThis.narrativeDefinitionTabs[i].key ==
                  localThis.typeOfNarrative[j].CODE
                ) {
                  found = true;
                  break;
                }
              }
              if (!found) {
                let obj = {
                  ID: null,
                  CODE: localThis.narrativeDefinitionTabs[i].key,
                  SECTION_TITLE: "",
                  SECTION_TEXT: "",
                  DESCR: localThis.narrativeDefinitionTabs[i].name,
                  MINWORDS: localThis.narrativeDefinitionTabs[i].minwords,
                  MAXWORDS: localThis.narrativeDefinitionTabs[i].maxwords,
                  REFERENCE_ID: projectId,
                  REMOVED: false,
                  CustomSections: [],
                  tmpidx: 0,
                } as NarrativeSection;
                localThis.typeOfNarrative.push(obj);
              }
            }
          }
        }
      );
    },
    /**
     * Save the project narrative sections.
     */
    saveNarrativeData() {
      let localThis: any = this as any;

      //Check if all context sections have a title and a body
      let i: number;
      let j: number;
      for (i = 0; i < localThis.typeOfNarrative.length; i++) {
        let invalidSectionIndex = localThis.typeOfNarrative[i].CustomSections.findIndex(
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
            text: "Warning: All context sections defined must have a title and a body.",
          });
          return;
        }
      }

      let msg: string = "";
      var foundStop: boolean = false;
      for (i = 0; i < localThis.typeOfNarrative.length; i++) {
        let cont = localThis.typeOfNarrative[i].SECTION_TEXT;
        cont = cont.replace(/<[^>]*>/g, " ");
        cont = cont.replace(/\s+/g, " ");
        cont = cont.trim();
        var n = cont.split(" ").length;
        if (cont == "") n = 0;

        if (
          (localThis.typeOfNarrative[i].MINWORDS != undefined &&
            localThis.typeOfNarrative[i].MINWORDS > 0 &&
            n < localThis.typeOfNarrative[i].MINWORDS) ||
          (localThis.typeOfNarrative[i].MAXWORDS != undefined &&
            localThis.typeOfNarrative[i].MAXWORDS > 0 &&
            n > localThis.typeOfNarrative[i].MAXWORDS)
        ) {
          if (
            localThis.typeOfNarrative[i].MINWORDS != undefined &&
            localThis.typeOfNarrative[i].MINWORDS > 0 &&
            n < localThis.typeOfNarrative[i].MINWORDS
          ) {
            msg +=
              "Number of words in the section: " +
              localThis.typeOfNarrative[i].DESCR +
              " less then " +
              localThis.typeOfNarrative[i].MINWORDS +
              ". <br/>";
          } else {
            msg =
              "Number of words in the section: " +
              localThis.typeOfNarrative[i].DESCR +
              " exceeds " +
              localThis.typeOfNarrative[i].MAXWORDS +
              ". <br/>";
          }
        }
      }
      if (msg != "") {
        localThis.$confirm(msg + "Continue with saving?").then((res: any) => {
          if (!res) {
            return;
          } else {
            localThis.saveNarrativeData2();
          }
        });
      } else {
        localThis.saveNarrativeData2();
      }
    },

    saveNarrativeData2() {
      let localThis: any = this as any;
      localThis.narrativeData = [];
      //Check if all context sections have a title and a body
      let i: number;
      let j: number;
      for (i = 0; i < localThis.typeOfNarrative.length; i++) {
        localThis.narrativeData.push(localThis.typeOfNarrative[i]);
        for (j = 0; j < localThis.typeOfNarrative[i].CustomSections.length; j++) {
          localThis.narrativeData.push(localThis.typeOfNarrative[i].CustomSections[j]);
        }
      }

      let savePayload: GenericSqlPayload = {
        spName: "IMS_SP_INS_UPD_NARRATIVE_SECTION",
        actionType: SqlActionType.NUMBER_0,
        jsonPayload: JSON.stringify(localThis.narrativeData),
      };

      localThis
        .execSPCall(savePayload)
        .then((res: any) => {
          localThis.showNewSnackbar({ typeName: "success", text: res.message }); // Feedback to user
          if (localThis.selectedObjectId > 0) {
            localThis.clearNarrativeData(localThis.selectedObjectId);
            localThis.setNarrativeData(localThis.selectedObjectId);
          }
          localThis.narrativeChanged = false;
        })
        .catch((err: any) => {
          localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
        });
    },
    /**
     * Setup new context node
     */
    addNewSection(idx: number) {
      let localThis = this as any;
      let idxmax: number = 0;
      if (localThis.typeOfNarrative[idx].CustomSections.length > 0)
        idxmax =
          localThis.typeOfNarrative[idx].CustomSections[
            localThis.typeOfNarrative[idx].CustomSections.length - 1
          ].tmpidx + 1;
      let obj = {
        ID: null,
        CODE: localThis.typeOfNarrative[idx].CODE + "_NODE",
        SECTION_TITLE: "",
        SECTION_TEXT: "",
        REFERENCE_ID: localThis.selectedObjectId,
        REMOVED: false,
        CustomSections: [],
        tmpidx: idxmax,
      } as NarrativeSection;
      localThis.typeOfNarrative[idx].CustomSections.push(obj);
    },
    /**
     * Remove the context node at the given index.
     * @param index - index of the elemet to remove
     */
    removeSection(idx: number, sectionId: number) {
      let localThis = this as any;
      localThis.typeOfNarrative[idx].CustomSections = localThis.typeOfNarrative[
        idx
      ].CustomSections.map((section: NarrativeSection) => {
        if (section.tmpidx == sectionId) {
          section.REMOVED = true;
        }
        return section;
      });
    },
  },

  watch: {
    selectedObjectId(to: number) {
      let localThis: any = this as any;
      localThis.clearNarrativeData(to);
      localThis.setNarrativeData(to);
    },

    // saveNarrativeAction(to: boolean) {
    //   let localThis: any = this as any;
    //   if (to == true) {
    //     if (localThis.narrativeChanged == true) {
    //       localThis.$confirm("Save the modified narrative?").then((res: any) => {
    //         if (!res) {
    //           return;
    //         } else {
    //           localThis.saveNarrativeData();
    //         }
    //       });
    //     }
    //   }
    // },
  },
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
