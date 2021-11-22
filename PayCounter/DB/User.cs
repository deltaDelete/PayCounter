using System;
using System.Collections.Generic;
using System.Text;

namespace PayCounter.DB
{
    public class User
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public uint PINCode { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
