const BAD_USERNAME_MSG = 'Неправильный формат имени пользователя';
const BAD_EMAIL_MSG = 'Неправильный формат e-mail';
const BAD_PASSWORD_MSG = 'Неправильный формат пароля';
const PASS_MISMATCH_MSG = 'Пароли не совпадают';
const USERNAME_EXISTS = 'Имя пользователя занято';
const EMAIL_EXISTS = 'E-mail уже зарегистрирован';

let checked = false;

function validateAndSend() {
    document.getElementById('errorList').innerHTML = '';
    validate().then((res) => {
        if (res) document.getElementById('registrationForm').submit();
    });
    
}

async function validate() {
    let err = false;

    let usernameRegExp = /^\w{4,20}$/;
    let emailRegExp = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+$/;
    let passwordRegExp = /^\w{6,20}$/;

    let username = document.getElementById('username');
    let email = document.getElementById('email');
    let password = document.getElementById('password');
    let rePassword = document.getElementById('rePassword');

    var usernameAndEmailTask = checkUsernameAndEmail(username.value, email.value);

    username.style = '';
    email.style = '';
    password.style = '';
    rePassword.style = '';

    let errorlist = document.getElementById('errorList');

    if (!usernameRegExp.test(username.value)) {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = BAD_USERNAME_MSG;
        errorList.append(label);
        username.style = 'border-color: red';
        err = true;
    }
    if (!emailRegExp.test(email.value)) {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = BAD_EMAIL_MSG;
        errorList.append(label);
        email.style = 'border-color: red';
        err = true;
    }
    if (!passwordRegExp.test(password.value)) {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = BAD_PASSWORD_MSG;
        errorList.append(label);
        password.style = 'border-color: red';
        err = true;
    }
    if (password.value != rePassword.value) {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = PASS_MISMATCH_MSG;
        errorList.append(label);
        rePassword.style = 'border-color: red';
        rePassword.value = '';
        err = true;
    }

    var usernameAndEmailStatus = await usernameAndEmailTask;

    if (usernameAndEmailStatus[0] === 'true') {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = USERNAME_EXISTS;
        errorList.append(label);
        rePassword.style = 'border-color: red';
        rePassword.value = '';
        err = true;
    }

    if (usernameAndEmailStatus[1] === 'true') {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = EMAIL_EXISTS;
        errorList.append(label);
        rePassword.style = 'border-color: red';
        rePassword.value = '';
        err = true;
    }

    return !err;
}

async function checkUsernameAndEmail(username, email) {
    return p = new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
        xhr.onload = () => resolve(xhr.response.split(','));
        xhr.open('POST', '/Authentification/CheckRegistration', true);
        xhr.setRequestHeader("Username", username);
        xhr.setRequestHeader("Email", email);
        xhr.send();
    });
}