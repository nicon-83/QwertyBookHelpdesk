<template>
		<div>
				<!-- <div id="create-ticket-button" style="margin-bottom: 10px">
						<md-button
							class="md-dense md-raised md-primary md-theme-default"
							@click="createTicket"
							:disabled="HASCREATETICKETTAB"
						>Создать заявку</md-button>
				</div>-->

				<!--				<md-table class="md-table-Tickets" md-card :style="`height:${tableHeight}px;`">-->

				<!--						<md-table-toolbar id="tickets-table-toolbar" class="md-dense">-->
				<!--								<div class="md-toolbar-section-start">-->
				<!--										<div class="md-layout md-gutter md-alignment-center-left" style="margin-top: 4px;width:100%">-->
				<!--												<div class="md-layout-item md-size-15">-->
				<!--														<md-button-->
				<!--																		tabindex="-1"-->
				<!--																		class="md-dense md-raised md-primary md-theme-default"-->
				<!--																		@click="createTicket"-->
				<!--																		:disabled="HASCREATETICKETTAB"-->
				<!--														>Создать заявку-->
				<!--														</md-button>-->
				<!--												</div>-->
				<!--												<div class="md-layout-item md-size-15">-->
				<!--														<md-field>-->
				<!--																<label>Статус</label>-->
				<!--																<md-select-->
				<!--																				placeholder="Статус"-->
				<!--																				v-model="fltStates"-->
				<!--																				id="FilterStates"-->
				<!--																				md-dense-->
				<!--																				style="margin-right: -10px;"-->
				<!--																>-->
				<!--																		<md-option-->
				<!--																						v-for="state in TICKETSSTATES"-->
				<!--																						v-bind:value="state.name"-->
				<!--																						:key="state.name"-->
				<!--																		>{{state.name}}-->
				<!--																		</md-option>-->
				<!--																</md-select>-->
				<!--																<md-button-->
				<!--																				tabindex="-1"-->
				<!--																				class="md-icon-button md-dense"-->
				<!--																				@click="clearFltStates"-->
				<!--																				v-if="fltStates!=''"-->
				<!--																>-->
				<!--																		<md-icon>clear</md-icon>-->
				<!--																</md-button>-->
				<!--														</md-field>-->
				<!--												</div>-->
				<!--												&lt;!&ndash; <div class="md-layout-item md-size-25"></div>-->
				<!--													<md-field md-clearable>-->
				<!--														<label>Аптека</label>-->
				<!--														<md-input placeholder="Аптека" v-model="fltClients" id="FilterClients" md-dense></md-input>-->
				<!--													</md-field>-->
				<!--												</div>&ndash;&gt;-->
				<!--												<div class="md-layout-item md-size-5" style="padding-left: 0px;">-->
				<!--														<md-button-->
				<!--																		class="md-icon-button"-->
				<!--																		id="tickets-update-button"-->
				<!--																		@click="clickUpdate"-->
				<!--																		:disabled="loading"-->
				<!--														>-->
				<!--																<md-icon>refresh</md-icon>-->
				<!--																<md-tooltip md-direction="bottom" md-delay="500">Обновить данные</md-tooltip>-->
				<!--														</md-button>-->
				<!--												</div>-->
				<!--										</div>-->
				<!--								</div>-->
				<!--						</md-table-toolbar>-->
				<!--						<md-table-empty-state md-label="Заявок не найдено" md-description></md-table-empty-state>-->

				<!--						<md-table-row class="md-table-fixed-header">-->
				<!--								<md-table-head>Номер</md-table-head>-->
				<!--								<md-table-head v-if="USERTICKETSWITHOUTANSWER.length > 0"></md-table-head>-->
				<!--								<md-table-head>Тема</md-table-head>-->
				<!--								<md-table-head>Статус</md-table-head>-->
				<!--								<md-table-head v-if="SITEUSERS.length > 0">Пользователь</md-table-head>-->
				<!--								<md-table-head>Исполнитель</md-table-head>-->
				<!--								<md-table-head>Обновлено</md-table-head>-->
				<!--						</md-table-row>-->

				<!--						<md-table-row v-for="item in localInputData" :style="`background-color:${calcStateColor(item.stateName)};`">-->
				<!--								<md-table-cell>{{ item.number }}</md-table-cell>-->
				<!--								<md-table-cell v-if="USERTICKETSWITHOUTANSWER.length > 0">-->
				<!--										<md-icon style="color:#FF5252" v-if="calcNewTicketWithoutAnswer(item.npp)">new_releases</md-icon>-->
				<!--								</md-table-cell>-->
				<!--								<md-table-cell>-->
				<!--										<md-badge md-dense v-if="calcTicketChanges(item.npp)"-->
				<!--										          :md-content="calcNewMessagesCount(item.npp)">-->
				<!--												<md-button-->
				<!--																class="md-primary md-theme-default md-dense"-->
				<!--																href="#"-->
				<!--																@click.prevent="openTicket(item.number, item.npp)"-->
				<!--												>{{ item.title }}-->
				<!--												</md-button>-->
				<!--										</md-badge>-->
				<!--										<md-button-->
				<!--														class="md-primary md-theme-default md-dense"-->
				<!--														href="#"-->
				<!--														@click.prevent="openTicket(item.number, item.npp)"-->
				<!--														v-if="!calcTicketChanges(item.npp)"-->
				<!--										>{{ item.title }}-->
				<!--										</md-button>-->
				<!--								</md-table-cell>-->
				<!--								<md-table-cell>{{ item.stateName }}</md-table-cell>-->
				<!--								<md-table-cell v-if="SITEUSERS.length > 0">{{ item.userFullName }}</md-table-cell>-->
				<!--								<md-table-cell>{{ item.agent }}</md-table-cell>-->
				<!--								<md-table-cell>{{ item.lastUpdate }}</md-table-cell>-->
				<!--						</md-table-row>-->
				<!--				</md-table>-->

				<!--Таблица с v-model -->
				<!--если есть пользователи сайта аптеки-->
				<md-table
								v-if="hasSiteUsers"
								class="md-table-Tickets"
								:md-height.sync="tableHeight"
								v-model="localInputData"
								md-card
								md-fixed-header
				>
						<md-table-toolbar id="tickets-table-toolbar" class="md-dense">
								<div class="md-toolbar-section-start">
										<div class="md-layout md-gutter md-alignment-center-left" style="margin-top: 4px;width:100%">
												<div class="md-layout-item md-size-15" style="min-width: 150px">
														<md-button
																		tabindex="-1"
																		class="md-dense md-raised md-primary md-theme-default"
																		@click="createTicket"
																		:disabled="HASCREATETICKETTAB"
														>Создать заявку
														</md-button>
												</div>
												<div class="md-layout-item md-size-15">
														<md-field>
																<label>Статус</label>
																<md-select
																				placeholder="Статус"
																				v-model="fltStates"
																				id="FilterStates"
																				md-dense
																				style="margin-right: -10px;"
																>
																		<md-option
																						v-for="state in TICKETSSTATES"
																						v-bind:value="state.name"
																						:key="state.name"
																		>{{state.name}}
																		</md-option>
																</md-select>
																<md-button
																				tabindex="-1"
																				class="md-icon-button md-dense"
																				@click="clearFltStates"
																				v-if="fltStates!=''"
																>
																		<md-icon>clear</md-icon>
																</md-button>
														</md-field>
												</div>
												<!-- <div class="md-layout-item md-size-25"></div>
													<md-field md-clearable>
														<label>Аптека</label>
														<md-input placeholder="Аптека" v-model="fltClients" id="FilterClients" md-dense></md-input>
													</md-field>
												</div>-->
												<div class="md-layout-item md-size-5" style="padding-left: 0px;">
														<md-button
																		class="md-icon-button"
																		id="tickets-update-button"
																		@click="clickUpdate"
																		:disabled="loading"
														>
																<md-icon>refresh</md-icon>
																<md-tooltip md-direction="bottom" md-delay="500">Обновить данные</md-tooltip>
														</md-button>
												</div>
										</div>
								</div>
						</md-table-toolbar>
						<md-table-empty-state md-label="Заявок не найдено" md-description></md-table-empty-state>

						<md-table-row slot="md-table-row" slot-scope="{ item }"
						              :style="`background-color:${calcStateColor(item.stateName)};`">
								<!-- <md-table-cell md-label="Npp" md-numeric>{{ item.npp }}</md-table-cell> -->
								<md-table-cell class="number-column" md-label="Номер" style="width: 5%" md-numeric>{{ item.number }}</md-table-cell>
								<md-table-cell class="icon-column" style="width: 5%" md-label="">
										<md-icon style="color:#FF5252" v-if="calcTicketsWithUserMessages(item.npp)">new_releases</md-icon>
										<md-tooltip md-direction="bottom" md-delay="500">есть новое сообщение в заявке в которой вы участвуете</md-tooltip>
								</md-table-cell>
								<md-table-cell class="theme-column" md-label="Тема" style="width: 35%">
										<span v-if="!calcTicketChanges(item.npp)" @click.prevent="openTicket(item.number, item.npp)">{{ item.title }}</span>
										<md-badge md-dense v-if="calcTicketChanges(item.npp)"
										          :md-content="calcNewMessagesCount(item.npp)">
												<span @click.prevent="openTicket(item.number, item.npp)">{{ item.title }}</span>
										</md-badge>
								</md-table-cell>
								<!-- <md-table-cell md-label="Приоритет">
									<div style="display:flex;">
										<div :style="`height:inherit;width:6px;background-color:#${item.priorityColor};`"></div>
										<div style="margin-left:7px">{{ item.priorityName }}</div>
									</div>
								</md-table-cell> -->
								<md-table-cell md-label="Статус" style="width: 10%">{{ item.stateName }}</md-table-cell>
								<!-- <md-table-cell md-label="Клиент">{{ item.customer }}</md-table-cell> -->
								<md-table-cell md-label="Пользователь" v-if="SITEUSERS.length > 0" style="width: 20%">{{ item.userFullName }}</md-table-cell>
								<md-table-cell md-label="Исполнитель" style="width: 10%">{{ item.agent }}</md-table-cell>
								<!--								<md-table-cell md-label="Дата создания">{{ item.createTime }}</md-table-cell>-->
								<md-table-cell md-label="Обновлено" style="width: 15%">{{ item.lastUpdate }}</md-table-cell>
						</md-table-row>
				</md-table>

				<!--если нет пользователей сайта апетки-->
				<md-table
								v-if="!hasSiteUsers"
								class="md-table-Tickets"
								:md-height.sync="tableHeight"
								v-model="localInputData"
								md-card
								md-fixed-header
				>
						<md-table-toolbar id="tickets-table-toolbar" class="md-dense">
								<div class="md-toolbar-section-start">
										<div class="md-layout md-gutter md-alignment-center-left" style="margin-top: 4px;width:100%">
												<div class="md-layout-item md-size-15" style="min-width: 150px">
														<md-button
																		tabindex="-1"
																		class="md-dense md-raised md-primary md-theme-default"
																		@click="createTicket"
																		:disabled="HASCREATETICKETTAB"
														>Создать заявку
														</md-button>
												</div>
												<div class="md-layout-item md-size-15">
														<md-field>
																<label>Статус</label>
																<md-select
																				placeholder="Статус"
																				v-model="fltStates"
																				id="FilterStates"
																				md-dense
																				style="margin-right: -10px;"
																>
																		<md-option
																						v-for="state in TICKETSSTATES"
																						v-bind:value="state.name"
																						:key="state.name"
																		>{{state.name}}
																		</md-option>
																</md-select>
																<md-button
																				tabindex="-1"
																				class="md-icon-button md-dense"
																				@click="clearFltStates"
																				v-if="fltStates!=''"
																>
																		<md-icon>clear</md-icon>
																</md-button>
														</md-field>
												</div>
												<!-- <div class="md-layout-item md-size-25"></div>
													<md-field md-clearable>
														<label>Аптека</label>
														<md-input placeholder="Аптека" v-model="fltClients" id="FilterClients" md-dense></md-input>
													</md-field>
												</div>-->
												<div class="md-layout-item md-size-5" style="padding-left: 0px;">
														<md-button
																		class="md-icon-button"
																		id="tickets-update-button"
																		@click="clickUpdate"
																		:disabled="loading"
														>
																<md-icon>refresh</md-icon>
																<md-tooltip md-direction="bottom" md-delay="500">Обновить данные</md-tooltip>
														</md-button>
												</div>
										</div>
								</div>
						</md-table-toolbar>
						<md-table-empty-state md-label="Заявок не найдено" md-description></md-table-empty-state>

						<md-table-row slot="md-table-row" slot-scope="{ item }" :style="`background-color:${calcStateColor(item.stateName)};`">
								<md-table-cell class="number-column" md-label="Номер" style="width: 5%" md-numeric>{{ item.number }}</md-table-cell>
								<md-table-cell class="icon-column" style="width: 5%" md-label="">
										<md-icon style="color:#FF5252" v-if="calcTicketsWithUserMessages(item.npp)">new_releases</md-icon>
										<md-tooltip md-direction="bottom" md-delay="500">есть новое сообщение в заявке в которой вы участвуете</md-tooltip>
								</md-table-cell>
								<md-table-cell class="theme-column" md-label="Тема" style="width: 45%">
										<span v-if="!calcTicketChanges(item.npp)" @click.prevent="openTicket(item.number, item.npp)">{{ item.title }}</span>
										<md-badge md-dense v-if="calcTicketChanges(item.npp)"
										          :md-content="calcNewMessagesCount(item.npp)">
												<span @click.prevent="openTicket(item.number, item.npp)">{{ item.title }}</span>
										</md-badge>
								</md-table-cell>
								<md-table-cell md-label="Статус" style="width: 15%">{{ item.stateName }}</md-table-cell>
								<md-table-cell md-label="Исполнитель" style="width: 15%">{{ item.agent }}</md-table-cell>
								<md-table-cell md-label="Обновлено" style="width: 15%">{{ item.lastUpdate }}</md-table-cell>
						</md-table-row>
				</md-table>
		</div>
</template>

<script>
	import {mapGetters} from "vuex";

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
			url_data: "data/GetClientTicketsPagination",
			tableHeight: 700, //сделал обычным свойством, т.к. вычисляемое свойство глючит при переходе с внешней панели вкладок
			// clients: [],
			// clientsLoading: false
		}),
		methods: {
			// getClients(searchTerm) {
			// 	this.clients = new Promise(resolve => {
			// 		if (!searchTerm) {
			// 			resolve([]);
			// 		} else {
			// 			this.clientsLoading = true;
			// 			const term = searchTerm.toLowerCase();
			// 			this.$http.get("data/getClients").then(
			// 				response => {
			// 					resolve(
			// 						response.body.filter(item => item.toLowerCase().includes(term))
			// 					);
			// 				},
			// 				error => {
			// 					resolve(["ошибка при получении данных"]);
			// 				}
			// 			);
			// 			this.clientsLoading = false;
			// 		}
			// 	});
			// },
			createTicket() {
				this.$store.dispatch("ADD_TABB", {
					id: this.LAST_NUMBER,
					Name: "Новая заявка",
					Value: "newTicket",
					Route: "newTicket",
					Pagination: false
				});
				this.$store.dispatch("INCREASE_LAST_NUMBER");
				this.$store.dispatch("SET_HASCREATETICKETTAB", true);
				this.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "newTicket");
				this.$router.push({path: "/support/newTicket"});
			},
			openTicket(num, npp) {
				if (this.OPENEDTICKETSTABS.includes(npp)) {
					this.$store.dispatch(
						"SET_SUPPORTCURRENTTABROUTE",
						"messages?tn=" + npp
					);
					this.$router.push({path: "/support/messages", query: {tn: npp}});
				} else {
					this.$store.dispatch("ADD_TABB", {
						id: this.LAST_NUMBER,
						Name: "Заявка " + num,
						Value: "messages",
						Route: "messages?tn=" + npp,
						Pagination: false,
						Num: npp
						// Npp: npp
					});
					this.$store.dispatch("INCREASE_LAST_NUMBER");
					this.$store.dispatch("ADD_OPENEDTICKETSTABS", npp);
					this.$store.dispatch(
						"SET_SUPPORTCURRENTTABROUTE",
						"messages?tn=" + npp
					);
					this.$router.push({path: "/support/messages", query: {tn: npp}});
				}

				//убираем заявку из массива непросмотренных
				if (this.TICKETSWITHCHANGES.find(x => x.npp === npp)) {
					let index = this.TICKETSWITHCHANGES.findIndex(x => x.npp === npp);
					this.$store.dispatch('REMOVE_TICKETSWHITHCHANGES', index);
				}

				//обновляем в таблице HistoryMessages время последнего просмотра заявки
				//this.$store.dispatch('SET_TICKETLASTUPDATETIME', {ticketNpp: npp, clientNpp: this.CURRENTAPTEKA.npp});
				//обновляем last_update_time таблицы HistoryClientTickets для запуска проверки наличия изменений,
				// которые снимут пометку непросмотрености заявки если одновременно данная заявка открыта в другом браузере
				//this.$store.dispatch('SET_LASTUPDATETIMEFROMSERVER', {clientNpp: this.CURRENTAPTEKA.npp});
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
				}
				this.setCountPagination([row_min, row_max, data_count]);
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
							this.$store.dispatch("showSnackBar", {
								snackText: "Ошибка получения списка заявок"
							});
						}
					)
					.finally(() => {
						this.setParentCountPagination();
						this.loading = false;
						//для корректного рассчета ширины столбцов шапки таблицы
						this.windowsResize();
					});
			},
			clearLoading() {
				this.loading = false;
			},
			clearFltStates() {
				this.fltStates = "";
			},
			clearFltClients() {
				this.fltClients = "";
			},
			clickUpdate() {
				this.setCurrentPage(1);
				this.update();
			},
			calcTicketChanges(ticketNpp) {
				if (this.TICKETSWITHCHANGES.find(x => x.npp === ticketNpp)) {
					return true;
				}
				return false;
			},
			calcStateColor(stateName) {
				switch (stateName) {

					// case "Новая":
					// 	return "darkred";
					// case "В работе":
					// 	return "goldenrod";
					// case "Закрыта":
					// 	return "darkgreen";

					// case "Новая":
					// 	return "red";
					// case "В работе":
					// 	return "yellow";
					// case "Закрыта":
					// 	return "green";

					case "Новая":
						return "#FFD7D7";
					case "В работе":
						return "#FFFFD7";
					case "Закрыта":
						return "#D7FFD7";

					// case "Новая":
					// 	return "#FFEBEB";
					// case "В работе":
					// 	return "#FFFFEB";
					// case "Закрыта":
					// 	return "#EBFFEB";
				}
			},
			calcNewMessagesCount(ticketNpp) {
				let ticketObj = this.TICKETSWITHCHANGES.find(x => x.npp === ticketNpp);
				return ticketObj.newMessagesCount;
			},
			calcTicketsWithUserMessages(ticketNpp) {
				if (this.TICKETSWITHUSERMESSAGES.includes(ticketNpp)) return true;
				return false;
			},
			windowsResize() {
				window.dispatchEvent(new Event('resize'));
			},
			calcTableHeight() {
				let pageHeight = document.documentElement.clientHeight;
				let updateTimeToolbarHeight = document.getElementById("update-time-toolbar")
					.clientHeight;
				let toolbarHeight = document.getElementById("application-toolbar")
					.clientHeight;
				let tabsPanel = document.getElementById("second-tabs-panel");
				let tabsPanelHeight = tabsPanel.clientHeight ? tabsPanel.clientHeight : 48;
				let ticketsTableToolbarHeight = document.getElementById(
					"tickets-table-toolbar"
				).clientHeight;
				let tableHead = document.querySelector(".md-table-fixed-header");
				let tableHeadheight = tableHead.clientHeight ? tableHead.clientHeight : 56;
				let paddingMarginHeight = 60;

				// console.log('pageHeight = ' + pageHeight);
				// console.log('toolbarHeight = ' + toolbarHeight);
				// console.log('tabsPanelHeight = ' + tabsPanelHeight);
				// console.log('ticketsTableToolbarHeight = ' + ticketsTableToolbarHeight);
				// console.log('tableHeadheight = ' + tableHeadheight);
				// console.log('paddingMarginHeight = ' + paddingMarginHeight);
				// console.log('Итого = '  + (pageHeight - toolbarHeight - tabsPanelHeight - ticketsTableToolbarHeight - tableHeadheight - paddingMarginHeight));

				this.$store.dispatch("setApp", {
					tableHeight:
						pageHeight -
						updateTimeToolbarHeight -
						toolbarHeight -
						tabsPanelHeight -
						ticketsTableToolbarHeight -
						tableHeadheight -
						paddingMarginHeight
				});
				this.tableHeight = this.$store.state.app.tableHeight;
				// this.tableHeight = pageHeight - updateTimeToolbarHeight - toolbarHeight - tabsPanelHeight - paddingMarginHeight;
			}
		},
		computed: {
			...mapGetters([
				"LAST_NUMBER",
				"TICKETS",
				"HASCREATETICKETTAB",
				"OPENEDTICKETSTABS",
				"SUPPORTTABS",
				"TICKETSINPUTDATA",
				"TICKETSFIRSTLOAD",
				"TICKETSSTATES",
				// "TICKETSCLIENTS",
				"TICKETSFILTERSTATES",
				"TICKETSFILTERCLIENTS",
				"CURRENTAPTEKA",
				"TICKETSLISTLASTUPDATETIME",
				"TICKETSWITHCHANGES",
				'TICKETSWITHUSERMESSAGES',
				'SITEUSERS'
			]),
			localInputData: {
				get() {
					return this.TICKETSINPUTDATA;
				},
				set(value) {
					this.$store.dispatch("SET_TICKETSINPUTDATA", value);
				}
			},
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
					filterStates: this.fltStates,
					// filterClients: this.fltClients,
					row_num_begin: (currentPage - 1) * rowsPerPage + 1,
					row_num_end: currentPage * rowsPerPage,
					apteka_npp: this.CURRENTAPTEKA.npp
				};
			},
			fltStates: {
				get() {
					return this.TICKETSFILTERSTATES;
				},
				set(value) {
					this.$store.dispatch("SET_TICKETSFILTERSTATES", value);
				}
			},
			fltClients: {
				get() {
					return this.TICKETSFILTERCLIENTS;
				},
				set(value) {
					this.$store.dispatch("SET_TICKETSFILTERCLIENTS", value);
				}
			},
			hasSiteUsers() {
				if (this.SITEUSERS.length > 0) return true;
				return false;
			}
			// tableHeight: {
			// 	get() { return this.$store.state.app.tableHeight; },
			// 	set(value) { this.$store.dispatch('setApp', { 'tableHeight': value }); }
			// }
		},
		created() {
			//console.log('created tickets');
		},
		updated() {
			//console.log('updated tickets');
		},
		beforeUpdate() {
			//console.log('beforeUpdate tickets');
		},
		mounted() {
			// console.log('mounted tickets');

			//подписка на событие расчета высоты таблицы
			this.$root.$on('calcTicketsTableHeight', this.calcTableHeight);

			//список статусов заявок
			if (this.TICKETSSTATES.length === 0)
				this.$store.dispatch("GET_TICKETS_STATES");

			//список клиентов
			// if (this.TICKETSCLIENTS.length == 0)
			// 	this.$store.dispatch("GET_TICKETS_CLIENTS");

			//устанавливаем данные для пагинации
			if (this.TICKETSFIRSTLOAD === true) {
				this.$store.dispatch("SET_TICKETSFIRSTLOAD", false);
				this.setCurrentPage(1);
				this.update();
			} else {
				//   this.localInputData = this.TICKETSINPUTDATA;
				this.loading = true;
				this.setParentCountPagination();
				window.setTimeout(this.clearLoading, 500);
			}
			this.$root.$on("SUPPORTUPDATE", this.update);

			//расчет высоты таблицы
			this.calcTableHeight();
		},
		beforeDestroy: function () {
			// console.log("tickets beforeDestroy");
			this.$root.$off("SUPPORTUPDATE", this.update);
			this.$root.$off('calcTicketsTableHeight', this.calcTableHeight);
			this.$store.dispatch("SET_TICKETSINPUTDATA", this.localInputData);
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

		// .md-table {
		// 	margin: 0;
		// }

		.md-table-Tickets {
				margin: 0 !important;
		}

		#tickets-table-toolbar {
				height: 52px;
		}

		a:hover {
				cursor: pointer;
		}

		button {
				padding: 5px;
		}
</style>

<style lang="css">

		.md-table-cell-container {
				padding: 0 10px;
		}

		.icon-column .md-table-cell-container {
				padding: 0;
				width: 24px;
				/*max-width: 24px;*/
				margin: auto;
		}

		th.md-table-head {
				text-align: left;
		}

		.theme-column .md-table-cell-container {
				padding-left: 0;
		}

		.theme-column .md-badge {
				right: -20px;
				top: -10px;
		}

		.theme-column span {
				color: #448AFF;
				padding: 5px 0;
		}

		.theme-column span:hover {
				cursor: pointer;
		}

		/*.icon-column .md-table-cell-container i:hover {*/
		/*		cursor: pointer;*/
		/*}*/

		.md-table-head-label {
				padding: 0 0 0 10px;
		}

		.md-table-fixed-header {
				width: 100%;
		}

</style>