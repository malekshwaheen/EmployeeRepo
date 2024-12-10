﻿namespace EmployeeProjectManagement.API.DTOs
{
	public class EmployeeDTO
	{
		public int? Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Email { get; set; } = string.Empty;
		public string ?PhoneNumber { get; set; } = string.Empty;
		public string ?Department { get; set; } = string.Empty;
	}
}
