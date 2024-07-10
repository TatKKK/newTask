using Microsoft.AspNetCore.Mvc;
using newTask.Models;
using System.ComponentModel.DataAnnotations;

namespace newTask.Validators
{
    public class CompanyValidator 
    {
        public int? CompanyId { get; set; }
        public string? Name { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }

        // User Properties
        public int? UserId { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            // Validate Company
            if (string.IsNullOrEmpty(Name))
                errors.Add("Company name is required.");
            if (string.IsNullOrEmpty(TaxCode) || !(TaxCode.Length == 9 || TaxCode.Length == 11))
                errors.Add("Tax Code must be either 9 or 11 digits.");
            if (string.IsNullOrEmpty(Address))
                errors.Add("Address is required.");

            // Validate User
            if (string.IsNullOrEmpty(Fname) || Fname.Length < 3)
                errors.Add("First name is required and must be at least 3 characters long.");
            if (string.IsNullOrEmpty(Lname) || Lname.Length < 4)
                errors.Add("Last name is required and must be at least 4 characters long.");
            if (string.IsNullOrEmpty(Phone) || Phone.Length != 9 || !System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^-?(0|[1-9]\d*)?$"))
                errors.Add("Phone is required and must be exactly 9 digits.");
            if (string.IsNullOrEmpty(Email) || !System.Text.RegularExpressions.Regex.IsMatch(Email, @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])"))
                errors.Add("Invalid email pattern.");
            if (string.IsNullOrEmpty(Username) || Username.Length < 5)
                errors.Add("Username is required and must be at least 5 characters.");
            if (string.IsNullOrEmpty(Password) || !System.Text.RegularExpressions.Regex.IsMatch(Password, @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{6,}$"))
                errors.Add("Password is required and must be at least 6 characters long, include one uppercase letter, one number, and one special character.");
            if (Password != ConfirmPassword)
                errors.Add("Passwords do not match.");

            return errors;
        }

    }
}
