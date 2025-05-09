using Project_3.Models;
using System.Web.Mvc;
using System.Linq;
using System;


public class TransaksiController : Controller
{
    private mydatabase3Entities db = new mydatabase3Entities();

    // GET
    public ActionResult Index()
    {
        if (Session["username"] == null)
        {
            return RedirectToAction("Login", "Account");
        }
        ViewBag.TransaksiList = db.Product_Transaksi.OrderByDescending(t => t.date_time).ToList();
        return View();
    }

    // POST
    [HttpPost]
    public ActionResult Index(Transaksi model)
    {
        if (ModelState.IsValid)
        {
            Transaksi newTrans = new Transaksi
            {
                username_customer = model.username_customer,
                ProductCode = model.ProductCode,
                Quantity = model.Quantity,
                SubTotal = model.SubTotal,
                date_time = DateTime.Now
            };

            db.Transaksis.Add(newTrans);
            db.SaveChanges();

            TempData["Success"] = "Transaksi berhasil disimpan!";
            return RedirectToAction("Index");
        }

        ViewBag.TransaksiList = db.Product_Transaksi.OrderByDescending(t => t.date_time).ToList();
        return View(model);
    }

    // AJAX method
    public JsonResult GetProductByCode(string productCode)
    {
        var product = db.Products.FirstOrDefault(p => p.ProductCode == productCode);

        if (product != null)
        {
            return Json(new
            {
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice
            }, JsonRequestBehavior.AllowGet);
        }

        return Json(null, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult RemoveProductTransaksi(int id)
    {
        var transaksi = db.Product_Transaksi.Find(id);
        if (transaksi != null)
        {
            db.Product_Transaksi.Remove(transaksi);
            db.SaveChanges();
            TempData["Success"] = "Transaksi berhasil dihapus!";
        }

        return RedirectToAction("Index");
    }
}
