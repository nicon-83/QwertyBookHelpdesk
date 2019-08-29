<template>
    <main-parent>
        <div class="page-container">
            <md-app md-mode="fixed">
                <md-app-toolbar class="md-primary md-dense">
                    <md-button class="md-icon-button" @click="toggleMenu">
                        <md-icon>menu</md-icon>
                        <md-tooltip md-direction="top">Меню</md-tooltip>
                    </md-button>
                    <span class="md-title">{{page.header}}</span>
                    <div class="md-toolbar-section-end">
                        <div class="right">
                            <div><span class="md-subheading">{{userInfo.originalName}}</span></div>
                            <div><span class="md-caption" v-if="userInfo.rolesName">{{userInfo.clientName}} ({{userInfo.rolesName}})</span></div>
                        </div>
                        <md-menu>
                            <md-button class="md-icon-button" md-menu-trigger>
                                <md-icon>tonality</md-icon>
                                <md-tooltip md-direction="top">Цвет темы</md-tooltip>
                            </md-button>

                            <md-menu-content>
                                <md-menu-item @click="setTheme('default')">
                                    <span>Синяя</span>
                                </md-menu-item>
                                <md-menu-item @click="setTheme('green')">
                                    <span>Зеленая</span>
                                </md-menu-item>
                                <md-menu-item @click="setTheme('default-dark')">
                                    <span>Синяя ночная</span>
                                </md-menu-item>
                                <md-menu-item @click="setTheme('green-dark')">
                                    <span>Зеленая ночная</span>
                                </md-menu-item>
                            </md-menu-content>
                        </md-menu>
                        <md-button class="md-icon-button" @click="logout()">
                            <md-icon>exit_to_app</md-icon>
                            <md-tooltip md-direction="top">Выход</md-tooltip>
                        </md-button>
                    </div>
                </md-app-toolbar>


                <!--<md-app-drawer v-if="menuVisible" md-permanent="clipped">-->
                <md-app-drawer v-if="menuVisible" md-persistent="mini">
                    <md-list>
                        <router-link to="/">
                            <md-list-item>
                                <md-icon>home</md-icon>
                                <span class="md-list-item-text">Главная</span>
                            </md-list-item>
                        </router-link>

                        <router-link to="/phone/device">
                            <md-list-item v-for="item in menu">
                                <!--router-link :to='item["url"]'-->
                                <md-icon>{{ item["iconlabel"] }}</md-icon>
                                <span class="md-list-item-text">{{ item["menuname"] }}</span>
                                <md-tooltip md-direction="top">{{ item["menuname"] }}</md-tooltip>
                                <!--/router-link-->
                            </md-list-item>
                        </router-link>

                        <md-list-item @click="logout()">
                            <md-icon>move_to_inbox</md-icon>
                            <span class="md-list-item-text">Inbox</span>
                        </md-list-item>
                        <!--
    <md-list-item v-for="item in menu" md-expand>
        <md-icon>{{ item["iconlabel"] }}</md-icon>
        <span class="md-list-item-text">{{ item["menuname"] }}</span>
        <md-tooltip md-direction="top">{{ item["menuname"] }}</md-tooltip>
        <md-list slot="md-expand">
            <router-link v-for="child in item['childs']" :to='child["url"]'>
                <md-list-item style="height: 36px;">
                    <md-icon>{{ child["iconlabel"] }}</md-icon>
                    <span class="md-list-item-text">{{ child["menuname"] }}</span>
                    <md-tooltip md-direction="top">{{ child["menuname"] }}</md-tooltip>
                </md-list-item>
            </router-link>
        </md-list>
    </md-list-item>
    -->
                        <md-list-item @click="logout()">
                            <md-icon>exit_to_app</md-icon>
                            <span class="md-list-item-text">Выход</span>
                        </md-list-item>
                    </md-list>
                </md-app-drawer>

                <md-app-content>
                    <drawer-progress-bar />
                    <div class="content">
                        <slot v-if='page.status !== "loading"'></slot>
                        <div class="loading" v-if='page.status === "loading"'>
                            <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
                        </div>
                    </div>
                </md-app-content>
            </md-app>
        </div>
    </main-parent>
</template>

<script>
    import MainParent from '../MainParent'
    import DrawerProgressBar from "./DrawerProgressBar";

    export default {
        components: {
            DrawerProgressBar,
            MainParent,
        },
        name: 'Drawer',
        data: () => ({
            menuVisible: true,
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
                this.$store.dispatch('logout')
            }
        },
        computed: {
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

            if (this.$store.state.user.originalName === undefined) {
                this.$store.dispatch('getAuthUser')
            }

            this.$http.get('api/user/menu_list').then(response => {
                this.$store.dispatch('setMenu', response.body);
            }, () => {
                this.$store.dispatch('showSnackBar', { 'snackText': 'Ошибка обращения к api безовасности' });
                this.loading--;
            });


        }
    }

</script>


<style lang="scss" scoped>
    .center {
        position: absolute;
        margin: 0 auto;
        left: 0;
        right: 0;
        width: 40%;
        min-width: 400px;
        max-width: 1100px;
    }

        .center:before {
            top: 0;
            right: 0;
            left: 0;
            bottom: 0;
            margin: auto;
            content: '';
            width: 100%;
            position: absolute;
            height: 48px;
            background-color: rgba(255,255,255,0.2);
            padding: 4px;
            border-radius: 4px;
        }

    .right {
        text-align: right;
        margin-right: 10px;
    }

        .right .md-caption {
            color: #fff;
        }

    .content {
        width: 100%;
        max-width: 1250px;
        margin: 0 auto;
        position: relative;
        min-height: calc(100vh - 50px);
        min-width: 700px;
    }

        .content .loading {
            margin: auto;
            width: 60px;
            height: 60px;
            position: absolute;
            top: calc(50vh - 90px);
            z-index: 99;
            z-index: 99;
            left: 0;
            right: 0;
        }

    .md-drawer {
        border: 1px solid rgba(#000, .12);
        position: absolute;
        width: 320px;
    }

    @media (max-width: 1900px) {
        .md-drawer {
            position: relative;
        }
    }
</style>
