CREATE TABLE Course
(
	Id INT,
	ExamImage VARCHAR(100),
	CourseName VARCHAR(100),
	Rating NUMERIC(2,1)
);

INSERT INTO Course(Id, ExamImage, CourseName, Rating) VALUES(1, 'https://az305demost.blob.core.windows.net/images/AZ-900.png', 'AZ-900 Fundamentals', 4.4);
INSERT INTO Course(Id, ExamImage, CourseName, Rating) VALUES(2, 'https://az305demost.blob.core.windows.net/images/AZ-104.png', 'AZ-104 Administator', 4.5);
INSERT INTO Course(Id, ExamImage, CourseName, Rating) VALUES(3, 'https://az305demost.blob.core.windows.net/images/AZ-204.png', 'AZ-204 Developer', 4.6);
INSERT INTO Course(Id, ExamImage, CourseName, Rating) VALUES(4, 'https://az305demost.blob.core.windows.net/images/AZ-305.png', 'AZ-305 Architect', 4.7);

SELECT * FROM Course;
