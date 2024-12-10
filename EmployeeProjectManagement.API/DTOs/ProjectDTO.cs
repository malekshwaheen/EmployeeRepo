namespace EmployeeProjectManagement.API.DTOs
{
	public class ProjectDTO
	{
		public int? Id { get; set; }
		public string ProjectName { get; set; } 
		public DateTime StartDate { get; set; } 
		public DateTime EndDate { get; set; } 
		public string Status { get; set; } 
	}
}
