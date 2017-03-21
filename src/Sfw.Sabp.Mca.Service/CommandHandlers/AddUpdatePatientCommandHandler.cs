using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class AddUpdatePatientCommandHandler:ICommandHandler<AddUpdatePatientCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUpdatePatientCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(AddUpdatePatientCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var patient = _unitOfWork.Context.Set<Patient>().FirstOrDefault(x => x.PatientId == command.PatientId);

            if (patient != null)
            {
                patient.ClinicalSystemId = command.ClinicalSystemId;
                patient.NhsNumber = command.NhsNumber;
                patient.FirstName = command.FirstName;
                patient.LastName = command.LastName;
                patient.DateOfBirth = command.DateOfBirth;
                patient.GenderId = command.GenderId;
            }
            else
            {
                patient = new Patient
                {
                    PatientId = command.PatientId,
                    ClinicalSystemId = command.ClinicalSystemId,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    NhsNumber = command.NhsNumber,
                    DateOfBirth = command.DateOfBirth,
                    GenderId = command.GenderId
                };

                _unitOfWork.Context.Set<Patient>().Add(patient);    
            }
        }

    }
}
