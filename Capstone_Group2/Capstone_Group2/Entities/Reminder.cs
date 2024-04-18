namespace Capstone_Group2.Entities
{
    public class Reminder
    {
        public int ReminderId { get; set; }
        public string? ReminderContent { get;set; }


        //FK
        public int TaskId { get; set; }
    }
}
