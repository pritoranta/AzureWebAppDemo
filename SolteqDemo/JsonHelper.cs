using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace SolteqDemo2
{
    // A class for converting (String key, String value) objects into String-form JSON objects, and vice-versa
    public static class JsonHelper
    {
        // Returns a line of valid JSON (RFC 8259), in format:
        // "[Key]":"[Value]",
        private static String WriteJsonLine(String Key, String Value)
        {
            return $"\"{Key}\": \"{Value}\",";
        }

        // Returns a (String,String) representation of a line of JSON, if the line had objects.
        // Not usable for nested types, and doesn't work with missing '"' characters.
        // Only works with format: "[Key]":"[Value]"
        private static (String,String)? ParseJsonObject(String line)
        {
            String[] SubStrings;
            if ((SubStrings = line.Split('"')).Length>2)
            {
                return (SubStrings[1], SubStrings[3]);
            }
            return null;
        }

        // Returns a String representation of a valid JSON object.
        // The first String of the tuple will be set as the type and the second one as the value.
        public static String JsonObject(IEnumerable<(String, String)> container)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach ((String, String) tuple in container)
            {
                sb.AppendLine(WriteJsonLine(tuple.Item1, tuple.Item2));
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        // Reads a JSON String representation and returns it as a list of
        // (String key,String value) value pairs.
        // Warning! This method is only intended to read JSON String representations that:
        // - are not nested
        // - have '"' characters nesting both value and key.
        // It's safe to use in reading JSON objects made by this class only!
        public static LinkedList<(String, String)> ReadJsonObject(String JO)
        {
            LinkedList<(String, String)> list = new LinkedList<(String, String)>();
            using (StringReader reader = new StringReader(JO))
            {
                String line;
                (String,String)? obj;
                while ((line = reader.ReadLine()) != null)
                {
                    if ((obj = ParseJsonObject(line)) != null)
                        list.AddLast(((String,String)) obj); // casts nullable (String,String)? into non-nullable form
                }
            }
            return list;
        }
    }
}
