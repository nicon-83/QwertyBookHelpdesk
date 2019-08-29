<template>
		<div>
				<!-- <div style="display:flex;justify-content:space-between">
					<h3>Создание заявки</h3>
					<md-button
						type="button"
						class="md-icon-button md-primary md-theme-default"
						@click="closeTab(CURRENTTABID)"
					>
						<md-icon>close</md-icon>
						<md-tooltip md-direction="top" md-delay="500">Закрыть</md-tooltip>
					</md-button>
				</div>-->
				<h3>Создание заявки</h3>
				<form @submit.prevent="createTicket">
						<md-field md-clearable style="width:600px">
								<label for="title">Тема</label>
								<md-input type="text" placeholder="Введите тему" v-model.lazy.trim="title" id="title" required/>
						</md-field>
						<md-field style="width:180px">
								<label for="priority">Приоритет</label>
								<md-select
												placeholder="Приоритет"
												id="priority"
												name="priority"
												v-model="selectedPriority"
												md-dense
								>
										<md-option
														v-for="priority in PRIORITIES"
														:key="priority.id"
														:value="priority.name"
										>{{priority.name}}
										</md-option>
								</md-select>
						</md-field>
						<md-field md-clearable style="width:800px">
								<md-textarea
												style="resize:none;"
												id="message"
												placeholder="Текст сообщения"
												v-model.lazy.trim="message"
												rows="5"
												required
								></md-textarea>
						</md-field>
						<div id="attachments-link">
								<md-icon style="color: #000;">attach_file</md-icon>
								<span @click="clickFileInput()">Прикрепить файлы</span>
						</div>
						<md-field style="display: none">
								<label>Прикрепить файлы</label>
								<md-file id="file-input" @md-change="onFileUpload($event)" multiple/>
						</md-field>
						<transition name="fadeattachments">
								<div class="attachments-container" v-if="files">
										<div class="attachments-item" v-for="file of files" :key="file.name">
												<md-icon :md-src="`${calcTypeOfAttachment(file)}`" class="md-size-2x"/>
												<md-tooltip md-direction="bottom">{{file.name}} &#8195; {{bytesToSize(file.size)}}</md-tooltip>
												<span>{{file.name}} &nbsp; {{bytesToSize(file.size)}}</span>
												<span class="remove-attachment" @click="removeAttachment(file)">&times;</span>
										</div>
								</div>
						</transition>
						<md-button
										type="submit"
										class="md-dense md-raised md-primary md-theme-default"
										:disabled="!title || !message"
						>Создать
						</md-button>
				</form>
		</div>
</template>

<script>
	import {mapGetters} from "vuex";
	import axios from "axios";

	export default {
		props: [],
		components: {},
		name: "newTicket",
		data: () => ({
			selectedPriority: "обычный",
			title: "",
			message: "",
			files: [] //прикрепленные файлы
		}),
		methods: {
			createTicket() {
				axios
					.post(
						"data/CreateTicket",
						JSON.stringify({
							title: this.title,
							customer: this.CURRENTAPTEKA.npp,
							userId: this.USERID,
							state: "new",
							priority: this.npp,
							agent: "27", //Administrator по-умолчанию для новых заявок
							message: this.message,
							attachmentsData: this.attachmentsData.length > 0 ? JSON.stringify(this.attachmentsData) : '',
							idapt: this.CURRENTAPTEKA.idapt
						}),
						{headers: {"Content-Type": "application/json"}}
					)
					.then(response => {
						if (response.data.statusCode > 200) {
							this.$store.dispatch("showSnackBar", {
								snackText: `Ошибка! создания новой заявки`
							});
							console.error(response.data.reasonPhrase);
							return;
						}
						let string = response.data.reasonPhrase;
						let arr = string.split(';');
						let ticketNpp = arr[1];
						let messageNpp = arr[2];
						let ticketNumber = arr[3];

						//сохраняем вложения на сервер
						if (this.files.length > 0) {
							this.uploadFilesToServer(this.files, ticketNpp, messageNpp)
								.then(response => {
									console.log(response);
								}, error => console.error(error))
								.finally(() => {
									//закрывем вкладку
									this.closeTab(this.CURRENTTABID);
									this.$emit("createTicket"); //обновление списка заявок
									this.$store.dispatch("showSnackBar", {snackText: `Создана заявка ${ticketNumber}`});
								});
						} else {
							//закрывем вкладку
							this.closeTab(this.CURRENTTABID);
							this.$emit("createTicket"); //обновление списка заявок
							this.$store.dispatch("showSnackBar", {snackText: `Создана заявка ${ticketNumber}`});
						}
					})
					.catch(error => {
						console.error(error);
						this.$store.dispatch("showSnackBar", {
							snackText: "Ошибка создания новой заявки"
						});
					})
					.then(() => {
						// always executed
					});
			},
			closeTab(tab_id) {
				this.$emit("closeTab", tab_id);
				// this.$store.dispatch("SET_HASCREATETICKETTAB", false); //делаем активной кнопку "Создать заявку"
			},
			onFileUpload(fileList) {
				Array.from(fileList).forEach(file => {
					this.files.push(file);
				});
			},
			uploadFilesToServer(files, ticketNpp, messageNpp) {
				let formData = new FormData();
				files.forEach(file => formData.append('files', file));
				formData.append('ticketNpp', ticketNpp);
				formData.append('messageNpp', messageNpp);

				return this.$http.post('data/UploadAttachmentsFiles', formData);
			},
			calcTypeOfAttachment(file) {
				let fileName = file.name;
				//определяем расширение файла
				let fileType = fileName.slice((Math.max(0, fileName.lastIndexOf(".")) || Infinity) + 1);
				fileType = fileType.toLowerCase();
				return this.KNOWNFILETYPES.includes(fileType) ? `/dist/svg/${fileType}.svg` : `/dist/svg/raw.svg`;
			},
			clickFileInput() {
				document.getElementById('file-input').click();
			},
			removeAttachment(attachment) {
				let index = this.files.indexOf(attachment);
				if (index !== -1) this.files.splice(index, 1);
			},
			bytesToSize(bytes) {
				const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
				if (bytes === 0) return 'n/a';
				const i = parseInt(Math.floor(Math.log(Math.abs(bytes)) / Math.log(1024)), 10);
				if (i === 0) return `${bytes} ${sizes[i]}`;
				return `${(bytes / (1024 ** i)).toFixed(1)} ${sizes[i]}`;
			},
			// closeTabFromParent() {
			// 	// console.log("newTicket closeTabFromParent()");
			// 	this.closeTab(this.CURRENTTABID);
			// }
		},
		computed: {
			...mapGetters([
				"LAST_NUMBER",
				"PRIORITIES",
				"CURRENTTABID",
				"CURRENTAPTEKA",
				'USERID',
				'KNOWNFILETYPES'
			]),
			npp() {
				switch (this.selectedPriority) {
					case "самый низкий":
						return "very_low";
					case "низкий":
						return "low";
					case "обычный":
						return "normal";
					case "высокий":
						return "hight";
					case "безотлагательный":
						return "very_hight";
				}
			},
			attachmentsData() {
				let attacmentsData = [];
				this.files.forEach(file => {
					let {name: file_name, size: file_size, type: file_type} = file;
					attacmentsData.push({file_name, file_size, file_type});
				});
				return attacmentsData;
			}
		},
		mounted() {
			if (this.PRIORITIES.length === 0) this.$store.dispatch("GET_PRIORITIES");

			// this.$store.dispatch("setPageData", {
			// 	header: "Новая заявка",
			// 	status: "mounted",
			// 	data: {}
			// });
			// this.$root.$on("closeTabFromParent", this.closeTabFromParent); //подписываемся на событие - закрытие вкладки
		},
		beforeDestroy: function () {
			// console.log("newTicket beforeDestroy()");
			// this.$root.$off("closeTabFromParent", this.closeTabFromParent);
		}
	};
</script>

<style lang="scss" scoped>
		.attachments-container {
				display: flex;
				flex-wrap: wrap;
				margin-bottom: 10px;
		}

		.attachments-item {
				padding: 5px;
				height: 100px;
				width: 100px;
				border: 1px solid #DADADA;
				background-color: #F0F0F0;
				border-radius: 5px;
				margin-right: 10px;
				position: relative;
				display: flex;
				flex-direction: column;
		}

		.attachments-item span {
				font-size: 10px;
				white-space: nowrap;
				overflow: hidden;
		}

		.attachments-item span.remove-attachment {
				height: 16px;
				width: 16px;
				position: absolute;
				top: 0;
				right: 0;
				text-align: center;
				line-height: 16px;
				font-size: 18px;
				border-radius: 8px;
		}

		.attachments-item span.remove-attachment:hover {
				cursor: pointer;
				background-color: #448AFF;
				color: white;
		}

		#attachments-link {
				margin-bottom: 10px;
		}

		#attachments-link span {
				text-decoration: underline;
				padding: 10px 10px 10px 0;
		}

		#attachments-link span:hover {
				cursor: pointer;
				text-decoration: none;
		}

		.fadeattachments-enter-active, .fadeattachments-leave-active {
				transition: opacity .5s;
		}

		.fadeattachments-enter, .fadeattachments-leave-to {
				opacity: 0;
		}
</style>