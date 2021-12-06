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
		let file;
		if (mediaController.data.fileInput.files.length === 1) {
			file = mediaController.data.fileInput.files[0]
		}
		else {
			file = null;
        }
		editData.questions[questionIndex].media = file;
    }

	function setMedia(questionIndex) {
		let file = null;
		if (mediaController.data.fileInput.files.length != 0) {
			file = mediaController.data.fileInput.files[0];
		}
		editData.questions[questionIndex].media = file;
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

		if (editData.name === '') warnings.push('Название опроса не указано');
		if (editData.startAt === '') warnings.push('Дата начала не указана');
		if (editData.finishAt === '') warnings.push('Дата окончания не указана');
		if (editData.startAt > editData.finishAt) warnings.push('Окончание опроса указано раньше его начала');
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
				return response.text();
			}).catch((err) => console.error(err)).then((id) => {
				editData.questions.forEach((question, i) => {
					if (question.media) {
						let form = new FormData();
						form.append('file', question.media);
						form.append('pollId', id);
						form.append('questionIndex', i)
						fetch('/Poll/UploadFile', {
							method: 'POST',
							body: form
						});
					}
				})
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
