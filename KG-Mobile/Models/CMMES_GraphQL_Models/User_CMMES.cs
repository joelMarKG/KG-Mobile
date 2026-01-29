
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    public partial class User_CMMES
    {
        public string userId { get; set; }
        public string username { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public DateTime? issuedAt { get; set; }
        public DateTime? expiresAt { get; set; }
        public string? data { get; set; }
        public DateTime? dateCreated { get; set; }
        public string? userCreated { get; set; }
        public DateTime? dateUpdated { get; set; }
        public string? userUpdated { get; set; }
    }
}
