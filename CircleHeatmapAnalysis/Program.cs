﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Pubg.Net;
using System.Net;
using System.Diagnostics;

namespace CircleHeatmapAnalysis
{
    class Program
    {

        static void Main( string[] args )
        {
            string ApiKey = "";
            ApiKey = File.ReadAllText( @"C:\Projects\CircleHeatmap\APIKey.txt" );

            Rootobject matches;
            using ( StreamReader reader = new StreamReader( @"C:\Projects\CircleHeatmap\SampleMatches.json" ) )
            {
                string json = reader.ReadToEnd();
                matches = JsonConvert.DeserializeObject<Rootobject>( json );
            }


            /*var matchService = new Pubg.Net.PubgMatchService( ApiKey );
            var telemetryService = new PubgTelemetryService();
            telemetryService.ApiKey = ApiKey;

            var test = matchService.GetMatch( PubgRegion.PCNorthAmerica, matches.data.relationships.matches.data.FirstOrDefault().id );
            
            var telemetry = telemetryService.GetTelemetry( PubgRegion.PCNorthAmerica, test.Assets.FirstOrDefault() );

            var LastCircleData = telemetry.OfType<LogGameStatePeriodic>().OrderBy(x=>x.GameState.SafetyZoneRadius);*/
            

            var matchService = new Pubg.Net.PubgMatchService( ApiKey );
            var telemetryService = new PubgTelemetryService();
            telemetryService.ApiKey = ApiKey;
            PubgMatch match;
            IEnumerable<PubgTelemetryEvent> telemetryData;
            LogGameStatePeriodic LastGameState;

            List<Point> MiramarPoints = new List<Point>();
            List<Point> ErangelPoints = new List<Point>();
            
            // Start at
            int i = 0;
            // Take this per loop
            int TakeNum = 50;
            // Go until this record
            int GoUntil = 500;

            while ( i < GoUntil )
            {
                Parallel.ForEach( 
                    // Skip the first i that have already been taken, and take the next TakeNum amount
                    matches.data.relationships.matches.data.Skip( i ).Take( TakeNum ),
                    new ParallelOptions { MaxDegreeOfParallelism = 10 }, m =>
                    {
                        // Get the match to find the telemetry link
                        match = matchService.GetMatch( PubgRegion.PCNorthAmerica, m.id );

                        // Get the telemetry data from the match
                        telemetryData = telemetryService.GetTelemetry( PubgRegion.PCNorthAmerica, match.Assets.FirstOrDefault() );

                        // Find the last game state to record the circle position
                        LastGameState = telemetryData.OfType<LogGameStatePeriodic>().OrderBy( x => x.GameState.SafetyZoneRadius ).FirstOrDefault();

                        if( match.MapName.ToString().ToLower() == "miramar" )
                            MiramarPoints.Add( new Point {  X = LastGameState.GameState.SafetyZonePosition.X, Y = LastGameState.GameState.SafetyZonePosition.Y } );
                        else if ( match.MapName.ToString().ToLower() == "erangel" )
                            ErangelPoints.Add( new Point { X = LastGameState.GameState.SafetyZonePosition.X, Y = LastGameState.GameState.SafetyZonePosition.Y } );
                    } );
                Console.WriteLine( i + " to " + (i + TakeNum ) + " DONE" );
                i += TakeNum;
                System.Threading.Thread.Sleep( 61000 );              
            }

            string MiramarJson = JsonConvert.SerializeObject( MiramarPoints );
            string ErangelJson = JsonConvert.SerializeObject( ErangelPoints );

            File.WriteAllText( @"C:\Projects\CircleHeatmap\MiramarPointOutput.json", MiramarJson );
            File.WriteAllText( @"C:\Projects\CircleHeatmap\ErangelPointOutput.json", ErangelJson );
        }

        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
        
    }
}
