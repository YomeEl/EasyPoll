let answers = [];
let selected = 0;

function constructCurrent() {
    clearCurrent();
    appendMedia();
    let div = document.createElement('div');
    div.innerText = questions[index];
    questionDiv.append(div);

    options[index].forEach((opt, i) => {

        let label = document.createElement('label');
        label.className = 'answer-text';
        label.innerText = opt;

        let ansDiv = document.createElement('div');
        ansDiv.className = 'answer-box-active';
        ansDiv.id = (i + 1);
        ansDiv.append(label);

        let src = mediaController.loadedOptionSrc[index][i];
        if (src) {
            let media = mediaController.createMediaDiv(src);
            media.className = 'img-wrapper-small';
            let hr = document.createElement('hr');
            ansDiv.append(hr, media);
        }

        ansDiv.onclick = function () {
            answer(this.id)
        };

        answersDiv.append(ansDiv);
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