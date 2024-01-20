using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StlToInpConverter.Tests
{
    [TestFixture]
    public class PartTests
    {
        [Test]
        public void TestPartConstructor_InitializesNodesAndTriangles()
        {
            // Arrange
            var part = new Part();

            // Assert
            Assert.IsNotNull(part.Nodes);
            Assert.IsNotNull(part.Triangles);
        }

        [Test]
        public void TestPartConstructor_SetsName()
        {
            // Arrange
            var name = "test";
            var part = new Part(name);

            // Assert
            Assert.AreEqual(name, part.Name);
        }

        [Test]
        public void TestNodesProperty_InitializedAsBiDictionary()
        {
            // Arrange
            var part = new Part();

            // Assert
            Assert.IsInstanceOf<BiDictionary<int, Point3D>>(part.Nodes);
        }

        [Test]
        public void TestTrianglesProperty_InitializedAsList()
        {
            // Arrange
            var part = new Part();

            // Assert
            Assert.IsInstanceOf<List<IndexTriangle>>(part.Triangles);
        }

        [Test]
        public void TestNodesProperty_AddsNewNode()
        {
            // Arrange
            var part = new Part();
            var node = new Point3D(1, 2, 3);

            // Act
            part.Nodes.Add(1, node);

            // Assert
            Assert.IsTrue(part.Nodes.ContainsValue(node));
        }

        [Test]
        public void TestTrianglesProperty_AddsNewTriangle()
        {
            // Arrange
            var part = new Part();
            var triangle = new IndexTriangle(1, 2, 3);

            // Act
            part.Triangles.Add(triangle);

            // Assert
            Assert.IsTrue(part.Triangles.Contains(triangle));
        }

        // Add more test methods to cover other scenarios and edge cases

    }
}
