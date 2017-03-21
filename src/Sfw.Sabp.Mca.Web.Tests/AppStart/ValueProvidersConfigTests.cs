using System.Web.Mvc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ValueProviders;

namespace Sfw.Sabp.Mca.Web.Tests.AppStart
{
    [TestClass]
    public class ValueProvidersConfigTests
    {
        [TestMethod]
        public void RegisterValueProviders_DateOfBirthValueProviderFactoryShouldBeRegistered()
        {
            ValueProvidersConfig.RegisterValueProviders();
            ValueProviderFactories.Factories.Should()
                .Contain(x => x.GetType() == typeof (DateOfBirthCustomValueProviderFactory));
        }
    }
}