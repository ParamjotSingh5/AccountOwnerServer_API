using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class AccountForUpdateDto
    {
        [Required(ErrorMessage = "Account Type is required")]
        public string AccountType { get; set; }
        [Required(ErrorMessage = "OwnerId is required")]
        public Guid ownerid { get; set; }
    }
}
