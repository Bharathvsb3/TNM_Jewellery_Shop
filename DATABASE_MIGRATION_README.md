# Database Migration Guide

## Error Fixed
**Error**: `SQLite Error 1: 'no such column: c.DefaultGst'`

**Solution**: Run the database migration to add the missing columns.

---

## Quick Start (Recommended)

### Option 1: Automatic Migration Script
```powershell
cd E:\JMW
.\MigrateDatabase.ps1
```

This script will:
- Automatically find your database
- Try sqlite3 command if available
- Fall back to C# migration tool if sqlite3 is not found
- Add all required columns

---

## Manual Migration Options

### Option 2: Using PowerShell (if sqlite3 is installed)
```powershell
cd E:\JMW
.\RunAllMigrationsSQLite.ps1
```

### Option 3: Using C# Migration Tool
```powershell
cd E:\JMW\DatabaseMigration
dotnet run
```

### Option 4: Direct SQL (if you have SQLite GUI tool)

Run these SQL commands in your SQLite tool:

```sql
-- Add Enquiry-Order linking
ALTER TABLE Enquiries ADD COLUMN ConvertedToOrderId INTEGER NULL;
ALTER TABLE Orders ADD COLUMN EnquiryId INTEGER NULL;

-- Add Customer default values
ALTER TABLE Customers ADD COLUMN DefaultMakingCharges REAL NOT NULL DEFAULT 0;
ALTER TABLE Customers ADD COLUMN DefaultWastage REAL NOT NULL DEFAULT 0;
ALTER TABLE Customers ADD COLUMN DefaultGst REAL NOT NULL DEFAULT 0;
```

---

## After Migration

1. **Restart your API server** (important!)
2. Refresh your browser
3. The error should be gone

---

## Database Location

Your SQLite database is typically located at:
- `E:\JMW\JewelleryShopAPI\jewellery_shop.db`
- OR `E:\JMW\JewelleryShopAPI\bin\Debug\net8.0\jewellery_shop.db`

---

## What These Columns Do

### Customer Defaults
- **DefaultMakingCharges**: Making charges per gram for this customer (0 = use system default)
- **DefaultWastage**: Wastage percentage for this customer (0 = use system default)  
- **DefaultGst**: GST percentage for this customer (0 = use system default)

### Enquiry-Order Linking
- **ConvertedToOrderId**: Tracks which order an enquiry was converted to
- **EnquiryId**: Tracks which enquiry created an order

---

## Troubleshooting

### "Database not found"
- Check if your database path matches in the migration scripts
- Look in both locations mentioned above

### "sqlite3 not found"
- Use Option 1 (MigrateDatabase.ps1) - it will automatically use C# tool
- OR install SQLite: `winget install SQLite.SQLite`

### "Column already exists"
- This is normal if you've run the migration before
- The error is safely ignored

---

## Need Help?
If migration fails, check:
1. API server is stopped (close it before migration)
2. Database file is not locked/in use
3. You have write permissions to the database file
