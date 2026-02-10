using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Models;
using KG.Mobile.Models.CMMES_GraphQL_Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static SQLite.SQLite3;

namespace KG.Mobile.Services
{
    //Helper Class for GraphQLApiServices for common calls
    public class GraphQLApiServicesHelper
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();

        #region Queries

        //Product Function, GetByProductName
        public async Task<Product_CMMES?> ProductGetByProductName(string productName)
        {
            try
            {
                // GraphQL query to get product by productName
                string query = @"
                query GetProductByName($productName: String!) {
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

                // Otherwise, cast to List<Product_CMMES>
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

        //Product Function, GetByProductId
        public async Task<Product_CMMES?> ProductGetByProductId(string productId)
        {
            try
            {
                // GraphQL query to get product by productId
                string query = @"
                query GetProductById($productId: ID!) {
                    productByFilter(filter: { productId: $productId }) {
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

                var variables = new { productId };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<ProductByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to ProductByFilterResponse
                if (response is ProductByFilterResponse product && product.ProductByFilter.Count > 0)
                {
                    return product.ProductByFilter[0];
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
                // GraphQL query to get Location by LocationId
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

                var variables = new { locationId = locationId };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<LocationByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LocationByFilterResponse
                if (response is LocationByFilterResponse location && location.LocationByFilter.Count > 0)
                {
                    return location.LocationByFilter[0];
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
                // GraphQL query to get Location by LocationName
                string query = @"query LocationGetByLocationName($locationName: String!) {
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
                var response = await _graphQLApiServices.ExecuteAsync<LocationByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LocationByFilterResponse
                if (response is LocationByFilterResponse location && location.LocationByFilter.Count > 0)
                {
                    return location.LocationByFilter[0];
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

        //Inventory Function, GetByLocationId
        public async Task<List<Lot_CMMES?>> InventoryGetByLocationId(string locationId)
        {
            try
            {
                // GraphQL query to get Inventory by LocationId
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
                var response = await _graphQLApiServices.ExecuteAsync<LotByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LotByFilterResponse
                if (response is LotByFilterResponse lots && lots.LotByFilter.Count > 0)
                {
                    return lots.LotByFilter;
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

        //Inventory Function, GetByLotName
        public async Task<Lot_CMMES?> InventoryGetByLotName(string lotName)
        {
            try
            {
                // GraphQL query to get Inventory by LotName
                string query = @"query InventoryGetByLotName($name: String!) {
                    lotByFilter(filter: { name: $name }) {
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

                var variables = new { name = lotName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<LotByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LotByFilterResponse
                if (response is LotByFilterResponse lot && lot.LotByFilter.Count > 0)
                {
                    return lot.LotByFilter[0];
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

        //StorageExec Function, GetByLocationId
        public async Task<StorageExec_CMMES?> StorageExecByLocationId(string locationId)
        {
            try
            {
                // GraphQL query to get Location by LocationId
                string query = @"query StorageExecByLocationId($locationId: ID!) {
                    StorageExecByFilter: locationByFilter(filter: { locationId: $locationId }) {
                        locationId
                        name
                        type: vLocationPropertyByFilter(filter: { name: ""Type"" }) {
                            value
                        }
                        status: vLocationPropertyByFilter(filter: { name: ""Status"" }) {
                            value
                        }
                        spareint1: vLocationPropertyByFilter(filter: { name: ""StorageExec_spare_int1"" }) {
                            value
                        }
                        spare1: vLocationPropertyByFilter(filter: { name: ""StorageExec_spare1"" }) {
                            value
                        }
                        movable: vLocationPropertyByFilter(filter: { name: ""Movable"" }) {
                            value
                        }
                        canStore: vLocationPropertyByFilter(filter: { name: ""CanStore"" }) {
                            value
                        }
                        canShip: vLocationPropertyByFilter(filter: { name: ""CanShip"" }) {
                            value
                        }
                    }
                }
                ";

                var variables = new { locationId = locationId };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<StorageExecByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to StorageExecByFilterResponse
                if (response is StorageExecByFilterResponse storage_exec && storage_exec.StorageExecByFilter.Count > 0)
                {
                    return storage_exec.StorageExecByFilter[0];
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

        //LocationStorage Function, GetByLocationId
        public async Task<LocationStorage_CMMES?> LocationStorageByLocationId(string locationId)
        {
            try
            {
                // GraphQL query to get Location by LocationId
                string query = @"query LocationStorageByFilter($locationId: ID!) {
                    locationStorageByFilter(filter: { locationId: $locationId }) {
                        locationStorageId
                        locationId
                        locationId_StoredIn
                        locationStorageStateId
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
                var response = await _graphQLApiServices.ExecuteAsync<LocationStorageByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to StorageExecByFilterResponse
                if (response is LocationStorageByFilterResponse locationStorage && locationStorage.LocationStorageByFilter.Count > 0)
                {
                    return locationStorage.LocationStorageByFilter[0];
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

        //LotPropertyType Function, GetByName
        public async Task<LotPropertyType_CMMES?> LotPropertyTypeGetByName(string lotPropertyTypeName)
        {
            try
            {
                // GraphQL query to get LotPropertyType by LotPropertyTypeName
                string query = @"query LotPropertyTypeGetByName($name: String!) {
                    lotPropertyTypeByFilter(filter: { name: $name }) {
                        lotPropertyTypeId
                        name
                        description
                        lotPropertyEnumTypeId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new { name = lotPropertyTypeName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<LotPropertyTypeByFilterResponse>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LotPropertyTypeByFilterResponse
                if (response is LotPropertyTypeByFilterResponse lotPropertyType && lotPropertyType.lotPropertyTypeByFilter.Count > 0)
                {
                    return lotPropertyType.lotPropertyTypeByFilter[0];
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

        //LotProperty Function, GetByLotId and LotPropertyTypeId
        public async Task<LotProperty_CMMES?> LotPropertyGetByLotIdLotPropertyTypeId(string lotId, string lotPropertyTypeId)
        {
            try
            {
                // GraphQL query to get LotProperty by LotId and LotPropertyTypeId
                string query = @"query GetLotProperty($lotId: ID!, $lotPropertyTypeId: ID!) {
                    lotPropertyByFilter(
                        filter: { lotId: $lotId, lotPropertyTypeId: $lotPropertyTypeId }
                    ) {
                        lotPropertyId
                        lotId
                        lotPropertyTypeId
                        value
                        lotPropertyEnumId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new {
                    lotId = lotId,
                    lotPropertyTypeId = lotPropertyTypeId
                };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync< LotPropertyByFilterResponse> (query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LotPropertyByFilterResponse
                if (response is LotPropertyByFilterResponse lotPropertyType && lotPropertyType.lotPropertyByFilter.Count > 0)
                {
                    return lotPropertyType.lotPropertyByFilter[0];
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

        //LocationPropertyType Function, GetByName
        public async Task<LocationPropertyType_CMMES?> LocationPropertyTypeGetByName(string locationPropertyTypeName)
        {
            try
            {
                // GraphQL query to get LocationPropertyType by LocationPropertyTypeName
                string query = @"query LocationPropertyTypeGetByName($name: String!) {
                    locationPropertyTypeByFilter(filter: { name: $name }) {
                        locationPropertyTypeId
                        name
                        description
                        locationPropertyEnumTypeId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new { locationPropertyTypeName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<LocationPropertyType_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<LocationPropertyType_CMMES>
                if (response is List<LocationPropertyType_CMMES> locationPropertyType && locationPropertyType.Count > 0)
                {
                    return locationPropertyType[0];
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

        //LocationPropertyType Function, GetByName
        public async Task<WorkOrderPropertyType_CMMES?> WorkOrderPropertyTypeGetByName(string workOrderPropertyTypeName)
        {
            try
            {
                // GraphQL query to get WorkOrderPropertyType by WorkOrderPropertyTypeName
                string query = @"query WorkOrderPropertyTypeGetByName($name: String!) {
                    workOrderPropertyTypeByFilter(filter: { name: $name }) {
                        workOrderPropertyTypeId
                        name
                        description
                        workOrderPropertyEnumTypeId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                var variables = new { workOrderPropertyTypeName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<WorkOrderPropertyType_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<WorkOrderPropertyType_CMMES>
                if (response is List<WorkOrderPropertyType_CMMES> workOrderPropertyType && workOrderPropertyType.Count > 0)
                {
                    return workOrderPropertyType[0];
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

        //WorkOrderProperty Function, GetByWokrOrderPropertyTypeId&WorkOrderPropertyValue
        public async Task<WorkOrderProperty_CMMES?> WorkOrderPropertyGetByWorkOrderPropertyTypeIdWorkOrderPropertyValue(string attrId, string attrValue)
        {
            try
            {
                // GraphQL query to get WorkOrderProperty by WorkOrderPropertyTypeId and WorkOrderPropertyValue
                string query = @"query WorkOrderPropertyGetByWorkOrderPropertyTypeIdWorkOrderPropertyValue(
                    $workOrderPropertyTypeId: ID!
                    $value: String!
                ) {
                    workOrderPropertyByFilter(
                        filter: { workOrderPropertyTypeId: $workOrderPropertyTypeId, value: $value }
                    ) {
                        workOrderPropertyTypeId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                        value
                        workOrderPropertyEnumId
                        workOrderId
                        workOrderPropertyId
                    }
                }
                ";

                var variables = new { attrId, attrValue };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<WorkOrderProperty_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<WorkOrderProperty_CMMES>
                if (response is List<WorkOrderProperty_CMMES> workOrderProperty && workOrderProperty.Count > 0)
                {
                    return workOrderProperty[0];
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



        ////Job Function, GetByWoIdOperId
        //public async Task<List<job>> JobGetByWoIdOperId(string woId, string operId)
        //{

        //    //setup path
        //    string path;
        //    path = $"/api/Job/GetByWoIdOperId?woId=";
        //    path += HttpUtility.UrlEncode(woId.ToString(), Encoding.UTF8);
        //    path += $"&operId=";
        //    path += HttpUtility.UrlEncode(operId.ToString(), Encoding.UTF8);

        //    //api call
        //    object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<job>());

        //    //api call threw an error
        //    if (response.GetType() == typeof(PopupMessage))
        //    {
        //        MessagingCenter.Send((PopupMessage)response, "PopupError");
        //    }
        //    //api call responded with deseralised object
        //    else if (response.GetType() == typeof(List<job>))
        //    {

        //        List<job> job = (List<job>)response;

        //        return job;
        //    }

        //    return null; //default return
        //}

        ////Job State Function, GetByStateDesc
        //public async Task<List<job_state>> JobStateGetByStateDesc(string stateDesc)
        //{

        //    //setup path
        //    string path;
        //    path = $"/api/JobState/GetByStateDesc?stateDesc=";
        //    path += HttpUtility.UrlEncode(stateDesc, Encoding.UTF8);

        //    //api call
        //    object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<job_state>());

        //    //api call threw an error
        //    if (response.GetType() == typeof(PopupMessage))
        //    {
        //        MessagingCenter.Send((PopupMessage)response, "PopupError");
        //    }
        //    //api call responded with deseralised object
        //    else if (response.GetType() == typeof(List<job_state>))
        //    {

        //        List<job_state> job_state = (List<job_state>)response;

        //        return job_state;
        //    }

        //    return null; //default return
        //}


        //GradeReasonGroup Function, GradeReasonGroupGet
        public async Task<List<GradeReasonGroup_CMMES>> GradeReasonGroupGet()
        {
            try
            {
                // GraphQL query to get WorkOrderProperty by WorkOrderPropertyTypeId and WorkOrderPropertyValue
                string query = @"
                query GradeReasonGroupGet {
                    gradeReasonGroup {
                        gradeReasonGroupId
                        gradeReasonGroupParentId
                        name
                        description
                        locationId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<GradeReasonGroupResponse>(query);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<GradeReasonGroup_CMMES>
                if (response is GradeReasonGroupResponse gradeReasonGroup && gradeReasonGroup.gradeReasonGroup.Count > 0)
                {
                    return gradeReasonGroup.gradeReasonGroup;
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

        //GradeReason Function, GradeReasonGet
        public async Task<List<GradeReason_CMMES>> GradeReasonGet()
        {
            try
            {
                // GraphQL query to get Grade Reasons 
                string query = @"
                query GradeReasonGet {
                    gradeReason {
                        gradeReasonId
                        name
                        description
                        gradeReasonGroupId
                        gradeId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }   
                ";

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<GradeReasonResponse>(query);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to GradeReasonResponse
                if (response is GradeReasonResponse gradeReason && gradeReason.gradeReason.Count > 0)
                {
                    return gradeReason.gradeReason;
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

        /// <summary>
        /// Checks if the current bearer token is valid.
        /// </summary>
        public async Task LoggedInCheck(string username)
        {
            try
            {
                // GraphQL query for user info
                string query = @"
                query User($username: String!) {
                    userByFilter(filter: { username: $username }) {
                        userId
                        username
                        name
                        description
                        issuedAt
                        expiresAt
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }";
                var variables = new { username };

                // Call GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<User_CMMES>>(query, variables);

                // Handle errors
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return;
                }

                // Otherwise, cast to List<User_CMMES>
                if (response is List<User_CMMES> user && user.Count > 0)
                {
                    if (user[0].expiresAt < DateTime.UtcNow )
                    {
                        WeakReferenceMessenger.Default.Send(new PopupMessage("LogIn", "Logged In", "User Already Logged In", "OK"));
                    }
                }
            }
            catch (Exception)
            {
                // Ignore errors; will continue to normal login
            }
            finally
            {
                // Hide Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));
            }
        }

        //Below has been replaced by InventoryGetByLotName
        ////Lot Function, GetByLotNo
        //public async Task<List<lot>> LotGetByLotNo(string lotNo)
        //{

        //    //setup path
        //    string path;
        //    path = $"/api/Lot/GetByLotNo?lotNo=";
        //    path += HttpUtility.UrlEncode(lotNo, Encoding.UTF8);

        //    //api call
        //    object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.GET, path, new List<lot>());

        //    //api call threw an error
        //    if (response.GetType() == typeof(PopupMessage))
        //    {
        //        MessagingCenter.Send((PopupMessage)response, "PopupError");
        //    }
        //    //api call responded with deseralised object
        //    else if (response.GetType() == typeof(List<lot>))
        //    {

        //        List<lot> lot = (List<lot>)response;

        //        return lot;
        //    }

        //    return null; //default return
        //}
        #endregion
        #region Mutations
        //Move Inventory To LocationId 
        public async Task<Lot_CMMES?> MoveInventorytoLocationId(string lotId, decimal quantityToMove, string unitOfMeasureId, string moveToLocationId)
        {
            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                mutation InventoryMove(
                    $lotId: ID!
                    $quantityToMove: Float!
                    $unitOfMeasureId: ID!
                    $moveToLocationId: ID!
                ) {
                    inventoryMove(
                        inventoryMove: {
                            lotId: $lotId
                            quantityToMove: $quantityToMove
                            unitOfMeasureId: $unitOfMeasureId
                            moveToLocationId: $moveToLocationId
                        }
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
                    }
                }";

                // Prepare variables
                var variables = new
                {
                    lotId = lotId,
                    quantityToMove = quantityToMove,
                    unitOfMeasureId = unitOfMeasureId,
                    moveToLocationId = moveToLocationId
                };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<InventoryMoveResponse>(mutation, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to InventoryMoveResponse
                if (response is InventoryMoveResponse inventoryMove && inventoryMove.InventoryMove.Count > 0)
                {
                    return inventoryMove.InventoryMove[0];
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

        //Add Lot Property
        public async Task<LotProperty_CMMES?> LotPropertyAdd(string lotId, string lotPropertyTypeId, string value)
        {
            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                mutation AddLotProperty($lotId: ID!, $lotPropertyTypeId: ID!, $value: String!) {
                    lotPropertyAdd(
                        lotProperty: { lotId: $lotId, lotPropertyTypeId: $lotPropertyTypeId, value: $value }
                    ) {
                        lotPropertyId
                        lotId
                        lotPropertyTypeId
                        value
                        lotPropertyEnumId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                // Prepare variables
                var variables = new
                {
                    lotId = lotId,
                    lotPropertyTypeId = lotPropertyTypeId,
                    value = value
                };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<LotPropertyAddResponse>(mutation, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LotPropertyAddResponse
                if (response is LotPropertyAddResponse lotProperty && lotProperty.lotPropertyAdd.Count > 0)
                {
                    return lotProperty.lotPropertyAdd[0];
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

        //Update Lot Property
        public async Task<LotProperty_CMMES?> LotPropertyUpdate(string lotId, string lotPropertyId, string value)
        {
            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                mutation UpdateLotProperty($lotId: ID!, $lotPropertyId: ID!, $value: String!) {
                    lotPropertyUpdate(
                        lotProperty: { lotId: $lotId, lotPropertyId: $lotPropertyId, value: $value }
                    ) {
                        lotPropertyId
                        lotId
                        lotPropertyTypeId
                        value
                        lotPropertyEnumId
                        data
                        dateCreated
                        userCreated
                        dateUpdated
                        userUpdated
                    }
                }
                ";

                // Prepare variables
                var variables = new
                {
                    lotId = lotId,
                    lotPropertyId = lotPropertyId,
                    value = value
                };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<LotPropertyUpdateResponse>(mutation, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to LotPropertyUpdateResponse
                if (response is LotPropertyUpdateResponse lotProperty && lotProperty.lotPropertyUpdate.Count > 0)
                {
                    return lotProperty.lotPropertyUpdate[0];
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

        #endregion
    }
}
