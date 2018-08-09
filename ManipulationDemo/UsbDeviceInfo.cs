using System;
using System.Collections.Generic;
using System.Management;

namespace ManipulationDemo
{
    class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }

        public USBDeviceInfo(Exception exception)
        {
            _exception = exception;
        }

        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }

        private Exception _exception;

        public override string ToString()
        {
            if (_exception is null)
            {
                return $@"- {DeviceID} ({Description})";
            }
            else
            {
                return _exception.ToString();
            }
        }

        public static List<USBDeviceInfo> GetAll()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (var device in collection)
                {
                    try
                    {
                        devices.Add(new USBDeviceInfo(
                            (string)device.GetPropertyValue("DeviceID"),
                            (string)device.GetPropertyValue("PNPDeviceID"),
                            (string)device.GetPropertyValue("Description")
                        ));
                    }
                    catch (Exception ex)
                    {
                        devices.Add(new USBDeviceInfo(ex));
                    }
                }
            }

            return devices;
        }
    }
}
