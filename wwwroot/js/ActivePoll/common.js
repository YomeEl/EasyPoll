var questions;
var options;
var allAnswers;
var userSelection;
var answered;

let loadedSrc = [];

var index = 0;

var questionDiv = document.getElementById('question');
var answersDiv = document.getElementById('answers');
var buttonNext = document.getElementById('btn-next');
var buttonPrev = document.getElementById('btn-prev');
var buttonFinish = document.getElementById('btn-finish');

init();

function init() {
	fetch('/Poll/GetActivePollInfo')
		.then((response) => response.text())
        .then((dataRaw) => {
            let data = JSON.parse(dataRaw);
            questions = data['questions'];
            options = data['options'];
            allAnswers = data['answers'];
            userSelection = data['userselection'];
            answered = data['answered'];
			data['sources'].forEach((src) => {
				let s = src.match(/\d+\./)[0];
				s = s.slice(0, s.length - 1);
				loadedSrc[s] = src;
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