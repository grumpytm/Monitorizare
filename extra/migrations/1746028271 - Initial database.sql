CREATE TABLE `incarcare` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` REAL NOT NULL UNIQUE, `siloz` INTEGER NOT NULL, `greutate` INTEGER NOT NULL);

CREATE TABLE `descarcare` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` REAL NOT NULL UNIQUE, `siloz` INTEGER NOT NULL, `greutate` INTEGER NOT NULL, `hala` INTEGER NOT NULL, `buncar` INTEGER NOT NULL);

CREATE TABLE `logs` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` REAL NOT NULL, `msg` TEXT NOT NULL);