const mediaController = (function () {
	const data = {
		fileInput: null,
		imgPreview: null,
		videoPreview: null,
		deletedMedia: [],
		deletedOptionsMedia: [[]]
    }

	const acceptedImageFormats = ['.jpg', '.jpeg', '.png', '.gif'];
	const acceptedVideoFormats = ['.mp4'];

	let loadedSrc = [];
	let loadedOptionSrc = [[]];

	function init(fileInput, imgPreview, videoPreview) {
		data.fileInput = fileInput;
		data.imgPreview = imgPreview;
		data.videoPreview = videoPreview;
    }

	function deleteQuestionMedia(question) {
		data.deletedMedia.push(question);
		loadedSrc[question] = '';
    }

	function createMediaDiv(source = '') {
		let img = document.createElement('img');

		let video = document.createElement('video');
		video.setAttribute('controls', '');

		img.style = 'display: none';
		img.setAttribute('hide', 'true');
		video.style = 'display: none';
		video.setAttribute('hide', 'true');

		if (source != '') {
			let regexpStr = acceptedImageFormats.join('|').replaceAll('.', '');
			let regexp = new RegExp(`\\.(${regexpStr})`, 'i');
			if (source.match(regexp)) {
				img.src = source;
				img.style = '';
				img.setAttribute('hide', 'false');
			} else {
				video.src = source;
				video.style = '';
				video.setAttribute('hide', 'false');
			}
		}

		let wrapper = document.createElement('div');
		wrapper.className = 'img-wrapper';
		wrapper.append(img, video);

		img.onclick = () => {
			let w1 = wrapper.cloneNode(true);
			w1.className = 'img-wrapper-full';
			w1.id = 'full';
			w1.onclick = () => w1.remove();
			let loading = document.createElement('label');
			loading.className = 'question loading-label';
			loading.id = 'loadingLabel';
			loading.innerText = 'Загрузка изображения...';
			wrapper.append(loading);
			w1.firstElementChild.onload = () => {
				document.body.append(w1);
				document.getElementById('loadingLabel').remove();
            }
        }

		return wrapper;
    }

	function createMediaInput(id) {
		let input = document.createElement('input');
		input.type = 'file';
		input.style = 'display: none';
		input.id = id;
		let extensions = acceptedImageFormats.concat(acceptedVideoFormats).join(',');
		input.setAttribute('accept', extensions);

		return input;
    }

	return {
		data,
		loadedSrc,
		loadedOptionSrc,
		createMediaDiv,
		createMediaInput,
		init,
		deleteQuestionMedia
    }
}) ();