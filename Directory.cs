
class DirectoryNode
{
    public String path;
    public DirectoryNode parent;
    public List<DirectoryNode> children;
    public List<String> content;
    public int depth;

    public DirectoryNode(String path, DirectoryNode parent)
    {
        this.path = path;
        children = new();
        content = new();
        this.depth = 0;
        this.parent = parent;
    }

    private DirectoryNode(String path, DirectoryNode parent, int depth)
    {
        this.path = path;
        children = new();
        content = new();
        this.depth = depth;
        this.parent = parent;
    }

    public void showChildren()
    {
        int i = 0;
        string[] subDirectories = Directory.GetDirectories(path);
        String[] Files = Directory.GetFiles(path);
        foreach(String file in Files)
        {
            content.Add(file);
        }
        i++;
        foreach(String dir in subDirectories)
        {
            DirectoryNode node = new(dir, this, this.depth + 1);
            children.Add(node);
            addChild(node);
        }
    }

    public void addChild(DirectoryNode node)
    {
        string[] subDirectories;
        String[] Files;
        try
        {
            subDirectories = Directory.GetDirectories(node.path);
            Files = Directory.GetFiles(node.path);
        }
        catch (System.Exception)
        {
            return;
        }

        foreach(String file in Files)
        {
            node.content.Add(file);
        }

        Parallel.For
        (
            0, subDirectories.Length, (i) =>
            {
                DirectoryNode newNode = new(subDirectories[i], node, node.depth + 1);
                node.children.Add(newNode);
                node.addChild(newNode);
            }
        );

    }

    public static void generateHTML(DirectoryNode node)
    {
        String html = "<html>\n<body>";

        if(node.parent != null)
        {
            html += $"<marquee direction=\"right\">go back \\/</marquee>\n";
            html += $"<br><marquee direction=\"right\"><a href=\"file:///{node.parent.path}.html\">{node.parent.path}</a></marquee>\n";
            html += $"<br><marquee direction=\"right\">go back /\\</marquee>";
        }

        //base condition, if node is a leaf node then it will have no children, therefore ending the recursive nodes
        foreach(DirectoryNode dir in node.children)
        {
            generateHTML(dir);
            html += $"<br><br><a href=\"file:///{dir.path}.html\">{dir.path}</a>\n";
        }
        foreach(String file in node.content)
        {
            if(file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                html += $"<br><br><img src=\"file:///{file}\">\n";
            }
            else if(file.EndsWith(".txt"))
            {
                StreamReader reader = new(File.Open(file, FileMode.Open));
                html += "<hr>this is the text description of this (formerly) anon:<br>\n";
                html += reader.ReadToEnd();
                html += "\n<hr>";
                reader.Close();
            }
        }

        if(node.parent != null)
        {
            html += $"<marquee>go back \\/</marquee>\n";
            html += $"<br><marquee><a href=\"file:///{node.parent.path}.html\">{node.parent.path}</a></marquee>\n";
            html += $"<br><marquee>go back /\\</marquee>";
        }

        html += "</body>\n</html>";
        
        StreamWriter writer = new(File.Open($"{node.path}.html", FileMode.Create));
        writer.Write(html);
        writer.Close();
    }

}