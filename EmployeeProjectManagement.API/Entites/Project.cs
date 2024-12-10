using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectManagement.API.Entites
{
	public class Project
	{
		[Key]
		public int Id { get; set; }
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Status { get; set; }
		public ICollection<EmployeeProject> EmployeeProjects { get; set; }
	}
}
