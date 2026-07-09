using System;
using System.Collections.Generic;
using agame.Rooms;
using agame.Rooms.Tile;
using Microsoft.Xna.Framework;

namespace agame.Rendering;
class ShadowHullLoader
{
    public enum ShadowHullEntryType
    {
        None,
        Full,
        SBR,
        SBL,
        STR,
        STL
    }
    public static List<Vector2[]> GetHulls(int[] tiles, int width, int height)
    {
        return MergeSegments(GetHullSegments(GetShadowHulls(tiles),width,height));
    }
    public static ShadowHullEntryType[] GetShadowHulls(int[] tiles)
    {
        ShadowHullEntryType[] n=new ShadowHullEntryType[tiles.Length];
        
        for(int i = 0; i < tiles.Length; i++)
        {
            

            int t=tiles[i];
            if ((!TileInfo.IsEmpty(t))&&TileInfo.IsOpaque(t))
            {
                if (TileInfo.IsFullSolid(t))
                {
                    n[i]=ShadowHullEntryType.Full;
                }else if (TileInfo.IsSolidSlant(t, 0))
                {
                    n[i]=ShadowHullEntryType.SBR;
                }else if (TileInfo.IsSolidSlant(t, 1))
                {
                    n[i]=ShadowHullEntryType.SBL;
                }else if (TileInfo.IsSolidSlant(t, 2))
                {
                    n[i]=ShadowHullEntryType.STR;
                }else if (TileInfo.IsSolidSlant(t, 3))
                {
                    n[i]=ShadowHullEntryType.STL;
                }
            }
            else
            {
                n[i]=ShadowHullEntryType.None;
            }
        }
        return n;
    }
    public static List<Vector2[]> GetHullSegments(ShadowHullEntryType[] hullEntries, int width, int height)
    {
        List<Vector2[]> segments=new List<Vector2[]>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                switch (hullEntries[x + width * y])
                {
                    case ShadowHullEntryType.None:
                        break;
                    case ShadowHullEntryType.Full:
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        break;
                    case ShadowHullEntryType.SBL:
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        break;
                    case ShadowHullEntryType.SBR:
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        break;
                    case ShadowHullEntryType.STL:
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        break;
                    case ShadowHullEntryType.STR:
                        segments.Add([new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y)*Room.screenTileSize),
                                        new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize)]);
                        segments.Add([new Vector2((x+1)*Room.screenTileSize,(y+1)*Room.screenTileSize),
                                        new Vector2((x)*Room.screenTileSize,(y)*Room.screenTileSize)]);
                        break;
                }
            }
        }
        List<Vector2[]> segmentsB=new List<Vector2[]>();
        foreach(Vector2[] segment in segments)
        {
            if (-1==segments.FindIndex((Vector2[] seg) =>
            {
                return (seg[0]==segment[1])&&(seg[1]==segment[0]);
            }))
            {
                segmentsB.Add(segment);
            }
            
        }
        segments.Clear();
        return segmentsB;
    }
    public static List<Vector2[]> MergeSegments(List<Vector2[]> old)
    {

        List<Vector2[]> newSegments=new List<Vector2[]>(old);
        bool merged=false;
        foreach(Vector2[] seg in old)
        {
            if(newSegments.Contains(seg)){
                if(seg[seg.Length-1]!=seg[0]){
                    Vector2[] m=FindMergeable(seg,old);
                    if(m!=null){
                        if (newSegments.Contains(m))
                        {
                            newSegments.Remove(seg);
                            newSegments.Remove(m);
                            newSegments.Add(Merge(seg,m));
                            merged=true;
                        }
                    }
                }
            }
        }
        if (merged)
        {
            return MergeSegments(newSegments);
        }else{
            return newSegments;
        }
    }
    private static Vector2[] FindMergeable(Vector2[] a, List<Vector2[]> segments)
    {
        List<int> s= new List<int>();
        foreach(Vector2[] seg in segments)
        {
            if ((seg[0] == a[a.Length - 1])&&(seg!=a))
            {
                s.Add(segments.IndexOf(seg));
            }
        }
        if (s.Count == 0)
        {
            return null;
        }
        double angle=200;
        int id=-1;
        double anglea=Math.Atan2(a[a.Length-1].Y-a[a.Length-2].Y,a[a.Length-1].X-a[a.Length-1].X);
        foreach(int ind in s)
        {
            Vector2[] seg=segments[ind];
            double angleb=Math.Atan2(seg[1].Y-seg[0].Y,seg[1].X-seg[0].X);
            angleb=angleb-anglea+Math.PI;
            if (angleb < 0)
            {
                angleb+=2*Math.PI;
            }
            if (angleb < angle)
            {
                angle=angleb;
                id=ind;
            }
        }
        return segments[id];
    }
    private static Vector2[] Merge(Vector2[] a, Vector2[] b)
    {
        Vector2[] x=new Vector2[a.Length+b.Length-1];
        for(int i = 0; i < a.Length; i++)
        {
            x[i]=a[i];
        }
        for(int i = 1; i < b.Length; i++)
        {
            x[a.Length+i-1]=b[i];
        }
        return x;
    }
}