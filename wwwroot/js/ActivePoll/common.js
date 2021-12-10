let questions;
let options;
let allAnswers;
let answersByDepartment;
let userSelection;
let answered;

let loadedSrc = [];

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
    let url = `/Poll/GetPollInfo?id=${pollId}`;
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
				loadedSrc[i] = src;
			});
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