namespace RangeTreeTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using MB.Algodat;

    using RangeTreeExamples;

    [TestFixture]
    public class NearestItemsTests
    {
        #region QueryNearestLeft
        [Test]
        public void QueryNearestLeft_RetriveNonIntersectingValues_ExpectTwoItems()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F         T        |   F         T
            //   |====1====|        |   |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T        |   F          T
            //  |=====5====|        |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 11, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestLeft(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(new RangeItem(0, 11, "5")));
            Assert.That(results[1], Is.EqualTo(new RangeItem(1, 11, "1")));
        }

        [Test]
        public void QueryNearestLeft_RetriveNonIntersectingValues_ExpectSingleItem()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F         T        |   F         T
            //   |====1===|         |   |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T        |   F          T
            //  |=====5====|        |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 10, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestLeft(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(new RangeItem(0, 11, "5")));
        }

        [Test]
        public void QueryNearestLeft_RetriveNonIntersectingValues_ExpectNoItem()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //                      |   F         T
            //                      |   |====2====|
            //  01234567890123456789012345678901234567890
            //                      |      F           T
            //                      |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //                      |   F          T
            //                      |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem(24, 34, "2"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestLeft(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        public void QueryNearestLeft_RetriveIntersectingValues_ExpectSingleItem()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F                  | T  F         T
            //   |========1===========|  |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T        |   F          T
            //  |=====5====|        |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 22, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestLeft(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(new RangeItem(1, 22, "1")));
        }

        [Test]
        public void QueryNearestLeft_RetriveIntersectingValues_ExpectTwoItems()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F                  | T  F         T
            //   |========1===========|  |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T  F     |               T
            //  |=====5====|  |==========6==========|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 22, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(14, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestLeft(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(new RangeItem(1, 22, "1")));
            Assert.That(results[1], Is.EqualTo(new RangeItem(14, 36, "6")));
        }
        #endregion

        #region QueryNearestRight
        [Test]
        public void QueryNearestRight_RetriveNonIntersectingValues_ExpectTwoItems()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F         T        |   F         T
            //   |====1====|        |   |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T        |   F          T
            //  |=====5====|        |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 11, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestRight(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(new RangeItem(24, 34, "2")));
            Assert.That(results[1], Is.EqualTo(new RangeItem(24, 36, "6")));
        }

        [Test]
        public void QueryNearestRight_RetriveNonIntersectingValues_ExpectSingleItem()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F         T        |    F         T
            //   |====1===|         |    |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T        |   F          T
            //  |=====5====|        |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 10, "1"),
                             new RangeItem(25, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestRight(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(new RangeItem(24, 36, "6")));
        }

        [Test]
        public void QueryNearestRight_RetriveNonIntersectingValues_ExpectNoItem()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F         T        |
            //   |====1===|         |
            //  01234567890123456789012345678901234567890
            //     F     T          |
            //     |==3==|          |
            //  01234567890123456789012345678901234567890
            //  F          T        |
            //  |=====5====|        |
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 10, "1"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem( 0, 11, "5"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestRight(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        public void QueryNearestRight_RetriveIntersectingValues_ExpectOneItem()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F                  | T  F         T
            //   |========1===========|  |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T        |   F          T
            //  |=====5====|        |   |=====6=====|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 22, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(24, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestRight(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(new RangeItem(1, 22, "1")));
        }

        [Test]
        public void QueryNearestRight_RetriveIntersectingValues_ExpectTwoItems()
        {
            //Arrange
            var tree = new RangeTree<int, RangeItem>(new RangeItemComparer());

            //  00000000001111111111222222222233333333334
            //  01234567890123456789012345678901234567890
            //                      V
            //   F                  | T  F         T
            //   |========1===========|  |====2====|
            //  01234567890123456789012345678901234567890
            //     F     T          |      F           T
            //     |==3==|          |      |=====4=====|
            //  01234567890123456789012345678901234567890
            //  F          T  F     |               T
            //  |=====5====|  |==========6==========|
            //
            //  01234567890123456789012345678901234567890
            //  00000000001111111111222222222233333333334

            RangeItemComparer rangeItemComparer = new RangeItemComparer();
            IEnumerable<RangeItem> items = new RangeItem[]
                         {
                             new RangeItem( 1, 22, "1"),
                             new RangeItem(24, 34, "2"),
                             new RangeItem( 3,  9, "3"),
                             new RangeItem(27, 39, "4"),
                             new RangeItem( 0, 11, "5"),
                             new RangeItem(14, 36, "6"),
                         };
            tree = new RangeTree<int, RangeItem>(items, rangeItemComparer);

            // Act
            var results = tree.QueryNearestRight(20);

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(new RangeItem(1, 22, "1")));
            Assert.That(results[1], Is.EqualTo(new RangeItem(14, 36, "6")));
        }
#endregion
    }
}