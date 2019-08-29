<template>
		<div id="ticket-messages">
				<md-toolbar
								id="messages-toolbar"
								class="md-dense md-transparent"
								md-elevation="2"
								style="padding: 0 10px;margin-bottom: 10px"
				>
						<div class="md-toolbar-section-start">
								<!-- модальное окно для сохранения измененных значений приоритета и владельца -->
								<!-- <md-dialog :md-active.sync="showDialog">
									<p class="dialog">Данные изменены.
										<br>Сохранить?
									</p>
									<md-dialog-actions>
										<md-button class="md-primary" @click="saveChangesFromDialog(CURRENTTABID, tn)">Да</md-button>
										<md-button class="md-primary" @click="closeTab(CURRENTTABID, tn)">Нет</md-button>
										<md-button class="md-primary" @click="showDialog = false">Отмена</md-button>
									</md-dialog-actions>
								</md-dialog>-->
								<!-- модальное окно для сохранения неотправленного нового сообщения -->
								<md-dialog :md-active.sync="showDialogForNewMessage">
										<p class="dialog">
												Сообщение не отправлено.
												<br>Отправить?
										</p>
										<md-dialog-actions>
												<md-button class="md-primary" @click="sendMessage()">Да</md-button>
												<md-button class="md-primary" @click="toggleShowTextarea()">Нет</md-button>
												<md-button class="md-primary" @click="showDialogForNewMessage = false">Отмена</md-button>
										</md-dialog-actions>
								</md-dialog>
								<!-- <md-field style="width:350px;margin-left:10px" v-show="activeState !== 'new'">
									<label for="agent">Владелец</label>
									<md-select
										placeholder="Изменить владельца"
										id="agent"
										name="agent"
										v-model="selectedAgent"
										@md-selected="toggleChanges"
										md-dense
										:style="`border-bottom: 3px solid ${agentBackgroundColor};`"
									>
										<md-option v-for="agent in AGENTS" :key="agent.id" :value="agent.name">{{agent.name}}</md-option>
									</md-select>
								</md-field>-->
								<!-- <md-button
									type="button"
									class="md-icon-button md-dense md-raised md-primary md-theme-default"
									@click="saveAgent()"
									v-show="activeState !== 'new'"
								>
									<md-icon>save</md-icon>
									<md-tooltip md-direction="top" md-delay="500">Сохранить владельца</md-tooltip>
								</md-button>-->
								<!-- <md-field style="width:180px;margin-left:10px">
									<label for="priority">Приоритет</label>
									<md-select
										placeholder="Изменить приоритет"
										id="priority"
										name="priority"
										v-model="selectedPriority"
										@md-selected="toggleChanges"
										md-dense
										:style="`border-bottom: 3px solid ${priorityBackgroundColor};border-left: 10px solid #${priorityBorderLeftColor};padding-left: 5px;`"
									>
										<md-option
											v-for="priority in PRIORITIES"
											:key="priority.id"
											:value="priority.name"
											:style="`border-left: 10px solid #${priority.color};`"
										>{{priority.name}}</md-option>
									</md-select>
								</md-field>-->
								<!-- <md-button
									type="button"
									class="md-icon-button md-dense md-raised md-primary md-theme-default"
									@click="savePriority()"
								>
									<md-icon>save</md-icon>
									<md-tooltip md-direction="top" md-delay="500">Сохранить приоритет</md-tooltip>
								</md-button>-->
								<!-- <md-button
									type="button"
									class="md-dense md-raised md-theme-default"
									:class="`${saveButtonColor}`"
									@click="saveChanges()"
								>Сохранить</md-button>-->

								<!-- <md-button
									type="button"
									style="margin-left:10px"
									class="md-icon-button md-dense md-raised md-primary"
									@click="lockTicket()"
									v-show="activeState === 'new'"
								>
									<md-icon>lock</md-icon>
									<md-tooltip md-direction="top" md-delay="500">Принять в работу</md-tooltip>
								</md-button>-->

								<div style="font-size:1rem" v-if="isChangeTicketTitle" key="input">
										<b>Тема:</b>
										<md-field style="width:300px;display:inline-block;">
												<md-input
																type="text"
																placeholder="Введите тему"
																v-model.lazy.trim="newTicketTitle"
																style="width:inherit;"
												/>
										</md-field>
								</div>
								<div style="font-size:1rem" v-if="!isChangeTicketTitle" key="noinput">
										<b>Тема:</b>
										{{ticketTitle}}
								</div>
								<md-button
												type="button"
												style="margin-left:10px"
												class="md-icon-button md-dense md-primary"
												@click="changeTicketTitle()"
												v-show="!isChangeTicketTitle"
								>
										<md-icon>create</md-icon>
										<md-tooltip md-direction="top" md-delay="500">Изменить тему</md-tooltip>
								</md-button>
								<md-button
												type="button"
												style="margin-left:10px"
												class="md-icon-button md-dense md-primary"
												@click="saveNewTitle(tn, newTicketTitle)"
												v-show="isChangeTicketTitle"
												:disabled="newTicketTitle === '' || newTicketTitle === ticketTitle"
								>
										<md-icon>done</md-icon>
										<md-tooltip md-direction="top" md-delay="500">Сохранить</md-tooltip>
								</md-button>
								<md-button
												type="button"
												style="margin-left:10px"
												class="md-icon-button md-dense md-accent"
												@click="isChangeTicketTitle = !isChangeTicketTitle"
												v-show="isChangeTicketTitle"
								>
										<md-icon>close</md-icon>
										<md-tooltip md-direction="top" md-delay="500">Отмена</md-tooltip>
								</md-button>

								<!--								<md-button-->
								<!--												class="md-icon-button"-->
								<!--												id="tickets-update-button"-->
								<!--												@click="clickUpdate"-->
								<!--												:disabled="loading"-->
								<!--								>-->
								<!--										<md-icon>refresh</md-icon>-->
								<!--										<md-tooltip md-direction="bottom" md-delay="500">Обновить данные</md-tooltip>-->
								<!--								</md-button>-->

								<!-- <md-button
									type="button"
									style="margin-left:10px"
									class="md-icon-button md-dense md-raised md-primary"
									@click="startTimer()"
									v-show="activeState !== 'new'"
								>
									<md-icon>play_arrow</md-icon>
									<md-tooltip md-direction="top" md-delay="500">Начать выполнение</md-tooltip>
								</md-button>
								<md-button
									type="button"
									style="margin-left:10px"
									class="md-icon-button md-dense md-raised md-primary md-theme-default"
									@click="stopTimer()"
									v-show="activeState !== 'new'"
								>
									<md-icon>stop</md-icon>
									<md-tooltip md-direction="top" md-delay="500">Остановить выполнение</md-tooltip>
								</md-button>-->
						</div>

						<!--						<div class="md-toolbar-section-end">-->
						<!--								<md-button-->
						<!--												type="button"-->
						<!--												style="margin-left:10px"-->
						<!--												class="md-button md-dense md-raised md-primary md-theme-default"-->
						<!--												@click="toggleShowTextarea()"-->
						<!--												:disabled="showTextarea"-->
						<!--								>-->
						<!--										Добавить сообщение-->
						<!--										&lt;!&ndash; <md-icon>add_comment</md-icon>-->
						<!--										<md-tooltip md-direction="top" md-delay="500">Добавить сообщение</md-tooltip>&ndash;&gt;-->
						<!--								</md-button>-->
						<!--								&lt;!&ndash; <md-button-->
						<!--									type="button"-->
						<!--									class="md-icon-button md-dense md-primary md-theme-default"-->
						<!--									@click="checkUnsavedData(CURRENTTABID, tn)"-->
						<!--								>-->
						<!--									<md-icon>close</md-icon>-->
						<!--									<md-tooltip md-direction="top" md-delay="500">Закрыть вкладку</md-tooltip>-->
						<!--								</md-button>&ndash;&gt;-->
						<!--						</div>-->
				</md-toolbar>

				<!--				<div-->
				<!--								id="message-field"-->
				<!--								style="display:flex;align-items:flex-end;position:relative"-->
				<!--								v-show="showTextarea"-->
				<!--				>-->
				<!--						<md-field style="margin:0;margin-right:10px">-->
				<!--								<md-textarea-->
				<!--												style="resize:none;"-->
				<!--												id="message"-->
				<!--												placeholder="Текст сообщения"-->
				<!--												v-model.lazy.trim="newMessage"-->
				<!--												@change="toggleChanges()"-->
				<!--												rows="5"-->
				<!--								></md-textarea>-->
				<!--						</md-field>-->
				<!--						<md-button-->
				<!--										class="md-icon-button md-dense"-->
				<!--										@click="checkUnsavedNewMessage()"-->
				<!--										style="position:absolute;top:0;right:0;"-->
				<!--						>-->
				<!--								<md-icon>close</md-icon>-->
				<!--								<md-tooltip md-direction="top" md-delay="500">Закрыть окно сообщения</md-tooltip>-->
				<!--						</md-button>-->
				<!--						<md-button-->
				<!--										class="md-icon-button md-raised md-primary md-theme-default"-->
				<!--										@click="sendMessage"-->
				<!--										:disabled="!newMessage"-->
				<!--						>-->
				<!--								<md-icon>mail</md-icon>-->
				<!--								<md-tooltip md-direction="top" md-delay="500">Отправить</md-tooltip>-->
				<!--						</md-button>-->
				<!--				</div>-->

				<ul id="mlist" class="md-content md-scrollbar md-theme-default" :style="{height: tableHeight + 'px'}" @mousemove="move">
						<li
										v-for="message of messages"
										class="mes"
										:class="calcTypeOfMessage(message.messageAgent)"
										:style="calcNewMessage(message.messageNpp)"
										:key="message.messagesNpp"
						>
								<span v-if="message.messageAgent !== ''" class="message-title">
										{{ message.createTime }} {{message.messageAgent}}
								</span>
								<span v-if="message.messageClient !== ''" class="message-title">
										{{ message.createTime }} {{message.messageClient}}
										<span v-if="message.userFullName !== ''">({{message.userFullName}})</span>
										<span class="done"><md-icon :style="`color:${calcSeenedByAgentsMessage(message.messageNpp, message.messageClient)};`">done</md-icon></span>
								</span>
								<pre>{{message.text}}</pre>
								<div class="attachments" v-if="message.attachmentsData !== ''">
										<md-icon style="color: #000;margin: 0">attach_file</md-icon>
										<span>Прикрепленные файлы:</span>
										<md-button
														class="md-icon-button md-primary"
														v-for="attachment of calcMessageAttachments(message.attachmentsData)"
														:key="attachment.name"
														@click="getAttachmentFile(attachment)"
										>
												<md-icon :md-src="`${calcTypeOfAttachment(attachment)}`"/>
												<md-tooltip md-direction="bottom">{{attachment.file_name}} &#8195; {{bytesToSize(attachment.file_size)}}</md-tooltip>
										</md-button>
										<!--										<md-button class="md-icon-button md-primary">-->
										<!--												<md-icon md-src="/dist/svg/doc.svg"/>-->
										<!--										</md-button>-->
										<!--										<md-button class="md-icon-button md-primary">-->
										<!--												<md-icon md-src="/dist/svg/pdf.svg"/>-->
										<!--										</md-button>-->
								</div>
								<!--								<span id="done" style="display: flex"><md-icon v-if="calcSeenedByAgentsMessage(message.messageNpp, message.messageClient)">done</md-icon></span>-->
								<!--								<md-button-->
								<!--												class="md-button md-dense md-primary"-->
								<!--												v-if="message.messageAgent !== '' && message.isSeen === false"-->
								<!--												@click="setIsSeen(message.messageNpp)"-->
								<!--								>прочитано-->
								<!--								</md-button>-->
						</li>
				</ul>

				<!--прикрепление файлов-->
				<md-field style="display: none">
						<label>Прикрепить файлы</label>
						<md-file id="file-input" @md-change="onFileUpload($event)" multiple/>
				</md-field>
				<transition name="fadeattachments">
						<div class="attachments-container" v-if="files">
								<div class="attachments-item" v-for="file of files" :key="file.name">
										<md-icon :md-src="`${calcTypeOfAttachmentForAttachedFiles(file)}`" class="md-size-2x"/>
										<md-tooltip md-direction="bottom">{{file.name}} &#8195; {{bytesToSize(file.size)}}</md-tooltip>
										<span>{{file.name}} &nbsp; {{bytesToSize(file.size)}}</span>
										<span class="remove-attachment" @click="removeAttachment(file)">&times;</span>
								</div>
						</div>
				</transition>

				<!--окно ввода сообщения-->
				<div
								id="message-field"
								style="display:flex;align-items:flex-end;position:relative"
								v-show="showTextarea"
				>
						<md-field style="margin:0;margin-right:10px">
								<md-textarea
												style="resize:none;"
												id="message"
												placeholder="Текст сообщения"
												v-model.lazy.trim="newMessage"
												@change="toggleChanges()"
												rows="5"
								></md-textarea>
						</md-field>
						<!--						<md-button-->
						<!--										class="md-icon-button md-dense"-->
						<!--										@click="checkUnsavedNewMessage()"-->
						<!--										style="position:absolute;top:0;right:0;"-->
						<!--						>-->
						<!--								<md-icon>close</md-icon>-->
						<!--								<md-tooltip md-direction="top" md-delay="500">Закрыть окно сообщения</md-tooltip>-->
						<!--						</md-button>-->
						<div>
								<div id="attachments-link">
										<md-icon style="color: #000;">attach_file</md-icon>
										<span @click="clickFileInput()">Прикрепить файлы</span>
								</div>
								<md-button
												class="md-button md-dense md-raised md-primary md-theme-default"
												@click="sendMessage"
												:disabled="!newMessage"
								>Отправить
										<!--								<md-icon>mail</md-icon>-->
										<!--								<md-tooltip md-direction="top" md-delay="500">Отправить</md-tooltip>-->
								</md-button>
						</div>
				</div>
		</div>
</template>

<script>
	import axios from "axios";
	import {mapGetters} from "vuex";

	export default {
		components: {},
		name: "Messages",
		props: [],
		inject: ["getRowsPerPage", "getCurrentPage", "getLoading", "setLoading"],
		data: () => ({
			messages: [],
			newMessages: [],
			seenedByAgentsMessages: [],
			tableHeight: 700,
			newMessage: "",
			tn: 0,
			isChangeTicketTitle: false,
			newTicketTitle: "",
			// selectedPriority: "",
			// selectedAgent: "",
			showTextarea: true,
			changes: [],
			// showDialog: false,
			showDialogForNewMessage: false,
			timerId: 0,
			mouseMoveCounter: 0,
			firstLoad: true,
			files: [], //прикрепленные файлы
			// notifyTimerId: 0,
			// lastUpdateTime: ""
			// changedElemBackgroundColor: "#FF5252",
			// priorityBackgroundColor: "transparent",
			// priorityBorderLeftColor: "",
			// agentBackgroundColor: "transparent",
			// saveButtonColor: "md-primary"
		}),

		computed: {
			...
				mapGetters([
					"CURRENTTABID",
					"OPENEDTICKETSTABS",
					"PRIORITIES",
					"CURRENTAPTEKA",
					'TICKETSWITHCHANGES',
					'CURRENTUSER',
					'USERID',
					'KNOWNFILETYPES'
					// "TICKETSLISTLASTUPDATETIME"
					// "AGENTS",
					// "AGENT"
				]),
			// activePriority() {
			// 	if (this.messages.length == 0) return "";
			// 	return this.messages[0].priority;
			// },
			// activeAgent() {
			// 	if (this.messages.length == 0) return "";
			// 	return this.messages[0].agent;
			// },
			// activeState() {
			// 	if (this.messages.length == 0) return "";
			// 	return this.messages[0].state;
			// },
			npp() {
				if (this.messages.length == 0) return "";
				return this.messages[0].ticketNpp;
			},
			ticketNumber() {
				if (this.messages.length == 0) return "";
				return this.messages[0].ticketNumber;
			},
			isChanged() {
				if (this.changes.length == 0) return false;
				return true;
			},
			// priority() {
			// 	switch (this.selectedPriority) {
			// 		case "самый низкий":
			// 			return "very_low";
			// 		case "низкий":
			// 			return "low";
			// 		case "обычный":
			// 			return "normal";
			// 		case "высокий":
			// 			return "hight";
			// 		case "безотлагательный":
			// 			return "very_hight";
			// 	}
			// },
			loading: {
				get() {
					return this.getLoading();
				}
				,
				set(value) {
					this.setLoading(value);
				}
			},
			ticketTitle: {
				get() {
					if (this.messages.length == 0) return "";
					return this.messages[0].ticketTitle;
				}
			},
			// hasNewMessages() {
			// 	return this.messages.some(message => message.isSeen === false && message.messageAgent !== '')
			// },
			hasNewMessages() {
				return this.newMessages.length > 0;
			},
			attachmentsData() {
				let attacmentsData = [];
				this.files.forEach(file => {
					let {name: file_name, size: file_size, type: file_type} = file;
					attacmentsData.push({file_name, file_size, file_type});
				});
				return attacmentsData;
			}
		},

		methods: {
			sendMessage() {
				axios
					.post(
						"data/saveMessage",
						JSON.stringify({
							newMessage: this.newMessage,
							ticketNumber: this.tn,
							clientNpp: this.CURRENTAPTEKA.npp,
							userId: this.USERID,
							attachmentsData: this.attachmentsData.length > 0 ? JSON.stringify(this.attachmentsData) : '',
							idapt: this.CURRENTAPTEKA.idapt,
							number: this.ticketNumber,
							state: this.messages[0].state,
							title: this.ticketTitle
						}),
						{headers: {"Content-Type": "application/json"}}
					)
					.then(resp => {
						if (resp.data.statusCode > 200) {
							this.$store.dispatch("showSnackBar", {
								snackText: `Произошла ошибка при записи сообщения!`
							});
							console.error(resp.data.reasonPhrase);
							return;
						}

						let string = resp.data.reasonPhrase;
						let arr = string.split(';');
						let messageNpp = arr[1];

						//сохраняем вложения на сервер
						if (this.files.length > 0) {
							this.loading = true;
							this.uploadFilesToServer(this.files, this.npp, messageNpp)
								.then(response => {
									console.log(response);
								}, error => {
									console.error(error);
									this.$store.dispatch("showSnackBar", {snackText: `Произошла ошибка при сохранении вложений на сервер.`});
								})
								.finally(() => {
									//очищаем массив вложений
									this.files = [];
									this.scrollMessagesListToBottom();
									this.loading = false;
								});
						}

						//обновляем в таблице HistoryMessages время последнего просмотра заявки
						this.$store.dispatch('SET_TICKETLASTUPDATETIME', {
							ticketNpp: this.npp,
							clientNpp: this.CURRENTAPTEKA.npp,
							userId: this.USERID
						});

						this.$store.dispatch("showSnackBar", {
							snackText: "Сообщение отправлено"
						});

						this.getMessages(this.tn)
							.then(response => {
								this.messages = response.data;
								this.toggleShowTextarea();
								this.calcHeight();
								//прокручиваем список сообщений вниз
								this.scrollMessagesListToBottom();
							})
							.catch(error => {
								console.error(error);
								this.$store.dispatch("showSnackBar", {
									snackText: "Произошла ошибка при получении списка сообщений"
								});
							});
					})
					.catch(error => {
						console.error(error);
						this.$store.dispatch("showSnackBar", {
							snackText: "Произошла ошибка при записи нового сообщения"
						});
					});
			},
			// closeTab(tab_id, ticketNumber) {
			// 	//закрываем вкладку
			// 	this.$emit("closeTab", tab_id);

			// 	//удаляем данные вкладки из localStorage
			// 	localStorage.removeItem("Ticket" + this.tn);

			// 	//делаем на странице /support/tickets активной ссылку на данную заявку
			// 	for (let index in this.OPENEDTICKETSTABS) {
			// 		if (this.OPENEDTICKETSTABS[index] === ticketNumber) {
			// 			this.$store.dispatch("REMOVE_OPENEDTICKETSTABS", index);
			// 		}
			// 	}
			// },
			// checkUnsavedData(tab_id, ticketNumber) {
			// 	if (this.changes.length > 0) {
			// 		if (this.changes.some(elem => elem === "newMessage")) {
			// 			this.showDialogForNewMessage = true;
			// 			return; //костыль, если есть несохраненное сообщение, то сначала разбираемся с ним (сохраняем, отменяем изменения, дописываем и т.д.),
			// 			//а после этого разбираемся с оставшимися изменениями. Сложность возникает из-за модального окна - пока в нем кликаем, код метода уже выполнен.
			// 			//В дальнейшем надо придумать что-то более корректное.
			// 		}
			// 		if (
			// 			this.changes.some(elem => elem === "priority") ||
			// 			this.changes.some(elem => elem === "agent")
			// 		)
			// 			this.showDialog = true;
			// 	} else {
			// 		this.closeTab(tab_id, ticketNumber);
			// 	}
			// },
			checkUnsavedNewMessage() {
				if (this.changes.some(elem => elem === "newMessage")) {
					this.showDialogForNewMessage = true;
				} else {
					this.toggleShowTextarea();
				}
			},
			// savePriority() {
			// 	const params = {
			// 		npp: this.npp,
			// 		priority: this.priority
			// 	};
			//
			// 	this.$http.post("data/SetPriority", params, {emulateJSON: true}).then(
			// 		() => {
			// 			this.getMessages(this.tn)
			// 				.then(response => {
			// 					this.messages = response.data;
			// 					this.$emit("createTicket"); //обновление списка заявок в store
			// 					// this.updateTicketsInputData();
			// 				})
			// 				.catch(error => console.error(error));
			// 		},
			// 		error => console.error(error)
			// 	);
			// },
			// saveAgent() {
			// 	let agentNpp;
			// 	for (let agent of this.AGENTS) {
			// 		if (agent.name === this.selectedAgent) {
			// 			agentNpp = agent.id;
			// 		}
			// 	}
			// 	const params = {
			// 		npp: this.npp,
			// 		agentNpp: agentNpp
			// 	};
			// 	this.$http.post("data/SetAgent", params, {emulateJSON: true}).then(
			// 		() => {
			// 			this.getMessages(this.tn)
			// 				.then(response => {
			// 					this.messages = response.data;
			// 					this.$emit("createTicket"); //обновление списка заявок в store
			// 					// this.updateTicketsInputData();
			// 				})
			// 				.catch(error => console.error(error));
			// 		},
			// 		error => console.error(error)
			// 	);
			// },
			// lockTicket() {
			// 	const params = {
			// 		npp: this.npp,
			// 		state: "open",
			// 		agentNpp: this.AGENT
			// 	};
			//
			// 	this.$http.post("data/SetState", params, {emulateJSON: true}).then(
			// 		() => {
			// 			this.getMessages(this.tn)
			// 				.then(response => {
			// 					this.messages = response.data;
			// 					this.$emit("createTicket"); //обновление списка заявок в store
			// 					// this.updateTicketsInputData();
			// 					this.selectedAgent = this.activeAgent;
			// 					this.$store.dispatch("showSnackBar", {
			// 						snackText: "Заявка взята в работу"
			// 					});
			// 				})
			//
			// 				.catch(error => console.error(error));
			// 		},
			// 		error => console.error(error)
			// 	);
			// },
			startTimer() {
				console.log("Запущен таймер");
			},
			stopTimer() {
				console.log("Остановлен таймер");
			},
			toggleShowTextarea() {
				// this.showTextarea = !this.showTextarea;
				this.showDialogForNewMessage = false;
				this.newMessage = ""; //очищаем окно сообщения перед toggleChanges()
				this.toggleChanges(); //нужно для удаления записи из localStorage
				this.calcHeight();
			},
			calcHeight() {
				//расчет высоты чата
				let pageHeight = document.documentElement.clientHeight;
				let updateTimeToolbarHeight = document.getElementById("update-time-toolbar")
					.clientHeight;
				let toolbar = document.getElementById("application-toolbar");
				let toolbarHeight = toolbar.clientHeight;
				let tabsPanel = document.getElementById("second-tabs-panel");
				let tabsPanelHeight = tabsPanel.clientHeight
					? tabsPanel.clientHeight
					: 48;
				let messagesToolbar = document.getElementById("messages-toolbar");
				let messagesToolbarHeight = messagesToolbar.clientHeight
					? messagesToolbar.clientHeight
					: 84;
				let messageField = document.getElementById("message-field");
				let messageFieldHeight = 0;
				let paddingMarginHeight = 90;
				if (this.showTextarea) {
					messageFieldHeight = messageField.clientHeight;
					// paddingMarginHeight = 195;
				}
				// let attachmentsLinkHeight = document.getElementById("attachments-link").clientHeight;
				let attachmentsContainer = document.getElementsByClassName('attachments-container')[0];
				let attachmentsContainerHeight = attachmentsContainer.clientHeight ? attachmentsContainer.clientHeight + 20 : 0;

				this.$store.dispatch("setApp", {
					tableHeight:
						pageHeight -
						updateTimeToolbarHeight -
						toolbarHeight -
						tabsPanelHeight -
						messagesToolbarHeight -
						messageFieldHeight -
						// attachmentsLinkHeight -
						attachmentsContainerHeight -
						paddingMarginHeight
				});
				this.tableHeight = this.$store.state.app.tableHeight;
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
			updateTicketMessagesFromParent() {
				this.getMessages(this.tn)
					.then(response => {
						this.messages = response.data;
						// this.showTextarea = false;
						// this.newMessage = '';
						this.toggleShowTextarea();
						this.toggleChanges(); //проверяем наличие изменений на странице
					})
					.catch(error => console.error(error));
			},
			updateTicketsInputData() {
				let paramsData = {
					row_num_begin: (this.getCurrentPage() - 1) * this.getRowsPerPage() + 1,
					row_num_end: this.getCurrentPage() * this.getRowsPerPage()
				};
				this.$http
					.get("data/GetClientTicketsPagination", {
						params: paramsData
					})
					.then(
						response => {
							this.$store.dispatch(
								"SET_TICKETSINPUTDATA",
								Array.from(response.body)
							);
						},
						error => console.error(error)
					);
			},
			saveChanges() {
				if (this.changes.some(elem => elem === "priority")) {
					this.savePriority();
					let index = this.changes.findIndex(elem => elem === "priority");
					this.changes.splice(index, 1);
				}
				if (this.changes.some(elem => elem === "agent")) {
					this.saveAgent();
					let index = this.changes.findIndex(elem => elem === "agent");
					this.changes.splice(index, 1);
				}
				this.$store.dispatch("showSnackBar", {
					snackText: "Изменения сохранены"
				});
				this.priorityBackgroundColor = "transparent";
				this.agentBackgroundColor = "transparent";
				this.saveButtonColor = "md-primary";
			},
			// saveChangesFromDialog(tab_id, ticketNumber) {
			// 	this.saveChanges();
			// 	this.closeTab(tab_id, ticketNumber);
			// },
			calcPriorityBorderLeftColor(selectedPriority) {
				for (let priority of this.PRIORITIES) {
					if (priority.name == selectedPriority) return priority.color;
				}
				return "";
			},
			// closeTabFromParent() {
			// 	// console.log("messages closeTabFromParent()");
			// 	this.checkUnsavedData(this.CURRENTTABID, this.tn);
			// },
			toggleChanges() {
				//записываем изменения приоритета
				// if (this.activePriority !== this.selectedPriority) {
				// 	if (this.changes.every(elem => elem !== "priority")) {
				// 		this.changes.push("priority");
				// 		this.priorityBackgroundColor = this.changedElemBackgroundColor; //выделяем измененный элемент
				// 	}
				// } else {
				// 	let index = this.changes.findIndex(elem => elem === "priority");
				// 	if (index != -1) {
				// 		this.changes.splice(index, 1);
				// 		this.priorityBackgroundColor = "transparent"; //убираем выделение
				// 	}
				// }

				//записываем изменения владельца
				// if (this.activeAgent !== this.selectedAgent) {
				// 	if (this.changes.every(elem => elem !== "agent")) {
				// 		this.changes.push("agent");
				// 		this.agentBackgroundColor = this.changedElemBackgroundColor; //выделяем измененный элемент
				// 	}
				// } else {
				// 	let index = this.changes.findIndex(elem => elem === "agent");
				// 	if (index != -1) {
				// 		this.changes.splice(index, 1);
				// 		this.agentBackgroundColor = "transparent"; //убираем выделение
				// 	}
				// }

				//записываем текст из окна ввода нового сообщения
				if (this.newMessage !== "") {
					if (this.changes.every(elem => elem !== "newMessage")) {
						this.changes.push("newMessage");
					}
				} else {
					let index = this.changes.findIndex(elem => elem === "newMessage");
					if (index !== -1) {
						this.changes.splice(index, 1);
						// this.agentBackgroundColor = "transparent"; //убираем выделение
					}
				}

				//записываем измененные значения в localStorage, для сохранения состояния при перемещении по вкладкам
				if (this.changes.length > 0) {
					let data = {
						changes: this.changes,
						tn: this.tn,
						// selectedPriority: this.selectedPriority,
						// selectedAgent: this.selectedAgent,
						newMessage: this.newMessage,
						showTextarea: this.showTextarea
					};
					localStorage.setItem("Ticket" + this.tn, JSON.stringify(data));
					// if (
					// 	this.changes.some(elem => elem === "agent") ||
					// 	this.changes.some(elem => elem === "priority")
					// ) {
					// 	this.saveButtonColor = "md-accent"; //меняем цвет кнопки "Сохранить"
					// } else {
					// 	this.saveButtonColor = "md-primary"; //срабатывает если из измеений есть только новое несохраненное сообщение, поэтому кнопку "Сохранить" не красим
					// }
				} else {
					localStorage.removeItem("Ticket" + this.tn);
					// this.saveButtonColor = "md-primary"; //меняем цвет кнопки "Сохранить"
				}

				//вычисляем цвет приоритета
				// this.priorityBorderLeftColor = this.calcPriorityBorderLeftColor(
				// 	this.selectedPriority
				// );
			},
			recoverUnsavedDataFromLocalStorage() {
				//восстанавливаем на вкладке несохраненные изменения
				let storage = JSON.parse(localStorage.getItem("Ticket" + this.tn));
				if (storage) {
					// this.selectedPriority = storage.selectedPriority;
					// this.selectedAgent = storage.selectedAgent;
					this.newMessage = storage.newMessage;
					this.showTextarea = storage.showTextarea;
					this.changes = storage.changes;
				} else {
					// this.selectedPriority = this.activePriority;
					// this.selectedAgent = this.activeAgent;
					this.newMessage = "";
					// this.showTextarea = false;
				}
			},
			calcTypeOfMessage(agent) {
				if (agent !== "") {
					return "agent-message";
				}
				return "client-message";
			},
			// calcNewMessage(agent, isSeen) {
			// 	if (agent !== "" && isSeen === false) return "background-color:#5ae27061";
			// 	return "";
			// },
			calcNewMessage(messageNpp) {
				if (this.newMessages.find(x => x.npp === messageNpp)) return "background-color:#5ae27061";
				return "";
			},
			calcSeenedByAgentsMessage(messageNpp, messageAgent) {
				if (this.seenedByAgentsMessages.find(x => x.npp === messageNpp && messageAgent !== '')) return '#448AFF';
				return '#E0E0E0';
			},
			changeTicketTitle() {
				this.isChangeTicketTitle = !this.isChangeTicketTitle;
				this.newTicketTitle = this.ticketTitle;
			},
			saveNewTitle(ticketNpp, newTitle) {
				const params = {
					ticketNpp: ticketNpp,
					newTitle: newTitle
				};

				this.$http
					.post("data/SetNewTicketTitle", params, {emulateJSON: true})
					.then(
						() => {
							this.getMessages(this.tn)
								.then(response => {
									this.messages = response.data;
									this.isChangeTicketTitle = false;
									this.newTicketTitle = "";
								})
								.catch(error => console.error(error));
							this.$emit("createTicket"); //обновление списка заявок в store
							this.$store.dispatch("showSnackBar", {
								snackText: "Тема изменена"
							});
						},
						error => {
							console.error(error);
							this.$store.dispatch("showSnackBar", {
								snackText: "Ошибка при изменении темы"
							});
						}
					);
			},
			clickUpdate() {
				this.loading = true;
				this.getMessages(this.tn)
					.then(response => {
						this.messages = response.data;
						//this.lastUpdateTime = +new Date();
						//this.showNotify();
					})
					.catch(error => {
						console.error(error);
						this.$store.dispatch("showSnackBar", {
							snackText: "Ошибка обновления списка сообщений"
						});
					})
					.finally(() => {
						this.loading = false;

						//получаем новые сообщения
						this.getNewMessages(this.tn, this.CURRENTAPTEKA.npp, this.USERID).then(response => {
							this.newMessages = response.data;
						}, error => console.log(error));

						//получаем просмотренные агентами сообщения
						this.getSeenedByAgentsMessagesOfTickets(this.tn).then(response => {
							this.seenedByAgentsMessages = response.data;
						}, error => console.error(error));

						//прокручиваем вниз список сообщений
						if (!this.firstLoad) {
							this.scrollMessagesListToBottom();
						}
					});
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
			// setIsSeen(messageNpp) {
			// 	const params = {
			// 		messageNpp: messageNpp
			// 	};
			// 	this.$http.put("data/SetIsSeen", params, {emulateJSON: true}).then(
			// 		() => {
			// 			this.getMessages(this.tn)
			// 				.then(response => {
			// 					this.messages = response.data;
			//
			// 					//если нет непросмотренных сообщений, то отключаем уведомление
			// 					if (!this.hasNewMessages) {
			// 						if (this.notifyTimerId !== 0) clearInterval(this.notifyTimerId);
			// 						this.notifyTimerId = 0;
			// 					}
			// 					// this.$emit("createTicket"); //обновление списка заявок в store
			// 				})
			// 				.catch(error => console.error(error));
			// 		},
			// 		error => {
			// 			console.error(error);
			// 			this.$store.dispatch("showSnackBar", {
			// 				snackText: "Ошибка при установке признака просмотрено"
			// 			});
			// 		}
			// 	);
			// },
			// showNotify() {
			// 	if (this.hasNewMessages) {
			// 		//если уведомление уже запущено, то пропускаем
			// 		if (this.notifyTimerId === 0) {
			// 			this.notify(`В заявке ${this.ticketNumber} есть новое сообщение`, +new Date());
			// 			this.notifyTimerId = setInterval(() => {
			// 				this.notify(`В заявке ${this.ticketNumber} есть новое сообщение`, +new Date());
			// 			}, 30000);
			// 		}
			// 	}
			// },
			updateMessagesFromParent() {
				// console.log('updateMessagesFromParent()');
				this.clickUpdate();
			},
			getNewMessages(ticketNpp, clientNpp, userId) {
				return this.$http.get("data/GetNewMessagesOfTickets", {
					params: {ticket_npp: ticketNpp, client_npp: clientNpp, userId: userId}
				});
			},
			updateTimeInTables() {
				//обновляем в таблице HistoryMessages время последнего просмотра заявки
				this.$store.dispatch('SET_TICKETLASTUPDATETIME', {
					ticketNpp: this.tn,
					clientNpp: this.CURRENTAPTEKA.npp,
					userId: this.USERID
				});

				//обновляем время последних изменений в таблице HistoryClientTickets для запуска цикла проверки
				// которая снимет пометку непросмотрености заявки если одновременно данная заявка открыта в другом браузере
				this.$store.dispatch('SET_LASTUPDATETIMEFROMSERVER', {clientNpp: this.CURRENTAPTEKA.npp, userId: this.USERID});
			},
			move($event) {
				if (this.hasNewMessages && this.mouseMoveCounter === 0 && !this.firstLoad) {
					console.log(`mousemove`);
					this.mouseMoveCounter++;

					//убираем заявку из массива непросмотренных
					if (this.TICKETSWITHCHANGES.find(x => x.npp === this.npp)) {
						let index = this.TICKETSWITHCHANGES.findIndex(x => x.npp === this.npp);
						this.$store.dispatch('REMOVE_TICKETSWHITHCHANGES', index);
					}

					//через 10 секунд обновляем время просмотра заявки, для запуска процедуры автообновления
					setTimeout(() => {
						this.updateTimeInTables();
						this.newMessages = [];
						this.mouseMoveCounter = 0;
					}, 10000);
				}
			},
			getSeenedByAgentsMessagesOfTickets(ticketNpp) {
				return this.$http.get("data/GetSeenedByAgentsMessagesOfTickets", {
					params: {ticket_npp: ticketNpp}
				});
			},
			scrollMessagesListToBottom() {
				setTimeout(() => {
					let ul = document.getElementById('mlist');
					ul.scrollTo(0, ul.scrollHeight);
				}, 200);
			},
			calcMessageAttachments(attachmentsData) {
				return JSON.parse(attachmentsData);
			},
			calcTypeOfAttachment(attachment) {
				let fileName = attachment.file_name;
				//определяем расширение файла
				let fileType = fileName.slice((Math.max(0, fileName.lastIndexOf(".")) || Infinity) + 1);
				fileType = fileType.toLowerCase();
				return this.KNOWNFILETYPES.includes(fileType) ? `/dist/svg/${fileType}.svg` : `/dist/svg/raw.svg`;
			},
			calcTypeOfAttachmentForAttachedFiles(file) {
				let fileName = file.name;
				//определяем расширение файла
				let fileType = fileName.slice((Math.max(0, fileName.lastIndexOf(".")) || Infinity) + 1);
				fileType = fileType.toLowerCase();
				return this.KNOWNFILETYPES.includes(fileType) ? `/dist/svg/${fileType}.svg` : `/dist/svg/raw.svg`;
			},
			getAttachmentFile(attachment) {
				axios.get("data/GetAttachmentFile", {
					params: {
						file_path: attachment.file_path
					},
					responseType: 'arraybuffer'
				})
					.then(response => {
						let headers = response.headers;
						const blob = new Blob([response.data], {type: headers['content-type']});
						const url = window.URL.createObjectURL(blob);
						let link = document.createElement('a');
						link.href = url;
						link.setAttribute('download', attachment.file_name);
						document.body.appendChild(link);
						link.click();
						link.remove();
						window.URL.revokeObjectURL(url);
					}, error => {
						console.log(error);
						this.$store.dispatch("showSnackBar", {snackText: `Произошла ошибка при загрузке файла вложения!`});
					});
			},
			bytesToSize(bytes) {
				const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
				if (bytes === 0) return 'n/a';
				const i = parseInt(Math.floor(Math.log(Math.abs(bytes)) / Math.log(1024)), 10);
				if (i === 0) return `${bytes} ${sizes[i]}`;
				return `${(bytes / (1024 ** i)).toFixed(1)} ${sizes[i]}`;
			},
			onFileUpload(fileList) {
				Array.from(fileList).forEach(file => {
					this.files.push(file);
				});
			},
			uploadFilesToServer(files, ticketNpp, messageNpp) {
				let formData = new FormData();
				files.forEach(file => formData.append('files', file));
				formData.append('ticketNpp', ticketNpp);
				formData.append('messageNpp', messageNpp);

				return this.$http.post('data/UploadAttachmentsFiles', formData);
			},
			clickFileInput() {
				document.getElementById('file-input').click();
			},
			removeAttachment(attachment) {
				let index = this.files.indexOf(attachment);
				if (index !== -1) this.files.splice(index, 1);
			},
		},

		updated() {
			this.calcHeight();
		},

		mounted() {
			// console.log("messages mounted()");
			//toggleChanges() неявно запускается при установке значений свойств selectedPriority, selectedAgent в методе recoverUnsavedDataFromLocalStorage()
			this.loading = true;
			this.tn = this.$route.query["tn"];
			// this.$root.$on("closeTabFromParent", this.closeTabFromParent); //подписываемся на событие - закрытие вкладки

			//подписываемся на событие - обновление списка сообщений
			this.$root.$on("updateTicketMessagesFromParent", this.updateTicketMessagesFromParent);

			//подписываемся на событие - обновление списка сообщений (автозагрузка обновлений)
			this.$root.$on("updateMessagesFromParent", this.updateMessagesFromParent);

			//подписываемся на событие - обновление времени последних обновлений в таблицах
			this.$root.$on("updateTimeFromParent", this.updateTimeInTables);

			this.getMessages(this.tn)
				.then(response => {
					this.messages = response.data;
					// this.showNotify();
				})
				.catch(error => console.error(error))
				.finally(() => {
					this.recoverUnsavedDataFromLocalStorage();
					this.calcHeight();
					// this.lastUpdateTime = +new Date();
					this.loading = false;

					//через 20 сек. убираем у компонента признак первой загрузки (защита от двойной загрузки данных при переходе на вкладку)
					//если пользователь находится на вкладке более 20сек. то просмотр новых сообщений будет происходить по событию mousemove
					//иначе новые сообщения просматриваются при переходе на вкладку
					setTimeout(() => {
						this.firstLoad = false;
					}, 20000);

					//получаем новые сообщения
					this.getNewMessages(this.tn, this.CURRENTAPTEKA.npp, this.USERID).then(response => {
						this.newMessages = response.data;
					}, error => console.log(error));

					//получаем просмотренные агентами сообщения
					this.getSeenedByAgentsMessagesOfTickets(this.tn).then(response => {
						this.seenedByAgentsMessages = response.data;
					}, error => console.error(error));

					//прокручиваем список сообщений вниз
					this.scrollMessagesListToBottom();
				});

			//автообновление списка сообщений
			// this.timerId = setInterval(() => {
			// 	this.getLastUpdateTimeFromServer(this.CURRENTAPTEKA.npp).then(
			// 		response => {
			// 			let serverTime = response.body;
			// 			if (serverTime !== "") {
			// 				serverTime = +new Date(serverTime); //переводим дату в миллисекунды
			// 				if (serverTime > this.lastUpdateTime) {
			// 					//обновляем список сообщений
			// 					this.clickUpdate();
			// 				}
			// 			}
			// 		},
			// 		error => console.error(error)
			// 	);
			// }, 60000 * 2);

			// this.$store.dispatch("setPageData", {
			// 	header: "Заявка " + this.tn,
			// 	status: "mounted",
			// 	data: {}
			// });

			// if (this.PRIORITIES.length == 0) this.$store.dispatch("GET_PRIORITIES");
			// if (this.AGENTS.length == 0) this.$store.dispatch("GET_AGENTS");
			// console.log("end mounted()");
		},

		beforeDestroy() {
			// console.log("messages beforeDestroy()");
			// this.$root.$off("closeTabFromParent", this.closeTabFromParent);
			this.$root.$off("updateTicketMessagesFromParent", this.updateTicketMessagesFromParent);
			this.$root.$off("updateMessagesFromParent", this.updateMessagesFromParent);
			this.$root.$off("updateTimeFromParent", this.updateTimeInTables);
			clearInterval(this.timerId);
			// clearInterval(this.notifyTimerId);
		},

		watch: {
			$route(toR, fromR) {
				this.loading = true;
				this.tn = toR.query["tn"];
				this.files = [];

				//отключаем запуск уведомлений для данной заявки
				// clearInterval(this.notifyTimerId);
				// this.notifyTimerId = 0;

				this.getMessages(this.tn)
					.then(response => {
						this.messages = response.data;
						// this.showNotify();
					})
					.catch(error => console.error(error))
					.finally(() => {
						this.isChangeTicketTitle = false;
						this.recoverUnsavedDataFromLocalStorage();
						// this.lastUpdateTime = +new Date(); //дата в милисикундах от 1970г
						this.loading = false;

						this.firstLoad = true;
						setTimeout(() => {
							this.firstLoad = false;
						}, 20000);

						//получаем новые сообщения
						this.getNewMessages(this.tn, this.CURRENTAPTEKA.npp, this.USERID).then(response => {
							this.newMessages = response.data;
						}, error => console.log(error));

						//получаем просмотренные агентами сообщения
						this.getSeenedByAgentsMessagesOfTickets(this.tn).then(response => {
							this.seenedByAgentsMessages = response.data;
						}, error => console.log(error));

					});

				// this.$store.dispatch("setPageData", {
				// 	header: "Заявка " + this.tn,
				// 	status: "mounted",
				// 	data: {}
				// });
			}
		}
	};
</script>

<style lang="scss" scoped>
		ul {
				padding: 0 20px;
				margin-bottom: 0;
				margin-top: 0;
				overflow-y: auto;
				display: flex;
				flex-direction: column;
		}

		#message-field {
				margin: 10px 0;
		}

		.mes {
				list-style: none;
				border: 1px solid #ced4da;
				border-radius: 5px 5px;
				padding: 10px;
				// background-color: aliceblue;
				// background-color: #DBF4FD;
				margin: 10px 0;
		}

		.new-message {
				background-color: palevioletred;
		}

		.agent-message {
				background-color: #f2f6f9;
				margin-right: 20%;
		}

		.client-message {
				background-color: #dbf4fd;
				margin-left: 20%;
		}

		.message-title {
				font-weight: bold;
		}

		pre {
				text-align: justify;
				/*text-indent: 20px;*/
				white-space: pre-wrap;
				word-wrap: break-word;
		}

		.dialog {
				text-indent: 0;
				margin: 20px 20px 0 20px;
		}

		.md-dialog {
				max-width: 768px;
		}

		.done > i {
				position: absolute;
				top: 5px;
				right: 5px;
		}

		li {
				position: relative;
		}

		.attachments {
				border-top: 1px solid black;
				padding-top: 10px;
				display: flex;
				align-items: center;
		}

		.attachments-container {
				display: flex;
				flex-wrap: wrap;
				margin: 10px 0;
		}

		.attachments-item {
				padding: 5px;
				height: 100px;
				width: 100px;
				border: 1px solid #DADADA;
				background-color: #F0F0F0;
				border-radius: 5px;
				margin-right: 10px;
				position: relative;
				display: flex;
				flex-direction: column;
		}

		.attachments-item span {
				font-size: 10px;
				white-space: nowrap;
				overflow: hidden;
		}

		.attachments-item span.remove-attachment {
				height: 16px;
				width: 16px;
				position: absolute;
				top: 0;
				right: 0;
				text-align: center;
				line-height: 16px;
				font-size: 18px;
				border-radius: 8px;
		}

		.attachments-item span.remove-attachment:hover {
				cursor: pointer;
				background-color: #448AFF;
				color: white;
		}

		#attachments-link {
				margin-bottom: 40px;
				width: max-content
		}

		#attachments-link span {
				text-decoration: underline;
				padding: 10px 10px 10px 0;
		}

		#attachments-link span:hover {
				cursor: pointer;
				text-decoration: none;
		}

		.fadeattachments-enter-active, .fadeattachments-leave-active {
				transition: opacity .5s;
		}

		.fadeattachments-enter, .fadeattachments-leave-to {
				opacity: 0;
		}

</style>