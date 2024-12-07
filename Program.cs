using System.Diagnostics;

DirectoryNode root = new("D:\\static website builder\\assets\\<your images folder here>", null);
Stopwatch timer = new();
timer.Start();
root.showChildren();
DirectoryNode.generateHTML(root);
timer.Stop();
Console.WriteLine($"Time taken: {timer}");