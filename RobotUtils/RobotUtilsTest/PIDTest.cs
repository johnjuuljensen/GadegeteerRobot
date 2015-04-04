using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotUtils.Control;

namespace RobotUtilsTest
{
    [TestClass]
    public class PIDTest
    {
        [TestMethod]
        public void PIDWithWindupCompensationTest()
        {
            float dt = 0.01F;
            float min = 0F;
            float max = 1F;

            var pid = new PID();
            pid.Initialize( 0.5F, 0.3F, 0.4F, dt, 0F, 1F );

            int IterCount = 500;

            float[] setpoint = new float[IterCount];
            float[] input = new float[IterCount];
            float[] output = new float[IterCount];
            float[] integral = new float[IterCount];
            float[] derivative = new float[IterCount];

            pid.SetPoint = 1.1F;
            float lastValue = min;
            for (int i = 0; i < IterCount; i++)
            {
                if (i == 100) pid.SetPoint = 0.5F;
                if (i == 150) pid.SetPoint = 0.0F;
                if (i == 200) pid.SetPoint = 1.0F;
                if (i == 250) pid.SetPoint = 0.0F;
                if (i > 300) pid.SetPoint = (i - 300F) / 200F;

                setpoint[i] = pid.SetPoint;
                input[i] = lastValue;
                //pid.Step( (float)Math.Log(lastValue+1), dt );

                pid.Step( lastValue, dt );
                lastValue = pid.Output > max ? max : pid.Output;
                lastValue = lastValue < min ? min : lastValue;

                output[i] = pid.Output;
                derivative[i] = pid.Derivative;
                integral[i] = pid.Integral;
            }

            Plot.New( 700, 500 )
                .WithArea( "input" )
                    .Line( "input", output )
                .WithArea( "output" )
                    .Line( "setpoint", setpoint )
                    .Line( "output", output )
                .WithArea( "derivative" )
                    .Line( "derivative", derivative )
                .WithArea( "integral" )
                    .Line( "integral", integral )
                .Save( "PID_WithWindupCompensation.png", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png );

        }


        [TestMethod]
        public void PIDWithWindupTest()
        {
            float dt = 0.01F;
            float min = 0F;
            float max = 1F;

            var pid = new PIDWithInputDerivative();
            pid.Initialize( 0.5F, 0.3F, 0.4F, dt );

            int IterCount = 500;

            float[] setpoint = new float[IterCount];
            float[] input = new float[IterCount];
            float[] output = new float[IterCount];
            float[] integral = new float[IterCount];
            float[] derivative = new float[IterCount];

            pid.SetPoint = 1.1F;
            float lastValue = min;
            for (int i = 0; i < IterCount; i++)
            {
                if (i == 100) pid.SetPoint = 0.5F;
                if (i == 150) pid.SetPoint = 0.0F;
                if (i == 200) pid.SetPoint = 1.0F;
                if (i == 250) pid.SetPoint = 0.0F;
                if (i > 300) pid.SetPoint = (i - 300F) / 200F;

                setpoint[i] = pid.SetPoint;
                input[i] = lastValue;
                //pid.Step( (float)Math.Log(lastValue+1), dt );

                pid.Step( lastValue, dt );
                lastValue = pid.Output > max ? max : pid.Output;
                lastValue = lastValue < min ? min : lastValue;

                output[i] = pid.Output;
                derivative[i] = pid.Derivative;
                integral[i] = pid.Integral;
            }

            Plot.New( 700, 500 )
                .WithArea( "input" )
                    .Line( "input", output )
                .WithArea( "output" )
                    .Line( "setpoint", setpoint )
                    .Line( "output", output )
                .WithArea( "derivative" )
                    .Line( "derivative", derivative )
                .WithArea( "integral" )
                    .Line( "integral", integral )
                .Save( "PID_WithWindup.png", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png );

        }
        
        
        [TestMethod]
        public void PIDWithInputDerivativeTest()
        {
            float dt = 0.01F;
            float min = 0F;
            float max = 1F;

            var pid = new PIDWithInputDerivative();
            pid.Initialize( 0.5F, 0.3F, 0.4F, dt );

            int IterCount = 500;

            float[] setpoint = new float[IterCount];
            float[] input = new float[IterCount];
            float[] output = new float[IterCount];
            float[] integral = new float[IterCount];
            float[] derivative = new float[IterCount];

            pid.SetPoint = 1F;
            float lastValue = min;
            for (int i = 0; i < IterCount; i++)
            {
                if (i == 100) pid.SetPoint = 0.5F;
                if (i == 150) pid.SetPoint = 0.0F;
                if (i == 200) pid.SetPoint = 1.0F;
                if (i == 250) pid.SetPoint = 0.0F;
                if (i > 300) pid.SetPoint = (i - 300F) / 200F;

                setpoint[i] = pid.SetPoint;
                input[i] = lastValue;
                //pid.Step( (float)Math.Log(lastValue+1), dt );

                pid.Step( lastValue, dt );
                lastValue = pid.Output > max ? max : pid.Output;
                lastValue = lastValue < min ? min : lastValue;

                output[i] = pid.Output;
                derivative[i] = pid.Derivative;
                integral[i] = pid.Integral;
            }

            Plot.New( 700, 500 )
                .WithArea( "input" )
                    .Line( "input", output )
                .WithArea( "output" )
                    .Line( "setpoint", setpoint )
                    .Line( "output", output )
                .WithArea( "derivative" )
                    .Line( "derivative", derivative )
                .WithArea( "integral" )
                    .Line( "integral", integral )
                .Save( "PID_WithInputDerivative.png", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png );

        }

        
        [TestMethod]
        public void NaivePIDTest()
        {
            float dt = 0.01F;
            float min = 0F;
            float max = 1F;

            var pid = new NaivePID();
            pid.Initialize( 0.5F, 0.1F, 0.4F, dt );

            int IterCount = 500;

            float[] output = new float[IterCount];
            float[] integral = new float[IterCount];
            float[] derivative = new float[IterCount];

            pid.SetPoint = 1;
            float lastValue = min;
            for (int i = 0; i < IterCount; i++)
            {
                if (i == 100) pid.SetPoint = 0.5F;
                if (i == 150) pid.SetPoint = 0.0F;
                if (i == 200) pid.SetPoint = 1.0F;
                if (i == 250) pid.SetPoint = 0.0F;
                if (i > 300) pid.SetPoint = (i - 300F) / 200F;

                pid.Step( lastValue, dt );
                lastValue = pid.Output > max ? max : pid.Output;
                lastValue = lastValue < min ? min : lastValue;

                output[i] = pid.Output;
                derivative[i] = pid.Derivative;
                integral[i] = pid.Integral;
            }

            Plot.New( 500, 900 )
                .WithArea()
                    .Line( "output", output )
                .WithArea()
                    .Line( "derivative", derivative )
                .WithArea()
                    .Line( "integral", integral )
                .Save( "PID_Naive.png", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png );

        }


        public class PIDWithInputDerivative
        {
            float m_lastMeasuredValue;

            public float SetPoint, Output;
            public float Kp, Ki, Kd;
            public float Derivative, Integral;

            public float Step( float measuredValue, float dt )
            {
                float measureDiff = measuredValue - m_lastMeasuredValue;
                float error = SetPoint - measuredValue;
                Integral += Ki * error * dt;
                Derivative = measureDiff / dt;
                Output = Kp * error + Integral + Kd * Derivative;
                m_lastMeasuredValue = measuredValue;
                return Output;
            }

            public void Initialize( float kp, float ki, float kd, float expected_dt )
            {
                Kp = kp * expected_dt;
                Ki = ki / expected_dt;
                Kd = kd * expected_dt;
            }
        }




        public class NaivePID
        {
            float m_lastErrorValue;

            public float SetPoint, Output;
            public float Kp, Ki, Kd;
            public float Derivative, Integral;

            public float Step( float measuredValue, float dt )
            {
                float error = SetPoint - measuredValue;
                Integral += Ki * error * dt;
                Derivative = (error - m_lastErrorValue) / dt;
                Output = Kp * error + Integral + Kd * Derivative;
                m_lastErrorValue = error;
                return Output;
            }

            public void Initialize( float kp, float ki, float kd, float expected_dt )
            {
                Kp = kp * expected_dt;
                Ki = ki / expected_dt;
                Kd = kd * expected_dt;
            }
        }

    }
}
