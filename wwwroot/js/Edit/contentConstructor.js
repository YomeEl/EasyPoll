let cntr = 0;
let questionCntr = 0;

let optionsInputs = [];

let contentDiv = document.getElementById('content');

let selectedQuestion = 0;

constructNewPoll();

function constructNewQuestion() {
	clearContent();
	cntr = 0;
	optionsInputs = [];
	
	appendImage();
	hr();
	appendSectionTitle('Текст вопроса');
	appendText();
	hr();
	appendSectionTitle('Варианты ответа');
	appendOptionsDiv();
	for (let i = 0; i < options[selectedQuestion].length; i++)
	{
		appendOption(options[selectedQuestion][i]);	
	}
	appendOptionsButtons();
	hr();
	appendSubmitQuestionButton();
	
	if (cntr <= 1)
	{
		let btnRemove = document.getElementById('btnRemove');
		btnRemove.style = 'display: none';
	}
}

function clearContent() {
	contentDiv.innerHTML = '';
}

function hr()
{
	let line = document.createElement('hr');
	contentDiv.append(line);
}

function appendSectionTitle(title)
{
	let label = document.createElement('label');
	label.className = 'question';
	label.style = 'width: 100%; font-size: 150%';
	label.innerText = title;
	
	contentDiv.append(label);
}

function appendImage() {
	let img = document.createElement('img');
	img.src = 'img/sample_img.png';
	
	let btn = document.createElement('div');
	btn.className = 'btn-container';
	btn.style = 'margin-top: 5px';
	let a = document.createElement('a');
	a.innerText = 'Загрузить изображение';
	btn.append(a);
	
	let wrapper = document.createElement('div');
	wrapper.className = 'img-wrapper';
	wrapper.append(img);
	
	contentDiv.append(wrapper, btn);
}

function appendText() {
	 let text = document.createElement('textarea');
	 text.id = 'text';
	 text.value = questions[selectedQuestion];
	 text.className = 'login-form-input';
	 
	 contentDiv.append(text);
}

function appendOptionsDiv()
{
	let optionsDiv = document.createElement('div');
	optionsDiv.id = 'options';
	
	contentDiv.append(optionsDiv);
}

function appendOption(opt) {
	cntr++;

    let input = document.createElement('input');
    input.className = 'login-form-input';
    input.type = 'text';
    input.placeholder = 'Текст ответа ' + cntr;
	input.value = opt;
	optionsInputs.push(input);

    let a = document.createElement('a');
    a.innerText = 'Загрузить изображение';
	
    let optionDiv = document.createElement('div');
    optionDiv.id = 'option' + cntr;
	optionDiv.style = 'margin-bottom: 1vh';
    optionDiv.append(input, a);

    let optionsDiv = document.getElementById('options');
    optionsDiv.append(optionDiv);
	
	let btnRemove = document.getElementById('btnRemove');
	if (btnRemove)
		btnRemove.style = '';
}

function removeOption() {
	if (cntr == 0) 
		return;
	
	let optionDiv = document.getElementById('option' + cntr);
	optionDiv.remove();
	optionsInputs.pop();
	
	cntr--;
	if (cntr == 0)
	{
		let btnRemove = document.getElementById('btnRemove');
		btnRemove.style = 'display: none';
	}
}

function appendOptionsButtons() {
	let btnAdd = createButton('Добавить вариант', () => { appendOption('') });
	btnAdd.id = 'btnAdd';
	
	let btnRemove = createButton('Удалить вариант', removeOption);
	btnRemove.id = 'btnRemove';
	
	let wrapper = document.createElement('div');
	wrapper.className = 'btn-container';
	wrapper.style = 'flex-wrap: wrap; flex-direction: row; justify-content: center';
	wrapper.append(btnAdd, btnRemove);
	
	contentDiv.append(wrapper);
}

function createButton(text, onclick) {
	let label = document.createElement('label');
	label.className = 'btn-text';
	label.innerText = text;
	
	let wrapper = document.createElement('div');
	wrapper.className = 'btn-common';
	wrapper.onclick = onclick;
	wrapper.append(label);
	
	return wrapper;
}

function appendSubmitQuestionButton() {
	let btn = createButton('Сохранить', null);
	btn.onclick = () => { 
		questions[selectedQuestion] = document.getElementById('text').value;
		options[selectedQuestion] = [];
		for (let i = 0; i < optionsInputs.length; i++)
		{
			options[selectedQuestion].push(optionsInputs[i].value);
		}
		constructNewPoll(); 
	};
	contentDiv.append(btn);
	
	let a = document.createElement('a');
	a.onclick = () => { constructNewPoll() };
	a.innerText = 'Отмена';
	
	let wrapper = document.createElement('div');
	wrapper.className = 'btn-container';
	wrapper.append(btn, a);
	
	contentDiv.append(wrapper);
}



function constructNewPoll() {
	questionCntr = 0;
	clearContent();
	
	appendSectionTitle('Название опроса');
	appendInput();
	hr();
	appendSectionTitle('Даты');
	appendDates();
	hr();
	appendSectionTitle('Уведомления');
	appendCheckboxes();
	hr();
	appendSectionTitle('Список вопросов');
	appendQuestionsList();
	hr();
	appendWarningsDiv();
	appendSubmitPollButton();
}

function appendInput() {
	let input = document.createElement('input');
	input.id = 'pollName';
    input.className = 'login-form-input';
    input.type = 'text';
    input.placeholder = 'Введите название';
	input.value = name;
	input.onchange = () => { name = document.getElementById('pollName').value; };
	
	contentDiv.append(input);
}

function appendDates() {
	let label1 = document.createElement('label');
	label1.className = 'question';
	label1.innerText = 'Дата начала:';
	
	let input1 = document.createElement('input');
	input1.id = 'startAt';
	input1.type = 'datetime-local';
	input1.style = 'margin-left: 5px; margin-bottom: 1vh;';
	input1.onchange = () => { startAt = document.getElementById('startAt').value; };
	input1.value = startAt;
	
	let br = document.createElement('br');
	
	let label2 = document.createElement('label');
	label2.className = 'question';
	label2.innerText = 'Дата окончания:';
	
	let input2 = document.createElement('input');
	input2.id = 'finishAt';
	input2.type = 'datetime-local';
	input2.style = 'margin-left: 5px';
	input2.onchange = () => { finishAt = document.getElementById('finishAt').value; };
	input2.value = finishAt;
	
	contentDiv.append(label1, input1, br, label2, input2);
}

function appendCheckboxes() {
	let checkbox1 = document.createElement('input');
	checkbox1.id = 'sendStart';
	checkbox1.className = 'checkbox';
	checkbox1.type='checkbox';
	checkbox1.checked = sendStart;
	checkbox1.onchange = () => { sendStart = document.getElementById('sendStart').checked; };

	let label1 = document.createElement('label');
	label1.className = 'checkbox-label';
	label1.setAttribute('for', 'sendStart');
	label1.innerText = 'Отправить уведомление сотрудникам о начале опроса';
	
	let wrapper1 = document.createElement('div');
	wrapper1.style = 'display: flex; align-items: center;';
	wrapper1.append(checkbox1, label1); 
	
	let checkbox2 = document.createElement('input');
	checkbox2.id = 'sendFinish';
	checkbox2.className = 'checkbox';
	checkbox2.type='checkbox';
	checkbox2.checked = sendFinish;
	checkbox2.oncellchange = () => { sendFinish = document.getElementById('sendFinish').checked; };
	
	let label2 = document.createElement('label');
	label2.className = 'checkbox-label';
	label2.setAttribute('for', 'sendEnd');
	label2.innerText = 'Отправить уведомление сотрудникам об окончании опроса';
	
	let wrapper2 = document.createElement('div');
	wrapper2.style = 'display: flex; align-items: center;';
	wrapper2.append(checkbox2, label2); 
	
	let wrapper = document.createElement('div');
	wrapper.append(wrapper1, wrapper2);
	
	contentDiv.append(wrapper);
}

function appendQuestionsList() {
	let ol = document.createElement('ol');
	ol.id = 'questionsList'
	ol.className = 'questions-list';
	
	let a = document.createElement('a');
	a.innerText = 'Добавить вопрос';
	a.onclick = () => { addQuestion(); constructNewPoll(); };
	
	contentDiv.append(ol, a);
	
	for (let i = 0; i < questions.length; i++)
	{
		appendQuestion(questions[i]);
	}
}

function appendQuestion(name) {	
	let label = document.createElement('label');
	label.innerText = name;
	
	let br = document.createElement('br');
	
	let i = questionCntr;
	
	let a1 = document.createElement('a');
	a1.className = 'separator';
	a1.innerText = 'редактировать';
	a1.onclick = () => { selectedQuestion = i; constructNewQuestion(); };
	
	let a2 = document.createElement('a');
	a2.className = 'separator';
	a2.innerText = 'удалить';
	a2.onclick = () => { removeQuestion(i); constructNewPoll(); };
	
	let a3 = document.createElement('a');
	a3.className = 'separator';
	a3.innerText = 'вверх';
	a3.onclick = () => { moveUp(i); constructNewPoll(); };
	
	let a4 = document.createElement('a');
	a4.innerText = 'вниз';
	a4.onclick = () => { moveDown(i); constructNewPoll(); };
	
	let li = document.createElement('li');
	li.append(label, br, a1, a2, a3, a4);

	let ol = document.getElementById('questionsList');
	ol.append(li);
	
	questionCntr++;
}

function appendWarningsDiv() {
	let div = document.createElement('div');
	div.id = 'warnings';
	div.className = 'login-warning-wrapper';
	
	contentDiv.append(div);
}

function appendSubmitPollButton() {
	let btn = createButton('Добавить опрос', () => {
		let warnings = submitPoll();
		if (warnings.length > 0)
		{
			let warningsDiv = document.getElementById('warnings');
			warningsDiv.innerHTML = '';
			for (let i = 0; i < warnings.length; i++)
			{
				let label = document.createElement('label');
				label.className = 'login-warning';
				label.innerText = warnings[i];
				warningsDiv.append(label);
			}
		}
	});
	contentDiv.append(btn);
	
	let wrapper = document.createElement('div');
	wrapper.className = 'btn-container';
	wrapper.append(btn);
	
	contentDiv.append(wrapper);
}