using System;
using System.IO;
using System.Collections.Generic;

namespace CommentScraperApp {
    static class CommentScraper {

        [Serializable]
        public struct DelimiterInfo {
            public string[] fileEndings;
            public string[] lineDelimiters;
            public string[] startDelimiters;
            public string[] endDelimiters;
            public string[] ignoreLineDelimiters;
            public string[] startIgnoreDelimiters;
            public string[] endIgnoreDelimiters;
            public string[] specialCharacterDelimiters;
        }

        public class DefaultDelimiterInfos {
            public static DelimiterInfo[] GetDelimiterInfos() {
                DelimiterInfo[] delimiterInfos = new DelimiterInfo[3];
                delimiterInfos[0].fileEndings = new string[] { ".cs", ".java" };
                delimiterInfos[0].lineDelimiters = new string[] { "//" };
                delimiterInfos[0].startDelimiters = new string[] { "/*" };
                delimiterInfos[0].endDelimiters = new string[] { "*/" };
                delimiterInfos[0].ignoreLineDelimiters = new string[] { "///" };
                delimiterInfos[0].startIgnoreDelimiters = new string[] { "\"" };
                delimiterInfos[0].endIgnoreDelimiters = new string[] { "\"" };
                delimiterInfos[0].specialCharacterDelimiters = new string[] { "\\" };

                delimiterInfos[1].fileEndings = new string[] { ".js" };
                delimiterInfos[1].lineDelimiters = new string[] { "//" };
                delimiterInfos[1].startDelimiters = new string[] { "/*" };
                delimiterInfos[1].endDelimiters = new string[] { "*/" };
                delimiterInfos[1].ignoreLineDelimiters = new string[] { "///" };
                delimiterInfos[1].startIgnoreDelimiters = new string[] { "\"", "'" };
                delimiterInfos[1].endIgnoreDelimiters = new string[] { "\"", "'" };
                delimiterInfos[1].specialCharacterDelimiters = new string[] { "\\" };

                delimiterInfos[2].fileEndings = new string[] { ".html" };
                delimiterInfos[2].lineDelimiters = new string[] { };
                delimiterInfos[2].startDelimiters = new string[] { "<!--" };
                delimiterInfos[2].endDelimiters = new string[] { "-->" };
                delimiterInfos[2].ignoreLineDelimiters = new string[] { };
                delimiterInfos[2].startIgnoreDelimiters = new string[] { };
                delimiterInfos[2].endIgnoreDelimiters = new string[] { };
                delimiterInfos[2].specialCharacterDelimiters = new string[] { };



                return delimiterInfos;
            }
        }

        public static void Scrape(string inputDir, string outputFileDir, DelimiterInfo[] delimiterInfos) {

            List<string> selectedFiles = new List<string>();
            {
                string[] allFiles = Directory.GetFiles(inputDir, "*.*", SearchOption.AllDirectories);
                foreach (var fileDir in allFiles)
                    foreach (var delimiterInfo in delimiterInfos)
                        foreach (var fileEnding in delimiterInfo.fileEndings)
                            if (fileEnding == fileDir.Substring(fileDir.Length - fileEnding.Length))
                                selectedFiles.Add(fileDir);
            }      
                            

            using (StreamWriter outFile = new StreamWriter(outputFileDir)) {
                foreach (var fileDir in selectedFiles) {
                    DelimiterInfo curDelimInfo;
                    if ((curDelimInfo = GetDelimInfo(fileDir, delimiterInfos)).fileEndings.Length == 0)
                        continue;

                    bool isIgnoring = false;
                    bool isMultiLineComment = false;
                    string endIgnoreDelim = "";
                    string endDelim = "";
                    outFile.WriteLine("==========" + fileDir.Substring(fileDir.LastIndexOf('\\')+1) +"==========");

                    using (StreamReader inFile = new StreamReader(fileDir)) {
                        string line;
                        while ((line = inFile.ReadLine()) != null) {
                            string outputString = "";
                            for (int i = 0; i < line.Length; i++) {

                                if (!isMultiLineComment && isIgnoring)
                                    if (IsThisDelimiter(line, i, curDelimInfo.specialCharacterDelimiters)) {
                                        i += GetThisDelimiter(line, i, curDelimInfo.specialCharacterDelimiters).Length;
                                        continue;
                                    }

                                if (!isMultiLineComment && !isIgnoring) 
                                    if (IsThisDelimiter(line, i, curDelimInfo.ignoreLineDelimiters))
                                        break;

                                if (!isMultiLineComment) {
                                    if (!isIgnoring && IsThisDelimiter(line, i, curDelimInfo.startIgnoreDelimiters)) {
                                        endIgnoreDelim = GetEndIgnoreDelimiter(line, i, curDelimInfo);
                                        isIgnoring = true;
                                        continue;
                                    }

                                    if (isIgnoring && IsThisDelimiter(line, i, endIgnoreDelim)) {
                                        isIgnoring = false;
                                        i += endIgnoreDelim.Length - 1;
                                        endIgnoreDelim = "";
                                        continue;
                                    }
                                }

                                if (!isIgnoring) {
                                    if (!isMultiLineComment && IsThisDelimiter(line, i, curDelimInfo.startDelimiters)) {
                                        endDelim = GetEndDelimiter(line, i, curDelimInfo);
                                        isMultiLineComment = true;
                                        outputString += line[i].ToString();
                                        continue;
                                    }

                                    if (isMultiLineComment && IsThisDelimiter(line, i, endDelim)) {
                                        isMultiLineComment = false;
                                        outputString += endDelim;
                                        i += endDelim.Length - 1;
                                        endDelim = "";
                                        continue;
                                    }

                                    if (isMultiLineComment) {
                                        outputString += line[i].ToString();
                                        continue;
                                    }

                                    if (!isMultiLineComment && IsThisDelimiter(line, i, curDelimInfo.lineDelimiters)) {
                                        outFile.WriteLine(line.Substring(i));
                                        break;
                                    }
                                }
                            }
                            if (outputString != "")
                                outFile.WriteLine(outputString);
                        }
                    }
                }
            }
        }

        private static string GetEndIgnoreDelimiter(string line, int index, DelimiterInfo delimInfo) {
            for (int i = 0; i < delimInfo.startIgnoreDelimiters.Length; i++)
                for (int j = 0; j < delimInfo.startIgnoreDelimiters[i].Length; j++)
                    if (index + delimInfo.startIgnoreDelimiters[i].Length - 1 < line.Length && delimInfo.startIgnoreDelimiters[i] == line.Substring(index, j + 1))
                        return delimInfo.endIgnoreDelimiters[i];

            return "";
        }


        private static string GetEndDelimiter(string line, int index, DelimiterInfo curDelimInfo) {
            for (int i = 0; i < curDelimInfo.startDelimiters.Length; i++) 
                for (int j = 0; j < curDelimInfo.startDelimiters[i].Length; j++) 
                    if (index + curDelimInfo.startDelimiters[i].Length-1 < line.Length && curDelimInfo.startDelimiters[i] == line.Substring(index, j + 1))
                        return curDelimInfo.endDelimiters[i];
                
            return "";
        }

        private static string GetThisDelimiter(string line, int index, string[] delimiters) {
            foreach (var delim in delimiters)
                for (int i = 0; i < delim.Length; i++)
                    if (index + delim.Length - 1 < line.Length && delim == line.Substring(index, i + 1))
                        return delim;
            return "";
        }

        private static bool IsThisDelimiter(string line, int index, string[] delimiters) {
            foreach (var delimiter in delimiters)
                for (int i = 0; i < delimiter.Length; i++)
                    if (index + delimiter.Length - 1 < line.Length && delimiter == line.Substring(index, i + 1))
                        return true;

            return false;
        }

        private static bool IsThisDelimiter(string line, int index, string delimiter) {
                for (int i = 0; i < delimiter.Length; i++)
                    if (index + delimiter.Length - 1 < line.Length && delimiter == line.Substring(index, i + 1))
                        return true;

            return false;
        }

        private static DelimiterInfo GetDelimInfo(string filePath, DelimiterInfo[] delimInfos) {
            DelimiterInfo delimInfoRez = new DelimiterInfo();
            foreach (var delimInfo in delimInfos)
                foreach (var fileEnding in delimInfo.fileEndings)
                    if (filePath.Contains(fileEnding))
                        return delimInfo;
            return delimInfoRez;
        }

    }
}


//do what you can't