using LoveMKERegistration.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LoveMKERegistration.API
{
    public static class CCBchurchAPI
    {
        public static async Task<XDocument> APIcall(string serviceData)
        {
            string url = $"https://{ConfigurationManager.AppSettings["CCBAPI:site"]}/api";
            string requestUrl = url + serviceData;
            string username = ConfigurationManager.AppSettings["CCBAPI:username"];
            string password = ConfigurationManager.AppSettings["CCBAPI:password"];
            string basicCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

            try
            {
                WebRequest request = WebRequest.Create(requestUrl);

                CredentialCache credentials = new CredentialCache();
                credentials.Add(new Uri(url), "Basic", new NetworkCredential(username, password));
                request.Credentials = credentials;
                request.Headers.Add("Authorization", $"Basic {basicCredentials}");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebResponse response = await request.GetResponseAsync();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                XDocument xdoc = new XDocument();
                xdoc = XDocument.Load(reader);

                return xdoc;
            }
            catch (WebException webExcp)
            {
                Console.WriteLine("A WebException has been caught.");
                Console.WriteLine(webExcp.ToString());
                WebExceptionStatus status = webExcp.Status;
                if (status == WebExceptionStatus.ProtocolError)
                {
                    Console.Write("The server returned protocol error ");
                    HttpWebResponse httpResponse = (HttpWebResponse)webExcp.Response;
                    Console.WriteLine((int)httpResponse.StatusCode + " - "
                       + httpResponse.StatusCode);
                }
                return null;
            }
        }
        public static XDocument APIPostcall(string postData, string service)
        {
            string url = $"https://{ConfigurationManager.AppSettings["CCBAPI:site"]}/api" + service;
            string username = ConfigurationManager.AppSettings["CCBAPI:username"];
            string password = ConfigurationManager.AppSettings["CCBAPI:password"];
            string usernamePassword = $"{username}:{password}";
            string requestContentType = "application/x-www-form-urlencoded";
            var data = Encoding.ASCII.GetBytes(postData);

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = requestContentType;
                request.ContentLength = data.Length;

                CredentialCache credentials = new CredentialCache();
                credentials.Add(new Uri(url), "Basic", new NetworkCredential(username, password));
                request.Credentials = credentials;
                request.Headers.Add("Authorization", "Basic" + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                XDocument xdoc = new XDocument();
                xdoc = XDocument.Load(reader);

                return xdoc;
            }
            catch (WebException webExcp)
            {
                Console.WriteLine("A WebException has been caught.");
                Console.WriteLine(webExcp.ToString());
                WebExceptionStatus status = webExcp.Status;
                if (status == WebExceptionStatus.ProtocolError)
                {
                    Console.Write("The server returned protocol error ");
                    HttpWebResponse httpResponse = (HttpWebResponse)webExcp.Response;
                    Console.WriteLine((int)httpResponse.StatusCode + " - "
                       + httpResponse.StatusCode);
                }
                return null;
            }
        }
        public static string PostIndividualString(string firstName, string lastName, string phone, string email)
        {
            string requestUrl = $"first_name={firstName}&last_name={lastName}&phone={phone}&email={email}";
            return requestUrl;
        }
        public static string IndividualSearchService(string individualId)
        {
            string baseUrl = ".php?srv=individual_profile_from_id";
            string requestUrl = $"{baseUrl}&individual_id={individualId}";
            return requestUrl;
        }
        public static string IndividualSearchService(string firstName, string lastName)
        {
            string baseUrl = ".php?srv=individual_search";
            string requestUrl = $"{baseUrl}&first_name={firstName}&last_name={lastName}";
            return requestUrl;
        }
        public static string IndividualSearchService(string firstName, string lastName, string emailOrEmail, char emailOrPhoneIndicator)
        {
            string baseUrl = ".php?srv=individual_search";
            string requestUrl;
            if (emailOrPhoneIndicator == 'P')
            {
                requestUrl = $"{baseUrl}&first_name={firstName}&last_name={lastName}&phone={emailOrEmail}";
            }
            else
            {
                requestUrl = $"{baseUrl}&first_name={firstName}&last_name={lastName}&email={emailOrEmail}";
            }
            return requestUrl;
        }
        public static string IndividualSearchService(string firstName, string lastName, string phone, string email)
        {
            string baseUrl = ".php?srv=individual_search";
            string requestUrl = $"{baseUrl}&first_name={firstName}&last_name={lastName}&phone={phone}&email={email}";
            return requestUrl;
        }
        public static string IndividualSearchService(string firstName, string lastName, string phone, string email, string address)
        {
            string baseUrl = ".php?srv=individual_search";
            string requestUrl = $"{baseUrl}&first_name={firstName}&last_name={lastName}&phone={phone}&email={email}&street_address={address}";
            return requestUrl;
        }
        public static string GroupTypeListService()
        {
            return ".php?srv=group_type_list";
        }
        public static string MembershipListService()
        {
            return $".php?srv=membership_type_list";
        }
        public static string GroupListService(string type_id)
        {
            return $".php?srv=group_search&type_id={type_id}";
        }
        public static string GetGroupProfileService(string groupId)
        {
            return $".php?srv=group_profile_from_id&id={groupId}";
        }
        public static string GetGroupParticipantsService(string groupId)
        {
            return $".php?srv=group_participants&id={groupId}";
        }
        public static string GetAddToGroupService(string groupId, string individualId)
        {
            return $".php?srv=add_individual_to_group&id={individualId}&group_id={groupId}&status=add";
        }
        public static async Task<string> GetTypeID(string groupTypeName)
        {
            var doc = await APIcall(GroupTypeListService());

            if (doc != null)
            {
                var id = doc.Descendants("item").Where(w => w.Element("name").Value == groupTypeName).Select(s => s.Element("id").Value).FirstOrDefault();
                return id;
            }
            else
            {
                return null;
            }


            
        }
        public static async Task<string> GetMembershipID(string membershipTypeName)
        {
            var doc = await APIcall(MembershipListService());

            var id = doc.Descendants("item").Where(w => w.Element("name").Value == membershipTypeName).Select(s => s.Element("id").Value).FirstOrDefault();

            return id;
        }
        public static async Task<List<string>> GetGroupIdList(string typeId)
        {
            var doc = await APIcall(GroupListService(typeId));

            var idList = doc.Descendants("item").Select(s => s.Element("id").Value.ToString()).ToList();

            return idList;
        }
        private static string GetFamilyId(XDocument document)
        {
            var search = document.Descendants("family").Select(s => s);
            string familyId = search.Select(s => s.Attribute("id").Value).FirstOrDefault();
            return familyId;
        }
        public static async Task<List<GroupViewModel>> GetGroups(List<string> groupIds)
        {
            List<GroupViewModel> groupsList = new List<GroupViewModel>();
            List<Task<GroupViewModel>> taskList = new List<Task<GroupViewModel>>();

            foreach (var g in groupIds)
            {
                try
                {

                    taskList.Add(Task.Run(async () =>
                     {
                         GroupViewModel group = new GroupViewModel(g);
                         var doc = await APIcall(GetGroupProfileService(g));
                         var selectedGroup = doc.Descendants("group").Where(w => w.Attribute("id").Value == g).Select(s => s);
                         var selectedLeader = selectedGroup.Select(s => s.Element("main_leader"));

                         var leaderFirstName = selectedLeader.Select(s => s.Element("first_name").Value)?.FirstOrDefault();
                         var leaderLastName = selectedLeader.Select(s => s.Element("last_name").Value)?.FirstOrDefault();
                         var leaderEmail = selectedLeader.Select(s => s.Element("email").Value)?.FirstOrDefault();
                         var leaderPhone = selectedLeader.Select(s => s.Element("phones").Element("phone")).Where(w => w.Attribute("type").Value == "contact").Select(s => s.Value)?.FirstOrDefault();

                         group.GroupId = g;
                         group.Name = selectedGroup.Select(s => s.Element("name").Value)?.FirstOrDefault();
                         group.Description = selectedGroup.Select(s => s.Element("description").Value)?.FirstOrDefault();
                         group.LeaderID = selectedGroup.Select(s => s.Element("main_leader").Attribute("id").Value)?.FirstOrDefault();
                         var capacity = selectedGroup.Select(s => s.Element("group_capacity").Value)?.FirstOrDefault();
                         if (capacity is string && capacity == "Unlimited")
                         {
                             group.Capacity = 100;
                         }
                         else
                         {
                             try
                             {
                                 group.Capacity = Convert.ToInt32(capacity) - 1;
                             }
                             catch
                             {
                                 group.Capacity = 100;
                             }
                         }
                         if (selectedGroup.Select(s => s.Element("addresses").Element("address")).First() != null) //Make sure address exists
                         {
                             group.Address = selectedGroup.Select(s => s.Element("addresses").Element("address").Element("street_address").Value)?.FirstOrDefault();
                             group.City = selectedGroup.Select(s => s.Element("addresses").Element("address").Element("city").Value)?.FirstOrDefault();
                             group.State = selectedGroup.Select(s => s.Element("addresses").Element("address").Element("state").Value)?.FirstOrDefault();
                             group.ZipCode = selectedGroup.Select(s => s.Element("addresses").Element("address").Element("zip").Value)?.FirstOrDefault();
                         }
                         group.Leader = new IndividualViewModel(group.LeaderID, leaderFirstName, leaderLastName, leaderPhone, leaderEmail);
                         group.CurrentMembers = FillCurrentMembersList(selectedGroup);
                         group.SpotsRemaining = group.Capacity - Convert.ToInt32(selectedGroup.Select(s => s.Element("current_members").Value)?.FirstOrDefault()) + 1;

                         return group;
                     }));
                }
                catch
                {
                    Console.WriteLine("Something went wrong while accessing CCB API.");
                }
            }
            var results = await Task.WhenAll(taskList);
            return results.ToList();
        }
        public static async Task<List<string>> GetGroupParticipantIds(string groupId)
        {
            List<string> groupParticipantIds = new List<string>();

            var doc = await APIcall(GetGroupParticipantsService(groupId));

            var participantIdList = doc.Descendants("participant").Where(w => w.Element("status").Attribute("id").Value == "2").Select(s => s.Attribute("id").Value).ToList();

            return participantIdList;
        }
        public static async Task<IndividualViewModel> GetIndividualViewModelById(string individualId)
        {
            IndividualViewModel individual = new IndividualViewModel();

            var doc = await APIcall(IndividualSearchService(individualId));

            var selectedGroup = doc.Descendants("individual").Select(s => s);

            individual.IndividualId = individualId;
            individual.FamilyId = selectedGroup.Select(s => s.Element("family").Attribute("id").Value).FirstOrDefault();
            individual.FirstName = selectedGroup.Select(s => s.Element("first_name").Value).FirstOrDefault();
            individual.LastName = selectedGroup.Select(s => s.Element("last_name").Value).FirstOrDefault();
            individual.Phone = selectedGroup.Select(s => s.Element("phones").Element("phone")).Where(w => w.Attribute("type").Value == "contact").Select(s => s.Value).First();
            individual.Email = selectedGroup.Select(s => s.Element("email").Value).FirstOrDefault();


            FamilyViewModel familyMember;
            var selectedFamily = doc.Descendants("family_member").Select(s => s);
            individual.Family = selectedFamily.Select(s =>
            {
                familyMember = new FamilyViewModel();
                familyMember.IndividualId = s.Element("individual").Attribute("id").Value;
                familyMember.Position = s.Element("family_position").Value;
                string fullName = s.Element("individual").Value;
                familyMember.FirstName = fullName.Split(' ').FirstOrDefault();
                familyMember.LastName = fullName.Split(' ').Last();
                return familyMember;
            }).ToList();

            return individual;
        }
        public static async Task<bool> PostIndividualFamilyToCCB(IndividualViewModel person)
        {
            string membershipId = await GetMembershipID("LoveMKE");
            string familyId = PostIndividualToCCB(person, membershipId);
            if (familyId != "" && person.Family?.Count() > 0)
            {
                for (int i = 0; i < person.Family.Count(); i++)
                {
                    PostFamilyMemberToCCB(person.Family[i].FirstName.Trim(), person.Family[i].LastName, person.Email, person.Phone, membershipId, familyId);
                }
                return true;
            }
            else if (familyId != "" && (person.Family?.Count() == 0 || person.Family?.Count() == null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static string PostIndividualToCCB(IndividualViewModel person, string membershipId)
        {
            string service = ".php?srv=create_individual";
            string postData = $"first_name={person.FirstName}&last_name={person.LastName}&contact_phone={person.Phone}&email={person.Email}&membership_type_id={membershipId}";
            var doc = APIPostcall(postData, service);
            if (IndividualAddCount(doc) == "1")
                return GetFamilyId(doc);
            else
                return "";

        }
        private static bool PostFamilyMemberToCCB(string firstName, string lastName, string email, string phone, string membershipId, string familyId)
        {
            string service = ".php?srv=create_individual";
            string postData = $"first_name={firstName}&last_name={lastName}&contact_phone={phone}&email={email}&membership_type_id={membershipId}&family_id={familyId}";
            var doc = APIPostcall(postData, service);
            if (IndividualAddCount(doc) == "1")
                return true;
            else
                return false;

        }
        private static List<IndividualViewModel> FillCurrentMembersList(IEnumerable<XElement> document)
        {
            List<IndividualViewModel> currentMemberList = new List<IndividualViewModel>();
            var selectLeaders = document.Descendants("leader");
            var selectMembers = document.Descendants("participant");
            IndividualViewModel individual;


            foreach (var person in selectLeaders)
            {
                individual = new IndividualViewModel();
                individual.IndividualId = person.Attribute("id").Value;
                individual.Email = person.Element("email").Value;
                var phones = person.Descendants("phones");
                individual.Phone = phones.Select(p => p.Element("phone")).Where(s => s.Attribute("type").Value == "contact").Select(s => s.Value).FirstOrDefault();
                individual.FirstName = person.Element("first_name").Value;
                individual.LastName = person.Element("last_name").Value;
                currentMemberList.Add(individual);
            }

            foreach (var person in selectMembers)
            {
                individual = new IndividualViewModel();
                individual.IndividualId = person.Attribute("id").Value;
                individual.Email = person.Element("email").Value ?? "";
                XElement result = person.Descendants("phones")
                                        .FirstOrDefault(el => el.Attribute("type") != null &&
                                                              el.Attribute("type").Value == "contact");
                if (result != null)
                    individual.Phone = result.Element("phone").Value;
                individual.FirstName = person.Element("first_name").Value;
                individual.LastName = person.Element("last_name").Value;
                currentMemberList.Add(individual);
            }


            return currentMemberList;
        }
        private static string IndividualAddCount(XDocument document)
        {
            var individualSearch = document.Descendants("response").Select(s => s);
            string countString = individualSearch.Select(s => s.Element("individuals").Attribute("count").Value).FirstOrDefault();
            return countString;
        }

    }

}