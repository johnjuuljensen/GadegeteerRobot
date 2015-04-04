using System;



namespace RobotUtils.SensorAnalysis
{
    public class QuadratureEncoder
    {
        int[] QEM = new[] {
             0,-1, 1, 2,
             1, 0, 2,-1,
            -1, 2, 0, 1,
             2, 1,-1, 0,
        };

        byte m_a;
        byte m_b;
        int m_lastChange;
        int m_lastQEMIndex;

        PulseCounter m_pulse;


        public byte A { get { return m_a; } }
        public byte B { get { return m_b; } }

        public PulseCounter PulseCounter { get { return m_pulse; } }
        public uint[] PulseTypeCounts { get { return m_pulse.TypeCounts; } }
        public int LastChange { get { return m_lastChange; } }

        public QuadratureEncoder( bool a, bool b )
        {
            m_a = (byte)(a ? 1 : 0);
            m_b = (byte)(b ? 1 : 0);
            m_lastQEMIndex = GetQEMIndex( m_a, m_b );

            m_pulse = new PulseCounter( 4, 2 );
        }

        public static int GetQEMIndex( byte a, byte b )
        {
            return a << 1 | b;
        }

        public void AddPulse( Pin a, Pin b, TimeSpan pulseTime )
        {
            //Debug.Assert( a == Pin.Unchanged || b == Pin.Unchanged );
            m_a = a == Pin.Unchanged ? m_a : (byte)a;
            m_b = b == Pin.Unchanged ? m_b : (byte)b;
            int qemIndex = GetQEMIndex( m_a, m_b );
            m_lastChange = QEM[qemIndex << 2 | m_lastQEMIndex];
            m_lastQEMIndex = qemIndex;

            int type = a != Pin.Unchanged ? 0 : 1;
            m_pulse.Increment( pulseTime, (byte)type );
        }

    }
}
