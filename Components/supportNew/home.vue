<template>
	<div id="client-info" :style="{height: tableHeight + 'px'}">
		<md-card style="margin-top:50px; width:700px;">
			<md-card-header></md-card-header>
			<md-card-content>
				<div class="md-layout md-gutter md-alignment-center-left">
					<div class="md-layout-item md-size-35">
						<md-field md-clearable>
							<label>ID Аптеки</label>
							<md-input placeholder="ID Аптеки" v-model="aptekaId" id="aptekaId" md-dense></md-input>
						</md-field>
					</div>
					<div class="md-layout-item md-size-5" style="padding-left: 0px;">
						<md-tooltip md-direction="bottom" md-delay="500">Найти аптеку</md-tooltip>
						<md-button
							class="md-icon-button"
							id="login-search-button"
							@click="search"
							:disabled="!aptekaId"
						>
							<md-icon>search</md-icon>
						</md-button>
					</div>
				</div>
				<div v-if="aptekaName">
					<p>
						<b>Ваша аптека:</b>
					</p>
					<p>{{apteka.name}}</p>
				</div>
			</md-card-content>
			<md-card-actions>
				<md-button
					class="md-button md-raised md-primary md-dense"
					id="login-button"
					@click="login"
					:disabled="!aptekaName"
				>Войти</md-button>
			</md-card-actions>
		</md-card>
	</div>
</template>

<script>
export default {
	components: {},
	name: "Home",
	data: () => ({
		aptekaId: "",
		loading: false,
		apteka: []
	}),
	methods: {
		search() {
			this.$http
				.get("data/getClient", {
					params: {
						apteka_id: this.aptekaId
					}
				})
				.then(
					response => {
						this.apteka = Array.from(response.body)[0];
					},
					() => {
						this.$store.dispatch("showSnackBar", {
							snackText: `Ошибка получения данных аптеки ${this.aptekaId}`
						});
					}
				);
		},
		login() {
			console.log(`apteka.npp = ${this.apteka.npp}`);
			this.$store.dispatch('SET_CURRENTAPTEKA', this.apteka);
		}
	},
	computed: {
		tableHeight() {
			return this.$store.state.app.tableHeight;
		},
		aptekaName() {
			if (this.apteka.length == 0) return "";
			return this.apteka.name;
		}
	},
	mounted() {
		//расчет высоты содержимого вкладки
		let pageHeight = document.documentElement.clientHeight;
		let toolbar = document.getElementById("application-toolbar");
		let toolbarHeight = toolbar.clientHeight;
		let tabsPanel = document.getElementById("second-tabs-panel");
		let tabsPanelHeight = tabsPanel.clientHeight;
		let paddingMarginHeight = 20;

		if (pageHeight > 900) {
			this.$store.dispatch("setApp", {
				tableHeight:
					pageHeight - toolbarHeight - tabsPanelHeight - paddingMarginHeight
			});
		} else {
			this.$store.dispatch("setApp", {
				tableHeight:
					pageHeight - toolbarHeight - tabsPanelHeight - paddingMarginHeight
			});
		}

		this.$store.dispatch("setPageData", {
			header: "Главная",
			status: "mounted",
			data: {}
		});
	}
};
</script>

<style lang="scss">
.tab {
	border: none !important;
}
</style>