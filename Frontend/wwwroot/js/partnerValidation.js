document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("createPartnerForm");

    form.addEventListener("submit", function (event) {
        let isValid = true;

        // Validate FirstName
        const firstName = document.getElementById("FirstName");
        const firstNameError = document.getElementById("FirstNameError");
        if (!firstName.value.trim()) {
            firstNameError.textContent = "Partner first name is required.";
            isValid = false;
        } else if (firstName.value.length < 2 || firstName.value.length > 255) {
            firstNameError.textContent = "Partner's first name must be between 2 and 255 characters.";
            isValid = false;
        } else {
            firstNameError.textContent = "";
        }

        // Validate PartnerNumber
        const partnerNumber = document.getElementById("PartnerNumber");
        const partnerNumberError = document.getElementById("PartnerNumberError");
        if (!/^\d{20}$/.test(partnerNumber.value)) {
            partnerNumberError.textContent = "PartnerNumber must be exactly 20 digits.";
            isValid = false;
        } else {
            partnerNumberError.textContent = "";
        }

        // Prevent form submission if not valid
        if (!isValid) {
            event.preventDefault();
        }
    });
});