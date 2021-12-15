const departmentsController = (function () {
    let departmentsDiv = document.getElementById('departments');

    function loadDepartments() {
        fetch('/Settings/GetDepartments').then((res) => res.text().then((text) => {
            departmentsDiv.innerHTML = '';
            let depts = JSON.parse(text);
            depts.forEach((dept) => appendDepartment(dept.Name));
        }));
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

        let div = document.createElement('div');
        div.setAttribute('name', isNew ? 'newDept' : 'oldDept');
        div.append(label, action);

        departmentsDiv.append(div);
    }

    function isDepartmentUnique(dept) {
        let flag = true;
        document.getElementsByName('newDept').forEach((newDept) => {
            flag = flag && newDept.firstElementChild.innerText != dept;
        });
        document.getElementsByName('oldDept').forEach((oldDept) => {
            flag = flag && oldDept.firstElementChild.innerText != dept;
        });

        return flag;
    }

    function validateDepartmentName(name) {
        if (name == '') {
            alert('Название не указано');
            return false;
        }

        if (name.length > 50) {
            alert('В названии должно быть не более 50 символов');
            return false;
        }

        if (!isDepartmentUnique(name)) {
            alert('Название не уникально');
            return false;
        }

        return true;
    }

    function appendDepartmentFromInput() {
        let inp = document.getElementById('newDeptName');

        if (!validateDepartmentName(inp.value)) {
            inp.value = '';
            return;
        }

        appendDepartment(inp.value, true);
        inp.value = '';
    }

    function getDeletedDepartments() {
        let res = [];
        departmentsDiv.childNodes.forEach((item) => {
            if (item.hasChildNodes() && item.lastElementChild.innerText != 'удалить') {
                res.push(item.firstElementChild.innerText);
            }
        });

        return res;
    }

    function getAddedDepartments() {
        return Array.from(document.getElementsByName('newDept'))
            .map((item) => item.firstElementChild.innerText);
    }

    function updateDepartments() {
        fetch('/Settings/UpdateDepartments', {
            method: 'POST',
            body: new URLSearchParams({
                addRaw: JSON.stringify(getAddedDepartments()),
                deleteRaw: JSON.stringify(getDeletedDepartments())
            })
        }).then((res) => res.text().then(loadDepartments));
    }

    return {
        loadDepartments,
        appendDepartmentFromInput,
        updateDepartments
    }
})();

const rolesController = (function () {
    let lastRow = document.getElementById('lastRow');
    let usernameInput = document.getElementById('username');

    let rolesDelta = [];

    function loadUsers() {
        fetch('/Settings/GetUsers').then((res) => res.text().then((text) => {
            let users = JSON.parse(text);
            for (let [username, role] of Object.entries(users)) {
                appendUserToTable(username, role - 1);
            }
        }));
    }

    function appendUserToTable(val = '', selectedIndex = -1) {
        if (val == '') {
            val = usernameInput.value;
        }

        if (tableContainsUsername(val)) {
            usernameInput.value = 'Имя уже добавлено';
            usernameInput.focus();
            usernameInput.select();
            return;
        };

        let label = document.createElement('label');
        label.style = 'margin: 0';
        label.innerText = val;

        let left = document.createElement('td');
        left.append(label);
        if (selectedIndex == -1) {
            let info = document.createElement('label');
            info.innerText = '(проверка...)';
            info.style = 'margin-left: 5px; margin: 0;';
            left.append(info);
        }

        let select = createSelect(val, selectedIndex);

        let right = document.createElement('td');
        right.append(select);

        let newRow = document.createElement('tr');
        let name = selectedIndex != -1 ? 'oldUser' : 'newUser';
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

    function onSelectChange(event) {
        let name = event.target.id;
        let role = event.target.selectedIndex + 1;
        let index = rolesDelta.findIndex((val, _i, _o) => val.name == name);
        if (index != -1) {
            rolesDelta[index].role = role;
        } else {
            rolesDelta.push({
                Username: name,
                RoleId: role
            });
        }
    };

    function createSelect(id, selectedIndex) {
        let select = document.getElementById('roleSelect').cloneNode(true);
        if (selectedIndex != -1) {
            select.selectedIndex = selectedIndex;
        } else {
            select.selectedIndex = document.getElementById('roleSelect').selectedIndex;
        }
        select.id = id;
        select.onchange = onSelectChange;

        return select;
    }

    function tableContainsUsername(username) {
        return Array.from(document.getElementsByName('oldUser'))
            .concat(Array.from(document.getElementsByName('newUser')))
            .findIndex((val) => val.firstElementChild.firstElementChild.innerText == username) != -1;
    }

    function checkRow(val, newRow) {
        fetch('/Settings/CheckUsername', {
            method: 'POST',
            body: new URLSearchParams({ username: val })
        }).then((res) => res.text().then((text) => {
            if (text != 'true') {
                newRow.style = 'transition: 1.0s ease-out; background-color: red;';
                setTimeout(() => newRow.remove(), 1000);
            } else {
                rolesDelta.push({
                    Username: val,
                    RoleId: newRow.lastElementChild.firstElementChild.selectedIndex + 1
                });
                newRow.firstElementChild.lastElementChild.remove();
            }
        }));
    }

    function submitRoles() {
        fetch('/Settings/UpdateRoles', {
            method: 'POST',
            body: new URLSearchParams({
                itemsRaw: JSON.stringify(rolesDelta)
            })
        }).then(() => location.reload())
    }

    return {
        loadUsers,
        submitRoles,
        appendUserToTable
    }
})();

departmentsController.loadDepartments();
rolesController.loadUsers();