<template>
	<div>

		<ul>
			<li v-for="message of messages">
				<span>{{ message.createTime }}</span>
				<p>{{message.text}} </p>
			</li>
		</ul>

		<textarea class="form-control" rows="3" style="resize:none" v-model.lazy="newMessage"></textarea>
		<div class="text-right">
			<button class="btn btn-sm btn-success mt-2 mb-2" @click="sendMessage">Отправить</button>
		</div>
	</div>
</template>

<script>

	import axios from 'axios';

	export default {

		props: ['ticketNumber'],

		data: () => ({
			messages: [],
			newMessage: ''
		}),

		methods: {
			sendMessage() {
				axios.post('data/saveMessage', JSON.stringify({
					newMessage: this.newMessage,
					ticketNumber: this.ticketNumber
				}),
					{ headers: { 'Content-Type': 'application/json' } }
				)
					.then((response) => {
						axios.post('data/getMessages', JSON.stringify({
							ticketNumber: this.ticketNumber,
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
			}
		},

		mounted() {
			axios.post('data/getMessages', JSON.stringify({
				ticketNumber: this.ticketNumber,
			}),
				{ headers: { 'Content-Type': 'application/json' } }
			)
				.then((response) => {
					this.messages = response.data;
				})
				.catch(function (error) {
					console.log(error);
				})
		}
	}

</script>

<style lang="scss" scoped>

	ul {
		padding-left: 0;
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
</style>