<template>
  <div>
    <div v-if="cellObject !=undefined && 
            cellObject.ALLOCATED_PERCENTAGE != undefined">
    <table
      :class="cellObject.ALLOCATED_PERCENTAGE<100?'GapCell':
      cellObject.ALLOCATED_PERCENTAGE==100?'NoGapCell':'OverGapCell'"
    >
      <tr>
        <td>
          <!--Allocated Percentage-->
          <div style="text-align: center"
          ><h3>{{cellObject.ALLOCATED_PERCENTAGE}} %</h3></div>
          <div style="text-align: center">
             {{cellObject.ALLOCATED_VALUE}}/{{cellObject.REQUIRED_VALUE}}
          </div>
        </td>
      </tr>
    </table>
    </div>
    <div v-else>
       <!-- <table
      :class='NACell'
    >
      <tr>
        <td>

          <div style="text-align: center"
          >N/A</div>
        </td>
      </tr>
    </table> -->
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
interface OBJECT_MONTH {
  SELECTED: boolean;
  REQUIRED_VALUE: any;
  ALLOCATED_VALUE: any;
  ALLOCATED_PERCENTAGE: any;
  CURRENCY: string;
}
export default Vue.extend({
  props: {
    cellObject: {
      type: Object as () => OBJECT_MONTH,
      required: false,
    },
    // showActivityDetails: {
    //   type: Boolean,
    //   required: true,
    // },
  },
  computed: {
    options() {
      let localThis: any = this as any;
      return {
        locale: localThis.locale,
      };
    },
  },
});
</script>

<style scoped>
.GapCell {
  background-color: lightcoral;
  color: white;
  font-size: 12px;
  border-style: none;
  height: 10px;
  width: 100%;
  padding: 0px;
  margin: 0px;
}
.NoGapCell {
  background-color: lightgreen;
  color: white;
  font-size: 12px;
  border-style: none;
  height: 10px;
  width: 100%;
  padding: 0px;
  margin: 0px;
}

.OverGapCell {
  background-color: lightyellow;
  color: white;
  font-size: 12px;
  border-style: none;
  height: 10px;
  width: 100%;
  padding: 0px;
  margin: 0px;
}

.NACell {
  background-color: greenyellow;
  font-size: 1px;
  border-color: gainsboro;
  height: 10px;
  width: 100%;
}

</style>