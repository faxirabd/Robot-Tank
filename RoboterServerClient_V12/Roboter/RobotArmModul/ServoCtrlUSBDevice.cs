using Pololu.UsbWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roboter.RobotArmModul
{
    ///<summary>
    /// These are the values to put in to bRequest when making a setup packet
    /// for a control transfer to the Maestro.  See the comments and code in Usc.cs
    /// for more information about what these requests do and the format of the
    /// setup packet.
    ///</summary>
    public enum uscRequest
    {
        REQUEST_GET_PARAMETER = 0x81,
        REQUEST_SET_PARAMETER = 0x82,
        REQUEST_GET_VARIABLES = 0x83,
        REQUEST_SET_SERVO_VARIABLE = 0x84,
        REQUEST_SET_TARGET = 0x85,
        REQUEST_CLEAR_ERRORS = 0x86,
        REQUEST_GET_SERVO_SETTINGS = 0x87,

        // GET STACK and GET CALL STACK are only used on the Mini Maestro.
        REQUEST_GET_STACK = 0x88,
        REQUEST_GET_CALL_STACK = 0x89,
        REQUEST_SET_PWM = 0x8A,

        REQUEST_REINITIALIZE = 0x90,
        REQUEST_ERASE_SCRIPT = 0xA0,
        REQUEST_WRITE_SCRIPT = 0xA1,
        REQUEST_SET_SCRIPT_DONE = 0xA2, // value.low.b is 0 for go, 1 for stop, 2 for single-step
        REQUEST_RESTART_SCRIPT_AT_SUBROUTINE = 0xA3,
        REQUEST_RESTART_SCRIPT_AT_SUBROUTINE_WITH_PARAMETER = 0xA4,
        REQUEST_RESTART_SCRIPT = 0xA5,
        REQUEST_START_BOOTLOADER = 0xFF
    }

    class ServoCtrlUSBDevice : UsbDevice
    {

        /// <summary>
        /// The device interface GUID used to detect the native USB interface
        /// of the Maestro Servo Controllers in windows.
        /// </summary>
        /// <remarks>From maestro.inf.</remarks>
        public static Guid deviceInterfaceGuid = new Guid("e0fbe39f-7670-4db6-9b1a-1dfb141014a7");

        /// <summary>Pololu's USB vendor id.</summary>
        /// <value>0x1FFB</value>
        public const ushort vendorID = 0x1ffb;

        /// <summary>The Micro Maestro's product ID.</summary>
        /// <value>0x0089</value>
        public static ushort[] productIDArray = new ushort[] { 0x0089, 0x008a, 0x008b, 0x008c };

        public ServoCtrlUSBDevice()
            : base(null)
        {
        }

        /// <summary>
        /// The number of servos on the device.  This will be 6, 12, 18, or 24.
        /// </summary>
        public readonly byte servoCount;

        public ServoCtrlUSBDevice(DeviceListItem deviceListItem)
            : base(deviceListItem)
        {
            // Determine the number of servos from the product id.
            switch (getProductID())
            {
                case 0x89: servoCount = 6; break;
                case 0x8A: servoCount = 12; break;
                case 0x8B: servoCount = 18; break;
                case 0x8C: servoCount = 24; break;
                default: throw new Exception("Unknown product id " + getProductID().ToString("x2") + ".");
            }
        }

        /// <summary>
        /// Connects to a Maestro using native USB and returns the Usc object
        /// representing that connection.  When you are done with the
        /// connection, you should close it using the Dispose() method so that
        /// other processes or functions can connect to the device later.  The
        /// "using" statement can do this automatically for you.
        /// </summary>
        public static ServoCtrlUSBDevice connectToDevice()
        {

            // Get a list of all connected devices of this type.
            List<DeviceListItem> connectedDevices = ServoCtrlUSBDevice.getConnectedDevices();

            foreach (DeviceListItem dli in connectedDevices)
            {
                // If you have multiple devices connected and want to select a particular
                // device by serial number, you could simply add a line like this:
                //   if (dli.serialNumber != "00012345"){ continue; }

                ServoCtrlUSBDevice device = new ServoCtrlUSBDevice(dli); // Connect to the device.
                return device;             // Return the device.
            }
            throw new Exception("Could not find device.  Make sure it is plugged in to USB " +
                "and check your Device Manager (Windows) or run lsusb (Linux).");
        }

        public static List<DeviceListItem> getConnectedDevices()
        {
            try
            {
                return UsbDevice.getDeviceList(ServoCtrlUSBDevice.deviceInterfaceGuid);
            }
            catch (NotImplementedException)
            {
                // use vendor and product instead
                return UsbDevice.getDeviceList(ServoCtrlUSBDevice.vendorID, ServoCtrlUSBDevice.productIDArray);
            }
        }

        public void setTarget(byte servo, ushort value)
        {
            try
            {
                controlTransfer(0x40, (byte)uscRequest.REQUEST_SET_TARGET, value, servo);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to set target of servo " + servo + " to " + value + ".", e);
            }
        }

        public void setSpeed(byte servo, ushort value)
        {
            try
            {
                controlTransfer(0x40, (byte)uscRequest.REQUEST_SET_SERVO_VARIABLE, value, servo);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to set speed of servo " + servo + " to " + value + ".", e);
            }
        }
    }
}
