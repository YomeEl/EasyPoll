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

function init(questionsArray, answersArray, userSelectionArray, answeredBool) {
    questions = questionsArray;
    allAnswers = answersArray;
    userSelection = userSelectionArray;
    answered = answeredBool;
    reset();
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