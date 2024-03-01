using Ajax_crud.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Ajax_crud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly HttpClient _client;
        readonly Uri baseAddress = new("http://localhost:24254/api");

        public HomeController()
        {
            _client = new HttpClient
            {
                BaseAddress = baseAddress
            };
        }
        //public static List<PersonModel> Persons = [];
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            //Persons =
            //[
            //	new PersonModel()
            //	{
            //		Id = 1,
            //		Email = "ankush1802@outlook.com",
            //		Contact = "1234567890",
            //		Name = "Ankush"
            //	},
            //	new PersonModel()
            //	{
            //		Id = 2,
            //		Email = "rohit@outlook.com",
            //		Contact = "1234567890",
            //		Name = "Rohit"
            //	},
            //	new PersonModel()
            //	{
            //		Id = 3,
            //		Email = "sunny@outlook.com",
            //		Contact = "1234567890",
            //		Name = "Sunny"
            //	},
            //	new PersonModel()
            //	{
            //		Id = 4,
            //		Email = "amit@outlook.com",
            //		Contact = "1234567890",
            //		Name = "Amit"
            //	},
            //];
        }

        [HttpGet]
        public List<PersonModel> GetPerson()
        {
            List<PersonModel> Persons = [];
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Person/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);
                var dataOfObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataOfObject, Formatting.Indented);
                Persons = JsonConvert.DeserializeObject<List<PersonModel>>(extractedDataJson);
            }
            return Persons;
        }

        public IActionResult Index()
        {
            return View(GetPerson());
        }
        [HttpGet]
        public JsonResult GetDetailsById(int id)
        {
            List<PersonModel> Persons = GetPerson();
            var Person = Persons.Where(d => d.Id.Equals(id)).FirstOrDefault();
            JsonResponseViewModel model = new();
            if (Person != null)
            {
                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(Person);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult InsertPerson(IFormCollection formcollection)
        {
            MultipartFormDataContent Person = new()
            {
                { new StringContent(formcollection["name"]), "Name" },
                { new StringContent(formcollection["contact"]), "Contact" },
                { new StringContent(formcollection["email"]), "Email" }
            };
            //MAKE DB CALL and handle the response
            HttpResponseMessage response = _client.PostAsync($"{_client.BaseAddress}/Person/InsertPerson", Person).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Person Saved Successfully";
            }
            else
            {
                TempData["Message"] = "Failed to save Person";
            }
            return Json(response);
        }
        [HttpPut]
        public JsonResult UpdatePerson(IFormCollection formcollection)
        {
            PersonModel Person = new()
            {
                Id = int.Parse(formcollection["id"]),
                Email = formcollection["email"],
                Contact = formcollection["contact"],
                Name = formcollection["name"]
            };
            JsonResponseViewModel model = new();
            //MAKE DB CALL and handle the response
            if (Person != null)
            {
                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(Person);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }
        [HttpDelete]
        public JsonResult DeletePerson(IFormCollection formcollection)
        {
            PersonModel Person = new()
            {
                Id = int.Parse(formcollection["id"])
            };
            JsonResponseViewModel model = new();
            //MAKE DB CALL and handle the response
            if (Person != null)
            {
                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(Person);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }
    }
}
