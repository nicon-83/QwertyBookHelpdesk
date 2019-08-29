<template>
	<div>

		<!--<md-tabs md-sync-route style="margin-top:5px">
			<md-tab id="tickets-main" md-label="Главная" to="/ticketsMain"></md-tab>
			<md-tab id="tickets-list" md-label="Список заявок" to="/ticketsList"></md-tab>
			<md-tab id="tickets-list-archive" md-label="Архив заявок" to="/ticketsArchive"></md-tab>
			<md-tab id="ticket" md-label="Заявка #..." to="/ticket"></md-tab>
		</md-tabs>-->

		<b-card no-body>
			<b-tabs card id="tabs_panel">
				<!-- Render Tabs -->
				<b-tab :title="`${tab.title}`" v-for="tab in TABS" :key="tab.id">
					{{tab.title}}
					<MainContent v-if="tab.id == 0"></MainContent>
					<TicketsTable v-if="tab.id == 1"></TicketsTable>
					<ListArchive v-if="tab.id == 2"></ListArchive>
					<CreateTicket 
								  :tabId="tab.id" 
								  v-if="tab.type == 'new'"
								  @closeTicketTabs="closeTab"
								  >
					</CreateTicket>
					<TicketMessages 
									v-if="tab.type == 'open'"
									:ticketNumber="tab.ticketNumber"
									>
					</TicketMessages>
					<b-btn size="sm" variant="danger" class="float-right mb-2 mt-2" @click="()=>closeTab(tab.id)" v-show="tab.id > 3">
						Закрыть
					</b-btn>
				</b-tab>

				<!-- New Tab Button (Using tabs slot) -->
				<!--<b-nav-item slot="tabs" @click.prevent="newTab" href="/ticketsList">
					+
				</b-nav-item>-->

				<!-- Render this if no tabs -->
				<div slot="empty" class="text-center text-muted">
					Нет открытых вкладок
					<br>Нажмите кнопку Создать заявку для открытия вкладки
				</div>
			</b-tabs>
		</b-card>

	</div>
</template>

<script>

	import { mapGetters } from 'vuex'
	import TicketsTable from './TicketsTable.vue'
	import MainContent from './TicketsMainContent.vue'
	import ListArchive from './TicketsListArchive1.vue'
	import CreateTicket from './CreateTicket.vue'
	import TicketMessages from './TicketMessages.vue'
	import $ from 'jquery'

	export default {
		components: {
			MainContent ,
			TicketsTable,
			ListArchive,
			CreateTicket,
			TicketMessages
		},

		name: 'SupportTabs',

		data: () => ({

		}),

		computed: {
			...mapGetters(['TABS'])
		},

		methods: {
			closeTab(tab_id) {
				for (let i = 0; i < this.TABS.length; i++) {
					if (this.TABS[i].id === tab_id) {
						this.$store.dispatch('CLOSE_TAB', i);
					}
				}
			}
		},

		mounted: function () {
			//console.log(this.$store.getters.TABS);
			//console.log($('[role="tab"]'));
		}
	}

</script>

<style lang="scss" scoped>
	#support {
		/*min-height: 90vh;*/
	}
</style>