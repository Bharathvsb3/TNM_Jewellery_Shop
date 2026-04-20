// API Configuration
const API_BASE_URL = 'http://tnm-gopi.runasp.net/api';

// Shop Branding Configuration
const SHOP_CONFIG = {
    name: 'T.N. Mani Gopi Jewellers',
    tagline: 'Legacy of Excellence',
    description: 'Every piece in our atelier is a testament to four decades of uncompromising artistry and the pursuit of absolute brilliance.',
    logo: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgdmlld0JveD0iMCAwIDEwMCAxMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxwYXRoIGQ9Ik01MCA1TDY1IDM1TDk1IDQwTDcyLjUgNjJMNzggOTJMNTAgNzVMMjIgOTJMMjcuNSA2Mkw1IDQwTDM1IDM1TDUwIDVaIiBmaWxsPSIjRDRBRjM3IiBzdHJva2U9IiNCODk0MUYiIHN0cm9rZS13aWR0aD0iMiIvPgo8L3N2Zz4=',
    address: {
        line1: '123 Main Street, Gandhi Nagar',
        line2: 'Coimbatore, Tamil Nadu - 641012',
        phone: '+91 98765 43210',
        email: 'info@tnmanigopijewellers.com',
        gstin: '33AAAAA0000A1Z5'
    },
    colors: {
        primary: '#D4AF37',      // Gold
        primaryLight: '#F4E5C2',
        primaryDark: '#B8941F',
        secondary: '#1A4D7B',    // Blue
        secondaryLight: '#4A7BA7',
        secondaryDark: '#0F3556',
        accent: '#50C878',       // Emerald
        error: '#E0115F'         // Ruby
    }
};

// Metal Type Color Themes
const METAL_THEMES = {
    'Gold': {
        primary: '#D4AF37',
        light: '#F4E5C2',
        dark: '#B8941F',
        gradient: 'linear-gradient(135deg, #D4AF37 0%, #F4E5C2 100%)',
        border: '#D4AF37',
        bg: '#FEF9E7',
        text: '#B8941F',
        shadow: 'rgba(212, 175, 55, 0.3)'
    },
    'Silver': {
        primary: '#C0C0C0',
        light: '#E8E8E8',
        dark: '#808080',
        gradient: 'linear-gradient(135deg, #C0C0C0 0%, #E8E8E8 100%)',
        border: '#C0C0C0',
        bg: '#F5F5F5',
        text: '#606060',
        shadow: 'rgba(192, 192, 192, 0.3)'
    },
    'Platinum': {
        primary: '#E5E4E2',
        light: '#F5F5F5',
        dark: '#A8A8A8',
        gradient: 'linear-gradient(135deg, #E5E4E2 0%, #F5F5F5 100%)',
        border: '#E5E4E2',
        bg: '#FAFAFA',
        text: '#707070',
        shadow: 'rgba(229, 228, 226, 0.3)'
    },
    'Other': {
        primary: '#775a19',
        light: '#c5a059',
        dark: '#4e3700',
        gradient: 'linear-gradient(135deg, #775a19 0%, #c5a059 100%)',
        border: '#775a19',
        bg: '#fcf9f8',
        text: '#775a19',
        shadow: 'rgba(119, 90, 25, 0.3)'
    }
};

// System Configuration
const CONFIG = {
    AUTO_APPROVE_CUSTOMER: false,  // Set to true for automatic customer approval, false for manual admin approval
    DEFAULT_WASTAGE: 10,            // Default wastage percentage (if customer default is 0)
    DEFAULT_GST: 3                  // Default GST percentage (if customer default is 0)
};

// Jewelry Dropdown Options
const JEWELRY_TYPES = [
    { value: '', label: 'Select Type' },
    { value: 'Ring', label: 'Ring' },
    { value: 'Necklace', label: 'Necklace' },
    { value: 'Earrings', label: 'Earrings' },
    { value: 'Bracelet', label: 'Bracelet' },
    { value: 'Bangles', label: 'Bangles' },
    { value: 'Chain', label: 'Chain' },
    { value: 'Pendant', label: 'Pendant' },
    { value: 'Anklet', label: 'Anklet' },
    { value: 'Nose Pin', label: 'Nose Pin' },
    { value: 'Mangalsutra', label: 'Mangalsutra' },
    { value: 'Other', label: 'Other' }
];

const JEWELRY_CATEGORIES = [
    { value: '', label: 'Select Category' },
    { value: 'Chains', label: 'Chains' },
    { value: 'Rings', label: 'Rings' },
    { value: 'Bangles', label: 'Bangles' },
    { value: 'Earrings', label: 'Earrings' },
    { value: 'Necklace Set', label: 'Necklace Set' },
    { value: 'Pendant', label: 'Pendant' },
    { value: 'Bracelet', label: 'Bracelet' },
    { value: 'Anklet', label: 'Anklet' },
    { value: 'Mangalsutra', label: 'Mangalsutra' },
    { value: 'Other', label: 'Other' }
];

// Jewelry Type to Category Mapping
const JEWELRY_TYPE_CATEGORY_MAP = {
    'Ring': 'Rings',
    'Necklace': 'Necklace Set',
    'Earrings': 'Earrings',
    'Bracelet': 'Bracelet',
    'Bangles': 'Bangles',
    'Chain': 'Chains',
    'Pendant': 'Pendant',
    'Anklet': 'Anklet',
    'Nose Pin': 'Other',
    'Mangalsutra': 'Mangalsutra',
    'Other': 'Other'
};

const METAL_TYPES = [
    { value: '', label: 'Select Metal' },
    { value: 'Gold', label: '💛 Gold' },
    { value: 'Silver', label: '🤍 Silver' },
    { value: 'Platinum', label: '⚪ Platinum' },
    { value: 'Other', label: 'Other' }
];

const PURITY_OPTIONS = [
    { value: '', label: 'Select Purity' },
    { value: '999', label: '999 (24K Gold)' },
    { value: '916', label: '916 (22K Gold)' },
    { value: '925', label: '925 (Sterling Silver)' }
];

// Purity options for enquiry form (different format)
const PURITY_OPTIONS_ENQUIRY = [
    { value: '', label: 'Select Purity' },
    { value: '24K', label: '24K (999)' },
    { value: '22K', label: '22K (916)' },
    { value: 'Sterling', label: 'Sterling Silver (925)' }
];

const MAKING_TYPES = [
    { value: '', label: 'Select Type' },
    { value: 'KRDM', label: 'KRDM (Karigari)' },
    { value: 'Normal', label: 'Normal' },
    { value: 'Casting', label: 'Casting' },
    { value: 'Handmade', label: 'Handmade' },
    { value: 'Machine Made', label: 'Machine Made' }
];

// Helper function to populate dropdown
function populateDropdown(elementId, options, selectedValue = '') {
    const select = document.getElementById(elementId);
    if (!select) return;
    
    select.innerHTML = options.map(opt => 
        `<option value="${opt.value}" ${opt.value === selectedValue ? 'selected' : ''}>${opt.label}</option>`
    ).join('');
}

// API Endpoints
const API = {
    AUTH: {
        LOGIN: `${API_BASE_URL}/Auth/login`,
        REGISTER: `${API_BASE_URL}/Auth/register`,
        CHANGE_PASSWORD: `${API_BASE_URL}/Auth/change-password`,
        PROFILE: `${API_BASE_URL}/Auth/profile`
    },
    CUSTOMERS: `${API_BASE_URL}/Customers`,
    ENQUIRIES: `${API_BASE_URL}/Enquiries`,
    ORDERS: `${API_BASE_URL}/Orders`,
    WORKERS: `${API_BASE_URL}/Workers`,
    PRODUCTS: `${API_BASE_URL}/Products`,
    GOLD_RATES: `${API_BASE_URL}/GoldRates`,
    CRAFT: `${API_BASE_URL}/CraftWorkflow`,
    MESSAGES: `${API_BASE_URL}/Messages`,
    REPORTS: `${API_BASE_URL}/Reports`,
    SHOP_SETTINGS: `${API_BASE_URL}/ShopSettings`
};

// Local Storage Keys
const STORAGE_KEYS = {
    TOKEN: 'jmw_token',
    USER: 'jmw_user',
    SHOP_NAME: 'jmw_shop_name',
    SHOP_LOGO: 'jmw_shop_logo'
};

// Get auth token
function getAuthToken() {
    return localStorage.getItem(STORAGE_KEYS.TOKEN);
}

// Get current user
function getCurrentUser() {
    const userStr = localStorage.getItem(STORAGE_KEYS.USER);
    return userStr ? JSON.parse(userStr) : null;
}

// Check if user is logged in
function isLoggedIn() {
    return !!getAuthToken();
}

// Logout
function logout() {
    localStorage.removeItem(STORAGE_KEYS.TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER);
    window.location.href = 'index.html';
}

// API Request Helper
async function apiRequest(url, options = {}) {
    const token = getAuthToken();
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    try {
        const response = await fetch(url, {
            ...options,
            headers
        });

        if (response.status === 401) {
            logout();
            return null;
        }

        // Check if response is JSON
        const contentType = response.headers.get('content-type');
        const isJson = contentType && contentType.includes('application/json');
        
        let data;
        if (isJson) {
            data = await response.json();
        } else {
            // If not JSON, get text response
            const text = await response.text();
            data = { message: text || 'An error occurred' };
        }

        if (!response.ok) {
            throw new Error(data.message || `HTTP ${response.status}: ${response.statusText}`);
        }

        return data;
    } catch (error) {
        // Only log unexpected errors, not validation errors
        const isValidationError = error.message && (
            error.message.includes('already exists') ||
            error.message.includes('Invalid credentials') ||
            error.message.includes('do not match')
        );
        
        if (!isValidationError) {
            console.error('API Error:', error);
        }
        throw error;
    }
}

// Apply Metal Type Color Theme
function applyMetalTheme(metalType, elementId) {
    const theme = METAL_THEMES[metalType] || METAL_THEMES['Other'];
    const element = document.getElementById(elementId);
    
    if (!element) return;
    
    // Apply theme colors to the element
    element.style.borderColor = theme.border;
    element.style.background = theme.bg;
    element.style.boxShadow = `0 0 0 2px ${theme.primary}`;
    
    // Add class for additional styling
    element.classList.remove('theme-gold', 'theme-silver', 'theme-platinum', 'theme-other');
    element.classList.add(`theme-${metalType.toLowerCase()}`);
}

// Apply Metal Theme to Multiple Elements
function applyMetalThemeToForm(metalType, containerSelector) {
    const theme = METAL_THEMES[metalType] || METAL_THEMES['Other'];
    const container = document.querySelector(containerSelector);
    
    if (!container) return;
    
    // Update all form fields in container
    const fields = container.querySelectorAll('input, select, textarea');
    fields.forEach(field => {
        field.style.borderColor = theme.border;
        field.style.transition = 'all 0.3s ease';
        
        // Add focus effect
        field.addEventListener('focus', function() {
            this.style.boxShadow = `0 0 0 3px ${theme.shadow}`;
            this.style.borderColor = theme.primary;
        });
        
        field.addEventListener('blur', function() {
            this.style.boxShadow = 'none';
            this.style.borderColor = theme.border;
        });
    });
    
    // Update labels
    const labels = container.querySelectorAll('label');
    labels.forEach(label => {
        label.style.color = theme.text;
    });
    
    // Update submit button if exists
    const submitBtn = container.querySelector('button[type="submit"]');
    if (submitBtn) {
        submitBtn.style.background = theme.gradient;
        submitBtn.style.boxShadow = `0 4px 12px ${theme.shadow}`;
    }
}

// Global function to handle metal type selection changes
function handleMetalTypeChange(selectElement, formContainerSelector) {
    selectElement.addEventListener('change', function() {
        const metalType = this.value;
        if (metalType && METAL_THEMES[metalType]) {
            applyMetalThemeToForm(metalType, formContainerSelector);
            
            // Add visual feedback
            const theme = METAL_THEMES[metalType];
            this.style.borderColor = theme.primary;
            this.style.background = theme.bg;
            this.style.color = theme.text;
            this.style.fontWeight = 'bold';
            
            // Animate the change
            this.classList.add('metal-theme-active');
        }
    });
}

// ============================================
// DECIMAL CONFIGURATION & FORMATTING HELPERS
// ============================================

// Get decimal places setting from localStorage (default: 2)
function getDecimalPlaces() {
    const stored = localStorage.getItem('jmw_decimal_places');
    return stored ? parseInt(stored, 10) : 2;
}

// Set decimal places setting
function setDecimalPlaces(places) {
    const validPlaces = Math.max(0, Math.min(4, parseInt(places, 10) || 2)); // Limit 0-4
    localStorage.setItem('jmw_decimal_places', validPlaces);
    return validPlaces;
}

// Format currency with configured decimal places
function formatCurrency(amount, includeSymbol = true) {
    const decimals = getDecimalPlaces();
    const formatted = parseFloat(amount || 0).toLocaleString('en-IN', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    });
    return includeSymbol ? `₹${formatted}` : formatted;
}

// Format decimal number (weight, percentage, etc.)
function formatDecimal(value, forceDecimals = null) {
    const decimals = forceDecimals !== null ? forceDecimals : getDecimalPlaces();
    return parseFloat(value || 0).toFixed(decimals);
}

// Format weight with 'g' suffix
function formatWeight(grams) {
    return `${formatDecimal(grams)}g`;
}

// Format percentage
function formatPercentage(percentage) {
    return `${formatDecimal(percentage)}%`;
}

// Shorthand aliases for convenience
window.fCurrency = formatCurrency;
window.fDecimal = formatDecimal;
window.fWeight = formatWeight;
window.fPercent = formatPercentage;
