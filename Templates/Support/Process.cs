using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Templates.Support;

public class Process {

    Dictionary<string, string> Variables;
    List<string> InstalledFiles = new List<string>();

    public Process(Dictionary<string, string> variables) {
        Variables = variables;
    }

    public Task CopyFilesAsync(string sourcePath, string destPath) {
        InternalCopyFiles(sourcePath, destPath);
        ProcessFolder(destPath);
        return Task.CompletedTask;
    }

    protected void InternalCopyFiles(string sourcePath, string destPath) {
        Directory.CreateDirectory(destPath);
        foreach (string file in Directory.GetFiles(sourcePath)) {
            string name = Path.GetFileName(file)!;
            if (ProcessFileName(ref name))
                PreProcessFile(file, destPath, name);
        }
        foreach (string dir in Directory.GetDirectories(sourcePath)) {
            string name = Path.GetFileName(dir)!;
            if (ProcessFileName(ref name))
                InternalCopyFiles(dir, Path.Combine(destPath, name));
        }
    }

    protected void ProcessFolder(string folder) {
        folder = folder.ToLower();
        string d = Path.GetFileName(Path.GetDirectoryName(folder))!;
        if (d == "bin") return;
        if (d == "obj") return;
        if (d == "node_modules") return;

        foreach (string dir in Directory.GetDirectories(folder)) {
            ProcessFolder(dir);
        }
        foreach (string file in Directory.GetFiles(folder)) {
            ProcessFile(file);
        }
    }

    private void PreProcessFile(string inFile, string outPath, string outName) {
        string ext = Path.GetExtension(inFile).ToLower();
        string outFile = Path.Combine(outPath, outName);
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp") {
            // always copied
        } else {
            List<string> lines = File.ReadAllLines(inFile).ToList();
            if (lines.Count > 0) {
                string expr = lines[0].Trim();
                if (expr.StartsWith("$pp")) {
                    // preprocess marker found
                    expr = expr.Substring(3).Trim();
                    if (string.IsNullOrWhiteSpace(expr)) throw new InternalError($"Missing $pp expression in {inFile}");
                    if (!Evaluate(expr))
                        return;// file not copied
                    lines.RemoveAt(0);
                    InstalledFiles.Add(outName.ToLower());
                    File.WriteAllLines(inFile, lines);
                    return;
                }
            }
        }
        File.Copy(inFile, outFile, true);
        InstalledFiles.Add(outName.ToLower());
        return;
    }

    private void ProcessFile(string inFile) {
        string ext = Path.GetExtension(inFile).ToLower();
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp")
            return;

        List<string> lines = File.ReadAllLines(inFile).ToList();
        if (lines.Count < 1) return;

        lines = ProcessAllLines(lines);
        File.WriteAllLines(inFile, lines);
    }

    private enum Mode {
        None,
        SkipIf,
        ProcessIf,
        SkipToEnd,
    }

    private List<string> ProcessAllLines(List<string> lines) {
        List<string> newLines = new List<string>();
        Mode mode = Mode.None;
        foreach (string l in lines) {
            string line = ProcessLine(l);
            switch (mode) {
                case Mode.None:
                    if (line.StartsWith("$iff ")) {
                        string f = line.Substring(4);
                        if (Evaluate(f))
                            mode = Mode.ProcessIf;
                        else
                            mode = Mode.SkipIf;
                    } else
                        newLines.Add(line);
                    break;
                case Mode.ProcessIf:
                    if (line.Trim() == "$end")
                        mode = Mode.None;
                    else if (line.StartsWith("$elseiff "))
                        mode = Mode.SkipToEnd;
                    else if (line.Trim() == "$else")
                        mode = Mode.SkipToEnd;
                    else
                        newLines.Add(line);
                    break;
                case Mode.SkipIf:
                    if (line.Trim() == "$end")
                        mode = Mode.None;
                    else if (line.StartsWith("$elseiff ")) {
                        string f = line.Substring(8);
                        if (Evaluate(f))
                            mode = Mode.ProcessIf;
                        else
                            mode = Mode.SkipIf;
                    } else if (line.Trim() == "$else") {
                        mode = Mode.ProcessIf;
                    }
                    break;
                case Mode.SkipToEnd:
                    if (line.Trim() == "$end")
                        mode = Mode.None;
                    break;
            }
        }
        if (mode != Mode.None)
            throw new ApplicationException("Unterminated $iff statement");
        return newLines;
    }

    private bool Evaluate(string expr) {
        expr = expr.Trim();
        if (InstalledFiles.Contains(expr.ToLower()))
            return true;
        if (expr.StartsWith("$dp$") && Variables.ContainsKey("$dp$")) {
            string dp = Variables["$dp$"];
            expr = expr.Substring(4).Trim();
            if (expr.StartsWith("==")) {
                expr = expr.Substring(2).Trim();
                return expr == dp;
            } else if (expr.StartsWith("!=")) {
                expr = expr.Substring(2).Trim();
                return expr != dp;
            } else
                throw new InternalError($"Invalid expression {expr}");
        }
        return false;
    }

    private string ProcessLine(string line) {
        foreach (var entry in Variables) {
            line = line.Replace(entry.Key, entry.Value);
        }
        return line;
    }
    private bool ProcessFileName(ref string name) {
        foreach (var entry in Variables) {
            if (name.Contains(entry.Key)) {
                if (string.IsNullOrEmpty(entry.Value))
                    return false;
                name = name.Replace(entry.Key, entry.Value);
                return true;
            }
        }
        return true;
    }
}