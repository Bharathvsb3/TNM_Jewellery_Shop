// Login functionality
document.addEventListener('DOMContentLoaded', async () => {
    // Check if already logged in
    if (isLoggedIn()) {
        window.location.href = 'dashboard.html';
        return;
    }

    // Load shop settings
    await loadShopSettings();

    const form = document.getElementById('login-form');
    const emailPhone = document.getElementById('email-phone');
    const password = document.getElementById('password');
    const errorMessage = document.getElementById('error-message');
    const submitBtn = form.querySelector('.btn-submit');
    const btnText = submitBtn.querySelector('.btn-text');
    const btnIcon = submitBtn.querySelector('.btn-icon');
    const btnLoader = submitBtn.querySelector('.btn-loader');

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        errorMessage.textContent = '';

        const emailOrMobile = emailPhone.value.trim();
        const passwordValue = password.value;

        if (!emailOrMobile || !passwordValue) {
            errorMessage.textContent = 'Please fill in all fields';
            return;
        }

        // Show loading state
        submitBtn.disabled = true;
        btnText.style.display = 'none';
        btnIcon.style.display = 'none';
        btnLoader.style.display = 'inline-block';

        try {
            const response = await fetch(`${API_BASE_URL}/Auth/unified-login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    emailOrMobile,
                    password: passwordValue
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Login failed');
            }

            // Save token and user data
            localStorage.setItem(STORAGE_KEYS.TOKEN, data.token);
            localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(data));

            // Register device for chat notifications
            try {
                const fcmToken = localStorage.getItem('fcmToken') || '';
                const userMobile = data.mobile || data.phone || emailOrMobile;
                
                if (fcmToken && userMobile) {
                    await fetch(`${API_BASE_URL}/Chat/registerDevice`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            userId: userMobile,
                            fcmToken: fcmToken,
                            deviceType: 'WEB'
                        })
                    });
                }
            } catch (err) {
                console.log('Device registration skipped:', err);
            }

            // Redirect based on backend response or user type
            let redirectUrl = data.redirectUrl || 'dashboard.html'; // Use backend URL or default
            
            // Fallback logic if redirectUrl not provided
            if (!data.redirectUrl) {
                if (data.userType === 'customer') {
                    redirectUrl = 'customer.html';
                } else if (data.userType === 'craftsman') {
                    redirectUrl = 'craftsman-index.html';
                } else if (data.userType === 'admin') {
                    redirectUrl = 'dashboard.html';
                }
            }

            window.location.href = redirectUrl;

        } catch (error) {
            errorMessage.textContent = error.message || 'Invalid credentials. Please try again.';
            
            // Reset button state
            submitBtn.disabled = false;
            btnText.style.display = 'inline';
            btnIcon.style.display = 'inline';
            btnLoader.style.display = 'none';
        }
    });
});

async function loadShopSettings() {
    try {
        // Use SHOP_CONFIG from config.js
        const shopName = SHOP_CONFIG.name;
        const shopTagline = SHOP_CONFIG.tagline;
        const shopDescription = SHOP_CONFIG.description;
        
        const heroTitle = document.getElementById('shop-name-hero');
        const mobileTitle = document.getElementById('shop-name-mobile');
        const tagline = document.getElementById('shop-tagline');
        const description = document.getElementById('shop-description');
        const copyright = document.getElementById('footer-copyright');
        
        if (heroTitle) heroTitle.textContent = shopName;
        if (mobileTitle) mobileTitle.textContent = shopName.split(' ').map(w => w[0]).join('').substring(0, 3);
        if (tagline) tagline.textContent = shopTagline;
        if (description) description.textContent = shopDescription;
        if (copyright) copyright.textContent = `© ${new Date().getFullYear()} ${shopName}`;
        
        // Store in localStorage for other pages
        localStorage.setItem(STORAGE_KEYS.SHOP_NAME, shopName);
    } catch (error) {
        console.error('Could not load shop settings:', error);
    }
}
