const state = {
  tabs: [
    {
      id: 0,
      Name: "Заявки",
      Value: "tickets",
      Route: "tickets",
      Pagination: true
    },
    {
      id: 1,
      Name: "Архив",
      Value: "archive",
      Route: "archive",
      Pagination: false
    }
  ],
  openedTicketsTabs: [], //хранилище для номеров заявок которые открыты во вкладках, защита от дублей открытых вкладок
  countMutation: 0,
  CurrentTabId: 0,
  hasCreateTicketTab: false //индикатор наличия открытой вкладки создания новой заявки, должны быть открыта только одна вкладка создания новой заявки
};

const getters = {
  ADMINSUPPORTTABS: state => {
    return state.tabs;
  },
  ADMINSUPPORTCURRENTPAGINATION: state => param_id => {
    let idx = state.tabs.findIndex(x => x.Route === param_id); // param_id = this.$route.params.id
    if (idx == -1) {
      //Default if not found in route
      return false;
    }
    return state.tabs[idx].Pagination;
  },
  ADMINSUPPORTCURRENTTAB: state => param_id => {
    //определяем какой компонент будем загружать
    let idx = state.tabs.findIndex(x => x.Value === param_id); // param_id = this.$route.params.id
    if (idx == -1) {
      //Default if not found in route
      console.log(
        "Нет компонента в массиве вкладок, загружен компонент - NotFound"
      );
      return "NotFound";
    }
    return state.tabs[idx].Value;
  },
  ADMINSUPPORTCURRENTTABROUTE: state => {
    if (state.countMutation < 0) {
      return "never happen";
    }
    let route = "unknown";
    let storage = localStorage.AdminSupportTab;
    if (storage) {
      route = storage;
    }
    let idx = state.tabs.findIndex(x => x.Route === route);
    if (idx == -1) {
      route = state.tabs[0].Route;
    }
    return "/adminsupport/" + route;
  },
  ADMINCURRENTTABID: state => {
    return state.CurrentTabId;
  },
  ADMINHASCREATETICKETTAB: state => {
    return state.hasCreateTicketTab;
  },
  ADMINOPENEDTICKETSTABS: state => {
    return state.openedTicketsTabs;
  }
};

const mutations = {
  SETADMINSUPPORTCURRENTTABROUTE: (state, data) => {
    if (data == null) {
      return;
    }
    localStorage.AdminSupportTab = data.toString();
    state.countMutation++;
  },
  ADDADMINTAB: (state, data) => {
    state.tabs.push(data);
  },
  CLOSEADMINTAB: (state, data) => {
    state.tabs.splice(data, 1);
  },
  SETADMINCURRENTTABID: (state, data) => {
    state.CurrentTabId = data;
  },
  SETADMINHASCREATETICKETTAB: (state, data) => {
    state.hasCreateTicketTab = data;
  },
  ADDADMINOPENEDTICKETSTABS: (state, data) => {
    state.openedTicketsTabs.push(data);
  },
  REMOVEADMINOPENEDTICKETSTABS: (state, data) => {
    state.openedTicketsTabs.splice(data, 1);
  }
};

const actions = {
  SET_ADMINSUPPORTCURRENTTABROUTE: (context, data) => {
    context.commit("SETADMINSUPPORTCURRENTTABROUTE", data);
  },
  ADD_ADMIN_TABB: (context, data) => {
    context.commit("ADDADMINTAB", data);
  },
  CLOSE_ADMIN_TABB: (context, data) => {
    context.commit("CLOSEADMINTAB", data);
  },
  SET_ADMINCURRENTTABID: (context, data) => {
    context.commit("SETADMINCURRENTTABID", data);
  },
  SET_ADMINHASCREATETICKETTAB: (context, data) => {
    context.commit("SETADMINHASCREATETICKETTAB", data);
  },
  ADD_ADMINOPENEDTICKETSTABS: (context, data) => {
    context.commit("ADDADMINOPENEDTICKETSTABS", data);
  },
  REMOVE_ADMINOPENEDTICKETSTABS: (context, data) => {
    context.commit("REMOVEADMINOPENEDTICKETSTABS", data);
  }
};

export default {
  state,
  getters,
  mutations,
  actions
};
