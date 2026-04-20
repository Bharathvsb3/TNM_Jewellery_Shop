// WebSocket Chat Manager - Real-time messaging
// Based on ChatWebSocketMiddleware.cs pattern

class ChatManager {
    constructor(isAdmin = true) {
        this.isAdmin = isAdmin;
        this.currentUser = null;
        this.currentChatUser = null;
        this.messages = [];
        this.users = [];
        this.ws = null;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;
        this.reconnectDelay = 3000;
        this.isConnected = false;
        this.messageQueue = [];
    }

    // Initialize chat system with WebSocket
    async initialize() {
        this.currentUser = this.getCurrentLoggedInUser();
        
        if (!this.currentUser) {
            console.error('No user logged in');
            return;
        }

        // Connect to WebSocket
        await this.connectWebSocket();
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

    // Get WebSocket URL
    getWebSocketUrl() {
        // Convert HTTP/HTTPS API URL to WS/WSS
        const apiUrl = API_BASE_URL.replace('http://', '').replace('https://', '').replace('/api', '');
        const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
        
        const toUser = this.isAdmin ? '' : this.getAdminPhone();
        
        return `${protocol}//${apiUrl}/ws?user=${encodeURIComponent(this.currentUser.phone)}&Touser=${encodeURIComponent(toUser)}`;
    }

    // Connect to WebSocket server
    async connectWebSocket() {
        if (this.ws && (this.ws.readyState === WebSocket.OPEN || this.ws.readyState === WebSocket.CONNECTING)) {
            return;
        }

        try {
            const wsUrl = this.getWebSocketUrl();
            console.log('🔌 Connecting to WebSocket:', wsUrl);
            
            this.ws = new WebSocket(wsUrl);
            
            this.ws.onopen = () => this.handleOpen();
            this.ws.onmessage = (event) => this.handleMessage(event);
            this.ws.onerror = (error) => this.handleError(error);
            this.ws.onclose = () => this.handleClose();
            
        } catch (error) {
            console.error('❌ WebSocket connection error:', error);
            this.scheduleReconnect();
        }
    }

    // Handle WebSocket open
    handleOpen() {
        console.log('✅ WebSocket connected');
        this.isConnected = true;
        this.reconnectAttempts = 0;
        
        // Send any queued messages
        this.flushMessageQueue();
        
        // Request unread count
        this.send({
            type: 'getUnread'
        });
    }

    // Handle incoming WebSocket messages
    handleMessage(event) {
        try {
            const data = JSON.parse(event.data);
            
            console.log('📨 WebSocket message:', data.type, data);
            
            switch (data.type) {
                case 'history':
                    this.handleHistory(data.data);
                    break;
                    
                case 'message':
                    this.handleIncomingMessage(data);
                    break;
                    
                case 'sent':
                    this.handleSentConfirmation(data);
                    break;
                    
                case 'unreadCount':
                    this.handleUnreadCount(data.count);
                    break;
                    
                case 'readAck':
                    console.log('✓ Read acknowledgment received');
                    break;
                    
                case 'error':
                    console.error('❌ Server error:', data.message);
                    break;
                    
                default:
                    console.log('Unknown message type:', data.type);
            }
        } catch (error) {
            console.error('Error parsing WebSocket message:', error);
        }
    }

    // Handle WebSocket error
    handleError(error) {
        console.error('❌ WebSocket error:', error);
    }

    // Handle WebSocket close
    handleClose() {
        console.log('🔌 WebSocket disconnected');
        this.isConnected = false;
        
        // Try to reconnect
        this.scheduleReconnect();
    }

    // Schedule reconnection
    scheduleReconnect() {
        if (this.reconnectAttempts >= this.maxReconnectAttempts) {
            console.error('❌ Max reconnection attempts reached');
            return;
        }
        
        this.reconnectAttempts++;
        const delay = this.reconnectDelay * this.reconnectAttempts;
        
        console.log(`🔄 Reconnecting in ${delay}ms (attempt ${this.reconnectAttempts}/${this.maxReconnectAttempts})`);
        
        setTimeout(() => {
            this.connectWebSocket();
        }, delay);
    }

    // Send message via WebSocket
    send(message) {
        if (this.ws && this.ws.readyState === WebSocket.OPEN) {
            this.ws.send(JSON.stringify(message));
        } else {
            console.warn('⚠️ WebSocket not connected, queuing message');
            this.messageQueue.push(message);
        }
    }

    // Flush queued messages
    flushMessageQueue() {
        while (this.messageQueue.length > 0) {
            const message = this.messageQueue.shift();
            this.send(message);
        }
    }

    // Handle chat history
    handleHistory(history) {
        this.messages = history || [];
        this.renderMessages();
        
        // Mark as read if messages exist
        if (this.messages.length > 0 && !this.isAdmin) {
            const toUser = this.getAdminPhone();
            this.send({
                type: 'markRead',
                to: toUser
            });
        }
    }

    // Handle incoming message
    handleIncomingMessage(data) {
        // Add message to list if not already present
        const exists = this.messages.find(m => 
            m.fromUser === data.from && 
            m.toUser === data.to && 
            m.messageText === data.text
        );
        
        if (!exists) {
            this.messages.push({
                fromUser: data.from,
                toUser: data.to,
                messageText: data.text,
                addDate: data.sentUtc || new Date().toISOString(),
                isRead: false
            });
            
            this.renderMessages();
            
            // Mark as read if this is the active conversation
            if (this.isAdmin && data.from === this.currentChatUser) {
                this.send({
                    type: 'markRead',
                    to: data.from
                });
            } else if (!this.isAdmin) {
                this.send({
                    type: 'markRead',
                    to: data.from
                });
            }
        }
    }

    // Handle sent confirmation
    handleSentConfirmation(data) {
        // Message was successfully sent
        console.log('✓ Message sent:', data.text);
        
        // Add to messages if not already present
        const exists = this.messages.find(m => 
            m.fromUser === data.from && 
            m.messageText === data.text
        );
        
        if (!exists) {
            this.messages.push({
                fromUser: data.from,
                toUser: data.to,
                messageText: data.text,
                addDate: data.sentUtc || new Date().toISOString(),
                isRead: true
            });
            
            this.renderMessages();
        }
    }

    // Handle unread count
    handleUnreadCount(count) {
        console.log('📬 Unread count:', count);
        
        // Update UI badge
        const badge = document.getElementById('chat-unread-badge') || document.getElementById('chatBadge');
        if (badge) {
            if (count > 0) {
                badge.textContent = count;
                badge.style.display = 'block';
            } else {
                badge.style.display = 'none';
            }
        }
    }

    get connectionState() {
        if (!this.ws) return 'disconnected';
        
        switch (this.ws.readyState) {
            case WebSocket.CONNECTING: return 'connecting';
            case WebSocket.OPEN: return 'connected';
            case WebSocket.CLOSING: return 'closing';
            case WebSocket.CLOSED: return 'disconnected';
            default: return 'unknown';
        }
    }

    // Send message
    async sendMessage(text, toUser = null) {
        if (!text || !text.trim()) return false;

        if (text.length > 5000) {
            alert('Message too long. Maximum 5000 characters.');
            return false;
        }

        // Determine recipient
        const recipient = toUser || (this.isAdmin ? this.currentChatUser : this.getAdminPhone());
        
        if (!recipient) {
            console.error('No recipient selected');
            return false;
        }

        console.log('📤 Sending message to:', recipient);

        // Send via WebSocket
        this.send({
            type: 'message',
            to: recipient,
            text: text.trim(),
            admin: this.isAdmin ? this.currentUser.phone : null
        });

        // Add optimistically to UI (only for customer mode with renderMessages)
        if (!this.isAdmin) {
            this.addMessageOptimistically(text, true);
        }
        
        return true;
    }

    // Select user for chat (Admin only)
    selectUser(phone, userName = null) {
        this.currentChatUser = phone;
        this.currentChatUserName = userName || phone;
        
        console.log('👤 Selected user for chat:', this.currentChatUserName, phone);
        
        // Request history via WebSocket
        if (this.isConnected) {
            this.requestHistory(phone);
        } else {
            // Fallback to REST if not connected
            this.loadMessagesREST(phone);
        }
    }

    // Request chat history via WebSocket
    requestHistory(phone) {
        console.log('📜 Requesting history for:', phone);
        this.send({
            type: 'getHistory',
            with: phone
        });
    }

    // Alias for requestHistory
    async loadHistory(phone) {
        return this.requestHistory(phone);
    }

    // Load messages via REST (fallback)
    async loadMessagesREST(toUser = null) {
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
                
                // Dispatch history event for admin chat page
                window.dispatchEvent(new CustomEvent('chatHistory', {
                    detail: {
                        messages: this.messages.map(m => ({
                            from: m.fromUser,
                            to: m.toUser,
                            text: m.messageText,
                            time: m.addDate
                        }))
                    }
                }));
                
                this.renderMessages();
            }
        } catch (error) {
            console.error('Error loading messages:', error);
        }
    }

    // Load users (Admin only) via REST
    async loadUsers() {
        if (!this.isAdmin) return;
        
        try {
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
        }
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
        const container = document.getElementById('messages-container') || 
                         document.getElementById('customer-messages-wrapper');
        
        if (!container) return;

        if (this.messages.length === 0) {
            // Keep welcome message if exists
            if (!container.querySelector('.welcome-message')) {
                container.innerHTML = `
                    <div style="text-align: center; padding: 40px; color: #6b7280;">
                        <p>No messages yet. Start the conversation!</p>
                    </div>
                `;
            }
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
                    <div class="date-divider customer-date-divider">
                        <span>${this.formatDateDivider(msgDate)}</span>
                    </div>
                `;
                lastDate = dateStr;
            }

            const isSent = msg.fromUser === this.currentUser.phone;
            const time = this.formatTime(msgDate);

            html += `
                <div class="message-bubble customer-message ${isSent ? 'sent' : 'received'}">
                    ${!isSent ? `
                        <div class="message-avatar customer-message-avatar">
                            <span class="material-symbols-outlined">storefront</span>
                        </div>
                    ` : ''}
                    <div class="message-content customer-message-bubble">
                        <div class="customer-message-content">
                            <p class="message-text customer-message-text">${this.escapeHtml(msg.messageText)}</p>
                            <span class="message-time customer-message-time">${time}</span>
                        </div>
                    </div>
                </div>
            `;
        });

        container.innerHTML = html;
        this.scrollToBottom();
    }

    // Add message optimistically
    addMessageOptimistically(text, isSent) {
        this.messages.push({
            fromUser: this.currentUser.phone,
            toUser: this.isAdmin ? this.currentChatUser : this.getAdminPhone(),
            messageText: text,
            addDate: new Date().toISOString(),
            isRead: false
        });
        
        this.renderMessages();
    }

    // Scroll to bottom
    scrollToBottom() {
        const container = document.getElementById('messages-container') || 
                         document.getElementById('customer-messages-area');
        
        if (container) {
            setTimeout(() => {
                container.scrollTop = container.scrollHeight;
            }, 100);
        }
    }

    // Utility methods
    formatTime(date) {
        if (!date) return '';
        const d = typeof date === 'string' ? new Date(date) : date;
        return d.toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit' });
    }

    formatDateDivider(date) {
        const today = new Date();
        const msgDate = typeof date === 'string' ? new Date(date) : date;
        
        if (msgDate.toDateString() === today.toDateString()) {
            return 'Today';
        }
        
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);
        
        if (msgDate.toDateString() === yesterday.toDateString()) {
            return 'Yesterday';
        }
        
        return msgDate.toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' });
    }

    getInitials(name) {
        if (!name) return '?';
        return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    getAdminPhone() {
        // Get admin phone from config or localStorage
        return localStorage.getItem('admin_phone') || '9876543210';
    }

    getHeaders() {
        const token = localStorage.getItem('token');
        return {
            'Content-Type': 'application/json',
            'Authorization': token ? `Bearer ${token}` : ''
        };
    }

    // Cleanup
    destroy() {
        if (this.ws) {
            this.ws.close();
            this.ws = null;
        }
    }
}

// Auto-initialize if elements exist
if (typeof adminChat === 'undefined' && document.getElementById('conversations-list')) {
    var adminChat = new ChatManager(true);
    adminChat.initialize();
    // Load users for admin
    adminChat.loadUsers();
}

if (typeof customerChat === 'undefined' && document.getElementById('customer-messages-wrapper')) {
    var customerChat = new ChatManager(false);
    customerChat.initialize();
}
