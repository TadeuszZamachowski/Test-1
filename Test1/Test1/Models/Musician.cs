using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test1.Controllers
{
    public class Musician
    {
        [Required(ErrorMessage = "Id is required")]
        public int IdMusician { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public List<string> Tracks { get; set; }
        public List<int> IdTracks { get; set; }

    }
}
