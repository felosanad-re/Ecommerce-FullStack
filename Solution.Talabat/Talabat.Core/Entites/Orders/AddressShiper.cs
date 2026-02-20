using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Orders
{
    public class AddressShiper
    {
        public AddressShiper()
        {
            
        }
        public AddressShiper(string firstName, string lastName, string city, string street)
        {
            FirstName = firstName;
            LastName = lastName;
            City = city;
            Street = street;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
    }
}
