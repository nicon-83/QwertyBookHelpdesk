<template>
    <drawer>
        <md-toolbar id="mainphones-toolbar" class="md-dense" md-elevation="0" style="margin-left: 5px; padding: 0; background: transparent;">
            <div class="md-toolbar-section-start">
                <md-tabs :md-active-tab="currentTab" @md-changed="tabChanged" style="padding-left: 0px;">
                    <md-tab v-for="tab in tabs" :id="tab.Value" :key="tab.Value" :md-label="tab.Name" :to="tab.Route">
                    </md-tab>
                </md-tabs>

                <div class="md-table-pagination">
                    <div v-if="pagination" class="md-table-pagination" id="phones-table-pagination">
                        <span class="md-table-pagination-label">Строк на странице:</span>
                        <md-field>
                            <md-select @md-selected="updateRowsPerPage" v-model="rowsPerPage" md-dense md-class="md-pagination-select">
                                <md-option v-for="amount in pageSize" :key="amount" :value="amount">{{ amount }}</md-option>
                            </md-select>
                        </md-field>
                        <span>{{ currentPageMinCount }}-{{ currentPageMaxCount }} из {{ currentDataCount }}</span>
                        <md-button class="md-icon-button md-table-pagination-previous" @click="goToPrevious()" :disabled="currentPage === 1 || lastPage === 0 || loading">
                            <md-icon>keyboard_arrow_left</md-icon>
                        </md-button>
                        <md-button class="md-icon-button md-table-pagination-next" @click="goToNext()" :disabled="currentPage == lastPage || lastPage === 0 || loading">
                            <md-icon>keyboard_arrow_right</md-icon>
                        </md-button>
                    </div>
                </div>
            </div>
        </md-toolbar>
        <div id="phones-progress-bar">
            <md-progress-bar v-if="loading" md-mode="query"></md-progress-bar>
        </div>
        <component v-bind:is="currentTabComponent" class="tab" id="current-tab"></component>
    </drawer>
</template>

<script>
    import Drawer from '../common/Drawer.vue'
    import NotFound from '../views/NotFound.vue'

    import Phones from './phones.vue'
    import History from './history.vue'
    import EmptyNpp from './emptynpp.vue'
    import DND from './dnd.vue'

    import { mapGetters } from 'vuex'

    export default {
        components: {
            Drawer,
            NotFound,
            Phones,
            EmptyNpp,
            History,
            DND
        },
        name: 'MainPhones',
        props: {
        },
        data: () => ({
            //currentTab: 'Phones',
            rowsPerPageOld: 100,
            rowsPerPage: 100,
            currentPageMinCount: 0,
            currentPageMaxCount: 0,
            currentDataCount: 0,
            currentPage: 1,
            pageSize: [100, 200, 300],
            loading: false,
        }),
        methods: {
            tabChanged: function (value) {
                let idx = this.tabs.findIndex(x => x.Value === value);
                if (idx == -1) {
                    idx = 0;
                }
                this.loading = false;
                this.$store.dispatch('SET_PHONESCURRENTTABROUTE', this.tabs[idx].Route);
            },
            update() {
                this.$root.$emit('PHONESUPDATE');
            },
            updateRowsPerPage() {
                if (this.rowsPerPageOld == this.rowsPerPage) {
                    return;
                }
                this.rowsPerPageOld = this.rowsPerPage;
                this.currentPage = 1;
                this.update();
            },
            goToPage(delta) {
                this.currentPage = this.currentPage + delta;
                this.update()
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
        provide: function () {
            return {
                setCountPagination: this.setCountPagination,
                getRowsPerPage: this.getRowsPerPage,
                getCurrentPage: this.getCurrentPage,
                setCurrentPage: this.setCurrentPage,
                getLoading: this.getLoading,
                setLoading: this.setLoading
            }
        },
        computed: {
            ...mapGetters(['PHONESTABS', 'PHONESCURRENTTABROUTE']),
            tabs: {
                get() {
                    return this.PHONESTABS;
                }
            },
            pagination: {
                get() {
                    return this.$store.getters['PHONESCURRENTPAGINATION'](this.$route.params.id);
                }
            },
            currentTab: {
                get() {
                    return this.$store.getters['PHONESCURRENTTAB'](this.$route.params.id);
                },
                set(value) {
                    /*
                    let idx = this.tabs.findIndex(x => x.Value === value);
                    if (idx == -1) {
                        idx = 0;
                    }
                    this.$store.dispatch('SET_PHONESCURRENTTABROUTE', this.tabs[idx].Route);
                    this.$router.push({ path: this.tabs[idx].Route });
                    */
                }
            },
            currentTabComponent() {
                let flg = false;
                if (this.currentTab == 'Phones') {
                    flg = true;
                }
                this.$store.dispatch('SET_PHONE_ENABLEAUTOREFRESH', flg);
                return this.currentTab;
            },
            lastPage() {
                let cnt = this.currentDataCount;
                let i = parseInt(cnt / this.rowsPerPage);
                if (i * this.rowsPerPage < cnt) {
                    i++;
                }
                return parseInt(i);
            },
            page() {
                return this.$store.state.page;
            },

        }
    }

</script>

<style lang="scss" scoped>
    .md-tab {
        padding: 0px;
        margin: 0;
    }

    .tab {
        /*border: 1px solid #ccc;*/
        padding-top: 5px;
        padding-left: 5px;
        padding-right: 5px;
        padding-bottom: 0px;
    }

    .md-table-pagination {
        border-top: 0;
        height: 48px;
    }

    #phones-progress-bar {
        height: 5px;
        max-height: 5px;
        padding-top: 5px;
        padding-left: 5px;
        padding-right: 5px;
        padding-bottom: 0px;
    }
</style>