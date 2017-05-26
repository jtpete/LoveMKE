using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoveMKERegistration.Models;
using LoveMKERegistration.Controllers;
using LoveMKERegistration.API;

namespace LoveMKERegistration.Tests.API
{
    [TestClass]
    public class APITest
    {
        [TestMethod]
        public async void TestMethod2()
        {
            string typeId = await CCBchurchAPI.GetTypeID("LoveMKE");
            var groupIdList = await CCBchurchAPI.GetGroupIdList(typeId);
            var groupsList = await CCBchurchAPI.GetGroups(groupIdList);

            foreach (var g in groupIdList)
            {
                var participantIdList = CCBchurchAPI.GetGroupParticipantIds(g);
            }

            await CCBchurchAPI.GetIndividualViewModelById("1");



            await CCBchurchAPI.APIcall(CCBchurchAPI.GroupTypeListService());
            await CCBchurchAPI.APIcall(CCBchurchAPI.GroupListService("34"));
        }
    }
}
