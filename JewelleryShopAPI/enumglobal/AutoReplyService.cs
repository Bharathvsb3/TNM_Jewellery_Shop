public class AutoReplyService
{
    private static readonly Dictionary<string, string> AutoReplyKeywords =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Login & access
        { "login", "Thank you for reaching out. We’ve received your login issue and our support team will assist you shortly." },
        { "password", "Password-related issue noted. Please don’t worry, our team will help you soon." },
        { "access", "Access issue received. Our admin will review and get back to you shortly." },
        { "permission", "Permission request noted. Our team will check and update you soon." },

        // General Ledger (GL)
        { "gl", "Thank you for the message. We’ve received your General Ledger concern and will review it shortly." },
        { "ledger", "Ledger-related issue noted. Our finance support team will assist you soon." },
        { "posting", "Posting issue received. Our team will verify and respond shortly." },
        { "voucher", "Voucher-related issue noted. Please wait while we review it." },
        { "balance", "Balance-related concern received. Our team will check and update you shortly." },

        // Purchase Order (PO)
        { "po", "Purchase Order issue received. Our support team will look into this shortly." },
        { "purchase", "Purchase-related query noted. Our team will respond soon." },
        { "vendor", "Vendor-related issue received. Please wait while we review it." },
        { "approval", "Approval-related issue noted. Our team will check and update you shortly." },

        // Sales Order (SO)
        { "so", "Sales Order issue received. Our team will review and assist you shortly." },
        { "sales", "Sales-related query noted. Our support team will respond soon." },
        { "customer", "Customer-related issue received. Our team will look into this shortly." },
        { "invoice", "Invoice-related issue noted. Please wait while our team reviews it." },

        // Warehouse
        { "warehouse", "Warehouse-related issue received. Our team will check and respond shortly." },
        { "stock", "Stock-related concern noted. Our team will verify and update you soon." },
        { "inventory", "Inventory issue received. Please wait while we review it." },
        { "qty", "Quantity-related issue noted. Our team will look into it shortly." },
        { "grn", "Goods receipt issue received. Our team will review and assist you shortly." },
        { "delivery", "Delivery-related issue noted. Our team will check and update you soon." },

        // Performance & errors
        { "slow", "We’re sorry for the inconvenience. Performance issue noted and will be checked soon." },
        { "error", "Thank you for informing us. The issue is received and will be reviewed shortly." },
        { "crash", "We’re sorry about this. The issue is noted and our team will assist you soon." },
        { "loading", "Loading issue received. Please wait while we check this." },

        // Payment & billing
        { "payment", "Payment-related concern received. Our team will review and assist you shortly." },
        { "paid", "Thank you for the update. We’ll verify the payment details and get back to you soon." },
        { "billing", "Billing-related query noted. Our team will respond shortly." },
        { "refund", "Refund request received. Our team will review and update you soon." },

        // General support
        { "help", "Thank you for contacting support. Our admin will respond as soon as possible." },
        { "issue", "Thanks for informing us. Our team will review and get back to you shortly." },
        { "urgent", "We understand the urgency. Your request is received and will be addressed soon." }

    };

    public string GetAutoReply(string userText)
    {
        if (string.IsNullOrWhiteSpace(userText))
            return GetDefaultReply();

        foreach (var item in AutoReplyKeywords)
        {
            if (userText.Contains(item.Key, StringComparison.OrdinalIgnoreCase))
                return item.Value;
        }

        return GetDefaultReply();
    }

    private string GetDefaultReply()
    {
        return "Thank you for contacting us. Our support team will get back to you shortly.";
    }
}
