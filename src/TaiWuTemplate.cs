using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TaiWuTemplate
{

    string fileName = "";
    public List<EventData> eventData = new List<EventData>();
    public Dictionary<string, EventData> eventMap = new Dictionary<string, EventData>();

    FileInfo file;
    public TaiWuTemplate(FileInfo file)
    {
        EventData activeEventData = null;
        fileName = file.Name;
        this.file = file;
        // Debug.Log($"Generating Template For {fileName}");
        StreamReader textStream = file.OpenText();
        while (!textStream.EndOfStream)
        {
            string line = textStream.ReadLine();
            if (line.Length == 0) continue;
            int index = line.IndexOf(':');
            string key = line.Substring(0, index);
            string value = line.Substring(index + 2); //Skip the ':' and the whitespace

            switch (new string(key.Where(Char.IsLetter).ToArray()))
            {
                case "EventGuid":
                    if (activeEventData != null)
                    {
                        eventData.Add(activeEventData);
                    }
                    activeEventData = new EventData() { guid = value };
                    break;
                case "EventContent":
                    activeEventData.content = value;
                    break;
                case "Option":
                    activeEventData.options.Add(value);
                    break;
            }
        }
        textStream.Close();
        //Adds the last one
        eventData.Add(activeEventData);

        eventMap = eventData.ToDictionary(eventData => eventData.guid);
    }

    public void WriteBackToFile()
    {
        StringBuilder sb = new StringBuilder();
        StreamReader textStream = file.OpenText();
        EventData activeEventData = null;
        while (!textStream.EndOfStream)
        {
            string line = textStream.ReadLine();
            string lineToWrite = line;

            if (line.Length == 0)
            {
                sb.AppendLine();
                continue;
            }

            int index = line.IndexOf(':');
            string key = line.Substring(0, index);
            string value = line.Substring(index + 2); //Skip the ':' and the whitespace

            switch (new string(key.Where(Char.IsLetter).ToArray()))
            {
                case "EventGuid":
                    activeEventData = eventMap[value];
                    break;
                case "EventContent":
                    lineToWrite = activeEventData.GenerateContentString();
                    break;
                case "Option":
                    lineToWrite = activeEventData.GenerateOptString(new string(key.Where(Char.IsDigit).ToArray()));
                    break;
            }
            if (lineToWrite != "")
            {
                sb.AppendLine(lineToWrite);
            }
            else
            {
                sb.AppendLine(line);
            }
        }
        textStream.Close();
        File.WriteAllText(file.FullName, sb.ToString());
    }


    public Dictionary<string, string> FlattenTemplateToDict()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        eventData.ForEach(x =>
        {
            string baseString = $"{fileName}|{x.guid}";
            if (x.content != null || x.content != "" || x.content != " ") dict.Add($"{baseString}|EventContent", x.content);
            for (int i = 0; i < x.options.Count; i++)
            {
                dict.Add($"{baseString}|Option|{i}", x.options[i]);
            }
        });
        return dict;
    }
}

public class EventData
{
    public string guid;
    public string content = "";
    public List<string> options = new List<string>();

    public void ApplyValue(string templateKey, string val, int index = -1)
    {
        if (val == "") return;
        if (templateKey.Contains("Option") && index > -1)
        {
            options[index] = " " + val;
            return;
        }
        else if (templateKey == "EventContent")
        {
            content = " " + val;
            return;
        }

        Console.WriteLine($"Ya fucked up son, ${guid} | {templateKey} , index: {index}");
    }

    public string GenerateOptString(string i)
    {
        string val = options[int.Parse(i) - 1];
        if (val == "") return "";


        return $"		-- Option_{i} :{val}";
    }

    public string GenerateContentString()
    {
        if (content == "") return "";


        return $"		-- EventContent :{content}";
    }
}