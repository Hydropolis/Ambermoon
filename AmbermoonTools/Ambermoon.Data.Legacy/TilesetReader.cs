﻿namespace Ambermoon.Data.Legacy
{
    public class TilesetReader : ITilesetReader
    {
        public void ReadTileset(Tileset tileset, IDataReader dataReader)
        {
            int numTiles = dataReader.ReadWord();
            tileset.Tiles = new Tileset.Tile[numTiles];

            for (int i = 0; i < numTiles; ++i)
            {
                var tileFlags = dataReader.ReadDword();

                tileset.Tiles[i] = new Tileset.Tile();
                tileset.Tiles[i].GraphicIndex = dataReader.ReadWord();
                tileset.Tiles[i].NumAnimationFrames = dataReader.ReadByte();
                tileset.Tiles[i].Unknown2 = dataReader.ReadByte(); // Unknown
                tileset.Tiles[i].Flags = tileFlags; // TODO: REMOVE later

                ParseTileFlags(tileset.Tiles[i], tileFlags);
            }
        }

        void ParseTileFlags(Tileset.Tile tile, ulong flags)
        {
            // Bit 8: Allow movement (0 means block movement)
            // Bit 16: Unknown. It has the same value as bit 8 most of the times (but not always).
            // Bit 23-25: Sit/sleep value
            //  0 -> no sitting nor sleeping
            //  1 -> sit and look up
            //  2 -> sit and look right
            //  3 -> sit and look down
            //  4 -> sit and look left
            //  5 -> sleep (always face down)
            // Bit 26: Player invisible (doors, behind towers/walls, etc)

            tile.BlockMovement = (flags & 0x0100) == 0;
            var sitSleepValue = (flags >> 23) & 0x07;
            tile.SitDirection = (sitSleepValue == 0 || sitSleepValue > 4) ? (CharacterDirection?)null : (CharacterDirection)(sitSleepValue - 1);
            tile.Sleep = sitSleepValue == 5;
            tile.Invisible = (flags & 0x04000000) != 0;
        }
    }
}
