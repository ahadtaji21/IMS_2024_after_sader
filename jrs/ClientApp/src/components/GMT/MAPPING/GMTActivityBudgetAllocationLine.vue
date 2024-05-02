<template>
  <div>
    <table>
      <tr>
        <td>
          <!--Month -->
        </td>
        <td>
          <!--For Each Donor -->
          <table>
            <tr v-for="donor in donorList" :key="donor.DONOR_ID">
              <td>
                <table>
                  <tr v-for="grant in donorList.Grants" :key="grant.GRANT_ID">
                    <td>
                      <!--Initial Grant Value -->
                    </td>
                    <td>
                      <!--Actual Grant Value -->
                    </td>
                    <td>
                      <!--Actual Grant Value -->
                    </td>
                    <td>
                      <!-- Max Grant allocable % -->
                    </td>
                    <td>
                      <!-- Assigned % -->
                    </td>
                    <td>
                      <!-- Assigned (Value) -->
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </td>
        <td>
          <!-- JRS Budget Unallocated -->
        </td>
        <td>
          <!-- JRS Needs -->
        </td>
      </tr>
    </table>
  </div>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  data() {
    return {
      donorList: {},
    };
  },
});
</script>

<style scoped>
</style>