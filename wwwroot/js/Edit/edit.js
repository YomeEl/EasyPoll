var cntr = 0;

init(3);

function init(count) {
    for (var i = 0; i < count; i++) {
        addOption();
    }
}

function addOption(val = '') {
    cntr++;

    var newAnswer = document.createElement("input");
    newAnswer.name = 'Answer' + cntr;
    newAnswer.className = "login-form-input";
    newAnswer.type = "text";
    newAnswer.placeholder = "Текст ответа " + cntr;
    newAnswer.value = val;

    var newA = document.createElement("a");
    newA.href = "";
    newA.innerText = "Загрузить изображение";

    var newHr = document.createElement("hr");

    var newDiv = document.createElement("div");
    newDiv.id = "AnswerDiv" + cntr;
    newDiv.appendChild(newAnswer);
    newDiv.appendChild(newA);
    newDiv.appendChild(newHr);

    var ansDiv = document.getElementById("answers");
    ansDiv.appendChild(newDiv);
}

function removeOption() {
    document.getElementById("answers").removeChild(document.getElementById("AnswerDiv" + cntr));
    cntr--;
}