<template src="./GridHTML.html">
</template>

<script>
    import { gridMixin } from './GridMixin.js';
    export default {
        mixins: [gridMixin]
    }
</script>

<style lang="scss">
    @import './GridMixin.scss';
    table {
        border: 2px solid #d8202c;
    }

    th {
        background-color: #d8202c;
    }
</style>
