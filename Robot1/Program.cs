using System;
using System.Collections;
using System.Threading;
using SPOT = Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.SocketInterfaces;

using RobotUtils;
using RobotUtils.SensorAnalysis;

namespace GadgeteerApp1
{


    public class WheelConfig
    {
        public DigitalOutput Dir;
        public PwmOutput PWM;
        public InterruptInput EncoderA;
        public InterruptInput EncoderB;
    }

    public class WheelState
    {
        public WheelConfig Config;
        public QuadratureEncoder QuadratureEncoder;
        public double MeasuredVelocity;
    }

    public class Robot
    {
        public WheelState[] WheelState;

        double m_lastVelocity;

        public Robot(
            WheelConfig[] wheels )
        {
            //if (wheels.Length != 4)
            //    throw new System.ArgumentOutOfRangeException( "wheels", "Exactly 4 wheel configs must be provided" );

            WheelState = new WheelState[wheels.Length];
            for (int iloop = 0; iloop < wheels.Length; iloop++)
            {
                int i = iloop;
                var wc = wheels[i];

                bool a = wc.EncoderA.Read();
                bool b = wc.EncoderB.Read();

                var ws = WheelState[i] = new WheelState
                {
                    Config = wc,
                    QuadratureEncoder = new QuadratureEncoder( a, b )
                };

                wc.PWM.Set( (uint)10000, 0, PwmScaleFactor.Microseconds );
                wc.Dir.Write( false );

                wc.EncoderA.Interrupt += ( sender, value ) =>
                {
                    var pulseTime = SPOT.Hardware.Utility.GetMachineTime();
                    ws.QuadratureEncoder.AddPulse( value ? Pin.High : Pin.Low, Pin.Unchanged, pulseTime );
                    //SPOT.Debug.Print( "Wheel " + i + ", Encoder A: " + value );
                };

                wc.EncoderB.Interrupt += ( sender, value ) =>
                {
                    var pulseTime = SPOT.Hardware.Utility.GetMachineTime();
                    ws.QuadratureEncoder.AddPulse( value ? Pin.High : Pin.Low, Pin.Unchanged, pulseTime );
                    //SPOT.Debug.Print( "Encoder B: " + value );
                    SPOT.Debug.Print( "Last change: " + ws.QuadratureEncoder.LastChange );
                };

                SPOT.Debug.Print( "Wheel " + i + " initialized" );
                SPOT.Debug.Print( "Encoder A: " + a );
                SPOT.Debug.Print( "Encoder B: " + b );
            }

        }


        public void Set( double velocity )
        {
            if (velocity != m_lastVelocity)
            {
                //SPOT.Debug.Print( "Velocity: " + velocity );
                var speed = Math.Abs( velocity );
                foreach (var w in WheelState)
                {
                    var wc = w.Config;
                    wc.Dir.Write( velocity > 0 ? true : false );
                    wc.PWM.Set( (uint)10000, (uint)(speed * 10000), PwmScaleFactor.Microseconds );
                }
                m_lastVelocity = velocity;
            }
        }
    }

    public partial class Program
    {
        Robot robot;
        DigitalOutput referenceVoltage;


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


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            SPOT.Debug.Print( "Program Started" );

            referenceVoltage = io60P16.CreateDigitalOutput( 0, 7, true );

            var socket = GT.Socket.GetSocket( 8, true, null, null );
            socket.EnsureTypeIsSupported( 'Y', null );

            //for (int i = 3; i <= 9; ++i) {
            //    int pin = i;
            //    int count = 0;
            //    var intr = InterruptInputFactory.Create( socket, (GT.Socket.Pin)pin, GlitchFilterMode.Off, ResistorMode.Disabled, InterruptMode.RisingAndFallingEdge, null );
            //    intr.Interrupt += ( sender, value ) =>
            //    {
            //        count++;
            //        SPOT.Debug.Print( "pin:" + pin + ", count;" + count + "      " + value );
            //    };

            //}

            var wheels = new WheelConfig[1]; for (int i = 0; i < 1; i++)
                wheels[i] = new WheelConfig
                {
                    Dir = io60P16.CreateDigitalOutput( 0, i, false ),
                    PWM = io60P16.CreatePwmOutput( 8 + i ),
                    EncoderA = InterruptInputFactory.Create( socket, (GT.Socket.Pin)3, GlitchFilterMode.Off, ResistorMode.Disabled, InterruptMode.RisingAndFallingEdge, null ),
                    EncoderB = InterruptInputFactory.Create( socket, (GT.Socket.Pin)4, GlitchFilterMode.Off, ResistorMode.Disabled, InterruptMode.RisingAndFallingEdge, null )
                };

            robot = new Robot( wheels );

            var timer = new GT.Timer( 100 );
            timer.Tick += timer_Tick;
            timer.Start();
        }


        void timer_Tick( GT.Timer timer )
        {
            var position = joystick.GetPosition();

            var velocity = position.Y;
            var speed = Math.Abs( velocity );
            if (speed < 0.05)
            {
                velocity = 0;
                speed = 0;
            }

            led7R.SetLeds( (int)(8 * speed) );
            //robot.Set( velocity );

            //SPOT.Debug.Print( "Encoder: A=" + robot.WheelState[0].PulseCount );

            //SPOT.Debug.Print( "x: " + Robot.x );



        }
    }
}
