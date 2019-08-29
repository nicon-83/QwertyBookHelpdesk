<template>
    <div>
        <md-table class="md-table-History" :md-height.sync="tableHeight" v-model="localInputData" md-card md-fixed-header :md-selected-value.sync="selected">
            <md-table-toolbar id="history-table-toolbar" class="md-dense">
                <div class="md-toolbar-section-start">
                    <div class="md-layout md-gutter" style="margin-top: 4px;">
                        <div class="md-layout-item md-size-25">
                            <div class="md-layout">
                                <div class="md-layout-item md-size-50" style="padding-right: 5px;">
                                    <md-datepicker class="md-datepicker-history" v-model="paramsBeginDate" md-immediately>
                                        <!--:md-override-native="false" глючит под FireFox-->
                                        <label>Начало</label>
                                    </md-datepicker>
                                </div>
                                <div class="md-layout-item md-size-50" style="padding-left: 5px;">
                                    <md-datepicker class="md-datepicker-history" v-model="paramsEndDate" md-immediately>
                                        <label>Конец</label>
                                    </md-datepicker>
                                </div>
                            </div>
                        </div>
                        <div class="md-layout-item md-size-25">
                            <div class="md-layout">
                                <div class="md-layout-item md-size-35" style="padding-right: 5px;">
                                    <md-field>
                                        <label>Отдел</label>
                                        <md-select placeholder="Отдел" v-model="fltOtdels" id="FilterOtdels" md-dense style="margin-right: -10px;">
                                            <md-option v-for="Otdel in HISTORYOTDELS" v-bind:value="Otdel">
                                                {{Otdel}}
                                            </md-option>
                                        </md-select>
                                        <md-button tabindex="-1" class="md-icon-button md-dense" @click="clearFltOtdels" v-if="fltOtdels!=''">
                                            <md-icon>clear</md-icon>
                                        </md-button>
                                    </md-field>
                                </div>
                                <div class="md-layout-item md-size-65" style="padding-left: 5px;">
                                    <md-field>
                                        <label>Сотрудник</label>
                                        <md-select placeholder="Сотрудник" v-model="fltUsers" id="FilterUsers" md-dense style="margin-right: -10px;">
                                            <md-option v-for="User in HISTORYUSERS" v-bind:value="User.user_clid">
                                                {{User.user_clid}}
                                            </md-option>
                                        </md-select>
                                        <md-button tabindex="-1" class="md-icon-button md-dense" @click="clearFltUsers" v-if="fltUsers!=''">
                                            <md-icon>clear</md-icon>
                                        </md-button>
                                    </md-field>
                                </div>
                            </div>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-field>
                                <label>Вызов</label>
                                <md-select placeholder="Вызов" v-model="fltDisposition" id="FilterDisposition" md-dense style="margin-right: -10px;">
                                    <md-option v-for="item in DISPOSITIONHISTORY" v-bind:value="item.DISPOSITION">
                                        {{item.STATUS}}
                                    </md-option>
                                </md-select>
                                <md-button tabindex="-1" class="md-icon-button md-dense" @click="clearFltDisposition" v-if="fltDisposition!=''">
                                    <md-icon>clear</md-icon>
                                </md-button>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-field>
                                <label>Фильтр</label>
                                <md-select placeholder="Фильтр" v-model="fltHistory" id="FilterHistory" md-dense style="margin-right: -10px;">
                                    <md-option v-for="Filter in FILTERHISTORY" v-bind:value="Filter.DATA">
                                        {{Filter.LABEL}}
                                    </md-option>
                                </md-select>
                                <md-button tabindex="-1" class="md-icon-button md-dense" @click="clearFltHistory" v-if="fltHistory!=''">
                                    <md-icon>clear</md-icon>
                                </md-button>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-15">
                            <md-field md-clearable>
                                <label>Клиент/телефон</label>
                                <md-input placeholder="Клиент или телефон" v-model="fltClient" id="FilterClient" md-dense>
                                </md-input>
                            </md-field>
                        </div>
                        <div class="md-layout-item md-size-5" style="padding-left: 0px; padding-top: 16px;">
                            <md-tooltip md-direction="bottom" md-delay="500">Обновить данные</md-tooltip>
                            <md-button class="md-icon-button" id="history-update-button" @click="clickUpdate" :disabled="loading">
                                <md-icon>refresh</md-icon>
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
                <!--md-table-cell md-label="Сотрудник"><a href="#" @click.prevent="clickUser_Clid(item.user_clid)">{{ item.user_clid }}</a></!--md-table-cell-->
                <md-table-cell md-label="Источник"><a href="#" @click.prevent="clickSrc_Dst(item.src)">{{ item.src }}</a></md-table-cell>
                <md-table-cell md-label="Назначение"><a href="#" @click.prevent="clickSrc_Dst(item.dst)">{{ item.dst }}</a></md-table-cell>
                <md-table-cell md-label="Клиент"><a href="#" @click.prevent="clickClidUnknown()">{{ clidUnknownForRow(item.npp, item.is_unknown) }}</a>{{ item.clid }}</md-table-cell>
                <md-table-cell md-label="Вызов">{{ item.disposition }}</md-table-cell>
                <md-table-cell md-label="Сек." md-numeric>{{ item.duration }}</md-table-cell>
            </md-table-row>
        </md-table>

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
        name: 'History',
        inject: ['setCountPagination', 'getRowsPerPage', 'getCurrentPage', 'setCurrentPage', 'getLoading', 'setLoading'],
        props: {
        },
        data: () => ({
            showDialog: false,
            emptyNPP: true,
            url_date_for_history: 'data/getdateforhistory',
            url_data: 'data/gethistory',
            url_data_uniqueid: 'data/gethistoryuniqueid',
            createdate: Date,
            params: {},
            localInputData: [//Костыль для правильной работы первоначального выбора в таблице
                { field1: "abrakadabra" }
            ],
            localSelected: [],
            inputDataUniqueid: [],
            countUpdateOld: 0,
        }),
        created() {
        },
        mounted: function () {
            this.$store.dispatch('setPageData', {
                'header': 'История звонков',
                'status': 'mounted',
                'data': {}
            });
            if (this.HISTORYFIRSTLOAD === true) {
                this.createdate = new Date();
                this.$store.dispatch('GET_HISTORYOTDELS', {});
                this.$store.dispatch('GET_HISTORYUSERS', {});
                this.$store.commit('SETHISTORYFIRSTLOAD', false);
                this.setCurrentPage(1);
                this.update();
            } else {
                this.loading = true;
                this.localInputData = this.HISTORYINPUTDATA;
                this.localSelected = this.HISTORYSELECTED;
                this.setParenCountPagination();
                window.setTimeout(this.clearLoading, 2000);
            };
            this.$root.$on('PHONESUPDATE', this.update);
        },
        beforeDestroy: function () {
            this.$root.$off('PHONESUPDATE', this.update);
            this.$store.dispatch('SET_HISTORYINPUTDATA', this.localInputData);
        },
        methods: {
            clearFltDisposition() {
                this.fltDisposition = '';  
            },
            clearFltOtdels() {
                this.fltOtdels = '';
            },
            clearFltUsers() {
                this.fltUsers = '';
            },
            clearFltHistory() {
                this.fltHistory = '';
            },
            clidUnknownForRow(npp, is_unknown) {
                if (is_unknown === 0) {
                    return '';
                }
                return '<Нет в КБ> ';
            },
            clickClidUnknown() {
                this.fltHistory = this.FILTERHISTORY[0].DATA;
                this.$store.dispatch('showSnackBar', { 'snackText': 'Выбран фильтр: ' + this.FILTERHISTORY[0].LABEL });
            },
            clickSrc_Dst(value) {
                if (value.substring(3, 4) !== ' ') {//100 Секретарь
                    this.fltClient = value;
                    this.$store.dispatch('showSnackBar', { 'snackText': 'Выбран телефон: ' + value });
                } else {
                    let idx = this.HISTORYUSERSFULL.findIndex(x => x.user_clid.substring(5) === value.substring(5));
                    if (idx !== -1) {
                        this.fltOtdels = this.HISTORYUSERSFULL[idx].otdel;
                        this.fltUsers = this.HISTORYUSERSFULL[idx].user_clid;
                        this.$store.dispatch('showSnackBar', { 'snackText': 'Выбран сотрудник: ' + this.fltUsers + ' и Отдел: ' + this.fltOtdels});
                    }
                }
            },
            setParenCountPagination() {
                let row_min = 0;
                let data_count = 0;
                let row_max = 0;
                let ln = this.localInputData.length;
                if (ln > 0) {
                    row_min = this.localInputData[0]['row_num'];
                    data_count = this.localInputData[0]['max_row'];
                    row_max = this.localInputData[ln - 1]['row_num'];
                    this.setCountPagination(
                        [
                            row_min,
                            row_max,
                            data_count
                        ]
                    );
                }
            },
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
            clickUpdate() {
                this.setCurrentPage(1);
                this.update();
            },
            update() {
                if (!this.checkParams()) {
                    return;
                }
                this.loading = true;
                if (this.selected.length > 0) {
                    this.selected = []; //очищаем текущий выбор при переходе на след.страницу
                }
                let store = this.$store;
                this.$http.get(this.url_data, {
                    params: this.paramsData
                }).then(response => {
                    this.localInputData = Array.from(response.body);
                    this.createdate = new Date();
                }, () => {
                        store.dispatch('showSnackBar', { 'snackText': 'Ошибка получения данных истории звонков' });
                    }).finally(() => {
                        this.setParenCountPagination();
                        this.loading = false;
                    });
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
            clearLoading() {
                this.loading = false;
            }
        },
        computed: {
            ...mapGetters([
                'HISTORYINPUTDATA',
                'HISTORYSELECTED',
                'HISTORYFIRSTLOAD',
                'DISPOSITIONHISTORY',
                'FILTERHISTORY',
                'HISTORYFILTER',
                'HISTORYFILTERDISPOSITION',
                'HISTORYDATELIST',
                'HISTORYOTDELS',
                'HISTORYUSERS',
                'HISTORYUSERSFULL',
                'HISTORYFILTEROTDELS',
                'HISTORYFILTERUSERS',
                'HISTORYFILTERCLIENT',
                'HISTORYPARAMSBEGIN',
                'HISTORYPARAMSEND',
                'HISTORYPARAMSBEGIN_JSON',
                'HISTORYPARAMSEND_JSON']),
            loading: {
                get() {
                    return this.getLoading();
                },
                set(value) {
                    this.setLoading(value);
                }
            },
            selected: {
                get() {
                    return this.localSelected
                },
                set(value) {
                    this.localSelected = value;
                    this.$store.dispatch('SET_HISTORYSELECTED', value);
                }
            },
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
            fltHistory: {
                get() {
                    return this.HISTORYFILTER
                },
                set(value) {
                    this.$store.dispatch('SET_HISTORYFILTER', value);
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
                let delta = 0;
                return this.$store.state.app.tableHeight - delta;
            },
            paramsData() {
                let rowsPerPage = this.getRowsPerPage();
                let currentPage = this.getCurrentPage();
                this.params = 
                    {
                    xpName: 'xp_History_Json',
                    DateBegin: this.HISTORYPARAMSBEGIN.Date ? this.HISTORYPARAMSBEGIN_JSON : undefined,
                    DateEnd: this.HISTORYPARAMSEND.Date ? this.HISTORYPARAMSEND_JSON : undefined,
                    Disposition: this.HISTORYFILTERDISPOSITION ? this.HISTORYFILTERDISPOSITION : undefined,
                    Otdel: this.HISTORYFILTEROTDELS ? this.HISTORYFILTEROTDELS : undefined,
                    User: this.HISTORYFILTERUSERS ? this.HISTORYFILTERUSERS : undefined,
                    Filter: this.HISTORYFILTER ? this.HISTORYFILTER : undefined,
                    Client: this.HISTORYFILTERCLIENT ? this.HISTORYFILTERCLIENT : undefined,
                    row_num_begin: (currentPage - 1) * rowsPerPage + 1,
                    row_num_end: (currentPage) * rowsPerPage,
                    }
                return this.params;
            },
            paramsDataUniqueid() {
                return {
                    uniqueid1: this.localSelected.length >= 1 ? this.localSelected[0].uniqueid : '-',
                    uniqueid2: this.localSelected.length >= 2 ? this.localSelected[1].uniqueid : '-',
                    uniqueid3: this.localSelected.length >= 3 ? this.localSelected[2].uniqueid : '-',
                    uniqueid4: this.localSelected.length >= 4 ? this.localSelected[3].uniqueid : '-',
                    uniqueid5: this.localSelected.length >= 5 ? this.localSelected[4].uniqueid : '-',
                }
            },
            notFound() {
                if (this.params.DateBegin === undefined) {
                    return '';
                }
                let val = this.params;
                let disposition = '';
                if (val.Disposition != null) {
                    let fnd = this.DISPOSITIONHISTORY.find(x => x.DISPOSITION === val.Disposition).STATUS;
                    disposition = ' Вызов: "' + fnd +'"';
                }
                let otdel = ''
                if (val.Otdel != null) {
                    otdel = ' Отдел: "' + val.Otdel + '"';
                }
                let user = '';
                if (val.User != null) {
                    user = ' Сотрудник: "' + val.User + '"';
                }
                let filter = '';
                if (val.Filter != null) {
                    let fndFilter = this.FILTERHISTORY.find(x => x.DATA === val.Filter).LABEL;
                    user = ' Фильтр: "' + fndFilter + '"';
                }
                let client = val.Client ? ' Клиент: "' + val.Client + '"' : '';
                return 'Период: "' + val.DateBegin.toLocaleString("ru").substring(0, 10) + ' - ' + val.DateEnd.toLocaleString("ru").substring(0, 10) + '"'
                    + disposition
                    + otdel
                    + user
                    + filter
                    + client;
            }
        }
    }
</script>

<style lang="scss">
    /*@import "~vue-material/dist/theme/engine";*/

    .md-datepicker-history {
        .md-datepicker-panel {
            max-height: 230px; /*Add Serg для исправления глюка при открытия календаря, style lang="scss" должен быть быть БЕЗ scope!!!*/
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

        .md-table-cell {
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