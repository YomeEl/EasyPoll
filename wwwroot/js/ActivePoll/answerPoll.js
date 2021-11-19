var answers = [];
var selected = 0;

function constructCurrent() {
    clearCurrent();
    questionDiv.innerHTML = questions[index].question;
    const options = questions[index].options.split('~!');
    for (let i = 0; i < options.length; i++) {
        let label1 = document.createElement('label');
        label1.className = 'answer-text';
        label1.innerText = (i + 1) + ".\xa0";
        let label2 = document.createElement('a');
        label2.className = 'answer-text';
        label2.innerText = options[i];

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
    answers.push(selected);
    let xhr = new XMLHttpRequest();
    let formData = new FormData();
    formData.append('Answers', answers);
    xhr.onload = function () {
        window.location.reload();
    }
    xhr.open('POST', '/');
    xhr.send(formData);
}