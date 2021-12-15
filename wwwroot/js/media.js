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
				data.videoPreview.style = 'display: none';
			}
			else {
				data.videoPreview.src = src;
				data.videoPreview.style = '';
				data.imgPreview.style = 'display: none';
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

	function createMediaDiv(source) {
		let img = document.createElement('img');

		let video = document.createElement('video');
		video.setAttribute('controls', '');

		if (source != '') {
			let regexpStr = acceptedImageFormats.join('|').replaceAll('.', '');
			let regexp = new RegExp(`\\.(${regexpStr})`, 'i');
			if (source.match(regexp)) {
				img.src = source;
				video.style = 'display: none';
			} else {
				video.src = source;
				img.style = 'display: none';
			}
		}
		else {
			img.style = 'display: none';
			video.style = 'display: none';
		}

		let wrapper = document.createElement('div');
		wrapper.className = 'img-wrapper';
		wrapper.append(img, video);

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