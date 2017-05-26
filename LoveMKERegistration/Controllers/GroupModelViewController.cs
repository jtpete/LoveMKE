using LoveMKERegistration.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoveMKERegistration.Models;
using System.Threading.Tasks;

namespace LoveMKERegistration.Controllers
{
    [RoutePrefix("groups")]
    public class GroupModelViewController : Controller
    {
        [Route("{groupTypeName}")]
        // GET: groups/Groups?GroupTypeName=LoveMKE
        public async Task<ActionResult> Groups(string groupTypeName)
        {
            string typeId = await CCBchurchAPI.GetTypeID(groupTypeName);
            var groupIdList = await CCBchurchAPI.GetGroupIdList(typeId);
            var modelList = await CCBchurchAPI.GetGroups(groupIdList);

            return View(modelList);
        }

        //[Route("group-details")]
        //// GET: groups/groupName
        //public ActionResult Details(GroupViewModel group)
        //{
        //    return View(group);
        //}
    }
}
