<template>

	<div id="ticket-messages">

		<div id="close-tab-button" style="display:flex;justify-content:flex-end">
			<md-button type="button" class="md-icon-button md-primary md-theme-default" @click="closeTab(ADMINCURRENTTABID, tn)">
				<md-icon>close</md-icon>
				<md-tooltip md-direction="top" md-delay="500">Закрыть</md-tooltip>
			</md-button>
		</div>

		<ul class="md-content md-scrollbar md-theme-default" :style="{height: tableHeight + 'px'}">
			<li v-for="message of messages">
				<span>{{ message.createTime }}</span>
				<p>{{message.text}} </p>
			</li>
		</ul>

		<md-field id="message-field" md-clearable>
			<md-textarea style="resize:none;" id="message" placeholder="Текст сообщения" v-model.lazy="newMessage" rows="5"></md-textarea>
		</md-field>

		<div id="send-message-button" style="display:flex; justify-content:flex-end">
			<md-button class="md-dense md-raised md-primary md-theme-default" @click="sendMessage">Отправить</md-button>
		</div>

	</div>

</template>

<script>

	import axios from 'axios';
	import { mapGetters } from 'vuex'

	export default {

		components: {

		},

		props: [],

		data: () => ({
			messages: [],
			newMessage: '',
			tn: 0
		}),

		computed: {
			...mapGetters(['ADMINCURRENTTABID', 'ADMINOPENEDTICKETSTABS']),
			tableHeight() {
				return this.$store.state.app.tableHeight;
			}
		},

		methods: {
			sendMessage() {
				axios.post('data/saveMessage', JSON.stringify({
					newMessage: this.newMessage,
					ticketNumber: this.tn
				}),
					{ headers: { 'Content-Type': 'application/json' } }
				)
					.then((response) => {
						axios.post('data/getMessages', JSON.stringify({
							ticketNumber: this.tn,
						}),
							{ headers: { 'Content-Type': 'application/json' } }
						)
							.then((response) => {
								this.messages = response.data;
							})
							.catch(function (error) {
								console.log(error);
							})
						this.newMessage = '';
					})
					.catch(function (error) {
						console.log(error);
					})
			},
			getMessages(ticketNumber) {
				axios.post('data/getMessages', JSON.stringify({
					ticketNumber: ticketNumber,
				}),
					{ headers: { 'Content-Type': 'application/json' } }
				)
					.then((response) => {
						this.messages = response.data;
					})
					.catch(function (error) {
						console.log(error);
					})
			},
			closeTab(tab_id, ticketNumber) {
				//закрываем вкладку
				this.$emit('closeAdminTab', tab_id);

				//делаем активной ссылку на данную заявку
				for (let index in this.ADMINOPENEDTICKETSTABS) {
					if (this.ADMINOPENEDTICKETSTABS[index] === ticketNumber) {
						this.$store.dispatch('REMOVE_ADMINOPENEDTICKETSTABS', index);
					}
				}
			},
		},

		mounted() {
			//расчет высоты чата
			let pageHeight = document.documentElement.clientHeight;
			let toolbar = document.getElementById("application-toolbar");
			let toolbarHeight = toolbar.clientHeight;
			let tabsPanel = document.getElementById("second-tabs-panel");
			let tabsPanelHeight = tabsPanel.clientHeight;
			let closeButton = document.getElementById("close-tab-button");
			let closeButtonHeight = closeButton.clientHeight;
			let messageField = document.getElementById("message-field");
			let messageFieldHeight = messageField.clientHeight;
			let sendButton = document.getElementById("send-message-button");
			let sendButtonHeight = sendButton.clientHeight;
			let paddingMarginHeight = 145;

			this.$store.dispatch('setApp', { 'tableHeight': pageHeight - toolbarHeight - tabsPanelHeight - closeButtonHeight - messageFieldHeight - sendButtonHeight - paddingMarginHeight });

			this.tn = this.$route.query['tn'];
			this.getMessages(this.tn);

			this.$store.dispatch('setPageData', {
				'header': 'Заявка ' + this.tn,
				'status': 'mounted',
				'data': {}
			});
		},

		watch: {
			$route(toR, fromR) {
				this.tn = toR.query['tn'];
				this.getMessages(this.tn);

				this.$store.dispatch('setPageData', {
					'header': 'Заявка ' + this.tn,
					'status': 'mounted',
					'data': {}
				});
			}
		}
	}

</script>

<style lang="scss" scoped>

	ul {
		padding-left: 0;
		margin-top: 0;
		overflow-y: auto;
	}

	li {
		list-style: none;
		border: 1px solid #CED4DA;
		border-radius: 5px 5px;
		padding: 10px;
		background-color: aliceblue;
		margin: 10px 0;
	}

	span {
		font-weight: bold;
	}

	p {
		text-align: justify;
		text-indent: 20px;
	}

	.close-btn {
		padding: 0;
		height: 18px;
		width: 18px;
		border-radius: 9px;
		background-color: #FF5252;
		border-color: transparent;
		box-shadow: rgba(0,0,0,0.5) 1px 1px;
		font-size: 0.7rem;
		text-align: center;
	}

		.close-btn:hover {
			cursor: pointer;
			background-color: #FF4242;
		}
</style>