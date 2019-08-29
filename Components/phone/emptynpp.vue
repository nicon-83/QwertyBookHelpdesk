<template>
    <div>
        <div style="height: 5px; max-height: 5px;">
            <md-progress-bar v-if="loading" md-mode="query"></md-progress-bar>
        </div>
        <md-table class="md-table-History" :md-height.sync="tableHeight" v-model="inputData" md-card md-fixed-header :md-selected-value.sync="selected">
            <md-table-toolbar id="history-table-toolbar" class="md-dense">
                <div class="md-toolbar-section-start">
                    <div class="md-layout md-gutter" style="margin-top: 4px;">
                        <div class="md-layout-item md-size-15">
                            <md-datepicker class="md-datepicker-history" v-model="paramsBeginDate" md-immediately>
                                <!--:md-override-native="false" глючит под FireFox-->
                                <label>Начало</label>
                            </md-datepicker>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-datepicker class="md-datepicker-history" v-model="paramsEndDate" md-immediately>
                                <label>Конец</label>
                            </md-datepicker>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-field>
                                <label for="FilterDisposition">Вызов</label>
                                <md-select placeholder="Все вызовы" v-model="fltDisposition" name="FilterDisposition" id="FilterDisposition" md-dense>
                                    <md-option v-for="item in DISPOSITIONHISTORY" v-bind:value="item.DISPOSITION">
                                        {{item.STATUS}}
                                    </md-option>
                                </md-select>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-field>
                                <label for="FilterOtdels">Отдел</label>
                                <md-select placeholder="Выберите отдел" v-model="fltOtdels" name="FilterOtdels" id="FilterOtdels" md-dense>
                                    <md-option v-for="Otdel in HISTORYOTDELS" v-bind:value="Otdel">
                                        {{Otdel}}
                                    </md-option>
                                </md-select>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-field>
                                <label for="FilterUsers">Сотрудник</label>
                                <md-select placeholder="Выберите сотрудника" v-model="fltUsers" name="FilterUsers" id="FilterUsers" md-dense>
                                    <md-option v-for="User in HISTORYUSERS" v-bind:value="User.user_clid">
                                        {{User.user_clid}}
                                    </md-option>
                                </md-select>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-20">
                            <md-field md-clearable>
                                <label for="FilterClient">Клиент/телефон</label>
                                <md-input placeholder="Клиент или телефон" v-model="fltClient" name="FilterClient" id="FilterClient" md-dense>
                                </md-input>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-5" style="padding-left: 0px; padding-top: 16px;">
                            <md-button class="md-icon-button" id="history-update-button" @click="update()" :disabled="loading">
                                <md-icon>refresh</md-icon>
                                <md-tooltip md-direction="top" md-delay="500">Обновить данные</md-tooltip>
                            </md-button>
                        </div>
                    </div>
                </div>
            </md-table-toolbar>

            <md-table-empty-state md-label="Ничего не найдено" v-if="!loading"
                                  :md-description="notFound">
            </md-table-empty-state>

            <md-table-toolbar slot="md-table-alternate-header" slot-scope="{ count }">
                <div class="md-toolbar-section-start">Выбрано: {{ count }}</div>
                <div class="md-toolbar-section-end">
                    <md-button class="md-primary md-raised" @click="execShowDialog()">Показать весь маршрут вызова</md-button>
                </div>
            </md-table-toolbar>

            <md-table-row slot="md-table-row" slot-scope="{ item }" md-selectable="multiple">
                <md-table-cell md-label="№" md-numeric>{{ item.row_num }}</md-table-cell>
                <md-table-cell md-label="Дата">{{ item.calldate }}</md-table-cell>
                <md-table-cell md-label="Отдел">{{ item.otdel }}</md-table-cell>
                <md-table-cell md-label="Сотрудник">{{ item.user_clid }}</md-table-cell>
                <md-table-cell md-label="Источник">{{ item.src }}</md-table-cell>
                <md-table-cell md-label="Назначение">{{ item.dst }}</md-table-cell>
                <md-table-cell md-label="Клиент">{{ item.clid }}</md-table-cell>
                <md-table-cell md-label="Вызов">{{ item.disposition }}</md-table-cell>
                <md-table-cell md-label="Сек." md-numeric>{{ item.duration }}</md-table-cell>
            </md-table-row>
        </md-table>

        <div class="md-table-pagination" id="table-pagination" v-if="lastPage > 0" style="border-top-color: rgba(0, 0, 0, 0.13); height: 32px;">
            <md-switch v-model="boolean">Accent <small>(Default)</small></md-switch>
            <md-radio v-model="fltState" class="md-accent" value="INUSE" @change="updateFilterOtdel"><small>Занят</small></md-radio>
            <span class="md-table-pagination-label">Строк на странице:</span>
            <md-field>
                <md-select @md-selected="updateRowsPerPage" v-model="rowsPerPage" md-dense md-class="md-pagination-select">
                    <md-option v-for="amount in pageSize" :key="amount" :value="amount">{{ amount }}</md-option>
                </md-select>
            </md-field>
            <span>{{ currentPageMinCount }}-{{ currentPageMaxCount }} из {{ dataCount }}</span>
            <md-button class="md-icon-button md-table-pagination-previous" @click="goToPrevious()" :disabled="currentPage === 1 || lastPage === 0 || loading">
                <md-icon>keyboard_arrow_left</md-icon>
            </md-button>
            <md-button class="md-icon-button md-table-pagination-next" @click="goToNext()" :disabled="currentPage == lastPage || lastPage === 0 || loading">
                <md-icon>keyboard_arrow_right</md-icon>
            </md-button>
        </div>

        <md-dialog class="history-dialog" :md-active.sync="showDialog" @md-closed="finishDialog">
            <md-dialog-title style="margin: 0; padding-top: 10px;">Маршрут вызова</md-dialog-title>
            <md-dialog-content style="padding: 10px;">
                <md-table class="dialog-table" :md-height="100" v-model="selected" md-card md-fixed-header>
                    <md-table-row slot="md-table-row" slot-scope="{ item }">
                        <md-table-cell md-label="ID">{{ item.uniqueid }}</md-table-cell>
                        <md-table-cell md-label="Клиент">{{ item.clid }}</md-table-cell>
                        <md-table-cell md-label="Код клиента">{{ item.npp }}</md-table-cell>
                    </md-table-row>
                </md-table>
                <div style="height: 10px;"></div>
                <md-table class="dialog-table" :md-height="180" v-model="inputDataUniqueid" md-card md-fixed-header>
                    <md-table-row slot="md-table-row" slot-scope="{ item }">
                        <md-table-cell md-label="№" md-numeric>{{ item.row_num }}</md-table-cell>
                        <md-table-cell md-label="ID">{{ item.uniqueid }}</md-table-cell>
                        <md-table-cell md-label="Время">{{ item.calldate }}</md-table-cell>
                        <md-table-cell md-label="Отдел">{{ item.otdel }}</md-table-cell>
                        <md-table-cell md-label="Сотрудник">{{ item.user_clid }}</md-table-cell>
                        <md-table-cell md-label="Источник">{{ item.src }}</md-table-cell>
                        <md-table-cell md-label="Назначение">{{ item.dst }}</md-table-cell>
                        <md-table-cell md-label="Вызов">{{ item.disposition }}</md-table-cell>
                        <md-table-cell md-label="Сек." md-numeric>{{ item.duration }}</md-table-cell>
                    </md-table-row>
                </md-table>
            </md-dialog-content>
            <md-dialog-actions>
                <md-button class="md-primary md-raised" @click="showDialog=false">Закрыть</md-button>
            </md-dialog-actions>
        </md-dialog>

    </div>
</template>

<script>

    import Drawer from '../common/Drawer.vue'
    import { mapGetters } from 'vuex'

    export default {
        components: {
            Drawer
        },
        name: 'EmptyNpp',
        props: {
        },
        data: () => ({
            loading: false,
            showDialog: false,
            url_date_for_history: 'data/getdateforhistory',
            url_data: 'data/gethistory',
            url_data_uniqueid: 'data/gethistoryuniqueid',
            createdate: Date,
            rowsPerPageOld: 100,
            rowsPerPage: 100,
            currentPage: 1,
            numberPages: 10,
            pageSize: [100, 200, 300],
            inputData: [//Костыль для правильной работы первоначального выбора в таблице
                { field1: "abrakadabra"}
            ],
            selected: [],
            inputDataUniqueid: [],
        }),
        methods: {
            checkParams() {
                if (this.HISTORYPARAMSBEGIN.Date == null) {
                    this.$store.dispatch('showSnackBar', { 'snackText': 'Введите дату начала периода!' });
                    return false;
                }
                if (this.HISTORYPARAMSEND.Date == null) {
                    this.$store.dispatch('showSnackBar', { 'snackText': 'Введите дату конца периода!' });
                    return false;
                }
                /*
                if (this.HISTORYPARAMSBEGIN.Date > this.HISTORYPARAMSEND.Date) {
                    this.$store.dispatch('showSnackBar', { 'snackText': 'Дата начала больше даты конца периода! \r\n' + this.HISTORYPARAMSBEGIN.Date.toLocaleString("ru") + ' > ' + this.HISTORYPARAMSEND.Date.toLocaleString("ru")});
                    return false;
                }*/
                return true;
            },
            updateRowsPerPage() {
                if (this.rowsPerPageOld == this.rowsPerPage) {
                    return;
                }
                this.rowsPerPageOld = this.rowsPerPage;
                this.update();
            },
            update() {
                if (!this.checkParams()) {
                    return;
                }
                this.loading = true;
                let store = this.$store;
                this.$http.get(this.url_data, {
                    params: this.paramsData
                }).then(response => {
                    this.inputData = Array.from(response.body);
                    this.createdate = new Date();
                }, () => {
                        store.dispatch('showSnackBar', { 'snackText': 'Ошибка получения данных истории звонков' });
                    }).finally(() => { this.loading = false; });
            },
            goToPage(delta) {
                if (this.selected.length > 0) {
                    this.selected = []; //очищаем текущий выбор при переходе на след.страницу
                }
                this.currentPage = this.currentPage + delta;
                this.update()
            },
            goToPrevious() {
                this.goToPage(-1);
            },
            goToNext() {
                this.goToPage(1);
            },
            execShowDialog() {
                if (this.selected.length > 5) {
                    this.$store.dispatch('showSnackBar', { 'snackText': 'Выберите не более 5 строк!' });
                    return;
                }
                this.loading = true;
                let store = this.$store;
                this.$http.get(this.url_data_uniqueid, {
                    params: this.paramsDataUniqueid
                }).then(response => {
                    this.loading = false;
                    this.inputDataUniqueid = Array.from(response.body);
                }, () => {
                    this.loading = false;
                    store.dispatch('showSnackBar', { 'snackText': 'Ошибка получения данных маршрута вызова' });
                });
                this.showDialog = true;
            },
            finishDialog() {
                //Need Input Code
            },
        },
        created() {
            this.createdate = new Date();
            /*this.$store.dispatch('GET_HISTORYDATELIST', {});*/
            this.$store.dispatch('GET_HISTORYOTDELS', {});
            this.$store.dispatch('GET_HISTORYUSERS', {});
            this.$store.dispatch('setPageData', {
                'header': 'Неизвестный клиент',
                'status': 'mounted',
                'data': {}
            });
        },
        mounted: function () {
            this.update();
        },
        computed: {
            ...mapGetters(['DISPOSITIONHISTORY', 'HISTORYFILTERDISPOSITION', 'HISTORYDATELIST', 'HISTORYOTDELS', 'HISTORYUSERS', 'HISTORYFILTEROTDELS', 'HISTORYFILTERUSERS', 'HISTORYFILTERCLIENT', 'HISTORYPARAMSBEGIN', 'HISTORYPARAMSEND', 'HISTORYPARAMSBEGIN_JSON', 'HISTORYPARAMSEND_JSON']),
            fltDisposition: {
                get() {
                    return this.HISTORYFILTERDISPOSITION
                },
                set(value) {
                    this.$store.dispatch('SET_HISTORYFILTERDISPOSITION', value);
                }
            },
            fltOtdels: {
                get() {
                    return this.HISTORYFILTEROTDELS
                },
                set(value) {
                    this.$store.dispatch('SET_HISTORYFILTEROTDELS', value);
                }
            },
            fltUsers: {
                get() {
                    return this.HISTORYFILTERUSERS
                },
                set(value) {
                    this.$store.dispatch('SET_HISTORYFILTERUSERS', value);
                }
            },
            fltClient: {
                get() {
                    return this.HISTORYFILTERCLIENT
                },
                set(value) {
                    this.$store.dispatch('SET_HISTORYFILTERCLIENT', value);
                }
            },
            paramsBeginDate: {
                get() {
                    return this.HISTORYPARAMSBEGIN.Date;
                },
                set(value) {
                    let val = new Object();
                    val.Name = 'BeginDate';
                    val.Date = value;
                    this.$store.dispatch('SET_HISTORYPARAMSBEGIN', val)
                }
            },
            paramsEndDate: {
                get() {
                    return this.HISTORYPARAMSEND.Date;
                },
                set(value) {
                    let val = new Object();
                    val.Name = 'EndDate';
                    val.Date = value;
                    this.$store.dispatch('SET_HISTORYPARAMSEND', val)
                }
            },
            paramsBeginName: {
                get() {
                    return this.HISTORYPARAMSBEGIN.Name;
                },
                set(value) {
                    let val = new Object();
                    val.Name = value;
                    let idx = this.HISTORYDATELIST.findIndex(x => x.NameDate === value);
                    if (idx != -1) {
                        val.Date = this.HISTORYDATELIST[idx].BeginDate;
                    }
                    this.$store.dispatch('SET_HISTORYPARAMSBEGIN', val)
                }
            },
            paramsEndName: {
                get() {
                    return this.HISTORYPARAMSEND.Name;
                },
                set(value) {
                    let val = new Object();
                    val.Name = value;
                    let idx = this.HISTORYDATELIST.findIndex(x => x.NameDate === value);
                    if (idx != -1) {
                        val.Date = this.HISTORYDATELIST[idx].EndDate;
                    }
                    this.$store.dispatch('SET_HISTORYPARAMSEND', val)
                }
            },
            tableHeight() {
                let delta = 32;
                //var elem = document.getElementById("table-pagination");
                //if (elem != null) {
                //    delta = elem.clientHeight;
                //}
                return this.$store.state.app.tableHeight - delta;
            },
            paramsData() {
                return {
                    xpName: 'xp_History_EmptyNpp_Json',
                    DateBegin: this.HISTORYPARAMSBEGIN.Date ? this.HISTORYPARAMSBEGIN_JSON : undefined,
                    DateEnd: this.HISTORYPARAMSEND.Date ? this.HISTORYPARAMSEND_JSON : undefined,
                    Disposition: this.HISTORYFILTERDISPOSITION ? this.HISTORYFILTERDISPOSITION : undefined,
                    Otdel: this.HISTORYFILTEROTDELS ? this.HISTORYFILTEROTDELS : undefined,
                    User: this.HISTORYFILTERUSERS ? this.HISTORYFILTERUSERS : undefined,
                    Client: this.HISTORYFILTERCLIENT ? this.HISTORYFILTERCLIENT : undefined,
                    row_num_begin: (this.currentPage - 1) * this.rowsPerPage + 1,
                    row_num_end: (this.currentPage) * this.rowsPerPage,
                }
            },
            paramsDataUniqueid() {
                return {
                    uniqueid1: this.selected.length >= 1 ? this.selected[0].uniqueid : '-',
                    uniqueid2: this.selected.length >= 2 ? this.selected[1].uniqueid : '-',
                    uniqueid3: this.selected.length >= 3 ? this.selected[2].uniqueid : '-',
                    uniqueid4: this.selected.length >= 4 ? this.selected[3].uniqueid : '-',
                    uniqueid5: this.selected.length >= 5 ? this.selected[4].uniqueid : '-',
                }
            },
            currentPageMinCount() {
                if (this.inputData.length > 0) {
                    return this.inputData[0]['row_num'];
                } else {
                    return 0;
                }
            },
            currentPageMaxCount() {
                if (this.inputData.length > 0) {
                    return this.inputData[this.inputData.length - 1]['row_num'];
                } else {
                    return 0;
                }
            },
            dataCount() {
                if (this.inputData.length > 0) {
                    return this.inputData[0]['max_row'];
                } else {
                    return 0;
                }
            },
            lastPage() {
                let cnt = this.dataCount;
                let i = parseInt(cnt / this.rowsPerPage);
                if (i * this.rowsPerPage < cnt) {
                    i++;
                }
                return parseInt(i)
            },
            page() {
                return this.$store.state.page;
            },
            notFound() {
                let val = this.paramsData;
                let disposition = '';
                if (val.Disposition != 'ALL') {
                    disposition = ' Вызов: ' + val.Disposition
                }
                let otdel = ''
                if (val.Otdel != 'Все отделы') {
                    otdel = ' Отдел: ' + val.Otdel
                }
                let user = '';
                if (val.User != 'Все сотрудники') {
                    user = ' Сотрудник: ' + val.User
                }
                let client = val.Client ? ' Клиент: ' + val.Client : '';
                return 'За период ' + val.DateBegin.toLocaleString("ru").substring(0, 10) + ' - ' + val.DateEnd.toLocaleString("ru").substring(0, 10)
                    + disposition
                    + otdel
                    + user
                    + client
                    + ' ничего не найдено.';
            }
        }
    }
</script>

<style lang="scss">
    /*@import "~vue-material/dist/theme/engine";*/

    .md-datepicker-history {
        .md-datepicker-panel

    {
        max-height: 230px; /*Add Serg для исправления глюка при открытия календаря, style lang="scss" должен быть быть scope!!!*/
    }

    }

    .history-dialog {
        /*width: 400px;*/
    }

    .md-table-History {
        margin: 0 !important;
        /*.md-table-head-container {*/
        /*background: md-get-palette-color(grey, 200);*/
        /*    height: 32px !important;
            padding: 0 0 !important;
        }*/
        .md-table-cell

    {
        height: 14px;
        position: relative;
        font-size: 13px;
        padding: 0 0 0 0;
    }

    }

    #history-table-toolbar {
        /*background: md-get-palette-color(grey, 200);*/
        height: 52px;
    }
    /*Убивает календарь((
    .md-field {
        max-width: 300px;
    }
    */

</style>