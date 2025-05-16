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

    // POST: Simpan transaksi & hapus 1 record di Product_Transaksi
    [HttpPost]
    public ActionResult Index(Transaksi model, int idTransaksi)
    {
        if (ModelState.IsValid)
        {
            // Simpan data ke tabel Transaksi
            Transaksi newTrans = new Transaksi
            {
                username_customer = model.username_customer,
                ProductCode = model.ProductCode,
                Quantity = model.Quantity,
                SubTotal = model.SubTotal,
                date_time = DateTime.Now
            };

            db.Transaksis.Add(newTrans);

            // Hapus record di Product_Transaksi berdasar idTransaksi
            var toDelete = db.Product_Transaksi.FirstOrDefault(pt => pt.id_ProductTransaksi == idTransaksi);
            if (toDelete != null)
            {
                db.Product_Transaksi.Remove(toDelete);
            }

            db.SaveChanges();

            TempData["Success"] = "Transaksi berhasil disimpan dan data riwayat berhasil dihapus!";
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

    /*
    // Uncomment jika butuh fitur hapus transaksi secara manual
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
    */

    public ActionResult RiwayatTransaksi()
    {
        var username = Session["username"]?.ToString();
        var riwayat = db.Transaksis
                        .Where(t => t.username_customer == username)
                        .OrderByDescending(t => t.date_time)
                        .ToList();
        return View(riwayat);
    }

    public JsonResult GetProductTransaksiById(int id)
    {
        var data = (from pt in db.Product_Transaksi
                    join p in db.Products on pt.ProductCode equals p.ProductCode
                    where pt.id_ProductTransaksi == id
                    select new
                    {
                        pt.username_customer,
                        pt.ProductCode,
                        p.ProductName,
                        p.ProductPrice,
                        pt.Quantity,
                        SubTotal = pt.Quantity * p.ProductPrice
                    }).FirstOrDefault();

        return Json(data, JsonRequestBehavior.AllowGet);
    }
}
