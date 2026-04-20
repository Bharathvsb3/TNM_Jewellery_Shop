// Complete Chat Manager - Admin & Customer
// Handles all Chat API integrations based on Flutter implementation

class ChatManager {
    constructor(isAdmin = true) {
        this.isAdmin = isAdmin;
        this.currentUser = null;
        this.currentChatUser = null;
        this.messages = [];
        this.users = [];
        this.pollingInterval = null;
        this.messagePollingInterval = null;
        this.isAtBottom = true;
        this.isLoading = false;
    }

    // Initialize chat system
    async initialize() {
        this.currentUser = this.getCurrentLoggedInUser();
        
        if (!this.currentUser) {
            console.error('No user logged in');
            return;
        }

        if (this.isAdmin) {
            await this.loadUsers();
            this.startUserPolling();
        } else {
            await this.loadMessages();
            this.startMessagePolling();
        }
    }

    getCurrentLoggedInUser() {
        const userStr = localStorage.getItem('user');
        if (userStr) {
            const user = JSON.parse(userStr);
            return {
                phone: user.mobile || user.phone,
                name: user.name || user.userName || 'User'
            };
        }
        return null;
    }

    // Admin: Load all users with unread counts
    async loadUsers() {
        try {
            this.showLoading(true);
            const response = await fetch(
                `${API_BASE_URL}/Chat/users?admnuser=${this.currentUser.phone}`,
                {
                    headers: this.getHeaders()
                }
            );

            const data = await response.json();
            
            if (data.success) {
                this.users = data.data || [];
                this.renderUsersList();
            }
        } catch (error) {
            console.error('Error loading users:', error);
        } finally {
            this.showLoading(false);
        }
    }

    // Load chat messages
    async loadMessages(toUser = null) {
        try {
            const targetUser = toUser || this.currentChatUser || (this.isAdmin ? null : this.getAdminPhone());
            
            if (!targetUser) return;

            const url = `${API_BASE_URL}/Chat/history?custno=${this.currentUser.phone}&toUser=${targetUser}`;
            const response = await fetch(url, {
                headers: this.getHeaders()
            });

            const data = await response.json();
            
            if (data.success) {
                this.messages = data.data || [];
                this.renderMessages();
                
                if (this.messages.length > 0) {
                    await this.markAsRead(targetUser);
                }
            }
        } catch (error) {
            console.error('Error loading messages:', error);
        }
    }

    // Send message
    async sendMessage(text) {
        if (!text || !text.trim()) return false;

        if (text.length > 5000) {
            this.showLimitWarning();
            return false;
        }

        try {
            const toUser = this.isAdmin ? this.currentChatUser : this.getAdminPhone();
            
            const response = await fetch(`${API_BASE_URL}/Chat/sendMessage`, {
                method: 'POST',
                headers: this.getHeaders(),
                body: JSON.stringify({
                    fromUser: this.currentUser.phone,
                    toUser: toUser,
                    message: text.trim()
                })
            });

            const data = await response.json();
            
            if (data.success) {
                // Add optimistically to UI
                this.addMessageOptimistically(text, true);
                
                // Reload messages after short delay
                setTimeout(() => this.loadMessages(), 500);
                
                return true;
            }
            
            return false;
        } catch (error) {
            console.error('Error sending message:', error);
            return false;
        }
    }

    // Mark messages as read
    async markAsRead(fromUser) {
        try {
            await fetch(
                `${API_BASE_URL}/Chat/markRead?custno=${this.currentUser.phone}&curuser=${fromUser}`,
                {
                    method: 'POST',
                    headers: this.getHeaders()
                }
            );
            
            // Refresh user list to update unread counts
            if (this.isAdmin) {
                await this.loadUsers();
            }
        } catch (error) {
            console.error('Error marking as read:', error);
        }
    }

    // Select user for chat (Admin only)
    selectUser(phone) {
        this.currentChatUser = phone;
        this.loadMessages(phone);
        
        // Stop message polling if active
        if (this.messagePollingInterval) {
            clearInterval(this.messagePollingInterval);
        }
        
        // Start polling for this conversation
        this.messagePollingInterval = setInterval(() => {
            this.loadMessages(phone);
        }, 5000);
    }

    // Start polling for user list (Admin)
    startUserPolling() {
        this.pollingInterval = setInterval(() => {
            if (!this.currentChatUser) {
                this.loadUsers();
            }
        }, 30000); // Every 30 seconds
    }

    // Start polling for messages (Customer)
    startMessagePolling() {
        this.messagePollingInterval = setInterval(() => {
            this.loadMessages();
        }, 5000); // Every 5 seconds
    }

    // Cleanup
    destroy() {
        if (this.pollingInterval) clearInterval(this.pollingInterval);
        if (this.messagePollingInterval) clearInterval(this.messagePollingInterval);
    }

    // Render user list (Admin)
    renderUsersList() {
        const container = document.getElementById('conversations-list');
        
        if (!container) return;

        if (this.users.length === 0) {
            container.innerHTML = `
                <div class="empty-users">
                    <p>No conversations yet</p>
                </div>
            `;
            return;
        }

        container.innerHTML = this.users.map(user => {
            const initials = this.getInitials(user.userName || user.userId);
            const isActive = this.currentChatUser === user.userId;
            const time = this.formatTime(user.lastMessageTime);
            
            return `
                <div class="conversation-item ${isActive ? 'active' : ''}" 
                     onclick="adminChat.selectUser('${user.userId}')">
                    <div class="user-avatar-small">
                        <span>${initials}</span>
                    </div>
                    <div class="conversation-details">
                        <p class="conversation-name">${this.escapeHtml(user.userName || user.userId)}</p>
                        <p class="conversation-last-msg">${this.escapeHtml(user.lastMessage || 'No messages yet')}</p>
                    </div>
                    <div class="conversation-meta">
                        ${time ? `<span class="conversation-time">${time}</span>` : ''}
                        ${user.unreadCount > 0 ? `<span class="unread-badge">${user.unreadCount}</span>` : ''}
                    </div>
                </div>
            `;
        }).join('');
    }

    // Render messages
    renderMessages() {
        const container = document.getElementById('messages-container');
        
        if (!container) return;

        if (this.messages.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 40px; color: #6b7280;">
                    <p>No messages yet. Start the conversation!</p>
                </div>
            `;
            return;
        }

        let lastDate = null;
        let html = '';

        this.messages.forEach(msg => {
            const msgDate = new Date(msg.addDate);
            const dateStr = msgDate.toLocaleDateString('en-IN');

            // Add date divider
            if (dateStr !== lastDate) {
                html += `
                    <div class="date-divider">
                        <span>${this.formatDateDivider(msgDate)}</span>
                    </div>
                `;
                lastDate = dateStr;
            }

            const isSent = msg.fromUser === this.currentUser.phone;
            const time = msgDate.toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit' });

            html += `
                <div class="message-bubble ${isSent ? 'sent' : 'received'}">
                    ${!isSent ? `
                        <div class="message-avatar">
                            <span class="material-symbols-outlined">person</span>
                        </div>
                    ` : ''}
                    <div class="message-content">
                        <p class="message-text">${this.escapeHtml(msg.messageText)}</p>
                        <span class="message-time">${time}</span>
                    </div>
                    ${isSent ? `
                        <div class="message-avatar">
                            <span class="material-symbols-outlined">support_agent</span>
                        </div>
                    ` : ''}
                </div>
            `;
        });

        container.innerHTML = html;
        this.scrollToBottom();
    }

    // Add message optimistically (before server confirms)
    addMessageOptimistically(text, isSent) {
        const container = document.getElementById('messages-container');
        if (!container) return;

        const time = new Date().toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit' });
        
        const messageHtml = `
            <div class="message-bubble ${isSent ? 'sent' : 'received'}">
                ${!isSent ? `
                    <div class="message-avatar">
                        <span class="material-symbols-outlined">person</span>
                    </div>
                ` : ''}
                <div class="message-content">
                    <p class="message-text">${this.escapeHtml(text)}</p>
                    <span class="message-time">${time}</span>
                </div>
                ${isSent ? `
                    <div class="message-avatar">
                        <span class="material-symbols-outlined">support_agent</span>
                    </div>
                ` : ''}
            </div>
        `;

        container.insertAdjacentHTML('beforeend', messageHtml);
        this.scrollToBottom();
    }

    // Utility functions
    getHeaders() {
        const token = localStorage.getItem('token');
        return {
            'Content-Type': 'application/json',
            ...(token && { 'Authorization': `Bearer ${token}` })
        };
    }

    getAdminPhone() {
        // Get admin phone from config or hardcode
        return localStorage.getItem('admin_phone') || '9876543210'; // Default admin phone
    }

    getInitials(name) {
        if (!name) return '?';
        return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
    }

    formatTime(dateStr) {
        if (!dateStr) return '';
        
        const date = new Date(dateStr);
        const now = new Date();
        const diff = now - date;
        
        const minutes = Math.floor(diff / 60000);
        const hours = Math.floor(diff / 3600000);
        const days = Math.floor(diff / 86400000);

        if (minutes < 1) return 'Just now';
        if (minutes < 60) return `${minutes}m`;
        if (hours < 24) return `${hours}h`;
        if (days < 7) return `${days}d`;
        
        return date.toLocaleDateString('en-IN', { month: 'short', day: 'numeric' });
    }

    formatDateDivider(date) {
        const today = new Date();
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);

        if (date.toDateString() === today.toDateString()) return 'Today';
        if (date.toDateString() === yesterday.toDateString()) return 'Yesterday';
        
        return date.toLocaleDateString('en-IN', { 
            month: 'long', 
            day: 'numeric', 
            year: 'numeric' 
        });
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    scrollToBottom() {
        setTimeout(() => {
            const container = document.getElementById('chat-messages-area');
            if (container) {
                container.scrollTop = container.scrollHeight;
            }
        }, 100);
    }

    showLoading(show) {
        const skeleton = document.querySelector('.loading-skeleton');
        if (skeleton) {
            skeleton.style.display = show ? 'block' : 'none';
        }
    }

    showLimitWarning() {
        console.warn('Message too long!');
        // Could add UI warning here
    }
}

// Global instances
let adminChat = null;
let customerChat = null;

// Initialize based on page type
window.addEventListener('pageLoaded', (e) => {
    if (e.detail.route === 'chat' || e.detail.route === 'admin-chat') {
        if (!adminChat) {
            adminChat = new ChatManager(true);
            adminChat.initialize();
        }
    } else if (e.detail.route === 'customer-chat') {
        if (!customerChat) {
            customerChat = new ChatManager(false);
            customerChat.initialize();
        }
    }
});

// Cleanup on page unload
window.addEventListener('beforePageUnload', (e) => {
    if (adminChat) {
        adminChat.destroy();
    }
    if (customerChat) {
        customerChat.destroy();
    }
});

// Helper function for send button
async function sendChatMessage() {
    const input = document.getElementById('message-input');
    if (!input) return;

    const text = input.value.trim();
    if (!text) return;

    const manager = adminChat || customerChat;
    if (!manager) return;

    const success = await manager.sendMessage(text);
    
    if (success) {
        input.value = '';
        input.style.height = 'auto';
    }
}

// Helper for refresh
function refreshChatUsers() {
    if (adminChat) {
        adminChat.loadUsers();
    }
}

function refreshMessages() {
    const manager = adminChat || customerChat;
    if (manager) {
        manager.loadMessages();
    }
}
