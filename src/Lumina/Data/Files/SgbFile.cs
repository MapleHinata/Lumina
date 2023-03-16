using System.IO;
using Lumina.Data.Attributes;
using Lumina.Data.Parsing.Layer;

using Lumina.Data.Parsing.Scene;

// field assigned but not read warning
#pragma warning disable 414

namespace Lumina.Data.Files
{
    [FileExtension( ".sgb" )]
    public class SgbFile : FileResource
    {
        public struct FileHeader
        {
            char[] FileID; //[4]
            int FileSize;
            int TotalChunkCount;

            public static FileHeader Read( BinaryReader br )
            {
                return new()
                {
                    FileID = br.ReadChars( 4 ),
                    FileSize = br.ReadInt32(),
                    TotalChunkCount = br.ReadInt32(),
                };
            }
        }

        public FileHeader Header { get; private set; }
        public Parsing.Scene.SceneChunk ChunkHeader { get; private set; }
        public LayerGroup[] LayerGroups { get; private set; }
        public SceneCommon.LGBAssetPath[] LgbAssetPaths { get; private set; }
        public SceneCommon.SGTimeline[] SGTimelines { get; private set; }

        public override void LoadFile()
        {

            Header = FileHeader.Read( Reader );

            ChunkHeader = Parsing.Scene.SceneChunk.Read( Reader );
            LayerGroups = ChunkHeader.LayerGroups;
            LgbAssetPaths = ChunkHeader.LGBAssetPaths;
            SGTimelines = ChunkHeader.SGTimelines;
        }
    }
}