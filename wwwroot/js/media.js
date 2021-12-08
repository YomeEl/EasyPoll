﻿const mediaController = (function () {
	const data = {
		fileInput: null,
		imgPreview: null,
		videoPreview: null,
		deletedMedia: []
    }

	function init(fileInput, imgPreview, videoPreview) {
		data.fileInput = fileInput;
		data.imgPreview = imgPreview;
		data.videoPreview = videoPreview;
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
		data.deletedMedia.push(question);
		data.fileInput.value = '';
		data.imgPreview.src = '';
		data.imgPreview.style = 'display: none';
		data.videoPreview.src = '';
		data.videoPreview.style = 'display: none';
	}

	return {
		data,
		init,
		previewMedia,
		deleteMedia
    }
}) ();