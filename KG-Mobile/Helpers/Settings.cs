using System;
using Microsoft.Maui.Storage;

namespace KG.Mobile.Helpers
{
    public static class Settings
    {
        #region Setting Constants

        private const string GraphQLApiUrlKey = "GraphQlApiUrl";
        private static readonly string GraphQLApiUrlDefault = "http://WSDEVWWWF001/CMMES.Core.Graph/api/graphql";

        private const string GraphQLApiSecurityUrlKey = "GraphQlApiSecurityUrl";
        private static readonly string GraphQLApiSecurityUrlDefault = "http://WSDEVWWWF001/CMMES.Security.Graph/api/graphql";

        private const string UsernameKey = "Username";
        private static readonly string UsernameDefault = "KG.Mobile";

        private const string PasswordKey = "Password";
        private static readonly string PasswordDefault = "Password2!";

        private const string DeviceNameKey = "DeviceName";
        private static readonly string DeviceNameDefault = Guid.NewGuid().ToString();

        private const string AccessTokenKey = "AccessToken";
        private static readonly string AccessTokenDefault = "";

        private const string GraphQLTimeoutSecondsKey = "GraphQlApiTimeout";
        private static readonly int GraphQLTimeoutSecondsDefault = 60;

        private const string DisablePopupKeyboardKey = "DisablePopupKeyboard";
        private static readonly bool DisablePopupKeyboardDefault = true;

        private const string CheckBinsforInventoryBeforeMovingKey = "CheckBinsforInventoryBeforeMoving";
        private static readonly bool CheckBinsforInventoryBeforeMovingDefault = true;

        private const string InventoryAutoMoveOnByDefaultKey = "InventoryAutoMoveOnByDefault";
        private static readonly bool InventoryAutoMoveOnByDefaultDefault = true;

        private const string LocationAutoMoveOnByDefaultKey = "LocationAutoMoveOnByDefault";
        private static readonly bool LocationAutoMoveOnByDefaultDefault = true;

        private const string JobTakeOutAutoOnByDefaultKey = "JobTakeOutAutoOnByDefault";
        private static readonly bool JobTakeOutAutoOnByDefaultDefault = true;

        private const string QualityMoveAutoOnByDefaultKey = "QualityMoveAutoOnByDefault";
        private static readonly bool QualityMoveAutoOnByDefaultDefault = true;

        private const string QualityMoveToEntDefaultKey = "QualityMoveToEntOnByDefault";
        private static readonly string QualityMoveToEntDefaultDefault = "KG_S001_A90_WC10_WU150";

        private const string AutoSelectEntryFieldKey = "AutoSelectEntryField";
        private static readonly bool AutoSelectEntryFieldDefault = true;

        private const string LocationMoveNameKey = "MenuLocationMoveName";
        private static readonly string LocationMoveNameDefault = "Trolley";

        private const string JobTakeoutWoAttrNameKey = "JobTakeoutWoAttrName";
        private static readonly string JobTakeoutWoAttrNameDefault = "CabinetBarcode,CabinetBarcodeLegacy";

        #endregion

        #region Properties

        public static string GraphQLApiUrl
        {
            get => Preferences.Get(GraphQLApiUrlKey, GraphQLApiUrlDefault);
            set => Preferences.Set(GraphQLApiUrlKey, value);
        }

        public static string GraphQLApiSecurityUrl
        {
            get => Preferences.Get(GraphQLApiSecurityUrlKey, GraphQLApiSecurityUrlDefault);
            set => Preferences.Set(GraphQLApiSecurityUrlKey, value);
        }

        public static string Username
        {
            get => Preferences.Get(UsernameKey, UsernameDefault);
            set => Preferences.Set(UsernameKey, value);
        }

        public static string Password
        {
            get => Preferences.Get(PasswordKey, PasswordDefault);
            set => Preferences.Set(PasswordKey, value);
        }

        public static string DeviceName
        {
            get => Preferences.Get(DeviceNameKey, DeviceNameDefault);
            set => Preferences.Set(DeviceNameKey, value);
        }

        public static string AccessToken
        {
            get => Preferences.Get(AccessTokenKey, AccessTokenDefault);
            set => Preferences.Set(AccessTokenKey, value);
        }

        public static int GraphQLTimeoutSeconds
        {
            get => Preferences.Get(GraphQLTimeoutSecondsKey, GraphQLTimeoutSecondsDefault);
            set => Preferences.Set(GraphQLTimeoutSecondsKey, value);
        }

        public static bool DisablePopupKeyboard
        {
            get => Preferences.Get(DisablePopupKeyboardKey, DisablePopupKeyboardDefault);
            set => Preferences.Set(DisablePopupKeyboardKey, value);
        }

        public static bool CheckBinsforInventoryBeforeMoving
        {
            get => Preferences.Get(CheckBinsforInventoryBeforeMovingKey, CheckBinsforInventoryBeforeMovingDefault);
            set => Preferences.Set(CheckBinsforInventoryBeforeMovingKey, value);
        }

        public static bool InventoryAutoMoveOnByDefault
        {
            get => Preferences.Get(InventoryAutoMoveOnByDefaultKey, InventoryAutoMoveOnByDefaultDefault);
            set => Preferences.Set(InventoryAutoMoveOnByDefaultKey, value);
        }

        public static bool LocationAutoMoveOnByDefault
        {
            get => Preferences.Get(LocationAutoMoveOnByDefaultKey, LocationAutoMoveOnByDefaultDefault);
            set => Preferences.Set(LocationAutoMoveOnByDefaultKey, value);
        }

        public static bool JobTakeOutAutoOnByDefault
        {
            get => Preferences.Get(JobTakeOutAutoOnByDefaultKey, JobTakeOutAutoOnByDefaultDefault);
            set => Preferences.Set(JobTakeOutAutoOnByDefaultKey, value);
        }

        public static bool QualityMoveAutoOnByDefault
        {
            get => Preferences.Get(QualityMoveAutoOnByDefaultKey, QualityMoveAutoOnByDefaultDefault);
            set => Preferences.Set(QualityMoveAutoOnByDefaultKey, value);
        }

        public static string QualityMoveToEntDefault
        {
            get => Preferences.Get(QualityMoveToEntDefaultKey, QualityMoveToEntDefaultDefault);
            set => Preferences.Set(QualityMoveToEntDefaultKey, value);
        }

        public static bool AutoSelectEntryField
        {
            get => Preferences.Get(AutoSelectEntryFieldKey, AutoSelectEntryFieldDefault);
            set => Preferences.Set(AutoSelectEntryFieldKey, value);
        }

        public static string LocationMoveName
        {
            get => Preferences.Get(LocationMoveNameKey, LocationMoveNameDefault);
            set => Preferences.Set(LocationMoveNameKey, value);
        }

        public static string JobTakeoutWoAttrName
        {
            get => Preferences.Get(JobTakeoutWoAttrNameKey, JobTakeoutWoAttrNameDefault);
            set => Preferences.Set(JobTakeoutWoAttrNameKey, value);
        }

        #endregion
    }
}
