using Microsoft.AspNetCore.Identity;

namespace LinkShortener.Models;

public class Link
{
    public int Id { get; set; } 
    public string OriginalUrl { get; set; } 
    public string ShortCode { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public int ClickCount { get; set; } 
    public string UserId { get; set; }
    public IdentityUser User { get; set; } 
}
