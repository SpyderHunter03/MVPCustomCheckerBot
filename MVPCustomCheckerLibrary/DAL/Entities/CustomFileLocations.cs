using System.ComponentModel.DataAnnotations;

namespace MVPCustomCheckerLibrary.DAL.Entities
{
  public class CustomFileLocations
  {
	public int Id { get; set; }

	[MaxLength(50)]
	public string FileName { get; set; }

	[MaxLength(250)]
	public string FileLocation { get; set; }
  }
}
