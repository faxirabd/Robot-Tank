using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using System.Windows.Threading;
using System.Windows;
using SlimDX;

namespace Client.ServoMotorModul
{
    class GamePadController
    {
        Joystick joystick;

        JoystickState state = new JoystickState();
        DispatcherTimer timer = new DispatcherTimer();
        MotorWnd motorwnd = null;
        UdpSender udpsender = null;
        byte[] buff = null;

        public GamePadController(UdpSender udpsender, MotorWnd motorwnd, byte[] buff)
        {
            this.udpsender = udpsender;
            this.motorwnd = motorwnd;
            this.buff = buff;
        }

        public JoystickState State
        {
            get { return state; }
            set { state = value; }
        }

        public void Start()
        {
            // make sure that DirectInput has been initialized
            DirectInput dinput = new DirectInput();

            // search for devices
            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    joystick = new Joystick(dinput, device.InstanceGuid);
                    //joystick.SetCooperativeLevel(frm1, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);
                    break;
                }
                catch (DirectInputException)
                {
                }
            }

            if (joystick == null)
            {
                MessageBox.Show("There are no joysticks attached to the system.");
                return;
            }

            foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);

            }

            // acquire the device
            joystick.Acquire();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        public void ReadImmediateData()
        {
            if (joystick.Acquire().IsFailure)
                return;

            if (joystick.Poll().IsFailure)
                return;

            state = joystick.GetCurrentState();

            if (Result.Last.IsFailure)
                return;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.ReadImmediateData();
            switch (state.X)
            {
                case 1000:
                    //motorwnd.Title = "Right"; Turn right
                        buff[0] = 2;
                        buff[2] = 1;
                    break;
                case -1000:
                    //motorwnd.Title = "Left"; Turn Left
                        buff[0] = 1;
                        buff[2] = 2;
                    break;
            }

            switch (state.Y)
            {
                case 1000:
                    //motorwnd.Title = "Down"; BWD
                        buff[0] = 2;
                        buff[2] = 2;
                    break;
                case -1000:
                        //motorwnd.Title = "Up"; FWD
                        buff[0] = 1;
                        buff[2] = 1;
                    break;
            }

            if ((state.X == -8) && (state.Y == -8))
            {
                //motorwnd.Title = "neutral";
                buff[1] = 0;
                buff[3] = 0;
                udpsender.Send(buff);
                ActualizeKoordinate();
            }

            int index=0;
            foreach (bool act in this.State.GetButtons())
            {
                if (act)
                {
                    switch (index)
                    {
                        case 0://Up
                                buff[1]++;
                                buff[3]++;
                                udpsender.Send(buff);
                                ActualizeKoordinate();
                            break;
                        case 1://Right
                            break;
                        case 2://Down
                                buff[1]--;
                                buff[3]--;
                                udpsender.Send(buff);
                                ActualizeKoordinate();
                            break;
                        case 3://Left
                            break;
                    }
                }
                    index++;
            }
        }

        void ActualizeKoordinate()
        {
            motorwnd.Title = "(" + buff[1] + ", " + buff[3] + ")";
        }
    }
}
