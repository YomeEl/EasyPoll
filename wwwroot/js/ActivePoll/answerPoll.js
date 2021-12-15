let answers = [];
let selected = 0;

function constructCurrent() {
    clearCurrent();
    appendMedia();
    let div = document.createElement('div');
    div.innerText = questions[index];
    questionDiv.append(div);

    options[index].forEach((opt, i) => {
        let label1 = document.createElement('label');
        label1.className = 'answer-text';
        label1.innerText = (i + 1) + ".\xa0";
        let label2 = document.createElement('a');
        label2.className = 'answer-text';
        label2.innerText = opt;

        let ansDiv = document.createElement('div');
        ansDiv.className = 'answer-box-active';
        ansDiv.id = (i + 1);
        ansDiv.appendChild(label1);
        ansDiv.appendChild(label2);

        ansDiv.onclick = function () {
            answer(this.id)
        };

        answersDiv.appendChild(ansDiv);
    })
}

function appendMedia() {
    let wrapper = mediaController.createMediaDiv(mediaController.loadedSrc[index]);
    questionDiv.append(wrapper);
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

    fetch('/', {
        method: 'POST',
        body: new URLSearchParams({
            answers: JSON.stringify(answers)
        })
    }).then((r) => location.reload());
}