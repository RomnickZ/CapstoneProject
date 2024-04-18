using Capstone_Group2.Entities;

public class TaskViewModel
{
    public int TaskId { get; set; }
    public string? TaskName { get; set; }
    public string? TaskDescription { get; set; }
    public DateTime? Start_Date { get; set; }
    public DateTime? End_Date { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }
    public int PriorityId { get; set; }

    // Navigation properties
    public Category? Category { get; set; }
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
}