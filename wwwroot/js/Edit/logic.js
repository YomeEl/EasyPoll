/*
interface Question {
	name: string;
	options: Option[];
	media: File;
}
*/

/*
interface Option {
	text: string;
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

	function addOption(questionIndex, option, media) {
		editData.questions[questionIndex].options.push({
			text: option,
			media: media
		});
    }

	function resetQuestion(questionIndex, name) {
		questionsChanged = true;
		editData.questions[questionIndex].name = name;
		editData.questions[questionIndex].options = [];
    }

	function setQuestionMedia(questionIndex) {
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
		let warnings = validateData();

		if (warnings.length === 0) {
			updatePoll().then((r) => r.text().then((id) => {
				deleteFiles(id).then((r) => r.text().then(() => {
					uploadFilesAndRedirect(id)
				}));
			}));
		}

		return warnings;
	}

	function updatePoll() {
		return fetch('/Poll/AddNew', {
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
				optionsRaw: JSON.stringify(editData.questions.map(question => question.options.map((opt) => opt.text))),
				questionsChangedRaw: questionsChanged
			})
		})
    }

	function deleteFiles(id) {
		return fetch('/Poll/DeleteFiles', {
			method: 'POST',
			body: new URLSearchParams({
				questionsRaw: JSON.stringify(mediaController.data.deletedMedia),
				optionsRaw: JSON.stringify(mediaController.data.deletedOptionsMedia),
				pollId: id
			})
		})
	}

	function uploadFilesAndRedirect(id) {
		let uploadPromises = [];
		editData.questions.forEach((question, i) => {
			if (question.media) {
				let uploadForm = new FormData();
				uploadForm.append('file', question.media);
				uploadForm.append('pollId', id);
				uploadForm.append('questionIndex', i);
				uploadPromises.push(fetch('/Poll/UploadFile', {
					method: 'POST',
					body: uploadForm
				}));
			}
			question.options.forEach((option, j) => {
				if (option.media) {
					let uploadForm = new FormData();
					uploadForm.append('file', option.media);
					uploadForm.append('pollId', id);
					uploadForm.append('questionIndex', i);
					uploadForm.append('optionIndex', j)
					uploadPromises.push(fetch('/Poll/UploadFile', {
						method: 'POST',
						body: uploadForm
					}));
				}
			})
		});
		Promise.all(uploadPromises).then(() => {
			document.location.assign('/Settings/ControlPanel');
		});
	}

	return {
		editData,
		newPoll,
		addQuestion,
		removeQuestion,
		addOption,
		resetQuestion,
		setQuestionMedia,
		moveUp,
		moveDown,
		submitPoll
	}
})()
