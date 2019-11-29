USE users;

CREATE TABLE users.`user` (
	UserId INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Username VARCHAR(100) NOT NULL
);

CREATE UNIQUE INDEX IX_user_Username ON users.`user` (Username);
