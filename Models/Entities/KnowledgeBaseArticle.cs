namespace ITDestek.Models.Entities;

public class KnowledgeBaseArticle
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int Views { get; set; }
    public bool IsHelpful { get; set; } // Simple rating: helpful/not helpful
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    
    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual ApplicationUser CreatedBy { get; set; } = null!;
}
