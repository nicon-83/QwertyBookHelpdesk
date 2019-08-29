<template>
    <div v-if="progressBar.show">
        <md-progress-bar class="md-accent" md-mode="buffer" :md-value="progressBar.amount" :md-buffer="progressBar.buffer"></md-progress-bar>
    </div>
</template>

<script>
  export default {
    name: "DrawerProgressBar",
    computed: {
      progressBar() {
        return this.$store.state.drawerProgressBar
      }
    },
  }
</script>

<style scoped>
    .md-progress-bar {
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        z-index: 5;
        margin-top: 0;
    }

    .md-progress-bar-fill {
        transition: all 40ms cubic-bezier(.25,.8,.25,1),-webkit-transform 40ms cubic-bezier(.25,.8,.25,1);
    }
</style>
