using Moq;
using HomeControl.ADVANCED_COMPONENTS;
using HomeControl.BASIC_CONSTANTS;
using BASIC_COMPONENTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Timers;


namespace HomeControl.UNIT_TESTS.ADVANCED_COMPONENTS
{
    [TestClass]
    public class unittest_devicesequencer
    {
        Mock<ITimer> _PulseOnTimer           = new Mock<ITimer>();
        Mock<ITimer> _PulseOffTimer          = new Mock<ITimer>();
        Mock<ITimer> _NextSequenceTimer      = new Mock<ITimer>();
        Mock<ITimer> _SequencerFinishedTimer = new Mock<ITimer>();

        DeviceSignalSequencer              Sequencer;
        DeviceSignalSequencerConfiguration Config;
        SequencerState                     TestState;
        bool                               TestValue;
        decimal                            ActualCountTestSignal;
        decimal                            ActualSequence;
        int                                TestIndex         = 0;
        decimal                            TestCountSignals  = 5;
        decimal                            TestCountSequencs = 100;
        decimal                            RestartCountValue = 1;
        decimal                            RestartSequenceValue = 1;

        void SingleSequencer( )
        {
            Config = new DeviceSignalSequencerConfiguration( TestIndex, SequencerModi.Single )
            {
                SequencerProperty = SequencerProperty.StartWithHighPulse,
                CountSequenceSignals = TestCountSignals
            };

            Sequencer = new DeviceSignalSequencer( Config, _PulseOnTimer.Object, _PulseOffTimer.Object );
            Sequencer.EUpdate += Sequencer_EUpdate;
        }

        [TestMethod]
        public void TestSingleSequencerINACTIVE( )
        {
            SingleSequencer( );

            Assert.AreEqual( SequencerState.Inactive, TestState );

            Sequencer.Start( );
            Assert.IsTrue( TestValue );
        }

        [TestMethod]
        public void TestSingleSequencerTOGGLE( )
        {
            SingleSequencer( );
            Sequencer.Start( );
            Assert.IsTrue( TestValue );

            _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsFalse( TestValue );

            _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            Assert.IsTrue( TestValue );
        }

        [TestMethod]
        public void TestSingleSequencerTOGGLEinLOOP( )
        {
            SingleSequencer( );
            Sequencer.Start( );

            for( int i = 0; i < TestCountSignals; i++ )
            {
                _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                Assert.IsFalse( TestValue );

                if( i == TestCountSignals )
                {
                    break;
                }

                _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                Assert.IsTrue( TestValue );
            }
        }

        void Toggle()
        {
            for( int i = 0; i < TestCountSignals; i++ )
            {
                _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );

                if( i == TestCountSignals )
                {
                    break;
                }

                _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }
        }

        [TestMethod]
        public void TestSingleSequencerTOGGLEinLOOP_FINISHED( )
        {
            SingleSequencer( );
            Sequencer.Start( );

            Toggle( );

            Assert.AreEqual( SequencerState.Finished, TestState );
        }

        [TestMethod]
        public void TestSingleSequencerTOGGLEinLOOP_COUNT( )
        {
            SingleSequencer( );
            Sequencer.Start( );

            for( int i = 0; i < TestCountSignals; i++ )
            {
                _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );

                Assert.AreEqual( i + 1, ActualCountTestSignal );

                _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }
        }

        void SequencerMulti_()
        {
            Config = new DeviceSignalSequencerConfiguration( TestIndex, SequencerModi.MultipleCount )
            {
                SequencerProperty      = SequencerProperty.StartWithHighPulse,
                CountSequenceSignals   = TestCountSignals,
                CountSequencePeriodes  = TestCountSequencs
            };

            Sequencer = new DeviceSignalSequencer( Config, _PulseOffTimer.Object, _PulseOnTimer.Object, _NextSequenceTimer.Object );
            Sequencer.EUpdate += Sequencer_EUpdate;

            Sequencer.Start( );
        }
 
        [TestMethod]
        public void TestMultipleSequencesWithCount_RUNNING( )
        {
            SequencerMulti_( );

            Assert.AreEqual( SequencerState.Running, TestState );
            _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            _PulseOnTimer.Verify( s => s.Stop( ) );
            _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            _PulseOnTimer.Verify( s => s.Start() );
        }

        void StartOneSequence()
        {
            // SEQUENCE STARTED
            for( int j = 0; j < TestCountSignals; j++ )
            {
                _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                if( j == TestCountSignals )
                {
                    break;
                }
                _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }
        }

        void SingleStop()
        {
            SingleSequencer( );

            Sequencer.Stop( );
        }

        [TestMethod]
        public void TestSingle_Stop( )
        {
            SingleStop( );

            _PulseOnTimer.Verify( s => s.Stop( ) );
            _PulseOffTimer.Verify( s => s.Stop( ) );
        }

        [TestMethod]
        public void TestSingle_Stop_State( )
        {
            SingleStop( );

            Assert.AreEqual( SequencerState.Finished, TestState );
        }

        [TestMethod]
        public void TestSingle_Stop_Signal( )
        {
            SingleStop( );

            Assert.IsFalse( TestValue );
        }

        [TestMethod]
        public void TestSingle_Stop_Count( )
        {
            SingleStop( );

            Assert.AreEqual( 0, ActualCountTestSignal );
        }

        void StopMulti()
        {
            SequencerMulti_( );
            Sequencer.Stop( );
        }

        [TestMethod]
        public void TestMulti_Stop( )
        {
            StopMulti( );
            _NextSequenceTimer.Verify( s => s.Stop( ) );
        }

        [TestMethod]
        public void TestMulti_Stop_State( )
        {
            StopMulti( );
            Assert.AreEqual( SequencerState.Finished, TestState );
        }

        [TestMethod]
        public void TestMulti_Stop_Count( )
        {
            StopMulti( );
            Assert.AreEqual( 0, ActualCountTestSignal );
        }

        [TestMethod]
        public void TestMulti_Stop_Signal( )
        {
            StopMulti( );
            Assert.IsFalse( TestValue );
        }

        [TestMethod]
        public void TestMulti_Stop_Sequence( )
        {
            StopMulti( );
            Assert.AreEqual( 1, ActualSequence );
        }


        [TestMethod]
        public void TestMultipleSequencesWithCount_WAITFORNEXTSEQUENCE( )
        {
            SequencerMulti_( );

            StartOneSequence( );

            Assert.AreEqual( SequencerState.WaitForNextSequence, TestState );
            _NextSequenceTimer.Verify( s => s.Start( ) );
        }

        [TestMethod]
        public void TestMultiple_FirstSequence_Count()
        {
            SequencerMulti_( );

            StartOneSequence( );

            Assert.AreEqual( RestartCountValue, ActualCountTestSignal );
        }

        [TestMethod]
        public void TestMultiple_FirstSequence_State( )
        {
            SequencerMulti_( );

            StartOneSequence( );

            Assert.AreEqual( SequencerState.WaitForNextSequence, TestState );
        }

        [TestMethod]
        public void TestMultiple_NextSequence_Count( )
        {
            SequencerMulti_( );

            StartOneSequence( );

            _NextSequenceTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );

            Assert.AreEqual( 2, ActualSequence );
        }

        [TestMethod]
        public void TestMultiple_NextSequence_State( )
        {
            SequencerMulti_( );

            StartOneSequence( );

            _NextSequenceTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );

            Assert.AreEqual( SequencerState.Running, TestState );
        }

        void Sequences()
        {
            for( int i = 0; i < TestCountSequencs; i++ )
            {
                _NextSequenceTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                // SEQUENCE STARTED
                for( int j = 0; j < TestCountSignals; j++ )
                {
                    _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                    if( j == TestCountSignals )
                    {
                        break;
                    }
                    _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                }
            }
        }

        [TestMethod]
        public void TestMultiple_NextSequence_State_Finished( )
        {
            SequencerMulti_( );

            Sequences( );

            Assert.AreEqual( SequencerState.Finished, TestState );
        }

        [TestMethod]
        public void TestMultiple_NextSequence_Finished_Count( )
        {
            SequencerMulti_( );

            Sequences( );

            Assert.AreEqual( 1, ActualSequence );
        }

        [TestMethod]
        public void TestMultiple_NextSequence_Finished_CountValue( )
        {
            SequencerMulti_( );

            for( int i = 0; i < TestCountSequencs; i++ )
            {
                StartOneSequence( );

                _NextSequenceTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }

            Assert.AreEqual( RestartCountValue, ActualCountTestSignal );
        }


        [TestMethod]
        public void TestMultiple_NextSequence_SignalCount( )
        {
            SequencerMulti_( );

            StartOneSequence( );

            _NextSequenceTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );

            Assert.AreEqual( RestartCountValue, ActualCountTestSignal );
        }

        [TestMethod]
        public void TestMultipleSequencesWithCount_FirstStep( )
        {
            SequencerMulti_( );

            for( int j = 0; j < TestCountSignals; j++ )
            {
                _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                if( j == TestCountSignals -1  )
                {
                    break;
                }
                _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }
            Assert.IsFalse( TestValue );
        }

        [TestMethod]
        public void TestMultipleSequencesWithCount_WAITFORNEXTSEQUENCE_RestartCounts( )
        {
            SequencerMulti_( );

            for( int i = 0; i < TestCountSequencs; i++ )
            {
                StartOneSequence( );
                Assert.AreEqual( RestartCountValue, ActualCountTestSignal );
                _NextSequenceTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }
        }

        void FirstSignalLoop()
        {
            for( int i = 1; i < TestCountSignals; i++ )
            {
                _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
            }
        }

        [TestMethod]
        public void TestMultipleSequencesWithCount_COUNT_SIGNALES( )
        {
            SequencerMulti_( );

            Assert.AreEqual( 1, ActualCountTestSignal );

            FirstSignalLoop( );

            Assert.AreEqual( TestCountSignals, ActualCountTestSignal );
        }

        [TestMethod]
        public void TestMultipleSequencesWithCount_COUNT_SIGNALES_WITH_OVERRUN( )
        {
            SequencerMulti_( );
            FirstSignalLoop( );

            for( int j = 0; j < TestCountSequencs; j++ )
            {
                for( int i = 1; i <= TestCountSignals; i++ )
                {
                    _PulseOnTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                    _PulseOffTimer.Raise( timer => timer.Elapsed += null, new System.EventArgs( ) as ElapsedEventArgs );
                    Assert.AreEqual( i, ActualCountTestSignal );
                }
                Assert.AreEqual( TestCountSignals, ActualCountTestSignal );
            }
        }

        private void Sequencer_EUpdate( object sender, SeqUpdateEventArgs e )
        {
            TestState                = e.State;
            TestValue                = e.Value;
            ActualCountTestSignal    = e.ActualCountSingleONSignals;
            ActualSequence           = e.ActualCountSequences;
        }
    }
}
