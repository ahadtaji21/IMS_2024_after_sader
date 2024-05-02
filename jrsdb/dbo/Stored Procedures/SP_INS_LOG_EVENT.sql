
create procedure[dbo].[SP_INS_LOG_EVENT]
	@Messaggio as Text=null,
	@UserId as char(50),
	@IP as varchar(15),
	@Classe as varchar(50),
	@Procedura as varchar(50),
	@Parametri as varchar(3000)

AS
BEGIN
	INSERT INTO dbo.JRS_LOGEVENTI
           ([Messaggio]
           ,[DataInserimento]
           ,[UserId]
           ,[IP]
           ,[Classe]
           ,[Procedura]
           ,[Parametri])
     VALUES
           (@Messaggio
           ,getdate()
           ,@UserId
           ,@IP 
           ,@Classe
           ,@Procedura 
           ,@Parametri)
END