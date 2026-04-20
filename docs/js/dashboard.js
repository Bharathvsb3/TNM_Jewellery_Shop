// Dashboard initialization and common functionality
document.addEventListener('DOMContentLoaded', async () => {
    // Check authentication
    if (!isLoggedIn()) {
        window.location.href = 'index.html';
        return;
    }

    // Check if user has admin access
    const user = getCurrentUser();
    if (!user) {
        logout();
        return;
    }

    // Role-based access control - only allow admin users
    if (user.role && user.role.toUpperCase() !== 'ADMIN') {
        // Redirect non-admin users to their appropriate pages
        if (user.userType === 'customer' || user.role.toUpperCase() === 'CUSTOMER') {
            window.location.href = 'customer.html';
            return;
        } else if (user.userType === 'craftsman' || user.role.toUpperCase() === 'CRAFTSMAN') {
            window.location.href = 'craftsman-index.html';
            return;
        } else {
            // Unknown role, logout for safety
            alert('Unauthorized access. Please login with an admin account.');
            logout();
            return;
        }
    }

    // Initialize dashboard
    await initDashboard();

    // Load gold rate
    await loadGoldRate();

    // Refresh gold rate every 5 minutes
    setInterval(loadGoldRate, 5 * 60 * 1000);
});

async function initDashboard() {
    const user = getCurrentUser();
    
    if (!user) {
        logout();
        return;
    }

    // Update user info
    const userNameEl = document.getElementById('user-name');
    const userRoleEl = document.getElementById('user-role');
    const userInitialsEl = document.getElementById('user-initials');
    
    if (userNameEl) userNameEl.textContent = user.name || 'Admin';
    if (userRoleEl) userRoleEl.textContent = (user.role || 'ADMIN').toUpperCase();
    
    // Set initials
    if (userInitialsEl) {
        const name = user.name || 'Admin';
        const initials = name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
        userInitialsEl.textContent = initials;
    }

    // Load shop settings
    await loadShopSettings();
}

async function loadGoldRate() {
    try {
        // Get today's rates and absolute latest
        const [todayRates, latestRate] = await Promise.all([
            apiRequest(`${API_BASE_URL}/GoldRates/today`),
            apiRequest(`${API_BASE_URL}/GoldRates/latest`)
        ]);
        
        let displayRate = null;
        
        // Smart logic: prioritize today's Evening > today's Morning > previous latest
        if (todayRates && todayRates.length > 0) {
            // Check if today has Evening rate
            const eveningRate = todayRates.find(r => r.timeSlot === 'Evening');
            if (eveningRate) {
                displayRate = eveningRate;
            } else {
                // Use today's Morning rate
                const morningRate = todayRates.find(r => r.timeSlot === 'Morning');
                if (morningRate) {
                    displayRate = morningRate;
                }
            }
        }
        
        // Fallback to absolute latest if no today's rate
        if (!displayRate && latestRate) {
            displayRate = latestRate;
        }
        
        if (displayRate && displayRate.gold916Rate) {
            const rateEl = document.getElementById('gold-916-rate');
            if (rateEl) {
                rateEl.textContent = `₹${displayRate.gold916Rate.toLocaleString()}`;
            }
        }
    } catch (error) {
        console.error('Error loading gold rate:', error);
        const rateEl = document.getElementById('gold-916-rate');
        if (rateEl) rateEl.textContent = '--';
    }
}

async function loadShopSettings() {
    try {
        // Try to load from API first
        const data = await apiRequest(API.SHOP_SETTINGS);
        
        if (data && data.shopName) {
            document.getElementById('sidebar-shop-name').textContent = data.shopName;
            localStorage.setItem(STORAGE_KEYS.SHOP_NAME, data.shopName);
            
            if (data.shopLogo) {
                localStorage.setItem(STORAGE_KEYS.SHOP_LOGO, data.shopLogo);
            }
        }
    } catch (error) {
        console.error('Error loading shop settings from API, using SHOP_CONFIG:', error);
        
        // Fallback to SHOP_CONFIG from config.js
        if (typeof SHOP_CONFIG !== 'undefined') {
            document.getElementById('sidebar-shop-name').textContent = SHOP_CONFIG.name;
            localStorage.setItem(STORAGE_KEYS.SHOP_NAME, SHOP_CONFIG.name);
        } else {
            // Final fallback to localStorage or default
            const storedName = localStorage.getItem(STORAGE_KEYS.SHOP_NAME) || 'JMW';
            document.getElementById('sidebar-shop-name').textContent = storedName;
        }
    }
}

// Helper function to format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

// Helper function to format currency
function formatCurrency(amount) {
    return `₹${parseFloat(amount).toLocaleString('en-IN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    })}`;
}

// Helper function to format weight
function formatWeight(weight) {
    return `${parseFloat(weight).toFixed(3)} g`;
}

// Show toast notification
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        background: ${type === 'success' ? '#10B981' : '#EF4444'};
        color: white;
        border-radius: 0.5rem;
        box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        z-index: 9999;
        animation: slideIn 0.3s ease-out;
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    .loading-spinner {
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 3rem;
        font-size: 1.25rem;
        color: var(--gray-600);
    }
    .loading-spinner i {
        margin-right: 0.75rem;
        font-size: 1.5rem;
    }
    .error-container {
        text-align: center;
        padding: 3rem;
    }
    .error-container h2 {
        margin-bottom: 1rem;
        color: var(--gray-900);
    }
    .error-container p {
        margin-bottom: 2rem;
        color: var(--gray-600);
    }
`;
document.head.appendChild(style);
