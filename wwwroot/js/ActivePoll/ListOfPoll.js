var polls;
var pollname = document.getElementById('pollname');

function init(pollsArray) {
     polls = pollsArray;
}

function DisplayPolls() {
    for (let i = 0; i < polls.length; i++) {
        let linePoll = document.createElement('div');   //добавить if для неактивных
        linePoll.className = 'poll-summary-current';
        let label1 = document.createElement('label');
        label1.className = 'poll-desc';
        label1.innerText = polls.PollName;
        let label2 = document.createElement('label');
        label2.className = 'poll-date';
        label2.innerText = polls.CreatedAt;
        linePoll.appendChild(pollname);
    }
}