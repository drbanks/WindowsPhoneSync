using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPhoneSync.DataContracts
{
    public interface ILibrary
    {
        SongMetadata[] FindAlbumTracks(string artist, string album);
        SongMetadata FindTrack(string artist, string album, string trackName, int? trackNumber = null);
        Artwork GetArtwork(SongMetadata song);
        Artwork GetArtwork(AlbumMetadata album);
        Artwork GetArtwork(ArtistMetadata artist);

    }
}
