<template src="./GridHTML.html"></template>

<script>
    import { gridMixin } from './GridMixin.js';
    export default {
        mixins: [gridMixin]
    }
</script>

<style lang="scss">
    @import './GridMixin.scss';
</style>
