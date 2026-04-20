# 🏪 Jewellery Shop Management System

**Full-Stack SPA with CRM, Order Management, Craft Workflow, and Gold Tracking**

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Tech Stack](#tech-stack)
3. [Features](#features)
4. [Project Structure](#project-structure)
5. [Installation & Setup](#installation--setup)
6. [API Endpoints](#api-endpoints)
7. [Database Schema](#database-schema)
8. [Frontend SPA Routing](#frontend-spa-routing)
9. [User Guide](#user-guide)
10. [Development Guide](#development-guide)

---

## 🎯 Overview

A comprehensive jewellery shop management system with:

- **Full CRM** for customer management
- **Order Management** with auto-calculation
- **Craft Workflow** with gold tracking
- **Gold Rate Management** (2 entries per day - strict validation)
- **Dynamic Products** (fully configurable)
- **Reports & Analytics**
- **Chat System**
- **Responsive Design** (Mobile, Tablet, Desktop)

---

## 🧱 Tech Stack

### Backend
- **Framework**: ASP.NET Core 8.0 Web API
- **ORM**: Entity Framework Core
- **Database**: SQLite (easily migrates to SQL Server)
- **Authentication**: JWT Bearer Tokens
- **Password Hashing**: BCrypt.Net

### Frontend
- **HTML5, CSS3, Vanilla JavaScript**
- **SPA**: Hash-based routing (no page reload)
- **Fonts**: Google Fonts (Noto Serif + Manrope)
- **Icons**: Material Symbols + Font Awesome
- **Design**: Premium rich & bright theme with gold/blue palette

---

## ✨ Features

### 🔐 Authentication
- Login with Email/Mobile
- Registration with mobile validation
- JWT token-based auth
- Password change
- **ADMIN role** only (for now)

###  👥 Customer Management
- Add/Edit customers
- Unique mobile validation
- Active/Inactive toggle
- Notes section
- Create orders directly

### 📞 Enquiries (CRM)
- **Kanban-style workflow**
- Status: New → Follow-up → Converted → Closed
- Customer linking

### 🧾 Orders (Advanced)
- **Auto-calculation** based on latest gold rate
- Fields: Jewelry Type, Category, Metal, Purity, Weight, Wastage, Making Charges, Stones
- **Mandatory advance payment**
- Auto balance calculation
- Status tracking

### 👷 Workers
- Shop / External workers
- CRUD operations
- Active toggle

### 💎 Products (Dynamic)
- Fully dynamic product catalog
- Categories, Metal Type, Weight
- **3 images per product** (with compression)
- Active/Inactive management

### 💰 Gold Tracking (Strict Rules)
- **2 entries per day only**
- Time windows:
  - Morning: 10 AM - 12 PM
  - Evening: 5 PM - 6 PM
- Validates time slot and prevents duplicates
- Latest rate automatically used in orders

### 🛠️ Craft Workflow
1. **Assign Craftsman**: Select worker, assign raw gold, add stones (optional)
2. **Completion Entry**: Final weight, stone weight, other materials
3. **Auto-calculation**:
   - Used Gold = Final - (Stone + Other)
   - Balance Gold = Raw Given - Used Gold
4. **Verification**: Checkbox or manual entry (audit trail)
5. **Status**: Assigned → InProgress → Completed

### 📊 Reports
- Daily sales report
- Date range filtering
- Order count, total sales, advance, balance

### 💬 Chat
- User-to-user messaging
- Conversation history
- Read/unread status

### 👤 Profile
- Edit profile
- Change password

### 🏪 Shop Settings
- Shop name customization
- Logo upload
- Display in navbar & login page

---

## 📁 Project Structure

```
JMW/
├── JewelleryShopAPI/                # Backend
│   ├── Controllers/                 # API Controllers
│   │   ├── AuthAndCustomerControllers.cs
│   │   ├── BusinessControllers.cs
│   │   └── WorkflowControllers.cs
│   ├── Data/
│   │   └── AppDbContext.cs          # EF Core DbContext
│   ├── DTOs/                        # Data Transfer Objects
│   │   ├── AuthDTOs.cs
│   │   ├── CustomerDTOs.cs
│   │   ├── OrderDTOs.cs
│   │   ├── CommonDTOs.cs
│   │   └── CraftDTOs.cs
│   ├── Models/                      # Database Models
│   │   └── Models.cs
│   ├── Repositories/                # Data Access Layer
│   │   ├── IRepositories.cs
│   │   └── Repositories.cs
│   ├── Services/                    # Business Logic Layer
│   │   ├── AuthService.cs
│   │   ├── CustomerService.cs
│   │   ├── OrderService.cs
│   │   ├── CommonServices.cs
│   │   ├── GoldRateService.cs
│   │   ├── CraftWorkflowService.cs
│   │   └── OtherServices.cs
│   ├── Program.cs                   # App entry point
│   ├── appsettings.json            # Configuration
│   └── JewelleryShopAPI.csproj     # Project file
│
└── Frontend/                        # Frontend SPA
    ├── index.html                   # Login page
    ├── register.html                # Registration page
    ├── dashboard.html               # Main dashboard (SPA shell)
    ├── css/
    │   ├── premium-theme.css        # Premium design system
    │   ├── login.css                # Login/Register styles
    │   └── dashboard.css            # Dashboard layout
    ├── js/
    │   ├── config.js                # API config & helpers
    │   ├── login.js                 # Login logic
    │   ├── register.js              # Register logic
    │   ├── spa-router.js            # SPA router
    │   └── dashboard.js             # Dashboard logic
    └── pages/                       # SPA module pages
        ├── dashboard.html
        ├── customers.html
        ├── enquiries.html
        ├── orders.html
        ├── workers.html
        ├── products.html
        ├── gold-tracking.html
        ├── craft-work.html
        ├── reports.html
        ├── chat.html
        └── profile.html
```

---

## 🚀 Installation & Setup

### Prerequisites
- **.NET 8 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio Code** or **Visual Studio 2022**
- **Live Server** extension (for frontend) or any web server

### Backend Setup

1. **Navigate to API folder**:
   ```powershell
   cd E:\JMW\JewelleryShopAPI
   ```

2. **Restore NuGet packages**:
   ```powershell
   dotnet restore
   ```

3. **Run the API**:
   ```powershell
   dotnet run
   ```

   The API will start at: **`http://localhost:5000`**

4. **Database**:
   - SQLite database (`jewelleryshop.db`) is auto-created on first run
   - Default admin user:
     - **Mobile**: `9999999999`
     - **Password**: `admin123`

### Frontend Setup

1. **Update API URL** (if needed):
   - Open `Frontend/js/config.js`
   - Update `API_BASE_URL` if your backend runs on a different port

2. **Run Frontend**:
   - Using **VS Code Live Server**:
     - Right-click `Frontend/index.html` → "Open with Live Server"
   - Or serve via any web server on: **`http://localhost:5500`** (or any other port)

3. **Login**:
   - Use default admin credentials above

---

## 📡 API Endpoints

### Authentication
- `POST /api/Auth/login` - Login
- `POST /api/Auth/register` - Register
- `POST /api/Auth/change-password` - Change password

### Customers
- `GET /api/Customers` - Get all customers
- `GET /api/Customers/{id}` - Get customer by ID
- `POST /api/Customers` - Create customer
- `PUT /api/Customers/{id}` - Update customer

### Enquiries
- `GET /api/Enquiries` - Get all enquiries
- `GET /api/Enquiries/by-status/{status}` - Get by status
- `GET /api/Enquiries/{id}` - Get by ID
- `POST /api/Enquiries` - Create enquiry
- `PUT /api/Enquiries/{id}` - Update enquiry

### Orders
- `GET /api/Orders` - Get all orders
- `GET /api/Orders/{id}` - Get by ID
- `GET /api/Orders/customer/{customerId}` - Get by customer
- `POST /api/Orders` - Create order
- `PUT /api/Orders/{id}` - Update order

### Workers
- `GET /api/Workers` - Get all workers
- `GET /api/Workers/{id}` - Get by ID
- `POST /api/Workers` - Create worker
- `PUT /api/Workers/{id}` - Update worker

### Products
- `GET /api/Products` - Get all products
- `GET /api/Products/{id}` - Get by ID
- `POST /api/Products` - Create product
- `PUT /api/Products/{id}` - Update product

### Gold Rates
- `GET /api/GoldRates/latest` - Get latest rate
- `GET /api/GoldRates/today` - Get today's rates
- `POST /api/GoldRates` - Create rate (validates time window)

### Craft Workflow
- `GET /api/CraftWorkflow` - Get all assignments
- `GET /api/CraftWorkflow/{id}` - Get by ID
- `GET /api/CraftWorkflow/worker/{workerId}` - Get by worker
- `POST /api/CraftWorkflow/assign` - Assign craft
- `POST /api/CraftWorkflow/{id}/complete` - Complete craft

### Messages
- `GET /api/Messages/conversation/{otherUserId}` - Get conversation
- `POST /api/Messages` - Send message
- `POST /api/Messages/{id}/read` - Mark as read

### Reports
- `GET /api/Reports/daily-sales?startDate=&endDate=` - Daily sales report

### Shop Settings
- `GET /api/ShopSettings` - Get settings
- `PUT /api/ShopSettings` - Update settings

---

## 🗄️ Database Schema

### Key Tables

**Users**: id, name, mobile (unique), email, passwordHash, role, isActive

**Customers**: id, name, mobile (unique), notes, isActive

**Enquiries**: id, customerId, subject, description, status

**Orders**: id, customerId, jewelryType, category, metalType, purity, grossWeight, wastage, makingCharges, stoneDetails, estimatedWeight, goldRate, totalPrice, advanceAmount, balanceAmount, status

**Workers**: id, name, mobile, workerType, isActive

**Products**: id, name, category, metalType, weight, image1, image2, image3, isActive

**GoldRates**: id, gold916Rate, goldNormalRate, silverRate, timeSlot, entryDate

**CraftAssignments**: id, orderId, workerId, assignedDate, rawGoldGiven, stoneGiven, completionDate, finalWeight, stoneWeight, otherMaterialWeight, usedGold, balanceGold, isVerified, manualUsedGold, manualBalanceGold, status

**StoneDetails**: id, craftAssignmentId, stoneType, weight

**Messages**: id, senderId, receiverId, messageText, isRead, sentAt

**ShopSettings**: id, shopName, shopLogo

---

## 🧭 Frontend SPA Routing

The system uses **hash-based routing** for a true SPA experience:

- `#dashboard` → Dashboard
- `#customers` → Customers
- `#enquiries` → Enquiries
- `#orders` → Orders
- `#workers` → Workers
- `#products` → Products
- `#gold-tracking` → Gold Tracking
- `#craft-work` → Craft Work
- `#reports` → Reports
- `#chat` → Chat
- `#profile` → Profile

**No page reload!** Content is dynamically loaded into the content area.

---

## 📱 Responsive Design

Fully responsive across all devices:

- **Desktop**: Full sidebar + 2-column layout
- **Tablet**: Smaller sidebar, optimized cards
- **Mobile**: Collapsible sidebar, single-column layout, touch-friendly

**Breakpoints**:
- Desktop: > 1024px
- Tablet: 768px - 1024px
- Mobile: < 768px

---

## 👨‍💼 User Guide

### First Time Setup

1. **Login** with default admin (mobile: `9999999999`, password: `admin123`)
2. **Add Gold Rate** (required before creating orders)
3. **Add Customers**
4. **Add Workers** (for craft workflow)
5. **Add Products** (optional)
6. **Create Orders**

### Daily Workflow

1. **Enter Gold Rates** (Morning 10-12 AM & Evening 5-6 PM)
2. **Manage Enquiries** (CRM workflow)
3. **Create Orders** for customers
4. **Assign Craft Work** to workers
5. **Complete Craft Work** with verification
6. **Review Reports**

---

## 🛠️ Development Guide

### Migrating to SQL Server

1. Update `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=JewelleryShop;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

2. The system auto-detects SQL Server and uses it instead of SQLite.

### Adding New Features

**Backend**:
1. Create model in `Models/`
2. Add DbSet to `AppDbContext`
3. Create repository interface & implementation
4. Create service interface & implementation
5. Register in `Program.cs`
6. Create controller with endpoints

**Frontend**:
1. Create HTML page in `pages/`
2. Add route in `spa-router.js`
3. Add nav item in `dashboard.html`
4. Implement page logic with `pageLoaded` event

### Security Notes

- Change JWT secret key in production (`appsettings.json`)
- Implement HTTPS in production
- Add rate limiting
- Implement refresh tokens for long sessions
- Add input sanitization
- Enable CORS only for trusted origins

---

## 🎨 Theme Customization

Edit `css/premium-theme.css` to customize colors:

```css
:root {
    --gold-primary: #D4AF37;      /* Primary gold */
    --blue-primary: #1A4D7B;      /* Primary blue */
    --accent-emerald: #50C878;    /* Success green */
    --accent-ruby: #E0115F;       /* Error red */
}
```

---

## 📝 License

Proprietary - All rights reserved

---

## 🙏 Support

For support and questions, contact your system administrator.

---

**Built with ❤️ for Jewellery Management**
