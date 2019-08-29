import axios from 'axios';

const state = {
	tabs: [
		{
			id: 0,
			title: 'Главная',
			type: 'static',
			ticketNumber: 0
		},
		{
			id: 1,
			title: 'Список заявок',
			type: 'static',
			ticketNumber: 0
		},
		{
			id: 2,
			title: 'Архив заявок',
			type: 'static',
			ticketNumber: 0
		}
	],
	localTicketNumber: '0', //LTN
	currentTicket: {}
};

const getters = {
	TABS: state => {
		return state.tabs
	},
	LOCAL_TICKET_NUMBER: state => {
		return state.localTicketNumber
	},
	CURRENT_TICKET: state => {
		return state.currentTicket
	}
};

const mutations = {
	ADD__TAB: (state, data) => {
		state.tabs.push(data);
	},
	CLOSE__TAB: (state, data) => {
		state.tabs.splice(data, 1);
	},
	SET__LTN: (state, data) => {
		state.localTicketNumber = data;
	},
	INCREASE__LTN: (state, data) => {
		state.localTicketNumber++;
	},
	SET__CURRENT__TICKET: (state, data) => {
		state.currentTicket = data;
	}
};

const actions = {
	ADD_TAB: (context, data) => {
		context.commit('ADD__TAB', data);
	},
	CLOSE_TAB: (context, data) => {
		context.commit('CLOSE__TAB', data);
	},
	// GET_TICKETS: (context, payload) => {
	// 	axios.get('data/gettickets')
	// 		.then(function (response) {
	// 			context.commit('SET__TICKETS', response.data);
	// 		})
	// 		.catch(function (error) {
	// 			console.log(error);
	// 		})
	// 		.then(function () {
	// 			// always executed
	// 		});
	// },
	// GET_PRIORITIES: (context, payload) => {
	// 	axios.get('data/getpriorities')
	// 		.then(function (response) {
	// 			context.commit('SET__PRIORITIES', response.data);
	// 		})
	// 		.catch(function (error) {
	// 			console.log(error);
	// 		})
	// 		.then(function () {
	// 			// always executed
	// 		});
	// },
	GET_LTN: (context, payload) => {
		axios.post('data/getltn', JSON.stringify({
			customer: state.customer
		}),
			{ headers: { 'Content-Type': 'application/json' } }
		)
			.then(function (response) {
				context.commit('SET__LTN', response.data);
			})
			.catch(function (error) {
				console.log(error);
			})
	},
	INCREASE_LTN: (context, data) => {
		context.commit('INCREASE__LTN', data);
	},
	SET_CURRENT_TICKET: (context, data) => {
		context.commit('SET__CURRENT__TICKET', data);
	}
}

export default {
	state,
	getters,
	mutations,
	actions,
};
