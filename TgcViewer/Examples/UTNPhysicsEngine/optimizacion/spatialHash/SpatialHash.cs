using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Collections;
using Examples.UTNPhysicsEngine.physics.body;

namespace Examples.UTNPhysicsEngine.optimizacion.spatialHash
{
    public class SpatialHash
    {
        private Vector3 _maxSize;
        private Vector3 _cellSize;
        private float _indexScale;
        //private Dictionary<SpatialHashKey, SpatialHashBucket> _hash = new Dictionary<SpatialHashKey, SpatialHashBucket>();
        private SpatialHashBucket[,,] _hash;
        public SpatialHash(Vector3 maxSize, Vector3 cellSize, float indexScale)
        {
            _maxSize = maxSize;
            _hash = new SpatialHashBucket[(long)maxSize.X, (long)maxSize.Y, (long)maxSize.Y]; 
            _cellSize = cellSize;
            _indexScale = indexScale;
        }

        public void clear(int capacity)
        {
            _hash = new SpatialHashBucket[(long)_maxSize.X, (long)_maxSize.Y, (long)_maxSize.Y]; 
        }

        public void add(Vector3 aabb_min, Vector3 aabb_max, Body body)
        {
            SpatialHashKey key_min = new SpatialHashKey(_maxSize, _cellSize, aabb_min * _indexScale);
            SpatialHashKey key_max = new SpatialHashKey(_maxSize, _cellSize, aabb_max * _indexScale);
            for (long cz = key_min.cell_z; cz <= key_max.cell_z; ++cz)
                for (long cy = key_min.cell_y; cy <= key_max.cell_y; ++cy)
                    for (long cx = key_min.cell_x; cx <= key_max.cell_x; ++cx)
                        addToBucket(cx,cy,cz, body);
        }

        public void remove(Vector3 aabb_min, Vector3 aabb_max, Body body)
        {
            SpatialHashKey key_min = new SpatialHashKey(_maxSize,_cellSize, aabb_min * _indexScale);
            SpatialHashKey key_max = new SpatialHashKey(_maxSize,_cellSize, aabb_max * _indexScale);
            for (long cz = key_min.cell_z; cz <= key_max.cell_z; ++cz)
                for (long cy = key_min.cell_y; cy <= key_max.cell_y; ++cy)
                    for (long cx = key_min.cell_x; cx <= key_max.cell_x; ++cx)
                        removeFromBucket(cx, cy, cz, body);
        }
 
        public void update(SpatialHashAABB old_aabb, SpatialHashAABB new_aabb, Body body)
        {
            SpatialHashKey old_key_min = new SpatialHashKey(_maxSize,_cellSize, old_aabb.aabbMin * _indexScale);
            SpatialHashKey old_key_max = new SpatialHashKey(_maxSize,_cellSize, old_aabb.aabbMax * _indexScale);
            SpatialHashKey new_key_min = new SpatialHashKey(_maxSize,_cellSize, new_aabb.aabbMin * _indexScale);
            SpatialHashKey new_key_max = new SpatialHashKey(_maxSize,_cellSize, new_aabb.aabbMax * _indexScale);

            if (old_key_min.Equals(new_key_min) && old_key_max.Equals(new_key_max))
                return;

            // add new
            for (long cz = new_key_min.cell_z; cz <= new_key_max.cell_z; ++cz)
                for (long cy = new_key_min.cell_y; cy <= new_key_max.cell_y; ++cy)
                    for (long cx = new_key_min.cell_x; cx <= new_key_max.cell_x; ++cx)
                        if (cx < old_key_min.cell_x || cx > old_key_max.cell_x ||
                            cy < old_key_min.cell_y || cy > old_key_max.cell_y || 
                            cz < old_key_min.cell_z || cz > old_key_max.cell_z)
                                addToBucket(cx, cy, cz, body);
 
            // remove old
            for (long cz = old_key_min.cell_z; cz <= old_key_max.cell_z; ++cz)
                for (long cy = old_key_min.cell_y; cy <= old_key_max.cell_y; ++cy)
                    for (long cx = old_key_min.cell_x; cx <= old_key_max.cell_x; ++cx)
                        if (cx < new_key_min.cell_x || cx > new_key_max.cell_x ||
                            cy < new_key_min.cell_y || cy > new_key_max.cell_y ||
                            cz < new_key_min.cell_z || cz > new_key_max.cell_z)
                                removeFromBucket(cx, cy, cz, body);
        }
 
        public ArrayList getNeighbors(Vector3 aabb_min, Vector3 aabb_max)
        {
            ArrayList result = new ArrayList();
            SpatialHashKey key_min = new SpatialHashKey(_maxSize,_cellSize, aabb_min * _indexScale);
            SpatialHashKey key_max = new SpatialHashKey(_maxSize,_cellSize, aabb_max * _indexScale);
            for (long cz = key_min.cell_z; cz <= key_max.cell_z; ++cz)
                for (long cy = key_min.cell_y; cy <= key_max.cell_y; ++cy)
                    for (long cx = key_min.cell_x; cx <= key_max.cell_x; ++cx)
                    {
                        SpatialHashBucket bucket = _hash[cx,cy,cz];
                        if (bucket == null || bucket.items == null || bucket.items.Count <= 0) continue;
                        result.AddRange(bucket.items);                        
                    }
            return result;
        }

        private void addToBucket(long cell_x, long cell_y, long cell_z, Body body)
        {
            if (_hash[cell_x, cell_y, cell_z] == null)
            {
                _hash[cell_x, cell_y, cell_z] = new SpatialHashBucket();
            }
            if (!_hash[cell_x, cell_y, cell_z].items.Contains(body))
                _hash[cell_x, cell_y, cell_z].items.Add(body);
        }

        private void removeFromBucket(long cell_x, long cell_y, long cell_z, Body body)
        {
            if (_hash[cell_x, cell_y, cell_z] == null || _hash[cell_x, cell_y, cell_z].items.Count <= 0) 
                return;

            _hash[cell_x, cell_y, cell_z].items.Remove(body);
            if (_hash[cell_x, cell_y, cell_z].items.Count <= 0)
                _hash[cell_x, cell_y, cell_z] =null;
        }
    }
}
