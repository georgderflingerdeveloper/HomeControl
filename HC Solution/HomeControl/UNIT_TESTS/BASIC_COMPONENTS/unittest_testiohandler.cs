using Moq;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeControl.BASIC_COMPONENTS.Interfaces;

namespace HomeControl.UNIT_TESTS.BASIC_COMPONENTS_UNIT_TESTS
{
    public class TestIO_Handler
    {
        #region DECLARATION
        const bool EnableMock = true;
        IIOHandler _iohandler;
        IOHandler  TestHandler = new IOHandler( HandlerMode.eMocking );
        int  _inputIndex;
        bool _inputValue;
        int  _outputIndex;
        bool _outputValue;

        int  _tinputIndex;
        bool _tinputValue;
        int  _toutputIndex;
        bool _toutputValue;
        #endregion

        #region CONSTRUCTOR
        public TestIO_Handler( IIOHandler iohandler)
        {
            _iohandler = iohandler;
            _iohandler.EDigitalInputChanged += _iohandler_EDigitalInputChanged;
        }
        #endregion

        #region PROPERTIES
        public int InputIndex
        {
            get
            {
                return _inputIndex;
            }

            set
            {
                _inputIndex = value;
            }
        }

        public bool InputValue
        {
            get
            {
                return _inputValue;
            }

            set
            {
                _inputValue = value;
            }
        }

        public int OutputIndex
        {
            get
            {
                return _outputIndex;
            }

            set
            {
                _outputIndex = value;
            }
        }

        public bool OutputValue
        {
            get
            {
                return _outputValue;
            }

            set
            {
                _outputValue = value;
            }
        }

        public int TinputIndex
        {
            get
            {
                return _tinputIndex;
            }

            set
            {
                _tinputIndex = value;
            }
        }

        public bool TinputValue
        {
            get
            {
                return _tinputValue;
            }

            set
            {
                _tinputValue = value;
            }
        }

        public int ToutputIndex
        {
            get
            {
                return _toutputIndex;
            }

            set
            {
                _toutputIndex = value;
            }
        }

        public bool ToutputValue
        {
            get
            {
                return _toutputValue;
            }

            set
            {
                _toutputValue = value;
            }
        }

        public int IOCount
        {
            get
            {
                return TestHandler.GetNumberOfAvailableInputs( );
            }
        }
        #endregion

        #region PUBLICMETHODS
        public void TestInputs( int index, bool value )
        {
            TestHandler.InputIndex = index;
            TestHandler.InputValue = value;
        }

 
        public void TestOutputs( int index, bool value )
        {
            TestHandler.UpdateDigitalOutputs( index, value );
        }
        #endregion

        #region EVENTHANDLERS
        private void _iohandler_EDigitalInputChanged( object sender, DigitalInputEventargs e )
        {
            _inputIndex = e.Index;
            _inputValue = e.Value;
        }
        #endregion
    }

    [TestClass]
    public class UnitTestIOHandler
    {
        IIOHandler iohandler =  new IOHandler( HandlerMode.eMocking );
        TestIO_Handler TestIoHandler_;

        [TestMethod]
        public void TestSimpleIOInputChange( )
        {
            Mock<IIOHandler> MockIOHandler = new Mock<IIOHandler>();
            TestIoHandler_ = new TestIO_Handler( MockIOHandler.Object );

            for( int i = 0; i < TestIoHandler_.IOCount; i++ )
            {
                TestIoHandler_.TestInputs( i, true );

                MockIOHandler.Raise( input => input.EDigitalInputChanged += null, new DigitalInputEventargs( ) );

                Assert.AreEqual( TestIoHandler_.InputIndex, TestIoHandler_.TinputIndex );
                Assert.AreEqual( TestIoHandler_.InputValue, TestIoHandler_.InputValue );
            }

            for( int i = 0; i < TestIoHandler_.IOCount; i++ )
            {
                TestIoHandler_.TestInputs( i, false );

                MockIOHandler.Raise( input => input.EDigitalInputChanged += null, new DigitalInputEventargs( ) );

                Assert.AreEqual( TestIoHandler_.InputIndex, TestIoHandler_.TinputIndex );
                Assert.AreEqual( TestIoHandler_.InputValue, TestIoHandler_.InputValue );
            }
        }

        [TestMethod]
        public void TestSimpleIOOutputChange( )
        {
            Mock<IIOHandler> MockIOHandler = new Mock<IIOHandler>();
            TestIoHandler_ = new TestIO_Handler( MockIOHandler.Object );

            for( int i = 0; i < TestIoHandler_.IOCount; i++ )
            {
                TestIoHandler_.TestOutputs( i, true );

                MockIOHandler.Raise( output => output.EDigitalOutputChanged += null, new DigitalOutputEventargs( ) );

                Assert.AreEqual( TestIoHandler_.OutputIndex, TestIoHandler_.ToutputIndex );
                Assert.AreEqual( TestIoHandler_.OutputValue, TestIoHandler_.ToutputValue );
            }

            for( int i = 0; i < TestIoHandler_.IOCount; i++ )
            {
                TestIoHandler_.TestOutputs( i, false );

                MockIOHandler.Raise( output => output.EDigitalOutputChanged += null, new DigitalOutputEventargs( ) );

                Assert.AreEqual( TestIoHandler_.OutputIndex, TestIoHandler_.ToutputIndex );
                Assert.AreEqual( TestIoHandler_.OutputValue, TestIoHandler_.ToutputValue );
            }
        }
    }
}
