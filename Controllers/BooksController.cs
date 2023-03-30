using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library_MVC_API.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace Library_MVC_API.Controllers
{
    public class BooksController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                List<Book> Book1 = new List<Book>();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("https://localhost:7230/api/Books\r\n");

                    if (Res.IsSuccessStatusCode)
                    {
                        var apiResponse = Res.Content.ReadAsStringAsync().Result;

                        Book1 = JsonConvert.DeserializeObject<List<Book>>(apiResponse);

                    }
                    return View(Book1);
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }
            
        }

        public async Task<IActionResult> Details(string id)
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                Book book = new Book();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        book = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }
        }
        public IActionResult Create()
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bid,Bname,Author,Jonour,Price,NoofCopies")] Book book)
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                if (ModelState.IsValid)
                {
                    Book b2 = new Book();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(book),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("https://localhost:7230/api/Books/", valuesToAdd))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                ViewBag.message = response.Content.ReadAsStringAsync().Result;
                                return View(book);
                            }
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            b2 = JsonConvert.DeserializeObject<Book>(apiResponse);
                        }
                    }                    
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }
        
        public async Task<IActionResult> Edit(string id)
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                Book book = new Book();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        book = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }                
                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Bid,Bname,Author,Jonour,Price,NoofCopies")] Book book)
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                if (id != book.Bid)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    Book b2 = new();
                    //string foodId = TempData["foodId"].ToString();
                    using (var httpClient = new HttpClient())
                    {
                        //string id = foodItemFromMVC.FoodId;
                        StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(book)
                 , Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PutAsync("https://localhost:7230/api/Books/" + id, valueToUpdate))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                ViewBag.mes = response.Content.ReadAsStringAsync().Result;
                                return View(book);
                            }
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            b2 = JsonConvert.DeserializeObject<Book>(apiResponse);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }
        public async Task<IActionResult> Delete(string id)
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                Book book = new Book();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        book = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                if (book == null)
                {
                    return NotFound();
                }

                return View(book);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                Book book = new Book();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        book = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                if (book != null)
                {
                    book.NoofCopies = 0;
                    Book b2 = new();;
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(book)
                 , Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PutAsync("https://localhost:7230/api/Books/" + id, valueToUpdate))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            b2 = JsonConvert.DeserializeObject<Book>(apiResponse);
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }

        [HttpGet]
        public IActionResult Entry()
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                ViewBag.name = HttpContext.Session.GetString("StaffName");
                if (ViewBag.name != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("AdminLogin", "Login");
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }
        public async Task<IActionResult> SalesDetails()
        {
            var c = HttpContext.Session.GetString("StaffName");
            if (c != null)
            {
                List<Booking> Booking1 = new List<Booking>();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("https://localhost:7230/api/Bookings\r\n");

                    if (Res.IsSuccessStatusCode)
                    {
                        var apiResponse = Res.Content.ReadAsStringAsync().Result;

                        Booking1 = JsonConvert.DeserializeObject<List<Booking>>(apiResponse);

                    }
                    //return View(Book1);
                }
                return View(Booking1);
            }
            else
            {
                return RedirectToAction("AdminLogin", "Login");
            }            
        }
    }
}
