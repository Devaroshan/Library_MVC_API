using Library_MVC_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Library.Controllers
{
    public class LibraryController : Controller
    {        
        public async Task<IActionResult> Books()
        {            
            ViewBag.name = HttpContext.Session.GetString("Name");
            if (ViewBag.name != null)
            {                               
                var b = HttpContext.Session.GetString("Book");
                IEnumerable<Book> bl = JsonConvert.DeserializeObject<IEnumerable<Book>>(b);
                return View(bl.ToList());                
            }
            else
            {
                return RedirectToAction("login","Login");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Buy(string? id)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                Book b = new Book();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        b = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                if (b == null)
                {
                    return NotFound();
                }
                return View(b);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Buy(string id,Book b)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                HttpContext.Session.SetString("flag", "0");
                Book bc = new Book();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        bc = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                if (bc != null)
                {
                    //var bc = db.Books.Find(res.Bid);
                    if (b.NoofCopies > bc.NoofCopies)
                    {
                        ViewBag.message = "Only " + bc.NoofCopies + " copies available";
                        return View(bc);
                    }
                    else if (b.NoofCopies <= 0)
                    {
                        ViewBag.message = "Number of copies should be a positive number";
                        return View(bc);
                    }
                    var c = HttpContext.Session.GetString("Client");
                    if (c != null)
                    {
                        Client cl = JsonConvert.DeserializeObject<Client>(c);
                        Booking bk1= new();
                        bk1.Bid = id;
                        bk1.Cid = cl.Cid;                        
                        using (var client = new HttpClient())
                        {
                            StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(bk1),
                              Encoding.UTF8, "application/json");
                            HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/Bookings/Client_Orders\r\n",valuesToAdd);
                            if (Res.IsSuccessStatusCode)
                            {
                                var apiResponse = Res.Content.ReadAsStringAsync().Result;
                                bk1 = JsonConvert.DeserializeObject<Booking>(apiResponse);
                            }
                        }
                        if (bk1.Status != null)
                        {
                            int? ex = bk1.NoofCopies;
                            bk1.NoofCopies += b.NoofCopies;
                            bk1.TotalPrice += (b.NoofCopies * bc.Price);
                            if (bc.NoofCopies < bk1.NoofCopies)
                            {
                                ViewBag.message = "Only " + (bc.NoofCopies - ex) + " copies available";
                                return View(bc);
                            }
                            bk1.Status = 0;
                            Booking b2 = new();
                            //string foodId = TempData["foodId"].ToString();
                            using (var httpClient = new HttpClient())
                            {
                                //string id = foodItemFromMVC.FoodId;
                                StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(bk1)
                         , Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PutAsync("https://localhost:7230/api/Bookings/" + bk1.Bkid, valueToUpdate))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    b2 = JsonConvert.DeserializeObject<Booking>(apiResponse);
                                }
                            }
                        }
                        else
                        {
                            Booking bk = new();
                            bk.Bid = id;
                            bk.Bname = bc.Bname;
                            bk.Author = bc.Author;
                            bk.Jonour = bc.Jonour;
                            bk.NoofCopies = b.NoofCopies;
                            bk.TotalPrice = (b.NoofCopies * bc.Price);
                            bk.Price = bc.Price;
                            bk.Cid = cl.Cid;
                            bk.Status = 0;
                            Booking b2 = new ();
                            using (var httpClient = new HttpClient())
                            {
                                StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(bk),
                              Encoding.UTF8, "application/json");

                                using (var response = await httpClient.PostAsync("https://localhost:7230/api/Bookings/", valuesToAdd))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    b2 = JsonConvert.DeserializeObject<Booking>(apiResponse);
                                }
                            }
                        }
                        Client cl1 = new ();
                        using (var httpClient = new HttpClient())
                        {
                            using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl.Cid))
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                            }
                        }
                        if (cl1.NoofBooks != null)
                        {
                            cl1.NoofBooks += b.NoofCopies;
                            cl1.TotalPrice += (b.NoofCopies * bc.Price);
                        }
                        else
                        {
                            cl1.NoofBooks = b.NoofCopies;
                            cl1.TotalPrice = (b.NoofCopies * bc.Price);
                        }
                        Client c2 = new();
                        using (var httpClient = new HttpClient())
                        {
                            StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(cl1)
                     , Encoding.UTF8, "application/json");
                            using (var response = await httpClient.PutAsync("https://localhost:7230/api/Clients/" + cl1.Cid, valueToUpdate))
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                c2 = JsonConvert.DeserializeObject<Client>(apiResponse);
                            }
                        }
                    }
                    TempData["success"] = "Book added to Cart";
                    return RedirectToAction("Favourite");
                }
                else
                {
                    ViewBag.message = "Enter valid Book ID";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }
        
        [HttpGet]
        public async Task<IActionResult> Cart() 
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                var c = HttpContext.Session.GetString("Client");
                Client cl = JsonConvert.DeserializeObject<Client>(c);
                /*var result = new List<Booking>();
                var res = new Client();*/
                if (cl == null)
                {
                    return RedirectToAction("login", "Login");
                }
                else
                {
                    Client cl1 = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl.Cid))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                        }
                    }
                    ViewBag.Price = cl1.TotalPrice;
                    var x = HttpContext.Session.GetString("flag");
                    if (x == "1")
                    {
                        ViewBag.message = HttpContext.Session.GetString("message");
                    }
                    List<Booking> Booking1 = new List<Booking>();
                    using (var client = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(cl1),
                              Encoding.UTF8, "application/json");
                        HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/Bookings/Unpaid_Orders\r\n",valuesToAdd);
                        if (Res.IsSuccessStatusCode)
                        {
                            var apiResponse = Res.Content.ReadAsStringAsync().Result;
                            Booking1 = JsonConvert.DeserializeObject<List<Booking>>(apiResponse);

                        }
                        //return View(Book1);
                    }
                    /*result = (from i in Booking1
                              where i.Cid == cl.Cid && i.Status == 0
                              select i).ToList();*/
                    if(Booking1.Count == 0)
                    {
                        ViewBag.empty = "Cart is empty";
                        return View();
                    }
                    return View(Booking1);
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
                       
        }
        public async Task<IActionResult> History()
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                var c = HttpContext.Session.GetString("Client");
                Client cl = JsonConvert.DeserializeObject<Client>(c);
                /*var result = new List<Booking>();
                var res = new Client();*/
                if (cl == null)
                {
                    return RedirectToAction("login", "Login");
                }
                else
                {
                    List<Booking> Booking1 = new List<Booking>();
                    using (var client = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(cl),
                              Encoding.UTF8, "application/json"); ;
                        HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/Bookings/Paid_Orders\r\n", valuesToAdd);
                        if (Res.IsSuccessStatusCode)
                        {
                            var apiResponse = Res.Content.ReadAsStringAsync().Result;
                            Booking1 = JsonConvert.DeserializeObject<List<Booking>>(apiResponse);
                        }
                    }
                    /*result = (from i in Booking1
                              where i.Cid == cl.Cid && i.Status == 1
                              select i).ToList();*/
                    if (Booking1.Count == 0)
                    {
                        ViewBag.empty = "There is no purchase history";
                        return View();
                    }
                    else
                    {
                        return View(Booking1);
                    }                    
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }

        public async Task<IActionResult> DeleteCart(int id)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                Booking booking = new ();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Bookings/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        booking = JsonConvert.DeserializeObject<Booking>(apiResponse);
                    }
                }
                if (booking == null)
                {
                    return NotFound();
                }

                return View(booking);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("DeleteCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                HttpContext.Session.SetString("flag", "0");
                Booking booking = new ();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Bookings/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        booking = JsonConvert.DeserializeObject<Booking>(apiResponse);
                    }
                }
                var c = HttpContext.Session.GetString("Client");
                Client cl = JsonConvert.DeserializeObject<Client>(c);
                if (booking != null)
                {
                    Client cl1 = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl.Cid))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                        }
                    }
                    cl1.TotalPrice -= booking.TotalPrice;
                    cl1.NoofBooks -= booking.NoofCopies;
                    //b.NoofCopies += booking.NoofCopies;
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.DeleteAsync("https://localhost:7230/api/Bookings/" + booking.Bkid))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                        }
                    };
                    Client c2 = new();
                    using (var httpClient = new HttpClient())
                    {
                        //string id = foodItemFromMVC.FoodId;
                        StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(cl1)
                 , Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PutAsync("https://localhost:7230/api/Clients/" + cl.Cid, valueToUpdate))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            c2 = JsonConvert.DeserializeObject<Client>(apiResponse);
                        }
                    }
                }
                return RedirectToAction("Cart");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Favourite()
        {
            
            ViewBag.name = HttpContext.Session.GetString("Name");
            if (ViewBag.name != null)
            {
                var b = HttpContext.Session.GetString("Favour");
                if (b == null)
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
                        //return View(Book1);
                    }
                    List<string?> A = new();
                    A.Add(null);
                    A.AddRange(Book1.Select(i => i.Author).Distinct().ToList());
                    List<string?> J = new();
                    J.Add(null);
                    J.AddRange(Book1.Select(i => i.Jonour).Distinct().ToList());
                    ViewBag.Author = new SelectList(A, A);
                    ViewBag.Jonour = new SelectList(J, J);
                    return View();
                }
                else
                {
                    Fav bl = JsonConvert.DeserializeObject<Fav>(b);
                    return await Favourite(bl);
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }            
        }
        [HttpPost]
        public async Task<IActionResult> Favourite(Fav b)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {                
                var result = new List<Book>();
                //List<Book> books = new List<Book>();
                using (var client = new HttpClient())
                {
                    StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(b),
                          Encoding.UTF8, "application/json");
                    HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/Books/Fav\r\n",valuesToAdd);
                    if (Res.IsSuccessStatusCode)
                    {
                        var apiResponse = Res.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<List<Book>>(apiResponse);
                    }
                    //return View(Book1);
                }
                HttpContext.Session.SetString("Book", JsonConvert.SerializeObject(result));
                HttpContext.Session.SetString("Favour", JsonConvert.SerializeObject(b));
                return RedirectToAction("Books");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }
        
        [HttpPost]
        public async Task<IActionResult> Payment(int id,Booking bk)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                decimal? payment = 0;
                ViewBag.Mes = "Payment is done";
                var c = HttpContext.Session.GetString("Client");
                Client cl = JsonConvert.DeserializeObject<Client>(c);
                
                Client cl1 = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl.Cid))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                    }
                }
                List<Booking> Booking1 = new List<Booking>();
                using (var client = new HttpClient())
                {
                    StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(cl1),
                              Encoding.UTF8, "application/json");
                    HttpResponseMessage Res = await client.PostAsync("https://localhost:7230/api/Bookings/Unpaid_Orders\r\n", valuesToAdd);
                    if (Res.IsSuccessStatusCode)
                    {
                        var apiResponse = Res.Content.ReadAsStringAsync().Result;
                        Booking1 = JsonConvert.DeserializeObject<List<Booking>>(apiResponse);
                    }
                }
                /*var res = (from i in Booking1
                           where i.Cid == cl.Cid && i.Status == 0
                           select i).ToList();*/
                if (Booking1.Count() != 0)
                {                    
                    foreach (var j in Booking1)
                    {
                        payment += j.TotalPrice;
                        //j.Status = 1;
                        Book b = new Book();
                        using (var httpClient = new HttpClient())
                        {
                            using (var response = await httpClient.GetAsync("https://localhost:7230/api/Books/" + j.Bid))
                            {
                                if (!response.IsSuccessStatusCode)
                                {
                                    ViewBag.Mes = "Payment not completed";
                                    return RedirectToAction("Cart");
                                }
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                b = JsonConvert.DeserializeObject<Book>(apiResponse);
                            }
                        }
                        if (b.NoofCopies >= j.NoofCopies)
                        {
                            b.NoofCopies -= j.NoofCopies;
                            Book b2 = new();
                            using (var httpClient = new HttpClient())
                            {
                                //string id = foodItemFromMVC.FoodId;
                                StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(b)
                         , Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PutAsync("https://localhost:7230/api/Books/" + b.Bid, valueToUpdate))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    b2 = JsonConvert.DeserializeObject<Book>(apiResponse);
                                }
                            }
                            j.Status = 1;
                            Booking bk2 = new();
                            using (var httpClient = new HttpClient())
                            {
                                //string id = foodItemFromMVC.FoodId;
                                StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(j)
                         , Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PutAsync("https://localhost:7230/api/Bookings/" + j.Bkid, valueToUpdate))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    bk2 = JsonConvert.DeserializeObject<Booking>(apiResponse);
                                }
                            }
                            cl1.TotalPrice -= j.TotalPrice;
                            Client c2 = new();
                            using (var httpClient = new HttpClient())
                            {
                                //string id = foodItemFromMVC.FoodId;
                                StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(cl1)
                         , Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PutAsync("https://localhost:7230/api/Clients/" + cl.Cid, valueToUpdate))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    c2 = JsonConvert.DeserializeObject<Client>(apiResponse);
                                }
                            }
                            HttpContext.Session.SetString("flag", "0");
                        }
                        else
                        {
                            HttpContext.Session.SetString("flag", "1");
                            HttpContext.Session.SetString("message", b.Bname+" Only " + (b.NoofCopies) + " copies available");
                            return RedirectToAction("Cart");
                        }                    
                    }
                }
                else 
                { 
                    ViewBag.Mes = "Payment not completed";
                    return RedirectToAction("cart");
                }
                TempData["success"] = "\u20B9"+payment + " Payment Successfull";
                return RedirectToAction("History");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            
        }
        
        public async Task<IActionResult> Details()
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                var c = HttpContext.Session.GetString("Client");
                Client cl = JsonConvert.DeserializeObject<Client>(c);
                if (cl != null)
                {
                    Client cl1 = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl.Cid))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                        }
                    }
                    return View(cl1);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }                
        }
        
        public async Task<IActionResult> edit(int id)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                Client cl1 = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                    }
                }
                return View(cl1);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,Client cl)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                if (cl == null)
                {
                    return View();
                }
                else
                {
                    Client cl1 = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + id))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            cl1 = JsonConvert.DeserializeObject<Client>(apiResponse);
                        }
                    }
                    //var cl1 = db.Clients.Find(id);
                    if (cl1 != null)
                    {
                        cl1.Cname = cl.Cname;
                        cl1.Caddress = cl.Caddress;
                        cl1.Userid = cl.Userid;
                        Client c2 = new();
                        //string foodId = TempData["foodId"].ToString();
                        using (var httpClient = new HttpClient())
                        {
                            //string id = foodItemFromMVC.FoodId;
                            StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(cl1)
                     , Encoding.UTF8, "application/json");
                            using (var response = await httpClient.PutAsync("https://localhost:7230/api/Clients/" + id, valueToUpdate))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    c2 = JsonConvert.DeserializeObject<Client>(apiResponse);
                                }
                                else
                                {
                                    ViewBag.email = response.Content.ReadAsStringAsync().Result;
                                    return View(cl);
                                }                                
                            }
                        }
                    }
                    //db.SaveChanges();
                    return RedirectToAction("Details", "Library");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public IActionResult Change_pwd(int id)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Change_pwd(Cpwd c)
        {
            var y = HttpContext.Session.GetString("Name");
            if (y != null)
            {
                var c1 = HttpContext.Session.GetString("Client");
                Client cl1 = JsonConvert.DeserializeObject<Client>(c1);
                Client cl = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl1.Cid))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        cl = JsonConvert.DeserializeObject<Client>(apiResponse);
                    }
                }
                if (cl != null)
                {
                    if (cl.Password == c.OldPassword)
                    {
                        if (ModelState.IsValid)
                        {
                            if (c.OldPassword != c.Password)
                            {
                                Client x = new();
                                using (var httpClient = new HttpClient())
                                {
                                    using (var response = await httpClient.GetAsync("https://localhost:7230/api/Clients/" + cl.Cid))
                                    {
                                        string apiResponse = await response.Content.ReadAsStringAsync();
                                        x = JsonConvert.DeserializeObject<Client>(apiResponse);
                                    }
                                }
                                //var x = db.Clients.Find(cl.Cid);
                                x.Password = c.Password;
                                x.CPassword = c.CPassword;
                                Client c2 = new();
                                //string foodId = TempData["foodId"].ToString();
                                using (var httpClient = new HttpClient())
                                {
                                    //string id = foodItemFromMVC.FoodId;
                                    StringContent valueToUpdate = new StringContent(JsonConvert.SerializeObject(x)
                             , Encoding.UTF8, "application/json");
                                    using (var response = await httpClient.PutAsync("https://localhost:7230/api/Clients/" + cl.Cid, valueToUpdate))
                                    {
                                        string apiResponse = await response.Content.ReadAsStringAsync();
                                        c2 = JsonConvert.DeserializeObject<Client>(apiResponse);
                                    }
                                }
                                //db.SaveChanges();
                                TempData["success"] = "Password Changed";
                                return RedirectToAction("Details");
                            }
                            else
                            {
                                ViewBag.mes = "Enter a new password";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.mes = "Password does not match";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.mes = "Password is wrong";
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("login", "Login");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public IActionResult Error()
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}
