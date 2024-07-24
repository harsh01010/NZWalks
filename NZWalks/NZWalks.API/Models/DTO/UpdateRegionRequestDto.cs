using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(3,ErrorMessage ="Min length should be 3")]
        [MaxLength(5,ErrorMessage ="max length should be 4")]
        public string Code { get; set; }
        
        [Required]
        
        public string Name { get; set; } 
        public string? RegionImageUrl { get; set; }
    }
}
