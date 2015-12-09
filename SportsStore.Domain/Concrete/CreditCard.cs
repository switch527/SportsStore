using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class CreditCard
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter expiration date")]
        public DateTime Expiration { get; set; }

        [Required(ErrorMessage = "Please enter credit card type")]
        public string CC_Type { get; set; }

        [Required(ErrorMessage = "Please enter CVC code")]
        public int CVC { get; set; }

        public string AspnetUser_Id { get; set; }
    }
}

