<template>
    <div>
        <md-field>
            <label>{{ label }}</label>
            <md-input @click="open()" v-model="valueInput" readonly></md-input>
            <md-icon v-if="icon">{{ icon }}</md-icon>
        </md-field>
        <div class="wrapper" v-if="show">
            <div class="bg" @click="close()"></div>
            <div class="content">
                <div class="bg" @click="close()"></div>
                <md-card>
                    <md-card-header>
                        <span class="md-title">{{ label }}</span>
                    </md-card-header>
                    <md-card-content>
                        <md-table class="md-elevation-9" v-model="query" @md-selected="onSelect">
                            <md-table-empty-state :md-label="message.label"
                                                  :md-description="message.text">
                            </md-table-empty-state>

                            <md-table-row slot="md-table-row" slot-scope="{ item }" md-selectable="single">
                                <md-table-cell v-for="itemCol in col" :md-label="itemCol.label">{{ item[itemCol.name] }}</md-table-cell>
                            </md-table-row>

                            <div class="md-table-pagination" v-if="pagination">

                                <span>{{ currentPageMinCount }}-{{ currentPageMaxCount }} из {{ dataCount }}</span>

                                <md-button class="md-icon-button md-table-pagination-previous" @click="goToPrevious()" :disabled="currentPage === 1">
                                    <md-icon>keyboard_arrow_left</md-icon>
                                </md-button>

                                <md-button class="md-icon-button md-table-pagination-next" @click="goToNext()" :disabled="currentPage == lastPage">
                                    <md-icon>keyboard_arrow_right</md-icon>
                                </md-button>
                            </div>
                        </md-table>
                    </md-card-content>
                    <md-card-actions>
                        <md-button @click="close()" class="md-raised">Отмена</md-button>
                        <md-button @click="save()" class="md-raised md-primary">Выбрать</md-button>
                    </md-card-actions>
                    <div class="loading" v-if='loading'>
                        <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
                    </div>
                </md-card>
                <div class="bg" @click="close()"></div>
            </div>
            <div class="bg" @click="close()"></div>
        </div>
    </div>
    </div>
</template>

<script>
  export default {
    name: 'MdSelectDialog',
    props: {
      value: '',
      icon: {
        default: 'arrow_drop_down'
      },
      label: {
        default: 'Выпадающее меню'
      },
      valueCol: {
        default: ''
      },
      labelCol: {
        default: ''
      },
      url: {
        default: false
      },
      idName: {
        default: false
      },
      inputParams: {
        default: false
      },
      col: {
        default: []
      },
      pagination: {
        default: false
      }
    },
    data: () => ({
      show: false,
      loading: false,
      query: [],
      valueInput: '',
      message: {
        label: '',
        text: ''
      },
      selected: undefined,


      rowsPerPage: 10,
      currentPage: 1,
      numberPages: 10,
      pageSize: [10, 25, 50, 100],
    }),
    computed: {
      params() {
        return {
          ...this.searchParams,
          p1: (this.currentPage-1)*this.rowsPerPage + 1,
          p2: this.currentPage*this.rowsPerPage
        }
      },


      currentPageMinCount() {
        if (this.query.length > 0) {
          return this.query[0]['row_num'];
        } else {
          return 0;
        }
      },
      currentPageMaxCount() {
        if (this.query.length > 0) {
          return this.query[this.query.length - 1]['row_num'];
        } else {
          return 0;
        }
      },
      dataCount() {
        if (this.query.length > 0) {
          return this.query[0]['max_row'];
        } else {
          return 0;
        }
      },
      lastPage() {
        return parseInt(this.dataCount / this.rowsPerPage)
      },
    },
    methods: {
      onSelect(item) {
        this.selected = item;
      },
      close() {
        this.show = false;
        this.loading = false;
        this.query = [];
      },
      open() {
        this.show = true;
        //this.loading = true;
        this.downloadList();
      },
      save() {
        this.show = false;
        this.loading = false;
        this.valueInput = this.selected[this.labelCol];
        this.$emit('input', this.selected[this.valueCol])
      },

      downloadList() {
        this.message.label = 'Загрузка...';
        this.message.text = 'Идёт загрузка справочника';
        if (this.url) {
          this.$http.get(this.url, {
            params: this.params
          }).then(response => {
            this.message.label = 'Ничего не найденно';
            this.message.text = 'По данному запросу ничего не найденно.';
            this.loading = false;
            this.query = response.body;
          }, () => {
            this.loading = false;
            this.message.label = 'Ошибка!';
            this.message.text = 'При попытке обращения к api произошла ошибка, повторите ещё раз или обратитесь к администратору.';
            this.$store.dispatch('showSnackBar', {'snackText': 'Ошибка обращения к апи ' + this.label});
          })
        }
      },


      goToPrevious() {
        this.currentPage = this.currentPage - 1;
        this.downloadList()
      },
      goToNext() {
        this.currentPage = this.currentPage + 1;
        this.downloadList()
      }
    }
  }
</script>

<style scoped>
    .md-field {
        cursor: pointer;
    }

        .md-field input {
            cursor: pointer;
        }

    .loading {
        padding: calc(30% - 40px) calc(50% - 40px);
        width: 100%;
        height: 100%;
        position: absolute;
        z-index: 2;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        z-index: 99;
    }

    .wrapper {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        left: 0;
        z-index: 99;
        display: flex;
        flex-direction: column;
        height: 100vh;
        flex: 1;
    }

    .content {
        display: flex;
        flex-direction: row;
    }

    .bg {
        flex: 1;
    }

    .md-card {
        width: 90%;
        max-width: 920px;
        max-height: 80vh;
        padding: 20px;
    }
</style>
