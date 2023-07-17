using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Lumina.Data;
using Lumina.Data.Files;
using Lumina.Data.Files.Excel;
using Newtonsoft.Json;

namespace Lumina.Cmd.Commands
{
    [Command( "file", Description = "Prints information about a file and provides other filesystem utils" )]
    public class FileInfo : ICommand
    {
        [CommandParameter( 0, Description = "path to game file" )]
        public string FilePath { get; set; }

        [CommandOption( "dataPath", 'p',
            Description = "Path to the client sqpack folder",
            IsRequired = true,
            EnvironmentVariableName = "LUMINA_CMD_CLIENT_PATH" )]
        public string DataPath { get; set; }

        private FileResource LoadResource( GameData gameData, string path )
        {
            var ext = Path.GetExtension( path );

            switch( ext )
            {
                case ".exh":
                    return gameData.GetFile< ExcelHeaderFile >( path );

                case ".exd":
                    return gameData.GetFile< ExcelDataFile >( path );

                case ".exl":
                    return gameData.GetFile< ExcelListFile >( path );

                case ".imc":
                    return gameData.GetFile< ImcFile >( path );

                case ".lgb":
                    return gameData.GetFile< LgbFile >( path );
                case ".lvb":
                    return gameData.GetFile< LvbFile >( path );
                case ".sgb":
                    return gameData.GetFile< SgbFile >( path );
                case ".tex":
                    return gameData.GetFile< TexFile >( path );
                case ".hwc":
                    return gameData.GetFile< HwcFile >( path );
            }

            return null;
        }

        public ValueTask ExecuteAsync( IConsole console )
        {
            var lumina = new GameData( DataPath );

            var file = LoadResource( lumina, FilePath );

            var co = console.Output;

            co.WriteLine( $"File info ({file.GetType().FullName.Data()})" );
            co.WriteLine( $" path: {file.FilePath.Path.Data()}" );
            co.WriteLine( $" repo: {file.FilePath.Repository.Data()} cat: {file.FilePath.Category.Data()}" );
            co.WriteLine( $" size: {file.Data.Length.Data()}" );

            switch( file )
            {
                case LgbFile f:
                {
                    co.WriteLine( "Lgb Info:" );
                    string fileDir = $"{f.FilePath.Path.Replace( '/', '-' )}";
                    
                    string chunkHeaderJson = JsonConvert.SerializeObject(f.ChunkHeader, Formatting.Indented);
                    co.WriteLine( $" Scene Chunk Header: {chunkHeaderJson.Data()}");
                    
                    co.WriteLine( $" layers: {f.Layers.Length.Data()}");

                    for( int i = 0; i < f.Layers.Length; i++ )
                    {
                        var layer = f.Layers[ i ];
                        co.WriteLine($" - layer {i.Data()} - {layer.Name.Data()}");
                        string formattedJson = JsonConvert.SerializeObject(layer, Formatting.Indented);

                        System.IO.Directory.CreateDirectory( fileDir );
                        File.WriteAllText($"{fileDir}/{layer.Name}.json", formattedJson);
                        co.WriteLine($"Layer: {formattedJson}");
                        
                    }
                    break;
                }
                case SgbFile f:
                {
                    co.WriteLine( "Sgb Info:" );
                    string fileName = $"{f.FilePath.Path.Split( '/' ).Last()}.json";

                    var sb = new StringBuilder();
                    
                    var chunkHeaderJson = JsonConvert.SerializeObject(f.ChunkHeader, Formatting.Indented);
                    sb.AppendLine( chunkHeaderJson + ",");
                    co.WriteLine( $" Scene Chunk Header: {chunkHeaderJson.Data()}");
                    
                    
                    string layerGroupsJson = JsonConvert.SerializeObject(f.LayerGroups, Formatting.Indented);
                    sb.AppendLine( layerGroupsJson + ",");
                    co.WriteLine( $" Layer Groups: {layerGroupsJson.Data()}");
                    
                    string lgbAssetPaths = JsonConvert.SerializeObject(f.LgbAssetPaths, Formatting.Indented);
                    sb.AppendLine( lgbAssetPaths + ",");
                    co.WriteLine( $" LGB Asset Paths: {lgbAssetPaths.Data()}");
                    
                    
                    string layerSetsJson = JsonConvert.SerializeObject(f.SGTimelines, Formatting.Indented);
                    sb.AppendLine( layerSetsJson + ",");
                    co.WriteLine( $" Layer Sets: {layerSetsJson.Data()}");
                    
                    File.WriteAllText(fileName, sb.ToString());
                    break;
                }
                case LvbFile f:
                {
                    co.WriteLine( "Lvb Info:" );
                    string fileName = $"{f.FilePath.Path.Split( '/' ).Last()}.json";

                    var sb = new StringBuilder();
                    
                    var chunkHeaderJson = JsonConvert.SerializeObject(f.ChunkHeader, Formatting.Indented);
                    sb.AppendLine( chunkHeaderJson + ",");
                    co.WriteLine( $" Scene Chunk Header: {chunkHeaderJson.Data()}");
                    
                    
                    string layerGroupsJson = JsonConvert.SerializeObject(f.LayerGroups, Formatting.Indented);
                    sb.AppendLine( layerGroupsJson + ",");
                    co.WriteLine( $" Layer Groups: {layerGroupsJson.Data()}");
                    
                    string lgbAssetPaths = JsonConvert.SerializeObject(f.LgbAssetPaths, Formatting.Indented);
                    sb.AppendLine( lgbAssetPaths + ",");
                    co.WriteLine( $" LGB Asset Paths: {lgbAssetPaths.Data()}");
                    
                    
                    string layerSetsJson = JsonConvert.SerializeObject(f.LayerSets, Formatting.Indented);
                    sb.AppendLine( layerSetsJson + ",");
                    co.WriteLine( $" Layer Sets: {layerSetsJson.Data()}");
                    
                    File.WriteAllText(fileName, sb.ToString());
                    break;
                }
            }

            return default;
        }
    }
}