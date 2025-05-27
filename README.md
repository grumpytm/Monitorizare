# About the app
Based on a cron schedule (adjustable via Settings.json file) the app downloads two log files in CVS format from a FTP server that's hosted on a PLC (aka. **P**rogramable **L**ogic **C**ontroller), a TM221 series device by Schneider Electric, then process them (parse, validate and sanitize) and save the collected data into a SQLite database from where the user can review or export data to an Excel spreadsheet at any given time from within the app.

Initial WinForms version used MetroModernUI and Krypton.Components.Suite libraries for a fancy look, NPOI library for exporting data to Excel spreadsheet part, [this](https://github.com/HenriqueCaires/cron) cron library, a FTP client I found online (sadly don't recall where I found it), an overcomplicated mutex to maintain a single-instance application, among other smaller things I don't remember right now so I'm not taking credit for all the stuff. :grin:

### A bit of background
Sometimes around 2018 a customer asked us (the company I was working at the time) for help at his pig ranch that was in dire need of some TLC, aka. some major mechanical changes/upgrades to automate things as much as possible in order to increase overall productivity. Taking out (preferably, but not 100% possible) the human factor from the equation was the cherry on the cake, as at the time his employees used the old school "pencil and a piece of paper" method to keep track of things within the "system". Obviously this method prooved to be prone to errors multiple times, hence the upgrades. Semi-automating the process, and I say semi cos someone still had to push some buttons on both ends, helped the customer to gain a more reliable source of information about the type (based on what silo was being filled) and amount (there are weights on both ends) of forage that was created at the local mill and then to where (building and section) said forage was later on distributed to.

Anyway, the customer took care of the mechanical part (rails, silos to deposit the different mixes, scales to weight when flling and emptying, and so on) and we did the automation and logic behind it, aka. keeping track of what goes in and out. The PLC saves data on a SD card in two separate files (one called called 'Incarcare.log' for filling the silos and a file called 'Descarcare.log' when extracting forage from the silos) in a cyclic format (meaning at some point old data would be overwritten and thus potential data could be lost). The PLC has a FTP server where with a known user/password combo (hence why this are still hard coded) one can download and do whatever with those files, after a bit of sanitization.

### **TLDR**:
Started working on this app for two reasons:
- because how was the data saved on the PLC end (cyclic, so records would be overwritten at some point due to space) and because it's in a raw format and would be a hard task for the customer to process the data
- for my pure enjoyment to learn a new programming language.

I gave the fully working app (the initial .NET 4.6.1 version) to the customer free of charge to help him get the data from the PLC from the past and future records without too much hassle on his part. Last time I talked with them the app is used on a daily basis.

### To do:
- [ ] transition to WPF since MetroFramework and Krypton.Components are no longer maintained (they are somewhat still working with .NET 6 but cause some visual artefacts). Progress is slow as I have to change some of the controls to suit my particular needs (look at [this](https://github.com/grumpytm/Monitorizare/blob/master/extra/wpf%20layout.png) for current progress).
- [ ] continue crude experiments with SkiaSharp to get some fancy charts (see 'crude bar chart.png' in Extra for one of them)
- [ ] fully transition from NPOI to MiniExcel for handling the export of data from database into an Excel file

### Done:
- [x] handling the single-instance application with a simplified and more straightforward solution based on ([this](https://stackoverflow.com/a/819808)) answer on StackOverflow
- [x] transitioned to records from using DataTable, mostly impacting the functionality of the log parsing and saving in the database
- [x] externalized some of the previously hard-coded settings into a JSON file to be more flexible to changes via custom service (for example if the customer wishes to change the the PLC's IP address)
- [x] refactored the code in multiple sections of the application, most notable ones are in the 'transport' logs processing (parsing, filtering, and saving to the database)
- [x] restructured the project into separate folders to improve it's maintainability and readability
- [x] added a simple logging functionality to make debugging easier (to some extent at least)
- [x] implemented a thread-safe singleton pattern for managing database connections (SQLite is used, but can be easily changed)
- [x] use FluentFTP for handling the downloads of the log files from the PLC (functionality disabled while developing this)
- [x] refactored code behind the 'Mesaje' and 'Vizualizare' pages and fixed the visual artefacts