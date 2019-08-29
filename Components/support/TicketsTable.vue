<template>
	<div>
		<div class="md-content">

			<div style="margin-bottom: 10px">
				<button class="btn btn-sm btn-success" @click="addNewTab">Создать заявку</button>
			</div>

			<div class="md-card md-table">

				<div class="md-table-fixed-header">
					<div class="md-table-fixed-header-container">
						<table>
							<thead>
								<tr>
									<th class="md-table-head md-sortable" style="width:100px">
										<div class="md-table-head-container md-ripple">
											<div class="md-table-head-label">
												<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
													<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
														<path d="M0 0h24v24H0V0z" fill="none"></path>
														<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
													</svg>
												</i>
												Номер
											</div>
										</div>
									</th>
									<th class="md-table-head md-sortable" style="width:400px">
										<div class="md-table-head-container md-ripple">
											<div class="md-table-head-label">
												<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
													<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
														<path d="M0 0h24v24H0V0z" fill="none"></path>
														<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
													</svg>
												</i>
												Тема
											</div>
										</div>
									</th>
									<th class="md-table-head md-sortable" style="width:100px">
										<div class="md-table-head-container md-ripple">
											<div class="md-table-head-label">
												<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
													<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
														<path d="M0 0h24v24H0V0z" fill="none"></path>
														<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
													</svg>
												</i>
												Приоритет
											</div>
										</div>
									</th>
									<th class="md-table-head md-sortable" style="width:100px">
										<div class="md-table-head-container md-ripple">
											<div class="md-table-head-label">
												<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
													<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
														<path d="M0 0h24v24H0V0z" fill="none"></path>
														<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
													</svg>
												</i>
												Статус
											</div>
										</div>
									</th>
									<th class="md-table-head md-sortable" style="width:200px">
										<div class="md-table-head-container md-ripple">
											<div class="md-table-head-label">
												<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
													<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
														<path d="M0 0h24v24H0V0z" fill="none"></path>
														<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
													</svg>
												</i>
												Исполнитель
											</div>
										</div>
									</th>
									<th class="md-table-head md-sortable" style="width:200px">
										<div class="md-table-head-container md-ripple">
											<div class="md-table-head-label">
												<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
													<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
														<path d="M0 0h24v24H0V0z" fill="none"></path>
														<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
													</svg>
												</i>
												Дата создания
											</div>
										</div>
									</th>
								</tr>
							</thead>
						</table>
					</div>
				</div>

				<div class="md-content md-table-content md-scrollbar md-theme-default" style="height: 650px;">
					<table>
						<tbody v-if="TICKETS.length > 0">
							<tr class="md-table-row" v-for="item in TICKETS">
								<td class="md-table-cell" style="width:100px">{{ item.number }}</td>
								<td class="md-table-cell" style="width:400px">
									<a href="#" @click.prevent="openTicket($event)" :data-num="item.number">{{ item.title }}</a>
								</td>
								<td class="md-table-cell" style="width:100px">{{ item.priority }}</td>
								<td class="md-table-cell" style="width:100px">{{ item.state }}</td>
								<td class="md-table-cell" style="width:200px">{{ item.agent }}</td>
								<td class="md-table-cell" style="width:200px">{{ item.createTime }}</td>
							</tr>
						</tbody>
						<tbody v-else>
							<tr class="md-table-row">
								<td colspan="7" style="text-align:center;padding-top:30px">Нет заявок</td>
							</tr>
						</tbody>
					</table>
				</div>

			</div>

		</div>
	</div>
</template>

<script>

	import { mapGetters } from 'vuex'
	import $ from 'jquery'

	export default {
		components: {

		},
		name: 'TicketsTable',
		data: () => ({

		}),
		methods: {
			addNewTab() {
				//console.log('click');
				this.$store.dispatch('ADD_TAB', { id: this.LAST_NUMBER, title: 'Заявка ' + this.LAST_NUMBER, type: 'new', ticketNumber: 0 });
				this.$store.dispatch('INCREASE_LAST_NUMBER');
			},
			openTicket(event) {
				//console.log(event.target.parentNode.parentNode.parentNode.parentNode.parentNode);
				let num = event.target.getAttribute('data-num');
				this.$store.dispatch('ADD_TAB', { id: this.LAST_NUMBER, title: 'Заявка ' + num, type: 'open', ticketNumber: num });
				this.$store.dispatch('INCREASE_LAST_NUMBER');
				//console.log($('[role="tab"]'));
				//$('[role="tab"]').removeClass('active');
			}
		},
		computed: {
			...mapGetters(['LAST_NUMBER', 'TICKETS'])
		},
		mounted() {
			this.$store.dispatch('GET_TICKETS');
		}
	}

</script>

<style lang="scss" scoped>

	.md-content {
		padding: 10px;
	}

	a:hover {
		cursor: pointer;
	}

	button {
		padding: 5px;
	}
</style>