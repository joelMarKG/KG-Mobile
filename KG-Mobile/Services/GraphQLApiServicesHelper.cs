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
    class GraphQLApiServicesHelper
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
                query GetItemByName($productId: ID!) {
                    productByFilter(filter: { $productId: $productId }) {
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

                var variables = new { locationId };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Location_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Location_CMMES>
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
                var response = await _graphQLApiServices.ExecuteAsync<List<Location_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Location_CMMES>
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
                var response = await _graphQLApiServices.ExecuteAsync<List<Lot_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Lot_CMMES>
                if (response is List<Lot_CMMES> lots && lots.Count > 0)
                {
                    return lots;
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
        public async Task<List<Lot_CMMES?>> InventoryGetByLotName(string lotName)
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

                var variables = new { lotName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Lot_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Lot_CMMES>
                if (response is List<Lot_CMMES> lot && lot.Count > 0)
                {
                    return lot;
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
        public async Task<Storage_Exec_CMMES?> StorageExecByLocationId(string locationId)
        {
            try
            {
                // GraphQL query to get Location by LocationId
                string query = @"query LocationByFilter($locationId: ID!) {
                    locationByFilter(filter: { locationId: $locationId }) {
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
                    }
                    locationStorageDetails: locationStorageByFilter(
                        filter: { locationId: $locationId }
                    ) {
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
                var response = await _graphQLApiServices.ExecuteAsync<List<Storage_Exec_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Storage_Exec_CMMES>
                if (response is List<Storage_Exec_CMMES> storage_exec && storage_exec.Count > 0)
                {
                    return storage_exec[0];
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

                var variables = new { lotPropertyTypeName };

                // Call your generic GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<LotPropertyType_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<LotPropertyType_CMMES>
                if (response is List<LotPropertyType_CMMES> lotPropertyType && lotPropertyType.Count > 0)
                {
                    return lotPropertyType[0];
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
                var response = await _graphQLApiServices.ExecuteAsync<List<LotProperty_CMMES>>(query, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<LotPropertyType_CMMES>
                if (response is List<LotProperty_CMMES> lotPropertyType && lotPropertyType.Count > 0)
                {
                    return lotPropertyType[0];
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

        
        //GradeReasonGroup Function, GetGradeReasonGroup
        public async Task<List<GradeReasonGroup_CMMES>> GetGradeReasonGroup()
        {
            try
            {
                // GraphQL query to get WorkOrderProperty by WorkOrderPropertyTypeId and WorkOrderPropertyValue
                string query = @"
                query GetGradeReasonGroup {
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
                var response = await _graphQLApiServices.ExecuteAsync<List<GradeReasonGroup_CMMES>>(query);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<GradeReasonGroup_CMMES>
                if (response is List<GradeReasonGroup_CMMES> gradeReasonGroup && gradeReasonGroup.Count > 0)
                {
                    return gradeReasonGroup;
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

        //GradeReason Function, GetGradeReason
        public async Task<List<GradeReason_CMMES>> GetGradeReason()
        {
            try
            {
                // GraphQL query to get Grade Reasons 
                string query = @"
                query GetGradeReason {
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
                var response = await _graphQLApiServices.ExecuteAsync<List<GradeReason_CMMES>>(query);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<GradeReason_CMMES>
                if (response is List<GradeReason_CMMES> gradeReason && gradeReason.Count > 0)
                {
                    return gradeReason;
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
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, string.Empty));
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
        public async Task<List<Lot_CMMES?>> MoveInventorytoLocationId(string lotId, decimal quantityToMove, string unitOfMeasureId, string moveToLocationId)
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
                var response = await _graphQLApiServices.ExecuteAsync<List<Lot_CMMES>>(mutation, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<Lot_CMMES>
                if (response is List<Lot_CMMES> lot && lot.Count > 0)
                {
                    return lot;
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
        public async Task<LotProperty_CMMES?> AddLotProperty(string lotId, string lotPropertyTypeId, string value)
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
                var response = await _graphQLApiServices.ExecuteAsync<List<LotProperty_CMMES>>(mutation, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<LotProperty_CMMES>
                if (response is List<LotProperty_CMMES> lotProperty && lotProperty.Count > 0)
                {
                    return lotProperty[0];
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
        public async Task<LotProperty_CMMES?> UpdateLotProperty(string lotId, string lotPropertyId, string value)
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
                var response = await _graphQLApiServices.ExecuteAsync<List<LotProperty_CMMES>>(mutation, variables);

                // Check if response is a PopupMessage (error)
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return null;
                }

                // Otherwise, cast to List<LotProperty_CMMES>
                if (response is List<LotProperty_CMMES> lotProperty && lotProperty.Count > 0)
                {
                    return lotProperty[0];
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
