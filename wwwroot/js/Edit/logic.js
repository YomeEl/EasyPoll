let questions = ['Вопрос 1', 'Вопрос 2'];
let options = [[], []];

let name = '';
let startAt = '';
let finishAt = '';
let sendStart = false;
let sendFinish = false;

function addQuestion() {
	questions.push('Новый вопрос');
	options.push([]);
}

function removeQuestion(index) {
	for (let i = index; i < questions.length - 1; i++)
	{
		questions[i] = questions[i + 1];
		options[i] = options[i + 1];
	}
	questions.pop();
	options.pop();
}

function moveUp(index) {
	if (index == 0) return;
	
	let tmp1 = questions[index];
	questions[index] = questions[index - 1];
	questions[index - 1] = tmp1;
	
	let tmp2 = options[index];
	options[index] = options[index - 1];
	options[index - 1] = tmp2;
}

function moveDown(index) {
	if (index == questions.length - 1) return;
	
	let tmp1 = questions[index];
	questions[index] = questions[index + 1];
	questions[index + 1] = tmp1;
	
	let tmp2 = options[index];
	options[index] = options[index + 1];
	options[index + 1] = tmp2;
}

function submitPoll() {
	let warnings = [];
	if (name == '') warnings.push('Название опроса не указано');
	if (startAt == '') warnings.push('Дата начала не указана');
	if (finishAt == '') warnings.push('Дата окончания не указана');
	if (new Date(startAt) > new Date(finishAt)) warnings.push('Окончание вопроса указано раньше его начала');
	if (questions.length == 0)
	{
		warnings.push('Не добавлено ни одного вопроса');
	}
	for (let i = 0; i < options.length; i++) {
		if (options[i].length == 0) {
			warnings.push('Вопрос ' + (i + 1) + ': не заданы варианты ответа');
		}
		for (let j = 0; j < options[i].length; j++)
		{
			warnings.push('Вопрос ' + (i + 1) + ': текст ответа ' + (j + 1) + ' не задан');
		}
	}
		
	if (warnings.length == 0) {
		
	}
	
	return warnings;
}