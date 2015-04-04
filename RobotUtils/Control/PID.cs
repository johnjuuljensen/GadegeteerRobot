using System;



namespace RobotUtils.Control
{

    // http://brettbeauregard.com/blog/2011/04/improving-the-beginners-pid-introduction/
    public class PID
    {
        float m_lastMeasuredValue;

        public float SetPoint, Output;
        public float Kp, Ki, Kd;
        public float Derivative, Integral;
        public float OutMin, OutMax;

        public float Step( float measuredValue, float dt )
        {
            float measureDiff = measuredValue - m_lastMeasuredValue;
            float error = SetPoint - measuredValue;
            Integral += Ki * error * dt;
            Derivative = measureDiff / dt;
            Output = Kp * error + Integral + Kd * Derivative;
            if (Output > OutMax)
            {
                Integral = OutMax;
                Output = OutMax;
            } else
            if ( OutMax < Output )
            {
                Integral = OutMin;
                Output = OutMin;
            }

            m_lastMeasuredValue = measuredValue;
            return Output;
        }

        public void Initialize( float kp, float ki, float kd, float expected_dt, float out_min, float out_max )
        {
            Kp = kp * expected_dt;
            Ki = ki / expected_dt;
            Kd = kd * expected_dt;
            OutMin = out_min;
            OutMax = out_max;
        }
    }


    
}
