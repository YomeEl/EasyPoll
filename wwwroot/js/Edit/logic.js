/*
interface Question {
	name: string;
	options: string[];
}
*/

const editLogic = (function () {
	const editData = {
		name: '',  // string
		startAt: null,  // Date
		finishAt: null,  // Date
		sendStart: false,  // boolean
		sendFinish: false,  // boolean
		questions: [  // Question[]
			{
				name: 'Вопрос 1',
				options: []
			},
			{
				name: 'Вопрос 2',
				options: []
			}
		]
	}

	function addQuestion () {
		editData.questions.push({
			name: 'Новый вопрос',
			options: []
		});
	}

	function removeQuestion (index) {
		editData.questions.splice(i, 1);
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
		if (editData.startAt > editData.finishAt) warnings.push('Окончание вопроса указано раньше его начала');
		if (editData.questions.length === 0) warnings.push('Не добавлено ни одного вопроса');

		editData.questions.forEach((question, i) => {
			if (question.length === 0) {
				warnings.push(`Вопрос ${i + 1}: не заданы варианты ответа`);
			}
			question.options.forEach((option, j) => {
				if (option === '') {
					warnings.push(`Вопрос ${i + 1}: текст ответа ${j + 1} не задан`);
				}
			}
		})
			
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
				body: URLSearchParams({
					name: editData.name,
					startAtRaw: editData.startAt.toISOString(),
					finishAtRaw: editData.finishAt.toISOString(),
					sendStartRaw: editData.sendStart,
					sendFinishRaw: editData.sendFinish,
					questionsRaw: editData.questions.map(question => question.name),
					optionsRaw: editData.questions.map(question => question.options)
				}
			).then((response) => {
				window.location.assign('/Settings/ControlPanel');
			}).catch((err) => console.error(err));
		}

		return warnings;
	}
					   
	return {
		editData,
		addQuestion,
		removeQuestion,
		moveUp,
		moveDown,
		validateData,
		submitPoll
	}
})()
