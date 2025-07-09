using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Raytracer
{
    class OBJParser
    {
        static public Group ParsOBJFile(string filename)
        {
            Group group = new Group();
            Group currentGroup = group;
            Dictionary<string, Group> subGroups = new Dictionary<string, Group>();
            List<Tuple> vertices = new List<Tuple>();
            List<Tuple> normals = new List<Tuple>();

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        //Console.WriteLine(line);
                        if(words.Length > 0)
                        {
                            if (words[0] == "v")
                            {
                                double v1 = double.Parse(words[1]);
                                double v2 = double.Parse(words[2]);
                                double v3 = double.Parse(words[3]);

                                Tuple v = Tuple.Point(v1, v2, v3);
                                vertices.Add(v);
                            }
                            else if (words[0] == "vn")
                            {
                                double n1 = double.Parse(words[1]);
                                double n2 = double.Parse(words[2]);
                                double n3 = double.Parse(words[3]);

                                Tuple n = Tuple.Vector(n1, n2, n3);
                                normals.Add(n);
                            }
                            else if (words[0] == "f")
                            {
                                string[,] values = new string[words.Length - 1, 3];
                                for (int i = 1; i < words.Length; i++)
                                {
                                    string[] vals = words[i].Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                    for (int j = 0; j < vals.Length; j++)
                                    {
                                        values[i - 1, j] = vals[j];
                                    }
                                }
                                for (int i = 1; i < values.GetLength(0) - 1; i++)
                                {
                                    //int i1 = int.Parse(words[1]);
                                    //int i2 = int.Parse(words[i]);
                                    //int i3 = int.Parse(words[i + 1]);

                                    int i1 = int.Parse(values[0, 0]);
                                    int i2 = int.Parse(values[i, 0]);
                                    int i3 = int.Parse(values[i + 1, 0]);

                                    if (i1 < 0) i1 = vertices.Count + i1 + 1;
                                    if (i2 < 0) i2 = vertices.Count + i2 + 1;
                                    if (i3 < 0) i3 = vertices.Count + i3 + 1;

                                    i1 = i1 - 1;
                                    i2 = i2 - 1;
                                    i3 = i3 - 1;

                                    Tuple v1 = vertices[i1];
                                    Tuple v2 = vertices[i2];
                                    Tuple v3 = vertices[i3];

                                    Triangle t;

                                    if (values[0, 2] != null && values[i, 2] != null && values[i + 1, 2] != null)
                                    {
                                        int ni1 = int.Parse(values[0, 2]);
                                        int ni2 = int.Parse(values[i, 2]);
                                        int ni3 = int.Parse(values[i + 1, 2]);

                                        if (ni1 < 0) i1 = normals.Count + ni1 + 1;
                                        if (ni2 < 0) i2 = normals.Count + ni2 + 1;
                                        if (ni3 < 0) i3 = normals.Count + ni3 + 1;

                                        ni1 = ni1 - 1;
                                        ni2 = ni2 - 1;
                                        ni3 = ni3 - 1;

                                        Tuple n1 = normals[ni1];
                                        Tuple n2 = normals[ni2];
                                        Tuple n3 = normals[ni3];

                                        t = new Triangle(v1, v2, v3, n1, n2, n3);
                                    }
                                    else
                                    {
                                        t = new Triangle(v1, v2, v3);
                                    }


                                    currentGroup.AddChild(t);
                                }
                            }
                            else if (words[0] == "g")
                            {
                                string groupName = words[1];
                                Group subGroup;
                                bool subGroupExists = subGroups.TryGetValue(groupName, out subGroup);
                                if (!subGroupExists)
                                {
                                    subGroup = new Group();
                                    subGroups.Add(groupName, subGroup);
                                }
                                currentGroup = subGroup;
                            }
                        }
                    }
                }
            }
            catch(IOException e)
            {
                Console.WriteLine(" ParseOBJFile Error: filename: " + filename + " not found :(");
                Console.WriteLine(e.ToString());
            }

            foreach (SceneObject subGroup in subGroups.Values)
            {
                group.AddChild(subGroup);
            }

            return group;
        }
    }
}
