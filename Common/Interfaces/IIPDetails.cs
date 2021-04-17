using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    interface IIPDetails
    {
        string City { get; set; }
        string Country { get; set; }
        string Continent { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
