using EmployeeProjectManagements.UI.Manager;
using EmployeeProjectManagements.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EmployeeProjectManagements.UI.Controllers
{
	public class EmployeesController : Controller
	{

		private readonly HttpClient _httpClient;
		private readonly EMPManager _empManager;
		private readonly IConfiguration _configuration;
		public EmployeesController(IHttpClientFactory clientFactory, EMPManager empManager, IConfiguration configuration)
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
		public async Task<IActionResult> GetEmployees(int draw, int start, int length, string search)
		{
			try
			{
				//Initial Data
				string token = HttpContext.Session.GetString("AuthToken");
				if (string.IsNullOrEmpty(token))
				{
					//Initial Data
					var userName = _configuration.GetSection("TokenUser:UserName").Value;
					var password = _configuration.GetSection("TokenUser:Password").Value;

					// Create token
					token = _empManager.CreateToken(userName, password);

					if (string.IsNullOrEmpty(token))
					{
						return Json(new { Error = "Token is invalid" });
					}

					// Save token in session
					HttpContext.Session.SetString("AuthToken", token);
				}
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var response = await _httpClient.GetAsync($"api/Employees/GetEmployees?draw={draw}&start={start}&length={length}&search={search}&ascending=true");
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
				var result = System.Text.Json.JsonSerializer.Deserialize<EmployeeResponse>(responseBody, options);

				return Json(new
				{
					draw = result.draw,
					recordsTotal = result.recordsTotal,
					recordsFiltered = result.recordsFiltered,
					data = result.data
				});
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(ex.Message);
			}
		}
		[HttpPost]
		public async Task<IActionResult> DeleteEmployee(int id)
		{
			try
			{
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var response = await _httpClient.DeleteAsync($"api/Employees/DeleteEmployee?id={id}");
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
		public async Task<IActionResult> AddOrEditEmployee([FromBody] EmployeeDTO employee)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var employeeJson = JsonConvert.SerializeObject(employee);
				var content = new StringContent(employeeJson, Encoding.UTF8, "application/json");
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.PostAsync("api/Employees/AddOrEditEmployee", content);

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

		[HttpGet]
		public async Task<IActionResult> GetListEmployee()
		{
			try
			{
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync("api/Employees/GetListEmployee");
				if (!response.IsSuccessStatusCode)
				{
					return View("Error", new { message = "There was an issue retrieving the employee list." });
				}

				var employeesJson = await response.Content.ReadAsStringAsync();
				var employees = JsonConvert.DeserializeObject<List<ListEmployeeDto>>(employeesJson);

				return new JsonResult(employees);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, new { message = "An error occurred while retrieving the employee list." });
			}
		}


		[HttpPost]
		public async Task<IActionResult> GetAssignedEmployees(int projectId)
		{
			try
			{
				string token = HttpContext.Session.GetString("AuthToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.PostAsync($"api/Projects/GetAssignedEmployees?projectId={projectId}",null);
				if (!response.IsSuccessStatusCode)
				{
					return View("Error", new { message = "There was an issue retrieving the employee list." });
				}

				var employeesJson = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Employees JSON: {employeesJson}");

				var employees = JsonConvert.DeserializeObject<List<string>>(employeesJson)
	.Select(name => new { Name = name }) 
	.ToList();

				return new JsonResult(employees);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, new { message = "An error occurred while retrieving the employee list." });
			}
		}
	}
}
