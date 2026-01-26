namespace KG_Mobile.Models.CMMES_GraphQL_Models
{
    public partial class Location_CMMES
    {
        public string LocationId { get; set; }
        public string? LocationParentId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}

