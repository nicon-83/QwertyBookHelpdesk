<template>
		<main-parent>
				<div class="bg"></div>
				<div class="wrapper">
						<div class="content">
								<md-card>
										<md-card-header>
												<div class="md-title">Вход</div>
												<div class="md-subhead"></div>
										</md-card-header>

										<md-card-content>
												<md-field :class="loginErrorClass">
														<label>id аптеки или email</label>
														<!--														<md-input v-model.trim="aptekaId" @input="clearAptekaIdError" @blur="getSiteUsers" required/>-->
														<md-input v-model.trim="aptekaId" @input="clearAptekaIdError" required/>
														<span class="md-error">Неверный логин</span>
												</md-field>
												<!--												<md-field>-->
												<!--														<label for="selectedUser">Пользователь</label>-->
												<!--														<md-select v-model="selectedUser" name="user" id="user" md-dense :disabled="users.length === 0" required>-->
												<!--																<md-option v-for="user in users" :value="user.fullName" :key="user.email">{{user.fullName}}</md-option>-->
												<!--														</md-select>-->
												<!--												</md-field>-->
												<!--												<md-autocomplete v-model="selectedUser" :md-options="usersNames">-->
												<!--														<label>Пользователь</label>-->
												<!--												</md-autocomplete>-->
												<md-field :class="passwordErrorClass">
														<label>Пароль</label>
														<md-input v-model.trim="password" @input="clearPasswordError" type="password" required/>
														<span class="md-error">Неверный пароль</span>
												</md-field>
										</md-card-content>

										<md-card-actions>
												<md-button class="md-raised md-primary" @click="auth3">Войти</md-button>
										</md-card-actions>
								</md-card>
						</div>
				</div>
		</main-parent>
</template>


<script>
	import MainParent from "../MainParent";

	export default {
		components: {
			MainParent
		},
		data: () => ({
			aptekaId: "",
			password: "",
			wrongName: false,
			wrongPassword: false,
			// selectedUser: null,
			// users: [],
			user: [],
			// usersNames: []
		}),
		methods: {
			getApteka(apteka_id) {
				return this.$http.get("data/getClient", {
					params: {
						apteka_id: apteka_id
					}
				});
			},
			auth1() {
				if (!this.isNumeric(this.aptekaId)) {
					this.wrongName = true;
					return;
				}
				this.search(this.aptekaId).then(
					response => {
						let res = response.body;
						let apteka;
						if (res.length == 0) {
							apteka = [];
							localStorage.setItem("apteka", JSON.stringify(apteka));
							this.wrongName = true;
						} else {
							apteka = Array.from(response.body)[0];
							this.wrongName = false;
							this.wrongPassword = false;
							let apteka_id = apteka.idapt.toString();
							if (
								this.password !=
								apteka_id
									.split("")
									.reverse()
									.join("")
							) {
								this.wrongPassword = true;
								return;
							}
							localStorage.setItem("apteka", JSON.stringify(apteka));
							this.$store.dispatch("SET_ISAUTHENTICATED", true);
							this.$store.dispatch("SET_CURRENTAPTEKA", apteka);
							if (this.$route.query.from) {
								this.$router.push({path: this.$route.query.from});
							} else {
								this.$router.push({path: "/support/tickets"});
							}
						}
					},
					() => {
						this.$store.dispatch("showSnackBar", {
							snackText: `Ошибка получения данных аптеки ${this.aptekaId}`
						});
					}
				);
			},
			auth2() {
				if (!this.isNumeric(this.aptekaId)) {
					this.wrongName = true;
					return;
				}
				//если не выбран пользователь
				if (this.selectedUser === null) {
					this.search(this.aptekaId).then(
						response => {
							let res = response.body;
							let apteka;
							if (res.length == 0) {
								apteka = [];
								localStorage.setItem("apteka", JSON.stringify(apteka));
								this.wrongName = true;
							} else {
								apteka = Array.from(response.body)[0];
								this.wrongName = false;
								this.wrongPassword = false;
								let apteka_id = apteka.idapt.toString();
								if (
									this.password !=
									apteka_id
										.split("")
										.reverse()
										.join("")
								) {
									this.wrongPassword = true;
									return;
								}
								localStorage.setItem("apteka", JSON.stringify(apteka));
								this.$store.dispatch("SET_ISAUTHENTICATED", true);
								this.$store.dispatch("SET_CURRENTAPTEKA", apteka);
								if (this.$route.query.from) {
									this.$router.push({path: this.$route.query.from});
								} else {
									this.$router.push({path: "/support/tickets"});
								}
							}
						},
						() => {
							this.$store.dispatch("showSnackBar", {
								snackText: `Ошибка получения данных аптеки ${this.aptekaId}`
							});
						}
					);
				} else {
					this.getSiteUser().then(
						response => {
							//получаем объект пользователя
							this.user = response.data[0];
							if (!this.user) {
								this.wrongPassword = true;
								return;
							}
							localStorage.setItem('user', JSON.stringify(this.user));
							this.$store.dispatch('SET_CURRENTUSER', this.user);

							//получаем объект аптеки и сохраняем его в store
							this.search(this.aptekaId).then(
								response => {
									let apteka = Array.from(response.body)[0];
									localStorage.setItem("apteka", JSON.stringify(apteka));
									this.$store.dispatch("SET_ISAUTHENTICATED", true);
									this.$store.dispatch("SET_CURRENTAPTEKA", apteka);
									if (this.$route.query.from) {
										this.$router.push({path: this.$route.query.from});
									} else {
										this.$router.push({path: "/support/tickets"});
									}
								}, error => console.log(error)
							);

						}, error => console.error(error)
					);
				}
			},
			auth3() {
				//если в поле логин введен id аптеки
				if (this.aptekaId.indexOf('@') === -1) {
					this.authByAptekaId();
				} else {
					this.authByEmail();
				}
			},
			clearAptekaIdError() {
				this.wrongName = false;
				this.selectedUser = null;
			},
			clearPasswordError() {
				this.wrongPassword = false;
			},
			isNumeric(n) {
				return !isNaN(parseFloat(n)) && isFinite(n);
			},
			// getSiteUsers() {
			// 	if (!this.isNumeric(this.aptekaId) && this.aptekaId !== '') {
			// 		this.wrongName = true;
			// 		return;
			// 	}
			// 	// console.info(`getSiteUsers`);
			// 	this.$http.get("data/GetSiteUsers", {
			// 		params: {aptekaId: this.aptekaId}
			// 	}).then(
			// 		response => {
			// 			this.users = response.data;
			// 			this.usersNames = this.users.map(x => x.fullName)
			// 		}, error => console.error(error)
			// 	);
			// },
			// getSiteUser() {
			// 	let userObj = this.users.find(x => x.fullName === this.selectedUser);
			// 	let email = userObj.email;
			// 	return this.$http.get("data/GetSiteUser", {
			// 		params: {aptekaId: this.aptekaId, email: email, ps: this.password}
			// 	});
			// },
			getSiteUser() {
				return this.$http.get("data/GetSiteUser", {
					params: {email: this.aptekaId, ps: this.password}
				});
			},
			authByAptekaId() {
				if (!this.isNumeric(this.aptekaId)) {
					this.wrongName = true;
					return;
				}
				this.$http.get("data/GetApteka", {
					params: {
						aptekaId: this.aptekaId,
						ps: this.password
					}
				}).then(
					response => {
						let result = response.data;
						if (result === 0) {
							this.wrongName = true;
							this.wrongPassword = true;
							return;
						}

						//если авторизация успешна, то получаем объект аптеки по ее id
						this.getApteka(this.aptekaId).then(
							response => {
								let res = response.body;
								let apteka;
								if (res.length === 0) {
									apteka = [];
									localStorage.setItem("apteka", JSON.stringify(apteka));
									this.wrongName = true;
								} else {
									apteka = Array.from(response.body)[0];

									localStorage.setItem("apteka", JSON.stringify(apteka));
									this.$store.dispatch("SET_ISAUTHENTICATED", true);
									this.$store.dispatch("SET_CURRENTAPTEKA", apteka);
									if (this.$route.query.from) {
										this.$router.push({path: this.$route.query.from});
									} else {
										this.$router.push({path: "/support/tickets"});
									}
								}
							},
							() => {
								this.$store.dispatch("showSnackBar", {
									snackText: `Ошибка получения данных аптеки ${this.aptekaId}`
								});
							}
						);
					}, error => console.error(error)
				);
			},
			authByEmail() {
				this.getSiteUser().then(
					response => {
						//получаем объект пользователя
						this.user = response.data[0];
						if (!this.user) {
							this.wrongPassword = true;
							this.wrongName = true;
							return;
						}
						localStorage.setItem('user', JSON.stringify(this.user));
						this.$store.dispatch('SET_CURRENTUSER', this.user);

						//получаем объект аптеки и сохраняем его в store
						let aptekaId = this.user.aptekaId;
						this.getApteka(aptekaId).then(
							response => {
								let apteka = Array.from(response.body)[0];
								localStorage.setItem("apteka", JSON.stringify(apteka));
								this.$store.dispatch("SET_ISAUTHENTICATED", true);
								this.$store.dispatch("SET_CURRENTAPTEKA", apteka);
								if (this.$route.query.from) {
									this.$router.push({path: this.$route.query.from});
								} else {
									this.$router.push({path: "/support/tickets"});
								}
							}, error => console.log(error)
						);

					}, error => console.error(error)
				);
			},
		},
		computed: {
			loginErrorClass() {
				return {
					"md-invalid": this.wrongName
				};
			},
			passwordErrorClass() {
				return {
					"md-invalid": this.wrongPassword
				};
			}
		}
	};
</script>


<style scoped>
		.bg {
				background: url("../../assets/bg_auth.jpg");
				opacity: 0.5;
				position: absolute;
				top: 0;
				right: 0;
				bottom: 0;
				left: 0;
		}

		.wrapper {
				display: flex;
				flex-direction: column;
				height: 100vh;
				flex: 1;
		}

		.content {
				display: flex;
				flex-direction: row;
		}

		.md-card {
				width: 460px;
				padding: 20px;
		}

		.content:before,
		.content:after,
		.wrapper:before,
		.wrapper:after {
				content: "";
				flex: 1;
		}
</style>
