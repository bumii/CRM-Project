using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatriksCRM.Models
{
    public class Kullanici
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string Isim { get; set; }
        public string Soyad { get; set; }
        public string Telefon { get; set; }
        public DateTime DogumTarihi { get; set; }
    }
}