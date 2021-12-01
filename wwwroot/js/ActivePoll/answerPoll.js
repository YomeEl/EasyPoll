var answers = [];
var selected = 0;

function constructCurrent() {
    clearCurrent();
    questionDiv.innerHTML = questions[index].Question;
    for (let i = 0; i < options[index].length; i++) {
        let label1 = document.createElement('label');
        label1.className = 'answer-text';
        label1.innerText = (i + 1) + ".\xa0";
        let label2 = document.createElement('a');
        label2.className = 'answer-text';
        label2.innerText = options[index][i];

        let ansDiv = document.createElement('div');
        ansDiv.className = 'answer-box-active';
        ansDiv.id = (i + 1);
        ansDiv.appendChild(label1);
        ansDiv.appendChild(label2);

        ansDiv.onclick = function () {
            answer(this.id)
        };

        answersDiv.appendChild(ansDiv);
    }
}

function answer(ans) {
    if (selected != 0) {
        document.getElementById(selected).className = 'answer-box-active';
    }
    document.getElementById(ans).className = 'answer-box-active-selected';
    selected = ans;
    if (index < questions.length - 1) {
        buttonNext.style = '';
    }
    else {
        buttonFinish.style = '';
    }
}

function nextQuestion() {
    index++;
    answers.push(selected);
    selected = 0;
    reset();
}

function sendPoll() {
    buttonFinish.onclick = '';
    buttonFinish.innerHTML = '';
    let newLabel = document.createElement('label');
    newLabel.className = 'btn-text';
    newLabel.innerText = 'Сохранение...';
    buttonFinish.appendChild(newLabel);

    answers.push(selected);
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        window.location.reload();
    }

    xhr.open('POST', '/');
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.send('answers=' + JSON.stringify(answers));
}