using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Models.CMMES_GraphQL_Models;
using KG.Mobile.Services;
using KG_Data_Access;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace KG.Mobile.ViewModels._03_Quality
{
    class DataLogQualityViewModel : INotifyPropertyChanged
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        private readonly SoundHelper _soundHelper;
        private MobileDatabase database = MobileDatabase.Instance;

        private const string defectLotPropertyTypeName = "PreviouslyDefected";
        
        //Grade Reason data
        List<GradeReason_CMMES> gradeReasons;

        //Grade Reason Group data
        List<GradeReasonGroup_CMMES> gradeReasonGroups;

        //lot
        List<Lot_CMMES> lot;

        #region constructor

        //constructor
        public DataLogQualityViewModel(SoundHelper soundHelper)
        {
            //get the group ids for each of the data log groups
            Task.Run(async () => await DataLogQualityViewModelAsync());

            //auto move on/off by default
            if (Settings.QualityMoveAutoOnByDefault)
            {
                AutoMoveEnabled = true;
            }
            _soundHelper = soundHelper;
        }

        //async constructor
        public async Task DataLogQualityViewModelAsync()
        {

            gradeReasons = await _graphQLApiServicesHelper.GetGradeReason();

            //build the defect level 1 items
            gradeReasonGroups = await _graphQLApiServicesHelper.GetGradeReasonGroup();

            var xDefectLevel1 = gradeReasonGroups.Select(x => new
            {
                x.Description,
                x.GradeReasonGroupId
            })
            .Distinct()
            .ToList();

            List<PickerList>  tDefectLevel1 = new List<PickerList>();

            foreach (var x in xDefectLevel1)
            {
                tDefectLevel1.Add(new PickerList(x.GradeReasonGroupId, x.Description));
            }

            DefectLevel1 = tDefectLevel1;

        }

        #endregion

        #region XAML Bound Tags

        private string _Result;
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Result"));
                if (value != "")
                {
                    if (ResultError)
                    {
                        ResultErrorVisible = true;
                        ResultMessageVisible = false;

                        //save to log
                        database.LogAdd(DateTime.Now, "Error", "Job Takeout", value);
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        database.LogAdd(DateTime.Now, "Info", "Job Takeout", value);
                    }

                }
                else
                {
                    ResultErrorVisible = false;
                    ResultMessageVisible = false;
                }
            }
        }

        //ResultError Tag
        private bool ResultError;

        //ResultVisible Tags
        private bool _ResultErrorVisible;
        public bool ResultErrorVisible
        {
            get
            {
                return _ResultErrorVisible;
            }
            set
            {
                if (value)
                {
                    //Play the Error sound
                    _soundHelper.PlayErrorAsync();
                }

                _ResultErrorVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultErrorVisible"));
            }
        }

        private bool _ResultMessageVisible;
        public bool ResultMessageVisible
        {
            get
            {
                return _ResultMessageVisible;
            }
            set
            {
                if (value)
                {
                    //Play the Success sound
                    _soundHelper.PlaySuccessAsync();
                }

                _ResultMessageVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultMessageVisible"));
            }
        }

        //DefectLevel1 Tags
        private List<PickerList> _DefectLevel1;
        public List<PickerList> DefectLevel1
        {
            get
            {
                return _DefectLevel1;
            }
            set
            {
                _DefectLevel1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DefectLevel1"));
            }
        }

        private PickerList _DefectLevel1Selected;
        public PickerList DefectLevel1Selected
        {
            get
            {
                return _DefectLevel1Selected;
            }
            set
            {
                _DefectLevel1Selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DefectLevel1Selected"));

                //build the defect level 2 items, if item 1 is selected
                if (DefectLevel1Selected != null)
                { 
                    var xDefectLevel2 = gradeReasons.Select(x => new
                    {
                        x.Name,
                        x.GradeReasonGroupId,
                        x.GradeReasonId
                    })
                    .Where(x => x.GradeReasonGroupId == value.id)
                    .ToList();

                    List<PickerList> tDefectLevel2 = new List<PickerList>();

                    foreach (var x in xDefectLevel2)
                    {
                        tDefectLevel2.Add(new PickerList(x.GradeReasonId, x.Name));
                    }

                    DefectLevel2 = tDefectLevel2;

                    // Get ent_id for defect bin
                    var xDefectBinEntId = gradeReasonGroups.Select(x => new
                    {
                        x.Name,
                        x.LocationId,
                        x.GradeReasonGroupId,
                    })
                    .Where(x => x.GradeReasonGroupId == value.id && x.LocationId != null)
                    .ToList();

                    List<PickerList> tDefectBinEntId = new List<PickerList>();

                    foreach (var x in xDefectBinEntId)
                    {
                        tDefectBinEntId.Add(new PickerList(x.LocationId, x.Name));
                    }

                    DefectBinEntId = tDefectBinEntId.First().id;
                }
            }
        }

        //DefectLevel2 Tags
        private List<PickerList> _DefectLevel2;
        public List<PickerList> DefectLevel2
        {
            get
            {
                return _DefectLevel2;
            }
            set
            {
                _DefectLevel2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DefectLevel2"));
            }
        }

        //DefectBinEntId Tags
        private string _DefectBinEntId;
        public string DefectBinEntId
        {
            get
            {
                return _DefectBinEntId;
            }
            set
            {
                _DefectBinEntId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DefectBinEntId"));
            }
        }

        private PickerList _DefectLevel2Selected;
        public PickerList DefectLevel2Selected
        {
            get
            {
                return _DefectLevel2Selected;
            }
            set
            {
                _DefectLevel2Selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DefectLevel2Selected"));
            }
        }

        //Barcode Tags
        private string _Barcode;
        public string Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                _Barcode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Barcode"));
            }
        }

        //AutoMoveEnabled Tags
        private bool _AutoMoveEnabled;
        public bool AutoMoveEnabled
        {
            get
            {
                return _AutoMoveEnabled;
            }
            set
            {
                _AutoMoveEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AutoMoveEnabled"));
            }
        }

        #endregion

        #region WebAPI Calls and Commands

        //reset everything
        void resetBarcode()
        {
            Barcode = "";
            lot = null;
            DefectLevel1Selected = null;
            DefectLevel2Selected = null;
        }

        //Clear the Current Barcode
        public ICommand CancelBarcode
        {
            get
            {
                return new Command(() =>
                {
                    //reset related values
                    Barcode = "";
                });
            }

        }

        //Execute the Barcode
        public ICommand ExecuteBarcode
        {
            get
            {
                return new Command(async () =>
                {
                    //look up the lot
                    lot = await _graphQLApiServicesHelper.InventoryGetByLotName(Barcode);

                    if ((lot == null) || (lot.Count == 0))
                    {
                        ResultError = true;
                        Result = "Inventory not found";
                    }
                    else
                    {
                        ResultError = false;
                        Result = "Inventory found";
                    }

                });
            }

        }

        //Save the current data log quality entry
        public ICommand ProcessCommand
        {
            get
            {
                return new Command(async () =>
                {
                    //Save the current data log quality entry
                    await UpdateLotGrade();
                });
            }

        }

        //change the Lot Grade
        async Task UpdateLotGrade()
        {
            if (DefectLevel2Selected != null && lot != null && lot.Count != 0)
            {
                WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Grade Reason Update"));

                try
                {
                    // GraphQL mutation to move inventory
                    string mutation = @"
                    mutation UpdateInventory($lotId: ID!, $gradeReasonId: ID!) {
                        inventoryUpdate(
                            inventoryUpdate: { lotId: $lotId, gradeReasonId: $gradeReasonId }
                        ) {
                            lotId
                            productId
                            lotParentId
                            name
                            description
                            lotStateId
                            lotStatusId
                            gradeReasonId
                            purchaseOrderId
                            quantity
                            unitOfMeasureId
                            locationId
                            data
                            dateCreated
                            userCreated
                            dateUpdated
                            userUpdated
                            lotHistoryId
                        }
                    }
                    ";

                    // Prepare variables
                    var variables = new
                    {
                        lotId = lot[0].LotId,
                        gradeReasonId = DefectLevel2Selected.id
                    };

                    //api call
                    var response = await _graphQLApiServices.ExecuteAsync<List<Lot_CMMES>>(mutation, variables);

                    // Handle errors
                    if (response is PopupMessage popup)
                    {
                        WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                        return;
                    }
                    //check if response was ok
                    if (response is List<Lot_CMMES> inventory && inventory.Count > 0)
                    {
                        ResultError = false;
                        Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;

                        await MarkLotAsDefect();

                        //Move the inventory if enabled
                        if (AutoMoveEnabled)
                        {
                            await MoveInventoryToDefectLocation();
                        }
                    }
                    else
                    {
                        ResultError = true;
                        Result = "Lot Grade Update Failed";

                        var popup2 = new PopupMessage("Lot Grade Update Failed", "Lot Grade Update", "", "Ok");
                        WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup2));
                    }
                }
                catch (Exception ex)
                {
                    // Catch unexpected exceptions
                    var popup = new PopupMessage("GraphQL Exception", "ItemService", ex.Message, "Ok");
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                }
                finally
                {
                    // Hide Busy
                    WeakReferenceMessenger.Default.Send(new BusyMessage(false, string.Empty));
                }

                //reset everything
                resetBarcode();
            }
            else
            {
                ResultError = true;
                Result = "Please select a valid Barcode, Factory Area and Defect Type";
            }
        }


        //Move Inventory API Call
        async Task MoveInventoryToDefectLocation()
        {

            //get location details for where to move the defect too
            Location_CMMES toDefectLocation = await _graphQLApiServicesHelper.LocationGetByLocationId(DefectBinEntId);

            //confirm the default move to location actually exists.
            if (toDefectLocation == null)
            {
                ResultError = true;
                Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                Result += ". Defect Bin not defined for location " + DefectLevel1Selected.name + "!";

            }
            else
            {
                //api call
                var response = await _graphQLApiServicesHelper.MoveInventorytoLocationId(lot[0].LotId, lot[0].Quantity, lot[0].UnitOfMeasureId, toDefectLocation.LocationId);

                // Handle errors
                if (response is null)
                {
                    ResultError = true;
                    Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                    Result += ". Failed to move to location " + DefectLevel1Selected.name + " Defect Bin!";
                    return;
                }
                //check if response was ok
                // Handle success
                if (response is List<Lot_CMMES> movedLots && movedLots.Count > 0)
                {
                    ResultError = false;
                    Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                    Result += ". " + lot[0].Name + " moved to " + toDefectLocation.Description;
                }
            }
        }

        //Mark Lot as Defected.
        async Task MarkLotAsDefect()
        {

            //get lotPropertyType using Name 
            LotPropertyType_CMMES defectLotPropertyType = await _graphQLApiServicesHelper.LotPropertyTypeGetByName(defectLotPropertyTypeName);

            //confirm we have the defect property type information
            if (defectLotPropertyType == null)
            {
                ResultError = true;
                Result = "Unable to find Lot Property Type with name: " + defectLotPropertyTypeName;
                return;
            }

            //See if Previously Defected lotProperty already exists for Lot

            LotProperty_CMMES defectLotProperty = await _graphQLApiServicesHelper.LotPropertyGetByLotIdLotPropertyTypeId(lot[0].LotId, defectLotPropertyType.LotPropertyTypeId);

            //If it Doesn't Exist Create it
            if (defectLotProperty == null)
            {
                LotProperty_CMMES addDefectLotProperty = await _graphQLApiServicesHelper.AddLotProperty(lot[0].LotId, defectLotPropertyType.LotPropertyTypeId, "1");

                if (addDefectLotProperty == null)
                {
                    ResultError = true;
                    Result = "Unable to Create Defect Lot Property for : " + Barcode;
                    return;
                }
                ResultError = false;
                Result = "Lot " + Barcode + " marked with defect lot property.";
                return;
            }
            //If it does exist and is '0' then update to '1'
            else if (defectLotProperty != null && defectLotProperty.Value == "0")
            {

                LotProperty_CMMES updateDefectLotProperty = await _graphQLApiServicesHelper.UpdateLotProperty(lot[0].LotId, defectLotProperty.LotPropertyId, "1");

                if (updateDefectLotProperty == null)
                {
                    ResultError = true;
                    Result = "Unable to Update Defect Lot Property for : " + Barcode;
                    return;
                }
                ResultError = false;
                Result = "Lot " + Barcode + " updated with defect lot property.";
                return;

            }
            //If it does exist and is '1' do nothing.
            else if (defectLotProperty != null && defectLotProperty.Value == "1")
            {
                ResultError = false;
                Result = "Lot " + Barcode + " already marked with Defect lot property.";
                return;
            }

        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

    //class for the picker data
    class PickerList
    {
        public string id { get; set; }
        public string name { get; set; }
       // public string gradeReasonGroupId { get; set; }
        public PickerList(string id, string name)
        {
            this.id = id;
            this.name = name;
            //this.gradeReasonGroupId = gradeReasonGroupId;
        }
    }
}
