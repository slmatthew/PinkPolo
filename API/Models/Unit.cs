namespace API.Models
{
    public class Unit
    {
        public int UnitID { get; set; }
        public long CompanyID { get; set; }
        public bool IsActive { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string UniqueNumber { get; set; }
    }
}
