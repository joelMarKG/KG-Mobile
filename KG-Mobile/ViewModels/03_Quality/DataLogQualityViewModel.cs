using SBMOM.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using SBMOM_Data_Access;
using SBMOM.Mobile.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using RestSharp;
using System.Web;
using SBMOM.Mobile.Helpers;
using System.Linq;

namespace SBMOM.Mobile.ViewModels._03_Quality
{
    class DataLogQualityViewModel : INotifyPropertyChanged
    {
        private WebApiServices _webApiServices = new WebApiServices();
        private WebApiServicesHelper _webApiServicesHelper = new WebApiServicesHelper();
        private SoundHelper _soundHelper = new SoundHelper();
        private MobileDatabase database = MobileDatabase.Instance;
        
        //data log group names
        string grpNameDefectTracking = "DefectTracking";
        string grpNameDefectTrackingReasons = "DefectTrackingReasons";

        //data log group data
        List<data_log_grp> grpDefectTracking;
        List<data_log_grp> grpDefectTrackingReasons;

        //data log defect data
        List<data_log_16> defectData;

        //lot
        List<lot> lot;

        #region constructor

        //constructor
        public DataLogQualityViewModel()
        {
            //get the group ids for each of the data log groups
            Task.Run(async () => await DataLogQualityViewModelAsync());

            //auto move on/off by default
            if (Settings.QualityMoveAutoOnByDefault)
            {
                AutoMoveEnabled = true;
            }
        }

        //async constructor
        public async Task DataLogQualityViewModelAsync()
        {
            //get the group ids for each of the data log groups
            grpDefectTracking = await _webApiServicesHelper.DataLogGetByGrpDesc(grpNameDefectTracking);
            grpDefectTrackingReasons = await _webApiServicesHelper.DataLogGetByGrpDesc(grpNameDefectTrackingReasons);

            //build the defect level 1 items
            defectData = await _webApiServicesHelper.GetDataLog16ByGrpId(grpDefectTrackingReasons[0].grp_id.ToString());

            var xDefectLevel1 = defectData.Select(x => new
            {
                x.value1,
                x.value3
            })
            .Where(x => Int32.Parse(x.value3) == 1)
            .Distinct()
            .ToList();

            List<PickerList>  tDefectLevel1 = new List<PickerList>();

            foreach (var x in xDefectLevel1)
            {
                tDefectLevel1.Add(new PickerList(0, x.value1));
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
                    _soundHelper.playError();
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
                    _soundHelper.playSuccess();
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
                    var xDefectLevel2 = defectData.Select(x => new
                    {
                        x.row_id,
                        x.value1,
                        x.value2
                    })
                    .Where(x => x.value1 == value.name)
                    .ToList();

                    List<PickerList> tDefectLevel2 = new List<PickerList>();

                    foreach (var x in xDefectLevel2)
                    {
                        tDefectLevel2.Add(new PickerList(x.row_id, x.value2));
                    }

                    DefectLevel2 = tDefectLevel2;

                    // Get ent_id for defect bin
                    var xDefectBinEntId = defectData.Select(x => new
                    {
                        x.value1,
                        x.ent_id
                    })
                    .Where(x => x.value1 == value.name && x.ent_id != null)
                    .ToList();

                    List<PickerList> tDefectBinEntId = new List<PickerList>();

                    foreach (var x in xDefectBinEntId)
                    {
                        tDefectBinEntId.Add(new PickerList(x.ent_id.Value, x.value1));
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
        private int _DefectBinEntId;
        public int DefectBinEntId
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
                    lot = await _webApiServicesHelper.LotGetByLotNo(Barcode);

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
                    await AddToDataLog();
                });
            }

        }

        //update the data log
        async Task AddToDataLog()
        {

            if (DefectLevel2Selected != null && lot != null && lot.Count != 0)
            {
                //Show Busy
                BusyMessage msg = new BusyMessage(true, "Adding Quality Record");
                MessagingCenter.Send(msg, "BusyPopup");

                //setup path
                string path;
                path = $"/api/DataLog/DataLog16Add?";
                path += "grpId=" + HttpUtility.UrlEncode(grpDefectTracking[0].grp_id.ToString(), Encoding.UTF8);
                path += "&sampleTimeLocal=" + HttpUtility.UrlEncode(System.DateTime.Now.ToString(), Encoding.UTF8);
                path += "&shiftStartLocal=" + HttpUtility.UrlEncode(System.DateTime.Now.ToString(), Encoding.UTF8);
                path += "&shiftId=" + HttpUtility.UrlEncode("0", Encoding.UTF8);
                path += "&value1=" + HttpUtility.UrlEncode(DefectLevel2Selected.id.ToString(), Encoding.UTF8);

                //add lot no to datalog if it has been validated
                if ((lot != null) && (lot.Count != 0))
                {
                    path += "&lotNo=" + HttpUtility.UrlEncode(Barcode, Encoding.UTF8);
                }

                //api call
                object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.POST, path);

                //api call threw an error
                if (response.GetType() == typeof(PopupMessage))
                {
                    MessagingCenter.Send((PopupMessage)response, "PopupError");
                }
                //check if response was ok
                else
                {
                    var resp = (IRestResponse)response;
                    if (resp?.StatusCode == HttpStatusCode.OK)
                    {
                        ResultError = false;
                        Result =  Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;

                        //Move the inventory if enabled
                        if (AutoMoveEnabled)
                        {
                            await MoveInventory();
                        }
                    }
                    else
                    {
                        ResultError = true;
                        Result = "Quality Record Save Failed";

                        //Message result back to App.xaml
                        PopupMessage msg2 = new PopupMessage("Quality Record Save Failed", "Data Log Quality", resp.Content, "Ok");
                        MessagingCenter.Send(msg2, "PopupError");
                    }
                }
                
                //Hide Busy
                msg.visible = false;
                MessagingCenter.Send(msg, "BusyPopup");

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
        async Task MoveInventory()
        {
            //Show Busy
            BusyMessage msg = new BusyMessage(true, "Moving Inventory");
            MessagingCenter.Send(msg, "BusyPopup");

            //get item_inv data from the lot_no
            List<item_inv> item_inv_list = await _webApiServicesHelper.ItemInvGetByLotNo(lot[0].lot_no);

            //check to see if the lot has been created as invetnory yet, potentially it hasn't
            if (item_inv_list != null && item_inv_list.Count != 0)
            {
                var item_inv = item_inv_list[0];

                //get ent details for where to move the defect too
                //ent toEnt = await _webApiServicesHelper.EntityGetByEntName(Settings.QualityMoveToEntDefault);
                ent toEnt = await _webApiServicesHelper.EntityGetByEntId(DefectBinEntId);

                //confirm the default move to location actually exists.
                if (toEnt == null)
                {
                    ResultError = true;
                    Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                    // Result += ". Default move to location does not exist";
                    Result += ". Defect Bin not defined for location " + DefectLevel1Selected.name + "!";

                }
                else
                {
                    //get the item details
                    item item = await _webApiServicesHelper.ItemGetByItemId(item_inv.item_id);

                    //setup path
                    string path;
                    path = $"/api/inventory/MoveInventory?" +
                        $"fromRowId=" + HttpUtility.UrlEncode(item_inv.row_id.ToString(), Encoding.UTF8) +
                        $"&toEntId=" + HttpUtility.UrlEncode(toEnt.ent_id.ToString(), Encoding.UTF8) +
                        $"&transferQty=" + HttpUtility.UrlEncode(item_inv.qty_left.ToString(), Encoding.UTF8) +
                        $"&transferComments=" + "Moved_By_SBMOM.Mobile" +
                        $"&toItemId=" + HttpUtility.UrlEncode(item_inv.item_id.ToString(), Encoding.UTF8) +
                        $"&toLotNo=" + HttpUtility.UrlEncode(item_inv.lot_no.ToString(), Encoding.UTF8) +
                        $"&toGradeCd=" + HttpUtility.UrlEncode(item_inv.grade_cd.ToString(), Encoding.UTF8) +
                        $"&toStatusCd=" + HttpUtility.UrlEncode(item_inv.status_cd.ToString(), Encoding.UTF8) +
                        $"&toUomId="; //populated below, based some additional logic

                    //if the item_inv row uom_id is null, then use the item default
                    if (item_inv.uom_id == null)
                    {
                        path += HttpUtility.UrlEncode(item.uom_id.ToString(), Encoding.UTF8);
                    }
                    else
                    {
                        path += HttpUtility.UrlEncode(item_inv.uom_id.ToString(), Encoding.UTF8);
                    }

                    //api call
                    object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.PATCH, path);

                    //api call threw an error
                    if (response.GetType() == typeof(PopupMessage))
                    {
                        MessagingCenter.Send((PopupMessage)response, "PopupError");
                    }
                    //check if response was ok
                    else
                    {
                        var resp = (IRestResponse)response;
                        if (resp?.StatusCode == HttpStatusCode.OK)
                        {
                            ResultError = false;
                            Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                            Result += ". " + lot[0].lot_no + " moved to " + toEnt.description;
                        }
                        else
                        {
                            ResultError = true;
                            Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                            Result += ". " + "Failed to move " + lot[0].lot_no + " to " + toEnt.description;

                            //Message result back to App.xaml
                            PopupMessage msg2 = new PopupMessage("Inventory Move Failed", "Defect", resp.Content, "Ok");
                            MessagingCenter.Send(msg2, "PopupError");
                        }
                    }
                }
            }
            else
            {
                //error to operator when the quality record is saved, but the inventory doesn't exist therefore cannot be moved
                ResultError = true;
                Result = Barcode + " marked as " + DefectLevel2Selected.name + " at " + DefectLevel1Selected.name;
                Result += ". " + lot[0].lot_no + " does not exist as inventory and cannot be moved";
            }

            //Hide Busy
            msg.visible = false;
            MessagingCenter.Send(msg, "BusyPopup");

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
        public int id { get; set; }
        public string name { get; set; }

        public PickerList(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
