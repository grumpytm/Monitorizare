### Summary
At a pig ranch we installed a PLC to monitor the forage production and subsequent consumption. A small app running in the background (has a system tray icon with a menu with a few options) connects automatically (based on a cron schedule, by default is hourly but customisable via `Settings.json` file) to the PLC’s FTP server and from there downloads two log files (the production and subsequent consumption logs), proceeds to parse, sanitize and store validated logs into a SQLite database. From there the user has means to view or export data to an Excel spreadsheet from any moment in the past.

### Detalied version
Back in 2018, a client asked us (my previous employer) to help them automate the 'weighing operations', among other things, at a pig ranch that badly needed modernization. The goal was to reduce human involvement and improve the accuracy of tracking the amount of forage that was being produced (type - based on what silo it was stored) and subsequently consumed (location - based on from what silo it was taken and where it was sent). They where relying on their employees to keep track of everything via the old school 'pencil and a piece of paper' method and that obviously lead to frequent errors.

We deployed Schneider Electric TM221 series PLC that logs the production data (a file called 'Incarcare.log') and consumption data (a file called 'Descarcare.log') to it's internal SD card. These logs, accessible with a known user/password combination via it's internal FTP server, are stored cyclically, meaning old data gets overwritten over time. To avoid a potential data loss someone would have had to regularly copy, sanitize and merge only new logs (to avoid duplicates), a tedious task and obviously prone to errors.

Thereby I built a WinForms app (initially on .NET Framework 4.6.1) where the aforementioned tasks where completed automatically without any human involvement. Among the resources the used at the time: [MetroModernUI](https://github.com/dennismagno/metroframework-modern-ui) and [Krypton.Components](https://github.com/ComponentFactory/Krypton) libraries to give it a 'polished look', [NPOI](https://github.com/nissl-lab/npoi/) for Excel exports, [this](https://github.com/HenriqueCaires/cron) for the cron schedule, a FTP client I found online (sadly don't recall the source) and an over complicated mutex than needed to be to enforce single-instance behavior. I don’t claim credit for all the libraries or tools used, just the glue that holds everything together.

### To do:
- [ ] create a separate repository for the app where I will switch to WPF and upgrade to last available .NET version among other things
- [ ] figure out the look of the charts I want to add to the app via [SkiaSharp](https://github.com/mono/SkiaSharp) library. [Here](https://github.com/grumpytm/SkiaChart) is something I fiddled with to see what can be achieved
- [ ] enhance the filtering mechanism behind all `DataGridView` controls, as right now I'm relying on 3x `ComboBox` and 2x `KryptonDateTimePicker` to achieve this goal

### Done:
- [x] handling the single-instance application with a simplified and more straightforward solution based on [this](https://stackoverflow.com/a/819808) answer on StackOverflow
- [x] transitioned to `records` from `DataTable` to handle collections within the application
- [x] externalized most of the previously hard-coded settings into a JSON file in case the user wishes to make changes to the PLC's IP address or whatever
- [x] refactored the code in multiple sections of the application, most notable ones are in the logs processing (parsing, filtering, and saving data to the database)
- [x] restructured the project into separate folders to improve (to some extent at least) it's maintainability and readability
- [x] added a simple logging functionality to make debugging easier (to some extent at least)
- [x] implemented a thread-safe singleton pattern for managing database connections
- [x] using [FluentFTP](https://github.com/robinrodricks/FluentFTP) for handling the downloads of the log files from the PLC
- [x] switched to `StreamReader.ReadLineAsync` to reduce memory usage when parsing log files
- [x] refactored UI logic for data viewing, exporting, and log reading and everything is fully functional again
- [x] enhanced all `DataGridView` controls with column sorting, filtering (where needed), and row highlighting for out-of-bounds values
- [x] switched from [NPOI](https://github.com/nissl-lab/npoi/) to [MiniExcel](https://github.com/mini-software/MiniExcel) for exporting
- [x] replaced the previously used 'integrity check' with a simple SQL-based migration system, allowing on-the-fly database structure updates (samples used in `Extra/migrations`)
- [x] extended the logging of errors that might show up when attempting to download the log files from the PLC's FTP server and/or when attempting to parse them