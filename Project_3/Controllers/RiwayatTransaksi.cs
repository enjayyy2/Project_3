using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_3.Models;

namespace Project_3.Controllers
{
    public class RiwayatTransaksiController : Controller
    {
        private mydatabase3Entities db = new mydatabase3Entities();

        //public ActionResult RiwayatTransaksi()
        //{
        //    var username = Session["username"]?.ToString();
        //    var riwayat = db.Transaksis
        //                    .Where(t => t.username_customer == username)
        //                    .OrderByDescending(t => t.date_time)
        //                    .ToList();
        //    return View(riwayat);
        //}
        public ActionResult Index()
        {
            var transaksis = db.Transaksis.ToList(); // Ambil data dari database
            return View(transaksis);
        }
    }
}