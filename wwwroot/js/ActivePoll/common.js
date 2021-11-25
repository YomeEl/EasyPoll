var questions;
var allAnswers;
var userSelection;
var answered;

var index = 0;

var questionDiv = document.getElementById('question');
var answersDiv = document.getElementById('answers');
var buttonNext = document.getElementById('btn-next');
var buttonPrev = document.getElementById('btn-prev');
var buttonFinish = document.getElementById('btn-finish');

init();

function init() {
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        let res = JSON.parse(xhr.response);
        questions = res['questions'];
        allAnswers = res['answers'];
        userSelection = res['userselection'];
        answered = res['answered'];
        reset();
    }
    xhr.open('GET', '/Poll/GetActivePollInfo', true);
    xhr.send();
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