using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomCollections;

using devDept.Geometry;

namespace StlToInpConverter
{
    public class Part
    {
        public Part()
        {
            Nodes = new BiDictionary<int, Point3D>();
            Triangles = new List<IndexTriangle>();
        }

        public Part(string name)
        {
            Name = name;
            Nodes = new BiDictionary<int, Point3D>();
            Triangles = new List<IndexTriangle>();
        }

        public string Name;

        public BiDictionary<int, Point3D> Nodes;

        public List<IndexTriangle> Triangles;
    }
}
