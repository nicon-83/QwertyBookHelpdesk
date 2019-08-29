<template>
	<div>
		<h5>Создание заявки</h5>

		<form @submit.prevent="createTicket">
			<div class="form-group">
				<label for="title">Тема</label>
				<input type="text" class="form-control" id="title" placeholder="Введите тему" v-model.lazy="title" style="width:400px">
			</div>
			<div class="form-group" style="width:180px">
				<label for="priority">Приоритет</label>
				<select class="form-control" id="priority" v-model="selectedPriority">
					<option v-for="priority in PRIORITIES"
							:key="priority.id">
						{{priority.name}}
					</option>
				</select>
			</div>
			<div class="form-group">
				<!--<label for="message">Текст сообщения</label>-->
				<textarea class="form-control" id="message" rows="3" placeholder="Текст сообщения" v-model.lazy="message" style="width:600px"></textarea>
			</div>
			<button type="submit" class="btn btn-primary">Создать</button>
		</form>

	</div>
</template>

<script>

	import { mapGetters } from 'vuex'
	import axios from 'axios';

	export default {
		props: ['tabId'],
		components: {

		},
		name: 'CreateTicket',
		data: () => ({
			selectedPriority: 'обычный',
			title: '',
			message: ''
		}),
		methods: {
			createTicket() {
				axios.post('data/CreateTicket', JSON.stringify({
					//number: this.LOCAL_TICKET_NUMBER + 1,
					title: this.title,
					customer: this.CUSTOMER,
					state: "new",
					priority: this.npp,
					agent: "167",
					message: this.message
				}),
					{ headers: { 'Content-Type': 'application/json' } }
				)
					.then((response) => {
						this.$store.dispatch('GET_TICKETS');
						this.$emit('closeTicketTabs', this.tabId);
						this.$store.dispatch('showSnackBar', { 'snackText': 'Создана заявка' });
					})
					.catch(function (error) {
						console.log(error);
					})
					.then(function () {
						// always executed
					});
			}
		},
		computed: {
			...mapGetters(['LAST_NUMBER', 'PRIORITIES', 'CUSTOMER']),
			npp: function () {
				switch (this.selectedPriority) {
					case 'самый низкий':
						return 'very_low';
					case 'низкий':
						return 'low';
					case 'обычный':
						return 'normal';
					case 'высокий':
						return 'hight';
					case 'безотлагательный':
						return 'very_hight';
				}
			}
		},
		mounted() {
			this.$store.dispatch('GET_PRIORITIES');
			//this.$store.dispatch('GET_LTN'); //получаем последний внутренний номер заявки этого клиента
		}
	}


</script>

<style lang="scss" scoped>
</style>