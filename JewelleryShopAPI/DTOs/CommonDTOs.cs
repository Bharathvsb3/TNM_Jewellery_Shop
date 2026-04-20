namespace JewelleryShopAPI.DTOs
{
    public class EnquiryDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string? MetalType { get; set; }
        public string? JewelryType { get; set; }
        public string? Purity { get; set; }
        public decimal? EstimatedWeight { get; set; }
        public string? MakingType { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
        public int? ConvertedToOrderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateEnquiryRequest
    {
        public int CustomerId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string? MetalType { get; set; }
        public string? JewelryType { get; set; }
        public string? Purity { get; set; }
        public decimal? EstimatedWeight { get; set; }
        public string? MakingType { get; set; }
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
    }

    public class UpdateEnquiryRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string? MetalType { get; set; }
        public string? JewelryType { get; set; }
        public string? Purity { get; set; }
        public decimal? EstimatedWeight { get; set; }
        public string? MakingType { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DesignImage { get; set; }
        public string? DesignImage2 { get; set; }
        public string? DesignImage3 { get; set; }
        public int? ConvertedToOrderId { get; set; }
    }

    public class WorkerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string WorkerType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateWorkerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string WorkerType { get; set; } = "External";
        public bool IsActive { get; set; } = true;
        public string? Password { get; set; }
    }

    public class UpdateWorkerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string WorkerType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Password { get; set; }
    }

    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? JewelryType { get; set; }
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = string.Empty;
        public int? Purity { get; set; }
        public decimal Weight { get; set; }
        public decimal? Wastage { get; set; }
        public decimal? MakingCharges { get; set; }
        public decimal? Gst { get; set; }
        public int? Quantity { get; set; }
        public string? StoneDetails { get; set; }
        public string? Description { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? JewelryType { get; set; }
        public string Category { get; set; } = string.Empty;
        public string MetalType { get; set; } = "Gold";
        public int? Purity { get; set; }
        public decimal Weight { get; set; }
        public decimal? Wastage { get; set; }
        public decimal? MakingCharges { get; set; }
        public decimal? Gst { get; set; }
        public int? Quantity { get; set; }
        public string? StoneDetails { get; set; }
        public string? Description { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }
    }

    public class GoldRateDTO
    {
        public int Id { get; set; }
        public decimal Gold24KRate { get; set; }
        public decimal Gold916Rate { get; set; }
        public decimal GoldNormalRate { get; set; }
        public decimal SilverRate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateGoldRateRequest
    {
        public decimal Gold24KRate { get; set; }
        public decimal Gold916Rate { get; set; }
        public decimal GoldNormalRate { get; set; }
        public decimal SilverRate { get; set; }
        public string TimeSlot { get; set; } = "Morning";
    }

    public class UpdateGoldRateRequest
    {
        public decimal Gold24KRate { get; set; }
        public decimal Gold916Rate { get; set; }
        public decimal GoldNormalRate { get; set; }
        public decimal SilverRate { get; set; }
        public string TimeSlot { get; set; } = "Morning";
    }

    public class MessageDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SendMessageRequest
    {
        public int ReceiverId { get; set; }
        public string MessageText { get; set; } = string.Empty;
    }

    public class ShopSettingsDTO
    {
        public int Id { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string? ShopLogo { get; set; }
    }

    public class UpdateShopSettingsRequest
    {
        public string ShopName { get; set; } = string.Empty;
        public string? ShopLogo { get; set; }
    }

    // Chat System DTOs
    public class RegisterDeviceRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string FCMToken { get; set; } = string.Empty;
        public string? DeviceType { get; set; }
    }

    public class ChatSendMessageRequest
    {
        public string FromUser { get; set; } = string.Empty;
        public string ToUser { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
