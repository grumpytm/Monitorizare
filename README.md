### Summary
At a pig farm, we installed a Schneider Electric TM221 series PLC to monitor the forage production and consumption. A system tray app (running in the background with a small menu) connects hourly (based on a cron schedule) to the PLC’s FTP server, downloads two log files with records of production and consumption, process and validate the records and then store everything in a SQLite database. From where the user can then view or export records to an Excel spreadsheet.

### Detalied version
Back in 2018, a client asked us (my former company) to help automate operations at a pig farm that badly needed modernization. The goal was to reduce human involvement and improve the accuracy of tracking the forage production and consumption data. Previously, employees recorded everything via the old school 'pencil and a piece of paper' method that lead to frequent errors.

We deployed a PLC that logs the production data (file called 'Incarcare.log') and consumption data (file called 'Descarcare.log') to an internal SD card. These logs, accessible via it's FTP server with a known user/password combination, are stored cyclically, meaning old data gets overwritten over time. To avoid a potential data loss, someone had to regularly copy, sanitize and merge new logs, an tedious task and prone to errors.

To solve this issue, I built a WinForms app (initially on .NET 4.6.1) that automates the entire process. I used [MetroModernUI](https://github.com/dennismagno/metroframework-modern-ui) and [Krypton.Components](https://github.com/ComponentFactory/Krypton) libraries to give it a 'fancy look', [NPOI](https://github.com/nissl-lab/npoi/) library for Excel exports, [this](https://github.com/HenriqueCaires/cron) cron library, a FTP client I found online (sadly don't recall where I found it) and an over complicated mutex to enforce single-instance behavior, among other things. I don’t claim credit for all the libraries or tools used, just the glue that holds everything together.

### To do:
- [ ] switch to WPF and upgrade to last available .NET version
- [ ] figure out the shape and form of the charts I want to add to the app via [SkiaSharp](https://github.com/mono/SkiaSharp) library. [Here](https://github.com/grumpytm/SkiaChart) I I fiddled with it a bit to see what can be achieved
- [ ] enhanced the filtering mechanism behind all `DataGridView` controls, as right now I'm relying on three `ComboBox` and two `KryptonDateTimePicker` for filtering and loading data

### Done:
- [x] handling the single-instance application with a simplified and more straightforward solution based on [this](https://stackoverflow.com/a/819808) answer on StackOverflow
- [x] transitioned to records from DataTable to handle collections within the application
- [x] externalized most of the previously hard-coded settings into a JSON file in case the customer wishes to change the the PLC's IP address due to changes in it's internal network or whatnot
- [x] refactored the code in multiple sections of the application, most notable ones are in the logs processing (parsing, filtering, and saving data to the database)
- [x] restructured the project into separate folders to improve (to some extent) it's maintainability and readability
- [x] added a simple logging functionality to make debugging easier (to some extent at least)
- [x] implemented a thread-safe singleton pattern for managing database connections
- [x] using [FluentFTP](https://github.com/robinrodricks/FluentFTP) for handling the downloads of the log files from the PLC
- [x] switched to `StreamReader.ReadLineAsync` and used `record struct` where applicable to reduce memory usage
- [x] refactored UI logic for data viewing, exporting, and log reading and everything is fully functional again
- [x] enhanced all `DataGridView` controls with column sorting, filtering (where needed), and row highlighting for out-of-bounds values
- [x] switched from [NPOI](https://github.com/nissl-lab/npoi/) to [MiniExcel](https://github.com/mini-software/MiniExcel) for exporting
- [x] replaced the integrity check with a simple SQL-based migration system, allowing on-the-fly database structure updates (samples used in `Extra/migrations`)
