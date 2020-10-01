using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class OwnerForUpdateDto
    {
        [Required(ErrorMessage ="Name is required")]
        [StringLength(60, ErrorMessage ="Name can't be longer then 60 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Date of birth is required")]
        public DateTime DateofBirth { get; set; }

        [Required(ErrorMessage ="Address is required")]
        [StringLength(100,ErrorMessage = "Address cannot be longer then 100 characters")]
        public string Address { get; set; }
    }
}
