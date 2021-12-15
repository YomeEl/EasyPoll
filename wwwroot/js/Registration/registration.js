const registrationController = (function () {
    const BAD_USERNAME_MSG = 'Неправильный формат имени пользователя';
    const BAD_EMAIL_MSG = 'Неправильный формат e-mail';
    const BAD_PASSWORD_MSG = 'Неправильный формат пароля';
    const PASS_MISMATCH_MSG = 'Пароли не совпадают';
    const USERNAME_EXISTS = 'Имя пользователя занято';
    const EMAIL_EXISTS = 'E-mail уже зарегистрирован';

    let errorList = document.getElementById('errorList');
    let err = false;

    async function validateAndSend() {
        document.getElementById('errorList').innerHTML = '';
        if (await validate()) document.getElementById('registrationForm').submit()
    }

    function appendWarning(text) {
        let label = document.createElement('label');
        label.className = 'login-warning';
        label.innerText = text;
        errorList.append(label);
        username.style = 'border-color: red';
        err = true;
    };

    async function validate() {
        err = false;

        let usernameRegExp = /^\w{4,20}$/;
        let emailRegExp = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+$/;
        let passwordRegExp = /^\w{6,20}$/;

        let username = document.getElementById('username');
        let email = document.getElementById('email');
        let password = document.getElementById('password');
        let rePassword = document.getElementById('rePassword');

        let checkingTask = checkUsernameAndEmail(username.value, email.value);

        username.style = '';
        email.style = '';
        password.style = '';
        rePassword.style = '';

        if (!usernameRegExp.test(username.value)) appendWarning(BAD_USERNAME_MSG);
        if (!emailRegExp.test(email.value)) appendWarning(BAD_EMAIL_MSG);
        if (!passwordRegExp.test(password.value)) appendWarning(BAD_PASSWORD_MSG);
        if (password.value != rePassword.value) appendWarning(PASS_MISMATCH_MSG);

        let usernameAndEmailStatus = (await (await checkingTask).text()).split(',');

        if (usernameAndEmailStatus[0] === 'True') appendWarning(USERNAME_EXISTS);
        if (usernameAndEmailStatus[1] === 'True') appendWarning(EMAIL_EXISTS);

        return !err;
    }

    async function checkUsernameAndEmail(username, email) {
        return fetch('/Authentification/CheckRegistration', {
            method: 'POST',
            body: new URLSearchParams({
                username: username,
                email: email
            })
        });
    }

    return {
        validateAndSend
    }
})();