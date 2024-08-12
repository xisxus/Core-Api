using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcCoreApiEmployee.Models.DTOs;
using MvcCoreApiEmployee.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace MvcCoreApiEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _db;
        public EmployeesController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            List<Employee> employees = _db.Employees.Include(e => e.Experiences).ToList();



            string jsonString = JsonConvert.SerializeObject(employees, Newtonsoft.Json.Formatting.Indented, new
            JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.  Ignore
            });
            return Content(jsonString, "application/json");
        }


        [HttpGet("{employeeId}")]
        public IActionResult GetEmployeeById(int employeeId)
        {
            Employee employee = _db.Employees.Include(e => e.Experiences).FirstOrDefault(e => e.EmployeeId ==
            employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            string jsonString = JsonConvert.SerializeObject(employee, Newtonsoft.Json.Formatting.Indented, new
            JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(jsonString, "application/json");
        }


        [HttpPost]
        public async Task<IActionResult> PostEmployee([FromForm] CommonDTO objCommon)
        {
            ImgUpload FileApi = new ImgUpload();
            string fileName = objCommon.ImgName + ".jpg";
            FileApi.ImgName = "\\Upload\\" + fileName;
            if (objCommon.ImgFile?.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Upload"))
                {
                    Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                }
                string filePath = _environment.WebRootPath + "\\Upload\\" + fileName;
                using (FileStream fileStream = System.IO.File.Create(filePath))
                {
                    objCommon.ImgFile.CopyTo(fileStream);
                    fileStream.Flush();
                }
                FileApi.ImgName = "/Upload/" + fileName;
            }
            Employee empObj = new Employee();
            empObj.Name = objCommon.Name;
            empObj.IsActive = objCommon.IsActive;
            empObj.JoinDate = objCommon.JoinDate;
            empObj.ImageName = objCommon.ImgName;
            empObj.ImageUrl = FileApi.ImgName;
            _db.Add(empObj);
            await _db.SaveChangesAsync();
            
            List<Experience> list = JsonConvert.DeserializeObject<List<Experience>>(objCommon.Experiences);

         

            var emp = _db.Employees.FirstOrDefault(x => x.Name == objCommon.Name);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    Experience objEx = new Experience
                    {
                        EmployeeId = emp.EmployeeId,
                        Title = item.Title,
                        Duration = item.Duration,
                    };
                    _db.Experiences.Add(objEx);
                }
                await _db.SaveChangesAsync();
            }
            return Ok("Employee created successfully.");
        }


        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, [FromForm] CommonDTO objCommon)
        {
            var empObj = await _db.Employees.FindAsync(employeeId);
            if (empObj == null)
            {
                return NotFound("Employee not found.");
            }
            ImgUpload FileApi = new ImgUpload();
            string fileName = objCommon.ImgName + ".jpg";
            FileApi.ImgName = "\\Upload\\" + fileName;
            if (objCommon.ImgFile?.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Upload"))
                {
                    Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                }
                string filePath = _environment.WebRootPath + "\\Upload\\" + fileName;
                using (FileStream fileStream = System.IO.File.Create(filePath))
                {
                    objCommon.ImgFile.CopyTo(fileStream);
                    fileStream.Flush();
                }
                FileApi.ImgName = "/Upload/" + fileName;
            }
            empObj.Name = objCommon.Name;
            empObj.IsActive = objCommon.IsActive;
            empObj.JoinDate = objCommon.JoinDate;
            empObj.ImageName = objCommon.ImgName;
            empObj.ImageUrl = FileApi.ImgName;


            List<Experience> list = JsonConvert.DeserializeObject<List<Experience>>(objCommon.Experiences);

            


            var existingExperiences = _db.Experiences.Where(e => e.EmployeeId == employeeId);
            _db.Experiences.RemoveRange(existingExperiences);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    Experience objEx = new Experience
                    {
                        EmployeeId = empObj.EmployeeId,
                        Title = item.Title,
                        Duration = item.Duration,
                    };

                    _db.Experiences.Add(objEx);
                }
            }
            await _db.SaveChangesAsync();
            return Ok("Employee updated successfully.");
        }


        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var empObj = await _db.Employees.FindAsync(employeeId);
            if (empObj == null)
            {
                return NotFound("Employee not found.");
            }
            _db.Employees.Remove(empObj);
            await _db.SaveChangesAsync();
            return Ok("Employee deleted successfully.");
        }
    }
            
}

