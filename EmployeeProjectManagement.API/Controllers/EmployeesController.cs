using EmployeeProjectManagement.API.Context;
using EmployeeProjectManagement.API.DTOs;
using EmployeeProjectManagement.API.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagement.API.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	[Route("api/[controller]")]
	public class EmployeesController : ControllerBase
	{
		private readonly EMPDbContext _context;

		public EmployeesController(EMPDbContext context)
		{
			_context = context;
		}

		//[HttpGet]
		//public async Task<IActionResult> GetEmployees()
		//{
		//	var employees = await _context.Employees.ToListAsync();


		//	return Ok(employees);
		//}
		[HttpGet]
		[Route("GetEmployees")]
		public async Task<IActionResult> GetEmployees(int draw, int start, int length, string search = null, string orderBy = null, bool ascending = true)
		{
			try
			{


				IQueryable<Employee> query = _context.Employees;
				orderBy = "name";

				if (!string.IsNullOrEmpty(search))
				{
					query = query.Where(e => e.Name.Contains(search) || e.Email.Contains(search) || e.PhoneNumber.Contains(search) || e.Department.Contains(search));
				}


				query = orderBy switch
				{
					"email" => ascending ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email),
					"phoneNumber" => ascending ? query.OrderBy(e => e.PhoneNumber) : query.OrderByDescending(e => e.PhoneNumber),
					"department" => ascending ? query.OrderBy(e => e.Department) : query.OrderByDescending(e => e.Department),
					_ => ascending ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name),
				};


				var totalRecords = await query.CountAsync();
				var employees = await query.Skip(start).Take(length).ToListAsync();

				if (totalRecords < start + length)
				{
					start = 0;
					employees = await query.Skip(start).Take(totalRecords - start).ToListAsync();  // Adjust the 'take' based on the available records
				}

				var result = new
				{
					draw = draw,
					recordsTotal = totalRecords,
					recordsFiltered = totalRecords,
					data = employees
				};

				return Ok(result);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		[HttpGet]
		[Route("GetListEmployee")]
		public async Task<IActionResult> GetListEmployee()
		{
			try
			{
				var employees = await _context.Employees
					.Select(e => new { e.Id, e.Name })
					.ToListAsync();

				return new JsonResult(employees);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, "An error occurred while retrieving the employee list.");
			}
		}
		[HttpPost]
		[Route("AddOrEditEmployee")]
		public async Task<IActionResult> AddOrEditEmployee(EmployeeDTO employee)
		{
			if (employee.Id == 0)
			{
				var empEnity = new Employee()
				{
					Name = employee.Name,
					Department = employee.Department,
					Email = employee.Email,
					PhoneNumber = employee.PhoneNumber
				};
				_context.Employees.Add(empEnity);
				await _context.SaveChangesAsync();
				return Ok(employee);
			}
			var emoloyeeToUpdate = _context.Employees.FirstOrDefault(e => e.Id == employee.Id);
			emoloyeeToUpdate.PhoneNumber = employee.PhoneNumber;
			emoloyeeToUpdate.Name = employee.Name;
			emoloyeeToUpdate.Department= employee.Department;
			emoloyeeToUpdate.Email = employee.Email;
			_context.Employees.Update(emoloyeeToUpdate);
			await _context.SaveChangesAsync();
			return Ok(employee);
		}


		[HttpDelete]
		[Route("DeleteEmployee")]
		public async Task<IActionResult> DeleteEmployee(int id)
		{
			var employee = _context.Employees.Find(id);
			_context.Employees.Remove(employee);
			await _context.SaveChangesAsync();
			return Ok("Deleted Success");
		}

		[HttpPost]
		[Route("AssignEmployeeToProject")]
		public async Task<IActionResult> AssignEmployeeToProject(int projectId, int employeeId)
		{
			try
			{
				var empProject = new EmployeeProject()
				{
					EmployeeId = employeeId,
					ProjectId = projectId
				};
				await _context.EmployeeProjects.AddAsync(empProject);
				await _context.SaveChangesAsync();
				return Ok(empProject);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, "An error occurred while AssignEmployeeToProject.");
			}
		}
	}

}
