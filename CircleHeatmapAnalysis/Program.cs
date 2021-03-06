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
    public static class Extensions
    {
        public static void CreateNewOrUpdateExisting(
        this IDictionary<string, int> map, string key, int value )
        {
            try
            {
                map[key] += value;
            }
            catch(Exception e )
            {
                map.Add( key, value );
            }
        }
    }

    public class Items
    {
        public int NumPackages { get; set; }
        public Dictionary<string, int> ItemCounts { get; set; }
    }

    class Program
    {

        static void Main( string[] args )
        {
            string ApiKey = "";
            ApiKey = File.ReadAllText( @"C:\Projects\CircleHeatmap\APIKey.txt" );

            int i = 33;
            var req = new PUBGLibrary.API.APIRequest();
            List<string> samples = req.RequestSamples( ApiKey, "pc-na" );

            foreach(string s in samples.Skip(32) )
            {
                var api = new PUBGLibrary.API.API(ApiKey);
                var matchReq = api.RequestMatch( s, PUBGLibrary.API.PlatformRegionShard.PC_NA, true );

                if ( matchReq.Match.CreatedAt?.Day > 2 && matchReq.Match.CreatedAt?.Month == 5 ) 
                {
                    File.WriteAllText( string.Format( @"Z:\PubgMatchData\MatchesNew\{0}.json", s ), matchReq.Match.BaseJSON );
                    File.WriteAllText( string.Format( @"Z:\PubgMatchData\TelemetryNew\{0}.json", s ), matchReq.Telemetry.BaseJSON );
                    Console.WriteLine( i++ );
                }
                if(i == 1000 )
                {
                    break;
                }
                //File.WriteAllText( string.Format( @"Z:\PubgMatchData\MatchesNew\{0}.json", s ), matchReq.Match.BaseJSON );
                //File.WriteAllText( string.Format( @"Z:\PubgMatchData\TelemetryNew\{0}.json", s ), matchReq.Telemetry.BaseJSON );
            }

            //int NumberOfCarePackages = 0;
            //Dictionary<string, int> ItemCounts = new Dictionary<string, int>();

            //int i = 1;
            //foreach (var file in new DirectoryInfo( @"Z:\PubgMatchData\Telemetry" ).GetFiles() )
            //{
            //    var req = new PUBGLibrary.API.APIRequest();

            //    var telemetry = req.TelemetryPhraser( File.ReadAllText( string.Format( file.FullName ) ) );

            //    NumberOfCarePackages += telemetry.LogCarePackageSpawnList.Count;

            //    telemetry.LogCarePackageSpawnList.ForEach(x=> x.CarePackage.Items.ForEach(c=> ItemCounts.CreateNewOrUpdateExisting(c.ItemID, 1) ) );
            //    Console.WriteLine( i++ );
            //}

            //Items ite = new Items() { NumPackages = NumberOfCarePackages, ItemCounts = ItemCounts };
            //File.WriteAllText( @"Z:\PubgMatchData\CarePackageResults.json", JsonConvert.SerializeObject( ite ) );

        }

        

        public void GetFinalCircleData()
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
            //PubgMatch match;
            //IEnumerable<PubgTelemetryEvent> telemetryData;
            //LogGameStatePeriodic LastGameState;

            List<Point> MiramarPoints = new List<Point>();
            List<Point> ErangelPoints = new List<Point>();

            // Start at (will start at +1 after the number entered)
            int i = 4000;
            // Take this per loop
            int TakeNum = 50;
            // Go until this record
            int GoUntil = 12000;

            while ( i < GoUntil )
            {
                //Parallel.ForEach(
                //    // Skip the first i that have already been taken, and take the next TakeNum amount
                //    matches.data.relationships.matches.data.Skip( i ).Take( TakeNum ),
                //    new ParallelOptions { MaxDegreeOfParallelism = 10 }, m =>
                //    {
                //        // Get the match to find the telemetry link
                //        match = matchService.GetMatch( PubgRegion.PCNorthAmerica, m.id );

                //        // Get the telemetry data from the match
                //        telemetryData = telemetryService.GetTelemetry( PubgRegion.PCNorthAmerica, match.Assets.FirstOrDefault() );

                //        // Find the last game state to record the circle position
                //        LastGameState = telemetryData.OfType<LogGameStatePeriodic>().OrderBy( x => x.GameState.SafetyZoneRadius ).FirstOrDefault();

                //        if ( match.MapName.ToString().ToLower() == "miramar" )
                //            MiramarPoints.Add( new Point { X = LastGameState.GameState.SafetyZonePosition.X, Y = LastGameState.GameState.SafetyZonePosition.Y, MatchId = match.Id } );
                //        else if ( match.MapName.ToString().ToLower() == "erangel" )
                //            ErangelPoints.Add( new Point { X = LastGameState.GameState.SafetyZonePosition.X, Y = LastGameState.GameState.SafetyZonePosition.Y, MatchId = match.Id } );
                //    } );
                //Console.WriteLine( i + " to " + ( i + TakeNum ) + " DONE" );
                //i += TakeNum;



                for ( int z = 0; z < TakeNum; z++ )
                {
                    try
                    {
                        var matchesList = matches.data.relationships.matches.data.Skip( i ).Take( TakeNum ).ToList();

                        var match = matchService.GetMatch( PubgRegion.PCNorthAmerica, matchesList[z].id );

                        // Get the telemetry data from the match
                        var telemetryData = telemetryService.GetTelemetry( PubgRegion.PCNorthAmerica, match.Assets.FirstOrDefault() );

                        // Find the last game state to record the circle position
                        var LastGameState = telemetryData.OfType<LogGameStatePeriodic>().OrderBy( x => x.GameState.SafetyZoneRadius ).FirstOrDefault();

                        if ( match.MapName.ToString().ToLower() == "miramar" )
                            MiramarPoints.Add( new Point { X = LastGameState.GameState.SafetyZonePosition.X, Y = LastGameState.GameState.SafetyZonePosition.Y, MatchId = match.Id } );
                        else if ( match.MapName.ToString().ToLower() == "erangel" )
                            ErangelPoints.Add( new Point { X = LastGameState.GameState.SafetyZonePosition.X, Y = LastGameState.GameState.SafetyZonePosition.Y, MatchId = match.Id } );

                        Console.WriteLine( z + i );
                    }
                    catch ( Exception e )
                    {
                        // Eat the exception
                        // Dont care why it happened
                        System.Threading.Thread.Sleep( 20000 );
                    }
                    break;
                }
                i += TakeNum;
                break;
                //System.Threading.Thread.Sleep( 61000 );
            }


            string MiramarJson = JsonConvert.SerializeObject( MiramarPoints );
            string ErangelJson = JsonConvert.SerializeObject( ErangelPoints );

            File.WriteAllText( @"C:\Projects\CircleHeatmap\MiramarPointOutput.json", MiramarJson );
            File.WriteAllText( @"C:\Projects\CircleHeatmap\ErangelPointOutput.json", ErangelJson );
        }

        public class Point
        {
            public string MatchId { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }
        
    }
}
