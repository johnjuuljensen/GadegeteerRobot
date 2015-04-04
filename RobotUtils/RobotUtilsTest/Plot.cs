using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace RobotUtilsTest
{
    public static class Plot
    {

        public static Chart New(  int width, int height, Action<Chart> config = null )
        {
            var chart = new Chart
            {
                Size = new Size( width, height ), 
            };

            //var legend = new Legend( "fghh" );
            //chart.Legends.Add( legend );

            if ( config != null )
            {
                config( chart );
            }

            return chart;
        }

        public static ChartArea WithArea( this Chart chart, string titleText = null, Action<ChartArea> config = null )
        {
            var chartArea = new ChartArea
            {
                Name = Guid.NewGuid().ToString(),
                Tag = chart
            };

            if (titleText != null)
                chartArea = chartArea.AreaTitle( titleText );

            if ( config != null )
            {
                config( chartArea );
            }

            chart.ChartAreas.Add( chartArea );
            return chartArea;
        }


        public static ChartArea Simple( int width, int height )
        {
            return New( width, height )
                .WithArea();
        }


        public static ChartArea WithArea( this ChartArea chartArea, string titleText = null, Action<ChartArea> config = null )
        {
            return chartArea.Chart().WithArea( titleText, config );
        }


        public static ChartArea AreaTitle( this ChartArea chartArea, string titleText, Action<Title> config = null )
        {
            var title = new Title( titleText );
            title.DockedToChartArea = chartArea.Name;

            chartArea.Chart().Titles.Add( title );

            return chartArea;
        }

        public static Chart Chart( this ChartArea chartArea )
        {
            return (Chart)chartArea.Tag;
        }

        public static ChartArea ChartArea( this Series series )
        {
            return (ChartArea)series.Tag;
        }

        public static Chart Chart( this Series series )
        {
            return series.ChartArea().Chart();
        }

        public static Series WithSeries( this ChartArea chartArea, string name, SeriesChartType type )
        {
            var series = new Series
            {
                Name = name,
                //Legend = name,
                //LegendText = name,
                XValueType = ChartValueType.Auto,
                Tag = chartArea,
                ChartArea = chartArea.Name,
                ChartType = type, IsVisibleInLegend = true
            };

            chartArea.Chart().Series.Add( series );

            return series;
        }

        public static Series WithSeries( this Series series, string name, SeriesChartType type )
        {
            return series.ChartArea().WithSeries( name, type );
        }

        public static Series DataY<T>( this Series series, params IEnumerable<T>[] yValues )
        {
            series.Points.DataBindY( yValues );
            return series;
        }

        public static ChartArea Line<T>( this ChartArea chartArea, string name, IEnumerable<T> yValues )
        {
            var series = chartArea.WithSeries( name, SeriesChartType.Line )
                .DataY( yValues );

            return chartArea;
        }

        public static ChartArea Line<T>( this Series series, string name, params IEnumerable<T>[] yValues )
        {
            return series.ChartArea().Line( name, yValues );
        }

        public static void Save( this Chart chart, string filename, ChartImageFormat format )
        {
            chart.Invalidate();
            chart.SaveImage( filename, format );
        }

        public static void Save( this ChartArea chartArea, string filename, ChartImageFormat format )
        {
            chartArea.Chart().Save( filename, format );
        }

        public static void Save( this Series series, string filename, ChartImageFormat format )
        {
            series.ChartArea().Chart().Save( filename, format );
        }
        
    }

    [TestClass]
    public class PlotTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

            Plot.Simple( 500, 500 )
                //.Title("title")
                .Line( "1", new[] { 1, 2, 3, 4 } )
                .Line( "2", new[] { 4, 7, 2, 10 } )
                .Save( "plottest.png", ChartImageFormat.Png );

            var p = System.Diagnostics.Process.Start( @"plottest.png" );
        }
    }

}
