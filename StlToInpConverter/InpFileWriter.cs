using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace StlToInpConverter
{
    public class InpFileWriter
    {
        public void WriteInpFile(List<Part> parts, CultureInfo culture)
        {
            using (var stw = new StreamWriter("model.inp"))
            {
                stw.WriteLine("*Heading");
                stw.WriteLine($"** Job name: Nb74 Model name: model");
                stw.WriteLine($"** Generated by: StlToInpConverter");
                stw.WriteLine($"*Preprint, echo=NO, model=NO, history=NO, contact=NO");
                stw.WriteLine($"**");
                stw.WriteLine($"** PARTS");
                stw.WriteLine($"**");

                foreach (var part in parts)
                {
                    stw.WriteLine($"*Part, name={part.Name}");
                    stw.WriteLine($"*Node");
                    foreach (var node_item in part.Nodes)
                    {
                        var id = node_item.Key;
                        var point = node_item.Value;
                        stw.WriteLine($"{id}, {point.X.ToString(culture)}, {point.Y.ToString(culture)}, {point.Z.ToString(culture)}");
                    }
                    stw.WriteLine("*Element, type=S3R");
                    for (int i = 0; i < part.Triangles.Count; i++)
                    {
                        var triangle = part.Triangles[i];
                        stw.WriteLine($"{i + 1}, {triangle.V1}, {triangle.V2}, {triangle.V3}");
                    }
                    stw.WriteLine($"*End Part");
                }

                stw.WriteLine($"**");
                stw.WriteLine($"**");
                stw.WriteLine($"** ASSEMBLY");
                stw.WriteLine($"**");
                stw.WriteLine($"*Assembly, name=model_assmbly");

                foreach (var part in parts)
                {
                    stw.WriteLine($"*Instance, name={part.Name}_ins, part={part.Name}");
                    stw.WriteLine($"*End Instance");
                }
                stw.WriteLine($"*End Assembly");
            }
        }
    }
}
