document.querySelector('form').addEventListener('submit', function (event) {
    const accountNumberInput = document.getElementById('accountNumber');
    const accountNumber = accountNumberInput.value.trim();

    if (!/^\d{10}$/.test(accountNumber)) {
        alert("Account number must be 10digits.");
        accountNumberInput.focus();
        event.preventDefault(); // Prevent form submission
    }
});

document.querySelector('#edit-account-form').addEventListener('submit', function (event) {
    const holderNameInput = document.getElementById('holderName');
    const phoneNumberInput = document.getElementById('phoneNumber');
    let valid = true;

    // Validate Holder Name (Max length 35 characters)
    if (holderNameInput.value.length > 35) {
        holderNameInput.setCustomValidity('Holder name must be 35 characters or less.');
        valid = false;
    } else {
        holderNameInput.setCustomValidity('');
    }

    // Validate Phone Number (Between 1 and 13 digits)
    const phonePattern = /^\d{1, 13}$/;
    if (!phonePattern.test(phoneNumberInput.value)) {
        phoneNumberInput.setCustomValidity('Phone number must be between 1 and 13 digits.');
        valid = false;
    } else {
        phoneNumberInput.setCustomValidity('');
    }

    // Prevent form submission if validation fails
    if (!valid) {
        event.preventDefault();
    }
});

