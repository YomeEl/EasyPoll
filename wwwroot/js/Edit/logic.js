/*
interface Question {
	name: string;
	options: string[];
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

	function addQuestion () {
		editData.questions.push({
			name: 'Новый вопрос',
			options: []
		});
	}

	function removeQuestion (index) {
		editData.questions.splice(index, 1);
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
					optionsRaw: JSON.stringify(editData.questions.map(question => question.options))
				})
			}).then((response) => {
				window.location.assign('/Settings/ControlPanel');
			}).catch((err) => console.error(err));
		}

		return warnings;
	}
					   
	return {
		editData,
		newPoll,
		addQuestion,
		removeQuestion,
		moveUp,
		moveDown,
		validateData,
		submitPoll
	}
})()
