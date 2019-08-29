<template>
    <main-parent>
        <div class="page-container">
            <md-app md-mode="fixed">
                <md-app-toolbar id="application-toolbar" class="md-primary md-dense" md-elevation="0" style="height: 52px;">
                    <div class="md-toolbar-section-start" style=" padding-top: 0px;">
                        <!-- <md-button class="md-icon-button" @click="menuVisible = true">
                            <md-icon>menu</md-icon>
                            <md-tooltip md-direction="top" md-delay="500">Меню</md-tooltip>
                        </md-button>
                        <span class="md-title">{{page.header}}</span> -->
						<md-tabs class="md-primary" :md-active-tab="$route.path.split('/')[1]" style="margin-right: 50px;">
                            <!-- <md-tab id="notes" md-label="Записки" to="/notes"></md-tab>
                            <md-tab id="clients" md-label="Клиенты" to="/clients"></md-tab>
                            <md-tab id="reports" md-label="Отчеты" to="/reports"></md-tab>
                            <md-tab id="phones" md-label="" :to="phonesTab"></md-tab> -->
                            <!--md-tab id="tab-history" md-label="История" to="/history"></md-tab-->
                            <!--md-tab id="tab-test" :md-label="test_value" to="/test">qwer</md-tab-->
                            <md-tab id="support" md-label="Поддержка" :to="supportTab"></md-tab>
                        </md-tabs>

                    </div>
                    <div class="md-toolbar-section-end">
                        <!-- <md-button class="md-icon-button" @click="logout()">
                            <md-icon>account_box</md-icon>
                            <md-tooltip md-direction="top" md-delay="500">Выход</md-tooltip>
                        </md-button>
                        <div class="right">
                            <div><span class="md-subheading">{{userInfo.originalName}}</span></div>
                            <div><span class="md-caption" v-if="CURRENTAPTEKA.length > 0">{{CURRENTAPTEKA.name}}</span></div>
                        </div> -->

                        <div><span v-if="ISAUTHENTICATED">{{CURRENTAPTEKA.name}} ({{CURRENTAPTEKA.idapt}})</span></div>
                        <md-button class="md-icon-button" @click="goToUserPage">
                            <md-icon>account_box</md-icon>
                            <md-tooltip md-direction="top" md-delay="500" v-if="CURRENTUSER.userId">Страница пользователя</md-tooltip>
                            <md-tooltip md-direction="top" md-delay="500" v-else>Регистрация пользователя</md-tooltip>
                        </md-button>
                        <md-button class="md-icon-button" @click="logout()" style="margin-left:0">
                            <md-icon>exit_to_app</md-icon>
                            <md-tooltip md-direction="top" md-delay="500">Выход</md-tooltip>
                        </md-button>
                    </div>
                </md-app-toolbar>

                <!--<md-app-drawer :md-active.sync="menuVisible" md-permanent="clipped">-->
                <!-- <md-app-drawer :md-active.sync="menuVisible">
                    <md-toolbar class="md-transparent" md-elevation="1">
                        <div>
                            <md-button class="md-icon-button" @click="menuVisible = false">
                                <md-icon>menu</md-icon>
                            </md-button>
                        </div>
                        <span class="md-title">КвертиБук</span>
                    </md-toolbar>

                    <md-list>
                        <router-link to="/phone/device">
                            <md-list-item @click="simpleClick()">
                                <md-icon>move_to_inbox</md-icon>
                                <span class="md-list-item-text">Inbox</span>
                            </md-list-item>
                        </router-link>

                        <router-link to="/">
                            <md-list-item>
                                <md-icon>send</md-icon>
                                <span class="md-list-item-text">Sent Mail</span>
                            </md-list-item>
                        </router-link>

                        <md-list-item>
                            <md-icon>delete</md-icon>
                            <span class="md-list-item-text">Trash</span>
                        </md-list-item>

                        <md-list-item>
                            <md-icon>error</md-icon>
                            <span class="md-list-item-text">Spam</span>
                        </md-list-item>
                    </md-list>
                </md-app-drawer> -->
                <md-app-content id="app-content">
                    <div class="content">
                        <slot v-if='page.status !== "loading"'></slot>
                        <!--div class="loading" v-if='page.status === "loading"'>
                            <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
                        </div-->
                    </div>
                </md-app-content>
            </md-app>
        </div>
    </main-parent>
</template>

<script>
    import MainParent from '../MainParent'
    import DrawerProgressBar from "./DrawerProgressBar";
    import { mapGetters } from 'vuex'

    export default {
        components: {
            DrawerProgressBar,
            MainParent,
        },
        name: 'Drawer',
        data: () => ({
            menuVisible: false,
            search: '',
        }),
        methods: {
            setTheme(theme) {
                this.$material.theming.theme = theme
            },
            toggleMenu() {
                this.menuVisible = !this.menuVisible
			},
            logout() {
				//очистка state (возвращение перевоначального состояния) происходит в router/index.js action CLEAR_STATE
				this.$store.dispatch('SET_ISAUTHENTICATED', false);
				this.$router.push({ path: "/login" });
            },
            simpleClick() {
                this.menuVisible = false
            },
            goToUserPage(){
                this.$router.push({ path: "/userPage" });
            }
        },
        computed: {
			...mapGetters(['PHONESCURRENTTABROUTE', 'SUPPORTCURRENTTABROUTE', 'CURRENTAPTEKA', 'ISAUTHENTICATED', 'CURRENTUSER']),
            phonesTab() {
                return this.PHONESCURRENTTABROUTE;
			},
			supportTab() {
				return this.SUPPORTCURRENTTABROUTE;
			},
            test_value() {
                if (this.$store.state.phones.Phone_EnableAutoRefresh) {
                    return 'TRUE'
                }
                return 'FALSE'
            },
            page() {
                return this.$store.state.page
            },
            userInfo() {
                return this.$store.state.user
            },
            menu() {
                return this.$store.state.menu
            }
        },
        mounted() {
            var elem = document.getElementById("application-toolbar");
            this.$store.dispatch('setApp', { 'toolbarHeight': elem.clientHeight });
            if (document.documentElement.clientHeight > 900) {
                this.$store.dispatch('setApp', { 'tableHeight': document.documentElement.clientHeight - 216 - 50 - 5}); //216
            } else {
                this.$store.dispatch('setApp', { 'tableHeight': document.documentElement.clientHeight - 192 - 50 - 5}); //192
            }

            /*
            if (this.$store.state.user.originalName === undefined) {
                this.$store.dispatch('getAuthUser')
            }

            this.$http.get('api/user/menu_list').then(response => {
                this.$store.dispatch('setMenu', response.body);
            }, () => {
                this.$store.dispatch('showSnackBar', { 'snackText': 'Ошибка обращения к api безовасности' });
                this.loading--;
            });
            */
        }
    }
</script>

<style lang="scss" scoped>
    @media (max-width: 1900px) {
        .md-drawer {
            width: 200px;
            max-width: calc(100vw - 125px);
        }

        .md-app {
            height: 100vh;
            max-height: 100vh;
            margin: 0;
        }

        .md-app-content {
            /*background: aqua;*/
            padding: 5px;
        }
    }

    .page-container{
        min-width: 960px;
    }

</style>
