<template>
    <div>
        <span>{{helloworld()}}</span>
    </div>
</template>
<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'

@Component
export default class HelloWorld2 extends Vue {
    private testo: string = 'Hello world2!';

    get computedAttr(): string {
        return this.testo;
    }
    // classic 
    mounted() {
        //(this.testo);
    }

    helloworld() {
        return this.computedAttr;
    }
}
</script>
<style>

</style>