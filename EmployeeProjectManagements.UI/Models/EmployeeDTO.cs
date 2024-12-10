using System.Text.Json.Serialization;

namespace EmployeeProjectManagements.UI.Models
{
	public class EmployeeResponse
	{
		public int draw { get; set; }
		public int recordsTotal { get; set; }
		public int recordsFiltered { get; set; }
		public List<EmployeeDTO> data { get; set; }
	}
	public class ProjectResponse
	{
		public int draw { get; set; }
		public int recordsTotal { get; set; }
		public int recordsFiltered { get; set; }
		public List<ProjectDTOs> data { get; set; }
	}
	public class EmployeeDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Department { get; set; }

		[JsonPropertyName("employeeProjects")]
		public List<ProjectDTO> EmployeeProjects { get; set; }

		public EmployeeDTO()
		{
			EmployeeProjects = new List<ProjectDTO>();
		}
	}

	public class ProjectDTOs
	{
		public int Id { get; set; }
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Status { get; set; }
	    
	}
	public class ProjectDTO
	{
		// Add properties relevant to a project, for example:
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
	}
}
