using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Collections;
using Examples.UTNPhysicsEngine.physics.body;

namespace Examples.UTNPhysicsEngine.optimizacion.spatialHash
{
    class SpatialHash
    {
        private uint _cellSize;
        private uint _maxBuckets;
        private Dictionary<SpatialHashKey, SpatialHashBucket> _hash = new Dictionary<SpatialHashKey, SpatialHashBucket>();

        public SpatialHash(uint cellSize, uint maxBuckets)
        {
            _cellSize = cellSize;
            _maxBuckets = maxBuckets;
        }

        public void add(Vector3 aabb_min, Vector3 aabb_max, Body body)
        {
            SpatialHashKey key_min = new SpatialHashKey(_cellSize, aabb_min);
            SpatialHashKey key_max = new SpatialHashKey(_cellSize, aabb_max);
            for (long cz = key_min.cell_z; cz <= key_max.cell_z; ++cz)
                for (long cy = key_min.cell_y; cy <= key_max.cell_y; ++cy)
                    for (long cx = key_min.cell_x; cx <= key_max.cell_x; ++cx)
                        addToBucket(new SpatialHashKey(cx,cy,cz), body);
        }

        public void remove(Vector3 aabb_min, Vector3 aabb_max, Body body)
        {
            SpatialHashKey key_min = new SpatialHashKey(_cellSize, aabb_min);
            SpatialHashKey key_max = new SpatialHashKey(_cellSize, aabb_max);
            for (long cz = key_min.cell_z; cz <= key_max.cell_z; ++cz)
                for (long cy = key_min.cell_y; cy <= key_max.cell_y; ++cy)
                    for (long cx = key_min.cell_x; cx <= key_max.cell_x; ++cx)
                        removeFromBucket(new SpatialHashKey(cx,cy,cz), body);
        }
 
        public void update(SpatialHashAABB old_aabb, SpatialHashAABB new_aabb, Body body)
        {
            SpatialHashKey old_key_min = new SpatialHashKey(_cellSize, old_aabb.aabbMin);
            SpatialHashKey old_key_max = new SpatialHashKey(_cellSize, old_aabb.aabbMax);
            SpatialHashKey new_key_min = new SpatialHashKey(_cellSize, new_aabb.aabbMin);
            SpatialHashKey new_key_max = new SpatialHashKey(_cellSize, new_aabb.aabbMax);

            if (old_key_min.Equals(new_key_min) && old_key_max.Equals(new_key_max))
                return;

            // add new
            for (long cz = new_key_min.cell_z; cz <= new_key_max.cell_z; ++cz)
                for (long cy = new_key_min.cell_y; cy <= new_key_max.cell_y; ++cy)
                    for (long cx = new_key_min.cell_x; cx <= new_key_max.cell_x; ++cx)
                        if (cx < old_key_min.cell_x || cx > old_key_max.cell_x ||
                            cy < old_key_min.cell_y || cy > old_key_max.cell_y || 
                            cz < old_key_min.cell_z || cz > old_key_max.cell_z)
                                addToBucket(new SpatialHashKey(cx, cy, cz), body);
 
            // remove old
            for (long cz = old_key_min.cell_z; cz <= old_key_max.cell_z; ++cz)
                for (long cy = old_key_min.cell_y; cy <= old_key_max.cell_y; ++cy)
                    for (long cx = old_key_min.cell_x; cx <= old_key_max.cell_x; ++cx)
                        if (cx < new_key_min.cell_x || cx > new_key_max.cell_x ||
                            cy < new_key_min.cell_y || cy > new_key_max.cell_y ||
                            cz < new_key_min.cell_z || cz > new_key_max.cell_z)
                                removeFromBucket(new SpatialHashKey(cx, cy, cz), body);
        }
 
        public ArrayList getNeighbors(Vector3 aabb_min, Vector3 aabb_max)
        {
            ArrayList result = new ArrayList();
            SpatialHashKey key_min = new SpatialHashKey(_cellSize, aabb_min);
            SpatialHashKey key_max = new SpatialHashKey(_cellSize, aabb_max);
            for (long cz = key_min.cell_z; cz <= key_max.cell_z; ++cz)
                for (long cy = key_min.cell_y; cy <= key_max.cell_y; ++cy)
                    for (long cx = key_min.cell_x; cx <= key_max.cell_x; ++cx)
                    {
                        SpatialHashBucket bucket = _hash[new SpatialHashKey(cx,cy,cz)];
                        if (bucket.items == null || bucket.items.Count <= 0) continue;
                        result.AddRange(bucket.items);                        
                    }
            return result;
        }
 
        private void addToBucket(SpatialHashKey key, Body body)
        {
            if (!_hash.ContainsKey(key))
            {
                _hash.Add(key, new SpatialHashBucket());
            }
            if (!_hash[key].items.Contains(body))
                _hash[key].items.Add(body);
        }
 
        private void removeFromBucket(SpatialHashKey key, Body body)
        {
            if (!_hash.ContainsKey(key) || _hash[key].items.Count <= 0) 
                return;
            
            _hash[key].items.Remove(body);
            if (_hash[key].items.Count <= 0)
                _hash.Remove(key);
        }
    }
}
