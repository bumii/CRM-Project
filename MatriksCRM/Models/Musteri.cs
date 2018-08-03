using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatriksCRM.Models
{
    public class Musteri
    {
        public int MusteriID { get; set; }
        public string FirmaAdi { get; set; }
        public string YetkiliAd { get; set; }
        public string YetkiliPoz { get; set; }
        public string MusteriTel { get; set; }
        public string MusteriMail { get; set; }
        public string MusteriDurum { get; set; }
    }
}