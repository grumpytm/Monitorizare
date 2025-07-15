<<<<<<< HEAD
CREATE TABLE logs_new (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT DEFAULT "Unset" CHECK(type IN ('Unset', 'Info', 'Warning', 'Error', 'Exception')),
    data INTEGER NOT NULL,
    message TEXT NOT NULL,
    details TEXT
);

INSERT INTO logs_new (id, type, data, message) SELECT id, 'Unset', data, msg FROM logs;

DROP TABLE logs;

=======
CREATE TABLE logs_new (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT DEFAULT "Unset" CHECK(type IN ('Unset', 'Info', 'Warning', 'Error', 'Exception')),
    data INTEGER NOT NULL,
    message TEXT NOT NULL,
    details TEXT
);

INSERT INTO logs_new (id, type, data, message) SELECT id, 'Unset', data, msg FROM logs;

DROP TABLE logs;

>>>>>>> ae7f0dfdf88292892791240606ce211b84c387b5
ALTER TABLE logs_new RENAME TO logs;