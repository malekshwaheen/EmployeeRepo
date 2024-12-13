using EmployeeProjectManagements.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using EmployeeProjectManagements.UI.Manager;
using Newtonsoft.Json;
using System.Text;

namespace EmployeeProjectManagements.UI.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly EMPManager _empManager;
		private readonly IConfiguration _configuration;
		public ProjectsController(IHttpClientFactory clientFactory, EMPManager empManager, IConfiguration configuration)
		{
			_httpClient = clientFactory.CreateClient("API");
			_empManager = empManager;
			_configuration = configuration;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> GetProjects(int draw, int start, int length, string search)
		{
			try
			{
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var response = await _httpClient.GetAsync($"api/Projects/GetProjects?draw={draw}&start={start}&length={length}&search={search}&ascending=true");
				if (!response.IsSuccessStatusCode)
				{
					return View("Error");
				}

				var options = new JsonSerializerOptions
				{
					DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
					AllowTrailingCommas = true,
					PropertyNameCaseInsensitive = true
				};

				var responseBody = await response.Content.ReadAsStringAsync();
				var result = System.Text.Json.JsonSerializer.Deserialize<ProjectResponse>(responseBody, options);

				return Json(new
				{
					draw = result.draw,
					recordsTotal = result.recordsTotal,
					recordsFiltered = result.recordsFiltered,
					data = result.data
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(ex.Message);
			}
		}

		public async Task<IActionResult> DeleteProject(int id)
		{
			try
			{
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var response = await _httpClient.DeleteAsync($"api/Projects/DeleteProject?id={id}");
				if (!response.IsSuccessStatusCode)
				{
					return View("Error", new { message = "There was an issue deleting the employee." });
				}

				TempData["SuccessMessage"] = "Employee deleted successfully.";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return View("Error", new { message = "An error occurred while deleting the employee." });
			}
		}

		[HttpPost]
		public async Task<IActionResult> AddOrEditProject([FromBody] ProjectDTOs project)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values
			  .SelectMany(v => v.Errors)
			  .Select(e => e.ErrorMessage)
			  .ToList();
					return BadRequest(new { success = false, errors });
				}
				var employeeJson = JsonConvert.SerializeObject(project);
				var content = new StringContent(employeeJson, Encoding.UTF8, "application/json");
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.PostAsync("api/Projects/AddOrEditProject", content);

				if (!response.IsSuccessStatusCode)
				{
					return View("Error", new { message = "There was an issue AddOrEditProject." });
				}

				TempData["SuccessMessage"] = "Project Added successfully.";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return View("Error", new { message = "An error occurred while deleting the Project." });
			}
		}
		[HttpPost]

		public async Task<IActionResult> AssignEmployeeToProject(int projectId, int employeeId)
		{
			try
			{
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.PostAsync($"api/Employees/AssignEmployeeToProject?projectId={projectId}&employeeId={employeeId}", null);
				if (!response.IsSuccessStatusCode)
				{
					return View("Error", new { message = "There was an issue SaveAsignedEMPTOProject the employee." });
				}

				TempData["SuccessMessage"] = "Employee Assigned To Project successfully.";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return View("Error", new { message = "An error occurred while SaveAsignedEMPTOProject the employee." });
			}
		}
	}
}
