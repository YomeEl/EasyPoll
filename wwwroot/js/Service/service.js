let table = document.getElementById('table');
let lastRow = document.getElementById('lastRow');
let usernameInput = document.getElementById('username');
let departmentsDiv = document.getElementById('departments');

loadDepartments().then((departments) => loadUsersWithoutDept(departments));
loadUsers();

//Departments section

async function loadDepartments() {
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
        xhr.onload = function () {
            departmentsDiv.innerHTML = '';
            let depts = JSON.parse(xhr.response);
            resolve(depts);
            for (let dept of depts) {
                appendDepartment(dept);
            }
        }
        xhr.open('GET', '/Settings/GetDepartments', true);
        xhr.send();
    });
}

function appendDepartment(dept, isNew = false) {
    let label = document.createElement('label');
    label.innerText = dept;
    if (isNew) {
        label.style = 'font-weight: bold;';
    }
    let action = document.createElement('a');
    action.innerText = 'удалить';
    action.style = 'margin-left: 5px;';
    let div = document.createElement('div');
    if (isNew) {
        div.setAttribute('name', 'newDept');
    }
    div.append(label, action);

    let toggle;
    if (isNew) {
        toggle = () => { div.remove(); };
    } else {
        toggle = () => {
            if (action.innerText == 'удалить') {
                label.style = 'text-decoration: line-through red;';
                action.innerText = 'отмена';
            } else {
                label.style = '';
                action.innerText = 'удалить';
            }
        };
    }
    action.onclick = toggle;

    departmentsDiv.append(div);
}

function appendDepartmentFromInput() {
    let inp = document.getElementById('newDeptName');
    if (inp.value != '') {
        if (inp.value.length > 50) {
            alert('В названии должно быть не более 50 символов');
            inp.value = '';
            return;
        }
        appendDepartment(inp.value, true);
        inp.value = '';
    }
}

function getDeletedDepartments() {
    let res = [];
    for (let item of departmentsDiv.childNodes) {
        if (item.hasChildNodes() && item.lastElementChild.innerText != 'удалить') {
            res.push(item.firstElementChild.innerText);
        }
    }
    return res;
}

function getAddedDepartments() {
    let res = [];
    for (let item of document.getElementsByName('newDept')) {
        res.push(item.firstElementChild.innerText);
    }
    return res;
}

function updateDepartments() {
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        loadDepartments().then((departments) => loadUsersWithoutDept(departments));
    }
    xhr.open('POST', '/Settings/UpdateDepartments', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    let add = JSON.stringify(getAddedDepartments());
    let del = JSON.stringify(getDeletedDepartments());
    xhr.send('addRaw=' + add + '&deleteRaw=' + del);
}

//Users section

function loadUsersWithoutDept(departments) {
    clearUsersTable();
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        let users = JSON.parse(xhr.response);
        for (let username of users) {
            constructUsersRow(username, departments);
        }
    }
    xhr.open('GET', '/Settings/GetUsersWithoutDepartment', true);
    xhr.send();
}

function clearUsersTable() {
    let table = document.getElementById('usersTable').firstElementChild;
    let lastChild = table.lastElementChild;
    if (lastChild.id != 'header') {
        lastChild.remove();
        clearUsersTable();
    }
}

function constructUsersRow(username, departments) {
    let usersTable = document.getElementById('usersTable').firstElementChild;

    let left = document.createElement('td');
    let label = document.createElement('label');
    label.style = 'margin: 0';
    label.innerText = username;
    left.append(label);

    let right = document.createElement('td');
    let select = document.createElement('select');
    let zeroOpt = document.createElement('option');
    zeroOpt.value = 0;
    zeroOpt.innerText = 'Не назначено';
    select.append(zeroOpt);
    for (let i = 0; i < departments.length; i++) {
        let opt = document.createElement('option');
        opt.value = i + 1;
        opt.innerText = departments[i];
        select.append(opt);
    }
    right.append(select);

    let newRow = document.createElement('tr');
    newRow.append(left);
    newRow.append(right);

    usersTable.append(newRow);
}

function submitUsersDepts() {
    let table = document.getElementById('usersTable');
    let deltaUsers = [];
    let deltaDepartments = [];
    for (let row of table.rows) {
        if (row.id == 'header') continue;

        let left = row.firstElementChild;
        let right = row.lastElementChild;
        let username = left.firstElementChild.innerText;
        let departmentSelect = right.firstElementChild;
        let index = departmentSelect.selectedIndex;
        if (index > 0) {
            deltaUsers.push(username);
            deltaDepartments.push(departmentSelect[index].innerText);
        }
    }

    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        loadDepartments().then((departments) => loadUsersWithoutDept(departments));
    }
    xhr.open('POST', '/Settings/UpdateUserDepartments', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    let usersRaw = JSON.stringify(deltaUsers);
    let departmentsRaw = JSON.stringify(deltaDepartments);
    xhr.send('usersRaw=' + usersRaw + '&departmentsRaw=' + departmentsRaw);
}

//Roles section

function loadUsers() {
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        let users = JSON.parse(xhr.response);
        for (let [username, role] of Object.entries(users)) {
            appendToTable(username, role - 1);
        }
    }
    xhr.open('GET', '/Settings/GetUsers', true);
    xhr.send();
}

function appendToTable(val = '', selectedIndex = -1) {
    if (val == '') {
        val = usernameInput.value;
    }

    if (tableContainsUsername(val)) {
        usernameInput.value = 'Имя уже добавлено';
        usernameInput.focus();
        usernameInput.select();
        return;
    };

    let left = document.createElement('td');
    let label = document.createElement('label');
    label.style = 'margin: 0';
    label.innerText = val;
    left.append(label);
    if (selectedIndex == -1) {
        let info = document.createElement('label');
        info.innerText = '(проверка...)';
        info.style = 'margin-left: 5px; margin: 0;';
        left.append(info);
    }
    let right = document.createElement('td');
    let select = document.getElementById('roleSelect').cloneNode(true);
    if (selectedIndex != -1) {
        select.selectedIndex = selectedIndex;
    } else {
        select.selectedIndex = document.getElementById('roleSelect').selectedIndex;
    }
    select.id = '';
    right.append(select);

    let newRow = document.createElement('tr');
    let name = selectedIndex != -1 ? 'newUser' : 'oldUser';
    newRow.setAttribute('name', name);
    newRow.append(left);
    newRow.append(right);

    lastRow.before(newRow);

    if (selectedIndex == -1) {
        usernameInput.focus();
        usernameInput.select();
        checkRow(val, newRow);
    }
}

function tableContainsUsername(username) {
    let tbody = table.firstElementChild;
    let usernames = [];
    for (var row of tbody.childNodes) {
        if (!row.hasChildNodes()) continue;
        let name = row.getAttribute('name');
        if (name == 'oldUser' || name == 'newUser') {
            usernames.push(row.firstElementChild.firstElementChild.innerText);
        }
    }
    return usernames.findIndex((element, index, array) => element == username) != -1;
}

function checkRow(val, newRow) {
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        newRow.firstElementChild.lastElementChild.remove();
        let res = xhr.response;
        if (res != 'true') {
            newRow.style = 'transition: 1.0s ease-out; background-color: red;';
            (function () {
                return new Promise((resolve) => setTimeout(resolve, 1000));
            }()).then(() => newRow.remove());
        }
    }
    xhr.open('POST', '/Settings/CheckUsername', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.send('username=' + val);
}