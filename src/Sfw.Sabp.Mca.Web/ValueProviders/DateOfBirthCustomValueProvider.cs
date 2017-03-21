using System;
using System.Globalization;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Core.Constants;

namespace Sfw.Sabp.Mca.Web.ValueProviders
{
    public class DateOfBirthCustomValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (IsPersonController(controllerContext) &&
                IsPersonControllerEditAction(controllerContext) && 
                IsPostRequest(controllerContext))
            {
                return new DateOfBirthCustomValueProvider(controllerContext);
            }

            return null;
        }
       
        #region private

        private bool IsPostRequest(ControllerContext controllerContext)
        {
            return controllerContext.HttpContext.Request.HttpMethod == HttpVerbs.Post.ToString().ToUpper();
        }

        private bool IsPersonController(ControllerContext controllerContext)
        {
            return controllerContext.RouteData.Values["controller"].ToString() == MVC.Person.Name;
        }

        private bool IsPersonControllerEditAction(ControllerContext controllerContext)
        {
            return controllerContext.RouteData.Values["action"].ToString() == MVC.Person.ActionNames.Edit;
        }

        #endregion
    }

    public class DateOfBirthCustomValueProvider : IValueProvider
    {
        private readonly ControllerContext _controllerContext;

        public DateOfBirthCustomValueProvider(ControllerContext controllerContext)
        {
            _controllerContext = controllerContext;
        }

        public bool ContainsPrefix(string prefix)
        {
            return false;
        }

        public ValueProviderResult GetValue(string key)
        {
            if (key == ApplicationStringConstants.DateOfBirthValueKey)
            {
                var day = Convert.ToInt32(_controllerContext.HttpContext.Request.Form[ApplicationStringConstants.DateofBirthViewModelDayKey]);
                var month = Convert.ToInt32(_controllerContext.HttpContext.Request.Form[ApplicationStringConstants.DateofBirthViewModelMonthKey]);
                var year = Convert.ToInt32(_controllerContext.HttpContext.Request.Form[ApplicationStringConstants.DateofBirthViewModelYearKey]);

                var date = new DateTime(year, month, day);

                return new ValueProviderResult(date, date.ToShortDateString(), CultureInfo.CurrentCulture);
            }
            return null;
        }
    }
}