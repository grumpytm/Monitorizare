-- Rename the 'data' column to 'timestamp' to better reflect the content it's supposed to hold
ALTER TABLE "incarcare" RENAME COLUMN "data" TO "timestamp";

ALTER TABLE "descarcare" RENAME COLUMN "data" TO "timestamp";

ALTER TABLE "migrations" RENAME COLUMN "data" TO "timestamp";

ALTER TABLE "logs" RENAME COLUMN "data" TO "timestamp";

-- Fix the file column type in migrations table
CREATE TABLE "migrations_new" (
	"id"	INTEGER NOT NULL UNIQUE,
	"file"	TEXT NOT NULL UNIQUE,
	"timestamp"	INTEGER NOT NULL,
	PRIMARY KEY("id" AUTOINCREMENT)
);

INSERT INTO "migrations_new" ("id", "file", "timestamp") SELECT "id", "file", "timestamp" FROM "migrations";

DROP TABLE "migrations";

ALTER TABLE "migrations_new" RENAME TO "migrations";