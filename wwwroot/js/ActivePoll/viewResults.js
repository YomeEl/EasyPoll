function constructCurrent() {
    questionDiv.innerHTML = questions[index];

    for (let i = 0; i < options[index].length; i++) {
        let label1 = document.createElement('label');
        label1.className = 'answer-text';
        label1.innerText = (i + 1) + ".\xa0";
        let label2 = document.createElement('label');
        label2.className = 'answer-text';
        label2.innerText = options[index][i] + '\xa0(' + percentage(index, i) +')';

        let ansBarResult = document.createElement('div');
        ansBarResult.className = 'answer-bar-result';
        let styleStr = 'width: ' + percentage(index, i);
        if (userSelection[index] == i + 1) {
            styleStr += '; background-color: #FF0000;'
        }
        ansBarResult.style = styleStr;
        

        let ansBar = document.createElement('div');
        ansBar.className = 'answer-bar';
        ansBar.appendChild(ansBarResult);

        let ansDiv = document.createElement('div');
        ansDiv.className = 'answer-box';
        ansDiv.id = (i + 1);
        ansDiv.appendChild(label1);
        ansDiv.appendChild(label2);
        ansDiv.appendChild(ansBar);

        answersDiv.appendChild(ansDiv);

        if (index < questions.length - 1) {
            buttonNext.style = '';
        }
        if (index > 0) {
            buttonPrev.style = '';
        }
    }
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
    for (let i = 0; i < allAnswers[question].length; i++) {
        sum += allAnswers[question][i];
    }
    let perc = (allAnswers[question][ans] / sum * 100).toFixed(1) + '%';
    return perc;
}