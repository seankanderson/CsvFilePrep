/****************************************************************************
 * Sean Anderson 2015-2016
 * Public Domain
 * sean.anderson@datavirtue.com
 ****************************************************************************/

using System;
using System.IO;
using System.Text;

namespace CsvFilePrep
{
    internal class CsvFilePrep
    {
        private static void Main(string[] args)
        {
            string sourceFilePath = args[0];
            //string sourceFilePath = "d:\\temp\\messy_file.txt";

            string newFilePath = args[1];
            //string newFilePath = "d:\\temp\\clean_file.txt";

            processFile(sourceFilePath, newFilePath);
        }

        private static void processFile(string sourceFile, string targetFile)
        {
            int character;
            int lastChar = 0;
            StringBuilder newString = new StringBuilder();  //might increase performance to initialize this with a size that would prevent reallocation for each added character per line
            bool betweenQuotes = false;

            using (var filestream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamWriter newFile = File.AppendText(targetFile))
                {
                    var f = new StreamReader(filestream, Encoding.UTF8, true, 8196);

                    /* looks at every character in the file and considers its context -- each loop is one character */
                    while ((character = f.Read()) != -1)  //while not end of file
                    {
                        /* *******************************************************
                         * Footer detection:  This could get pretty elaborate if
                         * you have to leave headers and footers in the files.
                         * For production use this should be checked against an
                         * external phrase file that is read into a dictionary.
                         ********************************************************/
                        if (newString.ToString().Equals("TOTAL")) break;

                        // quote "
                        if (lastChar == 34 && betweenQuotes == false &&
                            (character != 44 && // comma
                             character != 9 && // tab
                             character != 10 && // NL
                             character != 13)) // CR
                        {
                            newString.Remove(newString.Length - 1, 1);
                            betweenQuotes = true;
                        }

                        lastChar = character;

                        if (character == 34)
                        {
                            betweenQuotes = betweenQuotes ? false : true;  //toggle between token state
                        }

                        /*************************************************************
                         * Get rid of commas that are between quotes.
                         * I forget why this is in here.  It can probably be removed.
                         *************************************************************/
                        if (character == 44)  //comma
                        {
                            if (!betweenQuotes)
                            {
                                newString.Append(Convert.ToChar(character));  //if we encounter a comma and we are not between quotes just leave it alone
                                continue;
                            }

                            continue; //discard current character
                        }

                        //     TAB                  RETURN            NL
                        if (character == 9 || character == 10 || character == 13)  //should be able to check for NULL here as well
                        {
                            if (!betweenQuotes)
                            {
                                if (newString.Length == 0) continue;  //skips blank lines without writing them to the target file

                                newFile.WriteLine(newString.ToString());  //end of line, go ahead and write it to the target file
                                newString = new StringBuilder();
                                continue;
                            }

                            continue; //discard current character
                        }

                        newString.Append(Convert.ToChar(character));  //if you made it this far you are just a regular old text character
                    }
                }
            }
        }
    }
}