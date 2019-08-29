import Vue from 'vue'
import Vuex from 'vuex'
import VueResource from 'vue-resource'

import router from '../router'
// import { log } from 'util'
// import mainphones from './mainphones'
// import phones from './phones'
// import history from './history'

import mainsupport from "./mainsupport";
import tickets from "./tickets"
// import { stat } from 'fs';

Vue.use(Vuex);
Vue.use(VueResource);


const store = new Vuex.Store({
	state: {
		// user: {},
		// drawerProgressBar: {
		// 	show: false,
		// 	amount: 10,
		// 	buffer: 100,
		// 	interval: undefined
		// },
		snackBar: {
			showSnackbar: false,
			position: "center",
			duration: 4000,
			isInfinity: false,
			snackText: ""
		},
		page: {
			header: "",
			status: "",
			data: {
				status: ""
			}
		},
		app: {
			toolbarHeight: 52,
			tableHeight: 400
		},
		// menu: {},
		isAuthenticated: false
	},

	getters: {
		ISAUTHENTICATED: state => {
			return state.isAuthenticated;
		}
	},

	mutations: {
		set(states, { type, data }) {
			states[type] = data;
		},
		setField(states, { type, field, data }) {
			states[type][field] = data;
		},
		SETISAUTHENTICATED: (state, data) => {
			state.isAuthenticated = data;
		},
		CLEARSTATE: (state, data) => {
			state.mainsupport.tabs = [{
				id: 0,
				Name: "Список заявок ",
				Value: "tickets",
				Route: "tickets",
				Pagination: true
			}];
			state.mainsupport.numberOfFirstTab = 1;
			state.mainsupport.openedTicketsTabs = [];
			state.mainsupport.countMutation = 0;
			state.mainsupport.CurrentTabId = 0;
			state.mainsupport.hasCreateTicketTab = false;
			state.mainsupport.apteka = [];
			state.mainsupport.ticketsWithChanges = [];
			state.mainsupport.user = [];
			state.mainsupport.siteUsers = [];
			state.mainsupport.userId = 0;

			state.tickets.inputData = [];
			state.tickets.firstLoad = true;
			state.tickets.currentPage = 1;
			state.tickets.rowsPerPage = 10;
			state.tickets.rowsPerPageOld = 10;
			state.tickets.ticketsListLastUpdateTime = null;
			state.tickets.TicketsFilterStates = null;
			state.tickets.TicketsFilterClients = null;
		}
	},

	actions: {
		SET_ISAUTHENTICATED: (context, data) => {
			context.commit("SETISAUTHENTICATED", data);
		},
		/* Блок авторизации */
		auth({ state, dispatch, commit }, data) {
			store.dispatch("hideSnackbar");

			const LOGIN_URL = "/api/user/auth";

			const auth_data = {
				username: data.username,
				password: data.password
			};

			//Add Serg
			var obj_username = {
				originalName: auth_data.username,
				rolesName: "Пользователь"
			};
			commit("set", { type: "user", data: obj_username });
			Vue.nextTick(() => {
				router.push("/");
			});
			/*
            Vue.http.post(LOGIN_URL, auth_data).then(response => {
                if (!!response.body.success) {
                    const GET_AUTH_USER_URL = '/api/user/get_auth_user';
                    Vue.http.get(GET_AUTH_USER_URL).then(response => {
                        if (response.body.length > 0) {
                            commit('set', { type: 'user', data: response.body[0] });
                            Vue.nextTick(() => {
                                router.push('/');
                            })
                        } else {
                            store.dispatch('showSnackBar', { 'snackText': 'Ошибка получения данных пользователя' });
                        }
                    });
                } else {
                    store.dispatch('showSnackBar', { 'snackText': 'Неизвестная ошибка, обратитесь в службу поддержки. Код: 101' });
                }
            }, err => {
                switch (err.body.error) {
                    case 'To many users with this username':
                        store.dispatch('showSnackBar', { 'snackText': 'Ошибка множественности пользователей, обратитесь в службу поддержки' });
                        break;
                    case 'The user not found':
                        store.dispatch('showSnackBar', { 'snackText': 'Пользователь с таким именем не найден' });
                        break;
                    case 'Invalid password':
                        store.dispatch('showSnackBar', { 'snackText': 'Пароль неверен, попробуйте ещё раз.' });
                        break;
                    default:
                        store.dispatch('showSnackBar', { 'snackText': 'Неизвестная ошибка, обратитесь в службу поддержки. Код: 102' });
                        break;
                }
                console.log(err);
                });
            */
		},
		checkAuth({ commit }, data) {
			/*
            const TEST_AUTH_URL = '/api/user/check_auth';
            Vue.http.get(TEST_AUTH_URL).then(response => {
                if (response.body.length == 0) {
                    router.push('/login')
                }
            });
            */
		},
		getAuthUser({ commit }, data) {
			const GET_AUTH_USER_URL = "/api/user/get_auth_user";
			Vue.http.get(GET_AUTH_USER_URL).then(response => {
				if (response.body.length > 0) {
					commit("set", { type: "user", data: response.body[0] });
				} else {
					store.dispatch("showSnackBar", {
						snackText: "Ошибка получения данных пользователя"
					});
				}
			});
		},
		logout({ commit }) {
			const LOGOUT_URL = "/api/user/logout";
			Vue.http.get(LOGOUT_URL).then(
				response => {
					commit("set", { type: "user", data: {} });

					router.push("/login");
				},
				() => {
					store.dispatch("showSnackBar", {
						snackText: "Неизвестная ошибка, обратитесь в службу поддержки"
					});
				}
			);
		},

		/* Блок снекбара */
		showSnackBar({ commit }, data) {
			const snackBarData = {
				showSnackbar: data["showSnackbar"] ? data["showSnackbar"] : true,
				position: data["position"] ? data["position"] : "center",
				duration: data["duration"] ? data["duration"] : 4000,
				isInfinity: data["isInfinity"] ? data["isInfinity"] : false,
				snackText: data["snackText"]
			};
			commit("set", { type: "snackBar", data: snackBarData });
		},
		hideSnackbar({ commit }) {
			commit("setField", {
				type: "snackBar",
				field: "showSnackbar",
				data: false
			});
		},

		/* Блок прогресс бара */
		// showProgressBar({ commit }, data) {
		// 	const progressBarData = {
		// 		showSnackbar: data["show"] ? data["show"] : true,
		// 		position: data["amount"] ? data["position"] : 0,
		// 		duration: data["buffer"] ? data["duration"] : 100
		// 	};
		// 	commit("set", { type: "drawerProgressBar", data: progressBarData });
		// },
		// showProgressBarTimer({ commit }, time) {
		// 	let step = 100 / parseInt(time / 40);
		// 	commit("setField", {
		// 		type: "drawerProgressBar",
		// 		field: "amount",
		// 		data: 0
		// 	});
		// 	commit("setField", {
		// 		type: "drawerProgressBar",
		// 		field: "show",
		// 		data: true
		// 	});
		// 	commit("setField", {
		// 		type: "drawerProgressBar",
		// 		field: "interval",
		// 		data: setInterval(() => {
		// 			commit("setField", {
		// 				type: "drawerProgressBar",
		// 				field: "amount",
		// 				data: this.state.drawerProgressBar.amount + step
		// 			});
		// 			if (this.state.drawerProgressBar.amount > 100) {
		// 				clearInterval(this.state.drawerProgressBar.interval);
		// 			}
		// 		}, 40)
		// 	});
		// },
		// hideProgressBar({ commit }) {
		// 	commit("setField", {
		// 		type: "drawerProgressBar",
		// 		field: "show",
		// 		data: false
		// 	});
		// 	clearInterval(this.state.drawerProgressBar.interval);
		// 	commit("setField", {
		// 		type: "drawerProgressBar",
		// 		field: "interval",
		// 		data: {}
		// 	});
		// },

		/* Блок установки данных страницы, легаси */
		setPageData({ commit }, data) {
			commit("setField", { type: "page", field: "header", data: data.header });
			commit("setField", { type: "page", field: "data", data: data.data });
			commit("setField", { type: "page", field: "status", data: data.status });
		},
		setPageStatus({ commit }, data) {
			commit("setField", { type: "page", field: "status", data: data });
		},

		/* Блок отправки данных на сервер */
		editElement({ commit }, data) {
			commit("setField", { type: "page", field: "status", data: "loading" });

			const EDIT_PAGE_URL = "/api/" + data.page;
			Vue.http.get(EDIT_PAGE_URL, { params: data.params }).then(
				response => {
					commit("setField", {
						type: "page",
						field: "status",
						data: "success"
					});
					store.dispatch("showSnackBar", { snackText: response.body.text });
					return response;
				},
				response => {
					commit("setField", {
						type: "page",
						field: "status",
						data: "success"
					});
					store.dispatch("showSnackBar", {
						snackText: "Ошибка обработки запроса"
					});
					return response;
				}
			);
		},

		setApp({ commit }, data) {
			const appData = {
				toolbarHeight: data["toolbarHeight"] ? data["toolbarHeight"] : 52,
				tableHeight: data["tableHeight"] ? data["tableHeight"] : 400
			};
			commit("set", { type: "app", data: appData });
		},

		setMenu({ commit }, data) {
			//Add Serg
			/*
            var valuedata = [];

            var valuemenuchild = new Object();
            valuemenuchild["menuid"] = 1;
            valuemenuchild["iconlabel"] = "contact_phone";
            valuemenuchild["menuname"] = "Телефоны сотрудников";
            valuemenuchild["url"] = "/phone/device";
            valuemenuchild["sort"] = 1;
            valuemenuchild["parentid"] = 1;

            var valuemenu = new Object();
            valuemenu["menuid"] = 1;
            valuemenu["iconlabel"] = "phone";
            valuemenu["menuname"] = "Телефония";
            valuemenu["url"] = "";
            valuemenu["sort"] = 1;
            valuemenu["parentid"] = 1;
            valuemenu["childs"] = [];
            valuemenu["childs"].push(valuemenuchild);

            valuedata.push(valuemenu);
            */

			var obj_menu = [
				{
					menuname: "Телефоны сотрудников",
					iconlabel: "contact_phone",
					url: "/phone/device"
				}
			];
			/*console.log(obj_menu, obj_menu["menuname"]);*/
			commit("set", { type: "menu", data: obj_menu }); //valuedata
			//commit('set', { type: 'menu', data: data });
		},
		CLEAR_LOCAL_STORAGE: () => {
			localStorage.removeItem("apteka");
			localStorage.removeItem("user");
			localStorage.removeItem("openedTabs");
			localStorage.removeItem("SupportTab");
			localStorage.removeItem("TicketsFilterStates");
			localStorage.removeItem("TicketsFilterClients");
		},
		CLEAR_STATE: (context, data) => {
			context.commit('CLEARSTATE');
		}
	},

	modules: {
		// mainphones,
		// phones,
		// history,
		mainsupport,
		tickets
	}
});


Vue.http.interceptors.push(function (request, next) {
    next(function (response) {
        if (response.body.error === 'permission denied') {
            this.$store.dispatch('showSnackBar', { 'snackText': 'Нет доступа' });
        }
        if (response.body.error === 'Unauthorized access') {
            this.$store.dispatch('showSnackBar', { 'snackText': 'Вы не авторизированны' });
            this.$router.push('/login')
        }
    });
});

export default store
