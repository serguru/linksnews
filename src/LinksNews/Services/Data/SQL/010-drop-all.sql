--use LinksNewsDB;
use DB_A1443B_linksnews
go

if (object_id('spAddReservedAccount') is not null)
	drop procedure spAddReservedAccount;
go

if (object_id('vwNewsSource') is not null)
	drop view vwNewsSource;
go

if (object_id('spGetContentCategory') is not null)
	drop procedure spGetContentCategory;
go

if (object_id('spCategoryCount') is not null)
	drop procedure spCategoryCount;
go

if (object_id('spLogin') is not null)
	drop procedure spLogin;
go

if (object_id('spCreateAccount') is not null)
	drop procedure spCreateAccount;
go

if (object_id('spUpdatePassword') is not null)
	drop procedure spUpdatePassword;
go

-- tables

if (object_id('newsSource') is not null)
	drop table newsSource;
go

if (object_id('messageRecipient') is not null)
	drop table messageRecipient;
go

if (object_id('message') is not null)
	drop table message;
go

if (object_id('messageGroup') is not null)
	drop table messageGroup;
go

if (object_id('communicationMethod') is not null)
	drop table communicationMethod;
go

if (object_id('log') is not null)
	drop table log;
go

if (object_id('exceptionSeverity') is not null)
	drop table exceptionSeverity;
go

if (object_id('visit') is not null)
	drop table visit;
go

if (object_id('link') is not null)
	drop table link;
go

if (object_id('arow') is not null and object_id('arow_fk1') is not null)
	alter table arow 
		drop constraint arow_fk1;
go

if (object_id('acolumn') is not null)
	drop table acolumn;
go

if (object_id('arow') is not null)
	drop table arow;
go

if (object_id('columnType') is not null)
	drop table columnType;
go

if (object_id('pageCategory') is not null)
	drop table pageCategory;
go

if (object_id('accountPage') is not null)
	drop table accountPage;
go

if (object_id('page') is not null)
	drop table page;
go

if (object_id('account') is not null)
	drop table account;
go

if (object_id('arole') is not null)
	drop table arole;
go

if (object_id('translateVersion') is not null)
	drop table translateVersion;
go

if (object_id('translate') is not null)
	drop table translate;
go

if (object_id('englishMessage') is not null)
	drop table englishMessage;
go

if (object_id('language') is not null)
	drop table language;
go

if (object_id('website') is not null)
	drop table website;
go

if (object_id('theme') is not null)
	drop table theme;
go

if (object_id('contentCategoryMap') is not null)
	drop table contentCategoryMap;
go

if (object_id('sourcePriority') is not null)
	drop table sourcePriority;
go

if (object_id('contentCategory') is not null)
	drop table contentCategory;
go

if (object_id('newsProviderDef') is not null)
	drop table newsProviderDef;
go

if (object_id('newsProviderType') is not null)
	drop table newsProviderType;
go

if (object_id('country') is not null)
	drop table country;
go

if (object_id('viewMode') is not null)
	drop table viewMode;
go

