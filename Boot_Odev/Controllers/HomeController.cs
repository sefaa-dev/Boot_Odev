using Boot_Odev.Models;
using Boot_Odev.Models;
using Boot_Odev.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;




namespace Boot_Odev.Controllers
{
    public class HomeController : Controller
    {
        NORTHWNDContext context = new NORTHWNDContext();

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            var users = context.Employees.ToList();
            List<LoginViewModel> loginViewModels = new List<LoginViewModel>();

            // contextden userName ve password listesi olusturuldu
            for (int i = 0; i < users.Count(); i++)
            {
                loginViewModels.Add(new LoginViewModel
                {
                    Id = users[i].EmployeeId,
                    userName = $"{users[i].FirstName.ToLower()}.{users[i].LastName.ToLower()}",
                    password = $"{users[i].LastName.ToLower()}{DateTime.Parse(users[i].BirthDate.ToString()).Year}"
                });
            }
            var user = loginViewModels.FirstOrDefault(x => x.userName == login.userName.ToLower() && x.password == login.password.ToLower());

            // kişi eşleşmesi kontrol edildi
            if (user == null)
            {
                return View(login);
            }
            return RedirectToAction("GetOrder", new { Id = user.Id });
        }

        public IActionResult GetOrder(int Id)
        {
            var query = (from e in context.Employees
                         join o in context.Orders on e.EmployeeId equals o.EmployeeId
                         join od in context.OrderDetails on o.OrderId equals od.OrderId
                         join c in context.Customers on o.CustomerId equals c.CustomerId
                         where e.EmployeeId == Id                      
                         select new
                         { c.CompanyName, o.OrderDate, o.OrderId, price = od.Quantity * (float)od.UnitPrice * (1 - od.Discount) }).ToList();

        


            List<TableViewModel> tableViewModels = new List<TableViewModel>();

            foreach (var item in query)
            {
                tableViewModels.Add(new TableViewModel
                {
                    CompanyName = item.CompanyName,
                    OrderDate = $"{DateTime.Parse(item.OrderDate.ToString()).Day}/{DateTime.Parse(item.OrderDate.ToString()).Month}/{DateTime.Parse(item.OrderDate.ToString()).Year}",
                    OrderId = item.OrderId,
                    Price = item.price
                });
            }

            return View(tableViewModels);
        }

    }

}
