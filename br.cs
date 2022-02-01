using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class BandcampRename
{
    static void Usage()
    {
        Console.WriteLine( @"Usage: br [folder]" );
        Console.WriteLine( @"Bandcamp rename. Renames music files from bandcamp by removing band and album name." );
        Console.WriteLine( @"  arguments:  [folder]   Where files are renamed. Default is the current directory." );
        Console.WriteLine( @"  example:    br" );
        Console.WriteLine( @"              br ." );
        Console.WriteLine( @"              br honeymoon" );
        Console.WriteLine( @"              br ""d:\flac\mourn\self worth" );
        Console.WriteLine( @"  songs named like:  ""mourn - self worth - 01 this feeling is disgusting.flac""" );
        Console.WriteLine( @"    are renamed to:  ""01 this feeling is disgusting.flac""" );

        Environment.Exit( 1 );
    } //Usage

    static void Main( string[] args )
    {
        string srcRoot = ".";

        if ( args.Length > 1 )
            Usage();

        if ( args.Length == 1 )
        {
            srcRoot = args[ 0 ];

            // Is the user looking for usage information?

            if ( -1 != srcRoot.IndexOf( '?' ) ||
                 '/' == srcRoot[ 0 ] ||
                 '-' == srcRoot[ 0 ] )
                Usage();
        }

        try
        {
            srcRoot = Path.GetFullPath( srcRoot );
            Console.WriteLine( "renaming files in {0}", srcRoot );
    
            Directory.SetCurrentDirectory( srcRoot );
    
            SortedSet<string> renamedExtensions = new SortedSet<string>( new ByStringCompare() );
    
            renamedExtensions.Add( ".aac" );
            renamedExtensions.Add( ".aiff" );
            renamedExtensions.Add( ".alac" );
            renamedExtensions.Add( ".flac" );
            renamedExtensions.Add( ".m4a" );
            renamedExtensions.Add( ".mogg" );
            renamedExtensions.Add( ".mp3" );
            renamedExtensions.Add( ".oga" );
            renamedExtensions.Add( ".ogg" );
            renamedExtensions.Add( ".wav" );
            renamedExtensions.Add( ".wma" );
    
            string extension = "*.*";
            const string regex = @" - [0-9]{2} ";
    
            foreach ( FileInfo fi in GetFilesInfo( srcRoot, extension, false ) )
            {
                try
                {
                    string ext = Path.GetExtension( fi.Name );
    
                    if ( renamedExtensions.Contains( ext ) )
                    {
                        Match m = Regex.Match( fi.Name, regex );
                        int dash = -1;
    
                        if ( m.Success )
                            dash = m.Index;
    
                        if ( -1 != dash )
                        {
                            // dash points to one character before the dash, e.g.:
                            // mourn - self worth - 01 this feeling is disgusting.flac
                            //                   ^
                            //                   ^
    
                            string newName = fi.Name.Substring( dash + 3 );
    
                            Console.WriteLine( "renaming '{0}' to '{1}'", fi.Name, newName );
    
                            File.Move( fi.Name, newName );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( "exception {0} caught and ignored", ex.ToString() );
                }
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( "exception {0} caught with source directory {1}", ex.ToString(), srcRoot );
            Usage();
        }
    } //Main

    public class ByStringCompare : IComparer<string>
    {
        CaseInsensitiveComparer comp = new CaseInsensitiveComparer();

        public int Compare( string x, string y )
        {
            return comp.Compare( x, y );
        }
    } //ByStringCompare

    static IEnumerable<FileInfo> GetFilesInfo( string path, string extension, bool recurse = true )
    {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue( path );
    
        while ( queue.Count > 0 )
        {
            path = queue.Dequeue();

            if ( recurse )
            {
                try
                {
                    // GetDirectories will not return any subdirectories under a reparse point
    
                    foreach ( string subDir in Directory.GetDirectories( path ) )
                        queue.Enqueue( subDir );
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine( "Exception finding subdirectories {0} in {1}", ex.ToString(), path );
                }
            }

            FileInfo[] files = null;
            try
            {
                DirectoryInfo di = new DirectoryInfo( path );
                files = di.GetFiles(  extension );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine( "Exception finding .xmp files {0} in {1}", ex.ToString(), path );
            }
    
            if ( files != null )
            {
                for ( int i = 0 ; i < files.Length; i++ )
                {
                    yield return files[ i ];
                }
            }
        }
    } //GetFilesInfo
} //BandcampRename
