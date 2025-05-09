using System.Linq;
using System.Web.Mvc;
using Project_3.Models; // sesuaikan namespace dengan projek kamu

public class AccountController : Controller
{
    private mydatabase3Entities db = new mydatabase3Entities();

    // GET: /Account/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        // Cek di tabel employe
        var employe = db.Employees.FirstOrDefault(e => e.username_employee == username && e.password_employee == password);
        if (employe != null)
        {
            Session["username"] = employe.username_employee;
            Session["role"] = employe.akses;

            if (employe.akses == "Admin")
                return RedirectToAction("Index", "Product");
            else if (employe.akses == "Cashier")
                return RedirectToAction("Index", "Transaksi");
        }

        // Cek di tabel customer
        var customer = db.Customers.FirstOrDefault(c => c.username_customer == username && c.password_customer == password);
        if (customer != null)
        {
            Session["username"] = customer.username_customer;
            Session["role"] = "customer";
            Session["id_customer"] = customer.id_Customer;

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Username atau password salah!";
        return View();
    }

    // GET: /Account/Register
    public ActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    public ActionResult Register(string email, string username, string password)
    {
        if (db.Customers.Any(c => c.username_customer == username))
        {
            ViewBag.Error = "Username sudah digunakan.";
            return View();
        }

        var customer = new Customer
        {
            email_customer = email,
            username_customer = username,
            password_customer = password
        };

        db.Customers.Add(customer);
        db.SaveChanges();

        return RedirectToAction("Login");
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Login");
    }
}
