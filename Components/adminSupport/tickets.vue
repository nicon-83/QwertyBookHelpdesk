<template>
  <div>
    <div class="md-content">
      <div id="create-ticket-button" style="margin-bottom: 10px">
        <md-button
          class="md-dense md-raised md-primary md-theme-default"
          @click="createTicket"
          :disabled="ADMINHASCREATETICKETTAB"
        >Создать заявку</md-button>
      </div>

      <md-table :md-height.sync="tableHeight" v-model="localInputData" md-card md-fixed-header>
        <md-table-empty-state md-label="Заявок не найдено" md-description></md-table-empty-state>

        <md-table-row slot="md-table-row" slot-scope="{ item }">
          <md-table-cell md-label="Номер" md-numeric>{{ item.number }}</md-table-cell>
          <md-table-cell md-label="Тема">
            <md-button
              class="md-primary md-theme-default md-dense"
              href="#"
              @click.prevent="openTicket(item.number)"
              :disabled="ADMINOPENEDTICKETSTABS.indexOf(item.number) !== -1"
            >{{ item.title }}</md-button>
          </md-table-cell>
          <md-table-cell md-label="Приоритет">{{ item.priority }}</md-table-cell>
          <md-table-cell md-label="Статус">{{ item.state }}</md-table-cell>
          <md-table-cell md-label="Исполнитель">{{ item.agent }}</md-table-cell>
          <md-table-cell md-label="Дата создания">{{ item.createTime }}</md-table-cell>
        </md-table-row>
      </md-table>
    </div>
  </div>
</template>

<script>
import { mapGetters } from "vuex";

export default {
  components: {},
  name: "Tickets",
  inject: [
    "setCountPagination",
    "getRowsPerPage",
    "getCurrentPage",
    "setCurrentPage",
    "getLoading",
    "setLoading"
  ],
  data: () => ({
    url_data: "data/getticketspagination",
    tableHeight: 700, //сделал обычным свойством, т.к. вычисляемое свойство глючит при переходе с внешней панели вкладок
    localInputData: []
  }),
  methods: {
    createTicket() {
      this.$store.dispatch("ADD_ADMIN_TABB", {
        id: this.LAST_NUMBER,
        Name: "Новая заявка",
        Value: "newAdminTicket",
        Route: "newAdminTicket",
        Pagination: false
      });
      this.$store.dispatch("INCREASE_LAST_NUMBER");
      this.$store.dispatch("SET_ADMINHASCREATETICKETTAB", true);
      this.$store.dispatch("SET_ADMINSUPPORTCURRENTTABROUTE", "newAdminTicket");
      this.$router.push({ path: "/adminsupport/newAdminTicket" });
    },
    openTicket(num) {
      this.$store.dispatch("ADD_ADMIN_TABB", {
        id: this.LAST_NUMBER,
        Name: "Заявка " + num,
        Value: "amessages",
        Route: "amessages?tn=" + num,
        Pagination: false
      });
      this.$store.dispatch("INCREASE_LAST_NUMBER");
      this.$store.dispatch("ADD_ADMINOPENEDTICKETSTABS", num);
      this.$store.dispatch("SET_ADMINSUPPORTCURRENTTABROUTE", "amessages?tn=" + num);
      this.$router.push({ path: "/adminsupport/amessages", query: { tn: num } });
    },
    setParentCountPagination() {
      let row_min = 0;
      let data_count = 0;
      let row_max = 0;
      let ln = this.localInputData.length;
      if (ln > 0) {
        row_min = this.localInputData[0]["rowNum"];
        data_count = this.localInputData[0]["maxRow"];
        row_max = this.localInputData[ln - 1]["rowNum"];
        this.setCountPagination([row_min, row_max, data_count]);
      }
    },
    update() {
      //   console.log("tickets update()");
      //   console.log("rowsPerPage = " + this.getRowsPerPage());
      //   console.log("currentPage = " + this.getCurrentPage());
      this.loading = true;
      this.$http
        .get(this.url_data, {
          params: this.paramsData
        })
        .then(
          response => {
            this.localInputData = Array.from(response.body);
          },
          () => {
            store.dispatch("showSnackBar", {
              snackText: "Ошибка получения данных списка заявок"
            });
          }
        )
        .finally(() => {
          this.setParentCountPagination();
          this.loading = false;
        });
    }
  },
  computed: {
    ...mapGetters([
      "LAST_NUMBER",
      "TICKETS",
      "ADMINHASCREATETICKETTAB",
      "ADMINOPENEDTICKETSTABS",
      "ADMINSUPPORTTABS",
      "ADMINTICKETSINPUTDATA", //для пагинации
      "ADMINTICKETSFIRSTLOAD" //для пагинации
    ]),
    loading: {
      get() {
        return this.getLoading();
      },
      set(value) {
        this.setLoading(value);
      }
    },
    paramsData() {
      let rowsPerPage = this.getRowsPerPage();
      let currentPage = this.getCurrentPage();
      return {
        row_num_begin: (currentPage - 1) * rowsPerPage + 1,
        row_num_end: currentPage * rowsPerPage
      };
    }
    //tableHeight: {
    //	get() { return this.$store.state.app.tableHeight; },
    //	set(value) { this.$store.dispatch('setApp', { 'tableHeight': value }); }
    //}
  },
  created() {
    //console.log('created tickets');
    //console.log('getRowsPerPage = ' + this.getRowsPerPage());
    //console.log('getCurrentPage = ' + this.getCurrentPage());
  },
  updated() {
    //console.log('updated tickets');
  },
  beforeUpdate() {
    //console.log('beforeUpdate tickets');
  },
  mounted() {
    // console.log('mounted tickets');
    // console.log(this.TICKETSINPUTDATA);

    //устанавливаем данные для пагинации
    if (this.ADMINTICKETSFIRSTLOAD === true) {
      this.$store.dispatch("SET_ADMINTICKETSFIRSTLOAD", false);
      this.setCurrentPage(1);
      this.update();
    } else {
      this.localInputData = this.ADMINTICKETSINPUTDATA;
      this.setParentCountPagination();
    }
    this.$root.$on("ADMINSUPPORTUPDATE", this.update);

    //расчет высоты таблицы
    let pageHeight = document.documentElement.clientHeight;
    let toolbarHeight = document.getElementById("application-toolbar")
      .clientHeight;
    let tabsPanel = document.getElementById("second-tabs-panel");
    let tabsPanelHeight = tabsPanel.clientHeight ? tabsPanel.clientHeight : 48;
    let createButtonHeight = document.getElementById("create-ticket-button")
      .clientHeight;
    let tableHead = document.querySelector(".md-table-fixed-header");
    let tableHeadheight = tableHead.clientHeight ? tableHead.clientHeight : 56;
    let paddingMarginHeight = 90;

    //console.log('pageHeight = ' + pageHeight);
    //console.log('toolbarHeight = ' + toolbarHeight);
    //console.log('tabsPanelHeight = ' + tabsPanelHeight);
    //console.log('createButtonHeight = ' + createButtonHeight);
    //console.log('paddingMarginHeight = ' + paddingMarginHeight);
    //console.log('tableHeadheight = ' + tableHeadheight);
    //console.log('Итого = '  + (pageHeight - toolbarHeight - tabsPanelHeight - createButtonHeight - paddingMarginHeight));

    this.$store.dispatch("setApp", {
      tableHeight:
        pageHeight -
        toolbarHeight -
        tabsPanelHeight -
        createButtonHeight -
        tableHeadheight -
        paddingMarginHeight
    });
    this.tableHeight = this.$store.state.app.tableHeight;
    //this.tableHeight = pageHeight - toolbarHeight - tabsPanelHeight - createButtonHeight - paddingMarginHeight;

    // this.$store.dispatch("GET_TICKETS");

    //информация в заголовке окна
    this.$store.dispatch("setPageData", {
      header: "Список заявок",
      status: "mounted",
      data: {}
    });
  },
  beforeDestroy: function() {
    // console.log("tickets beforeDestroy");
    this.$root.$off("ADMINSUPPORTUPDATE", this.update);
    this.$store.dispatch("SET_ADMINTICKETSINPUTDATA", this.localInputData);
  },
  beforeRouteUpdate(to, from, next) {
    //console.log('beforeRouteUpdate');
  },
  watch: {}
};
</script>

<style lang="scss" scoped>
.md-content {
  padding: 10px;
}

.md-table {
  margin: 0;
}

a:hover {
  cursor: pointer;
}

button {
  padding: 5px;
}

/*a:hover {
		background-color: #EBEBEB;
	}*/
</style>