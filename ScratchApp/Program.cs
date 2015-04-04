using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp1
{
    public partial class Program
    {
        GT.Timer timer;

        //System.Threading.Thread thread;

        TimeSpan timerInterval = new TimeSpan(0, 0, 0, 0, 100);
        TimeSpan animateInterval = new TimeSpan(0, 0, 0, 0, 1000);

        //GTM.IanLee.IO60P16.IO60P16Module io60p16d;
        //GTM.IanLee.IO60P16.OutputPort op00;
        //GTM.IanLee.IO60P16.PWM pwm15;

        IO60P16.OutputPort[] outputPorts = new IO60P16.OutputPort[64];
        IO60P16.PWM[] pwmPorts = new IO60P16.PWM[16];

        byte[] outputPortStates = new byte[64];


        void InitializeOutputPort(int portIdx, int pinIdx, bool initialState)
        {
            IO60P16.IOPin ioPin = (IO60P16.IOPin)(portIdx<<4 | pinIdx);
            outputPorts[portIdx<<3 | pinIdx] = new IO60P16.OutputPort(ioPin, initialState);
            outputPortStates[portIdx<<3 | pinIdx] = (byte)(initialState ? 1 : 0);
        }

        void InitializePWMPort( int pwmIdx, IO60P16.PWM.TickWidth tickWidth )
        {
            int portIdx = (pwmIdx>>3) + 6;
            int pinIdx = pwmIdx & 0x7;
            IO60P16.PWMPin pwmPin = (IO60P16.PWMPin)(portIdx<<4 | pinIdx);
            pwmPorts[pwmIdx] = new IO60P16.PWM(pwmPin, tickWidth);
            outputPortStates[portIdx<<3 | pinIdx] = 0;
        }

        void SetPWMPortDutyCycle(int pwmIdx, byte dutyCycle)
        {
            //Debug.Print("DutyCycle:" + dutyCycle);
            dutyCycle = dutyCycle > 254 ? (byte)254 : dutyCycle;
            pwmPorts[pwmIdx].Set((double)dutyCycle / 255.0);
            int portIdx = (pwmIdx>>3) + 6;
            int pinIdx = pwmIdx & 0x7;
            outputPortStates[portIdx << 3 | pinIdx] = dutyCycle;
        }

        byte GetPWMPortDutyCycle(int pwmIdx)
        {
            int portIdx = (pwmIdx>>3) + 6;
            int pinIdx = pwmIdx & 0x7;
            return outputPortStates[portIdx << 3 | pinIdx];
        }

        void SetOutputPortState(int portIdx, int pinIdx, bool state)
        {
            outputPorts[portIdx << 3 | pinIdx].Write(state);
            outputPortStates[portIdx << 3 | pinIdx] = (byte)(state ? 1 : 0);
        }

        bool GetOutputPortState(int portIdx, int pinIdx)
        {
            return outputPortStates[portIdx << 3 | pinIdx] != 0;
        }

        void Print(byte row, byte col, byte width, string str)
        {
            display_HD44780.SetCursor(row, col);
            int spaceCount = width - str.Length;
            if (spaceCount > 0)
            {
                display_HD44780.PrintString( new string(' ', spaceCount ) );
            }

            display_HD44780.PrintString( str.Substring( 0, width < str.Length ? width : str.Length ) );
        }


        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {


            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/

            timer = new GT.Timer(timerInterval, GT.Timer.BehaviorType.RunContinuously);
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);

            //compass.ContinuousMeasurementInterval = animateInterval;
            //compass.SetGain(Compass.Gain.Gain6);
            //compass.MeasurementComplete += new Compass.MeasurementCompleteEventHandler(compass_MeasurementComplete);

            io60p16.Reset();

            InitializePWMPort(15, IO60P16.PWM.TickWidth.TickWidth_Servo_23438hz_42666ns);
            InitializeOutputPort(0, 7, false);

            display_HD44780.Initialize();

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            PulseDebugLED();
            timer.Start();

            joystick.JoystickReleased += new Joystick.JoystickEventHandler(joystick_JoystickReleased);

            //compass.StartContinuousMeasurements();

            //display_HD44780.Initialize();

            display_HD44780.SetBacklight(true);
            display_HD44780.CursorHome();
            

            //thread = new Thread(CounterLoop);
            //thread.Start();

            Print(0, 0, 3, "0");
            Print(1, 0, 3, "Off");
            Print(1, 5, 9, "Hej Moffe");

        }


        void joystick_JoystickReleased(Joystick sender, Joystick.JoystickState state)
        {
            PulseDebugLED();

            bool newState = !GetOutputPortState( 0, 7 );
            SetOutputPortState( 0, 7, newState);

            Print(1, 0, 3, newState ? "On" : "Off");
        }

        //void compass_MeasurementComplete(Compass sender, Compass.SensorData sensorData)
        //{
        //    display_HD44780.Clear();
        //    display_HD44780.CursorHome();
        //    display_HD44780.PrintString( "Angle:" + sensorData.Angle);
        //    //Debug.Print("sensor data: " + sensorData.ToString());
        //}

        double joystickCenterEpsilon = 0.1;

        int timerCount = 0;

        //void CounterLoop()
        //{
        //    DateTime start = DateTime.Now;
        //    while (timerCount < 1000000)
        //    {
        //        timerCount++;
        //    }
        //    DateTime end = DateTime.Now;

        //    TimeSpan s = end.Subtract(start);

        //    Debug.Print(s.ToString());
        //}


        //int lastTimerCount = 0;
        //int currentLed = 0;

        long animateAccum = 0;


        void timer_Tick(GT.Timer timer)
        {
            var jpos = joystick.GetJoystickPosition();

            var y = (jpos.Y - 0.5) * 2;
            var x = (jpos.X - 0.5) * 2;

            var speed = System.Math.Sqrt(y * y + x * x);
            speed = speed > 1.0 ? 1.0 : speed;

            if (speed < joystickCenterEpsilon)
            {
                byte dutyCycle = GetPWMPortDutyCycle(15);
                if (dutyCycle > 0)
                {
                    --dutyCycle;
                    Print(0, 0, 3, dutyCycle.ToString());
                    SetPWMPortDutyCycle(15, dutyCycle);
                }
            }
            else
            {
                byte dutyCycle = (byte)(speed * 255);
                Print(0, 0, 3, dutyCycle.ToString());
                SetPWMPortDutyCycle(15, dutyCycle);
            }
        }
        //    animateAccum += timerInterval.Ticks;
        //    bool animate = false;
        //    if (animateAccum > animateInterval.Ticks)
        //    {
        //        animate = true;
        //        animateAccum -= animateInterval.Ticks;
         
        //        string s = "light percentage : " + lightSensor.ReadLightSensorPercentage() + "\n";
        //        s += "light voltage : " + lightSensor.ReadLightSensorVoltage();


        //        Debug.Print("light percentage : " + lightSensor.ReadLightSensorPercentage());
        //        Debug.Print("light voltage : " + lightSensor.ReadLightSensorVoltage());

        //        display_HD44780.Clear();
        //        display_HD44780.CursorHome();
        //        display_HD44780.PrintString(s);

        //    }

        //    //if (timerCount - lastTimerCount >= 1000)
        //    //{
        //    //    lastTimerCount = timerCount;
        //    //    currentLed = (currentLed + 1) % 7;
        //    //    led7r.TurnLightOn(currentLed + 1, true);
        //    //}
            
        //    var jpos = joystick.GetJoystickPosition();

        //    var y = jpos.Y - 0.5;
        //    var x = jpos.X - 0.5;

        //    var speed = System.Math.Sqrt(y * y + x * x)*2;

        //    if (speed < joystickCenterEpsilon)
        //    {
        //        led7r.TurnLightOn(7, true);
        //        if (animate)
        //            led7r.Animate( (int)(timerInterval.Ticks/TimeSpan.TicksPerMillisecond) / 7, false, true, false);
        //    }
        //    else
        //    {
        //        x = x/speed;
        //        y = y/speed;

        //        var angle = System.Math.Atan2(y, x);

        //        int ledNo = (int)System.Math.Floor((((angle / System.Math.PI) + 1.0) / 2.0) * 6.0)+1;

        //        //Debug.Print("ledno : " + ledNo);


        //        led7r.TurnLightOn(ledNo, true);
        //    }

        //    //Debug.Print( "light: " + lightSensor.ReadLightSensorPercentage() + ", " + lightSensor.ReadLightSensorVoltage() );
        //}


    }
}
