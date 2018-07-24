using MatriksCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

namespace MatriksCRM.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            if (HttpContext.Request.Cookies["cookie"] != null)
            {
                ViewBag.Email = HttpContext.Request.Cookies["cookie"]["Email"];
                return View();
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(Kullanici kullanici)
        {
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);
            
            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "KullaniciLogin"
            };
            
            command.Parameters.Add(new SqlParameter("@Email", kullanici.Email));
            command.Parameters.Add(new SqlParameter("@Sifre", kullanici.Sifre));

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            string Email = kullanici.Email;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Session.Add("ID", reader.GetInt32(0));
                    Session.Add("Isim", reader.GetString(3));
                    Session.Add("Soyad", reader.GetString(4));

                    if (kullanici.hatırla == true)
                    {
                        if (HttpContext.Request.Cookies["cookie"] == null)
                        {
                            HttpCookie cookie = new HttpCookie("cookie");
                            cookie["Isim"] = Session["Isim"].ToString();
                            cookie["Soyad"] = Session["Soyad"].ToString();
                            cookie["Email"] = Email;
                            cookie.Expires = DateTime.Now.AddDays(14);
                            Response.Cookies.Add(cookie);
                        }
                    }
                }
                connection.Close();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                connection.Close();
                return View();
            }
        }
    }
}