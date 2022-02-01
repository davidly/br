# br
Bandcamp Rename. Renames audio files downloaded from Bandcamp.

Build using your favorite version of .net like this:

    c:\windows\microsoft.net\framework64\v4.0.30319\csc.exe br.cs /nologo
    
Usage:

    Usage: br [folder]
    Bandcamp rename. Renames music files from bandcamp by removing band and album name.
      arguments:  [folder]   Where files are renamed. Default is the current directory.
      example:    br
                  br .
                  br honeymoon
                  br "d:\flac\mourn\self worth
      songs named like:  "mourn - self worth - 01 this feeling is disgusting.flac"
        are renamed to:  "01 this feeling is disgusting.flac"

