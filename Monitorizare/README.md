# About the app
### Short version
At a pig ranch installed a PLC (aka. Programable Logic Controller, a TM221 series device from Schneider Electric) to monitor the production and consumption of pigs forage. The app runs in task tray (adds an icon in there with a short menu) on a cron schedule (hourly by default) and downloads two log files from PLC's FTP server that after a bit of processing (parsing, cleaning and validating data) saves their content to a SQLite database from where user has means to view or export the content to an Excel spreadsheet and do whatever they please with it.

The initial WinForms version (as in .NET 4.6.1 and then NET 6) I used [MetroModernUI](https://github.com/dennismagno/metroframework-modern-ui) and [Krypton.Components](https://github.com/ComponentFactory/Krypton) libraries to give it a 'fancy look', [NPOI](https://github.com/nissl-lab/npoi/) library for exporting data to Excel spreadsheet, [this](https://github.com/HenriqueCaires/cron) cron library, a FTP client I found online (sadly don't recall where I found it) and an overcomplicated Mutex to maintain a single-instance application among other smaller things I don't remember right now. Bottom line is that I'm not taking credit for everything the app comes bundled with.

### Longer version
Sometimes around 2018 a customer asked us (the company I was working at the time) to help them to automate things as much as possible in order to increase overall productivity at a pig ranch that was in dire need of major mechanical changes and upgrades. The main goal was to limit the human interaction as much as possible and to gain a more reliable source of information about the forage mixes (based on silo it was deposited after it was weighted) that was being created at the local mill and then to where said forage was distributed to ( meaning building and section of that building). Up until the automation upgrade the customer had to rely on employees who used the old school 'pencil and a piece of paper' method to keep track of what happened within the system, obviously this method prooved to be prone to human errors.

The PLC saves loading data (file called 'Incarcare.log') and unloading data (file called 'Descarcare.log') onto an internal SD card, information that can be accessed via it's FTP server with a known user/password combination. Given the way data is saved, meaning in a cyclic pattern resulting in a potential data loss since old data will be overwritten at some point in time, someone would have had to make a copy of the records so information isn't lost. Apart the download itself that someone also had to do some cleanup of the files (remove headers, remove the extra spaces that needs to be cleaned and whatnot) and remove the duplicated logs and whatnot, hence the app.

### **TLDR**:
Started working on this app for two main reasons:
- collected data is saved in a cyclic manner (meaning oldest items are overwritten when new data comes in - aka. FILO) on the SD card located inside the the PLC, thus if the logs aren't saved on a freqvent basis then there's a potential data loss. On top of this, since the logs are in a raw format this would mean the customer would also have had the resposability of processing and filtering the logs in order to only save the new ones. Bottom line is this would have been a PITA for the customer.
- and most important for my pure enjoyment to learn a new programming language.

I gave the customer a free of charge fully working 4.6.1 version of the app to help them automate the entire process of downloading, processing and saving data for future access to the older records without needing any effort on their part. Last time I talked with one of their empoloyees he said that they are still using the app.

### To do:
- [ ] upgrade to last available .NET version and switch to WPF (or maybe something else?) since MetroFramework and Krypton.Components are sadly no longer maintained. Progress on switching to WPF is slow, cos I have to adapt some of the controls to suit my needs or invent some to cover the WinForm's shortcoming in controls.
- [ ] figure out the shape and form of the charts I want to add to the app via [SkiaSharp](https://github.com/mono/SkiaSharp) library. I fiddled a bit with it [here](https://github.com/grumpytm/SkiaChart), nothing fancy.
- [ ] improove the filtering `DataGridView` content mechanism, right now I'm relying on three `ComboBox` and two `KryptonDateTimePicker` for filtering and loading data.

### Done:
- [x] handling the single-instance application with a simplified and more straightforward solution based on [this](https://stackoverflow.com/a/819808) answer on StackOverflow
- [x] transitioned to records from DataTable to handle collections within the appplication
- [x] externalized most of the previously hard-coded settings into a JSON file in case the customer wishes to change the the PLC's IP address due to changes in it's internal network or whatnot
- [x] refactored the code in multiple sections of the application, most notable ones are in the logs processing (parsing, filtering, and saving data to the database)
- [x] restructured the project into separate folders to improve (to some extent) it's maintainability and readability
- [x] added a simple logging functionality to make debugging easier (to some extent at least)
- [x] implemented a thread-safe singleton pattern for managing database connections
- [x] useing [FluentFTP](https://github.com/robinrodricks/FluentFTP) for handling the downloads of the log files from the PLC
- [x] in order to reduce the memory usage I've switched to `StreamReader.ReadLineAsync` and `record struct` (in place where it was possible)
- [x] refactored the code behind the UI forms (view data, export and read logged messages) and once again everything is functional
- [x] all used `DataGridView` have column sorting and filtering (where needed), highlighting rows that aren't within bounds
- [x] switched from [NPOI](https://github.com/nissl-lab/npoi/) to [MiniExcel](https://github.com/mini-software/MiniExcel) for exporting data from database into an Excel file
- [x] replaced integrity check with a migration mechanism to be allow me to perform updates the database structure on the fly via simple SQL statements stored in a .sql file