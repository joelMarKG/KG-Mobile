using KG.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Input;
//using SBMOM_Data_Access;
using KG.Mobile.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using RestSharp;
using KG.Mobile.CustomControls;
using KG.Mobile.Helpers;
using System.Web;

namespace KG.Mobile.ViewModels._01_Inventory
{
    class InventoryMoveViewModel : INotifyPropertyChanged
    {
        private GraphQLAPIServices _graphQLApiServices = new WebApiServices();
        private WebApiServicesHelper _graphQLApiServicesHelper = new WebApiServicesHelper();
        private SoundHelper _soundHelper = new SoundHelper();
        private MobileDatabase database = MobileDatabase.Instance;

        //current item inventory and related data
        private item_inv item_inv { get; set; }
        private lot lot { get; set; }
        private item item { get; set; }
        private ent ent { get; set; }
        private ent toEnt { get; set; }

        #region XAML Bound Tags
        //Item Barcode Tags
        private string _ItemBarcode;
        public string ItemBarcode {
            get { return _ItemBarcode; }
            set
            {
                _ItemBarcode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemBarcode"));

                ItemID = "";
                ItemDescription = "";
                LocationName = "";
                LocationDescription = "";
                item_inv = null;
                lot = null; 
                item = null;
                ent = null;
            }
        }

        //Result Tags
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
                    if(ResultError)
                    {
                        ResultErrorVisible = true;
                        ResultMessageVisible = false;

                        //save to log
                        database.LogAdd(DateTime.Now, "Error", "Inventory Move", value);
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        database.LogAdd(DateTime.Now, "Info", "Inventory Move", value);
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


        //ItemID Tags
        private string _ItemID;
        public string ItemID
        {
            get
            {
                return _ItemID;
            }
            set
            {
                _ItemID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemID"));
            }
        }

        //ItemDescription Tags
        private string _ItemDescription;
        public string ItemDescription
        {
            get
            {
                return _ItemDescription;
            }
            set
            {
                _ItemDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemDescription"));
            }
        }

        //LocationName Tags
        private string _LocationName;
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                _LocationName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationName"));
            }
        }

        //LocationDescription Tags
        private string _LocationDescription;
        public string LocationDescription
        {
            get
            {
                return _LocationDescription;
            }
            set
            {
                _LocationDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationDescription"));
            }
        }

        //MoveToLocationName Tags
        private string _MoveToLocationName;
        public string MoveToLocationName
        {
            get
            {
                return _MoveToLocationName;
            }
            set
            {
                _MoveToLocationName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoveToLocationName"));
            }
        }
        
        //MoveToLocationDescription Tags
        private string _MoveToLocationDescription;
        public string MoveToLocationDescription
        {
            get
            {
                return _MoveToLocationDescription;
            }
            set
            {
                _MoveToLocationDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoveToLocationDescription"));
            }
        }
        
        //MultiMoveEnableToggle Tags
        private bool _MultiMoveEnabled;
        public bool MultiMoveEnabled
        {
            get
            {
                return _MultiMoveEnabled;
            }
            set
            {
                _MultiMoveEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiMoveEnabled"));
            }
        }

        #endregion

        #region WebAPI Calls and Commands

        //Command for ItemBarcode Enter key
        public ICommand ExecuteItemBarcode
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    await LoadInventoryData();

                    //select the ItemBarcode Field
                    if (Settings.AutoSelectEntryField)
                    {
                        var entry = (CustomEntry)obj;
                        entry.Focus();
                    }
                });
            }

        }

        //Command for Location Barcode Enter key
        public ICommand ExecuteLocationBarcode
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    //reset values
                    toEnt = null;
                    MoveToLocationDescription = "";

                    if (!String.IsNullOrEmpty(MoveToLocationName))
                    {
                        //Show Busy
                        BusyMessage msg = new BusyMessage(true, "Find location");
                        MessagingCenter.Send(msg, "BusyPopup");

                        toEnt = await _webApiServicesHelper.EntityGetByEntName(MoveToLocationName);

                        //entity found?
                        if (toEnt != null)
                        {
                            MoveToLocationDescription = this.toEnt.description;
                            ResultError = false;
                            Result = "";

                            if (Settings.InventoryAutoMoveOnByDefault)
                            {
                                MultiMoveEnabled = true;
                            }

                            //select the ItemBarcode Field
                            if (Settings.AutoSelectEntryField)
                            {
                                var entry = (CustomEntry)obj;
                                entry.Focus();
                            }

                            ResultError = false;
                            Result = "Location '" + MoveToLocationName + "' Found";
                        }
                        else
                        {
                            ResultError = true;
                            Result = "Location '" + MoveToLocationName + "' Not Found";

                            //reset related values
                            MoveToLocationName = "";
                            MoveToLocationDescription = "";
                            toEnt = null;

                            MessagingCenter.Send("Show", "InventoryMovePage-SetMoveToLocationNameFocus");

                            }
                        //Hide Busy
                        msg.visible = false;
                        MessagingCenter.Send(msg, "BusyPopup");
                        
                    }
                    else
                    {
                        ResultError = true;
                        Result = "Blank Location Name";                        
                    }
                });
            }

        }

        //Populate Item Inventory Data
        async Task LoadInventoryData()
        {
            ResultErrorVisible = false;

            if (ItemBarcode != "")
            {
                bool existsInABin = false;
                ent binDetails = new ent();

                //Show Busy
                BusyMessage msg = new BusyMessage(true, "Searching for inventory");
                MessagingCenter.Send(msg, "BusyPopup");

                //setup path
                string path;
                path = $"/api/inventory/getByLotNo?lotNo=";
                path += HttpUtility.UrlEncode(ItemBarcode, Encoding.UTF8);

                //api call
                object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<item_inv>());

                //api call threw an error
                if (response.GetType() == typeof(PopupMessage))
                {
                    MessagingCenter.Send((PopupMessage)response, "PopupError");
                }
                //api call responded with deseralised object
                else if (response.GetType() == typeof(List<item_inv>))
                {

                    List<item_inv> item_inv = (List<item_inv>)response;

                    //if inventory is found, update the gui
                    if (item_inv.Count != 0)
                    {

                        //handle entites with 0 qty_left follwing a move and clean up on item_inv not completed
                        foreach (item_inv i in item_inv)
                        {
                            if (i.qty_left > 0)
                            {                                
                                //check if this exists in a storage bin, if enabled
                                if (Settings.CheckBinsforInventoryBeforeMoving && await InventoryBinCheck(i.ent_id))
                                {
                                    existsInABin = true;

                                    //get details for where it is stored
                                    binDetails = await _webApiServicesHelper.EntityGetByEntId(i.ent_id);

                                    break;
                                }
                                    
                                this.item_inv = i;
                            }
                        }

                        //Popup Error to user on inventory existing in a bin
                        if (existsInABin)
                        {
                            //Message result back to App.xaml
                            PopupMessage msg2 = new PopupMessage("Inventory Error", "Inventory Move", "Piece of Inventory found in Storage Bin for Item Barcode = " + ItemBarcode 
                                + ". Bin Name = " + binDetails.ent_name
                                + ". Bin Description = " + binDetails.description
                                + ". This piece of Inventory cannot be tracked."
                                , "Ok");
                            MessagingCenter.Send(msg2, "PopupError");

                            ItemBarcode = "";
                        }
                        else
                        {
                            ItemID = this.item_inv.item_id;

                            //update the other UI Values
                            await LoadItemData(this.item_inv.item_id);
                            await LoadLocationData(this.item_inv.ent_id);
                        }
                    }
                    else //no inventory, does the lot exist but no inventory?
                    {
                        await LoadLotData();
                    }
                }
                
                //Hide Busy
                msg.visible = false;
                MessagingCenter.Send(msg, "BusyPopup");
                
                //Auto move the item if enabled
                if (MultiMoveEnabled && !existsInABin)
                {
                    ResultError = false;
                    Result = "";

                    //determine if we create or move inventory or specific error to user
                    if (toEnt != null && item_inv == null && lot != null)
                    {
                        await CreateInventory();
                        ItemBarcode = "";
                    }
                    else if(toEnt != null && item_inv != null)
                    {
                        await MoveInventory();
                        ItemBarcode = "";
                    }
                    else if (toEnt == null)
                    {
                        ResultError = true;
                        Result = "Not a valid destination to auto move";                        
                    }
                    else if (item_inv == null && lot == null)
                    {
                        ResultError = true;
                        Result = "Not a valid piece of inventory to auto move";
                        ItemBarcode = "";
                    }

                }

            }
        }

        //Populate Item Description based on ItemId
        async Task LoadItemData(string item_id)
        {
            //setup path
            string path;
            path = $"/api/item/getByItemId?itemId=";
            path += HttpUtility.UrlEncode(item_id, Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<item>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<item>))
            {

                List<item> item = (List<item>)response;

                //if item is found, update the gui
                if (item.Count != 0)
                {
                    this.item = item[0];

                    ItemDescription = this.item.item_desc;
                    Result = "";
                }
                else
                {
                    ResultError = true;
                    Result = "No Item Data Found";
                }
            }
        }

        //Populate LocationName and LocationDescription based on Ent_Id
        async Task LoadLocationData(int ent_id)
        {

            //setup path
            string path;
            path = $"/api/entity/getById?entId=";
            path += HttpUtility.UrlEncode(ent_id.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<ent>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<ent>))
            {

                List<ent> ent = (List<ent>)response;

                //if ent is found, update the gui
                if (ent.Count != 0)
                {
                    this.ent = ent[0];

                    LocationName = this.ent.ent_name;
                    LocationDescription = this.ent.description;
                }
            }         
        }

        //Populate Item Data from the Lot Definition when no Inventory Exists
        async Task LoadLotData()
        {
            //setup path
            string path;
            path = $"/api/lot/getByLotNo?lotNo=";
            path += HttpUtility.UrlEncode(ItemBarcode, Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<lot>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<lot>))
            {

                List<lot> lot = (List<lot>)response;

                //if lot is found, update the gui
                if (lot.Count != 0)
                { 
                    this.lot = lot[0];
                    ItemID = this.lot.item_id;

                    //update the other UI Values
                    await LoadItemData(this.lot.item_id);
                }
                else //no lot definition
                {
                    ResultError = true;
                    Result = "Lot Definition for '" + ItemBarcode + "' does not exist";
                    ItemBarcode = "";
                }
            }
        }

        //Call Move Inventory
        public ICommand ProcessMoveCommand
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    if(toEnt != null && item_inv != null)
                    {
                        await MoveInventory();
                        ItemBarcode = "";

                        //select the ItemBarcode Field
                        if (Settings.AutoSelectEntryField)
                        {
                            var entry = (CustomEntry)obj;
                            entry.Focus();
                        }

                    }
                    else if(toEnt != null && lot != null)
                    {
                        await CreateInventory();
                        ItemBarcode = "";

                        //select the ItemBarcode Field
                        if (Settings.AutoSelectEntryField)
                        {
                            var entry = (CustomEntry)obj;
                            entry.Focus();
                        }

                    }
                    
                });
            }

        }

        //Move Inventory API Call
        async Task MoveInventory()
        {
            //Show Busy
            BusyMessage msg = new BusyMessage(true, "Moving inventory");
            MessagingCenter.Send(msg, "BusyPopup");

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
                    Result = ItemBarcode + " moved to " + toEnt.description;                    
                }
                else
                {
                    ResultError = true;
                    Result = "Failed to move " + ItemBarcode + " to " + toEnt.description;
                                        
                    //Message result back to App.xaml
                    PopupMessage msg2 = new PopupMessage("Inventory Move Failed", "Inventory Move", resp.Content, "Ok");
                    MessagingCenter.Send(msg2, "PopupError");
                }
            }

            //Hide Busy
            msg.visible = false;
            MessagingCenter.Send(msg, "BusyPopup");

        }

        //Create Inventory API Call
        async Task CreateInventory()
        {
            //Show Busy
            BusyMessage msg = new BusyMessage(true, "Creating inventory");
            MessagingCenter.Send(msg, "BusyPopup");

            //setup path
            string path;
            path = $"/api/inventory/AddInventory?" +
                $"lotNo=" + HttpUtility.UrlEncode(lot.lot_no.ToString(), Encoding.UTF8) +
                $"&entId=" + HttpUtility.UrlEncode(toEnt.ent_id.ToString(), Encoding.UTF8) +
                $"&addQty=" + "1" +
                $"&statusCd=" + HttpUtility.UrlEncode(lot.status_cd.ToString(), Encoding.UTF8) +
                $"&gradeCd=" + HttpUtility.UrlEncode(lot.grade_cd.ToString(), Encoding.UTF8) +
                $"&itemId=" + HttpUtility.UrlEncode(lot.item_id.ToString(), Encoding.UTF8) +
                $"&transferComments=" + "Created_By_SBMOM.Mobile" +
                $"&uomId=" + HttpUtility.UrlEncode(item.uom_id.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.PUT, path);

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //check if response was ok
            else
            {
                var resp = (IRestResponse)response;
                if (resp?.StatusCode == HttpStatusCode.Created)
                {
                    ResultError = false;
                    Result = ItemBarcode + " created at " + toEnt.description;
                }
                else
                {
                    ResultError = true;
                    Result = "Failed";

                    //Message result back to App.xaml
                    PopupMessage msg2 = new PopupMessage("Inventory Move Failed", "Inventory Move", resp.Content, "Ok");
                    MessagingCenter.Send(msg2, "PopupError");
                }
            }

            //Hide Busy
            msg.visible = false;
            MessagingCenter.Send(msg, "BusyPopup");

        }

        //Check if entId is Type == "bin" in storage exec
        async Task<bool> InventoryBinCheck(int entId)
        {
            //setup path
            string path;
            path = $"/api/storageexec/getById?entId=";
            path += HttpUtility.UrlEncode(entId.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<storage_exec>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<storage_exec>))
            {

                List<storage_exec> storage_exec = (List<storage_exec>)response;

                //if storage_exec row is found
                if (storage_exec.Count != 0)
                {
                    //look for type == "bin"
                    if (storage_exec[0].type.ToLower() == "bin")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //no storage exec definition, assume not a bin
                {
                    return false;
                }
            }

            return false; //default

        }


        //Cancel Current Location
        public ICommand CancelLocation
        {
            get
            {
                return new Command((Object obj) =>
                {
                    //reset related values
                    MoveToLocationName = "";
                    MoveToLocationDescription = "";
                    toEnt = null;

                    //set location name focus
                    if (Settings.AutoSelectEntryField)
                    {
                        var entry = (CustomEntry)obj;
                        entry.Focus();
                    }
                });
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
}
