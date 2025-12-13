namespace StorageAndRetrieval
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //question 1 test
            string input = "12.5, \"sth, stk\", 31.4, \"mmm \\\"kiosk\\\"\", 42";
            var result = StringParsing(input);
            foreach (var (index, number) in result)
            {
                Console.WriteLine($"Index: {index}, Number: {number}");
            }
            //question 2 test
            var projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            projectRoot = Path.GetFullPath(projectRoot);

            var inputFiles = new List<string>
            {
                Path.Combine(projectRoot, "file1.csv"),
                Path.Combine(projectRoot, "file2.csv"),
                Path.Combine(projectRoot, "file3.csv")
            };
            var outputFile = Path.Combine(projectRoot, "output.csv");

            MergeFiles(inputFiles, outputFile);

            Console.WriteLine($"Create output file successfully!");
        }
        public static List<(int, Double)> StringParsing(string input)
        {
            var result = new List<(int, Double)>();
            var stack = new Stack<string>();
            int itemIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                int j = i;
                bool quote = false;
                bool escaped = false;
                if (input[i] == '"')
                {
                    quote = true;
                    stack.Push("\""); 
                    j++;
                }
                while (j < input.Length)
                {
                    char c = input[j];
                    if (c == '\\' && !escaped)
                    {
                        escaped = true;
                        j++;
                        continue;
                    }
                    if (c == '"' && !escaped)
                    {
                        quote = !quote; 
                    }
                    if (c == ',' && !quote)
                        break;
                    escaped = false;
                    j++;
                }

                string item = input.Substring(i, j - i).Trim();

                if (item.StartsWith("\"") && item.EndsWith("\""))
                {
                    item = item.Substring(1, item.Length - 2);
                }

                if (double.TryParse(item, out double number))
                {
                    result.Add((itemIndex, number));
                }

                i = j;
                itemIndex++;

            }
            return result;
        }
        public static void MergeFiles(List<string> inputFiles, string output)
        {
            List<string> listId = new List<string>();
            List<string> listCol = new List<string>();
            Dictionary<string, Dictionary<string, string>> maxtrix = new Dictionary<string, Dictionary<string, string>>();

            // Read each file
            for (int i = 0; i < inputFiles.Count; i++)
            {
                string[] rows = File.ReadAllLines(inputFiles[i]);
                string[] header = rows[0].Split(',');
                int idIndex = -1;
                //find index of header that have Id
                for (int j = 0; j < header.Length; j++)
                {
                    if (header[j] == "ID")
                        idIndex = j;

                    if (!listCol.Contains(header[j]))
                        listCol.Add(header[j]);
                }

                for (int k = 1; k < rows.Length; k++)
                {
                    string[] cells = rows[k].Split(',');
                    string currentId = cells[idIndex];

                    if (!listId.Contains(currentId))
                    {
                        listId.Add(currentId);
                        maxtrix[currentId] = new Dictionary<string, string>();
                    }

                    for (int j = 0; j < header.Length; j++)
                    {
                        maxtrix[currentId][header[j]] = cells[j];
                    }
                }
            }

            // sort columns and id
            List<string> orderedCols = new List<string>();
            orderedCols.Add("ID");
            listCol.Sort();
            for (int k = 0; k < listCol.Count; k++)
            {
                if (listCol[k] != "ID")
                    orderedCols.Add(listCol[k]);
            }

            listId.Sort();

            // wwrite to output file
            StreamWriter writer = new StreamWriter(output);

            //writee header
            string headerRow = "";
            for (int m = 0; m < orderedCols.Count; m++)
            {
                headerRow += orderedCols[m];
                if (m < orderedCols.Count - 1)
                    headerRow += ",";
            }
            writer.WriteLine(headerRow);

            //write dataa rows
            for (int n = 0; n < listId.Count; n++)
            {
                string currentId = listId[n];
                string dataRow = "";

                for (int p = 0; p < orderedCols.Count; p++)
                {
                    string val = "";
                    if (maxtrix[currentId].ContainsKey(orderedCols[p]))
                        val = maxtrix[currentId][orderedCols[p]];
                    dataRow += val;
                    if (p < orderedCols.Count - 1)
                        dataRow += ",";
                }

                writer.WriteLine(dataRow);
            }

            writer.Close();
        }
    }
}
