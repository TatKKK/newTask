namespace newTask.Models
{
    public class Task
    {
        public int? Id { get; set; }
        public int? AssigneeId {  get; set; }
        public User? User { get; set; }
        public int? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }=DateTime.Now;
        public DateTime? ModifyDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? FinishDate {  get; set; }

        public TaskLevel? Level { get; set; }
        public TaskStatus? Status { get; set; }
       

    }

   public enum TaskLevel
    {
        Low,
        Medium,
        High
    }

    public enum TaskStatus
    {
        New,
        InProgress,
        Completed,
        Overdue
    }

    // sxvanairad validaciebs mtxovs arasachiro fieldebze
    public class TaskStatusUpdateDto
    {
        public int TaskId { get; set; }
        public TaskStatus Status { get; set; }
    }


   public class EditTaskDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
    }


}
