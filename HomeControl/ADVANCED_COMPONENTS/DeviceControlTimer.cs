using System.Collections.Generic;
using System.Timers;
using BASIC_COMPONENTS;
using HomeControl.ADVANCED_COMPONENTS.Interfaces;
using HomeControl.ADVANCED_COMPONENTS;

namespace HomeControl.ADVANCED_COMPONENTS
{
    public delegate void  ControlOn( object sender, ElapsedEventArgs e );
    public delegate void  ControlOff( object sender, ElapsedEventArgs e );     
    public delegate void  ControlFinalOff( object sender, ElapsedEventArgs e );

    public class DeviceControlTimer : IDeviceControlTimer
    {
        #region DECLARATIONS
        ITimer _TurnOn;                                // idea ist to push f.e a button for a certain time, after that all lights go on
        ITimer _TurnOff;                               // "single" group of devices can be turned off manually
        ITimer _TurnFinalOff;                          // all devices off, even the forced ones

        public event          ControlOn                   EControlOn;
        public event          ControlOff                  EControlOff;
        public event          ControlFinalOff             EControlFinalOff;
        #endregion

        #region CONSTRUCTOR
        public DeviceControlTimer( ) { }

        public DeviceControlTimer( ITimer TurnOff )
        {
            _TurnOff = TurnOff;
            _TurnOff.Elapsed += SingleOff_Elapsed;
        }

        public DeviceControlTimer( ITimer TurnOn, ITimer TurnOff )
        {
            OnOffConstructor( TurnOn, TurnOff );
        }

        public DeviceControlTimer( ITimer TurnOn, ITimer TurnOff, ITimer TurnFinalOff )
        {
            OnOffConstructor( TurnOn, TurnOff );
            _TurnFinalOff = TurnFinalOff;
            _TurnFinalOff.Elapsed += _TurnFinalOff_Elapsed;
        }
        #endregion

        #region PUBLIC_METHODS
        public void StartOn( )
        {
            _TurnOn.Start( );
        }

        public void StartOff( )
        {
            _TurnOff.Start( );
        }

        public void RestartOff( )
        {
            _TurnOff?.Stop( );
            _TurnOff?.Start( );
            _TurnFinalOff?.Stop( );
            _TurnFinalOff?.Start( );
        }

        public void StopOn( )
        {
            _TurnOn.Stop( );
        }

        public void StartAll( )
        {
            _TurnOn.Start( );
            _TurnOff.Start( );
        }

        public void StopAll( )
        {
            _TurnOn.Stop( );
            _TurnOff.Stop( );
        }

        public void StartFinalOff()
        {
            _TurnFinalOff.Start( );
        }
        public void StopFinalOff( )
        {
            _TurnFinalOff.Stop( );
        }
        #endregion

        #region PRIVATEMETHODS
        void StartSingleOff( )
        {
            _TurnOff.Start( );
        }

        void StopSingleOff( )
        {
            _TurnOff.Stop( );
        }

        void OnOffConstructor( ITimer TurnOn, ITimer TurnOff )
        {
            _TurnOn            = TurnOn;
            _TurnOn.Elapsed   += DelayAllOn_Elapsed;
            _TurnOff           = TurnOff;
            _TurnOff.Elapsed += SingleOff_Elapsed;
        }
        #endregion

        #region EVENTHANDLERS

        void SingleOff_Elapsed( object sender, ElapsedEventArgs e )
        {
            _TurnOff.Stop( );
            EControlOff?.Invoke( this, e );
        }

        void DelayAllOn_Elapsed( object sender, ElapsedEventArgs e )
        {
            _TurnOn.Stop( );
            EControlOn?.Invoke( this, e );
        }

        private void _TurnFinalOff_Elapsed( object sender, ElapsedEventArgs e )
        {
            _TurnFinalOff.Stop( );
            EControlFinalOff?.Invoke( this, e );
        }
        #endregion
    }
}
