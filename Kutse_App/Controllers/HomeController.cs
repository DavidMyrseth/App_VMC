using Kutse_App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Data.Entity;

namespace Kutse_App.Controllers
{
    public class HomeController : Controller
    {

        private static readonly Dictionary<int, string> Pidu = new Dictionary<int, string>
        {
            {1, "Head uut aastat!"},
            {2, "Head Eesti iseseisvuspäeva!"},
            {12, "Haid jõule"}
        };

        public ActionResult Index()
        {
            string greeting;



            int month = DateTime.Now.Month;
            int hour = DateTime.Now.Hour;

            if (hour >= 6 && hour < 12)
            {
                greeting = "Tere hommikust!";
            }
            else if (hour >= 12 && hour < 18)
            {
                greeting = "Tere tere!";
            }
            else if (hour >= 18 && hour < 21)
            {
                greeting = "Tere õhtust!";
            }
            else
            {
                greeting = "Head ööd!";
            }

            ViewBag.Greeting = greeting;
            string holidayMessage = Pidu.ContainsKey(month) ? Pidu[month] : "";

            ViewBag.Greeting = greeting + (string.IsNullOrEmpty(holidayMessage) ? "" : " " + holidayMessage);
            ViewBag.Message = "Ootan sind minu peole!";
            return View();
        }

        [HttpGet]

        public ActionResult Ankeet()
        {
            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name");

            var guest = new Guest();

            if (!User.IsInRole("Admin"))
            {
                guest.WillAttend = false;
            }

            return View(guest);
        }

        [HttpPost]

        public ViewResult Ankeet(Guest guest)
        {
            E_mail(guest);

            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name");

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                    }
                }
                return View(guest);
            }

            try
            {
                db.Guests.Add(guest);
                db.SaveChanges();

                return View("Holidays", db.Holidays.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                ModelState.AddModelError("", "Registration failed: " + ex.Message);
                return View(guest);
            }
        }
        public void E_mail(Guest guest)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "david.mirsetSSS@gmail.com";
                WebMail.Password = "lqwr makk gdoc ktoa";
                WebMail.From = "david.mirsetSSS@gmail.com";
                WebMail.Send(guest.Email, " Vastus kutsele ", guest.Name + " vastas " + ((guest.WillAttend ?? false ? " tuleb peole" : " ei tule saatnud")));
                ViewBag.Message = "Kiri on saatnud!";
            }
            catch (Exception)
            {
                ViewBag.Message = "Mul on kahju! Ei saa kirja saada!!!";
            }
        }

        [HttpPost]
        public ActionResult Meeldetuletus(Guest guest, string Meeldetuletus)
        {
            if (!string.IsNullOrEmpty(Meeldetuletus))
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "david.mirsetSSS @gmail.com";
                    WebMail.Password = "lqwr makk gdoc ktoa";
                    WebMail.From = "david.mirsetSSS@gmail.com";

                    WebMail.Send(guest.Email, "Meeldetuletus", guest.Name + ", ara unusta. Toimub 20.11.25",
                    null, "david.mirsetSSS@gmail.com",
                    filesToAttach: new String[] { Path.Combine(Server.MapPath("~/Images/"), Path.GetFileName("yippy.jpg ")) }
                   );

                    ViewBag.Message = "Kutse saadetud!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Tekkis viga kutse saatmisel: " + ex.Message;
                }
            }

            return View("Aitäh", guest);
        }
        GuestContext db = new GuestContext();

        [HttpPost]
        public ActionResult SendReminder(string email)
        {
            var guest = db.Guests.Include(g => g.Holiday).FirstOrDefault(g => g.Email == email);

            if (guest == null)
            {
                ViewBag.Message = "Ühtegi registreeringut selle e-posti aadressiga ei leitud.";
                return View("MyRegistration", new List<Guest>());
            }

            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "david.mirsetSSS@gmail.com";
                WebMail.Password = "lqwr makk gdoc ktoa";
                WebMail.From = "david.mirsetSSS@gmail.com";

                WebMail.Send(
                    guest.Email,
                    "Meeldetuletus sündmusest",
                    $"{guest.Name}, ärge unustage sündmust! See toimub {guest.Holiday?.Name ?? "määramata kuupäeval"}!"
                );

                ViewBag.Message = "Meeldetuletus saadetud!";
            }
            catch (Exception)
            {
                ViewBag.Message = "Kahjuks ei õnnestunud meeldetuletust saata!";
            }

            return View("MyRegistration", new List<Guest> { guest });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Guests()
        {
            var holidays = db.Holidays.ToList();

            var guests = db.Guests.Include(g => g.Holiday).ToList();

            ViewBag.Holidays = new SelectList(holidays, "Id", "Name");

            return View(guests);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Guests.Add(guest);
                db.SaveChanges();
                return RedirectToAction("Guests");
            }
            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name");
            return View(guest);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Guest g = db.Guests.Find(id);
            if (g == null)
            {
                return HttpNotFound();
            }
            return View(g);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Guest g = db.Guests.Find(id);
            if (g == null)
            {
                return HttpNotFound();
            }
            db.Guests.Remove(g);
            db.SaveChanges();
            return RedirectToAction("Guests");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }

            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name", guest.HolidayId);
            return View(guest);
        }

        [HttpGet]
        public ActionResult Edit3(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }

            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name", guest.HolidayId);
            return View(guest);
        }

        [HttpPost, ActionName("Edit3")]
        public ActionResult EditConfirmed3(Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guest).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Guests");
            }
            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name", guest.HolidayId);
            return View(guest);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirmed(Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guest).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Guests");
            }
            var holidays = db.Holidays.ToList();
            ViewBag.Holidays = new SelectList(holidays, "Id", "Name", guest.HolidayId);
            return View(guest);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult WillAttendGuests()
        {
            var guests = db.Guests.Where(g => g.WillAttend == true).ToList();
            return View("Guests", guests);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NotAttendingGuests()
        {
            var guests = db.Guests.Where(g => g.WillAttend == false).ToList();
            return View("Guests", guests);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AllGuests()
        {
            var guests = db.Guests.ToList();
            return View("Guests", guests);
        }
        public ActionResult Holidays()
        {
            IEnumerable<Holiday> holidays = db.Holidays;
            return View(holidays);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create2()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create2(Holiday holiday)
        {
            db.Holidays.Add(holiday);
            db.SaveChanges();
            return RedirectToAction("Holidays");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete2(int id)
        {
            Holiday h = db.Holidays.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            return View(h);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete2")]
        public ActionResult DeleteConfirmed2(int id)
        {
            Holiday h = db.Holidays.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            db.Holidays.Remove(h);
            db.SaveChanges();
            return RedirectToAction("Holidays");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit2(int? id)
        {
            Holiday h = db.Holidays.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            return View(h);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit2")]
        public ActionResult EditConfirmed2(Holiday holiday)
        {
            db.Entry(holiday).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Holidays");
        }

        [HttpGet]
        public ActionResult MyRegistration(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Palun sisestage oma e-posti aadress.";
                return View(new List<Guest>());
            }

            var guest = db.Guests.Include(g => g.Holiday).FirstOrDefault(g => g.Email == email);

            if (guest == null)
            {
                ViewBag.Message = "Ühtegi registreeringut selle e-posti aadressiga ei leitud.";
                return View(new List<Guest>());
            }

            return View(new List<Guest> { guest });
        }
    }
}
