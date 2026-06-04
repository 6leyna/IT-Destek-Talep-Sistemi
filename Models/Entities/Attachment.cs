namespace ITDestek.Models.Entities;

public class Attachment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; } // in bytes
    public string UploadedById { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.Now;
    
    // Navigation properties
    public virtual Ticket Ticket { get; set; } = null!;
    public virtual ApplicationUser UploadedBy { get; set; } = null!;
}
