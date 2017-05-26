using LoveMKERegistration.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoveMKERegistration.Models
{
    public class IndividualViewModel
    {
        public IndividualViewModel()
        {

        }
        public IndividualViewModel(string individualId, string firstName, string lastName, string phone, string email)
        {
            IndividualId = individualId;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
        }
       
        public string IndividualId { get; set; }
        public string FamilyId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Phone Number")]
        [Phone]
        public string Phone { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public List<FamilyViewModel> Family { get; set; }

        [Display(Name = "Name")]
        public string DisplayName
        {
            get
            {
                string displayFirstName = string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string displayLastName = string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string.Format($"{displayFirstName} {displayLastName}");

            }
        }
    }
    public class FamilyViewModel
    {
       
        public string IndividualId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string DisplayName
        {
            get
            {
                string displayFirstName = string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string displayLastName = string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string.Format($"{displayFirstName} {displayLastName}");

            }
        }
        public string Position { get; set; }
    }

    public class GroupViewModel : IEnumerable
    {

        public string GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string DisplayAddress { get { return $"{Address}, {City}, {State}, {ZipCode}"; } }
        public int Capacity { get; set; }
        public string LeaderID { get; set; }
        public IndividualViewModel Leader { get; set; }
        public List<IndividualViewModel> CurrentMembers { get; set; }
        public int SpotsRemaining { get; set; }

        public GroupViewModel(string groupId)
        {
            this.GroupId = groupId;
        }
        public GroupViewModel()
        {

        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class SignupViewModel
    { 
        public IndividualViewModel Family { get; set; }
        public List<GroupViewModel> Groups { get; set; }
    }

}