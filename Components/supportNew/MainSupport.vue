<template>
		<drawer>
				<!-- модальное окно для сохранения измененных значений приоритета и владельца -->
				<md-dialog :md-active.sync="showDialog">
						<p class="dialog">
								Данные в заявке {{ ticketNum }} изменены.
								<br>Сохранить?
						</p>
						<md-dialog-actions>
								<md-button class="md-primary" @click="saveChangesFromDialog(tabId, ticketNum)">Да</md-button>
								<md-button class="md-primary" @click="closeTabFromDialog(tabId, ticketNum)">Нет</md-button>
								<md-button class="md-primary" @click="showDialog = false">Отмена</md-button>
						</md-dialog-actions>
				</md-dialog>

				<!-- модальное окно для сохранения неотправленного нового сообщения -->
				<md-dialog :md-active.sync="showDialogForNewMessage">
						<p class="dialog">
								Сообщение в заявке {{ ticketNum }} не отправлено.
								<br>Отправить?
						</p>
						<md-dialog-actions>
								<md-button
												class="md-primary"
												@click="saveChangesFromNewMessageDialog(tabId, ticketNum, newMessage)"
								>Да
								</md-button>
								<md-button class="md-primary" @click="closeTabFromNewMessageDialog(tabId, ticketNum)">Нет</md-button>
								<md-button class="md-primary" @click="showDialogForNewMessage = false">Отмена</md-button>
						</md-dialog-actions>
				</md-dialog>

				<span v-if="DATABASEVERSION === 'test'" style="position: fixed;bottom: 15px;right: 15px;z-index: 1000;opacity: 0.5;font-size: 1rem;font-weight: bold;">тестовая база</span>

				<md-toolbar id="update-time-toolbar" class="md-dense" md-elevation="0" style="padding: 0 10px; background: transparent;">
						<div class="md-toolbar-section-start">
								<span class="md-title">Обновлено</span>
								<span>
								<transition name="fadedate">
									<span class="md-title" style="margin-left: 10px" v-show="showDate">{{(new Date(timeOfLastCheck)).toLocaleTimeString('ru')}}</span>
								</transition>
						</span>
								<span style="position: fixed; left: 215px; z-index: 10;">
                <md-badge class="md-primary" md-dense :md-content.sync="badgeTimer">
                    <md-button class="md-icon-button md-dense md-primary" id="time-update-button" @click="manualAutoUpdate">
                        <md-icon>refresh</md-icon>
                        <md-tooltip md-direction="top" md-delay="500">Обновить данные</md-tooltip>
                    </md-button>
                </md-badge>
             </span>
						</div>
						<div class="md-toolbar-section-end" v-if="CURRENTUSER.fullName">
								<span>пользователь:</span>
								<span style="margin-left: 10px">{{CURRENTUSER.fullName}}</span>
						</div>
				</md-toolbar>
				<md-toolbar
								id="mainsupport-toolbar"
								class="md-dense"
								md-elevation="0"
								style="padding: 0; background: transparent;"
				>
						<div class="md-toolbar-section-start" style="width: 100%">
								<md-tabs
												id="second-tabs-panel"
												:md-active-tab="activeTab"
												@md-changed="tabChanged"
												style="padding-left:0;"
								>
										<template slot="md-tab" slot-scope="{ tab }">
												<span v-if="calcTicketChanges(tab.data.tn)" class="tab-with-badge">
														{{ tab.label }}
														<i class="tab-badge md-badge md-theme-default md-dense">{{ calcNewMessagesCount(tab.data.tn) }}</i>
												</span>
												<span v-else>
														{{ tab.label }}
												</span>
												<div id="cl" v-if="tab.data.tabId > 0">
														<button
																		id="close-button"
																		@click.prevent.stop="sendCloseTabEvent(tab.data.tabId, tab.data.tn)"
														>
																<span>&times;</span>
														</button>
														<md-tooltip md-direction="top">Закрыть вкладку</md-tooltip>
												</div>
										</template>
										<md-tab
														v-for="tab in tabs"
														:id="tab.Route"
														:md-label="tab.Name"
														:to="tab.Route"
														:key="tab.id"
														:md-template-data="{ tabId: tab.id, tn: tab.Num }"
										></md-tab>
								</md-tabs>

								<div class="md-table-pagination">
										<div v-if="pagination" class="md-table-pagination" id="support-table-pagination">
												<span class="md-table-pagination-label">Строк на странице:</span>
												<md-field>
														<md-select
																		@md-selected="updateRowsPerPage"
																		v-model="rowsPerPage"
																		md-dense
																		md-class="md-pagination-select"
														>
																<md-option v-for="amount in pageSize" :key="amount" :value="amount">{{ amount }}</md-option>
														</md-select>
												</md-field>
												<span>{{ currentPageMinCount }}-{{ currentPageMaxCount }} из {{ currentDataCount }}</span>
												<md-button
																class="md-icon-button md-table-pagination-previous"
																@click="goToPrevious()"
																:disabled="currentPage === 1 || lastPage === 0 || loading"
												>
														<md-icon>keyboard_arrow_left</md-icon>
												</md-button>
												<md-button
																class="md-icon-button md-table-pagination-next"
																@click="goToNext()"
																:disabled="currentPage == lastPage || lastPage === 0 || loading"
												>
														<md-icon>keyboard_arrow_right</md-icon>
												</md-button>
										</div>
								</div>
						</div>
				</md-toolbar>
				<div id="tickets-progress-bar">
						<md-progress-bar md-mode="query" v-show="loading"></md-progress-bar>
				</div>
				<component
								v-bind:is="currentTabComponent"
								class="tab"
								id="current-tab"
								@closeTab="closeTab"
								@createTicket="createTicket"
				></component>
				<div style="display: flex;justify-content: center;position: fixed;bottom: 0;width: 99%">
						<span style="font-size: 0.9rem;opacity: 0.7;">&copy; «ООО КВЕРТИ+», {{new Date().getFullYear()}}</span>
				</div>
		</drawer>
</template>

<script>
	import Drawer from "../common/Drawer.vue";
	// import Home from "./home.vue";
	import Tickets from "./tickets.vue";
	// import Archive from "./archive.vue";
	import newTicket from "./newTicket.vue";
	import messages from "./messages.vue";
	import NotFound from "../views/NotFound.vue";
	import {mapGetters} from "vuex";
	import axios from "axios";

	export default {
		components: {
			Drawer,
			// Home,
			Tickets,
			// Archive,
			newTicket,
			messages,
			NotFound
		},
		name: "MainSupport",
		props: {},
		data: () => ({
			currentPageMinCount: 0,
			currentPageMaxCount: 0,
			currentDataCount: 0,
			pageSize: [10, 15, 20, 25],
			loading: false,
			url_data: "data/GetClientTicketsPagination",
			showDialog: false,
			tabId: 0, //обновляем при нажатии кнопки закрыть вкладку
			ticketNum: 0, //обновляем при нажатии кнопки закрыть вкладку
			timerId: 0,
			badgeTimer: 9,
			updateTimeTimerId: 0,
			timeOfLastCheck: 0,
			showDate: true,
			localInputData: [],
			// activePriority: "",
			// activeAgent: "",
			// selectedPriority: "",
			// selectedAgent: "",
			newMessage: "",
			showDialogForNewMessage: false
			// isCloseButtonVisible: false
		}),
		watch: {},
		computed: {
			...mapGetters([
				"SUPPORTTABS",
				"SUPPORTCURRENTTAB",
				"CURRENTTABID",
				"SUPPORTCURRENTTABROUTE",
				"LAST_NUMBER",
				"TICKETS",
				// "AGENTS",
				"TICKETSINPUTDATA",
				"HASCREATETICKETTAB",
				"OPENEDTICKETSTABS",
				"TICKETSROWSPERPAGE",
				"TICKETSROWSPERPAGEOLD",
				"TICKETSCURRENTPAGE",
				"TICKETSFILTERSTATES",
				"TICKETSFILTERCLIENTS",
				"CURRENTAPTEKA",
				"TICKETSLISTLASTUPDATETIME",
				"TICKETSWITHCHANGES",
				'TICKETSWITHUSERMESSAGES',
				'CURRENTUSER',
				'USERID',
				'DATABASEVERSION'
			]),
			currentPage: {
				get() {
					return this.TICKETSCURRENTPAGE;
				},
				set(value) {
					this.$store.dispatch("SET_TICKETSCURRENTPAGE", value);
				}
			},
			rowsPerPage: {
				get() {
					return this.TICKETSROWSPERPAGE;
				},
				set(value) {
					this.$store.dispatch("SET_TICKETSROWSPERPAGE", value);
				}
			},
			rowsPerPageOld: {
				get() {
					return this.TICKETSROWSPERPAGEOLD;
				},
				set(value) {
					this.$store.dispatch("SET_TICKETSROWSPERPAGEOLD", value);
				}
			},
			tabs: {
				get() {
					return this.SUPPORTTABS;
				}
			},
			currentTab: {
				get() {
					let param = this.$route.params.id;
					if (param === "messages") {
						return this.$store.getters["SUPPORTCURRENTTAB"](
							this.$route.fullPath.split("/")[2]
						);
					}
					return this.$store.getters["SUPPORTCURRENTTAB"](this.$route.params.id); //определяем компонент который надо загрузить
				}
			},
			pagination: {
				get() {
					return this.$store.getters["SUPPORTCURRENTPAGINATION"](
						this.$route.params.id
					);
				}
			},
			currentTabComponent() {
				return this.currentTab;
			},
			activeTab() {
				let arr = this.SUPPORTCURRENTTABROUTE.split("/");
				return arr[arr.length - 1];
			},
			lastPage() {
				let cnt = this.currentDataCount;
				let i = parseInt(cnt / this.rowsPerPage);
				if (i * this.rowsPerPage < cnt) {
					i++;
				}
				return parseInt(i);
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
			// fltClients: {
			// 	get() {
			// 		return this.TICKETSFILTERCLIENTS;
			// 	},
			// 	set(value) {
			// 		this.$store.dispatch("SET_TICKETSFILTERCLIENTS", value);
			// 	}
			// },
		},

		methods: {
			sendCloseTabEvent(tab_id, ticketNumber) {
				this.tabId = tab_id;
				this.ticketNum = ticketNumber;
				this.closeTab(tab_id, ticketNumber);
			},
			// showCloseButton() {
			// 	this.isCloseButtonVisible = true;
			// },
			// hideCloseButton() {
			// 	this.isCloseButtonVisible = false;
			// },
			createTicket() {
				// console.log("createTicket");
				this.$http
					.get(this.url_data, {
						params: this.paramsData
					})
					.then(
						response => {
							let data = response.body;

							//обновляем список заявок
							this.$store.dispatch("SET_TICKETSINPUTDATA", data);

							//обновляем пагинацию
							let row_min = 0;
							let data_count = 0;
							let row_max = 0;
							let ln = data.length;
							if (ln > 0) {
								row_min = data[0]["rowNum"];
								data_count = data[0]["maxRow"];
								row_max = data[ln - 1]["rowNum"];
							}
							this.setCountPagination([row_min, row_max, data_count]);
						},
						error => {
							console.error(error);
							this.$store.dispatch("showSnackBar", {
								snackText: `Ошибка получения списка заявок`
							});
						}
					);
			},
			getPriorityNpp(priorityName) {
				switch (priorityName) {
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
			},
			// savePriority(ticketNumber, newPriority) {
			// 	const params = {
			// 		npp: ticketNumber,
			// 		priority: this.getPriorityNpp(newPriority)
			// 	};
			//
			// 	this.$http
			// 		.post("data/SetPriority", params, {emulateJSON: true})
			// 		.then(
			// 			() => this.updateTicketsInputData(this.paramsData),
			// 			error => console.error(error)
			// 		);
			// },
			// saveAgent(ticketNumber, newAgent) {
			// 	let agentNpp;
			// 	for (let agent of this.AGENTS) {
			// 		if (agent.name === newAgent) {
			// 			agentNpp = agent.id;
			// 		}
			// 	}
			// 	const params = {
			// 		npp: ticketNumber,
			// 		agentNpp: agentNpp
			// 	};
			// 	this.$http
			// 		.post("data/SetAgent", params, {emulateJSON: true})
			// 		.then(
			// 			() => this.updateTicketsInputData(this.paramsData),
			// 			error => console.error(error)
			// 		);
			// },
			sendMessage(ticketNumber, newMessage) {
				return axios.post(
					"data/saveMessage",
					JSON.stringify({
						newMessage: newMessage,
						ticketNumber: ticketNumber,
						clientNpp: this.CURRENTAPTEKA.npp
					}),
					{headers: {"Content-Type": "application/json"}}
				);
			},
			// updateTicketsInputData(params) {
			// 	this.$http
			// 		.get("data/getticketspagination", {
			// 			params: params
			// 		})
			// 		.then(
			// 			response => {
			// 				this.$store.dispatch(
			// 					"SET_TICKETSINPUTDATA",
			// 					Array.from(response.body)
			// 				);
			// 			},
			// 			error => console.error(error)
			// 		);
			// },
			saveChangesFromDialog(tab_id, ticketNumber) {
				let storage = JSON.parse(localStorage.getItem("Ticket" + ticketNumber));
				if (storage) {
					if (storage.changes.includes("priority"))
						this.savePriority(ticketNumber, storage.selectedPriority);
					if (storage.changes.includes("agent"))
						this.saveAgent(ticketNumber, storage.selectedAgent);
				}
				this.$store.dispatch("showSnackBar", {
					snackText: "Изменения сохранены"
				});
				this.closeTabFromDialog(tab_id, ticketNumber);
			},
			saveChangesFromNewMessageDialog(tab_id, ticketNumber, newMessage) {
				//отправляем сообщение
				this.sendMessage(ticketNumber, newMessage)
					.then(resp => {
						if (resp.data.statusCode > 200) {
							this.$store.dispatch("showSnackBar", {
								snackText: `Произошла ошибка при записи сообщения!`
							});
							console.error(resp.data.reasonPhrase);
							return;
						}
						//обновляем в таблице HistoryMessages время последнего просмотра заявки
						this.$store.dispatch('SET_TICKETLASTUPDATETIME', {
							ticketNpp: ticketNumber,
							clientNpp: this.CURRENTAPTEKA.npp,
							userId: this.USERID
						});

						//генерируем событие для обновления списка сообщений на странице messages
						this.$root.$emit("updateTicketMessagesFromParent");

						//закрываем модальное окно
						this.showDialogForNewMessage = false;

						this.$store.dispatch("showSnackBar", {
							snackText: "Сообщение отправлено"
						});

						//если изменен приоритет или владелец
						if (
							this.activePriority != this.selectedPriority ||
							this.activeAgent != this.selectedAgent
						) {
							//показываем модальное окно
							this.showDialog = true;
							//прекращаем выполнение этого метода и передаем управление в модальное окно сохранения изменений приоритета и владельца
							return;
						}

						//закрываем вкладку
						this.closeTabFromDialog(tab_id, ticketNumber);
					})
					.catch(error => console.error(error));
			},
			closeTabFromDialog(tab_id, ticketNumber) {
				//делаем на странице /support/tickets активной ссылку на данную заявку
				for (let index in this.OPENEDTICKETSTABS) {
					if (this.OPENEDTICKETSTABS[index] === ticketNumber) {
						this.$store.dispatch("REMOVE_OPENEDTICKETSTABS", index);
					}
				}

				//закрываем вкладку
				for (let i = 0; i < this.SUPPORTTABS.length; i++) {
					if (this.SUPPORTTABS[i].id === tab_id) {
						this.$store.dispatch("CLOSE_TABB", i);
					}
				}

				//удаляем данные вкладки из localStorage
				localStorage.removeItem("Ticket" + ticketNumber);

				//переходим на вкладку со списком заявок
				this.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "tickets");
				this.$router.push({path: "/support/tickets"});

				//закрываем модальное окно
				this.showDialog = false;
			},
			closeTabFromNewMessageDialog(tab_id, ticketNumber) {
				//закрываем модальное окно
				this.showDialogForNewMessage = false;

				//если изменен приоритет или владелец
				if (
					this.activePriority !== this.selectedPriority ||
					this.activeAgent !== this.selectedAgent
				) {
					//показываем модальное окно
					this.showDialog = true;
					//прекращаем выполнение этого метода и передаем управление в модальное окно сохранения изменений приоритета и владельца
					return;
				}

				//закрываем вкладку
				this.closeTabFromDialog(tab_id, ticketNumber);
			},
			closeTab(tab_id, ticketNumber = null) {
				//если закрываем вкладку с сообщениями
				if (ticketNumber) {
					let storage = JSON.parse(localStorage.getItem("Ticket" + ticketNumber));

					//если на вкладке есть несохраненные изменения
					if (storage) {
						this.getMessages(ticketNumber)
							.then(response => {
								let messages = response.data;
								// this.activePriority = messages[0].priority;
								// this.activeAgent = messages[0].agent;
								// this.selectedPriority = storage.selectedPriority;
								// this.selectedAgent = storage.selectedAgent;
								this.newMessage = storage.newMessage;

								//если есть неотправленное сообщение
								if (this.newMessage !== "") {
									//показываем модальное окно
									this.showDialogForNewMessage = true;
									//прекращаем выполнение этого метода и передаем управление в модальное окно отправки сообщения
									return;
								}

								//если изменен приоритет или владелец
								if (
									this.activePriority !== this.selectedPriority ||
									this.activeAgent !== this.selectedAgent
								)
								//показываем модальное окно
									this.showDialog = true;
							})
							.catch(error => console.error(error));
					} else {
						//делаем на странице /support/tickets активной ссылку на данную заявку
						for (let index in this.OPENEDTICKETSTABS) {
							if (this.OPENEDTICKETSTABS[index] === ticketNumber) {
								this.$store.dispatch("REMOVE_OPENEDTICKETSTABS", index);
							}
						}

						//закрываем вкладку
						for (let i = 0; i < this.SUPPORTTABS.length; i++) {
							if (this.SUPPORTTABS[i].id === tab_id) {
								this.$store.dispatch("CLOSE_TABB", i);
							}
						}

						//переходим на вкладку со списком заявок
						this.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "tickets");
						this.$router.push({path: "/support/tickets"});
					}
				} else {
					//если закрываем вкладку создания новой заявки, то делаем активной кнопку "Создать заявку"
					if (this.HASCREATETICKETTAB)
						this.$store.dispatch("SET_HASCREATETICKETTAB", false);

					//закрываем вкладку
					for (let i = 0; i < this.SUPPORTTABS.length; i++) {
						if (this.SUPPORTTABS[i].id === tab_id) {
							this.$store.dispatch("CLOSE_TABB", i);
						}
					}

					//переходим на вкладку со списком заявок
					this.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "tickets");
					this.$router.push({path: "/support/tickets"}); //переходим на вкладку с заявками
				}

				//генерация события для рассчета высоты таблицы с заявками
				if (this.$route.params.id === 'tickets') {
					setTimeout(() => this.$root.$emit("calcTicketsTableHeight"), 0); //иначе глючит, DOM не успевает обновиться
				}
			},
			getMessages(ticketNumber) {
				return axios.post(
					"data/getMessages",
					JSON.stringify({ticketNumber: ticketNumber}),
					{
						headers: {"Content-Type": "application/json"}
					}
				);
			},
			tabChanged(value) {
				// console.log("tabChanged()");
				//value это атрибут :id в md-tab
				let idx = this.tabs.findIndex(x => x.Route === value);
				if (idx === -1) {
					idx = 0;
				}
				this.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", this.tabs[idx].Route);
				this.$store.dispatch("SET_CURRENTTABID", this.tabs[idx].id);

				//убираем номер заявки из массива непросмотренных
				let ticketNpp = this.tabs[idx].Num;
				if (ticketNpp) { //если это вкладка с заявкой
					if (this.TICKETSWITHCHANGES.find(x => x.npp === ticketNpp)) {
						let index = this.TICKETSWITHCHANGES.findIndex(x => x.npp === ticketNpp);
						this.$store.dispatch('REMOVE_TICKETSWHITHCHANGES', index);
					}

					//обновляем время в таблицах через 10 сек., для загрузки новых данных
					//задержка 10 сек. необходима для показа новых сообщений в заявке
					setTimeout(() => this.$root.$emit("updateTimeFromParent"), 10000);

					//обновляем в таблице HistoryMessages время последнего просмотра заявки
					//this.$store.dispatch('SET_TICKETLASTUPDATETIME', {ticketNpp: ticketNpp, clientNpp: this.CURRENTAPTEKA.npp});

					//обновляем время последних изменений для запуска цикла проверки
					//this.$store.dispatch('SET_LASTUPDATETIMEFROMSERVER', {clientNpp: this.CURRENTAPTEKA.npp});
				}
			},
			update() {
				//   console.log("MainSupport update()");
				this.$root.$emit("SUPPORTUPDATE");
			},
			updateMessages() {
				// console.log("MainSupport updateMessages()");
				this.$root.$emit("updateMessagesFromParent");
			},
			autoUpdate() {
				console.log(`MainSupport autoupdate()`);
				//когда открыта вкладка с заявками
				if (this.$route.path.indexOf('tickets') !== -1) {
					this.update();//обновляем информацию в списке заявок (localInputData в tickets.vue)

					//когда открыта вкладка с сообщениями
				} else if (this.$route.path.indexOf('messages') !== -1) {
					this.updateMessages();

					//необходимо для обновления в store TICKETSINPUTDATA (обнвление информации для вкладки tickets)
					this.loading = true;
					this.$http
						.get('data/GetClientTicketsPagination', {
							params: this.paramsData
						})
						.then(
							response => {
								this.localInputData = Array.from(response.body);
								this.$store.dispatch("SET_TICKETSINPUTDATA", this.localInputData);

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
							() => {
								this.$store.dispatch("showSnackBar", {
									snackText: "Ошибка получения списка заявок"
								});
							}
						)
						.finally(() => {
							// this.setCountPagination();
							this.loading = false;
						});
				} else {
				}
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
				this.update();
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
			},
			getLastUpdateTimeFromServer(clientNpp, userId) {
				return this.$http.get("data/GetLastUpdateTimeFromServer", {
					params: {clientNpp: clientNpp, userId: userId}
				});
			},
			getLastUpdateTimeOnClient(clientNpp, userId) {
				return this.$http.get("data/GetLastUpdateTimeOnClient", {
					params: {clientNpp: clientNpp, userId: userId}
				});
			},
			setLastUpdateTimeOnClient(clientNpp, userId) {
				const params = {
					clientNpp: clientNpp,
					userId: userId
				};
				this.$http.put("data/SetLastUpdateTimeOnClient", params, {emulateJSON: true}).then(
					() => {

					},
					error => {
						console.error(error);
						this.$store.dispatch("showSnackBar", {
							snackText: `Ошибка при записи времени последних изменений в браузере клиента`
						});
					}
				);
			},
			GetAllTicketsWithChanges(clientNpp, userId) {
				return this.$http.get("data/GetAllTicketsWithChanges", {
					params: {apteka_npp: clientNpp, userId: userId}
				});
			},
			getUserTicketsWithChanges(clientNpp, userId) {
				return this.$http.get("data/GetUserTicketsWithChanges", {
					params: {apteka_npp: clientNpp, userId: userId}
				});
			},
			geTicketsWithUserMessages(clientNpp, userId) {
				return this.$http.get("data/GeTicketsWithUserMessages", {
					params: {apteka_npp: clientNpp, userId: userId}
				});
			},
			checkForChanges() {
				this.GetAllTicketsWithChanges(this.CURRENTAPTEKA.npp, this.USERID).then(
					response => {
						let tickets = response.body;
						//если есть изменения
						if (tickets.length > 0) {
							this.$store.dispatch('SET_TICKETSWHITHCHANGES', []);
							for (let ticket of tickets) {
								console.log(`есть изменения в заявке ${ticket.number}`);
								this.$store.dispatch('ADD_TICKETSWHITHCHANGES', ticket);
								// this.notify(`Есть новое сообщение в заявке ${ticket.number}`, +new Date());
							}
						} else {
							//очищаем массив заявок с изменениями
							this.$store.dispatch('SET_TICKETSWHITHCHANGES', []);
						}

						//запрашиваем с сервера обновленные данные
						this.autoUpdate();

						//обновляем время в хранилище
						localStorage.setItem(`LastUpdate_${this.CURRENTAPTEKA.idapt}_${this.USERID}`, (+new Date()).toString());
						//обновляем время в приложении
						this.$store.dispatch('SET_TICKETSLISTLASTUPDATETIME', +new Date());
						//обновляем время в таблице HistoryClientTickets
						this.setLastUpdateTimeOnClient(this.CURRENTAPTEKA.npp, this.USERID);
					},
					error => console.error(error)
				);

				//выводим уведомления для непросмотренных заявок в которых участвует пользователь
				this.getUserTicketsWithChanges(this.CURRENTAPTEKA.npp, this.USERID).then(
					response => {
						let tickets = response.body;
						//если есть заявки пользователя с изменениями
						if (tickets.length > 0) {
							for (let ticket of tickets) {
								console.log(`есть изменения в заявке пользователя ${ticket.number}`);
								this.notify(`Есть новое сообщение в заявке ${ticket.number}`, +new Date());
							}
						}
					},
					error => console.error(error)
				);

				//помечаем заявки в которых участвует пользователь (т.е. заявки  в которых есть сообщение от пользователя)
				this.geTicketsWithUserMessages(this.CURRENTAPTEKA.npp, this.USERID).then(
					response => {
						let tickets = response.body;
						this.$store.dispatch('SET_TICKETSWITHUSERMESSAGES', []);
						if (tickets.length > 0) {
							for (let ticket of tickets) {
								this.$store.dispatch('ADD_TICKETSWITHUSERMESSAGES', ticket.npp);
							}
						}
					},
					error => console.error(error)
				);
			},
			checkForUpdate() {
				//автопроверка на необходимость обновления данных с сервера
				this.timerId = setInterval(() => {
					this.getLastUpdateTimeFromServer(this.CURRENTAPTEKA.npp, this.USERID).then(
						response => {
							let serverTime = response.body;
							if (serverTime !== "") {
								serverTime = +new Date(serverTime); //переводим дату в миллисекунды

								console.log(`serverTime > this.TICKETSLISTLASTUPDATETIME = ${serverTime > this.TICKETSLISTLASTUPDATETIME}`);

								if (serverTime > this.TICKETSLISTLASTUPDATETIME) {
									//обновляем список сообщений
									console.log('обновляем данные');
									this.checkForChanges();
								}
							}
						},
						error => console.error(error)
					);

					//запускаем отсчет времени в бадже
					this.startTimerDuration();

				}, 10000);
			},
			notify(message, tag) {
				// Let's check if the browser supports notifications
				if (!("Notification" in window)) {
					console.log("This browser does not support system notifications");
				}

				// Let's check whether notification permissions have already been granted
				else if (Notification.permission === "granted") {
					// If it's okay let's create a notification
					new Notification("Уведомление QB", {
						body: message,
						requireInteraction: false,
						tag: tag,
						// icon: $('#notify_icon').prop('src'),
						renotify: false
					});
				}

				// Otherwise, we need to ask the user for permission
				else {
					Notification.requestPermission().then(function (permission) {
						// If the user accepts, let's create a notification
						if (permission === "granted") {
							new Notification("Уведомление QB", {
								body: message,
								requireInteraction: false,
								tag: tag,
								// icon: $('#notify_icon').prop('src'),
								renotify: false
							});
						}
					});
				}

				//else if (Notification.permission !== 'denied') {
				//	Notification.requestPermission().then(function (permission) {
				//		// If the user accepts, let's create a notification
				//		if (permission === "granted") {
				//			let notification = new Notification("Уведомление OTRS", {
				//				body: message,
				//				requireInteraction: false,
				//				tag: tag,
				//				icon: $('#notify_icon').prop('src'),
				//				renotify: false
				//			});
				//		}
				//	});
				//}
			},
			startTimerDuration() {
				this.timeOfLastCheck = +new Date();
				clearInterval(this.updateTimeTimerId);
				this.badgeTimer = 9;
				this.showDate = true;
				this.updateTimeTimerId = setInterval(() => {
					if (this.badgeTimer !== 0) this.badgeTimer--;
					if (this.badgeTimer === 0) this.showDate = false;
				}, 1000);
			},
			manualAutoUpdate() {
				// console.log(`manualAutoUpdate()`);

				//запускаем проверку на наличие изменений
				this.checkForChanges();

				//запускаем отсчет времени в бадже
				this.showDate = false; //чтобы сработала анимация перехода при обновлении времени
				clearInterval(this.timerId);
				this.startTimerDuration();

				//запускаем в цикле с интервалом 10 сек. автопроверку на необходимость обновления данных с сервера
				this.checkForUpdate();
			},
			calcTicketChanges(ticketNpp) {
				if (this.TICKETSWITHCHANGES.find(x => x.npp === ticketNpp)) {
					return true;
				}
				return false;
			},
			calcNewMessagesCount(ticketNpp) {
				let ticketObj = this.TICKETSWITHCHANGES.find(x => x.npp === ticketNpp);
				return ticketObj.newMessagesCount;
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
			};
		},
		created() {
			// console.log("created MainSupport");
		},
		mounted() {
			// console.log("MainSupport mounted()");

			//вычисляем userId
			let userId = this.CURRENTUSER.userId ? this.CURRENTUSER.userId : 0;
			this.$store.dispatch('SET_USERID', userId);

			//дополняем массив вкладок вкладками из localStorage для сохранения открытых вкладок при обновлении страницы
			if (localStorage.openedTabs) {
				let arr1 = [];
				let arr2 = [];
				let tabs = JSON.parse(localStorage.openedTabs);

				for (let item of this.SUPPORTTABS) {
					arr1.push(item.id);
				}

				for (let item of tabs) {
					arr2.push(item.id);
				}

				function diff(a1, a2) {
					return a1
						.filter(i => a2.indexOf(i) < 0)
						.concat(a2.filter(i => a1.indexOf(i) < 0));
				}

				let arr3 = diff(arr1, arr2); //убираем из массива статические вкладки
				if (arr3.length > 0) {
					for (let tab of tabs) {
						for (let item of arr3) {
							if (tab.id == item) {
								let num = tab.Num ? tab.Num : null;
								let route = tab.Route;
								this.$store.dispatch("ADD_TABB", {
									id: this.LAST_NUMBER,
									Name: tab.Name,
									Value: tab.Value,
									Route: tab.Route,
									Pagination: tab.Pagination,
									Num: num
								});
								this.$store.dispatch("INCREASE_LAST_NUMBER");
								if (num) {
									this.$store.dispatch("ADD_OPENEDTICKETSTABS", num);
								}
								if (route === "newTicket") {
									this.$store.dispatch("SET_HASCREATETICKETTAB", true);
								}
							}
						}
					}
				}
			}

			//логика для проверки наличия изменений в заявках
			//получаем и устанавливаем время последнего обновления данных на клиенте

			let storage = localStorage.getItem(`LastUpdate_${this.CURRENTAPTEKA.idapt}_${this.USERID}`);
			if (storage) {
				this.$store.dispatch('SET_TICKETSLISTLASTUPDATETIME', parseInt(storage));
			} else {
				this.getLastUpdateTimeOnClient(this.CURRENTAPTEKA.npp, this.USERID).then(
					response => {
						let serverTime = response.body;
						if (serverTime !== "") {
							serverTime = +new Date(serverTime);
							this.$store.dispatch('SET_TICKETSLISTLASTUPDATETIME', serverTime);
							localStorage.setItem(`LastUpdate_${this.CURRENTAPTEKA.idapt}_${this.USERID}`, serverTime.toString());
						} else {
							this.$store.dispatch('SET_TICKETSLISTLASTUPDATETIME', +new Date());
							localStorage.setItem(`LastUpdate_${this.CURRENTAPTEKA.idapt}_${this.USERID}`, (+new Date()).toString());
						}
					},
					error => {
						console.error(error);
						this.$store.dispatch('SET_TICKETSLISTLASTUPDATETIME', +new Date());
						localStorage.setItem(`LastUpdate_${this.CURRENTAPTEKA.idapt}_${this.USERID}`, (+new Date()).toString());
					}
				);
			}

			//запускаем процедуру проверки наличия изменений и необходимости обновления данных, цикл автопроверки !!!обязательно только после установки TICKETSLISTLASTUPDATETIME
			this.manualAutoUpdate();

			//заполняем массив пользователей сайта данной аптеки
			this.$store.dispatch('GET_SITEUSERS', this.CURRENTAPTEKA.idapt);

			//определяем версию БД
			this.$store.dispatch('GET_DATABASEVERSION');
		},
		beforeDestroy() {
			clearInterval(this.timerId);
			clearInterval(this.updateTimeTimerId);
			// clearInterval(this.notifyTimerId);
		},
		beforeRouteEnter(to, from, next) {
			// вызывается до подтверждения пути, соответствующего этому компоненту.
			// НЕ ИМЕЕТ доступа к контексту экземпляра компонента `this`,
			// так как к моменту вызова экземпляр ещё не создан!

			// console.log("beforeRouteEnter");

			//передаем callback в next для доступа к контексту экземпляра компонента `this`
			next(vm => {
				// экземпляр компонента доступен как `vm`

				//добавляем и открываем вкладку "Новая заявка"
				if (vm.$route.params.id === "newTicket" && !vm.HASCREATETICKETTAB) {
					vm.$store.dispatch("ADD_TABB", {
						id: vm.LAST_NUMBER,
						Name: "Новая заявка",
						Value: "newTicket",
						Route: "newTicket",
						Pagination: false
					});
					vm.$store.dispatch("INCREASE_LAST_NUMBER");
					vm.$store.dispatch("SET_HASCREATETICKETTAB", true);
					vm.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "newTicket");
					vm.$router.push({path: "/support/newTicket"});
				} else if (vm.$route.params.id === "newTicket" && vm.HASCREATETICKETTAB) {
					vm.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "newTicket");
					//открываем вкладку "Список заявок"
				} else if (vm.$route.params.id === "tickets") {
					vm.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "tickets");
				}

				//добавляем и открываем вкладку с содержанием заявки
				if (vm.$route.params.id === "messages") {
					//признак того что переход на страницу выполнен напрямую по ссылке
					if (vm.TICKETS.length === 0) {
						vm.$http
							.get("data/GetClientTickets", {
								params: {apteka_npp: vm.CURRENTAPTEKA.npp}
							})
							.then(
								function (response) {
									let npp = vm.$route.query.tn;
									let tickets = response.body;
									let tabs = vm.SUPPORTTABS;
									let flag = false; //признак того что запрошенная вкладка уже есть в массиве SUPPORTTABS
									for (let tab of tabs) {
										if (tab.Num == npp) {
											flag = true;
										}
									}

									for (let ticket of tickets) {
										//проверяем что заявка с запрошенным номером существует
										if (ticket.npp == npp && !flag) {
											vm.$store.dispatch("ADD_TABB", {
												id: vm.LAST_NUMBER,
												Name: "Заявка " + npp,
												Value: "messages",
												Route: "messages?tn=" + npp,
												Pagination: false,
												Num: npp
											});
											vm.$store.dispatch("INCREASE_LAST_NUMBER");
											vm.$store.dispatch("ADD_OPENEDTICKETSTABS", npp);
											vm.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "messages?tn=" + npp);
											vm.$router.push({path: "/support/messages", query: {tn: npp}});
										} else {
											vm.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "messages?tn=" + npp);
										}
									}
									vm.$store.dispatch("GET_TICKETS", vm.CURRENTAPTEKA.npp);
								},
								error => {
									console.error(error);
									vm.$store.dispatch("showSnackBar", {
										snackText: `Ошибка получения списка заявок`
									});
								}
							);
					}
				}
			});
		},
		beforeRouteUpdate(toR, fromR, next) {
			//console.log('beforeRouteUpdate MainSupport');

			// вызывается когда маршрут, что рендерит этот компонент изменился,
			// но этот компонент будет повторно использован в новом маршруте.
			// Например, для маршрута с динамическими параметрами `/foo/:id`, когда мы
			// перемещаемся между `/foo/1` и `/foo/2`, экземпляр того же компонента `Foo`
			// будет использован повторно, и этот хук будет вызван когда это случится.
			// Также имеется доступ в `this` к экземпляру компонента.

			//console.log('beforeRouteUpdate');
			next();
		}
	};
</script>

<style lang="scss" scoped>
		.tab {
				border: 1px solid #ccc;
				padding: 10px;
		}

		.md-tab {
				padding: 0px;
				margin: 0;
		}

		.md-table-pagination {
				border-top: 0;
				height: 48px;
				min-width: 320px;
		}

		#tickets-progress-bar {
				height: 5px;
				max-height: 5px;
				padding-top: 5px;
				padding-left: 5px;
				padding-right: 5px;
				padding-bottom: 0px;
		}

		// .material-icons.md-18 {
		// 	font-size: 18px;
		// }

		// #second-tabs-panel{
		// 	max-width: 500px;
		// 	overflow-y: auto;
		// }

		#close-button {
				display: inline-block;
				width: 100%;
				height: 100%;
				margin: 0;
				padding: 0;
				font-size: 22px;
				line-height: 24px;
				color: #448aff;
				// color: rgba(0, 0, 0, 0.7);
				background-color: transparent;
				border-radius: 12px;
				border: none;
				z-index: 10;
		}

		#close-button:hover {
				cursor: pointer;
				background-color: #448aff;
				color: white;
				// transform: scale(1.5);
		}

		#cl {
				height: 24px;
				width: 24px;
				background-color: transparent;
				position: absolute;
				top: 0;
				right: 0;
		}

		.dialog {
				text-indent: 0;
				margin: 20px 20px 0 20px;
		}

		.md-dialog {
				max-width: 768px;
		}

		.fadedate-enter-active, .fadedate-leave-active {
				transition: opacity .5s;
		}

		/* .fade-leave-active до версии 2.1.8 */
		.fadedate-enter, .fadedate-leave-to {
				opacity: 0;
		}

		.tab-badge {
				right: 20px;
				bottom: 27px;
		}

		.tab-with-badge {
				margin-right: 16px;
		}

</style>
<style lang="css">
		#support-table-pagination .md-field {
				margin-left: 5px;
				margin-right: 5px;
		}

		#support-table-pagination .md-button {
				margin-left: 0;
				margin-right: 0;
		}

		#second-tabs-panel {
				width: calc(100% - 320px);
		}

		#second-tabs-panel .md-tabs-navigation {
				flex-wrap: wrap;
		}

		#second-tabs-panel .md-tabs-navigation span.md-tabs-indicator {
				display: none;
		}

		#second-tabs-panel a.router-link-exact-active {
				border-bottom: 2px solid #448AFF;
		}

</style>