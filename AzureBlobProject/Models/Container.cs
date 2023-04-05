using System.ComponentModel.DataAnnotations;

namespace AzureBlobProject.Models
{
    public class Container
    {
        [Required]
        [RegularExpression("^[a-z0-9](?!.*--)[a-z0-9-]{1,61}[a-z0-9]$", ErrorMessage = "This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.Each hyphen must be preceded and followed by a non-hyphen character.The name must also be between 3 and 63 characters long.")]
        public string? Name { get; set; }    
    }
}
