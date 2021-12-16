loadActivePollData();

function loadActivePollData() {
	fetch('/PollApi/GetPollInfo')
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
			editLogic.editData.questions.forEach((q) => {
				q.options.forEach((opt, i) => {
					q.options[i] = {
						text: opt,
						media: null
                    }
				})
			});
			editLogic.newPoll = false;
			data['sources'].forEach((src, i) => {
				mediaController.loadedSrc[i] = src;
			});
			data['optionSources'].forEach((question, q) => question.forEach((optSrc, o) => {
				if (o == 0) {
					mediaController.loadedOptionSrc[q] = [];
                }
				mediaController.loadedOptionSrc[q][o] = optSrc;
			}));
			pollConstructor.construct();
		});
}