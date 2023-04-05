using System.ComponentModel.DataAnnotations;

namespace AzureBlobProject.Models
{
	public class Blob
	{
		[Required]
		public string? Title { get; set; }
		public string? Comment { get; set; }
		public string? Uri { get; set; }
	}
}
