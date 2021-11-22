using System;
using System.Collections.Generic;
using System.Text;

namespace PayCounter.DB
{
    public class Payment
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public uint Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Cost => Quantity * Price;
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
    