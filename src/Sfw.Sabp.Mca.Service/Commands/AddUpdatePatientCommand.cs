using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class AddUpdatePatientCommand:ICommand
    {
        public Guid PatientId { get; set; }
        
        public string ClinicalSystemId { get; set; }
                
        public decimal? NhsNumber { get; set; }
                
        public string FirstName { get; set; }
                
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int GenderId { get; set; }
       
    }
}
