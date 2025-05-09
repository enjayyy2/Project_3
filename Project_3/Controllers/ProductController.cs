using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_3.Models;
using System.IO;

namespace Project_3.Controllers
{
    public class ProductController : Controller
    {
        private mydatabase3Entities _context = new mydatabase3Entities();

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Generate kode unik: 3 digit random + menit + detik + tanggal (MMdd)
                var random = new Random();
                int rand = random.Next(100, 1000); // 3 digit
                string timePart = DateTime.Now.ToString("mmssMMdd");
                product.ProductCode = $"{rand}{timePart}";

                // Simpan gambar jika ada
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                    var path = Server.MapPath("~/Content/Images/" + fileName);
                    ImageFile.SaveAs(path);
                    product.ProductImage = fileName;
                }

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(string id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Products.Find(product.ProductCode);
                if (existing == null) return HttpNotFound();

                // Update basic fields
                existing.ProductName = product.ProductName;
                existing.ProductPrice = product.ProductPrice;
                existing.ProductStock = product.ProductStock;

                // Cek apakah ada gambar baru
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Server.MapPath("~/Content/Images/" + fileName);
                    Directory.CreateDirectory(Server.MapPath("~/Content/Images/")); // auto buat folder jika belum ada
                    ImageFile.SaveAs(path);

                    existing.ProductImage = fileName;
                }

                _context.Entry(existing).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // Hapus gambar dari folder (jika ada)
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    var path = Server.MapPath("~/Content/Images/" + product.ProductImage);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
