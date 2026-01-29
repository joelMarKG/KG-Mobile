
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class Grade_CMMES
    {
        public string GradeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool? GoodProduct { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
