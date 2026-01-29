
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class Product_CMMES
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string UnitOfMeasureId { get; set; }
        public string? ProductStateId { get; set; }
        public string? ProductTypeId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
