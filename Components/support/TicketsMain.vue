<template>
	<drawer>
		<div>
			<supportTabs></supportTabs>
			<!--<div class="md-content md-elevation-2">
				<TicketsMainContent></TicketsMainContent>
			</div>-->
		</div>
	</drawer>
</template>

<script>

	import Drawer from '../common/Drawer.vue'
	import SupportTabs from './supportTabs.vue';
	import TicketsMainContent from './TicketsMainContent.vue';

	export default {
		components: {
			Drawer,
			SupportTabs,
			TicketsMainContent
		},
		data: () => ({

		}),
		methods: {

		}
	}

</script>

<style lang="scss" scoped>

	.md-content {
		border-radius: 5px;
		margin: 10px 5px;
		padding: 10px;
	}

</style>