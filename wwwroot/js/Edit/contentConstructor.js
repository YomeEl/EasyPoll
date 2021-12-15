const commonElements = (function () {
	let contentDiv = document.getElementById('content');

	let selectedQuestion = 0;

	function appendHr() {
		let line = document.createElement('hr');
		contentDiv.append(line);
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

	function clearContent() {
		contentDiv.innerHTML = '';
	}

	function appendSectionTitle(title) {
		let label = document.createElement('label');
		label.className = 'question';
		label.style = 'width: 100%; font-size: 150%';
		label.innerText = title;

		contentDiv.append(label);
	}

	return {
		contentDiv,
		selectedQuestion,
		appendHr,
		createButton,
		clearContent,
		appendSectionTitle
    }
})();

const questionConstructor = (function () {
	let optionCntr = 0;
	let optionsInputs = [];

	let deleteQuestionMedia = false;

	let optionsMediaDelta = {
		add: [],
		delete: []
    }

	function construct() {
		let titles = document.getElementsByName('pageTitle');
		titles.forEach((title) => {
			title.innerText = title.innerText.replace('опрос', 'вопрос');
		});

		commonElements.clearContent();
		optionCntr = 0;
		optionsInputs = [];
		deleteQuestionMedia = false;
		optionsMediaDelta = {
			add: [],
			delete: []
        }

		commonElements.appendSectionTitle('Медиа');
		appendMediaWithControls();
		commonElements.appendHr();
		commonElements.appendSectionTitle('Текст вопроса');
		appendText();
		commonElements.appendHr();
		commonElements.appendSectionTitle('Варианты ответа');
		appendOptionsDiv();
		editLogic.editData.questions[commonElements.selectedQuestion].options.forEach((option) => {
			appendOption(option);
		});
		appendOptionsButtons();
		commonElements.appendHr();
		appendSubmitQuestionButton();

		if (optionCntr <= 1) {
			let btnRemove = document.getElementById('btnRemove');
			btnRemove.style = 'display: none';
		}
	}

	function appendMediaWithControls() {
		let wrapper = mediaController.createMediaDiv(mediaController.loadedSrc[commonElements.selectedQuestion]);
		commonElements.contentDiv.append(wrapper);
		let controls = createMediaControls(wrapper.firstElementChild, wrapper.lastElementChild);
		commonElements.contentDiv.append(controls);
	}

	function createMediaControls(img, video) {
		let btnContainer = document.createElement('div');
		btnContainer.className = 'btn-container';
		btnContainer.style = 'margin-top: 5px';

		let input = mediaController.createMediaInput('questionMedia');
		input.onchange = () => {
			deleteQuestionMedia = true;
			previewMedia(null, img, video, input);
		};

		mediaController.init(input, img, video);
		let questionMedia = editLogic.editData.questions[commonElements.selectedQuestion].media;
		if (questionMedia) {
			previewMedia(questionMedia, img, video, input);
		}

		let label = document.createElement('label');
		label.setAttribute('for', 'questionMedia');
		label.className = 'upload-label';
		label.innerText = 'Выбрать медиа';

		let a = document.createElement('a');
		a.innerText = 'Удалить медиа';
		a.onclick = () => {
			deleteQuestionMedia = true;
			deleteMedia(input, img, video);
		}

		btnContainer.append(input, label, a);

		return btnContainer;
	}

	function previewMedia(file = null, img, video, input) {
		let oFReader = new FileReader();
		if (!file) {
			file = input.files[0];
		}
		oFReader.readAsDataURL(file);

		oFReader.onload = function (oFREvent) {
			let src = oFREvent.target.result;
			if (file.type.match(/image/)) {
				img.src = oFREvent.target.result;
				img.style = '';
				img.setAttribute('hide', 'false');
				video.style = 'display: none';
				video.setAttribute('hide', 'true');
			}
			else {
				video.src = src;
				video.style = '';
				video.setAttribute('hide', 'false');
				img.style = 'display: none';
				img.setAttribute('hide', 'true');
			}
		};
	};

	function deleteMedia(input, img, video) {
		input.value = '';
		img.src = '';
		img.style = 'display: none';
		img.setAttribute('hide', 'true');
		video.src = '';
		video.style = 'display: none';
		video.setAttribute('hide', 'true');
	}

	function appendText() {
		let text = document.createElement('textarea');
		text.id = 'text';
		text.value = editLogic.editData.questions[commonElements.selectedQuestion].name;
		text.className = 'login-form-input';

		commonElements.contentDiv.append(text);
	}

	function appendOptionsDiv() {
		let optionsDiv = document.createElement('div');
		optionsDiv.id = 'options';

		commonElements.contentDiv.append(optionsDiv);
	}

	function appendOption(opt) {
		optionCntr++;
		let optionNum = optionCntr - 1;

		let label = document.createElement('label');
		label.className = 'question';
		label.innerText = `Вариант ${optionCntr}`;
		label.style = 'width: 100%; font-weight: bold;';

		let src = mediaController.loadedOptionSrc[commonElements.selectedQuestion][optionNum];
		if (opt.text == '') {
			src = '';
        }
		let media = mediaController.createMediaDiv(src);
		media.className = 'img-wrapper-small';
		let img = media.firstElementChild;
		let video = media.lastElementChild;

		let input = document.createElement('input');
		input.className = 'login-form-input';
		input.type = 'text';
		input.placeholder = 'Текст ответа ' + optionCntr;
		input.value = opt.text;
		optionsInputs.push(input);

		let mediaInput = mediaController.createMediaInput(`option${optionNum}Media`)
		mediaInput.onchange = () => {
			let src = mediaController.loadedOptionSrc[commonElements.selectedQuestion][optionNum];
			if (src != '' && optionsMediaDelta.delete.findIndex((d) => d == optionNum) == -1) {
				optionsMediaDelta.delete.push(optionNum);
			}
			optionsMediaDelta.add[optionNum] = mediaInput.files[0];
			previewMedia(null, img, video, mediaInput);
			console.log(optionsMediaDelta);
		}

		let q = editLogic.editData.questions[commonElements.selectedQuestion];
		let o = q.options[optionNum];
		if (o && o.media) {
			previewMedia(o.media, img, video, mediaInput);
		}

		let uploadLabel = document.createElement('label');
		uploadLabel.className = 'upload-label separator';
		uploadLabel.style = 'margin-bottom: 0';
		uploadLabel.innerText = 'выбрать медиа';
		uploadLabel.setAttribute('for', `option${optionNum}Media`)

		let a = document.createElement('a');
		a.innerText = 'удалить медиа';
		a.onclick = () => {
			let src = mediaController.loadedOptionSrc[commonElements.selectedQuestion][optionNum];
			if (src != '' && optionsMediaDelta.delete.findIndex((d) => d == optionNum) == -1) {
				optionsMediaDelta.delete.push(optionNum);
            }
			if (optionsMediaDelta.add[optionNum]) {
				optionsMediaDelta.add[optionNum] = null;
            }
			deleteMedia(input, img, video);
			console.log(optionsMediaDelta);
		}

		let container = document.createElement('div');
		container.className = 'btn-container';
		container.style = 'flex-direction: row; justify-content: center;';
		container.append(mediaInput, uploadLabel, a);

		let hr = document.createElement('hr');

		let optionDiv = document.createElement('div');
		optionDiv.id = 'option' + optionCntr;
		optionDiv.append(hr, label, media, container, input);

		let optionsDiv = document.getElementById('options');
		optionsDiv.append(optionDiv);
	}

	function removeOption() {
		if (optionCntr == 0)
			return;

		let optionDiv = document.getElementById('option' + optionCntr);
		optionDiv.remove();
		optionsInputs.pop();

		optionCntr--;
		if (optionCntr == 0) {
			let btnRemove = document.getElementById('btnRemove');
			btnRemove.style = 'display: none';
		}

		let src = mediaController.loadedOptionSrc[commonElements.selectedQuestion][optionCntr];
		if (src != '' && optionsMediaDelta.delete.findIndex((d) => d == optionCntr) == -1) {
			optionsMediaDelta.delete.push(optionCntr);
		}
	}

	function appendOptionsButtons() {
		let btnAdd = commonElements.createButton('Добавить вариант', () => {
			appendOption({
				text: '',
				media: null
			})
		});
		btnAdd.id = 'btnAdd';

		let btnRemove = commonElements.createButton('Удалить вариант', removeOption);
		btnRemove.id = 'btnRemove';

		let wrapper = document.createElement('div');
		wrapper.className = 'btn-container';
		wrapper.style = 'flex-wrap: wrap; flex-direction: row; justify-content: center';
		wrapper.append(btnAdd, btnRemove);

		commonElements.contentDiv.append(wrapper);
	}

	function appendSubmitQuestionButton() {
		let btn = commonElements.createButton('Сохранить', () => {
			let sel = commonElements.selectedQuestion;
			let optMedia = editLogic.editData.questions[sel].options.map((opt) => opt.media);
			editLogic.resetQuestion(sel, document.getElementById('text').value);
			optionsInputs.forEach((input, i) => {
				editLogic.addOption(sel, input.value, optMedia[i]);
			});
			if (deleteQuestionMedia) {
				mediaController.deleteQuestionMedia(sel);
            }
			editLogic.setQuestionMedia(sel);

			mediaController.data.deletedOptionsMedia[sel] = optionsMediaDelta.delete;

			editLogic.editData.questions[sel].options.forEach((opt, i) => {
				if (optionsMediaDelta.add[i]) {
					opt.media = optionsMediaDelta.add[i];
                }
			});

			pollConstructor.construct();
		});
		commonElements.contentDiv.append(btn);

		let a = document.createElement('a');
		a.onclick = () => { pollConstructor.construct() };
		a.innerText = 'Отмена';

		let wrapper = document.createElement('div');
		wrapper.className = 'btn-container';
		wrapper.append(btn, a);

		commonElements.contentDiv.append(wrapper);
	}

	return {
		construct
    }
})();

const pollConstructor = (function () {
	let questionCntr = 0;

	function construct() {
		let titles = document.getElementsByName('pageTitle');
		titles.forEach((title) => {
			title.innerText = title.innerText.replace('вопрос', 'опрос');
		});

		questionCntr = 0;
		commonElements.clearContent();

		commonElements.appendSectionTitle('Название опроса');
		appendInput();
		commonElements.appendHr();
		commonElements.appendSectionTitle('Даты');
		appendDates();
		commonElements.appendHr();
		commonElements.appendSectionTitle('Уведомления');
		appendCheckboxes();
		commonElements.appendHr();
		commonElements.appendSectionTitle('Список вопросов');
		appendQuestionsList();
		commonElements.appendHr();
		appendWarningsDiv();
		appendSubmitPollButton();
	}

	function appendInput() {
		let input = document.createElement('input');
		input.id = 'pollName';
		input.className = 'login-form-input';
		input.type = 'text';
		input.placeholder = 'Введите название';
		input.value = editLogic.editData.newName;
		input.onchange = () => { editLogic.editData.newName = document.getElementById('pollName').value; };

		commonElements.contentDiv.append(input);
	}

	function convertDate(date) {
		if (!date) return '';

		let dd = date.getDate().toString();
		let MM = (date.getMonth() + 1).toString();
		let yyyy = date.getFullYear().toString();
		let hh = date.getHours().toString();
		let mm = date.getMinutes().toString();

		if (dd.length === 1) dd = '0' + dd;
		if (MM.length === 1) MM = '0' + MM;
		if (hh.length === 1) hh = '0' + hh;
		if (mm.length === 1) mm = '0' + mm;

		return `${yyyy}-${MM}-${dd}T${hh}:${mm}`
	}

	function appendDates() {
		let label1 = document.createElement('label');
		label1.className = 'question';
		label1.innerText = 'Дата начала:';

		let input1 = document.createElement('input');
		input1.id = 'startAt';
		input1.type = 'datetime-local';
		input1.style = 'margin-left: 5px; margin-bottom: 1vh;';
		input1.value = convertDate(editLogic.editData.startAt);
		input1.onchange = () => {
			editLogic.editData.startAt = new Date(input1.value);
		};

		let br = document.createElement('br');

		let label2 = document.createElement('label');
		label2.className = 'question';
		label2.innerText = 'Дата окончания:';

		let input2 = document.createElement('input');
		input2.id = 'finishAt';
		input2.type = 'datetime-local';
		input2.style = 'margin-left: 5px';
		input2.value = convertDate(editLogic.editData.finishAt);
		input2.onchange = () => {
			editLogic.editData.finishAt = new Date(input2.value);
		};

		commonElements.contentDiv.append(label1, input1, br, label2, input2);
	}

	function appendCheckboxes() {
		let checkbox1 = document.createElement('input');
		checkbox1.id = 'sendStart';
		checkbox1.className = 'checkbox';
		checkbox1.type = 'checkbox';
		checkbox1.checked = editLogic.editData.sendStart;
		checkbox1.onchange = () => { editLogic.editData.sendStart = document.getElementById('sendStart').checked; };

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
		checkbox2.type = 'checkbox';
		checkbox2.checked = editLogic.editData.sendFinish;
		checkbox2.onchange = () => { editLogic.editData.sendFinish = document.getElementById('sendFinish').checked; };

		let label2 = document.createElement('label');
		label2.className = 'checkbox-label';
		label2.setAttribute('for', 'sendFinish');
		label2.innerText = 'Отправить уведомление сотрудникам об окончании опроса';

		let wrapper2 = document.createElement('div');
		wrapper2.style = 'display: flex; align-items: center;';
		wrapper2.append(checkbox2, label2);

		let wrapper = document.createElement('div');
		wrapper.append(wrapper1, wrapper2);

		commonElements.contentDiv.append(wrapper);
	}

	function appendQuestionsList() {
		let ol = document.createElement('ol');
		ol.id = 'questionsList'
		ol.className = 'questions-list';

		let a = document.createElement('a');
		a.innerText = 'Добавить вопрос';
		a.onclick = () => { editLogic.addQuestion(); construct(); };

		commonElements.contentDiv.append(ol, a);

		editLogic.editData.questions.forEach((question) => {
			appendQuestion(question.name)
		});
	}

	function appendQuestion(name) {
		let label = document.createElement('label');
		label.innerText = name;

		let br = document.createElement('br');

		let i = questionCntr;

		let a1 = document.createElement('a');
		a1.className = 'separator';
		a1.innerText = 'редактировать';
		a1.onclick = () => { commonElements.selectedQuestion = i; questionConstructor.construct(); };

		let a2 = document.createElement('a');
		a2.className = 'separator';
		a2.innerText = 'удалить';
		a2.onclick = () => { editLogic.removeQuestion(i); construct(); };

		let a3 = document.createElement('a');
		a3.className = 'separator';
		a3.innerText = 'вверх';
		a3.onclick = () => { editLogic.moveUp(i); construct(); };

		let a4 = document.createElement('a');
		a4.innerText = 'вниз';
		a4.onclick = () => { editLogic.moveDown(i); construct(); };

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

		commonElements.contentDiv.append(div);
	}

	function appendSubmitPollButton() {
		let btnText = editLogic.newPoll ? 'Добавить опрос' : 'Сохранить опрос';
		let btn = commonElements.createButton(btnText, () => {
			let warnings = editLogic.submitPoll();

			if (warnings.length === 0) {
				btn.firstElementChild.innerText = 'Сохранение...';
				btn.onclick = null;
			}

			let warningsDiv = document.getElementById('warnings');
			warningsDiv.innerHTML = '';
			warnings.forEach((warning) => {
				let label = document.createElement('label');
				label.className = 'login-warning';
				label.innerText = warning;
				warningsDiv.append(label);
			});
		});
		btn.firstElementChild.id = 'submitBtnLabel';
		commonElements.contentDiv.append(btn);

		let a = document.createElement('a');
		a.onclick = () => location.assign('/Settings/ControlPanel');
		a.innerText = 'Отмена';

		let wrapper = document.createElement('div');
		wrapper.className = 'btn-container';
		wrapper.append(btn, a);

		commonElements.contentDiv.append(wrapper);
	}

	return {
		construct
    }
})();

pollConstructor.construct();