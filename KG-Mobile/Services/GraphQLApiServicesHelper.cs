using KG.Mobile.Models;
using KG_Mobile.Models.CMMES_GraphQL_Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Mvvm.Messaging;

namespace KG.Mobile.Services
{
    //Helper Class for GraphQLApiServices for common calls
    class GraphQLApiServicesHelper
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();

        //Product Function, GetByProductId
        public async Task<Product_CMMES?> ProductGetByProductName(string productName)
        {
            try
            {
                // GraphQL query to get product by productName
                string query = @"
                query GetItemByName($productName: String!) {
                    productByFilter(filter: { name: $productName }) {
                        productId
                        name
                        description
                        unitOfMeasureId
                        productStateId
                        productTypeId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new { productName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Product_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Product>
                if (response is List<Product_CMMES> product && product.Count > 0)
                {
                    return product[0];
                }
            }
            catch (Exception ex)
            {
                // Optional: handle unexpected exceptions
                var popup = new PopupMessage(
                    "GraphQL Exception",
                    "ItemService",
                    ex.Message,
                    "Ok"
                );
                WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
            }

            return null; // default return
        }

        //Location Function, GetByLocationId
        public async Task<Location_CMMES?> LocationGetByLocationId(string locationId)
        {
            try
            {
                // GraphQL query to get product by productName
                string query = @"query LocationGetByLocationId($locationId: ID!) {
                    locationByFilter(filter: { locationId: $locationId }) {
                        locationId
                        locationParentId
                        name
                        description
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new { locationId };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Location_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Product>
                if (response is List<Location_CMMES> location && location.Count > 0)
                {
                    return location[0];
                }
            }
            catch (Exception ex)
            {
                // Optional: handle unexpected exceptions
                var popup = new PopupMessage(
                    "GraphQL Exception",
                    "ItemService",
                    ex.Message,
                    "Ok"
                );
                WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
            }

            return null; // default return
        }

        //Location Function, GetByLocationName
        public async Task<Location_CMMES?> LocationGetByLocationName(string locationName)
        {
            try
            {
                // GraphQL query to get product by productName
                string query = @"query LocationGetByLocationId($locationName: String!) {
                    locationByFilter(filter: { name: $locationName }) {
                        locationId
                        locationParentId
                        name
                        description
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new { locationName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Location_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Product>
                if (response is List<Location_CMMES> location && location.Count > 0)
                {
                    return location[0];
                }
            }
            catch (Exception ex)
            {
                // Optional: handle unexpected exceptions
                var popup = new PopupMessage(
                    "GraphQL Exception",
                    "ItemService",
                    ex.Message,
                    "Ok"
                );
                WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
            }
            return null; // default return
        }

        //____ Function, GetBy___
        public async Task<Lot_CMMES?> InventoryGetByLocationId(string locationId)
        {
            try
            {
                // GraphQL query to get product by productName
                string query = @"query InventoryGetByLocationId($locationId: ID!) {
                    lotByFilter(filter: { locationId: $locationId }) {
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
                    }
                }
                ";

                var variables = new { locationId };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Lot_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<___>
                if (response is List<Lot_CMMES> lot && lot.Count > 0)
                {
                    return lot[0];
                }
            }
            catch (Exception ex)
            {
                // Optional: handle unexpected exceptions
                var popup = new PopupMessage(
                    "GraphQL Exception",
                    "ItemService",
                    ex.Message,
                    "Ok"
                );
                WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
            }

            return null; // default return
        }

        //____ Function, GetBy___
        public async Task<_____?> ___GetBy___(string ____)
        {
            try
            {
                // GraphQL query to get product by productName
                string query = @"_____
                 ";

                var variables = new { _____ };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<______>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<___>
                if (response is List<___> ___ && ___.Count > 0)
                {
                    return ___[0];
                }
            }
            catch (Exception ex)
            {
                // Optional: handle unexpected exceptions
                var popup = new PopupMessage(
                    "GraphQL Exception",
                    "ItemService",
                    ex.Message,
                    "Ok"
                );
                WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
            }

            return null; // default return
        }


        //ItemInv Function, GetByLotNo
        public async Task<List<item_inv>> ItemInvGetByLotNo(string lotNo)
        {

            //setup path
            string path;
            path = $"/api/inventory/getByLotNo?lotNo=";
            path += HttpUtility.UrlEncode(lotNo, Encoding.UTF8);

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
                return item_inv;
            }

            return null; //default return
        }

        //StorageExec Function, ByEntId
        public async Task<List<storage_exec>> StorageExecByEntId(int entId)
        {

            //setup path
            string path;
            path = $"/api/StorageExec/getById?entId=";
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
                return storage_exec;
            }

            return null; //default return
        }

        //Attr Function, GetAttrDesc
        public async Task<attr> AttrGetAttrDesc(string attrDesc)
        {

            //setup path
            string path;
            path = $"/api/Attr/getByAttrDesc?AttrDesc=";
            path += HttpUtility.UrlEncode(attrDesc.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<attr>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<attr>))
            {

                List<attr> attr = (List<attr>)response;

                //if attr is found, update the gui
                if (attr.Count != 0)
                {
                    return attr[0];
                }
            }

            return null; //default return
        }

        //WoAttr Function, GetByAttrIdAttrValue
        public async Task<List<wo_attr>> WoAttrGetByAttrIdAttrValue(int attrId, string attrValue)
        {

            //setup path
            string path;
            path = $"/api/WoAttr/getByAttrIdAttrValue?AttrId=";
            path += HttpUtility.UrlEncode(attrId.ToString(), Encoding.UTF8);
            path += $"&AttrValue=";
            path += HttpUtility.UrlEncode(attrValue.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<wo_attr>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<wo_attr>))
            {

                List<wo_attr> wo_attr = (List<wo_attr>)response;

                return wo_attr;
            }

            return null; //default return
        }

        //Job Function, GetByWoIdOperId
        public async Task<List<job>> JobGetByWoIdOperId(string woId, string operId)
        {

            //setup path
            string path;
            path = $"/api/Job/GetByWoIdOperId?woId=";
            path += HttpUtility.UrlEncode(woId.ToString(), Encoding.UTF8);
            path += $"&operId=";
            path += HttpUtility.UrlEncode(operId.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<job>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<job>))
            {

                List<job> job = (List<job>)response;

                return job;
            }

            return null; //default return
        }

        //Job State Function, GetByStateDesc
        public async Task<List<job_state>> JobStateGetByStateDesc(string stateDesc)
        {

            //setup path
            string path;
            path = $"/api/JobState/GetByStateDesc?stateDesc=";
            path += HttpUtility.UrlEncode(stateDesc, Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<job_state>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<job_state>))
            {

                List<job_state> job_state = (List<job_state>)response;

                return job_state;
            }

            return null; //default return
        }

        //DataLog Function, GetByGrpDesc
        public async Task<List<data_log_grp>> DataLogGetByGrpDesc(string grpDesc)
        {

            //setup path
            string path;
            path = $"/api/DataLog/GetByGrpDesc?grpDesc=";
            path += HttpUtility.UrlEncode(grpDesc, Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<data_log_grp>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<data_log_grp>))
            {

                List<data_log_grp> job_state = (List< data_log_grp>)response;

                return job_state;
            }

            return null; //default return
        }

        //DataLog Function, GetDataLog16ByGrpId
        public async Task<List<data_log_16>> GetDataLog16ByGrpId(string grpId)
        {

            //setup path
            string path;
            path = $"/api/DataLog/GetDataLog16ByGrpId?grpId=";
            path += HttpUtility.UrlEncode(grpId, Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<data_log_16>());

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<data_log_16>))
            {

                List<data_log_16> job_state = (List<data_log_16>)response;

                return job_state;
            }

            return null; //default return
        }

        //Lot Function, GetByLotNo
        public async Task<List<lot>> LotGetByLotNo(string lotNo)
        {

            //setup path
            string path;
            path = $"/api/Lot/GetByLotNo?lotNo=";
            path += HttpUtility.UrlEncode(lotNo, Encoding.UTF8);

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

                return lot;
            }

            return null; //default return
        }

    }
}
