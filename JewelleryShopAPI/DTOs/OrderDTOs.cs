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
}
