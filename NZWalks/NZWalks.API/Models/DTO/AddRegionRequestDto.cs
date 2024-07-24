using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class AddRegionRequestDto
    {
        [Required]
        [MinLength(3,ErrorMessage ="Code has to be minimum 3 length")]
        [MaxLength(5,ErrorMessage ="Max length is 5")]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
