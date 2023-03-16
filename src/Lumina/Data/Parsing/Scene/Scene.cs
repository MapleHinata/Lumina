using Lumina.Data.Parsing.Layer;
using Lumina.Extensions;

namespace Lumina.Data.Parsing.Scene
{
    public struct SceneChunk
    {
        // 4
        public char[] ChunkID;
        public int ChunkSize;

        public Layer.LayerGroup[] LayerGroups;

        public int Unknown10;
        public SceneCommon.LayerSet[] LayerSets;
        public SceneCommon.SGTimeline[] SGTimelines;
        public SceneCommon.LGBAssetPath[] LGBAssetPaths;
        public int Unknown24;
        public int Unknown28;
        public int Unknown2C;
        public int Unknown30;
        public int Unknown34;
        public int Unknown38;
        // 3
        public int Padding3C;
        public int Padding40;
        public int Padding44;

        public static SceneChunk Read( LuminaBinaryReader br )
        {
            SceneChunk ret = new SceneChunk();
            long start = br.BaseStream.Position;

            ret.ChunkID = br.ReadChars( 4 );
            ret.ChunkSize = br.ReadInt32();

            long rewind = br.BaseStream.Position;
            int layerGroupOffset = br.ReadInt32();
            int layerGroupCount = br.ReadInt32();

            ret.Unknown10 = br.ReadInt32();
            int layerSetFolderOffset = br.ReadInt32(); //Unknown14
            int sgTimelineFolderOffset = br.ReadInt32(); //Unknown18
            int lgbAssetsOffset = br.ReadInt32(); //Unknown1C
            int lgbAssetPathCount = br.ReadInt32(); //Unknown20
            ret.Unknown24 = br.ReadInt32();
            ret.Unknown28 = br.ReadInt32();
            ret.Unknown2C = br.ReadInt32();
            ret.Unknown30 = br.ReadInt32();
            ret.Unknown34 = br.ReadInt32();
            ret.Unknown38 = br.ReadInt32();

            ret.Padding3C = br.ReadInt32();
            ret.Padding40 = br.ReadInt32();
            ret.Padding44 = br.ReadInt32();

            // read LayerSets
            br.Seek( 20 + layerSetFolderOffset );
            int layerSetOffset = br.ReadInt32();
            int layerSetCount = br.ReadInt32();
            ret.LayerSets = new SceneCommon.LayerSetArray( layerSetOffset, layerSetCount, br, 20 + layerSetFolderOffset ).GetArray();

            // read SGTimelines
            br.Seek( 20 + sgTimelineFolderOffset );
            int sgTimelineOffset = br.ReadInt32();
            int sgTimelineCount = br.ReadInt32();
            ret.SGTimelines = new SceneCommon.SgTimelineArray( sgTimelineOffset, sgTimelineCount, br, 20 + sgTimelineFolderOffset ).GetArray();

            // read layer groups
            br.Seek( start + layerGroupOffset );
            ret.LayerGroups = new LayerGroup[layerGroupCount];
            for( int i = 0; i < layerGroupCount; ++i )
            {
                br.Seek( rewind + layerGroupOffset + ( i * 4 ) );
                ret.LayerGroups[i] = Layer.LayerGroup.Read( br );
            }

            // read LGB AssetPaths
            br.Seek( start + lgbAssetsOffset );
            ret.LGBAssetPaths = new SceneCommon.LGBAssetPath[lgbAssetPathCount];
            for( int i = 0; i < lgbAssetPathCount; i++ )
            {
                br.Seek( rewind + lgbAssetsOffset + ( i * 4 ) );
                ret.LGBAssetPaths[ i ] = SceneCommon.LGBAssetPath.Read( br, rewind + lgbAssetsOffset );
            }
            return ret;
        }
    };
}