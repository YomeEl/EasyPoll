loadActivePollData();

function loadActivePollData() {
	fetch('/Poll/GetActivePollInfo')
		.then((response) => response.text())
		.then((dataRaw) => {
			let data = JSON.parse(dataRaw);
			editLogic.editData.oldName = data['pollName'];
			editLogic.editData.newName = data['pollName'];
			editLogic.editData.startAt = new Date(data['startAt']);
			editLogic.editData.finishAt = new Date(data['finishAt']);
			editLogic.editData.sendStart = data['sendStart'];
			editLogic.editData.sendFinish = data['sendFinish'];
			data['questions'].forEach((question, i) => {
				editLogic.editData.questions.push({
					name: question,
					options: data['options'][i]
				});
			});
			editLogic.newPoll = false;
			constructNewPoll();
		});
}