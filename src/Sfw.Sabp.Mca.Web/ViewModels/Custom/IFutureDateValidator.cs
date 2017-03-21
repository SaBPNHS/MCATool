using System;

namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public interface IFutureDateValidator
    {
        bool Valid(DateTime? datetime);

        bool Valid(DateTime datetime);
    }
}