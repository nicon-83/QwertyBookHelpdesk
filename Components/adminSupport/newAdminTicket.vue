<template>
  <div>
    <div style="display:flex;justify-content:space-between">
      <h3>Создание заявки</h3>
      <md-button
        type="button"
        class="md-icon-button md-primary md-theme-default"
        @click="closeTab(ADMINCURRENTTABID)"
      >
        <md-icon>close</md-icon>
        <md-tooltip md-direction="top" md-delay="500">Закрыть</md-tooltip>
      </md-button>
    </div>
    <form @submit.prevent="createTicket">
      <md-field md-clearable style="width:600px">
        <label for="title">Тема</label>
        <md-input type="text" placeholder="Введите тему" v-model.lazy="title" id="title"/>
      </md-field>
      <md-field style="width:180px">
        <label for="priority">Приоритет</label>
        <md-select
          placeholder="Приоритет"
          id="priority"
          name="priority"
          v-model="selectedPriority"
          md-dense
        >
          <md-option
            v-for="priority in PRIORITIES"
            :key="priority.id"
            :value="priority.name"
          >{{priority.name}}</md-option>
        </md-select>
      </md-field>
      <md-field md-clearable style="width:800px">
        <md-textarea
          style="resize:none;"
          id="message"
          placeholder="Текст сообщения"
          v-model.lazy="message"
          rows="5"
        ></md-textarea>
      </md-field>
      <md-button type="submit" class="md-dense md-raised md-primary md-theme-default">Создать</md-button>
    </form>
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import axios from "axios";

export default {
  props: [],
  components: {},
  name: "newTicket",
  data: () => ({
    selectedPriority: "обычный",
    title: "",
    message: ""
  }),
  methods: {
    createTicket() {
      axios
        .post(
          "data/CreateTicket",
          JSON.stringify({
            title: this.title,
            customer: this.CUSTOMER,
            state: "new",
            priority: this.npp,
            agent: "167",
            message: this.message
          }),
          { headers: { "Content-Type": "application/json" } }
        )
        .then(response => {
          // this.$store.dispatch('GET_TICKETS');

          //закрывем вкладку
          this.closeTab(this.ADMINCURRENTTABID);
        //   this.$emit("createAdminTicket"); //обновление списка заявок
          this.$store.dispatch("showSnackBar", { snackText: "Создана заявка" });
        })
        .catch(function(error) {
          console.log(error);
        })
        .then(() => {
          // always executed
        });
    },
    closeTab(tab_id) {
      this.$emit("closeAdminTab", tab_id);
      this.$store.dispatch("SET_ADMINHASCREATETICKETTAB", false); //делаем активной кнопку "Создать заявку"
    }
  },
  computed: {
    ...mapGetters(["LAST_NUMBER", "PRIORITIES", "CUSTOMER", "ADMINCURRENTTABID"]),
    npp: function() {
      switch (this.selectedPriority) {
        case "самый низкий":
          return "very_low";
        case "низкий":
          return "low";
        case "обычный":
          return "normal";
        case "высокий":
          return "hight";
        case "безотлагательный":
          return "very_hight";
      }
    }
  },
  mounted() {
    this.$store.dispatch("GET_PRIORITIES");

    this.$store.dispatch("setPageData", {
      header: "Новая заявка",
      status: "mounted",
      data: {}
    });
  }
};
</script>

<style lang="scss" scoped>
select {
  margin: 10px 0;
  padding: 5px;
  border-radius: 3px;
}

select:hover,
option:hover {
  cursor: pointer;
}

option {
  height: 20px;
}
</style>