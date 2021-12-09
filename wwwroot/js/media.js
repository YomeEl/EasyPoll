const mediaController = (function () {
	const data = {
		fileInput: null,
		imgPreview: null,
		videoPreview: null,
		deletedMedia: [],
		deleteMediaFor: -1
    }

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

	return {
		data,
		init,
		previewMedia,
		deleteMedia,
		sumbitDeletion
    }
}) ();