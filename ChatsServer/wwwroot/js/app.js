new Vue({
    el: '#app',
    data: {
        username: '',
        password: '',
        authMessage: '',
        contacts: [],
        selectedContact: null,
        messages: [],
        messageInput: '',
        connection: null
    },
    mounted() {
        this.loadContacts();
        this.initializeSignalR();
    },
    methods: {
        initializeSignalR() {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/chatsHub")
                .build();

            this.connection.on("ReceiveMessage", (message) => {
                if (message.user_id === this.selectedContact.id) {
                    this.messages.push(message);
                    this.$nextTick(() => {
                        this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
                    });
                }
            });

            this.connection.on("NewUser", (user) => {
                this.contacts.push(user);
            });

            this.connection.start().catch(err => console.error(err.toString()));
        },
        loadContacts() {
            const token = this.getCookie('token');
            if (!token) {
                UIkit.modal('#authWindow').show();
                return;
            }

            fetch(`/api/${token}/chats/`)
                .then(this.checkUnauthorized)
                .then(response => response.json())
                .then(contacts => {
                    this.contacts = contacts;
                })
                .catch(error => console.error('Error loading contacts:', error));
        },
        selectContact(contact) {
            this.selectedContact = contact;
            this.loadMessages(contact.id);
        },
        loadMessages(userId) {
            const token = this.getCookie('token');
            fetch(`/api/${token}/messages/${userId}`)
                .then(this.checkUnauthorized)
                .then(response => response.json())
                .then(messages => {
                    this.messages = messages;
                    this.$nextTick(() => {
                        this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
                    });
                })
                .catch(error => console.error('Error loading messages:', error));
        },
        sendMessage() {
            const token = this.getCookie('token');
            const message = this.messageInput.trim();
            if (!message || !this.selectedContact) return;

            const data = {
                user_id: this.selectedContact.id,
                is_operator_message: true,
                value: message,
                added_time: new Date().toISOString()
            };

            fetch(`/api/${token}/messages/${this.selectedContact.id}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
                .then(this.checkUnauthorized)
                .then(response => {
                    if (response.ok) {
                        this.messageInput = '';
                    }
                })
                .catch(error => console.error('Error sending message:', error));
        },
        authenticate() {
            fetch('/api/auth', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: this.username, password: this.password })
            })
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    }
                    throw new Error('Authentication failed');
                })
                .then(data => {
                    this.setCookie('token', data.token, 7);
                    UIkit.modal('#authWindow').hide();
                    this.loadContacts();
                })
                .catch(error => {
                    this.authMessage = error.message;
                });
        },
        checkUnauthorized(response) {
            if (response.status === 401) {
                UIkit.modal('#authWindow').show();
            }
            return response;
        },
        formatDate(dateString) {
            const date = new Date(dateString);
            return date.toLocaleTimeString();
        },
        getCookie(name) {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts.length === 2) return parts.pop().split(';').shift();
        },
        setCookie(name, value, days) {
            const expires = new Date(Date.now() + days * 864e5).toUTCString();
            document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expires}; path=/`;
        }
    }
});