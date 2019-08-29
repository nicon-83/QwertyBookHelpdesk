<template>
	<drawer>
		<div style="padding: 0 10px">
			<div>
				<supportTabs></supportTabs>
				<div class="md-content md-elevation-2 ticket-title">
					<span>Заявка #... - тема заявки...</span>
				</div>
			</div>
			<div id="ticket_chat">
				<div id="message-block">
					<div class="md-field md-theme-default md-has-textarea" style="margin-bottom:5px">
						<textarea id="message" class="md-textarea" style="resize:none" v-model.trim.lazy="message"></textarea>
					</div>
					<div style="text-align:right">
						<button id="sendBtn" class="md-button md-raised md-dense md-primary md-theme-default" @click="sendMessage">Отправить</button>
					</div>
				</div>
				<div id="chatroom" class="md-content md-scrollbar md-theme-default"></div>
			</div>
		</div>
	</drawer>
</template>

<script>

	import Drawer from '../common/Drawer.vue'
	import SupportTabs from './supportTabs.vue';
	import axios from 'axios';
	const signalR = require('@aspnet/signalr');

	export default {
		components: {
			Drawer,
			SupportTabs
		},
		data: () => ({
			customer: 'apteka_57279',
			title: 'Тестовая заявка №1',
			status: 'new',
			agent: 'Владимир Македонов',
			message: ''
		}),
		methods: {
			sendMessage() {
				let message = document.getElementById("message").value;
				hubConnection.invoke('Send', message);

				axios.post('data/setmessages', {
					customer: this.customer,
					title: this.title,
					status: this.status,
					agent: this.agent,
					message: this.message
				})
					.then((response) => {
						console.log(response);
						this.message = '';
					})
					.catch((error) => {
						console.log(error);
					});
			}
		}
	}

	//let hubUrl = document.getElementById('#link_for_chat').getAttribute('href');
	const hubConnection = new signalR.HubConnectionBuilder()
		.withUrl('/support', { transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling })
		.configureLogging(signalR.LogLevel.Information)
		.build();

	hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 3; //минуты

	//let userName = document.getElementById("user_name").innerText;

	// получение сообщения от сервера
	hubConnection.on('Receive', function (message, connectionId) {

		// создаем элемент сообщения
		let divCard = document.createElement("div");
		divCard.classList.add('message-wrapper');
		//divCard.classList.add('md-card');
		//divCard.classList.add('md-theme-default');

		let divHeader = document.createElement("div");
		divHeader.classList.add('message-header');
		//divHeader.classList.add('md-card-header');
		let title = document.createElement("p");
		//title.classList.add('md-title');
		let date = new Date();
		let dateString = date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
		title.appendChild(document.createTextNode('От: ' + connectionId + ' - ' + dateString));
		divHeader.appendChild(title);

		let divContent = document.createElement("div");
		divContent.classList.add('message-content');
		//divContent.classList.add('md-card-content');
		let content = document.createElement('p');
		content.appendChild(document.createTextNode(message));
		divContent.appendChild(content);

		divCard.appendChild(divHeader);
		divCard.appendChild(divContent);

		let chatroom = document.getElementById("chatroom");
		let firstElem = chatroom.firstChild;
		chatroom.insertBefore(divCard, firstElem);

	});

	//диагностические уведомления
	hubConnection.on('Notify', function (message) {

		let notifyElem = document.createElement("b");
		let date = new Date();
		let dateString = date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
		notifyElem.appendChild(document.createTextNode(dateString + ' ' + message));
		let elem = document.createElement("p");
		elem.appendChild(notifyElem);
		var firstElem = document.getElementById("chatroom").firstChild;
		document.getElementById("chatroom").insertBefore(elem, firstElem);

	});

	// установка имени пользователя
	//document.getElementById("loginBtn").addEventListener("click", function (e) {
	//	userName = document.getElementById("user_name").innerText;
	//	document.getElementById("header").innerHTML = '<h3>Welcome ' + userName + '</h3>';
	//});

	hubConnection.start();

</script>

<style lang="scss">

	.ticket-title {
		border-radius: 5px;
		margin-top: 10px;
		padding: 10px;
	}

	#ticket_chat {
		/*outline: 1px solid black;*/
		display: flex;
		flex-direction: column-reverse;
		/*min-height: 100vh;*/
		max-height: 80vh;
		min-height: 80vh;
		position: relative;
		/*margin-top: -165px;*/
		/*margin-left: 1.5rem;
		margin-right: 1.5rem;*/
	}

	#sendBtn {
		display: inline-block;
	}

	#chatroom {
		/*outline: 1px solid red;*/
		display: flex;
		flex-direction: column-reverse;
		max-height: 65vh;
		margin-top: 10px;
		margin-bottom: 170px;
		overflow-y: auto;
	}

		#chatroom > div {
			border: 1px solid grey;
			background-color: #F6F9FF;
			padding: 10px;
			border-radius: 5px;
			margin: 10px 0;
		}

	#message-block {
		width: 100%;
		height: 170px;
		position: absolute;
		bottom: 0;
		z-index: 1000;
		background-color: white;
	}

	.message-header > p {
		font-weight: bold;
		color: #448AFF;
		font-size: 0.95rem;
		margin: 0.5rem 0;
	}

	.message-content > p {
		margin: 0.5rem 0;
		text-align: justify;
	}
</style>