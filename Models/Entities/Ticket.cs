namespace ITDestek.Models.Entities;

public class Ticket
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty; // IT-2026-0152 format
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? OfficeLocation { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int CategoryId { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; } = Status.Yeni;
    public string? AssignedToId { get; set; } // IT Staff user ID
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Department Department { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ApplicationUser? AssignedTo { get; set; }
    public virtual ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
