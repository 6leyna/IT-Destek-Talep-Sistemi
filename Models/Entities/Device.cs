namespace ITDestek.Models.Entities;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public string? AssignedUserId { get; set; }
    public int DepartmentId { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser? AssignedUser { get; set; }
    public virtual Department Department { get; set; } = null!;
}
