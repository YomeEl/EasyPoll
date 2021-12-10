/*
interface Question {
	name: string;
	options: string[];
	media: File;
}
*/

const editLogic = (function () {
	const editData = {
		oldName: '',  // string
		newName: '',  // string
		startAt: null,  // Date
		finishAt: null,  // Date
		sendStart: false,  // boolean
		sendFinish: false,  // boolean
		questions: []  // Question[]
	}

	let newPoll = true;
	let questionsChanged = false;

	function addQuestion() {
		questionsChanged = true;
		editData.questions.push({
			name: 'Новый вопрос',
			options: [],
			media: null
		});
	}

	function removeQuestion(index) {
		questionsChanged = true;
		editData.questions.splice(index, 1);
	}

	function addOption(questionIndex, option) {
		editData.questions[questionIndex].options.push(option);
    }

	function resetQuestion(questionIndex, name) {
		questionsChanged = true;
		editData.questions[questionIndex].name = name;
		editData.questions[questionIndex].options = [];
    }

	function setMedia(questionIndex) {
		if (mediaController.data.fileInput.files.length === 1) {
			editData.questions[questionIndex].media = mediaController.data.fileInput.files[0];
		}
    }

	function moveUp (index) {
		if (index === 0) return;

		[editData.questions[index], editData.questions[index - 1]] = [editData.questions[index - 1], editData.questions[index]];
	}

	function moveDown (index) {
		if (index === editData.questions.length - 1) return;

		[editData.questions[index], editData.questions[index + 1]] = [editData.questions[index + 1], editData.questions[index]];
	}

	function validateData () {
		const warnings = [];

		if (editData.newName === '') warnings.push('Название опроса не указано');
		if (editData.startAt === null) warnings.push('Дата начала не указана');
		if (editData.finishAt === null) warnings.push('Дата окончания не указана');
		if (editData.startAt > editData.finishAt) warnings.push('Окончание опроса указано раньше его начала');
		if (editData.finishAt < Date.now()) warnings.push('Окончание опроса указано раньше сегодняшней даты');
		if (editData.questions.length === 0) warnings.push('Не добавлено ни одного вопроса');

		editData.questions.forEach((question, i) => {
			if (question.options.length === 0) {
				warnings.push(`Вопрос ${i + 1}: не заданы варианты ответа`);
			}
			question.options.forEach((option, j) => {
				if (option === '') {
					warnings.push(`Вопрос ${i + 1}: текст ответа ${j + 1} не задан`);
				}
			})
		});
			
		return warnings
	}

	function submitPoll () {
		const warnings = validateData()

		if (warnings.length === 0) {
			//Update poll
			console.log('Updating poll...');
			fetch('/Poll/AddNew', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/x-www-form-urlencoded'
				},
				body: new URLSearchParams({
					oldName: editData.oldName,
					newName: editData.newName,
					startAtRaw: editData.startAt.toISOString(),
					finishAtRaw: editData.finishAt.toISOString(),
					sendStartRaw: editData.sendStart,
					sendFinishRaw: editData.sendFinish,
					questionsRaw: JSON.stringify(editData.questions.map(question => question.name)),
					optionsRaw: JSON.stringify(editData.questions.map(question => question.options)),
					questionsChangedRaw: questionsChanged
				})
			}).then((response) => {
				//Get new poll id
				console.log('Poll updated');
				return response.text();
			}).then((id) => {
				//Delete files
				console.log('Deleting files...');
				let deleteForm = new FormData();
				deleteForm.append('questionsRaw', JSON.stringify(mediaController.data.deletedMedia));
				deleteForm.append('pollId', id);
				fetch('/Poll/DeleteFiles', {
					method: 'POST',
					body: deleteForm
				}).then((response) => {
					console.log('Files deleted');
					//Upload files
					console.log('Uploading files...');
					let uploadPromises = [];
					editData.questions.forEach((question, i) => {
						if (question.media) {
							console.log(`Uploading media for question ${i + 1}...`);
							let uploadForm = new FormData();
							uploadForm.append('file', question.media);
							uploadForm.append('pollId', id);
							uploadForm.append('questionIndex', i)
							uploadPromises.push(fetch('/Poll/UploadFile', {
								method: 'POST',
								body: uploadForm
							}));
							uploadPromises[uploadPromises.length - 1].then(() => {
								console.log(`Question ${i + 1} done`)
							});
						}
					});
					Promise.all(uploadPromises).then(() => {
						console.log('Files uploaded');
						console.log('Redirecting...');
						document.location.assign('/Settings/ControlPanel');
					});
				});
			});
		}

		return warnings;
	}
					   
	return {
		editData,
		newPoll,
		addQuestion,
		removeQuestion,
		addOption,
		resetQuestion,
		setMedia,
		moveUp,
		moveDown,
		validateData,
		submitPoll
	}
})()
