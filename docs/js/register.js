// Register functionality
document.addEventListener('DOMContentLoaded', () => {
    // Check if already logged in
    if (isLoggedIn()) {
        window.location.href = 'dashboard.html';
        return;
    }

    const form = document.getElementById('register-form');
    const name = document.getElementById('name');
    const mobile = document.getElementById('mobile');
    const email = document.getElementById('email');
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirm-password');
    const errorMessage = document.getElementById('error-message');
    const submitBtn = form.querySelector('.btn-submit');
    const btnText = submitBtn.querySelector('.btn-text');
    const btnIcon = submitBtn.querySelector('.btn-icon');
    const btnLoader = submitBtn.querySelector('.btn-loader');

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        errorMessage.textContent = '';

        const nameValue = name.value.trim();
        const mobileValue = mobile.value.trim();
        const emailValue = email.value.trim();
        const addressValue = document.getElementById('address') ? document.getElementById('address').value.trim() : '';
        const passwordValue = password.value;
        const confirmPasswordValue = confirmPassword.value;

        // Validation
        if (!nameValue || !mobileValue || !passwordValue) {
            errorMessage.textContent = 'Please fill in all required fields';
            return;
        }

        if (mobileValue.length !== 10 || !/^\d+$/.test(mobileValue)) {
            errorMessage.textContent = 'Mobile number must be 10 digits';
            return;
        }

        if (passwordValue.length < 6) {
            errorMessage.textContent = 'Password must be at least 6 characters';
            return;
        }

        if (passwordValue !== confirmPasswordValue) {
            errorMessage.textContent = 'Passwords do not match';
            return;
        }

        // Show loading state
        submitBtn.disabled = true;
        btnText.style.display = 'none';
        btnIcon.style.display = 'none';
        btnLoader.style.display = 'inline-block';

        try {
            const response = await fetch(API.AUTH.REGISTER, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    name: nameValue,
                    mobile: mobileValue,
                    email: emailValue || null,
                    address: addressValue || null,
                    password: passwordValue,
                    role: 'CUSTOMER',
                    autoApprove: true
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Registration failed');
            }

            // Save token and user data
            localStorage.setItem(STORAGE_KEYS.TOKEN, data.token);
            localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(data));

            // Redirect to customer portal
            window.location.href = 'customer.html';

        } catch (error) {
            errorMessage.textContent = error.message || 'Registration failed. Please try again.';
            
            // Reset button state
            submitBtn.disabled = false;
            btnText.style.display = 'inline';
            btnIcon.style.display = 'inline';
            btnLoader.style.display = 'none';
        }
    });
});
