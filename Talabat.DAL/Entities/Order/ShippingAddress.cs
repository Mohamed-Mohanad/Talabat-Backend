using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities.Order
{
    public class ShippingAddress
    {
        public ShippingAddress()
        {
        }

        public ShippingAddress(string fistName, string lastName, string country, string city, 
                        string street, string zipCode)
        {
            FistName = fistName;
            LastName = lastName;
            Country = country;
            City = city;
            Street = street;
            ZipCode = zipCode;
        }

        public string? FistName { get; set; }
        public string? LastName  { get; set; }
        public string? Country  { get; set; }
        public string? City  { get; set; }
        public string? Street  { get; set; }
        public string? ZipCode  { get; set; }
    }
}
