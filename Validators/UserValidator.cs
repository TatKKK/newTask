using newTask.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace newTask.Validators
{
    public static class UserValidator
    {
        public static bool Validate(User user, out string errorMessage)
        {
            if (user == null)
            {
                errorMessage = "User cannot be null.";
                return false;
            }

            if (string.IsNullOrEmpty(user.Fname) || string.IsNullOrEmpty(user.Lname)
                ||string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Phone)||string.IsNullOrEmpty(user.Username)
                || string.IsNullOrEmpty(user.Password)
                || user.CompanyId == 0)
            {
                errorMessage = "Fields are required";
                return false;
            }
            if(user.Fname.Length <4 || user.Lname.Length < 4)
            {
                errorMessage = "Must be min 3 characters";
                return false;
            }

            if(user.Phone.Length != 9)
            {
                errorMessage = "Invalid mobile number";
                return false;
            }
            if ( !System.Text.RegularExpressions.Regex.IsMatch(user.Email, @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])"))
            {
                errorMessage = "Invalid email pattern";
                return false;   
            }
               
            if (user.Username.Length < 5)
            {
                errorMessage = "Username must be at least 5 characters";
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(user.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$"))
            {
                errorMessage = "Passwordmust be at least 6 characters long, include one uppercase letter, one number, and one special character.";
                return false;
            }
              
            errorMessage = string.Empty;
            return true;
        }

    }
}
