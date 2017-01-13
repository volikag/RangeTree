using System;
using System.Collections.Generic;

namespace MB.Algodat
{
    /// <summary>
    /// A node of the range tree. Given a list of items, it builds
    /// its subtree. Also contains methods to query the subtree.
    /// Basically, all interval tree logic is here.
    /// </summary>
    public class RangeTreeNode<TKey, T>
        where TKey : IComparable<TKey>
        where T : IRangeProvider<TKey>
    {
        private TKey _center;
        private RangeTreeNode<TKey, T> _leftNode;
        private RangeTreeNode<TKey, T> _rightNode;
        private List<T> _items;

        private readonly IComparer<T> _rangeComparer;

        /// <summary>
        /// Initializes an empty node.
        /// </summary>
        /// <param name="rangeComparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IComparer<T> rangeComparer = null)
        {
            if (rangeComparer != null)
                _rangeComparer = rangeComparer;

            _center = default(TKey);
            _leftNode = null;
            _rightNode = null;
            _items = null;
        }

        /// <summary>
        /// Initializes a node with a list of items, builds the sub tree.
        /// </summary>
        /// <param name="rangeComparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IEnumerable<T> items, IComparer<T> rangeComparer = null)
        {
            if (rangeComparer != null)
                _rangeComparer = rangeComparer;

            // first, find the median
            var endPoints = new List<TKey>();
            foreach (var o in items)
            {
                var range = o.Range;
                endPoints.Add(range.From);
                endPoints.Add(range.To);
            }
            endPoints.Sort();

            // the median is used as center value
            _center = endPoints[endPoints.Count / 2];
            _items = new List<T>();
            
            var left = new List<T>();
            var right = new List<T>();

            // iterate over all items
            // if the range of an item is completely left of the center, add it to the left items
            // if it is on the right of the center, add it to the right items
            // otherwise (range overlaps the center), add the item to this node's items
            foreach (var o in items)
            {
                var range = o.Range;

                if (range.To.CompareTo(_center) < 0)
                    left.Add(o);
                else if (range.From.CompareTo(_center) > 0)
                    right.Add(o);
                else
                    _items.Add(o);
            }

            // sort the items, this way the query is faster later on
            if (_items.Count > 0)
                _items.Sort(_rangeComparer);
            else
                _items = null;

            // create left and right nodes, if there are any items
            if (left.Count > 0)
                _leftNode = new RangeTreeNode<TKey, T>(left, _rangeComparer);
            if (right.Count > 0)
                _rightNode = new RangeTreeNode<TKey, T>(right, _rangeComparer);
        }

        /// <summary>
        /// Performans a "stab" query with a single value.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public List<T> Query(TKey value)
        {
            var results = new List<T>();

            // If the node has items, check their ranges.
            if (_items != null)
            {
                foreach (var o in _items)
                {
                    if (o.Range.From.CompareTo(value) > 0)
                        break;
                    else if (o.Range.Contains(value))
                        results.Add(o);
                }
            }

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (value.CompareTo(_center) < 0 && _leftNode != null)
                results.AddRange(_leftNode.Query(value));
            else if (value.CompareTo(_center) > 0 && _rightNode != null)
                results.AddRange(_rightNode.Query(value));
            
            return results;
        }

        /// <summary>
        /// Retrive list of items, nearest to the left of given value.
        /// If given value is in bounds of items, these items returns.
        /// If there is no such item, returns empty list.
        /// Query return rightmost items that has rightest "To" property value.
        /// Folowing example will return items #1 an #3.
        /// 
        ///                    V
        ///                    |
        ///      F         T   |
        ///      |===#1====|   |
        ///        F      T    |
        ///        |==#2==|    |
        ///     F          T   |
        ///     |====#3====|   |
        /// 
        /// See Tests for more information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> QueryNearestLeft(TKey value)
        {
            var results = new List<T>();

            // If the node has items, check their ranges.
            if (_items != null)
            {
                foreach (var o in _items)
                {
                    if (o.Range.From.CompareTo(value) > 0)
                        break;
                    else
                    {
                        if (o.Range.Contains(value))
                        {
                            results.Add(o);
                        }
                    }
                }
            }

            //We found matches, so just do query to get others
            if (results.Count != 0)
            {
                if (value.CompareTo(_center) > 0 && _rightNode != null)
                {
                    results.AddRange(_rightNode.Query(value));
                }
                if (value.CompareTo(_center) < 0 && _leftNode != null)
                {
                    results.AddRange(_leftNode.Query(value));
                }
                return results;
            }

            //If we not found any intersecting items
            //first search for candidates in subtrees
            var candidates = new List<T>();

            if (value.CompareTo(_center) > 0 && _rightNode != null)
            {
                candidates.AddRange(_rightNode.QueryNearestLeft(value));
            }
            if (value.CompareTo(_center) < 0 && _leftNode != null)
            {
                candidates.AddRange(_leftNode.QueryNearestLeft(value));
            }

            //If we found any candidates in subtree,
            //we nedd to check if they are matches or just other candidates
            if (candidates.Count != 0)
            {
                //And if they are matches we
                //add them to result and returns
                if (candidates[0].Range.Contains(value))
                {
                    results.AddRange(candidates);
                    return results;
                }
            }

            //Else they are just candidates
            //and we need to merge them with current candidates
            //and find leftmost of them

            //Get local candidates
            if (_items != null)
            {
                foreach (var o in _items)
                {
                    if (o.Range.From.CompareTo(value) > 0)
                        break;
                    else
                    {
                        candidates.Add(o);
                    }
                }
            }

            //If candidates found
            if (candidates.Count != 0)
            {
                //Get the rightmost of them
                List<T> localCandidates = new List<T>();

                TKey rightmost = candidates[0].Range.To;
                foreach (var x in candidates)
                {
                    if (x.Range.To.Equals(rightmost))
                    {
                        localCandidates.Add(x);
                    }
                    if (x.Range.To.CompareTo(rightmost) > 0)
                    {
                        localCandidates.Clear();
                        localCandidates.Add(x);
                        rightmost = x.Range.To;
                    }
                }

                results.AddRange(localCandidates);
            }

            return results;
        }

        /// <summary>
        /// Retrive list of items, nearest to the right of given value.
        /// If given value is in bounds of items, these items returns.
        /// If there is no such item, returns empty list.
        /// Query return leftmost items that has leftest "From" property value.
        /// Folowing example will return items #1 an #3.
        /// 
        ///  V
        ///  |
        ///  |   F         T
        ///  |   |===#1====|
        ///  |      F           T
        ///  |      |=====#2====|
        ///  |   F          T
        ///  |   |=====#3=====|
        ///  
        /// See Tests for more information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> QueryNearestRight(TKey value)
        {
            var results = new List<T>();

            // If the node has items, check their ranges.
            if (_items != null)
            {
                foreach (var o in _items)
                {
                    if (o.Range.From.CompareTo(value) > 0)
                        break;
                    else
                    {
                        if (o.Range.Contains(value))
                        {
                            results.Add(o);
                        }
                    }
                }
            }

            //We found matches, so just do query to get others
            if (results.Count != 0)
            {
                if (value.CompareTo(_center) > 0 && _rightNode != null)
                {
                    results.AddRange(_rightNode.Query(value));
                }
                if (value.CompareTo(_center) < 0 && _leftNode != null)
                {
                    results.AddRange(_leftNode.Query(value));
                }
                return results;
            }

            //If we not found any intersecting items
            //first search for candidates in subtrees
            var candidates = new List<T>();

            if (value.CompareTo(_center) > 0 && _rightNode != null)
            {
                candidates.AddRange(_rightNode.QueryNearestRight(value));
            }
            if (value.CompareTo(_center) < 0 && _leftNode != null)
            {
                candidates.AddRange(_leftNode.QueryNearestRight(value));
            }

            //If we found any candidates in subtree,
            //we nedd to check if they are matches or just other candidates
            if (candidates.Count != 0)
            {
                //And if they are matches we
                //add them to result and returns
                if (candidates[0].Range.Contains(value))
                {
                    results.AddRange(candidates);
                    return results;
                }
            }

            //Else they are just candidates
            //and we need to merge them with current candidates
            //and find leftmost of them

            //Get local candidates
            if (_items != null)
            {
                foreach (var o in _items)
                {
                    if (o.Range.From.CompareTo(value) < 0)
                        break;
                    else
                    {
                        candidates.Add(o);
                    }
                }
            }

            //If candidates found
            if (candidates.Count != 0)
            {
                //Get the leftmost of them
                List<T> localCandidates = new List<T>();

                TKey leftmost = candidates[0].Range.From;
                foreach (var x in candidates)
                {
                    if (x.Range.From.Equals(leftmost))
                    {
                        localCandidates.Add(x);
                    }
                    if (x.Range.From.CompareTo(leftmost) < 0)
                    {
                        localCandidates.Clear();
                        localCandidates.Add(x);
                        leftmost = x.Range.From;
                    }
                }

                results.AddRange(localCandidates);
            }

            return results;
        }


        /// <summary>
        /// Performans a range query.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public List<T> Query(Range<TKey> range)
        {
            var results = new List<T>();

            // If the node has items, check their ranges.
            if (_items != null)
            {
                foreach (var o in _items)
                {
                    if (o.Range.From.CompareTo(range.To) > 0)
                        break;
                    else if (o.Range.Intersects(range))
                        results.Add(o);
                }
            }

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (range.From.CompareTo(_center) < 0 && _leftNode != null)
                results.AddRange(_leftNode.Query(range));
            if (range.To.CompareTo(_center) > 0 && _rightNode != null)
                results.AddRange(_rightNode.Query(range));
            
            return results;
        }
    }
}
