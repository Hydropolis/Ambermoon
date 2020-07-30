﻿namespace Ambermoon.Data
{
    public interface IMapManager
    {
        Map GetMap(uint index);
        Tileset GetTilesetForMap(Map map);
        Labdata GetLabdataForMap(Map map);
    }
}
