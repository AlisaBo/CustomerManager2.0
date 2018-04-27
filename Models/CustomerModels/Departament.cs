using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CustomerManager2.Models.CustomerModels
{
    public class Departament
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Departament Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public CustomerInformation Customer { get; set; }

        public ICollection<LoginUser> UsersInDepartament { get; set; }

        public LoginUser Manager { get; set; }

        [Display(Name = "Manager Name")]
        [NotMapped]
        public string ManagerName { get; set; }
    }
}