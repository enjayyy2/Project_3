using Project_3.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Project_3.Controllers
{
    public class CartController : Controller
    {
        private mydatabase3Entities db = new mydatabase3Entities();

        // GET: Cart
        public ActionResult Index()
        {
            string username = Session["Username"]?.ToString();

            var cartItems = db.Carts
                              .Include(c => c.Product)
                              .Where(c => c.username_customer == username)
                              .Select(c => new CartDisplay
                              {
                                  ProductName = c.Product.ProductName,
                                  ProductCode = c.ProductCode,
                                  Quantity = c.Quantity
                              })
                              .ToList();

            return View(cartItems);
        }



        [HttpPost]
        public ActionResult AddToCart(string productCode, int quantity = 1)
        {
            string username = Session["Username"]?.ToString();

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingItem = db.Carts.FirstOrDefault(c => c.username_customer == username && c.ProductCode == productCode);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Cart newItem = new Cart
                {
                    username_customer = username,
                    ProductCode = productCode,
                    Quantity = quantity
                };
                db.Carts.Add(newItem);
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(string productCode)
        {
            string username = Session["Username"]?.ToString();

            var item = db.Carts.FirstOrDefault(c => c.username_customer == username && c.ProductCode == productCode);
            if (item != null)
            {
                db.Carts.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Checkout()
        {
            var cartItems = db.Carts.ToList();

            if (cartItems.Any())
            {
                foreach (var item in cartItems)
                {
                    var transaksi = new Product_Transaksi
                    {
                        username_customer = item.username_customer,
                        ProductCode = item.ProductCode,
                        Quantity = item.Quantity,
                        date_time = DateTime.Now
                    };

                    db.Product_Transaksi.Add(transaksi);
                }

                db.Carts.RemoveRange(cartItems); // Hapus semua isi cart
                db.SaveChanges();

                TempData["Success"] = "Checkout berhasil, data telah dimasukkan ke dalam transaksi!";
            }
            else
            {
                TempData["Error"] = "Keranjang kosong!";
            }

            return RedirectToAction("Index");
        }

    }
}
