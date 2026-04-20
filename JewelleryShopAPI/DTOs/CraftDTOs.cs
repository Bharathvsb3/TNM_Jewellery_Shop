namespace JewelleryShopAPI.DTOs
{
    public class CraftAssignmentDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string JewelryType { get; set; } = string.Empty;
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
        public string Status { get; set; } = string.Empty;
        public string? CraftsmanStatus { get; set; }
        public decimal? ReturnWeight { get; set; }
        public string? CraftsmanRemarks { get; set; }
        public bool PendingAdminApproval { get; set; }
        public bool AdminConfirmed { get; set; }
        public string? AdminRemarks { get; set; }
        public decimal? MaterialReturn { get; set; }
        public decimal? StoneReturn { get; set; }
        public string? ReturnRemarks { get; set; }
        public List<StoneDetailDTO> StoneDetails { get; set; } = new();
        public List<MaterialDetailDTO> MaterialDetails { get; set; } = new();
        
        // Order fields for craftsman display
        public string? MetalType { get; set; }
        public string? Purity { get; set; }
        public decimal? GrossWeight { get; set; }
        public string? Category { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        
        // Design images from Order
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
    }

    public class MaterialDetailDTO
    {
        public int Id { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string? Unit { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string? Notes { get; set; }
    }

    public class StoneDetailDTO
    {
        public int Id { get; set; }
        public string StoneType { get; set; } = string.Empty;
        public decimal Weight { get; set; }
    }

    public class AssignCraftRequest
    {
        public int OrderId { get; set; }
        public int WorkerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public decimal RawGoldGiven { get; set; }
        public bool StoneGiven { get; set; }
        public List<CreateStoneDetailRequest>? StoneDetails { get; set; }
        public List<CreateMaterialDetailRequest>? MaterialDetails { get; set; }
    }

    public class CreateMaterialDetailRequest
    {
        public string MaterialType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string? Unit { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateStoneDetailRequest
    {
        public string StoneType { get; set; } = string.Empty;
        public decimal Weight { get; set; }
    }

    public class CompleteCraftRequest
    {
        public DateTime CompletionDate { get; set; }
        public decimal FinalWeight { get; set; }
        public decimal StoneWeight { get; set; }
        public decimal OtherMaterialWeight { get; set; }
        public bool? IsVerified { get; set; }
        public decimal? ManualUsedGold { get; set; }
        public decimal? ManualBalanceGold { get; set; }
        public decimal? MaterialReturn { get; set; }
        public decimal? StoneReturn { get; set; }
        public string? ReturnRemarks { get; set; }
        public string? AdminRemarks { get; set; }
    }

    public class CraftsmanUpdateRequest
    {
        public string Status { get; set; } = string.Empty; // Making, Ready
        public string? Remarks { get; set; }
    }

    public class CraftsmanCompleteRequest
    {
        public decimal FinalWeight { get; set; }
        public decimal? ReturnWeight { get; set; }
        public string? Remarks { get; set; }
    }

    public class AdminVerifyReturnRequest
    {
        public decimal? AdjustedReturnWeight { get; set; }
        public string? AdminRemarks { get; set; }
        public bool Approved { get; set; }
    }

    public class StartWorkRequest
    {
        public string? InitialRemarks { get; set; }
    }

    public class CraftCompletionDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CraftAssignmentId { get; set; }
        public decimal FinalWeight { get; set; }
        public decimal TotalRawGiven { get; set; }
        public decimal SystemCalculatedReturn { get; set; }
        public decimal? ActualReturnWeight { get; set; }
        public decimal? ReturnDifference { get; set; }
        public string? CompletionRemarks { get; set; }
        public string? ReturnRemarks { get; set; }
        public DateTime CompletedAt { get; set; }
        public bool IsVerified { get; set; }
        public int? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerificationRemarks { get; set; }
    }

    public class CompleteWorkRequest
    {
        public decimal FinalWeight { get; set; }
        public decimal? ActualReturnWeight { get; set; }
        public string? CompletionRemarks { get; set; }
        public string? ReturnRemarks { get; set; }
    }

    public class DailySalesReport
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalAdvance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
