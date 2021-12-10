document.getElementById('deptSelect').onchange = switchDept;

getInfo.then(fillDepts);

function switchDept(event) {
    allAnswers = answersByDepartment[event.target.value];
    reset();
}

function fillDepts() {
    let select = document.getElementById('deptSelect');
    let counts = document.getElementById('counts');

    let maxCount = 0;
    let mostActiveDept = 'Нет данных';

    for (let dept in answersByDepartment) {
        if (dept != 'all') {
            let opt = document.createElement('option');
            opt.innerText = dept;
            select.append(opt);
        }

        let label = document.createElement('label');
        label.className = 'question';
        let text = dept == 'all' ? 'Всего участников' : dept;
        let count = sum(index, answersByDepartment[dept]);
        if (dept != 'all' && count > maxCount) {
            mostActiveDept = dept;
            maxCount = count;
        }
        label.innerHTML = `${text}: <b>${count}</b>`;
        let br = () => document.createElement('br');
        if (dept == 'all') {
            counts.prepend(label, br(), br());
        } else {
            counts.append(label, br());
        }
    }

    document.getElementById('mostActiveDept').innerText = mostActiveDept;
}

function sum(question, answers) {
    let sum = 0;
    answers[question].forEach((ans) => sum += ans);
    return sum;
}