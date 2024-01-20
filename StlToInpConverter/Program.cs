using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;
using StlToInpConverter;

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
                var part = new Part(part_name, new List<Node>(), new List<IndexTriangle>());
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
                parts.Add(part);
            }
            Console.WriteLine("Writing inp file");
            var inpFileWriter = new InpFileWriter();
            inpFileWriter.WriteInpFile(parts, culture);
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
                if (double.TryParse(datas[i].Replace(" ", ""), NumberStyles.Any, culture, out result))
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
    }
}
