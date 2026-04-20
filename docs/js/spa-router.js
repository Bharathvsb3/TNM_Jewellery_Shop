// SPA Router - Hash-based routing with dynamic content loading
class SPARouter {
    constructor() {
        this.routes = {
            'dashboard': 'pages/dashboard.html',
            'customers': 'pages/customers.html',
            'enquiries': 'pages/enquiries.html',
            'orders': 'pages/orders.html',
            'workers': 'pages/workers.html',
            'products': 'pages/products.html',
            'gold-tracking': 'pages/gold-tracking.html',
            'gold-dispatch': 'pages/gold-dispatch.html',
            'craft-work': 'pages/craft-work.html',
            'craftsman-approvals': 'pages/craftsman-approvals.html',
            'customer-outstanding-report': 'pages/customer-outstanding-report.html',
            'reports': 'pages/reports.html',
            'chat': 'pages/chat.html',
            'database-management': 'pages/database-management.html',
            'profile': 'pages/profile.html'
        };

        this.contentArea = document.getElementById('content-area');
        this.pageTitle = document.getElementById('page-title');

        this.init();
    }

    init() {
        // Listen for hash changes
        window.addEventListener('hashchange', () => this.handleRoute());

        // Handle navigation clicks
        document.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const hash = link.getAttribute('href');
                window.location.hash = hash;
                this.setActiveNav(hash);
            });
        });

        // Load initial route
        this.handleRoute();
    }

    async handleRoute() {
        let hash = window.location.hash.slice(1) || 'dashboard';
        
        // Remove any query parameters
        hash = hash.split('?')[0];

        const route = this.routes[hash];

        if (route) {
            await this.loadPage(route, hash);
            this.setActiveNav('#' + hash);
        } else {
            // Fallback to dashboard
            window.location.hash = 'dashboard';
        }
    }

    async loadPage(pageUrl, routeName) {
        try {
            // Show loading state
            this.contentArea.innerHTML = '<div class="flex items-center justify-center py-20"><span class="material-symbols-outlined text-4xl text-primary animate-spin">progress_activity</span><span class="ml-4 text-outline">Loading...</span></div>';

            const response = await fetch(pageUrl);
            
            if (!response.ok) {
                throw new Error('Page not found');
            }

            const html = await response.text();
            
            // Create a temporary container to parse the HTML
            const tempContainer = document.createElement('div');
            tempContainer.innerHTML = html;
            
            // Extract and remove script tags
            const scripts = tempContainer.querySelectorAll('script');
            const scriptContents = Array.from(scripts).map(script => {
                const content = script.textContent;
                script.remove();
                return content;
            });
            
            // Set the HTML content (without scripts)
            this.contentArea.innerHTML = tempContainer.innerHTML;

            // Update page title
            this.updatePageTitle(routeName);

            // Execute the scripts in order
            scriptContents.forEach(scriptContent => {
                try {
                    // Create and execute a new script element
                    const scriptElement = document.createElement('script');
                    scriptElement.textContent = scriptContent;
                    document.body.appendChild(scriptElement);
                    // Remove it immediately after execution
                    document.body.removeChild(scriptElement);
                } catch (error) {
                    console.error('Error executing page script:', error);
                }
            });

            // Trigger custom event for page-specific initialization
            this.executePageScripts(routeName);

        } catch (error) {
            console.error('Error loading page:', error);
            this.contentArea.innerHTML = `
                <div class="max-w-md mx-auto text-center py-20">
                    <span class="material-symbols-outlined text-6xl text-error mb-4">error</span>
                    <h2 class="text-2xl font-bold serif-font mb-2">Page Not Found</h2>
                    <p class="text-outline mb-6">The requested page could not be loaded.</p>
                    <button class="px-6 py-3 golden-gradient text-white text-sm uppercase tracking-widest font-bold" onclick="window.location.hash='dashboard'">Go to Dashboard</button>
                </div>
            `;
        }
    }

    setActiveNav(hash) {
        document.querySelectorAll('.nav-item').forEach(link => {
            link.classList.remove('active');
            if (link.getAttribute('href') === hash) {
                link.classList.add('active');
            }
        });
    }

    updatePageTitle(routeName) {
        const titles = {
            'dashboard': 'Dashboard',
            'customers': 'Customers',
            'enquiries': 'Enquiries',
            'orders': 'Orders',
            'workers': 'Craftsmen',
            'products': 'Products',
            'gold-tracking': 'Gold Tracking',
            'gold-dispatch': 'Material Dispatch',
            'craft-work': 'Craft Work',
            'craftsman-approvals': 'Craftsman Approvals',
            'customer-outstanding-report': 'Outstanding Report',
            'reports': 'Analytics',
            'database-management': 'Database',
            'chat': 'Chat',
            'profile': 'Profile'
        };

        this.pageTitle.textContent = titles[routeName] || 'Dashboard';
    }

    executePageScripts(routeName) {
        // Trigger custom event for page-specific initialization
        const event = new CustomEvent('pageLoaded', { detail: { route: routeName } });
        window.dispatchEvent(event);
    }
}

// Initialize router when DOM is ready
let router;
document.addEventListener('DOMContentLoaded', () => {
    router = new SPARouter();
});
