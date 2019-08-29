<template>
    <main-parent>
        <div class="bg"></div>
        <div class="wrapper">
            <div class="content">
                <md-card>
                    <md-card-header>
                        <div class="md-title"><span class="md-display-2">Ошибка 404</span></div>
                    </md-card-header>

                    <md-card-content>
                        <span class="md-headline">Адрес страницы куда вы пытаетесь перейти, не найден.</span>
                    </md-card-content>

                    <md-card-actions>
                        <md-button class="md-raised md-primary" @click="back()">Вернуться на главную</md-button>
                    </md-card-actions>
                </md-card>
            </div>
        </div>
    </main-parent>
</template>


<script>
  import MainParent from '../MainParent'

  export default {
    name: 'NotFound',
    components: {
      MainParent
    },
    methods: {
      back() {
        this.$router.push('/');
      }
    }
  }
</script>


<style scoped>
    .bg {
        background: url('../../assets/bg_auth.jpg');
        opacity: 0.5;
        position: absolute;
        top: 0;
        right: 0;
        bottom: 0;
        left: 0;
    }

    .wrapper {
        display: flex;
        flex-direction: column;
        height: 100vh;
        flex: 1;
    }

    .content {
        display: flex;
        flex-direction: row;
    }

    .md-card {
        width: 800px;
        padding: 60px;
    }

    .content:before, .content:after, .wrapper:before, .wrapper:after {
        content: '';
        flex: 1;
    }
</style>
