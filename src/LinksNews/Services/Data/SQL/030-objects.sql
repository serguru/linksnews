--use LinksNewsDB;
use DB_A1443B_linksnews
go

create procedure spLogin @Login varchar(100), @Password varchar(100)
as
	set nocount on;

	select Id
	from account
	where
		Login = @Login and Password = hashbytes('sha2_256', @Password) and Locked = 0;
go

create procedure spCreateAccount @Login varchar(100), @Password varchar(100), @Email varchar(100),
	@FirstName nvarchar(255), @LastName nvarchar(255)
as
	set nocount on;

	if (exists(select null from account where Login = @Login))
	begin
		select -1;
		return;
	end

	if (exists(select null from account where Email = @Email))
	begin
		select -2;
		return;
	end

	declare @RoleId bigint = (select Id from arole where Name = 'User');

	insert into account (Login, Password, RoleId, Email, FirstName, LastName)
		values (@Login, hashbytes('sha2_256', @Password), @RoleId, @Email, @FirstName, @LastName);

	select @@identity;		
go

create procedure spUpdatePassword @Login varchar(100), @OldPassword varchar(100), @NewPassword varchar(100)
as
	set nocount on;

	update account set
		Password = hashbytes('sha2_256', @NewPassword)
	where 
		Login = @login and Password = hashbytes('sha2_256', @OldPassword)
go

create procedure spCategoryCount 
as
	set nocount on;

	select -1 as Id, count(distinct AccountId) as Authors, count(distinct Id) as Pages, null as NewsSources
	from page
	where
		PublicAccess = 1
	union all

	select a.Id as Id, count(distinct c.AccountId) as Authors, count(distinct c.Id) as Pages, null as NewsSources
	from contentCategory a
	left join pageCategory b on a.Id = b.ContentCategoryId
	left join page c on c.Id = b.PageId and c.PublicAccess = 1
	group by
		a.Id

go

create procedure spGetContentCategory @Category nvarchar(100)
as
	set nocount on;

	select a.*
	from contentCategory a
	left join contentCategoryMap b on a.Id = b.ContentCategoryId
	where
	  a.Name = @Category or b.Name = @Category
go

create view vwNewsSource 
as
	select 
		a.*, 
		b.Name as ContentCategory, 
		c.Code as CountryCode, 
		c.Name as CountryName,
		d.Code as LanguageCode, 
		d.Name as LanguageName,
		e.Name as NewsProvider
	from newsSource a
	left join contentCategory b on a.ContentCategoryId = b.Id
	left join country c on a.CountryId = c.Id
	left join language d on a.LanguageId = d.Id
	left join newsProviderDef e on a.NewsProviderId = e.Id
go 

create procedure spAddReservedAccount @Login nvarchar(100)
as
	set nocount on;

	declare @l varchar(100);
	set @l = ltrim(rtrim(@Login));
	if (len(@l) = 0)
	begin
		return;
	end

	declare @Email varchar(100);
	set @Email = concat(@l, '@linksnews.org');


	if (exists(select null from account where Login = @l or Email = @Email))
	begin
		return;
	end

	declare @RoleId bigint = (select Id from arole where Name = 'User');

	insert into account (Login, Password, RoleId, Email, Comment)
		values (@l, hashbytes('sha2_256', 'res_acc'), @RoleId, @Email, 'reserved');
go
