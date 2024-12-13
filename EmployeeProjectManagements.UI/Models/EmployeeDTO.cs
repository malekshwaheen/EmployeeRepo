using EmployeeProjectManagements.UI.Helpers;
using System.ComponentModel.DataAnnotations;
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

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[Phone(ErrorMessage = "Invalid phone number format.")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Department is required.")]
		public string Department { get; set; }

		public List<ProjectDTO> EmployeeProjects { get; set; }

		public EmployeeDTO()
		{
			EmployeeProjects = new List<ProjectDTO>();
		}
	}

	public class ProjectDTOs
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Project Name is required.")]
        [StringLength(100, ErrorMessage = "Project Name cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "End Date must be greater than Start Date.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

    }
	public class ProjectDTO
	{
		// Add properties relevant to a project, for example:
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
	}
}
