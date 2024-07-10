namespace newTask.Models
{
    public class ErrorLog
    {
        public int? Id { get; set; }
        public string ErrorText { get; set; } 
        public int? UserId {  get; set; }
        public DateTime? LogDate { get; set; }
    }
}
