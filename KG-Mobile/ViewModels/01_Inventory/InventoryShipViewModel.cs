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
using SBMOM.Mobile.CustomControls;

namespace SBMOM.Mobile.ViewModels._01_Inventory
{
    class InventoryShipViewModel : INotifyPropertyChanged
    {
        private WebApiServices _webApiServices = new WebApiServices();
        private WebApiServicesHelper _webApiServicesHelper = new WebApiServicesHelper();
        private SoundHelper _soundHelper = new SoundHelper();
        private MobileDatabase database = MobileDatabase.Instance;

        //current item inventory and related data
        private ent ent { get; set; }

        #region DataGrid Handling
        //Item Inventory Tags
        private List<item_inv> _item_inv { get; set; }
        public List<item_inv> item_inv
        {
            get
            {
                return _item_inv;
            }
            set
            {
                _item_inv = value;

                //update the count message
                if (value != null)
                {
                    item_inv_count_message = "Inventory Count: " + value.Count;
                }
                else
                {
                    item_inv_count_message = "Inventory Count: 0";
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item_inv"));
            }
        }
        #endregion

        #region XAML Bound Tags

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
                        database.LogAdd(DateTime.Now, "Error", "Inventory Ship", value);
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        database.LogAdd(DateTime.Now, "Info", "Inventory Ship", value);
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

        //item_inv_count Tags
        private string _item_inv_count_message;
        public string item_inv_count_message
        {
            get
            {
                return _item_inv_count_message;
            }
            set
            {
                _item_inv_count_message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item_inv_count_message"));
            }
        }

        #endregion

        #region WebAPI Calls and Commands

        //Command for Location Barcode Enter key
        public ICommand ExecuteLocationBarcode
        {
            get
            {
                return new Command(async () =>
                {
                    //reset values
                    ent = null;
                    LocationDescription = "";

                    if (!String.IsNullOrEmpty(LocationName))
                    {
                        //Show Busy
                        BusyMessage msg = new BusyMessage(true, "Find location");
                        MessagingCenter.Send(msg, "BusyPopup");

                        ent = await _webApiServicesHelper.EntityGetByEntName(LocationName);

                        //entity found?
                        if (ent != null)
                        {
                            LocationDescription = this.ent.description;

                            ResultError = false;
                            Result = "Location '" + LocationName + "' Found";                            

                            //query all inventory for the location
                            item_inv = await _webApiServicesHelper.ItemInvGetByEntId(ent.ent_id);
                        }
                        else
                        {
                            ResultError = true;
                            Result = "Location '" + LocationName + "' Not Found";

                            //reset related values
                            LocationName = "";
                            LocationDescription = "";
                            ent = null;

                            MessagingCenter.Send("Show", "InventoryShipPage-SetMoveToLocationNameFocus");

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

        //Call Move Inventory
        public ICommand ProcessShipCommand
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    //check for no inventory or entity
                    if (ent == null)
                    {
                        ResultError = true;
                        Result = "No Entity selected";                        
                    }
                    else if (item_inv?.Count == 0)
                    {
                        ResultError = true;
                        Result = "No Inventory in Location selected";                       
                    }
                    else
                    {
                        //Yes No Popup
                        bool response = await App.Current.MainPage.DisplayAlert("Ship Inventory", "Are you sure you want to ship all inventory in " + ent.ent_name + "?", "Ok", "Cancel");

                        //If user responds yes, ship inventory
                        if (response)
                        {
                            await InventoryShip();

                            //set location name focus
                            if (Settings.AutoSelectEntryField)
                            {
                                var entry = (CustomEntry)obj;
                                entry.Focus();
                            }
                        }
                    }
                });
            }
        }

        //Ship inventory in ent
        async Task InventoryShip()
        {

            //Show Busy
            BusyMessage msg = new BusyMessage(true, "Shipping inventory");
            MessagingCenter.Send(msg, "BusyPopup");

            //setup path
            string path;
            path = $"/api/inventory/shipInventory?" +
                $"entId=" + HttpUtility.UrlEncode(ent.ent_id.ToString(), Encoding.UTF8);
            
            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.DELETE, path);

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
                    Result = "All Inventory Shipped from '" + ent.ent_name + "'";                    
                    
                    LocationName = "";
                    LocationDescription = "";
                    ent = null;
                    item_inv = null;
                }
                else
                {
                    ResultError = true;
                    Result = "Failed to ship from '" + ent.ent_name + "'";                    

                    //query all inventory for the location
                    item_inv = await _webApiServicesHelper.ItemInvGetByEntId(ent.ent_id);

                    //Message result back to App.xaml
                    PopupMessage msg2 = new PopupMessage("Inventory Move Failed", "Inventory Ship", resp.Content, "Ok");
                    MessagingCenter.Send(msg2, "PopupError");
                }
            }
            
            //Hide Busy
            msg.visible = false;
            MessagingCenter.Send(msg, "BusyPopup");
        }

        //Cancel Current Location
        public ICommand CancelLocation
        {
            get
            {
                return new Command((Object obj) =>
                {
                    //reset related values
                    LocationName = "";
                    LocationDescription = "";
                    ent = null;

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
