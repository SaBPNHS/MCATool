using System;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IDateOfBirthBuilder
    {
        DateOfBirthViewModel BuildDateOfBirthViewModel(DateTime? selectedDate);
    }
}