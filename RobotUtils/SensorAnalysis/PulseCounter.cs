using System;



namespace RobotUtils.SensorAnalysis
{
    public class PulseCounter
    {
        public uint[] TypeCounts { get { return m_pulseTypeCount; } }

        uint m_pulseHistoryLengthModuloPattern;
        long[] m_pulseTimeHistory;
        uint m_pulseTimeHistoryIndex;

        uint[] m_pulseTypeCount;

        public PulseCounter( int pulseHistoryLengthExponent, byte numPulseTypes )
        {
            uint pulseHistoryLength = 1U << pulseHistoryLengthExponent;
            m_pulseHistoryLengthModuloPattern = pulseHistoryLength - 1;
            m_pulseTimeHistoryIndex = m_pulseHistoryLengthModuloPattern;

            m_pulseTimeHistory = new long[pulseHistoryLength];

            for (int i = 0; i < m_pulseTimeHistory.Length; ++i)
                m_pulseTimeHistory[i] = long.MinValue;

            m_pulseTypeCount = new uint[numPulseTypes];
        }

        public void Increment( TimeSpan pulseTime, byte pulseType )
        {
            m_pulseTypeCount[pulseType]++;
            m_pulseTimeHistoryIndex = (m_pulseTimeHistoryIndex + 1) & m_pulseHistoryLengthModuloPattern;
            m_pulseTimeHistory[m_pulseTimeHistoryIndex] = pulseTime.Ticks;
        }


        public int GetTimeWindowedPulseCount( TimeSpan currentTime, TimeSpan ts )
        {
            int i = 0;
            uint pulseTimeHistoryIndex = m_pulseTimeHistoryIndex;

            while (i < m_pulseTimeHistory.Length && currentTime.Ticks < m_pulseTimeHistory[pulseTimeHistoryIndex])
            {
                pulseTimeHistoryIndex = (pulseTimeHistoryIndex - 1) & m_pulseHistoryLengthModuloPattern;
                ++i;
            }


            long countedTS = currentTime.Ticks - m_pulseTimeHistory[pulseTimeHistoryIndex];
            int count = 0;
            while (i < m_pulseTimeHistory.Length && countedTS < ts.Ticks)
            {
                pulseTimeHistoryIndex = (pulseTimeHistoryIndex - 1) & m_pulseHistoryLengthModuloPattern;
                countedTS = currentTime.Ticks - m_pulseTimeHistory[pulseTimeHistoryIndex];
                ++count;
                ++i;
            }

            return count;
        }


        public TimeSpan[] GetPulseTimeHistory()
        {
            TimeSpan[] res = new TimeSpan[m_pulseTimeHistory.Length];
            for (int i = 0; i < m_pulseTimeHistory.Length; ++i)
                res[i] = TimeSpan.FromTicks( m_pulseTimeHistory[i] );

            return res;
        }
    }
}
