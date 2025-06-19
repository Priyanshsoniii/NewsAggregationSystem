namespace NewsAggregation.Server.Models.Entities
{
    public class BaseModel
    {
        public int Id { get; set; }
        public int GetKey() => Id;
    }
}
