setTimeout(function () { 
    $('.alert').alert('close');
}, 4000)

function closeAlert() {
    var alertBox = document.getElementById('error-message');
    alertBox.style.display = 'none';
}