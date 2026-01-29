
namespace KG.Mobile.Models.CMMES_GraphQL_Models
{
    // CMMES Replica model of the Storage_Exec table.
    public partial class Storage_Exec_CMMES
    {
        public string LocationId { get; set; }
        public string Name { get; set; }
        public List<LocationProperty_CMMES>? Type { get; set; }
        public List<LocationProperty_CMMES>? Status { get; set; }
        public List<LocationProperty_CMMES>? SpareInt1 { get; set; }
        public List<LocationProperty_CMMES>? Spare1 { get; set; }
        public List<LocationProperty_CMMES>? Movable { get; set; }
        public List<LocationStorage_CMMES>? LocationStorageDetails { get; set; }

    }
}
