<template>
    <div class="module-details">
        <v-card-title>{{ moduleData.moduleName }}</v-card-title>
        <v-card-text>
            <v-list dense>
                <v-list-item v-for="(item, index) in moduleData.menuItems" :key="index">
                    <menu-btn :menu="item" :module="moduleData"></menu-btn>
                </v-list-item>
            </v-list>
        </v-card-text>
    </div>
</template>

<script>
    export default {
        props: {
            moduleData: {
                type: Object,
                required: true
            },
        },
    };
</script>

<style scoped>
    /* Add styles here */
</style>
