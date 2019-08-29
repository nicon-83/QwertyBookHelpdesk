<template>
    <drawer>
        <!-- <span class="md-display-1">20 недавних новостей</span> -->
        <!--
        <md-card v-for="item in news">
            <md-card-header>
                <div class="md-title">{{item.title}}</div>
                <div class="md-subhead">{{item.date}}</div>
            </md-card-header>

            <md-card-content>
                <span v-html="item.text"></span>
            </md-card-content>
        </md-card>
        -->
    </drawer>
</template>
<script>
  import Drawer from '../common/Drawer'

  export default {
    components: {
      Drawer
    },
    data: function() {
      return {
        news: []
      }
    },
    computed: {
      page() {
        return this.$store.state.page
      }
    },
    mounted () {
      this.$store.dispatch('setPageData', {
        'header': 'Новости1',
        'status': false,
        'data': {}
      });
      this.$http.get('api/news/').then(response => {
        /*this.news = response.body;*/
        this.news = ['a','b'];
        this.$store.dispatch('setPageData', {
          'header': 'Новости2',
          'status': true,
          'data': {}
        });
      }, () => {
        this.$store.dispatch('showSnackBar', {'snackText': 'Ошибка загрузки клиентов'});
        this.$store.dispatch('setPageData', {
        'header': 'Новости3',
          'status': true,
          'data': {}
        });
      });
    }

  }
</script>
<style lang="scss" scoped>
    .md-card {
        margin: 20px 0;
    }
</style>
