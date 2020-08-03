using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace OnlineSearchInfotrack.Models
{
    public class SearchModel
    {

        [Required]
        public string SearchTerm { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
