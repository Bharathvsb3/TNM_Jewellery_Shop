using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelleryShopAPI.Models
{
    [Table("Chatusr")]
    public class ChatUser
    {
        [Key]
        [Column("id_col")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("userid")]
        public string UserId { get; set; } = string.Empty; // Phone number

        [Required]
        [MaxLength(500)]
        [Column("fcmtoken")]
        public string FcmToken { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("devicetype")]
        public string? DeviceType { get; set; }

        [Column("status")]
        public int Status { get; set; } = 1; // 1 = active, 0 = inactive

        [MaxLength(50)]
        [Column("adduser")]
        public string? AddUser { get; set; }

        [Column("adddate")]
        public DateTime AddDate { get; set; } = DateTime.UtcNow;

        [MaxLength(8)]
        [Column("addtime")]
        public string? AddTime { get; set; }
    }

    [Table("Chathst")]
    public class ChatHistory
    {
        [Key]
        [Column("id_col")]
        public int Id { get; set; }

        [Column("adddate")]
        public DateTime AddDate { get; set; } = DateTime.UtcNow;

        [MaxLength(8)]
        [Column("addtime")]
        public string? AddTime { get; set; }

        [MaxLength(50)]
        [Column("adduser")]
        public string? AddUser { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("fromusr")]
        public string FromUser { get; set; } = string.Empty; // Phone number

        [Required]
        [MaxLength(50)]
        [Column("tousr")]
        public string ToUser { get; set; } = string.Empty; // Phone number

        [Required]
        [Column("msgtxt")]
        public string MessageText { get; set; } = string.Empty;

        [Column("isread")]
        public int IsRead { get; set; } = 0; // 0 = unread, 1 = read
    }
}
