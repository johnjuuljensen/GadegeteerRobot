﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GadgeteerApp1 {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Joystick module using socket 14 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Joystick joystick;
        
        /// <summary>The IO60P16 module using socket 5 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.IO60P16 io60p16;
        
        /// <summary>The Display_HD44780 module using socket 8 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Display_HD44780 display_HD44780;
        
        /// <summary>The USB Client SP module using socket 2 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.USBClientSP usbClientSP2;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZHydra Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZHydra)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZHydra();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.joystick = new GTM.GHIElectronics.Joystick(14);
            this.io60p16 = new GTM.GHIElectronics.IO60P16(5);
            this.display_HD44780 = new GTM.GHIElectronics.Display_HD44780(8);
            this.usbClientSP2 = new GTM.GHIElectronics.USBClientSP(2);
        }
    }
}