CREATE TABLE "logs_new" (
	"id"	INTEGER,
	"type"	TEXT DEFAULT Unset CHECK("type" IN ('Unset', 'Info', 'Warning', 'Error', 'Exception')),
	"data"	INTEGER NOT NULL,
	"message"	TEXT NOT NULL,
	"details"	TEXT,
	PRIMARY KEY("id" AUTOINCREMENT)
);

INSERT INTO "logs_new" (id, type, data, message) SELECT "id", 'Unset', "data", "msg" FROM "logs";

DROP TABLE "logs";

ALTER TABLE "logs_new" RENAME TO "logs";