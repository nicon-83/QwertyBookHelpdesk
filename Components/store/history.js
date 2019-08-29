import axios from 'axios';
import Vue from 'vue';

const state = {
    inputData: [],
    selected: [],
    firstLoad: true,
    DispositionHistory: [/*{ DISPOSITION: "ALL", STATUS: "Все вызовы" },*/ { DISPOSITION: "ANSWERED", STATUS: "Разговор" }, { DISPOSITION: "BUSY", STATUS: "Занято" }, { DISPOSITION: "NO ANSWER", STATUS: "Пропущен" }, { DISPOSITION: "DND", STATUS: "DND" }, { DISPOSITION: "QUEUE", STATUS: "Очередь" }, { DISPOSITION: "FAILED", STATUS: "FAILED" }, { DISPOSITION: "CONGESTION", STATUS: "CONGESTION" } ],
    OtdelsHistory: [],
    UsersHistory: [],
    FilterHistory: [{ LABEL: "Нет в КБ", DATA: "EMPTY_NPP" }, { LABEL: "Внешний вызов", DATA: "EXTERNAL" }, { LABEL: "Локальный вызов", DATA: "INTERNAL" }],
    HistoryDateList: null,
    HistoryFilterDispositon: null,
    HistoryFilterOtdels: null,
    HistoryFilterUsers: null,
    HistoryFilter: null,
    HistoryFilterClient: null,
    HistoryParamsBegin: {
        Name: null,
        Date: null,
    },
    HistoryParamsEnd: {
        Name: null,
        Date: null,
    },
};

const getters = {
    HISTORYSELECTED: state => {
        return state.selected;
    },
    HISTORYINPUTDATA: state => {
        return state.inputData;
    },
    HISTORYFIRSTLOAD: state => {
        return state.firstLoad;
    },
    DISPOSITIONHISTORY: state => {
        return state.DispositionHistory;
    },
    FILTERHISTORY: state => {
        return state.FilterHistory;
    },
    HISTORYFILTERDISPOSITION: state => {
        if (state.HistoryFilterDispositon == null) {
            let value = [];
            if (localStorage.HistoryFilterDispositon) {
                value = localStorage.HistoryFilterDispositon;
            }
            if (value != null) {
                if (value != '') {
                    state.HistoryFilterDispositon = value;
                    return state.HistoryFilterDispositon;
                }
            }
            /*state.HistoryFilterDispositon = 'ANSWERED';*/
            state.HistoryFilterDispositon = '';
        }
        return state.HistoryFilterDispositon;
    },
    HISTORYFILTER: state => {
        if (state.HistoryFilter == null) {
            let value = [];
            if (localStorage.HistoryFilter) {
                value = localStorage.HistoryFilter;
            }
            if (value != null) {
                if (value != '') {
                    state.HistoryFilter = value;
                    return state.HistoryFilter;
                }
            }
            state.HistoryFilter = '';
        }
        return state.HistoryFilter;
    },
    HISTORYUSERS: state => {
        if (state.HistoryFilterOtdels == null) {
            return state.UsersHistory;
        }
        /*if (state.HistoryFilterOtdels == 'Все отделы') {*/
        if (state.HistoryFilterOtdels == '') {
            return state.UsersHistory;
        }
        return state.UsersHistory.filter(item => {
            if (item.id_group === "0") {
                return true;
            }
            if (item.otdel === state.HistoryFilterOtdels) {
                return true;
            }
            return false;
        })
    },
    HISTORYUSERSFULL: state => {
        return state.UsersHistory;
    },
    HISTORYOTDELS: state => {
        return state.OtdelsHistory;
    },
    HISTORYDATELIST: state => {
        return state.HistoryDateList;
    },
    HISTORYPARAMSBEGIN_JSON: state => {
        let d = state.HistoryParamsBegin.Date;
        if (d == null) {
            return null
        } else {  
            d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
            let s = d.toJSON();
            s = s.substr(0, 11);//2019-01-31T
            s = s + '00:00:00';
            return s
        }
    },
    HISTORYPARAMSEND_JSON: state => {
        let d = state.HistoryParamsEnd.Date;
        if (d == null) {
            return null
        } else {
            d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
            let s = d.toJSON();
            s = s.substr(0, 11);//2019-01-31T
            s = s + '23:59:59';
            return s
        }
    },
    HISTORYPARAMSBEGIN: state => {
        if (state.HistoryParamsBegin.Date == null) {
            let d = new Date;
            let year = d.getYear() + 1900;
            let month = d.getMonth();//-1
            let day = d.getDate();
            let d2 = new Date(year, month, day);
            /*d.setMonth(d.getMonth() - 1);*/
            state.HistoryParamsBegin.Date = d2;
        }
        return state.HistoryParamsBegin;
    },
    HISTORYPARAMSEND: state => {
        if (state.HistoryParamsEnd.Date == null) {
            let d = new Date;
            let year = d.getYear() + 1900;
            let month = d.getMonth();
            let day = d.getDate();
            let d2 = new Date(year, month, day);
            state.HistoryParamsEnd.Date = d2;
        }
        return state.HistoryParamsEnd;
    },
    HISTORYFILTERUSERS: state => {
        if (state.HistoryFilterUsers == null) {
            let value = [];
            if (localStorage.HistoryFilterUsers) {
                value = localStorage.HistoryFilterUsers;
            }
            if (value != null) {
                if (value != '') {
                    state.HistoryFilterUsers = value;
                    return state.HistoryFilterUsers;
                }
            }
            /*state.HistoryFilterUsers = 'Все сотрудники';*/
            state.HistoryFilterUsers = '';
        }
        return state.HistoryFilterUsers;
    },
    HISTORYFILTEROTDELS: state => {
        if (state.HistoryFilterOtdels == null) {
            let value = [];
            if (localStorage.HistoryFilterOtdels) {
                value = localStorage.HistoryFilterOtdels;
            }
            if (value != null) {
                if (value != '') {
                    state.HistoryFilterOtdels = value;
                    return state.HistoryFilterOtdels;
                }
            }
            /*state.HistoryFilterOtdels = 'Все отделы';*/
            state.HistoryFilterOtdels = '';
        }
        return state.HistoryFilterOtdels;
    },
    HISTORYFILTERCLIENT: state => {
        if (state.HistoryFilterClient == null) {
            let value = null;
            if (localStorage.HistoryFilterClient) {
                value = localStorage.HistoryFilterClient;
            }
            if (value != null) {
                if (value != '') {
                    state.HistoryFilterClient = value;
                    return state.HistoryFilterClient;
                }
            }
            state.HistoryFilterClient = null;
        }
        return state.HistoryFilterClient;
    },};

const mutations = {
    SETHISTORYINPUTDATA: (state, data) => {
        state.inputData = data;
    },
    SETHISTORYSELECTED: (state, data) => {
        state.selected = data;
    },
    SETHISTORYFIRSTLOAD: (state, data) => {
        state.firstLoad = data;
    },
    SETHISTORYUSERS: (state, data) => {
        state.UsersHistory = data;
    },
    SETHISTORYOTDELS: (state, data) => {
        state.OtdelsHistory = data;
    },
    SETHISTORYDATELIST: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryDateList = data;
    },
    SETHISTORYPARAMSBEGIN: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryParamsBegin = data;
    },
    SETHISTORYPARAMSEND: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryParamsEnd = data;
    },
    SETHISTORYFILTERUSERS: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryFilterUsers = data;
        localStorage.HistoryFilterUsers = data.toString();
    },
    SETHISTORYFILTERCLIENT: (state, data) => {
        state.HistoryFilterClient = data;
        if (data == null) {
            localStorage.HistoryFilterClient = '';
        } else {
            localStorage.HistoryFilterClient = data.toString();
        }
    },
    SETHISTORYFILTEROTDELS: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryFilterOtdels = data;
        localStorage.HistoryFilterOtdels = data.toString();
    },
    SETHISTORYFILTERDISPOSITION: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryFilterDispositon = data;
        localStorage.HistoryFilterDispositon = data.toString();
    },
    SETHISTORYFILTER: (state, data) => {
        if (data == null) {
            return
        }
        state.HistoryFilter= data;
        localStorage.HistoryFilter = data.toString();
    },
};

const actions = {
    GET_HISTORYUSERS: (context, data) => {
        Vue.http.get('data/getusers', {
            params: ''
        }
        ).then(response => {
            let usr = [];
            /*let valueToPush = new Object();
            valueToPush.otdel = '';
            valueToPush.id_group = '0';
            valueToPush.user_clid = 'Все сотрудники';
            usr.push(valueToPush);*/
            for (let value of response.body) {
                let valueToPush = new Object();
                valueToPush.otdel = value.otdel;
                valueToPush.id_group = value.id_group;
                valueToPush.user_clid = value.user_clid;
                usr.push(valueToPush);
            }
            context.commit('SETHISTORYUSERS', usr);
        }, () => {
            console.log(error);
        })
    },
    GET_HISTORYOTDELS: (context, data) => {
        Vue.http.get('data/getotdels', {
            params: ''
        }
        ).then(response => {
            let otd = [];
            /*otd.push('Все отделы');*/
            for (let value of response.body) {
                otd.push(value.otdel);
            }
            otd.push('IVR');
            otd.push('VoiceMail');
            otd.push('НетНомера');
            context.commit('SETHISTORYOTDELS', otd);
        }, () => {
            console.log(error);
        })
    },
    GET_HISTORYDATELIST: (context, payload) => {
        axios.get('data/getdateforhistory')
            .then(function (response) {
                // handle success
                context.commit('SETHISTORYDATELIST', response.data);
                if (state.HistoryParamsBegin.Name == null) {
                    let value = new Object()
                    value.Name = response.data[0].NameDate;
                    value.Date = response.data[0].BeginDate;
                    /*state.HistoryParamsBegin = value;*/
                    context.commit('SETHISTORYPARAMSBEGIN', value);
                }
                if (state.HistoryParamsEnd.Name == null) {
                    let value = new Object()
                    value.Name = response.data[0].NameDate;
                    value.Date = response.data[0].EndDate;
                    /*state.HistoryParamsEnd = value;*/
                    context.commit('SETHISTORYPARAMSEND', value);
                }
            })
            .catch(function (error) {
                // handle error
                console.log(error);
            })
            .then(function () {
                // always executed
            });
    },
    SET_HISTORYINPUTDATA: (context, data) => {
        context.commit('SETHISTORYINPUTDATA', data);
    },
    SET_HISTORYSELECTED: (context, data) => {
        context.commit('SETHISTORYSELECTED', data);
    },
    SET_HISTORYFIRSTLOAD: (context, data) => {
        context.commit('SETHISTORYFIRSTLOAD', data);
    },
    SET_HISTORYPARAMSBEGIN: (context, data) => {
        context.commit('SETHISTORYPARAMSBEGIN', data);
    },
    SET_HISTORYPARAMSEND: (context, data) => {
        context.commit('SETHISTORYPARAMSEND', data);
    },
    SET_HISTORYFILTEROTDELS: (context, data) => {
        context.commit('SETHISTORYFILTEROTDELS', data);
    },
    SET_HISTORYFILTERUSERS: (context, data) => {
        context.commit('SETHISTORYFILTERUSERS', data);
    },
    SET_HISTORYFILTERCLIENT: (context, data) => {
        context.commit('SETHISTORYFILTERCLIENT', data);
    },
    SET_HISTORYFILTERDISPOSITION: (context, data) => {
        context.commit('SETHISTORYFILTERDISPOSITION', data);
    },
    SET_HISTORYFILTER: (context, data) => {
        context.commit('SETHISTORYFILTER', data);
    },
};

export default {
    state,
    getters,
    mutations,
    actions,
};