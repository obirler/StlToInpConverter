﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;

namespace StlToInpConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            var dir_info = new DirectoryInfo("files");
            var files = dir_info.EnumerateFiles();

            var parts = new List<Part>();

            var file_count = files.Count();
            var count = 0;
            foreach (var file in files)
            {
                count++;
                Console.WriteLine($"reading file {count}/{file_count}");
                var part_name = file.Name.Split('.')[0];
                var part = new Part(part_name);
                using (var str = new StreamReader(file.FullName))
                {
                    string line;
                    int line_count = 0;
                    while (str.Peek() > 0)
                    {
                        line = str.ReadLine();
                        line_count++;
                        if (line.Contains("endsolid"))
                        {
                            break;
                        }
                        handle_line(line, ref line_count, str, part);
                    }
                }

                private static Point3D ParsePointFromString(string line)
                {
                    line = line.Replace("\t", "");
                    var datas = line.Split(' ');
                    bool xread = false;
                    bool yread = false;
                    bool zread = false;
                    double x = double.NaN;
                    double y = double.NaN;
                    double z = double.NaN;

                    for (int i = 0; i < datas.Length; i++)
                    {
                        double result;
                        if (double.TryParse(datas[i], NumberStyles.Any, culture, out result))
                        {
                            if (!xread)
                            {
                                x = result;
                                xread = true;
                                continue;
                            }

                            if (!yread)
                            {
                                y = result;
                                yread = true;
                                continue;
                            }
                            if (!zread)
                            {
                                z = result;
                                zread = true;
                                continue;
                            }
                        }
                    }
                    return new Point3D(x, y, z);
                }
                parts.Add(part);
            }
            Console.WriteLine("Writing inp file");
            write_inp_file(parts);
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        private static CultureInfo culture;

        private static void handle_line(string line, ref int line_count, StreamReader str, Part part)
        {
            if (line.Contains("facet") && !line.Contains("endfacet"))
            {
                line = str.ReadLine();
                line_count++;

                line = str.ReadLine();
                line_count++;
                var p1 = get_point(line);

                line = str.ReadLine();
                line_count++;
                var p2 = get_point(line);

                line = str.ReadLine();
                line_count++;
                var p3 = get_point(line);

                int i1;
                if (!part.Nodes.Reverse.TryGetValue(p1, out i1))
                {
                    i1 = part.Nodes.Count+1;
                    part.Nodes.Add(i1, p1);
                }

                int i2;
                if (!part.Nodes.Reverse.TryGetValue(p2, out i2))
                {
                    i2 = part.Nodes.Count+1;
                    part.Nodes.Add(i2, p2);
                }

                int i3;
                if (!part.Nodes.Reverse.TryGetValue(p3, out i3))
                {
                    i3 = part.Nodes.Count+1;
                    part.Nodes.Add(i3, p3);
                }

                var il = new IndexTriangle(i1, i2, i3);
                part.Triangles.Add(il);
            }
        }

        private static Point3D get_point(string line)
        {
            line = line.Replace("\t", "");
            var datas = line.Split(' ');
            bool xread = false;
            bool yread = false;
            bool zread = false;
            double x = double.NaN;
            double y = double.NaN;
            double z = double.NaN;

            for (int i = 0; i < datas.Length; i++)
            {
                double result;
                if (double.TryParse(datas[i], NumberStyles.Any, culture, out result))
                {
                    if (!xread)
                    {
                        x = result;
                        xread = true;
                        continue;
                    }
                    if (!yread)
                    {
                        y = result;
                        yread = true;
                        continue;
                    }
                    if (!zread)
                    {
                        z = result;
                        zread = true;
                        continue;
                    }
                }
            }
            return new Point3D(x, y, z);
        }

        private static void write_inp_file(List<Part> parts)
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
                    WritePartSection(stw, part)
                    stw.WriteLine($"*Node");
                    foreach (var node_item in part.Nodes)
                    {
                        var id = node_item.Key;
                        var point = node_item.Value;
                        stw.WriteLine($"{id}, {point.X.ToString(culture)}, {point.Y.ToString(culture)}, {point.Z.ToString(culture)}");
                    }
                    WriteElementSection(stw, part)
                    for (int i = 0; i < part.Triangles.Count; i++)
                    {
                        var triangle = part.Triangles[i];
                        stw.WriteLine($"{i+1}, {triangle.V1}, {triangle.V2}, {triangle.V3}");
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
