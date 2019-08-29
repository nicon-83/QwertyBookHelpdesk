
const state = {
    tabs: [
        { Name: 'Телефоны сотрудников', Value: "Phones", Route: 'users', Pagination: false },
        { Name: 'История звонков', Value: "History", Route: 'history', Pagination: true },
        { Name: 'Неизвестный клиент', Value: "EmptyNpp", Route: 'emptynpp', Pagination: false },
        { Name: 'DND ', Value: "DND", Route: 'dnd', Pagination: false }
    ],
    countMutation: 0,
};

const getters = {
    PHONESTABS: state => {
        return state.tabs;
    },
    PHONESCURRENTPAGINATION: state => param_id => {
        let idx = state.tabs.findIndex(x => x.Route === param_id); // param_id = this.$route.params.id
        if (idx == -1) {
            //Default if not found in route
            return false;
        }
        return state.tabs[idx].Pagination;
    },
    PHONESCURRENTTAB: state => param_id => {
        let idx = state.tabs.findIndex(x => x.Route === param_id); // param_id = this.$route.params.id
        if (idx == -1) {
            //Default if not found in route
            return 'NotFound';
        }
        return state.tabs[idx].Value;
    },
    PHONESCURRENTTABROUTE: state => {
        if (state.countMutation < 0) {
            return 'never happen';
        }
        let route = 'unknown';
        let storage = localStorage.PhonesTab;
        if (storage) {
            route = storage;
        }
        let idx = state.tabs.findIndex(x => x.Route === route);
        if (idx == -1) {
            route = state.tabs[0].Route;
        }
        return '/phones/' + route;
    },
};

const mutations = {
    SETPHONESCURRENTTABROUTE: (state, data) => {
        if (data == null) {
            return;
        }
        localStorage.PhonesTab = data.toString();
        state.countMutation++;
    }
};

const actions = {
    SET_PHONESCURRENTTABROUTE: (context, data) => {
        context.commit('SETPHONESCURRENTTABROUTE', data);
    }
};

export default {
    state,
    getters,
    mutations,
    actions,
};