namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    // CMMES Replica model of the Storage_Exec table.
    public partial class LocationStorage_CMMES
    {
        public string LocationStorageId { get; set; }
        public string LocationId { get; set; }
        public string? LocationId_StoredIn { get; set; }
        public string? LocationStorageStateId { get; set; }
        public string? Data { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }

    }
}
