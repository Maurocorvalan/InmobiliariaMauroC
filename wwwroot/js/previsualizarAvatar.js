document.getElementById('avatarFileInput').addEventListener('change', function() {
    var reader = new FileReader();

    reader.onload = function(e) {
        document.getElementById('avatarPreview').src = e.target.result;
    };

    reader.readAsDataURL(this.files[0]);
});
