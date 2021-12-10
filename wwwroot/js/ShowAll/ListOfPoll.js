function DisplayPolls(pollsArray, lastIsActive) {
    let polllist = document.getElementById('polllist');
    for (let i = 0; i < pollsArray.length; i++) {
        let linePoll = document.createElement('div');
        if (lastIsActive && i == pollsArray.length - 1) {
            linePoll.className = 'poll-summary-current';
        } else {
            linePoll.className = 'poll-summary';
        }
        linePoll.onclick = () => {
            location.assign(`/Poll/Details?id=${pollsArray[i].id}`);
        }

        let label1 = document.createElement('label');
        label1.className = 'poll-desc';
        label1.innerText = pollsArray[i].pollName;

        let label2 = document.createElement('label');
        label2.className = 'poll-date';
        let innerText = pollsArray[i].createdAt;
        innerText = innerText.replace('T', ' ');
        let dateTimeArr = innerText.split(' ');
        let dateArr = dateTimeArr[0].split('-');
        let dateText = dateArr[2] + '.' + dateArr[1] + '.' + dateArr[0] + ' ';

        label2.innerText = dateText + dateTimeArr[1];

        linePoll.append(label1);
        linePoll.append(label2);

        if (lastIsActive && i == pollsArray.length - 1 && pollsArray.length > 1) {
            polllist.prepend(document.createElement('hr'));
        }
        polllist.prepend(linePoll);
    }
}