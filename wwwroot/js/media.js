const mediaController = (function () {
	const data = {
		fileInput: null,
		imgPreview: null,
		videoPreview: null
    }

	function init(fileInput, imgPreview, videoPreview) {
		data.fileInput = fileInput;
		data.imgPreview = imgPreview;
		data.videoPreview = videoPreview;
    }

	function previewMedia() {
		let oFReader = new FileReader();
		oFReader.readAsDataURL(data.fileInput.files[0]);

		oFReader.onload = function (oFREvent) {
			let src = oFREvent.target.result;
			if (data.fileInput.files[0].type.match(/image/)) {
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

	function deleteMedia() {
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