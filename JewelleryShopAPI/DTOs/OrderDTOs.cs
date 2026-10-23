namespace JewelleryShopAPI.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerMobile { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int? EnquiryId { get; set; }
        public string JewelryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = string.Empty;
        public string Purity { get; set; } = string.Empty;
        public decimal GrossWeight { get; set; }
        public decimal Wastage { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal? Gst { get; set; }
        public string? StoneDetails { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal GoldRate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        
        // Design Images
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
        
        // Delivery fields
        public decimal? ActualWeight { get; set; }
        public decimal? WastageGrams { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? AdjustmentAmount { get; set; }
        public string? DeliveryNotes { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? ExchangedToOrderId { get; set; }  // non-null if this order was exchanged
    }

    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public int? EnquiryId { get; set; }
        public string JewelryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = string.Empty;
        public string Purity { get; set; } = "916";
        public decimal GrossWeight { get; set; }
        public decimal Wastage { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal? Gst { get; set; }
        public string? StoneDetails { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
    }

    public class UpdateOrderRequest
    {
        public string JewelryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = string.Empty;
        public string Purity { get; set; } = string.Empty;
        public decimal GrossWeight { get; set; }
        public decimal Wastage { get; set; }
        public decimal MakingCharges { get; set; }
        public decimal? Gst { get; set; }
        public string? StoneDetails { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        
        // Design Images
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
        
        // Delivery fields
        public decimal? ActualWeight { get; set; }
        public decimal? WastageGrams { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? AdjustmentAmount { get; set; }
        public string? DeliveryNotes { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal? GoldRate { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? BalanceAmount { get; set; }
    }

    public class CustomerOrderRequest
    {
        public int? ProductId { get; set; }
        public decimal? CustomWeight { get; set; }
        public string? CustomerRemarks { get; set; }
    }

    public class OrderTrackingDTO
    {
        public int Id { get; set; }
        public string JewelryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public DateTime? ApprovalDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public TrackingStep[] TrackingSteps { get; set; } = Array.Empty<TrackingStep>();
    }

    public class TrackingStep
    {
        public string StepName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Completed, Current, Pending
        public DateTime? CompletedAt { get; set; }
    }

    public class ApproveOrderRequest
    {
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class CustomerOutstandingReportDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerMobile { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Outstanding { get; set; }
        public decimal GstAmount { get; set; }
        public decimal WastageGrams { get; set; }
        public decimal MakingCharges { get; set; }
        public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
    }

    public class CustomerOutstandingRequestDTO
    {
        public int? StartCustomerId { get; set; }
        public int? EndCustomerId { get; set; }
    }

    public class ExchangeOrderRequest
    {
        // New item details
        public string JewelryType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = "Gold";
        public string Purity { get; set; } = "916";
        public decimal GrossWeight { get; set; }
        public decimal Wastage { get; set; }       // Wastage in grams
        public decimal MakingCharges { get; set; }
        public decimal Gst { get; set; }

        // Exchange payment info
        public decimal ExtraAdvancePaid { get; set; } // Cash paid by customer at exchange (above exchange credit)
        public decimal OldOrderUnpaidBalance { get; set; } // Any remaining balance the customer hadn't paid on the old order
        public string? Notes { get; set; }

        // Locked rates passed from frontend (today's rate at time of exchange)
        public decimal ExchangeRateForOldItem { get; set; }   // Rate used to value old item (today's rate for old purity)
        public decimal ExchangeRateForNewItem { get; set; }   // Rate used to price new item (today's rate for new purity)
        public decimal ExchangeCreditOverride { get; set; }   // If > 0, use this as exchange credit (e.g. total amount paid by customer)
    }

    public class ExchangeOrderResponseDTO
    {
        public int OriginalOrderId { get; set; }
        public int NewOrderId { get; set; }
        public decimal OldItemWeight { get; set; }
        public decimal OldItemRate { get; set; }
        public decimal ExchangeCredit { get; set; }       // Old item value credited
        public decimal NewItemTotal { get; set; }         // Full cost of new item
        public decimal ExtraAdvancePaid { get; set; }     // Cash paid at exchange
        public decimal BalanceDue { get; set; }           // Net balance customer still owes (positive = customer pays, negative = shop refunds)
        public OrderDTO NewOrder { get; set; } = null!;
    }
}
