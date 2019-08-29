import Vue from "vue";

const state = {
	tabs: [
		// {
		// 	id: 0,
		// 	Name: "Вход",
		// 	Value: "home",
		// 	Route: "home",
		// 	Pagination: false
		// },
		{
			id: 0,
			Name: "Список заявок ",
			Value: "tickets",
			Route: "tickets",
			Pagination: true
		}
		// {
		// 	id: 2,
		// 	Name: "Архив заявок ",
		// 	Value: "archive",
		// 	Route: "archive",
		// 	Pagination: false
		// }
	], //массив для хранения объектов открытых вкладок
	numberOfFirstTab: 1, //следующий номер после номера последней статической вкладки state.tabs
	tickets: [], // массив для хранения всех заявок аптеки, используется для защиты от показа заявок других аптек
	priorities: [], //массив для хранния приоритетов заявок
	// customer: "501", //npp из таблицы COMPANYS
	//agent: "167", // аутентифицированный в системе пользователь, на период разработки Македонов В.А., затем надо будет получать из блока авторизации
	openedTicketsTabs: [], //хранилище для номеров заявок которые открыты во вкладках, защита от дублей открытых вкладок
	countMutation: 0,
	CurrentTabId: 0, // id текущей активной вкладки
	hasCreateTicketTab: false, //индикатор наличия открытой вкладки создания новой заявки, должны быть открыта только одна вкладка создания новой заявки
	// agents: [],
	ticketsStates: [], // массив для хранения статусов заявок
	// ticketsClients: [], //массив для хранения списка аптек, планировалось использовать для фильтрации заявок по аптекам, в клиентской части не используется
	apteka: [], //объект аутентифицировавшейся аптеки
	ticketsWithChanges: [], // массив объектов всех заявок с изменениями
	ticketsWithUserMessages: [], // массив заявок в которых участвует пользователь (т.е. заявки в которых есть сообщение от пользователя)
	user: [], //объект пользователя сайта аптеки под которым выполнен вход
	siteUsers: [], //массив объектов пользователей сайта аптеки
	userId: 0, //id пользователя сайта под которым выполнен вход
	knownFileTypes: ['doc', 'docx', 'xls', 'xlsx', 'zip', 'pdf', 'ppt', 'pptx', 'txt', 'bmp', 'jpg', 'js', 'css', 'xml', 'sql', 'tif', 'html', 'gif', 'png', 'php', 'dmg'], //svg иконки в /dist/svg
	dataBaseVersion: 'production'
};

const getters = {
	SUPPORTTABS: state => {
		return state.tabs;
	},
	SUPPORTCURRENTPAGINATION: state => param_id => {
		let idx = state.tabs.findIndex(x => x.Route === param_id); // param_id = this.$route.params.id
		if (idx === -1) {
			//Default if not found in route
			return false;
		}
		return state.tabs[idx].Pagination;
	},
	SUPPORTCURRENTTAB: state => param_id => {
		//определяем какой компонент будем загружать
		let idx = state.tabs.findIndex(x => x.Route === param_id);
		if (idx === -1) {
			//Default if not found in route
			console.log(
				"Нет компонента в массиве вкладок, загружен компонент - NotFound"
			);
			return "NotFound";
		}
		return state.tabs[idx].Value;
	},
	SUPPORTCURRENTTABROUTE: state => {
		if (state.countMutation < 0) {
			return "never happen";
		}
		let route = "unknown";
		let storage = localStorage.SupportTab;
		if (storage) {
			route = storage;
		}
		let idx = state.tabs.findIndex(x => x.Route === route);
		if (idx === -1) {
			route = state.tabs[0].Route;
		}
		return "/support/" + route;
	},
	CURRENTTABID: state => {
		return state.CurrentTabId;
	},
	LAST_NUMBER: state => {
		return state.numberOfFirstTab;
	},
	TICKETS: state => {
		return state.tickets;
	},
	PRIORITIES: state => {
		return state.priorities;
	},
	// CUSTOMER: state => {
	// 	return state.customer;
	// },
	// AGENT: state => {
	// 	return state.agent;
	// },
	HASCREATETICKETTAB: state => {
		return state.hasCreateTicketTab;
	},
	OPENEDTICKETSTABS: state => {
		return state.openedTicketsTabs;
	},
	// AGENTS: state => {
	// 	return state.agents;
	// },
	TICKETSSTATES: state => {
		return state.ticketsStates;
	},
	// TICKETSCLIENTS: state => {
	// 	return state.ticketsClients;
	// },
	CURRENTAPTEKA: state => {
		return state.apteka;
	},
	TICKETSWITHCHANGES: state => {
		return state.ticketsWithChanges;
	},
	TICKETSWITHUSERMESSAGES: state => {
		return state.ticketsWithUserMessages;
	},
	CURRENTUSER: state => {
		return state.user;
	},
	SITEUSERS: state => {
		return state.siteUsers;
	},
	USERID: state => {
		return state.userId;
	},
	KNOWNFILETYPES: state => {
		return state.knownFileTypes;
	},
	DATABASEVERSION: state => {
		return state.dataBaseVersion;
	}
};

const mutations = {
	SETSUPPORTCURRENTTABROUTE: (state, data) => {
		if (data == null) {
			return;
		}
		localStorage.SupportTab = data.toString();
		state.countMutation++;
	},
	ADDTAB: (state, data) => {
		state.tabs.push(data);
	},
	CLOSETAB: (state, data) => {
		state.tabs.splice(data, 1);
	},
	INCREASE__LAST__NUMBER: state => {
		state.numberOfFirstTab++;
	},
	SET__TICKETS: (state, data) => {
		state.tickets = data;
	},
	SET__PRIORITIES: (state, data) => {
		state.priorities = data;
	},
	SETCURRENTTABID: (state, data) => {
		state.CurrentTabId = data;
	},
	SETHASCREATETICKETTAB: (state, data) => {
		state.hasCreateTicketTab = data;
	},
	ADDOPENEDTICKETSTABS: (state, data) => {
		state.openedTicketsTabs.push(data);
	},
	REMOVEOPENEDTICKETSTABS: (state, data) => {
		state.openedTicketsTabs.splice(data, 1);
	},
	// SETAGENTS: (state, data) => {
	// 	state.agents = data;
	// },
	SETTICKETSSTATES: (state, data) => {
		state.ticketsStates = data;
	},
	// SETTICKETSCLIENTS: (state, data) => {
	// 	state.ticketsClients = data;
	// },
	SETCURRENTAPTEKA: (state, data) => {
		state.apteka = data;
	},
	ADDTICKETSWHITHCHANGES: (state, data) => {
		state.ticketsWithChanges.push(data);
	},
	ADDTICKETSWITHUSERMESSAGES: (state, data) => {
		state.ticketsWithUserMessages.push(data);
	},
	REMOVETICKETSWHITHCHANGES: (state, data) => {
		state.ticketsWithChanges.splice(data, 1);
	},
	SETTICKETSWHITHCHANGES: (state, data) => {
		state.ticketsWithChanges = data;
	},
	SETTICKETSWITHUSERMESSAGES: (state, data) => {
		state.ticketsWithUserMessages = data;
	},
	SETCURRENTUSER: (state, data) => {
		state.user = data;
	},
	SETSITEUSERS: (state, data) => {
		state.siteUsers = data;
	},
	SETUSERID: (state, data) => {
		state.userId = data;
	},
	SETDATABASEVERSION: (state, data) => {
		state.dataBaseVersion = data;
	},
};

const actions = {
	SET_SUPPORTCURRENTTABROUTE: (context, data) => {
		context.commit("SETSUPPORTCURRENTTABROUTE", data);
	},
	ADD_TABB: (context, data) => {
		context.commit("ADDTAB", data);
		localStorage.openedTabs = JSON.stringify(state.tabs);
	},
	CLOSE_TABB: (context, data) => {
		context.commit("CLOSETAB", data);
		localStorage.openedTabs = JSON.stringify(state.tabs);
	},
	INCREASE_LAST_NUMBER: context => {
		context.commit("INCREASE__LAST__NUMBER");
	},
	GET_TICKETS: (context, data) => {
		Vue.http
			.get("data/GetClientTickets", {
				params: {apteka_npp: data}
			})
			.then(
				response => {
					context.commit("SET__TICKETS", response.body);
				},
				error => {
					console.log(error);
				}
			);
	},
	GET_PRIORITIES: (context, payload) => {
		Vue.http.get("data/getpriorities").then(
			response => {
				context.commit("SET__PRIORITIES", response.body);
			},
			error => {
				console.log(error);
			}
		);
	},
	SET_CURRENTTABID: (context, data) => {
		context.commit("SETCURRENTTABID", data);
	},
	SET_HASCREATETICKETTAB: (context, data) => {
		context.commit("SETHASCREATETICKETTAB", data);
	},
	ADD_OPENEDTICKETSTABS: (context, data) => {
		context.commit("ADDOPENEDTICKETSTABS", data);
	},
	REMOVE_OPENEDTICKETSTABS: (context, data) => {
		context.commit("REMOVEOPENEDTICKETSTABS", data);
	},
	// GET_AGENTS: (context, payload) => {
	// 	Vue.http.get("data/getAgents").then(
	// 		response => {
	// 			context.commit("SETAGENTS", response.body);
	// 		},
	// 		error => {
	// 			console.log(error);
	// 		}
	// 	);
	// },
	GET_TICKETS_STATES: (context, payload) => {
		Vue.http.get("data/getStates").then(
			response => {
				context.commit("SETTICKETSSTATES", response.body);
			},
			error => {
				console.error(error);
			}
		);
	},
	// GET_TICKETS_CLIENTS: (context, payload) => {
	// 	Vue.http.get("data/getClients").then(
	// 		response => {
	// 			context.commit("SETTICKETSCLIENTS", response.body);
	// 		},
	// 		error => {
	// 			console.error(error);
	// 		}
	// 	);
	// },
	SET_CURRENTAPTEKA: (context, data) => {
		context.commit("SETCURRENTAPTEKA", data);
	},
	ADD_TICKETSWHITHCHANGES: (context, data) => {
		if (!state.ticketsWithChanges.find(x => x.npp === data.npp)) {
			context.commit("ADDTICKETSWHITHCHANGES", data);
		}
	},
	ADD_TICKETSWITHUSERMESSAGES: (context, data) => {
		if (!state.ticketsWithUserMessages.includes(data)) {
			context.commit("ADDTICKETSWITHUSERMESSAGES", data);
		}
	},
	REMOVE_TICKETSWHITHCHANGES: (context, data) => {
		context.commit("REMOVETICKETSWHITHCHANGES", data);
		// localStorage.setItem(`ticketsWithChanges_${state.apteka.idapt}`, JSON.stringify(state.ticketsWithChanges));
	},
	SET_TICKETSWHITHCHANGES: (context, data) => {
		context.commit("SETTICKETSWHITHCHANGES", data);
	},
	SET_TICKETSWITHUSERMESSAGES: (context, data) => {
		context.commit("SETTICKETSWITHUSERMESSAGES", data);
	},
	SET_TICKETLASTUPDATETIME: (context, data) => {
		const params = {
			ticketNpp: data.ticketNpp,
			clientNpp: data.clientNpp,
			userId: data.userId
		};
		Vue.http.put("data/SetTicketLastUpdateTime", params, {emulateJSON: true}).then(
			() => {

			},
			error => {
				console.error(error);
				Vue.store.dispatch("showSnackBar", {
					snackText: `Ошибка при обновлении времени последнего просмотра заявки ${data.ticketNpp}`
				});
			}
		);
	},
	SET_LASTUPDATETIMEFROMSERVER: (context, data) => {
		const params = {
			clientNpp: data.clientNpp,
			userId: data.userId
		};
		Vue.http.put("data/SetLastUpdateTimeFromServer", params, {emulateJSON: true}).then(
			() => {

			},
			error => {
				console.error(error);
				Vue.store.dispatch("showSnackBar", {
					snackText: `Ошибка при обновлении времени последних изменений`
				});
			}
		);
	},
	SET_CURRENTUSER: (context, data) => {
		context.commit("SETCURRENTUSER", data);
	},
	SET_USERID: (context, data) => {
		context.commit("SETUSERID", data);
	},
	GET_SITEUSERS: (context, data) => {
		Vue.http.get("data/GetSiteUsers", {
			params: {aptekaId: data}
		}).then(
			response => {
				context.commit("SETSITEUSERS", response.body);
			},
			error => {
				console.error(error);
			}
		);
	},
	GET_DATABASEVERSION: (context, data) => {
		Vue.http.get("data/GetDataBaseVersion").then(
			response => {
				context.commit("SETDATABASEVERSION", response.body);
			},
			error => {
				console.error(error);
			}
		);
	},
};

export default {
	state,
	getters,
	mutations,
	actions
};
