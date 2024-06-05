namespace API.Models
{
    public class Operation
    {
        public int OperationID { get; set; }
        public long? EmployeeID { get; set; }
        public int UnitID { get; set; }
        public int OperationType { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
