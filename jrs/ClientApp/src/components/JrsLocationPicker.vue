<template>
  <div class="d-flex">
    <v-dialog
      v-model="editMode"
      persistent
      retain-focus
      :overlay="false"
      max-width="60em"
      transition="dialog-transition"
    >
      <template v-slot:activator="{ on }">
        <v-text-field
          v-on="on"
          @click="editMode = !editMode"
          :label="label"
          v-model="locationName"
          :hint="hint ? hint : ''"
          :persistent-hint="hint ? true : false"
          readonly
          :required="required"
          :rules="required ? requiredTextFieldRule : []"
          prepend-icon="mdi-map"
        ></v-text-field>
      </template>
      <v-card>
        <v-card-title>{{ $t("datasource.ims.location.title") }}</v-card-title>
        <v-card-text>
          <!-- LOAD SPINNER -->
          <div class="d-flex flex-row justify-center" v-if="isLoading">
            <v-progress-circular indeterminate></v-progress-circular>
          </div>
          <v-form
            v-if="!isLoading"
            ref="locationForm"
            lazy-validation
            v-model="formIsValid"
          >
            <v-row>
              <v-col>
                <!-- ADMIN AREA 1 -->
                <v-autocomplete
                  :label="$t('datasource.ims.location.admin-1')"
                  id="ADMIN-1"
                  v-model="admin_1"
                  :hint="$t('datasource.ims.location.admin-1-hint')"
                  persistent-hint
                  required
                  :rules="
                    requiredTextFieldRule && requiredCountry ? requiredTextFieldRule : []
                  "
                  return-object
                  :items="admin_1_items"
                  item-value="IMS_ADMIN_AREA_ID"
                  item-text="IMS_ADMIN_AREA_NAME"
                  @change="setDatasets('COUNT')"
                  :loading="loadingDatasetName == 'admin_1_items'"
                ></v-autocomplete>
              </v-col>
              <v-col>
                <!-- ADMIN AREA 2 -->
                <v-autocomplete
                  :label="$t('datasource.ims.location.admin-2')"
                  id="ADMIN-2"
                  v-model="admin_2"
                  :hint="$t('datasource.ims.location.admin-2-hint')"
                  persistent-hint
                  required
                  :rules="
                    requiredTextFieldRule && requiredState ? requiredTextFieldRule : []
                  "
                  return-object
                  :items="admin_2_items"
                  item-value="IMS_ADMIN_AREA_ID"
                  item-text="IMS_ADMIN_AREA_NAME"
                  @change="setDatasets('PROVI')"
                  :disabled="!admin_1.IMS_ADMIN_AREA_ID"
                  :loading="loadingDatasetName == 'admin_2_items'"
                ></v-autocomplete>
              </v-col>
              <v-col>
                <!-- ADMIN AREA 3 -->
                <v-autocomplete
                  :label="$t('datasource.ims.location.admin-3')"
                  id="ADMIN-3"
                  v-model="admin_3"
                  :hint="$t('datasource.ims.location.admin-3-hint')"
                  persistent-hint
                  required
                  :rules="
                    requiredTextFieldRule && requiredCity ? requiredTextFieldRule : []
                  "
                  return-object
                  :items="admin_3_items"
                  item-value="IMS_ADMIN_AREA_ID"
                  item-text="IMS_ADMIN_AREA_NAME"
                  @change="loadAddress()"
                  :disabled="!admin_2.IMS_ADMIN_AREA_ID"
                  :loading="loadingDatasetName == 'admin_3_items'"
                ></v-autocomplete>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <!-- ADDRESS -->
                <v-combobox
                  :label="$t('datasource.ims.location.address')"
                  name="address"
                  id="ADDRESS"
                  v-model="address"
                  :items="locationLists"
                  item-text="IMS_LOCATION_ADDRESS"
                  item-value="IMS_LOCATION_ID"
                  :hint="$t('datasource.ims.location.address-hint')"
                  persistent-hint
                  required
                  :rules="
                    requiredTextFieldRule && requiredAddress ? requiredTextFieldRule : []
                  "
                  :disabled="!admin_3.IMS_ADMIN_AREA_ID"
                  :counter="250"
                ></v-combobox>
              </v-col>
              <v-col>
                <!-- POSTAL CODE -->
                <v-text-field
                  :label="$t('datasource.ims.location.postal-code')"
                  name="postal_code"
                  id="POSTAL_CODE"
                  v-model="postalCode"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-btn color="secondary darken-1" text @click="closeEdit()"
            >X {{ $t("general.close") }}</v-btn
          >
          <v-btn color="primary" @click="saveLocation()">{{ $t("general.save") }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-icon @click="clearField()">mdi-close</v-icon>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapActions } from "vuex";
import {
  GenericSqlSelectPayload,
  GenericSqlPayload,
  SqlActionType,
} from "../axiosapi/api";
import { AdminArea, Location } from "../models/Location";
import { translate } from "../i18n";

export default Vue.extend({
  props: {
    value: {
      type: Number,
      required: false,
    },
    requiredCountry: {
      type: Boolean,
      required: false,
      default: true,
    },

    requiredState: {
      type: Boolean,
      required: false,
      default: true,
    },
    requiredCity: {
      type: Boolean,
      required: false,
      default: true,
    },
    requiredAddress: {
      type: Boolean,
      required: false,
      default: true,
    },

    label: {
      type: String,
      required: false,
      default: null,
    },
    hint: {
      type: String,
      required: false,
      default: null,
    },
    required: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      editMode: false,
      admin_1: {} as AdminArea,
      admin_2: {} as AdminArea,
      admin_3: {} as AdminArea,
      admin_1_items: [],
      admin_2_items: [],
      admin_3_items: [],
      address: "",
      locationLists: [],
      isLoading: false,
      loadingDatasetName: "",
      postalCode: "",
      formIsValid: false,
    };
  },
  computed: {
    locationName: {
      get() {
        let localThis: any = this as any;
        let add: string =
          localThis.address != undefined && localThis.address.IMS_LOCATION_ADDRESS
            ? localThis.address.IMS_LOCATION_ADDRESS
            : "";
        let adm1: string =
          localThis.admin_1 != undefined && localThis.admin_1.IMS_ADMIN_AREA_NAME
            ? ", " + localThis.admin_1.IMS_ADMIN_AREA_NAME
            : "";
        let adm2: string =
          localThis.admin_2 != undefined && localThis.admin_2.IMS_ADMIN_AREA_NAME
            ? ", " + localThis.admin_2.IMS_ADMIN_AREA_NAME
            : "";
        let pCode: string =
          localThis.postalCode != undefined && localThis.postalCode != ""
            ? " " + localThis.postalCode
            : "";
        let adm3: string =
          localThis.admin_3 != undefined && localThis.admin_3.IMS_ADMIN_AREA_NAME
            ? localThis.admin_3.IMS_ADMIN_AREA_NAME
            : "";

        let composed = `${add} ${adm3}${adm2}${pCode}${adm1}`;
        composed = (composed.replace(/ /g, "").length > 0 ? composed : "").trimStart();
        if (composed[0] == ",") {
          composed = composed.substring(1);
        }
        return composed;
      },
      set(newVal: string) {
        //Setter operation is allowed onlto clear the field
        if (newVal == null) {
          let localThis: any = this as any;
          localThis.updateValue(null);
          localThis.admin_1 = {} as AdminArea;
          localThis.admin_2 = {} as AdminArea;
          localThis.admin_3 = {} as AdminArea;
          localThis.address = "";
          localThis.postalCode = "";
        }
      },
    },
    locationValue() {
      let localThis: any = this as any;
      let retVal: Location = {
        IMS_LOCATION_ID: localThis.value,
        IMS_LOCATION_ADMIN_AREA_ID_1: localThis.admin_1.IMS_ADMIN_AREA_ID,
        IMS_LOCATION_ADMIN_AREA_ID_2: localThis.admin_2.IMS_ADMIN_AREA_ID,
        IMS_LOCATION_ADMIN_AREA_ID_3: localThis.admin_3.IMS_ADMIN_AREA_ID,
        IMS_LOCATION_ADDRESS: localThis.address.IMS_LOCATION_ADDRESS
          ? localThis.address.IMS_LOCATION_ADDRESS
          : localThis.address,
        IMS_LOCATION_POSTAL_CODE: localThis.postalCode,
      };
      return retVal;
    },
    requiredTextFieldRule() {
      return [(v: any) => !!v || translate("error.required-field")];
    },
  },
  watch: {
    editMode(to: boolean, from: boolean) {
      let localThis: any = this as any;

      // Initial load if inserting new data
      if (to == true && !localThis.locationValue.IMS_LOCATION_ID) {
        this.setDatasets();
      }

      // Load location components
      if (localThis.locationValue.IMS_LOCATION_ID) {
        localThis.isLoading = true;

        localThis
          .loadLocationComponents(localThis.locationValue.IMS_LOCATION_ID)
          .catch((err: any) => {
            localThis.showNewSnackbar({
              typeName: "error",
              text: "Error: recovering Location information.",
            }); // Feedback to user
          })
          .finally(() => {
            localThis.isLoading = false;
          });
      }
    },
    // address(to: any, from: any) {
    //   // Emit location change
    //   (this as any).updateValue(to.IMS_LOCATION_ID);
    //   // if (to.IMS_LOCATION_ID) {
    //   //   (this as any).updateValue(to.IMS_LOCATION_ID);
    //   // }
    // }
  },
  methods: {
    ...mapActions({
      showNewSnackbar: "showNewSnackbar",
    }),
    ...mapActions("apiHandler", {
      getGenericSelect: "getGenericSelect",
      execSPCall: "execSPCall",
    }),
    /**
     * Emit value changes to parent component.
     */
    updateValue(newVal: number) {
      (this as any).$emit("input", newVal);
    },
    /**
     * Refresh Admin Area autocomplete.
     * @param changedCode - code of the autocomplete which has changed
     */
    setDatasets(changedCode?: any) {
      let localThis: any = this as any;
      // let datasetName: string;
      let datasetName: string = "admin_1_items";
      let condition: string | null = null;
      let defaultItemCode: string; //item code for defautl null value on loaded selectItems

      switch (changedCode) {
        case undefined:
          datasetName = "admin_1_items";
          condition = `IMS_ADMIN_AREA_TYPE_CODE = 'COUNT'`;

          localThis.admin_1_items.length = 0;
          // localThis.admin_1 = Object.assign({});

          localThis.admin_2_items.length = 0;
          localThis.admin_2 = Object.assign({});

          localThis.admin_3_items.length = 0;
          localThis.admin_3 = Object.assign({});

          defaultItemCode = "PROVI";
          break;
        case "COUNT":
          datasetName = "admin_2_items";
          condition = `
                        IMS_ADMIN_AREA_TYPE_CODE = 'PROVI'
                            AND IMS_ADMIN_AREA_NODE.IsDescendantOf(cast('${localThis.admin_1.IMS_ADMIN_AREA_NODE}' as hierarchyid)) = 1`;

          localThis.admin_2_items.length = 0;
          // localThis.admin_2 = Object.assign({});

          localThis.admin_3_items.length = 0;
          localThis.admin_3 = Object.assign({});

          defaultItemCode = "MUNIC";
          break;
        case "PROVI":
          datasetName = "admin_3_items";
          condition = `
                        IMS_ADMIN_AREA_TYPE_CODE = 'MUNIC'
                            AND IMS_ADMIN_AREA_NODE.IsDescendantOf(cast('${localThis.admin_2.IMS_ADMIN_AREA_NODE}' as hierarchyid)) = 1`;

          localThis.admin_3_items.length = 0;
          // localThis.admin_3 = Object.assign({});
          break;
      }

      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_VI_ADMIN_AREA_BY_TYPE",
        colList:
          "IMS_ADMIN_AREA_ID, IMS_ADMIN_AREA_NAME,IMS_ADMIN_AREA_NODE,IMS_ADMIN_AREA_TYPE_CODE",
        whereCond: condition,
        orderStmt: "IMS_ADMIN_AREA_NAME",
      };

      // Set loading dataset
      localThis.loadingDatasetName = datasetName;

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis[datasetName] = [...(res.table_data ? res.table_data : [])];
          localThis[datasetName].unshift({
            IMS_ADMIN_AREA_ID: null,
            IMS_ADMIN_AREA_NAME: "N/A",
            IMS_ADMIN_AREA_TYPE_CODE: "IMS_ADMIN_AREA_NODE",
          });
          localThis.loadingDatasetName = "";
        });
    },
    /**
     * Loads address sudgestions based on the selected admin areas.
     */
    loadAddress() {
      let localThis = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_LOCATION",
        colList: "IMS_LOCATION_ID,IMS_LOCATION_ADDRESS",
        whereCond: `
                        IMS_LOCATION_ADMIN_AREA_ID_1 = ${localThis.locationValue.IMS_LOCATION_ADMIN_AREA_ID_1}
                        AND IMS_LOCATION_ADMIN_AREA_ID_2 = ${localThis.locationValue.IMS_LOCATION_ADMIN_AREA_ID_2}
                        AND IMS_LOCATION_ADMIN_AREA_ID_3 = ${localThis.locationValue.IMS_LOCATION_ADMIN_AREA_ID_3}`,
        orderStmt: "IMS_LOCATION_ADDRESS",
      };

      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          localThis.locationLists = [...(res.table_data ? res.table_data : [])];
          localThis.locationLists.unshift({
            IMS_LOCATION_ID: null,
            IMS_LOCATION_ADDRESS: "N/A",
          });
        });
    },
    /**
     * Save new location.
     */
    saveLocation() {
      let localThis: any = this as any;
      if (localThis.validate()) {
        //Timeout is required to workoround for combobox value update (https://github.com/vuetifyjs/vuetify/issues/5203#issuecomment-439321802)
        setTimeout(() => {
          let savePayload: GenericSqlPayload = {
            spName: "IMS_SP_INS_LOCATION",
            actionType: SqlActionType.NUMBER_1,
            jsonPayload: JSON.stringify(localThis.locationValue),
          };
          localThis
            .execSPCall(savePayload)
            .then((res: any) => {
              if (!res.pk_id) {
                throw new Error("No PK was returned.");
              }
              localThis.updateValue(res.pk_id);
              localThis.showNewSnackbar({ typeName: "info", text: res.message }); //Feedback to user
              localThis.editMode = false;
            })
            .catch((err: any) => {
              localThis.showNewSnackbar({ typeName: "error", text: err.message }); // Feedback to user
            });
        });
      }
    },
    /**
     * Close the edit dialogue.
     * @param clearData - clears the data
     */
    closeEdit(clearData: boolean = false) {
      let localThis: any = this as any;

      // fix Bug 245 (OpenProject) - clear filed when are not correct populate
      if (!localThis.value) {
        localThis.clearField();
      }
      localThis.editMode = false;
    },
    /**
     * Load location components.
     * @param id - id of location
     */
    loadLocationComponents(id: number) {
      let localThis: any = this as any;
      let selectPayload: GenericSqlSelectPayload = {
        viewName: "IMS_VI_LOCATION",
        colList: null,
        whereCond: `IMS_LOCATION_ID = ${localThis.locationValue.IMS_LOCATION_ID}`,
        orderStmt: null,
      };

      let tmp: any;
      return localThis
        .getGenericSelect({ genericSqlPayload: selectPayload })
        .then((res: any) => {
          // Load AdminArea 1 dataset
          tmp = res.table_data[0];

          return this.setDatasets().then((res: any) => {
            localThis.admin_1 = {
              IMS_ADMIN_AREA_ID: tmp.IMS_LOCATION_ADMIN_AREA_ID_1,
              IMS_ADMIN_AREA_NAME: tmp.IMS_ADMIN_AREA_NAME_1,
              IMS_ADMIN_AREA_NODE: tmp.IMS_ADMIN_AREA_NODE_1,
            } as AdminArea;
          });
        })
        .then((res: any) => {
          // Load AdminArea 2 dataset
          return this.setDatasets("COUNT").then((res: any) => {
            localThis.admin_2 = {
              IMS_ADMIN_AREA_ID: tmp.IMS_LOCATION_ADMIN_AREA_ID_2,
              IMS_ADMIN_AREA_NAME: tmp.IMS_ADMIN_AREA_NAME_2,
              IMS_ADMIN_AREA_NODE: tmp.IMS_ADMIN_AREA_NODE_2,
            } as AdminArea;
          });
        })
        .then((res: any) => {
          // Load AdminArea 3 dataset
          return this.setDatasets("PROVI").then((res: any) => {
            localThis.admin_3 = {
              IMS_ADMIN_AREA_ID: tmp.IMS_LOCATION_ADMIN_AREA_ID_3,
              IMS_ADMIN_AREA_NAME: tmp.IMS_ADMIN_AREA_NAME_3,
              IMS_ADMIN_AREA_NODE: tmp.IMS_ADMIN_AREA_NODE_3,
            } as AdminArea;
          });
        })
        .then((res: any) => {
          // Load Address dataset
          return this.loadAddress().then((res: any) => {
            localThis.address = {
              IMS_LOCATION_ID: tmp.IMS_LOCATION_ID,
              IMS_LOCATION_ADDRESS:
                tmp.IMS_LOCATION_ADDRESS != undefined ? tmp.IMS_LOCATION_ADDRESS : "",
            };
          });
        })
        .then((res: any) => {
          // Set Postal Code
          localThis.postalCode = tmp.IMS_LOCATION_POSTAL_CODE;
        });
    },
    /**
     * Clear field data.
     */
    clearField() {
      let localThis: any = this as any;
      localThis.updateValue(null);
      localThis.locationName = null;
    },
    validate() {
      return (this as any).$refs.locationForm.validate();
    },
  },
  mounted() {
    let localThis: any = this as any;

    // Load location components
    if (localThis.locationValue.IMS_LOCATION_ID) {
      localThis
        .loadLocationComponents(localThis.locationValue.IMS_LOCATION_ID)
        .catch((err: any) => {
          localThis.showNewSnackbar({
            typeName: "error",
            text: "Error: recovering Location information.",
          }); // Feedback to user
        });
    }
  },
});
</script>

<style scoped></style>
