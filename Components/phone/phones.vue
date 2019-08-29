<template>
    <div>
        <!--<md-table v-model="searched" md-sort="name" md-sort-order="asc" md-card md-fixed-header>-->
        <md-table :md-height.sync="tableHeight" v-model="searched" :md-sort.sync="currentSort" :md-sort-order.sync="currentSortOrder" :md-sort-fn="customSort" md-card md-fixed-header>
            <md-table-toolbar id="phones-table-toolbar" class="md-dense">
                <div class="md-toolbar-section-start">
                    <span style="width: 350px;">
                        <span class="md-title">Обновлено </span>
                        <span>
                            <transition name="fadedate">
                                <span class="md-title" v-show="showDate" style="margin-left: 0;">{{ createdate.toLocaleTimeString("ru") }}</span>
                            </transition>
                        </span>
                        <span style="position: fixed; left: 225px;">
                            <md-badge class="md-primary" md-dense :md-content.sync="autoRefresh">
                                <md-button class="md-icon-button" id="phones-update-button" @click="update()">
                                    <md-icon>refresh</md-icon>
                                    <md-tooltip md-direction="top" md-delay="500">Обновить данные</md-tooltip>
                                </md-button>
                            </md-badge>
                        </span>
                    </span>
                    <md-field md-clearable>
                        <label for="search">Найти</label>
                        <md-input placeholder="Найти ..." v-model="search" @input="searchOnTable" />
                    </md-field>
                </div>
                <div class="md-toolbar-section-end">
                    <div style="margin-right: 16px;">
                        <md-radio v-model="fltState" class="md-primary" value="All" @change="updateFilterOtdel"><small>Все</small></md-radio>
                        <md-radio v-model="fltState" md-theme="green" class="md-primary" value="NOT_INUSE" @change="updateFilterOtdel"><small>Свободен</small></md-radio>
                        <md-radio v-model="fltState" class="md-accent" value="INUSE" @change="updateFilterOtdel"><small>Занят</small></md-radio>
                    </div>
                    <div>
                        <md-field>
                            <label for="FilterOtdels">Отдел(ы)</label>
                            <md-select placeholder="Все отделы" v-model="fltOtdels" name="FilterOtdels" id="FilterOtdels" md-dense multiple @md-selected="updateFilterOtdel">
                                <md-option v-for="Otdel in OTDELS" v-bind:value="Otdel">
                                    {{Otdel}}
                                </md-option>
                            </md-select>
                        </md-field>
                    </div>
                    <!--<md-tooltip md-direction="top">Фильтр: XXX</md-tooltip>-->
                </div>
            </md-table-toolbar>

            <md-table-empty-state md-label="Ничего не найдено" v-if="!loading"
                                  :md-description="`По запросу '${search}' ничего не найдено.`">
            </md-table-empty-state>

            <md-table-row slot="md-table-row" slot-scope="{ item }">
                <!--
            <md-table-cell md-label="№ п/п" md-numeric md-sort-by="RowNumber">{{ item.RowNumber }}</md-table-cell>
            -->
                <md-table-cell md-label="Отдел" md-sort-by="Sort_Otdel" style="width: 80px;">
                    {{ item.Otdel }}
                </md-table-cell>
                <md-table-cell md-label="Сотрудник" md-sort-by="id" style="width: 260px;">
                    <md-menu style="text-align: right;" md-direction="bottom-end">
                        <md-chip :class="getThemeChip(item)" md-theme="green" md-clickable md-menu-trigger>{{ item.id }}</md-chip> <!--md-static-->
                        <md-menu-content>
                            <md-menu-item @click="HistoryCall(item.id)">
                                <md-icon>edit</md-icon>
                                <span>История звонков</span>
                            </md-menu-item>
                        </md-menu-content>
                    </md-menu>
                    &nbsp;{{ item.Clid}}
                </md-table-cell>
                <md-table-cell md-label="Статус" md-sort-by="Sort_Device_State" style="width: 110px;">
                    {{ item.Device_State_Desc }}
                </md-table-cell>
                <md-table-cell md-label="Дежурный" md-sort-by="Duty_Desc" style="width: 115px;">
                    {{ item.Duty_Desc }}
                </md-table-cell>
                <md-table-cell md-label="Информация" md-sort-by="Current_Companys_Name">
                    {{ item.InfoData }}
                </md-table-cell>
                <md-table-cell md-label="Время" md-sort-by="Duration_Second" style="width: 100px;">
                    {{ item.Duration_Time}}
                </md-table-cell>
            </md-table-row>
        </md-table>
    </div>
</template>

<script>
    const toLower = text => {
        return text.toString().toLowerCase()
    }

    const searchByName = (items, term, filterState, filterOtdels, columns_no_find) => {
        const toLower = text => {
            return (text + '').toLowerCase()
        };
        if (filterState != 'All') {
            items = items.filter(item => {
                for (let key in item) {
                    if (filterState == 'NOT_INUSE') {
                        if (item.Device_State_Desc == 'Свободен') {
                            return true
                        }
                        return false
                    }
                    if (item.Device_State_Desc != 'Свободен') {
                        return true
                    }
                    return false
                }
            })
        }
        if (filterOtdels.length > 0) {
            items = items.filter(item => {
                for (let key in item) {
                    if (filterOtdels.indexOf(item.Otdel) != -1) {
                        return true;
                    }
                    return false;
                }
            })
        }

        if (term) {
            return items.filter(item => {
                for (let key in item) {
                    if (columns_no_find.indexOf(key) == -1) {
                        if (toLower(item[key]).includes(toLower(term))) {
                            return true
                        }
                    }
                }
                return false;
            })
        }

        return items;
    }

    import Drawer from '../common/Drawer.vue'
    import { mapGetters } from 'vuex'

    export default {
        components: {
            Drawer
        },
        name: 'Phones',
        props: {
        },
        data: () => ({
            loading: false,
            url_data: 'data/getphones',
            currentSort: 'Sort_Otdel',
            currentSortOrder: 'asc',
            columns_no_find: ['RowNumber', 'Device_State_Desc', 'Duration_Second', 'Duration_Time', 'Sort_Otdel', 'Sort_Device_State'],
            createdate: Date,
            search: null,
            autoRefresh: 9,
            duration: 0,
            checkDuration: null,
            searched: [],
            inputData: [],
            firstLoad: true,
            enabledCalcField: false,
            showDate: true
        }),
        methods: {
            setDuration() {
                if (!this.enabledCalcField) {
                    return
                }
                for (let value of this.searched) {
                    if (value.Duration_Second !== undefined) {
                        var sec = value.Duration_Second + this.duration;

                        var day = Math.trunc(sec / 60 / 60 / 24) % 24;
                        var hour = Math.trunc(sec / 60 / 60) % 24;
                        var minute = Math.trunc(sec / 60) % 60;
                        var second = sec % 60;

                        var str = '';
                        if (day > 0) {
                            str = day + ' дн. ';
                        }
                        if (hour >= 10) {
                            str = str + hour;
                        } else {
                            str = str + '0' + hour;
                        }
                        str = str + ':';
                        if (minute >= 10) {
                            str = str + minute;
                        } else {
                            str = str + '0' + minute;
                        }
                        str = str + ':';
                        if (second >= 10) {
                            str = str + second;
                        } else {
                            str = str + '0' + second;
                        }
                        value.Duration_Time = str;
                    } else {
                        value.Duration_Time = '';
                    }
                }
            },
            searchOnTable() {
                this.searched = searchByName(this.inputData, this.search, this.fltState, this.fltOtdels, this.columns_no_find)
            },
            update() {
                /*window.clearInterval(this.checkDuration);
                this.checkDuration = null;*/
                this.enabledCalcField = false;
                this.duration = 0;

                this.loading = true;
                this.$http.get(this.url_data, {
                    params: ''
                }).then(response => {
                    for (let value of response.body) {
                        if (this.firstLoad) {
                            let valueToPush = new Object();
                            valueToPush.RowNumber = value.RowNumber;
                            valueToPush.id = value.id;
                            valueToPush.Otdel = value.Otdel;
                            valueToPush.Sort_Otdel = value.Sort_Otdel;
                            let idx = value.Clid.indexOf('>');
                            if (idx != -1) {
                                valueToPush.Clid = value.Clid.substring(idx + 2);
                            } else {
                                valueToPush.Clid = value.Clid;
                            }
                            valueToPush.Device_State_Desc = value.Device_State_Desc;
                            valueToPush.Sort_Device_State = value.Sort_Device_State;
                            valueToPush.Duty_Desc = value.Duty_Desc;
                            //valueToPush.NoWorkplace_Desc = value.NoWorkplace_Desc;
                            //valueToPush.Current_Companys_Name = value.Current_Companys_Name;
                            if (value.Current_Companys_Name != null) {
                                valueToPush.InfoData = value.Current_Companys_Name;
                            } else {
                                if (value.NoWorkplace_Desc != null) {
                                    valueToPush.InfoData = value.NoWorkplace_Desc;
                                }    
                                else {
                                    valueToPush.InfoData = '';
                                }
                            }
                            valueToPush.Duration_Second = value.Duration_Second;
                            valueToPush.Duration_Time = '';
                            this.inputData.push(valueToPush);
                        } else {
                            let idx = this.inputData.findIndex(x => x.id === value.id);
                            if (idx != -1) {
                                var item = this.inputData[idx];
                                if (item.RowNumber !== value.RowNumber) {
                                    item.RowNumber = value.RowNumber;
                                }
                                if (item.Sort_Otdel !== value.Sort_Otdel) {
                                    item.Sort_Otdel = value.Sort_Otdel;
                                }
                                if (item.Device_State_Desc !== value.Device_State_Desc) {
                                    item.Device_State_Desc = value.Device_State_Desc;
                                }
                                if (item.Sort_Device_State !== value.Sort_Device_State) {
                                    item.Sort_Device_State = value.Sort_Device_State;
                                }
                                if (item.Duty_Desc !== value.Duty_Desc) {
                                    item.Duty_Desc = value.Duty_Desc;
                                }
                                //if (item.NoWorkplace_Desc !== value.NoWorkplace_Desc) {
                                //    item.NoWorkplace_Desc = value.NoWorkplace_Desc;
                                //}
                                //if (item.Current_Companys_Name !== value.Current_Companys_Name) {
                                //    item.Current_Companys_Name = value.Current_Companys_Name;
                                //}
                                let infoData = '';
                                if (value.Current_Companys_Name != null) {
                                    infoData = value.Current_Companys_Name;
                                } else {
                                    if (value.NoWorkplace_Desc != null) {
                                        infoData = value.NoWorkplace_Desc;
                                    }
                                    else {
                                        infoData = '';
                                    }
                                }
                                if (item.InfoData !== infoData) {
                                    item.InfoData = infoData;
                                }
                                if (item.Duration_Second !== value.Duration_Second) {
                                    item.Duration_Second = value.Duration_Second;
                                }
                            }
                        }
                    }
                    this.firstLoad = false;
                    this.searchOnTable();
                    this.createdate = new Date();
                    this.enabledCalcField = true;
                    if (this.autoRefresh != 0) {
                        this.autoRefresh = 9
                    };
                    /*this.startTimerDuration();*/
                }, () => {
                    this.$store.dispatch('showSnackBar', { 'snackText': 'Ошибка получения данных телефонов сотрудников' });
                }).finally(() => { this.loading = false; });
            },
            customSort(value) {
                return value.sort((a, b) => {
                    const sortBy = this.currentSort;
                    var a_sortBy = a[sortBy];
                    var b_sortBy = b[sortBy];
                    /*if (sortBy == "Duty_Desc" || sortBy == "NoWorkplace_Desc" || sortBy == "Current_Companys_Name") {*/
                    if (sortBy == "Duty_Desc" || sortBy == "InfoData") {
                        a_sortBy = b[sortBy];
                        b_sortBy = a[sortBy];
                    }

                    if (a_sortBy == null) {
                        a_sortBy = ""
                    }
                    if (b_sortBy == null) {
                        b_sortBy = ""
                    }

                    var calc;
                    if (typeof (a_sortBy) == 'number') {
                        if (this.currentSortOrder == 'asc') {
                            calc = a_sortBy - b_sortBy
                        } else {
                            calc = b_sortBy - a_sortBy;
                        }
                        if (calc == 0) {
                            return a.RowNumber - b.RowNumber
                        }
                        return calc
                    }
                    //default desc
                    if (this.currentSortOrder == 'asc') {
                        calc = a_sortBy.localeCompare(b_sortBy)
                    } else {
                        calc = b_sortBy.localeCompare(a_sortBy)
                    }
                    if (calc == 0) {
                        return a.RowNumber - b.RowNumber
                    }
                    return calc
                })
            },
            updateFilterOtdel() {
                this.searchOnTable();
            },
            getThemeChip({ Device_State_Desc }) {
                const chip = "";
                if (Device_State_Desc == "Свободен") {
                    return "md-primary" + chip
                }
                return "md-accent" + chip
            },
            startTimerDuration() {
                if (!this.checkDuration) {
                    this.checkDuration = window.setInterval(() => {
                        if ((!this.enabledCalcField) || (!this.PHONE_ENABLEAUTOREFRESH)) {
                            return
                        }
                        this.duration++;
                        this.setDuration();
                        if (this.autoRefresh == 1) {
                            this.showDate = false;
                        }
                        if (this.autoRefresh == 0) {
                            this.update();
                            this.autoRefresh = 10;
                        }
                        this.autoRefresh--;
                        if ((this.autoRefresh >= 1) & (!this.showDate)) {
                            this.showDate = true;
                        }
                    }, 1000)
                }
            }
        },
        created() {
            this.createdate = new Date();
            this.$store.dispatch('GET_OTDELS', {});
        },
        mounted: function () {
            this.$store.dispatch('setPageData', {
                'header': 'Телефоны сотрудников',
                'status': true,
                'data': {}
            });
            this.update();
            this.startTimerDuration();
        },
        computed: {
            ...mapGetters(['PHONE_ENABLEAUTOREFRESH', 'OTDELS', 'FILTEROTDELS', 'FILTERSTATE']),
            fltState: {
                get() {
                    return this.FILTERSTATE
                },
                set(value) {
                    this.$store.dispatch('SET_FILTERSTATE', value);
                }
            },
            fltOtdels: {
                get() {
                    return this.FILTEROTDELS
                },
                set(value) {
                    this.$store.dispatch('SET_FILTEROTDELS', value);
                }
            },
            tableHeight() {
                return this.$store.state.app.tableHeight;
            },
        }
    }
</script>

<style lang="scss" scoped>
    
    .fadedate-enter-active, .fadedate-leave-active {
        transition: opacity .5s;
    }
    /* .fade-leave-active до версии 2.1.8 */
    .fadedate-enter, .fadedate-leave-to {
        opacity: 0;
    }

    .md-table {
        margin: 0 !important;
    }

    .md-table-row {
    }

    .md-table-cell {
        height: 32px;
        position: relative;
        font-size: 13px;
        line-height: 18px;
    }

    .md-title {
        line-height: 40px;
        padding-top: 4px;
    }

    .md-field {
        max-width: 300px;
    }

    /*mozilla*/
    /*
    @-moz-document url-prefix() {
        .md-chip {
            padding-top: 0px;
        }
    }*/

    /*Chrome*/
    /*
    @media screen and (-webkit-min-device-pixel-ratio:0) {
        .md-chip {
            padding-top: 2px; 
        }
    }
    */

    .md-chip {
        font-size: 13px;
        height: 20px; /*24*/
        line-height: 16px;
        -webkit-padding-before: 2px;
        padding-bottom: 2px; /*4*/
        /*
        padding-left: 0px;
        padding-right: 0px;
        */
    }

</style>