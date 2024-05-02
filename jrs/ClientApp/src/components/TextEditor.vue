<template>
  <div>
    <div
      class="d-inline-flex justify-space-between pb-1"
      style="width: 100%"
      v-if="!readonly"
    >
      <div id="LABEL_HOLDER" class="d-flex flex-column">
        <span class="v-label ml-1 subtitle-1">{{ label }}</span>
        <span class="v-label mb-1 ml-1 caption">{{ hint }}</span>
      </div>
    </div>
    <vue-editor
      ref="editor"
      v-model="compValue"
      :api-key="apiKey"
      :init="initOptions"
      :disabled="readonly"
      v-if="showEditor && !readonly"
    ></vue-editor>

    <v-card outlined v-if="readonly">
      <v-card-title>{{ label }}</v-card-title>
      <v-card-subtitle>{{ hint }}</v-card-subtitle>
      <v-card-text v-html="compValue"></v-card-text>
    </v-card>
  </div>
</template>

<script lang="ts">
import VueEditor from "@tinymce/tinymce-vue";
import Vue from "vue";
export default Vue.extend({
  components: {
    "vue-editor": VueEditor,
  },
  props: {
    value: {
      type: String,
      default: "",
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
    // is_required: {
    //     type: Boolean,
    //     required: false,
    //     default: false,
    // },
    readonly: {
      type: Boolean,
      required: false,
      default: false,
    },
    fieldKey: {
      type: [String, Number],
    },
  },
  //account tiny : vat100.imsblogic@jrsexternal.info
  //JRs.123456
  data: () => ({
    apiKey: "u1zaygen61a491aghepiyyboodl01fl3flltxllqyy8937nq",
    initOptions: {
      heigth: "100em",
      plugins: [
        "advlist autolink lists link image charmap print preview anchor",
        "image anchor toc table code",
      ], // TODO: Chekcout more info @ https://www.tiny.cloud/docs/plugins/
      toolbar: `undo redo | formatselect | fontselect | bold italic strikethrough underline | bullist numlist table | subscript superscript | backcolor forecolor | 
                                              alignleft aligncenter alignright alignnone alignjustify indent outdent | cut copy paste | help
                                              image | anchor toc`,
      toolbar_mode: "sliding",
      menu: "", // TODO: Chekcout more info @ https://www.tiny.cloud/docs/advanced/available-menu-items/
      menubar: "edit view insert table format tools",
      toc_header: "div",
      /* enable title field in the Image dialog*/
      image_title: true,
      /* enable automatic uploads of images represented by blob or data URIs*/
      automatic_uploads: true,
      file_picker_types: "image",
      /* and here's our custom image picker*/

      file_picker_callback: function (cb: any, value: any, meta: any) {
        const editor: any = this as any;

        var input = document.createElement("input");
        input.setAttribute("type", "file");
        input.setAttribute("accept", "image/*");

        /*
                                    Note: In modern browsers input[type="file"] is functional without
                                    even adding it to the DOM, but that might not be the case in some older
                                    or quirky browsers like IE, so you might want to add it to the DOM
                                    just in case, and visually hide it. And do not forget do remove it
                                    once you do not need it anymore.
                                */

        input.onchange = function () {
          // eslint-disable-next-line
          const th: any = this as any;
          const tmpEditor = editor;
          var file = th.files[0];

          var reader = new FileReader();
          reader.onload = function () {
            /*
                                            Note: Now we need to register the blob in TinyMCEs image blob
                                            registry. In the next release this part hopefully won't be
                                            necessary, as we are looking to handle it internally.
                                        */
            var id = "blobid" + new Date().getTime();
            // var blobCache = (tinymce as any).activeEditor.editorUpload.blobCache;
            var blobCache = editor.editorUpload.blobCache;

            // eslint-disable-next-line
            var base64: any | null = reader.result
              ? reader.result.toString().split(",")[1]
              : "";
            var blobInfo = blobCache.create(id, file, base64);
            blobCache.add(blobInfo);

            /* call the callback and populate the Title field with the file name */
            cb(blobInfo.blobUri(), { title: file.name });
          };
          reader.readAsDataURL(file);
        };
        input.click();
      },
    },
    showEditor: false, //variable to resolve problem of Tiny
  }),

  computed: {
    compValue: {
      get() {
        const localThis: any = this as any;
        return localThis.value;
      },
      set(newVal: string) {
        const localThis: any = this as any;
        localThis.updateValue(newVal);
      },
    },
  },
  methods: {
    /**
     * Emit value changes to parent component.
     */
    updateValue(newVal: any) {
      (this as any).$emit("input", newVal);
    },
    /**
     * Clear field data.
     */
    clearField() {
      const localThis: any = this as any;
      localThis.compValue = null;
    },
  },
  mounted() {
    const localThis: any = this as any;
    localThis.$nextTick(() => {
      localThis.showEditor = true;
    });
  },
});
</script>
