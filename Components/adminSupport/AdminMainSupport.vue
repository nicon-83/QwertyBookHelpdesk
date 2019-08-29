<template>
  <drawer>
    <md-toolbar
      id="mainsupport-toolbar"
      class="md-dense"
      md-elevation="0"
      style="margin-left: 5px; padding: 0; background: transparent;"
    >
      <div class="md-toolbar-section-start">
        <md-tabs
          id="second-tabs-panel"
          :md-active-tab="activeTab"
          @md-changed="tabChanged"
          style="padding-left:0;"
        >
          <md-tab
            v-for="tab in tabs"
            :id="tab.Route"
            :md-label="tab.Name"
            :to="tab.Route"
            :key="tab.id"
          ></md-tab>
        </md-tabs>

        <div class="md-table-pagination">
          <div v-if="pagination" class="md-table-pagination" id="support-table-pagination">
            <span class="md-table-pagination-label">Строк на странице:</span>
            <md-field>
              <md-select
                @md-selected="updateRowsPerPage"
                v-model="rowsPerPage"
                md-dense
                md-class="md-pagination-select"
              >
                <md-option v-for="amount in pageSize" :key="amount" :value="amount">{{ amount }}</md-option>
              </md-select>
            </md-field>
            <span>{{ currentPageMinCount }}-{{ currentPageMaxCount }} из {{ currentDataCount }}</span>
            <md-button
              class="md-icon-button md-table-pagination-previous"
              @click="goToPrevious()"
              :disabled="currentPage === 1 || lastPage === 0 || loading"
            >
              <md-icon>keyboard_arrow_left</md-icon>
            </md-button>
            <md-button
              class="md-icon-button md-table-pagination-next"
              @click="goToNext()"
              :disabled="currentPage == lastPage || lastPage === 0 || loading"
            >
              <md-icon>keyboard_arrow_right</md-icon>
            </md-button>
          </div>
        </div>
      </div>
    </md-toolbar>

    <component
      v-bind:is="currentTabComponent"
      class="tab"
      @closeAdminTab="closeTab"
    ></component>
  </drawer>
</template>

<script>
import Drawer from "../common/Drawer.vue";
import Tickets from "./tickets.vue";
import Archive from "./archive.vue";
import newAdminTicket from "./newAdminTicket.vue";
import amessages from "./amessages.vue";
import NotFound from "../views/NotFound.vue";
import { mapGetters } from "vuex";

export default {
  components: {
    Drawer,
    Tickets,
    Archive,
    newAdminTicket,
    amessages,
    NotFound
  },
  name: "AdminMainSupport",
  props: {},
  data: () => ({
    rowsPerPageOld: 5,
    rowsPerPage: 5,
    currentPageMinCount: 0,
    currentPageMaxCount: 0,
    currentDataCount: 0,
    currentPage: 1,
    pageSize: [5, 10, 15],
    loading: false,
    url_data: "data/getticketspagination"
  }),
  watch: {},
  computed: {
    ...mapGetters([
      "ADMINSUPPORTTABS",
      "ADMINSUPPORTCURRENTTAB",
      "ADMINCURRENTTABID",
	  "ADMINSUPPORTCURRENTTABROUTE",
	  "ADMINHASCREATETICKETTAB",
	  "ADMINOPENEDTICKETSTABS",
      "LAST_NUMBER",
      "TICKETS",
      "TICKETSINPUTDATA"
    ]),
    tabs: {
      get() {
        return this.ADMINSUPPORTTABS;
      }
    },
    currentTab: {
      get() {
        return this.$store.getters["ADMINSUPPORTCURRENTTAB"](this.$route.params.id); //определяем компонент который надо загрузить
      }
    },
    pagination: {
      get() {
        return this.$store.getters["ADMINSUPPORTCURRENTPAGINATION"](
          this.$route.params.id
        );
      }
    },
    currentTabComponent() {
      return this.currentTab;
    },
    activeTab() {
      let arr = this.ADMINSUPPORTCURRENTTABROUTE.split("/");
      return arr[arr.length - 1];
    },
    lastPage() {
      let cnt = this.currentDataCount;
      let i = parseInt(cnt / this.rowsPerPage);
      if (i * this.rowsPerPage < cnt) {
        i++;
      }
      return parseInt(i);
    },
    paramsData() {
      let rowsPerPage = this.getRowsPerPage();
      let currentPage = this.getCurrentPage();
      return {
        row_num_begin: (currentPage - 1) * rowsPerPage + 1,
        row_num_end: currentPage * rowsPerPage
      };
    }
  },

  methods: {
    closeTab(tab_id) {
      for (let i = 0; i < this.ADMINSUPPORTTABS.length; i++) {
        if (this.ADMINSUPPORTTABS[i].id === tab_id) {
          this.$store.dispatch("CLOSE_ADMIN_TABB", i);
        }
      }
      this.$store.dispatch("SET_ADMINSUPPORTCURRENTTABROUTE", "tickets");
      this.$router.push({ path: "/adminsupport/tickets" }); //переходим на вкладку с заявками
    },
    tabChanged(value) {
      //value это атрибут :id в md-tab
      let idx = this.tabs.findIndex(x => x.Route === value);
      if (idx == -1) {
        idx = 0;
      }
      this.$store.dispatch("SET_ADMINSUPPORTCURRENTTABROUTE", this.tabs[idx].Route);
      this.$store.dispatch("SET_ADMINCURRENTTABID", this.tabs[idx].id);
    },
    update() {
      //   console.log("MainSupport update()");
      this.$root.$emit("ADMINSUPPORTUPDATE");
      //   console.log("MainSupport update()");
      //   console.log("rowsPerPage = " + this.rowsPerPage);
      //   console.log("currentPage = " + this.currentPage);
      //   console.log("lastPage = " + this.lastPage);
    },
    updateRowsPerPage() {
      if (this.rowsPerPageOld == this.rowsPerPage) {
        return;
      }
	  this.rowsPerPageOld = this.rowsPerPage;
	  
	  //нужно настроить сохранение rowsPerPage при навигации с внешней панели вкладок
      //   if (this.ROWSPERPAGEOLD == this.rowsPerPage) {
      //     return;
      //   }
      //   localStorage.ticketsRowsPerPage = this.rowsPerPage;
      //   this.$store.dispatch("SET_ROWSPERPAGEOLD", this.rowsPerPage);

      this.currentPage = 1;
      this.update();
    },
    goToPage(delta) {
      this.currentPage = this.currentPage + delta;
      this.update();
    },
    goToPrevious() {
      this.goToPage(-1);
    },
    goToNext() {
      this.goToPage(1);
    },
    setCountPagination(value) {
      this.currentPageMinCount = value[0];
      this.currentPageMaxCount = value[1];
      this.currentDataCount = value[2];
    },
    getRowsPerPage() {
      return this.rowsPerPage;
    },
    getCurrentPage() {
      return this.currentPage;
    },
    setCurrentPage(value) {
      this.currentPage = value;
    },
    getLoading() {
      return this.loading;
    },
    setLoading(value) {
      this.loading = value;
    }
  },
  provide: function() {
    return {
      setCountPagination: this.setCountPagination,
      getRowsPerPage: this.getRowsPerPage,
      getCurrentPage: this.getCurrentPage,
      setCurrentPage: this.setCurrentPage,
      getLoading: this.getLoading,
      setLoading: this.setLoading
    };
  },
  created() {
    //console.log('created MainSupport');
  },
  mounted() {
  },
  beforeRouteEnter(to, from, next) {
    // вызывается до подтверждения пути, соответствующего этому компоненту.
    // НЕ ИМЕЕТ доступа к контексту экземпляра компонента `this`,
    // так как к моменту вызова экземпляр ещё не создан!

    //console.log('beforeRouteEnter');

    //передаем callback в next для доступа к контексту экземпляра компонента `this`
    next(vm => {
      // экземпляр компонента доступен как `vm`

      //добавляем и открываем вкладку "Новая заявка"
      if (vm.$route.params.id === "newAdminTicket" && !vm.ADMINHASCREATETICKETTAB) {
        vm.$store.dispatch("ADD_ADMIN_TABB", {
          id: vm.LAST_NUMBER,
          Name: "Новая заявка",
          Value: "newAdminTicket",
          Route: "newAdminTicket"
        });
        vm.$store.dispatch("INCREASE_LAST_NUMBER");
        vm.$store.dispatch("SET_ADMINHASCREATETICKETTAB", true);
        vm.$store.dispatch("SET_ADMINSUPPORTCURRENTTABROUTE", "newAdminTicket");
        vm.$router.push({ path: "/adminsupport/newAdminTicket" });
      }

      //добавляем и открываем вкладку с содержанием заявки
      if (vm.$route.params.id === "messages" && vm.ADMINOPENEDTICKETSTABS.length == 0) {
        if (vm.TICKETS.length == 0) {
          //признак того что переход на страницу выполнен напрямую по ссылке

          vm.$http.get("data/gettickets").then(function(response) {
            let tickets = response.body;
            let num = vm.$route.query.tn;

            for (let ticket of tickets) {
              if (ticket.number == num) {
                //проверяем что заявка с запрошенным номером существует
                vm.$store.dispatch("ADD_TABB", {
                  id: vm.LAST_NUMBER,
                  Name: "Заявка " + num,
                  Value: "messages",
                  Route: "messages?tn=" + num
                });
                vm.$store.dispatch("INCREASE_LAST_NUMBER");
                vm.$store.dispatch("ADD_OPENEDTICKETSTABS", num);
                vm.$store.dispatch(
                  "SET_SUPPORTCURRENTTABROUTE",
                  "messages?tn=" + num
                );
                vm.$router.push({
                  path: "/support/messages",
                  query: { tn: num }
                });
              }
            }
          });
        }
      }
    });
  },
  beforeRouteUpdate(toR, fromR, next) {
    //console.log('beforeRouteUpdate MainSupport');

    // вызывается когда маршрут, что рендерит этот компонент изменился,
    // но этот компонент будет повторно использован в новом маршруте.
    // Например, для маршрута с динамическими параметрами `/foo/:id`, когда мы
    // перемещаемся между `/foo/1` и `/foo/2`, экземпляр того же компонента `Foo`
    // будет использован повторно, и этот хук будет вызван когда это случится.
    // Также имеется доступ в `this` к экземпляру компонента.

    //console.log('beforeRouteUpdate');
    next();
  }
};
</script>

<style lang="scss" scoped>
.tab {
  border: 1px solid #ccc;
  padding: 10px;
}

.md-tab {
  padding: 0px;
  margin: 0;
}

.md-table-pagination {
  border-top: 0;
  height: 48px;
}
</style>