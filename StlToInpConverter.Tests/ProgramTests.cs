using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace StlToInpConverter.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            program = new Program();
        }

        [Test]
        public void TestHandleLine_ValidLine_AddsTriangleToPart()
        {
            // Arrange
            var line = "facet normal 0 0 1";
            var lineCount = 1;
            var str = new StreamReader(new MemoryStream());
            var part = new Part("test");

            // Act
            program.HandleLine(line, ref lineCount, str, part);

            // Assert
            Assert.AreEqual(1, part.Triangles.Count);
            // Add more assertions as needed
        }

        [Test]
        public void TestGetPoint_ValidLine_ReturnsPoint3D()
        {
            // Arrange
            var line = "vertex 1 2 3";

            // Act
            var point = program.GetPoint(line);

            // Assert
            Assert.AreEqual(1, point.X);
            Assert.AreEqual(2, point.Y);
            Assert.AreEqual(3, point.Z);
        }

        // Add more test methods to cover other functions and scenarios

        [Test]
        public void TestWriteInpFile_CreatesCorrectOutput()
        {
            // Arrange
            var parts = new List<Part>
            {
                new Part("Part1")
                {
                    Nodes = new BiDictionary<int, Point3D>
                    {
                        { 1, new Point3D(0.0, 0.0, 0.0) }
                    },
                    Triangles = new List<IndexTriangle>
                    {
                        new IndexTriangle(1, 2, 3)
                    }
                }
            };
            var culture = CultureInfo.InvariantCulture;
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var inpFileWriter = new InpFileWriter(streamWriter);

            // Act
            inpFileWriter.WriteInpFile(parts, culture);
            streamWriter.Flush();

            var writtenData = Encoding.UTF8.GetString(memoryStream.ToArray());

            // Assert
            Assert.That(writtenData, Does.Contain("*Heading"));
            Assert.That(writtenData, Does.Contain("*Part, name=Part1"));
            Assert.That(writtenData, Does.Contain("1, 0, 0, 0"));
            Assert.That(writtenData, Does.Contain("1, 2, 3"));
            Assert.That(writtenData, Does.Contain("*End Assembly"));

            // Cleanup
            memoryStream.Dispose();
            streamWriter.Dispose();
        }

    }
}
