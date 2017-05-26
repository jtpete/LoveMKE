using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using LoveMKERegistration;
using LoveMKERegistration.Controllers;
using System.Threading.Tasks;

namespace LoveMKERegistration.Tests.Controllers
{
    [TestClass]
    public class GroupControllerTest
    {

        [TestMethod]
        public async Task TestGroupsViewDetails()
        {
            //Arrange
            var controller = new GroupModelViewController();

            //Act
            var result = await controller.Groups("LoveMKE") as ViewResult;
            //Assert
            Assert.AreEqual("", result.ViewName);
        }
    }
}
