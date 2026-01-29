
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class Lot_CMMES
    {
        public string LotId { get; set; }
        public string? LotParentId { get; set; }
        public string? ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LotStateId { get; set; }
        public string? LotStatusId { get; set; }
        public string? GradeReasonId { get; set; }
        public string? PurchaseOrderId { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasureId { get; set; }
        public string? LocationId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
