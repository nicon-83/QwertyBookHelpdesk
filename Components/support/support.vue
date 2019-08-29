<template>
	<div>
		<TicketsMain></TicketsMain>
	</div>
</template>

<script>

	import Drawer from '../common/Drawer.vue'
	import TicketsMain from './TicketsMain'

	export default {
		components: {
			Drawer,
			TicketsMain
		},
		name: 'Support',
		data: () => ({

		}),
		methods: {

		}
	}

</script>

<style lang="scss" scoped>
	#support {
		/*min-height: 90vh;*/
	}
</style>