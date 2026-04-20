using System.ComponentModel.DataAnnotations;

namespace JewelleryShopAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "ADMIN";

        [Required]
        [MaxLength(20)]
        public string AccountStatus { get; set; } = "Approved"; // Pending, Approved, Rejected

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
        
        // Customer-specific default values for orders
        public decimal DefaultWastage { get; set; } = 0; // If 0, use config default
        public decimal DefaultGst { get; set; } = 0; // If 0, use config default

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
    }

    public class Enquiry
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; } // Internal notes (not visible to customer)

        public DateTime? ExpectedDate { get; set; } // When customer expects the jewelry

        public DateTime? FollowupDate { get; set; } // When to follow up with customer

        [MaxLength(50)]
        public string? MetalType { get; set; } // Gold, Silver, Platinum, Other

        [MaxLength(100)]
        public string? JewelryType { get; set; } // Ring, Necklace, Earrings, etc.

        [MaxLength(50)]
        public string? Purity { get; set; } // 24K, 22K, 18K, etc.

        public decimal? EstimatedWeight { get; set; }

        [MaxLength(50)]
        public string? MakingType { get; set; } // KRDM, Normal, Casting, Handmade

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "New"; // New, Follow-up, Converted, Closed
        
        // Design reference images (base64 encoded)
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
        
        // Track which order this enquiry was converted to
        public int? ConvertedToOrderId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public class Worker
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string WorkerType { get; set; } = "External"; // Internal, External

        // Password for external workers (craftsmen) to login
        public string? PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<CraftAssignment> CraftAssignments { get; set; } = new List<CraftAssignment>();
    }

    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? JewelryType { get; set; } // Ring, Necklace, Earring, etc.

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MetalType { get; set; } = "Gold"; // Gold, Silver, Platinum

        public int? Purity { get; set; } // 916, 925, 999, etc.

        public decimal Weight { get; set; }
        
        public decimal? Wastage { get; set; } // Wastage percentage
        
        public decimal? MakingCharges { get; set; } // Making charges amount
        
        public decimal? Gst { get; set; } // GST percentage
        
        public int? Quantity { get; set; } // Optional stock quantity

        [MaxLength(500)]
        public string? StoneDetails { get; set; } // Optional stone/diamond details
        
        [MaxLength(1000)]
        public string? Description { get; set; } // Optional product description

        // Product images (base64 encoded) - no MaxLength for base64 support
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        
        // Track if this order was created from an enquiry
        public int? EnquiryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string JewelryType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MetalType { get; set; } = "Gold";

        [Required]
        [MaxLength(20)]
        public string Purity { get; set; } = "916";

        public decimal GrossWeight { get; set; }

        public decimal Wastage { get; set; }

        public decimal MakingCharges { get; set; }

        public decimal? Gst { get; set; }

        [MaxLength(500)]
        public string? StoneDetails { get; set; }

        public decimal EstimatedWeight { get; set; }

        public decimal GoldRate { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal AdvanceAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        [MaxLength(20)]
        public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        public int? ApprovedById { get; set; }
        public User? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Ordered"; // Ordered, Approved, Assigned, InProgress, Completed, Shipped, Ready, Delivered

        [MaxLength(1000)]
        public string? CustomerRemarks { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedCompletionDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Delivery fields
        public decimal? ActualWeight { get; set; }
        public decimal? WastageGrams { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? AdjustmentAmount { get; set; }
        
        [MaxLength(1000)]
        public string? DeliveryNotes { get; set; }
        
        // Design reference images (base64 encoded)
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
        
        public DateTime? DeliveryDate { get; set; }

        public CraftAssignment? CraftAssignment { get; set; }
    }

    public class GoldRate
    {
        public int Id { get; set; }

        public decimal Gold24KRate { get; set; } // Pure Gold (24K)

        public decimal Gold916Rate { get; set; } // Standard Gold (22K)

        public decimal GoldNormalRate { get; set; }

        public decimal SilverRate { get; set; }

        [Required]
        [MaxLength(10)]
        public string TimeSlot { get; set; } = "Morning"; // Morning, Evening

        public DateTime EntryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CraftAssignment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int WorkerId { get; set; }
        public Worker Worker { get; set; } = null!;

        public DateTime AssignedDate { get; set; }

        public decimal RawGoldGiven { get; set; }

        public bool StoneGiven { get; set; }

        public DateTime? CompletionDate { get; set; }

        public decimal? FinalWeight { get; set; }

        public decimal? StoneWeight { get; set; }

        public decimal? OtherMaterialWeight { get; set; }

        public decimal? UsedGold { get; set; }

        public decimal? BalanceGold { get; set; }

        public bool? IsVerified { get; set; }

        public decimal? ManualUsedGold { get; set; }

        public decimal? ManualBalanceGold { get; set; }

        [MaxLength(500)]
        public string? DesignImage { get; set; } // Optional design reference image for craftsman

        [MaxLength(20)]
        public string Status { get; set; } = "Assigned"; // Assigned, InProgress, Completed

        // Craftsman portal fields
        [MaxLength(20)]
        public string? CraftsmanStatus { get; set; } // Making, Ready, Delivered

        public decimal? ReturnWeight { get; set; }

        [MaxLength(1000)]
        public string? CraftsmanRemarks { get; set; }

        public bool PendingAdminApproval { get; set; } = false;

        public bool AdminConfirmed { get; set; } = false;

        [MaxLength(1000)]
        public string? AdminRemarks { get; set; }

        public decimal? MaterialReturn { get; set; }

        public decimal? StoneReturn { get; set; }

        [MaxLength(2000)]
        public string? ReturnRemarks { get; set; }

        public DateTime? UpdateTimestamp { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<StoneDetail> StoneDetails { get; set; } = new List<StoneDetail>();
        public ICollection<MaterialDetail> MaterialDetails { get; set; } = new List<MaterialDetail>();
    }

    public class StoneDetail
    {
        public int Id { get; set; }

        public int CraftAssignmentId { get; set; }
        public CraftAssignment CraftAssignment { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string StoneType { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // Material detail for line-by-line tracking of all materials given to craftsman
    public class MaterialDetail
    {
        public int Id { get; set; }

        public int CraftAssignmentId { get; set; }
        public CraftAssignment CraftAssignment { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string MaterialType { get; set; } = string.Empty; // RawGold, Stone, Diamond, Pearl, OtherMetal, Finding, etc.

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        [MaxLength(20)]
        public string? Unit { get; set; } // grams, pieces, carats, etc.

        public decimal? EstimatedValue { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // CraftCompletion model to track completion details and material returns
    public class CraftCompletion
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int CraftAssignmentId { get; set; }
        public CraftAssignment CraftAssignment { get; set; } = null!;

        // Final product details
        public decimal FinalWeight { get; set; }

        // Total raw materials given (calculated from MaterialDetails)
        public decimal TotalRawGiven { get; set; }

        // System-calculated return (Total Given - Final Product)
        public decimal SystemCalculatedReturn { get; set; }

        // Actual return entered by admin/craftsman
        public decimal? ActualReturnWeight { get; set; }

        // Difference between system calculation and actual return
        public decimal? ReturnDifference { get; set; }

        [MaxLength(1000)]
        public string? CompletionRemarks { get; set; }

        [MaxLength(1000)]
        public string? ReturnRemarks { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; } = false;

        public int? VerifiedBy { get; set; }
        
        public DateTime? VerifiedAt { get; set; }

        [MaxLength(1000)]
        public string? VerificationRemarks { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string MessageText { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    public class ShopSettings
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ShopName { get; set; } = "T.N. Mani Gopi Jewellers";

        [MaxLength(500)]
        public string? ShopLogo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public int? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class GoldDispatch
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string PersonName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ShopName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MaterialType { get; set; } = "Gold"; // Gold, Silver, Platinum, Other

        public decimal DispatchWeight { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending, returned, sold

        public DateTime DispatchDate { get; set; } = DateTime.UtcNow;

        public decimal? ReturnWeight { get; set; }

        public DateTime? ReturnDate { get; set; }

        [MaxLength(1000)]
        public string? ReturnRemarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
