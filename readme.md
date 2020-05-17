This small program processes a CSV text file to ensure that any quotes appearing between qualifiers (quotes) are removed.
It will remove quotes between quotes, Input:"Sea"n"  Output:"Sean"

It also handles any whitespace characters such as carrige returns, tabs, or line feed characters that fall between qualifiers (quotes).
On top of random concealed quotes, the files I wrote this for would have random EOL characters as well. Sometimes a TAB and other times a NL and/or CR+NL. These crazy files came from a well known leader in the payments and banking industry. Having supported an integration with thier systems for years, I was not surprised.

I wrote it to prep files for SQL Server Integration Services (SSIS). Even though SSIS and powershell can be custom-coded to handle these situations I ran into difficult edge cases and significant performance issues with them.

Proven to work against things like this:
"Sean","Anderson""
"Sean",""Anderson"
"Sean"","Anderson"
"Se"an","Anderson"
""Sean"",""Anderson""

Of course, this can be adapted to handle other issues you might encounter with text files. If you change anything in this code do not assume it is going to work without exhaustive testing--no matter how trivial the change. It took a while to get this right.

I used a slight variation of this code to process hundreds of large files (10-20GB each) during a short window of time in the midst of a hot cutover for a debit card system migration serving millions of people as thier only means of banking. The success of an entire company-wide migration effort depended on this small bit of code working, and working fast. Performance is not bad.
