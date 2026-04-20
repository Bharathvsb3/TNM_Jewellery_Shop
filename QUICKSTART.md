# 🚀 QUICK START GUIDE

## Setup in 5 Minutes

### Step 1: Install .NET 8 SDK
Download from: https://dotnet.microsoft.com/download/dotnet/8.0

### Step 2: Start Backend

```powershell
cd E:\JMW\JewelleryShopAPI
dotnet restore
dotnet run
```

✅ Backend running at: `http://localhost:5000`

### Step 3: Start Frontend

**Option A - VS Code Live Server:**
1. Install "Live Server" extension
2. Right-click `E:\JMW\Frontend\index.html`
3. Select "Open with Live Server"

**Option B - Python:**
```powershell
cd E:\JMW\Frontend
python -m http.server 5500
```

✅ Frontend running at: `http://localhost:5500`

### Step 4: Login

Navigate to `http://localhost:5500` and login:

- **Mobile**: `9999999999`
- **Password**: `admin123`

---

## First Steps After Login

### 1. Add Gold Rate ⭐ (REQUIRED)
- Go to **Gold Tracking** page
- Click **Add Rate**
- Enter rates (only allowed during time windows):
  - **Morning**: 10 AM - 12 PM
  - **Evening**: 5 PM - 6 PM
- Maximum 2 entries per day

### 2. Add a Customer
- Go to **Customers** page
- Click **Add Customer**
- Enter name and **unique mobile number**
- Save

### 3. Add a Worker (Optional)
- Go to **Workers** page
- Click **Add Worker**
- Select type: Shop / External
- Save

### 4. Create Your First Order
- Go to **Orders** page
- Click **Create Order**
- Select customer
- Fill in jewelry details:
  - Type, Category, Metal, Purity
  - Weight, Wastage, Making Charges
- **Gold rate auto-filled** from latest entry
- **Total auto-calculated**
- Enter **advance amount** (required)
- Balance auto-calculated
- Save

### 5. Assign Craft Work (Optional)
- Go to **Craft Work** page
- Click **Assign Craft**
- Select order and worker
- Enter raw gold given
- Add stones if needed
- Save

---

## ⚠️ Important Rules

### Gold Rate Entry:
- ✅ Only 2 entries per day
- ✅ Morning slot: 10 AM - 12 PM
- ✅ Evening slot: 5 PM - 6 PM
- ❌ Cannot add outside time windows
- ❌ Cannot add duplicate time slot same day

### Mobile Numbers:
- ✅ Must be exactly 10 digits
- ✅ Must be unique for customers
- ✅ Must be unique for users

### Orders:
- ✅ Advance amount is mandatory
- ✅ Gold rate must exist before creating order
- ✅ All calculations are server-side (secure)

### Decimal Precision:
- All weight/amounts: 3 decimal places
- Example: 45.567 grams

---

## 🎨 Features Checklist

### Available Now:
- ✅ Login / Register
- ✅ Dashboard with stats
- ✅ Customer Management
- ✅ Enquiries (CRM)
- ✅ Orders with auto-calculation
- ✅ Workers management
- ✅ Products catalog (dynamic)
- ✅ Gold Tracking (strict validation)
- ✅ Craft Workflow (assign & complete)
- ✅ Reports (daily sales)
- ✅ Chat system
- ✅ Profile & password change
- ✅ Shop settings
- ✅ Fully responsive design

### Roles:
- ✅ ADMIN (current)
- 🔜 Shop Worker (future)
- 🔜 External Worker (future)
- 🔜 Customer (future)

---

## 📱 Mobile Access

The system is fully responsive! Access from:
- 📱 Mobile phones
- 📱 Tablets
- 💻 Desktop

**Mobile users**: Tap menu icon (☰) to toggle sidebar

---

## 🔧 Troubleshooting

### Backend won't start:
```powershell
# Check .NET version
dotnet --version

# Should be 8.0.x
# Clean and rebuild
dotnet clean
dotnet build
dotnet run
```

### Frontend errors:
1. Check API URL in `Frontend/js/config.js`
2. Make sure backend is running
3. Check browser console for errors
4. Clear browser cache

### Login fails:
- Verify backend is running at `http://localhost:5000`
- Check network tab in browser dev tools
- Try default credentials: `9999999999` / `admin123`

### CORS errors:
- Backend has CORS enabled for all origins (development)
- For production, configure allowed origins in `Program.cs`

---

## 🔐 Change Default Password

After first login:
1. Click **Profile** in sidebar
2. Enter current password: `admin123`
3. Enter new password
4. Save

---

## 📊 Database Location

SQLite database file:
```
E:\JMW\JewelleryShopAPI\jewelleryshop.db
```

View with tools like:
- DB Browser for SQLite
- SQLiteStudio
- VS Code SQLite extension

---

## 🌐 API Testing

Use tools like:
- **Postman**
- **Swagger UI**: `http://localhost:5000/swagger`
- **Thunder Client** (VS Code extension)

---

## 📸 Screenshots Coming Soon

(Add screenshots of your system here)

---

## ✅ Production Deployment

### Backend:
1. Change JWT secret in `appsettings.json`
2. Update connection string for SQL Server
3. Enable HTTPS
4. Publish: `dotnet publish -c Release`
5. Deploy to IIS / Azure / AWS

### Frontend:
1. Update API URL to production endpoint
2. Deploy to web server / CDN
3. Enable HTTPS

---

**🎊 You're all set! Happy managing!**
