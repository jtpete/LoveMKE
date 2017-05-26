using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using LoveMKERegistration;
using LoveMKERegistration.Controllers;
using LoveMKERegistration.Models;
using System.Threading.Tasks;

namespace LoveMKERegistration.Tests.Controllers
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SignupControllerTest
    {
        [TestMethod]
        public async Task TestSignupView1()
        {
            //Arrange
            var controller = new SignupViewController();

            //Act
            var result = await controller.Signup("LoveMKE") as ViewResult;
            //Assert
            Assert.AreEqual("", result.ViewName);
        }
        [TestMethod]
        public async Task TestSignupView2()
        {
            //Arrange
            var controller = new SignupViewController();
            var signupModel = new SignupViewModel();
            string[] peopleToAdd = new string[] { "" };

            //Act
            var result = (RedirectToRouteResult) await controller.AddIndividuals(signupModel, peopleToAdd);

            //Assert
            Assert.AreEqual("TShirts", result.RouteValues["action"]);
        }
    }
}
