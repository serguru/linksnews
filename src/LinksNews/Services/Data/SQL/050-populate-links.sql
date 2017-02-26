set nocount on;
set xact_abort on;

--use LinksNewsDB;
use DB_A1443B_linksnews
go 

begin transaction
go


declare @AccountId bigint;
declare @PageId bigint;
declare @ColumnId bigint;
declare @LinksColumnId bigint;
declare @RowId bigint;
declare @ChildRowId bigint;
declare @ChildColumnId bigint;
declare @ColumnTypeId bigint;
declare @NewsProviderId bigint;
declare @i int = 0;
declare @j int = 0;
declare @k int = 0;
declare @l int = 0;
declare @m int = 0;
declare @linksLimit int = 0;
declare @n int = 0;
declare @sourceId nvarchar(100);
declare @showTitle bit;
declare @contentCategoryName nvarchar(100);
declare @contentCategoryId bigint;
declare @pageCategoryId bigint;


set @NewsProviderId = (select Id from newsProviderDef where Name = 'NewsApi.org');

set @AccountId = (select Id from account where Login = 'Lol');

set @n = 1; 
while @n < 21 -- number of pages to create
begin

	set @contentCategoryName = 
		case 
			when @n < 3 then
				'Business'
			when @n < 6 then
				'Entertainment'
			when @n < 9 then
				'Gaming'
			when @n < 12 then
				'Music'
			when @n < 17 then
				'Science'
			else 
				'Technology'
		end;


	set @contentCategoryId = (select Id from contentCategory where Name = @contentCategoryName);

	insert into page (AccountId, Name, Title, PublicAccess, PageIndex, ShowTitle, ShowDescription, Description, ShowImage)
		values (@AccountId, 'TestPage' + cast(@n as varchar), 'Test Page '  + cast(@n as varchar), 1, @n, 1, 1,
		'Long page description 1 Long page description 2 Long page description 3 Long page description 4 Long page description 5',
		1);

	set @PageId = @@Identity;

	insert into pageCategory (PageId, ContentCategoryId) values (@PageId, @contentCategoryId);
	set @pageCategoryId = @@Identity;


	-- Rows type
	insert into acolumn (ColumnTypeId, RowId, PageId, ColumnIndex, ColumnWidth, Title, ShowTitle, ViewModeId)
		values (3, null, @PageId, 1, 7, 'My Links', 1, 1); 
	set @ColumnId = @@Identity;

	set @i = 0;

	while @i < 5
	begin
		insert into arow (ColumnId, RowIndex, Title, ShowTitle)
			values (@ColumnId, @i + 1, N'Строка ' + cast(@i + 1 as nvarchar), 0);
		set @RowId = @@Identity;

		set @j = 0;

		while @j < 3
		begin
		
			if @i = 1 and @j = 1 or @i = 3 and @j = 2		
			begin
				set @ColumnTypeId = 3; -- rows
				set @showTitle = 0;
			end else
			begin
				set @ColumnTypeId = 1; -- links
				set @showTitle = 1;
			end

			insert into acolumn (ColumnTypeId, RowId, PageId, ColumnIndex, ColumnWidth, Title, ShowTitle, ViewModeId) 
				values (@ColumnTypeId, @RowId, null, @j + 1, 4, 'Column ' + cast(@j as nvarchar), @showTitle, 1); 
			set @LinksColumnId = @@Identity;

			if @i = 1 and @j = 1 or @i = 3 and @j = 2		
			begin
				set @l = 0;

				while @l < 3
				begin
					insert into arow (ColumnId, RowIndex, Title, ShowTitle)
						values (@LinksColumnId, @l + 1, N'Строка ' + cast(@l + 1 as nvarchar), 0);
					set @ChildRowId = @@Identity;

					insert into acolumn (ColumnTypeId, RowId, PageId, ColumnIndex, ColumnWidth, Title, ShowTitle, ViewModeId) 
						values (1, @ChildRowId, null, 1, 12, 'Inner Column ' + cast(@l as nvarchar), 1, 1); 
					set @ChildColumnId = @@Identity;

					set @m = 0;
					while @m < 5
					begin
						insert into link (ColumnId, LinkIndex, Title, Href, ShowImage, ShowDescription, ViewModeId)
							values(@ChildColumnId, @m + 1, 'Link ' + cast(@m as varchar), 'http://linksnews.org/', 0, 0, 1);

						set @m = @m + 1;
					end

					set @l = @l + 1;
				end
			end
			else 
			begin
				set @k = 0;

				if @i = 1 and @j = 2 or @i = 2 and @j = 0		
					set @linksLimit = 10
				else
					set @linksLimit = 3;


				while @k < @linksLimit
				begin
					insert into link (ColumnId, LinkIndex, Title, Href, ShowImage, ShowDescription, Description, ViewModeId)
						values(@LinksColumnId, @k + 1, 'Link ' + cast(@k as varchar), 'http://linksnews.org/', 1, 1,
							'Long link description 1 Long link description 2 Long link description 3 Long link description 4', 1
						);
					set @k = @k + 1;
				end
			end


			set @j = @j + 1;
		end
		set @i = @i + 1;
	end;


	--
	-- News
	--
	-- Rows type
	insert into acolumn (ColumnTypeId, RowId, PageId, ColumnIndex, ColumnWidth, Title, ShowTitle, ViewModeId)
		values (3, null, @PageId, 2, 5, 'My News', 1, 1); 
	set @ColumnId = @@Identity;

	set @i = 0;

	while @i < 3
	begin

		set @sourceId = 
			case @i
				when 0 then 'associated-press'
				when 1 then 'bbc-news'
				when 2 then 'bloomberg'
				else ''
				end;

		insert into arow (ColumnId, RowIndex, Title)
			values (@ColumnId, @i + 1, N'Строка ' + cast(@i + 1 as nvarchar));
		set @RowId = @@Identity;

		insert into acolumn (ColumnTypeId, Title, RowId, PageId, ColumnIndex, ColumnWidth, NewsProviderId, NewsProviderSourceId, ShowTitle, ViewModeId)
			values (2, N'Колонка ' + cast(@i + 1 as nvarchar), @RowId, null, 1, 12, @NewsProviderId, @sourceId, 1, 1); 

		set @i = @i + 1
	end

	set @n = @n + 1
end
go

commit transaction
go
