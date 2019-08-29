//import axios from 'axios';
import Vue from 'vue';

const state = {
    Otdels: [],
    FilterOtdels: null,
    FilterState: null,
    Phone_EnableAutoRefresh: false
};

const getters = {
    PHONE_ENABLEAUTOREFRESH: state => {
        return state.Phone_EnableAutoRefresh;
    },
    OTDELS: state => {
        return state.Otdels;
    },
    FILTEROTDELS: state => {
        if (state.FilterOtdels == null) {
            let value = [];
            if (localStorage.PhonesFilterOtdels) {
                value = localStorage.PhonesFilterOtdels.split(",");
            }
            state.FilterOtdels = value;
        }
        return state.FilterOtdels;
    },
    FILTERSTATE: state => {
        if (state.FilterState == null) {
            let value = 'All';
            if (localStorage.PhonesFilterState) {
                value = localStorage.PhonesFilterState;
            }
            state.FilterState = value;
        }
        return state.FilterState;
    },
};

const mutations = {
    SETPHONE_ENABLEAUTOREFRESH: (state, data) => {
        state.Phone_EnableAutoRefresh = data;
    },
    SETOTDELS: (state, data) => {
        state.Otdels = data;
    },
    SETFILTEROTDELS: (state, data) => {
        state.FilterOtdels = data;
        localStorage.PhonesFilterOtdels = data.toString();
    },
    SETFILTERSTATE: (state, data) => {
        state.FilterState = data;
        localStorage.PhonesFilterState = data;
    },
};

const actions = {
    SET_PHONE_ENABLEAUTOREFRESH: (context, data) => {
        context.commit('SETPHONE_ENABLEAUTOREFRESH', data);
    },
    GET_OTDELS: (context, data) => {
        Vue.http.get('data/getotdels', {
            params: ''
        }
        ).then(response => {
            let otd = [];
            for (let value of response.body) {
                otd.push(value.otdel);
            }
            context.commit('SETOTDELS', otd);
        }, () => {
            console.log(error);
        })
        /*
        axios.get('data/getotdels')
            .then(function (response) {
                // handle success
                let otd = [];
                for (let value of response.data) {
                    otd.push(value.otdel);
                }
                context.commit('SETOTDELS', otd);
            })
            .catch(function (error) {
                // handle error
                console.log(error);
            })
            .then(function () {
                // always executed
            });
            */
    },
    SET_OTDELS: (context, data) => {
        context.commit('SETOTDELS', data );
    },
    SET_FILTEROTDELS: (context, data) => {
        context.commit('SETFILTEROTDELS', data);
    },
    SET_FILTERSTATE: (context, data) => {
        context.commit('SETFILTERSTATE', data);
    },
};

export default {
    state,
    getters,
    mutations,
    actions,
};