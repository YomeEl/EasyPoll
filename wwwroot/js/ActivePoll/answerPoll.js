var answers = [];
var selected = 0;

function constructCurrent() {
    clearCurrent();
    appendMedia();
    let div = document.createElement('div');
    div.innerText = questions[index];
    questionDiv.append(div);
    
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

function appendMedia() {
    let img = document.createElement('img');

    let video = document.createElement('video');
    video.setAttribute('controls', '');

    if (loadedSrc[index]) {
        if (loadedSrc[index].match(/.(jpg|jpeg|png|gif)$/i)) {
            img.src = loadedSrc[index];
            video.style = 'display: none';
        } else {
            video.src = loadedSrc[index];
            img.style = 'display: none';
        }
    }
    else {
        img.style = 'display: none';
        video.style = 'display: none';
    }

    let wrapper = document.createElement('div');
    wrapper.className = 'img-wrapper';
    wrapper.append(img, video);

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
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        window.location.reload();
    }

    xhr.open('POST', '/');
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.send('answers=' + JSON.stringify(answers));
}