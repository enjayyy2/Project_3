using System.Linq;
using System.Web.Mvc;
using Project_3.Models; // Ganti dengan namespace project Anda

namespace WebClothes.Controllers
{
    public class HomeController : Controller
    {
        private mydatabase3Entities db = new mydatabase3Entities();

        public ActionResult Index()
        {
            var products = db.Products.ToList(); // Ambil semua produk
            return View(products);
        }
    }
}
