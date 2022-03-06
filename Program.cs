using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using static TestDispatchUtility.DispatchUtility;
using System.Text.RegularExpressions;
using System.Linq;
using TestDispatchUtility;
using System.Security.Principal;

namespace SimpleICS {
    enum NETCON_STATUS {
        NCS_DISCONNECTED = 0,
        NCS_CONNECTING = (NCS_DISCONNECTED + 1),
        NCS_CONNECTED = (NCS_CONNECTING + 1),
        NCS_DISCONNECTING = (NCS_CONNECTED + 1),
        NCS_HARDWARE_NOT_PRESENT = (NCS_DISCONNECTING + 1),
        NCS_HARDWARE_DISABLED = (NCS_HARDWARE_NOT_PRESENT + 1),
        NCS_HARDWARE_MALFUNCTION = (NCS_HARDWARE_DISABLED + 1),
        NCS_MEDIA_DISCONNECTED = (NCS_HARDWARE_MALFUNCTION + 1),
        NCS_AUTHENTICATING = (NCS_MEDIA_DISCONNECTED + 1),
        NCS_AUTHENTICATION_SUCCEEDED = (NCS_AUTHENTICATING + 1),
        NCS_AUTHENTICATION_FAILED = (NCS_AUTHENTICATION_SUCCEEDED + 1),
        NCS_INVALID_ADDRESS = (NCS_AUTHENTICATION_FAILED + 1),
        NCS_CREDENTIALS_REQUIRED = (NCS_INVALID_ADDRESS + 1),
        NCS_ACTION_REQUIRED = (NCS_CREDENTIALS_REQUIRED + 1),
        NCS_ACTION_REQUIRED_RETRY = (NCS_ACTION_REQUIRED + 1),
        NCS_CONNECT_FAILED = (NCS_ACTION_REQUIRED_RETRY + 1)
    };

    enum NETCON_MEDIATYPE {
        NCM_NONE = 0,
        NCM_DIRECT = (NCM_NONE + 1),
        NCM_ISDN = 2,
        NCM_LAN = (NCM_ISDN + 1),
        NCM_PHONE = (NCM_LAN + 1),
        NCM_TUNNEL = (NCM_PHONE + 1),
        NCM_PPPOE = (NCM_TUNNEL + 1),
        NCM_BRIDGE = (NCM_PPPOE + 1),
        NCM_SHAREDACCESSHOST_LAN = (NCM_BRIDGE + 1),
        NCM_SHAREDACCESSHOST_RAS = (NCM_SHAREDACCESSHOST_LAN + 1)
    };

    enum SHARINGCONNECTIONTYPE : int {
        ICSSHARINGTYPE_PUBLIC = 0,
        ICSSHARINGTYPE_PRIVATE = (ICSSHARINGTYPE_PUBLIC + 1)
    };

    /// <summary>
    /// Manage ICS using COMObject HNetCfg.HNetShare
    /// </summary>
    static class NetConnectionDispatch {
        private static object _netShare;

        static NetConnectionDispatch() {
            Process.Start(new ProcessStartInfo("regsvr32.exe", "/s hnetcfg.dll"));
        }

        public static NetConnectionSharing[] GetAllNetConnections() {
            _netShare = ProgIdInstance("HNetCfg.HNetShare");

            List<NetConnectionSharing> nets = new List<NetConnectionSharing>();
            foreach (var i in EnumEveryConnection()) {
                nets.Add(GetNetConnectionSharingObject(i));
            }
            return nets.ToArray();
        }

        private static NetConnectionSharing GetNetConnectionSharingObject(object i) {
            var ncp = Invoke(_netShare, "NetConnectionProps", new[] { i });
            var inscfinc = Invoke(_netShare, "INetSharingConfigurationForINetConnection", new[] { i });

            NetConnectionSharing netConnection = new NetConnectionSharing() {
                Guid = (string)GetPropertyValue(ncp, "Guid"),
                Name = (string)GetPropertyValue(ncp, "Name"),
                DeviceName = (string)GetPropertyValue(ncp, "DeviceName"),
                Status = (NETCON_STATUS)GetPropertyValue(ncp, "Status"),
                MediaType = (NETCON_MEDIATYPE)GetPropertyValue(ncp, "MediaType"),
                Characteristics = (uint)GetPropertyValue(ncp, "Characteristics"),
                SharingEnabled = (bool)GetPropertyValue(inscfinc, "SharingEnabled"),
                SharingConnectionType = (SHARINGCONNECTIONTYPE)GetPropertyValue(inscfinc, "SharingConnectionType"),
                InternetFirewallEnabled = (bool)GetPropertyValue(inscfinc, "InternetFirewallEnabled")
            };
            return netConnection;
        }

        private static IEnumerable<object> EnumEveryConnection() {
            List<object> result = new List<object>();
            var eec = Invoke(_netShare, "EnumEveryConnection", null);
            int count = (int)Invoke(eec, "Count", null);
            var @enum = Invoke(eec, "_NewEnum", null);
            while (count-- > 0) {
                @enum.GetType().InvokeMember("MoveNext", BindingFlags.InvokeMethod, null, @enum, null);
                var current = @enum.GetType().GetProperties()[0].GetValue(@enum, null);
                result.Add(current);
            }
            return result;
        }

        private static object GetPropertyValue(object target, string propertyName) {
            return target.GetType().InvokeMember(propertyName, BindingFlags.InvokeMethod | BindingFlags.GetProperty, null, target, null, null);
        }

        private static object ProgIdInstance(string progId, params string[] memberNames) {
            Type standardType = Type.GetTypeFromProgID(progId);
            object obj = Activator.CreateInstance(standardType);

            // Make sure it implements IDispatch.
            if (!DispatchUtility.ImplementsIDispatch(obj)) {
                throw new ArgumentException("The object created for " + progId + " doesn't implement IDispatch.");
            }

            // See if we can get Type info and then do some reflection.
            //Type dispatchType = DispatchUtility.GetType(obj, false);

            return obj;
        }

        public static bool EnableSharing(NetConnectionSharing netConnection, int SHARINGCONNECTIONTYPE) {
            Process.Start(new ProcessStartInfo("regsvr32.exe", "/u /s hnetcfg.dll"));
            Process.Start(new ProcessStartInfo("regsvr32.exe", "/s hnetcfg.dll"));
            var INetConnection = EnumEveryConnection().Where(d => GetNetConnectionSharingObject(d).Guid == netConnection.Guid).FirstOrDefault();
            if (INetConnection == null) return false;
            var inscfinc = Invoke(_netShare, "INetSharingConfigurationForINetConnection", new[] { INetConnection });
            var _result = inscfinc.GetType().InvokeMember("EnableSharing", BindingFlags.InvokeMethod, null, inscfinc, new object[] { SHARINGCONNECTIONTYPE }, null);
            return false;
        }

        public static bool DisableSharing(NetConnectionSharing netConnection) {
            Process.Start(new ProcessStartInfo("regsvr32.exe", "/u /s hnetcfg.dll"));
            Process.Start(new ProcessStartInfo("regsvr32.exe", "/s hnetcfg.dll"));
            var INetConnection = EnumEveryConnection().Where(d => GetNetConnectionSharingObject(d).Guid == netConnection.Guid).FirstOrDefault();
            if (INetConnection == null) return false;
            var inscfinc = Invoke(_netShare, "INetSharingConfigurationForINetConnection", new[] { INetConnection });
            var _result = inscfinc.GetType().InvokeMember("DisableSharing", BindingFlags.InvokeMethod, null, inscfinc, null);
            return true;
        }
    }

    /// <summary>
    /// Class to represent INetConnectionProps and INetSharingConfiguration properties
    /// </summary>
    class NetConnectionSharing {
        public string Guid { get; internal set; }
        public string Name { get; internal set; }
        public string DeviceName { get; internal set; }
        public NETCON_STATUS Status { get; internal set; }
        public NETCON_MEDIATYPE MediaType { get; internal set; }
        public uint Characteristics { get; internal set; }
        public bool SharingEnabled { get; internal set; }
        public SHARINGCONNECTIONTYPE SharingConnectionType { get; internal set; }
        public bool InternetFirewallEnabled { get; internal set; }

        public bool StartSharing(int connectionType) {
            return NetConnectionDispatch.EnableSharing(this, connectionType);
        }

        public bool StartSharing(NetConnectionSharing netConnection) {
            NetConnectionDispatch.EnableSharing(this, 0);
            NetConnectionDispatch.EnableSharing(netConnection, 1);
            return false;
        }

        public bool StopSharing() {
            return NetConnectionDispatch.DisableSharing(this);
        }

        public bool StopSharing(NetConnectionSharing netConnection) {
            NetConnectionDispatch.DisableSharing(this);
            NetConnectionDispatch.DisableSharing(netConnection);
            return false;
        }

        public override string ToString() {
            return Name;
        }
    }
    class Program {
        static int Main(string[] args) {
            if (!HasAdministratorPrivileges()) {
                Console.Error.WriteLine("Permission Denied. The operation requires an elevated command prompt.");
                return 740; // ERROR_ELEVATION_REQUIRED
            }
            if (args.Length > 0) {
                if (args[0] == "-l") {
                    foreach (var i in NetConnectionDispatch.GetAllNetConnections()) {
                        Console.WriteLine($"Interface {i.Name}:");
                        Console.WriteLine($"  Guid:                       {i.Guid}");
                        Console.WriteLine($"  DeviceName:                 {i.DeviceName}");
                        Console.WriteLine($"  Status:                     {i.Status}");
                        Console.WriteLine($"  MediaType:                  {i.MediaType}");
                        Console.WriteLine($"  SharingEnabled:             {i.SharingEnabled}");
                        Console.WriteLine($"  SharingConnectionType:      {i.SharingConnectionType}");
                        Console.WriteLine($"  InternetFirewallEnabled:    {i.InternetFirewallEnabled}");
                        Console.WriteLine();
                    }
                } else if (args.Length == 5 &&
                    args[0] == "-from" &&
                    !Regex.IsMatch(args[1], "-[\\/:*?\"<>|]") &&
                    Regex.IsMatch(args[1], "(\"[\\S] + \"|[\\w\\W]+)") &&
                    args[2] == "-to" &&
                    !Regex.IsMatch(args[3], "-[\\/:*?\"<>|]") &&
                    Regex.IsMatch(args[3], "(\"[\\S] + \"|[\\w\\W]+)")) {
                    NetConnectionSharing @public = null;
                    NetConnectionSharing @private = null;
                    foreach (var i in NetConnectionDispatch.GetAllNetConnections()) {
                        if (i.Name == (args[1].StartsWith("\"") ? args[1].Replace("\"", "") : args[1])) {
                            @public = i;
                        }
                        if (i.Name == (args[3].StartsWith("\"") ? args[3].Replace("\"", "") : args[3])) {
                            @private = i;
                        }
                    }
                    if (args[4] == "1") {
                        if (@public != null && @private != null) {
                            @public.StartSharing(@private);
                            //@private.StartSharing(1);
                            Console.WriteLine($"Sharing Internet Connection from {@public} to {@private}");
                        }
                    } else if (args[4] == "0")
                        if (@public != null && @private != null) {
                            @public.StopSharing();
                            @private.StopSharing();
                            Console.WriteLine($"Stopped sharing internet connection from {@public} to {@private}");
                        }
                } else if (args.Length == 2 &&
                     !Regex.IsMatch(args[0], "-[\\/:*?\"<>|]") &&
                     Regex.IsMatch(args[0], "(\"[\\S] + \"|[\\w\\W]+)") &&
                     args[1] == "0") {
                    NetConnectionSharing net = null;
                    foreach (var i in NetConnectionDispatch.GetAllNetConnections()) {
                        if (i.Name == (args[0].StartsWith("\"") ? args[0].Replace("\"", "") : args[0])) {
                            net = i;
                            break;
                        }
                    }
                    if (net != null) {
                        net.StopSharing();
                        Console.WriteLine($"Stopped sharing in {net.Name}");
                    }
                } else if (args.Length == 3 &&
                      !Regex.IsMatch(args[0], "-[\\/:*?\"<>|]") &&
                      Regex.IsMatch(args[0], "(\"[\\S] + \"|[\\w\\W]+)") &&
                      args[1] == "1" && (args[2] == "0" || args[2] == "1")) {
                    NetConnectionSharing net = null;
                    foreach (var i in NetConnectionDispatch.GetAllNetConnections()) {
                        if (i.Name == (args[0].StartsWith("\"") ? args[0].Replace("\"", "") : args[0])) {
                            net = i;
                            break;
                        }
                    }
                    if (net != null) {
                        net.StartSharing(args[2] == "0" ? 0 : 1);
                        Console.WriteLine($"Sharing started in {net.Name}");
                    }
                }
            }

            if (Debugger.IsAttached) {
                Console.Write("Press any key to continue. . . ");
                Console.ReadKey(true);
            }
            return 0;
        }

        private static bool HasAdministratorPrivileges() {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
