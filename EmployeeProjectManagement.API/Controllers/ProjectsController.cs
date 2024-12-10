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
	public class ProjectsController : ControllerBase
	{
		private readonly EMPDbContext _context;

		public ProjectsController(EMPDbContext context)
		{
			_context = context;
		}
		[HttpGet]
		[Route("GetProjects")]
		public async Task<IActionResult> GetProjects(int draw, int start, int length, string search = null, string orderBy = null, bool ascending = true)
		{

			IQueryable<Project> query = _context.Projects;
			orderBy = "name";

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(e => e.ProjectName.Contains(search) || e.Status.Contains(search));
			}


			query = orderBy switch
			{
				"name" => ascending ? query.OrderBy(e => e.ProjectName) : query.OrderByDescending(e => e.ProjectName),
				"status" => ascending ? query.OrderBy(e => e.Status) : query.OrderByDescending(e => e.Status),
				_ => ascending ? query.OrderBy(e => e.ProjectName) : query.OrderByDescending(e => e.ProjectName),
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

		[HttpDelete]
		[Route("DeleteProject")]
		public async Task<IActionResult> DeleteProject(int id)
		{
			var employee = _context.Projects.Find(id);
			_context.Projects.Remove(employee);
			await _context.SaveChangesAsync();
			return Ok("Deleted Success");
		}


		[HttpPost]
		[Route("AddOrEditProject")]
		public async Task<IActionResult> AddOrEditProject(ProjectDTO project)
		{
			if (project.Id == 0)
			{
				var proEnity = new Project()
				{
					ProjectName = project.ProjectName,
					StartDate = project.StartDate,
					EndDate= project.EndDate,
					Status = project.Status
				};
				_context.Projects.Add(proEnity);
				await _context.SaveChangesAsync();
				return Ok(proEnity);
			}
			var projectToUpdate = _context.Projects.FirstOrDefault(e => e.Id == project.Id);
			projectToUpdate.ProjectName = project.ProjectName;
			projectToUpdate.StartDate = project.StartDate;
			projectToUpdate.EndDate= project.EndDate;
			projectToUpdate.Status = project.Status;
			_context.Projects.Update(projectToUpdate);
			await _context.SaveChangesAsync();
			return Ok(projectToUpdate);
		}


		[HttpPost]
		[Route("GetAssignedEmployees")]

		public async Task<IActionResult> GetAssignedEmployees(int projectId)
		{
			try
			{
				var employees = _context.EmployeeProjects.Where(x => x.ProjectId == projectId).Select(x=>x.Employee.Name).ToList();
				return new JsonResult(employees);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		[HttpPost]
		[Route("GetProjectWithEmployeeAssigned")]

		public async Task<IActionResult> GetProjectWithEmployeeAssigned()
		{
			try
			{
				var projects = _context.EmployeeProjects.Where(x => x.Project != null).Select(x => x.Project.ProjectName).Distinct().ToList();
				return new JsonResult(projects);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
