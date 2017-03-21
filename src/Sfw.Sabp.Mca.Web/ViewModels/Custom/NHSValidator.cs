using System;
using System.Linq;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public class NhsValidator : INhsValidator
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public NhsValidator(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public bool Valid(decimal? nhsNumber)
        {
            if (!nhsNumber.HasValue) return true;

            var nhsNumberString = Convert.ToString(nhsNumber);

            if (nhsNumberString.Length != 10) return false;

            var digPosn1 = Convert.ToInt32(nhsNumberString.Substring(0, 1)) * 10;
            var digPosn2 = Convert.ToInt32(nhsNumberString.Substring(1, 1)) * 9;
            var digPosn3 = Convert.ToInt32(nhsNumberString.Substring(2, 1)) * 8;
            var digPosn4 = Convert.ToInt32(nhsNumberString.Substring(3, 1)) * 7;
            var digPosn5 = Convert.ToInt32(nhsNumberString.Substring(4, 1)) * 6;
            var digPosn6 = Convert.ToInt32(nhsNumberString.Substring(5, 1)) * 5;
            var digPosn7 = Convert.ToInt32(nhsNumberString.Substring(6, 1)) * 4;
            var digPosn8 = Convert.ToInt32(nhsNumberString.Substring(7, 1)) * 3;
            var digPosn9 = Convert.ToInt32(nhsNumberString.Substring(8, 1)) * 2;

            var calCheckDig = (digPosn1 + digPosn2 + digPosn3 + digPosn4 + digPosn5 + digPosn6 + digPosn7 + digPosn8 + digPosn9);
            calCheckDig = (11 - (calCheckDig % 11));

            if (calCheckDig == 11) calCheckDig = 0;

            return Convert.ToInt32(nhsNumberString.Substring(9, 1)) == calCheckDig;
        }

        public bool Unique(decimal? nhsNumber)
        {
            if (!nhsNumber.HasValue) return true;

            var patient = _queryDispatcher.Dispatch<PatientByNhsNumberQuery, Patients>(new PatientByNhsNumberQuery
            {
                NhsNumber = nhsNumber.Value
            });

            return !patient.Items.Any();
        }
    }
}