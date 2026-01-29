
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class LotProperty_CMMES
    {
        public string LotPropertyId { get; set; }
        public string LotId { get; set; }
        public string LotPropertyTypeId { get; set; }
        public string? Value { get; set; }
        public string? LotPropertyEnumId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
