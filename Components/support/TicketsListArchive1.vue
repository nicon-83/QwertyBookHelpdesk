<template>
		<div>
			<div class="md-content">

				<div class="md-card md-table">

					<div class="md-table-fixed-header">
						<div class="md-table-fixed-header-container">
							<table>
								<thead>
									<tr>
										<th class="md-table-head md-sortable">
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
										<th class="md-table-head md-sortable">
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
										<th class="md-table-head md-sortable">
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
										<th class="md-table-head md-sortable">
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
										<th class="md-table-head md-sortable">
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
										<th class="md-table-head md-sortable">
											<div class="md-table-head-container md-ripple">
												<div class="md-table-head-label">
													<i class="md-icon md-icon-font md-icon-image md-table-sortable-icon md-theme-default">
														<svg height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
															<path d="M0 0h24v24H0V0z" fill="none"></path>
															<path d="M4 12l1.41 1.41L11 7.83V20h2V7.83l5.58 5.59L20 12l-8-8-8 8z"></path>
														</svg>
													</i>
													Последнее сообщение
												</div>
											</div>
										</th>
									</tr>
								</thead>
							</table>
						</div>
					</div>

					<div class="md-content md-table-content md-scrollbar md-theme-default" style="height: 700px;">
						<table>
							<tbody>
								<tr class="md-table-row" v-for="item in 20">
									<td class="md-table-cell">#{{ item }}</td>
									<td class="md-table-cell"><a href="#" @click.prevent="">Заявка #{{ item }} в архиве</a></td>
									<td class="md-table-cell">обычный</td>
									<td class="md-table-cell">в работе</td>
									<td class="md-table-cell">Владимир Македонов</td>
									<td class="md-table-cell">06.02.2019 11:47</td>
								</tr>
							</tbody>
						</table>
					</div>

				</div>

			</div>
		</div>
</template>

<script>

	export default {
		components: {

		},
		data: () => ({

		}),
		methods: {

		}
	}
</script>

<style lang="scss" scoped>

	.md-content {
		/*border-radius: 5px;*/
		/*margin: 10px 5px;*/
		padding: 10px;
	}

	a:hover {
		cursor: pointer;
	}

</style>