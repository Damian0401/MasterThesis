using System.ComponentModel.DataAnnotations;

public class PackageAssignmentRequest
{
    [Required]
    public string LockerId { get; set; }
    [Required]
    public string PackageCode { get; set; }
}