using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManager2.Models.CustomerModels
{
    public class LoginUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Mobile")]
        [Phone]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Mail")]
        public string Mail { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public CustomerInformation Customer { get; set; }

        [ForeignKey("DepartamentId")]
        public Departament Departament { get; set; }

        public ICollection<Departament> DepartamentsToManage { get; set; }

        [NotMapped]
        public string OldName { get; set; }

        [Display(Name = "Departament Name")]
        [NotMapped]
        public string DepartamentName { get; set; }
    }
}