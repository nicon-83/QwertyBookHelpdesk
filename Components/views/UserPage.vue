<template>
		<main-parent>
				<div class="bg"></div>
				<div class="wrapper">
						<div class="content">
								<md-card>
										<md-card-header>
												<div class="md-title" v-if="CURRENTUSER.userId">Страница пользователя</div>
												<div class="md-title" v-else>Регистрация пользователя</div>
												<div class="md-subhead"></div>
										</md-card-header>

										<md-card-content>
												<md-field :class="loginErrorClass" :md-counter="false" md-clearable>
														<label>Фамилия</label>
														<md-input type="text" v-model.trim="lastName" @input="clearUserNameError" maxlength="30" required/>
														<span class="md-error">Неверный логин</span>
												</md-field>
												<md-field :class="loginErrorClass" :md-counter="false" md-clearable>
														<label>Имя</label>
														<md-input type="text" v-model.trim="firstName" @input="clearUserNameError" maxlength="30" required/>
														<span class="md-error">Неверный логин</span>
												</md-field>
												<md-field :class="loginErrorClass" :md-counter="false" md-clearable>
														<label>Отчество</label>
														<md-input type="text" v-model.trim="middleName" @input="clearUserNameError" maxlength="30" required/>
														<span class="md-error">Неверный логин</span>
												</md-field>
												<md-field :class="emailErrorClass" :md-counter="false" md-clearable>
														<label>email</label>
														<md-input type="email" v-model.trim="email" @input="clearEmailError" maxlength="50" required/>
														<span class="md-error">email уже существует</span>
												</md-field>
												<md-field :class="passwordErrorClass" :md-counter="false">
														<label>Пароль</label>
														<md-input v-model.trim="password" @input="clearPasswordError" type="password" maxlength="30" required/>
														<span class="md-error">Неверный пароль</span>
												</md-field>
												<md-checkbox v-model="isSendByEmail" class="md-primary">Отправлять ответы по эл.почте</md-checkbox>
												<md-checkbox v-model="isSendBySystemOrder" class="md-primary">Отправлять ответы по системе Заказ</md-checkbox>
										</md-card-content>

										<md-card-actions>
												<md-button class="md-dense md-primary" @click="goToTicketsList">Вернуться к списку заявок</md-button>
												<md-button class="md-dense md-primary" @click="save" :disabled="!validate">Сохранить</md-button>
										</md-card-actions>
								</md-card>
						</div>
				</div>
		</main-parent>
</template>


<script>
	import MainParent from "../MainParent";
	import {mapGetters} from "vuex";

	export default {
		components: {
			MainParent
		},
		data: () => ({
			firstName: "",
			lastName: "",
			middleName: "",
			email: '',
			password: "",
			wrongName: false,
			wrongPassword: false,
			wrongEmail: false,
			isSendByEmail: true,
			isSendBySystemOrder: true
		}),
		computed: {
			...mapGetters([
				"CURRENTAPTEKA",
				"CURRENTUSER"
			]),
			loginErrorClass() {
				return {
					"md-invalid": this.wrongName
				};
			},
			passwordErrorClass() {
				return {
					"md-invalid": this.wrongPassword
				};
			},
			emailErrorClass() {
				return {
					"md-invalid": this.wrongEmail
				};
			},
			validate() {
				if (this.name === '' || this.lastName === '' || this.middleName === '' || this.email === '' || !this.validateEmail(this.email) || this.password === '') return false;
				return true;
			}
		},
		methods: {
			search(apteka_id) {
				return this.$http.get("data/getClient", {
					params: {
						apteka_id: apteka_id
					}
				});
			},
			auth() {
				if (!this.isNumeric(this.username)) {
					this.wrongName = true;
					return;
				}
				this.search(this.username).then(
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
							snackText: `Ошибка получения данных аптеки ${this.username}`
						});
					}
				);
			},
			clearUserNameError() {
				this.wrongName = false;
			},
			clearPasswordError() {
				this.wrongPassword = false;
			},
			clearEmailError() {
				this.wrongEmail = false;
			},
			isNumeric(n) {
				return !isNaN(parseFloat(n)) && isFinite(n);
			},
			goToTicketsList() {
				this.$store.dispatch("SET_SUPPORTCURRENTTABROUTE", "tickets");
				this.$router.push({path: "/support/tickets"});
			},
			save() {
				const params = {
					aptekaId: this.CURRENTAPTEKA.idapt,
					userId: this.CURRENTUSER.userId ? this.CURRENTUSER.userId : 0,
					firstName: this.firstName,
					lastName: this.lastName,
					middleName: this.middleName,
					email: this.email,
					password: this.password,
					isSendByEmail: this.isSendByEmail ? 1 : 0,
					isSendBySystemOrder: this.isSendBySystemOrder ? 1 : 0
				};
				this.$http.put("data/SaveUser", params, {emulateJSON: true}).then(
					(resp) => {
						// this.$store.dispatch('SET_ISAUTHENTICATED', false);
						if (resp.body.statusCode > 200) {
							this.$store.dispatch("showSnackBar", {
								snackText: `Внимание! ${resp.body.reasonPhrase}`
							});
							this.wrongEmail = true;
							return;
						}
						this.getSiteUser().then(
							response => {
								//получаем пользователя из БД
								let user = response.data[0];
								if (!user) {
									this.$store.dispatch("showSnackBar", {
										snackText: `Ошибка записи информации в БД`
									});
									return;
								}
								//сохраняем его в localStorage и store
								localStorage.setItem('user', JSON.stringify(user));
								this.$store.dispatch('SET_CURRENTUSER', user);
								//переходим на страницу с заявками
								this.$router.push({path: "/support/tickets"});
							}, error => console.error(error)
						);
					},
					error => {
						console.error(error);
					}
				);
			},
			getSiteUser() {
				return this.$http.get("data/GetSiteUser", {
					params: {aptekaId: this.CURRENTAPTEKA.idapt, email: this.email, ps: this.password}
				});
			},
			validateEmail(email) {
				let re = /\S+@\S+\.\S+/;
				return re.test(email);
			}
		},
		mounted() {
			//если текущий пользователь установлен
			if (this.CURRENTUSER.userId) {
				this.firstName = this.CURRENTUSER.firstName;
				this.lastName = this.CURRENTUSER.lastName;
				this.middleName = this.CURRENTUSER.middleName;
				this.email = this.CURRENTUSER.email;
				this.isSendByEmail = this.CURRENTUSER.sendByMail;
				this.isSendBySystemOrder = this.CURRENTUSER.sendBySystemOrder;
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
				width: 480px;
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
