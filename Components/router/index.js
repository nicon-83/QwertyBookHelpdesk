import Vue from "vue";
import Router from "vue-router";

import store from "../store";

import MainPage from "../views/MainPage.vue";
import Auth from "../views/Auth.vue";
import NotFound from "../views/NotFound.vue";
import UserPage from "../views/UserPage.vue";

// import MainPhones from '../phone/MainPhones.vue'

//import Phones from '../phone/phones.vue'
//import History from '../phone/history.vue'

import MainSupport from "../supportNew/MainSupport.vue";

/*
import { books } from './books'
import { documents } from './documents'
import { business_processes } from './business_processes'
import { reports } from './reports'
import { settings } from './settings'
*/
Vue.use(Router);
Vue.component("router-link", Vue.options.components.RouterLink);
Vue.component("router-view", Vue.options.components.RouterView);

let routers = [
	{
		path: "/",
		redirect: '/support/tickets',
		component: MainPage,
		name: "MainPage",
		meta: {
			requiresAuth: true
		}
	},
	// {
	// 	path: "/notes",
	// 	component: MainPage,
	// 	name: "MainPage1"
	// },
	// {
	// 	path: "/clients",
	// 	component: MainPage,
	// 	name: "MainPage2"
	// },
	// {
	// 	path: "/reports",
	// 	component: MainPage,
	// 	name: "MainPage3"
	// },
	// {
	// 	path: "/phones/:id",
	// 	component: MainPhones /*Phones*/,
	// 	name: "MainPhones" /*'Phones'*/
	// 	/*props: {
	//     },*/
	// 	/*beforeEnter: (to, from, next) => {
	//         store.dispatch('SET_PHONE_ENABLEAUTOREFRESH', true);
	//         next();
	//     }*/
	// },

	{
		path: "/support/:id/",
		component: MainSupport,
		name: "MainSupport",
		meta: {
			requiresAuth: true
		}
		//children: [
		//	{
		//		path: ':tn',
		//		component: Messages,
		//		name: 'messages'
		//	}
		//]
		/*props: {
        },*/
		//beforeEnter: (to, from, next) => {
		//	console.log('beforeEnter');
		//          next();
		//      }
	},

	{
		path: "/login",
		component: Auth,
		name: "login",
		meta: {
			title: "Авторизация",
			guest: true
		}
	},

	{
		path: "/userPage",
		component: UserPage,
		name: "userPage",
		meta: {
			title: "Страница пользователя",
			requiresAuth: true
		}
	},

	/* 404 */
	{
		path: "/404",
		name: "404",
		component: NotFound,
		meta: {
			title: "Ошибка 404: Страница не найденна"
		}
	},
	{
		path: "*",
		redirect: "/404"
	}
];

let router = new Router({
	routes: routers,
	mode: "history"
});

const title = "КвертиБук";

router.beforeEach((to, from, next) => {
	// store.dispatch('SET_PHONE_ENABLEAUTOREFRESH', false);
	store.dispatch("hideSnackbar");
	// store.dispatch('checkAuth', { to: to.name });
	if (to.meta.title === undefined) {
		document.title = title;
	} else {
		document.title = to.meta.title + " - " + title;
	}

	//удаляем записи из Local Storage
	if (to.path.indexOf('/login') !== -1) {
		store.dispatch("CLEAR_LOCAL_STORAGE");
		store.dispatch('CLEAR_STATE');
	}

	//проверка аутентификации
	if (to.matched.some(record => record.meta.requiresAuth)) {
		if (localStorage.getItem("apteka") == null) {
			next({
				path: "/login?from=" + to.fullPath
			});
		} else {
			let apteka = JSON.parse(localStorage.getItem("apteka"));
			if (apteka.length === 0) {
				next({
					path: "/login?from=" + to.fullPath
				});
			}
			store.dispatch('SET_ISAUTHENTICATED', true);
			store.dispatch("SET_CURRENTAPTEKA", apteka);

			//проверяем наличие объекта пользователя
			let user = JSON.parse(localStorage.getItem("user"));
			if (!user) user = [];
			store.dispatch("SET_CURRENTUSER", user);

			next();
		}
	} else {
		next();
	}
	// next()
});

export default router;
