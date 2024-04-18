using Capstone_Group2.Entities;
using System.ComponentModel.DataAnnotations;

public class TimetableTask
{
    // PK
    [Key]
    public int TaskId { get; set; }

    public string? TaskName { get; set; }
    public string? TaskDescription { get; set; }
    public DateTime? Start_Date { get; set; }
    public DateTime? End_Date { get; set; }

    // FK
    public int CategoryId { get; set; }
    public int PriorityId { get; set; }
    public int StatusId { get; set; }
    public string? TimetableId { get; set; }

    // Navigation properties
    public Category? Category { get; set; }
    public Priority? Priority { get; set; }
    public Status? Status { get; set; }
}