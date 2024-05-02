<template>
  <div>
    <jrs-readonly-detail
      :detailFields="detailFields"
      :detailData="detailData"
      :nbrOfColumns="nbrOfColumns"
    ></jrs-readonly-detail>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { mapGetters } from "vuex";
import TableMixin from "../mixins/TableMixin";
import JrsReadonlyDetail from "../components/JrsReadonyDetail.vue";

export default Vue.extend({
  props: {
    /**
     * Name of the view for which to recover table and form definition.
     */
    viewName: {
      type: String,
      required: true,
    },
    whereCondition: {
      type: String,
      required: true,
    },
    /**
     * Number of columns fo the form.
     * Excepted values: {1, 2, 3}
     */
    nbrOfColumns: {
      type: Number,
      required: false,
      default: 2,
      validator: function (value: any) {
        // The value must match one of these values
        return [1, 2, 3, 4].indexOf(value) !== -1;
      },
    },
  },
  components: {
    "jrs-readonly-detail": JrsReadonlyDetail,
  },
  mixins: [TableMixin],
  data() {
    return {
      detailFields: [],
      detailData: {},
    };
  },
  computed: {
    ...mapGetters({
      getUserUid: "getUserUid",
      getCurrentOffice: "getCurrentOffice",
    }),
  },
  methods: {
    /**
     * Load and set "detailFields" and "detailData".
     */
    setupTableData() {
      const localThis: any = this as any;
      localThis
        .getTableData(localThis.viewName, localThis.getCurrentOffice.HR_OFFICE_ID, [], {
          whereCond: this.whereCondition,
        })
        .then((res: any) => {
          localThis.detailFields = res.formFields;
          localThis.detailData = res.rows[0];
        });
    },
  },
  mounted() {
    const localThis: any = this as any;
    localThis.setupTableData();
  },
});
</script>

<style scoped></style>
