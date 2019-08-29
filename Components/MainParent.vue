<template>
    <div>
        <slot></slot>
        <md-snackbar :md-position="snackBar.position" :md-duration="snackBar.isInfinity ? Infinity : snackBar.duration" :md-active.sync="snackBar.showSnackbar" md-persistent>
            <span>{{snackBar.snackText}}</span>
            <md-button class="md-primary" @click="hideSnackbar()">Скрыть</md-button>
        </md-snackbar>
    </div>
</template>

<script>
  export default {
    name: "MainParent",
    computed: {
      snackBar() {
        return this.$store.state.snackBar
      }
    },
    methods: {
      hideSnackbar() {
        this.$store.dispatch('hideSnackbar')
      }
    }
  }

</script>

<style scoped>
</style>
