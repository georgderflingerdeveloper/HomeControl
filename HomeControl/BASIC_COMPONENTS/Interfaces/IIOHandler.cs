using BASIC_COMPONENTS;


namespace HomeControl.BASIC_COMPONENTS.Interfaces
{
    public interface IIOHandler
    {
        void  UpdateDigitalOutputs( int index, bool value );
        event DigitalInputChanged  EDigitalInputChanged;
        event DigitalOutputChanged EDigitalOutputChanged;
    }
}
