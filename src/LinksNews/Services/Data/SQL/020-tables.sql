-- TODO: review all tables for indices
--use LinksNewsDB;
use DB_A1443B_linksnews
go

create table viewMode
(
	Id int not null primary key check(Id in (1,2)),
	Name varchar(100) not null unique
)
go

create table country
(
	Id bigint not null identity primary key,
	Code varchar(2) not null unique,
	Name varchar(100) not null unique
)
go

create table theme
(
	Id bigint not null identity primary key,
	Name nvarchar(100) not null unique
)
go

create table language
(
	Id bigint not null identity primary key,
	Code nvarchar(2) not null unique,
	Name nvarchar(255) not null unique,
	SupportedByInterface bit not null default(0),
	SupportedByNews bit not null default(0)
)
go

create table englishMessage
(
	Id bigint not null identity primary key,
	Name varchar(900) not null unique
)
go

create table translate
(
	Id bigint not null identity primary key,
	Name nvarchar(max) not null,
	EnglishMessageId bigint not null references englishMessage(Id),
	LanguageId bigint not null references language(Id),
	Reference bit not null default(0),

	constraint uk0_translate unique (EnglishMessageId, LanguageId) 
)
go

create table translateVersion
(
	Id bigint not null identity primary key,
	TranslateId bigint not null references translate(Id),
	Version nvarchar(max) not null,
	Name nvarchar(max) not null,
	Reference bit not null default(0)
)
go

create table arole
(
	Id bigint not null identity primary key,
	Name nvarchar(100) not null unique
)
go 

create table account
(
	Id bigint not null identity primary key,
	RoleId bigint not null references arole(Id),
	Login varchar(100) not null unique check(len(Login) >= 3),
	Password varbinary(max) not null,
	FirstName nvarchar(255) null,
	LastName nvarchar(255) null,
	LanguageId bigint null references language(Id),
	Email varchar(100)  not null unique check(len(Email) >= 5),
	Address nvarchar(max) null,
	Telephone nvarchar(100) null,
	Website nvarchar(100) null,
	Comment nvarchar(max) null,
	ThemeId bigint null references theme(Id),
	Locked bit not null default(0),
	DateCreated datetimeoffset not null default(GetUtcDate()),
	-- in minutes
	NewsRefreshInterval bigint null check(NewsRefreshInterval >= 10) default(60)
);
go

create table website
(
	Id bigint not null identity primary key,
	Name nvarchar(255) not null unique
)
go

create table visit
(
	Id bigint not null identity primary key,
	AccountId bigint null references account(Id),
	RequestIp varchar(max) null,
	Route varchar(max) null,
	RequestTime datetimeoffset not null
)
go


create table contentCategory
(
	Id bigint not null identity primary key,
	Name nvarchar(100) not null unique,
	Description nvarchar(max) null
)
go

create table contentCategoryMap
(
	Id bigint not null identity primary key,
	ContentCategoryId bigint not null references contentCategory(Id),
	Name nvarchar(100) not null unique
)
go

create table page
(
	Id bigint not null identity primary key,
	AccountId bigint not null references account(Id),
	Name varchar(100) not null,
	Title nvarchar(100) not null,
	PublicAccess bit not null default (0),
	Description nvarchar(max) null,
	PageIndex bigint not null check(PageIndex >= 0),
	ShowTitle bit not null default(1),
	ShowDescription bit not null default(0),
	ShowImage bit not null default(1),

	constraint page_uk1 unique (AccountId, Name)
)
go

-- TODO: add triggers ensuring not including own pages as external 
create table accountPage
(
	Id bigint not null identity primary key,
	PageId bigint not null references page(Id),
	AccountId bigint not null references account(Id),
	PageIndex bigint not null check(PageIndex >= 0),

	constraint accountPage_uk1 unique (PageId, AccountId)
)
go

create table pageCategory
(
	Id bigint not null identity primary key,
	PageId bigint not null references page(Id),
	ContentCategoryId bigint not null references contentCategory(Id),

	constraint pageCategory_uk1 unique (PageId, ContentCategoryId)
)
go

create table newsProviderType
(
	Id int not null primary key check(Id in (1,2)),
	Name nvarchar(100) not null unique
)
go 

create table newsProviderDef
(
	Id bigint not null identity primary key,
	Name nvarchar(100) not null unique,
	Website nvarchar(max) null,
	Locked bit not null default(0),
	NewsProviderTypeId int not null references newsProviderType(Id)
)
go 

create table sourcePriority
(
	Id bigint not null identity primary key,
	NewsProviderId bigint null references newsProviderDef(Id),
	ContentCategoryId bigint not null references contentCategory(Id),
	Priority bigint not null check(Priority > 0),
	constraint providerCategoryPriority_uk1 unique (ContentCategoryId, Priority)
)
go

create table newsSource
(
	Id bigint not null identity primary key,
	NewsProviderId bigint null references newsProviderDef(Id),

    NewsSourceId nvarchar(100) not null unique,
    NewsSourceDescription nvarchar(max) null,
    NewsSourceUrl varchar(max) not null,

    NewsSourceSmallLogoUrl nvarchar(max) null,
    NewsSourceMediumLogoUrl nvarchar(max) null,
    NewsSourceLargeLogoUrl nvarchar(max) null,

	ContentCategoryId bigint not null references contentCategory(Id),

	CountryId bigint null references country(Id),
	LanguageId bigint null references language(Id)
)
go

create table columnType
(
	Id int not null primary key check (Id between 0 and 3),
	Name nvarchar(100) not null unique
)
go

create table arow
(
	Id bigint not null identity primary key check (Id > 0),
	ColumnId bigint not null,
	RowIndex bigint not null check (RowIndex >= 0),
	Title nvarchar(100) not null,
	ShowTitle bit not null default(0)

	--constraint arow_uk1 unique (RowIndex, ColumnId)
)
go

-- TODO: create a trigger preventing columns overlapping
-- TODO: create a trigger preventing having more than 100 rows per column
create table acolumn
(
	Id bigint not null identity primary key check(Id >= 0),
	ColumnTypeId int not null references columnType(Id) default(0),
	RowId bigint null references arow(Id),
	PageId bigint null references page(Id),
	ColumnIndex bigint not null check (ColumnIndex between 1 and 12),
	ColumnWidth bigint not null check (ColumnWidth between 1 and 12),
	Title nvarchar(100) not null,
	Description nvarchar(max) null,
	NewsProviderId bigint null references newsProviderDef(Id),
	NewsProviderSourceId nvarchar(100) null,
	ShowTitle bit not null default(1),
	ShowImage bit not null default(0),
	ShowDescription bit not null default(0),
	ShowNewsImages bit not null default(1),
	ShowNewsDescriptions bit not null default(1),
	ViewModeId int not null references viewMode(Id),

	--constraint acolumn_uk1 unique (ColumnIndex, RowId, PageId),
	constraint acolumn_ch1 check (ColumnTypeId = 2 and NewsProviderId is not null or ColumnTypeId <> 2),
	constraint acolumn_ch2 check (RowId is null and PageId is not null or RowId is not null and PageId is null),
	constraint acolumn_ch3 check (NewsProviderId is null and NewsProviderSourceId is null or NewsProviderId is not null and NewsProviderSourceId is not null)
)
go

alter table arow add constraint arow_fk1 foreign key (ColumnID) references acolumn(Id);
go


-- TODO: add trigger ensuring not saving news links
-- TODO: add trigger ensuring no more than 100 links per column

create table link
(
	Id bigint not null identity primary key,
	ColumnId bigint not null references acolumn(Id),
	LinkIndex bigint not null check(LinkIndex >= 0),
	Title nvarchar(100) not null,
	Description nvarchar(max) null,
	Hint nvarchar(max) null,
	Href nvarchar(max) not null,
	ButtonAccess bit not null default(0),
	ButtonTitle nvarchar(30) null,
	ButtonIndex bigint null check(ButtonIndex is null or ButtonIndex > 0),
	ShowImage bit not null default(1),
	ShowDescription bit not null default(1),
	NewsLink bit not null default(0) check (NewsLink = 0),
	ViewModeId int not null references viewMode(Id) 

	--constraint link_uk1 unique(ColumnId, LinkIndex)
)
go

create table exceptionSeverity
(
	Id bigint not null primary key check(Id >= 1 and Id <= 3),
	Name varchar(100) not null unique
)
go

create table log
(
	Id bigint not null identity primary key,
	ExceptionSeverityId bigint not null references exceptionSeverity(Id),
	LogDate DateTimeOffset not null default(sysdatetimeoffset()),
	Message nvarchar(Max) null
)
go

create table communicationMethod
(
	Id bigint not null identity primary key,
	Name varchar(100) not null unique
)
go

create table messageGroup
(
	Id bigint not null primary key check(Id between 1 and 3),
	Name varchar(100) not null unique
)
go

-- TODO: create a trigger preventing loops
create table message
(
	Id bigint not null identity primary key,
	MessageGroupId bigint not null references messageGroup(Id),
	ParentMessageId bigint null references message(Id), 
	SentDate DateTimeOffset not null default(sysdatetimeoffset()),
	SentFromIP varchar(255) null,
	PageId bigint null references page(Id),
	AuthorAccountId bigint null references account(Id),
	AuthorName nvarchar(100) null,
	AuthorEmail nvarchar(100) null,
	Subject nvarchar(255) null,
	MessageText nvarchar(500) null,

	index in1_message (SentFromIP)
)
go

create table messageRecipient
(
	Id bigint not null identity primary key,
	MessageId bigint not null references message(Id), 
	RecipientAccountId bigint null references account(Id),
	-- Email, skype, telephone etc.
	RecipientAddress nvarchar(255) null,
	ReceiveDate DateTimeOffset null,
	CommunicationMethodId bigint not null references communicationMethod(Id)
)
go
