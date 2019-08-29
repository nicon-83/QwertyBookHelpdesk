<template>
    <md-card class="pagination" v-show="parseInt(length)">
        <md-card-content>
            <div class="md-table-pagination">
                <span>{{ min }}-{{ max }} из {{ count }}</span>

                <md-button class="md-icon-button md-table-pagination-previous" @click="goToPrevious()" :disabled="parseInt(currentPage) === 1">
                    <md-icon>keyboard_arrow_left</md-icon>
                </md-button>

                <md-button class="md-icon-button md-table-pagination-next" @click="goToNext()" :disabled="parseInt(currentPage) >= lastPage">
                    <md-icon>keyboard_arrow_right</md-icon>
                </md-button>
            </div>
        </md-card-content>
    </md-card>
</template>

<script>
  export default {
    name: "Pagination",
    props: {
      min:{
        default: 0
      },
      max:{
        default: 0
      },
      count:{
        default: 0
      },
      length:{
        default: 0
      },
      currentPage:{
        default: 1
      },
    },
    computed: {
      lastPage() {
        return Math.ceil(this.count / 10)
      }
    },
    methods: {
      goToNext() {
        this.$emit('nextPage')
      },
      goToPrevious() {
        this.$emit('prevPage')
      }
    }
  }
</script>

<style scoped>
    .pagination .md-card-content {
        padding-top: 0;
        padding-bottom: 0;
    }

    .md-table-pagination {
        border-top: none;
    }
</style>
