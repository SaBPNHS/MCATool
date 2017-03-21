using System.Web.Mvc;
using Sfw.Sabp.Mca.Web;
using Sfw.Sabp.Mca.Web.ValueProviders;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ValueProvidersConfig), "RegisterValueProviders")]
namespace Sfw.Sabp.Mca.Web
{
    public class ValueProvidersConfig
    {
        public static void RegisterValueProviders()
        {
            ValueProviderFactories.Factories.Add(new DateOfBirthCustomValueProviderFactory());
        }
    }
}