using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DeThamKhaoHK2_2018_2019.Models;
using Newtonsoft.Json;

namespace DeThamKhaoHK2_2018_2019.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cau01(BaiHat bh)
        {         
            if (bh != null && bh.MaBaiHat != null) 
            {
                StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;   
                int result = storeContext.ThemBaiHat(bh);
                ViewData["result"] = result;
            }            
            return View();
        }

        public IActionResult Cau02()
        {
            return View();
        }

        //[Route("/Home/Cau02_AJAX/{NgayThang}")]
        public String Cau02_AJAX(String NgayThang)
        {           
            StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;   
            List<CaSi> casis = storeContext.GetCaSiNgayThang(NgayThang);
            return JsonConvert.SerializeObject(casis);
        }

        public IActionResult Cau03()
        {
            StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;
            ViewData["nns"] = storeContext.GetNguoiNghes();
            return View();
        }

        public String Cau03_AJAX_GetPlaylist(String MaNN) 
        {
            StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;   
            List<PlayList> playlists = storeContext.GetPlaylistByMaNN(MaNN);
            return JsonConvert.SerializeObject(playlists);
        }

        public int Cau03_AJAX_DeletePlaylist(String MaPlayList) 
        {
            StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;   
            return storeContext.DeletePlaylistAndSongByMaPlayList(MaPlayList);
        }

        public IActionResult Cau04()
        {
            StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;   
            ViewData["casis"] = storeContext.GetCaSiHatTatCaBaiHat();
            return View();
        }

        public IActionResult Cau05()
        {
            StoreContext storeContext = HttpContext.RequestServices.GetService(typeof(StoreContext)) as StoreContext;   
            ViewData["bhslxhs"] = storeContext.GetBaiHatSoLanXuatHien();
            return View();
        }
    }
}
