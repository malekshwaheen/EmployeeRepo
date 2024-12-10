using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectManagement.API.Entites
{
	public class Employee
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Department { get; set; }
		public ICollection<EmployeeProject> EmployeeProjects { get; set; }
	}
}
