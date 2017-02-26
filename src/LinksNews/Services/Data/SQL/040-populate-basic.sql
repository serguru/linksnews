set nocount on;
set xact_abort on;

--use LinksNewsDB;
use DB_A1443B_linksnews
go 

begin transaction
go

insert into viewMode(Id,Name) values (1, 'List')
insert into viewMode(Id,Name) values (2, 'Tile')

declare @enLanguageId int;
declare @ruLanguageId int;

insert into language (Code,Name,SupportedByInterface,SupportedByNews) values ('en','English',1,1);
set @enLanguageId = @@Identity;

insert into language (Code,Name,SupportedByInterface,SupportedByNews) values ('ru','Russian',1,1);
set @ruLanguageId = @@Identity;

insert into language (Code,Name,SupportedByInterface,SupportedByNews) values ('de','German',0,1);
insert into language (Code,Name,SupportedByInterface,SupportedByNews) values ('fr','French',0,1);

insert into arole (Name) values ('User');
insert into arole (Name) values ('Administrator');
insert into arole (Name) values ('Anonimus');

insert into columnType (Id, Name) values (1, 'Links');
insert into columnType (Id, Name) values (2, 'News');
insert into columnType (Id, Name) values (3, 'Rows');


declare @newsNewsApiProviderId bigint;
declare @contentCategoryId bigint;
declare @newsLentaRuProviderId bigint; 

insert into newsProviderType(Id, Name) values (1,'Api');
insert into newsProviderType(Id, Name) values (2,'Rss');

insert into newsProviderDef(Name, Website, NewsProviderTypeId) 
	values ('newsapi.org','https://newsapi.org',1);
set @newsNewsApiProviderId = @@Identity;

insert into newsProviderDef(Name, Website, NewsProviderTypeId) 
	values ('lenta.ru','https://lenta.ru',2);
set @newsLentaRuProviderId = @@Identity;


declare @ruCountryId bigint;
set @ruCountryId = (select Id from country where Code = 'ru');

insert into contentCategory (Name) values ('General');
set @contentCategoryId = @@Identity;

	insert into sourcePriority (NewsProviderId, ContentCategoryId, Priority)
		values(@newsNewsApiProviderId, @contentCategoryId, 1);

	insert into sourcePriority (NewsProviderId, ContentCategoryId, Priority)
		values(@newsLentaRuProviderId, @contentCategoryId, 2);


	insert into newsSource
	(
		NewsProviderId,
		NewsSourceId,
		NewsSourceDescription,
		NewsSourceUrl,
		NewsSourceSmallLogoUrl,
		NewsSourceMediumLogoUrl,
		NewsSourceLargeLogoUrl,
		ContentCategoryId,
		CountryId,
		LanguageId
	) values 
	(
		@newsLentaRuProviderId,
		N'lenta.ru новости',
		N'все новости',
		'https://lenta.ru/rss/news',
		'https://assets.lenta.ru/small_logo.png',
		'https://assets.lenta.ru/small_logo.png',
		'https://assets.lenta.ru/small_logo.png',
		@contentCategoryId,
		@ruCountryId,
		@ruLanguageId
	);

	insert into newsSource
	(
		NewsProviderId,
		NewsSourceId,
		NewsSourceDescription,
		NewsSourceUrl,
		NewsSourceSmallLogoUrl,
		NewsSourceMediumLogoUrl,
		NewsSourceLargeLogoUrl,
		ContentCategoryId,
		CountryId,
		LanguageId
	) values 
	(
		@newsLentaRuProviderId,
		N'lenta.ru важные новости',
		N'самые свежие и самые важные новости',
		'https://lenta.ru/rss/top7',
		'https://assets.lenta.ru/small_logo.png',
		'https://assets.lenta.ru/small_logo.png',
		'https://assets.lenta.ru/small_logo.png',
		@contentCategoryId,
		@ruCountryId,
		@ruLanguageId
	);

	insert into newsSource
	(
		NewsProviderId,
		NewsSourceId,
		NewsSourceDescription,
		NewsSourceUrl,
		NewsSourceSmallLogoUrl,
		NewsSourceMediumLogoUrl,
		NewsSourceLargeLogoUrl,
		ContentCategoryId,
		CountryId,
		LanguageId
	) values 
	(
		@newsLentaRuProviderId,
		N'lenta.ru новости за сутки',
		N'главные новости за последние сутки',
		'https://lenta.ru/rss/last24',
		'https://assets.lenta.ru/small_logo.png',
		'https://assets.lenta.ru/small_logo.png',
		'https://assets.lenta.ru/small_logo.png',
		@contentCategoryId,
		@ruCountryId,
		@ruLanguageId
	);

insert into contentCategory (Name) values ('Business');
set @contentCategoryId = @@Identity;

insert into contentCategory (Name) values ('Entertainment');
set @contentCategoryId = @@Identity;

insert into contentCategory (Name) values ('Gaming');
set @contentCategoryId = @@Identity;

insert into contentCategory (Name) values ('Music');
set @contentCategoryId = @@Identity;

insert into contentCategory (Name) values ('Science');
set @contentCategoryId = @@Identity;

	insert into contentCategoryMap (ContentCategoryId, Name) values (@contentCategoryId, 'science-and-nature');

insert into contentCategory (Name) values ('Sport');
set @contentCategoryId = @@Identity;

insert into contentCategory (Name) values ('Technology');
set @contentCategoryId = @@Identity;
go 

execute spCreateAccount 'Lol', 'lol', 'lol@a.b', N'Лол', N'Пупкин';
go 

execute spCreateAccount 'Sergey', 'wv', 'sergey@a.b', N'Сергей', N'Чернышов';
go

insert into exceptionSeverity (Id, Name) values (1, 'Info');
insert into exceptionSeverity (Id, Name) values (2, 'Warning');
insert into exceptionSeverity (Id, Name) values (3, 'Error');
go

insert into communicationMethod (Name) values ('Site');
insert into communicationMethod (Name) values ('Email');
insert into communicationMethod (Name) values ('Telephone');
insert into communicationMethod (Name) values ('Skype');
insert into communicationMethod (Name) values ('SMS');
insert into communicationMethod (Name) values ('Personally');
go

insert into messageGroup (Id, Name) values (1,'Site notification');
insert into messageGroup (Id, Name) values (2,'Contact us');
insert into messageGroup (Id, Name) values (3,'Forum');
go

commit transaction
go
