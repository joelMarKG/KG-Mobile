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

namespace SBMOM.Mobile.ViewModels._02_Job
{
    class JobTakeoutViewModel : INotifyPropertyChanged
    {
        private WebApiServices _webApiServices = new WebApiServices();
        private WebApiServicesHelper _webApiServicesHelper = new WebApiServicesHelper();
        private SoundHelper _soundHelper = new SoundHelper();
        private MobileDatabase database = MobileDatabase.Instance;

        //current attribute, woAttr
        private List<attr> attr { get; set; }
        private List<wo_attr> wo_attr { get; set; }

        #region constructor

        public JobTakeoutViewModel()
        {
            //auto move on/off by default
            if (Settings.InventoryAutoMoveOnByDefault)
            {
                AutoTakeoutEnabled = true;
            }
        }

        #endregion

        #region DataGrid Handling
        //Job Tag
        private List<job> _job { get; set; }
        public List<job> job
        {
            get
            {
                return _job;
            }
            set
            {
                _job = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("job"));
            }
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

        //MultiMoveEnableToggle Tags
        private bool _AutoTakeoutEnabled;
        public bool AutoTakeoutEnabled
        {
            get
            {
                return _AutoTakeoutEnabled;
            }
            set
            {
                _AutoTakeoutEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AutoTakeoutEnabled"));
            }
        }

        #endregion

        #region WebAPI Calls and Commands

        //Command for Barcode Enter key
        public ICommand ExecuteBarcode
        {
            get
            {
                return new Command(async () =>
                {
                    //reset values
                    wo_attr = new List<wo_attr>();
                    job = new List<job>();

                    await FindAttribute();

                    if ((attr != null) && (attr.Count != 0))                    {

                        if (!String.IsNullOrEmpty(Barcode))
                        {
                            //Show Busy
                            BusyMessage msg = new BusyMessage(true, "Find WO Attr");
                            MessagingCenter.Send(msg, "BusyPopup");

                            //loop through all attrs
                            foreach (attr a in attr)
                            {
                                wo_attr.AddRange(await _webApiServicesHelper.WoAttrGetByAttrIdAttrValue(a.attr_id, Barcode));
                            }

                            //wo_attr found?
                            if (wo_attr != null && wo_attr.Count != 0)
                            {
                                //local copies of job so there is only a single update, otherwise the grid misses updates with the Add/AddRange function
                                List<job> local_job;
                                local_job = new List<job>();
                                List<job> local_job2;
                                local_job2 = new List<job>();
                                
                                //for each work order
                                foreach (wo_attr woa in wo_attr)
                                {
                                    //add all takeout work orders to the job list
                                    local_job.AddRange(await _webApiServicesHelper.JobGetByWoIdOperId(woa.wo_id, "Takeout"));
                                }
                                                                
                                //get the job state code for new
                                List<job_state> job_state_from = await _webApiServicesHelper.JobStateGetByStateDesc("New");
                                int state_cd_from = job_state_from[0].state_cd;

                                //add jobs which are in state new
                                foreach (job j in local_job)
                                {
                                    if (j.state_cd == state_cd_from)
                                    {
                                        local_job2.Add(j);
                                    }
                                }
                                
                                //update job that the data grid and subsequent code references
                                job = local_job2;

                                if (job.Count > 0)
                                {
                                    ResultError = false;
                                    Result = "Barcode " + Barcode + " Found";
                                }

                                if (AutoTakeoutEnabled)
                                {
                                    await TakeoutJobs();
                                }

                            }
                            else
                            {
                                ResultError = true;
                                Result = "Barcode " + Barcode + " Not Found";

                                //reset related values
                                resetBarcode();

                            }

                            //Hide Busy
                            msg.visible = false;
                            MessagingCenter.Send(msg, "BusyPopup");
                        }
                        else
                        {
                            ResultError = true;
                            Result = "Blank Barcode";
                        }
                    }
                });
            }

        }

        //Call Move Inventory
        public ICommand ProcessTakeoutCommand
        {
            get
            {
                return new Command(async () =>
                {
                     await TakeoutJobs();
                });
            }
        }

        //find the attr used to store the Cabinet Barcode against the Work Order
        async Task FindAttribute()
        {
            if (!String.IsNullOrEmpty(Settings.JobTakeoutWoAttrName))
            {
                //reset values
                attr = new List<attr>();

                //Show Busy
                BusyMessage msg = new BusyMessage(true, "Find WO Attribute");
                MessagingCenter.Send(msg, "BusyPopup");

                //split wo attr names to list of strings
                List<string> woAttrNames = Settings.JobTakeoutWoAttrName.Split(',').Select(x => x).ToList();

                foreach (string s in woAttrNames)
                {
                    attr.Add(await _webApiServicesHelper.AttrGetAttrDesc(s));
                }

                //attr found?
                if (attr.Count != 0)
                {
                    ResultError = false;
                    Result = "WO Attr " + Settings.JobTakeoutWoAttrName + " Found";
                }
                else
                {
                    ResultError = true;
                    Result = "WO Attr " + Settings.JobTakeoutWoAttrName + " Not Found";
                }

                //Hide Busy
                msg.visible = false;
                MessagingCenter.Send(msg, "BusyPopup");
            }
            else
            {
                ResultError = true;
                Result = "WO Attr(s) Not Configured";
            }
        }

        //Cancel Current Barcode
        public ICommand CancelBarcode
        {
            get
            {
                return new Command(() =>
                {
                    //reset related values
                    resetBarcode();
                });
            }

        }

        //reset everything
        void resetBarcode()
        {
            attr = null;
            wo_attr = null;
            Barcode = "";
            job = null;

            MessagingCenter.Send("Show", "JobTakeoutPage-SetBarcodeFocus");
        }

        //set all jobs to running in the job List when current state is new
        async Task TakeoutJobs()
        {
            //check for no jobs
            if ((job == null) || (job.Count == 0))
            {
                ResultError = true;
                Result = "Nothing to Takeout";
            }
            else
            {
                //Show Busy
                BusyMessage msg = new BusyMessage(true, "Marking Inventory for Takeout");
                MessagingCenter.Send(msg, "BusyPopup");

                //get the job state code for ready
                List<job_state> job_state_to = await _webApiServicesHelper.JobStateGetByStateDesc("Ready");                

                if (job_state_to != null)
                {
                    int state_cd_to = job_state_to[0].state_cd;
                    bool successTracker = true;

                    //loop through all jobs an update the status to be running
                    foreach (job j in job)
                    {
                        //setup path
                        string path;
                        path = $"/api/job/ChangeJobState?";
                        path += "woId=" + HttpUtility.UrlEncode(j.wo_id, Encoding.UTF8);
                        path += "&operId=" + HttpUtility.UrlEncode(j.oper_id, Encoding.UTF8);
                        path += "&seqNo=" + HttpUtility.UrlEncode(j.seq_no.ToString(), Encoding.UTF8);
                        path += "&toState=" + HttpUtility.UrlEncode(state_cd_to.ToString(), Encoding.UTF8);

                        //api call
                        object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.PATCH, path);

                        //api call threw an error
                        if (response.GetType() == typeof(PopupMessage))
                        {
                            MessagingCenter.Send((PopupMessage)response, "PopupError");
                            break;
                        }
                        //check if response was ok
                        else
                        {
                            var resp = (IRestResponse)response;
                            if (resp?.StatusCode == HttpStatusCode.OK)
                            {
                                successTracker = true;

                                //save to log
                                database.LogAdd(DateTime.Now, "Info", "Job Takeout", "Takeout Success: woId = " + j.wo_id + ", seqNo= " + j.seq_no);
                            }
                            else
                            {
                                successTracker = false;
                                ResultError = true;
                                Result = "No all Items marked for Takeout";

                                //Message result back to App.xaml
                                PopupMessage msg2 = new PopupMessage("Set Item for Takeout Failed", "Job Takeout", resp.Content, "Ok");
                                MessagingCenter.Send(msg2, "PopupError");
                                break;
                            }
                        }
                    }

                    //success message
                    if (successTracker)
                    {
                        ResultError = false;
                        Result = "Takeout Success for " + Barcode;
                    }
                }
                else
                {
                    ResultError = true;
                    Result = "Job Running State Code not found";
                }

                //Hide Busy
                msg.visible = false;
                MessagingCenter.Send(msg, "BusyPopup");

                //reset everything
                resetBarcode();

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
