namespace ITDestek.Models.Entities;

public class TicketComment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false; // Technician notes (not visible to employee)
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    // Navigation properties
    public virtual Ticket Ticket { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
