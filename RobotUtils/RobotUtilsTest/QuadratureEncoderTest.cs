using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotUtils;
using RobotUtils.SensorAnalysis;

namespace SensorAnalysisTest
{
    [TestClass]
    public class QuadratureEncoderTest
    {
        void AssertValidState(QuadratureEncoder qe)
        {
            Assert.AreNotEqual( 2, qe.LastChange );
        }

        [TestMethod]
        public void TestOneDirection()
        {
            var qe = new QuadratureEncoder( false, false );

            var milliSecond = TimeSpan.FromTicks( TimeSpan.TicksPerMillisecond);
            var pulseTime = TimeSpan.Zero;
            for ( int i = 0; i < 10; ++i )
            {
                qe.AddPulse( Pin.Unchanged, Pin.High, pulseTime += milliSecond );
                qe.LastChange.Should().Be( 1 );
                qe.AddPulse( Pin.High, Pin.Unchanged, pulseTime += milliSecond );
                qe.LastChange.Should().Be( 1 );
                qe.AddPulse( Pin.Unchanged, Pin.Low, pulseTime += milliSecond );
                qe.LastChange.Should().Be( 1 );
                qe.AddPulse( Pin.Low, Pin.Unchanged, pulseTime += milliSecond );
                qe.LastChange.Should().Be( 1 );

                Assert.AreEqual( 0, qe.A );
                Assert.AreEqual( 0, qe.B );
            }

            Assert.AreEqual( 40, qe.PulseTypeCounts.Sum( _ => _ ) );

            Assert.AreEqual( 0, qe.PulseCounter.GetTimeWindowedPulseCount( pulseTime, TimeSpan.Zero ) );
            Assert.AreEqual( 1, qe.PulseCounter.GetTimeWindowedPulseCount( pulseTime, milliSecond ) );
            Assert.AreEqual( 0, qe.PulseCounter.GetTimeWindowedPulseCount( TimeSpan.Zero, TimeSpan.Zero ) );
            Assert.AreEqual( 10, qe.PulseCounter.GetTimeWindowedPulseCount( pulseTime, TimeSpan.FromTicks( 10 * TimeSpan.TicksPerMillisecond ) ) );
            Assert.AreEqual( 10, qe.PulseCounter.GetTimeWindowedPulseCount( pulseTime.Subtract( milliSecond ), TimeSpan.FromTicks( 10 * TimeSpan.TicksPerMillisecond ) ) );
        }
    }
}
