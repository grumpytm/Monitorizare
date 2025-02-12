# About the app
Based on a cron schedule (hourly) download two log files that are in CVS format (needs a bit of cleanup) from the FTP server of a PLC (a TM221 series device by Schneider Electric), process (parse, clean and validate) and save content to a SQLite dataabse. User has means to view the log's content within the app or export data to an Excel spreadsheet at any given time.

Initial WinForms version used MetroModernUI and Krypton.Components libraries for a 'fancy look', NPOI library for exporting data to Excel spreadsheet part, [this](https://github.com/HenriqueCaires/cron) cron library, a FTP client I found online (sadly don't recall where I found it), an overcomplicated mutex to maintain a single-instance application, among other smaller things I don't remember right now so I'm not taking credit for all the stuff. :grin:

### A bit of background
Sometimes around 2018 a customer asked us (the company I was working at the time) to help them to automate things as much as possible in order to increase overall productivity at a pig ranch that was in dire need of some TLC, aka. some major mechanical changes/upgrades. Taking out the human factor from the equation (preferably, but not 100% possible) would have been the cherry on the cake, as at the time his employees used the old school 'pencil and a piece of paper' method to keep track of what happened within the "system". This method prooved to be prone to errors multiple times, hence the much needed automation and upgrades. Semi-automating (I say 'semi' cos someone still had to push some buttons on both ends) the process helped the customer to achieve a more reliable source of information about forage mix (based on silo it was deposited after it was weighted) that was created at the local mill and then to where said forage was distributed to, meaning building and section of that building.

Anyway, the customer did the mechanical part (transporting rails, silos to deposit the different forage mixes, scales to weight at input and output, and so on) and we did the automation and logic behind it, aka keeping track of what goes in and out. Thus we used a PLC from Schneider Electric (a TM221 series device, if my memory serves me well) to save data on a SD card in two separate files in a cyclic way (meaning at some point old data would be overwritten), one called called 'Incarcare.log' for loading part and a file called 'Descarcare.log' for taking out part of the system. The PLC has a FTP server where with a known user/password combo (hence why this are still hard coded) one could download and do whatever with those files that are in CVS format with headers and some extra spaces that needs some cleanup.

### **TLDR**:
Started working on this app for two main reasons:
- collected data is saved in a cyclic manner (meaning oldest items are overwritten when new data comes in - aka. FILO) on the SD card located inside the the PLC, thus if the logs aren't saved on a freqvent basis then there's a potential data loss. On top of this, since the logs are in a raw format this would mean the customer would also have had the resposability of processing and filtering the logs in order to only save the new ones. Bottom line is this would have been a PITA for the customer.
- and most important for my pure enjoyment to learn a new programming language.

I gave the fully working app (found in the brach with the .NET Framework 4 version) to the customer free of charge to help them automate the entire process of donwload, process and save the data so the customer could have access to the older records without too much hassle on his part. Last time I talked with them they are still using the app every day.

### Addendum:
- according to the logs in the PLC the first entry is in fact from 24 September 2018, not 2019 as I initially mentioned.

> [!NOTE]
> Apart of fixing bugs or adding new functionality to the application, I also try to align with best practices and community standards as I advance my knowledge of the language. With this in mind I would appreciate any sort of feedback related to the coding style or the application itself. :ok_hand:

### To do:
- [ ] fully switch to WPF since MetroFramework and Krypton.Components are no longer maintained and aren't 100% compatible with .NET 6 and they cause some visual artefacts (look at 'Mesaje' form for example). Progress is slow as I have to change some of the controls to suit my particular needs (file 'wpf layout.png' in 'Extra' folder shows what I've done so far).
- [ ] complete the refactoring the the application where the the UI is mostly involved (considering making a mockup to get this part out of the way)
- [ ] do more experiments with SkiaSharp to get some fancy charts (some examples in the 'Extra' folder)
- [ ] fully transition from NPOI to MiniExcel for handling the export of data from database into an Excel file

### Done:
- [x] handling the single-instance application with a simplified and more straightforward solution based on [this](https://stackoverflow.com/a/819808) answer on StackOverflow
- [x] transitioned to records from using DataTable, mostly impacting the functionality of the log parsing and saving in the database
- [x] externalized some of the previously hard-coded settings into a JSON file in case the customer wishes to change the the PLC's IP address due to changes in it's internal network or whatnot
- [x] refactored the code in multiple sections of the application, most notable ones are in the 'transport' logs processing (parsing, filtering, and saving to the database)
- [x] restructured the project into separate folders to improve it's maintainability and readability
- [x] added a simple logging functionality to make debugging easier (to some extent at least)
- [x] implemented a thread-safe singleton pattern for managing database connections (SQLite is used, but can be easily changed)
- [x] use FluentFTP for handling the downloads of the log files from the PLC (not added to the project yet)