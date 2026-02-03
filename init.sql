CREATE DATABASE Authentication101;
CREATE TABLE Authentication101.dbo.UserPermission (
	username varchar(100) COLLATE Thai_CI_AS NOT NULL,
	refresh_token varchar(MAX) COLLATE Thai_CI_AS NOT NULL,
	password varchar(MAX) COLLATE Thai_CI_AS NOT NULL,
	allow_function varchar(30) COLLATE Thai_CI_AS NULL,
	CONSTRAINT PK_UserPermission PRIMARY KEY (username)
);