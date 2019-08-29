const state = {
	inputData: [],
	firstLoad: true,
	currentPage: 1,
	rowsPerPage: 10,
	rowsPerPageOld: 10,
	ticketsListLastUpdateTime: null,
	TicketsFilterStates: null,
	TicketsFilterClients: null
};

const getters = {
	TICKETSINPUTDATA: state => {
		return state.inputData;
	},
	TICKETSFIRSTLOAD: state => {
		return state.firstLoad;
	},
	TICKETSCURRENTPAGE: state => {
		return state.currentPage;
	},
	TICKETSROWSPERPAGE: state => {
		return state.rowsPerPage;
	},
	TICKETSROWSPERPAGEOLD: state => {
		return state.rowsPerPageOld;
	},
	TICKETSFILTERSTATES: state => {
		if (state.TicketsFilterStates == null) {
			let value = [];
			if (localStorage.TicketsFilterStates) {
				value = localStorage.TicketsFilterStates;
			}
			if (value != null) {
				if (value != "") {
					state.TicketsFilterStates = value;
					return state.TicketsFilterStates;
				}
			}
			state.TicketsFilterStates = "";
		}
		return state.TicketsFilterStates;
	},
	TICKETSFILTERCLIENTS: state => {
		if (state.TicketsFilterClients == null) {
			let value = [];
			if (localStorage.TicketsFilterClients) {
				value = localStorage.TicketsFilterClients;
			}
			if (value != null) {
				if (value != "") {
					state.TicketsFilterClients = value;
					return state.TicketsFilterClients;
				}
			}
			state.TicketsFilterClients = "";
		}
		return state.TicketsFilterClients;
	},
	TICKETSLISTLASTUPDATETIME: state => {
		return state.ticketsListLastUpdateTime;
	}
};

const mutations = {
	SETTICKETSINPUTDATA: (state, data) => {
		state.inputData = data;
	},
	SETTICKETSFIRSTLOAD: (state, data) => {
		state.firstLoad = data;
	},
	SETTICKETSCURRENTPAGE: (state, data) => {
		state.currentPage = data;
	},
	SETTICKETSROWSPERPAGE: (state, data) => {
		state.rowsPerPage = data;
	},
	SETTICKETSROWSPERPAGEOLD: (state, data) => {
		state.rowsPerPageOld = data;
	},
	SETTICKETSFILTERSTATES: (state, data) => {
		if (data == null) {
			return;
		}
		state.TicketsFilterStates = data;
		localStorage.TicketsFilterStates = data.toString();
	},
	SETTICKETSFILTERCLIENTS: (state, data) => {
		if (data == null) {
			return;
		}
		state.TicketsFilterClients = data;
		localStorage.TicketsFilterClients = data.toString();
	},
	SETTICKETSLISTLASTUPDATETIME: (state, data) => {
		state.ticketsListLastUpdateTime = data;
	}
};

const actions = {
	SET_TICKETSINPUTDATA: (context, data) => {
		context.commit("SETTICKETSINPUTDATA", data);
	},
	SET_TICKETSFIRSTLOAD: (context, data) => {
		context.commit("SETTICKETSFIRSTLOAD", data);
	},
	SET_TICKETSCURRENTPAGE: (context, data) => {
		context.commit("SETTICKETSCURRENTPAGE", data);
	},
	SET_TICKETSROWSPERPAGE: (context, data) => {
		context.commit("SETTICKETSROWSPERPAGE", data);
	},
	SET_TICKETSROWSPERPAGEOLD: (context, data) => {
		context.commit("SETTICKETSROWSPERPAGEOLD", data);
	},
	SET_TICKETSFILTERSTATES: (context, data) => {
		context.commit("SETTICKETSFILTERSTATES", data);
	},
	SET_TICKETSFILTERCLIENTS: (context, data) => {
		context.commit("SETTICKETSFILTERCLIENTS", data);
	},
	SET_TICKETSLISTLASTUPDATETIME: (context, data) => {
		context.commit("SETTICKETSLISTLASTUPDATETIME", data);
	}
};

export default {
	state,
	getters,
	mutations,
	actions
};
