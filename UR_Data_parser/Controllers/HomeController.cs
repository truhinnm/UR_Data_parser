using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UR_Data_parser.Models;

namespace UR_Data_parser.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(DBSend model, IFormFile csvData)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Please complete all fields";
                return View();
            }
            if (csvData != null && csvData.Length > 0 && Path.GetExtension(csvData.FileName) == ".csv")
            {
                var connString = $"Host={model.Host};Port={model.Port};Database={model.Database};Username={model.Username};Password={model.Password}";
                try
                {
                    using (ApplicationContext db = new ApplicationContext(connString))
                    {
                        using (var reader = new StreamReader(csvData.OpenReadStream()))
                        {
                            reader.ReadLine();
                            string? line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] parts = line.Split(',');
                                User user = new User();
                                user.FirstName = parts[0];
                                user.LastName = parts[1];
                                user.Email = parts[2];
                                user.Gender = parts[3];
                                user.IPAddress = parts[4];
                                user.Age = int.Parse(parts[5]);
                                db.Users.Add(user);
                            }
                            db.SaveChanges();
                        }
                    }
                    ViewBag.Title = "UPLOADED!";
                    return View();
                }
                catch
                {
                    ViewBag.Title = "Wrong DB params or csv format differs from model";
                    return View();
                }
            }
            else
            {
                ViewBag.Title = "Wrong Format!";
                return View();
            }
        }
    }
}
