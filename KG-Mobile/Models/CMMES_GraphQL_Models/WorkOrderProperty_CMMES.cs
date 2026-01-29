
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class WorkOrderProperty_CMMES
    {
        public string WorkOrderPropertyId { get; set; }
        public string WorkOrderId { get; set; }
        public string WorkOrderPropertyTypeId { get; set; }
        public string Value { get; set; }
        public string? WorkOrderPropertyEnumId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
