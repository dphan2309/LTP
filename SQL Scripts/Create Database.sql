USE [master]
GO
/****** Object:  Database [ltp]    Script Date: 12/18/2018 9:01:12 PM ******/
CREATE DATABASE [ltp]
GO
USE [ltp]
GO
/****** Object:  Table [dbo].[person]    Script Date: 12/18/2018 9:01:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[person](
	[person_id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](50) NOT NULL,
	[last_name] [varchar](50) NOT NULL,
	[state_id] [int] NOT NULL,
	[gender] [char](1) NOT NULL,
	[dob] [datetime] NOT NULL,
 CONSTRAINT [PK_person] PRIMARY KEY CLUSTERED 
(
	[person_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[states]    Script Date: 12/18/2018 9:01:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[states](
	[state_id] [int] IDENTITY(1,1) NOT NULL,
	[state_code] [char](2) NOT NULL,
 CONSTRAINT [PK_states] PRIMARY KEY CLUSTERED 
(
	[state_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[person]  WITH CHECK ADD  CONSTRAINT [FK_person_states] FOREIGN KEY([state_id])
REFERENCES [dbo].[states] ([state_id])
GO
ALTER TABLE [dbo].[person] CHECK CONSTRAINT [FK_person_states]
GO
/****** Object:  StoredProcedure [dbo].[uspPersonSearch]    Script Date: 12/18/2018 9:01:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspPersonSearch] (
	@first_name varchar(50) = '',
	@last_name varchar(50) = '',
	@state_id int = 0,
	@gender char(1) = 'A',
	@dob datetime = '1753-1-1',
	@PageIndex int,
	@PageSize int,
	@RecordCount int output
) AS
BEGIN
	SET NOCOUNT ON;
      
	SELECT ROW_NUMBER() OVER (ORDER BY person_id ASC) AS RowNumber, p.person_id, p.first_name, p.last_name, s.state_id, s.state_code, p.gender, p.dob
	INTO #Results
	FROM person p INNER JOIN states s
	ON p.state_id = s.state_id
	WHERE
		(@first_name = '' OR p.first_name LIKE '%' + @first_name + '%') AND
		(@last_name = '' OR p.last_name LIKE '%' + @last_name + '%') AND
		(@state_id = 0 OR p.state_id = @state_id) AND
		(@gender = 'A' OR p.gender = @gender) AND
		(@dob = '1753-1-1' OR p.dob = @dob)

	SELECT @RecordCount = COUNT(*) FROM #Results
	SELECT * FROM #Results WHERE RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1 AND (((@PageIndex - 1) * @PageSize + 1) + @PageSize) - 1
    DROP TABLE #Results
END
GO
/****** Object:  StoredProcedure [dbo].[uspPersonUpsert]    Script Date: 12/18/2018 9:01:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspPersonUpsert] (
	@person_id int,
	@first_name varchar(50),
	@last_name varchar(50),
	@state_id int,
	@gender char(1),
	@dob datetime
) AS
--MERGE person AS target USING person AS source
--ON target.person_id = source.person_id AND source.person_id = @person_id
--WHEN MATCHED THEN
--	UPDATE SET first_name = @first_name, last_name = @last_name, state_id = @state_id, gender = @gender, dob = @dob
--WHEN NOT MATCHED THEN

--	INSERT (first_name, last_name, state_id, gender, dob)
--	VALUES (@first_name, @last_name, @state_id, @gender, @dob);

IF NOT EXISTS (SELECT 1 FROM person WHERE person_id = @person_id)
BEGIN
	INSERT person (first_name, last_name, state_id, gender, dob)
	VALUES (@first_name, @last_name, @state_id, @gender, @dob)
	SELECT SCOPE_IDENTITY()
END
ELSE
BEGIN
	UPDATE person SET
		first_name = @first_name,
		last_name = @last_name,
		state_id = @state_id,
		gender = @gender, dob = @dob
	WHERE person_id = @person_id
	IF @@ROWCOUNT = 1
		SELECT @person_id
	ELSE
		SELECT 0
END
GO
/****** Object:  StoredProcedure [dbo].[uspStatesList]    Script Date: 12/18/2018 9:01:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspStatesList]
AS
SELECT state_id, state_code FROM states
GO
USE [master]
GO
ALTER DATABASE [ltp] SET  READ_WRITE 
GO
