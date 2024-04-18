using System.ComponentModel.DataAnnotations;

namespace Capstone_Group2.Entities
{
    public class EmailNotification
    {
        //PK
        [Key]
        public int ENId { get; set; }
        public DateTime NotificationTimestamp { get; set; }
        //FK
        public string? UserId { get; set; }
        public int? TaskId { get; set; }
        public bool? NotificationSent { get; set; }
    }
}
