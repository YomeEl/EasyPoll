function constructCurrent() {
    appendMedia();
    let div = document.createElement('div');
    div.innerText = questions[index];
    questionDiv.append(div);

    options[index].forEach((opt, i) => {
        let label = document.createElement('label');
        label.className = 'answer-text';
        let perc = percentage(index, i);
        label.innerText = opt + (perc != '0' ? `\xa0(${perc})` : '');

        let ansBarResult = document.createElement('div');
        ansBarResult.className = 'answer-bar-result';
        if (showDetails || userSelection[index] == i + 1) {
            ansBarResult.className += '-active';
        }
        ansBarResult.style = `width: ${perc}`;

        let ansBar = document.createElement('div');
        ansBar.className = 'answer-bar';
        ansBar.append(ansBarResult);

        let ansDiv = document.createElement('div');
        ansDiv.className = 'answer-box';
        ansDiv.id = (i + 1);
        ansDiv.append(label, ansBar);

        let src = mediaController.loadedOptionSrc[index][i];
        if (src) {
            let media = mediaController.createMediaDiv(src);
            media.className = 'img-wrapper-small';
            label.before(media);
        }

        answersDiv.append(ansDiv);

        if (index < questions.length - 1) {
            buttonNext.style = '';
        }
        if (index > 0) {
            buttonPrev.style = '';
        }
    });
}

function appendMedia() {
    let wrapper = mediaController.createMediaDiv(mediaController.loadedSrc[index]);

    questionDiv.append(wrapper);
}

function nextQuestion() {
    index++;
    reset();
}

function prevQuestion() {
    index--;
    reset();
}

function percentage(question, ans) {
    let sum = 0;
    allAnswers[question].forEach((ans) => {
        sum += ans;
    });

    if (sum === 0) return '0';

    let perc = (allAnswers[question][ans] / sum * 100).toFixed(1) + '%';
    return perc;
}