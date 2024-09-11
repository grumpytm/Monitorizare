# A bit of background
Sometimes around 2019 a customer asked us (the company I used to work at the time) for help at his pig ranch that was in dire need of some TLC, aka. some major mechanical changes/upgrades to automate things as much as possible in order to increase overall productivity. Taking out (preferably, but not 100% possible) the human factor out of the equation was the cherry on the cake, as at the time his employees used the old school pencil and a piece of paper method to keep track of what happened in the "system", what food was created at the local mill and where said food was later on distributed. This method prooved to be prone to mistakes, hence the upgrades.

Anyway, the customer did the mechanical part (rails, silos to deposit the different mixes, scales to weight at input and output, and so on) and we did the automation and logic behind it, aka keeping track of what goes in and out. Thus we used a PLC from Schneider Electric (a TM221 series device, if my memory serves me well) to save data on a SD card in two separate files in a cyclic way (meaning at some point old data would be overwritten), one called called 'Incarcare.log' for loading part and a file called 'Descarcare.log' for taking out part of the system. The PLC has a FTP server where with a known user/password combo (hence why this are still hard coded) one could download and do whatever with those files that are a somewhat CVS format.

**TLDR**: Started working on this app for two reasons:
- because how was the data saved on the PLC end (cyclic, so records would be overwritten at some point due to space) and because it's in a raw format and would be a hard task for the customer to process the data
- for my pure enjoyment to learn a new programming language.

I gave the fully working app (the initial .NET Framework version 4 one) to the customer free of charge to help him get the data from the PLC from the past and future records without too much hassle on his part. Last time I talked with them they are still using the app every day.

# About the app
In initial release I've used Visual Studio 2019 Community Edition to put together a few things to automate the process of taking the raw logs from the PLC via it's FTP server on a cron based task, do some processing and save data in a SQLite database so the customer can read/view data at any time, export data in an Excel spreadsheet and whatnot. The UI is designed with the help of MetroModernUI library, some stuff from Krypton.Components.Suite, the Excel processing part is done via NPOI library, [this](https://github.com/HenriqueCaires/cron) cron library, a FTP client I found online over the Internet and I don't know from where exactly.. anyway, not taking credit for doing all the stuff. :)

## To do:
- [ ] switch to using the singleton database (created, partially used)
- [ ] update overall used logic and code
- [ ] create my own FTP client and drop the one used
- [ ] switch from NPOI to MiniExcel
- [ ] miscellaneous bug fixes and improvements
- [ ] fix visual artefacts that appeared after the change from .NET v4 to v6
- [ ] maybe switch to WPF and drop WinForms, MetroModernUI and so on

## Done:
- [x] drop the SingleInstanceMutex.cs altogether in favor of something a lot easier ([link](https://stackoverflow.com/a/819808))
- [x] switched to records instead of DataTable and major code changes in the logs parsing and saving them to the database
- [x] some stuff I don't remember right now xD
