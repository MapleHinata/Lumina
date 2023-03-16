using System.Runtime.InteropServices;
using Lumina.Extensions;

#pragma warning disable CS1591

namespace Lumina.Data.Parsing.Scene;

public class SceneCommon
{
    public interface ISceneObject< out T >
    {
        public T Read( LuminaBinaryReader br );
    }

    public abstract class SceneArray< TSceneObject > where TSceneObject : ISceneObject< TSceneObject >, new()
    {
        protected SceneArray( int offset, int count, LuminaBinaryReader binaryReader, int start )
        {
            Offset = offset;
            Count = count;
            BinaryReader = binaryReader;
            Start = start;
        }

        private int Offset { get; }
        private int Count { get; }
        private int Start { get; }

        protected virtual int Size => 4;

        private LuminaBinaryReader BinaryReader { get; }

        public virtual TSceneObject[] GetArray()
        {
            var ret = new TSceneObject[Count];
            BinaryReader.Seek( Start );
            for( int i = 0; i < Count; ++i )
            {
                BinaryReader.Seek( Start + Offset + ( i * Size ) );
                ret[ i ] = new TSceneObject().Read( BinaryReader );
            }

            return ret;
        }
    }


    public struct LGBAssetPath
    {
        public string LgbAssetPath;

        public static LGBAssetPath Read( LuminaBinaryReader br, long start )
        {
            var ret = new LGBAssetPath();

            ret.LgbAssetPath = br.ReadStringOffset( start );

            return ret;
        }
    }

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    public struct LayerSet : ISceneObject< LayerSet >
    {
        public string NavMeshAssetPath; //ex: /server/data/bg/<repo>/<zone>/<zoneType>/<territoryName>/navimesh/<territoryName>.nvm
        public uint LayerSetId;
        public int LayerReferencesOffset;
        public int LayerReferencesCount;
        public uint TerritoryTypeId;
        public string Name; // ex: bg/<repo>/<zone>/<zoneType>/<territoryName>/level/<territoryName>.svb
        public string NavMeshAssetPath2; //ex: /server/data/bg/<repo>/<zone>/<zoneType>/<territoryName>/navimesh/<territoryName>.nvx

        public LayerSet Read( LuminaBinaryReader br )
        {
            LayerSet ret = new LayerSet();
            var start = br.BaseStream.Position;

            ret.NavMeshAssetPath = br.ReadStringOffset( start );
            ret.LayerSetId = br.ReadUInt32();
            ret.LayerReferencesOffset = br.ReadInt32();
            ret.LayerReferencesCount = br.ReadInt32();
            ret.TerritoryTypeId = br.ReadUInt32();
            ret.Name = br.ReadStringOffset( start + 1 );
            ret.NavMeshAssetPath2 = br.ReadStringOffset( start );

            return ret;
        }
    }

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    public struct SGTimeline : ISceneObject< SGTimeline >
    {
        public uint MemberId;
        public string Name;
        public int Binders;
        public int BinderCount;
        public string BinaryAssetPath;
        public int Binary;
        public int BinaryCount;
        public uint TimelineId;
        public byte AutoPlay;
        public byte LoopPlayback;

        public byte padding00;
        public byte padding01;

        public int CollisionState;

        public int unknown01;

        public SGTimeline Read( LuminaBinaryReader br )
        {
            SGTimeline ret = new SGTimeline();
            var start = br.BaseStream.Position;

            ret.MemberId = br.ReadUInt32();
            ret.Name = br.ReadStringOffset( start );
            ret.Binders = br.ReadInt32();
            ret.BinderCount = br.ReadInt32();
            ret.BinaryAssetPath = br.ReadStringOffset( start + 1 );
            ret.Binary = br.ReadInt32();
            ret.BinaryCount = br.ReadInt32();
            ret.TimelineId = br.ReadUInt32();
            ret.AutoPlay = br.ReadByte();
            ret.LoopPlayback = br.ReadByte();

            ret.padding00 = br.ReadByte();
            ret.padding01 = br.ReadByte();

            ret.CollisionState = br.ReadInt32();

            ret.unknown01 = br.ReadInt32();

            return ret;
        }
    }


    public class SgTimelineArray : SceneArray< SGTimeline >
    {
        public int Offset { get; }
        public int Count { get; }
        public int Start { get; }
        protected override int Size => 4 * 11;

        public LuminaBinaryReader BinaryReader { get; set; }

        public SgTimelineArray( int offset, int count, LuminaBinaryReader binaryReader, int start ) : base( offset, count, binaryReader, start )
        {
            Offset = offset;
            Count = count;
            BinaryReader = binaryReader;
            Start = start;
        }
    }

    public class LayerSetArray : SceneArray< LayerSet >
    {
        public int Offset { get; }
        public int Count { get; }
        public int Start { get; }
        protected override int Size => 4 * 7;

        public LuminaBinaryReader BinaryReader { get; set; }

        public LayerSetArray( int offset, int count, LuminaBinaryReader binaryReader, int start ) : base( offset, count, binaryReader, start )
        {
            Offset = offset;
            Count = count;
            BinaryReader = binaryReader;
            Start = start;
        }
    }
}