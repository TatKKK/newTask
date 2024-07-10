namespace newTask.Models
{
    public class User
    {
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
        public string Fname {  get; set; }
        public string Lname { get; set; }                              
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }

        public int? RoleId {  get; set; }
        
        public Role? Role { get; set; }

    }

     public class Role
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        //public string Description { get; set; } 
    }

    //public class UserRoles
    //{
    //    public int RoleId { get; set;}
    //    public List<User> Users { get; set; }=new List<User>();


    //}

    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class UserDto
    {
        public int? UserId { get; set; }
       
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
     
        
        public bool? IsActive { get; set; }

        public int? RoleId { get; set; }

    }

    public class UserReport
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Username { get; set; }
        public int CompletedTasks { get; set; }
        public int NewTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}
