
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class GradeReason_CMMES
    {
        public string GradeReasonId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? GradeReasonGroupId { get; set; }
        public string? GradeId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
