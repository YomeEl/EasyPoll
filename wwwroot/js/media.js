const mediaController = (function () {
	const data = {
		fileInput: null,
		imgPreview: null,
		videoPreview: null,
		deletedMedia: [],
		deleteMediaFor: -1
    }

	const acceptedImageFormats = ['.jpg', '.jpeg', '.png', '.gif'];
	const acceptedVideoFormats = ['.mp4'];

	let loadedSrc = []

	function init(fileInput, imgPreview, videoPreview) {
		data.fileInput = fileInput;
		data.imgPreview = imgPreview;
		data.videoPreview = videoPreview;

		data.deleteMediaFor = -1;
    }

	function previewMedia(file=null) {
		let oFReader = new FileReader();
		if (!file) {
			file = data.fileInput.files[0];
        }
		oFReader.readAsDataURL(file);

		oFReader.onload = function (oFREvent) {
			let src = oFREvent.target.result;
			if (file.type.match(/image/)) {
				data.imgPreview.src = oFREvent.target.result;
				data.imgPreview.style = '';
				data.imgPreview.setAttribute('hide', 'false');
				data.videoPreview.style = 'display: none';
				data.videoPreview.setAttribute('hide', 'true');
			}
			else {
				data.videoPreview.src = src;
				data.videoPreview.style = '';
				data.videoPreview.setAttribute('hide', 'false');
				data.imgPreview.style = 'display: none';
				data.imgPreview.setAttribute('hide', 'true');
            }
		};
	};

	function deleteMedia(question) {
		data.deleteMediaFor = question;
		
		data.fileInput.value = '';
		data.imgPreview.src = '';
		data.imgPreview.style = 'display: none';
		data.videoPreview.src = '';
		data.videoPreview.style = 'display: none';
	}

	function sumbitDeletion() {
		if (data.deleteMediaFor === -1) return;
		data.deletedMedia.push(data.deleteMediaFor);
		loadedSrc[data.deleteMediaFor] = '';
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

		wrapper.onclick = () => {
			if (video.getAttribute('hide') == 'true') {
				let w1 = wrapper.cloneNode(true);
				w1.className = 'img-wrapper-full';
				w1.id = 'full';
				w1.onclick = () => w1.remove();
				document.body.append(w1);
			}
        }

		return wrapper;
    }

	function createMediaInput() {
		let input = document.createElement('input');
		input.type = 'file';
		input.style = 'display: none';
		input.id = 'file';
		let extensions = acceptedImageFormats.concat(acceptedVideoFormats).join(',');
		input.setAttribute('accept', extensions);

		return input;
    }

	return {
		data,
		loadedSrc,
		createMediaDiv,
		createMediaInput,
		init,
		previewMedia,
		deleteMedia,
		sumbitDeletion
    }
}) ();