using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Web.Attributes;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    public class PatientViewModel
    {
        public Guid PatientId { get; set; }

        [ClinicalSystemIdDisplay]
        public string ClinicalSystemId { get; set; }

        [Display(Name = "NHS Number")]
        public decimal? NhsNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Date of Birth")]
        public DateOfBirthViewModel DateOfBirthViewModel { get; set; }
                
        [Display(Name = "Gender")]
        public int GenderId { get; set; }                

        public IEnumerable<SelectListItem> Genders { get; set; }

        public string SelectedGender { get; set; }
    }
}