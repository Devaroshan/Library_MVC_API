using Library_MVC_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Library_MVC_API.Controllers
{
    public class LoginController : Controller
    {        
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> login(login l)
        {
            try
            {
                Client f = new();
                using (var client = new HttpClient())
                {
                    StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(l),
                          Encoding.UTF8, "application/json");
                    HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/Clients/Auth\r\n", valuesToAdd);
                    if (Res.IsSuccessStatusCode)
                    {
                        var apiResponse = Res.Content.ReadAsStringAsync().Result;
                        f = JsonConvert.DeserializeObject<Client>(apiResponse);
                        HttpContext.Session.SetString("Client", JsonConvert.SerializeObject(f));
                        HttpContext.Session.SetString("Name", f.Cname);
                        TempData["success"] = "You have been logged in!";
                        return RedirectToAction("Favourite", "Library");
                    }
                    else
                    {
                        ViewBag.message = Res.Content.ReadAsStringAsync().Result;
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.error = "Server Down try again later";
                return RedirectToAction("Error", "Library");
            }
        }

        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(Client c)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Client c2 = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(c),
                      Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync("https://localhost:7230/api/Clients/", valuesToAdd);
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                c2 = JsonConvert.DeserializeObject<Client>(apiResponse);
                            }
                            else
                            {
                                ViewBag.message = response.Content.ReadAsStringAsync().Result;
                                return View(c);
                            }
                        }
                    }
                }
                else
                {
                    return View(c);
                }
            }
            catch (Exception)
            {
                ViewBag.message = "Input exceed limits";
            }
            TempData["success"] = "Registration Successful";
            return RedirectToAction("login");
        }
        
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AdminLogin(login c)
        {
            LibStaff f = new();
            try
            {                
                using (var client = new HttpClient())
                {
                    StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(c),
                          Encoding.UTF8, "application/json");
                    HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/LibStaffs/Auth\r\n", valuesToAdd);
                    if (Res.IsSuccessStatusCode)
                    {
                        var apiResponse = Res.Content.ReadAsStringAsync().Result;
                        f = JsonConvert.DeserializeObject<LibStaff>(apiResponse);
                        HttpContext.Session.SetString("StaffName", f.Lsname);
                        return RedirectToAction("Entry", "Books");
                    }
                    else
                    {
                        ViewBag.message = Res.Content.ReadAsStringAsync().Result;
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.error = "Server Down try again later";
                return RedirectToAction("Error", "Library");
            }
        }
        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin");
        }
    }
}
