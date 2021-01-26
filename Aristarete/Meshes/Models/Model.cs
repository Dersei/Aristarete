using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Utils;

namespace Aristarete.Meshes.Models
{
    public class Model
    {
        public List<Triangle> Triangles { get; } = new();
        public List<Float3> Vertices { get; } = new();
        public List<Float3> Normals { get; } = new();
        public List<Int3[]> Faces { get; } = new();
        public List<Float2> UV { get; } = new();

        public PbrMaterial Material = PbrMaterial.Error;
        
        private Model()
        {
        }

        public static Model LoadFromFile(string filename, PbrMaterial material, float scale = 1f,
            Float3 position = default, Float3 rotationAxis = default, float angle = 0)
        {
            using var stream = new StreamReader(filename);
            var model = new Model {Material = material};
            
            while (!stream.EndOfStream)
            {
                var line = stream.ReadLine();
                if (line is null)
                {
                }
                else if (line.StartsWith("v "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(v => float.Parse(v, CultureInfo.InvariantCulture))
                        .ToArray();
                    model.Vertices.Add(Float3.Transform(new Float3(array[0], array[1], array[2]), scale, position,
                        rotationAxis, angle));
                }
                else if (line.StartsWith("f "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(fvn => fvn.Split("/").Select(int.Parse).ToArray())
                        .Select(a => new Int3(--a[0], --a[1], --a[2])).ToArray();
                    model.Faces.Add(array);
                }
                else if (line.StartsWith("vt "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(v => float.Parse(v, CultureInfo.InvariantCulture)).ToArray();
                    model.UV.Add(new Float2(array[0], 1 - array[1]));
                }
                else if (line.StartsWith("vn "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(v => float.Parse(v, CultureInfo.InvariantCulture))
                        .ToArray();
                    model.Normals.Add(new Float3(array[0], array[1], array[2]));
                }
            }


            for (var index = 0; index < model.Faces.Count; index++)
            {
                var face = model.GetFace(index);
                model.Triangles.Add(new Triangle(
                    new Vertex(model.Vertices[face[0]], model.Normals[model.Faces[index][0].Z], model.GetUV(index, 0)),
                    new Vertex(model.Vertices[face[1]], model.Normals[model.Faces[index][1].Z], model.GetUV(index, 1)),
                    new Vertex(model.Vertices[face[2]], model.Normals[model.Faces[index][2].Z],
                        model.GetUV(index, 2))));
            }

            Console.WriteLine(model.Triangles.Count);
            return model;
        }

        public static Model LoadFromFileSafe(string filename, PbrMaterial material, float scale = 1f,
            Float3 position = default, Float3 rotationAxis = default, float angle = 0)
        {
            var obj = new Model();
            try
            {
                using var reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
                obj = LoadFromStringSafe(reader.ReadToEnd(), material, scale, position, rotationAxis, angle);
            }
            catch (FileNotFoundException)
            {
                Logger.LogConsole($@"File not found: {filename}");
            }
            catch (Exception)
            {
                Logger.LogConsole($@"Error loading file: {filename}");
            }

            return obj;
        }


        private static Model LoadFromStringSafe(string text, PbrMaterial material, float scale, Float3 position,
            Float3 rotationAxis, float angle)
        {
            const NumberStyles style = NumberStyles.Number;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            // Separate lines from the file
            var lines = new List<string>(text.Split('\n'));

            var model = new Model();

            // Lists to hold model data
            var faces = new List<(TempVertex, TempVertex, TempVertex)>();

            // Read file line by line
            foreach (var line in lines)
            {
                if (line.StartsWith("v ")) // Vertex definition
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    Float3? vec = null;

                    if (temp.Trim().Count(c => c == ' ') == 2) // Check if there's enough elements for a vertex
                    {
                        var vertexParts = temp.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        // Attempt to parse each part of the vertex
                        var success = float.TryParse(vertexParts[0], style, culture, out var x);
                        success |= float.TryParse(vertexParts[1], style, culture, out var y);
                        success |= float.TryParse(vertexParts[2], style, culture, out var z);

                        vec = new Float3(x, y, z);
                        vec = Float3.Transform(vec.Value, scale, position, rotationAxis, angle);
                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing vertex: {line}");
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing vertex: {line}");
                    }

                    model.Vertices.Add(vec ?? new Float3());
                }
                else if (line.StartsWith("vt ")) // Texture coordinate
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    Float2? vec = null;

                    if (temp.Trim().Count(c => c == ' ') > 0) // Check if there's enough elements for a vertex
                    {
                        var texCoordParts = temp.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        // Attempt to parse each part of the vertex
                        var success = float.TryParse(texCoordParts[0], style, culture, out var x);
                        success |= float.TryParse(texCoordParts[1], style, culture, out var y);

                        vec = new Float2(x, 1 - y);

                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing texture coordinate: {line}");
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing texture coordinate: {line}");
                    }

                    model.UV.Add(vec ?? new Float2());
                }
                else if (line.StartsWith("vn ")) // Normal vector
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    Float3? vec = null;

                    if (temp.Trim().Count(c => c == ' ') == 2) // Check if there's enough elements for a normal
                    {
                        var vertexParts = temp.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        // Attempt to parse each part of the vertice
                        var success = float.TryParse(vertexParts[0], style, culture, out var x);
                        success |= float.TryParse(vertexParts[1], style, culture, out var y);
                        success |= float.TryParse(vertexParts[2], style, culture, out var z);

                        vec = new Float3(x, y, z);

                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing normal: {line}");
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing normal: {line}");
                    }

                    model.Normals.Add(vec ?? new Float3());
                }
                else if (line.StartsWith("f ")) // Face definition
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    if (temp.Trim().Count(c => c == ' ') == 2) // Check if there's enough elements for a face
                    {
                        var faceParts = temp.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        int t1, t2, t3;
                        int n1, n2, n3;

                        // Attempt to parse each part of the face
                        var success = int.TryParse(faceParts[0].Split('/')[0], out var v1);
                        success |= int.TryParse(faceParts[1].Split('/')[0], out var v2);
                        success |= int.TryParse(faceParts[2].Split('/')[0], out var v3);

                        if (faceParts[0].Count(c => c == '/') >= 2)
                        {
                            success |= int.TryParse(faceParts[0].Split('/')[1], out t1);
                            success |= int.TryParse(faceParts[1].Split('/')[1], out t2);
                            success |= int.TryParse(faceParts[2].Split('/')[1], out t3);
                            success |= int.TryParse(faceParts[0].Split('/')[2], out n1);
                            success |= int.TryParse(faceParts[1].Split('/')[2], out n2);
                            success |= int.TryParse(faceParts[2].Split('/')[2], out n3);
                        }
                        else
                        {
                            if (model.UV.Count > v1 && model.UV.Count > v2 && model.UV.Count > v3)
                            {
                                t1 = v1;
                                t2 = v2;
                                t3 = v3;
                            }
                            else
                            {
                                t1 = 0;
                                t2 = 0;
                                t3 = 0;
                            }


                            if (model.Normals.Count > v1 && model.Normals.Count > v2 && model.Normals.Count > v3)
                            {
                                n1 = v1;
                                n2 = v2;
                                n3 = v3;
                            }
                            else
                            {
                                n1 = 0;
                                n2 = 0;
                                n3 = 0;
                            }
                        }


                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing face: {line}");
                        }
                        else
                        {
                            var tv1 = new TempVertex(v1, n1, t1);
                            var tv2 = new TempVertex(v2, n2, t2);
                            var tv3 = new TempVertex(v3, n3, t3);
                            faces.Add((tv1, tv2, tv3));
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing face: {line}");
                    }
                }
            }

            // Create the Model
            Console.WriteLine(model.Normals.Count);
            Console.WriteLine(model.Vertices.Count);
            Console.WriteLine(faces.Count);
            foreach (var face in faces)
            {
                var v1 = new Vertex(model.Vertices[face.Item1.Vertex], model.Normals[face.Item1.Normal],
                    model.UV[face.Item1.TexCoords]);
                var v2 = new Vertex(model.Vertices[face.Item2.Vertex], model.Normals[face.Item2.Normal],
                    model.UV[face.Item2.TexCoords]);
                var v3 = new Vertex(model.Vertices[face.Item3.Vertex], model.Normals[face.Item3.Normal],
                    model.UV[face.Item3.TexCoords]);

                model.Triangles.Add(new Triangle(v1, v2, v3));
            }

            Console.WriteLine(model.Triangles.Count);
            model.Material = material;
            return model;
        }

        private class TempVertex
        {
            public readonly int Vertex;
            public readonly int Normal;
            public readonly int TexCoords;

            public TempVertex(int vertex = 0, int norm = 0, int tex = 0)
            {
                Vertex = vertex;
                Normal = norm;
                TexCoords = tex;
            }
        }


        private Int3 GetFace(int idx) =>
            new(
                Faces[idx][0].X,
                Faces[idx][1].X,
                Faces[idx][2].X
            );

        private Float2 GetUV(int face, int vertex)
        {
            var idx = Faces[face][vertex].Y;
            return new Float2(UV[idx].X, UV[idx].Y);
        }

        public ModelEnumerator GetEnumerator()
        {
            return new(Triangles);
        }


        public class ModelEnumerator
        {
            private readonly List<Triangle> _triangles;
            public ModelEnumerator(List<Triangle> triangle) => _triangles = triangle;
            private int _currentIndex = -1;
            public bool MoveNext() => ++_currentIndex < _triangles.Count;
            public void Reset() => _currentIndex = -1;
            public Triangle Current => _triangles[_currentIndex];
        }
    }
}