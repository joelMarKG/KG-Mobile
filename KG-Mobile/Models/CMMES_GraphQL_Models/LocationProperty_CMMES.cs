
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class LocationProperty_CMMES
    {
        public string LocationPropertyId { get; set; }
        public string LocationId { get; set; }
        public string LocationPropertyTypeId { get; set; }
        public string Value { get; set; }
        public string? LocationPropertyEnumId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
