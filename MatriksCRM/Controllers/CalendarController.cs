using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
//using FullCalendar.Models;
using MatriksCRM.Views.Home;

namespace FullCalendar.Controllers
{
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/

        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Veritabanındaki kayıtları takvime doldurur
        /// </summary>
        /// <param name="start">Başlangıç tarihi parametresi</param>
        /// <param name="end">Bitiş tarihi parametresi</param>
        /// <returns></returns>
        public JsonResult GetCalendarEvents(string start, string end)
        {
            List<CalendarEvent> eventItems = new List<CalendarEvent>();

            DBConnection connect = new DBConnection();

            try
            {
                connect.OpenConnection();

                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@StartDate", start));
                param.Add(new SqlParameter("@EndTime", end));
                var kullaniciid = Session["ID"].ToString();
                param.Add(new SqlParameter("@KullaniciID", kullaniciid));
                DataTable dt = connect.GetDataTable("Select * from tAgenda.tAgenda Where StartDate>=@StartDate and EndTime<=@EndTime and KullaniciID=@KullaniciID ", param);

                int i = 0, n = dt.Rows.Count;
                for (i = 0; i < n; i++)
                {
                    DataRow dr = dt.Rows[i];

                    CalendarEvent item = new CalendarEvent();
                    item.id = int.Parse(dr["NotID"].ToString());
                    item.title = dr["Title"].ToString();
                    item.start = string.Format("{0:s}", dr["StartDate"]);
                    item.end = string.Format("{0:s}", dr["EndTime"]);
                    item.color = dr["Color"].ToString();
                    item.allDay = bool.Parse(dr["AllDay"].ToString());
                    eventItems.Add(item);
                }

                return Json(eventItems, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        /// <summary>
        /// Veritabanına Ekleme yapar veya varolan kayıtta güncelleme yapar
        /// </summary>
        /// <param name="item">İşlem yapılması istenen event nesnesi</param>
        /// <returns></returns>
        public JsonResult AddOrEditItem(CalendarEvent item)
        {
            DBConnection connect = new DBConnection();
            try
            {
                var kullaniciid = Session["ID"].ToString();
                connect.OpenConnection();
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@KullaniciID", kullaniciid));
                param.Add(new SqlParameter("@Title", item.title));
                param.Add(new SqlParameter("@StartDate", item.start));
                param.Add(new SqlParameter("@EndTime", item.end));
                param.Add(new SqlParameter("@Color", item.color));
                param.Add(new SqlParameter("@AllDay", item.allDay));
                param.Add(new SqlParameter("@NOTLAR", item.note));
                string sql = "Insert into tAgenda.tAgenda(KullaniciID,Title,StartDate,EndTime,Color,AllDay,NOTLAR) ";
                sql += "Values(@KullaniciID,@Title,@StartDate,@EndTime,@Color,@AllDay,@NOTLAR) ";

                connect.RunSqlCommand(sql, param);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        //Random random = new Random();
        //public void AddItem(List<CalendarEvent> eventItems)
        //{
        //    CalendarEvent item = new CalendarEvent();

        //    DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, random.Next(1, 30));

        //    item.id = random.Next(1, 100);
        //    item.start = startDate.ToString("s");
        //    item.end = startDate.AddDays(random.Next(1, 5)).ToString("s");
        //    item.allDay = true;
        //    item.color = "orange";
        //    item.title = "Lorem ipsum dolor sit amet. " + item.id;
        //    eventItems.Add(item);
        //}


        /// <summary>
        /// Veritabanından kayıt siler
        /// </summary>
        /// <param name="id">Silinmesi istenilen kaydın id değeri</param>
        /// <returns></returns>
        public JsonResult DeleteItem(int id)
        {
            DBConnection connect = new DBConnection();
            try
            {
                connect.OpenConnection();
                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@NotID", id));

                string sql = "Delete tAgenda.tAgenda Where NotID=@NotID";

                connect.RunSqlCommand(sql, param);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                connect.CloseConnection();
            }

        }

        public JsonResult GetNote(string id){
            List<CalendarEvent> eventItems = new List<CalendarEvent>();

            DBConnection connect = new DBConnection();

            try
            {
                connect.OpenConnection();

                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@NotID", Convert.ToInt32(id)));
                var kullaniciid = Session["ID"].ToString();
                DataTable dt = connect.GetDataTable("Select * from tAgenda.tAgenda Where NotID=@NotID ", param);

                int i = 0, n = dt.Rows.Count;
                for (i = 0; i < n; i++)
                {
                    DataRow dr = dt.Rows[i];

                    CalendarEvent item = new CalendarEvent();
                    item.id = int.Parse(dr["NotID"].ToString());
                    item.title = dr["Title"].ToString();
                    item.note = dr["NOTLAR"].ToString();
                    eventItems.Add(item);
                }

                return Json(eventItems, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                connect.CloseConnection();
            }
  
        }
    }
}