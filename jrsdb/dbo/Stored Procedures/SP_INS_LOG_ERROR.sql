create  PROCEDURE [dbo].[SP_INS_LOG_ERROR] 
	@Source as varchar(255),
	@ServerName as varchar(255),
	@StackTrace as text,
	@UserId as char(50)=null,
	@IP as varchar(15)=null,
	@Classe as varchar(50)=null,
	@Procedura as varchar(50)=null,
	@Parametri as varchar(3000)=null


AS
BEGIN
	INSERT INTO dbo.JRS_LOGERRORI
	(Source,StackTrace,ServerName, DataInserimento,UserId,IP,Classe,Procedura,Parametri) values (@Source,@StackTrace,@ServerName,getdate(),@UserId,@IP,@Classe,@Procedura,@Parametri) 
END