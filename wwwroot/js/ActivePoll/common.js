let questions;
let options;
let allAnswers;
let answersByDepartment;
let userSelection;
let answered;

let index = 0;

let questionDiv = document.getElementById('question');
let answersDiv = document.getElementById('answers');
let buttonNext = document.getElementById('btn-next');
let buttonPrev = document.getElementById('btn-prev');
let buttonFinish = document.getElementById('btn-finish');

let showDetails = false;

let getInfo;

function init(pollId = 0) {
    showDetails = pollId != 0;
    let url = `/PollApi/GetPollInfo?id=${pollId}`;
	getInfo = fetch(url)
		.then((response) => response.text())
        .then((dataRaw) => {
            let data = JSON.parse(dataRaw);
            questions = data['questions'];
            options = data['options'];
            allAnswers = data['answers'];
            answersByDepartment = data['answersByDepartment'];
            answersByDepartment['all'] = allAnswers;
            userSelection = data['userselection'];
            answered = data['answered'];
			data['sources'].forEach((src, i) => {
                mediaController.loadedSrc[i] = src;
            });
            data['optionSources'].forEach((question, q) => question.forEach((optSrc, o) => {
                if (o == 0) {
                    mediaController.loadedOptionSrc[q] = [];
                }
                mediaController.loadedOptionSrc[q][o] = optSrc;
            }));
            reset();
		});
}

function reset() {
    clearCurrent();
    resetButtons();
    constructCurrent();
}

function resetButtons() {
    buttonNext.style = 'display: none';
    buttonPrev.style = 'display: none';
    buttonFinish.style = 'display: none';
}

function clearCurrent() {
    questionDiv.innerHTML = '';
    answersDiv.innerHTML = '';
}