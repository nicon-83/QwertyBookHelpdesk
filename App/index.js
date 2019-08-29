/*
import Vue from 'vue'
import VueRouter from 'vue-router'

import VueMaterial from 'vue-material'
//import 'vue-material/dist/vue-material.css'
import 'vue-material/dist/vue-material.min.css'
import 'vue-material/dist/theme/default.css'


import App from './App.vue'
//import GridBasic from '../Components/GridBasic.vue'
//import GridRed from '../Components/GridRed.vue'
import MDGrid from '../Components/MDGrid.vue'

Vue.config.productionTip = false
Vue.use(VueRouter)
Vue.use(VueMaterial)
*/

import "@babel/polyfill"
import Vue from 'vue'
import { sync } from 'vuex-router-sync'

import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.css'
import '../Components/theme-green.scss'
import '../Components/theme-blue.scss'

//import Vuetify from 'vuetify'
//import 'vuetify/dist/vuetify.min.css'

import App from '../Components/App.vue'
import store from '../Components/store'
import router from '../Components/router'

//подключение Bootstrap-Vue
//import BootstrapVue from 'bootstrap-vue'
//import 'babel-polyfill' // для поддержки IE11
//import 'bootstrap/dist/css/bootstrap.css'
//import 'bootstrap-vue/dist/bootstrap-vue.css'
//Vue.use(BootstrapVue);

//Бесшумный режим???
//Vue.config.silent = true;
//Для релиза
//Vue.config.productionTip = false;

Vue.use(VueMaterial);
//Vue.use(Vuetify);
//Vue.material.locale.dateFormat = 'DD-MM-YYYY'; //Add Serg 02/02/2019 for md-datepicker - Глючит(())

Vue.directive('focus', {
    inserted: function (el) {
        el.focus()
    }
});

sync(store, router);

new Vue({
    el: '#app',
    data: {
    },
    router,
    store,
    components: { App },
    template: '<App/>'
});
